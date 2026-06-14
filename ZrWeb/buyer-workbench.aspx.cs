using System;
using System.Data;
using System.Threading;
using System.Web;

public partial class buyer_workbench : System.Web.UI.Page
{
    public string PageTitle = "我是采购 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,采购工作台,需求管理,电子元器件采购";
    public string PageDescription = "阻容网采购工作台，管理采购需求、查看供应商报价、跟踪询价进度，一站式采购电子元器件。";
    
    public int OnlineDemandCount = 3;
    public int QuoteCount = 21;
    public int NewQuoteCount = 3;
    public int ExpiredCount = 4;
    public int CurrentPage = 1;
    public int TotalPages = 49;
    public bool HasDemandData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            string action = Request["action"];
            
            if (action == "publish_demand")
            {
                HandlePublishDemand();
                return;
            }
            else if (action == "take_off")
            {
                HandleTakeOff();
                return;
            }
            else if (action == "restock")
            {
                HandleRestock();
                return;
            }
        }

        if (!IsPostBack)
        {
            LoadStats();
            BindDemand();
            BindExpiredDemand();
        }
    }

    private void HandlePublishDemand()
    {
        try
        {
            string goodsSn = Request["goodsSn"] ?? "";
            string name = Request["name"] ?? "";
            string manufacturers = Request["manufacturers"] ?? "";
            int quantity = 0;
            string unit = Request["unit"] ?? "Kpcs";
            decimal price = 0;
            int isIncludingTax = 0;

            int.TryParse(Request["quantity"], out quantity);
            decimal.TryParse(Request["price"], out price);
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);

            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Charset = "utf-8";
                Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
                Response.End();
                return;
            }

            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            if (shopId == 0)
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Charset = "utf-8";
                Response.Write("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
                Response.End();
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.PublishDemand(goodsSn, name, manufacturers, quantity, unit, price, isIncludingTax, userId, shopId);

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            
            if (success)
            {
                Response.Write("{\"success\":true,\"message\":\"发布成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"发布失败\"}");
            }
            Response.End();
        }
        catch (ThreadAbortException)
        {
            // 忽略 ThreadAbortException
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            Response.Write("{\"success\":false,\"message\":\"发布异常:" + ex.Message.Replace("\"", "'") + "\"}");
            Response.End();
        }
    }

    private void HandleTakeOff()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId <= 0)
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Charset = "utf-8";
                Response.Write("{\"success\":false,\"message\":\"无效的商品ID\"}");
                Response.End();
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.TakeOff(goodsId);

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            
            if (success)
            {
                Response.Write("{\"success\":true,\"message\":\"下架成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"下架失败\"}");
            }
            Response.End();
        }
        catch (ThreadAbortException)
        {
            // 忽略 ThreadAbortException
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            Response.Write("{\"success\":false,\"message\":\"下架异常:" + ex.Message.Replace("\"", "'") + "\"}");
            Response.End();
        }
    }

    private void HandleRestock()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId <= 0)
            {
                Response.Clear();
                Response.ContentType = "application/json";
                Response.Charset = "utf-8";
                Response.Write("{\"success\":false,\"message\":\"无效的商品ID\"}");
                Response.End();
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.Restock(goodsId);

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            
            if (success)
            {
                Response.Write("{\"success\":true,\"message\":\"重新上架成功\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"重新上架失败\"}");
            }
            Response.End();
        }
        catch (ThreadAbortException)
        {
            // 忽略 ThreadAbortException
        }
        catch (Exception ex)
        {
            Response.Clear();
            Response.ContentType = "application/json";
            Response.Charset = "utf-8";
            Response.Write("{\"success\":false,\"message\":\"重新上架异常:" + ex.Message.Replace("\"", "'") + "\"}");
            Response.End();
        }
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
            OnlineDemandCount = shopId > 0 ? service.GetOnlineSupplyCount(2, shopId) : 0;
            ExpiredCount = shopId > 0 ? service.GetExpiredCount(2, shopId) : 0;

            if (shopId > 0)
            {
                string sql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 2 AND dataFlag = 1";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@shopId", shopId));
                QuoteCount = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                string sqlNew = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 2 AND dataFlag = 1 AND readStatus = 0";
                object resultNew = DbHelper.ExecuteScalar(sqlNew, DbHelper.CreateParameter("@shopId", shopId));
                NewQuoteCount = resultNew != DBNull.Value ? Convert.ToInt32(resultNew) : 0;
            }
        }
        catch
        {
            OnlineDemandCount = 3;
            ExpiredCount = 4;
            QuoteCount = 0;
            NewQuoteCount = 0;
        }
    }

    private void BindDemand()
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
            dt = shopId > 0 ? service.GetDemandList(2, shopId) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasDemandData = false;
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
            HasDemandData = true;
        }

        TotalPages = dt.Rows.Count > 0 ? (int)Math.Ceiling((double)dt.Rows.Count / 50) : 1;

        rptDemand.DataSource = dt;
        rptDemand.DataBind();
    }

    private void BindExpiredDemand()
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
            dt = shopId > 0 ? service.GetExpiredDemandList(2, shopId) : null;
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

        rptExpiredDemand.DataSource = dt;
        rptExpiredDemand.DataBind();
    }
}