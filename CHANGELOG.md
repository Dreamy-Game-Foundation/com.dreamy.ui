# Changelog

## [Unreleased]

- Added `IPanelTransition` so panels resolve transitions by interface.
- Added `TweenEffectPlayer` with serialized `TweenEffectEntry` effects.
- Added entry-based scale, fade, move, rotate, size, color, and punch tweens.
- Shared tween playback and settings resolution between legacy tween components
  and the new entry-based player.
- Kept `TweenPlayer` and legacy tween components supported without automatic
  prefab migration.

## [0.1.1] - 2026-06-15

- Made panel show/hide operations cancellation-safe and killed cancelled tweens.
- Deduplicated panel creation by panel type.
- Replaced persistent transition callbacks with manager-owned transition state.
- Serialized transitions and created full-stretch ordered layer roots.

## [0.1.0] - 2026-06-06

- Added UI package scaffold.
- Added `UIPanel` and `PanelManager`.
- Added button helpers.
- Added tab helpers.
- Added tween helpers.
- Added safe area and scalable pointer helpers.
