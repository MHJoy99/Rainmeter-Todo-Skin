# Deploying the Rainmeter Desktop Widgets package

This package installs the Todo skin and its companion Calendar skin for
[Rainmeter](https://www.rainmeter.net).

## Layout

```
Todo-Skin-v<version>/
├── manifest.json
├── DEPLOY.md
├── Rainmeter-4.5.26.exe        (optional installer)
├── Updater/
│   └── RainmeterDesktopWidgetsUpdater.ps1
└── Skins/
    ├── Todo/
    │   ├── Todo.ini
    │   └── @Resources/         (TodoHost.exe, CalendarHost.exe, icons, …)
    └── Calendar/
        ├── Calendar.ini
        └── @Resources/
```

## Install

1. Exit Rainmeter if it is running.
2. Copy the `Skins\Todo` and `Skins\Calendar` folders into
   `Documents\Rainmeter\Skins` (or your configured skins directory).
3. Start Rainmeter and load "Todo Board" and "Calendar".

The `Updater` folder plus `manifest.json` allow
`RainmeterDesktopWidgetsUpdater.ps1 -Mode InstallPackage -PackageRoot <root>`
to install or upgrade the skins programmatically (this is how the in-app
"Check for updates" flow works).

## Upgrade

The in-app updater downloads the newest `Todo-Skin-v<version>.zip` from the
GitHub releases of `MHJoy99/Rainmeter-Todo-Skin` and installs it, preserving
user data (`tasks.json`, `ui-scale.txt`, `*.secret`, PaperCache).
