using System.ComponentModel;
using Wpf.Ui.Controls;

namespace WpfDemo.Models.Home;

public sealed class ThemeOptionDefinition : INotifyPropertyChanged
{
    private bool _isSelected;

    public ThemeOptionDefinition(AppThemeOption theme, SymbolRegular icon, string title, string description)
    {
        Theme = theme;
        Icon = icon;
        Title = title;
        Description = description;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AppThemeOption Theme { get; }

    public SymbolRegular Icon { get; }

    public string Title { get; }

    public string Description { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }
}
