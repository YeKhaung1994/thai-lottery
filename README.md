# ထောပြီ (Htaw Pyi)

A Vue 3 single-page application for browsing Thailand Government Lottery results. Shows the latest draw, past results, and winning numbers — and checks a 6-digit ticket against every prize category of the latest draw. All data comes live from the official [Government Lottery Office (GLO)](https://www.glo.or.th) API.

The app source lives in the [`thai-lottery/`](thai-lottery/) directory.

## Features

- **Home** — latest draw hero with the first-prize number as digit tiles, "Check your ticket" widget, at-a-glance running numbers, next-draw countdown, and recent draws
- **Ticket checker** — enter a 6-digit number and see every prize it matches (main prizes, adjacent numbers, front/back 3-digit, 2-digit) — a ticket can win more than one
- **Results** (`/results`) — full draw history with number search, date filter, expandable draw cards, and pagination
- **Winners** (`/winners`) — winning numbers for any draw, with a draw selector, highlighted first prize + adjacent numbers, and collapsible prize grids
- **Responsive** — sticky top nav on desktop, fixed bottom tab bar on mobile
- **Burmese brand** — ထောပြီ wordmark (Noto Sans Myanmar) and ticket icon logo/favicon

## Tech Stack

- [Vue 3](https://vuejs.org/) with [Vue CLI 5](https://cli.vuejs.org/), [Vue Router 4](https://router.vuejs.org/) (history mode)
- [Axios](https://axios-http.com/) for API calls, [Swiper 9](https://swiperjs.com/) (promo slider, currently hidden)
- [Jest](https://jestjs.io/) unit tests, ESLint, GitHub Actions CI

## Data source & proxy

All endpoints are `POST` on `https://www.glo.or.th/api/lottery/`:

| Endpoint | Body | Returns |
|---|---|---|
| `getLatestLottery` | `{}` | Latest draw (prizes + running numbers) |
| `getLotteryResult` | `{"date":"16","month":"08","year":"2026"}` | One specific draw |
| `getPeriodList` | `{}` | All draw dates, newest first |

GLO sends **no CORS headers**, so the browser can't call it directly. The SPA calls `/glo/...` on its own origin and a proxy forwards it:

- **Development** — proxied by the dev server (see `thai-lottery/vue.config.js`)
- **Production** — ready-made configs are included for [Netlify](thai-lottery/netlify.toml) and [Vercel](thai-lottery/vercel.json); any other host needs an equivalent `/glo → https://www.glo.or.th` reverse proxy. The base path is configurable via the `VUE_APP_LOTTERY_API_BASE` env var.

## Getting Started

Requires [Node.js](https://nodejs.org/) 20+ and npm.

```bash
cd thai-lottery
npm install
npm run serve     # dev server with hot reload → http://localhost:8080
```

### Other scripts

```bash
npm test          # Jest unit tests (ticket checker, GLO normalizer, formatters)
npm run lint      # ESLint
npm run build     # production bundle → thai-lottery/dist/
```

## Project Structure

```
thai-lottery/
├── public/                  # index.html, favicons
├── tests/unit/              # Jest specs + real GLO response fixture
└── src/
    ├── main.js              # Router setup (/, /results, /winners, /about)
    ├── App.vue              # Layout shell (header, footer, mobile tab bar)
    ├── services/
    │   └── lotteryApi.js    # GLO client, response normalizer, checkTicket()
    ├── composables/
    │   └── useLatestDraw.js # Shared latest-draw state (loading/error/retry)
    └── components/
        ├── AppHeader.vue / BottomNav.vue
        ├── DigitTiles.vue / NumberChip.vue / PrizeCard.vue / DrawCard.vue
        ├── TicketChecker.vue
        └── views/           # LotteryHome, LotteryResults, LotteryWinners, AboutUs
```

## Docs

- [docs/redesign-prompt.md](docs/redesign-prompt.md) — the redesign spec (source of truth for the rebuild)
- [docs/phase-1-review.md](docs/phase-1-review.md) — phase-1 review meeting notes and action items

## Disclaimer

Unofficial results viewer, not affiliated with the GLO. Always verify winning tickets against the officially published results before claiming a prize.
