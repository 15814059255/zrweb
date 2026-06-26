-- 扩大 shops 表字段长度
ALTER TABLE [dbo].[shops] ALTER COLUMN [shopName] nvarchar(500) NULL;
GO
ALTER TABLE [dbo].[shops] ALTER COLUMN [shopCompany] nvarchar(500) NULL;
GO
ALTER TABLE [dbo].[shops] ALTER COLUMN [shopTel] nvarchar(500) NULL;
GO
ALTER TABLE [dbo].[shops] ALTER COLUMN [shopImg] nvarchar(500) NULL;
GO
ALTER TABLE [dbo].[shops] ALTER COLUMN [shopQQ] nvarchar(255) NULL;
GO
ALTER TABLE [dbo].[shops] ALTER COLUMN [shopAddress] nvarchar(1000) NULL;
GO

-- 扩大 userinfo 表字段长度
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [CompanyName] nvarchar(500) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Province] nvarchar(100) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [City] nvarchar(100) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [District] nvarchar(100) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Street] nvarchar(200) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Address] nvarchar(500) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [LinkMan] nvarchar(100) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [QQ] nvarchar(50) NULL;
GO
ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Email] nvarchar(100) NULL;
GO
