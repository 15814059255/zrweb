using System;
using System.Data;

public partial class received_inquiries : System.Web.UI.Page
{
    public string PageTitle = "收到询价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,收到询价,询价管理,供应商";
    public string PageDescription = "查看采购商发来的询价信息，及时回复报价，促成交易。";
    
    public int CurrentPage = 1;
    public int TotalPages = 2;

    protected void Page_Load(object sender, EventArgs e)
    {
        BindInquiries();
    }

    private void BindInquiries()
    {
        try
        {
            // 获取当前用户的 shopId
            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("EqId", typeof(int));
            dt.Columns.Add("GoodsId", typeof(int));
            dt.Columns.Add("Status");
            dt.Columns.Add("StatusClass");
            dt.Columns.Add("Model");
            dt.Columns.Add("BrandParams");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Unit");
            dt.Columns.Add("ExpectedPrice");
            dt.Columns.Add("BuyerName");
            dt.Columns.Add("BuyerContact");
            dt.Columns.Add("InquiryTime");
            dt.Columns.Add("Remarks");

            string sql = @"SELECT TOP 50 
                e.eqId, e.goodsId, e.goodsSn, e.fromQuantity, e.toQuantity, e.toPrice, e.fromPrice,
                e.isIncludingTax, e.fromRemarks, e.createTime, e.readStatus,
                e.fromCompany, e.fromContact, e.fromTel, e.brandName
                FROM enquiryquoteprice e
                WHERE e.toShopId = @shopId AND e.eqType = 1 AND e.dataFlag = 1
                ORDER BY e.createTime DESC";

            DataTable sourceDt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@shopId", shopId));

            if (sourceDt != null && sourceDt.Rows.Count > 0)
            {
                foreach (DataRow row in sourceDt.Rows)
                {
                    DataRow newRow = dt.NewRow();
                    
                    newRow["EqId"] = GetIntValue(row["eqId"], 0);
                    newRow["GoodsId"] = GetIntValue(row["goodsId"], 0);
                    newRow["Model"] = GetStringValue(row["goodsSn"]);
                    newRow["BuyerName"] = GetStringValue(row["fromCompany"]);
                    newRow["BuyerContact"] = GetStringValue(row["fromContact"]);
                    newRow["InquiryTime"] = Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm");
                    newRow["Remarks"] = GetStringValue(row["fromRemarks"]);

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
                    newRow["ExpectedPrice"] = price > 0 ? price.ToString("0.0000") : "面议";

                    // 状态
                    int readStatus = GetIntValue(row["readStatus"], 0);
                    if (readStatus == 0)
                    {
                        newRow["Status"] = "新询价";
                        newRow["StatusClass"] = "green";
                    }
                    else
                    {
                        newRow["Status"] = "已读";
                        newRow["StatusClass"] = "blue";
                    }

                    dt.Rows.Add(newRow);
                }

                TotalPages = (int)Math.Ceiling((double)dt.Rows.Count / 50);
                if (TotalPages < 1) TotalPages = 1;
            }

            rptInquiries.DataSource = dt;
            rptInquiries.DataBind();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("BindInquiries 错误: " + ex.Message);
            // 发生错误时显示空表格
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