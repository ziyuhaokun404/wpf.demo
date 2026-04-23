using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Wpf.Ui.Appearance;
using WpfDemo.Extensions;
using WpfDemo.Models;

namespace WpfDemo;

public partial class App : Application
{
    private const string ShellThemeDictionaryPrefix = "Resources/Themes/Shell.";
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        services.AddShellServices();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null;

        base.OnExit(e);
    }

    public static void ApplyShellTheme(AppThemeOption option)
    {
        if (Current is not App application)
        {
            return;
        }

        string targetTheme = ResolveShellThemeName(option);
        Uri targetUri = new($"{ShellThemeDictionaryPrefix}{targetTheme}.xaml", UriKind.Relative);

        var dictionaries = application.Resources.MergedDictionaries;
        ResourceDictionary? shellDictionary = dictionaries.FirstOrDefault(dictionary =>
            dictionary.Source is not null &&
            dictionary.Source.OriginalString.StartsWith(ShellThemeDictionaryPrefix, StringComparison.OrdinalIgnoreCase));

        if (shellDictionary is not null)
        {
            if (shellDictionary.Source == targetUri)
            {
                return;
            }

            shellDictionary.Source = targetUri;
            return;
        }

        dictionaries.Add(new ResourceDictionary { Source = targetUri });
    }

    private static string ResolveShellThemeName(AppThemeOption option)
    {
        if (option == AppThemeOption.Dark)
        {
            return "Dark";
        }

        if (option == AppThemeOption.Light)
        {
            return "Light";
        }

        ApplicationTheme systemTheme = ApplicationThemeManager.GetAppTheme();
        return systemTheme == ApplicationTheme.Dark ? "Dark" : "Light";
    }
}
