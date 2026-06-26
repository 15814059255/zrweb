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

            string finalSql = listSql + " ORDER BY createTime DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            
            SqlParameter[] listParams;
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                listParams = new[] { 
                    DbHelper.CreateParameter("@status", Convert.ToInt32(StatusFilter)),
                    DbHelper.CreateParameter("@offset", (CurrentPage - 1) * PageSize),
                    DbHelper.CreateParameter("@pageSize", PageSize)
                };
            }
            else
            {
                listParams = new[] { 
                    DbHelper.CreateParameter("@offset", (CurrentPage - 1) * PageSize),
                    DbHelper.CreateParameter("@pageSize", PageSize)
                };
            }

            FeedbackList = DbHelper.ExecuteQuery(finalSql, listParams);
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
}