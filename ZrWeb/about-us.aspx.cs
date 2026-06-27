using System;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class about_us : Page
{
    protected string PageTitle = "关于我们 - 阻容网";
    protected string PageKeywords = "阻容网,关于我们,电子元器件,B2B平台,供需撮合";
    protected string PageDescription = "阻容网是专业的电子元器件B2B交易平台，专注阻容元件供需信息撮合，帮助供应商和采购商快速对接。";
    
    protected string SiteDomain = ConfigurationManager.AppSettings["SiteDomain"] ?? "ZR.net.cn";
    protected string CompanyAddress = "深圳市福田区华强北电子商务产业带";
    protected string ServiceEmail = "service@ZR.net.cn";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["action"] == "submitFeedback")
        {
            HandleSubmitFeedback();
        }
    }

    private void HandleSubmitFeedback()
    {
        try
        {
            EnsureFeedbacksTableExists();
            
            string name = Request["name"] != null ? Request["name"].Trim() : "";
            string contact = Request["contact"] != null ? Request["contact"].Trim() : "";
            string content = Request["content"] != null ? Request["content"].Trim() : "";
            
            if (string.IsNullOrEmpty(name))
            {
                SendJsonResponse(false, "请填写您的称呼");
                return;
            }
            if (string.IsNullOrEmpty(contact))
            {
                SendJsonResponse(false, "请填写联系方式");
                return;
            }
            
            int userId = 0;
            try
            {
                userId = Convert.ToInt32(Session["UserID"] != null ? Session["UserID"] : "0");
            }
            catch { }
            
            string userIP = Request.UserHostAddress;
            if (userIP == "::1") userIP = "127.0.0.1";
            
            string insertSQL = @"INSERT INTO feedbacks (name, contact, content, userId, userIP, status, createTime)
                                VALUES (@name, @contact, @content, @userId, @userIP, 0, GETDATE())";
            
            SqlParameter[] paramsArray = new[] {
                new SqlParameter("@name", SqlDbType.NVarChar, 50) { Value = name },
                new SqlParameter("@contact", SqlDbType.NVarChar, 100) { Value = contact },
                new SqlParameter("@content", SqlDbType.Text) { Value = content },
                new SqlParameter("@userId", SqlDbType.Int) { Value = userId },
                new SqlParameter("@userIP", SqlDbType.NVarChar, 50) { Value = userIP }
            };
            
            int result = DbHelper.ExecuteNonQuery(insertSQL, paramsArray);
            
            if (result > 0)
            {
                SendJsonResponse(true, "留言提交成功");
            }
            else
            {
                SendJsonResponse(false, "插入数据失败，影响行数: " + result);
            }
        }
        catch (Exception ex)
        {
            SendJsonResponse(false, "提交失败: " + ex.Message + " Stack: " + ex.StackTrace);
        }
    }
    
    private void SendJsonResponse(bool success, string message)
    {
        try
        {
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + message.Replace("\"", "\\\"") + "\"}");
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        catch { }
    }
    
    private void EnsureFeedbacksTableExists()
    {
        try
        {
            object exists = DbHelper.ExecuteScalar("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'feedbacks'");
            int count = exists != null && exists != DBNull.Value ? Convert.ToInt32(exists) : 0;
            
            if (count == 0)
            {
                string createSql = @"CREATE TABLE [dbo].[feedbacks] (
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
)";
                
                DbHelper.ExecuteNonQuery(createSql);
                
                try { DbHelper.ExecuteNonQuery("CREATE INDEX [IX_feedbacks_status] ON [dbo].[feedbacks] ([status] ASC)"); } catch { }
                try { DbHelper.ExecuteNonQuery("CREATE INDEX [IX_feedbacks_createTime] ON [dbo].[feedbacks] ([createTime] DESC)"); } catch { }
            }
        }
        catch { }
    }
}