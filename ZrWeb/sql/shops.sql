/*
 Navicat SQL Server Data Transfer

 Source Server         : www123ic.sqlserver.rds.aliyuncs.com
 Source Server Type    : SQL Server
 Source Server Version : 14003460
 Source Host           : www123ic.sqlserver.rds.aliyuncs.com:3433
 Source Catalog        : hjhdb
 Source Schema         : dbo

 Target Server Type    : SQL Server
 Target Server Version : 14003460
 File Encoding         : 65001

 Date: 14/06/2026 15:00:56
*/


-- ----------------------------
-- Table structure for shops
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[shops]') AND type IN ('U'))
	DROP TABLE [dbo].[shops]
GO

CREATE TABLE [dbo].[shops] (
  [shopId] int  IDENTITY(1,1) NOT NULL,
  [shopSn] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [userId] int  NULL,
  [areaIdPath] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [areaId] int  NULL,
  [isSelf] tinyint  NULL,
  [shopName] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [shopkeeper] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [telephone] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NOT NULL,
  [shopCompany] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [shopImg] nvarchar(150) COLLATE Chinese_PRC_CI_AS  NULL,
  [shopTel] nvarchar(40) COLLATE Chinese_PRC_CI_AS  NULL,
  [shopQQ] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [shopAddress] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [bankId] int  NULL,
  [bankNo] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [bankUserName] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [isInvoice] tinyint  NULL,
  [invoiceRemarks] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [serviceStartTime] datetime  NULL,
  [serviceEndTime] datetime  NULL,
  [freight] decimal(18,2)  NULL,
  [shopAtive] tinyint  NULL,
  [shopStatus] tinyint  NOT NULL,
  [statusDesc] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [dataFlag] tinyint  NULL,
  [createTime] datetime  NULL,
  [bankAreaId] int  NULL,
  [bankAreaIdPath] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[shops] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店编号,供应商编号',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopSn'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店所有人ID	',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'userId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'区域路径',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'areaIdPath'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最终所属区域ID',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'areaId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否自营	1:自营 0:非自营',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'isSelf'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店名称',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopName'
GO

EXEC sp_addextendedproperty
'MS_Description', N'店主',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopkeeper'
GO

EXEC sp_addextendedproperty
'MS_Description', N'店主手机号',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'telephone'
GO

EXEC sp_addextendedproperty
'MS_Description', N'公司名称',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopCompany'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店图标，营业执照',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopImg'
GO

EXEC sp_addextendedproperty
'MS_Description', N'银行ID',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'bankId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'银行卡号',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'bankNo'
GO

EXEC sp_addextendedproperty
'MS_Description', N'银行卡所有人名称',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'bankUserName'
GO

EXEC sp_addextendedproperty
'MS_Description', N'能否开发票	1:能 0:不能',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'isInvoice'
GO

EXEC sp_addextendedproperty
'MS_Description', N'服务营业开始时间',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'serviceStartTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'服务营业结束时间',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'serviceEndTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'默认运费',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'freight'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店状态	1:营业中 0：休息中',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopAtive'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店审核状态 -2:已停止 -1:拒绝 0：未审核 1:已审核',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'shopStatus'
GO

EXEC sp_addextendedproperty
'MS_Description', N'状态说明	一般用于停止和拒绝说明',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'statusDesc'
GO

EXEC sp_addextendedproperty
'MS_Description', N'删除标志	-1:删除 1:有效',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'dataFlag'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'createTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'银行账号开卡地区',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'bankAreaId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'银行账号开卡地区ID路径',
'SCHEMA', N'dbo',
'TABLE', N'shops',
'COLUMN', N'bankAreaIdPath'
GO


-- ----------------------------
-- Auto increment value for shops
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[shops]', RESEED, 43)
GO


-- ----------------------------
-- Primary Key structure for table shops
-- ----------------------------
ALTER TABLE [dbo].[shops] ADD CONSTRAINT [PK__shops__E5C424DC434AC810] PRIMARY KEY CLUSTERED ([shopId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

