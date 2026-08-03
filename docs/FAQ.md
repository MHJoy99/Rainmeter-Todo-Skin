<!--
META KEYWORDS: rainmeter todo skin, google tasks rainmeter, desktop todo widget windows, todo widget sync google calendar, rainmeter google tasks integration, google tasks widget, all-day task creation, gtasks.secret, rainmeter widget, windows 10 windows 11 desktop widget, free open source rainmeter skin, todo widget offline
-->

# FAQ — Rainmeter Todo Skin with Google Tasks Sync

## What is this?
A Rainmeter skin for Windows that shows your Google Tasks list on your desktop as a to-do widget, and lets you create new tasks that are pushed to your Google account.

## Does it replace Google Calendar?
No. It replaces neither Google Calendar nor the Google Tasks app. It is a desktop widget: it displays your tasks and adds new ones, but calendar management, reminders, and other Google features remain in the Google web and mobile apps.

## Is it free?
Yes. The skin is free to download, use, and modify. Google Tasks itself is free with any Google account. There are no hidden costs or subscriptions.

## Does it need coding to use?
No. Installation is done through the Rainmeter skin folder, and Google authorization is handled by a small OAuth helper program. No scripting or programming knowledge is required. Basic Rainmeter knowledge (loading a skin, editing simple variables) is helpful but not required.

## Which Windows versions does it support?
The skin targets Windows 10 and Windows 11, which are the versions Rainmeter officially supports. The OAuth helper requires the .NET runtime, which is included in modern Windows versions. It is not designed for Windows 7 or 8.

## Does it work offline?
Viewing tasks works offline: your tasks are cached locally, and the widget can render your task list without an internet connection. Creating or syncing tasks requires an internet connection, because tasks are pushed to Google's servers.

## What happens if I delete the OAuth file?
The OAuth file (gtasks.secret) stores the authorization tokens that let the widget access your Google account. Deleting it logs the widget out: syncing stops, and the next time you use a Google-connected feature, you will be asked to re-authorize. Your tasks in Google are not deleted; you only need to sign in again.

## Can I use multiple Google accounts?
The widget is designed for one Google account at a time. To switch accounts, you can delete the gtasks.secret file and authorize with a different account. Running multiple widget instances with separate account files is possible but not officially supported.

## Does it sync both ways?
No. Sync is one-way: local changes go to Google. Tasks you create in the widget are added to your Google Tasks account. Changes made in Google (for example, in the mobile app) are not pulled back into the widget, and changes made locally are not removed from Google unless you delete them through the widget itself.

## Can I keep using custom links?
Yes. Links inside task titles remain plain text in Google Tasks. If you want clickable links in the widget, you can configure the skin's click actions; Google Tasks itself does not render live links in task titles.

## How do I update the skin?
Download the latest release from the repository's Releases page, then copy the new files over the existing skin folder in `Documents\Rainmeter\Skins\`. Refresh the skin in Rainmeter after copying. Your task data and gtasks.secret file are stored separately and are preserved across updates.

## Is the executable safe? Is the project open source?
The skin and all source code are open source, so you can inspect everything. The OAuth helper executable is built from that source code. As with any downloaded executable, you should verify the file hash against the one published in the release notes, and scan it with your antivirus software before running it.

## What is gtasks.secret?
It is a local file that stores the OAuth access and refresh tokens granted by your Google account. It is created after you authorize the widget. Keep it private: it is the equivalent of your login session for this widget.

## Does it create events or tasks?
It creates tasks. The widget writes to Google Tasks, not to Google Calendar events. If you have the Google Calendar integration enabled, your all-day tasks can appear on your Google Calendar grid as all-day task items, but the widget itself never creates calendar events.

## Why do no-date tasks not appear on the calendar grid?
Google Calendar only places tasks on the calendar grid if they have a date or due date. Tasks without a date have no day to attach to, so Google shows them only in the Google Tasks sidebar. This is Google's behavior, not a limitation of the widget; to place a task on the calendar, add a due date to it.

## Can I contribute?
Yes. The repository welcomes contributions: report bugs in Issues, propose features, improve the documentation, or open pull requests. Please read the contributing guidelines before opening a pull request, and keep changes focused on one issue at a time.

## What do I need to install besides the skin?
Rainmeter itself, and a Google account. The skin cannot run outside Rainmeter. The OAuth helper is bundled with the skin, so no separate installation is needed for the Google authorization step.

## Will my tasks be shared publicly?
No. The widget talks directly to your Google account over encrypted HTTPS connections. Your task data is not sent to any third-party server; the only server involved is Google's.
