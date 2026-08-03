# Troubleshooting Guide

This guide covers the most common problems with the Todo skin for Rainmeter and its Google Tasks / Google Calendar sync. Each entry follows the same structure: the symptom, the most likely cause, and the fix.

If your issue is not listed here, see the "Still stuck?" section at the bottom of this page.

---

## Task created in the skin is not in Google Calendar

**Problem:** You added a task in the skin, but it does not show up on the Google Calendar grid.

**Cause:** One of three things, in order of likelihood:

1. **You did not click the task to push it.** Typing text and pressing the plus button only stores the task locally in the skin. The task is not sent to Google until you click the task row itself, which triggers the authorization and upload flow.
2. **The task has no date.** Tasks without a due date are synced to Google Tasks but never appear on a calendar grid, because the grid only renders dated entries. Such tasks are visible in the Google Tasks panel only.
3. **The task is dated for a different day.** A dated task appears on the calendar day of its due date, not on the day you created it. Check the day matching the task's due date rather than today.

**Fix:** Click the task row to trigger the sync. If it still does not appear, open tasks.google.com and confirm the task is listed there. For no-due tasks, remember they only exist in the Tasks panel, never on the grid. For dated tasks, switch to the calendar day that matches the due date.

---

## I clicked and got multiple copies of the same task

**Problem:** Clicking a task a second time created a duplicate entry in Google Tasks.

**Cause:** The skin has no deduplication logic. Every click sends the task to Google Tasks as a new entry, so re-clicking the same row always produces another copy.

**Fix:** Do not click a task more than once after it has been synced. If duplicates already exist, delete the extra copies manually at tasks.google.com. There is no automatic cleanup feature.

---

## Status still says "Google Tasks setup needed: gtasks-client.json missing"

**Problem:** The skin status line keeps reporting that the OAuth client file is missing, even though you downloaded it.

**Cause:** The file `gtasks-client.json` is not in the folder the skin is actually reading. The skin checks two locations: the skin folder inside `Documents\Rainmeter\Skins\Todo` and the installation copy inside `Program Files\Rainmeter\Skins\Todo`. If the file exists in only one of them, the loaded copy may still be the one without it.

**Fix:** Verify the file exists in **both** locations:

- `Documents\Rainmeter\Skins\Todo\@Resources\gtasks-client.json`
- `Program Files\Rainmeter\Skins\Todo\@Resources\gtasks-client.json`

Copy the file to the one that is missing it, then refresh the skin.

---

## Error: gtasks-client.json is missing client_id or client_secret

**Problem:** The skin loads `gtasks-client.json`, but the error log reports that `client_id` or `client_secret` is missing.

**Cause:** The JSON file is of the wrong type or is corrupted. Google offers two client types: **Web application** and **Desktop app**. This skin requires the **Desktop app** client type. A Web application JSON does not contain the `client_id` / `client_secret` fields in the expected places, and a truncated or hand-edited file can lose them entirely.

**Fix:** Re-download the client JSON from the Google Cloud Console, making sure you create a **Desktop app** OAuth client. Do not edit the file manually. Replace the existing file and refresh the skin.

---

## Authorization expired / invalid_grant / authorization revoked

**Problem:** Syncing stops working and the log shows `invalid_grant`, or the skin asks for authorization again even though you authorized before.

**Cause:** The stored refresh token is no longer valid. This happens when the token expires, when you revoke access in your Google account security settings, or when the OAuth client was recreated in the console.

**Fix:** Delete the file `@Resources\gtasks.secret` in the skin folder. Click any todo row in the skin. The skin will start a fresh authorization flow. Complete the consent in the browser window that opens, and the token will be regenerated.

---

## Authorization timed out (no response in 2 minutes)

**Problem:** The browser opened for authorization, you completed the consent page, but the skin reports a timeout and nothing syncs.

**Cause:** The browser callback never returned to the skin. This usually means the redirect URL was not allowed, the browser window was left open too long, or the local callback listener was blocked by a firewall. The skin waits about two minutes and then gives up.

**Fix:** Click the todo row again to start a new authorization attempt. This time, complete the Google consent screen promptly and leave the browser tab open until the page redirects. If it still times out, verify the OAuth client's redirect URL includes the local callback address, and allow the skin through your firewall.

---

## Task shows one day off

**Problem:** A task that should be dated today appears on the calendar for the previous (or next) day.

**Cause:** All-day tasks are stored by Google in UTC. When the skin converts the UTC date to local time, the shift can land on the adjacent calendar day. The local date you entered is preserved in the task itself; only the grid placement can appear offset.

**Fix:** No action needed if the task's date is correct when opened in Google Tasks. The offset is a display artifact of UTC conversion and your machine's timezone setting. For reliable behavior, confirm your system timezone is set correctly. Dated tasks with an explicit time are not affected.

---

## No error but task never appears

**Problem:** Nothing fails visibly, but the task never shows up in Google Tasks or on the calendar.

**Cause:** The sync is failing silently, or the request is being rejected before it reaches Tasks. Two checks cover the common cases:

1. The last sync result is recorded in the skin's state file.
2. The OAuth client may have been disabled or deleted in the Google Cloud Console, which produces a quiet failure instead of a clear error.

**Fix:** Open the file `tasks.json` inside `@Resources` and look at the `meta.status` entry. It contains the result of the last sync attempt and usually names the failing step. Then open the Google Cloud Console and confirm the OAuth client still exists and is enabled. Re-enable or recreate it if needed, then retry the sync.

---

## Rainmeter says file not found

**Problem:** Clicking a task row triggers a Rainmeter error saying a file was not found.

**Cause:** The action in the skin's `.ini` file references `#@#` paths, which point to the `@Resources` folder relative to the currently loaded skin. If the skin was installed with a renamed folder, or only part of the package was copied, `#@#` resolves to a location where the scripts or data files do not exist.

**Fix:** Make sure `LeftMouseUpAction` lines use `#@#`-prefixed paths and that the `@Resources` folder sits next to the `.ini` file. If anything looks moved or missing, reinstall the skin from the original package so the folder structure is restored, then refresh the skin.

---

## Still stuck?

If your problem is not covered above:

- Open an issue on the GitHub Issues page of this repository. Include the skin version, Rainmeter version, and the relevant lines from the Rainmeter log (right-click the tray icon, then "Log").
- Read the FAQ in the repository before posting, since it answers common questions about permissions, tokens, and the sync flow.

Please attach the exact error text from the log and the content of `tasks.json`'s `meta.status` field, if present. This reduces back-and-forth and helps resolve your issue faster.
