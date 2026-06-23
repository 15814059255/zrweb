-- 超级管理员后台 - 数据库初始化脚本
-- 用于创建管理员表并插入默认账号

-- 创建管理员表
IF NOT EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[admin_users]') AND type IN ('U'))
BEGIN
    CREATE TABLE [dbo].[admin_users] (
        [AdminID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AdminName] nvarchar(50) COLLATE Chinese_PRC_CI_AS NOT NULL,
        [Password] nvarchar(50) COLLATE Chinese_PRC_CI_AS NOT NULL,
        [RealName] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL,
        [Status] int NULL DEFAULT 1,
        [CreateTime] datetime NULL DEFAULT GETDATE(),
        [LastLoginTime] datetime NULL,
        [LastLoginIP] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL
    )

    CREATE UNIQUE NONCLUSTERED INDEX [uc_AdminName] ON [dbo].[admin_users] ([AdminName] ASC)
    PRINT '管理员表创建成功'
END
ELSE
BEGIN
    PRINT '管理员表已存在'
END
GO

-- 删除可能存在的旧管理员账号（如果有）
DELETE FROM admin_users WHERE AdminName = 'superadmin'
GO

-- 插入默认管理员账号
-- 密码: ZrAdmin@2026 的 MD5 哈希值
INSERT INTO admin_users (AdminName, Password, RealName, Status, CreateTime)
VALUES ('superadmin', '18d27d4cadb243620ae331138788b3fe', '超级管理员', 1, GETDATE())
GO

-- 验证插入结果
SELECT AdminID, AdminName, Password, RealName, Status FROM admin_users WHERE AdminName = 'superadmin'
GO
