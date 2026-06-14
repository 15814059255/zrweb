using System;
using System.Data;

public partial class merchant_workbench : System.Web.UI.Page
{
    public string PageTitle = "我是商家 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,商家工作台,库存管理,电子元器件";
    public string PageDescription = "阻容网商家工作台，管理库存、查看询价、提交报价，一站式管理您的电子元器件供应业务。";
    
    public int OnlineSupplyCount = 3;
    public int InquiryCount = 7;
    public int NewInquiryCount = 7;
    public int QuoteCount = 3;
    public int ExpiredCount = 4;
    public int CurrentPage = 1;
    public int TotalPages = 49;
    public bool HasInventoryData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        // 处理AJAX请求
        if (Request["action"] == "publish_goods")
        {
            HandlePublishGoods();
            return;
        }

        if (Request["action"] == "take_off")
        {
            HandleTakeOff();
            return;
        }

        if (Request["action"] == "restock")
        {
            HandleRestock();
            return;
        }

        LoadStats();
        BindInventory();
        BindExpiredInventory();
    }

    private void HandlePublishGoods()
    {
        Response.ContentType = "application/json";
        try
        {
            string goodsSn = Request["goodsSn"];
            string name = Request["name"];
            string manufacturers = Request["manufacturers"];
            
            int goodsStock = 0;
            int.TryParse(Request["goodsStock"], out goodsStock);
            
            string goodsUnit = Request["goodsUnit"];
            if (string.IsNullOrEmpty(goodsUnit)) goodsUnit = "Kpcs";
            
            decimal shopPrice = 0;
            decimal.TryParse(Request["shopPrice"], out shopPrice);
            
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            
            int pubType = 1;
            int.TryParse(Request["pubType"], out pubType);

            if (string.IsNullOrEmpty(goodsSn))
            {
                Response.Write("{\"success\":false,\"message\":\"请输入型号\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }

            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            if (shopId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.InsertGoods(
                goodsSn, name, manufacturers, "",
                goodsStock, goodsUnit, shopPrice, isIncludingTax,
                pubType, "", shopId, userId);

            if (success)
            {
                Response.Write("{\"success\":true,\"message\":\"发布成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"发布失败\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
        Response.End();
    }

    private void HandleTakeOff()
    {
        Response.ContentType = "application/json";
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.TakeOff(goodsId);

            if (success)
            {
                Response.Write("{\"success\":true,\"message\":\"下架成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"下架失败\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
        Response.End();
    }

    private void HandleRestock()
    {
        Response.ContentType = "application/json";
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.Restock(goodsId);

            if (success)
            {
                Response.Write("{\"success\":true,\"message\":\"重新上架成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"重新上架失败\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
        Response.End();
    }

    private void LoadStats()
    {
        try
        {
            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            GoodsService service = new GoodsService();
            OnlineSupplyCount = shopId > 0 ? service.GetOnlineSupplyCount(1, shopId) : 0;
            ExpiredCount = shopId > 0 ? service.GetExpiredCount(1, shopId) : 0;

            if (shopId > 0)
            {
                string sql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 1 AND dataFlag = 1";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@shopId", shopId));
                InquiryCount = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                string sqlNew = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 1 AND dataFlag = 1 AND readStatus = 0";
                object resultNew = DbHelper.ExecuteScalar(sqlNew, DbHelper.CreateParameter("@shopId", shopId));
                NewInquiryCount = resultNew != DBNull.Value ? Convert.ToInt32(resultNew) : 0;

                string sqlQuote = "SELECT COUNT(*) FROM enquiryquoteprice WHERE fromShopId = @shopId AND eqType = 2 AND dataFlag = 1";
                object resultQuote = DbHelper.ExecuteScalar(sqlQuote, DbHelper.CreateParameter("@shopId", shopId));
                QuoteCount = resultQuote != DBNull.Value ? Convert.ToInt32(resultQuote) : 0;
            }
        }
        catch
        {
            OnlineSupplyCount = 3;
            ExpiredCount = 4;
            InquiryCount = 0;
            NewInquiryCount = 0;
            QuoteCount = 0;
        }
    }

    private void BindInventory()
    {
        DataTable dt = null;
        try
        {
            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            GoodsService service = new GoodsService();
            dt = shopId > 0 ? service.GetInventoryList(1, shopId) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasInventoryData = false;
            dt = new DataTable();
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("StatusClass", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("IsTaxed", typeof(bool));
            dt.Columns.Add("RemainingTime", typeof(string));
        }
        else
        {
            HasInventoryData = true;
        }

        TotalPages = dt.Rows.Count > 0 ? (int)Math.Ceiling((double)dt.Rows.Count / 50) : 1;

        rptInventory.DataSource = dt;
        rptInventory.DataBind();
    }

    private void BindExpiredInventory()
    {
        DataTable dt = null;
        try
        {
            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            GoodsService service = new GoodsService();
            dt = shopId > 0 ? service.GetExpiredInventoryList(1, shopId) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            dt = new DataTable();
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("IsTaxed", typeof(bool));
            dt.Columns.Add("OfflineTime", typeof(string));
        }

        rptExpiredInventory.DataSource = dt;
        rptExpiredInventory.DataBind();
    }
}