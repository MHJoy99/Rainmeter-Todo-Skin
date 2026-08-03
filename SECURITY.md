# Security Policy & Privacy

This document describes how the Rainmeter Todo skin handles credentials,
what data leaves your computer, and what to do if a secret is exposed.

## What credentials exist

All credential files live in the skin's `@Resources` folder and are created
locally during setup. None of them are distributed with the skin.

| File | Contents | Purpose |
| --- | --- | --- |
| `gtasks-client.json` | OAuth 2.0 client ID and client secret for the Google Tasks desktop app you register | Identifies your app to Google during OAuth; required to obtain and refresh tokens |
| `gtasks.secret` | Access token and refresh token granted by Google for your account | Authorizes the skin to create/read tasks via the Google Tasks API without asking for your password |
| `caldav.secret` | Legacy CalDAV credentials | Used only by older versions of the skin; kept for migration, otherwise unused |

## Data flow & privacy

The skin sends data to exactly one destination: the Google Tasks API.

- The only outbound requests are Google Tasks API calls made with the OAuth
  token: creating tasks, listing tasks, and refreshing an expired access token.
- The OAuth consent screen shown during setup declares the exact scope granted
  (`https://www.googleapis.com/auth/tasks`). Nothing else is requested.
- `tasks.json`, the local task list, never leaves your machine. It is read and
  written only by the skin on your local disk.
- No telemetry, analytics, or usage reporting is built into the skin. It does
  not contact any server other than Google's API endpoints.
- The refresh token is exchanged directly with Google's token endpoint over
  TLS; it is never sent to any other host.

## Do not share or commit

The following files must never be committed to the repository, uploaded
anywhere, or pasted into issues or chat:

```
@Resources/gtasks-client.json
@Resources/gtasks.secret
@Resources/caldav.secret
```

The `.gitignore` in this repository excludes them so that a `git add .` or
`git push` cannot accidentally include them.

Why this matters:

- A leaked **client secret** lets anyone impersonate your application on Google
  OAuth, including submitting their own scopes and confusing users into
  granting access.
- A leaked **refresh token** grants full access to the Google Tasks of the
  account it belongs to, with no password required and without expiring until
  explicitly revoked.

If either file is exposed:

1. Open https://myaccount.google.com/permissions and revoke access for the
   app, or delete the OAuth client in Google Cloud Console under
   "Credentials" -> "OAuth 2.0 Client IDs".
2. If the client secret itself was leaked, delete the OAuth client and create
   a new one.
3. Delete the old secret files from your machine, regenerate the client JSON,
   and re-run the OAuth setup so a fresh `gtasks.secret` is issued.
4. Rotate the account password if you have any reason to believe the account
   itself was compromised.

## Reporting vulnerabilities

If you believe the skin has a security issue, do not post details in a public
issue thread before it can be addressed.

- Open a GitHub Issue on this repository and prefix the title with
  `[SECURITY]`.
- Include the Rainmeter and skin versions, the steps to reproduce, and as much
  detail as you can share without exposing credentials.

## Safe defaults

- Tokens are stored only on the machine where OAuth setup was run, inside the
  skin's `@Resources` folder. They are never uploaded or mirrored elsewhere.
- The OAuth redirect URI is `http://127.0.0.1:8392/`, a loopback address. The
  authorization code is received by a listener on your own machine and never
  traverses the network.
- The secret files are excluded from releases and skin updates: updating the
  skin will not overwrite or remove your existing credentials.
- No file in this repository, including build artifacts, contains a real
  client ID, secret, or token.

## FAQ

**Is my Google password ever stored?**

No. The skin never sees, stores, or transmits your Google password. OAuth
happens in your browser against Google's own login page, and Google returns a
token instead of a password. The token is stored in `gtasks.secret`.

**Can the skin read other Google data on my machine?**

No. The OAuth scope is limited to Google Tasks, and the skin only ever calls
the Tasks API. It has no access to mail, drive, contacts, or browser sessions,
and it cannot read your cookies or saved passwords.

**Is this safe for corporate machines?**

The skin authenticates with a personal Google account, not a work account.
Some organizations restrict OAuth apps or personal account access. Check your
company's policy before installing, and note that the token grants access to
the Tasks of whichever Google account completes the OAuth flow.

**What if I lose `gtasks-client.json`?**

Re-downloading the file is not possible because the client secret is shown
only once when the OAuth client is created. Generate a new client in Google
Cloud Console, save the new JSON to `@Resources/gtasks-client.json`, delete
`gtasks.secret`, and re-run the OAuth setup to obtain fresh tokens.
