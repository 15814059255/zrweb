using System;
using System.Data;
using System.Data.SqlClient;

public partial class admin_goods : System.Web.UI.Page
{
    public string AdminName = "";
    public DataTable GoodsList = null;
    public int TotalGoods = 0;
    public int CurrentPage = 1;
    public int TotalPages = 1;
    private int PageSize = 20;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CheckLogin();
            HandleActions();
            LoadGoodsList();
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
        string goodsId = Request.QueryString["goodsId"];

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(goodsId))
        {
            int goodsID = Convert.ToInt32(goodsId);
            
            if (action == "toggleStatus")
            {
                int status = Convert.ToInt32(Request.QueryString["status"]);
                DbHelper.ExecuteNonQuery("UPDATE goods SET isSale = @status WHERE goodsId = @goodsId",
                    DbHelper.CreateParameter("@status", status),
                    DbHelper.CreateParameter("@goodsId", goodsID));
            }
            else if (action == "delete")
            {
                // 使用 0 表示已删除，兼容 tinyint 类型
                DbHelper.ExecuteNonQuery("UPDATE goods SET dataFlag = 0 WHERE goodsId = @goodsId",
                    DbHelper.CreateParameter("@goodsId", goodsID));
            }
        }
    }

    private void LoadGoodsList()
    {
        try
        {
            string keyword = Request.QueryString["keyword"];
            string pubType = Request.QueryString["pubType"];
            string pageStr = Request.QueryString["page"];
            int page = 1;
            if (!string.IsNullOrEmpty(pageStr) && int.TryParse(pageStr, out page))
            {
                CurrentPage = Math.Max(1, page);
            }

            string countSql = "SELECT COUNT(*) FROM goods WHERE dataFlag = 1";
            string listSql = "SELECT goodsId, goodsSn, shopPrice, goodsStock, goodsUnit, pubType, isSale, createTime FROM goods WHERE dataFlag = 1";
            
            if (!string.IsNullOrEmpty(keyword))
            {
                countSql += " AND goodsSn LIKE @keyword";
                listSql += " AND goodsSn LIKE @keyword";
            }
            
            if (!string.IsNullOrEmpty(pubType))
            {
                countSql += " AND pubType = @pubType";
                listSql += " AND pubType = @pubType";
            }
            
            SqlParameter[] countParams = BuildParams(keyword, pubType, null, null);
            object countResult = DbHelper.ExecuteScalar(countSql, countParams);
            TotalGoods = countResult != null && countResult != DBNull.Value ? Convert.ToInt32(countResult) : 0;
            TotalPages = TotalGoods > 0 ? (int)Math.Ceiling((double)TotalGoods / PageSize) : 1;
            
            // 确保当前页不超过总页数
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            string finalSql = listSql + " ORDER BY createTime DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            SqlParameter[] listParams = BuildParams(keyword, pubType, (CurrentPage - 1) * PageSize, PageSize);
            
            GoodsList = DbHelper.ExecuteQuery(finalSql, listParams);
            if (GoodsList != null)
            {
                rptGoods.DataSource = GoodsList;
                rptGoods.DataBind();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadGoodsList error: " + ex.Message);
            // 发生错误时创建空DataTable防止绑定失败
            GoodsList = new DataTable();
            rptGoods.DataSource = GoodsList;
            rptGoods.DataBind();
        }
    }

    private SqlParameter[] BuildParams(string keyword, string pubType, int? offset, int? pageSize)
    {
        var paramsList = new System.Collections.Generic.List<SqlParameter>();
        
        if (!string.IsNullOrEmpty(keyword))
        {
            paramsList.Add(DbHelper.CreateParameter("@keyword", "%" + keyword + "%"));
        }
        
        if (!string.IsNullOrEmpty(pubType))
        {
            paramsList.Add(DbHelper.CreateParameter("@pubType", Convert.ToInt32(pubType)));
        }
        
        if (offset.HasValue)
        {
            paramsList.Add(DbHelper.CreateParameter("@offset", offset.Value));
        }
        
        if (pageSize.HasValue)
        {
            paramsList.Add(DbHelper.CreateParameter("@pageSize", pageSize.Value));
        }
        
        return paramsList.ToArray();
    }

    public string GetPageUrl(int page)
    {
        string keyword = Request.QueryString["keyword"];
        string pubType = Request.QueryString["pubType"];
        string url = "/admin-goods.aspx?page=" + page;
        if (!string.IsNullOrEmpty(keyword))
        {
            url += "&keyword=" + Server.UrlEncode(keyword);
        }
        if (!string.IsNullOrEmpty(pubType))
        {
            url += "&pubType=" + pubType;
        }
        return url;
    }
}