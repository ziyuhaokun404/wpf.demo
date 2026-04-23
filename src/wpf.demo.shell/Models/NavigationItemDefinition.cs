using Wpf.Ui.Controls;

namespace WpfDemo.Models;

public enum NavigationItemPlacement
{
    Main,
    Footer
}

public sealed record NavigationItemDefinition(
    string Key,
    string Title,
    SymbolRegular IconSymbol,
    NavigationItemPlacement Placement,
    int Order,
    string? Section = null,
    string? Description = null,
    bool IsSectionHeader = false);

public static class NavigationPageKeys
{
    public const string Home = "Home";
    public const string Buttons = "Buttons";
    public const string InputControls = "InputControls";
    public const string DataDisplay = "DataDisplay";
    public const string LayoutContainers = "LayoutContainers";
    public const string Dialogs = "Dialogs";
    public const string AnimationEffects = "AnimationEffects";
    public const string Themes = "Themes";
    public const string Icons = "Icons";
    public const string DemoSectionHeader = "DemoSectionHeader";
    public const string FormExamples = "FormExamples";
    public const string DataManagement = "DataManagement";
    public const string ChartExamples = "ChartExamples";
    public const string FileBrowser = "FileBrowser";
    public const string Settings = "Settings";
}
