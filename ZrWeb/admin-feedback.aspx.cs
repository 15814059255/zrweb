using System;
using System.Data;
using System.Data.SqlClient;

public partial class admin_feedback : System.Web.UI.Page
{
    public string AdminName = "";
    public DataTable FeedbackList = null;
    public int TotalFeedbacks = 0;
    public int CurrentPage = 1;
    public int TotalPages = 1;
    public string StatusFilter = null;
    private int PageSize = 20;

    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureFeedbacksTableExists();
        CheckLogin();
        HandleActions();
        LoadFeedbackList();
    }

    private void CheckLogin()
    {
        if (Session["AdminID"] == null)
        {
            Response.Redirect("/admin-login.aspx");
        }
        AdminName = Session["AdminName"] != null ? Session["AdminName"].ToString() : "";
    }

    private void HandleActions()
    {
        string action = Request["action"];
        
        if (action == "reply")
        {
            HandleReply();
            return;
        }
        
        if (action == "delete")
        {
            string idStr = Request["id"];
            int feedbackId = 0;
            if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out feedbackId))
            {
                DbHelper.ExecuteNonQuery("DELETE FROM feedbacks WHERE feedbackId = @id",
                    DbHelper.CreateParameter("@id", feedbackId));
            }
        }
    }

    private void HandleReply()
    {
        try
        {
            string idStr = Request["id"];
            string content = Request["content"] != null ? Request["content"].Trim() : "";
            int feedbackId = 0;
            
            if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out feedbackId))
            {
                if (string.IsNullOrEmpty(content))
                {
                    Response.Write("{\"success\":false,\"message\":\"请填写回复内容\"}");
                    Response.End();
                    return;
                }
                
                int adminId = Session["AdminID"] != null ? Convert.ToInt32(Session["AdminID"]) : 0;
                
                DbHelper.ExecuteNonQuery("UPDATE feedbacks SET status = 1, replyContent = @content, replyTime = GETDATE(), replyAdminId = @adminId WHERE feedbackId = @id",
                    DbHelper.CreateParameter("@content", content),
                    DbHelper.CreateParameter("@adminId", adminId),
                    DbHelper.CreateParameter("@id", feedbackId));
                
                Response.Write("{\"success\":true,\"message\":\"回复成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"参数错误\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"" + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
        Response.End();
    }

    private void LoadFeedbackList()
    {
        try
        {
            StatusFilter = Request.QueryString["status"];
            string pageStr = Request.QueryString["page"];
            int page = 1;
            if (!string.IsNullOrEmpty(pageStr) && int.TryParse(pageStr, out page))
            {
                CurrentPage = Math.Max(1, page);
            }

            string countSql = "SELECT COUNT(*) FROM feedbacks";
            string listSql = "SELECT feedbackId, name, contact, content, status, createTime FROM feedbacks";
            
            SqlParameter[] paramsArray = null;
            
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                countSql += " WHERE status = @status";
                listSql += " WHERE status = @status";
                paramsArray = new[] { DbHelper.CreateParameter("@status", Convert.ToInt32(StatusFilter)) };
            }
            
            object countResult = DbHelper.ExecuteScalar(countSql, paramsArray);
            TotalFeedbacks = countResult != null && countResult != DBNull.Value ? Convert.ToInt32(countResult) : 0;
            TotalPages = TotalFeedbacks > 0 ? (int)Math.Ceiling((double)TotalFeedbacks / PageSize) : 1;
            
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            FeedbackList = DbHelper.ExecuteQuery(listSql + " ORDER BY createTime DESC", paramsArray);
            
            if (FeedbackList != null)
            {
                rptFeedbacks.DataSource = FeedbackList;
                rptFeedbacks.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadFeedbackList error: " + ex.Message);
            FeedbackList = new DataTable();
            rptFeedbacks.DataSource = FeedbackList;
            rptFeedbacks.DataBind();
        }
    }

    public string GetPageUrl(int page)
    {
        string url = "/admin-feedback.aspx?page=" + page;
        if (!string.IsNullOrEmpty(StatusFilter))
        {
            url += "&status=" + StatusFilter;
        }
        return url;
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