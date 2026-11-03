# Issue tracker: Local Markdown

Issues and specs for this repo live as Markdown files in `.scratch/`.

## Conventions

- Use one directory per feature: `.scratch/<feature-slug>/`.
- Store the feature spec at `.scratch/<feature-slug>/spec.md`.
- Store each implementation issue at `.scratch/<feature-slug>/<ticket-slug>.md`.
- Record triage state in a `Status:` line near the top of each issue file. See `triage-labels.md` for the allowed values.
- Append comments and conversation history under a `## Comments` heading at the bottom of the issue file.

## Publishing an issue

When a skill says to publish to the issue tracker, create a Markdown file at `.scratch/<feature-slug>/<ticket-slug>.md`, creating the feature directory when needed.

## Fetching an issue

When a skill says to fetch the relevant ticket, read the referenced Markdown file. The user will normally provide its path.
