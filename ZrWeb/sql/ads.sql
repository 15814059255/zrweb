/*
 创建广告表
*/

IF NOT EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ads]') AND type IN ('U'))
BEGIN
    CREATE TABLE [dbo].[ads] (
        [AdID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AdSlot] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL,
        [Title] nvarchar(100) COLLATE Chinese_PRC_CI_AS NULL,
        [Position] nvarchar(20) COLLATE Chinese_PRC_CI_AS NULL,
        [LinkUrl] nvarchar(255) COLLATE Chinese_PRC_CI_AS NULL,
        [StartDate] date NULL,
        [EndDate] date NULL,
        [Status] int NULL DEFAULT 1,
        [CreateTime] datetime NULL DEFAULT GETDATE()
    )

    CREATE NONCLUSTERED INDEX [idx_ads_AdSlot] ON [dbo].[ads] ([AdSlot] ASC)
    CREATE NONCLUSTERED INDEX [idx_ads_Status] ON [dbo].[ads] ([Status] ASC)
END
GO