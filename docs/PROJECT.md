# 项目说明

## 1. 项目目标

`wpf.demo` 是一个用于验证 WPF Fluent 风格壳层设计的本地演示项目。

它的主要目标是：

- 形成一个可持续演进的桌面应用壳层
- 验证 `WPF-UI` 在真实项目结构中的使用方式
- 为后续“组件演示页 / 示例页 / 仪表盘首页”提供宿主
- 用较轻量的代码结构替代过重的应用框架

当前项目更像“桌面界面实验场”，而不是最终业务产品。

## 2. 当前实现范围

当前已经落地的能力包括：

- 基于 `FluentWindow` 的主窗口
- 深色主题默认加载
- 顶部 `TitleBar` 品牌区
- 顶栏主题切换按钮
- 左侧 `NavigationView`
- 右侧 `BreadcrumbBar + Frame`
- `Home` / `Settings` 页面注册与切换
- 轻量 `INavigationService`
- Shell、导航和主题切换相关自动化测试

当前还没有实现的内容包括：

- 多级导航体系
- 首页复杂卡片布局
- 搜索框和组件预览区
- 更新日志面板
- 完整组件示例页矩阵
- 持久化配置和用户偏好存储

## 3. 目录职责

### `src/wpf.demo.shell`

主应用项目，负责桌面壳层、页面、导航和界面呈现。

关键目录：

- `Contracts/Services`
  放接口定义，例如 `INavigationService`
- `Extensions`
  放依赖注入注册逻辑
- `Models`
  放导航项等轻量模型
- `Services`
  放壳层服务实现
- `ViewModels`
  放页面与壳层的展示数据
- `Views`
  放 `MainWindow` 和页面 XAML

### `tests/wpf.demo.shell.tests`

测试项目，当前使用 `xUnit`，主要覆盖：

- 导航服务行为
- DI 注册行为
- 壳层默认导航行为
- 主题切换按钮行为

### `scripts`

预留给构建辅助脚本、导出脚本或开发辅助工具。当前还没有形成固定规范。

## 4. 当前架构特点

项目现在采用的是“轻量壳层 + 页面注册 + Frame 导航”的方式。

核心思路是：

- 用 `App.xaml.cs` 作为组合根
- 用 `Microsoft.Extensions.DependencyInjection` 做对象注册
- 用 `INavigationService` 封装页面切换
- 用 `MainWindowViewModel` 提供导航元数据
- 用 `MainWindow` 负责 Shell 级控件协调

这种结构的优点是：

- 比单窗口硬编码更清晰
- 比大型 MVVM 框架更轻
- 后续增加页面成本较低
- 适合当前 demo 阶段快速演进

## 5. 当前 UI 方向

目前界面方向已经确定为：

- 深色 Fluent 风格
- `NavigationView` 左侧导航壳层
- 顶栏操作按钮与系统控制按钮视觉对齐
- 中文化导航标签
- 逐步向“组件演示应用”方向扩展

如果继续迭代，下一阶段最值得做的是：

1. 重做 `HomePage`，变成仪表盘式首页
2. 扩展左侧导航，形成“组件分类 + 示例分组”
3. 把常见界面块拆成可复用卡片控件
4. 补完整的亮色 / 深色 / 跟随系统主题面板

## 6. 开发命令

### 运行

```powershell
dotnet run --project .\src\wpf.demo.shell\wpf.demo.shell.csproj
```

### 构建

```powershell
dotnet build .\src\wpf.demo.shell\wpf.demo.shell.csproj
```

### 测试

```powershell
dotnet test .\tests\wpf.demo.shell.tests\wpf.demo.shell.tests.csproj
```

## 7. 维护建议

为了保持这个 demo 可持续演进，建议遵守下面几个原则：

- 壳层逻辑放在 `MainWindow`，不要把页面内容重新塞回主窗口
- 导航元数据继续集中在 ViewModel，不要在 XAML 里重新写死一长串菜单
- 新增示例页时优先复用现有卡片和间距规范
- 先补测试再改导航和主题行为，避免壳层回归
- 不要过早引入重型框架，先保持当前轻量结构
