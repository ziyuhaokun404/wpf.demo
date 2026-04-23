using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfDemo.Contracts.Services;
using WpfDemo.Extensions;
using WpfDemo.Models;
using WpfDemo.Services;
using WpfDemo.ViewModels;
using WpfDemo.Views.Pages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Xunit;

namespace WpfDemo.Tests.Services;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddShellServices_RegistersNavigationServiceAndViewModels()
    {
        var services = new ServiceCollection();

        services.AddShellServices();

        using var serviceProvider = services.BuildServiceProvider();

        Assert.IsType<NavigationService>(serviceProvider.GetRequiredService<INavigationService>());
        Assert.IsType<ThemeService>(serviceProvider.GetRequiredService<IThemeService>());
        Assert.IsType<MainWindowViewModel>(serviceProvider.GetRequiredService<MainWindowViewModel>());
        Assert.IsType<HomeViewModel>(serviceProvider.GetRequiredService<HomeViewModel>());
        Assert.IsType<SettingsViewModel>(serviceProvider.GetRequiredService<SettingsViewModel>());
    }

    [Fact]
    public void AddShellServices_RegistersMainWindowAndPages()
    {
        RunOnStaThread(() =>
        {
            var services = new ServiceCollection();

            services.AddShellServices();

            using var serviceProvider = services.BuildServiceProvider();

            Assert.IsType<MainWindow>(serviceProvider.GetRequiredService<MainWindow>());
            Assert.IsType<HomePage>(serviceProvider.GetRequiredService<HomePage>());
            Assert.IsType<SettingsPage>(serviceProvider.GetRequiredService<SettingsPage>());
        });
    }

    [Fact]
    public void AddShellServices_RegistersPlaceholderPageFactoryForAllDemoDestinations()
    {
        RunOnStaThread(() =>
        {
            var services = new ServiceCollection();

            services.AddShellServices();

            using var serviceProvider = services.BuildServiceProvider();
            var navigationService = Assert.IsType<NavigationService>(serviceProvider.GetRequiredService<INavigationService>());
            var frame = new Frame();

            navigationService.Initialize(frame);

            foreach (string pageKey in new[]
                     {
                         NavigationPageKeys.Buttons,
                         NavigationPageKeys.InputControls,
                         NavigationPageKeys.DataDisplay,
                         NavigationPageKeys.LayoutContainers,
                         NavigationPageKeys.Dialogs,
                         NavigationPageKeys.AnimationEffects,
                         NavigationPageKeys.Themes,
                         NavigationPageKeys.Icons,
                         NavigationPageKeys.FormExamples,
                         NavigationPageKeys.DataManagement,
                         NavigationPageKeys.ChartExamples,
                         NavigationPageKeys.FileBrowser
                     })
            {
                navigationService.Navigate(pageKey);
                frame.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
                Assert.IsType<PlaceholderDemoPage>(frame.Content);
            }
        });
    }

    [Fact]
    public void MainWindow_OnContentRendered_NavigatesToHomeAndKeepsNavigationViewStateInSync()
    {
        RunOnStaThread(() =>
        {
            var navigationService = new RecordingNavigationService();
            var themeService = new ThemeService();
            var window = new MainWindow(new MainWindowViewModel(), navigationService, themeService);
            var navigationView = Assert.IsType<NavigationView>(window.FindName("RootNavigation"));
            var breadcrumb = Assert.IsType<BreadcrumbBar>(window.FindName("RootBreadcrumb"));

            InvokeOnContentRendered(window);

            var homeItem = Assert.IsType<NavigationViewItem>(navigationView.MenuItems[0]);
            var settingsItem = Assert.IsType<NavigationViewItem>(navigationView.FooterMenuItems[0]);

            Assert.Contains(
                navigationView.MenuItems.Cast<object>(),
                item => item is System.Windows.Controls.TextBlock textBlock && textBlock.Text == "演示分组");

            Assert.NotNull(navigationService.InitializedFrame);
            Assert.Same(window.FindName("PageHost"), navigationService.InitializedFrame);
            Assert.Equal(NavigationPageKeys.Home, navigationService.LastNavigatedKey);
            Assert.Single(navigationService.NavigationCalls);
            Assert.Equal(NavigationPageKeys.Home, navigationService.NavigationCalls[0]);
            Assert.Equal(NavigationPageKeys.Home, Assert.IsType<string>(homeItem.Tag));
            Assert.True(homeItem.IsActive);
            Assert.False(settingsItem.IsActive);
            Assert.Equal(["主页"], Assert.IsAssignableFrom<IEnumerable<string>>(breadcrumb.ItemsSource));

            InvokeNavItemClick(window, settingsItem);

            Assert.Equal(NavigationPageKeys.Settings, navigationService.LastNavigatedKey);
            Assert.Equal(2, navigationService.NavigationCalls.Count);
            Assert.Equal(NavigationPageKeys.Settings, navigationService.NavigationCalls[1]);
            Assert.True(settingsItem.IsActive);
            Assert.False(homeItem.IsActive);
            Assert.Equal(["系统设置"], Assert.IsAssignableFrom<IEnumerable<string>>(breadcrumb.ItemsSource));
        });
    }

    [Fact]
    public void MainWindow_ThemeToggleButton_UpdatesVisualStateWhenClicked()
    {
        RunOnStaThread(() =>
        {
            ApplicationThemeManager.Apply(ApplicationTheme.Dark);

            var themeService = new ThemeService();
            var window = new MainWindow(new MainWindowViewModel(), new RecordingNavigationService(), themeService);
            var titleBar = Assert.IsType<TitleBar>(window.FindName("MainTitleBar"));
            var button = Assert.IsType<Wpf.Ui.Controls.Button>(window.FindName("ThemeToggleButton"));
            var icon = Assert.IsType<SymbolIcon>(window.FindName("ThemeToggleIcon"));

            Assert.Equal(44d, titleBar.Height);
            Assert.Equal(40d, button.Width);
            Assert.Equal(40d, button.Height);
            Assert.Equal(new System.Windows.Thickness(0), button.Margin);
            Assert.Equal(new System.Windows.Thickness(0), button.Padding);
            Assert.False(button.Focusable);

            Assert.Equal("切换到浅色模式", Assert.IsType<string>(button.ToolTip));
            Assert.Equal(SymbolRegular.WeatherSunny24, icon.Symbol);

            InvokeThemeToggleClick(window, button);

            Assert.Equal(AppThemeOption.Light, themeService.CurrentTheme);
            Assert.Equal("切换到深色模式", Assert.IsType<string>(button.ToolTip));
            Assert.Equal(SymbolRegular.WeatherMoon24, icon.Symbol);
            Assert.Equal(ApplicationTheme.Light, ApplicationThemeManager.GetAppTheme());
        });
    }

    [Fact]
    public void MainWindow_HomeQuickAction_NavigatesAndActivatesMatchingNavItem()
    {
        RunOnStaThread(() =>
        {
            var navigationService = new RecordingNavigationService();
            var themeService = new ThemeService();
            var window = new MainWindow(new MainWindowViewModel(), navigationService, themeService);
            var navigationView = Assert.IsType<NavigationView>(window.FindName("RootNavigation"));
            var breadcrumb = Assert.IsType<BreadcrumbBar>(window.FindName("RootBreadcrumb"));

            InvokeOnContentRendered(window);
            InvokeNavigateFromDashboard(window, NavigationPageKeys.Dialogs);

            Assert.Equal(NavigationPageKeys.Dialogs, navigationService.LastNavigatedKey);
            Assert.Equal(["对话框"], Assert.IsAssignableFrom<IEnumerable<string>>(breadcrumb.ItemsSource));

            NavigationViewItem dialogsItem = navigationView.MenuItems
                .OfType<NavigationViewItem>()
                .Single(item => Equals(item.Tag, NavigationPageKeys.Dialogs));

            Assert.True(dialogsItem.IsActive);
        });
    }

    [Fact]
    public void MainWindow_SetThemeFromDashboard_UpdatesTitleBarToggleState()
    {
        RunOnStaThread(() =>
        {
            var themeService = new ThemeService();
            var window = new MainWindow(new MainWindowViewModel(), new RecordingNavigationService(), themeService);
            var button = Assert.IsType<Wpf.Ui.Controls.Button>(window.FindName("ThemeToggleButton"));

            InvokeSetThemeFromDashboard(window, AppThemeOption.System);

            Assert.Equal(AppThemeOption.System, themeService.CurrentTheme);
            Assert.Equal("切换到深色模式", Assert.IsType<string>(button.ToolTip));
        });
    }

    private static void InvokeOnContentRendered(MainWindow window)
    {
        var method = typeof(MainWindow).GetMethod("OnContentRendered", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        method.Invoke(window, [EventArgs.Empty]);
    }

    private static void InvokeNavItemClick(MainWindow window, NavigationViewItem item)
    {
        var method = typeof(MainWindow).GetMethod("NavItem_Click", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        method.Invoke(window, [item, new System.Windows.RoutedEventArgs()]);
    }

    private static void InvokeThemeToggleClick(MainWindow window, Wpf.Ui.Controls.Button button)
    {
        var method = typeof(MainWindow).GetMethod("ToggleThemeMode", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        method.Invoke(window, [button, new System.Windows.RoutedEventArgs()]);
    }

    private static void InvokeNavigateFromDashboard(MainWindow window, string pageKey)
    {
        var method = typeof(MainWindow).GetMethod("NavigateFromDashboard", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(method);

        method.Invoke(window, [pageKey]);
    }

    private static void InvokeSetThemeFromDashboard(MainWindow window, AppThemeOption theme)
    {
        var method = typeof(MainWindow).GetMethod("SetThemeFromDashboard", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.NotNull(method);

        method.Invoke(window, [theme]);
    }

    private static void RunOnStaThread(Action action)
    {
        Exception? capturedException = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                capturedException = exception;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (capturedException is not null)
        {
            ExceptionDispatchInfo.Capture(capturedException).Throw();
        }
    }

    private sealed class RecordingNavigationService : INavigationService
    {
        public List<string> NavigationCalls { get; } = [];

        public Frame? InitializedFrame { get; private set; }

        public string? LastNavigatedKey { get; private set; }

        public void Initialize(Frame frame)
        {
            InitializedFrame = frame;
        }

        public void Navigate(string pageKey)
        {
            LastNavigatedKey = pageKey;
            NavigationCalls.Add(pageKey);
        }
    }
}
