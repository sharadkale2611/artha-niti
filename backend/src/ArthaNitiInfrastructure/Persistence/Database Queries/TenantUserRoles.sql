CREATE TABLE TenantUserRoles
(
    TenantUserRoleId INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	TenantId         INT UNIQUE NOT NULL,
    TenantUserId     INT UNIQUE NOT NULL,
    TenantRoleId     INT UNIQUE NOT NULL,
	AssignedAt       DATETIME NOT NULL DEFAULT GETDATE(),

);