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

 Date: 14/06/2026 13:36:57
*/


-- ----------------------------
-- Table structure for userinfo
-- ----------------------------
IF EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[userinfo]') AND type IN ('U'))
	DROP TABLE [dbo].[userinfo]
GO

CREATE TABLE [dbo].[userinfo] (
  [UserID] int  IDENTITY(1,1) NOT NULL,
  [UserName] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Password] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [UserGuid] nvarchar(40) COLLATE Chinese_PRC_CI_AS  NULL,
  [SysStatus] int  NULL,
  [LinkMan] nvarchar(100) COLLATE Chinese_PRC_CI_AS  NULL,
  [MobilePhone] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [RoseID] int  NULL,
  [IsCheck] int  NULL,
  [CreateTime] datetime  NULL,
  [Email] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [UserImg] nvarchar(512) COLLATE Chinese_PRC_CI_AS  NULL,
  [IDCardName] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [IDCardNumber] nvarchar(50) COLLATE Chinese_PRC_CI_AS  NULL,
  [Source] int  NULL
)
GO

ALTER TABLE [dbo].[userinfo] SET (LOCK_ESCALATION = TABLE)
GO

EXEC sp_addextendedproperty
'MS_Description', N'主键，自增',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'UserID'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户名',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'UserName'
GO

EXEC sp_addextendedproperty
'MS_Description', N'密码',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'Password'
GO

EXEC sp_addextendedproperty
'MS_Description', N'用户标识',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'UserGuid'
GO

EXEC sp_addextendedproperty
'MS_Description', N'系统状态：0-正常，1-删除',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'SysStatus'
GO

EXEC sp_addextendedproperty
'MS_Description', N'联系人',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'LinkMan'
GO

EXEC sp_addextendedproperty
'MS_Description', N'手机',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'MobilePhone'
GO

EXEC sp_addextendedproperty
'MS_Description', N'角色：1-普通用户',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'RoseID'
GO

EXEC sp_addextendedproperty
'MS_Description', N'是否审核： 0-未审核，1-审核通',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'IsCheck'
GO

EXEC sp_addextendedproperty
'MS_Description', N'创建时间',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'CreateTime'
GO

EXEC sp_addextendedproperty
'MS_Description', N'电子邮件',
'SCHEMA', N'dbo',
'TABLE', N'userinfo',
'COLUMN', N'Email'
GO


-- ----------------------------
-- Auto increment value for userinfo
-- ----------------------------
DBCC CHECKIDENT ('[dbo].[userinfo]', RESEED, 3)
GO


-- ----------------------------
-- Indexes structure for table userinfo
-- ----------------------------
CREATE UNIQUE NONCLUSTERED INDEX [uc_UserName]
ON [dbo].[userinfo] (
  [UserName] ASC
)
GO


-- ----------------------------
-- Primary Key structure for table userinfo
-- ----------------------------
ALTER TABLE [dbo].[userinfo] ADD CONSTRAINT [PK__userinfo__1788CCACC9A5003C] PRIMARY KEY CLUSTERED ([UserID])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
ON [PRIMARY]
GO

