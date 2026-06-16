using System;
using System.Data;
using System.Data.SqlClient;

public class GoodsService
{
    public DataTable GetSupplyList(int pubType)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                g.goodsId, g.goodsSn, g.[Name], g.Manufacturers, g.goodsStock, g.goodsUnit, 
                g.shopPrice, g.isIncludingTax, g.createTime, g.validityDate, g.isSale, g.goodsStatus, g.dataFlag, g.pubType, g.shopId,
                s.shopName AS CompanyName
                FROM goods g
                LEFT JOIN shops s ON g.shopId = s.shopId
                WHERE g.dataFlag = 1 AND g.goodsStatus = 1
                ORDER BY g.createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql);
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToSupplyData(dt);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GoodsService.GetSupplyList 错误: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取商家工作台库存列表（只显示在线供应）
    /// </summary>
    public DataTable GetInventoryList(int pubType, int shopId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, createTime, validityDate, isSale, goodsStatus, dataFlag, pubType
                FROM goods 
                WHERE pubType = @pubType AND isSale = 1 AND dataFlag = 1 AND shopId = @shopId
                ORDER BY createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                DbHelper.CreateParameter("@pubType", pubType),
                DbHelper.CreateParameter("@shopId", shopId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToInventoryData(dt);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GoodsService.GetInventoryList 错误: " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 获取商家工作台已下架库存列表
    /// </summary>
    public DataTable GetExpiredInventoryList(int pubType, int shopId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, updateTime, validityDate, isSale, goodsStatus, dataFlag
                FROM goods 
                WHERE dataFlag = 1 AND pubType = @pubType AND isSale = 0 AND shopId = @shopId
                ORDER BY updateTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                DbHelper.CreateParameter("@pubType", pubType),
                DbHelper.CreateParameter("@shopId", shopId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToExpiredInventoryData(dt);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GoodsService.GetExpiredInventoryList 错误: " + ex.Message);
            return null;
        }
    }

    public DataTable GetExpiredSupplyList(int pubType)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, updateTime, validityDate, isSale, goodsStatus, dataFlag
                FROM goods 
                WHERE dataFlag = 1 AND pubType = @pubType AND isSale = 0
                ORDER BY updateTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@pubType", pubType));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToExpiredInventoryData(dt);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public int GetOnlineSupplyCount(int pubType, int shopId)
    {
        try
        {
            string sql = "SELECT COUNT(*) FROM goods WHERE dataFlag = 1 AND pubType = @pubType AND isSale = 1 AND goodsStatus = 1 AND shopId = @shopId";
            object result = DbHelper.ExecuteScalar(sql, 
                DbHelper.CreateParameter("@pubType", pubType),
                DbHelper.CreateParameter("@shopId", shopId));
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        catch
        {
            return 0;
        }
    }

    public int GetExpiredCount(int pubType, int shopId)
    {
        try
        {
            string sql = "SELECT COUNT(*) FROM goods WHERE dataFlag = 1 AND pubType = @pubType AND isSale = 0 AND shopId = @shopId";
            object result = DbHelper.ExecuteScalar(sql, 
                DbHelper.CreateParameter("@pubType", pubType),
                DbHelper.CreateParameter("@shopId", shopId));
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }
        catch
        {
            return 0;
        }
    }

    public bool InsertGoods(string goodsSn, string name, string manufacturers, string packaging,
        int goodsStock, string goodsUnit, decimal shopPrice, int isIncludingTax,
        int pubType, string remarks, int shopId, int userId)
    {
        try
        {
            string sql = @"INSERT INTO goods (goodsSn, [Name], Manufacturers, Packaging, goodsStock, goodsUnit, 
                          shopPrice, isIncludingTax, pubType, remarks, shopId, dataFlag, goodsStatus, isSale, createTime, updateTime)
                          VALUES (@goodsSn, @Name, @Manufacturers, @Packaging, @goodsStock, @goodsUnit, 
                          @shopPrice, @isIncludingTax, @pubType, @remarks, @shopId, 1, 1, 1, GETDATE(), GETDATE())";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@goodsSn", goodsSn ?? (object)DBNull.Value),
                new SqlParameter("@Name", name ?? (object)DBNull.Value),
                new SqlParameter("@Manufacturers", manufacturers ?? (object)DBNull.Value),
                new SqlParameter("@Packaging", packaging ?? (object)DBNull.Value),
                new SqlParameter("@goodsStock", goodsStock),
                new SqlParameter("@goodsUnit", goodsUnit ?? "Kpcs"),
                new SqlParameter("@shopPrice", shopPrice),
                new SqlParameter("@isIncludingTax", isIncludingTax),
                new SqlParameter("@pubType", pubType),
                new SqlParameter("@remarks", remarks ?? (object)DBNull.Value),
                new SqlParameter("@shopId", shopId)
            };

            int result = DbHelper.ExecuteNonQuery(sql, parameters);
            System.Diagnostics.Debug.WriteLine("InsertGoods affected rows: " + result);
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("GoodsService.InsertGoods 错误: " + ex.Message);
            System.Diagnostics.Debug.WriteLine("InsertGoods StackTrace: " + ex.StackTrace);
            return false;
        }
    }

    public DataTable GetDemandList(int pubType, int shopId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, createTime, validityDate, isSale, goodsStatus, dataFlag
                FROM goods 
                WHERE dataFlag = 1 AND pubType = @pubType AND shopId = @shopId
                ORDER BY createTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                DbHelper.CreateParameter("@pubType", pubType),
                DbHelper.CreateParameter("@shopId", shopId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToDemandData(dt);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public DataTable GetExpiredDemandList(int pubType, int shopId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, updateTime, validityDate, isSale, goodsStatus, dataFlag
                FROM goods 
                WHERE dataFlag = 1 AND pubType = @pubType AND isSale = 0 AND shopId = @shopId
                ORDER BY updateTime DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                DbHelper.CreateParameter("@pubType", pubType),
                DbHelper.CreateParameter("@shopId", shopId));
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToExpiredDemandData(dt);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    private DataTable ConvertToDemandData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("goodsId", typeof(int));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("StatusClass", typeof(string));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("IsTaxed", typeof(bool));
        dt.Columns.Add("RemainingTime", typeof(string));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int goodsId = GetIntValue(row["goodsId"], 0);
            string goodsSn = GetStringValue(row["goodsSn"]);
            string name = GetStringValue(row["Name"]);
            string manufacturers = GetStringValue(row["Manufacturers"]);
            int goodsStock = GetIntValue(row["goodsStock"], 0);
            string goodsUnit = GetStringValue(row["goodsUnit"]);
            decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
            DateTime validityDate = GetDateTimeValue(row["validityDate"], DateTime.Now.AddDays(3));

            int isSale = GetIntValue(row["isSale"], 0);
            if (isSale == 1)
            {
                TimeSpan diff = validityDate - DateTime.Now;
                if (diff.TotalHours < 24)
                {
                    newRow["Status"] = "即将到期";
                    newRow["StatusClass"] = "orange";
                    newRow["RemainingTime"] = "<span class=\"time-danger\">小于 24 小时</span>";
                }
                else if (diff.TotalDays < 3)
                {
                    newRow["Status"] = "即将到期";
                    newRow["StatusClass"] = "orange";
                    newRow["RemainingTime"] = diff.TotalHours.ToString("0") + " 小时";
                }
                else
                {
                    newRow["Status"] = "采购中";
                    newRow["StatusClass"] = "blue";
                    newRow["RemainingTime"] = "72 小时";
                }
            }
            else
            {
                newRow["Status"] = "已下架";
                newRow["StatusClass"] = "gray";
                newRow["RemainingTime"] = "已下架";
            }

            newRow["goodsId"] = goodsId;

            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                newRow["Model"] = name;
            }
            else
            {
                newRow["Model"] = "未知型号";
            }

            if (!string.IsNullOrEmpty(manufacturers))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
            }

            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private DataTable ConvertToExpiredDemandData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("goodsId", typeof(int));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("IsTaxed", typeof(bool));
        dt.Columns.Add("OfflineTime", typeof(string));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int goodsId = GetIntValue(row["goodsId"], 0);
            string goodsSn = GetStringValue(row["goodsSn"]);
            string name = GetStringValue(row["Name"]);
            string manufacturers = GetStringValue(row["Manufacturers"]);
            int goodsStock = GetIntValue(row["goodsStock"], 0);
            string goodsUnit = GetStringValue(row["goodsUnit"]);
            decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime updateTime = GetDateTimeValue(row["updateTime"], DateTime.Now);

            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                newRow["Model"] = name;
            }
            else
            {
                newRow["Model"] = "未知型号";
            }

            if (!string.IsNullOrEmpty(manufacturers))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
            }

            newRow["goodsId"] = goodsId;
            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;
            newRow["OfflineTime"] = updateTime.ToString("yyyy-MM-dd HH:mm");

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private DataTable ConvertToSupplyData(DataTable sourceTable)
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
        dt.Columns.Add("GoodsId", typeof(int));
        dt.Columns.Add("GoodsSn", typeof(string));
        dt.Columns.Add("ShopId", typeof(int));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            string goodsSn = GetStringValue(row["goodsSn"]);
            string name = GetStringValue(row["Name"]);
            string manufacturers = GetStringValue(row["Manufacturers"]);
            int goodsStock = GetIntValue(row["goodsStock"], 0);
            string goodsUnit = GetStringValue(row["goodsUnit"]);
            decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
            int goodsId = GetIntValue(row["goodsId"], 0);
            int pubType = GetIntValue(row["pubType"], 1);
            int shopId = GetIntValue(row["shopId"], 0);

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

            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                newRow["Model"] = name;
            }
            else
            {
                newRow["Model"] = "未知型号";
            }

            if (pubType == 2)
            {
                newRow["DetailUrl"] = "/demand-detail.aspx?id=" + goodsId;
            }
            else
            {
                newRow["DetailUrl"] = "/supply-detail.aspx?id=" + goodsId;
            }

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

            if (!string.IsNullOrEmpty(manufacturers))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
            }

            if (pubType == 2)
            {
                if (goodsStock > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    newRow["QuantityDisplay"] = "需求 " + goodsStock + "/" + unit;
                }
                else
                {
                    newRow["QuantityDisplay"] = "需求数量待定";
                }
            }
            else
            {
                if (goodsStock > 0)
                {
                    string unit = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
                    newRow["QuantityDisplay"] = "现货 " + goodsStock + "/" + unit;
                }
                else
                {
                    newRow["QuantityDisplay"] = "按需供应";
                }
            }

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

            string companyName = GetStringValue(row["CompanyName"]);
            if (!string.IsNullOrEmpty(companyName))
            {
                newRow["CompanyName"] = companyName;
            }
            else
            {
                newRow["CompanyName"] = "商家店铺";
            }
            
            if (pubType == 2)
            {
                newRow["ActionText"] = "我要报价";
            }
            else
            {
                newRow["ActionText"] = "立即询价";
            }

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private DataTable ConvertToInventoryData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("StatusClass", typeof(string));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("IsTaxed", typeof(bool));
        dt.Columns.Add("RemainingTime", typeof(string));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            string goodsSn = GetStringValue(row["goodsSn"]);
            string name = GetStringValue(row["Name"]);
            string manufacturers = GetStringValue(row["Manufacturers"]);
            int goodsStock = GetIntValue(row["goodsStock"], 0);
            string goodsUnit = GetStringValue(row["goodsUnit"]);
            decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime createTime = GetDateTimeValue(row["createTime"], DateTime.Now);
            DateTime validityDate = GetDateTimeValue(row["validityDate"], DateTime.Now.AddDays(3));

            int isSale = GetIntValue(row["isSale"], 0);
            if (isSale == 1)
            {
                TimeSpan diff = validityDate - DateTime.Now;
                if (diff.TotalHours < 24)
                {
                    newRow["Status"] = "即将到期";
                    newRow["StatusClass"] = "orange";
                    newRow["RemainingTime"] = "<span class=\"time-danger\">小于 24 小时</span>";
                }
                else if (diff.TotalDays < 3)
                {
                    newRow["Status"] = "即将到期";
                    newRow["StatusClass"] = "orange";
                    newRow["RemainingTime"] = diff.TotalHours.ToString("0") + " 小时";
                }
                else
                {
                    newRow["Status"] = "供应";
                    newRow["StatusClass"] = "blue";
                    newRow["RemainingTime"] = "72 小时";
                }
            }
            else
            {
                newRow["Status"] = "已下架";
                newRow["StatusClass"] = "gray";
                newRow["RemainingTime"] = "已下架";
            }

            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                newRow["Model"] = name;
            }
            else
            {
                newRow["Model"] = "未知型号";
            }

            if (!string.IsNullOrEmpty(manufacturers))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
            }

            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private DataTable ConvertToExpiredInventoryData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("goodsId", typeof(int));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("IsTaxed", typeof(bool));
        dt.Columns.Add("OfflineTime", typeof(string));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int goodsId = GetIntValue(row["goodsId"], 0);
            string goodsSn = GetStringValue(row["goodsSn"]);
            string name = GetStringValue(row["Name"]);
            string manufacturers = GetStringValue(row["Manufacturers"]);
            int goodsStock = GetIntValue(row["goodsStock"], 0);
            string goodsUnit = GetStringValue(row["goodsUnit"]);
            decimal shopPrice = GetDecimalValue(row["shopPrice"], 0);
            int isIncludingTax = GetIntValue(row["isIncludingTax"], 0);
            DateTime updateTime = GetDateTimeValue(row["updateTime"], DateTime.Now);

            if (!string.IsNullOrEmpty(goodsSn))
            {
                newRow["Model"] = goodsSn;
            }
            else if (!string.IsNullOrEmpty(name))
            {
                newRow["Model"] = name;
            }
            else
            {
                newRow["Model"] = "未知型号";
            }

            if (!string.IsNullOrEmpty(manufacturers))
            {
                newRow["BrandParams"] = manufacturers;
            }
            else
            {
                newRow["BrandParams"] = "品牌不限";
            }

            newRow["goodsId"] = goodsId;
            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;
            newRow["OfflineTime"] = updateTime.ToString("yyyy-MM-dd HH:mm");

            dt.Rows.Add(newRow);
        }

        return dt;
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

    /// <summary>
    /// 发布采购需求
    /// </summary>
    public bool PublishDemand(string goodsSn, string name, string manufacturers, int quantity, string unit, decimal price, int isIncludingTax, int userId, int shopId)
    {
        try
        {
            string sql = @"INSERT INTO goods (goodsSn, Name, Manufacturers, goodsStock, goodsUnit, shopPrice, isIncludingTax, pubType, isSale, goodsStatus, dataFlag, createTime, validityDate, shopId)
                VALUES (@goodsSn, @name, @manufacturers, @quantity, @unit, @price, @isIncludingTax, 2, 1, 1, 1, GETDATE(), DATEADD(day, 30, GETDATE()), @shopId)";

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@goodsSn", goodsSn ?? ""),
                new SqlParameter("@name", name ?? ""),
                new SqlParameter("@manufacturers", manufacturers ?? ""),
                new SqlParameter("@quantity", quantity),
                new SqlParameter("@unit", unit ?? "Kpcs"),
                new SqlParameter("@price", price),
                new SqlParameter("@isIncludingTax", isIncludingTax),
                new SqlParameter("@shopId", shopId),
            };

            int result = DbHelper.ExecuteNonQuery(sql, parameters);
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("PublishDemand 错误: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 下架商品（将 isSale 设置为 0）
    /// </summary>
    public bool TakeOff(int goodsId)
    {
        try
        {
            string sql = @"UPDATE goods SET isSale = 0, updateTime = GETDATE() WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("TakeOff 错误: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 重新上架商品（将 isSale 设置为 1）
    /// </summary>
    public bool Restock(int goodsId)
    {
        try
        {
            string sql = @"UPDATE goods SET isSale = 1, updateTime = GETDATE() WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Restock 错误: " + ex.Message);
            return false;
        }
    }

    public bool ToggleTax(int goodsId)
    {
        try
        {
            string sql = @"UPDATE goods SET isIncludingTax = CASE WHEN isIncludingTax = 1 THEN 0 ELSE 1 END, updateTime = GETDATE() WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
            return result > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ToggleTax 错误: " + ex.Message);
            return false;
        }
    }
}