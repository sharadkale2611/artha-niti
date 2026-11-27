IF OBJECT_ID('Menus', 'U') IS NULL
BEGIN
    CREATE TABLE Menus
    (
        MenuId INT IDENTITY(1,1) PRIMARY KEY,

        ParentMenuId INT NULL,

        Code VARCHAR(150) NOT NULL,
        Name VARCHAR(150) NOT NULL,
        Description VARCHAR(500) NULL,

        Route VARCHAR(500) NULL,
        Icon VARCHAR(100) NULL,

        ModuleCode VARCHAR(100) NOT NULL,
        PluginCode VARCHAR(100) NULL,

        MenuScope INT NOT NULL DEFAULT 2,
        MenuType INT NOT NULL DEFAULT 2,
        MenuLocation INT NOT NULL DEFAULT 1,

        DisplayOrder INT NOT NULL DEFAULT 0,

        IsVisible BIT NOT NULL DEFAULT 1,
        IsSystemMenu BIT NOT NULL DEFAULT 0,

        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,

        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy VARCHAR(450) NULL,

        UpdatedAt DATETIME NULL,
        UpdatedBy VARCHAR(450) NULL,

        CONSTRAINT UQ_Menus_Code UNIQUE (Code),
        CONSTRAINT CK_Menus_ParentMenu CHECK (ParentMenuId IS NULL OR ParentMenuId <> MenuId)
    );
END;
