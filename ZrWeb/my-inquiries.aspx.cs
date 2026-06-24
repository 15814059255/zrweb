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
            int page = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
            {
                int.TryParse(Request.QueryString["page"], out page);
            }
            if (page < 1) page = 1;
            CurrentPage = page;
            
            int pageSize = 30;
            
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
            
            // 如果Cookie也没有，从数据库查询
            if (shopId == 0)
            {
                int userId = UserHelper.GetUserId();
                if (userId > 0)
                {
                    shopId = GetDefaultShopId(userId);
                    if (shopId > 0)
                    {
                        Session["ShopId"] = shopId;
                    }
                }
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("EqId", typeof(int));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Brand", typeof(string));
            dt.Columns.Add("ParamsHtml", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("ExpectedPrice", typeof(string));
            dt.Columns.Add("ShopName", typeof(string));
            dt.Columns.Add("InquiryTime", typeof(string));
            dt.Columns.Add("Remarks", typeof(string));
            dt.Columns.Add("RemarksDisplay", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("StatusClass", typeof(string));

            if (shopId > 0)
            {
                // 先查总数
                string countSql = @"SELECT COUNT(*) 
                    FROM enquiryquoteprice e
                    WHERE e.fromShopID = @shopId AND e.eqType = 1 AND e.dataFlag = 1 AND (e.fromDataFlag IS NULL OR e.fromDataFlag = 1)";
                
                int totalCount = 0;
                object countObj = DbHelper.ExecuteScalar(countSql, DbHelper.CreateParameter("@shopId", shopId));
                if (countObj != null && countObj != DBNull.Value)
                {
                    int.TryParse(countObj.ToString(), out totalCount);
                }
                
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                if (TotalPages < 1) TotalPages = 1;
                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                
                if (totalCount > 0)
                {
                    int offset = (CurrentPage - 1) * pageSize;
                    
                    string sql = @"SELECT 
                        e.eqId, e.goodsId, e.goodsSn, e.fromQuantity, e.toQuantity, e.toPrice, e.fromPrice,
                        e.isIncludingTax, e.fromRemarks, e.createTime, e.readStatus,
                        e.brandName, e.toShopId,
                        s.shopName, s.shopCompany,
                        (SELECT TOP 1 Manufacturers FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as Manufacturers,
                        (SELECT TOP 1 Packaging FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as Packaging,
                        (SELECT TOP 1 Lot FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as Lot,
                        (SELECT TOP 1 Name FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as Name
                        FROM enquiryquoteprice e
                        LEFT JOIN shops s ON e.toShopId = s.shopId
                        WHERE e.fromShopID = @shopId AND e.eqType = 1 AND e.dataFlag = 1 AND (e.fromDataFlag IS NULL OR e.fromDataFlag = 1)
                        ORDER BY e.createTime DESC
                        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                    DataTable sourceDt = DbHelper.ExecuteQuery(sql, 
                        DbHelper.CreateParameter("@shopId", shopId),
                        DbHelper.CreateParameter("@offset", offset),
                        DbHelper.CreateParameter("@pageSize", pageSize));

                    if (sourceDt != null && sourceDt.Rows.Count > 0)
                    {
                        foreach (DataRow row in sourceDt.Rows)
                        {
                            DataRow newRow = dt.NewRow();
                            
                            newRow["EqId"] = GetIntValue(row["eqId"], 0);
                            newRow["Model"] = GetStringValue(row["goodsSn"]);
                            
                            // 获取品牌和参数
                            string brand = GetStringValue(row["brandName"]);
                            string manufacturers = GetStringValue(row["Manufacturers"]);
                            string packaging = GetStringValue(row["Packaging"]);
                            string lot = GetStringValue(row["Lot"]);
                            
                            string finalBrand = "";
                            System.Text.StringBuilder paramsBuilder = new System.Text.StringBuilder();
                            
                            // 解析 Manufacturers 字段（格式: 品牌 · 参数1 · 参数2 · ...）
                            if (!string.IsNullOrEmpty(manufacturers))
                            {
                                string[] parts = manufacturers.Split(new string[] { " · ", "·", " / ", "/" }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length > 0)
                                {
                                    finalBrand = parts[0].Trim();
                                    for (int i = 1; i < parts.Length; i++)
                                    {
                                        if (paramsBuilder.Length > 0)
                                            paramsBuilder.Append(" ");
                                        paramsBuilder.Append("<span class=\"param-tag\">");
                                        paramsBuilder.Append(System.Web.HttpUtility.HtmlEncode(parts[i].Trim()));
                                        paramsBuilder.Append("</span>");
                                    }
                                }
                            }
                            
                            // 如果没有解析出品牌，使用 brandName
                            if (string.IsNullOrEmpty(finalBrand))
                            {
                                finalBrand = !string.IsNullOrEmpty(brand) ? brand : "品牌不限";
                            }
                            
                            // 如果没有参数，用封装和批号补充
                            if (paramsBuilder.Length == 0)
                            {
                                if (!string.IsNullOrEmpty(packaging))
                                {
                                    paramsBuilder.Append("<span class=\"param-tag\">");
                                    paramsBuilder.Append("封装: ");
                                    paramsBuilder.Append(System.Web.HttpUtility.HtmlEncode(packaging));
                                    paramsBuilder.Append("</span>");
                                }
                                if (!string.IsNullOrEmpty(lot))
                                {
                                    if (paramsBuilder.Length > 0)
                                        paramsBuilder.Append(" ");
                                    paramsBuilder.Append("<span class=\"param-tag\">");
                                    paramsBuilder.Append("批号: ");
                                    paramsBuilder.Append(System.Web.HttpUtility.HtmlEncode(lot));
                                    paramsBuilder.Append("</span>");
                                }
                            }
                            
                            newRow["Brand"] = finalBrand;
                            newRow["ParamsHtml"] = paramsBuilder.ToString();

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
                            
                            // 备注显示HTML
                            string remarks = GetStringValue(row["fromRemarks"]);
                            if (!string.IsNullOrEmpty(remarks))
                            {
                                newRow["RemarksDisplay"] = "<div class=\"inquiry-remarks\"><span class=\"label\">备注</span><span>" + System.Web.HttpUtility.HtmlEncode(remarks) + "</span></div>";
                            }
                            else
                            {
                                newRow["RemarksDisplay"] = "";
                            }

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
                    }
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

    private int GetDefaultShopId(int userId)
    {
        try
        {
            string sql = "SELECT TOP 1 shopId FROM shops WHERE userId = @userId AND dataFlag = 1";
            object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@userId", userId));
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
        }
        catch
        {
        }
        return 0;
    }
}
