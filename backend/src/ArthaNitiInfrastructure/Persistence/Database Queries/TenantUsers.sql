CREATE TABLE dbo.TenantUsers
(
    TenantUserId INT IDENTITY(1,1) PRIMARY KEY,
    TenantId     INT NOT NULL DEFAULT 1,
    UserId       VARCHAR(450) NOT NULL,

    IsPrimary    BIT NOT NULL DEFAULT 0,
    Status       INT NOT NULL DEFAULT 1,

    JoinedAt     DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive     BIT NOT NULL DEFAULT 1,
    IsDeleted    BIT NOT NULL DEFAULT 0,
    CreatedAt    DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy    VARCHAR(450) NULL,
    UpdatedAt    DATETIME NULL,
    UpdatedBy    VARCHAR(450) NULL
);