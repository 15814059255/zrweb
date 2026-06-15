using System;
using System.Web.UI;
using System.Configuration;
using System.Data;
using System.Web;

public partial class index : Page
{
    protected string PageTitle = ConfigurationManager.AppSettings["SiteTitle"] ?? "首页 / 供需广场 - 电子元器件 B2B 平台";
    protected string PageKeywords = ConfigurationManager.AppSettings["SiteKeywords"] ?? "阻容网,电子元器件,B2B,贴片电容,贴片电阻,供需撮合";
    protected string PageDescription = ConfigurationManager.AppSettings["SiteDescription"] ?? "阻容网是专业的电子元器件B2B交易平台，提供贴片电容、贴片电阻等被动元件的供需撮合服务。";
    protected bool HasData = false;

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
        }

        if (!IsPostBack)
        {
            BindSupplyList();
        }
    }

    private void HandleSubmitInquiry()
    {
        Response.ContentType = "application/json";
        Response.Clear();
        
        string result = "";
        
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            string goodsSn = Request["goodsSn"];
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            decimal price = 0;
            decimal.TryParse(Request["price"], out price);
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
                result = "{\"success\":false,\"message\":\"无法获取您的店铺ID，请先登录并完善店铺信息\"}";
                Response.Write(result);
                Response.End();
                return;
            }

            if (toShopId == 0)
            {
                result = "{\"success\":false,\"message\":\"无法获取供应商店铺信息，请刷新页面后重试\"}";
                Response.Write(result);
                Response.End();
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
                goodsId, goodsSn, quantity, price,
                isIncludingTax, fromCompany, "", "",
                remarks, "", 0, fromUserId, "",
                fromShopId, toShopId, 1);

            if (success)
            {
                result = "{\"success\":true,\"message\":\"询价提交成功\"}";
            }
            else
            {
                result = "{\"success\":false,\"message\":\"询价提交失败\"}";
            }
        }
        catch (Exception ex)
        {
            result = "{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}";
        }
        
        Response.Write(result);
        Response.End();
    }

    private void HandleSubmitQuote()
    {
        Response.ContentType = "application/json";
        Response.Clear();
        
        string result = "";
        
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            string goodsSn = Request["goodsSn"];
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            decimal price = 0;
            decimal.TryParse(Request["price"], out price);
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
                result = "{\"success\":false,\"message\":\"无法获取您的店铺ID，请先登录并完善店铺信息\"}";
                Response.Write(result);
                Response.End();
                return;
            }

            if (toShopId == 0)
            {
                result = "{\"success\":false,\"message\":\"无法获取需求方店铺信息，请刷新页面后重试\"}";
                Response.Write(result);
                Response.End();
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
                    fromCompany = "匿名供应商";
                }
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            bool success = service.SubmitQuote(
                goodsId, goodsSn, quantity, price,
                isIncludingTax, fromCompany, "", "",
                remarks, "", 0, fromUserId, "",
                fromShopId, toShopId, 2);

            if (success)
            {
                result = "{\"success\":true,\"message\":\"报价提交成功\"}";
            }
            else
            {
                result = "{\"success\":false,\"message\":\"报价提交失败\"}";
            }
        }
        catch (Exception ex)
        {
            result = "{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}";
        }
        
        Response.Write(result);
        Response.End();
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

    private void BindSupplyList()
    {
        DataTable dt = null;
        try
        {
            GoodsService service = new GoodsService();
            dt = service.GetSupplyList(1);
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
}