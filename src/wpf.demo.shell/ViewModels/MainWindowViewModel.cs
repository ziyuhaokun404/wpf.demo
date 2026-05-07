using Wpf.Ui.Controls;
using WpfDemo.Models;

namespace WpfDemo.ViewModels;

public class MainWindowViewModel
{
    private readonly Dictionary<string, NavigationItemDefinition> _navigationItemsByKey;

    public MainWindowViewModel()
    {
        NavigationItems =
        [
            new(NavigationPageKeys.Home, "主页", SymbolRegular.Home24, NavigationItemPlacement.Main, 0, "Core", "组件演示总览"),
            new(NavigationPageKeys.Buttons, "按钮与命令", SymbolRegular.CursorClick24, NavigationItemPlacement.Main, 10, "Core", "按钮、命令栏与快捷动作"),
            new(NavigationPageKeys.InputControls, "输入控件", SymbolRegular.Textbox24, NavigationItemPlacement.Main, 20, "Core", "表单输入与编辑控件"),
            new(NavigationPageKeys.DataDisplay, "数据展示", SymbolRegular.Table24, NavigationItemPlacement.Main, 30, "Core", "列表、表格与展示控件"),
            new(NavigationPageKeys.LayoutContainers, "布局容器", SymbolRegular.PanelLeft24, NavigationItemPlacement.Main, 40, "Core", "布局与容器模式"),
            new(NavigationPageKeys.Dialogs, "对话框", SymbolRegular.Alert24, NavigationItemPlacement.Main, 50, "Core", "对话、通知与浮层"),
            new(NavigationPageKeys.AnimationEffects, "动画与效果", SymbolRegular.PlayCircle24, NavigationItemPlacement.Main, 60, "Core", "过渡、动效与视觉反馈"),
            new(NavigationPageKeys.Themes, "主题与样式", SymbolRegular.PaintBrush24, NavigationItemPlacement.Main, 70, "Core", "主题和视觉令牌"),
            new(NavigationPageKeys.Icons, "图标与资源", SymbolRegular.Image24, NavigationItemPlacement.Main, 80, "Core", "图标和资源组织"),
            new(NavigationPageKeys.DemoSectionHeader, "业务场景", SymbolRegular.Line24, NavigationItemPlacement.Main, 90, "Secondary", null, true),
            new(NavigationPageKeys.FormExamples, "表单示例", SymbolRegular.DocumentText24, NavigationItemPlacement.Main, 100, "Secondary", "完整表单演示"),
            new(NavigationPageKeys.DataManagement, "数据管理", SymbolRegular.Database24, NavigationItemPlacement.Main, 110, "Secondary", "数据录入与管理场景"),
            new(NavigationPageKeys.ChartExamples, "图表示例", SymbolRegular.DataBarVertical24, NavigationItemPlacement.Main, 120, "Secondary", "图表与趋势展示"),
            new(NavigationPageKeys.FileBrowser, "文件浏览器", SymbolRegular.Folder24, NavigationItemPlacement.Main, 130, "Secondary", "文件和资源浏览"),
            new(NavigationPageKeys.Settings, "系统设置", SymbolRegular.Settings24, NavigationItemPlacement.Footer, 999, "Footer", "主题、壳层与个性化选项")
        ];

        _navigationItemsByKey = NavigationItems.ToDictionary(item => item.Key, item => item);

        MainNavigationItems = NavigationItems
            .Where(item => item.Placement == NavigationItemPlacement.Main)
            .OrderBy(item => item.Order)
            .ToArray();

        FooterNavigationItems = NavigationItems
            .Where(item => item.Placement == NavigationItemPlacement.Footer)
            .OrderBy(item => item.Order)
            .ToArray();

        ClickableNavigationItems = NavigationItems
            .Where(item => !item.IsSectionHeader)
            .OrderBy(item => item.Order)
            .ToArray();

        DefaultNavigationItem = ClickableNavigationItems[0];
    }

    public IReadOnlyList<NavigationItemDefinition> NavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> MainNavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> FooterNavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> ClickableNavigationItems { get; }

    public NavigationItemDefinition DefaultNavigationItem { get; }

    public IReadOnlyList<string> GetBreadcrumbItems(string pageKey)
    {
        if (!_navigationItemsByKey.TryGetValue(pageKey, out NavigationItemDefinition? item))
        {
            throw new InvalidOperationException($"Breadcrumb item '{pageKey}' is not registered.");
        }

        return [item.Title];
    }
}
