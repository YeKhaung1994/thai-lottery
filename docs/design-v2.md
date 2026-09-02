# HtiMart Design v2 — layout, style, type, color, UX

**Status:** implemented (2 Sep 2026) — deferred: cart drawer (sticky bar +
checkout page shipped instead), inventory bulk-select, tablet icon-rail
(phone top-bar shipped), OG image. Added post-approval: upload draw-date
restricted to the next draw (API + client).
**Scope:** customer app + admin app; tokens & components land in `shared_ui/`

---

## 1. Design principles

1. **A shop, not a bulletin.** v1 still reads like a results viewer with a shop
   bolted on. v2 leads with buying and winning; results support that story.
2. **Numbers are the heroes.** Every screen's most important object is a
   6-digit number. Give digits their own type treatment everywhere.
3. **One system, two densities.** Customer = spacious and warm; admin = the
   same tokens at higher density. Never two visual languages.
4. **Trust is a feature.** Money changes hands: visible order states,
   countdowns, receipts, and honest empty/error states everywhere.

## 2. Color system (tokens.css v2)

Keep the logo-derived palette; add semantic + neutral ramps. All new names,
no hex hardcoding in components afterwards.

```css
/* brand */
--red: #d2232a;        --red-tint: #fdeeee;      /* brand, wins */
--gold: #f2b01e;       --gold-tint: #fdf3d9;     /* celebration, awning */
--amber: #d97706;      --amber-dark: #b45309;    /* THE interactive accent */
--amber-tint: #fdf3e0;
--teal: #3aa6b9;       --teal-tint: #e9f5f8;     /* informational */
/* neutrals (warm) */
--ink: #2b2b2b;  --ink-2: #514a3d;  --muted: #6b6455;
--cream: #faf6ec;  --card: #ffffff;  --line: #eee6d6;  --line-strong: #ddd2ba;
/* semantic — formalize what is hardcoded today */
--success: #1d7a2e;    --success-tint: #e6f4e6;
--danger: #b3261e;     --danger-tint: #fdeeee;
/* elevation + shape */
--radius: 12px;  --radius-lg: 16px;  --radius-pill: 999px;
--shadow: 0 1px 2px rgba(64,48,15,.06), 0 6px 16px rgba(64,48,15,.06);
--shadow-lg: 0 4px 8px rgba(64,48,15,.08), 0 16px 40px rgba(64,48,15,.10);
```

Rules: amber stays the only interactive color (buttons, links, active nav);
red = brand + wins only; gold = celebration moments only (confetti, jackpot
banner); teal = neutral info. Contrast: body text ≥ 4.5:1, large text ≥ 3:1 —
gold never carries text.

## 3. Typography

| Role | Face | Why |
|---|---|---|
| Display / headings | **Outfit** (Google Fonts, 500–800) | Geometric, friendly, matches the rounded ticket logo; not an AI-default face |
| Body | system stack (`Avenir, Helvetica, …`) | Fast, familiar; body copy is short everywhere |
| **Ticket numbers** | **Azeret Mono** (500/700, tabular) | Monospace = printed-serial feel; digits align in lists; slashed zero kills 0/O doubt |
| Burmese | Noto Sans Myanmar (kept) | Renders the brand + future MM localization; keep 1.6+ line-height |

Type scale (px): 13 / 15 / 16 (body) / 18 / 22 / 28 / 40 (display digits).
Ticket numbers always `letter-spacing: 0.08em`. Load fonts with
`display=swap`; fallbacks with close metrics.

## 4. Customer app redesign

### Layout
- **Header**: unchanged structure; add the wordmark "HtiMart" in Outfit 700
  next to the logo (icon-only header lost the name for first-time visitors).
- **Home, new order of scenes**:
  1. **Draw strip** (new): thin full-width band — "Next draw 16 Sep · closes
     in 13 d 21 h" with a live countdown; turns gold on draw day
     ("Results from 14:30 — refreshing"). Replaces the buried glance card.
  2. Hero: latest-draw card + My Tickets (as today).
  3. **Shop teaser** (new): one row of 4 buyable tickets + "Browse all →" —
     the store must be visible on the money page.
  4. Checker band, at-a-glance grid, recent draws (as today).
- **Buy Tickets**:
  - **Lucky-number picker** replaces the plain search: six per-digit boxes
    (leave blanks as wildcards, e.g. `_ _ 8 _ _ 8`) + quick chips: "ends in
    88", "doubles", "my birthday". This is how lottery buyers actually think.
  - Ticket cards get a perforation edge (CSS dashed mask) + Azeret digits.
  - **Reservation countdown** (15:00 → 0:00) as a pinned bar after checkout
    starts; expiry returns tickets with a toast, not a silent 409.
  - Cart drawer (right slide-over) with per-ticket remove; keep sticky bar
    as the trigger on mobile.
- **Checkout** (new step, replaces instant redirect): order summary — tickets,
  total, draw date, payment method logo — then "Pay". Trust pause.
- **My Purchases**: winning orders get a **gold celebration header** with a
  small confetti burst (one-shot CSS animation) and total won; add a
  per-order receipt view (order id, time, method) behind "Details".

### States & motion
- Skeletons (shimmering cream blocks) for hero, shop grid, order list —
  replaces text-only "Loading…".
- Toast atom for transient results (added to cart, reservation expired,
  copied); inline text stays for form errors.
- Motion: 150–200 ms ease-out on hover-lift/expand only; one 600 ms
  celebration on wins; respect `prefers-reduced-motion`.

## 5. Admin app redesign

- **Add a Dashboard landing** (`/` instead of redirect to Upload): four stat
  tiles — Available / Reserved / Sold (next draw), Revenue (this draw) —
  plus "latest 5 orders". Admins open the app to see *state*, not a form.
- **Sidebar**: icons (AppIcon) + labels, active state as amber left-rail bar;
  collapses to icon-rail ≤ 1024 px, top bar on phones.
- **Tables** (shared organism): sticky header, zebra rows on hover, right-
  aligned numerals (Azeret tabular), per-column sort, 25-row pagination,
  count in the header ("142 tickets"). Inventory adds bulk-select + bulk
  delete (Available only).
- **Upload**: drag-and-drop CSV zone + file picker; **preview table with
  per-row validation BEFORE commit** (green/red rows), then one confirm.
  Errors stop being a post-hoc report.
- Density: 44 px controls stay, but table rows 40 px, 14 px body — an admin
  scans hundreds of rows.

## 6. Component inventory (build in shared_ui)

| New | Type | Used by |
|---|---|---|
| `BaseButton` (primary/secondary/ghost/danger) | atom | both — kills 6 bespoke button styles |
| `TextField` (label, error, hint slots) | atom | both — kills 5 bespoke inputs |
| `Toast` + `useToasts()` | atom + composable | both |
| `Skeleton` | atom | both |
| `EmptyState` (icon, title, action slot) | molecule | both |
| `StatTile` | molecule | admin dashboard, customer glance |
| `DataTable` | organism | admin |
| `CountdownTimer` | atom | draw strip, reservation bar |
| `TicketNumber` (Azeret digits, sizes) | atom | everywhere a 6-digit number renders |

Existing atoms (AppIcon, NumberChip, DigitTiles, StatusPill, PrizeCard,
DrawCard) restyle via tokens only — no API changes.

## 7. Accessibility checklist (gates every screen)

Focus visible on every interactive element (2 px amber outline, offset 2);
44 px targets (tables exempt, 40 px); `aria-live="polite"` on toasts,
countdowns and checker results; forms labeled (no placeholder-as-label —
fix the Buy search); contrast per §2; full keyboard path through
buy → checkout → confirm.

## 8. Rollout

| Phase | Contents | Risk |
|---|---|---|
| 1 | tokens v2 + fonts + TicketNumber/BaseButton/TextField swap | low — visual only |
| 2 | Customer: draw strip, shop teaser, lucky-number picker, checkout step, skeletons/toasts | medium |
| 3 | Admin: dashboard, DataTable, upload preview | medium |
| 4 | Celebrations, cart drawer, icon-rail sidebar, receipt view | low |

Out of scope: dark mode (revisit after MM localization), Burmese UI copy
(separate roadmap item), any change to API contracts.
