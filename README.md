# com.dreamy.ui

Reusable UI package for Dreamy internal Unity projects.

The v0.1 API follows the current project `Assets/_BaseSource/Base.UI` flow: `UIPanel` registers with `PanelManager`, panels can be created from Addressables, Android/Escape back closes the latest backable panel, tabs are grouped by button/page, and tween components drive show/hide animation.

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

Use transition when opening a child panel over the current panel:

```csharp
await PanelManager.Instance.Transition<ShopPanel>("ui_shop");
```

## Scope

This package owns reusable runtime UI helpers. Game-specific popups, concrete panel prefabs, scene flow, sound routing, and localization belong in the game template or game project.
