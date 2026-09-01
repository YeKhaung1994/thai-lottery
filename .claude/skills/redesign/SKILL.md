---
name: redesign
description: Implement the Thai Lottery UI redesign — refactor tabs into routed pages, wire all pages to the live Rayriffy Lotto API, add the ticket checker, and apply the wireframe design system. Use when asked to run, continue, or resume the redesign.
---

# Thai Lottery Redesign

Wireframes: https://claude.ai/code/artifact/9d83e32c-7b5b-4243-b8b6-37a7a8fedd70
Full spec: [docs/redesign-prompt.md](../../../docs/redesign-prompt.md) — read it first; it is the source of truth.

You are a senior Vue 3 front-end engineer redesigning the existing app in
`thai-lottery/`. Work incrementally, keep the app runnable after every step,
and follow the wireframes exactly. Do not rewrite from scratch — refactor.

## Before starting

1. Read `docs/redesign-prompt.md` in full.
2. Check what has already been done (git log, existing `src/services/`,
   `src/composables/`, component names from the spec) and continue from
   there rather than redoing finished steps.

## Order of work

1. `src/services/lotteryApi.js` + `useLatestDraw()` composable
   (loading / error / cached states; visible retry on failure).
2. Router refactor: real routed pages with a shared sticky header,
   remove the tab rendering from LotteryHome.
3. Home page (hero draw card, ticket checker, slider, summary grid,
   recent draws).
4. Results page (live data, search + date filter, expandable draw
   cards, pagination).
5. Winners page (draw selector, first-prize highlight, prize grid,
   fix the dead `flag` logic).
6. About page content; remove the Tickets page and route.
7. Responsive pass: bottom tab bar on mobile, 44px hit targets.

## Definition of done

- `npm run serve` works and `npm run lint` passes with zero errors.
- No hardcoded lottery data; deep links refresh correctly.
- Ticket checker verified against a real `/latest` response.
- File-by-file summary of changes reported at the end.
