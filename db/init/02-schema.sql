-- ထောပြီ platform v2 — schema per docs/specs/platform-v2-monorepo.md
-- Target: SQL Server 2022. Idempotent: each object guarded by IF NOT EXISTS.

USE HtawPyi;
GO

-- Required for the filtered index on Payments (sqlcmd defaults these OFF
-- when reading from stdin without -I).
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

------------------------------------------------------------------- Users
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id           UNIQUEIDENTIFIER NOT NULL
                     CONSTRAINT DF_Users_Id DEFAULT NEWSEQUENTIALID(),
        Email        NVARCHAR(256)    NOT NULL,
        PasswordHash NVARCHAR(500)    NOT NULL,
        Role         NVARCHAR(20)     NOT NULL
                     CONSTRAINT DF_Users_Role DEFAULT N'Customer',
        CreatedAt    DATETIME2(0)     NOT NULL
                     CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Users PRIMARY KEY (Id),
        CONSTRAINT UQ_Users_Email UNIQUE (Email),
        CONSTRAINT CK_Users_Role CHECK (Role IN (N'Customer', N'Admin'))
    );
END
GO

----------------------------------------------------------- RefreshTokens
IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens (
        Id        UNIQUEIDENTIFIER NOT NULL
                  CONSTRAINT DF_RefreshTokens_Id DEFAULT NEWSEQUENTIALID(),
        UserId    UNIQUEIDENTIFIER NOT NULL,
        TokenHash NVARCHAR(500)    NOT NULL,
        ExpiresAt DATETIME2(0)     NOT NULL,
        RevokedAt DATETIME2(0)     NULL,
        CreatedAt DATETIME2(0)     NOT NULL
                  CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_RefreshTokens PRIMARY KEY (Id),
        CONSTRAINT FK_RefreshTokens_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens (UserId);
END
GO

----------------------------------------------------------------- Tickets
IF OBJECT_ID(N'dbo.Tickets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tickets (
        Id            UNIQUEIDENTIFIER NOT NULL
                      CONSTRAINT DF_Tickets_Id DEFAULT NEWSEQUENTIALID(),
        DrawDate      DATE             NOT NULL,
        Number        CHAR(6)          NOT NULL,
        Price         DECIMAL(10, 2)   NOT NULL,
        Status        NVARCHAR(20)     NOT NULL
                      CONSTRAINT DF_Tickets_Status DEFAULT N'Available',
        ReservedUntil DATETIME2(0)     NULL,
        UploadedBy    UNIQUEIDENTIFIER NOT NULL,
        RowVersion    ROWVERSION       NOT NULL,
        CreatedAt     DATETIME2(0)     NOT NULL
                      CONSTRAINT DF_Tickets_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Tickets PRIMARY KEY (Id),
        CONSTRAINT UQ_Tickets_Draw_Number UNIQUE (DrawDate, Number),
        CONSTRAINT CK_Tickets_Number CHECK (Number LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]'),
        CONSTRAINT CK_Tickets_Price CHECK (Price > 0),
        CONSTRAINT CK_Tickets_Status
            CHECK (Status IN (N'Available', N'Reserved', N'Sold')),
        CONSTRAINT FK_Tickets_Users
            FOREIGN KEY (UploadedBy) REFERENCES dbo.Users (Id)
    );

    -- Shop search: available tickets for a draw, filtered by number.
    CREATE INDEX IX_Tickets_Draw_Status
        ON dbo.Tickets (DrawDate, Status)
        INCLUDE (Number, Price, ReservedUntil);
END
GO

------------------------------------------------------------------ Orders
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders (
        Id        UNIQUEIDENTIFIER NOT NULL
                  CONSTRAINT DF_Orders_Id DEFAULT NEWSEQUENTIALID(),
        UserId    UNIQUEIDENTIFIER NOT NULL,
        Status    NVARCHAR(20)     NOT NULL
                  CONSTRAINT DF_Orders_Status DEFAULT N'Pending',
        Total     DECIMAL(12, 2)   NOT NULL,
        CreatedAt DATETIME2(0)     NOT NULL
                  CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Orders PRIMARY KEY (Id),
        CONSTRAINT CK_Orders_Status
            CHECK (Status IN (N'Pending', N'Paid', N'Failed', N'Expired')),
        CONSTRAINT CK_Orders_Total CHECK (Total >= 0),
        CONSTRAINT FK_Orders_Users
            FOREIGN KEY (UserId) REFERENCES dbo.Users (Id)
    );

    CREATE INDEX IX_Orders_User_CreatedAt
        ON dbo.Orders (UserId, CreatedAt DESC);
END
GO

-------------------------------------------------------------- OrderItems
IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems (
        OrderId         UNIQUEIDENTIFIER NOT NULL,
        TicketId        UNIQUEIDENTIFIER NOT NULL,
        PriceAtPurchase DECIMAL(10, 2)   NOT NULL,
        CONSTRAINT PK_OrderItems PRIMARY KEY (OrderId, TicketId),
        -- One ticket can only ever belong to one order.
        CONSTRAINT UQ_OrderItems_Ticket UNIQUE (TicketId),
        CONSTRAINT FK_OrderItems_Orders
            FOREIGN KEY (OrderId) REFERENCES dbo.Orders (Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_Tickets
            FOREIGN KEY (TicketId) REFERENCES dbo.Tickets (Id)
    );
END
GO

---------------------------------------------------------------- Payments
IF OBJECT_ID(N'dbo.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Payments (
        Id          UNIQUEIDENTIFIER NOT NULL
                    CONSTRAINT DF_Payments_Id DEFAULT NEWSEQUENTIALID(),
        OrderId     UNIQUEIDENTIFIER NOT NULL,
        Provider    NVARCHAR(20)     NOT NULL,
        ProviderRef NVARCHAR(100)    NULL,
        Amount      DECIMAL(12, 2)   NOT NULL,
        Status      NVARCHAR(20)     NOT NULL,
        RawCallback NVARCHAR(MAX)    NULL,
        CreatedAt   DATETIME2(0)     NOT NULL
                    CONSTRAINT DF_Payments_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Payments PRIMARY KEY (Id),
        CONSTRAINT CK_Payments_Provider CHECK (Provider IN (N'Mock', N'2C2P')),
        CONSTRAINT CK_Payments_Status
            CHECK (Status IN (N'Initiated', N'Succeeded', N'Failed')),
        CONSTRAINT FK_Payments_Orders
            FOREIGN KEY (OrderId) REFERENCES dbo.Orders (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Payments_OrderId ON dbo.Payments (OrderId);
    -- Idempotent callbacks: one ProviderRef processed at most once.
    CREATE UNIQUE INDEX UQ_Payments_ProviderRef
        ON dbo.Payments (Provider, ProviderRef)
        WHERE ProviderRef IS NOT NULL;
END
GO

------------------------------------------------------------- DrawResults
IF OBJECT_ID(N'dbo.DrawResults', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DrawResults (
        DrawDate   DATE          NOT NULL,
        FetchedAt  DATETIME2(0)  NOT NULL
                   CONSTRAINT DF_DrawResults_FetchedAt DEFAULT SYSUTCDATETIME(),
        ResultJson NVARCHAR(MAX) NOT NULL,
        CONSTRAINT PK_DrawResults PRIMARY KEY (DrawDate),
        CONSTRAINT CK_DrawResults_Json CHECK (ISJSON(ResultJson) = 1)
    );
END
GO
