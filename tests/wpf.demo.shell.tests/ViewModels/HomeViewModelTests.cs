using WpfDemo.ViewModels;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class HomeViewModelTests
{
    [Fact]
    public void Constructor_ExposesExpectedHomeContent()
    {
        var viewModel = new HomeViewModel();

        Assert.Equal("主页", viewModel.Title);
        Assert.Equal("展示对标 vision.aoi.studio 的 NavigationView 演示壳层。", viewModel.Description);
    }
}
