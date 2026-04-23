using WpfDemo.Models;
using WpfDemo.Services;
using WpfDemo.ViewModels;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class HomeViewModelTests
{
    [Fact]
    public void Constructor_ExposesDashboardSections()
    {
        var viewModel = new HomeViewModel(new ThemeService());

        Assert.Equal("主页", viewModel.Title);
        Assert.Equal("搜索组件或示例...", viewModel.SearchPlaceholder);
        Assert.Equal(3, viewModel.FeatureCards.Count);
        Assert.Equal(4, viewModel.QuickActions.Count);
        Assert.Equal(3, viewModel.ThemeOptions.Count);
        Assert.Equal(3, viewModel.ReleaseNotes.Count);
        Assert.NotNull(viewModel.InfoBanner);
    }

    [Fact]
    public void QuickActions_TargetRealNavigationKeys()
    {
        var viewModel = new HomeViewModel(new ThemeService());

        Assert.Equal(
            [
                NavigationPageKeys.Buttons,
                NavigationPageKeys.InputControls,
                NavigationPageKeys.DataDisplay,
                NavigationPageKeys.LayoutContainers
            ],
            viewModel.QuickActions.Select(item => item.TargetPageKey).ToArray());
    }

    [Fact]
    public void Constructor_SelectsDarkThemeByDefaultAndTracksThemeChanges()
    {
        var themeService = new ThemeService();
        var viewModel = new HomeViewModel(themeService);

        Assert.Equal(AppThemeOption.Dark, viewModel.CurrentTheme);
        Assert.True(viewModel.ThemeOptions.Single(option => option.Theme == AppThemeOption.Dark).IsSelected);

        themeService.SetTheme(AppThemeOption.Light);

        Assert.Equal(AppThemeOption.Light, viewModel.CurrentTheme);
        Assert.True(viewModel.ThemeOptions.Single(option => option.Theme == AppThemeOption.Light).IsSelected);
        Assert.False(viewModel.ThemeOptions.Single(option => option.Theme == AppThemeOption.Dark).IsSelected);
    }
}
