using System.Windows.Controls;
using WpfDemo.Contracts.Services;

namespace WpfDemo.Services;

public sealed class NavigationService : INavigationService
{
    private readonly Dictionary<string, Func<Page>> _pages;
    private Frame? _frame;

    public NavigationService(Dictionary<string, Func<Page>> pages)
    {
        _pages = pages is null
            ? throw new ArgumentNullException(nameof(pages))
            : new Dictionary<string, Func<Page>>(pages);
    }

    public void Initialize(Frame frame)
    {
        _frame = frame ?? throw new ArgumentNullException(nameof(frame));
    }

    public void Navigate(string pageKey)
    {
        if (_frame is null)
        {
            throw new InvalidOperationException("Navigation frame has not been initialized.");
        }

        if (!_pages.TryGetValue(pageKey, out var pageFactory))
        {
            throw new InvalidOperationException($"Page '{pageKey}' is not registered.");
        }

        _frame.Navigate(pageFactory());
    }
}
