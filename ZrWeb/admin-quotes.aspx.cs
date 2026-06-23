using System;
using System.Data;
using System.Data.SqlClient;

public partial class admin_quotes : System.Web.UI.Page
{
    public string AdminName = "";
    public DataTable QuoteList = null;
    public int TotalQuotes = 0;
    public int CurrentPage = 1;
    public int TotalPages = 1;
    private int PageSize = 20;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CheckLogin();
            HandleActions();
            LoadQuoteList();
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
        string eqId = Request.QueryString["eqId"];

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(eqId))
        {
            int EQID = Convert.ToInt32(eqId);
            
            if (action == "delete")
            {
                // 使用 0 表示已删除，兼容 tinyint 类型
                DbHelper.ExecuteNonQuery("UPDATE enquiryquoteprice SET dataFlag = 0 WHERE eqId = @eqId",
                    DbHelper.CreateParameter("@eqId", EQID));
            }
        }
    }

    private void LoadQuoteList()
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

            string countSql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE dataFlag = 1";
            string listSql = "SELECT eqId, goodsSn, fromCompany, toCompany, fromPrice, fromQuantity, createTime FROM enquiryquoteprice WHERE dataFlag = 1";
            
            SqlParameter[] countParams = null;
            
            if (!string.IsNullOrEmpty(keyword))
            {
                countSql += " AND (goodsSn LIKE @keyword OR fromCompany LIKE @keyword OR toCompany LIKE @keyword)";
                listSql += " AND (goodsSn LIKE @keyword OR fromCompany LIKE @keyword OR toCompany LIKE @keyword)";
                countParams = new[] { DbHelper.CreateParameter("@keyword", "%" + keyword + "%") };
            }
            
            object countResult = DbHelper.ExecuteScalar(countSql, countParams);
            TotalQuotes = countResult != null && countResult != DBNull.Value ? Convert.ToInt32(countResult) : 0;
            TotalPages = TotalQuotes > 0 ? (int)Math.Ceiling((double)TotalQuotes / PageSize) : 1;
            
            // 确保当前页不超过总页数
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            string finalSql = listSql + " ORDER BY createTime DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            
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

            QuoteList = DbHelper.ExecuteQuery(finalSql, listParams);
            if (QuoteList != null)
            {
                rptQuotes.DataSource = QuoteList;
                rptQuotes.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadQuoteList error: " + ex.Message);
            // 发生错误时创建空DataTable防止绑定失败
            QuoteList = new DataTable();
            rptQuotes.DataSource = QuoteList;
            rptQuotes.DataBind();
        }
    }

    public string GetPageUrl(int page)
    {
        string keyword = Request.QueryString["keyword"];
        string url = "/admin-quotes.aspx?page=" + page;
        if (!string.IsNullOrEmpty(keyword))
        {
            url += "&keyword=" + Server.UrlEncode(keyword);
        }
        return url;
    }
}