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

    INSERT INTO admin_users (AdminName, Password, RealName, Status, CreateTime)
    VALUES ('superadmin', '18d27d4cadb243620ae331138788b3fe', '超级管理员', 1, GETDATE())
END
GO