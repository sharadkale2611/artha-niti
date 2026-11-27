-- - ============================================================
-- 04. TenantUserRoles
-- ============================================================

IF OBJECT_ID('TenantUserRoles', 'U') IS NULL
BEGIN
    CREATE TABLE TenantUserRoles
    (
        TenantUserRoleId INT IDENTITY(1,1) PRIMARY KEY,

        TenantId INT NOT NULL,
        TenantUserId INT NOT NULL,
        TenantRoleId INT NOT NULL,

        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,

        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL,

        CONSTRAINT UQ_TenantUserRoles_Assignment UNIQUE (TenantId, TenantUserId, TenantRoleId)
    );
END;