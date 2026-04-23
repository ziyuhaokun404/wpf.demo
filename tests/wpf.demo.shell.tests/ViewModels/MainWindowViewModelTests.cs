using WpfDemo.Models;
using WpfDemo.ViewModels;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Constructor_BuildsOrderedNavigationCatalog()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal(
            [
                NavigationPageKeys.Home,
                NavigationPageKeys.Buttons,
                NavigationPageKeys.InputControls,
                NavigationPageKeys.DataDisplay,
                NavigationPageKeys.LayoutContainers,
                NavigationPageKeys.Dialogs,
                NavigationPageKeys.AnimationEffects,
                NavigationPageKeys.Themes,
                NavigationPageKeys.Icons,
                NavigationPageKeys.DemoSectionHeader,
                NavigationPageKeys.FormExamples,
                NavigationPageKeys.DataManagement,
                NavigationPageKeys.ChartExamples,
                NavigationPageKeys.FileBrowser,
                NavigationPageKeys.Settings
            ],
            viewModel.NavigationItems.Select(item => item.Key).ToArray());
    }

    [Fact]
    public void Constructor_SeparatesClickableItemsFromSectionHeaders()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal(14, viewModel.ClickableNavigationItems.Count);
        Assert.Contains(viewModel.MainNavigationItems, item => item.Key == NavigationPageKeys.DemoSectionHeader && item.IsSectionHeader);
        Assert.DoesNotContain(viewModel.ClickableNavigationItems, item => item.Key == NavigationPageKeys.DemoSectionHeader);
        Assert.Equal(NavigationPageKeys.Settings, viewModel.FooterNavigationItems[0].Key);
        Assert.Equal(NavigationPageKeys.Home, viewModel.DefaultNavigationItem.Key);
    }

    [Theory]
    [InlineData(NavigationPageKeys.Home, "主页")]
    [InlineData(NavigationPageKeys.Buttons, "按钮与命令")]
    [InlineData(NavigationPageKeys.FormExamples, "表单示例")]
    [InlineData(NavigationPageKeys.Settings, "系统设置")]
    public void GetBreadcrumbItems_ReturnsChineseShellLabels(string pageKey, string expectedTitle)
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal([expectedTitle], viewModel.GetBreadcrumbItems(pageKey));
    }

    [Fact]
    public void GetBreadcrumbItems_ThrowsForUnknownPageKey()
    {
        var viewModel = new MainWindowViewModel();

        var exception = Assert.Throws<InvalidOperationException>(() => viewModel.GetBreadcrumbItems("Missing"));

        Assert.Equal("Breadcrumb item 'Missing' is not registered.", exception.Message);
    }
}
