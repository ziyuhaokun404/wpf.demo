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
        new(SymbolRegular.CursorClick24, "按钮", "常用操作入口", NavigationPageKeys.Buttons),
        new(SymbolRegular.Textbox24, "文本框", "输入与编辑", NavigationPageKeys.InputControls),
        new(SymbolRegular.Table24, "数据表格", "展示与浏览", NavigationPageKeys.DataDisplay),
        new(SymbolRegular.Alert24, "对话框", "交互反馈", NavigationPageKeys.Dialogs)
    ];

    public IReadOnlyList<ThemeOptionDefinition> ThemeOptions { get; } =
    [
        new(AppThemeOption.Light, SymbolRegular.WeatherSunny24, "浅色", "明亮、清晰的展示模式"),
        new(AppThemeOption.Dark, SymbolRegular.WeatherMoon24, "深色", "沉浸式的默认演示模式"),
        new(AppThemeOption.System, SymbolRegular.Desktop24, "跟随系统", "使用系统主题偏好")
    ];

    public IReadOnlyList<ReleaseNoteDefinition> ReleaseNotes { get; } =
    [
        new("v0.3", "引入 dashboard 首页和扩展导航结构。", "2026-04-23", "SystemAccentColorPrimaryBrush"),
        new("v0.2", "统一 TitleBar 与 NavigationView 外壳。", "2026-04-22", "SystemFillColorCautionBrush"),
        new("v0.1", "初始化 Fluent 壳层与基础导航。", "2026-04-21", "SystemFillColorSuccessBrush")
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
