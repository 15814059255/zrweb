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
            SetPubTypeSelected();
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

    private void SetPubTypeSelected()
    {
        string pubType = Request.QueryString["pubType"];
        if (!string.IsNullOrEmpty(pubType))
        {
            selPubType.Value = pubType;
        }
    }

    private void HandleActions()
    {
        string action = Request.QueryString["action"];

        if (!string.IsNullOrEmpty(action))
        {
            if (action == "batchToggleStatus")
            {
                string goodsIds = Request.Form["goodsIds"];
                int status = Convert.ToInt32(Request.Form["status"]);
                if (!string.IsNullOrEmpty(goodsIds))
                {
                    string[] idArray = goodsIds.Split(',');
                    foreach (string id in idArray)
                    {
                        int goodsID;
                        if (int.TryParse(id.Trim(), out goodsID))
                        {
                            DbHelper.ExecuteNonQuery("UPDATE goods SET isSale = @status WHERE goodsId = @goodsId",
                                DbHelper.CreateParameter("@status", status),
                                DbHelper.CreateParameter("@goodsId", goodsID));
                        }
                    }
                }
            }
            else if (action == "batchDelete")
            {
                string goodsIds = Request.Form["goodsIds"];
                if (!string.IsNullOrEmpty(goodsIds))
                {
                    string[] idArray = goodsIds.Split(',');
                    foreach (string id in idArray)
                    {
                        int goodsID;
                        if (int.TryParse(id.Trim(), out goodsID))
                        {
                            DbHelper.ExecuteNonQuery("UPDATE goods SET dataFlag = 0 WHERE goodsId = @goodsId",
                                DbHelper.CreateParameter("@goodsId", goodsID));
                        }
                    }
                }
            }
            else
            {
                string goodsId = Request.QueryString["goodsId"];
                int goodsID;
                if (!string.IsNullOrEmpty(goodsId) && int.TryParse(goodsId, out goodsID))
                {
                    if (action == "toggleStatus")
                    {
                        int status = Convert.ToInt32(Request.QueryString["status"]);
                        DbHelper.ExecuteNonQuery("UPDATE goods SET isSale = @status WHERE goodsId = @goodsId",
                            DbHelper.CreateParameter("@status", status),
                            DbHelper.CreateParameter("@goodsId", goodsID));
                    }
                    else if (action == "delete")
                    {
                        DbHelper.ExecuteNonQuery("UPDATE goods SET dataFlag = 0 WHERE goodsId = @goodsId",
                            DbHelper.CreateParameter("@goodsId", goodsID));
                    }
                }
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

            string countSql = "SELECT COUNT(*) FROM goods g LEFT JOIN shops s ON g.shopId = s.shopId LEFT JOIN userinfo u ON s.userId = u.UserID WHERE g.dataFlag = 1";
            string listSql = @"SELECT g.goodsId, g.goodsSn, g.shopPrice, g.goodsStock, g.goodsUnit, g.pubType, g.isSale, g.createTime, 
                          ISNULL(u.CompanyName, s.shopCompany) AS PublisherName,
                          (SELECT COUNT(*) FROM enquiryquoteprice eq WHERE eq.dataFlag = 1 AND eq.goodsId = g.goodsId AND eq.eqType = CASE WHEN g.pubType = 1 THEN 1 ELSE 2 END) AS InteractionCount
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          LEFT JOIN userinfo u ON s.userId = u.UserID 
                          WHERE g.dataFlag = 1";
            
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