using System.Windows;
using System.Windows.Controls;
using WpfDemo.Models;
using WpfDemo.ViewModels;
using WpfDemo.Views.Controls;

namespace WpfDemo.Views.Pages;

public partial class HomePage : Page
{
    public Action<string>? QuickActionRequested { get; set; }

    public Action<AppThemeOption>? ThemeRequested { get; set; }

    public HomePage(HomeViewModel viewModel)
    {
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        InitializeComponent();
    }

    private void QuickActionCard_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is QuickActionCardControl card)
        {
            card.ActionRequested -= HandleQuickActionRequested;
            card.ActionRequested += HandleQuickActionRequested;
        }
    }

    private void ThemeOptionCard_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is ThemeOptionCardControl card)
        {
            card.ThemeRequested -= HandleThemeRequested;
            card.ThemeRequested += HandleThemeRequested;
        }
    }

    private void HandleQuickActionRequested(object? sender, string pageKey)
    {
        QuickActionRequested?.Invoke(pageKey);
    }

    private void HandleThemeRequested(object? sender, AppThemeOption option)
    {
        ThemeRequested?.Invoke(option);
    }
}
