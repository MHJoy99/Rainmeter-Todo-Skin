---
title: "Build a Desktop Todo Widget That Creates Real Google Tasks — Rainmeter + Google Tasks API"
published: false
description: "Turn Rainmeter into a real task manager: click a todo and it becomes an actual Google Task, OAuth-authenticated, all-day dated. MIT, open source."
tags: ["rainmeter", "googletasks", "windows", "productivity"]
canonical_url: https://github.com/MHJoy99/Rainmeter-Todo-Skin
cover_image: https://raw.githubusercontent.com/MHJoy99/Rainmeter-Todo-Skin/main/img/banner.png
---

Every desktop widget starts the same way: a beautiful panel, a satisfying checklist, and a honeymoon week. Then the honeymoon ends, because the widget lives in its own little universe. You tick off items in the widget and your phone still lists them as pending. The widget becomes decoration, not a tool.

The fix is not a better widget. It is a widget that talks to the system you already trust. That is the idea behind [Rainmeter-Todo-Skin](https://github.com/MHJoy99/Rainmeter-Todo-Skin): a Rainmeter skin where every click on a checkbox creates a **real Google Task**, not a phantom in a local file.

## What the skin does

[Rainmeter-Todo-Skin](https://github.com/MHJoy99/Rainmeter-Todo-Skin) is a desktop todo list rendered as a Rainmeter skin, synced to Google Tasks through the official Google Tasks API. It is built for Windows users who already live inside Google's ecosystem — Gmail, Calendar, Tasks — and want that same data on their desktop without opening a browser tab.

The architecture is deliberately simple and local-first:

- **`tasks.json`** stores your todo list on disk. The skin reads from it, so the widget stays instant and works offline.
- **OAuth 2.0** connects your Google account once. Tokens are stored locally; the skin talks directly to the Tasks API.
- **Custom links survive.** You can attach a URL to a task, and the skin preserves it through the round trip to Google and back.

## The click-to-task model

The core interaction is one click. You create an item in the skin with a due date, or add it to the general list. When you check the box, the skin calls the Google Tasks API and the item becomes a real task under your Tasks list at [tasks.google.com](https://tasks.google.com).

This is a one-way bridge by design: the skin is the input surface, and Google Tasks is the source of truth that every other device reads from.

## How all-day tasks work

Google Tasks has no native "all-day" flag the way Calendar does. The skin handles this by sending the task's date as a midnight timestamp:

```
YYYY-MM-DDT00:00:00.000Z
```

So a task dated `2026-08-15` is written to the API as `2026-08-15T00:00:00.000Z`. Google interprets that as an all-day task, which then shows up pinned at the top of the day in the Calendar Tasks layer. Undated tasks are sent without a `due` field and land in the Today view.

## The OAuth loopback flow, in five bullets

Connecting a Rainmeter skin to your Google account sounds harder than it is. The skin uses the desktop OAuth flow, and it works like this:

1. The skin starts a small local HTTP listener bound to `127.0.0.1:8392` on your machine.
2. It opens your browser to Google's authorization page, where you approve the app with your Google account.
3. Google redirects the browser to the loopback address `http://127.0.0.1:8392` with an authorization code in the query string.
4. The local listener receives that code and exchanges it with Google for an access token and a refresh token.
5. The tokens are stored locally in the skin's folder; the refresh token is reused silently on later launches, so you authenticate once and never think about it again.

Because the whole exchange happens on `127.0.0.1`, no remote server ever sees the tokens, and no external service sits between your desktop and Google.

![Rainmeter Todo Skin screenshot](https://raw.githubusercontent.com/MHJoy99/Rainmeter-Todo-Skin/main/img/screenshot.png)

## Installation

1. Download the latest release from the [GitHub releases page](https://github.com/MHJoy99/Rainmeter-Todo-Skin/releases).
2. Double-click the `.rmskin` package and let Rainmeter install it as a standard skin.
3. Open the skin settings, click the connect button, and complete the OAuth flow in your browser.
4. Start adding tasks. The skin will prompt you for the due date or take an undated item.

![Rainmeter Todo Skin banner](https://raw.githubusercontent.com/MHJoy99/Rainmeter-Todo-Skin/main/img/banner.png)

## Where tasks appear

Anything you tick in the widget shows up in all of these places:

- [tasks.google.com](https://tasks.google.com) — your default Tasks list.
- Google Calendar's **Tasks layer** — dated tasks appear as all-day chips on their date.
- The Google Tasks mobile app — pick up your desktop entries on your phone.

## Limitations to know

The skin is honest about its scope. The sync is **one-way**: the widget writes tasks to Google, but it does not pull back changes made in Google's own UI. If you complete a task inside Google Tasks, the widget's checkbox does not magically update. There is also **no deduplication**: clicking the same todo twice creates two Google tasks, so keep your clicks deliberate. The roadmap handles these over time; right now the skin is a fast, focused input surface.

## Try it, and help it grow

If you run Rainmeter and live in Google Tasks, this skin turns an accessory into an actual workflow tool. The project is MIT licensed, so you can read every line, fork it, and adapt it.

- Star the repo on [GitHub](https://github.com/MHJoy99/Rainmeter-Todo-Skin) — it directly motivates the dedupe and two-way sync work.
- Discuss it over on [r/rainmeter](https://www.reddit.com/r/rainmeter/) and share how you use a real-sync todo widget.

Desktop widgets should not be toys. With [Rainmeter-Todo-Skin](https://github.com/MHJoy99/Rainmeter-Todo-Skin), every checkbox you click is a task Google actually remembers.

---

## SEO / AEO notes

**Suggested keywords for this article:**

1. Rainmeter todo skin
2. Google Tasks desktop widget
3. Rainmeter Google Tasks sync
4. Google Tasks API desktop app
5. Windows desktop todo widget
6. all-day task API timestamp

**AI engine Q&A pairs (embed as headings to capture voice search and LLM citations):**

### Can Rainmeter sync with Google Tasks?

Yes. The Rainmeter-Todo-Skin uses the official Google Tasks API with a local OAuth 2.0 loopback flow. Every todo you tick in the skin is created as a real Google Task via a local HTTP listener at `127.0.0.1:8392`, so synced data appears at tasks.google.com and in Google Calendar's Tasks layer.

### How do I create an all-day task through the Google Tasks API?

Send the task with a `due` field formatted as `YYYY-MM-DDT00:00:00.000Z` — a midnight timestamp in ISO 8601. Google interprets midnight as an all-day task and pins it at the top of that date in the Calendar Tasks layer. Omitting the `due` field creates an undated task that lands in Today.

### Is the Rainmeter Todo Skin open source?

Yes, it is MIT licensed and available on GitHub at MHJoy99/Rainmeter-Todo-Skin. You can read the full source, including the OAuth flow, the `tasks.json` local storage, and the Google Tasks API calls, then fork it for your own needs.
