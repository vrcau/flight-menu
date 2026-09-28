# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- New editor only `FlightMenuReferenceItem` component to reuse a menu item in multiple menu groups.
  - It will be remove from the built world.
- New `FlightMenuStationMenuSwitcher` component to replace the menu group of the menu views when the local player enters a station.
  - It has to be on the same GameObject as the `VRCStation`, otherwise Udon will never receive the station events.
  - A single component can assign a menu group to the desktop, the left hand and the right hand menu view at once, one component per station.
- New `FlightMenuView.SetRootMenuGroup` to replace the root menu group of a menu view and navigate to it at runtime.

### Changed

- `FlightMenuSetup` is no longer editor only, it now also holds the desktop, the left hand and the right hand menu view, so that every station menu switcher can share the menu views of one menu system.
  - It is now a `UdonSharpBehaviour` and moved from `VAU.FlightMenuSystem.Runtime.EditorOnly` to `VAU.FlightMenuSystem.Runtime`.
  - A `FlightMenuSetup` in a scene gets its Udon backing behaviour when the scene is loaded, a `FlightMenuSetup` inside a prefab has to be re-added or the prefab re-saved once.

### Fixed

- Menu view without root menu group no longer throw an exception on start, it stays inactive until a menu group is assigned to it.

## [0.1.3] - 2026-09-27

### Added

- Menu item can continuously send a custom event to its event target every frame while the item is being held.

### Fixed

- Unable to modify view's menu group via flight menu setup editor

## [0.1.2] - 2026-09-17

### Added

- Keep push/pull thumbtack or hold left mouse button to keep trigger menu item. [#8](https://github.com/vrcau/flight-menu/pull/8)
  - Set given variable to true or false when use hold menu item.
  - Or send custom on hold start or end event

## [0.1.1] - 2026-09-15

### Changed

- Remove event target will no logger set update from event target to false.

### Added

- New `FlightMenuViewSetup` component to change and preview menu group for all child non-popup menu `MenuView`.

### Fixed

- Flight menu item with update from event target enabled but event target missing will no longer crash MenuView.

## [0.1.0] - 2026-09-12

### Added

- Fully configurable menu.
- New menu item type.
  - SubMenu
    - Popup
  - Button
  - Slider

[unreleased]: https://github.com/vrcau/flight-menu/compare/core-v0.1.3...HEAD
[0.1.3]: https://github.com/vrcau/flight-menu/compare/core-v0.1.2...core-v0.1.3
[0.1.2]: https://github.com/vrcau/flight-menu/compare/core-v0.1.1...core-v0.1.2
[0.1.1]: https://github.com/vrcau/flight-menu/compare/core-v0.1.0...core-v0.1.1
[0.1.0]: https://github.com/vrcau/flight-menu/releases/tag/core-v0.1.0
