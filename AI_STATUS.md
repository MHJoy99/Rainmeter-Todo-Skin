# AI_STATUS — Todo (Rainmeter skin)

Skin root: `c:\Users\Administrator\Documents\Rainmeter\Skins\Todo`
App host: `@Resources\TodoHost.exe` (invoked as `#@#TodoHost.exe`)
Skin version: 1.4.6 (`Todo.ini` → `[Metadata]`)

## Current Session

- Updated: 2026-08-03
- Focus: Google Tasks API Integration (v1.4.6) — create all-day tasks in Google Tasks, not events.
- Phase: Done — LIVE
- Status: Clicking a todo with an empty target creates a real Google Task. Verified end-to-end 2026-08-03: OAuth client "Todo Skin" (Desktop app) created in project `backup` (`massive-boulder-503110-h8`), Google Tasks API enabled, `gtasks-client.json` (installed type) + `gtasks.secret` present in both skin copies, live task "Make Landing Page for the BDX AI" created in "My Tasks" with no due date (task had no date). Pre-existing all-day dated tasks unaffected. Custom link targets still open normally.

## Session Log

### 2026-08-03 — Google Tasks LIVE via OAuth (v1.4.6) (Done)

- Created Desktop OAuth client **"Todo Skin"** in Cloud project `backup` (`massive-boulder-503110-h8`) via automated browser session (Playwright/CDP on the user's signed-in Chrome). Client ID `321463669656-g9o1tidis6jino730sodljem7rhe3p7n.apps.googleusercontent.com`, type `installed`, redirect `http://localhost` (loopback with arbitrary port accepted — verified with port 8392).
- Enabled **Google Tasks API** on the project.
- Placed `gtasks-client.json` in both `Documents\Rainmeter\Skins\Todo\@Resources` and `C:\Program Files\Rainmeter\Skins\Todo\@Resources`.
- Completed OAuth consent (account `mariyatv1234@gmail.com`); exe saved `gtasks.secret` (client_id/client_secret/access_token/refresh_token/expiry).
- Verified via Tasks API: task "Make Landing Page for the BDX AI" in "My Tasks" with `due: none` (no-date task), refresh-token flow works, dated tasks remain all-day.
- `tasks.json` status now reports "Added to Google Tasks: ..." on success.

### 2026-08-03 — Google Tasks API Auto-Create (v1.4.6) (Done)

- `backend/TodoGoogleTasks.cs`: Implemented Google Tasks API client with OAuth 2.0 loopback flow (`http://127.0.0.1:8392/`, scope `https://www.googleapis.com/auth/tasks`). Handles automatic token refresh, 120s loopback timeout, `invalid_grant` cleanup, and all-day date formatting (`YYYY-MM-DDT00:00:00.000Z`).
- `backend/TodoRules.cs`: `Open()` now delegates empty targets to `TodoApp.TryCreateTask(...)`.
- `scripts/Deploy-Todo.ps1`: Preserves `gtasks-client.json` and `gtasks.secret` during deployment and syncs to both `C:\Program Files\Rainmeter\Skins\Todo` and `C:\Users\Administrator\Documents\Rainmeter\Skins\Todo`.
- `.gitignore`, `Build-ReleasePackages.ps1`, `RainmeterDesktopWidgetsUpdater.ps1`: Added secret exclusions for `gtasks-client.json` and `gtasks.secret`.
- `VERSION` & `Todo.ini`: Bumped version to `1.4.6`. Git tagged `v1.4.6`.
- Verification: Compiled cleanly with `csc.exe`, deployed to both skin targets, verified smoke test status (`"Google Tasks setup needed: gtasks-client.json missing"`).
