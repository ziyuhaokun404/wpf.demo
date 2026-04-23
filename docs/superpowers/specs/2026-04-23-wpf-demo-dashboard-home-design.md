# WPF Demo Dashboard Home Design

**Date:** 2026-04-23

## Goal

Upgrade `wpf.demo` from a minimal `NavigationView` shell into a Fluent-style component demo application whose overall structure and home page closely align with the provided target screenshot.

This design intentionally focuses on the first major milestone rather than the entire finished application. The first milestone is:

- expand the left navigation into a richer demo-oriented structure
- rebuild `HomePage` into a dashboard-style landing page
- preserve the current shell architecture and theme-switching foundation
- keep deeper example pages mostly as placeholders in this iteration

## Current State

`wpf.demo` already has a working shell foundation:

- `FluentWindow` with dark theme and `Mica`
- custom `TitleBar`
- `NavigationView` shell with main and footer items
- `BreadcrumbBar + Frame` content host
- `Home` and `Settings` pages
- lightweight `INavigationService`
- xUnit coverage for shell navigation and theme toggle behavior

This is a strong shell baseline, but it is still far from the target screenshot in product shape:

- left navigation only contains two items instead of a full demo catalog
- the home page is still a single simple card
- there is no search surface, no feature cards, no timeline, no preview panel, and no bottom info banner
- there is no reusable content-card system for building a demo dashboard

## Scope

This design covers the first-stage transformation required to make the app feel like the target screenshot:

- expand the left-side navigation to a demo-catalog structure
- rebuild `HomePage` into a dashboard-style page with multiple content sections
- introduce home-page view models for cards, quick actions, theme options, release notes, and info banner content
- introduce reusable home-page card controls where repetition would otherwise bloat `HomePage.xaml`
- unify top-right theme toggle behavior with a visible home-page theme section
- add placeholder pages for the new navigation items so the shell structure is coherent

This design does not cover:

- full implementation of every demo page
- real search/filter behavior across the app
- persisted theme preference storage
- page-level animation systems
- business data, backend integration, or user settings persistence
- advanced route hierarchy or nested breadcrumb history

## Recommended Approach

Build this in three layers, but only implement the first two in this milestone:

1. expand the shell navigation model and page registration
2. rebuild `HomePage` as a dashboard composed of reusable cards and data-driven sections
3. defer rich example-page implementations to later milestones

This is the right balance because the screenshot's impact comes from shell scale and dashboard composition, not from fully implemented deep pages. If the shell and dashboard are correct, the app will already read as the intended product.

## Alternatives Considered

### 1. Rebuild everything directly in `HomePage.xaml`

Pros:

- fastest path to something visually similar
- fewer files in the short term

Cons:

- `HomePage.xaml` would become too large and brittle
- hard to reuse cards and visual patterns later
- hard to test and maintain

### 2. Build every target page before fixing the home dashboard

Pros:

- would make all navigation destinations real

Cons:

- delays the primary visual win
- spreads effort across too many pages too early
- makes it harder to stabilize the shell and content design language first

### 3. Expand navigation and build the dashboard first

Pros:

- matches the target screenshot fastest
- creates reusable patterns for later pages
- preserves momentum and architectural clarity

Cons:

- some navigation destinations will remain mostly placeholder pages at first

This design chooses option 3.

## Architecture

The shell architecture remains intact:

- `App` stays the composition root
- `MainWindow` stays the shell host
- `INavigationService` continues to own only `Frame` navigation
- `MainWindowViewModel` continues to own navigation metadata

The main architectural change is at the presentation layer:

- navigation metadata becomes rich enough to represent a larger demo catalog
- the home page becomes a composite dashboard page
- repeated UI sections are extracted into focused controls

This keeps the navigation service small and avoids introducing a route framework just to reproduce a dashboard screenshot.

## Navigation Design

The target screenshot implies a documentation-like or component-gallery application rather than a business dashboard. The left navigation should be restructured accordingly.

### Main Navigation

Recommended items:

- `主页`
- `按钮与命令`
- `输入控件`
- `数据展示`
- `布局容器`
- `对话框`
- `动画与效果`
- `主题与样式`
- `图标与资源`

### Section Label

After the main navigation block, show a non-clickable visual section label:

- `演示分组`

This can be modeled either as a display-only navigation item or as shell-owned text content in the pane. It should not navigate.

### Secondary Demo Group

Recommended items:

- `表单示例`
- `数据管理`
- `图表示例`
- `文件浏览器`

### Footer

Keep:

- `系统设置`

## Navigation Model Changes

`NavigationItemDefinition` should grow beyond the current shell-focused model to support richer presentation.

Recommended fields:

- `Key`
- `Title`
- `IconSymbol`
- `Placement`
- `Order`
- `Section`
- `Description`
- `IsSectionHeader`

Notes:

- `Placement` still separates main vs footer
- `Section` groups items within the main area
- `Order` guarantees stable rendering order
- `IsSectionHeader` allows non-clickable labels such as `演示分组`

If a simpler version is preferred for the first implementation, `Section` and `IsSectionHeader` can be introduced without adding a full hierarchical tree model.

## Home Page Design

The home page should become a dashboard-style landing page made of clearly separated sections.

### Overall Layout

Use a `ScrollViewer` with a vertically stacked content root. Within that root, use a grid-based layout to align major sections with consistent spacing and card rhythm.

Recommended major sections:

1. page header with title/description and search field
2. three feature cards
3. quick actions block
4. theme switch block
5. release timeline block
6. component preview block
7. bottom info banner

### Header Row

Left side:

- page title `主页`
- short descriptive subtitle

Right side:

- search box placeholder such as `搜索组件或示例...`

The search box can be presentational only in this milestone. It does not need actual filtering yet.

### Feature Cards

Three horizontally aligned cards:

- `丰富的组件`
- `Fluent 设计`
- `高可定制性`

Each card should contain:

- accent icon
- title
- short explanatory text
- subtle right-arrow affordance

These cards are presentation-first. Clicking them can remain optional or route to placeholder pages in this milestone.

### Quick Actions Block

This section should expose four compact entry cards:

- `按钮`
- `文本框`
- `数据表格`
- `对话框`

These should behave as shortcuts into relevant navigation destinations, so clicking one should update both the page host and the selected navigation item.

### Theme Switch Block

This section should present three mutually exclusive options:

- `浅色`
- `深色`
- `跟随系统`

The important requirement is state synchronization:

- top-right title-bar theme toggle and home-page theme cards must reflect the same current theme state

The first milestone does not need persistence, but it does need one shared source of truth for current theme mode.

### Release Timeline Block

This block should visually resemble a lightweight release log:

- version title
- short summary
- date aligned on the right
- colored timeline markers

The content can be static sample data in this milestone.

### Component Preview Block

This block should visually demonstrate several common controls:

- primary and secondary buttons
- switch
- text box
- combo box
- slider
- check boxes
- radio buttons

The point is not full functionality. The point is to communicate a modern component-gallery feel.

### Bottom Info Banner

This should be a wide info strip or rounded message banner with:

- info icon
- title
- explanatory sentence
- optional close glyph

It acts as a summary statement for the demo app.

## Home Page Data Model

Do not hard-code all dashboard content directly in XAML. The home page should be driven by structured view models.

Recommended models:

- `HomeDashboardViewModel`
- `FeatureCardViewModel`
- `QuickActionViewModel`
- `ThemeOptionViewModel`
- `ReleaseNoteItemViewModel`
- `InfoBannerViewModel`

`HomeViewModel` can either become the dashboard root or be replaced by `HomeDashboardViewModel`, depending on how much continuity with the current code is desired. The key requirement is that the page not be built from raw hard-coded layout text alone.

## Reusable Controls

To prevent `HomePage.xaml` from becoming too large, repeated patterns should be extracted.

Recommended reusable controls:

- `FeatureCardControl`
- `QuickActionCardControl`
- `ThemeOptionCardControl`
- `InfoBannerControl`

The release timeline can stay inline in `HomePage` if the markup stays manageable. It only needs extraction if it starts obscuring page readability.

## Placeholder Pages

The newly added navigation destinations do not need full implementations yet, but they should resolve cleanly through the shell.

Recommended approach:

- create a reusable template-style placeholder page
- bind title and description per destination
- use the same card/spacing language as the shell

This keeps navigation complete without overbuilding.

## Interaction Rules

Three interaction flows must be coherent:

### 1. Navigation Flow

Left navigation click:

- selects the item
- navigates the `Frame`
- updates the breadcrumb

### 2. Theme Flow

Theme change from either top-right button or home-page theme options:

- updates the application theme
- updates the title-bar icon and tooltip
- updates the selected theme card in the dashboard

### 3. Quick Action Flow

Dashboard shortcut click:

- navigates to the target page
- updates the left navigation selected state
- updates the breadcrumb

Without this shared behavior, the dashboard will feel visually correct but structurally inconsistent.

## File Impact

Expected to modify:

- `C:\Code\wpf.demo\src\wpf.demo.shell\Models\NavigationItemDefinition.cs`
- `C:\Code\wpf.demo\src\wpf.demo.shell\ViewModels\MainWindowViewModel.cs`
- `C:\Code\wpf.demo\src\wpf.demo.shell\Extensions\ServiceCollectionExtensions.cs`
- `C:\Code\wpf.demo\src\wpf.demo.shell\MainWindow.xaml`
- `C:\Code\wpf.demo\src\wpf.demo.shell\MainWindow.xaml.cs`
- `C:\Code\wpf.demo\src\wpf.demo.shell\Views\Pages\HomePage.xaml`

Expected to add:

- home dashboard view models
- reusable card controls
- placeholder pages for new navigation items
- tests for richer navigation metadata and theme-state synchronization

## Testing Strategy

Tests should cover behavior that can regress:

- richer navigation metadata ordering and placement
- shell navigation selection consistency
- theme toggle synchronization between title bar and dashboard model
- page registration for all added placeholder pages

Visual composition itself should be smoke-verified by running the app and checking:

- section ordering
- navigation completeness
- theme consistency
- alignment and spacing

## Success Criteria

The first milestone is complete when:

- the left navigation visually and structurally resembles the target screenshot
- the home page contains all major dashboard sections from the target screenshot
- the app still builds and tests pass
- theme switching is synchronized between the title bar and dashboard theme section
- quick actions can route into the corresponding demo pages
- placeholder pages exist for the expanded navigation catalog

## Risks And Mitigations

### Risk: `HomePage.xaml` becomes too large

Mitigation:

Extract repeated card patterns into reusable controls early.

### Risk: Navigation model becomes ad hoc

Mitigation:

Expand the model explicitly now instead of layering one-off properties later.

### Risk: Theme state duplicates across shell and home page

Mitigation:

Use one shared theme state source rather than two independent UI updates.

### Risk: Too much work spent on placeholder pages

Mitigation:

Use a common template for first-pass placeholder pages and keep them intentionally shallow.
