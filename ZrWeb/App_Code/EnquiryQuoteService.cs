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
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("BuyerName", typeof(string));
        dt.Columns.Add("QuoteTime", typeof(string));
        dt.Columns.Add("Validity", typeof(string));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int eqId = GetIntValue(row["eqId"], 0);
            int readStatus = GetIntValue(row["readStatus"], 0);
            string goodsSn = GetStringValue(row["goodsSn"]);
            string brandName = GetStringValue(row["brandName"]);
            int fromQuantity = GetIntValue(row["fromQuantity"], 0);
            decimal fromPrice = GetDecimalValue(row["fromPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
            string toCompany = GetStringValue(row["toCompany"]);
            string fromRemarks = GetStringValue(row["fromRemarks"]);

            newRow["EqId"] = eqId;

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

            // 设置品牌参数
            if (!string.IsNullOrEmpty(brandName))
            {
                newRow["BrandParams"] = brandName;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
            }

            // 设置数量和单位
            if (fromQuantity > 0)
            {
                newRow["Quantity"] = fromQuantity.ToString();
                newRow["Unit"] = "Kpcs";
            }
            else
            {
                newRow["Quantity"] = "0";
                newRow["Unit"] = "Kpcs";
            }

            // 设置价格
            if (fromPrice > 0)
            {
                string taxText = isIncludingTax == 1 ? "含税" : "未税";
                newRow["Price"] = "¥" + fromPrice.ToString("0.00") + " (" + taxText + ")";
            }
            else
            {
                newRow["Price"] = "面议";
            }

            // 设置采购商
            newRow["BuyerName"] = !string.IsNullOrEmpty(toCompany) ? toCompany : "匿名采购商";

            // 设置报价时间
            newRow["QuoteTime"] = createTime.ToString("yyyy-MM-dd HH:mm");

            // 设置有效期
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

            // 设置品牌参数
            if (!string.IsNullOrEmpty(brandName))
            {
                newRow["BrandParams"] = brandName;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
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
        int fromShopId, int toShopId, int eqType = 2)
    {
        try
        {
            string sql = @"INSERT INTO enquiryquoteprice (goodsId, goodsSn, eqType, fromQuantity, fromPrice, 
                          isIncludingTax, fromCompany, fromContact, fromTel, fromRemarks, 
                          toCompany, toUserId, fromUserId, brandName, dataFlag, createTime,
                          fromShopID, toShopId, fromDataFlag, toDataFlag)
                          VALUES (@goodsId, @goodsSn, @eqType, @fromQuantity, @fromPrice, @isIncludingTax, 
                          @fromCompany, @fromContact, @fromTel, @fromRemarks, @toCompany, 
                          @toUserId, @fromUserId, @brandName, 1, GETDATE(),
                          @fromShopID, @toShopId, 1, 1)";

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
                DbHelper.CreateParameter("@toShopId", toShopId));

            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("SubmitQuote 错误: " + ex.Message);
            return false;
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
                eqId, goodsSn, eqType, fromQuantity, fromPrice, isIncludingTax, 
                createTime, fromCompany, brandName, toCompany, readStatus, fromRemarks,
                toDataFlag
                FROM enquiryquoteprice 
                WHERE dataFlag = 1 AND eqType = 2 AND toUserId = @toUserId AND toDataFlag = 1
                ORDER BY createTime DESC";

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
                eqId, goodsSn, eqType, fromQuantity, fromPrice, isIncludingTax, 
                createTime, fromCompany, brandName, toCompany, readStatus, fromRemarks,
                fromDataFlag
                FROM enquiryquoteprice 
                WHERE dataFlag = 1 AND eqType = 2 AND fromUserId = @fromUserId AND fromDataFlag = 1
                ORDER BY createTime DESC";

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
                eqId, goodsSn, eqType, fromQuantity, fromPrice, isIncludingTax, 
                createTime, fromCompany, brandName, toCompany, readStatus, fromRemarks,
                fromDataFlag
                FROM enquiryquoteprice 
                WHERE dataFlag = 1 AND eqType = 2 AND fromShopId = @fromShopId AND fromDataFlag = 1
                ORDER BY createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@fromShopId", fromShopId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToQuoteRecordData(dt);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GetQuotesByShop 错误: " + ex.Message);
            return null;
        }
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