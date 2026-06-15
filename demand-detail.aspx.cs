using System;
using System.Web.UI;
using System.Configuration;
using System.Data;

public partial class demand_detail : Page
{
    protected string PageTitle = "";
    protected string PageKeywords = "";
    protected string PageDescription = "";
    
    protected string Title = "";
    protected string Model = "";
    protected string PriceDisplay = "";
    protected string PriceClass = "";
    protected string BrandRequirement = "";
    protected string Parameters = "";
    protected string Quantity = "";
    protected string Validity = "";
    protected string DeliveryRequirement = "";
    protected string CompanyName = "";
    protected string CompanyAddress = "";
    protected string AuthStatus = "";
    protected int GoodsId = 0;
    protected string GoodsSn = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            string action = Request["action"];
            if (action == "submit_quote")
            {
                HandleSubmitQuote();
                return;
            }
        }

        if (!IsPostBack)
        {
            LoadDemandDetail();
        }
    }

    private void HandleSubmitQuote()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            
            string goodsSn = Request["goodsSn"] ?? "";
            int fromQuantity = 0;
            int.TryParse(Request["quantity"], out fromQuantity);
            decimal fromPrice = 0;
            decimal.TryParse(Request["price"], out fromPrice);
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            string fromRemarks = Request["remarks"] ?? "";
            string brandName = Request["brandName"] ?? "";

            int fromUserId = 0;
            if (Session["UserID"] != null)
            {
                int.TryParse(Session["UserID"].ToString(), out fromUserId);
            }

            string fromCompany = "";
            string fromContact = "";
            string fromTel = "";
            if (Session["LinkMan"] != null)
            {
                fromContact = Session["LinkMan"].ToString();
            }
            if (Session["MobilePhone"] != null)
            {
                fromTel = Session["MobilePhone"].ToString();
            }

            string toCompany = CompanyName;
            int toUserId = 0;

            if (goodsId <= 0 || fromQuantity <= 0 || fromPrice <= 0 || fromUserId <= 0)
            {
                ResponseClearAndWrite(false, "参数错误");
                return;
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            bool success = service.SubmitQuote(
                goodsId, goodsSn, fromQuantity, fromPrice, isIncludingTax,
                fromCompany, fromContact, fromTel, fromRemarks,
                toCompany, toUserId, fromUserId, brandName);

            if (success)
            {
                ResponseClearAndWrite(true, "报价提交成功");
            }
            else
            {
                ResponseClearAndWrite(false, "报价提交失败");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("HandleSubmitQuote 错误: " + ex.Message);
            ResponseClearAndWrite(false, "报价提交异常: " + ex.Message.Replace("\"", "'"));
        }
    }

    private void ResponseClearAndWrite(bool success, string message)
    {
        Response.Clear();
        Response.ContentType = "application/json";
        Response.Charset = "utf-8";
        Response.Write("{\"success\":" + (success ? "true" : "false") + ",\"message\":\"" + message + "\"}");
        Response.End();
    }

    private void LoadDemandDetail()
    {
        int goodsId = 0;
        if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out goodsId) && goodsId > 0)
        {
            BindDemandDetail(goodsId);
        }
        else
        {
            LoadDefaultData();
        }
    }

    private void BindDemandDetail(int goodsId)
    {
        try
        {
            string sql = @"SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.shopStatus 
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          WHERE g.goodsId = @goodsId AND g.dataFlag = 1 AND g.pubType = 2";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                
                string goodsSn = GetStringValue(row["goodsSn"]);
                string name = GetStringValue(row["Name"]);
                string manufacturers = GetStringValue(row["Manufacturers"]);
                int goodsStock = GetIntValue(row["goodsStock"], 0);
                string goodsUnit = GetStringValue(row["goodsUnit"]);
                decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
                int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
                DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
                DateTime validityDate = GetDateTimeValue(row["validityDate"], DateTime.Now.AddDays(3));
                string shopName = GetStringValue(row["shopName"]);
                string shopCompany = GetStringValue(row["shopCompany"]);
                string shopAddress = GetStringValue(row["shopAddress"]);
                int shopStatus = GetIntValue(row["shopStatus"], 0);

                if (!string.IsNullOrEmpty(goodsSn))
                {
                    Title = "求购 " + goodsSn;
                    Model = goodsSn;
                }
                else if (!string.IsNullOrEmpty(name))
                {
                    Title = "求购 " + name;
                    Model = name;
                }
                else
                {
                    Title = "求购电子元器件";
                    Model = "不限型号";
                }

                if (shopPrice > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    string taxText = isIncludingTax == 1 ? "(含税)" : "(未税)";
                    PriceDisplay = "期望 ¥" + shopPrice.ToString("0.00") + " /" + unit + " " + taxText;
                    PriceClass = "is-expected";
                }
                else
                {
                    PriceDisplay = "面议";
                    PriceClass = "is-negotiable";
                }

                BrandRequirement = !string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限";
                Parameters = "采购需求";

                if (goodsStock > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    Quantity = "需求 " + goodsStock + "/" + unit;
                }
                else
                {
                    Quantity = "需求数量待定";
                }

                TimeSpan diff = validityDate - DateTime.Now;
                if (diff.TotalHours < 24)
                {
                    Validity = "24 小时";
                }
                else if (diff.TotalDays < 3)
                {
                    Validity = "3 天";
                }
                else if (diff.TotalDays < 7)
                {
                    Validity = "7 天";
                }
                else if (diff.TotalDays < 30)
                {
                    Validity = (int)diff.TotalDays + " 天";
                }
                else
                {
                    Validity = validityDate.ToString("yyyy-MM-dd");
                }

                DeliveryRequirement = "尽快";

                CompanyName = !string.IsNullOrEmpty(shopCompany) ? shopCompany : (!string.IsNullOrEmpty(shopName) ? shopName : "匿名采购商");
                CompanyAddress = !string.IsNullOrEmpty(shopAddress) ? shopAddress : "未填写";
                AuthStatus = shopStatus == 1 ? "已认证" : (shopStatus == 0 ? "待审核" : "未认证");
                
                GoodsId = goodsId;
                GoodsSn = goodsSn;
            }
            else
            {
                LoadDefaultData();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LoadDemandDetail 错误: " + ex.Message);
            LoadDefaultData();
        }

        SetSEO();
    }

    private void LoadDefaultData()
    {
        Title = "求购 100K 0603 贴片电阻";
        Model = "不限型号";
        PriceDisplay = "¥6.0 /盘";
        PriceClass = "is-expected";
        BrandRequirement = "品牌不限";
        Parameters = "±1% · 现货";
        Quantity = "需求 184/盘";
        Validity = "24 小时";
        DeliveryRequirement = "3天内";
        CompanyName = "东莞智造电子科技有限公司";
        CompanyAddress = "东莞市松山湖";
        AuthStatus = "已认证";
        
        SetSEO();
    }

    private void SetSEO()
    {
        PageTitle = Title + " 需求详情 - 阻容网";
        PageKeywords = Title + ",采购需求,电子元器件,阻容网";
        PageDescription = Title + " 需求信息，" + Quantity + "，有效期" + Validity + "，采购商：" + CompanyName;
    }

    private int GetIntValue(object value, int defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToInt32(value);
    }

    private decimal GetDecimalValue(object value, decimal defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToDecimal(value);
    }

    private DateTime GetDateTimeValue(object value, DateTime defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToDateTime(value);
    }

    private string GetStringValue(object value)
    {
        if (value == DBNull.Value || value == null)
            return "";
        return value.ToString();
    }
}