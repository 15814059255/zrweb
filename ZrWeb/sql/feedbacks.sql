CREATE TABLE [dbo].[feedbacks] (
  [feedbackId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [name] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL,
  [contact] nvarchar(100) COLLATE Chinese_PRC_CI_AS NULL,
  [content] text COLLATE Chinese_PRC_CI_AS NULL,
  [userId] int NULL,
  [userIP] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL,
  [status] tinyint DEFAULT 0,
  [createTime] datetime DEFAULT GETDATE(),
  [replyContent] text COLLATE Chinese_PRC_CI_AS NULL,
  [replyTime] datetime NULL,
  [replyAdminId] int NULL
)

GO

CREATE INDEX [IX_feedbacks_status] ON [dbo].[feedbacks] ([status] ASC)
CREATE INDEX [IX_feedbacks_createTime] ON [dbo].[feedbacks] ([createTime] DESC)
