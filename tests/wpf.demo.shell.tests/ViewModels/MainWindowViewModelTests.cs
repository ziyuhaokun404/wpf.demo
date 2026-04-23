using WpfDemo.Models;
using WpfDemo.ViewModels;
using Wpf.Ui.Controls;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Constructor_ExposesMainAndFooterNavigationItems()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Collection(
            viewModel.NavigationItems,
            item =>
            {
                Assert.Equal(NavigationPageKeys.Home, item.Key);
                Assert.Equal("主页", item.Title);
                Assert.Equal(SymbolRegular.Home24, item.IconSymbol);
                Assert.Equal(NavigationItemPlacement.Main, item.Placement);
            },
            item =>
            {
                Assert.Equal(NavigationPageKeys.Settings, item.Key);
                Assert.Equal("系统设置", item.Title);
                Assert.Equal(SymbolRegular.Settings24, item.IconSymbol);
                Assert.Equal(NavigationItemPlacement.Footer, item.Placement);
            });
    }

    [Fact]
    public void Constructor_SeparatesMainAndFooterCollections()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Single(viewModel.MainNavigationItems);
        Assert.Single(viewModel.FooterNavigationItems);
        Assert.Equal(NavigationPageKeys.Home, viewModel.MainNavigationItems[0].Key);
        Assert.Equal(NavigationPageKeys.Settings, viewModel.FooterNavigationItems[0].Key);
        Assert.Equal(NavigationPageKeys.Home, viewModel.DefaultNavigationItem.Key);
    }

    [Fact]
    public void GetBreadcrumbItems_ReturnsChineseShellLabels()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal(new[] { "主页" }, viewModel.GetBreadcrumbItems(NavigationPageKeys.Home));
        Assert.Equal(new[] { "系统设置" }, viewModel.GetBreadcrumbItems(NavigationPageKeys.Settings));
    }

    [Fact]
    public void GetBreadcrumbItems_ThrowsForUnknownPageKey()
    {
        var viewModel = new MainWindowViewModel();

        var exception = Assert.Throws<InvalidOperationException>(() => viewModel.GetBreadcrumbItems("Missing"));

        Assert.Equal("Breadcrumb item 'Missing' is not registered.", exception.Message);
    }
}
