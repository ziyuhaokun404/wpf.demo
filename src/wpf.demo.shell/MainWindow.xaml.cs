using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
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
    private readonly Dictionary<string, object> _navigationExpandedContentByKey = new(StringComparer.Ordinal);
    private readonly List<FrameworkElement> _sectionHeaders = [];
    private readonly Dictionary<UIElement, bool> _elementVisibilityStates = [];
    private bool _navigationInitialized;

    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService, IThemeService themeService)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        DataContext = _viewModel;

        InitializeComponent();
        _themeService.ThemeChanged += HandleThemeChanged;
        RootNavigation.PaneOpened += (_, _) => UpdateNavigationPaneLayout();
        RootNavigation.PaneClosed += (_, _) => UpdateNavigationPaneLayout();
        UpdateThemeToggleVisual(_themeService.CurrentTheme);
        Loaded += (_, _) => UpdateNavigationPaneLayout();
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
        _navigationExpandedContentByKey.Clear();
        _sectionHeaders.Clear();

        foreach (NavigationItemDefinition definition in _viewModel.MainNavigationItems)
        {
            if (definition.IsSectionHeader)
            {
                FrameworkElement header = CreateSectionHeader(definition.Title);
                RootNavigation.MenuItems.Add(header);
                _sectionHeaders.Add(header);
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
        object content = CreateNavigationItemContent(definition);

        NavigationViewItem item = new()
        {
            Content = content,
            Icon = new SymbolIcon(definition.IconSymbol),
            Tag = definition.Key,
            TargetPageTag = definition.Key,
            ToolTip = string.IsNullOrWhiteSpace(definition.Description)
                ? definition.Title
                : $"{definition.Title} - {definition.Description}"
        };

        _navigationExpandedContentByKey[definition.Key] = content;
        item.Click += NavItem_Click;
        return item;
    }

    private static StackPanel CreateNavigationItemContent(NavigationItemDefinition definition)
    {
        System.Windows.Controls.TextBlock title = new()
        {
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Text = definition.Title,
            TextTrimming = TextTrimming.CharacterEllipsis
        };

        StackPanel content = new()
        {
            Orientation = Orientation.Vertical
        };

        content.Children.Add(title);

        if (!string.IsNullOrWhiteSpace(definition.Description))
        {
            System.Windows.Controls.TextBlock description = new()
            {
                Margin = new Thickness(0, 3, 0, 0),
                FontSize = 11,
                Text = definition.Description,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            description.SetResourceReference(System.Windows.Controls.TextBlock.ForegroundProperty, "TextFillColorSecondaryBrush");
            content.Children.Add(description);
        }

        return content;
    }

    private static System.Windows.Controls.TextBlock CreateSectionHeader(string title)
    {
        System.Windows.Controls.TextBlock textBlock = new()
        {
            Text = title.ToUpperInvariant(),
            Margin = new Thickness(18, 18, 18, 8),
            FontSize = 11,
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
        IReadOnlyList<string> breadcrumbItems = _viewModel.GetBreadcrumbItems(pageKey);
        RootBreadcrumb.ItemsSource = breadcrumbItems;
        CurrentPageTitleText.Text = breadcrumbItems.LastOrDefault() ?? string.Empty;

        if (PageHost.Content is HomePage homePage)
        {
            homePage.QuickActionRequested = NavigateFromDashboard;
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
        App.ApplyShellTheme(option);
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

    private void UpdateNavigationPaneLayout()
    {
        if (ExpandedPaneHeaderCard is null || CompactPaneHeaderBadge is null || ExpandedPaneFooter is null || CompactPaneFooter is null)
        {
            return;
        }

        bool isPaneOpen = RootNavigation.IsPaneOpen;
        AnimatePaneWidth(isPaneOpen ? 292 : 64);
        AnimateVisibility(ExpandedPaneHeaderCard, isPaneOpen);
        AnimateVisibility(CompactPaneHeaderBadge, !isPaneOpen);
        AnimateVisibility(ExpandedPaneFooter, isPaneOpen);
        AnimateVisibility(CompactPaneFooter, !isPaneOpen);

        foreach ((string key, NavigationViewItem item) in _navigationItemsByKey)
        {
            item.Content = isPaneOpen ? _navigationExpandedContentByKey[key] : null;
        }

        foreach (FrameworkElement sectionHeader in _sectionHeaders)
        {
            sectionHeader.Visibility = isPaneOpen ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void AnimatePaneWidth(double targetWidth)
    {
        if (Math.Abs(NavigationPaneContainer.Width - targetWidth) < 0.5)
        {
            NavigationPaneContainer.Width = targetWidth;
            return;
        }

        DoubleAnimation animation = new()
        {
            To = targetWidth,
            Duration = TimeSpan.FromMilliseconds(180),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        NavigationPaneContainer.BeginAnimation(WidthProperty, animation, HandoffBehavior.SnapshotAndReplace);
    }

    private void AnimateVisibility(UIElement element, bool shouldShow)
    {
        _elementVisibilityStates[element] = shouldShow;
        element.BeginAnimation(OpacityProperty, null);

        if (shouldShow)
        {
            element.Visibility = Visibility.Visible;
            DoubleAnimation fadeIn = new()
            {
                From = element.Opacity,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(140),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            element.BeginAnimation(OpacityProperty, fadeIn, HandoffBehavior.SnapshotAndReplace);
            return;
        }

        DoubleAnimation fadeOut = new()
        {
            From = element.Opacity,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(120),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        fadeOut.Completed += (_, _) =>
        {
            if (_elementVisibilityStates.TryGetValue(element, out bool isVisible) && !isVisible)
            {
                element.Visibility = Visibility.Collapsed;
            }
        };
        element.BeginAnimation(OpacityProperty, fadeOut, HandoffBehavior.SnapshotAndReplace);
    }
}
