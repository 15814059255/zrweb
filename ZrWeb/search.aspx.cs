using System;
using System.Web.UI;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public partial class search : Page
{
    protected string PageTitle = "搜索结果 - 电子元器件 B2B 平台";
    protected string PageKeywords = "阻容网,搜索,电子元器件,贴片电容,贴片电阻";
    protected string PageDescription = "阻容网搜索功能，快速查找电子元器件供应和需求信息。";
    
    protected string SearchKeyword = "";
    protected int ResultCount = 0;
    protected string PaginationHtml = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        SearchKeyword = Request.QueryString["q"] ?? "";
        
        if (!IsPostBack)
        {
            BindSearchResults();
        }
    }

    private void BindSearchResults()
    {
        int pageIndex = 1;
        int pageSize = 10;
        string pageStr = Request.QueryString["page"];
        if (!string.IsNullOrEmpty(pageStr))
        {
            int.TryParse(pageStr, out pageIndex);
        }
        if (pageIndex < 1) pageIndex = 1;

        int totalCount = 0;
        GoodsService service = new GoodsService();
        DataTable dt = service.SearchGoods(SearchKeyword, pageIndex, pageSize, out totalCount);

        ResultCount = totalCount;

        if (totalCount > 0 && (dt == null || dt.Rows.Count == 0))
        {
            dt = GetSearchResultsFallback(SearchKeyword, pageIndex, pageSize);
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            dt = new DataTable();
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("DetailUrl", typeof(string));
            dt.Columns.Add("TagClass", typeof(string));
            dt.Columns.Add("TypeLabel", typeof(string));
            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("QuantityDisplay", typeof(string));
            dt.Columns.Add("Validity", typeof(string));
            dt.Columns.Add("PriceClass", typeof(string));
            dt.Columns.Add("PriceDisplay", typeof(string));
            dt.Columns.Add("ActionClass", typeof(string));
            dt.Columns.Add("ActionText", typeof(string));
            dt.Columns.Add("ShopId", typeof(int));
            dt.Columns.Add("GoodsId", typeof(int));
        }

        rptSearchResults.DataSource = dt;
        rptSearchResults.DataBind();

        PaginationHtml = GeneratePagination(pageIndex, pageSize, ResultCount);
    }

    private DataTable GetSearchResultsFallback(string keyword, int pageIndex, int pageSize)
    {
        try
        {
            string connStr = "";
            var connSetting = ConfigurationManager.ConnectionStrings["zrweb2ConnectionString"];
            if (connSetting != null) connStr = connSetting.ConnectionString;
            if (string.IsNullOrEmpty(connStr)) return null;

            string whereSql = "WHERE g.dataFlag = 1 AND g.goodsStatus = 1 AND g.isSale = 1";
            var parameters = new System.Collections.Generic.List<SqlParameter>();

            if (!string.IsNullOrEmpty(keyword) && keyword.Trim().Length > 0)
            {
                string kw = keyword.Trim();
                whereSql += " AND (g.goodsSn LIKE @keyword OR g.Name LIKE @keyword OR g.Manufacturers LIKE @keyword OR g.goodsDesc LIKE @keyword)";
                parameters.Add(new SqlParameter("@keyword", "%" + kw + "%"));
            }

            string countSql = "SELECT COUNT(*) FROM goods g " + whereSql;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (SqlCommand cmdCount = new SqlCommand(countSql, conn))
                {
                    foreach (var p in parameters)
                        cmdCount.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                    
                    object countObj = cmdCount.ExecuteScalar();
                    if (countObj != null && countObj != DBNull.Value)
                    {
                        ResultCount = Convert.ToInt32(countObj);
                    }
                }

                if (ResultCount == 0) return null;

                int startRow = (pageIndex - 1) * pageSize + 1;
                int endRow = pageIndex * pageSize;

                string sql = @"SELECT * FROM (
                    SELECT ROW_NUMBER() OVER (ORDER BY g.createTime DESC) AS rowNum,
                        g.goodsId, g.goodsSn, g.Name, g.Manufacturers, g.Packaging, g.goodsStock, g.goodsUnit, 
                        g.shopPrice, g.isIncludingTax, g.createTime, g.validityDate, g.isSale, 
                        g.goodsStatus, g.dataFlag, g.pubType, g.shopId,
                        g.Brand, g.Capacitance, g.Resistance, g.Tolerance, g.Voltage, g.Dielectric, g.Power, g.TempCoefficient,
                        ISNULL(s.shopCompany, s.shopName) AS companyName
                    FROM goods g
                    LEFT JOIN shops s ON g.shopId = s.shopId
                    " + whereSql + @"
                ) AS t WHERE rowNum BETWEEN @startRow AND @endRow ORDER BY createTime DESC";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    foreach (var p in parameters)
                        cmd.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                    cmd.Parameters.AddWithValue("@startRow", startRow);
                    cmd.Parameters.AddWithValue("@endRow", endRow);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    return ConvertToDisplayData(dt);
                }
            }
        }
        catch (Exception)
        {
        }
        return null;
    }

    private DataTable ConvertToDisplayData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ItemType", typeof(string));
        dt.Columns.Add("TagClass", typeof(string));
        dt.Columns.Add("TypeLabel", typeof(string));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("DetailUrl", typeof(string));
        dt.Columns.Add("PriceDisplay", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("QuantityDisplay", typeof(string));
        dt.Columns.Add("Validity", typeof(string));
        dt.Columns.Add("CompanyName", typeof(string));
        dt.Columns.Add("ActionText", typeof(string));
        dt.Columns.Add("PriceClass", typeof(string));
        dt.Columns.Add("ActionClass", typeof(string));
        dt.Columns.Add("GoodsId", typeof(int));
        dt.Columns.Add("GoodsSn", typeof(string));
        dt.Columns.Add("ShopId", typeof(int));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            string goodsSn = row["goodsSn"] != DBNull.Value ? row["goodsSn"].ToString() : "";
            string name = row["Name"] != DBNull.Value ? row["Name"].ToString() : "";
            string manufacturers = row["Manufacturers"] != DBNull.Value ? row["Manufacturers"].ToString() : "";
            int goodsStock = row["goodsStock"] != DBNull.Value ? Convert.ToInt32(row["goodsStock"]) : 0;
            string goodsUnit = row["goodsUnit"] != DBNull.Value ? row["goodsUnit"].ToString() : "";
            decimal shopPrice = row["shopPrice"] != DBNull.Value ? Convert.ToDecimal(row["shopPrice"]) : 0;
            int isIncludingTax = row["isIncludingTax"] != DBNull.Value ? Convert.ToInt32(row["isIncludingTax"]) : 0;
            DateTime createTime = row["createTime"] != DBNull.Value ? Convert.ToDateTime(row["createTime"]) : DateTime.Now;
            DateTime validityDate = row["validityDate"] != DBNull.Value ? Convert.ToDateTime(row["validityDate"]) : DateTime.Now.AddDays(3);
            int goodsId = row["goodsId"] != DBNull.Value ? Convert.ToInt32(row["goodsId"]) : 0;
            int pubType = row["pubType"] != DBNull.Value ? Convert.ToInt32(row["pubType"]) : 1;
            int shopId = row["shopId"] != DBNull.Value ? Convert.ToInt32(row["shopId"]) : 0;
            string companyName = row["companyName"] != DBNull.Value ? row["companyName"].ToString() : "";

            newRow["GoodsId"] = goodsId;
            newRow["GoodsSn"] = goodsSn;
            newRow["ShopId"] = shopId;

            if (pubType == 2)
            {
                newRow["ItemType"] = "demand";
                newRow["TagClass"] = "green";
                newRow["TypeLabel"] = "需求";
            }
            else
            {
                newRow["ItemType"] = "supply";
                newRow["TagClass"] = "blue";
                newRow["TypeLabel"] = "供应";
            }

            newRow["Model"] = !string.IsNullOrEmpty(goodsSn) ? goodsSn : (!string.IsNullOrEmpty(name) ? name : "未知型号");

            if (pubType == 2)
                newRow["DetailUrl"] = "/demand-detail.aspx?id=" + goodsId;
            else
                newRow["DetailUrl"] = "/supply-detail.aspx?id=" + goodsId;

            if (shopPrice > 0)
            {
                string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                string taxText = isIncludingTax == 1 ? "(含税)" : "(未税)";
                newRow["PriceDisplay"] = "¥" + shopPrice.ToString("0.00") + " /" + unit + " " + taxText;
            }
            else
            {
                newRow["PriceDisplay"] = "面议";
            }

            string brand = row["Brand"] != DBNull.Value ? row["Brand"].ToString() : "";
            string capacitance = row["Capacitance"] != DBNull.Value ? row["Capacitance"].ToString() : "";
            string resistance = row["Resistance"] != DBNull.Value ? row["Resistance"].ToString() : "";
            string tolerance = row["Tolerance"] != DBNull.Value ? row["Tolerance"].ToString() : "";
            string voltage = row["Voltage"] != DBNull.Value ? row["Voltage"].ToString() : "";
            string dielectric = row["Dielectric"] != DBNull.Value ? row["Dielectric"].ToString() : "";

            System.Collections.Generic.List<string> paramsList = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(brand)) paramsList.Add(brand);
            if (!string.IsNullOrEmpty(capacitance)) paramsList.Add(capacitance);
            if (!string.IsNullOrEmpty(resistance)) paramsList.Add(resistance);
            if (!string.IsNullOrEmpty(tolerance)) paramsList.Add(tolerance);
            if (!string.IsNullOrEmpty(voltage)) paramsList.Add(voltage);
            if (!string.IsNullOrEmpty(dielectric)) paramsList.Add(dielectric);
            
            string brandParams = string.Join(" · ", paramsList);
            newRow["BrandParams"] = !string.IsNullOrEmpty(brandParams) ? brandParams : (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限");

            if (pubType == 2)
            {
                if (goodsStock > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    newRow["QuantityDisplay"] = "需求 " + goodsStock + "/" + unit;
                }
                else
                    newRow["QuantityDisplay"] = "需求数量待定";
            }
            else
            {
                if (goodsStock > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    newRow["QuantityDisplay"] = "现货 " + goodsStock + "/" + unit;
                }
                else
                    newRow["QuantityDisplay"] = "按需供应";
            }

            TimeSpan diff = validityDate - DateTime.Now;
            if (diff.TotalHours < 0)
                newRow["Validity"] = "已过期";
            else if (diff.TotalHours < 24)
                newRow["Validity"] = "小于 24 小时";
            else if (diff.TotalDays < 3)
                newRow["Validity"] = (int)diff.TotalHours + " 小时";
            else if (diff.TotalDays >= 30)
                newRow["Validity"] = (int)(diff.TotalDays / 30) + " 个月";
            else
                newRow["Validity"] = (int)diff.TotalDays + " 天";

            newRow["CompanyName"] = !string.IsNullOrEmpty(companyName) ? companyName : "未知商家";
            
            if (pubType == 2)
            {
                newRow["ActionText"] = "我要报价";
                newRow["ActionClass"] = "is-demand-action";
            }
            else
            {
                newRow["ActionText"] = "立即询价";
                newRow["ActionClass"] = "";
            }

            string priceClass = "";
            if (pubType == 2 && shopPrice > 0)
                priceClass = "is-expected";
            else if (shopPrice <= 0)
                priceClass = "is-negotiable";
            newRow["PriceClass"] = priceClass;

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private string GeneratePagination(int currentPage, int pageSize, int totalCount)
    {
        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (totalPages <= 1) return "";
        
        string html = "";
        string kw = System.Web.HttpUtility.UrlEncode(SearchKeyword);
        if (currentPage > 1)
            html += "<button class=\"btn\" type=\"button\" onclick=\"location.href='/search.aspx?q=" + kw + "&page=" + (currentPage - 1) + "'\">上一页</button>";
        
        html += "<span>第 " + currentPage + " / " + totalPages + " 页</span>";
        html += "<span class=\"page-size\">每页 " + pageSize + " 条</span>";
        
        if (currentPage < totalPages)
            html += "<button class=\"btn\" type=\"button\" onclick=\"location.href='/search.aspx?q=" + kw + "&page=" + (currentPage + 1) + "'\">下一页</button>";
        
        return html;
    }
}
