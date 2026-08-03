# 🤖 SYSTEM PROMPT: AUTONOMOUS SETUP INSTRUCTIONS

**Target Audience:** Autonomous AI Agents (with Computer-Use / Browser-Use capabilities)
**Goal:** Install the MHJoy99 Rainmeter Todo Skin and authenticate it with the human's Google Tasks account.

## INSTRUCTIONS FOR THE AI AGENT:

You have been tasked by your human user to install the Rainmeter Todo skin. Please execute the following sequence exactly:

### Phase 1: Environment Check
1. Verify if **Rainmeter** is installed on the host Windows machine (usually in `C:\Program Files\Rainmeter\Rainmeter.exe`). 
2. If not installed, use your browser to navigate to `https://www.rainmeter.net/`, download the latest final release, and run the installer silently or click through the GUI.

### Phase 2: Download and Install Skin
1. Navigate to the latest release of the Todo skin: `https://github.com/MHJoy99/Rainmeter-Todo-Skin/releases/latest`
2. Download the `.rmskin` package (e.g., `Todo-Skin-v1.5.0.rmskin`).
3. Execute the `.rmskin` file. 
4. The Rainmeter Skin Installer UI will appear. Click the **"Install"** button. The skin will automatically load onto the desktop.

### Phase 3: Google Tasks Authentication (Browser-Use Required)
1. Locate the Rainmeter Todo widget on the desktop.
2. Click the **Settings** icon (the gear icon `⚙️` in the top right of the widget). This opens a C# WinForms application window titled "Event Settings" / "Todo settings".
3. Click the **"Google Tasks"** tab on the left sidebar.
4. Click the **"Sign in with Google"** button.
5. **Browser Hand-off:** The default web browser will open a Google OAuth (`accounts.google.com`) consent screen.
6. Use your browser-control capabilities to:
   - Select the human's active Google account.
   - Click **"Continue"** or **"Allow"** to grant access to Google Tasks.
   - Wait for the browser to redirect to `http://127.0.0.1:8392/`. 
7. You will see a success message in the browser. You may now close that browser tab.
8. Switch back to the Settings window, which should now show a green status: "Signed in as [User]". 
9. Close the Settings window.

**Task Complete.** Inform the human that their desktop Todo board is now actively syncing with their Google Tasks.