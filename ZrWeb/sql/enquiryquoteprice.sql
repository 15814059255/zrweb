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

 Date: 14/06/2026 14:43:23
*/


-- ----------------------------
-- Table structure for enquiryquoteprice
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[enquiryquoteprice]') AND type IN ('U'))
	DROP TABLE [dbo].[enquiryquoteprice]
GO

CREATE TABLE [dbo].[enquiryquoteprice] (
  [eqId] int  IDENTITY(1,1) NOT NULL,
  [goodsId] int  NULL,
  [goodsSn] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [eqType] tinyint  NULL,
  [fromShopID] int  NULL,
  [toShopId] int  NULL,
  [dataFlag] tinyint  NULL,
  [toDataFlag] tinyint  NULL,
  [fromDataFlag] tinyint  NULL,
  [readStatus] tinyint  NULL,
  [createTime] datetime  NULL,
  [fromQuantity] int  NULL,
  [toQuantity] int  NULL,
  [toPrice] decimal(18,4)  NULL,
  [fromPrice] decimal(18,4)  NULL,
  [isIncludingTax] tinyint  NULL,
  [pubSource] tinyint  NULL,
  [fromRemarks] nvarchar(400) COLLATE Chinese_PRC_CI_AS  NULL,
  [toRemarks] nvarchar(400) COLLATE Chinese_PRC_CI_AS  NULL,
  [fromContact] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [fromTel] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [fromCompany] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [toContact] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [toTel] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [toCompany] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [toUserId] int  NULL,
  [fromUserId] int  NULL,
  [brandName] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[enquiryquoteprice] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'产品ID',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'goodsId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'产品编号',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'goodsSn'
GO

EXEC sp_addextendedproperty
'MS_Description', N'询报价类型：1-询价，2-报价',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'eqType'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发起方公司ID',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromShopID'
GO

EXEC sp_addextendedproperty
'MS_Description', N'接收方公司ID',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'toShopId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'删除标志：０－删除，１－正常',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'dataFlag'
GO

EXEC sp_addextendedproperty
'MS_Description', N'接收方删除状态：０－删除，１－正常',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'toDataFlag'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发送方删除状态：０－删除，１－正常',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromDataFlag'
GO

EXEC sp_addextendedproperty
'MS_Description', N'读取状态：０－未读，１－已读',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'readStatus'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'createTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'数量',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromQuantity'
GO

EXEC sp_addextendedproperty
'MS_Description', N'价格',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromPrice'
GO

EXEC sp_addextendedproperty
'MS_Description', N'0-不包含税，1-包含税',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'isIncludingTax'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发布来源：1-PC，2-微信手机端',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'pubSource'
GO

EXEC sp_addextendedproperty
'MS_Description', N'發送方备注说明',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromRemarks'
GO

EXEC sp_addextendedproperty
'MS_Description', N'接收方備注説明',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'toRemarks'
GO

EXEC sp_addextendedproperty
'MS_Description', N'發重方聯係人',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromContact'
GO

EXEC sp_addextendedproperty
'MS_Description', N'發出方電話',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromTel'
GO

EXEC sp_addextendedproperty
'MS_Description', N'發出方公司名稱',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'fromCompany'
GO

EXEC sp_addextendedproperty
'MS_Description', N'接收方聯係人',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'toContact'
GO

EXEC sp_addextendedproperty
'MS_Description', N'接收方電話',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'toTel'
GO

EXEC sp_addextendedproperty
'MS_Description', N'接收方公司名稱',
'SCHEMA', N'dbo',
'TABLE', N'enquiryquoteprice',
'COLUMN', N'toCompany'
GO


-- ----------------------------
-- Auto increment value for enquiryquoteprice
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[enquiryquoteprice]', RESEED, 11)
GO


-- ----------------------------
-- Primary Key structure for table enquiryquoteprice
-- ----------------------------
ALTER TABLE [dbo].[enquiryquoteprice] ADD CONSTRAINT [PK__enquiryq__3B8EDE493E225E34] PRIMARY KEY CLUSTERED ([eqId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

