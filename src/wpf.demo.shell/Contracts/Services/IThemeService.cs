using WpfDemo.Models;

namespace WpfDemo.Contracts.Services;

public interface IThemeService
{
    event EventHandler<AppThemeOption>? ThemeChanged;

    AppThemeOption CurrentTheme { get; }

    void SetTheme(AppThemeOption theme);

    void ToggleTheme();
}
