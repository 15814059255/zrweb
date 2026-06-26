-- 创建商品操作日志表
IF NOT EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[goods_operation_log]') AND type IN ('U'))
BEGIN
    CREATE TABLE [dbo].[goods_operation_log] (
        [logId] int IDENTITY(1,1) NOT NULL,
        [goodsId] int NOT NULL,
        [userId] int NOT NULL,
        [shopId] int NULL,
        [operationType] varchar(20) NOT NULL,
        [beforeStatus] int NULL,
        [afterStatus] int NULL,
        [createTime] datetime NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_goods_operation_log] PRIMARY KEY CLUSTERED ([logId])
    )
    CREATE INDEX [IX_goods_operation_log_goodsId] ON [dbo].[goods_operation_log] ([goodsId])
    CREATE INDEX [IX_goods_operation_log_userId] ON [dbo].[goods_operation_log] ([userId])
    CREATE INDEX [IX_goods_operation_log_createTime] ON [dbo].[goods_operation_log] ([createTime])
END
GO
