using WpfDemo.Models;
using Wpf.Ui.Controls;

namespace WpfDemo.ViewModels;

public class MainWindowViewModel
{
    private readonly Dictionary<string, NavigationItemDefinition> _navigationItemsByKey;

    public MainWindowViewModel()
    {
        NavigationItems =
        [
            new(NavigationPageKeys.Home, "主页", SymbolRegular.Home24, NavigationItemPlacement.Main),
            new(NavigationPageKeys.Settings, "系统设置", SymbolRegular.Settings24, NavigationItemPlacement.Footer)
        ];

        MainNavigationItems = NavigationItems
            .Where(item => item.Placement == NavigationItemPlacement.Main)
            .ToArray();

        FooterNavigationItems = NavigationItems
            .Where(item => item.Placement == NavigationItemPlacement.Footer)
            .ToArray();

        DefaultNavigationItem = MainNavigationItems[0];
        _navigationItemsByKey = NavigationItems.ToDictionary(item => item.Key, item => item);
    }

    public IReadOnlyList<NavigationItemDefinition> NavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> MainNavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> FooterNavigationItems { get; }

    public NavigationItemDefinition DefaultNavigationItem { get; }

    public IReadOnlyList<string> GetBreadcrumbItems(string pageKey)
    {
        if (!_navigationItemsByKey.TryGetValue(pageKey, out NavigationItemDefinition? item))
        {
            throw new InvalidOperationException($"Breadcrumb item '{pageKey}' is not registered.");
        }

        return [item.Title];
    }
}
