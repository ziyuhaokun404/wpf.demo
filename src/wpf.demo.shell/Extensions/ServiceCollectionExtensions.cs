using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.Services;
using WpfDemo.ViewModels;
using WpfDemo.Views.Pages;

namespace WpfDemo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShellServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<HomePage>();
        services.AddTransient<SettingsPage>();

        services.AddSingleton<INavigationService>(provider =>
            new NavigationService(new Dictionary<string, Func<Page>>
            {
                [NavigationPageKeys.Home] = () => provider.GetRequiredService<HomePage>(),
                [NavigationPageKeys.Settings] = () => provider.GetRequiredService<SettingsPage>()
            }));

        services.AddSingleton<MainWindow>();

        return services;
    }
}
