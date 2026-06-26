using System;
using System.Data;

public partial class quote_detail : System.Web.UI.Page
{
    public string PageTitle = "报价详情 - 阻容网";
    public string PageKeywords = "阻容网,报价详情,报价记录,供应商";
    public string PageDescription = "查看报价详情，了解报价信息和采购商信息。";

    public string Model = "";
    public string BrandName = "";
    public string Quantity = "0";
    public string Unit = "Kpcs";
    public string PriceDisplay = "面议";
    public string BuyerName = "";
    public string BuyerContact = "";
    public string BuyerTel = "";
    public string SupplierName = "";
    public string SupplierContact = "";
    public string SupplierTel = "";
    public string QuoteTime = "";
    public string Validity = "";
    public string Status = "未知";
    public string StatusClass = "gray";
    public string Remarks = "无";
    public string PurchaseQuantity = "0";
    public string PurchaseUnit = "Kpcs";
    public int EqId = 0;

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
                EqId = eqId;
                Model = GetStringValue(row["goodsSn"]);
                BrandName = GetStringValue(row["brandName"]);
                Quantity = GetIntValue(row["fromQuantity"], 0).ToString();
                Unit = "Kpcs";
                
                PurchaseQuantity = GetIntValue(row["toQuantity"], 0).ToString();
                PurchaseUnit = "Kpcs";
                
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
                BuyerContact = GetStringValue(row["toContact"]);
                BuyerTel = GetStringValue(row["toTel"]);
                
                SupplierName = GetStringValue(row["fromCompany"]);
                if (string.IsNullOrEmpty(SupplierName))
                {
                    SupplierName = "匿名供应商";
                }
                SupplierContact = GetStringValue(row["fromContact"]);
                SupplierTel = GetStringValue(row["fromTel"]);
                
                DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
                QuoteTime = createTime.ToString("yyyy-MM-dd HH:mm:ss");
                
                string validity = GetStringValue(row["validity"]);
                if (!string.IsNullOrEmpty(validity))
                {
                    if (validity == "24小时") validity = "24 小时内";
                    else if (validity == "1个月") validity = "30 天内";
                    else if (validity.EndsWith("天")) validity = validity + "内";
                    Validity = validity;
                }
                else
                {
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
                
                PageTitle = Model + " " + BrandName + " 报价详情 - 阻容网";
                PageKeywords = Model + "," + BrandName + ",报价详情,报价记录,供应商,采购商,电子元器件,阻容网";
                PageDescription = Model + " (" + BrandName + ") 报价详情，报价数量：" + Quantity + "Kpcs，报价单价：" + PriceDisplay + "，采购商：" + BuyerName + "，供应商：" + SupplierName + "。";
                
                int currentUserId = UserHelper.GetUserId();
                int toUserId = GetIntValue(row["toUserId"], 0);
                int toShopId = GetIntValue(row["toShopId"], 0);
                
                bool isBuyerViewing = false;
                if (currentUserId > 0 && toUserId > 0 && currentUserId == toUserId)
                {
                    isBuyerViewing = true;
                }
                else if (toShopId > 0)
                {
                    int currentShopId = 0;
                    if (Session["ShopId"] != null)
                    {
                        int.TryParse(Session["ShopId"].ToString(), out currentShopId);
                    }
                    if (currentShopId == 0 && currentUserId > 0)
                    {
                        string shopSql = "SELECT shopId FROM shops WHERE userId = @userId";
                        DataTable shopDt = DbHelper.ExecuteQuery(shopSql, DbHelper.CreateParameter("@userId", currentUserId));
                        if (shopDt != null && shopDt.Rows.Count > 0)
                        {
                            currentShopId = GetIntValue(shopDt.Rows[0]["shopId"], 0);
                            Session["ShopId"] = currentShopId;
                        }
                    }
                    if (currentShopId > 0 && currentShopId == toShopId)
                    {
                        isBuyerViewing = true;
                    }
                }
                
                if (isBuyerViewing)
                {
                    service.UpdateReadStatus(eqId, 1);
                }
                
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