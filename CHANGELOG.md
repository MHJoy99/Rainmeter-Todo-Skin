# Changelog

## 1.5.0 (2026-08-04)

### Added

- One-click Google Tasks sign-in: the host app now ships with a built-in OAuth 2.0 client, so "Sign in with Google" (Settings → Google Tasks) works without creating a Google Cloud project; the browser consent flow uses the loopback redirect `http://127.0.0.1:8392/`
- OAuth tokens are stored encrypted with Windows DPAPI (current user) in `@Resources\gtasks.secret`, with automatic migration of previous plaintext token files
- Optional override of the built-in OAuth client via `@Resources\gtasks-client.json`
- "Sign out" action on the Google Tasks settings tab removes the saved account
- All-English UI: settings dialog ("Todo settings"), task editor ("New task" / "Edit task") and task manager ("All tasks")
- Assembly metadata for the host executables ("Rainmeter Todo Skin", MHJoy99, 1.5.0.0); About tab shows the runtime version read from `app-version.txt`

### Changed

- `Generated.inc` (Rainmeter settings) is written as UTF-8 without BOM and only rewritten when the content changes
- Dialogs are process-DPI aware and auto-scale; windows compensate for displays above the 120-DPI design baseline, and the tile layout scales via the UI scale setting (auto or 50%–200%)
- The updater now checks `MHJoy99/Rainmeter-Todo-Skin` releases, downloads the `Todo-Skin-v<version>.zip` package and installs both the Todo and Calendar skins while preserving user data (`tasks.json`, `ui-scale.txt`, secrets)

### Fixed

- Dialog layout on 100% DPI displays and correct scaling across 70%–125% UI scales and 200% Windows DPI, covered by the layout probes in `scripts\Test-Backends.ps1`

## 1.4.6 (2026-08-03)

### Added

- Google Tasks API integration: clicking a todo with no custom link now creates a real Google Task instead of a calendar event
- OAuth 2.0 desktop flow with loopback redirect `http://127.0.0.1:8392/`
- Automatic token refresh and `invalid_grant` recovery
- Google Tasks setup documentation

### Changed

- Dated todos are created as all-day tasks (`YYYY-MM-DDT00:00:00.000Z`)
- Secrets are excluded from releases

## Earlier Versions

### Added

- Todo board UI
- Add/edit/delete/toggle for todos
- Labels and notes
- Custom link targets
- Daily arXiv feed integration
- Updater
- Icons
- Settings, manage, and all-tasks views
