-- 为 userinfo 表添加地址字段（如果尚未存在）
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND name = 'Province')
BEGIN
    ALTER TABLE [dbo].[userinfo] ADD [Province] nvarchar(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND name = 'City')
BEGIN
    ALTER TABLE [dbo].[userinfo] ADD [City] nvarchar(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND name = 'District')
BEGIN
    ALTER TABLE [dbo].[userinfo] ADD [District] nvarchar(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND name = 'Street')
BEGIN
    ALTER TABLE [dbo].[userinfo] ADD [Street] nvarchar(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND name = 'Address')
BEGIN
    ALTER TABLE [dbo].[userinfo] ADD [Address] nvarchar(255) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND name = 'QQ')
BEGIN
    ALTER TABLE [dbo].[userinfo] ADD [QQ] nvarchar(50) NULL
END
GO

PRINT '字段添加完成'
