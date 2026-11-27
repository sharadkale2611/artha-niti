--============================================================
-- 05. TenantSettings
-- ============================================================

IF OBJECT_ID('TenantSettings', 'U') IS NULL
BEGIN
    CREATE TABLE TenantSettings
    (
        TenantSettingId INT IDENTITY(1,1) PRIMARY KEY,

        TenantId INT NOT NULL,

        SettingKey VARCHAR(150) NOT NULL,
        SettingValue VARCHAR(MAX) NULL,
        ValueType VARCHAR(30) NOT NULL DEFAULT 'String',

        Description VARCHAR(500) NULL,

        IsEncrypted BIT NOT NULL DEFAULT 0,
        IsSystemSetting BIT NOT NULL DEFAULT 0,

        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,

        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL,

        CONSTRAINT UQ_TenantSettings_TenantId_SettingKey UNIQUE (TenantId, SettingKey)
    );
END;