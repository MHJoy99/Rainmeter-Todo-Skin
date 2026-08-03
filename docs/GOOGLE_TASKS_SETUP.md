# Google Tasks Setup — Connect the Skin to Your Google Account

This guide explains how to connect the Todo skin to your Google account so your todos sync with Google Tasks. Once connected, clicking a todo in the skin creates a real task in Google Tasks. If the todo has a date, the task is created as an all-day task on that date; if it has no date, the task is created with no due date. The setup takes about five minutes and requires no coding.

## Prerequisites

- A Google account (free, such as a Gmail account).
- A computer with Rainmeter running the Todo skin.
- Nothing else. No developer tools are required; you only need to use your browser.

## Step 1 — Enable the Google Tasks API

1. Open your browser and go to the Google Cloud Console at [console.cloud.google.com](https://console.cloud.google.com).
2. If prompted, sign in with the Google account you want to use for tasks.
3. Accept the terms of service if this is your first visit.
4. Create a project (or select an existing one) using the project picker at the top of the page.
5. In the left sidebar, open **APIs & Services**, then click **Enable APIs and services**.
6. In the search box, type **Google Tasks API**.
7. Click **Google Tasks API** in the results, then click the **Enable** button.

The API is now enabled for your project. You do not need to configure billing for this step.

## Step 2 — Create a Desktop OAuth Client

1. In the same Google Cloud Console, open **APIs & Services**, then click **Credentials**.
2. Click **Create credentials** and choose **OAuth client ID**.
3. If asked to configure the consent screen first, click **Configure consent screen**, choose **External**, and fill in the required app name and email. (An app in "Testing" status is fine.)
4. For **Application type**, select **Desktop app**.
5. Give the client a name, for example `Rainmeter Todo`.
6. Click **Create**.
7. A dialog appears with your **Client ID** and **Client Secret**. The secret is shown only once, so download the credentials now:
   - Click **Download JSON** and save the file to any location you can find again (for example, your Downloads folder).
   - Alternatively, copy the Client ID and Client Secret values and keep them somewhere safe.
8. Close the dialog and click **OK**.

Important: the client secret is displayed only once. If you close the dialog without downloading or saving it, you must create a new OAuth client and repeat this step.

## Step 3 — Where to put the file (the exact places to enter)

Take the JSON file you downloaded and rename it to exactly `gtasks-client.json`. Then copy it into BOTH of the following locations:

```
C:\Users\<YourUserName>\Documents\Rainmeter\Skins\Todo\@Resources\
```

```
C:\Program Files\Rainmeter\Skins\Todo\@Resources\
```

Replace `<YourUserName>` with your actual Windows user name. If either folder does not exist, create it. Copying to both locations is intentional: some Rainmeter installations load skins from the Documents folder, others from the Program Files folder, so the skin looks in both places.

The file must be valid JSON and must look like this (with your real values in place of the placeholders):

```json
{
  "installed": {
    "client_id": "YOUR_CLIENT_ID.apps.googleusercontent.com",
    "project_id": "your-project-id",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "YOUR_CLIENT_SECRET",
    "redirect_uris": ["http://localhost"]
  }
}
```

The important fields are `client_id`, `client_secret`, and `redirect_uris` (which must contain `http://localhost`). The other fields are usually already present in the downloaded file, so in most cases you only need to rename the file and copy it.

## Step 4 — Authorize the skin

1. Make sure the Todo skin is loaded and visible on your desktop.
2. Click any todo item in the skin.
3. Your browser opens and shows a Google sign-in page.
4. Sign in with the Google account you used in Step 1 and Step 2.
5. If you see a warning that the app is unverified, click **Advanced** and then **Continue** to proceed. This is expected because the app is in Testing status.
6. Click **Allow** to grant the skin permission to manage your tasks.
7. You can close the browser tab after authorization. The browser may show a page that cannot be reached; this is normal, because the skin receives the response through a local port instead.

The token is saved automatically to `@Resources\gtasks.secret` in the skin folder. You only need to do this once. From now on, clicking a todo creates it in Google Tasks.

## What happens next

| Todo date | Google Tasks result |
| --- | --- |
| Todo has a date (for example, 2026-08-15) | An all-day task on that date |
| Todo has no date | A task with no due date |

All-day tasks appear on the specified date in Google Tasks, Google Calendar, and the Google Tasks mobile app. Tasks without a date appear in the default task list with no due date.

## FAQ

### The skin uses port 8392 for the redirect, but the JSON file says http://localhost. Is that a problem?

No. Google accepts any loopback port for installed apps. A redirect URI such as `http://localhost:8392` is treated as a valid form of `http://localhost`, so authorization works even though the registered URI does not mention a port.

### The authorization expires after a while. What do I do?

Google tokens expire automatically and the skin refreshes them without any action from you. If authorization ever stops working, delete the `gtasks.secret` file in the `@Resources` folder and repeat Step 4. The file is recreated the next time you authorize.

### Can I use a different Google account than the one I set up?

Yes. You must use the Google account that owns the OAuth client, so repeat Steps 1 through 4 with the other account, then authorize again as described in Step 4.

### Is the Google Tasks API free?

Yes. The Google Tasks API is free to use, and no billing or payment method is required for this setup.

### Are my todos and credentials sent anywhere else?

No. The client secret stays in the `gtasks-client.json` file on your computer, and the token stays in `gtasks.secret`. The skin communicates only with Google's servers and only to create and manage your tasks.

### What if the browser page says "This site can't be reached" after I click Allow?

This is expected behavior. The skin listens on a local port (8392) and the browser redirects to it after authorization. Ignore the browser error, close the tab, and check whether the todo was created in Google Tasks. If it was not, wait a few seconds and click the todo again.
