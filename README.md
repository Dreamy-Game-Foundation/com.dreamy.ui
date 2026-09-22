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

Panels resolve transitions through `IPanelTransition`. New prefabs use one
`UITweenPlayer`:

- **Auto** (default) collects `UITweenBase` components below the player,
  including inactive children. Collection stops at a nested `UITweenPlayer`, so
  a child player always owns its own effects.
- **Manual** stores `TweenTargetGroup` entries. Every group has one target and
  a serialized list of effects; no child tween components are required.

Invalid or destroyed effect targets are
skipped with a contextual warning; they do not fail the rest of a show/hide
operation.

## Tween Settings

Create timing preset assets from `Assets/Create/Dreamy/UI/Tween Preset`. Each tween
loads its own default asset from `Resources/Tween`:

- `MoveTweenSettings.asset`
- `ScaleTweenSettings.asset`
- `FadeTweenSettings.asset`
- `RotateTweenSettings.asset`
- `SizeTweenSettings.asset`
- `ColorTweenSettings.asset`

Tween components load their default asset from `Reset()` and retry during
initialization when the reference is missing. Show/hide delays remain on each
component so sequences can be staggered directly in the Inspector.

Each tween can override the shared ease and duration values. For staggered
lists, add `TweenDelayByIndex` to each animated item and one
`TweenDelayControl` to their parent. The controller applies show/hide intervals
in hierarchy order and can reverse the hide order.

Create one `TweenPresetLibrary` in
`Assets/Resources/Dreamy/UI/TweenPresetLibrary.asset`. Every player loads it
automatically. Manual effects resolve settings in this order: effect preset,
target-group preset, library preset for that effect type, then code fallback. Ease,
duration, and delay overrides are independent for show and hide, so an
un-overridden field keeps following its preset.

`UIScalable` can optionally run a lightweight idle pulse. Pointer press stops
the idle tween; release completes its feedback animation and resumes idle.

## Scope

This package owns reusable runtime UI helpers. Game-specific popups, concrete panel prefabs, scene flow, sound routing, and localization belong in the game template or game project.
