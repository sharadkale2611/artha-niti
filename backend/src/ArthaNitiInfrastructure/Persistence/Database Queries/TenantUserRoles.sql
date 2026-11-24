IF OBJECT_ID('TenantUserRoles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.TenantUserRoles
    (
        TenantUserRoleId INT IDENTITY(1,1) NOT NULL,
        TenantId         INT NOT NULL,
        TenantUserId     INT NOT NULL,
        TenantRoleId     INT NOT NULL,
        AssignedAt       DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT PK_TenantUserRoles
            PRIMARY KEY (TenantUserRoleId),

        CONSTRAINT UQ_TenantUserRoles_UserRole
            UNIQUE (TenantId, TenantUserId, TenantRoleId)
    );
END