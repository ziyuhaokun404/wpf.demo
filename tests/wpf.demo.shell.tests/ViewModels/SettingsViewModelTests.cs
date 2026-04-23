using WpfDemo.ViewModels;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class SettingsViewModelTests
{
    [Fact]
    public void Constructor_ExposesExpectedSettingsContent()
    {
        var viewModel = new SettingsViewModel();

        Assert.Equal("系统设置", viewModel.Title);
        Assert.Equal("用于放置演示应用的主题、偏好与运行参数。", viewModel.Description);
    }
}
