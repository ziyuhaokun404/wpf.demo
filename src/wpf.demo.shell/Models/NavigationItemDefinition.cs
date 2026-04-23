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
    NavigationItemPlacement Placement);

public static class NavigationPageKeys
{
    public const string Home = "Home";
    public const string Settings = "Settings";
}
