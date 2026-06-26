using System;
using System.Data;

public partial class received_quotes : System.Web.UI.Page
    {
        public string PageTitle = "收到报价 - 电子元器件 B2B 平台";
        public string PageKeywords = "阻容网,收到报价,报价管理,采购商";
        public string PageDescription = "查看供应商发来的报价信息，选择合适的供应商进行采购。";
        
        public int CurrentPage = 1;
        public int TotalPages = 1;
        public int TotalCount = 0;
        public bool HasQuoteData = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindQuotes();
            }
        }

        private void BindQuotes()
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
                
                int pageSize = 15;
                
                string filter = Request.QueryString["filter"] ?? "";
                int readStatusFilter = -1;
                if (filter == "unread")
                {
                    readStatusFilter = 0;
                }
                else if (filter == "read")
                {
                    readStatusFilter = 1;
                }
                
                string keyword = Request.QueryString["keyword"] ?? "";
                ViewState["Keyword"] = keyword;
                
                int userId = UserHelper.GetUserId();
                
                int shopId = 0;
                if (Session["ShopId"] != null)
                {
                    int.TryParse(Session["ShopId"].ToString(), out shopId);
                }
                
                if (shopId == 0 && userId > 0)
                {
                    string shopSql = "SELECT shopId FROM shops WHERE userId = @userId AND dataFlag = 1";
                    DataTable shopDt = DbHelper.ExecuteQuery(shopSql, DbHelper.CreateParameter("@userId", userId));
                    if (shopDt != null && shopDt.Rows.Count > 0)
                    {
                        shopId = GetIntValue(shopDt.Rows[0]["shopId"], 0);
                        Session["ShopId"] = shopId;
                    }
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("EqId", typeof(int));
                dt.Columns.Add("GoodsId", typeof(int));
                dt.Columns.Add("Model", typeof(string));
                dt.Columns.Add("Brand", typeof(string));
                dt.Columns.Add("ParamsHtml", typeof(string));
                dt.Columns.Add("Quantity", typeof(string));
                dt.Columns.Add("Unit", typeof(string));
                dt.Columns.Add("PriceDisplay", typeof(string));
                dt.Columns.Add("PriceNum", typeof(decimal));
                dt.Columns.Add("IsTax", typeof(bool));
                dt.Columns.Add("SellerName", typeof(string));
                dt.Columns.Add("SellerContact", typeof(string));
                dt.Columns.Add("QuoteTime", typeof(string));
                dt.Columns.Add("Validity", typeof(string));
                dt.Columns.Add("Remarks", typeof(string));
                dt.Columns.Add("RemarksHtml", typeof(string));
                dt.Columns.Add("PriceAreaHtml", typeof(string));
                dt.Columns.Add("SellerQQ", typeof(string));
                dt.Columns.Add("IsNew", typeof(bool));
                dt.Columns.Add("SellerInfoHiddenClass", typeof(string));
                dt.Columns.Add("Batch", typeof(string));
                dt.Columns.Add("IsViewed", typeof(bool));

                if (shopId > 0)
                {
                    // 先查总数
                    string countSql = @"SELECT COUNT(*) 
                        FROM enquiryquoteprice e
                        WHERE e.toShopId = @shopId AND e.eqType = 2 AND e.dataFlag = 1 AND (e.toDataFlag IS NULL OR e.toDataFlag = 1)";
                    
                    System.Collections.Generic.List<System.Data.SqlClient.SqlParameter> countParams = new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>();
                    countParams.Add(DbHelper.CreateParameter("@shopId", shopId));
                    
                    if (readStatusFilter >= 0)
                    {
                        countSql += " AND e.readStatus = @readStatus";
                        countParams.Add(DbHelper.CreateParameter("@readStatus", readStatusFilter));
                    }
                    
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        string[] searchFields = new string[] { "e.goodsSn", "e.brandName", "e.fromCompany" };
                        var searchCond = GoodsService.BuildParametricSearchCondition(keyword, searchFields, "ckw");
                        countSql += searchCond.Item1;
                        countParams.AddRange(searchCond.Item2);
                    }
                    
                    object countObj = DbHelper.ExecuteScalar(countSql, countParams.ToArray());
                    if (countObj != null && countObj != DBNull.Value)
                    {
                        int.TryParse(countObj.ToString(), out TotalCount);
                    }
                    
                    TotalPages = (int)Math.Ceiling((double)TotalCount / pageSize);
                    if (TotalPages < 1) TotalPages = 1;
                    if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                    
                    if (TotalCount > 0)
                    {
                        int offset = (CurrentPage - 1) * pageSize;
                        
                        string sql = @"SELECT 
                            e.eqId, e.goodsId, e.goodsSn, e.fromQuantity, e.toQuantity, e.fromPrice,
                            e.isIncludingTax, e.fromRemarks, e.createTime, e.readStatus, e.validity,
                            e.fromCompany, e.fromContact, e.fromTel, e.brandName, e.fromLot,
                            (SELECT TOP 1 Manufacturers FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as Manufacturers,
                            (SELECT TOP 1 Packaging FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as Packaging,
                            (SELECT TOP 1 Lot FROM goods WHERE (goodsId = e.goodsId OR (e.goodsId = 0 AND goodsSn = e.goodsSn)) AND dataFlag = 1 ORDER BY createTime DESC) as GoodsLot,
                            (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.fromShopId AND dataFlag = 1) as SellerQQ
                            FROM enquiryquoteprice e
                            WHERE e.toShopId = @shopId AND e.eqType = 2 AND e.dataFlag = 1 AND (e.toDataFlag IS NULL OR e.toDataFlag = 1)";
                    
                        System.Collections.Generic.List<System.Data.SqlClient.SqlParameter> sqlParams = new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>();
                        sqlParams.Add(DbHelper.CreateParameter("@shopId", shopId));
                        
                        if (readStatusFilter >= 0)
                        {
                            sql += " AND e.readStatus = @readStatus";
                            sqlParams.Add(DbHelper.CreateParameter("@readStatus", readStatusFilter));
                        }
                        
                        if (!string.IsNullOrEmpty(keyword))
                        {
                            string[] searchFields = new string[] { "e.goodsSn", "e.brandName", "e.fromCompany" };
                            var searchCond = GoodsService.BuildParametricSearchCondition(keyword, searchFields, "qkw");
                            sql += searchCond.Item1;
                            sqlParams.AddRange(searchCond.Item2);
                        }
                        
                        sql += " ORDER BY e.createTime DESC OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
                        
                        sqlParams.Add(DbHelper.CreateParameter("@offset", offset));
                        sqlParams.Add(DbHelper.CreateParameter("@pageSize", pageSize));
                        
                        DataTable sourceDt = DbHelper.ExecuteQuery(sql, sqlParams.ToArray());

                        if (sourceDt != null && sourceDt.Rows.Count > 0)
                        {
                            foreach (DataRow row in sourceDt.Rows)
                            {
                                DataRow newRow = dt.NewRow();
                                
                                int eqId = GetIntValue(row["eqId"], 0);
                                newRow["EqId"] = eqId;
                                newRow["GoodsId"] = GetIntValue(row["goodsId"], 0);
                                newRow["Model"] = GetStringValue(row["goodsSn"]);
                                newRow["SellerName"] = DbHelper.FixAndCleanString(GetStringValue(row["fromCompany"]));
                                newRow["SellerContact"] = DbHelper.FixAndCleanString(GetStringValue(row["fromContact"]));
                                newRow["SellerQQ"] = GetStringValue(row["SellerQQ"]);
                                newRow["QuoteTime"] = Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm");
                                newRow["Remarks"] = GetStringValue(row["fromRemarks"]);

                                // 查看状态
                                int readStatus = GetIntValue(row["readStatus"], 0);
                                bool isViewed = readStatus == 1;
                                
                                newRow["SellerInfoHiddenClass"] = isViewed ? "" : "seller-info-hidden";
                                newRow["IsViewed"] = isViewed;
                                
                                // 是否新报价（24小时内且未查看）
                                TimeSpan timeDiff = DateTime.Now - Convert.ToDateTime(row["createTime"]);
                                newRow["IsNew"] = !isViewed && timeDiff.TotalHours < 24;
                                
                                // 批次
                                string fromLot = GetStringValue(row["fromLot"]);
                                string goodsLot = GetStringValue(row["GoodsLot"]);
                                newRow["Batch"] = !string.IsNullOrEmpty(fromLot) ? fromLot : (!string.IsNullOrEmpty(goodsLot) ? goodsLot : "-");
                                
                                // 备注显示HTML
                                string remarks = GetStringValue(row["fromRemarks"]);
                                if (!string.IsNullOrEmpty(remarks))
                                {
                                    newRow["RemarksHtml"] = "<div class=\"quote-remarks\"><b>备注</b>：" + System.Web.HttpUtility.HtmlEncode(remarks) + "</div>";
                                }
                                else
                                {
                                    newRow["RemarksHtml"] = "";
                                }

                                // 品牌和参数
                                string brand = GetStringValue(row["brandName"]);
                                string manufacturers = GetStringValue(row["Manufacturers"]);
                                string packaging = GetStringValue(row["Packaging"]);
                                string displayLot = !string.IsNullOrEmpty(fromLot) ? fromLot : goodsLot;
                                
                                string finalBrand = "";
                                System.Text.StringBuilder paramsBuilder = new System.Text.StringBuilder();
                                
                                if (!string.IsNullOrEmpty(manufacturers))
                                {
                                    string[] parts = manufacturers.Split(new string[] { " · ", "·", " / ", "/" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length > 0)
                                    {
                                        finalBrand = parts[0].Trim();
                                        for (int i = 1; i < parts.Length; i++)
                                        {
                                            paramsBuilder.Append("<span class=\"param-chip\">");
                                            paramsBuilder.Append(System.Web.HttpUtility.HtmlEncode(parts[i].Trim()));
                                            paramsBuilder.Append("</span>");
                                        }
                                    }
                                }
                                
                                if (string.IsNullOrEmpty(finalBrand))
                                {
                                    finalBrand = !string.IsNullOrEmpty(brand) ? brand : "品牌不限";
                                }
                                
                                if (paramsBuilder.Length == 0)
                                {
                                    if (!string.IsNullOrEmpty(packaging))
                                    {
                                        paramsBuilder.Append("<span class=\"param-chip\">封装: ");
                                        paramsBuilder.Append(System.Web.HttpUtility.HtmlEncode(packaging));
                                        paramsBuilder.Append("</span>");
                                    }
                                }
                                
                                if (!string.IsNullOrEmpty(displayLot))
                                {
                                    paramsBuilder.Append("<span class=\"param-chip\">批次: ");
                                    paramsBuilder.Append(System.Web.HttpUtility.HtmlEncode(displayLot));
                                    paramsBuilder.Append("</span>");
                                }
                                
                                newRow["Brand"] = finalBrand;
                                newRow["ParamsHtml"] = paramsBuilder.ToString();

                                // 数量
                                int fromQty = GetIntValue(row["fromQuantity"], 0);
                                int toQty = GetIntValue(row["toQuantity"], 0);
                                int qty = toQty > 0 ? toQty : fromQty;
                                newRow["Quantity"] = qty > 0 ? qty.ToString() : "0";
                                newRow["Unit"] = "Kpcs";

                                // 价格显示
                                decimal price = GetDecimalValue(row["fromPrice"], 0);
                                bool isTax = GetIntValue(row["isIncludingTax"], 0) == 1;
                                newRow["PriceNum"] = price;
                                newRow["IsTax"] = isTax;
                                
                                string taxClass = isTax ? "tax" : "notax";
                                string taxText = isTax ? "含税" : "未税";
                                string priceText = price > 0 
                                    ? "¥" + price.ToString("0.0000") + "<span class=\"tax-label " + taxClass + "\">" + taxText + "</span>"
                                    : "面议";
                                newRow["PriceDisplay"] = priceText;

                                // 价格区域HTML（根据是否已查看）
                                if (isViewed)
                                {
                                    newRow["PriceAreaHtml"] = "<div class=\"quote-price\">" + priceText + "</div>";
                                }
                                else
                                {
                                    string encodedPrice = System.Web.HttpUtility.HtmlEncode(priceText);
                                    newRow["PriceAreaHtml"] = "<div class=\"price-hidden\">价格待查看</div>" +
                                        "<button class=\"view-price-btn\" data-view-price data-eq-id=\"" + eqId + "\" data-price=\"" + encodedPrice + "\">" +
                                        "<span>👁</span> 查看报价</button>";
                                }

                                // 有效期
                                string validity = GetStringValue(row["validity"]);
                                if (!string.IsNullOrEmpty(validity))
                                {
                                    if (validity == "24小时") validity = "1天";
                                    else if (validity == "1个月") validity = "30天";
                                    newRow["Validity"] = validity;
                                }
                                else
                                {
                                    DateTime createTime = Convert.ToDateTime(row["createTime"]);
                                    TimeSpan diff = DateTime.Now - createTime;
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
                                }

                                dt.Rows.Add(newRow);
                            }
                        }
                    }
                }

                HasQuoteData = dt != null && dt.Rows.Count > 0;
                rptQuotes.DataSource = dt;
                rptQuotes.DataBind();
            }
            catch (Exception ex)
            {
                HasQuoteData = false;
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
