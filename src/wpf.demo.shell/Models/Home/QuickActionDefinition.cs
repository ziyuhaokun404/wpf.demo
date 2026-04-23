using Wpf.Ui.Controls;

namespace WpfDemo.Models.Home;

public sealed record QuickActionDefinition(SymbolRegular Icon, string Title, string Subtitle, string TargetPageKey);
