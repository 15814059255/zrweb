using System;
using System.Data;
using System.Data.SqlClient;

public partial class admin_users : System.Web.UI.Page
{
    public string AdminName = "";
    public DataTable UserList = null;
    public int TotalUsers = 0;
    public int CurrentPage = 1;
    public int TotalPages = 1;
    private int PageSize = 20;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CheckLogin();
            HandleActions();
            LoadUserList();
        }
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
        string action = Request.QueryString["action"];
        string userId = Request.QueryString["userId"];

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(userId))
        {
            int userID = Convert.ToInt32(userId);
            
            if (action == "check")
            {
                int isCheck = Convert.ToInt32(Request.QueryString["isCheck"]);
                DbHelper.ExecuteNonQuery("UPDATE userinfo SET IsCheck = @isCheck WHERE UserID = @userId",
                    DbHelper.CreateParameter("@isCheck", isCheck),
                    DbHelper.CreateParameter("@userId", userID));
            }
            else if (action == "toggleStatus")
            {
                int status = Convert.ToInt32(Request.QueryString["status"]);
                DbHelper.ExecuteNonQuery("UPDATE userinfo SET SysStatus = @status WHERE UserID = @userId",
                    DbHelper.CreateParameter("@status", status),
                    DbHelper.CreateParameter("@userId", userID));
            }
        }
    }

    private void LoadUserList()
    {
        try
        {
            string keyword = Request.QueryString["keyword"];
            string pageStr = Request.QueryString["page"];
            int page = 1;
            if (!string.IsNullOrEmpty(pageStr) && int.TryParse(pageStr, out page))
            {
                CurrentPage = Math.Max(1, page);
            }

            string countSql = "SELECT COUNT(*) FROM userinfo WHERE SysStatus = 0";
            string listSql = "SELECT UserID, UserName, LinkMan, MobilePhone, IsCheck, SysStatus, CreateTime FROM userinfo WHERE SysStatus = 0";
            
            SqlParameter[] countParams = null;
            
            if (!string.IsNullOrEmpty(keyword))
            {
                countSql += " AND (UserName LIKE @keyword OR MobilePhone LIKE @keyword OR LinkMan LIKE @keyword)";
                listSql += " AND (UserName LIKE @keyword OR MobilePhone LIKE @keyword OR LinkMan LIKE @keyword)";
                countParams = new[] { DbHelper.CreateParameter("@keyword", "%" + keyword + "%") };
            }
            
            object countResult = DbHelper.ExecuteScalar(countSql, countParams);
            TotalUsers = countResult != null && countResult != DBNull.Value ? Convert.ToInt32(countResult) : 0;
            TotalPages = TotalUsers > 0 ? (int)Math.Ceiling((double)TotalUsers / PageSize) : 1;
            
            // 确保当前页不超过总页数
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            string finalSql = listSql + " ORDER BY CreateTime DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            
            SqlParameter[] listParams;
            if (string.IsNullOrEmpty(keyword))
            {
                listParams = new[] { 
                    DbHelper.CreateParameter("@offset", (CurrentPage - 1) * PageSize),
                    DbHelper.CreateParameter("@pageSize", PageSize)
                };
            }
            else
            {
                listParams = new[] { 
                    DbHelper.CreateParameter("@keyword", "%" + keyword + "%"),
                    DbHelper.CreateParameter("@offset", (CurrentPage - 1) * PageSize),
                    DbHelper.CreateParameter("@pageSize", PageSize)
                };
            }

            UserList = DbHelper.ExecuteQuery(finalSql, listParams);
            if (UserList != null)
            {
                rptUsers.DataSource = UserList;
                rptUsers.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadUserList error: " + ex.Message);
            // 发生错误时创建空DataTable防止绑定失败
            UserList = new DataTable();
            rptUsers.DataSource = UserList;
            rptUsers.DataBind();
        }
    }

    public string GetPageUrl(int page)
    {
        string keyword = Request.QueryString["keyword"];
        string url = "/admin-users.aspx?page=" + page;
        if (!string.IsNullOrEmpty(keyword))
        {
            url += "&keyword=" + Server.UrlEncode(keyword);
        }
        return url;
    }
}