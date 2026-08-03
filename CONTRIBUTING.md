# Contributing to Todo

Thanks for your interest in contributing to this Rainmeter todo skin with Google Tasks sync. Please take a moment to read the guidelines below before opening an issue or a pull request.

## Table of Contents

- [Reporting Bugs](#reporting-bugs)
- [Feature Requests](#feature-requests)
- [Code Contributions](#code-contributions)
- [Docs Contributions](#docs-contributions)
- [Local Dev Notes](#local-dev-notes)

## Reporting Bugs

Before opening a bug report:

1. Search the existing issues to make sure the bug has not already been reported.
2. Make sure you are on the latest release of the skin and Rainmeter.
3. Reproduce the bug with a minimal set of steps, if possible.

When you open the issue, use the bug report template and include:

- **Rainmeter version** (from the About tab in Rainmeter's Manage window).
- **Windows version** (e.g. Windows 11 23H2, build number if known).
- **The contents of `tasks.json`, specifically the `meta.status` field**, and any relevant entries from the log.
- **A screenshot** of the skin showing the problem, and a screenshot of Rainmeter's log (About > Log tab) if the issue produces errors.
- The steps to reproduce, and what you expected to happen instead.

Issues without this information may be closed as incomplete.

## Feature Requests

Feature requests are welcome. When opening one, please:

- Describe the problem you are trying to solve, not just the solution you want.
- Explain how it relates to the existing Google Tasks sync model of the skin.
- Mark the issue with the `enhancement` label if you can.

## Code Contributions

### Getting Started

1. Fork the repository and create a working branch off `main`.
2. Use a descriptive branch name with a prefix, for example:
   - `fix/google-tasks-auth`
   - `feat/due-date-sorting`
   - `docs/seo-headings`
3. Make your changes, keeping commits small and focused.

### Commit Style

- Write commit messages in the imperative mood (e.g. "Fix token refresh on startup", not "Fixes token refresh" or "Fixed token refresh").
- Use a short subject line (under 72 characters) and add a body explaining the why, not just the what.
- Do not mix unrelated changes in a single commit.

### Pull Request Checklist

Before opening a pull request, make sure all of the following are true:

- [ ] No secrets committed: no client secrets, access tokens, refresh tokens, or personal data in code, commits, or commit history.
- [ ] `.gitignore` is respected: no build output, logs, `tasks.json`, or local config files are committed.
- [ ] Tested with Rainmeter: the skin loads without errors and the tested flows (add, complete, sync with Google Tasks) work on a real Rainmeter installation.
- [ ] Code compiles and the C# host app builds without warnings you introduced.
- [ ] Docs are updated if user-facing behavior changed.
- [ ] Commit history is clean and messages follow the style above.

## Docs Contributions

Documentation is part of the product. When editing the README, wiki, or other docs:

- **SEO/AEO**: Use question-form headings for sections that answer a common question (e.g. "How do I sync with Google Tasks?" instead of "Syncing").
- Give a direct answer in the first sentence or two of the section, then follow with details.
- Keep instructions step-by-step and testable; a reader should be able to follow them without prior knowledge of the skin.
- Keep line width reasonable and use plain language.

## Local Dev Notes

- The skin is a compiled C# host application named `TodoHost.exe`, invoked by Rainmeter bangs (e.g. `PluginBang`-style or `[!CommandMeasure]` calls from the skin `.ini` files).
- The compiled binary lives at `@Resources\TodoHost.exe` relative to the skin folder. If you change the C# source, rebuild the project and replace that file, then refresh the skin (`!Refresh` on the skin config) to pick up the change.
- `tasks.json` is the local data file the host app reads and writes; it holds the task list and the `meta` object (including `meta.status`) used for diagnostics.
- Changes that only touch the skin's `.ini` files do not require a rebuild; just refresh the skin.
