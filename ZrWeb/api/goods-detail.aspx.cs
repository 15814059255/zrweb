using System;
using System.Data;

public partial class api_goods_detail : System.Web.UI.Page
{
    public int GoodsId = 0;
    public int PubType = 0;
    public string GoodsSn = "";
    public string PublisherName = "";
    public string PubTypeText = "";
    public string ShopPrice = "";
    public string GoodsStock = "";
    public string GoodsUnit = "";
    public string CreateTime = "";
    public DataTable QuoteList = null;
    public int QuoteCount = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        string goodsIdStr = Request.QueryString["id"];
        string pubTypeStr = Request.QueryString["pubType"];
        
        if (!string.IsNullOrEmpty(goodsIdStr) && int.TryParse(goodsIdStr, out GoodsId))
        {
            if (!string.IsNullOrEmpty(pubTypeStr) && int.TryParse(pubTypeStr, out PubType))
            {
                LoadGoodsDetail();
                LoadQuoteList();
            }
        }
    }

    private void LoadGoodsDetail()
    {
        try
        {
            string sql = @"SELECT g.goodsId, g.goodsSn, g.shopPrice, g.goodsStock, g.goodsUnit, g.pubType, g.createTime, g.isSale,
                          ISNULL(u.CompanyName, s.shopCompany) AS PublisherName
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          LEFT JOIN userinfo u ON s.userId = u.UserID 
                          WHERE g.goodsId = @goodsId AND g.dataFlag = 1";
            
            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@goodsId", GoodsId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                GoodsSn = GetStringValue(row["goodsSn"]);
                PublisherName = GetStringValue(row["PublisherName"]);
                PubType = GetIntValue(row["pubType"], 0);
                PubTypeText = PubType == 1 ? "供应" : "需求";
                ShopPrice = GetStringValue(row["shopPrice"]);
                GoodsStock = GetStringValue(row["goodsStock"]);
                GoodsUnit = GetStringValue(row["goodsUnit"]);
                
                DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
                CreateTime = createTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadGoodsDetail error: " + ex.Message);
        }
    }

    private void LoadQuoteList()
    {
        try
        {
            int targetEqType = PubType == 1 ? 1 : 2;
            
            string sql = @"SELECT eq.eqId, eq.fromContact AS LinkMan, eq.fromTel AS MobilePhone, eq.fromCompany, 
                          eq.fromQuantity, eq.fromPrice, eq.fromRemarks AS Remarks, 
                          ISNULL(u.CompanyName, s.shopCompany) AS CompanyName, 
                          eq.createTime AS CreateTime, eq.fromPrice AS Price,
                          eq.isIncludingTax, eq.fromLot, eq.brandName, eq.validity,
                          eq.toQuantity, eq.toPrice, eq.toRemarks,
                          eq.fromShopId, eq.toShopId, eq.eqType, eq.goodsSn, eq.goodsId,
                          s.shopQQ
                          FROM enquiryquoteprice eq 
                          LEFT JOIN shops s ON eq.fromShopID = s.shopId 
                          LEFT JOIN userinfo u ON s.userId = u.UserID 
                          WHERE eq.eqType = @eqType AND eq.dataFlag = 1 
                          AND (eq.goodsId = @goodsId OR eq.goodsSn = @goodsSn)
                          ORDER BY eq.createTime DESC";
            
            QuoteList = DbHelper.ExecuteQuery(sql, 
                DbHelper.CreateParameter("@goodsId", GoodsId),
                DbHelper.CreateParameter("@goodsSn", GoodsSn),
                DbHelper.CreateParameter("@eqType", targetEqType));
            QuoteCount = QuoteList != null ? QuoteList.Rows.Count : 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadQuoteList error: " + ex.Message);
            QuoteList = new DataTable();
            QuoteCount = 0;
        }
    }

    private string GetStringValue(object value)
    {
        return value != null && value != DBNull.Value ? value.ToString().Trim() : "";
    }

    private int GetIntValue(object value, int defaultValue)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;
        int result;
        return int.TryParse(value.ToString(), out result) ? result : defaultValue;
    }

    private DateTime GetDateTimeValue(object value, DateTime defaultValue)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;
        DateTime result;
        return DateTime.TryParse(value.ToString(), out result) ? result : defaultValue;
    }
}