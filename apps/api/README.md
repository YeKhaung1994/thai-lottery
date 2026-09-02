# ထောပြီ API (.NET 8)

Repository-pattern Web API per `docs/specs/platform-v2-monorepo.md`:
`Domain` (entities, TicketMatcher) ← `Application` (interfaces, services,
DTOs) ← `Infrastructure` (EF Core, repositories, JWT, GLO client, payment
providers) ← `Api` (controllers, DI).

## Run locally

1. SQL Server up with the schema (see `db/README.md`).
2. Set the connection string in the environment (blank by default —
   see `db/.env`):

```bash
export ConnectionStrings__Default="$(grep '^ConnectionStrings__Default=' ../../db/.env | cut -d= -f2-)"
dotnet run --project src/HtawPyi.Api     # → http://localhost:5210
```

`GET /health` answers `{"status":"ok"}` when up.

## Configuration (all optional in Development)

| Setting | Default | Notes |
|---|---|---|
| `ConnectionStrings__Default` | *(blank)* | Required — API refuses to start without it |
| `Jwt__Key` | *(blank)* | Blank in dev = ephemeral key per run; REQUIRED outside Development |
| `Payment__Provider` | `Mock` | `Mock` or `TwoCTwoP` |
| `PAYMENT__2C2P__MERCHANT_ID` / `SECRET_KEY` | *(blank)* | Must be filled before Provider=TwoCTwoP (startup refuses otherwise) |
| `Payment__CustomerAppUrl` | `http://localhost:8080` | CORS + redirect target |
| `AdminAppUrl` | `http://localhost:8081` | CORS |

No credentials are committed anywhere; fill secrets via env/user-secrets.

## Bootstrap an admin

Register through the API, then promote once in SQL:

```sql
UPDATE HtawPyi.dbo.Users SET Role = N'Admin' WHERE Email = N'you@example.com';
```

## Tests

```bash
dotnet test   # TicketMatcher port (real GLO fixture) + OrderService (SQLite)
```

## Notes

- The 2C2P provider is STRUCTURE ONLY — complete code path (payment token,
  signature-verified idempotent callback) but never run against the 2C2P
  sandbox. Verify there before any production use.
- Reservation expiry is lazy: expired holds become purchasable on the next
  search/order, and the stale order is auto-expired when its ticket is
  re-bought.
- Draw results are fetched from GLO server-to-server on first need and
  cached in `DrawResults`.
