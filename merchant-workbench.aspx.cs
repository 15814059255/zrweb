using System;
using System.Data;
using System.Threading;

public partial class merchant_workbench : System.Web.UI.Page
{
    public string PageTitle = "我是商家 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,商家工作台,库存管理,电子元器件";
    public string PageDescription = "阻容网商家工作台，管理库存、查看询价、提交报价，一站式管理您的电子元器件供应业务。";
    
    public int OnlineSupplyCount = 0;
    public int InquiryCount = 0;
    public int NewInquiryCount = 0;
    public int QuoteCount = 0;
    public int ExpiredCount = 0;
    public int CurrentPage = 1;
    public int TotalPages = 49;
    public bool HasInventoryData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request["action"] == "publish_goods")
        {
            HandlePublishGoods();
            return;
        }

        if (Request["action"] == "publish_demand")
        {
            HandlePublishDemand();
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
                WriteJsonResponse(false, "请输入型号");
                return;
            }

            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                WriteJsonResponse(false, "请先登录");
                return;
            }

            int shopId = UserHelper.GetShopId();

            if (shopId == 0)
            {
                WriteJsonResponse(false, "无法获取店铺信息，请完善店铺资料后重试");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.InsertGoods(
                goodsSn, name, manufacturers, "",
                goodsStock, goodsUnit, shopPrice, isIncludingTax,
                pubType, "", shopId, userId);

            if (success)
            {
                WriteJsonResponse(true, "发布成功");
            }
            else
            {
                WriteJsonResponse(false, "发布失败");
            }
        }
        catch (Exception ex)
        {
            if (!(ex is System.Threading.ThreadAbortException))
            {
                WriteJsonResponse(false, "错误: " + ex.Message);
            }
        }
    }

    private void HandlePublishDemand()
    {
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

            if (string.IsNullOrEmpty(goodsSn))
            {
                WriteJsonResponse(false, "请输入型号");
                return;
            }

            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                WriteJsonResponse(false, "请先登录");
                return;
            }

            int shopId = UserHelper.GetShopId();

            if (shopId == 0)
            {
                WriteJsonResponse(false, "无法获取店铺信息，请完善店铺资料后重试");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.PublishDemand(goodsSn, name, manufacturers, goodsStock, goodsUnit, shopPrice, isIncludingTax, userId, shopId);

            if (success)
            {
                WriteJsonResponse(true, "发布成功");
            }
            else
            {
                WriteJsonResponse(false, "发布失败");
            }
        }
        catch (Exception ex)
        {
            if (!(ex is System.Threading.ThreadAbortException))
            {
                WriteJsonResponse(false, "错误: " + ex.Message);
            }
        }
    }

    private string CleanJsonMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        return message.Replace("\\", "\\\\")
                     .Replace("\"", "\\\"")
                     .Replace("\r", "\\r")
                     .Replace("\n", "\\n")
                     .Replace("\t", "\\t")
                     .Replace("\0", "\\0");
    }

    private void WriteJsonResponse(bool success, string message)
    {
        Response.Clear();
        Response.ContentType = "application/json";
        Response.Charset = "utf-8";
        Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + CleanJsonMessage(message) + "\"}");
        try { Response.End(); } catch { }
    }

    private void HandleTakeOff()
    {
        Response.ContentType = "application/json";
        Response.Clear();

        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"无效的商品ID\"}");
                Response.End();
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
            if (!(ex is ThreadAbortException))
            {
                Response.Write("{\"success\":false,\"message\":\"错误: " + CleanJsonMessage(ex.Message) + "\"}");
            }
        }

        try { Response.End(); } catch { }
    }

    private void HandleRestock()
    {
        Response.ContentType = "application/json";
        Response.Clear();

        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId == 0)
            {
                Response.Write("{\"success\":false,\"message\":\"无效的商品ID\"}");
                Response.End();
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
            if (!(ex is ThreadAbortException))
            {
                Response.Write("{\"success\":false,\"message\":\"错误: " + CleanJsonMessage(ex.Message) + "\"}");
            }
        }

        try { Response.End(); } catch { }
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
            else
            {
                InquiryCount = 0;
                NewInquiryCount = 0;
                QuoteCount = 0;
            }
        }
        catch
        {
            OnlineSupplyCount = 0;
            ExpiredCount = 0;
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