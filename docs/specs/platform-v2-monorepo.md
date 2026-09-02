# Platform v2 — Monorepo: Customer, Admin, API (ticket sales)

**Status:** implemented (2 Sep 2026 — payment = Mock provider; 2C2P structure
awaits credentials + sandbox verification; see apps/api/README.md)
**Module(s):** new (platform restructure; existing SPA becomes the Customer app)

## Purpose
Turn ထောပြီ from a results viewer into a ticket-selling platform: customers
register, search and buy real tickets for the upcoming draw, and see their
purchase history with winnings; admins manage ticket inventory and sales. One
repository hosts all three projects with a shared contract.

## Decisions (locked)
- **API:** ASP.NET Core 8 Web API, repository design pattern (interface per
  aggregate + EF Core implementation), service layer, controllers thin.
- **DB:** SQL Server (MSSQL) via EF Core migrations.
- **Auth:** email + password (ASP.NET Identity password hashing), JWT access
  (15 min) + refresh token (30 days, rotated, stored hashed in DB).
- **Payment:** 2C2P — **setup only** for now: full integration structure
  (config binding, payment service interface, callback endpoint, signature
  logic) wired end-to-end, but credentials are BLANK env placeholders. Until
  keys are added, a built-in "mock provider" completes payments in
  development so the order flow is testable; flipping to real 2C2P is
  config-only, no code change.

## Monorepo layout
```
apps/
  customer/   ← existing Vue app moves here (results viewer + shop + login)
  admin/      ← new Vue 3 app (same design tokens, AppIcon set)
  api/        ← .NET 8: Api / Application / Domain / Infrastructure projects
docs/         ← specs (unchanged location)
```
Root scripts: `npm run dev:customer`, `dev:admin`; `dotnet run` for api.
GLO results features keep working unchanged (proxy `/glo` stays).

## Data model (MSSQL)
- `Users` (Id, Email unique, PasswordHash, Role: Customer|Admin, CreatedAt)
- `RefreshTokens` (Id, UserId FK, TokenHash, ExpiresAt, RevokedAt)
- `Tickets` (Id, DrawDate, Number char(6), Price decimal, Status:
  Available|Reserved|Sold, UploadedBy FK, RowVersion)
- `Orders` (Id, UserId FK, Status: Pending|Paid|Failed|Expired, Total,
  CreatedAt); `OrderItems` (OrderId FK, TicketId FK unique)
- `Payments` (Id, OrderId FK, Provider: Mock|2C2P, ProviderRef, Amount,
  Status, RawCallback nvarchar(max), CreatedAt)
- `DrawResults` (DrawDate PK, FetchedAt, ResultJson nvarchar(max)) — the
  API's server-side cache of GLO results for win calculation

## Functional requirements
**Auth** — FR1 Register (email, password ≥8) → 201; duplicate email → 409.
FR2 Login → JWT + refresh; FR3 refresh rotation; FR4 admin-only endpoints
require Role=Admin (403 otherwise).

**Customer** — FR5 Search available tickets for the next draw by full/partial
number (`GET /api/tickets?drawDate=&q=`). FR6 Create order for 1–10 tickets;
tickets become Reserved for 15 min (RowVersion prevents double-sell; expired
reservations auto-release). FR7 Order → 2C2P payment token → redirect URL
returned. FR8 2C2P callback (HMAC verified) marks Order Paid + Tickets Sold;
failure/timeout releases tickets. FR9 `GET /api/orders/mine` lists purchase
history; after a draw, each owned ticket shows its prize result. The matching
logic is PORTED to C# (`checkTicket` in `src/services/lotteryApi.js:83` is
client JS and stays there for My Tickets); the API fetches draw results from
GLO itself, server-to-server (`getLotteryResult`/`getLatestLottery` — no CORS
issue, no `/glo` proxy involved) and caches one result row per draw. FR10 Customer app
shows Login/Register, ticket shop ("Buy tickets" module), and "My purchases"
history; existing My Tickets (localStorage) stays for manually tracked numbers.

**Admin** — FR11 Admin app: login, then dashboard. FR12 Upload tickets:
single form + CSV bulk (`drawDate,number,price`); duplicate (DrawDate,Number)
rejected per-row with a report. FR13 Inventory list with status filter;
delete allowed only while Available. FR14 Sales history: orders with
customer email, tickets, payment status, totals per draw.

**Payment (setup only)** — FR15 `IPaymentProvider` abstraction with two
implementations: `TwoCTwoPProvider` (complete, reads MerchantId/SecretKey
from config) and `MockPaymentProvider` (auto-succeeds after confirm click);
provider chosen by config flag `Payment:Provider = Mock|TwoCTwoP`. FR16
env/user-secrets templates ship with BLANK 2C2P values
(`PAYMENT__2C2P__MERCHANT_ID=`, `PAYMENT__2C2P__SECRET_KEY=`); API refuses
to start with Provider=TwoCTwoP while they are blank. FR17 callback endpoint
is signature-verified and idempotent (replays ignored); amounts verified
server-side against the order total. FR18 no payment credential ever
appears in a client bundle or committed file.

## API surface (v1)
`POST /api/auth/register|login|refresh` · `GET /api/tickets` ·
`POST /api/orders` · `GET /api/orders/mine` · `POST /api/payments/callback` ·
admin: `POST /api/admin/tickets` (single|csv) · `GET /api/admin/tickets` ·
`DELETE /api/admin/tickets/{id}` · `GET /api/admin/orders`
All JSON; errors as RFC 7807 problem+json.

## UI behavior
- **Customer:** new "Buy Tickets" nav item (amber active state). Shop page:
  search field (reuse History search styling), ticket cards as NumberChip
  grid with price, cart drawer, 15-min reservation countdown; states:
  loading / empty ("no tickets match") / reserved-expired warning. "My
  purchases" page: order rows (DrawCard-like) with payment status pill and
  per-ticket win status in brand red. Login/register forms: 44px targets,
  `.sr-only` labels, error text under fields.
- **Admin:** separate app, same tokens on cream ground; sidebar nav
  (Upload / Inventory / Sales). CSV upload with per-row error table.
- Mobile: shop grid 2-col; bottom nav gains "Buy" (ticket AppIcon).

## Acceptance criteria
- [ ] `dotnet build` + `dotnet test` green: unit tests for repositories
      (in-memory/SQLite), reservation expiry, callback signature + idempotency
- [ ] EF migrations create the schema on a fresh MSSQL (connection string
      from env; localdb/docker documented in apps/api/README)
- [ ] Both Vue apps: lint clean, jest green, production build succeeds
- [ ] CI updated for the new layout: `.github/workflows/ci.yml` currently
      hardcodes `working-directory: thai-lottery` (lines 14, 21) — paths
      move to `apps/customer` + `apps/admin`, and a dotnet build/test job
      is added; Netlify/Vercel configs move with the customer app
- [ ] Browser-verified happy path with the mock provider: register → search
      → buy → confirm → order Paid → history shows ticket; admin sees the
      sale (real 2C2P path verified later once credentials exist)
- [ ] Double-purchase race test: two concurrent orders for one ticket —
      exactly one succeeds
- [ ] No secret in any client bundle or committed file

## Out of scope
Burmese localization; refunds/cancellations; SMS/email notifications;
seller payouts/accounting; PWA; migrating My Tickets into accounts.
**Note:** selling lottery tickets online has jurisdiction-specific rules —
the standing legal review (phase-1 review item 9) must clear before launch.
