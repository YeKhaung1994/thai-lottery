-- ထောပြီ platform v2 — database bootstrap
-- Target: SQL Server 2022 (docker). Idempotent: safe to re-run.

IF DB_ID(N'HtawPyi') IS NULL
BEGIN
    CREATE DATABASE HtawPyi;
END
GO

ALTER DATABASE HtawPyi SET READ_COMMITTED_SNAPSHOT ON;
GO
