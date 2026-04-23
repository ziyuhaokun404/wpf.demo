using System.Windows;
using System.Windows.Controls;
using WpfDemo.Models.Home;

namespace WpfDemo.Views.Controls;

public partial class QuickActionCardControl : UserControl
{
    public event EventHandler<string>? ActionRequested;

    public QuickActionCardControl()
    {
        InitializeComponent();
    }

    private void HandleClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is QuickActionDefinition definition)
        {
            ActionRequested?.Invoke(this, definition.TargetPageKey);
        }
    }
}
