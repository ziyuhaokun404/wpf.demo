# wpf.demo

一个基于 `WPF-UI` 的 WPF 桌面演示项目，用来验证和迭代 `NavigationView` 壳层、深色 Fluent 风格界面，以及后续组件演示页的组织方式。

当前项目重点不是业务功能，而是壳层能力：

- `FluentWindow + Mica` 桌面窗口
- 左侧 `NavigationView` 导航
- 顶部自定义 `TitleBar`
- 亮色 / 深色主题切换
- `BreadcrumbBar + Frame` 内容宿主
- 轻量 DI 启动和页面导航
- xUnit 测试覆盖基础壳层行为

## 技术栈

- `.NET 10`
- `WPF`
- [`WPF-UI` 4.2.0](https://github.com/lepoco/wpfui)
- `Microsoft.Extensions.DependencyInjection`
- `xUnit`

## 当前结构

```text
wpf.demo/
├─ src/
│  └─ wpf.demo.shell/
│     ├─ Contracts/
│     ├─ Extensions/
│     ├─ Models/
│     ├─ Services/
│     ├─ ViewModels/
│     └─ Views/
├─ tests/
│  └─ wpf.demo.shell.tests/
└─ scripts/
```

## 已实现内容

- 基础 Shell 窗口
- 中文导航元数据
- `主页` / `系统设置` 两个示例页面
- `NavigationView` 主导航和底部导航
- 顶栏主题切换按钮
- 壳层导航与主题切换相关测试

## 运行方式

在仓库根目录执行：

```powershell
dotnet run --project .\src\wpf.demo.shell\wpf.demo.shell.csproj
```

## 测试与构建

运行测试：

```powershell
dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj
```

构建项目：

```powershell
dotnet build .\src\wpf.demo.shell\wpf.demo.shell.csproj
```

## 项目定位

这个仓库当前是一个“界面壳层演示项目”，目标是逐步演化成更完整的 Fluent 风格组件演示应用，而不是直接承载真实业务系统。

如果你想了解更详细的项目说明、目录职责和后续演进方向，见 [docs/PROJECT.md](docs/PROJECT.md)。
