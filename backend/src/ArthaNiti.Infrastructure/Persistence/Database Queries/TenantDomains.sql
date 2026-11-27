-- DomainType: 1 = Subdomain, 2 = Custom Domain
IF OBJECT_ID(N'dbo.TenantDomains', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TenantDomains
    (
        TenantDomainId INT IDENTITY(1,1) PRIMARY KEY,
        TenantId INT NOT NULL,
        DomainName VARCHAR(253) NOT NULL,
        DomainType INT NOT NULL DEFAULT 1,
        IsPrimary BIT NOT NULL DEFAULT 0,
        IsVerified BIT NOT NULL DEFAULT 0,
        VerificationToken VARCHAR(256) NULL,
        VerifiedAt DATETIME NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL
    );
END;