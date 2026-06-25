using System;
using System.Data;

public partial class quote_records : System.Web.UI.Page
{
    public string PageTitle = "我的报价 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,我的报价,报价记录,供应商";
    public string PageDescription = "查看已提交的报价记录，跟踪报价状态，了解采购商反馈。";
    
    public int CurrentPage = 1;
    public int TotalPages = 1;
    public bool HasData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                EnquiryQuoteService fixService = new EnquiryQuoteService();
                fixService.FixQuoteRecordsWithInquiryInfo();
            }
            catch { }
            BindQuoteRecords();
        }
    }

    private int GetInquiryQuantity(string goodsSn, int fromShopId, int toShopId)
    {
        try
        {
            string sql = "SELECT TOP 1 fromQuantity FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = @goodsSn AND fromShopId = @fromShopId AND toShopId = @toShopId AND dataFlag = 1 ORDER BY createTime DESC";
            object result = DbHelper.ExecuteScalar(sql,
                DbHelper.CreateParameter("@goodsSn", goodsSn),
                DbHelper.CreateParameter("@fromShopId", fromShopId),
                DbHelper.CreateParameter("@toShopId", toShopId));
            if (result != null && result != DBNull.Value)
                return Convert.ToInt32(result);
        }
        catch { }
        return 0;
    }

    private decimal GetInquiryPrice(string goodsSn, int fromShopId, int toShopId)
    {
        try
        {
            string sql = "SELECT TOP 1 fromPrice FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = @goodsSn AND fromShopId = @fromShopId AND toShopId = @toShopId AND dataFlag = 1 ORDER BY createTime DESC";
            object result = DbHelper.ExecuteScalar(sql,
                DbHelper.CreateParameter("@goodsSn", goodsSn),
                DbHelper.CreateParameter("@fromShopId", fromShopId),
                DbHelper.CreateParameter("@toShopId", toShopId));
            if (result != null && result != DBNull.Value)
                return Convert.ToDecimal(result);
        }
        catch { }
        return 0;
    }

    private string GetInquiryRemarks(string goodsSn, int fromShopId, int toShopId)
    {
        try
        {
            string sql = "SELECT TOP 1 fromRemarks FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = @goodsSn AND fromShopId = @fromShopId AND toShopId = @toShopId AND dataFlag = 1 ORDER BY createTime DESC";
            object result = DbHelper.ExecuteScalar(sql,
                DbHelper.CreateParameter("@goodsSn", goodsSn),
                DbHelper.CreateParameter("@fromShopId", fromShopId),
                DbHelper.CreateParameter("@toShopId", toShopId));
            if (result != null && result != DBNull.Value)
                return result.ToString();
        }
        catch { }
        return "";
    }

    private void BindQuoteRecords()
    {
        DataTable dt = null;
        try
        {
            int userId = UserHelper.GetUserId();
            int shopId = 0;

            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }
            
            if (shopId == 0 && userId > 0)
            {
                string sql = "SELECT shopId FROM shops WHERE userId = @userId";
                DataTable shopDt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@userId", userId));
                if (shopDt != null && shopDt.Rows.Count > 0)
                {
                    shopId = GetIntValue(shopDt.Rows[0]["shopId"], 0);
                    Session["ShopId"] = shopId;
                }
            }

            EnquiryQuoteService service = new EnquiryQuoteService();
            
            if (shopId > 0)
            {
                dt = service.GetQuotesByShop(shopId);
            }
            else if (userId > 0)
            {
                dt = service.GetSentQuotes(userId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("BindQuoteRecords 错误: " + ex.Message);
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasData = false;
            dt = new DataTable();
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("StatusClass", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("InquiryQuantity", typeof(string));
            dt.Columns.Add("InquiryUnit", typeof(string));
            dt.Columns.Add("InquiryPrice", typeof(string));
            dt.Columns.Add("InquiryRemarks", typeof(string));
            dt.Columns.Add("MyQuantity", typeof(string));
            dt.Columns.Add("MyUnit", typeof(string));
            dt.Columns.Add("MyPrice", typeof(string));
            dt.Columns.Add("MyBatch", typeof(string));
            dt.Columns.Add("MyRemarks", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("Batch", typeof(string));
            dt.Columns.Add("Remarks", typeof(string));
            dt.Columns.Add("BuyerName", typeof(string));
            dt.Columns.Add("BuyerQQ", typeof(string));
            dt.Columns.Add("QuoteTime", typeof(string));
            dt.Columns.Add("Validity", typeof(string));
        }
        else
        {
            HasData = true;
            TotalPages = (int)Math.Ceiling((double)dt.Rows.Count / 50);
            
            // 确保 StatusClass 字段存在
            if (!dt.Columns.Contains("StatusClass"))
            {
                dt.Columns.Add("StatusClass", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    int readStatus = GetIntValue(row["readStatus"], 0);
                    row["StatusClass"] = readStatus == 1 ? "blue" : "green";
                    
                    if (!dt.Columns.Contains("Status"))
                    {
                        dt.Columns.Add("Status", typeof(string));
                    }
                    row["Status"] = readStatus == 1 ? "已查看" : "未查看";
                }
            }
            
            // 确保所有必要字段存在
            if (!dt.Columns.Contains("BuyerQQ"))
                dt.Columns.Add("BuyerQQ", typeof(string));
            if (!dt.Columns.Contains("BrandParams"))
                dt.Columns.Add("BrandParams", typeof(string));
            if (!dt.Columns.Contains("InquiryQuantity"))
                dt.Columns.Add("InquiryQuantity", typeof(string));
            if (!dt.Columns.Contains("InquiryUnit"))
                dt.Columns.Add("InquiryUnit", typeof(string));
            if (!dt.Columns.Contains("InquiryPrice"))
                dt.Columns.Add("InquiryPrice", typeof(string));
            if (!dt.Columns.Contains("InquiryRemarks"))
                dt.Columns.Add("InquiryRemarks", typeof(string));
            if (!dt.Columns.Contains("MyQuantity"))
                dt.Columns.Add("MyQuantity", typeof(string));
            if (!dt.Columns.Contains("MyUnit"))
                dt.Columns.Add("MyUnit", typeof(string));
            if (!dt.Columns.Contains("MyPrice"))
                dt.Columns.Add("MyPrice", typeof(string));
            if (!dt.Columns.Contains("MyBatch"))
                dt.Columns.Add("MyBatch", typeof(string));
            if (!dt.Columns.Contains("MyRemarks"))
                dt.Columns.Add("MyRemarks", typeof(string));
            if (!dt.Columns.Contains("Quantity"))
                dt.Columns.Add("Quantity", typeof(string));
            if (!dt.Columns.Contains("Unit"))
                dt.Columns.Add("Unit", typeof(string));
            if (!dt.Columns.Contains("Price"))
                dt.Columns.Add("Price", typeof(string));
            if (!dt.Columns.Contains("Batch"))
                dt.Columns.Add("Batch", typeof(string));
            if (!dt.Columns.Contains("Remarks"))
                dt.Columns.Add("Remarks", typeof(string));
            if (!dt.Columns.Contains("BuyerName"))
                dt.Columns.Add("BuyerName", typeof(string));
            if (!dt.Columns.Contains("QuoteTime"))
                dt.Columns.Add("QuoteTime", typeof(string));
            if (!dt.Columns.Contains("Validity"))
                dt.Columns.Add("Validity", typeof(string));
            
            // 修正询价方信息：从原始询价记录查询
            if (dt.Columns.Contains("Model") && dt.Columns.Contains("FromShopId") && dt.Columns.Contains("ToShopId"))
            {
                foreach (DataRow row in dt.Rows)
                {
                    string iqtyStr = row["InquiryQuantity"].ToString();
                    int iqty = 0;
                    int.TryParse(iqtyStr, out iqty);
                    
                    if (iqty <= 0)
                    {
                        string model = row["Model"].ToString();
                        int fromShopIdVal = 0;
                        int toShopIdVal = 0;
                        try { fromShopIdVal = Convert.ToInt32(row["FromShopId"]); } catch { }
                        try { toShopIdVal = Convert.ToInt32(row["ToShopId"]); } catch { }
                        
                        if (!string.IsNullOrEmpty(model) && fromShopIdVal > 0 && toShopIdVal > 0)
                        {
                            int realQty = GetInquiryQuantity(model, toShopIdVal, fromShopIdVal);
                            if (realQty > 0)
                            {
                                row["InquiryQuantity"] = realQty.ToString();
                            }
                            
                            decimal realPrice = GetInquiryPrice(model, toShopIdVal, fromShopIdVal);
                            if (realPrice > 0)
                            {
                                row["InquiryPrice"] = "¥" + realPrice.ToString("0.00");
                            }
                            
                            string realRemarks = GetInquiryRemarks(model, toShopIdVal, fromShopIdVal);
                            if (!string.IsNullOrEmpty(realRemarks))
                            {
                                row["InquiryRemarks"] = realRemarks;
                            }
                        }
                    }
                }
            }
            
            // 为缺失的字段填充默认值
            foreach (DataRow row in dt.Rows)
            {
                if (row["InquiryQuantity"] == DBNull.Value || string.IsNullOrEmpty(row["InquiryQuantity"].ToString()))
                    row["InquiryQuantity"] = "0";
                if (row["InquiryUnit"] == DBNull.Value || string.IsNullOrEmpty(row["InquiryUnit"].ToString()))
                    row["InquiryUnit"] = "Kpcs";
                if (row["InquiryPrice"] == DBNull.Value || string.IsNullOrEmpty(row["InquiryPrice"].ToString()))
                    row["InquiryPrice"] = "面议";
                if (row["InquiryRemarks"] == DBNull.Value || string.IsNullOrEmpty(row["InquiryRemarks"].ToString()))
                    row["InquiryRemarks"] = "-";
                if (row["MyQuantity"] == DBNull.Value || string.IsNullOrEmpty(row["MyQuantity"].ToString()))
                    row["MyQuantity"] = row["Quantity"];
                if (row["MyUnit"] == DBNull.Value || string.IsNullOrEmpty(row["MyUnit"].ToString()))
                    row["MyUnit"] = row["Unit"];
                if (row["MyPrice"] == DBNull.Value || string.IsNullOrEmpty(row["MyPrice"].ToString()))
                    row["MyPrice"] = row["Price"];
                if (row["MyBatch"] == DBNull.Value || string.IsNullOrEmpty(row["MyBatch"].ToString()))
                    row["MyBatch"] = row["Batch"];
                if (row["MyRemarks"] == DBNull.Value || string.IsNullOrEmpty(row["MyRemarks"].ToString()))
                    row["MyRemarks"] = row["Remarks"];
            }
        }

        rptQuoteRecords.DataSource = dt;
        rptQuoteRecords.DataBind();
    }

    private int GetIntValue(object value, int defaultValue)
    {
        if (value == DBNull.Value || value == null)
            return defaultValue;
        return Convert.ToInt32(value);
    }
}