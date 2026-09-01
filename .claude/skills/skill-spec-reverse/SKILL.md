---
name: skill-spec-reverse
description: Reverse-engineer a spec from existing ထောပြီ code — document what a module, page, or component actually does today, saved to docs/specs/. Use when asked to document, reverse-spec, or analyze current behavior.
---

# Reverse-engineer a spec from code

Produce a spec describing what the code **actually does** (not what it
should do) for the module named in the arguments (or ask which one). Save
to `docs/specs/<module>-current.md`.

## Process

1. Locate the module's files: start from the route in `src/main.js`, then
   the view under `src/components/views/`, its child components, and any
   composables/services it imports. Read every file fully.
2. Document observed behavior only. Where behavior depends on the GLO API,
   quote the real request/response shape from `src/services/lotteryApi.js`
   and `tests/unit/fixtures/glo-latest.json` — do not guess.
3. Trace every state: initial, loading, error, empty, success, and every
   user interaction (clicks, inputs, route params, localStorage effects).

## Output structure

```markdown
# <Module> — current behavior

**Files:** list with paths
**Route(s):** paths, params, redirects, document title

## What it does
Prose summary of the module's actual purpose as implemented.

## Behavior inventory
Numbered facts (B1, B2, ...): rendering rules, data flow, caching,
state transitions, edge-case handling. Each cites file:line.

## Data dependencies
API endpoints called, composables used, localStorage keys, shared state.

## Findings
Discrepancies, dead code, bugs, accessibility gaps, or surprising
behavior discovered while reading. REPORT these — do not fix them here.
```

## Rules

- This skill is read-only for source code: it writes only the spec file.
- Every claim must cite the file (and line where useful) that proves it.
- Findings feed `/skill-spec` (to decide fixes) and `/skill-implement`
  (to execute them) — end by suggesting next steps in one line.
