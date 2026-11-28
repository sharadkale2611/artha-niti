-- ============================================================
-- 10. TenantAuditLogs
-- Append-only audit history
-- ============================================================

IF OBJECT_ID('TenantAuditLogs', 'U') IS NULL
BEGIN
    CREATE TABLE TenantAuditLogs
    (
        TenantAuditLogId INT IDENTITY(1,1) PRIMARY KEY,

        TenantId INT NOT NULL,
        UserId VARCHAR(450) NULL,

        ModuleCode VARCHAR(100) NOT NULL,
        PluginCode VARCHAR(100) NULL,

        Action VARCHAR(100) NOT NULL,

        EntityName VARCHAR(150) NULL,
        EntityId VARCHAR(100) NULL,

        OldValues VARCHAR(MAX) NULL,
        NewValues VARCHAR(MAX) NULL,

        Description VARCHAR(1000) NULL,

        IpAddress VARCHAR(45) NULL,
        UserAgent VARCHAR(1000) NULL,
        CorrelationId VARCHAR(100) NULL,

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL
    );
END;
