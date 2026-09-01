---
name: skill-implement
description: Implement a feature in the ထောပြီ app from a spec in docs/specs/ (or an inline request), following project conventions, with verification and a commit. Use when asked to implement, build, or code a feature.
---

# Implement from a spec

Build the feature described by the spec named in the arguments (a file in
`docs/specs/`, or `docs/redesign-prompt.md` sections). If no spec exists,
say so and offer to run `/skill-spec` first — implementing without a spec
is the exception, not the rule.

## Before coding

1. Read the spec in full; treat its acceptance criteria as the definition
   of done.
2. Check what already exists: `git log --oneline -5`, the components under
   `src/components/`, composables, and services — refactor, don't duplicate.
3. Confirm the dev server is running (`npm run serve` in `thai-lottery/`,
   http://localhost:8080) or start it in the background.

## Project conventions (non-negotiable)

- Vue 3 options API in views/components; composables for shared state
  (see `useLatestDraw.js`, `useMyTickets.js` for the pattern).
- All lottery data flows through `src/services/lotteryApi.js` — never call
  the GLO API (or axios) directly from a component. The dev proxy is `/glo`.
- Design tokens from `App.vue` `:root` — never hardcode new hex values:
  amber = interactive accent, red = brand/win, gold/teal = category tints,
  cream ground, `var(--radius)`, `var(--shadow)`.
- Icons via `AppIcon.vue` (add new stroke paths there; no emoji).
- 44px minimum hit targets; `.sr-only` labels for inputs; keyboard usable.
- Old URLs must keep working — renames get router redirects.

## Work loop

Implement incrementally; the app must compile after every step. Then verify:

1. `npm run lint` — zero errors.
2. `npm test` — all pass; add tests for new pure logic (see `/skill-test`).
3. `npm run build` — production build succeeds.
4. Browser-verify each changed page with headless Chrome
   (`--headless=new --screenshot=... --virtual-time-budget=15000` against
   localhost:8080; note: window width clamps at 500px) and READ the
   screenshot — do not assume it rendered.

## Finish

- Update the spec's **Status** to `implemented`.
- Commit on the current branch with a body listing the changes (no
  Claude attribution — it is disabled in settings).
- Report: what shipped, what was verified and how, anything deferred.
