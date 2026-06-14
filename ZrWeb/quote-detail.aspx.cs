using System;
using System.Data;

public partial class quote_detail : System.Web.UI.Page
{
    public string PageTitle = "报价详情 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,报价详情,报价记录,供应商";
    public string PageDescription = "查看报价详情，了解报价信息和采购商信息。";

    public string Model = "";
    public string BrandName = "";
    public string Quantity = "0";
    public string Unit = "Kpcs";
    public string PriceDisplay = "面议";
    public string BuyerName = "";
    public string QuoteTime = "";
    public string Validity = "";
    public string Status = "未知";
    public string StatusClass = "gray";
    public string Remarks = "无";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindQuoteDetail();
        }
    }

    private void BindQuoteDetail()
    {
        int eqId = 0;
        if (!string.IsNullOrEmpty(Request.QueryString["id"]))
        {
            int.TryParse(Request.QueryString["id"], out eqId);
        }

        if (eqId > 0)
        {
            EnquiryQuoteService service = new EnquiryQuoteService();
            DataRow row = service.GetEnquiryQuoteById(eqId);
            
            if (row != null)
            {
                Model = GetStringValue(row["goodsSn"]);
                BrandName = GetStringValue(row["brandName"]);
                Quantity = GetIntValue(row["fromQuantity"], 0).ToString();
                Unit = "Kpcs";
                
                decimal price = GetDecimalValue(row["fromPrice"], 0);
                int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
                if (price > 0)
                {
                    string taxText = isIncludingTax == 1 ? "含税" : "未税";
                    PriceDisplay = "¥" + price.ToString("0.00") + " /Kpcs (" + taxText + ")";
                }
                
                BuyerName = GetStringValue(row["toCompany"]);
                if (string.IsNullOrEmpty(BuyerName))
                {
                    BuyerName = "匿名采购商";
                }
                
                DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
                QuoteTime = createTime.ToString("yyyy-MM-dd HH:mm");
                
                TimeSpan diff = DateTime.Now - createTime;
                if (diff.TotalHours < 24)
                {
                    Validity = "24 小时内";
                }
                else if (diff.TotalDays < 3)
                {
                    Validity = "3 天内";
                }
                else if (diff.TotalDays < 7)
                {
                    Validity = "7 天内";
                }
                else
                {
                    Validity = createTime.AddDays(3).ToString("yyyy-MM-dd");
                }
                
                int readStatus = GetIntValue(row["readStatus"], 0);
                if (readStatus == 1)
                {
                    Status = "已查看";
                    StatusClass = "blue";
                }
                else
                {
                    Status = "未查看";
                    StatusClass = "green";
                }
                
                Remarks = GetStringValue(row["fromRemarks"]);
                if (string.IsNullOrEmpty(Remarks))
                {
                    Remarks = "无";
                }
                
                service.UpdateReadStatus(eqId, 1);
                return;
            }
        }

        LoadDefaultData();
    }

    private void LoadDefaultData()
    {
        Model = "GRM188R71H104KA93D";
        BrandName = "Murata/村田";
        Quantity = "10";
        Unit = "Kpcs";
        PriceDisplay = "¥0.12 /Kpcs (含税)";
        BuyerName = "深圳某电子科技有限公司";
        QuoteTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        Validity = "3 天内";
        Status = "未查看";
        StatusClass = "green";
        Remarks = "现货供应，欢迎洽谈";
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