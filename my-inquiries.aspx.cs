using System;
using System.Data;

public partial class my_inquiries : System.Web.UI.Page
{
    public string PageTitle = "我的询价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,我的询价,询价管理,采购商";
    public string PageDescription = "查看您发起的询价记录，跟踪询价进度。";
    
    public int CurrentPage = 1;
    public int TotalPages = 1;
    public bool HasData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        BindInquiries();
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
            dt.Columns.Add("TargetCompany");
            dt.Columns.Add("InquiryTime");
            dt.Columns.Add("Validity");
            dt.Columns.Add("ReplyCount");

            if (shopId > 0)
            {
                string sql = @"SELECT TOP 50 
                    e.eqId, e.goodsId, e.goodsSn, e.fromQuantity, e.toQuantity, e.toPrice,
                    e.isIncludingTax, e.createTime, e.readStatus,
                    e.toCompany, e.brandName
                    FROM enquiryquoteprice e
                    WHERE e.fromShopId = @shopId AND e.eqType = 1 AND e.dataFlag = 1
                    ORDER BY e.createTime DESC";

                DataTable sourceDt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@shopId", shopId));

                if (sourceDt != null && sourceDt.Rows.Count > 0)
                {
                    HasData = true;
                    foreach (DataRow row in sourceDt.Rows)
                    {
                        DataRow newRow = dt.NewRow();
                        
                        newRow["EqId"] = GetIntValue(row["eqId"], 0);
                        newRow["GoodsId"] = GetIntValue(row["goodsId"], 0);
                        newRow["Model"] = GetStringValue(row["goodsSn"]);
                        newRow["TargetCompany"] = GetStringValue(row["toCompany"]);
                        newRow["InquiryTime"] = Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm");

                        string brand = GetStringValue(row["brandName"]);
                        newRow["BrandParams"] = string.IsNullOrEmpty(brand) ? "品牌不限" : brand;

                        int fromQty = GetIntValue(row["fromQuantity"], 0);
                        int toQty = GetIntValue(row["toQuantity"], 0);
                        int qty = toQty > 0 ? toQty : fromQty;
                        newRow["Quantity"] = qty > 0 ? qty.ToString() : "0";
                        newRow["Unit"] = "Kpcs";

                        decimal price = GetDecimalValue(row["toPrice"], 0);
                        string taxText = GetIntValue(row["isIncludingTax"], 0) == 1 ? "(含税)" : "(未税)";
                        newRow["Price"] = price > 0 ? price.ToString("0.0000") + " " + taxText : "面议";

                        int replyCount = GetReplyCount(GetIntValue(row["eqId"], 0));
                        newRow["ReplyCount"] = replyCount;

                        if (replyCount > 0)
                        {
                            newRow["Status"] = "已回复";
                            newRow["StatusClass"] = "green";
                        }
                        else
                        {
                            newRow["Status"] = "待回复";
                            newRow["StatusClass"] = "orange";
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
            }

            rptInquiries.DataSource = dt;
            rptInquiries.DataBind();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("BindInquiries 错误: " + ex.Message);
            rptInquiries.DataSource = null;
            rptInquiries.DataBind();
        }
    }

    private int GetReplyCount(int eqId)
    {
        try
        {
            string sql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE parentId = @eqId AND dataFlag = 1";
            object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@eqId", eqId));
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        catch
        {
            return 0;
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