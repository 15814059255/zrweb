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

 Date: 14/06/2026 14:57:51
*/


-- ----------------------------
-- Table structure for goods
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[goods]') AND type IN ('U'))
	DROP TABLE [dbo].[goods]
GO

CREATE TABLE [dbo].[goods] (
  [goodsId] int  IDENTITY(1,1) NOT NULL,
  [goodsSn] nvarchar(30) COLLATE Chinese_PRC_CI_AS  NULL,
  [productNo] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [Manufacturers] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Packaging] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Lot] nvarchar(20) COLLATE Chinese_PRC_CI_AS  NULL,
  [Name] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [goodsImg] nvarchar(150) COLLATE Chinese_PRC_CI_AS  NULL,
  [shopId] int  NULL,
  [marketPrice] decimal(18,2)  NULL,
  [shopPrice] decimal(18,2)  NULL,
  [warnStock] int  NULL,
  [goodsStock] int  NULL,
  [goodsUnit] nchar(10) COLLATE Chinese_PRC_CI_AS  NULL,
  [isSale] tinyint  NULL,
  [isBest] tinyint  NULL,
  [isHot] tinyint  NULL,
  [isNew] tinyint  NULL,
  [isRecom] tinyint  NULL,
  [goodsCatIdPath] nvarchar(255) COLLATE Chinese_PRC_CI_AS  NULL,
  [goodsCatId] int  NULL,
  [brandId] int  NULL,
  [goodsDesc] text COLLATE Chinese_PRC_CI_AS  NULL,
  [goodsStatus] tinyint  NULL,
  [saleNum] int  NULL,
  [saleTime] datetime  NULL,
  [visitNum] int  NULL,
  [appraiseNum] int  NULL,
  [isSpec] tinyint  NULL,
  [goodsSeoKeywords] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [galleryId] int  NULL,
  [illegalRemarks] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [dataFlag] tinyint  NULL,
  [createTime] datetime  NULL,
  [updateTime] datetime  NULL,
  [isIncludingTax] tinyint  NULL,
  [validityDate] datetime  NULL,
  [pubType] tinyint  NULL,
  [pubSource] tinyint  NULL,
  [remarks] nvarchar(300) COLLATE Chinese_PRC_CI_AS  NULL
)
GO

ALTER TABLE [dbo].[goods] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品编号	',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsSn'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品货号',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'productNo'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品名称',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'Name'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品图片',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsImg'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店ID,供应商ID',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'shopId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'市场价格，对外报价',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'marketPrice'
GO

EXEC sp_addextendedproperty
'MS_Description', N'门店价格，工厂价格',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'shopPrice'
GO

EXEC sp_addextendedproperty
'MS_Description', N'预警库存，库存不够时提醒',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'warnStock'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品总库存',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsStock'
GO

EXEC sp_addextendedproperty
'MS_Description', N'单位,如：盘，批，个',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsUnit'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否上架,0:不上架 1:上架',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isSale'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否精品	0:否 1:是',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isBest'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否热销产品	0:否 1:是',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isHot'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否新品	0:否 1:是',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isNew'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否推荐	0:否 1:是',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isRecom'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品分类ID路径	catId1_catId2_catId3',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsCatIdPath'
GO

EXEC sp_addextendedproperty
'MS_Description', N'最后一级商品分类ID',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsCatId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'品牌Id',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'brandId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品描述',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsDesc'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品状态	-1:违规 0:未审核 1:已审核',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsStatus'
GO

EXEC sp_addextendedproperty
'MS_Description', N'总销售量',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'saleNum'
GO

EXEC sp_addextendedproperty
'MS_Description', N'上架时间',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'saleTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'访问数',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'visitNum'
GO

EXEC sp_addextendedproperty
'MS_Description', N'评价数',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'appraiseNum'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否有规格	0:没有 1:有',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isSpec'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品SEO关键字',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'goodsSeoKeywords'
GO

EXEC sp_addextendedproperty
'MS_Description', N'商品相册ID',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'galleryId'
GO

EXEC sp_addextendedproperty
'MS_Description', N'状态说明	一般用于说明拒绝原因',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'illegalRemarks'
GO

EXEC sp_addextendedproperty
'MS_Description', N'删除标志	-1:删除 1:有效',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'dataFlag'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间	',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'createTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'更新时间',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'updateTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'0-不包含税，1-包含税',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'isIncludingTax'
GO

EXEC sp_addextendedproperty
'MS_Description', N'有效期',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'validityDate'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发布类型：1-供应，2-采购',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'pubType'
GO

EXEC sp_addextendedproperty
'MS_Description', N'发布来源：1-PC，2-微信手机端',
'SCHEMA', N'dbo',
'TABLE', N'goods',
'COLUMN', N'pubSource'
GO


-- ----------------------------
-- Auto increment value for goods
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[goods]', RESEED, 27757)
GO


-- ----------------------------
-- Indexes structure for table goods
-- ----------------------------
CREATE NONCLUSTERED INDEX [productNo]
ON [dbo].[goods] (
  [productNo] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table goods
-- ----------------------------
ALTER TABLE [dbo].[goods] ADD CONSTRAINT [PK__goods__110ED9F268A659E7] PRIMARY KEY CLUSTERED ([goodsId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

