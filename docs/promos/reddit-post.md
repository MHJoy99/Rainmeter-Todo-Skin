# Reddit Post: r/rainmeter Promo — Rainmeter Todo Skin

Draft for sharing MHJoy99/Rainmeter-Todo-Skin on r/rainmeter.
This is ready-to-paste promo copy. The exact path to this draft is:
`docs/promos/reddit-post.md` in the repo.

---

## 1. Title Options

Pick one, paste it as the post title. No clickbait, all true.

1. **Turn your Rainmeter desktop into a Google Tasks widget — todos you click become real Google Tasks via OAuth 2.0**

2. **I built a Rainmeter skin that syncs with Google Tasks: click a todo on your desktop and a real task is created in Google**

3. **Free, open-source Rainmeter skin that turns your desktop into a Google Tasks board — click to create tasks, dates become all-day events**

---

## 2. Post Body

Ready to paste (Reddit markdown).

---

I built a Rainmeter skin that turns your desktop into a Google Tasks widget. You
can see it in the screenshot below — the list lives on your wallpaper, and
interacting with it is the whole point.

Clicking a todo with no custom link attached doesn't just mark it done — it
creates a REAL Google Task through OAuth 2.0, using your own Google account. If
the todo has a date, it becomes an all-day task with that due date. Undated
todos get no due date at all. Todos that do have a custom link still open
normally, exactly as you'd expect.

Everything is stored in a local, human-readable `tasks.json` file, so the skin
keeps working even when your browser or Google API session is unreachable. No
third-party cloud in the middle — your tasks stay on your machine.

A few things I like about it:

- **Real Google Tasks sync** — OAuth 2.0 with your own account, no app
  registration required to get started
- **All-day task support** — dated todos become all-day tasks with due dates,
  undated ones stay due-less
- **Custom links still work** — linked todos open normally; unlinked ones create
  tasks
- **Local-first storage** — plain `tasks.json`, fully editable by hand
- **Free and open source** — MIT license, Windows 10/11, Rainmeter 4.4+

The repo is here: **https://github.com/MHJoy99/Rainmeter-Todo-Skin** — the
screenshot is in the repo at `img/screenshot.png`.

It's completely free, MIT-licensed, and open source. I'd love feedback, feature
requests, or ideas for the next release (currently at v1.4.6).

![Screenshot](img/screenshot.png)

---

*Word count: ~200 words. Trim the intro line if the subreddit requires shorter
posts; everything else is the core pitch.*

---

## 3. Suggested Flair

Use the **"Showcase"** flair if available. It signals "I made this" without
reading as spam.

- First choice: `Showcase`
- Second choice: `OC` (Original Content)
- Third choice: `Skin` / `Suite` if the subreddit uses skin-specific flairs

Check the subreddit's flair list on the day of posting — flairs change
occasionally.

---

## 4. Comment Reply Plan

Likely questions and short, warm replies you can paste as replies.

**Q1: Is it free?**
A1: Yes, completely. MIT license — free to use, modify, and share. It's a
hobby project, so I keep it free on purpose.

**Q2: How do I set up Google?**
A2: The setup guide in the README walks you through creating a Google Cloud
OAuth client ID (Desktop app type). It takes about 5 minutes — you only do it
once. The skin handles the token flow from there.

**Q3: Does it work offline?**
A3: Yes. All todos live in a local `tasks.json`, so the list always displays
and editing works offline. Only clicking a todo to create a Google Task needs a
connection. If the API call fails, the todo stays as-is and you can retry.

**Q4: Will it create calendar events?**
A4: No — it creates Google **Tasks** (to-dos), not calendar events. Dated todos
become all-day *tasks* with a due date, which show up in Google's Tasks side
panel in Calendar. They won't appear on your calendar grid.

**Q5: Can I use it with my own OAuth / app credentials?**
A5: Yes. The skin uses your own Google Cloud OAuth client ID — there's no shared
API key or central server. If you already have a client ID you use for other
Google apps, you can plug it in directly.

---

## 5. Best Posting Time + Self-Promo Ratio

**Best posting time (US/EU):**

- US: Tuesday–Thursday, 12:00–14:00 Eastern Time (9:00–11:00 Pacific).
  r/rainmeter activity peaks in the late-morning-to-early-afternoon US window.
- EU: Tuesday–Thursday, 19:00–21:00 CET (matches the US morning window overlap
  around 20:00–22:00 CET).

Aim for the **US morning / EU evening overlap** (roughly 20:00–22:00 CET =
14:00–16:00 Eastern). That catches EU readers after work and US readers at
lunch.

Avoid Monday mornings and weekends — those get lower engagement for
showcase-type posts.

**Self-promo ratio tip:**

- Reddit's official guideline is roughly 1 in 10 posts should be
  self-promotion. Before posting, make sure the account has organic comments
  on other people's posts in r/rainmeter.
- Reply to everyone who comments within the first 2–3 hours — engagement in the
  first hour is the biggest factor in the algorithm surfacing the post.
- Never use link shorteners or post the same title twice; use the exact repo
  URL. If it gets removed, message the mods with the repo link and "free MIT
  project" explanation — showcase posts like this are usually fine.
