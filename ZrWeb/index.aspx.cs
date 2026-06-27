using System;
using System.Web.UI;
using System.Configuration;
using System.Data;
using System.Web;
using System.Text;

public partial class index : Page
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> _publishRequests = new System.Collections.Concurrent.ConcurrentDictionary<string, DateTime>();
    
    protected string PageTitle = ConfigurationManager.AppSettings["SiteTitle"] ?? "首页 / 供需广场 - 电子元器件平台";
    protected string PageKeywords = ConfigurationManager.AppSettings["SiteKeywords"] ?? "阻容网,电子元器件,贴片电容,贴片电阻,供需撮合";
    protected string PageDescription = ConfigurationManager.AppSettings["SiteDescription"] ?? "阻容网是专业的电子元器件交易平台，提供贴片电容、贴片电阻等被动元件的供需撮合服务。";
    protected bool HasData = false;
    protected int CurrentPage = 1;
    protected int TotalPages = 1;
    protected int TotalCount = 0;
    protected int PageSize = 45;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            string action = Request["action"];

            if (action == "submit_inquiry")
            {
                HandleSubmitInquiry();
                return;
            }
            else if (action == "submit_quote")
            {
                HandleSubmitQuote();
                return;
            }
            else if (action == "publish_supply")
            {
                HandlePublishSupply();
                return;
            }
            else if (action == "publish_demand")
            {
                HandlePublishDemand();
                return;
            }
            else if (action == "load_more")
            {
                LoadMoreItems();
                return;
            }
        }

        if (!IsPostBack)
        {
            BindSupplyList();
        }
    }
    
    private void LoadMoreItems()
    {
        try
        {
            int page = 1;
            int.TryParse(Request["page"], out page);
            if (page < 1) page = 1;
            
            int pageSize = 45;
            int.TryParse(Request["pageSize"], out pageSize);
            if (pageSize < 1 || pageSize > 45) pageSize = 45;
            
            GoodsService service = new GoodsService();
            DataTable dt = service.GetSupplyList(1, page, pageSize);
            int totalCount = service.GetSupplyListCount(1);
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{\"success\":true,\"page\":").Append(page).Append(",\"pageSize\":").Append(pageSize)
              .Append(",\"total\":").Append(totalCount).Append(",\"items\":[");
            
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    var row = dt.Rows[i];
                    sb.Append("{");
                    sb.Append("\"goodsId\":").Append(GetIntValue(row["goodsId"], 0));
                    sb.Append(",\"model\":\"").Append(EscapeJson(GetStringValue(row["Model"]))).Append("\"");
                    sb.Append(",\"priceDisplay\":\"").Append(EscapeJson(GetStringValue(row["PriceDisplay"]))).Append("\"");
                    sb.Append(",\"brandParams\":\"").Append(EscapeJson(GetStringValue(row["BrandParams"]))).Append("\"");
                    sb.Append(",\"quantityDisplay\":\"").Append(EscapeJson(GetStringValue(row["QuantityDisplay"]))).Append("\"");
                    sb.Append(",\"validity\":\"").Append(EscapeJson(GetStringValue(row["Validity"]))).Append("\"");
                    sb.Append(",\"companyName\":\"").Append(EscapeJson(GetStringValue(row["CompanyName"]))).Append("\"");
                    sb.Append(",\"itemType\":\"").Append(EscapeJson(GetStringValue(row["ItemType"]))).Append("\"");
                    sb.Append(",\"tagClass\":\"").Append(EscapeJson(GetStringValue(row["TagClass"]))).Append("\"");
                    sb.Append(",\"typeLabel\":\"").Append(EscapeJson(GetStringValue(row["TypeLabel"]))).Append("\"");
                    sb.Append(",\"detailUrl\":\"").Append(EscapeJson(GetStringValue(row["DetailUrl"]))).Append("\"");
                    sb.Append(",\"actionText\":\"").Append(EscapeJson(GetStringValue(row["ActionText"]))).Append("\"");
                    sb.Append(",\"shopId\":").Append(GetIntValue(row["ShopId"], 0));
                    sb.Append(",\"goodsSn\":\"").Append(EscapeJson(GetStringValue(row["GoodsSn"]))).Append("\"");
                    sb.Append("}");
                }
            }
            
            sb.Append("]}");
            WriteJson(sb.ToString());
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"加载失败: " + ex.Message.Replace("\"", "'") + "\"}");
        }
    }
    
    private string EscapeJson(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
    }

    private void HandleSubmitInquiry()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            string goodsSn = Request["goodsSn"];
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            decimal? price = null;
            string priceStr = Request["price"];
            if (!string.IsNullOrEmpty(priceStr)) {
                decimal parsedPrice;
                if (decimal.TryParse(priceStr, out parsedPrice)) {
                    price = parsedPrice;
                }
            }
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            string remarks = Request["remarks"];

            int fromUserId = UserHelper.GetUserId();
            int fromShopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out fromShopId);
            }
            if (fromShopId == 0)
            {
                fromShopId = GetDefaultShopId();
            }

            int toShopId = 0;
            int.TryParse(Request["toShopId"], out toShopId);

            if (fromShopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取您的店铺ID，请先登录并完善店铺信息\"}");
                return;
            }

            if (toShopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取供应商店铺信息，请刷新页面后重试\"}");
                return;
            }

            if (fromShopId == toShopId)
            {
                WriteJson("{\"success\":false,\"message\":\"不能对自己店铺的供应信息进行询价\"}");
                return;
            }

            string fromCompany = "";
            if (Session["LinkMan"] != null)
            {
                fromCompany = Session["LinkMan"].ToString();
            }
            if (string.IsNullOrEmpty(fromCompany))
            {
                if (Session["UserName"] != null)
                {
                    fromCompany = Session["UserName"].ToString();
                }
                else
                {
                    fromCompany = "匿名采购商";
                }
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            bool success = service.SubmitQuote(
                goodsId, goodsSn, quantity, price ?? 0,
                isIncludingTax, fromCompany, "", "",
                remarks, "", 0, fromUserId, "",
                fromShopId, toShopId, 1);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"询价提交成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"询价提交失败\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandleSubmitQuote()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            string goodsSn = Request["goodsSn"];
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            decimal? price = null;
            string priceStr = Request["price"];
            if (!string.IsNullOrEmpty(priceStr)) {
                decimal parsedPrice;
                if (decimal.TryParse(priceStr, out parsedPrice)) {
                    price = parsedPrice;
                }
            }
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            string remarks = Request["remarks"];

            int fromUserId = UserHelper.GetUserId();
            int fromShopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out fromShopId);
            }
            if (fromShopId == 0)
            {
                fromShopId = GetDefaultShopId();
            }

            int toShopId = 0;
            int.TryParse(Request["toShopId"], out toShopId);

            if (fromShopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取您的店铺ID，请先登录并完善店铺信息\"}");
                return;
            }

            if (toShopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取需求方店铺信息，请刷新页面后重试\"}");
                return;
            }

            if (fromShopId == toShopId)
            {
                WriteJson("{\"success\":false,\"message\":\"不能对自己店铺的询价进行报价\"}");
                return;
            }

            string fromCompany = "";
            if (Session["ShopCompany"] != null)
            {
                fromCompany = Session["ShopCompany"].ToString();
            }
            if (string.IsNullOrEmpty(fromCompany) && Session["ShopName"] != null)
            {
                fromCompany = Session["ShopName"].ToString();
            }
            if (string.IsNullOrEmpty(fromCompany))
            {
                fromCompany = "匿名供应商";
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            bool success = service.SubmitQuote(
                goodsId, goodsSn, quantity, price ?? 0,
                isIncludingTax, fromCompany, "", "",
                remarks, "", 0, fromUserId, "",
                fromShopId, toShopId, 2);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"报价提交成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"报价提交失败\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private int GetDefaultShopId()
    {
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId > 0)
            {
                string sql = "SELECT TOP 1 shopId FROM shops WHERE userId = @userId AND dataFlag = 1";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@userId", userId));
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GetDefaultShopId 错误: " + ex.Message);
        }
        return 0;
    }
    
    private int GetIntValue(object value, int defaultValue)
    {
        if (value == null || value == DBNull.Value) return defaultValue;
        try { return Convert.ToInt32(value); }
        catch { return defaultValue; }
    }
    
    private string GetStringValue(object value)
    {
        if (value == null || value == DBNull.Value) return "";
        return value.ToString();
    }

    private int GetRoseId()
    {
        try
        {
            int userId = UserHelper.GetUserId();
            if (userId > 0)
            {
                string sql = "SELECT RoseID FROM userinfo WHERE UserID = @userId";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@userId", userId));
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GetRoseId 错误: " + ex.Message);
        }
        return 1;
    }

    private void BindSupplyList()
    {
        DataTable dt = null;
        try
        {
            // 获取分页参数
            int.TryParse(Request.QueryString["page"], out CurrentPage);
            if (CurrentPage < 1) CurrentPage = 1;
            
            GoodsService service = new GoodsService();
            dt = service.GetSupplyList(1, CurrentPage, PageSize);
            TotalCount = service.GetSupplyListCount(1);
            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            if (TotalPages < 1) TotalPages = 1;
            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("数据库查询错误: " + ex.Message);
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasData = false;
            dt = new DataTable();
        }
        else
        {
            HasData = true;
        }

        rptSupplyList.DataSource = dt;
        rptSupplyList.DataBind();
    }

    private void HandlePublishSupply()
    {
        Response.ClearContent();
        Response.ClearHeaders();
        Response.ContentType = "application/json";
        Response.Charset = "UTF-8";
        
        string requestId = Request["requestId"];
        if (string.IsNullOrEmpty(requestId))
        {
            WriteJson("{\"success\":false,\"message\":\"请求参数错误\"}");
            return;
        }
        
        if (_publishRequests.ContainsKey(requestId))
        {
            WriteJson("{\"success\":false,\"message\":\"请勿重复提交\"}");
            return;
        }
        
        _publishRequests.TryAdd(requestId, DateTime.Now);
        
        try
        {
            string goodsSn = Request["goodsSn"];
            string name = Request["name"];
            string manufacturers = Request["manufacturers"];
            decimal? price = null;
            string priceStr = Request["price"];
            if (!string.IsNullOrEmpty(priceStr)) {
                decimal parsedPrice;
                if (decimal.TryParse(priceStr, out parsedPrice)) {
                    price = parsedPrice;
                }
            }
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            string unit = Request["unit"] ?? "Kpcs";
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            string validity = Request["validity"] ?? "30天";

            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }
            if (shopId == 0)
            {
                shopId = GetDefaultShopId();
            }

            if (shopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取您的店铺ID，请先登录并完善店铺信息\"}");
                return;
            }

            int roseId = GetRoseId();
            if (roseId == 2)
            {
                WriteJson("{\"success\":false,\"message\":\"采购商只能发布采购需求，无法发布供应信息\"}");
                return;
            }

            if (string.IsNullOrEmpty(goodsSn))
            {
                WriteJson("{\"success\":false,\"message\":\"请输入型号\"}");
                return;
            }

            GoodsService service = new GoodsService();
            int userId = UserHelper.GetUserId();
            bool success = service.InsertGoods(goodsSn, name, manufacturers, "", quantity, unit, price, isIncludingTax, 1, "", shopId, userId, validity);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"发布成功\",\"redirect\":\"/merchant-workbench.aspx\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"发布失败\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"发布异常：" + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandlePublishDemand()
    {
        Response.ClearContent();
        Response.ClearHeaders();
        Response.ContentType = "application/json";
        Response.Charset = "UTF-8";
        
        string requestId = Request["requestId"];
        if (string.IsNullOrEmpty(requestId))
        {
            WriteJson("{\"success\":false,\"message\":\"请求参数错误\"}");
            return;
        }
        
        if (_publishRequests.ContainsKey(requestId))
        {
            WriteJson("{\"success\":false,\"message\":\"请勿重复提交\"}");
            return;
        }
        
        _publishRequests.TryAdd(requestId, DateTime.Now);
        
        try
        {
            string goodsSn = Request["goodsSn"];
            string name = Request["name"];
            string manufacturers = Request["manufacturers"];
            decimal? price = null;
            string priceStr = Request["price"];
            if (!string.IsNullOrEmpty(priceStr)) {
                decimal parsedPrice;
                if (decimal.TryParse(priceStr, out parsedPrice)) {
                    price = parsedPrice;
                }
            }
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            string unit = Request["unit"] ?? "Kpcs";
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            string validity = Request["validity"] ?? "30天";

            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }
            if (shopId == 0)
            {
                shopId = GetDefaultShopId();
            }

            if (shopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取您的店铺ID，请先登录并完善店铺信息\"}");
                return;
            }

            if (string.IsNullOrEmpty(goodsSn))
            {
                WriteJson("{\"success\":false,\"message\":\"请输入型号\"}");
                return;
            }

            GoodsService service = new GoodsService();
            int userId = UserHelper.GetUserId();
            bool success = service.PublishDemand(goodsSn, name, manufacturers, quantity, unit, price ?? 0, isIncludingTax, userId, shopId, validity);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"发布成功\",\"redirect\":\"/buyer-workbench.aspx\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"发布失败\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"发布异常：" + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }
    
    private void WriteJson(string json)
    {
        try {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/json";
            Response.Charset = "UTF-8";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
        } finally {
            Response.SuppressContent = true;
            ApplicationInstance.CompleteRequest();
        }
    }
}