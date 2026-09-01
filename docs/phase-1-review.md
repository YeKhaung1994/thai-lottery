# ထောပြီ — Project Review Meeting, Phase 1

**Date:** 1 September 2026
**Project:** ထောပြီ (Thai lottery results viewer) — Vue 3 SPA
**Phase 1 scope reviewed:** UI redesign, live GLO API integration, ticket checker, rebrand

---

## Project snapshot at review time

- Tabs refactored into real routed pages (`/`, `/results`, `/winners`, `/about`) with a sticky header and a mobile bottom tab bar.
- All lottery data now comes live from the official GLO API (the original Rayriffy API is dead — domain gone, repo archived May 2026); requests go through a dev-server proxy because GLO sends no CORS headers.
- New "Check your ticket" feature verified against a real draw (1 Sep 2026, first prize 417212).
- Rebranded from "Thai Lottery" to **ထောပြီ**: Burmese wordmark font (Noto Sans Myanmar), new ticket icon logo + favicon, icon-only header.
- Image slider built with three themed images (ticket / lottery / winner) but currently **hidden** on the home page.
- Lint clean; all pages visually verified in a real browser.

---

## 1. Project Owner — review and feedback

**Overall: Approved to proceed. The product finally does what it says.**

What I like:

- The winners page actually shows winning numbers now — the old version silently rendered nothing (the `flag` bug). That alone justifies the phase.
- The ticket checker is the feature users will come back for. Checking a number against every prize category in one tap is the core value of the site.
- Switching to the official GLO source was the right call. We can say "data from the Government Lottery Office" — that's a trust statement the old scraper API never gave us.
- The ထောပြီ rebrand with the Burmese identity clearly positions the product for the Myanmar-speaking audience.

Concerns / asks:

1. **We cannot ship this yet.** The GLO proxy only exists on the dev server. Until we have a production reverse proxy (or a small backend), the deployed site would show error states everywhere. This is the #1 blocker for phase 2.
2. **Legal footing.** We display official lottery data and the word "lottery" prominently. The About page disclaimer is good, but I want a short review of hosting jurisdiction rules before public launch.
3. **The work is not committed.** Everything from this phase is sitting uncommitted on `main`. Commit it on a branch before anything else happens to that machine.
4. Localization: the brand is Burmese but the UI copy is 100% English. If our audience is Myanmar users following the Thai lottery, a Burmese UI (or at least a working EN|MM toggle) should be on the phase-2 roadmap — the wireframes even sketched a language pill we later dropped.

---

## 2. Project Manager — review and feedback

**Overall: Phase 1 delivered beyond scope, but process debt needs paying down.**

Delivery review:

- All seven planned work items from the redesign spec (`docs/redesign-prompt.md`) were completed: API service layer, router refactor, four pages, responsive pass. Definition of done was met (lint zero errors, no hardcoded data, deep links verified, checker verified against a live response).
- Unplanned but necessary work absorbed mid-phase: replacing the dead Rayriffy API with GLO (including endpoint discovery), the rebrand, logo/favicon/font work, and slider image sourcing. Scope grew ~30% without a schedule slip — good, but it means phase 2 estimates should include a similar buffer.

Risks and gaps:

1. **Zero automated tests.** The ticket checker logic (`checkTicket`) and the GLO response normalizer are pure functions begging for unit tests. One GLO schema change would currently break the site silently. Priority: high, effort: low.
2. **Single point of failure on data.** GLO has no SLA for this API and no CORS; if they change or block it, we're down. Mitigation options for phase 2: cache last-known results server-side, or a scheduled job snapshotting each draw.
3. **No CI.** Lint and build should run on every push. Effort: trivial once the repo is on GitHub with the work committed.
4. **Uncommitted work / branching discipline.** All phase-1 work is local and unstaged. Action: commit to a `redesign` branch, PR to `main`, tag `v0.2.0`.
5. Housekeeping: `README.md` still describes the app as "Thai Lottery" with the Rayriffy API and a 3-image slider — update it to match reality (GLO API + proxy setup instructions, new brand).
6. The nested repo layout (`thai-lottery/thai-lottery/`) confuses tooling and contributors; consider flattening in phase 2.

Proposed phase-2 backlog (ordered): production proxy/deploy → commit + CI → unit tests for `lotteryApi` → README refresh → Burmese localization → draw-detail deep links (`/results/:date`).

---

## 3. Designer — review and feedback

**Overall: Strong foundation, consistent with the approved wireframes. A few refinements before I'd call it polished.**

What matches the design intent:

- The wireframes translated faithfully: hero draw card + ticket checker side by side, at-a-glance grid, expandable results cards, winners prize grid with chips, first-prize highlight with adjacent numbers.
- One accent (amber #d97706) used with restraint; active-nav states, digit tiles, and number chips are consistent across pages.
- Mobile bottom tab bar with stroke SVG icons and 44px+ hit targets, as specced. No fake device chrome.
- The new tilted-ticket logo reads well at 52px, and the red ticket + Burmese gold glyphs give the brand real character.

Feedback / refinements:

1. **Brand color tension.** The logo's palette is red #D2232A / gold #F2B01E, but the UI accent is amber #d97706. It works, but pick a stance: either adopt the logo's gold as the UI accent or keep amber and use red strictly for the brand mark. Two near-miss warm tones will read as accidental.
2. **Slider is hidden — decide its fate.** If it returns, restyle Swiper's default blue pagination dots to brand colors and give slides purposeful content (e.g., "next draw" countdown banner, how-to-play, responsible-play notice) rather than generic stock photos. If it stays hidden, delete the dead code and the 360 KB of images.
3. **Burmese typography.** Noto Sans Myanmar is only exercised by the (now hidden) wordmark and footer. If we go Burmese UI in phase 2, audit line heights everywhere — Burmese stacked diacritics need the looser leading we gave the old wordmark.
4. **Empty/edge states.** Loading and error states exist (good), but they're plain text. A lightweight skeleton for the hero card and draw list would remove the content jump on slow connections.
5. Small polish list: the header icon-only brand loses the site name for first-time visitors — consider showing the wordmark on the About page or as a tooltip; the date input on Results is browser-default styled and clashes slightly; footer bottom padding on mobile (90px) could tighten now that we know the tab bar height.

---

## Consolidated action items

| # | Action | Owner role | Phase |
|---|--------|-----------|-------|
| 1 | Commit phase-1 work to a branch, PR, tag | PM | Now |
| 2 | Production proxy for GLO API + deploy plan | Owner/PM | 2 (blocker) |
| 3 | Unit tests for `checkTicket` + normalizer; CI for lint/build | PM | 2 |
| 4 | Update README to GLO/ထောပြီ reality | PM | 2 |
| 5 | Decide accent color stance (gold vs amber) | Designer | 2 |
| 6 | Decide slider: redesign with purpose, or remove code | Designer/Owner | 2 |
| 7 | Burmese localization plan (EN|MM toggle) | Owner/Designer | 2–3 |
| 8 | GLO outage mitigation (cached snapshots) | PM | 3 |
| 9 | Legal/jurisdiction check before public launch | Owner | Before launch |
