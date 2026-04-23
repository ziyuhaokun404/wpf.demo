using System.Windows.Controls;
using WpfDemo.ViewModels;

namespace WpfDemo.Views.Pages;

public partial class PlaceholderDemoPage : Page
{
    public PlaceholderDemoPage(PlaceholderPageViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
