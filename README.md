<!--
Meta description: Todo Board is a Rainmeter todo skin that turns your desktop into a Google Tasks widget. Click any todo without a custom link to create a real Google Task via OAuth 2.0, with automatic all-day tasks when a date is set. Sync Google Tasks to desktop, add, edit, delete and toggle todos, all offline-first with a local tasks.json file.

Keywords: Rainmeter todo skin, Google Tasks Rainmeter widget, desktop todo board, sync Google Tasks to desktop, Windows todo widget, all-day Google Task, Rainmeter Google Tasks OAuth, Google Tasks desktop, todo board skin, TodoHost, MHJoy99
-->

# Rainmeter Todo Skin — Google Tasks Desktop Widget

## TL;DR / What is this?

Todo Board is a Windows Rainmeter skin that puts a todo board widget directly on your desktop and syncs it with Google Tasks. Click a todo that has no custom link and it is created as a real Google Task in your account via OAuth 2.0; give a todo a date and it becomes an all-day Google Task that shows up in the Calendar Tasks layer and at tasks.google.com. The skin is fully local-first — every todo lives in a local `tasks.json` file, so the board keeps working offline even without Google.

## Table of Contents

- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Google Account Setup](#google-account-setup)
- [Frequently Asked Questions](#frequently-asked-questions)
- [Documentation](#documentation)
- [License](#license)

## Status

- License: MIT
- Latest version: 1.4.6
- Maintainer: MHJoy99
- Platform: Windows 10 / Windows 11

## Features

| Feature | Description |
| --- | --- |
| Click-to-create Google Task | Clicking a todo without a custom link creates a real task in Google Tasks through the Google Tasks API |
| All-day tasks from dates | A todo with a date (YYYY-MM-DD) syncs as an all-day task (YYYY-MM-DDT00:00:00.000Z) |
| Dated vs. undated | Todos without a date are created as tasks without a due date |
| OAuth 2.0 authentication | Desktop-client OAuth 2.0 flow with loopback redirect at http://127.0.0.1:8392/ |
| Custom links preserved | Todos with custom links open the link normally and are never sent to Google |
| Full todo management | Add, edit, delete and toggle (complete/uncomplete) todos from the desktop widget |
| Local-first storage | All todos are stored locally in `tasks.json`; nothing is stored on the cloud except tasks you intentionally create |
| Offline capable | The todo board works completely offline; Google sync only happens when you click a todo without a custom link |
| C# host app | TodoHost.exe, a lightweight C# host that handles OAuth, the Google Tasks API and local storage |

## Requirements

- Windows 10 or Windows 11
- Rainmeter 4.4 or newer
- A Google account (for the Google Tasks sync feature)
- .NET runtime compatible with the included TodoHost.exe host application
- Internet connection only needed when creating tasks via the Google Tasks API

## Installation

1. Download the latest release (version 1.4.6) of the Todo Board skin from the Releases page.
2. Extract the archive and copy the skin folder into `Documents\Rainmeter\Skins` (your Rainmeter skins directory).
3. Open Rainmeter, click "Refresh All" in the Rainmeter context menu (or restart Rainmeter) so the new skin appears.
4. Load the "Todo Board" skin from the Rainmeter skin list. The widget now runs fully offline with its local `tasks.json`.
5. Set up Google account access — follow [docs/GOOGLE_TASKS_SETUP.md](docs/GOOGLE_TASKS_SETUP.md) to create a Google Cloud project, enable the Google Tasks API and generate your own OAuth 2.0 desktop client credentials.
6. Enter your own OAuth client ID and client secret where documented in [docs/CONFIGURATION.md](docs/CONFIGURATION.md) (the config file inside the skin folder where you place your credentials).
7. Restart the skin (or click the re-authenticate option) and allow the loopback redirect at `http://127.0.0.1:8392/` in your browser once to finish the OAuth 2.0 flow. Done — your desktop todo board now syncs with Google Tasks.

## Google Account Setup

The skin uses OAuth 2.0 with a desktop client and a loopback redirect URI (`http://127.0.0.1:8392/`). You need your own Google Cloud credentials:

1. Follow [docs/GOOGLE_TASKS_SETUP.md](docs/GOOGLE_TASKS_SETUP.md) to create a Google Cloud project and enable the Google Tasks API.
2. Create OAuth 2.0 desktop credentials and add `http://127.0.0.1:8392/` as the authorized redirect URI.
3. Place your client ID and client secret in the config location described in [docs/CONFIGURATION.md](docs/CONFIGURATION.md).

## Frequently Asked Questions

### Does this create Google Calendar events?

No. This skin creates real Google Tasks, not Google Calendar events. Tasks with a date are created as all-day tasks (sent to the Google Tasks API with a due date of `YYYY-MM-DDT00:00:00.000Z`), which then appear in the Google Calendar Tasks layer and at tasks.google.com.

### Where do my tasks appear?

All tasks you create appear at tasks.google.com and in the Tasks layer of Google Calendar. On the desktop, everything is shown on your Todo Board widget.

### Do I need to re-authenticate?

No. After the initial one-time OAuth 2.0 consent at `http://127.0.0.1:8392/`, the skin stores its tokens locally and refreshes them automatically. You only re-authenticate if you revoke access in your Google account or delete the token files.

### Is my client secret safe?

Yes. Your OAuth client ID and client secret stay in the local configuration file on your machine. They are never uploaded anywhere, and you are the only one who uses them with your own Google Cloud project.

### Can I use it without Google?

Yes. Todo Board is local-first: every todo lives in the local `tasks.json` file, and all add, edit, delete and toggle operations work completely offline. Google is only contacted when you click a todo without a custom link to create a task.

### Does it work offline?

Yes. The widget, your todo list and all editing operations work fully offline. Only the click-to-create-Google-Task feature needs an internet connection and valid OAuth credentials.

### What happens when I click a todo?

If the todo has a custom link, the link opens normally in your browser. If it has no custom link, TodoHost.exe calls the Google Tasks API and creates a real Google Task — all-day if a date is set, without a due date otherwise.

### How are all-day tasks handled?

When a todo has a date, it is sent to the Google Tasks API as an all-day task using the date with a midnight timestamp (`YYYY-MM-DDT00:00:00.000Z`), so it displays correctly in the Google Calendar Tasks layer.

### Where is my data stored?

Todo data is stored in a local `tasks.json` file inside the skin's folder. Tokens from the OAuth 2.0 flow are stored locally as well. Nothing is stored in the cloud except tasks you deliberately create via the Google Tasks API.

### Why is a C# host app needed?

The TodoHost.exe host application performs the OAuth 2.0 desktop flow, the loopback redirect on port 8392 and the Google Tasks API calls, because Rainmeter alone cannot handle HTTPS callbacks and token storage reliably.

## Documentation

- [Google Tasks Setup Guide](docs/GOOGLE_TASKS_SETUP.md) — create a Google Cloud project, enable the Google Tasks API and generate OAuth 2.0 desktop credentials.
- [Configuration Guide](docs/CONFIGURATION.md) — where to enter your client ID, client secret and other skin settings.
- [Changelog](CHANGELOG.md) — version history and changes for every release.

## License

This project is released under the MIT License. You may use, modify and distribute it freely, provided the copyright notice is retained. See the `LICENSE` file for the full license text.

---

Todo Board by MHJoy99 — a Rainmeter todo skin that syncs Google Tasks to your Windows desktop, with offline-first local storage, OAuth 2.0 authentication and all-day task support.
