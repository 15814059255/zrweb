using System;
using System.Data;
using System.Threading;
using System.Web;

public partial class buyer_workbench : System.Web.UI.Page
{
    public string PageTitle = "我是采购 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,采购工作台,需求管理,电子元器件采购";
    public string PageDescription = "阻容网采购工作台，管理采购需求、查看供应商报价、跟踪询价进度，一站式采购电子元器件。";
    
    public int OnlineDemandCount = 0;
    public int QuoteCount = 0;
    public int NewQuoteCount = 0;
    public int InquiryCount = 0;
    public int NewInquiryCount = 0;
    public int ExpiredCount = 0;
    public int CurrentPage = 1;
    public int TotalPages = 1;
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
            string validity = Request["validity"] ?? "1个月";

            int.TryParse(Request["quantity"], out quantity);
            decimal.TryParse(Request["price"], out price);
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);

            // 收集参数字段
            string brand = Request["attr_品牌"] ?? "";
            string packaging = Request["attr_封装"] ?? "";
            string capacity = Request["attr_容值"] ?? "";
            string resistance = Request["attr_阻值"] ?? "";
            string precision = Request["attr_精度"] ?? "";
            string voltage = Request["attr_耐压"] ?? "";
            string power = Request["attr_功率"] ?? "";
            string medium = Request["attr_介质"] ?? "";
            string tcr = Request["attr_温漂"] ?? "";

            // 组合品牌和参数信息
            string brandParams = "";
            if (!string.IsNullOrEmpty(brand))
            {
                brandParams = brand;
            }
            
            // 添加参数信息
            System.Collections.Generic.List<string> paramsList = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(packaging)) paramsList.Add(packaging);
            if (!string.IsNullOrEmpty(capacity)) paramsList.Add(capacity);
            if (!string.IsNullOrEmpty(resistance)) paramsList.Add(resistance);
            if (!string.IsNullOrEmpty(precision)) paramsList.Add(precision);
            if (!string.IsNullOrEmpty(voltage)) paramsList.Add(voltage);
            if (!string.IsNullOrEmpty(power)) paramsList.Add(power);
            if (!string.IsNullOrEmpty(medium)) paramsList.Add(medium);
            if (!string.IsNullOrEmpty(tcr)) paramsList.Add(tcr);
            
            if (paramsList.Count > 0)
            {
                if (!string.IsNullOrEmpty(brandParams))
                {
                    brandParams += " · " + string.Join(" · ", paramsList);
                }
                else
                {
                    brandParams = string.Join(" · ", paramsList);
                }
            }

            // 如果没有参数信息，使用原来的 manufacturers 字段
            if (string.IsNullOrEmpty(brandParams))
            {
                brandParams = manufacturers;
            }

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
            
            // 如果Session中没有ShopId，尝试从Cookie恢复
            if (shopId == 0)
            {
                HttpCookie userCookie = Request.Cookies["ZrWebUser"];
                if (userCookie != null)
                {
                    int.TryParse(userCookie["ShopId"], out shopId);
                    if (shopId > 0)
                    {
                        Session["ShopId"] = shopId;
                    }
                }
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
            bool success = service.PublishDemand(goodsSn, name, brandParams, quantity, unit, price, isIncludingTax, userId, shopId, validity);

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
            
            // 如果Session中没有ShopId，尝试从Cookie恢复
            if (shopId == 0)
            {
                HttpCookie userCookie = Request.Cookies["ZrWebUser"];
                if (userCookie != null)
                {
                    int.TryParse(userCookie["ShopId"], out shopId);
                    if (shopId > 0)
                    {
                        Session["ShopId"] = shopId;
                    }
                }
            }

            GoodsService service = new GoodsService();
            OnlineDemandCount = shopId > 0 ? service.GetOnlineSupplyCount(2, shopId) : 0;
            ExpiredCount = shopId > 0 ? service.GetExpiredCount(2, shopId) : 0;

            if (shopId > 0)
            {
                // 查询采购方收到的报价（商家回复的报价）
                string sql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 2 AND dataFlag = 1";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@shopId", shopId));
                QuoteCount = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                string sqlNew = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 2 AND dataFlag = 1 AND readStatus = 0";
                object resultNew = DbHelper.ExecuteScalar(sqlNew, DbHelper.CreateParameter("@shopId", shopId));
                NewQuoteCount = resultNew != DBNull.Value ? Convert.ToInt32(resultNew) : 0;

                // 查询采购方发出的询价（等待商家回复）
                string sqlInquiry = "SELECT COUNT(*) FROM enquiryquoteprice WHERE fromShopID = @shopId AND eqType = 1 AND dataFlag = 1";
                object resultInquiry = DbHelper.ExecuteScalar(sqlInquiry, DbHelper.CreateParameter("@shopId", shopId));
                InquiryCount = resultInquiry != DBNull.Value ? Convert.ToInt32(resultInquiry) : 0;

                string sqlNewInquiry = "SELECT COUNT(*) FROM enquiryquoteprice WHERE fromShopID = @shopId AND eqType = 1 AND dataFlag = 1 AND readStatus = 0";
                object resultNewInquiry = DbHelper.ExecuteScalar(sqlNewInquiry, DbHelper.CreateParameter("@shopId", shopId));
                NewInquiryCount = resultNewInquiry != DBNull.Value ? Convert.ToInt32(resultNewInquiry) : 0;
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
            
            // 如果Session中没有ShopId，尝试从Cookie恢复
            if (shopId == 0)
            {
                HttpCookie userCookie = Request.Cookies["ZrWebUser"];
                if (userCookie != null)
                {
                    int.TryParse(userCookie["ShopId"], out shopId);
                    if (shopId > 0)
                    {
                        Session["ShopId"] = shopId;
                        Session["ShopName"] = userCookie["ShopName"] ?? "";
                        Session["ShopCompany"] = userCookie["ShopCompany"] ?? "";
                    }
                }
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
            
            // 如果Session中没有ShopId，尝试从Cookie恢复
            if (shopId == 0)
            {
                HttpCookie userCookie = Request.Cookies["ZrWebUser"];
                if (userCookie != null)
                {
                    int.TryParse(userCookie["ShopId"], out shopId);
                    if (shopId > 0)
                    {
                        Session["ShopId"] = shopId;
                    }
                }
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