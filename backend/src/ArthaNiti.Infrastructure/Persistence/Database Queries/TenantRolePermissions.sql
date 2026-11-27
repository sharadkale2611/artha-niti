-- ============================================================
-- 07. TenantRolePermissions
-- ============================================================

IF OBJECT_ID('TenantRolePermissions', 'U') IS NULL
BEGIN
    CREATE TABLE TenantRolePermissions
    (
        TenantRolePermissionId INT IDENTITY(1,1) PRIMARY KEY,

        TenantId INT NOT NULL,
        TenantRoleId INT NOT NULL,
        PermissionId INT NOT NULL,

        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,

        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL,

        CONSTRAINT UQ_TenantRolePermissions_Assignment UNIQUE (TenantId, TenantRoleId, PermissionId)
    );
END;
