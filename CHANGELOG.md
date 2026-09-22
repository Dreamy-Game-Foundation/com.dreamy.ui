# Changelog

## [Unreleased]

- Added `UITweenPlayer`, a single Auto/Manual hybrid player that excludes
  effects owned by nested players and prunes destroyed references safely.
- Removed the duplicate entry-based tween system and the old `TweenPlayer`
  wrapper. Manual authoring now uses explicit target groups.
- Made legacy tween effects cancellation- and destroyed-target-safe, and moved
  post-hide notification before a non-cached panel is destroyed.
- Added per-show/per-hide timing override fields and player-level preset
  inheritance without `Resources.Load` defaults.
- Made tab initialization idempotent and wait for completion before auto-open.
- Added focused editor tests for tween ownership and destroyed effect pruning.
- Added `IPanelTransition` so panels resolve transitions by interface.

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
