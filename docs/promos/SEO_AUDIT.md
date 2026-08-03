# SEO / AEO Audit — MHJoy99/Rainmeter-Todo-Skin

Audit date: 2026-08-03. Scope: public repo state, README.md, docs/FAQ.md (17 Q&A), docs/AEO.md (keyword groups + JSON-LD), repo topics (10), releases/CHANGELOG, images.

## 1. Scorecard — On-Page Repo Signals

| Signal | Score (0-5) | Note |
|---|---|---|
| Repo title (visible name) | 3 | "Rainmeter-Todo-Skin" — keyword-strong but generic slug; could be "rainmeter-todo-skin-google-tasks" for exact-match value. |
| Description (one-liner) | 4 | "Rainmeter todo skin synced to Google Tasks" — contains 2 of 3 primary keywords; add "desktop widget" or "Windows". |
| Topics | 4 | 10 relevant topics set (rainmeter, google-tasks, desktop-widget, todo-list, windows, oauth2, google-api, todo, productivity, skin). Missing high-volume: `rainmeter-skin`, `desktop-widget` is fine, add `google-calendar`. 10 of 20 slots used. |
| README length | 4 | 124 lines, dense, keyword-rich first paragraph. Short for full SERP coverage; ~250-350 lines ideal. |
| Headings (H1/H2/H3) | 4 | H1 contains "Google Tasks Desktop Widget"; 10 H3 FAQ headings are exact user questions. H1 lacks "Rainmeter Todo Skin" as the literal first tokens (repo name covers it). |
| FAQ in README | 4 | 10 Q&A with question-phrased H3s; 17 more in docs/FAQ.md. |
| Images / screenshots | 0 | README has zero images. No img/screenshot.png or img/banner.png exists in the repo. Single biggest gap. |
| Alt text | 0 | No images, therefore no alt text. AEO.md mentions it but nothing is implemented. |
| Releases | 3 | Release notes for 1.4.6 exist (RELEASE_NOTES_v1.4.6.md, CHANGELOG.md) but release titles are not keyword-optimized ("v1.4.6" style). |
| Meta comment / description | 5 | Meta description comment present in README head, ~2 keywords per sentence, accurate. |
| JSON-LD structured data | 1 | Written in docs/AEO.md but not embedded anywhere (no site, no HTML, no GH Pages). Also stale: says version 1.2.0, repo is 1.4.6. |
| Overall | 2.9 | Strong text foundation; missing visuals, structured data deployment, and freshness. |

## 2. Keyword Gap Analysis

| Primary keyword | Appears where now | Missing |
|---|---|---|
| rainmeter todo skin | Repo title slug, README H1 (partial), meta comment, README body | Exact phrase in repo description; alt text; GitHub Pages title tag; release titles |
| google tasks rainmeter widget | README meta comment, docs/AEO.md | README H1/body near top; image alt text; blog post titles |
| desktop todo board windows | README meta comment, README body ("desktop todo board widget") | Not in H1 or description; no blog/landing page targeting it |
| todo widget google calendar | docs/AEO.md keyword table only | Never in README body; FAQ "Does it create events" is adjacent — add explicit "Google Calendar Tasks layer" phrasing near top |
| rainmeter google tasks oauth | README body (OAuth 2.0 section), docs/AEO.md | No dedicated question-phrased FAQ; no blog post "how to authorize" |

Cross-cutting gaps:
- Image alt text: zero images exist; add screenshot.png + banner.png with keyword-rich alts.
- URL slugs: only one slug exists (repo URL). Docs are GitHub-rendered with auto-slugs; put primary keyword in doc filenames (e.g. docs/google-tasks-desktop-widget.md).
- Page titles: no blog posts, no GitHub Pages site — page titles exist nowhere. First posts should use the keywords above verbatim.
- GitHub search tokens: ensure code has the keyword in README only — already the case; avoid keyword stuffing in code comments (AEO.md correctly notes this).

## 3. AEO Gap Analysis

Can AI engines answer "can rainmeter sync to google tasks" from this repo today? Yes, partially: the README FAQ "Does this create Google Calendar events?" and the intro paragraph state the sync model, and docs/FAQ.md has "Does it sync both ways?" (one-way). Missing: a single declarative sentence "Yes. Todo Board syncs Rainmeter with Google Tasks (one-way, local to Google)." that is directly quotable — the answer is currently only implied across two docs.

| Prompt | What answers it today | Gap / what to add |
|---|---|---|
| "can rainmeter sync to google tasks" | README TL;DR + FAQ.md Q9 "Does it sync both ways?" | Add "Yes" verbatim sentence in README TL;DR; currently the word "Yes" never precedes the sync claim in README |
| "is there a rainmeter widget for google tasks" | README H1 + intro | Add explicit "Yes, there is" phrasing; an image with alt "rainmeter google tasks widget" strengthens it |
| "how to put google tasks on windows desktop" | README Installation steps 1-7, GOOGLE_TASKS_SETUP.md | Works well; add numbered FAQ entry with the exact question as an H3 |
| "does the rainmeter todo skin work offline" | README FAQ "Does it work offline?" | Quotable as-is; move the answer above the fold if README is restructured |
| "how do i authorize rainmeter with google oauth" | README Google Account Setup + GOOGLE_TASKS_SETUP.md steps | Add a README H3 FAQ "How do I authorize the skin?" with the loopback URL in one sentence |

What to add for full AEO coverage:
- One quotable "Yes" sentence at the top of README.
- Deploy JSON-LD (fix version to 1.4.6, keep FAQPage entities in sync with the 10 README FAQs).
- Keep FAQ headings byte-stable across releases.

## 4. Image / Visual SEO

Current state: img/screenshot.png and img/banner.png do NOT exist. Nothing to optimize yet — create both.

Recommended alt texts:
- img/screenshot.png: "Rainmeter todo widget on Windows 11 showing Google Tasks list with all-day tasks" — primary keyword "rainmeter todo widget" front-loaded.
- img/banner.png: "Rainmeter Google Tasks desktop widget banner showing the todo board skin on a Windows desktop" — includes "Google Tasks desktop" phrase.

OG image tips:
- No og:image exists (repo has no HTML surface). When a GitHub Pages site or dev.to/blog mirror is added, use img/banner.png as og:image.
- Size banner.png at 1280x640 (1.91:1) per Open Graph spec; keep the todo board + Google Tasks logo visible in the center 1120x560 safe zone (Twitter/OG crop).
- Target under 300 KB (PNG or compressed JPG) for fast crawl.
- Put the alt text beside the markdown image: ![Rainmeter todo widget on Windows 11 showing Google Tasks list with all-day tasks](img/screenshot.png) — markdown alts are the only alt signal GitHub exposes.

## 5. Ranking Levers for a 0-Star Repo

Reality check: GitHub search ranks by stars/engagement and recency; Google ranks the repo page via its own signals (description, README, backlinks, freshness). A 0-star repo will not rank on page 1 for "rainmeter todo skin" for months. Timeline: GitHub search visibility in 2-6 weeks (freshness boost on exact-name queries), Google page 1 for long-tail questions in 3-6 months with backlinks, "rainmeter todo skin" head term in 6-12+ months.

What matters most (ranked): 1) backlinks with keyword anchor text (the only durable authority signal), 2) stars/engagement (GitHub search + social proof), 3) content freshness (commit activity, release cadence), 4) mentions in Reddit/discord/forums (Google entity association), 5) image indexation via alt text.

10 concrete actions, ranked by impact:
1. Create img/screenshot.png + img/banner.png with keyword-rich alts and add both to README top. (Highest effort-to-impact; unblocks image search and AEO.)
2. Post a how-to tutorial on dev.to titled "Put Google Tasks on your Windows desktop with Rainmeter" linking the repo (canonical backlink, keyword anchor).
3. Answer real questions on r/rainmeter and r/desktops linking the repo with "Google Tasks Rainmeter widget" anchor text.
4. Publish the first GitHub release (v1.4.6) with a keyword-rich title: "v1.4.6 — Google Tasks Rainmeter skin with all-day task support".
5. Deploy docs to GitHub Pages so JSON-LD (fixed to 1.4.6) actually renders; title tag = "Rainmeter Todo Skin with Google Tasks Sync".
6. Set the repo description to "Rainmeter todo skin synced to Google Tasks — desktop todo widget for Windows 10/11" (adds desktop todo widget).
7. Ask for stars: mention in README a one-line "If this saved you time, star the repo" + post to r/Rainmeter showcase thread.
8. Add 2 more topics (rainmeter-skin, google-calendar) to max out relevant topic coverage.
9. Get one mention per month on a productivity blog/roundup ("5 ways to put Google Tasks on your desktop").
10. File a Product Hunt / AlternativeTo listing (AlternativeTo is a real backlink domain and drives "todo widget windows" long-tail).

## 6. SERP / AEO Ready Checklist

Repo:
- [ ] Description contains "rainmeter", "google tasks", "desktop", "widget", "windows"
- [ ] 10-20 relevant topics set (currently 10; add rainmeter-skin, google-calendar)
- [ ] README H1 starts with primary keyword phrase
- [ ] README first paragraph is a quotable one-sentence summary with "Yes"-style answer for the top question
- [ ] README has img/screenshot.png near the top with keyword alt text
- [ ] README has img/banner.png with keyword alt text
- [ ] 10 FAQ H3s phrased as exact user questions (done — keep byte-stable)
- [ ] Every FAQ answer is self-contained (question restated in the answer)
- [ ] Meta description comment updated with current version and top 3 keywords
- [ ] Changelog/release notes use keyword-bearing titles per release

Docs:
- [ ] docs/FAQ.md headings stable across releases (17 Q&A — do not reword)
- [ ] docs/AEO.md JSON-LD deployed (not just example) with version 1.4.6
- [ ] FAQPage JSON-LD entities match README FAQ questions 1:1
- [ ] docs filenames contain keywords (e.g. google-tasks-setup.md already does)
- [ ] Setup doc includes exact OAuth question/answer pair for AI engines
- [ ] One doc answers "can rainmeter sync to google tasks" with an explicit Yes sentence

External:
- [ ] GitHub Pages site live with og:image = banner.png and keyword title tag
- [ ] First GitHub release published with keyword-rich title
- [ ] dev.to tutorial backlink with "Google Tasks Rainmeter" anchor
- [ ] r/rainmeter + r/desktops mention/backlink posted
- [ ] AlternativeTo / Product Hunt listing exists
- [ ] One freshness event per month (commit, release, or post)

## 7. 90-Day Refresh Plan

- Month 1: add screenshots + alt text, publish v1.4.6 release with keyword title, deploy Pages + JSON-LD, post dev.to tutorial. Commit on a schedule (e.g. weekly small docs commits) so the repo shows activity.
- Month 2: monthly README tweak (reword the first sentence with one new keyword phrase, e.g. "desktop todo board windows"), update meta comment, add month-2 screenshots (new skin states, dark mode if any), post the r/rainmeter how-to, answer one Google/Bing index-check query.
- Month 3: changelog entry for any bugfix + new screenshot (OAuth screen), publish "how to authorize rainmeter with google oauth" blog post, verify the 5 AEO prompts still return the repo (test in ChatGPT/Perplexity/Google AI Overview), fix any stale quoted answers.
- Ongoing cadence: commit or release at least once every 14 days (freshness signal for both GitHub and Google), re-crawl check quarterly, keep the 10 README FAQ headings byte-stable forever, update JSON-LD version on every release.

---

Summary: Strong text foundation (README, 27 total FAQs, AEO doc) but zero images/alt text, no deployed JSON-LD, no releases, no backlinks, and stale version data in docs/AEO.md — fix visuals, structured data, and one dev.to backlink first.
