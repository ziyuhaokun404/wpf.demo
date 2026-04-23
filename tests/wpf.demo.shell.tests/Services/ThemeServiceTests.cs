using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.Services;
using Xunit;

namespace WpfDemo.Tests.Services;

public class ThemeServiceTests
{
    [Fact]
    public void SetTheme_UpdatesCurrentThemeAndRaisesEvent()
    {
        IThemeService service = new ThemeService();
        AppThemeOption? changedTo = null;

        service.ThemeChanged += (_, option) => changedTo = option;

        service.SetTheme(AppThemeOption.Light);

        Assert.Equal(AppThemeOption.Light, service.CurrentTheme);
        Assert.Equal(AppThemeOption.Light, changedTo);
    }

    [Fact]
    public void ToggleTheme_CyclesBetweenDarkAndLight()
    {
        IThemeService service = new ThemeService();

        service.SetTheme(AppThemeOption.Dark);
        service.ToggleTheme();
        Assert.Equal(AppThemeOption.Light, service.CurrentTheme);

        service.ToggleTheme();
        Assert.Equal(AppThemeOption.Dark, service.CurrentTheme);
    }
}
