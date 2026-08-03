# Growth Plan — Rainmeter-Todo-Skin

30-day distribution and growth plan for [MHJoy99/Rainmeter-Todo-Skin](https://github.com/MHJoy99/Rainmeter-Todo-Skin), a Rainmeter todo skin synced to Google Tasks (MIT license, v1.4.6).

Goals for day 30: 50+ stars, 20+ forks, 10+ confirmed users with Google Tasks configured, and at least one inbound issue or testimonial from a stranger (not a friend).

---

## 1. Week 1 — Launch

### Day 1 — Setup and announce
- Double-check the repo: README renders correctly, one-click install instructions are tested on a clean machine, FAQ answers the top 5 questions.
- Confirm `topics` are set (see Week 2) and the MIT license file is present.
- Post the release to r/rainmeter: title "I built a Rainmeter skin that syncs with Google Tasks", body with 2-3 screenshots, install link, and a short "what problem it solves" line. Keep it demo-first, not salesy.
- Draft all Week 1 content in one sitting so posting takes 5 minutes per day.

### Day 2 — dev.to article
- Publish "Sync Google Tasks to your desktop with a Rainmeter skin" on dev.to.
- Structure: problem, the skin in 3 screenshots, install steps, how the OAuth loopback flow works (link to repo), what's next.
- Add tags: rainmeter, desktop, productivity, opensource. Add the repo link at the top and bottom.

### Day 3 — X thread
- Post a 6-8 part thread: screenshot of the widget, the "before" (checking the phone), the "after" (desktop widget), the OAuth loopback trick, install link, ask for feedback.
- Pin the thread. Reply to every reply within 12 hours.

### Day 4 — LinkedIn
- Short post (no emoji, no hashtag spam): "I open-source a Google Tasks widget for Rainmeter. It uses the OAuth loopback flow so no server or token storage is needed." Link the repo.
- Tag nothing. Keep it under 100 words. Comment on 2-3 productivity posts to get visibility.

### Day 5 — Facebook group
- Find the "Rainmeter" Facebook group and any Windows customization groups.
- Post the same screenshot + link with a one-line description. Read the group rules first; some groups require approvals or disallow self-promo.
- If rejected, skip and move on. Never repost after a rejection.

### Day 6 — Friends and first testers
- Ask 3 real friends who use Rainmeter (or Windows desktops) to: install the skin, sync Google Tasks, report anything broken, and star the repo if it works.
- Collect their feedback in a single list and fix the top issue over the weekend.
- Ask them for one honest line of feedback to use as a testimonial (only if they volunteer).

### Day 7 — Review and respond
- Audit the week: reply to every comment, issue, and DM received — target under 12 hours response time all week.
- Note any screenshots other users posted; save them in `docs/promos/user-screenshots/` (with permission) for Week 3.
- Write down which channels drove the most visits. This decides where Week 2 effort goes.

---

## 2. Week 2 — Visibility

### Rainmeter official forum
- Create a thread in the "Share your skins" section at rainmeter.net/forums.
- Title: "Google Tasks todo widget — syncs your tasks to the desktop".
- Content: 2-3 screenshots, install instructions, note that it uses the OAuth loopback flow (no API keys to paste), MIT license.
- Update the thread when new versions release. Answer every reply within 12 hours.

### Pinterest / Imgur album
- Create an Imgur album with 5-6 screenshots (different accent colors, a before/after, a config dialog shot).
- Pinterest: one pin per screenshot style with the repo URL in the description. Pin boards: "desktop customization", "productivity setup".
- Use descriptive filenames (e.g. `rainmeter-google-tasks-widget-dark.png`) — they count as searchable text.

### GitHub topic expansion
- Add these topics to the repo: `rainmeter`, `google-tasks`, `todo`, `widget`, `desktop`, `opensource`, `oauth`, `windows`, `productivity`, `task-management`.
- Ensure `About` section has a one-line description, website field pointing to the README, and the topics are public.

### Helpful comments, not spam
- Find the 5 most active Rainmeter threads this week (forums and r/rainmeter) and write genuinely useful comments: answer someone's question, share a setup tip, mention the skin only where it is directly relevant.
- Rule: 1 helpful comment for every 1 mention of the repo. Zero mention-only comments.

---

## 3. Week 3 — Content

### Second dev.to post (the technical one)
- Title: "How I built a Google Tasks widget with Rainmeter — OAuth loopback flow explained".
- Outline:
  1. Why Google Tasks has no official desktop widget.
  2. The problem: OAuth for a desktop app without a server.
  3. What the loopback flow is (redirect to `http://localhost:port`).
  4. How the skin catches the redirect with a local listener and exchanges the code.
  5. Token storage on disk and refresh handling.
  6. Where Rainmeter Lua comes in (rendering the list, polling for changes).
  7. Link to the full source.
- Cross-link this post from the first dev.to post and from the README.

### Short video demo
- Idea: 90-second screen recording, no voice needed (captions only).
- Script outline:
  - 0-10s: desktop without the widget; open the phone, show the tasks app.
  - 10-25s: skin installed, widget appears, tasks listed.
  - 25-45s: add a task on the phone; widget refreshes within seconds.
  - 45-60s: check a task on the widget; it disappears from the list.
  - 60-75s: open the config dialog, switch accent color.
  - 75-90s: end card with repo URL.
- Post to YouTube (unlisted first, then public) and embed in the dev.to posts.

### README testimonials
- Add a "What users say" section at the bottom of the README: 2-3 short quotes from real users (the 3 friends from Week 1, plus any stranger who commented).
- Every quote must be a real person who actually used the skin, with their handle or GitHub name. No invented testimonials, ever.

---

## 4. Week 4 — Consolidation

### Issues and feature requests
- Open the GitHub Discussions tab (or pin an issue) titled "What should the next version do?".
- List candidate features from user feedback: multiple task lists, due-date badges, tray icon, reminder popups.
- Reply to every issue and discussion within 12 hours. Close stale ones politely.

### Roadmap section
- Add a `ROADMAP.md` with three columns: now (v1.5), next (v1.6), later (v2.0).
- Link it from the README. Update it publicly when something ships.

### Measure with GitHub Insights
- Under Insights > Traffic, record at week 4: unique visitors, total clones, top referrers.
- Under Insights > Forks and stars, note which days grew and map them to the channel posted that day.
- Compare with the Week 1 channel notes to find what actually works.

### Top 3 referrer sources
- Identify the top 3 referrers (likely: dev.to, reddit, Rainmeter forums — or wherever the data points).
- Double down on those three in month 2: post a monthly update, answer new threads in those places, and refresh the old posts with new screenshots.

---

## 5. Metrics to Track

| Metric | Where to find it | Target by day 30 |
|---|---|---|
| Stars | Repo main page | 50+ |
| Forks | Repo main page | 20+ |
| Clones (weekly) | Insights > Traffic | 30+/week |
| Unique visitors | Insights > Traffic | 100+/week |
| Top referrers | Insights > Traffic | Identify top 3 |
| Issues opened | Issues tab | 3+ real (non-friend) |
| Pull requests | Pull requests tab | 1+ |
| Conversions (users with Google configured) | Poll in Discussions + testimonials | 10+ |
| Response time to comments | Manual log | Under 12h |

Track weekly in a simple table in `docs/promos/metrics-log.md` (append-only, one row per week).

---

## 6. Backlink Ideas

- **dev.to**: two articles (Week 1 and Week 3 posts), both linking the repo.
- **Reddit**: r/rainmeter release post plus any comment where the repo is a genuine answer.
- **Stack Overflow**: search for "how to sync rainmeter todo google tasks" (and similar). If a question exists or one is posted, answer it properly with a working setup, then link the repo as the full solution. Do not link the repo in answers that do not ask about this problem.
- **Rainmeter forums**: the "Share your skins" thread, updated with each release.
- **Medium**: republish the dev.to article (with a canonical link back to dev.to) under a "Productivity / Open source" publication that accepts community posts.
- **Personal blog**: a writeup of the OAuth loopback learning journey, linking the repo and the dev.to posts.
- **YouTube tutorial**: the 90-second demo video with the repo link in the description, plus (later) a 10-minute install-and-configure tutorial.

---

## 7. Do NOT Do

- Do not buy stars, followers, or engagement from any service. It poisons Insights data and can get the repo flagged.
- Do not spam subreddits: one post per relevant subreddit, and never the same content twice. Repeat posts get you banned and the repo downvoted.
- Do not invent testimonials or reviews. Every quote in the README must come from a real user who actually configured the skin.
- Do not keyword-stuff the README or article titles. One natural mention of "Google Tasks Rainmeter" per headline is enough; search engines punish stuffed text.
- Do not post links to the repo in threads where it is off-topic (e.g. a music-skin thread) just for views.
- Do not use emojis in posts or commit messages; they read as promo noise in technical communities.
- Do not ask for "upvotes" or "stars please" anywhere. Show the work; let people star it.
- Do not cross-post the identical text to multiple channels in one day; adapt each post to the platform.

---

## 8. Star-Seeding Plan (ethical)

- Recruit 3-5 people who actually use Rainmeter and actually want a Google Tasks widget: the 3 friends from Week 1, plus 1-2 people from the r/rainmeter post who commented positively and confirmed they installed it.
- Ask them to install, sync, and use the skin for at least a few days before asking for anything else.
- Only then ask: "If it worked, a star helps others find it — and one line about what it did for you would help the README."
- Collect their honest lines into the README "What users say" section with their handle or name.
- Never pay, trade, or arrange reciprocal stars. If a recruited user reports a bug instead of praise, fix the bug and thank them publicly.

---

## Monthly cadence after day 30

- One release per month with a changelog.
- One comment/ping per month in the top 3 referrer channels.
- Update the roadmap and testimonials after each release.
- Review Insights monthly; drop channels with zero clicks after two months.
