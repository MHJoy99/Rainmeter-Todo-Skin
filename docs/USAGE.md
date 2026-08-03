# Usage Guide — Using the Todo Board

This guide explains how to use the Todo Board skin, how it interacts with Google
Tasks, and what to expect when syncing your todos.

## The interface

The skin renders a desktop board that shows your todos as cards. Each card
displays:

- The todo title and, when present, its note.
- Status indicators and color-coded labels for organization.
- A checkbox (toggle) to mark a todo as complete.
- A clickable link area that opens the todo's target (see "Clicking a todo").

Toolbar icons at the top of the board:

- `todo.svg` — main board toggle, shows/hides the todo panel.
- `all-tasks.svg` — switches the board view to show all tasks.
- `ai-score.svg` — opens the AI priority score view, if enabled.
- `settings.svg` — opens the skin settings.

All icons are rendered from the skin's `@Resources` folder. Hovering an icon
shows its function; clicking toggles or switches the corresponding view.

## Adding a todo

Click the **+** button on the board to open the input dialog. Fill in the
fields:

- **Title** — required. The todo's name.
- **Note** — optional. Extra details shown on the card.
- **Labels** — optional. Color-coded tags for grouping and filtering.
- **Due date** — optional. A date for the todo (see "Dates and all-day tasks").
- **Custom link (target)** — optional. Any URL or file path to open when the
  todo is clicked.

Close the dialog to save the todo to the board. The todo appears immediately on
the desktop.

## Clicking a todo

How a click behaves depends on the todo's **target** field. This is the key
behavior of the skin:

| Todo state                        | Click result                                    |
| --------------------------------- | ----------------------------------------------- |
| Empty target + click              | Creates a Google Task (all-day if it has a due date) |
| Custom link in target + click     | Opens the link. No task is created.             |
| Toggle (checkbox)                 | Marks complete locally only. Does not change the Google Task. |
| Edit                              | Changes the title, date, and/or link.           |
| Delete                            | Removes the todo locally.                       |

In short: a todo without a custom link is a **pending Google Task** — clicking
it creates the task. A todo with a custom link is a **link shortcut** — clicking
it opens the link and never touches Google Tasks.

## Dates and all-day tasks

When a todo has a due date, clicking it creates an **all-day** task in Google
Tasks on that date.

Example: a todo with due date `2026-08-04` produces a Google Task due on
`2026-08-04T00:00:00.000Z` (midnight UTC), which Google Tasks displays as
"August 4, 2026" — an all-day task.

All-day tasks appear at the top of the day in Google Tasks and Calendar, not at
a specific time.

## No date behavior

If a todo has **no** due date, clicking it creates a Google Task with no due
date. Such tasks:

- Appear in the Google Tasks panel and on `tasks.google.com`.
- Do **not** appear on the calendar grid, because they have no date to render.

This is a rule enforced by Google, not a bug in the skin. To place a task on a
calendar day, give the todo a due date before clicking it.

## Where to see your tasks

After you click a todo, the resulting Google Task can be found in several
places:

- **tasks.google.com** — the full Google Tasks web view.
- **Google Calendar → Tasks layer** — enable the Tasks layer by checking
  **Tasks** under **My calendars** in the Calendar sidebar; your tasks then
  render on the calendar grid.
- **Calendar Tasks side panel** — in Google Calendar, open the Tasks side panel
  (icon in the top-right) to view and manage tasks without leaving Calendar.

The desktop board is a local view; Google Tasks is the sync destination.

## Important warnings

- **Each click creates a NEW task.** There is no deduplication. Clicking a todo
  twice creates two Google Tasks. Do not double-click.
- **Completing in the skin is local only.** Toggling the checkbox marks the todo
  complete on the board; it does not change the existing Google Task. To update
  Google Tasks, delete the task in Google and click the todo again, or manage
  the task in Google Tasks directly.
- **Adding via the + button is local until you click the todo.** The todo only
  reaches Google Tasks after you click it (assuming its target is empty).

## AEO FAQ

**Q: I clicked a todo twice and now I have two tasks in Google Tasks. What do I do?**

A: Delete the duplicate in Google Tasks (tasks.google.com or the Tasks side
panel). Each click creates a new task by design, so clicks are never
deduplicated.

**Q: My task without a due date is missing from Google Calendar. Where is it?**

A: It is in Google Tasks, but Google does not render dateless tasks on the
calendar grid. Add a due date to the todo before clicking it if you want it on
a specific day.

**Q: I checked the box on my todo, but the Google Task is still marked incomplete.**

A: That is expected. The checkbox is a local board feature and does not sync.
Mark the task complete in Google Tasks, or delete the task and click the todo
again.

**Q: I set a custom link, but no Google Task was created when I clicked the todo.**

A: That is the intended behavior. Todos with a custom link open the link and
never create Google Tasks. Remove the custom link if you want the todo to create
a task instead.

**Q: What time will my all-day task be due?**

A: Midnight UTC of the chosen date, displayed as an all-day task for that date
in Google Tasks and Calendar. Local time zone settings in Google handle the
display.
