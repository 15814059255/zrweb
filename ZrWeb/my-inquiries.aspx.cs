using System;
using System.Data;
using System.Threading;
using System.Web;

public partial class my_inquiries : System.Web.UI.Page
{
    public string PageTitle = "我的询价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,我的询价,询价管理,电子元器件采购";
    public string PageDescription = "阻容网询价记录，查看您向供应商发出的询价及回复状态。";
    
    public int CurrentPage = 1;
    public int TotalPages = 1;
    public bool HasInquiryData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindInquiries();
        }
    }

    private void BindInquiries()
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

            DataTable dt = new DataTable();
            dt.Columns.Add("EqId", typeof(int));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("ExpectedPrice", typeof(string));
            dt.Columns.Add("ShopName", typeof(string));
            dt.Columns.Add("InquiryTime", typeof(string));
            dt.Columns.Add("Remarks", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("StatusClass", typeof(string));

            if (shopId > 0)
            {
                string sql = @"SELECT TOP 50 
                    e.eqId, e.goodsSn, e.fromQuantity, e.toQuantity, e.toPrice, e.fromPrice,
                    e.isIncludingTax, e.fromRemarks, e.createTime, e.readStatus,
                    e.brandName, e.toShopId,
                    s.shopName, s.shopCompany
                    FROM enquiryquoteprice e
                    LEFT JOIN shops s ON e.toShopId = s.shopId
                    WHERE e.fromShopID = @shopId AND e.eqType = 1 AND e.dataFlag = 1
                    ORDER BY e.createTime DESC";

                DataTable sourceDt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@shopId", shopId));

                if (sourceDt != null && sourceDt.Rows.Count > 0)
                {
                    foreach (DataRow row in sourceDt.Rows)
                    {
                        DataRow newRow = dt.NewRow();
                        
                        newRow["EqId"] = GetIntValue(row["eqId"], 0);
                        newRow["Model"] = GetStringValue(row["goodsSn"]);
                        
                        // 获取品牌参数
                        string brand = GetStringValue(row["brandName"]);
                        newRow["BrandParams"] = string.IsNullOrEmpty(brand) ? "品牌不限" : brand;

                        // 获取数量
                        int fromQty = GetIntValue(row["fromQuantity"], 0);
                        int toQty = GetIntValue(row["toQuantity"], 0);
                        int qty = toQty > 0 ? toQty : fromQty;
                        newRow["Quantity"] = qty > 0 ? qty.ToString() : "0";
                        newRow["Unit"] = "Kpcs";

                        // 获取期望单价
                        decimal price = GetDecimalValue(row["toPrice"], 0);
                        if (price == 0)
                        {
                            price = GetDecimalValue(row["fromPrice"], 0);
                        }
                        newRow["ExpectedPrice"] = price > 0 ? "¥" + price.ToString("0.0000") : "面议";

                        // 获取询价对象（供应商名称）
                        string shopName = GetStringValue(row["shopCompany"]);
                        if (string.IsNullOrEmpty(shopName))
                        {
                            shopName = GetStringValue(row["shopName"]);
                        }
                        newRow["ShopName"] = string.IsNullOrEmpty(shopName) ? "未知供应商" : shopName;

                        newRow["InquiryTime"] = Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm");
                        newRow["Remarks"] = GetStringValue(row["fromRemarks"]);

                        // 状态
                        int readStatus = GetIntValue(row["readStatus"], 0);
                        if (readStatus == 0)
                        {
                            newRow["Status"] = "待回复";
                            newRow["StatusClass"] = "orange";
                        }
                        else
                        {
                            newRow["Status"] = "已回复";
                            newRow["StatusClass"] = "blue";
                        }

                        dt.Rows.Add(newRow);
                    }

                    TotalPages = (int)Math.Ceiling((double)dt.Rows.Count / 50);
                    if (TotalPages < 1) TotalPages = 1;
                }
            }

            HasInquiryData = dt != null && dt.Rows.Count > 0;
            rptInquiries.DataSource = dt;
            rptInquiries.DataBind();
        }
        catch (Exception)
        {
            HasInquiryData = false;
            rptInquiries.DataSource = null;
            rptInquiries.DataBind();
        }
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

    private string GetStringValue(object value)
    {
        if (value == DBNull.Value || value == null)
            return "";
        return value.ToString();
    }
}
