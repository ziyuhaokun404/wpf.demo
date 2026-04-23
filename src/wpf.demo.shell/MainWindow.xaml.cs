using System.Windows;
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace WpfDemo;

public partial class MainWindow : FluentWindow
{
    private readonly MainWindowViewModel _viewModel;
    private readonly INavigationService _navigationService;
    private readonly Dictionary<string, NavigationViewItem> _navigationItemsByKey = new(StringComparer.Ordinal);
    private bool _navigationInitialized;

    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        DataContext = _viewModel;

        InitializeComponent();
        UpdateThemeToggleVisual(ApplicationThemeManager.GetAppTheme());
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
        SetActiveNavigationItem(pageKey);
        RootBreadcrumb.ItemsSource = _viewModel.GetBreadcrumbItems(pageKey);
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
        ApplicationTheme nextTheme = ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Dark
            ? ApplicationTheme.Light
            : ApplicationTheme.Dark;

        ApplicationThemeManager.Apply(nextTheme);
        UpdateThemeToggleVisual(nextTheme);
    }

    private void UpdateThemeToggleVisual(ApplicationTheme applicationTheme)
    {
        if (ThemeToggleButton is null || ThemeToggleIcon is null)
        {
            return;
        }

        bool isDarkTheme = applicationTheme == ApplicationTheme.Dark;
        ThemeToggleIcon.Symbol = isDarkTheme ? SymbolRegular.WeatherSunny24 : SymbolRegular.WeatherMoon24;
        ThemeToggleButton.ToolTip = isDarkTheme ? "切换到浅色模式" : "切换到深色模式";
    }
}
