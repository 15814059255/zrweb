using System;
using System.Data;

public partial class received_quotes : System.Web.UI.Page
{
    public string PageTitle = "收到报价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,收到报价,报价管理,采购商";
    public string PageDescription = "查看供应商发来的报价信息，选择合适的供应商进行采购。";
    
    public int CurrentPage = 1;
    public int TotalPages = 3;

    protected void Page_Load(object sender, EventArgs e)
    {
        BindQuotes();
    }

    private void BindQuotes()
    {
        try
        {
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
            dt.Columns.Add("Price");
            dt.Columns.Add("SellerName");
            dt.Columns.Add("SellerContact");
            dt.Columns.Add("QuoteTime");
            dt.Columns.Add("Validity");
            dt.Columns.Add("Remarks");

            string sql = @"SELECT TOP 50 
                e.eqId, e.goodsId, e.goodsSn, e.fromQuantity, e.toQuantity, e.fromPrice,
                e.isIncludingTax, e.fromRemarks, e.createTime, e.readStatus,
                e.fromCompany, e.fromContact, e.fromTel, e.brandName
                FROM enquiryquoteprice e
                WHERE e.toShopId = @shopId AND e.eqType = 2 AND e.dataFlag = 1
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
                    newRow["SellerName"] = GetStringValue(row["fromCompany"]);
                    newRow["SellerContact"] = GetStringValue(row["fromContact"]);
                    newRow["QuoteTime"] = Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm");
                    newRow["Remarks"] = GetStringValue(row["fromRemarks"]);

                    string brand = GetStringValue(row["brandName"]);
                    newRow["BrandParams"] = string.IsNullOrEmpty(brand) ? "品牌不限" : brand;

                    int fromQty = GetIntValue(row["fromQuantity"], 0);
                    int toQty = GetIntValue(row["toQuantity"], 0);
                    int qty = toQty > 0 ? toQty : fromQty;
                    newRow["Quantity"] = qty > 0 ? qty.ToString() : "0";
                    newRow["Unit"] = "Kpcs";

                    decimal price = GetDecimalValue(row["fromPrice"], 0);
                    string taxText = GetIntValue(row["isIncludingTax"], 0) == 1 ? "(含税)" : "(未税)";
                    newRow["Price"] = price > 0 ? price.ToString("0.0000") + " " + taxText : "面议";

                    int readStatus = GetIntValue(row["readStatus"], 0);
                    if (readStatus == 0)
                    {
                        newRow["Status"] = "新报价";
                        newRow["StatusClass"] = "green";
                    }
                    else
                    {
                        newRow["Status"] = "已查看";
                        newRow["StatusClass"] = "gray";
                    }

                    TimeSpan diff = DateTime.Now - Convert.ToDateTime(row["createTime"]);
                    if (diff.TotalHours < 24)
                    {
                        newRow["Validity"] = "<24 小时";
                    }
                    else if (diff.TotalDays < 3)
                    {
                        newRow["Validity"] = "72 小时";
                    }
                    else if (diff.TotalDays < 7)
                    {
                        newRow["Validity"] = "7 天";
                    }
                    else
                    {
                        newRow["Validity"] = "长期";
                    }

                    dt.Rows.Add(newRow);
                }

                TotalPages = (int)Math.Ceiling((double)dt.Rows.Count / 50);
                if (TotalPages < 1) TotalPages = 1;
            }

            rptQuotes.DataSource = dt;
            rptQuotes.DataBind();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("BindQuotes 错误: " + ex.Message);
            rptQuotes.DataSource = null;
            rptQuotes.DataBind();
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