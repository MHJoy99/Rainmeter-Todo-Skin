# Changelog

## Unreleased

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
