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

        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<HomePage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<PlaceholderDemoPage>();

        services.AddSingleton<INavigationService>(provider =>
        {
            Dictionary<string, Func<Page>> pages = new()
            {
                [NavigationPageKeys.Home] = () => provider.GetRequiredService<HomePage>(),
                [NavigationPageKeys.Settings] = () => provider.GetRequiredService<SettingsPage>()
            };

            pages[NavigationPageKeys.Buttons] = () => CreatePlaceholderPage("按钮与命令", "展示按钮、命令栏和动作入口。");
            pages[NavigationPageKeys.InputControls] = () => CreatePlaceholderPage("输入控件", "展示输入、编辑和表单控件。");
            pages[NavigationPageKeys.DataDisplay] = () => CreatePlaceholderPage("数据展示", "展示列表、表格和数据可视化入口。");
            pages[NavigationPageKeys.LayoutContainers] = () => CreatePlaceholderPage("布局容器", "展示布局、容器和页面结构模式。");
            pages[NavigationPageKeys.Dialogs] = () => CreatePlaceholderPage("对话框", "展示对话、通知和轻量浮层。");
            pages[NavigationPageKeys.AnimationEffects] = () => CreatePlaceholderPage("动画与效果", "展示动效与视觉反馈。");
            pages[NavigationPageKeys.Themes] = () => CreatePlaceholderPage("主题与样式", "展示主题、配色和视觉样式。");
            pages[NavigationPageKeys.Icons] = () => CreatePlaceholderPage("图标与资源", "展示图标、插图和资源组织。");
            pages[NavigationPageKeys.FormExamples] = () => CreatePlaceholderPage("表单示例", "展示业务型表单组合示例。");
            pages[NavigationPageKeys.DataManagement] = () => CreatePlaceholderPage("数据管理", "展示数据维护和管理界面示例。");
            pages[NavigationPageKeys.ChartExamples] = () => CreatePlaceholderPage("图表示例", "展示图表和趋势视图。");
            pages[NavigationPageKeys.FileBrowser] = () => CreatePlaceholderPage("文件浏览器", "展示文件和资源浏览示例。");

            return new NavigationService(pages);

            PlaceholderDemoPage CreatePlaceholderPage(string title, string description)
            {
                return new PlaceholderDemoPage(new PlaceholderPageViewModel
                {
                    Title = title,
                    Description = description
                });
            }
        });

        services.AddSingleton<MainWindow>();

        return services;
    }
}
