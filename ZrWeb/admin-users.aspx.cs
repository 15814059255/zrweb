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
            SetRoleSelected();
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

    private void SetRoleSelected()
    {
        string role = Request.QueryString["role"];
        if (!string.IsNullOrEmpty(role))
        {
            selRole.Value = role;
        }
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
            string role = Request.QueryString["role"];
            string pageStr = Request.QueryString["page"];
            int page = 1;
            if (!string.IsNullOrEmpty(pageStr) && int.TryParse(pageStr, out page))
            {
                CurrentPage = Math.Max(1, page);
            }

            string countSql = "SELECT COUNT(*) FROM userinfo WHERE 1=1";
            string listSql = "SELECT UserID, UserName, LinkMan, MobilePhone, RoseID, IsCheck, SysStatus, CreateTime FROM userinfo WHERE 1=1";
            
            SqlParameter[] countParams = null;
            
            // 状态/角色筛选
            if (role == "disabled")
            {
                countSql += " AND SysStatus = 1";
                listSql += " AND SysStatus = 1";
            }
            else
            {
                countSql += " AND SysStatus = 0";
                listSql += " AND SysStatus = 0";
            }
            
            if (!string.IsNullOrEmpty(keyword))
            {
                countSql += " AND (UserName LIKE @keyword OR MobilePhone LIKE @keyword OR LinkMan LIKE @keyword)";
                listSql += " AND (UserName LIKE @keyword OR MobilePhone LIKE @keyword OR LinkMan LIKE @keyword)";
            }
            
            if (!string.IsNullOrEmpty(role) && role != "disabled")
            {
                countSql += " AND RoseID = @role";
                listSql += " AND RoseID = @role";
            }
            
            var paramList = new System.Collections.Generic.List<SqlParameter>();
            if (!string.IsNullOrEmpty(keyword))
            {
                paramList.Add(DbHelper.CreateParameter("@keyword", "%" + keyword + "%"));
            }
            if (!string.IsNullOrEmpty(role) && role != "disabled")
            {
                paramList.Add(DbHelper.CreateParameter("@role", Convert.ToInt32(role)));
            }
            countParams = paramList.ToArray();
            
            object countResult = DbHelper.ExecuteScalar(countSql, countParams);
            TotalUsers = countResult != null && countResult != DBNull.Value ? Convert.ToInt32(countResult) : 0;
            TotalPages = TotalUsers > 0 ? (int)Math.Ceiling((double)TotalUsers / PageSize) : 1;
            
            // 确保当前页不超过总页数
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            string finalSql = listSql + " ORDER BY CreateTime DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            
            var listParamList = new System.Collections.Generic.List<SqlParameter>();
            if (!string.IsNullOrEmpty(keyword))
            {
                listParamList.Add(DbHelper.CreateParameter("@keyword", "%" + keyword + "%"));
            }
            if (!string.IsNullOrEmpty(role) && role != "disabled")
            {
                listParamList.Add(DbHelper.CreateParameter("@role", Convert.ToInt32(role)));
            }
            listParamList.Add(DbHelper.CreateParameter("@offset", (CurrentPage - 1) * PageSize));
            listParamList.Add(DbHelper.CreateParameter("@pageSize", PageSize));
            
            SqlParameter[] listParams = listParamList.ToArray();

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
        string role = Request.QueryString["role"];
        string url = "/admin-users.aspx?page=" + page;
        if (!string.IsNullOrEmpty(keyword))
        {
            url += "&keyword=" + Server.UrlEncode(keyword);
        }
        if (!string.IsNullOrEmpty(role))
        {
            url += "&role=" + role;
        }
        return url;
    }

    public string GetRoleName(object roseIdObj)
    {
        int roseId = 0;
        if (roseIdObj != null && roseIdObj != DBNull.Value)
        {
            int.TryParse(roseIdObj.ToString(), out roseId);
        }
        switch (roseId)
        {
            case 1: return "普通用户";
            case 2: return "采购商";
            case 3: return "供应商";
            default: return "普通用户";
        }
    }

    public string GetRoleTagClass(object roseIdObj)
    {
        int roseId = 0;
        if (roseIdObj != null && roseIdObj != DBNull.Value)
        {
            int.TryParse(roseIdObj.ToString(), out roseId);
        }
        switch (roseId)
        {
            case 2: return "blue";
            case 3: return "purple";
            default: return "gray";
        }
    }
}