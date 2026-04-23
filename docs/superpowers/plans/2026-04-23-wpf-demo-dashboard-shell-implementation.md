# WPF Demo Dashboard Shell Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade `wpf.demo` from a minimal shell into a `vision.aoi.studio`-style component demo shell with richer `NavigationView`, dashboard home page, synchronized theme controls, and coherent placeholder pages.

**Architecture:** Keep the current `App -> MainWindow -> INavigationService -> Page` shell intact, but expand the presentation model around it. Add richer navigation metadata, a shared theme state service, dashboard-focused home view models, and lightweight reusable controls so the new UI remains data-driven instead of hard-coded into one page.

**Tech Stack:** .NET 10 WPF, WPF-UI (`FluentWindow`, `NavigationView`, `Card`, `TitleBar`), Microsoft DI, xUnit

---

## File Structure

### Existing files to modify

- `src/wpf.demo.shell/Models/NavigationItemDefinition.cs`
  Own richer navigation metadata, page keys, and section metadata.
- `src/wpf.demo.shell/ViewModels/MainWindowViewModel.cs`
  Build ordered navigation catalog, breadcrumb labels, and quick lookup helpers.
- `src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs`
  Register new pages, home dashboard models, and the shared theme state service.
- `src/wpf.demo.shell/MainWindow.xaml`
  Align the `NavigationView` pane with grouped demo navigation and keep title bar theme button consistent.
- `src/wpf.demo.shell/MainWindow.xaml.cs`
  Build grouped navigation items, route quick actions, and sync theme state with the title-bar button.
- `src/wpf.demo.shell/ViewModels/HomeViewModel.cs`
  Replace the simple title/description model with a dashboard root model.
- `src/wpf.demo.shell/Views/Pages/HomePage.xaml`
  Rebuild the landing page into a dashboard layout.
- `src/wpf.demo.shell/Views/Pages/HomePage.xaml.cs`
  Wire the page to the richer home view model and shell callbacks.
- `tests/wpf.demo.shell.tests/ViewModels/MainWindowViewModelTests.cs`
  Cover richer navigation ordering, grouping, and breadcrumbs.
- `tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs`
  Cover new registrations, shell sync behavior, and theme synchronization.
- `tests/wpf.demo.shell.tests/ViewModels/HomeViewModelTests.cs`
  Replace simple title assertions with dashboard data assertions.

### New files to create

- `src/wpf.demo.shell/Contracts/Services/IThemeService.cs`
  Shared theme-state contract.
- `src/wpf.demo.shell/Services/ThemeService.cs`
  Single source of truth for `Light`, `Dark`, and `System` theme modes.
- `src/wpf.demo.shell/Models/AppThemeOption.cs`
  Internal enum for app theme choices.
- `src/wpf.demo.shell/Models/Home/FeatureCardDefinition.cs`
- `src/wpf.demo.shell/Models/Home/QuickActionDefinition.cs`
- `src/wpf.demo.shell/Models/Home/ThemeOptionDefinition.cs`
- `src/wpf.demo.shell/Models/Home/ReleaseNoteDefinition.cs`
- `src/wpf.demo.shell/Models/Home/InfoBannerDefinition.cs`
  Focused dashboard presentation models.
- `src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml`
- `src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml.cs`
- `src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml`
- `src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml.cs`
- `src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml`
- `src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml.cs`
- `src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml`
- `src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml.cs`
  Reusable dashboard card controls.
- `src/wpf.demo.shell/ViewModels/PlaceholderPageViewModel.cs`
  Reusable page model for shallow demo destinations.
- `src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml`
- `src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml.cs`
  Common placeholder page template.
- `tests/wpf.demo.shell.tests/Services/ThemeServiceTests.cs`
  Theme service coverage.

## Task 1: Expand Navigation Metadata And Shell View Model

**Files:**
- Modify: `src/wpf.demo.shell/Models/NavigationItemDefinition.cs`
- Modify: `src/wpf.demo.shell/ViewModels/MainWindowViewModel.cs`
- Test: `tests/wpf.demo.shell.tests/ViewModels/MainWindowViewModelTests.cs`

- [ ] **Step 1: Write the failing navigation metadata tests**

```csharp
using WpfDemo.Models;
using WpfDemo.Services;
using WpfDemo.ViewModels;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Constructor_BuildsOrderedNavigationCatalog()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal(
            [
                NavigationPageKeys.Home,
                NavigationPageKeys.Buttons,
                NavigationPageKeys.InputControls,
                NavigationPageKeys.DataDisplay,
                NavigationPageKeys.LayoutContainers,
                NavigationPageKeys.Dialogs,
                NavigationPageKeys.AnimationEffects,
                NavigationPageKeys.Themes,
                NavigationPageKeys.Icons,
                NavigationPageKeys.DemoSectionHeader,
                NavigationPageKeys.FormExamples,
                NavigationPageKeys.DataManagement,
                NavigationPageKeys.ChartExamples,
                NavigationPageKeys.FileBrowser,
                NavigationPageKeys.Settings
            ],
            viewModel.NavigationItems.Select(item => item.Key).ToArray());
    }

    [Fact]
    public void Constructor_SeparatesClickableItemsFromSectionHeaders()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal(13, viewModel.ClickableNavigationItems.Count);
        Assert.Contains(viewModel.MainNavigationItems, item => item.Key == NavigationPageKeys.DemoSectionHeader && item.IsSectionHeader);
        Assert.DoesNotContain(viewModel.ClickableNavigationItems, item => item.Key == NavigationPageKeys.DemoSectionHeader);
        Assert.Equal(NavigationPageKeys.Home, viewModel.DefaultNavigationItem.Key);
    }

    [Theory]
    [InlineData(NavigationPageKeys.Home, "主页")]
    [InlineData(NavigationPageKeys.Buttons, "按钮与命令")]
    [InlineData(NavigationPageKeys.FormExamples, "表单示例")]
    [InlineData(NavigationPageKeys.Settings, "系统设置")]
    public void GetBreadcrumbItems_ReturnsLabelForRegisteredPages(string pageKey, string expectedTitle)
    {
        var viewModel = new MainWindowViewModel();

        Assert.Equal([expectedTitle], viewModel.GetBreadcrumbItems(pageKey));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter MainWindowViewModelTests -v minimal`

Expected: FAIL because the new page keys and navigation metadata properties do not exist yet.

- [ ] **Step 3: Write the minimal implementation**

`src/wpf.demo.shell/Models/NavigationItemDefinition.cs`

```csharp
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
    NavigationItemPlacement Placement,
    int Order,
    string? Section = null,
    string? Description = null,
    bool IsSectionHeader = false);

public static class NavigationPageKeys
{
    public const string Home = "Home";
    public const string Buttons = "Buttons";
    public const string InputControls = "InputControls";
    public const string DataDisplay = "DataDisplay";
    public const string LayoutContainers = "LayoutContainers";
    public const string Dialogs = "Dialogs";
    public const string AnimationEffects = "AnimationEffects";
    public const string Themes = "Themes";
    public const string Icons = "Icons";
    public const string DemoSectionHeader = "DemoSectionHeader";
    public const string FormExamples = "FormExamples";
    public const string DataManagement = "DataManagement";
    public const string ChartExamples = "ChartExamples";
    public const string FileBrowser = "FileBrowser";
    public const string Settings = "Settings";
}
```

`src/wpf.demo.shell/ViewModels/MainWindowViewModel.cs`

```csharp
using WpfDemo.Models;
using Wpf.Ui.Controls;

namespace WpfDemo.ViewModels;

public class MainWindowViewModel
{
    private readonly Dictionary<string, NavigationItemDefinition> _navigationItemsByKey;

    public MainWindowViewModel()
    {
        NavigationItems =
        [
            new(NavigationPageKeys.Home, "主页", SymbolRegular.Home24, NavigationItemPlacement.Main, 0, "Core", "组件演示总览"),
            new(NavigationPageKeys.Buttons, "按钮与命令", SymbolRegular.CursorClick24, NavigationItemPlacement.Main, 10, "Core", "按钮、命令栏与快捷动作"),
            new(NavigationPageKeys.InputControls, "输入控件", SymbolRegular.Textbox24, NavigationItemPlacement.Main, 20, "Core", "表单输入与编辑控件"),
            new(NavigationPageKeys.DataDisplay, "数据展示", SymbolRegular.Table24, NavigationItemPlacement.Main, 30, "Core", "列表、表格与展示控件"),
            new(NavigationPageKeys.LayoutContainers, "布局容器", SymbolRegular.PanelLeft24, NavigationItemPlacement.Main, 40, "Core", "布局与容器模式"),
            new(NavigationPageKeys.Dialogs, "对话框", SymbolRegular.Alert24, NavigationItemPlacement.Main, 50, "Core", "对话、通知与浮层"),
            new(NavigationPageKeys.AnimationEffects, "动画与效果", SymbolRegular.PlayCircle24, NavigationItemPlacement.Main, 60, "Core", "过渡、动效与视觉反馈"),
            new(NavigationPageKeys.Themes, "主题与样式", SymbolRegular.PaintBrush24, NavigationItemPlacement.Main, 70, "Core", "主题和视觉令牌"),
            new(NavigationPageKeys.Icons, "图标与资源", SymbolRegular.Image24, NavigationItemPlacement.Main, 80, "Core", "图标和资源组织"),
            new(NavigationPageKeys.DemoSectionHeader, "演示分组", SymbolRegular.Line24, NavigationItemPlacement.Main, 90, "Secondary", null, true),
            new(NavigationPageKeys.FormExamples, "表单示例", SymbolRegular.DocumentText24, NavigationItemPlacement.Main, 100, "Secondary", "完整表单演示"),
            new(NavigationPageKeys.DataManagement, "数据管理", SymbolRegular.Database24, NavigationItemPlacement.Main, 110, "Secondary", "数据录入与管理场景"),
            new(NavigationPageKeys.ChartExamples, "图表示例", SymbolRegular.DataBarVertical24, NavigationItemPlacement.Main, 120, "Secondary", "图表与趋势展示"),
            new(NavigationPageKeys.FileBrowser, "文件浏览器", SymbolRegular.Folder24, NavigationItemPlacement.Main, 130, "Secondary", "文件和资源浏览"),
            new(NavigationPageKeys.Settings, "系统设置", SymbolRegular.Settings24, NavigationItemPlacement.Footer, 999, "Footer", "壳层设置")
        ];

        _navigationItemsByKey = NavigationItems.ToDictionary(item => item.Key, item => item);

        MainNavigationItems = NavigationItems
            .Where(item => item.Placement == NavigationItemPlacement.Main)
            .OrderBy(item => item.Order)
            .ToArray();

        FooterNavigationItems = NavigationItems
            .Where(item => item.Placement == NavigationItemPlacement.Footer)
            .OrderBy(item => item.Order)
            .ToArray();

        ClickableNavigationItems = NavigationItems
            .Where(item => !item.IsSectionHeader)
            .OrderBy(item => item.Order)
            .ToArray();

        DefaultNavigationItem = ClickableNavigationItems[0];
    }

    public IReadOnlyList<NavigationItemDefinition> NavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> MainNavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> FooterNavigationItems { get; }

    public IReadOnlyList<NavigationItemDefinition> ClickableNavigationItems { get; }

    public NavigationItemDefinition DefaultNavigationItem { get; }

    public IReadOnlyList<string> GetBreadcrumbItems(string pageKey)
    {
        if (!_navigationItemsByKey.TryGetValue(pageKey, out NavigationItemDefinition? item))
        {
            throw new InvalidOperationException($"Breadcrumb item '{pageKey}' is not registered.");
        }

        return [item.Title];
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter MainWindowViewModelTests -v minimal`

Expected: PASS with updated navigation structure coverage.

- [ ] **Step 5: Commit**

```bash
git add tests/wpf.demo.shell.tests/ViewModels/MainWindowViewModelTests.cs src/wpf.demo.shell/Models/NavigationItemDefinition.cs src/wpf.demo.shell/ViewModels/MainWindowViewModel.cs
git commit -m "feat: expand shell navigation metadata"
```

## Task 2: Register Placeholder Pages For The Expanded Catalog

**Files:**
- Create: `src/wpf.demo.shell/ViewModels/PlaceholderPageViewModel.cs`
- Create: `src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml`
- Create: `src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml.cs`
- Modify: `src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs`
- Test: `tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs`

- [ ] **Step 1: Write the failing registration tests**

```csharp
[Fact]
public void AddShellServices_RegistersPlaceholderPageFactoryForAllDemoDestinations()
{
    RunOnStaThread(() =>
    {
        var services = new ServiceCollection();
        services.AddShellServices();

        using var serviceProvider = services.BuildServiceProvider();
        var navigationService = Assert.IsType<NavigationService>(serviceProvider.GetRequiredService<INavigationService>());
        var frame = new Frame();
        navigationService.Initialize(frame);

        foreach (string pageKey in new[]
        {
            NavigationPageKeys.Buttons,
            NavigationPageKeys.InputControls,
            NavigationPageKeys.DataDisplay,
            NavigationPageKeys.LayoutContainers,
            NavigationPageKeys.Dialogs,
            NavigationPageKeys.AnimationEffects,
            NavigationPageKeys.Themes,
            NavigationPageKeys.Icons,
            NavigationPageKeys.FormExamples,
            NavigationPageKeys.DataManagement,
            NavigationPageKeys.ChartExamples,
            NavigationPageKeys.FileBrowser
        })
        {
            navigationService.Navigate(pageKey);
            Assert.IsType<PlaceholderDemoPage>(frame.Content);
        }
    });
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ServiceCollectionExtensionsTests -v minimal`

Expected: FAIL because the page factories only know about `Home` and `Settings`.

- [ ] **Step 3: Write the minimal implementation**

`src/wpf.demo.shell/ViewModels/PlaceholderPageViewModel.cs`

```csharp
namespace WpfDemo.ViewModels;

public sealed class PlaceholderPageViewModel
{
    public required string Title { get; init; }

    public required string Description { get; init; }
}
```

`src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml`

```xml
<Page x:Class="WpfDemo.Views.Pages.PlaceholderDemoPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
      Background="Transparent">
    <Grid Margin="24">
        <ui:Card Padding="24">
            <StackPanel>
                <TextBlock FontSize="28" FontWeight="SemiBold" Text="{Binding Title}" />
                <TextBlock Margin="0,12,0,0"
                           Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                           TextWrapping="Wrap"
                           Text="{Binding Description}" />
            </StackPanel>
        </ui:Card>
    </Grid>
</Page>
```

`src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml.cs`

```csharp
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
```

`src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.Services;
using WpfDemo.ViewModels;
using WpfDemo.Views.Pages;

namespace WpfDemo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShellServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<HomePage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<PlaceholderDemoPage>();

        services.AddSingleton<INavigationService>(provider =>
        {
            Dictionary<string, Func<Page>> pages = new()
            {
                [NavigationPageKeys.Home] = () => provider.GetRequiredService<HomePage>(),
                [NavigationPageKeys.Settings] = () => provider.GetRequiredService<SettingsPage>()
            };

            pages[NavigationPageKeys.Buttons] = () => CreatePlaceholderPage(provider, "按钮与命令", "展示按钮、命令栏和动作入口。");
            pages[NavigationPageKeys.InputControls] = () => CreatePlaceholderPage(provider, "输入控件", "展示输入、编辑和表单控件。");
            pages[NavigationPageKeys.DataDisplay] = () => CreatePlaceholderPage(provider, "数据展示", "展示列表、表格和数据可视化入口。");
            pages[NavigationPageKeys.LayoutContainers] = () => CreatePlaceholderPage(provider, "布局容器", "展示布局、容器和页面结构模式。");
            pages[NavigationPageKeys.Dialogs] = () => CreatePlaceholderPage(provider, "对话框", "展示对话、通知和轻量浮层。");
            pages[NavigationPageKeys.AnimationEffects] = () => CreatePlaceholderPage(provider, "动画与效果", "展示动效与视觉反馈。");
            pages[NavigationPageKeys.Themes] = () => CreatePlaceholderPage(provider, "主题与样式", "展示主题、配色和视觉样式。");
            pages[NavigationPageKeys.Icons] = () => CreatePlaceholderPage(provider, "图标与资源", "展示图标、插图和资源组织。");
            pages[NavigationPageKeys.FormExamples] = () => CreatePlaceholderPage(provider, "表单示例", "展示业务型表单组合示例。");
            pages[NavigationPageKeys.DataManagement] = () => CreatePlaceholderPage(provider, "数据管理", "展示数据维护和管理界面示例。");
            pages[NavigationPageKeys.ChartExamples] = () => CreatePlaceholderPage(provider, "图表示例", "展示图表和趋势视图。");
            pages[NavigationPageKeys.FileBrowser] = () => CreatePlaceholderPage(provider, "文件浏览器", "展示文件和资源浏览示例。");

            return new NavigationService(pages);
        });

        services.AddSingleton<MainWindow>();

        return services;
    }

    private static PlaceholderDemoPage CreatePlaceholderPage(IServiceProvider provider, string title, string description)
    {
        return new PlaceholderDemoPage(new PlaceholderPageViewModel
        {
            Title = title,
            Description = description
        });
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ServiceCollectionExtensionsTests -v minimal`

Expected: PASS with all expanded navigation destinations resolvable.

- [ ] **Step 5: Commit**

```bash
git add tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs src/wpf.demo.shell/ViewModels/PlaceholderPageViewModel.cs src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml src/wpf.demo.shell/Views/Pages/PlaceholderDemoPage.xaml.cs src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs
git commit -m "feat: add placeholder demo pages for navigation catalog"
```

## Task 3: Introduce A Shared Theme Service

**Files:**
- Create: `src/wpf.demo.shell/Models/AppThemeOption.cs`
- Create: `src/wpf.demo.shell/Contracts/Services/IThemeService.cs`
- Create: `src/wpf.demo.shell/Services/ThemeService.cs`
- Modify: `src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs`
- Test: `tests/wpf.demo.shell.tests/Services/ThemeServiceTests.cs`

- [ ] **Step 1: Write the failing theme service tests**

```csharp
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.Services;
using Xunit;

namespace WpfDemo.Tests.Services;

public class ThemeServiceTests
{
    [Fact]
    public void SetTheme_UpdatesCurrentThemeAndRaisesEvent()
    {
        IThemeService service = new ThemeService();
        AppThemeOption? changedTo = null;

        service.ThemeChanged += (_, option) => changedTo = option;

        service.SetTheme(AppThemeOption.Light);

        Assert.Equal(AppThemeOption.Light, service.CurrentTheme);
        Assert.Equal(AppThemeOption.Light, changedTo);
    }

    [Fact]
    public void ToggleTheme_CyclesBetweenDarkAndLight()
    {
        IThemeService service = new ThemeService();

        service.SetTheme(AppThemeOption.Dark);
        service.ToggleTheme();
        Assert.Equal(AppThemeOption.Light, service.CurrentTheme);

        service.ToggleTheme();
        Assert.Equal(AppThemeOption.Dark, service.CurrentTheme);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ThemeServiceTests -v minimal`

Expected: FAIL because the shared theme service does not exist yet.

- [ ] **Step 3: Write the minimal implementation**

`src/wpf.demo.shell/Models/AppThemeOption.cs`

```csharp
namespace WpfDemo.Models;

public enum AppThemeOption
{
    Light,
    Dark,
    System
}
```

`src/wpf.demo.shell/Contracts/Services/IThemeService.cs`

```csharp
using WpfDemo.Models;

namespace WpfDemo.Contracts.Services;

public interface IThemeService
{
    event EventHandler<AppThemeOption>? ThemeChanged;

    AppThemeOption CurrentTheme { get; }

    void SetTheme(AppThemeOption theme);

    void ToggleTheme();
}
```

`src/wpf.demo.shell/Services/ThemeService.cs`

```csharp
using WpfDemo.Contracts.Services;
using WpfDemo.Models;

namespace WpfDemo.Services;

public sealed class ThemeService : IThemeService
{
    public event EventHandler<AppThemeOption>? ThemeChanged;

    public AppThemeOption CurrentTheme { get; private set; } = AppThemeOption.Dark;

    public void SetTheme(AppThemeOption theme)
    {
        if (CurrentTheme == theme)
        {
            return;
        }

        CurrentTheme = theme;
        ThemeChanged?.Invoke(this, theme);
    }

    public void ToggleTheme()
    {
        AppThemeOption next = CurrentTheme switch
        {
            AppThemeOption.Dark => AppThemeOption.Light,
            _ => AppThemeOption.Dark
        };

        SetTheme(next);
    }
}
```

`src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs`

```csharp
services.AddSingleton<IThemeService, ThemeService>();
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ThemeServiceTests -v minimal`

Expected: PASS with shared theme state behavior covered.

- [ ] **Step 5: Commit**

```bash
git add tests/wpf.demo.shell.tests/Services/ThemeServiceTests.cs src/wpf.demo.shell/Models/AppThemeOption.cs src/wpf.demo.shell/Contracts/Services/IThemeService.cs src/wpf.demo.shell/Services/ThemeService.cs src/wpf.demo.shell/Extensions/ServiceCollectionExtensions.cs
git commit -m "feat: add shared theme service"
```

## Task 4: Rework MainWindow To Render Grouped Navigation And Theme Sync

**Files:**
- Modify: `src/wpf.demo.shell/MainWindow.xaml`
- Modify: `src/wpf.demo.shell/MainWindow.xaml.cs`
- Modify: `tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs`

- [ ] **Step 1: Write the failing shell sync tests**

```csharp
[Fact]
public void MainWindow_OnContentRendered_RendersSectionHeaderWithoutMakingItClickable()
{
    RunOnStaThread(() =>
    {
        var navigationService = new RecordingNavigationService();
        var themeService = new ThemeService();
        var window = new MainWindow(new MainWindowViewModel(), navigationService, themeService);
        var navigationView = Assert.IsType<NavigationView>(window.FindName("RootNavigation"));

        InvokeOnContentRendered(window);

        Assert.Contains(
            navigationView.MenuItems.Cast<object>(),
            item => item is TextBlock textBlock && textBlock.Text == "演示分组");
        Assert.DoesNotContain(navigationService.NavigationCalls, call => call == NavigationPageKeys.DemoSectionHeader);
    });
}

[Fact]
public void MainWindow_ThemeToggleButton_UsesSharedThemeService()
{
    RunOnStaThread(() =>
    {
        var themeService = new ThemeService();
        var window = new MainWindow(new MainWindowViewModel(), new RecordingNavigationService(), themeService);
        var button = Assert.IsType<Wpf.Ui.Controls.Button>(window.FindName("ThemeToggleButton"));

        InvokeThemeToggleClick(window, button);

        Assert.Equal(AppThemeOption.Light, themeService.CurrentTheme);
        Assert.Equal("切换到深色模式", Assert.IsType<string>(button.ToolTip));
    });
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ServiceCollectionExtensionsTests -v minimal`

Expected: FAIL because `MainWindow` does not accept a theme service and still assumes every main item is clickable.

- [ ] **Step 3: Write the minimal implementation**

`src/wpf.demo.shell/MainWindow.xaml`

```xml
<ui:NavigationView
    x:Name="RootNavigation"
    Grid.Column="0"
    CompactPaneLength="48"
    OpenPaneLength="260"
    PaneDisplayMode="Left"
    PaneTitle="演示导航">
    <ui:NavigationView.PaneHeader>
        <StackPanel Margin="16,12,16,8">
            <TextBlock FontSize="12"
                       FontWeight="SemiBold"
                       Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                       Text="工作区" />
            <TextBlock Margin="0,4,0,0"
                       FontSize="11"
                       Foreground="{DynamicResource TextFillColorTertiaryBrush}"
                       Text="Fluent 组件目录" />
        </StackPanel>
    </ui:NavigationView.PaneHeader>
</ui:NavigationView>
```

`src/wpf.demo.shell/MainWindow.xaml.cs`

```csharp
using System.Windows;
using System.Windows.Controls;
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace WpfDemo;

public partial class MainWindow : FluentWindow
{
    private readonly MainWindowViewModel _viewModel;
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly Dictionary<string, NavigationViewItem> _navigationItemsByKey = new(StringComparer.Ordinal);
    private bool _navigationInitialized;

    public MainWindow(MainWindowViewModel viewModel, INavigationService navigationService, IThemeService themeService)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        DataContext = _viewModel;

        InitializeComponent();

        _themeService.ThemeChanged += HandleThemeChanged;
        ApplyTheme(_themeService.CurrentTheme);
    }

    private void PopulateNavigationItems()
    {
        RootNavigation.MenuItems.Clear();
        RootNavigation.FooterMenuItems.Clear();
        _navigationItemsByKey.Clear();

        foreach (NavigationItemDefinition definition in _viewModel.MainNavigationItems)
        {
            if (definition.IsSectionHeader)
            {
                RootNavigation.MenuItems.Add(CreateSectionHeader(definition.Title));
                continue;
            }

            NavigationViewItem item = CreateNavigationItem(definition);
            RootNavigation.MenuItems.Add(item);
            _navigationItemsByKey.Add(definition.Key, item);
        }

        foreach (NavigationItemDefinition definition in _viewModel.FooterNavigationItems)
        {
            NavigationViewItem item = CreateNavigationItem(definition);
            RootNavigation.FooterMenuItems.Add(item);
            _navigationItemsByKey.Add(definition.Key, item);
        }
    }

    private static TextBlock CreateSectionHeader(string title)
    {
        return new TextBlock
        {
            Text = title,
            Margin = new Thickness(16, 14, 16, 6),
            FontSize = 12,
            FontWeight = FontWeights.SemiBold,
            Foreground = (System.Windows.Media.Brush)Application.Current.FindResource("TextFillColorSecondaryBrush")
        };
    }

    private void ToggleThemeMode(object sender, RoutedEventArgs e)
    {
        _themeService.ToggleTheme();
    }

    private void HandleThemeChanged(object? sender, AppThemeOption option)
    {
        ApplyTheme(option);
    }

    private void ApplyTheme(AppThemeOption option)
    {
        ApplicationTheme applicationTheme = option switch
        {
            AppThemeOption.Light => ApplicationTheme.Light,
            AppThemeOption.System => ApplicationTheme.Unknown,
            _ => ApplicationTheme.Dark
        };

        ApplicationThemeManager.Apply(applicationTheme);
        UpdateThemeToggleVisual(option);
    }

    private void UpdateThemeToggleVisual(AppThemeOption option)
    {
        bool isDarkTheme = option == AppThemeOption.Dark;
        ThemeToggleIcon.Symbol = isDarkTheme ? SymbolRegular.WeatherSunny24 : SymbolRegular.WeatherMoon24;
        ThemeToggleButton.ToolTip = isDarkTheme ? "切换到浅色模式" : "切换到深色模式";
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ServiceCollectionExtensionsTests -v minimal`

Expected: PASS with grouped navigation rendering and shared theme-service flow.

- [ ] **Step 5: Commit**

```bash
git add tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs src/wpf.demo.shell/MainWindow.xaml src/wpf.demo.shell/MainWindow.xaml.cs
git commit -m "feat: group navigation items and centralize theme sync"
```

## Task 5: Build The Dashboard Home View Model

**Files:**
- Modify: `src/wpf.demo.shell/ViewModels/HomeViewModel.cs`
- Create: `src/wpf.demo.shell/Models/Home/FeatureCardDefinition.cs`
- Create: `src/wpf.demo.shell/Models/Home/QuickActionDefinition.cs`
- Create: `src/wpf.demo.shell/Models/Home/ThemeOptionDefinition.cs`
- Create: `src/wpf.demo.shell/Models/Home/ReleaseNoteDefinition.cs`
- Create: `src/wpf.demo.shell/Models/Home/InfoBannerDefinition.cs`
- Modify: `tests/wpf.demo.shell.tests/ViewModels/HomeViewModelTests.cs`

- [ ] **Step 1: Write the failing dashboard data tests**

```csharp
using WpfDemo.Models;
using WpfDemo.Services;
using WpfDemo.ViewModels;
using Xunit;

namespace WpfDemo.Tests.ViewModels;

public class HomeViewModelTests
{
    [Fact]
    public void Constructor_ExposesDashboardSections()
    {
        var viewModel = new HomeViewModel(new ThemeService());

        Assert.Equal("主页", viewModel.Title);
        Assert.Equal("搜索组件或示例...", viewModel.SearchPlaceholder);
        Assert.Equal(3, viewModel.FeatureCards.Count);
        Assert.Equal(4, viewModel.QuickActions.Count);
        Assert.Equal(3, viewModel.ThemeOptions.Count);
        Assert.Equal(3, viewModel.ReleaseNotes.Count);
        Assert.NotNull(viewModel.InfoBanner);
    }

    [Fact]
    public void QuickActions_TargetRealNavigationKeys()
    {
        var viewModel = new HomeViewModel(new ThemeService());

        Assert.Equal(
            [
                NavigationPageKeys.Buttons,
                NavigationPageKeys.InputControls,
                NavigationPageKeys.DataDisplay,
                NavigationPageKeys.Dialogs
            ],
            viewModel.QuickActions.Select(item => item.TargetPageKey).ToArray());
    }

    [Fact]
    public void Constructor_SelectsDarkThemeByDefaultAndTracksThemeChanges()
    {
        var themeService = new ThemeService();
        var viewModel = new HomeViewModel(themeService);

        Assert.Equal(AppThemeOption.Dark, viewModel.CurrentTheme);
        Assert.True(viewModel.ThemeOptions.Single(option => option.Theme == AppThemeOption.Dark).IsSelected);

        themeService.SetTheme(AppThemeOption.Light);

        Assert.Equal(AppThemeOption.Light, viewModel.CurrentTheme);
        Assert.True(viewModel.ThemeOptions.Single(option => option.Theme == AppThemeOption.Light).IsSelected);
        Assert.False(viewModel.ThemeOptions.Single(option => option.Theme == AppThemeOption.Dark).IsSelected);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter HomeViewModelTests -v minimal`

Expected: FAIL because `HomeViewModel` still only exposes `Title` and `Description`.

- [ ] **Step 3: Write the minimal implementation**

`src/wpf.demo.shell/Models/Home/FeatureCardDefinition.cs`

```csharp
using Wpf.Ui.Controls;

namespace WpfDemo.Models.Home;

public sealed record FeatureCardDefinition(SymbolRegular Icon, string Title, string Description);
```

`src/wpf.demo.shell/Models/Home/QuickActionDefinition.cs`

```csharp
using Wpf.Ui.Controls;

namespace WpfDemo.Models.Home;

public sealed record QuickActionDefinition(SymbolRegular Icon, string Title, string Subtitle, string TargetPageKey);
```

`src/wpf.demo.shell/Models/Home/ThemeOptionDefinition.cs`

```csharp
using System.ComponentModel;
using WpfDemo.Models;
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
```

`src/wpf.demo.shell/Models/Home/ReleaseNoteDefinition.cs`

```csharp
namespace WpfDemo.Models.Home;

public sealed record ReleaseNoteDefinition(string Version, string Summary, string DateLabel, string AccentBrushKey);
```

`src/wpf.demo.shell/Models/Home/InfoBannerDefinition.cs`

```csharp
using Wpf.Ui.Controls;

namespace WpfDemo.Models.Home;

public sealed record InfoBannerDefinition(SymbolRegular Icon, string Title, string Message);
```

`src/wpf.demo.shell/ViewModels/HomeViewModel.cs`

```csharp
using WpfDemo.Contracts.Services;
using WpfDemo.Models;
using WpfDemo.Models.Home;
using Wpf.Ui.Controls;

namespace WpfDemo.ViewModels;

public class HomeViewModel
{
    public HomeViewModel(IThemeService themeService)
    {
        CurrentTheme = themeService.CurrentTheme;
        themeService.ThemeChanged += (_, option) => ApplyTheme(option);
        ApplyTheme(CurrentTheme);
    }

    public string Title { get; } = "主页";

    public string Description { get; } = "对标 vision.aoi.studio 的 Fluent 组件演示首页。";

    public string SearchPlaceholder { get; } = "搜索组件或示例...";

    public AppThemeOption CurrentTheme { get; private set; }

    public IReadOnlyList<FeatureCardDefinition> FeatureCards { get; } =
    [
        new(SymbolRegular.Apps24, "丰富的组件", "覆盖组件、布局与交互模式。"),
        new(SymbolRegular.DesignIdeas24, "Fluent 设计", "统一的层次、材质和节奏。"),
        new(SymbolRegular.Code24, "高可定制性", "适合作为演示应用和壳层基础。")
    ];

    public IReadOnlyList<QuickActionDefinition> QuickActions { get; } =
    [
        new(SymbolRegular.CursorClick24, "按钮", "常用操作入口", NavigationPageKeys.Buttons),
        new(SymbolRegular.Textbox24, "文本框", "输入与编辑", NavigationPageKeys.InputControls),
        new(SymbolRegular.Table24, "数据表格", "展示与浏览", NavigationPageKeys.DataDisplay),
        new(SymbolRegular.Alert24, "对话框", "交互反馈", NavigationPageKeys.Dialogs)
    ];

    public IReadOnlyList<ThemeOptionDefinition> ThemeOptions { get; } =
    [
        new(AppThemeOption.Light, SymbolRegular.WeatherSunny24, "浅色", "明亮、清晰的展示模式"),
        new(AppThemeOption.Dark, SymbolRegular.WeatherMoon24, "深色", "沉浸式的默认演示模式"),
        new(AppThemeOption.System, SymbolRegular.Desktop24, "跟随系统", "使用系统主题偏好")
    ];

    public IReadOnlyList<ReleaseNoteDefinition> ReleaseNotes { get; } =
    [
        new("v0.3", "引入 dashboard 首页和扩展导航结构。", "2026-04-23", "SystemAccentColorPrimaryBrush"),
        new("v0.2", "统一 TitleBar 与 NavigationView 外壳。", "2026-04-22", "SystemFillColorCautionBrush"),
        new("v0.1", "初始化 Fluent 壳层与基础导航。", "2026-04-21", "SystemFillColorSuccessBrush")
    ];

    public InfoBannerDefinition InfoBanner { get; } =
        new(SymbolRegular.Info24, "组件演示", "当前阶段优先完成壳层、首页和导航结构对齐。");

    private void ApplyTheme(AppThemeOption option)
    {
        CurrentTheme = option;

        foreach (ThemeOptionDefinition themeOption in ThemeOptions)
        {
            themeOption.IsSelected = themeOption.Theme == option;
        }
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter HomeViewModelTests -v minimal`

Expected: PASS with dashboard data shape covered.

- [ ] **Step 5: Commit**

```bash
git add tests/wpf.demo.shell.tests/ViewModels/HomeViewModelTests.cs src/wpf.demo.shell/ViewModels/HomeViewModel.cs src/wpf.demo.shell/Models/Home/FeatureCardDefinition.cs src/wpf.demo.shell/Models/Home/QuickActionDefinition.cs src/wpf.demo.shell/Models/Home/ThemeOptionDefinition.cs src/wpf.demo.shell/Models/Home/ReleaseNoteDefinition.cs src/wpf.demo.shell/Models/Home/InfoBannerDefinition.cs
git commit -m "feat: add dashboard home view model"
```

## Task 6: Rebuild HomePage With Reusable Dashboard Controls

**Files:**
- Create: `src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml`
- Create: `src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml.cs`
- Create: `src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml`
- Create: `src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml.cs`
- Create: `src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml`
- Create: `src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml.cs`
- Create: `src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml`
- Create: `src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml.cs`
- Modify: `src/wpf.demo.shell/Views/Pages/HomePage.xaml`
- Modify: `src/wpf.demo.shell/Views/Pages/HomePage.xaml.cs`
- Modify: `src/wpf.demo.shell/MainWindow.xaml.cs`
- Modify: `tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs`

- [ ] **Step 1: Write the failing integration tests**

```csharp
[Fact]
public void MainWindow_HomeQuickAction_NavigatesAndActivatesMatchingNavItem()
{
    RunOnStaThread(() =>
    {
        var navigationService = new RecordingNavigationService();
        var themeService = new ThemeService();
        var window = new MainWindow(new MainWindowViewModel(), navigationService, themeService);

        InvokeOnContentRendered(window);
        window.NavigateFromDashboard(NavigationPageKeys.Dialogs);

        Assert.Equal(NavigationPageKeys.Dialogs, navigationService.LastNavigatedKey);
        Assert.Equal(["对话框"], Assert.IsAssignableFrom<IEnumerable<string>>(
            Assert.IsType<BreadcrumbBar>(window.FindName("RootBreadcrumb")).ItemsSource));
    });
}

[Fact]
public void MainWindow_SetThemeFromDashboard_UpdatesTitleBarToggleState()
{
    RunOnStaThread(() =>
    {
        var themeService = new ThemeService();
        var window = new MainWindow(new MainWindowViewModel(), new RecordingNavigationService(), themeService);
        var button = Assert.IsType<Wpf.Ui.Controls.Button>(window.FindName("ThemeToggleButton"));

        window.SetThemeFromDashboard(AppThemeOption.System);

        Assert.Equal(AppThemeOption.System, themeService.CurrentTheme);
        Assert.Equal("切换到深色模式", Assert.IsType<string>(button.ToolTip));
    });
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj --filter ServiceCollectionExtensionsTests -v minimal`

Expected: FAIL because dashboard interactions are not yet exposed by the shell.

- [ ] **Step 3: Write the minimal implementation**

`src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml`

```xml
<UserControl x:Class="WpfDemo.Views.Controls.FeatureCardControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">
    <ui:Card Padding="20">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
                <RowDefinition Height="Auto" />
            </Grid.RowDefinitions>
            <ui:SymbolIcon Symbol="{Binding Icon}" FontSize="24" Foreground="{DynamicResource SystemAccentColorPrimaryBrush}" />
            <TextBlock Grid.Row="1" Margin="0,16,0,0" FontSize="18" FontWeight="SemiBold" Text="{Binding Title}" />
            <TextBlock Grid.Row="2" Margin="0,8,0,0" Foreground="{DynamicResource TextFillColorSecondaryBrush}" TextWrapping="Wrap" Text="{Binding Description}" />
        </Grid>
    </ui:Card>
</UserControl>
```

`src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml`

```xml
<UserControl x:Class="WpfDemo.Views.Controls.QuickActionCardControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">
    <ui:Button Padding="16" Appearance="Subtle" Click="HandleClick">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            <ui:SymbolIcon Symbol="{Binding Icon}" FontSize="20" />
            <StackPanel Grid.Column="1" Margin="12,0,0,0">
                <TextBlock FontWeight="SemiBold" Text="{Binding Title}" />
                <TextBlock Foreground="{DynamicResource TextFillColorSecondaryBrush}" Text="{Binding Subtitle}" />
            </StackPanel>
            <ui:SymbolIcon Grid.Column="2" Symbol="ChevronRight24" />
        </Grid>
    </ui:Button>
</UserControl>
```

`src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml.cs`

```csharp
using System;
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
```

`src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml`

```xml
<UserControl x:Class="WpfDemo.Views.Controls.ThemeOptionCardControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">
    <ui:Button Padding="16" Appearance="Transparent" Click="HandleClick">
        <ui:Button.Style>
            <Style TargetType="ui:Button">
                <Setter Property="Background" Value="Transparent" />
                <Setter Property="BorderBrush" Value="Transparent" />
                <Style.Triggers>
                    <DataTrigger Binding="{Binding IsSelected}" Value="True">
                        <Setter Property="Background" Value="{DynamicResource SystemAccentColorLight3Brush}" />
                        <Setter Property="BorderBrush" Value="{DynamicResource SystemAccentColorPrimaryBrush}" />
                    </DataTrigger>
                </Style.Triggers>
            </Style>
        </ui:Button.Style>
        <ui:Card Padding="18">
            <StackPanel>
                <ui:SymbolIcon Symbol="{Binding Icon}" FontSize="20" />
                <TextBlock Margin="0,12,0,0" FontWeight="SemiBold" Text="{Binding Title}" />
                <TextBlock Margin="0,6,0,0" Foreground="{DynamicResource TextFillColorSecondaryBrush}" TextWrapping="Wrap" Text="{Binding Description}" />
            </StackPanel>
        </ui:Card>
    </ui:Button>
</UserControl>
```

`src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml.cs`

```csharp
using System;
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
```

`src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml`

```xml
<UserControl x:Class="WpfDemo.Views.Controls.InfoBannerControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">
    <Border Padding="18"
            Background="{DynamicResource LayerFillColorDefaultBrush}"
            BorderBrush="{DynamicResource CardStrokeColorDefaultBrush}"
            BorderThickness="1"
            CornerRadius="18">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="*" />
            </Grid.ColumnDefinitions>
            <ui:SymbolIcon VerticalAlignment="Top" Symbol="{Binding Icon}" FontSize="20" />
            <StackPanel Grid.Column="1" Margin="12,0,0,0">
                <TextBlock FontWeight="SemiBold" Text="{Binding Title}" />
                <TextBlock Margin="0,4,0,0" Foreground="{DynamicResource TextFillColorSecondaryBrush}" TextWrapping="Wrap" Text="{Binding Message}" />
            </StackPanel>
        </Grid>
    </Border>
</UserControl>
```

`src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml.cs`

```csharp
using System.Windows.Controls;

namespace WpfDemo.Views.Controls;

public partial class FeatureCardControl : UserControl
{
    public FeatureCardControl()
    {
        InitializeComponent();
    }
}
```

`src/wpf.demo.shell/Views/Pages/HomePage.xaml`

```xml
<Page x:Class="WpfDemo.Views.Pages.HomePage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:controls="clr-namespace:WpfDemo.Views.Controls"
      xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
      Background="Transparent">
    <ScrollViewer VerticalScrollBarVisibility="Auto">
        <StackPanel Margin="24">
            <Grid Margin="0,0,0,20">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="320" />
                </Grid.ColumnDefinitions>
                <StackPanel>
                    <TextBlock FontSize="34" FontWeight="SemiBold" Text="{Binding Title}" />
                    <TextBlock Margin="0,10,0,0"
                               Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                               TextWrapping="Wrap"
                               Text="{Binding Description}" />
                </StackPanel>
                <ui:TextBox Grid.Column="1"
                            Height="40"
                            VerticalAlignment="Center"
                            PlaceholderText="{Binding SearchPlaceholder}" />
            </Grid>

            <ItemsControl ItemsSource="{Binding FeatureCards}">
                <ItemsControl.ItemsPanel>
                    <ItemsPanelTemplate>
                        <UniformGrid Columns="3" />
                    </ItemsPanelTemplate>
                </ItemsControl.ItemsPanel>
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <controls:FeatureCardControl Margin="0,0,16,0" />
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>

            <Grid Margin="0,24,0,0">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="1.2*" />
                    <ColumnDefinition Width="0.8*" />
                </Grid.ColumnDefinitions>

                <ui:Card Grid.Column="0" Padding="20">
                    <StackPanel>
                        <TextBlock FontSize="22" FontWeight="SemiBold" Text="快捷入口" />
                        <ItemsControl Margin="0,16,0,0" ItemsSource="{Binding QuickActions}">
                            <ItemsControl.ItemTemplate>
                                <DataTemplate>
                                    <controls:QuickActionCardControl Margin="0,0,0,12" Loaded="QuickActionCard_Loaded" />
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </StackPanel>
                </ui:Card>

                <ui:Card Grid.Column="1" Margin="20,0,0,0" Padding="20">
                    <StackPanel>
                        <TextBlock FontSize="22" FontWeight="SemiBold" Text="主题模式" />
                        <ItemsControl Margin="0,16,0,0" ItemsSource="{Binding ThemeOptions}">
                            <ItemsControl.ItemTemplate>
                                <DataTemplate>
                                    <controls:ThemeOptionCardControl Margin="0,0,0,12" Loaded="ThemeOptionCard_Loaded" />
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </StackPanel>
                </ui:Card>
            </Grid>

            <Grid Margin="0,24,0,0">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="0.9*" />
                    <ColumnDefinition Width="1.1*" />
                </Grid.ColumnDefinitions>

                <ui:Card Grid.Column="0" Padding="20">
                    <StackPanel>
                        <TextBlock FontSize="22" FontWeight="SemiBold" Text="版本时间线" />
                        <ItemsControl Margin="0,16,0,0" ItemsSource="{Binding ReleaseNotes}">
                            <ItemsControl.ItemTemplate>
                                <DataTemplate>
                                    <Grid Margin="0,0,0,12">
                                        <Grid.ColumnDefinitions>
                                            <ColumnDefinition Width="Auto" />
                                            <ColumnDefinition Width="*" />
                                            <ColumnDefinition Width="Auto" />
                                        </Grid.ColumnDefinitions>
                                        <Ellipse Width="10"
                                                 Height="10"
                                                 Margin="0,6,12,0"
                                                 VerticalAlignment="Top"
                                                 Fill="{DynamicResource SystemAccentColorPrimaryBrush}" />
                                        <StackPanel Grid.Column="1">
                                            <TextBlock FontWeight="SemiBold" Text="{Binding Version}" />
                                            <TextBlock Margin="0,4,0,0"
                                                       Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                                                       TextWrapping="Wrap"
                                                       Text="{Binding Summary}" />
                                        </StackPanel>
                                        <TextBlock Grid.Column="2"
                                                   Margin="12,0,0,0"
                                                   Foreground="{DynamicResource TextFillColorTertiaryBrush}"
                                                   Text="{Binding DateLabel}" />
                                    </Grid>
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </StackPanel>
                </ui:Card>

                <ui:Card Grid.Column="1" Margin="20,0,0,0" Padding="20">
                    <StackPanel>
                        <TextBlock FontSize="22" FontWeight="SemiBold" Text="组件预览" />
                        <WrapPanel Margin="0,16,0,0">
                            <ui:Button Margin="0,0,12,12" Content="Primary Button" />
                            <ui:Button Margin="0,0,12,12" Appearance="Secondary" Content="Secondary Button" />
                            <ui:ToggleSwitch Margin="0,0,12,12" IsChecked="True" />
                            <ui:TextBox Width="180" Margin="0,0,12,12" PlaceholderText="TextBox" />
                            <ui:ComboBox Width="160" Margin="0,0,12,12" SelectedIndex="0">
                                <ComboBoxItem Content="Option A" />
                                <ComboBoxItem Content="Option B" />
                            </ui:ComboBox>
                            <Slider Width="180" Margin="0,0,12,12" Value="36" />
                            <CheckBox Margin="0,0,12,12" Content="CheckBox" IsChecked="True" />
                            <RadioButton Margin="0,0,12,12" Content="RadioButton" IsChecked="True" />
                        </WrapPanel>
                    </StackPanel>
                </ui:Card>
            </Grid>

            <controls:InfoBannerControl Margin="0,24,0,0" DataContext="{Binding InfoBanner}" />
        </StackPanel>
    </ScrollViewer>
</Page>
```

`src/wpf.demo.shell/Views/Pages/HomePage.xaml.cs`

```csharp
using System;
using System.Windows.Controls;
using WpfDemo.Models;
using WpfDemo.ViewModels;

namespace WpfDemo.Views.Pages;

public partial class HomePage : Page
{
    public Action<string>? QuickActionRequested { get; set; }

    public Action<AppThemeOption>? ThemeRequested { get; set; }

    public HomePage(HomeViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    private void QuickActionCard_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is Views.Controls.QuickActionCardControl card)
        {
            card.ActionRequested -= HandleQuickActionRequested;
            card.ActionRequested += HandleQuickActionRequested;
        }
    }

    private void ThemeOptionCard_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is Views.Controls.ThemeOptionCardControl card)
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
```

`src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml.cs`

```csharp
using System.Windows.Controls;

namespace WpfDemo.Views.Controls;

public partial class InfoBannerControl : UserControl
{
    public InfoBannerControl()
    {
        InitializeComponent();
    }
}
```

`src/wpf.demo.shell/MainWindow.xaml.cs`

```csharp
private void NavigateTo(string pageKey)
{
    _navigationService.Navigate(pageKey);
    SetActiveNavigationItem(pageKey);
    RootBreadcrumb.ItemsSource = _viewModel.GetBreadcrumbItems(pageKey);

    if (PageHost.Content is HomePage homePage)
    {
        homePage.QuickActionRequested = NavigateFromDashboard;
        homePage.ThemeRequested = SetThemeFromDashboard;
    }
}

internal void NavigateFromDashboard(string pageKey)
{
    NavigateTo(pageKey);
}

internal void SetThemeFromDashboard(AppThemeOption option)
{
    _themeService.SetTheme(option);
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj -v minimal`

Expected: PASS with dashboard interactions, navigation state, and theme synchronization covered.

- [ ] **Step 5: Commit**

```bash
git add tests/wpf.demo.shell.tests/Services/ServiceCollectionExtensionsTests.cs src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml src/wpf.demo.shell/Views/Controls/FeatureCardControl.xaml.cs src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml src/wpf.demo.shell/Views/Controls/QuickActionCardControl.xaml.cs src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml src/wpf.demo.shell/Views/Controls/ThemeOptionCardControl.xaml.cs src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml src/wpf.demo.shell/Views/Controls/InfoBannerControl.xaml.cs src/wpf.demo.shell/Views/Pages/HomePage.xaml src/wpf.demo.shell/Views/Pages/HomePage.xaml.cs src/wpf.demo.shell/MainWindow.xaml.cs
git commit -m "feat: rebuild home page as demo dashboard"
```

## Task 7: Final Verification And Smoke Check

**Files:**
- Modify: `README.md` only if implementation details diverged from the current project description
- Modify: `docs/PROJECT.md` only if architecture notes diverged from the current project description

- [ ] **Step 1: Run the focused test suite**

Run: `dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj -v minimal`

Expected: PASS for navigation, theme service, shell sync, and home dashboard coverage.

- [ ] **Step 2: Run the application build**

Run: `dotnet build .\src\wpf.demo.shell\wpf.demo.shell.csproj -v minimal`

Expected: PASS with no XAML compile errors.

- [ ] **Step 3: Smoke-check the shell manually**

Run: `dotnet run --project .\src\wpf.demo.shell\wpf.demo.shell.csproj`

Expected:
- Left pane shows the expanded demo catalog and one non-clickable `演示分组` label.
- Home page shows header, search, feature cards, quick actions, theme cards, release notes, component preview, and info banner.
- Clicking a quick action updates `Frame`, left navigation active state, and breadcrumb together.
- Title-bar theme button and home-page theme cards stay in sync.

- [ ] **Step 4: Update docs only if needed**

If the implemented shell materially changes the current wording in `README.md` or `docs/PROJECT.md`, update those descriptions before finalizing. If the current docs already describe the shell accurately, skip this step.

- [ ] **Step 5: Final commit**

```bash
git add README.md docs/PROJECT.md
git commit -m "docs: align project docs with dashboard shell" || echo "No documentation changes required"
```
