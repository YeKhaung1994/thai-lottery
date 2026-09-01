# Thai Lottery

A Vue 3 single-page application for browsing Thailand Lottery results and winning numbers. The latest draw's winning numbers are fetched live from the public [Rayriffy Lotto API](https://lotto.api.rayriffy.com/latest).

The app source lives in the [`thai-lottery/`](thai-lottery/) directory.

## Features

- **Home** — tabbed landing page (Results / Winners / Tickets / About Us) with the site logo
- **Winners** — fetches the latest draw from `https://lotto.api.rayriffy.com/latest` and lists the winning numbers grouped by prize category
- **Results** — lottery results list with keyword search and date filtering (sample data for now)
- **Image slider** — Swiper-based autoplay carousel component
- **Routing** — client-side routes via Vue Router: `/`, `/results`, `/winners`, `/about`

## Tech Stack

- [Vue 3](https://vuejs.org/) with [Vue CLI 5](https://cli.vuejs.org/)
- [Vue Router 4](https://router.vuejs.org/) (HTML5 history mode)
- [Axios](https://axios-http.com/) for API calls
- [Swiper 9](https://swiperjs.com/) for the image carousel
- ESLint (`plugin:vue/vue3-essential`) + Babel

## Project Structure

```
thai-lottery/
├── public/                  # Static assets and index.html
└── src/
    ├── main.js              # App entry point and router setup
    ├── App.vue              # Root component (router-view)
    ├── assets/              # Logo and slider images
    └── components/
        ├── Banner.vue       # Reusable banner component
        └── views/
            ├── LotteryHome.vue      # Landing page with tabs
            ├── LotteryResults.vue   # Results list with search/date filter
            ├── LotteryWinners.vue    # Winning numbers from the live API
            ├── LotteryTickets.vue   # Tickets page (placeholder)
            ├── AboutUs.vue          # About page (placeholder)
            ├── ImageSlider.vue      # Swiper carousel
            └── LotteryFooter.vue    # Footer
```

## Getting Started

Requires [Node.js](https://nodejs.org/) (LTS recommended) and npm.

```bash
cd thai-lottery
npm install
```

### Development server (hot reload)

```bash
npm run serve
```

Then open http://localhost:8080.

### Production build

```bash
npm run build
```

The optimized bundle is output to `thai-lottery/dist/`.

### Linting

```bash
npm run lint
```

## API

Winning numbers come from the free community-run Rayriffy Lotto API:

- `GET https://lotto.api.rayriffy.com/latest` — latest draw, including `prizes` (prize categories with winning numbers) and `runningNumbers`

No API key is required.
