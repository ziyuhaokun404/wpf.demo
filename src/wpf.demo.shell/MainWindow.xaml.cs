using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.ViewModels;
using WpfDemo.Views.Pages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace WpfDemo;

public partial class MainWindow : FluentWindow
{
    private readonly MainWindowViewModel _viewModel;
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly Dictionary<string, NavigationViewItem> _navigationItemsByKey = new(StringComparer.Ordinal);
    private bool _navigationInitialized;

    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService, IThemeService themeService)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        DataContext = _viewModel;

        InitializeComponent();
        _themeService.ThemeChanged += HandleThemeChanged;
        UpdateThemeToggleVisual(_themeService.CurrentTheme);
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (_navigationInitialized)
        {
            return;
        }

        _navigationService.Initialize(PageHost);
        PopulateNavigationItems();

        _navigationInitialized = true;
        NavigateTo(_viewModel.DefaultNavigationItem.Key);
    }

    private void PopulateNavigationItems()
    {
        RootNavigation.MenuItems.Clear();
        RootNavigation.FooterMenuItems.Clear();
        _navigationItemsByKey.Clear();

        foreach (NavigationItemDefinition definition in _viewModel.MainNavigationItems)
        {
            if (definition.IsSectionHeader)
            {
                RootNavigation.MenuItems.Add(CreateSectionHeader(definition.Title));
                continue;
            }

            NavigationViewItem item = CreateNavigationItem(definition);
            RootNavigation.MenuItems.Add(item);
            _navigationItemsByKey.Add(definition.Key, item);
        }

        foreach (NavigationItemDefinition definition in _viewModel.FooterNavigationItems)
        {
            NavigationViewItem item = CreateNavigationItem(definition);
            RootNavigation.FooterMenuItems.Add(item);
            _navigationItemsByKey.Add(definition.Key, item);
        }
    }

    private NavigationViewItem CreateNavigationItem(NavigationItemDefinition definition)
    {
        NavigationViewItem item = new()
        {
            Content = definition.Title,
            Icon = new SymbolIcon(definition.IconSymbol),
            Tag = definition.Key,
            TargetPageTag = definition.Key
        };

        item.Click += NavItem_Click;
        return item;
    }

    private static System.Windows.Controls.TextBlock CreateSectionHeader(string title)
    {
        System.Windows.Controls.TextBlock textBlock = new()
        {
            Text = title,
            Margin = new Thickness(16, 14, 16, 6),
            FontSize = 12,
            FontWeight = FontWeights.SemiBold
        };

        textBlock.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty, "TextFillColorSecondaryBrush");

        return textBlock;
    }

    private void NavItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not NavigationViewItem item || item.Tag is not string pageKey)
        {
            return;
        }

        NavigateTo(pageKey);
    }

    private void NavigateTo(string pageKey)
    {
        _navigationService.Navigate(pageKey);
        PageHost.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
        SetActiveNavigationItem(pageKey);
        RootBreadcrumb.ItemsSource = _viewModel.GetBreadcrumbItems(pageKey);

        if (PageHost.Content is HomePage homePage)
        {
            homePage.QuickActionRequested = NavigateFromDashboard;
            homePage.ThemeRequested = SetThemeFromDashboard;
        }
    }

    private void SetActiveNavigationItem(string pageKey)
    {
        foreach ((string key, NavigationViewItem item) in _navigationItemsByKey)
        {
            item.IsActive = key == pageKey;
        }
    }

    private void ToggleThemeMode(object sender, RoutedEventArgs e)
    {
        _themeService.ToggleTheme();
    }

    private void HandleThemeChanged(object? sender, AppThemeOption option)
    {
        ApplyTheme(option);
    }

    internal void NavigateFromDashboard(string pageKey)
    {
        NavigateTo(pageKey);
    }

    internal void SetThemeFromDashboard(AppThemeOption option)
    {
        _themeService.SetTheme(option);
    }

    private void ApplyTheme(AppThemeOption option)
    {
        ApplicationTheme applicationTheme = option switch
        {
            AppThemeOption.Light => ApplicationTheme.Light,
            AppThemeOption.System => ApplicationTheme.Unknown,
            _ => ApplicationTheme.Dark
        };

        ApplicationThemeManager.Apply(applicationTheme);
        UpdateThemeToggleVisual(option);
    }

    private void UpdateThemeToggleVisual(AppThemeOption option)
    {
        if (ThemeToggleButton is null || ThemeToggleIcon is null)
        {
            return;
        }

        bool isDarkTheme = option == AppThemeOption.Dark;
        ThemeToggleIcon.Symbol = isDarkTheme ? SymbolRegular.WeatherSunny24 : SymbolRegular.WeatherMoon24;
        ThemeToggleButton.ToolTip = isDarkTheme ? "切换到浅色模式" : "切换到深色模式";
    }
}
