IF OBJECT_ID(N'dbo.TenantRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TenantRoles
    (
        TenantRoleId INT IDENTITY(1,1) PRIMARY KEY,
        TenantId INT NOT NULL,
        Code VARCHAR(100) NOT NULL,
        Name VARCHAR(150) NOT NULL,
        Description VARCHAR(500) NULL,
        IsSystemRole BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL
    );
END;
