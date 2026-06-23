-- 品牌管理表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'brands')
BEGIN
    CREATE TABLE brands (
        BrandId INT IDENTITY(1,1) PRIMARY KEY,
        BrandName NVARCHAR(100) NOT NULL,
        BrandLogo NVARCHAR(255) NULL,
        BrandDesc NVARCHAR(500) NULL,
        SortOrder INT DEFAULT 0,
        Status INT DEFAULT 1,
        CreateTime DATETIME DEFAULT GETDATE()
    )
    
    -- 插入默认品牌
    INSERT INTO brands (BrandName, SortOrder, Status) VALUES
        ('美的', 1, 1),
        ('格力', 2, 1),
        ('海尔', 3, 1),
        ('海信', 4, 1),
        ('TCL', 5, 1)
END
