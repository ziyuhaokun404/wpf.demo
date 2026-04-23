using System.Windows;
using System.Windows.Controls;
using WpfDemo.Models;
using WpfDemo.Models.Home;

namespace WpfDemo.Views.Controls;

public partial class ThemeOptionCardControl : UserControl
{
    public event EventHandler<AppThemeOption>? ThemeRequested;

    public ThemeOptionCardControl()
    {
        InitializeComponent();
    }

    private void HandleClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is ThemeOptionDefinition definition)
        {
            ThemeRequested?.Invoke(this, definition.Theme);
        }
    }
}
