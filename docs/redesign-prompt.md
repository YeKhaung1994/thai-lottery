# Thai Lottery Redesign — Implementation Prompt

Wireframes: https://claude.ai/code/artifact/9d83e32c-7b5b-4243-b8b6-37a7a8fedd70

---

# Role
You are a senior Vue 3 front-end engineer redesigning an existing Thai Lottery
results app. Work incrementally, keep the app runnable after every step, and
follow the wireframes described below exactly.

# Existing codebase (do not rewrite from scratch — refactor)
- Vue 3 + Vue CLI 5 app in `thai-lottery/`, Vue Router 4 (history mode),
  axios, Swiper 9. Routes: `/`, `/results`, `/winners`, `/about`.
- Current problems to fix: LotteryHome re-renders every view as tabs instead
  of using the router; LotteryResults uses hardcoded sample data;
  LotteryWinners has a `flag` that is never set true, so winning numbers
  never render; Tickets and About pages are placeholders.

# Data source (single source of truth)
> Note: the Rayriffy Lotto API the app originally used is dead (repo archived
> May 2026, domain no longer resolves). Use the official GLO API instead.

Official Government Lottery Office (GLO) API, no key required. All endpoints
are `POST` with a JSON body:
- `POST https://www.glo.or.th/api/lottery/getLatestLottery` body `{}` →
  `response.date` (ISO), `response.data` with prize groups `first`, `near1`,
  `second`, `third`, `fourth`, `fifth` and running numbers `last3f`, `last3b`,
  `last2` — each `{price, number: [{round, value}]}`.
- `POST https://www.glo.or.th/api/lottery/getLotteryResult` body
  `{"date":"16","month":"08","year":"2026"}` → same shape for one draw.
- `POST https://www.glo.or.th/api/lottery/getPeriodList` body `{}` →
  `response.list`: ISO draw dates, newest first.

GLO sends no CORS headers, so the SPA cannot call it cross-origin: proxy
`/glo` → `https://www.glo.or.th` in `vue.config.js` devServer for
development, and read the base from `VUE_APP_LOTTERY_API_BASE` so production
can point at its own reverse proxy.

Create `src/services/lotteryApi.js` wrapping these with axios (normalize the
GLO shape into `{date, prizes[], front3, back3, last2}`), and one composable
`useLatestDraw()` with loading / error / cached states. Handle API failure
with a visible retry state — never a blank page.

# Pages (match the wireframes)
1. **Home `/`** — sticky header (logo, nav: Home/Results/Winners/About,
   EN|TH placeholder toggle). Hero row: (a) latest-draw card — draw date,
   first-prize number as six large digit tiles, prize amount; (b) "Check your
   ticket" card — 6-digit input + Check button that compares the input
   against ALL prize categories and running numbers of the latest draw and
   shows "You won <category> ฿<reward>" or "No prize this draw". Below:
   existing Swiper autoplay slider; then a 4-card summary grid (2-digit,
   front-3, back-3, next-draw date); then 3 recent draws linking to /results.
2. **Results `/results`** — search input (matches full or partial numbers)
   + date filter + clear. List of draw cards from the `list` endpoint,
   newest first, expandable to show that draw's full prizes; pagination.
3. **Winners `/winners`** — draw selector (prev/next + dropdown). Highlighted
   first-prize card with adjacent numbers, then a 3-column responsive grid of
   prize-category cards with numbers as chips; categories with >10 numbers
   collapse behind "show all". Fix the dead `flag` logic.
4. **About `/about`** — short real content: what the site is (unofficial
   results viewer), data source credit, disclaimer. Remove the Tickets page
   and route unless told otherwise.

# Design system
- Mobile-first responsive; breakpoint ~768px. On mobile, top nav collapses to
  a fixed bottom tab bar (Home, Results, Winners, About) with stroke SVG
  icons, min 44px hit targets.
- One accent color: amber `#d97706` (active nav underline, primary buttons,
  highlighted winning numbers). Neutral ink `#2b2b2b` on white; keep it
  restrained — no gradients, no emoji icons.
- Typography: one display face for the big draw digits (tabular/monospaced
  numerals), system stack for body.
- Digit tiles, number chips, and prize cards are reusable components:
  `DigitTiles.vue`, `NumberChip.vue`, `PrizeCard.vue`, `DrawCard.vue`.

# Acceptance criteria
- `npm run serve` works; `npm run lint` passes with zero errors.
- No hardcoded lottery data anywhere; all numbers come from the API.
- Deep-linking works: /results and /winners render correctly on refresh.
- Ticket checker verified against a real latest-draw response.
- Keyboard-accessible nav and inputs; images have alt text.

Deliver a file-by-file summary of changes at the end.
