# Search & AI Engine Optimization (AEO) — Rainmeter Todo Skin

This guide describes how to make the repository rank in search engines and be directly quotable by answer engines (Google AI Overviews, ChatGPT, Perplexity, Bing Copilot). The goal is that a user asking "how do I show Google Tasks on my desktop" gets an answer that quotes the README or FAQ.

## 1. Keyword Strategy

### Group A — Search keywords (users typing into Google/Bing)

| Keyword | Intent |
|---|---|
| rainmeter todo skin | Product discovery |
| google tasks rainmeter | Product discovery |
| desktop todo widget windows | Feature/benefit search |
| todo widget sync google calendar | Feature search |
| rainmeter google tasks integration | Feature search |
| all-day task creation | Feature search |
| google tasks widget windows 10 | Platform-specific search |
| rainmeter skin todo list | Product discovery |
| free desktop todo widget | Price/benefit search |
| windows 11 todo widget | Platform-specific search |
| google tasks on desktop | Problem search |
| rainmeter google calendar skin | Related product search |

### Group B — Question keywords (answer engines quote these directly)

- how to show google tasks in rainmeter
- how to sync rainmeter todo to google tasks
- does google tasks show in google calendar
- how to add a todo widget in windows
- how to use google tasks without the app
- is google tasks free
- does rainmeter todo widget work offline
- how to create all-day tasks in google tasks
- how to authorize a rainmeter skin with google oauth
- does a desktop todo widget sync both ways

## 2. Where Keywords Must Appear

- **README title (H1):** include the primary phrase "Rainmeter Todo Skin with Google Tasks Sync". The first sentence should repeat the core keywords ("A free, open-source desktop todo widget for Windows 10/11 that syncs with Google Tasks").
- **Meta description:** the README's first paragraph is often used as the meta description by GitHub and search engines. Write one sentence that contains "todo widget", "Google Tasks", "Windows", and "sync".
- **FAQ questions (H2/H3):** phrase every FAQ heading as the exact question users ask, because answer engines match question text to headings. See FAQ.md, which already follows this pattern.
- **Alt text of images:** every screenshot (`docs/screenshot-main.png`, `docs/screenshot-oauth.png`) must have descriptive alt text in the README markdown, e.g. "Rainmeter todo widget on Windows 11 showing the Google Tasks list" or "Google OAuth authorization screen for the rainmeter todo skin". Alt text is indexed and is quoted by answer engines when they need to describe the product.
- **Release notes:** each release title should contain a keyword phrase, e.g. "v1.2 — all-day task creation support".
- **Code comments and variable names** are not indexed; do not waste keywords there.

## 3. AEO-Optimized Q&A Blocks

Use these five Q&A blocks in the README (or a copy in FAQ.md). Each is written so an answer engine can quote it verbatim. Embed the JSON-LD markup from section 4 into the README's HTML section or the repository's site page.

**Q1. How do I show Google Tasks on my Rainmeter desktop?**
Install Rainmeter, copy the skin folder into `Documents\Rainmeter\Skins\`, load the skin, and run the OAuth helper once to sign in with your Google account. Your tasks then appear on the desktop and refresh automatically.

**Q2. How do I sync Rainmeter to Google Tasks?**
Sync is one-way: local to Google. Tasks you create in the widget are added to your Google Tasks account immediately. Changes made in the Google Tasks app are not pulled back into the widget.

**Q3. Does Google Tasks show in Google Calendar?**
Yes, if you enable the Google Tasks sidebar in Google Calendar. Tasks with a due date appear as all-day task items on the calendar grid. Tasks without a due date appear only in the Tasks sidebar.

**Q4. How do I create an all-day task?**
Add a task with a due date but no time. The widget sends it to Google Tasks with the due date set, and Google Calendar displays it as an all-day item on that date.

**Q5. Does the widget work offline?**
Viewing tasks works offline because the task list is cached locally. Creating tasks requires an internet connection, because tasks are pushed to Google's servers in real time.

## 4. Schema.org JSON-LD Example

Embed this into the repository page (e.g. a GitHub Pages site) so rich results and answer engines can parse it:

```json
{
  "@context": "https://schema.org",
  "@graph": [
    {
      "@type": "SoftwareApplication",
      "name": "Rainmeter Todo Skin with Google Tasks Sync",
      "operatingSystem": "Windows 10, Windows 11",
      "applicationCategory": "UtilityApplication",
      "softwareVersion": "1.2.0",
      "license": "MIT",
      "description": "A free, open-source desktop todo widget for Windows that syncs one-way with Google Tasks.",
      "offers": { "@type": "Offer", "price": "0", "priceCurrency": "USD" }
    },
    {
      "@type": "FAQPage",
      "mainEntity": [
        {
          "@type": "Question",
          "name": "How do I show Google Tasks on my Rainmeter desktop?",
          "acceptedAnswer": {
            "@type": "Answer",
            "text": "Install Rainmeter, copy the skin folder into Documents\\Rainmeter\\Skins\\, load the skin, and run the OAuth helper once to sign in with your Google account."
          }
        },
        {
          "@type": "Question",
          "name": "How do I sync Rainmeter to Google Tasks?",
          "acceptedAnswer": {
            "@type": "Answer",
            "text": "Sync is one-way: local to Google. Tasks created in the widget are added to your Google Tasks account immediately."
          }
        },
        {
          "@type": "Question",
          "name": "Does Google Tasks show in Google Calendar?",
          "acceptedAnswer": {
            "@type": "Answer",
            "text": "Yes, if you enable the Google Tasks sidebar in Google Calendar. Tasks with a due date appear as all-day task items on the calendar grid."
          }
        },
        {
          "@type": "Question",
          "name": "Does the widget work offline?",
          "acceptedAnswer": {
            "@type": "Answer",
            "text": "Viewing tasks works offline because the task list is cached locally. Creating tasks requires an internet connection."
          }
        }
      ]
    }
  ]
}
```

## 5. Ongoing Optimization Tips

- **Release notes:** describe every release in one plain sentence with the feature keyword up front ("v1.2 adds all-day task creation"), then 2–3 bullet details. GitHub indexes release notes; they appear in search results.
- **Repo topics and tags:** set topics on the GitHub repo: `rainmeter`, `google-tasks`, `todo`, `desktop-widget`, `windows`, `rainmeter-skin`, `productivity`. Topics power GitHub search and surface in Google results.
- **External backlinks:** link the repo from r/rainmeter (a how-to post describing installation), r/desktops, dev.to (a tutorial titled "Put Google Tasks on your Windows desktop with Rainmeter"), and short blog posts about productivity widgets. Every backlink uses the phrase "Google Tasks Rainmeter" in the anchor text.
- **Keep FAQ questions stable:** do not reword FAQ headings between releases. Answer engines index the heading text; changing it resets the ranking and breaks quoted answers.
- **Update the meta-description sentence** in the README whenever a major feature ships; answer engines re-crawl and re-index the first paragraph frequently.
- **Add real screenshots with keyword-rich alt text** in every release; images with descriptive alt text can appear in Google image results for "rainmeter todo skin".
- **Monitor:** once a quarter, search the product name and the question keywords, and check which page answer engines quote. Adjust the FAQ wording if the quoted answer is wrong or outdated.
