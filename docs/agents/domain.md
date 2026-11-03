# Domain Docs

This repository uses a single-context domain documentation layout.

## Before exploring

- Read `CONTEXT.md` at the repository root when it exists.
- Read relevant ADRs under `docs/adr/` when that directory exists.
- If either location is absent, proceed silently. Domain-modeling work creates these files only when terms or decisions are resolved.

## Layout

```text
/
|-- CONTEXT.md
|-- docs/
|   `-- adr/
`-- source directories
```

## Vocabulary

Use domain terms as defined in `CONTEXT.md` in issue titles, proposals, hypotheses, and test names. Avoid synonyms that the glossary explicitly rejects.

If a needed concept is missing, reconsider whether the project already uses a different term or note the gap for domain-modeling work.

## ADR conflicts

If proposed work contradicts an existing ADR, identify the conflict explicitly instead of silently overriding the recorded decision.
