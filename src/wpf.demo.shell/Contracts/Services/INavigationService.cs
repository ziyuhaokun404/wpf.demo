using System.Windows.Controls;

namespace WpfDemo.Contracts.Services;

public interface INavigationService
{
    void Initialize(Frame frame);

    void Navigate(string pageKey);
}
