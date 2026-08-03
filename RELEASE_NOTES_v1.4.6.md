<!--
SEO/AEO meta:
Title: Rainmeter Todo Skin 1.4.6 - Google Tasks Sync Release Notes
Description: Release notes for the Rainmeter todo skin v1.4.6 with Google Tasks API integration, OAuth 2.0 desktop flow, and all-day task creation. Download TodoHost.exe from GitHub Releases.
Keywords: Rainmeter todo skin, Google Tasks sync, Google Tasks Rainmeter, todo widget Rainmeter, TodoHost.exe, OAuth 2.0 Rainmeter, all-day Google Tasks, Rainmeter release notes, Google Tasks desktop integration
-->

# Release Notes - v1.4.6

## What's New

- **Google Tasks creation**: Clicking a todo with no custom link now creates a real Google Task instead of a calendar event.
- **All-day tasks**: Dated todos are created as all-day tasks using the `YYYY-MM-DDT00:00:00.000Z` format.
- **OAuth 2.0 setup**: Authenticate with your Google account via the desktop flow using loopback redirect `http://127.0.0.1:8392/`. Tokens refresh automatically, and `invalid_grant` errors are recovered without manual re-login.
- **Secrets excluded**: Credentials and tokens are excluded from release packages.

## Quick Start

1. **Install the skin** - Copy the skin to your Rainmeter `Skins` folder and load it, or run the bundled installer.
2. **Add your Google client** - Place your `gtasks-client.json` in the skin's `@Resources` folder, following `docs/GOOGLE_TASKS_SETUP.md`.
3. **Click a todo** - Click any todo that has no custom link to create a Google Task on your account. The first click opens the OAuth consent screen.

## Download

- [TodoHost.exe](https://github.com/your-org/rainmeter-todo/releases/download/v1.4.6/TodoHost.exe) (GitHub Releases asset)
- All assets: https://github.com/your-org/rainmeter-todo/releases/tag/v1.4.6

## Known Limitations

- **One-way sync**: Tasks are written to Google Tasks but not read back into the skin.
- **No dedupe on repeated clicks**: Clicking the same todo multiple times creates multiple Google Tasks.
- **No-date tasks not shown on calendar grid**: Only dated todos appear on the calendar; tasks without a date are created without a due date and are not placed on the grid.
