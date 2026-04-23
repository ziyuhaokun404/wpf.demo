using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.Models.Home;
using Wpf.Ui.Controls;

namespace WpfDemo.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(IThemeService themeService)
    {
        CurrentTheme = themeService.CurrentTheme;
        themeService.ThemeChanged += (_, option) => ApplyTheme(option);
        ApplyTheme(CurrentTheme);
    }

    public string Title { get; } = "主页";

    public string Description { get; } = "对标 vision.aoi.studio 的 Fluent 组件演示首页。";

    public string SearchPlaceholder { get; } = "搜索组件或示例...";

    public AppThemeOption CurrentTheme { get; private set; }

    public IReadOnlyList<FeatureCardDefinition> FeatureCards { get; } =
    [
        new(SymbolRegular.Apps24, "丰富的组件", "覆盖组件、布局与交互模式。"),
        new(SymbolRegular.DesignIdeas24, "Fluent 设计", "统一的层次、材质和节奏。"),
        new(SymbolRegular.Code24, "高可定制性", "适合作为演示应用和壳层基础。")
    ];

    public IReadOnlyList<QuickActionDefinition> QuickActions { get; } =
    [
        new(SymbolRegular.CursorClick24, "按钮与命令", "各类按钮、命令和交互组件", NavigationPageKeys.Buttons, "#2B74E4", "#182B74E4"),
        new(SymbolRegular.Textbox24, "输入控件", "丰富的输入框、选择器等组件", NavigationPageKeys.InputControls, "#2F8C7A", "#1833917D"),
        new(SymbolRegular.Grid24, "数据展示", "表格、列表、图表等数据组件", NavigationPageKeys.DataDisplay, "#8B4AE0", "#1B8B4AE0"),
        new(SymbolRegular.LayoutCellFour24, "布局容器", "各种布局面板和容器组件", NavigationPageKeys.LayoutContainers, "#C26A1A", "#1AC26A1A")
    ];

    public IReadOnlyList<ThemeOptionDefinition> ThemeOptions { get; } =
    [
        new(AppThemeOption.Light, SymbolRegular.WeatherSunny24, "浅色", "明亮、清晰的展示模式"),
        new(AppThemeOption.Dark, SymbolRegular.WeatherMoon24, "深色", "沉浸式的默认演示模式"),
        new(AppThemeOption.System, SymbolRegular.Desktop24, "跟随系统", "使用系统主题偏好")
    ];

    public IReadOnlyList<ReleaseNoteDefinition> ReleaseNotes { get; } =
    [
        new("v0.3", "引入 dashboard 首页和扩展导航结构", "2026-04-23", "#2B74E4"),
        new("v0.2", "新增动画效果和主题系统", "2026-04-10", "#3F8B7A"),
        new("v0.1", "项目初始化和基础组件", "2026-03-28", "#8B4AE0")
    ];

    public InfoBannerDefinition InfoBanner { get; } =
        new(SymbolRegular.Info24, "组件演示", "当前阶段优先完成壳层、首页和导航结构对齐。");

    private void ApplyTheme(AppThemeOption option)
    {
        CurrentTheme = option;

        foreach (ThemeOptionDefinition themeOption in ThemeOptions)
        {
            themeOption.IsSelected = themeOption.Theme == option;
        }
    }
}
