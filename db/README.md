# ထောပြီ database (SQL Server in Docker)

Schema for the platform-v2 spec (`docs/specs/platform-v2-monorepo.md`):
Users, RefreshTokens, Tickets, Orders, OrderItems, Payments, DrawResults.

## 1. Configure (once)

```bash
cd db
cp .env.example .env      # then EDIT .env
```

Set `MSSQL_SA_PASSWORD` in `db/.env` (blank by default; SQL Server requires
8+ chars with upper/lower/digit). `ConnectionStrings__Default` stays blank
until the API is wired up. `db/.env` is gitignored.

## 2. Start SQL Server

```bash
docker compose up -d
docker compose ps          # wait until STATUS shows (healthy)
```

## 3. Run the scripts (in order)

```bash
docker exec -i htawpyi-mssql /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$(grep ^MSSQL_SA_PASSWORD .env | cut -d= -f2)" -C -I \
  -i /scripts/01-create-database.sql -i /scripts/02-schema.sql -i /scripts/03-seed.sql
```

All scripts are idempotent — re-running is safe.

## 4. Verify

```bash
docker exec -i htawpyi-mssql /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "<your password>" -C \
  -Q "SELECT name FROM HtawPyi.sys.tables ORDER BY name;"
```

Expected: DrawResults, OrderItems, Orders, Payments, RefreshTokens,
Tickets, Users.

## Notes

- **Seed:** the admin user INSERT in `03-seed.sql` is commented out — the
  password hash must come from the API's hasher. Sample tickets insert only
  once that admin exists.
- **Concurrency:** `Tickets.RowVersion` + `UQ_OrderItems_Ticket` enforce
  one-sale-per-ticket; `READ_COMMITTED_SNAPSHOT` is on.
- **Reset:** `docker compose down -v` wipes the data volume completely.
- The future API consumes `ConnectionStrings__Default`; EF Core migrations
  will take over schema evolution from these bootstrap scripts.
