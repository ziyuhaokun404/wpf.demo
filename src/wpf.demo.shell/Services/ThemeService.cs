using WpfDemo.Contracts.Services;
using WpfDemo.Models;

namespace WpfDemo.Services;

public sealed class ThemeService : IThemeService
{
    public event EventHandler<AppThemeOption>? ThemeChanged;

    public AppThemeOption CurrentTheme { get; private set; } = AppThemeOption.Dark;

    public void SetTheme(AppThemeOption theme)
    {
        if (CurrentTheme == theme)
        {
            return;
        }

        CurrentTheme = theme;
        ThemeChanged?.Invoke(this, theme);
    }

    public void ToggleTheme()
    {
        AppThemeOption nextTheme = CurrentTheme switch
        {
            AppThemeOption.Dark => AppThemeOption.Light,
            _ => AppThemeOption.Dark
        };

        SetTheme(nextTheme);
    }
}
