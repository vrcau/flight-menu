# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[unreleased]: https://github.com/vrcau/flight-menu/compare/core-v0.1.2...HEAD
[0.1.2]: https://github.com/vrcau/flight-menu/compare/core-v0.1.1...core-v0.1.2
[0.1.1]: https://github.com/vrcau/flight-menu/compare/core-v0.1.0...core-v0.1.1
[0.1.0]: https://github.com/vrcau/flight-menu/releases/tag/core-v0.1.0
