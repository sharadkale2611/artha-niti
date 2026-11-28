IF OBJECT_ID(N'dbo.MenuPermissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MenuPermissions
    (
        MenuPermissionId INT IDENTITY(1,1) PRIMARY KEY,
        MenuId INT NOT NULL,
        PermissionId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL,
    );
END;