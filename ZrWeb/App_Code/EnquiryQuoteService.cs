using System;
using System.Data;

/// <summary>
/// 询报价服务类
/// </summary>
public class EnquiryQuoteService
{
    /// <summary>
    /// 获取询报价列表
    /// </summary>
    public DataTable GetEnquiryQuoteList(int topCount)
    {
        try
        {
            string sql = @"SELECT TOP " + topCount + @" 
                eqId, goodsSn, eqType, fromQuantity, fromPrice, isIncludingTax, 
                createTime, fromCompany, brandName, fromRemarks
                FROM enquiryquoteprice 
                WHERE dataFlag = 1 
                ORDER BY createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql);
            
            // 如果有数据，转换为展示格式
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToDisplayData(dt);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取报价记录列表（eqType=2）
    /// </summary>
    public DataTable GetQuoteRecords(int eqType)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                eqId, goodsSn, eqType, fromQuantity, fromPrice, isIncludingTax, 
                createTime, fromCompany, brandName, toCompany, readStatus, fromRemarks
                FROM enquiryquoteprice 
                WHERE dataFlag = 1 AND eqType = @eqType
                ORDER BY createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@eqType", eqType));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToQuoteRecordData(dt);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 转换报价记录数据为展示格式
    /// </summary>
    private DataTable ConvertToQuoteRecordData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("EqId", typeof(int));
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
        
        dt.Columns.Add("SellerQQ", typeof(string));
        dt.Columns.Add("BuyerQQ", typeof(string));
        dt.Columns.Add("BuyerName", typeof(string));
        dt.Columns.Add("QuoteTime", typeof(string));
        dt.Columns.Add("Validity", typeof(string));
        dt.Columns.Add("FromShopId", typeof(int));
        dt.Columns.Add("ToShopId", typeof(int));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int eqId = GetIntValue(row["eqId"], 0);
            int readStatus = GetIntValue(row["readStatus"], 0);
            string goodsSn = GetStringValue(row["goodsSn"]);
            string brandName = row.Table.Columns.Contains("SellerBrandName") ? GetStringValue(row["SellerBrandName"]) : GetStringValue(row["brandName"]);
            int fromQuantity = GetIntValue(row["fromQuantity"], 0);
            decimal fromPrice = GetDecimalValue(row["fromPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
            string toCompany = GetStringValue(row["toCompany"]);
            string fromRemarks = GetStringValue(row["fromRemarks"]);

            newRow["EqId"] = eqId;

            int fromShopIdVal = GetIntValue(row["fromShopId"], 0);
            int toShopIdVal = GetIntValue(row["toShopId"], 0);
            newRow["FromShopId"] = fromShopIdVal;
            newRow["ToShopId"] = toShopIdVal;

            // 设置状态
            if (readStatus == 1)
            {
                newRow["Status"] = "已查看";
                newRow["StatusClass"] = "blue";
            }
            else
            {
                newRow["Status"] = "未查看";
                newRow["StatusClass"] = "green";
            }

            // 设置型号
            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else
            {
                newRow["Model"] = fromRemarks.Length > 20 ? fromRemarks.Substring(0, 20) + "..." : fromRemarks;
            }

            // 设置品牌参数 - 优先使用商品表的完整参数（包含品牌、封装、容值等）
            string manufacturers = row.Table.Columns.Contains("Manufacturers") ? GetStringValue(row["Manufacturers"]) : "";
            if (!string.IsNullOrEmpty(manufacturers) && manufacturers.Contains("·"))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                // 其次使用对方询价时提供的品牌参数
                string buyerBrandName = row.Table.Columns.Contains("BuyerBrandName") ? GetStringValue(row["BuyerBrandName"]) : "";
                if (!string.IsNullOrEmpty(buyerBrandName) && buyerBrandName.Contains("·"))
                {
                    newRow["BrandParams"] = buyerBrandName;
                }
                else
                {
                    // 从商品表获取完整参数
                    string packaging = row.Table.Columns.Contains("Packaging") ? GetStringValue(row["Packaging"]) : "";
                    string goodsDesc = row.Table.Columns.Contains("goodsDesc") ? GetStringValue(row["goodsDesc"]) : "";
                    
                    System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                    
                    if (!string.IsNullOrEmpty(manufacturers)) paramList.Add(manufacturers);
                    if (!string.IsNullOrEmpty(packaging)) paramList.Add(packaging);
                    
                    if (!string.IsNullOrEmpty(goodsDesc))
                    {
                        string[] descParts = goodsDesc.Split(new[] { '·', '|', '/', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in descParts)
                        {
                            string trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !paramList.Contains(trimmed))
                            {
                                paramList.Add(trimmed);
                            }
                        }
                    }
                    
                    if (paramList.Count > 0)
                    {
                        newRow["BrandParams"] = string.Join(" · ", paramList);
                    }
                    else if (!string.IsNullOrEmpty(buyerBrandName))
                    {
                        newRow["BrandParams"] = buyerBrandName;
                    }
                    else if (!string.IsNullOrEmpty(brandName))
                    {
                        newRow["BrandParams"] = brandName;
                    }
                    else
                    {
                        newRow["BrandParams"] = "品牌不限";
                    }
                }
            }

            // 设置询价方信息
            int toQuantity = GetIntValue(row["toQuantity"], 0);
            decimal toPrice = GetDecimalValue(row["toPrice"], 0);
            string toRemarks = GetStringValue(row["toRemarks"]);
            
            // 优先使用 JOIN 得到的真实询价数据
            if (row.Table.Columns.Contains("RealInquiryQuantity"))
            {
                int realQty = GetIntValue(row["RealInquiryQuantity"], 0);
                if (realQty > 0) toQuantity = realQty;
            }
            if (row.Table.Columns.Contains("RealInquiryPrice"))
            {
                decimal realPrice = GetDecimalValue(row["RealInquiryPrice"], 0);
                if (realPrice > 0) toPrice = realPrice;
            }
            if (row.Table.Columns.Contains("RealInquiryRemarks"))
            {
                string realRemarks = GetStringValue(row["RealInquiryRemarks"]);
                if (!string.IsNullOrEmpty(realRemarks)) toRemarks = realRemarks;
            }
            
            newRow["InquiryQuantity"] = toQuantity > 0 ? toQuantity.ToString() : "0";
            newRow["InquiryUnit"] = "Kpcs";
            newRow["InquiryPrice"] = toPrice > 0 ? "¥" + toPrice.ToString("0.00") : "面议";
            newRow["InquiryRemarks"] = !string.IsNullOrEmpty(toRemarks) ? toRemarks : "-";

            // 设置我的报价信息
            if (fromQuantity > 0)
            {
                newRow["MyQuantity"] = fromQuantity.ToString();
                newRow["MyUnit"] = "Kpcs";
            }
            else
            {
                newRow["MyQuantity"] = "0";
                newRow["MyUnit"] = "Kpcs";
            }

            if (fromPrice > 0)
            {
                string taxText = isIncludingTax == 1 ? "含税" : "未税";
                newRow["MyPrice"] = "¥" + fromPrice.ToString("0.00") + " (" + taxText + ")";
            }
            else
            {
                newRow["MyPrice"] = "面议";
            }

            string batch = GetStringValue(row["fromLot"]);
            if (string.IsNullOrEmpty(batch))
            {
                batch = GetStringValue(row["Lot"]);
            }
            if (string.IsNullOrEmpty(batch))
            {
                batch = GetStringValue(row["GoodsLot"]);
            }
            newRow["MyBatch"] = !string.IsNullOrEmpty(batch) ? batch : "-";
            newRow["MyRemarks"] = !string.IsNullOrEmpty(fromRemarks) ? fromRemarks : "-";

            // 兼容旧字段名
            newRow["Quantity"] = newRow["MyQuantity"];
            newRow["Unit"] = newRow["MyUnit"];
            newRow["Price"] = newRow["MyPrice"];
            newRow["Batch"] = newRow["MyBatch"];
            newRow["Remarks"] = newRow["MyRemarks"];

            // 设置供应商QQ
            string sellerQQ = GetStringValue(row["SellerQQ"]);
            newRow["SellerQQ"] = sellerQQ;

            // 设置采购商QQ
            string buyerQQ = GetStringValue(row["BuyerQQ"]);
            newRow["BuyerQQ"] = buyerQQ;

            // 设置采购商
            newRow["BuyerName"] = !string.IsNullOrEmpty(toCompany) ? toCompany : "匿名采购商";

            // 设置报价时间
            newRow["QuoteTime"] = createTime.ToString("yyyy-MM-dd HH:mm");

            // 设置有效期 - 优先使用报价时选择的有效期
            string quoteValidity = GetStringValue(row["validity"]);
            if (!string.IsNullOrEmpty(quoteValidity))
            {
                newRow["Validity"] = quoteValidity;
            }
            else
            {
                TimeSpan diff = DateTime.Now - createTime;
                if (diff.TotalHours < 24)
                {
                    newRow["Validity"] = "24 小时";
                }
                else if (diff.TotalDays < 2)
                {
                    newRow["Validity"] = "48 小时";
                }
                else if (diff.TotalDays < 3)
                {
                    newRow["Validity"] = "72 小时";
                }
                else
                {
                    newRow["Validity"] = createTime.AddDays(3).ToString("yyyy-MM-dd");
                }
            }

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    /// <summary>
    /// 安全版本的报价记录转换（处理字段缺失情况）
    /// </summary>
    private DataTable ConvertToQuoteRecordDataSafe(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("EqId", typeof(int));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("StatusClass", typeof(string));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("Batch", typeof(string));
        dt.Columns.Add("Remarks", typeof(string));
        dt.Columns.Add("SellerQQ", typeof(string));
        dt.Columns.Add("BuyerQQ", typeof(string));
        dt.Columns.Add("BuyerName", typeof(string));
        dt.Columns.Add("QuoteTime", typeof(string));
        dt.Columns.Add("Validity", typeof(string));
        dt.Columns.Add("InquiryQuantity", typeof(string));
        dt.Columns.Add("InquiryUnit", typeof(string));
        dt.Columns.Add("InquiryPrice", typeof(string));
        dt.Columns.Add("InquiryRemarks", typeof(string));
        dt.Columns.Add("MyQuantity", typeof(string));
        dt.Columns.Add("MyUnit", typeof(string));
        dt.Columns.Add("MyPrice", typeof(string));
        dt.Columns.Add("MyBatch", typeof(string));
        dt.Columns.Add("MyRemarks", typeof(string));
        dt.Columns.Add("FromShopId", typeof(int));
        dt.Columns.Add("ToShopId", typeof(int));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            try { newRow["EqId"] = GetIntValue(row["eqId"], 0); } catch { newRow["EqId"] = 0; }
            
            int fromShopIdVal = 0;
            try { fromShopIdVal = GetIntValue(row["fromShopId"], 0); } catch { }
            newRow["FromShopId"] = fromShopIdVal;
            
            int toShopIdVal = 0;
            try { toShopIdVal = GetIntValue(row["toShopId"], 0); } catch { }
            newRow["ToShopId"] = toShopIdVal;
            
            int readStatus = 0;
            try { readStatus = GetIntValue(row["readStatus"], 0); } catch { readStatus = 0; }
            
            if (readStatus == 1)
            {
                newRow["Status"] = "已查看";
                newRow["StatusClass"] = "blue";
            }
            else
            {
                newRow["Status"] = "未查看";
                newRow["StatusClass"] = "green";
            }

            string goodsSn = "";
            try { goodsSn = GetStringValue(row["goodsSn"]); } catch { }
            string fromRemarks = "";
            try { fromRemarks = GetStringValue(row["fromRemarks"]); } catch { }
            
            string brandName = "";
            try { brandName = row.Table.Columns.Contains("SellerBrandName") ? GetStringValue(row["SellerBrandName"]) : GetStringValue(row["brandName"]); } catch { }
            
            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else
            {
                newRow["Model"] = fromRemarks.Length > 20 ? fromRemarks.Substring(0, 20) + "..." : fromRemarks;
            }

            string manufacturers = "";
            try { manufacturers = GetStringValue(row["Manufacturers"]); } catch { }
            
            if (!string.IsNullOrEmpty(manufacturers) && manufacturers.Contains("·"))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                string buyerBrandName = "";
                try { buyerBrandName = GetStringValue(row["BuyerBrandName"]); } catch { }
                
                if (!string.IsNullOrEmpty(buyerBrandName) && buyerBrandName.Contains("·"))
                {
                    newRow["BrandParams"] = buyerBrandName;
                }
                else
                {
                    string packaging = "";
                    try { packaging = GetStringValue(row["Packaging"]); } catch { }
                    string goodsDesc = "";
                    try { goodsDesc = GetStringValue(row["goodsDesc"]); } catch { }
                    
                    System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                    
                    if (!string.IsNullOrEmpty(manufacturers)) paramList.Add(manufacturers);
                    if (!string.IsNullOrEmpty(packaging)) paramList.Add(packaging);
                    
                    if (!string.IsNullOrEmpty(goodsDesc))
                    {
                        string[] descParts = goodsDesc.Split(new[] { '·', '|', '/', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in descParts)
                        {
                            string trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !paramList.Contains(trimmed))
                            {
                                paramList.Add(trimmed);
                            }
                        }
                    }
                    
                    if (paramList.Count > 0)
                    {
                        newRow["BrandParams"] = string.Join(" · ", paramList);
                    }
                    else if (!string.IsNullOrEmpty(buyerBrandName))
                    {
                        newRow["BrandParams"] = buyerBrandName;
                    }
                    else if (!string.IsNullOrEmpty(brandName))
                    {
                        newRow["BrandParams"] = brandName;
                    }
                    else
                    {
                        newRow["BrandParams"] = "品牌不限";
                    }
                }
            }

            int fromQuantity = 0;
            try { fromQuantity = GetIntValue(row["fromQuantity"], 0); } catch { }
            newRow["Quantity"] = fromQuantity > 0 ? fromQuantity.ToString() : "0";
            newRow["Unit"] = "Kpcs";
            newRow["MyQuantity"] = fromQuantity > 0 ? fromQuantity.ToString() : "0";
            newRow["MyUnit"] = "Kpcs";

            decimal fromPrice = 0;
            try { fromPrice = GetDecimalValue(row["fromPrice"], 0); } catch { }
            int isIncludingTax = 0;
            try { isIncludingTax = GetIntValue(row["isIncludingTax"], 0); } catch { }
            
            if (fromPrice > 0)
            {
                string taxText = isIncludingTax == 1 ? "含税" : "未税";
                newRow["Price"] = "¥" + fromPrice.ToString("0.00") + " (" + taxText + ")";
                newRow["MyPrice"] = "¥" + fromPrice.ToString("0.00") + " (" + taxText + ")";
            }
            else
            {
                newRow["Price"] = "面议";
                newRow["MyPrice"] = "面议";
            }

            // 询价方信息 - 优先使用 RealInquiryQuantity
            int toQty = 0;
            try { toQty = GetIntValue(row["toQuantity"], 0); } catch { }
            if (toQty <= 0)
            {
                try { toQty = GetIntValue(row["RealInquiryQuantity"], 0); } catch { }
            }
            newRow["InquiryQuantity"] = toQty > 0 ? toQty.ToString() : "0";
            newRow["InquiryUnit"] = "Kpcs";

            decimal toPrice = 0;
            try { toPrice = GetDecimalValue(row["toPrice"], 0); } catch { }
            if (toPrice <= 0)
            {
                try { toPrice = GetDecimalValue(row["RealInquiryPrice"], 0); } catch { }
            }
            newRow["InquiryPrice"] = toPrice > 0 ? "¥" + toPrice.ToString("0.00") : "面议";

            string toRemarks = "";
            try { toRemarks = GetStringValue(row["toRemarks"]); } catch { }
            if (string.IsNullOrEmpty(toRemarks))
            {
                try { toRemarks = GetStringValue(row["RealInquiryRemarks"]); } catch { }
            }
            newRow["InquiryRemarks"] = !string.IsNullOrEmpty(toRemarks) ? toRemarks : "-";

            string batch = "";
            try { batch = GetStringValue(row["fromLot"]); } catch { }
            if (string.IsNullOrEmpty(batch)) try { batch = GetStringValue(row["Lot"]); } catch { }
            if (string.IsNullOrEmpty(batch)) try { batch = GetStringValue(row["GoodsLot"]); } catch { }
            newRow["Batch"] = !string.IsNullOrEmpty(batch) ? batch : "-";
            newRow["MyBatch"] = !string.IsNullOrEmpty(batch) ? batch : "-";

            newRow["Remarks"] = !string.IsNullOrEmpty(fromRemarks) ? fromRemarks : "-";
            newRow["MyRemarks"] = !string.IsNullOrEmpty(fromRemarks) ? fromRemarks : "-";

            string sellerQQ = "";
            try { sellerQQ = GetStringValue(row["SellerQQ"]); } catch { }
            newRow["SellerQQ"] = sellerQQ;

            string buyerQQ = "";
            try { buyerQQ = GetStringValue(row["BuyerQQ"]); } catch { }
            newRow["BuyerQQ"] = buyerQQ;

            string toCompany = "";
            try { toCompany = GetStringValue(row["toCompany"]); } catch { }
            newRow["BuyerName"] = !string.IsNullOrEmpty(toCompany) ? toCompany : "匿名采购商";

            DateTime createTime = DateTime.Now;
            try { createTime = GetDateTimeValue(row["createTime"], DateTime.Now); } catch { }
            newRow["QuoteTime"] = createTime.ToString("yyyy-MM-dd HH:mm");

            string quoteValidity = "";
            try { quoteValidity = GetStringValue(row["validity"]); } catch { }
            if (!string.IsNullOrEmpty(quoteValidity))
            {
                newRow["Validity"] = quoteValidity;
            }
            else
            {
                TimeSpan diff = DateTime.Now - createTime;
                if (diff.TotalHours < 24)
                {
                    newRow["Validity"] = "24 小时";
                }
                else if (diff.TotalDays < 2)
                {
                    newRow["Validity"] = "48 小时";
                }
                else if (diff.TotalDays < 3)
                {
                    newRow["Validity"] = "72 小时";
                }
                else
                {
                    newRow["Validity"] = createTime.AddDays(3).ToString("yyyy-MM-dd");
                }
            }

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    /// <summary>
    /// 根据ID获取询报价详情
    /// </summary>
    public DataRow GetEnquiryQuoteById(int eqId)
    {
        try
        {
            string sql = @"SELECT * FROM enquiryquoteprice WHERE eqId = @eqId AND dataFlag = 1";
            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@eqId", eqId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 确保 enquiryquoteprice 表包含必要字段
    /// </summary>
    private void EnsureQuoteTableColumns()
    {
        try
        {
            string[] columns = {
                "fromManufacturers NVARCHAR(500) NULL",
                "fromPackaging NVARCHAR(500) NULL",
                "fromGoodsDesc NVARCHAR(500) NULL",
                "validity NVARCHAR(50) NULL",
                "toQuantity INT DEFAULT 0",
                "toPrice DECIMAL(18,4) DEFAULT 0",
                "toRemarks NVARCHAR(500) NULL",
                "sourceEqId INT DEFAULT 0"
            };

            foreach (string colDef in columns)
            {
                string[] parts = colDef.Split(new[] { ' ' }, 2);
                string colName = parts[0];
                string colType = parts[1];

                string checkSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('enquiryquoteprice') AND name = @colName";
                object result = DbHelper.ExecuteScalar(checkSql, DbHelper.CreateParameter("@colName", colName));
                if (result == null || Convert.ToInt32(result) == 0)
                {
                    string addSql = "ALTER TABLE enquiryquoteprice ADD " + colDef;
                    DbHelper.ExecuteNonQuery(addSql);
                }
            }
        }
        catch
        {
            // 字段可能已存在或其他错误，忽略
        }
    }

    /// <summary>
    /// 转换数据为展示格式
    /// </summary>
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

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int eqType = GetIntValue(row["eqType"], 1);
            string goodsSn = GetStringValue(row["goodsSn"]);
            string brandName = GetStringValue(row["brandName"]);
            int fromQuantity = GetIntValue(row["fromQuantity"], 0);
            decimal fromPrice = GetDecimalValue(row["fromPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
            string fromCompany = GetStringValue(row["fromCompany"]);
            string fromRemarks = GetStringValue(row["fromRemarks"]);

            // 设置类型
            if (eqType == 2)
            {
                // 报价 - 供应
                newRow["ItemType"] = "supply";
                newRow["TagClass"] = "blue";
                newRow["TypeLabel"] = "供应";
                newRow["ActionText"] = "立即询价";
            }
            else
            {
                // 询价 - 需求
                newRow["ItemType"] = "demand";
                newRow["TagClass"] = "green";
                newRow["TypeLabel"] = "需求";
                newRow["ActionText"] = "我要报价";
            }

            // 设置型号
            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else
            {
                newRow["Model"] = fromRemarks.Length > 30 ? fromRemarks.Substring(0, 30) + "..." : fromRemarks;
            }

            // 设置详情链接
            newRow["DetailUrl"] = "/supply-detail.aspx?id=" + GetIntValue(row["eqId"], 0);

            // 设置价格
            if (fromPrice > 0)
            {
                string taxText = isIncludingTax == 1 ? "含税" : "未税";
                newRow["PriceDisplay"] = "¥" + fromPrice.ToString("0.00") + " /Kpcs (" + taxText + ")";
            }
            else
            {
                newRow["PriceDisplay"] = "面议";
            }

            // 设置品牌参数 - 优先使用商品表的完整参数（包含品牌、封装、容值等）
            string manufacturers = row.Table.Columns.Contains("Manufacturers") ? GetStringValue(row["Manufacturers"]) : "";
            if (!string.IsNullOrEmpty(manufacturers) && manufacturers.Contains("·"))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                // 其次使用对方询价时提供的品牌参数
                string buyerBrandName = row.Table.Columns.Contains("BuyerBrandName") ? GetStringValue(row["BuyerBrandName"]) : "";
                if (!string.IsNullOrEmpty(buyerBrandName) && buyerBrandName.Contains("·"))
                {
                    newRow["BrandParams"] = buyerBrandName;
                }
                else
                {
                    // 从商品表获取完整参数
                    string packaging = row.Table.Columns.Contains("Packaging") ? GetStringValue(row["Packaging"]) : "";
                    string goodsDesc = row.Table.Columns.Contains("goodsDesc") ? GetStringValue(row["goodsDesc"]) : "";
                    
                    System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                    
                    if (!string.IsNullOrEmpty(manufacturers)) paramList.Add(manufacturers);
                    if (!string.IsNullOrEmpty(packaging)) paramList.Add(packaging);
                    
                    if (!string.IsNullOrEmpty(goodsDesc))
                    {
                        string[] descParts = goodsDesc.Split(new[] { '·', '|', '/', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in descParts)
                        {
                            string trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !paramList.Contains(trimmed))
                            {
                                paramList.Add(trimmed);
                            }
                        }
                    }
                    
                    if (paramList.Count > 0)
                    {
                        newRow["BrandParams"] = string.Join(" · ", paramList);
                    }
                    else if (!string.IsNullOrEmpty(buyerBrandName))
                    {
                        newRow["BrandParams"] = buyerBrandName;
                    }
                    else if (!string.IsNullOrEmpty(brandName))
                    {
                        newRow["BrandParams"] = brandName;
                    }
                    else
                    {
                        newRow["BrandParams"] = "品牌不限";
                    }
                }
            }

            // 设置数量
            if (fromQuantity > 0)
            {
                newRow["QuantityDisplay"] = "现货 " + fromQuantity.ToString() + "/Kpcs";
            }
            else
            {
                newRow["QuantityDisplay"] = "按需供应";
            }

            // 设置有效期（根据创建时间计算）
            TimeSpan diff = DateTime.Now - createTime;
            if (diff.TotalHours < 24)
            {
                newRow["Validity"] = "24 小时内";
            }
            else if (diff.TotalDays < 3)
            {
                newRow["Validity"] = "3 天内";
            }
            else if (diff.TotalDays < 7)
            {
                newRow["Validity"] = "7 天内";
            }
            else
            {
                newRow["Validity"] = createTime.ToString("yyyy-MM-dd");
            }

            // 设置公司名称
            newRow["CompanyName"] = !string.IsNullOrEmpty(fromCompany) ? fromCompany : "匿名公司";

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    /// <summary>
    /// 提交报价
    /// </summary>
    /// <param name="goodsId">商品ID</param>
    /// <param name="goodsSn">商品型号</param>
    /// <param name="fromQuantity">可供数量</param>
    /// <param name="fromPrice">报价单价</param>
    /// <param name="isIncludingTax">是否含税</param>
    /// <param name="fromCompany">报价公司</param>
    /// <param name="fromContact">联系人</param>
    /// <param name="fromTel">联系电话</param>
    /// <param name="fromRemarks">备注说明</param>
    /// <param name="toCompany">采购商公司</param>
    /// <param name="toUserId">采购商用户ID</param>
    /// <param name="fromUserId">报价方用户ID</param>
    /// <param name="brandName">品牌名称</param>
    /// <returns></returns>
    public bool SubmitQuote(int goodsId, string goodsSn, int fromQuantity, decimal fromPrice, 
        int isIncludingTax, string fromCompany, string fromContact, string fromTel, 
        string fromRemarks, string toCompany, int toUserId, int fromUserId, string brandName,
        int fromShopId, int toShopId, int eqType = 2, int sourceEqId = 0, string fromLot = "",
        string fromManufacturers = "", string fromPackaging = "", string fromGoodsDesc = "",
        string validity = "")
    {
        try
        {
            EnsureQuoteTableColumns();

            if (goodsId > 0)
            {
                string goodsSql = @"SELECT Manufacturers, Packaging, goodsDesc FROM goods WHERE goodsId = @goodsId AND dataFlag = 1";
                DataTable goodsDt = DbHelper.ExecuteQuery(goodsSql, DbHelper.CreateParameter("@goodsId", goodsId));
                if (goodsDt != null && goodsDt.Rows.Count > 0)
                {
                    DataRow goodsRow = goodsDt.Rows[0];
                    string manufacturers = GetStringValue(goodsRow["Manufacturers"]);
                    string packaging = GetStringValue(goodsRow["Packaging"]);
                    string goodsDesc = GetStringValue(goodsRow["goodsDesc"]);
                    
                    System.Collections.Generic.List<string> paramList = new System.Collections.Generic.List<string>();
                    if (!string.IsNullOrEmpty(manufacturers)) paramList.Add(manufacturers);
                    if (!string.IsNullOrEmpty(packaging)) paramList.Add(packaging);
                    
                    if (!string.IsNullOrEmpty(goodsDesc))
                    {
                        string[] descParts = goodsDesc.Split(new[] { '·', '|', '/', '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string part in descParts)
                        {
                            string trimmed = part.Trim();
                            if (!string.IsNullOrEmpty(trimmed) && !paramList.Contains(trimmed))
                            {
                                paramList.Add(trimmed);
                            }
                        }
                    }
                    
                    string fullParams = paramList.Count > 0 ? string.Join(" · ", paramList) : "";
                    
                    if (!string.IsNullOrEmpty(fullParams))
                    {
                        if (string.IsNullOrEmpty(brandName))
                        {
                            brandName = fullParams;
                        }
                        else if (!fullParams.Contains(brandName))
                        {
                            brandName = fullParams;
                        }
                    }
                }
            }

            int sourceQuantity = 0;
            decimal sourcePrice = 0;
            string sourceRemarks = "";
            if (sourceEqId > 0)
            {
                try
                {
                    string sourceSql = "SELECT fromQuantity, fromPrice, fromRemarks FROM enquiryquoteprice WHERE eqId = @eqId";
                    DataTable sourceDt = DbHelper.ExecuteQuery(sourceSql, DbHelper.CreateParameter("@eqId", sourceEqId));
                    if (sourceDt != null && sourceDt.Rows.Count > 0)
                    {
                        DataRow sourceRow = sourceDt.Rows[0];
                        sourceQuantity = GetIntValue(sourceRow["fromQuantity"], 0);
                        sourcePrice = GetDecimalValue(sourceRow["fromPrice"], 0);
                        sourceRemarks = GetStringValue(sourceRow["fromRemarks"]);
                    }
                }
                catch { }
            }
            else if (goodsSn != null && !string.IsNullOrEmpty(goodsSn) && toShopId > 0)
            {
                try
                {
                    string sourceSql = "SELECT TOP 1 fromQuantity, fromPrice, fromRemarks FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = @goodsSn AND fromShopId = @toShopId AND toShopId = @fromShopId ORDER BY createTime DESC";
                    DataTable sourceDt = DbHelper.ExecuteQuery(sourceSql, 
                        DbHelper.CreateParameter("@goodsSn", goodsSn),
                        DbHelper.CreateParameter("@toShopId", toShopId),
                        DbHelper.CreateParameter("@fromShopId", fromShopId));
                    if (sourceDt != null && sourceDt.Rows.Count > 0)
                    {
                        DataRow sourceRow = sourceDt.Rows[0];
                        sourceQuantity = GetIntValue(sourceRow["fromQuantity"], 0);
                        sourcePrice = GetDecimalValue(sourceRow["fromPrice"], 0);
                        sourceRemarks = GetStringValue(sourceRow["fromRemarks"]);
                    }
                }
                catch { }
            }

            string sql = @"INSERT INTO enquiryquoteprice (goodsId, goodsSn, eqType, fromQuantity, fromPrice, 
                          isIncludingTax, fromCompany, fromContact, fromTel, fromRemarks, 
                          toCompany, toUserId, fromUserId, brandName, dataFlag, createTime,
                          fromShopID, toShopId, fromDataFlag, toDataFlag, readStatus,
                          fromLot, fromManufacturers, fromPackaging, fromGoodsDesc, validity,
                          toQuantity, toPrice, toRemarks, sourceEqId)
                          VALUES (@goodsId, @goodsSn, @eqType, @fromQuantity, @fromPrice, @isIncludingTax, 
                          @fromCompany, @fromContact, @fromTel, @fromRemarks, @toCompany, 
                          @toUserId, @fromUserId, @brandName, 1, GETDATE(),
                           @fromShopID, @toShopId, 1, 1, 0,
                           @fromLot, @fromManufacturers, @fromPackaging, @fromGoodsDesc, @validity,
                           @toQuantity, @toPrice, @toRemarks, @sourceEqId)";

            int result = DbHelper.ExecuteNonQuery(sql,
                DbHelper.CreateParameter("@goodsId", goodsId),
                DbHelper.CreateParameter("@goodsSn", goodsSn ?? ""),
                DbHelper.CreateParameter("@eqType", eqType),
                DbHelper.CreateParameter("@fromQuantity", fromQuantity),
                DbHelper.CreateParameter("@fromPrice", fromPrice),
                DbHelper.CreateParameter("@isIncludingTax", isIncludingTax),
                DbHelper.CreateParameter("@fromCompany", fromCompany ?? ""),
                DbHelper.CreateParameter("@fromContact", fromContact ?? ""),
                DbHelper.CreateParameter("@fromTel", fromTel ?? ""),
                DbHelper.CreateParameter("@fromRemarks", fromRemarks ?? ""),
                DbHelper.CreateParameter("@toCompany", toCompany ?? ""),
                DbHelper.CreateParameter("@toUserId", toUserId),
                DbHelper.CreateParameter("@fromUserId", fromUserId),
                DbHelper.CreateParameter("@brandName", brandName ?? ""),
                DbHelper.CreateParameter("@fromShopID", fromShopId),
                DbHelper.CreateParameter("@toShopId", toShopId),
                DbHelper.CreateParameter("@fromLot", fromLot ?? ""),
                DbHelper.CreateParameter("@fromManufacturers", fromManufacturers ?? ""),
                DbHelper.CreateParameter("@fromPackaging", fromPackaging ?? ""),
                DbHelper.CreateParameter("@fromGoodsDesc", fromGoodsDesc ?? ""),
                DbHelper.CreateParameter("@validity", validity ?? ""),
                DbHelper.CreateParameter("@toQuantity", sourceQuantity),
                DbHelper.CreateParameter("@toPrice", sourcePrice),
                DbHelper.CreateParameter("@toRemarks", sourceRemarks ?? ""),
                DbHelper.CreateParameter("@sourceEqId", sourceEqId));

            if (result > 0 && sourceEqId > 0)
            {
                string updateSql = "UPDATE enquiryquoteprice SET toDataFlag = 0 WHERE eqId = @sourceEqId AND eqType = 1 AND dataFlag = 1";
                DbHelper.ExecuteNonQuery(updateSql, DbHelper.CreateParameter("@sourceEqId", sourceEqId));
            }

            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("SubmitQuote 错误: " + ex.Message);
            throw;
        }
    }

    /// <summary>
    /// 获取用户收到的报价列表（我是采购商）
    /// </summary>
    /// <param name="toUserId">接收方用户ID</param>
    /// <returns></returns>
    public DataTable GetReceivedQuotes(int toUserId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                e.eqId, e.goodsSn, e.eqType, e.fromQuantity, e.fromPrice, e.isIncludingTax, 
                e.createTime, e.fromCompany, e.brandName, e.toCompany, e.readStatus, e.fromRemarks,
                e.toDataFlag, e.goodsId, e.fromShopId, e.toShopId, e.fromLot,
                e.toQuantity, e.toPrice, e.toRemarks, e.sourceEqId,
                ISNULL(NULLIF(e.toQuantity, 0), ISNULL(iq.fromQuantity, 0)) as RealInquiryQuantity,
                ISNULL(NULLIF(e.toPrice, 0), ISNULL(iq.fromPrice, 0)) as RealInquiryPrice,
                ISNULL(NULLIF(e.toRemarks, ''), ISNULL(iq.fromRemarks, '')) as RealInquiryRemarks,
                ISNULL((SELECT TOP 1 Lot FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 Lot FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as GoodsLot,
                ISNULL(e.fromLot, 
                       ISNULL((SELECT TOP 1 Lot FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                              (SELECT TOP 1 Lot FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1))) as Lot,
                (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.fromShopId AND dataFlag = 1) as SellerQQ,
                (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.toShopId AND dataFlag = 1) as BuyerQQ,
                ISNULL((SELECT TOP 1 Manufacturers FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1), 
                       (SELECT TOP 1 Manufacturers FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as Manufacturers,
                ISNULL((SELECT TOP 1 Packaging FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 Packaging FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as Packaging,
                ISNULL((SELECT TOP 1 goodsDesc FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 goodsDesc FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as goodsDesc,
                (SELECT TOP 1 brandName FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = e.goodsSn AND toShopId = e.fromShopId AND dataFlag = 1 ORDER BY createTime DESC) as BuyerBrandName
                FROM enquiryquoteprice e
                LEFT JOIN enquiryquoteprice iq ON iq.eqId = e.sourceEqId AND iq.dataFlag = 1
                WHERE e.dataFlag = 1 AND e.eqType = 2 AND e.toUserId = @toUserId AND (e.toDataFlag IS NULL OR e.toDataFlag = 1)
                ORDER BY e.createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@toUserId", toUserId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToQuoteRecordData(dt);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GetReceivedQuotes 错误: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取用户发起的报价列表（我是供应商）
    /// </summary>
    /// <param name="fromUserId">发起方用户ID</param>
    /// <returns></returns>
    public DataTable GetSentQuotes(int fromUserId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                e.eqId, e.goodsSn, e.eqType, e.fromQuantity, e.fromPrice, e.isIncludingTax, 
                e.createTime, e.fromCompany, e.brandName, e.toCompany, e.readStatus, e.fromRemarks,
                e.fromDataFlag, e.toShopId, e.goodsId, e.fromShopId, e.fromLot, e.validity,
                e.toQuantity, e.toPrice, e.toRemarks, e.sourceEqId,
                ISNULL(NULLIF(e.toQuantity, 0), ISNULL(iq.fromQuantity, 0)) as RealInquiryQuantity,
                ISNULL(NULLIF(e.toPrice, 0), ISNULL(iq.fromPrice, 0)) as RealInquiryPrice,
                ISNULL(NULLIF(e.toRemarks, ''), ISNULL(iq.fromRemarks, '')) as RealInquiryRemarks,
                ISNULL((SELECT TOP 1 Lot FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 Lot FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as GoodsLot,
                ISNULL(e.fromLot, 
                       ISNULL((SELECT TOP 1 Lot FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                              (SELECT TOP 1 Lot FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1))) as Lot,
                (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.fromShopId AND dataFlag = 1) as SellerQQ,
                (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.toShopId AND dataFlag = 1) as BuyerQQ,
                ISNULL((SELECT TOP 1 Manufacturers FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1), 
                       (SELECT TOP 1 Manufacturers FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as Manufacturers,
                ISNULL((SELECT TOP 1 Packaging FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 Packaging FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as Packaging,
                ISNULL((SELECT TOP 1 goodsDesc FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 goodsDesc FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as goodsDesc,
                (SELECT TOP 1 brandName FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = e.goodsSn AND toShopId = e.fromShopId AND dataFlag = 1 ORDER BY createTime DESC) as BuyerBrandName
                FROM enquiryquoteprice e
                LEFT JOIN enquiryquoteprice iq ON iq.eqId = e.sourceEqId AND iq.dataFlag = 1
                WHERE e.dataFlag = 1 AND e.eqType = 2 AND e.fromUserId = @fromUserId AND (e.fromDataFlag IS NULL OR e.fromDataFlag = 1)
                ORDER BY e.createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@fromUserId", fromUserId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToQuoteRecordData(dt);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GetSentQuotes 错误: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取用户通过店铺发起的报价列表
    /// </summary>
    /// <param name="fromShopId">发起方店铺ID</param>
    /// <returns></returns>
    public DataTable GetQuotesByShop(int fromShopId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                e.eqId, e.goodsSn, e.eqType, e.fromQuantity, e.fromPrice, e.isIncludingTax, 
                e.createTime, e.fromCompany, e.brandName as SellerBrandName, e.toCompany, e.readStatus, e.fromRemarks,
                e.fromDataFlag, e.fromShopId, e.toShopId, e.goodsId, e.fromLot, e.validity,
                e.toQuantity, e.toPrice, e.toRemarks, e.sourceEqId,
                ISNULL(NULLIF(e.toQuantity, 0), ISNULL(iq.fromQuantity, 0)) as RealInquiryQuantity,
                ISNULL(NULLIF(e.toPrice, 0), ISNULL(iq.fromPrice, 0)) as RealInquiryPrice,
                ISNULL(NULLIF(e.toRemarks, ''), ISNULL(iq.fromRemarks, '')) as RealInquiryRemarks,
                e.fromLot,
                ISNULL((SELECT TOP 1 Lot FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 Lot FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as GoodsLot,
                ISNULL(e.fromLot, 
                       ISNULL((SELECT TOP 1 Lot FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                              (SELECT TOP 1 Lot FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1))) as Lot,
                (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.fromShopId AND dataFlag = 1) as SellerQQ,
                (SELECT TOP 1 shopQQ FROM shops WHERE shopId = e.toShopId AND dataFlag = 1) as BuyerQQ,
                ISNULL((SELECT TOP 1 Manufacturers FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1), 
                       (SELECT TOP 1 Manufacturers FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as Manufacturers,
                ISNULL((SELECT TOP 1 Packaging FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 Packaging FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as Packaging,
                ISNULL((SELECT TOP 1 goodsDesc FROM goods WHERE goodsId = e.goodsId AND dataFlag = 1),
                       (SELECT TOP 1 goodsDesc FROM goods WHERE goodsSn = e.goodsSn AND dataFlag = 1)) as goodsDesc
                FROM enquiryquoteprice e
                LEFT JOIN enquiryquoteprice iq ON iq.eqId = e.sourceEqId AND iq.dataFlag = 1
                WHERE e.dataFlag = 1 AND e.eqType = 2 AND e.fromShopId = @fromShopId AND (e.fromDataFlag IS NULL OR e.fromDataFlag = 1)
                ORDER BY e.createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@fromShopId", fromShopId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    return ConvertToQuoteRecordData(dt);
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine("ConvertToQuoteRecordData 错误: " + ex2.Message);
                    return ConvertToQuoteRecordDataSafe(dt);
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GetQuotesByShop 错误: " + ex.Message);
            return null;
        }
    }

    private void EnrichQuoteRecordWithInquiryInfo(DataTable dt)
    {
        if (dt == null || dt.Rows.Count == 0) return;
        try
        {
            foreach (DataRow row in dt.Rows)
            {
                int toQty = 0;
                try { toQty = Convert.ToInt32(row["toQuantity"]); } catch { }
                if (toQty <= 0)
                {
                    string goodsSn = "";
                    int sourceEqId = 0;
                    int fromShopIdVal = 0;
                    int toShopIdVal = 0;
                    try { goodsSn = row["goodsSn"].ToString(); } catch { }
                    try { sourceEqId = Convert.ToInt32(row["sourceEqId"]); } catch { }
                    try { fromShopIdVal = Convert.ToInt32(row["fromShopId"]); } catch { }
                    try { toShopIdVal = Convert.ToInt32(row["toShopId"]); } catch { }
                    
                    DataTable sourceDt = null;
                    
                    // 优先使用 sourceEqId 直接查找
                    if (sourceEqId > 0)
                    {
                        try
                        {
                            string sourceSqlById = "SELECT fromQuantity, fromPrice, fromRemarks FROM enquiryquoteprice WHERE eqId = @sourceEqId AND dataFlag = 1";
                            sourceDt = DbHelper.ExecuteQuery(sourceSqlById, 
                                DbHelper.CreateParameter("@sourceEqId", sourceEqId));
                        }
                        catch { }
                    }
                    
                    // 如果 sourceEqId 没找到，再用 goodsSn 和 shopId 查找
                    if ((sourceDt == null || sourceDt.Rows.Count == 0) && 
                        !string.IsNullOrEmpty(goodsSn) && toShopIdVal > 0 && fromShopIdVal > 0)
                    {
                        try
                        {
                            string sourceSql = "SELECT TOP 1 fromQuantity, fromPrice, fromRemarks FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = @goodsSn AND fromShopId = @fromShopIdParam AND toShopId = @toShopIdParam AND dataFlag = 1 ORDER BY createTime DESC";
                            sourceDt = DbHelper.ExecuteQuery(sourceSql, 
                                DbHelper.CreateParameter("@goodsSn", goodsSn),
                                DbHelper.CreateParameter("@fromShopIdParam", toShopIdVal),
                                DbHelper.CreateParameter("@toShopIdParam", fromShopIdVal));
                        }
                        catch { }
                    }
                    
                    if (sourceDt != null && sourceDt.Rows.Count > 0)
                    {
                        DataRow sourceRow = sourceDt.Rows[0];
                        int srcQty = 0;
                        decimal srcPrice = 0;
                        string srcRemarks = "";
                        try { srcQty = Convert.ToInt32(sourceRow["fromQuantity"]); } catch { }
                        try { srcPrice = Convert.ToDecimal(sourceRow["fromPrice"]); } catch { }
                        try { srcRemarks = sourceRow["fromRemarks"].ToString(); } catch { }
                        
                        if (srcQty > 0)
                        {
                            row["toQuantity"] = srcQty;
                        }
                        if (srcPrice > 0)
                            row["toPrice"] = srcPrice;
                        if (!string.IsNullOrEmpty(srcRemarks))
                            row["toRemarks"] = srcRemarks;
                    }
                }
            }
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine("EnrichQuoteRecordWithInquiryInfo 错误: " + ex.Message); }
    }

    /// <summary>
    /// 一次性数据修复：将已存在报价记录中 toQuantity=0 的记录从原始询价记录中补全并保存
    /// </summary>
    public int FixQuoteRecordsWithInquiryInfo()
    {
        try
        {
            EnsureColumnExistsForQuote("toQuantity", "INT DEFAULT 0");
            EnsureColumnExistsForQuote("toPrice", "DECIMAL(18,4) DEFAULT 0");
            EnsureColumnExistsForQuote("toRemarks", "NVARCHAR(500) NULL");

            string findSql = @"SELECT eqId, goodsSn, fromShopId, toShopId, toQuantity, sourceEqId 
                FROM enquiryquoteprice 
                WHERE dataFlag = 1 AND eqType = 2 AND (toQuantity IS NULL OR toQuantity = 0)";
            DataTable dt = DbHelper.ExecuteQuery(findSql);
            if (dt == null || dt.Rows.Count == 0) return 0;
            
            int fixedCount = 0;
            foreach (DataRow row in dt.Rows)
            {
                int eqId = 0;
                string goodsSn = "";
                int fromShopIdVal = 0;
                int toShopIdVal = 0;
                int sourceEqId = 0;
                try { eqId = Convert.ToInt32(row["eqId"]); } catch { }
                try { goodsSn = row["goodsSn"].ToString(); } catch { }
                try { fromShopIdVal = Convert.ToInt32(row["fromShopId"]); } catch { }
                try { toShopIdVal = Convert.ToInt32(row["toShopId"]); } catch { }
                try { sourceEqId = Convert.ToInt32(row["sourceEqId"]); } catch { }
                
                if (eqId <= 0)
                    continue;
                
                DataTable sourceDt = null;
                
                // 优先使用 sourceEqId 直接查找
                if (sourceEqId > 0)
                {
                    try
                    {
                        string sourceSqlById = "SELECT fromQuantity, fromPrice, fromRemarks FROM enquiryquoteprice WHERE eqId = @sourceEqId AND dataFlag = 1";
                        sourceDt = DbHelper.ExecuteQuery(sourceSqlById, 
                            DbHelper.CreateParameter("@sourceEqId", sourceEqId));
                    }
                    catch { }
                }
                
                // 如果 sourceEqId 没找到，再用 goodsSn 和 shopId 查找
                if ((sourceDt == null || sourceDt.Rows.Count == 0) && 
                    !string.IsNullOrEmpty(goodsSn) && fromShopIdVal > 0 && toShopIdVal > 0)
                {
                    try
                    {
                        string sourceSql = "SELECT TOP 1 fromQuantity, fromPrice, fromRemarks FROM enquiryquoteprice WHERE eqType = 1 AND goodsSn = @goodsSn AND fromShopId = @fromShopIdParam AND toShopId = @toShopIdParam AND dataFlag = 1 ORDER BY createTime DESC";
                        sourceDt = DbHelper.ExecuteQuery(sourceSql, 
                            DbHelper.CreateParameter("@goodsSn", goodsSn),
                            DbHelper.CreateParameter("@fromShopIdParam", toShopIdVal),
                            DbHelper.CreateParameter("@toShopIdParam", fromShopIdVal));
                    }
                    catch { }
                }
                
                if (sourceDt != null && sourceDt.Rows.Count > 0)
                {
                    DataRow sourceRow = sourceDt.Rows[0];
                    int srcQty = 0;
                    decimal srcPrice = 0;
                    string srcRemarks = "";
                    try { srcQty = Convert.ToInt32(sourceRow["fromQuantity"]); } catch { }
                    try { srcPrice = Convert.ToDecimal(sourceRow["fromPrice"]); } catch { }
                    try { srcRemarks = sourceRow["fromRemarks"].ToString(); } catch { }
                    
                    if (srcQty > 0 || srcPrice > 0 || !string.IsNullOrEmpty(srcRemarks))
                    {
                        string updateSql = "UPDATE enquiryquoteprice SET toQuantity = @toQuantity, toPrice = @toPrice, toRemarks = @toRemarks WHERE eqId = @eqId";
                        int result = DbHelper.ExecuteNonQuery(updateSql,
                            DbHelper.CreateParameter("@toQuantity", srcQty),
                            DbHelper.CreateParameter("@toPrice", srcPrice),
                            DbHelper.CreateParameter("@toRemarks", srcRemarks ?? ""),
                            DbHelper.CreateParameter("@eqId", eqId));
                        if (result > 0) fixedCount++;
                    }
                }
            }
            return fixedCount;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("FixQuoteRecordsWithInquiryInfo 错误: " + ex.Message);
            return -1;
        }
    }

    private void EnsureColumnExistsForQuote(string columnName, string columnType)
    {
        try
        {
            string checkColSql = "SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('enquiryquoteprice') AND name = @colName";
            object colResult = DbHelper.ExecuteScalar(checkColSql, DbHelper.CreateParameter("@colName", columnName));
            if (colResult == null || Convert.ToInt32(colResult) == 0)
            {
                string addColSql = "ALTER TABLE enquiryquoteprice ADD " + columnName + " " + columnType;
                DbHelper.ExecuteNonQuery(addColSql);
            }
        }
        catch { }
    }

    /// <summary>
    /// 更新报价读取状态
    /// </summary>
    /// <param name="eqId">报价ID</param>
    /// <param name="readStatus">读取状态</param>
    /// <returns></returns>
    public bool UpdateReadStatus(int eqId, int readStatus)
    {
        try
        {
            string sql = @"UPDATE enquiryquoteprice SET readStatus = @readStatus WHERE eqId = @eqId";
            int result = DbHelper.ExecuteNonQuery(sql,
                DbHelper.CreateParameter("@eqId", eqId),
                DbHelper.CreateParameter("@readStatus", readStatus));
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("UpdateReadStatus 错误: " + ex.Message);
            return false;
        }
    }

    #region 安全取值方法

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

    #endregion
}