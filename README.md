# com.dreamy.ui

Reusable UI package for Dreamy internal Unity projects.

The v0.1 API follows the current project `Assets/_BaseSource/Base.UI` flow: `UIPanel` registers with `PanelManager`, panels can be created from Addressables, Android/Escape back closes the latest backable panel, tabs are grouped by button/page, and tween components or tween effect entries drive show/hide animation.

## Requirements

- Unity 6000.0+
- `com.dreamy.core`
- `com.dreamy.assets`
- UniTask
- DOTween
- Unity UI
- TextMeshPro if using `UITabButton` text state

The game template should own these dependency URLs.

Assembly dependency direction:

```text
com.dreamy.ui -> com.dreamy.assets -> com.dreamy.core
com.dreamy.ui -> com.dreamy.core
```

`com.dreamy.core` must not reference UI or assets.

## Usage

Create a panel prefab with a `UIPanel` subclass:

```csharp
public sealed class MainMenuPanel : UIPanel
{
    public override bool CanBack => false;
}
```

Add a `PanelManager` to the UI root canvas, then:

```csharp
MainMenuPanel panel = await PanelManager.Instance.Show<MainMenuPanel>("ui_main_menu");
await PanelManager.Instance.Close<MainMenuPanel>();
```

Panels can override `Layer` to select `Screen`, `Popup`, or `Overlay`. Add a
`UILayerRoot` child for each layer under `PanelManager`; the manager discovers
them automatically. At runtime, missing roots are created as full-stretch
`RectTransform` objects in Screen, Popup, Overlay order.

Override `CanCache` with `true` to deactivate a hidden panel instead of
destroying it. A later `Show<TPanel>()` reuses the cached instance.

Use transition when opening a child panel over the current panel:

```csharp
await PanelManager.Instance.Transition<ShopPanel>("ui_shop");
```

## Tween Transitions

Panels use one `UITweenPlayer` directly:

- **Auto** (default) collects `UITweenBase` components below the player,
  including inactive children. Collection stops at a nested `UITweenPlayer`, so
  a child player always owns its own effects.
- **Manual** stores `TweenTargetGroup` entries. Every group has one target and
  a serialized list of effects; no child tween components are required. The
  Inspector presents each target as a card with a contextual `+` menu for
  adding an effect.

Invalid or destroyed effect targets are
skipped with a contextual warning; they do not fail the rest of a show/hide
operation.

## Tween Settings

Create timing preset assets from `Assets/Create/Dreamy/UI/Tween Preset` and
assign them by type in `TweenPresetLibrary`. Manual effects receive their
matching preset as soon as they are created in the Inspector.

Each tween can override the shared ease and duration values. Enable `Stagger`
on a `UITweenPlayer` to apply show/hide intervals in cached order for both
Auto and Manual mode; hide can use the reverse order. Existing
`TweenDelayByIndex` and `TweenDelayControl` remain available for component
lists that need explicit per-item ordering.

Create one `TweenPresetLibrary` in
`Assets/Resources/Dreamy/UI/TweenPresetLibrary.asset`. Every player loads it
automatically. Effects resolve settings in this order: effect preset, library
preset for that effect type, then code fallback. Ease, duration, and delay
overrides are independent for show and hide, so an un-overridden field keeps
following its preset. `FadeTweenEffect` adds a `CanvasGroup` to its target when
needed.

## Progress and shine

`UIProgressBar` renders a normalized value through an `Image` configured as
Filled and can animate to a new value with DOTween. `UIShineWave` applies the
included `Dreamy/UI/Shine Wave` shader to a `Graphic` with an isolated runtime
material, so its wave never changes a shared UI material. Set `Rotation` per
wave to orient its streak. Add one `UIShineWaveController` to the parent of
multiple waves to trigger idle waves with randomized interval, duration, and
rotation. Waves register and unregister themselves as item prefabs spawn or
despawn below that parent. Configure several `Wave Patterns`; every pattern
defines its burst size, delay between streaks, cooldown, duration, and rotation
range, and the controller chooses one pattern per burst.

`UIScalable` can optionally run a lightweight idle pulse. Pointer press stops
the idle tween; release completes its feedback animation and resumes idle.

## Scope

This package owns reusable runtime UI helpers. Game-specific popups, concrete panel prefabs, scene flow, sound routing, and localization belong in the game template or game project.
