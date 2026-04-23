using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using WpfDemo.Contracts.Services;
using WpfDemo.Services;
using Xunit;

namespace WpfDemo.Tests.Services;

public class NavigationServiceTests
{
    [Fact]
    public void Navigate_ThrowsWhenFrameHasNotBeenInitialized()
    {
        var service = CreateService();

        var exception = Assert.Throws<InvalidOperationException>(() => service.Navigate("Home"));

        Assert.Equal("Navigation frame has not been initialized.", exception.Message);
    }

    [Fact]
    public void Navigate_ThrowsWhenPageKeyIsUnknown()
    {
        RunOnStaThread(() =>
        {
            var service = CreateService();
            service.Initialize(new Frame());

            var exception = Assert.Throws<InvalidOperationException>(() => service.Navigate("Missing"));

            Assert.Equal("Page 'Missing' is not registered.", exception.Message);
        });
    }

    [Fact]
    public void Navigate_SetsFrameContentToRegisteredPage()
    {
        RunOnStaThread(() =>
        {
            var expectedPage = new TestPage();
            var service = CreateService(("Home", () => expectedPage));
            var frame = new Frame();

            service.Initialize(frame);
            service.Navigate("Home");
            frame.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

            Assert.Same(expectedPage, frame.Content);
        });
    }

    [Fact]
    public void Navigate_IgnoresMutationsToOriginalRegistrationDictionary()
    {
        RunOnStaThread(() =>
        {
            var expectedPage = new TestPage();
            var registrations = new Dictionary<string, Func<Page>>
            {
                ["Home"] = () => expectedPage
            };
            var service = new NavigationService(registrations);

            registrations["Home"] = () => new TestPage();

            var frame = new Frame();
            service.Initialize(frame);
            service.Navigate("Home");
            frame.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);

            Assert.Same(expectedPage, frame.Content);
        });
    }

    private static INavigationService CreateService(params (string Key, Func<Page> Factory)[] registrations)
    {
        var pages = registrations.ToDictionary(registration => registration.Key, registration => registration.Factory);

        return new NavigationService(pages);
    }

    private static void RunOnStaThread(Action action)
    {
        Exception? capturedException = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                capturedException = exception;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (capturedException is not null)
        {
            ExceptionDispatchInfo.Capture(capturedException).Throw();
        }
    }

    private sealed class TestPage : Page
    {
    }
}
