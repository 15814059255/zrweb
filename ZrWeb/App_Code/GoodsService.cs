using System;
using System.Data;
using System.Data.SqlClient;

public class GoodsService
{
    private static bool _fieldsChecked = false;
    private static object _fieldsLock = new object();

    public GoodsService()
    {
        EnsureGoodsFields();
    }

    private void EnsureGoodsFields()
    {
        if (_fieldsChecked) return;
        lock (_fieldsLock)
        {
            if (_fieldsChecked) return;
            try
            {
                string[] fields = {
                    "Brand NVARCHAR(100)",
                    "Capacitance NVARCHAR(50)",
                    "Resistance NVARCHAR(50)",
                    "Tolerance NVARCHAR(30)",
                    "Voltage NVARCHAR(30)",
                    "Dielectric NVARCHAR(30)",
                    "Power NVARCHAR(30)",
                    "TempCoefficient NVARCHAR(30)",
                    "pubSource INT DEFAULT 0"
                };
                foreach (string field in fields)
                {
                    string[] parts = field.Split(new[] {' '}, 2);
                    string fieldName = parts[0];
                    string checkSql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'goods' AND COLUMN_NAME = @fieldName";
                    object countObj = DbHelper.ExecuteScalar(checkSql, DbHelper.CreateParameter("@fieldName", fieldName));
                    int count = countObj != null && countObj != DBNull.Value ? Convert.ToInt32(countObj) : 0;
                    if (count == 0)
                    {
                        try
                        {
                            string addSql = "ALTER TABLE goods ADD " + field;
                            DbHelper.ExecuteNonQuery(addSql);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
            _fieldsChecked = true;
        }
    }

    public DataTable GetSupplyList(int pubType, int page = 1, int pageSize = 45)
    {
        try
        {
            int offset = (page - 1) * pageSize;
            string sql = string.Format(@"SELECT 
                g.goodsId, g.goodsSn, g.[Name], g.Manufacturers, g.Packaging, g.goodsStock, g.goodsUnit, 
                g.shopPrice, g.isIncludingTax, g.createTime, g.validityDate, g.isSale, g.goodsStatus, g.dataFlag, g.pubType, g.shopId,
                g.Brand, g.Capacitance, g.Resistance, g.Tolerance, g.Voltage, g.Dielectric, g.Power, g.TempCoefficient,
                ISNULL(s.shopCompany, s.shopName) AS companyName
                FROM goods g
                LEFT JOIN shops s ON g.shopId = s.shopId
                WHERE g.dataFlag = 1 AND g.goodsStatus = 1 AND g.isSale = 1
                ORDER BY g.createTime DESC
                OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, pageSize);

            DataTable dt = DbHelper.ExecuteQuery(sql);
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToSupplyData(dt);
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public int GetSupplyListCount(int pubType)
    {
        try
        {
            string sql = @"SELECT COUNT(*) 
                FROM goods g
                LEFT JOIN shops s ON g.shopId = s.shopId
                WHERE g.dataFlag = 1 AND g.goodsStatus = 1 AND g.isSale = 1";

            object result = DbHelper.ExecuteScalar(sql);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    /// <summary>
    /// 获取商家工作台库存列表（只显示在线供应）
    /// </summary>
    public DataTable GetInventoryList(int pubType, int shopId)
    {
        return GetInventoryList(pubType, shopId, "");
    }

    /// <summary>
    /// 获取商家工作台库存列表（带搜索）
    /// </summary>
    public DataTable GetInventoryList(int pubType, int shopId, string keyword)
    {
        try
        {
            string sql = @"SELECT 
                goodsId, goodsSn, Name, Manufacturers, Packaging, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, createTime, validityDate, isSale, goodsStatus, dataFlag, pubType, pubSource,
                Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient
                FROM goods 
                WHERE pubType = @pubType AND isSale = 1 AND dataFlag = 1 AND shopId = @shopId";

            var parameters = new System.Collections.Generic.List<SqlParameter>();
            parameters.Add(DbHelper.CreateParameter("@pubType", pubType));
            parameters.Add(DbHelper.CreateParameter("@shopId", shopId));

            if (!string.IsNullOrEmpty(keyword) && keyword.Trim().Length > 0)
            {
                string[] searchFields = new string[] { 
                    "goodsSn", "Name", "Manufacturers", "Brand", 
                    "Packaging", "Capacitance", "Resistance", 
                    "Tolerance", "Voltage", "Dielectric" 
                };
                var searchCond = BuildParametricSearchCondition(keyword, searchFields, "inv");
                sql += searchCond.Item1;
                parameters.AddRange(searchCond.Item2);
            }

            sql += " ORDER BY createTime DESC, validityDate ASC";

            DataTable dt = DbHelper.ExecuteQuery(sql, parameters.ToArray());
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToInventoryData(dt);
            }
            
            return null;
        }
        catch (Exception)
        {
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
                goodsId, goodsSn, Name, Manufacturers, Packaging, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, updateTime, validityDate, isSale, goodsStatus, dataFlag, pubSource,
                Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient
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
        catch (Exception)
        {
            return null;
        }
    }

    public DataTable GetExpiredSupplyList(int pubType)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, Packaging, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, updateTime, validityDate, isSale, goodsStatus, dataFlag, pubSource,
                Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient
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
        int goodsStock, string goodsUnit, decimal? shopPrice, int isIncludingTax,
        int pubType, string remarks, int shopId, int userId, string validity = "30天",
        string brand = "", string capacitance = "", string resistance = "", 
        string tolerance = "", string voltage = "", string dielectric = "", 
        string power = "", string tempCoefficient = "", int pubSource = 0)
    {
        try
        {
            DateTime validityDate = CalculateExpireTime(validity);
            
            string sql = @"INSERT INTO goods (goodsSn, [Name], Manufacturers, Packaging, goodsStock, goodsUnit, 
                          shopPrice, isIncludingTax, pubType, remarks, shopId, dataFlag, goodsStatus, isSale, 
                          createTime, updateTime, validityDate)
                          VALUES (@goodsSn, @Name, @Manufacturers, @Packaging, @goodsStock, @goodsUnit, 
                          @shopPrice, @isIncludingTax, @pubType, @remarks, @shopId, 1, 1, 1, 
                          GETDATE(), GETDATE(), @validityDate)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@goodsSn", goodsSn ?? (object)DBNull.Value),
                new SqlParameter("@Name", name ?? (object)DBNull.Value),
                new SqlParameter("@Manufacturers", manufacturers ?? (object)DBNull.Value),
                new SqlParameter("@Packaging", packaging ?? (object)DBNull.Value),
                new SqlParameter("@goodsStock", goodsStock),
                new SqlParameter("@goodsUnit", goodsUnit ?? "Kpcs"),
                new SqlParameter("@shopPrice", shopPrice.HasValue ? (object)shopPrice.Value : DBNull.Value),
                new SqlParameter("@isIncludingTax", isIncludingTax),
                new SqlParameter("@pubType", pubType),
                new SqlParameter("@remarks", remarks ?? (object)DBNull.Value),
                new SqlParameter("@shopId", shopId),
                new SqlParameter("@validityDate", validityDate)
            };

            int result = DbHelper.ExecuteNonQuery(sql, parameters);
            if (result > 0)
            {
                // 获取刚插入的记录ID
                int goodsId = 0;
                try
                {
                    string idSql = "SELECT TOP 1 goodsId FROM goods WHERE goodsSn = @goodsSn AND shopId = @shopId AND dataFlag = 1 ORDER BY createTime DESC";
                    object idResult = DbHelper.ExecuteScalar(idSql, 
                        DbHelper.CreateParameter("@goodsSn", goodsSn),
                        DbHelper.CreateParameter("@shopId", shopId));
                    if (idResult != null && idResult != DBNull.Value)
                    {
                        goodsId = Convert.ToInt32(idResult);
                        // 更新参数字段
                        UpdateGoodsParams(goodsId, brand, capacitance, resistance, tolerance, voltage, dielectric, power, tempCoefficient);
                    }
                }
                catch
                {
                    // 参数更新失败不影响主流程
                }
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            string logPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/InsertGoodsError.log");
            try
            {
                System.IO.File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - Error: " + ex.Message + " - goodsSn: " + goodsSn + " - shopId: " + shopId + Environment.NewLine);
            }
            catch
            {
            }
            return false;
        }
    }
    
    private void UpdateGoodsParams(int goodsId, string brand, string capacitance, string resistance, 
        string tolerance, string voltage, string dielectric, string power, string tempCoefficient)
    {
        try
        {
            string sql = "UPDATE goods SET ";
            System.Collections.Generic.List<string> setParts = new System.Collections.Generic.List<string>();
            System.Collections.Generic.List<SqlParameter> parameters = new System.Collections.Generic.List<SqlParameter>();
            
            if (!string.IsNullOrEmpty(brand))
            {
                setParts.Add("Brand = @Brand");
                parameters.Add(new SqlParameter("@Brand", brand));
            }
            if (!string.IsNullOrEmpty(capacitance))
            {
                setParts.Add("Capacitance = @Capacitance");
                parameters.Add(new SqlParameter("@Capacitance", capacitance));
            }
            if (!string.IsNullOrEmpty(resistance))
            {
                setParts.Add("Resistance = @Resistance");
                parameters.Add(new SqlParameter("@Resistance", resistance));
            }
            if (!string.IsNullOrEmpty(tolerance))
            {
                setParts.Add("Tolerance = @Tolerance");
                parameters.Add(new SqlParameter("@Tolerance", tolerance));
            }
            if (!string.IsNullOrEmpty(voltage))
            {
                setParts.Add("Voltage = @Voltage");
                parameters.Add(new SqlParameter("@Voltage", voltage));
            }
            if (!string.IsNullOrEmpty(dielectric))
            {
                setParts.Add("Dielectric = @Dielectric");
                parameters.Add(new SqlParameter("@Dielectric", dielectric));
            }
            if (!string.IsNullOrEmpty(power))
            {
                setParts.Add("Power = @Power");
                parameters.Add(new SqlParameter("@Power", power));
            }
            if (!string.IsNullOrEmpty(tempCoefficient))
            {
                setParts.Add("TempCoefficient = @TempCoefficient");
                parameters.Add(new SqlParameter("@TempCoefficient", tempCoefficient));
            }
            
            if (setParts.Count > 0)
            {
                sql += string.Join(", ", setParts);
                sql += " WHERE goodsId = @goodsId";
                parameters.Add(new SqlParameter("@goodsId", goodsId));
                DbHelper.ExecuteNonQuery(sql, parameters.ToArray());
            }
        }
        catch
        {
        }
    }
    
    private DateTime CalculateExpireTime(string validity)
    {
        DateTime now = DateTime.Now;
        switch (validity)
        {
            case "24小时":
                return now.AddHours(24);
            case "1天":
                return now.AddDays(1);
            case "3天":
                return now.AddDays(3);
            case "7天":
                return now.AddDays(7);
            case "15天":
                return now.AddDays(15);
            case "30天":
                return now.AddDays(30);
            case "长期":
                return now.AddYears(10);
            default:
                return now.AddDays(30);
        }
    }

    public DataTable GetDemandList(int pubType, int shopId)
    {
        try
        {
            string sql = @"SELECT TOP 50 
                goodsId, goodsSn, Name, Manufacturers, Packaging, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, createTime, validityDate, isSale, goodsStatus, dataFlag,
                Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient
                FROM goods 
                WHERE pubType = @pubType AND isSale = 1 AND goodsStatus = 1 AND dataFlag = 1 AND shopId = @shopId
                ORDER BY createTime DESC, validityDate ASC";

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
                goodsId, goodsSn, Name, Manufacturers, Packaging, goodsStock, goodsUnit, 
                shopPrice, isIncludingTax, updateTime, validityDate, isSale, goodsStatus, dataFlag, pubSource,
                Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient
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
        dt.Columns.Add("PubSource", typeof(int));

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
                if (diff.TotalDays < 0)
                {
                    newRow["Status"] = "已过期";
                    newRow["StatusClass"] = "gray";
                    newRow["RemainingTime"] = "已过期";
                }
                else if (diff.TotalDays < 1)
                {
                    newRow["Status"] = "即将到期";
                    newRow["StatusClass"] = "orange";
                    newRow["RemainingTime"] = "小于1天";
                }
                else
                {
                    newRow["Status"] = "采购中";
                    newRow["StatusClass"] = "blue";
                    newRow["RemainingTime"] = ((int)diff.TotalDays) + "天";
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

            string brandParams = BuildBrandParams(row);
            newRow["BrandParams"] = !string.IsNullOrEmpty(brandParams) ? brandParams : 
                (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限");

            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;
            
            int pubSource = 0;
            if (sourceTable.Columns.Contains("pubSource"))
            {
                pubSource = GetIntValue(row["pubSource"], 0);
            }
            newRow["PubSource"] = pubSource;

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

            string brandParams = BuildBrandParams(row);
            newRow["BrandParams"] = !string.IsNullOrEmpty(brandParams) ? brandParams : 
                (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限");

            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;
            newRow["OfflineTime"] = updateTime.ToString("yyyy-MM-dd HH:mm");
            
            int pubSource = 0;
            if (sourceTable.Columns.Contains("pubSource"))
            {
                pubSource = GetIntValue(row["pubSource"], 0);
            }
            newRow["PubSource"] = pubSource;

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
        dt.Columns.Add("PriceClass", typeof(string));
        dt.Columns.Add("ActionClass", typeof(string));
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
            DateTime validityDate = GetDateTimeValue(row["validityDate"], DateTime.Now.AddDays(3));
            int goodsId = GetIntValue(row["goodsId"], 0);
            int pubType = GetIntValue(row["pubType"], 1);
            int shopId = GetIntValue(row["shopId"], 0);
            string companyName = GetStringValue(row["companyName"]);

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

            string brandParams = BuildBrandParams(row);
            newRow["BrandParams"] = !string.IsNullOrEmpty(brandParams) ? brandParams : 
                (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限");

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

            // 根据剩余时间计算有效期显示
            TimeSpan diff = validityDate - DateTime.Now;
            if (diff.TotalDays < 0)
            {
                newRow["Validity"] = "已过期";
            }
            else if (diff.TotalDays < 1)
            {
                newRow["Validity"] = "小于1天";
            }
            else
            {
                int days = (int)diff.TotalDays;
                newRow["Validity"] = days + "天";
            }

            // 使用公司名称，优先使用 shopCompany，如果没有则使用 shopName
            if (!string.IsNullOrEmpty(companyName))
            {
                newRow["CompanyName"] = companyName;
            }
            else
            {
                newRow["CompanyName"] = "未知商家";
            }
            
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

            // PriceClass：需求且有价格显示"期望"，价格为0显示"面议"
            string priceClass = "";
            if (pubType == 2 && shopPrice > 0)
            {
                priceClass = "is-expected";
            }
            else if (shopPrice <= 0)
            {
                priceClass = "is-negotiable";
            }
            newRow["PriceClass"] = priceClass;

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private DataTable ConvertToInventoryData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("GoodsId", typeof(int));
        dt.Columns.Add("Status", typeof(string));
        dt.Columns.Add("StatusClass", typeof(string));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("IsTaxed", typeof(bool));
        dt.Columns.Add("RemainingTime", typeof(string));
        dt.Columns.Add("PubSource", typeof(int));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            newRow["GoodsId"] = GetIntValue(row["goodsId"], 0);
            
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
                if (diff.TotalDays < 0)
                {
                    newRow["Status"] = "已过期";
                    newRow["StatusClass"] = "gray";
                    newRow["RemainingTime"] = "已过期";
                }
                else if (diff.TotalDays < 1)
                {
                    newRow["Status"] = "即将到期";
                    newRow["StatusClass"] = "orange";
                    newRow["RemainingTime"] = "小于1天";
                }
                else
                {
                    newRow["Status"] = "供应";
                    newRow["StatusClass"] = "blue";
                    newRow["RemainingTime"] = ((int)diff.TotalDays) + "天";
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

            string brandParams = BuildBrandParams(row);
            newRow["BrandParams"] = !string.IsNullOrEmpty(brandParams) ? brandParams : 
                (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限");

            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;
            
            int pubSource = 0;
            if (sourceTable.Columns.Contains("pubSource"))
            {
                pubSource = GetIntValue(row["pubSource"], 0);
            }
            newRow["PubSource"] = pubSource;

            dt.Rows.Add(newRow);
        }

        return dt;
    }

    private DataTable ConvertToExpiredInventoryData(DataTable sourceTable)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("GoodsId", typeof(int));
        dt.Columns.Add("Model", typeof(string));
        dt.Columns.Add("BrandParams", typeof(string));
        dt.Columns.Add("Quantity", typeof(string));
        dt.Columns.Add("Unit", typeof(string));
        dt.Columns.Add("Price", typeof(string));
        dt.Columns.Add("IsTaxed", typeof(bool));
        dt.Columns.Add("OfflineTime", typeof(string));
        dt.Columns.Add("PubSource", typeof(int));

        foreach (DataRow row in sourceTable.Rows)
        {
            DataRow newRow = dt.NewRow();
            
            int goodsId = GetIntValue(row["goodsId"], 0);
            newRow["GoodsId"] = goodsId;
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

            string brandParams = BuildBrandParams(row);
            newRow["BrandParams"] = !string.IsNullOrEmpty(brandParams) ? brandParams : 
                (!string.IsNullOrEmpty(manufacturers) ? manufacturers : "品牌不限");

            newRow["Quantity"] = goodsStock > 0 ? goodsStock.ToString() : "0";
            newRow["Unit"] = !string.IsNullOrEmpty(goodsUnit) ? goodsUnit : "Kpcs";
            newRow["Price"] = shopPrice > 0 ? shopPrice.ToString("0.00") : "0.00";
            newRow["IsTaxed"] = isIncludingTax == 1;
            newRow["OfflineTime"] = updateTime.ToString("yyyy-MM-dd HH:mm");
            
            int pubSource = 0;
            if (sourceTable.Columns.Contains("pubSource"))
            {
                pubSource = GetIntValue(row["pubSource"], 0);
            }
            newRow["PubSource"] = pubSource;

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

    private string BuildBrandParams(DataRow row)
    {
        System.Collections.Generic.List<string> paramsList = new System.Collections.Generic.List<string>();
        
        DataColumnCollection cols = row.Table.Columns;
        
        if (cols.Contains("Brand") && row["Brand"] != DBNull.Value)
            paramsList.Add(row["Brand"].ToString());
        if (cols.Contains("Packaging") && row["Packaging"] != DBNull.Value)
            paramsList.Add(row["Packaging"].ToString());
        if (cols.Contains("Capacitance") && row["Capacitance"] != DBNull.Value)
            paramsList.Add(row["Capacitance"].ToString());
        if (cols.Contains("Resistance") && row["Resistance"] != DBNull.Value)
            paramsList.Add(row["Resistance"].ToString());
        if (cols.Contains("Tolerance") && row["Tolerance"] != DBNull.Value)
            paramsList.Add(row["Tolerance"].ToString());
        if (cols.Contains("Voltage") && row["Voltage"] != DBNull.Value)
            paramsList.Add(row["Voltage"].ToString());
        if (cols.Contains("Dielectric") && row["Dielectric"] != DBNull.Value)
            paramsList.Add(row["Dielectric"].ToString());
        if (cols.Contains("Power") && row["Power"] != DBNull.Value)
            paramsList.Add(row["Power"].ToString());
        if (cols.Contains("TempCoefficient") && row["TempCoefficient"] != DBNull.Value)
            paramsList.Add(row["TempCoefficient"].ToString());
        
        return paramsList.Count > 0 ? string.Join(" · ", paramsList) : "";
    }

    /// <summary>
    /// 发布采购需求
    /// </summary>
    public bool PublishDemand(string goodsSn, string name, string manufacturers, int quantity, string unit, decimal price, int isIncludingTax, int userId, int shopId, string validity = "30天",
        string brand = "", string capacitance = "", string resistance = "", 
        string tolerance = "", string voltage = "", string dielectric = "", 
        string power = "", string tempCoefficient = "")
    {
        try
        {
            EnsureGoodsFields();
            DateTime validityDate = CalculateExpireTime(validity);
            
            string sql = @"INSERT INTO goods (goodsSn, Name, Manufacturers, goodsStock, goodsUnit, shopPrice, isIncludingTax, pubType, isSale, goodsStatus, dataFlag, createTime, validityDate, shopId,
                Brand, Capacitance, Resistance, Tolerance, Voltage, Dielectric, Power, TempCoefficient)
                VALUES (@goodsSn, @name, @manufacturers, @quantity, @unit, @price, @isIncludingTax, 2, 1, 1, 1, GETDATE(), @validityDate, @shopId,
                @Brand, @Capacitance, @Resistance, @Tolerance, @Voltage, @Dielectric, @Power, @TempCoefficient)";

            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("@goodsSn", goodsSn ?? ""),
                new SqlParameter("@name", name ?? ""),
                new SqlParameter("@manufacturers", manufacturers ?? ""),
                new SqlParameter("@quantity", quantity),
                new SqlParameter("@unit", unit ?? "Kpcs"),
                new SqlParameter("@price", price),
                new SqlParameter("@isIncludingTax", isIncludingTax),
                new SqlParameter("@shopId", shopId),
                new SqlParameter("@validityDate", validityDate),
                new SqlParameter("@Brand", string.IsNullOrEmpty(brand) ? (object)DBNull.Value : brand),
                new SqlParameter("@Capacitance", string.IsNullOrEmpty(capacitance) ? (object)DBNull.Value : capacitance),
                new SqlParameter("@Resistance", string.IsNullOrEmpty(resistance) ? (object)DBNull.Value : resistance),
                new SqlParameter("@Tolerance", string.IsNullOrEmpty(tolerance) ? (object)DBNull.Value : tolerance),
                new SqlParameter("@Voltage", string.IsNullOrEmpty(voltage) ? (object)DBNull.Value : voltage),
                new SqlParameter("@Dielectric", string.IsNullOrEmpty(dielectric) ? (object)DBNull.Value : dielectric),
                new SqlParameter("@Power", string.IsNullOrEmpty(power) ? (object)DBNull.Value : power),
                new SqlParameter("@TempCoefficient", string.IsNullOrEmpty(tempCoefficient) ? (object)DBNull.Value : tempCoefficient)
            };

            int result = DbHelper.ExecuteNonQuery(sql, parameters);
            return result > 0;
        }
        catch (Exception ex)
        {
            try
            {
                DateTime validityDate = CalculateExpireTime(validity);
                string fallbackSql = @"INSERT INTO goods (goodsSn, Name, Manufacturers, goodsStock, goodsUnit, shopPrice, isIncludingTax, pubType, isSale, goodsStatus, dataFlag, createTime, validityDate, shopId)
                    VALUES (@goodsSn, @name, @manufacturers, @quantity, @unit, @price, @isIncludingTax, 2, 1, 1, 1, GETDATE(), @validityDate, @shopId)";

                SqlParameter[] fallbackParams = new SqlParameter[] {
                    new SqlParameter("@goodsSn", goodsSn ?? ""),
                    new SqlParameter("@name", name ?? ""),
                    new SqlParameter("@manufacturers", manufacturers ?? ""),
                    new SqlParameter("@quantity", quantity),
                    new SqlParameter("@unit", unit ?? "Kpcs"),
                    new SqlParameter("@price", price),
                    new SqlParameter("@isIncludingTax", isIncludingTax),
                    new SqlParameter("@shopId", shopId),
                    new SqlParameter("@validityDate", validityDate)
                };

                int result = DbHelper.ExecuteNonQuery(fallbackSql, fallbackParams);
                return result > 0;
            }
            catch (Exception ex2)
            {
                throw new Exception("PublishDemand failed: " + ex.Message + " | Fallback also failed: " + ex2.Message, ex2);
            }
        }
    }

    /// <summary>
    /// 更新需求信息（数量、单位、价格、税赋），不影响历史交互记录
    /// </summary>
    public bool UpdateDemand(int goodsId, int quantity, string unit, decimal price, int isIncludingTax)
    {
        try
        {
            string sql = @"UPDATE goods 
                SET goodsStock = @quantity, 
                    goodsUnit = @unit, 
                    shopPrice = @price, 
                    isIncludingTax = @isIncludingTax, 
                    updateTime = GETDATE() 
                WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, 
                DbHelper.CreateParameter("@goodsId", goodsId),
                DbHelper.CreateParameter("@quantity", quantity),
                DbHelper.CreateParameter("@unit", unit),
                DbHelper.CreateParameter("@price", price),
                DbHelper.CreateParameter("@isIncludingTax", isIncludingTax));
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 更新供应信息（数量、单位、价格、税赋），不影响历史交互记录
    /// </summary>
    public bool UpdateSupply(int goodsId, int quantity, string unit, decimal price, int isIncludingTax)
    {
        try
        {
            string sql = @"UPDATE goods 
                SET goodsStock = @quantity, 
                    goodsUnit = @unit, 
                    shopPrice = @price, 
                    isIncludingTax = @isIncludingTax, 
                    updateTime = GETDATE() 
                WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, 
                DbHelper.CreateParameter("@goodsId", goodsId),
                DbHelper.CreateParameter("@quantity", quantity),
                DbHelper.CreateParameter("@unit", unit),
                DbHelper.CreateParameter("@price", price),
                DbHelper.CreateParameter("@isIncludingTax", isIncludingTax));
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 下架商品（将 isSale 设置为 0）
    /// </summary>
    public bool TakeOff(int goodsId)
    {
        return TakeOff(goodsId, 0, 0);
    }

    /// <summary>
    /// 下架商品（带用户和店铺信息，用于防频繁操作检查）
    /// </summary>
    public bool TakeOff(int goodsId, int userId, int shopId, bool skipFrequentCheck = false)
    {
        try
        {
            if (!skipFrequentCheck && userId > 0)
            {
                string warning = CheckFrequentOperation(userId, goodsId, "takeoff");
                if (!string.IsNullOrEmpty(warning))
                {
                    return false;
                }
            }

            string sql = @"UPDATE goods SET isSale = 0, updateTime = GETDATE() WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
            
            if (result > 0 && userId > 0)
            {
                LogOperation(goodsId, userId, shopId, "takeoff", 1, 0);
            }
            
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 搜索商品（只返回上架中的有效信息）
    /// </summary>
    public DataTable SearchGoods(string keyword, int pageIndex, int pageSize, out int totalCount)
    {
        totalCount = 0;
        try
        {
            string whereSql = @"WHERE g.dataFlag = 1 AND g.goodsStatus = 1 AND g.isSale = 1";
            bool hasKeyword = !string.IsNullOrEmpty(keyword) && keyword.Trim().Length > 0;

            // 按空格拆分关键词，每个子关键词单独匹配
            var keywordGroups = new System.Collections.Generic.List<System.Collections.Generic.List<SearchVariant>>();
            if (hasKeyword)
            {
                string kw = keyword.Trim();
                // 按空格拆分多个关键词
                string[] parts = kw.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    var group = new System.Collections.Generic.List<SearchVariant>();
                    // 解析参数变体（先解析，确定类型）
                    var paramVariants = ExpandParamVariants(part);
                    if (paramVariants.Count > 0)
                    {
                        // 如果能识别为参数类型，原始关键词也使用对应类型
                        // 取第一个变体的类型作为原始关键词的类型
                        VariantType detectedType = paramVariants[0].Type;
                        group.Add(new SearchVariant(part, detectedType));
                        foreach (var v in paramVariants)
                        {
                            if (!group.Exists(x => x.Value.Equals(v.Value, StringComparison.OrdinalIgnoreCase)))
                            {
                                group.Add(v);
                            }
                        }
                    }
                    else
                    {
                        // 无法识别为参数，使用通用匹配
                        group.Add(new SearchVariant(part, VariantType.All));
                    }
                    if (group.Count > 0)
                    {
                        keywordGroups.Add(group);
                    }
                }
            }

            int paramIndex = 0;
            if (hasKeyword && keywordGroups.Count > 0)
            {
                // 构建 AND 条件：每个子关键词必须至少匹配一个变体
                System.Text.StringBuilder groupBuilder = new System.Text.StringBuilder();
                for (int g = 0; g < keywordGroups.Count; g++)
                {
                    var group = keywordGroups[g];
                    if (group.Count == 0) continue;
                    
                    if (groupBuilder.Length > 0) groupBuilder.Append(" AND ");
                    groupBuilder.Append("(");
                    
                    for (int i = 0; i < group.Count; i++)
                    {
                        var variant = group[i];
                        string fieldList = GetSearchFieldsForType(variant.Type);
                        if (string.IsNullOrEmpty(fieldList)) continue;
                        
                        if (i > 0) groupBuilder.Append(" OR ");
                        groupBuilder.Append("(");
                        var fields = fieldList.Split(',');
                        for (int j = 0; j < fields.Length; j++)
                        {
                            if (j > 0) groupBuilder.Append(" OR ");
                            groupBuilder.Append(fields[j].Trim()).Append(" LIKE @kw").Append(paramIndex);
                        }
                        groupBuilder.Append(")");
                        paramIndex++;
                    }
                    groupBuilder.Append(")");
                }
                if (groupBuilder.Length > 0)
                {
                    whereSql += " AND (" + groupBuilder.ToString() + ")";
                }
            }

            var countParams = new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>();
            if (hasKeyword)
            {
                paramIndex = 0;
                for (int g = 0; g < keywordGroups.Count; g++)
                {
                    var group = keywordGroups[g];
                    for (int i = 0; i < group.Count; i++)
                    {
                        countParams.Add(DbHelper.CreateParameter("@kw" + paramIndex, "%" + group[i].Value + "%"));
                        paramIndex++;
                    }
                }
            }

            string countSql = "SELECT COUNT(*) FROM goods g " + whereSql;
            object countObj = DbHelper.ExecuteScalar(countSql, countParams.ToArray());
            if (countObj != null && countObj != DBNull.Value)
            {
                totalCount = Convert.ToInt32(countObj);
            }

            if (totalCount == 0) return null;

            int startRow = (pageIndex - 1) * pageSize + 1;
            int endRow = pageIndex * pageSize;

            string sql = @"SELECT * FROM (
                SELECT ROW_NUMBER() OVER (ORDER BY g.createTime DESC) AS rowNum,
                    g.goodsId, g.goodsSn, g.[Name], g.Manufacturers, g.Packaging, g.goodsStock, g.goodsUnit, 
                    g.shopPrice, g.isIncludingTax, g.createTime, g.validityDate, g.isSale, 
                    g.goodsStatus, g.dataFlag, g.pubType, g.shopId,
                    g.Brand, g.Capacitance, g.Resistance, g.Tolerance, g.Voltage, g.Dielectric, g.Power, g.TempCoefficient,
                    ISNULL(s.shopCompany, s.shopName) AS companyName
                FROM goods g
                LEFT JOIN shops s ON g.shopId = s.shopId
                " + whereSql + @"
            ) AS t WHERE rowNum BETWEEN @startRow AND @endRow ORDER BY createTime DESC";

            var queryParams = new System.Collections.Generic.List<System.Data.SqlClient.SqlParameter>();
            if (hasKeyword)
            {
                paramIndex = 0;
                for (int g = 0; g < keywordGroups.Count; g++)
                {
                    var group = keywordGroups[g];
                    for (int i = 0; i < group.Count; i++)
                    {
                        queryParams.Add(DbHelper.CreateParameter("@kw" + paramIndex, "%" + group[i].Value + "%"));
                        paramIndex++;
                    }
                }
            }
            queryParams.Add(DbHelper.CreateParameter("@startRow", startRow));
            queryParams.Add(DbHelper.CreateParameter("@endRow", endRow));

            DataTable dt = DbHelper.ExecuteQuery(sql, queryParams.ToArray());
            
            if (dt != null && dt.Rows.Count > 0)
            {
                return ConvertToSupplyData(dt);
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// 根据变体类型获取适用的搜索字段
    /// </summary>
    private string GetSearchFieldsForType(VariantType type)
    {
        switch (type)
        {
            case VariantType.All:
                return "g.goodsSn, g.Name, g.Manufacturers, g.goodsDesc, g.Brand, g.Packaging";
            case VariantType.Capacitance:
                // 电容值可能在Capacitance字段，也可能在型号(goodsSn)中如104=100nF
                return "g.Capacitance, g.goodsSn, g.goodsDesc";
            case VariantType.Resistance:
                // 电阻值可能在Resistance字段，也可能在型号(goodsSn)中如104=100K
                return "g.Resistance, g.goodsSn, g.goodsDesc";
            case VariantType.Tolerance:
                // 精度可能在Tolerance字段，也可能在型号(goodsSn)中如K=±10%
                return "g.Tolerance, g.goodsSn, g.goodsDesc";
            case VariantType.Voltage:
                // 电压可能在Voltage字段，也可能在型号(goodsSn)中如1H=50V
                return "g.Voltage, g.goodsSn, g.goodsDesc";
            case VariantType.Dielectric:
                // 介质可能在Dielectric字段，也可能在型号(goodsSn)中如X7R/NPO
                return "g.Dielectric, g.TempCoefficient, g.goodsSn, g.goodsDesc";
            case VariantType.Packaging:
                return "g.Packaging, g.goodsSn, g.goodsDesc";
            case VariantType.Brand:
                return "g.Brand, g.Manufacturers";
            default:
                return "g.goodsSn, g.Name, g.Manufacturers, g.goodsDesc";
        }
    }

    /// <summary>
    /// 搜索变体类型
    /// </summary>
    private enum VariantType
    {
        All,        // 通用（匹配 goodsSn, Name, Manufacturers, goodsDesc）
        Capacitance,// 容值（匹配 Capacitance）
        Resistance, // 电阻值（匹配 Resistance）
        Tolerance,  // 精度（匹配 Tolerance）
        Voltage,    // 电压（匹配 Voltage）
        Dielectric, // 介质（匹配 Dielectric, TempCoefficient）
        Packaging,  // 封装（匹配 Packaging, goodsSn）
        Brand       // 品牌（匹配 Brand, Manufacturers）
    }

    /// <summary>
    /// 搜索变体
    /// </summary>
    private class SearchVariant
    {
        public string Value;
        public VariantType Type;
        public SearchVariant(string value, VariantType type) { Value = value; Type = type; }
    }

    /// <summary>
    /// 将元器件参数表示方式扩展为多种等价形式
    /// </summary>
    private System.Collections.Generic.List<SearchVariant> ExpandParamVariants(string input)
    {
        var result = new System.Collections.Generic.List<SearchVariant>();
        if (string.IsNullOrEmpty(input)) return result;

        string lower = input.ToLower().Trim();

        // 三位数字编码（电容）104 -> 0.1uF
        // 104 = 10 * 10^4 pF = 100000 pF = 100 nF = 0.1 uF
        var threeDigitMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d{3})$");
        if (threeDigitMatch.Success)
        {
            string num = threeDigitMatch.Groups[1].Value;
            int firstTwo = int.Parse(num.Substring(0, 2));
            int multiplier = int.Parse(num.Substring(2, 1));
            double pfValue = firstTwo * Math.Pow(10, multiplier);
            result.Add(new SearchVariant(num, VariantType.Capacitance));
            if (pfValue >= 1000000)
            {
                double ufValue = pfValue / 1000000;
                result.Add(new SearchVariant(TrimZero(ufValue) + "uf", VariantType.Capacitance));
                result.Add(new SearchVariant(TrimZero(ufValue) + "F", VariantType.Capacitance));
            }
            else if (pfValue >= 1000)
            {
                double nfValue = pfValue / 1000;
                result.Add(new SearchVariant(TrimZero(nfValue) + "nf", VariantType.Capacitance));
                result.Add(new SearchVariant(TrimZero(nfValue) + "nF", VariantType.Capacitance));
                result.Add(new SearchVariant(TrimZero(nfValue) + "n", VariantType.Capacitance));
            }
            else
            {
                result.Add(new SearchVariant(TrimZero(pfValue) + "pf", VariantType.Capacitance));
                result.Add(new SearchVariant(TrimZero(pfValue) + "pF", VariantType.Capacitance));
                result.Add(new SearchVariant(TrimZero(pfValue) + "p", VariantType.Capacitance));
            }
        }

        // 容值 100nf / 0.1uf / 100n / 0.1u / 100pf / 100p
        var capMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+\.?\d*)(pf|nf|uf|p|n|u)$");
        if (capMatch.Success)
        {
            string numStr = capMatch.Groups[1].Value;
            string unit = capMatch.Groups[2].Value;
            double numVal = double.Parse(numStr);
            // 添加原始写法（小写和大写）
            result.Add(new SearchVariant(numStr + unit, VariantType.Capacitance));
            string unitUpper = unit == "p" ? "pF" : (unit == "n" ? "nF" : (unit == "u" ? "uF" : unit));
            result.Add(new SearchVariant(numStr + unitUpper, VariantType.Capacitance));

            double pfValue = 0;
            if (unit == "pf" || unit == "p") pfValue = numVal;
            else if (unit == "nf" || unit == "n") pfValue = numVal * 1000;
            else if (unit == "uf" || unit == "u") pfValue = numVal * 1000000;

            if (pfValue > 0)
            {
                string threeDigit = ToThreeDigitCode(pfValue);
                if (!string.IsNullOrEmpty(threeDigit))
                {
                    result.Add(new SearchVariant(threeDigit, VariantType.Capacitance));
                }
                if (pfValue >= 1000000 && unit != "uf" && unit != "u")
                {
                    result.Add(new SearchVariant(TrimZero(pfValue / 1000000) + "uf", VariantType.Capacitance));
                    result.Add(new SearchVariant(TrimZero(pfValue / 1000000) + "uF", VariantType.Capacitance));
                }
                else if (pfValue >= 1000 && unit != "nf" && unit != "n")
                {
                    result.Add(new SearchVariant(TrimZero(pfValue / 1000) + "nf", VariantType.Capacitance));
                    result.Add(new SearchVariant(TrimZero(pfValue / 1000) + "nF", VariantType.Capacitance));
                }
                else if (pfValue < 1000 && unit != "pf" && unit != "p")
                {
                    result.Add(new SearchVariant(TrimZero(pfValue) + "pf", VariantType.Capacitance));
                    result.Add(new SearchVariant(TrimZero(pfValue) + "pF", VariantType.Capacitance));
                }
            }
        }

        // 电压: 50v / 50 v / 16V
        var voltageMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+\.?\d*)(v|kv|mv)$");
        if (voltageMatch.Success)
        {
            string numStr = voltageMatch.Groups[1].Value;
            string unit = voltageMatch.Groups[2].Value;
            result.Add(new SearchVariant(numStr + unit, VariantType.Voltage));
            result.Add(new SearchVariant(numStr + " " + unit, VariantType.Voltage));
            result.Add(new SearchVariant(numStr + unit.ToUpper(), VariantType.Voltage));
        }

        // 精度百分比: 5% / 10% / ±5% / ±10% / 1%
        var percentMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^[±]?(\d+\.?\d*)%$");
        if (percentMatch.Success)
        {
            string numStr = percentMatch.Groups[1].Value;
            // 添加各种精度写法
            result.Add(new SearchVariant("±" + numStr + "%", VariantType.Tolerance));
            result.Add(new SearchVariant(numStr + "%", VariantType.Tolerance));
            // 精度代号
            string code = TolerancePercentToCode(numStr);
            if (!string.IsNullOrEmpty(code))
            {
                result.Add(new SearchVariant(code, VariantType.Tolerance));
            }
        }

        // 精度代号: J(±5%), K(±10%), M(±20%), F(±1%), G(±2%)
        if (lower.Length == 1 && "fjgkdmc".IndexOf(lower[0]) >= 0)
        {
            string code = lower.ToUpper();
            string percent = ToleranceCodeToPercent(code);
            if (!string.IsNullOrEmpty(percent))
            {
                result.Add(new SearchVariant("±" + percent, VariantType.Tolerance));
                result.Add(new SearchVariant(percent + "%", VariantType.Tolerance));
            }
            result.Add(new SearchVariant(code, VariantType.Tolerance));
        }

        // 电阻: 104k / 100k / 4k7 / 4.7k
        var resistorCodeMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d{2,3})k$");
        if (resistorCodeMatch.Success)
        {
            string num = resistorCodeMatch.Groups[1].Value;
            if (num.Length == 3)
            {
                int firstTwo = int.Parse(num.Substring(0, 2));
                int multiplier = int.Parse(num.Substring(2, 1));
                double ohmValue = firstTwo * Math.Pow(10, multiplier);
                result.Add(new SearchVariant(num + "k", VariantType.Resistance));
                if (ohmValue >= 1000)
                {
                    result.Add(new SearchVariant(TrimZero(ohmValue / 1000) + "k", VariantType.Resistance));
                    result.Add(new SearchVariant(TrimZero(ohmValue / 1000) + "K", VariantType.Resistance));
                }
                result.Add(new SearchVariant(TrimZero(ohmValue) + "ohm", VariantType.Resistance));
                result.Add(new SearchVariant(TrimZero(ohmValue) + "Ω", VariantType.Resistance));
            }
        }

        // 4k7 表示法
        var resistorAltMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+)k(\d+)$");
        if (resistorAltMatch.Success)
        {
            string intPart = resistorAltMatch.Groups[1].Value;
            string decPart = resistorAltMatch.Groups[2].Value;
            result.Add(new SearchVariant(intPart + "." + decPart + "k", VariantType.Resistance));
            result.Add(new SearchVariant(intPart + "." + decPart + "K", VariantType.Resistance));
        }

        // 电阻值 100k / 1M / 4.7k / 10K
        var resistorValMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+\.?\d*)(k|m|ohm)$");
        if (resistorValMatch.Success)
        {
            string numStr = resistorValMatch.Groups[1].Value;
            string unit = resistorValMatch.Groups[2].Value;
            result.Add(new SearchVariant(numStr + unit, VariantType.Resistance));
            result.Add(new SearchVariant(numStr + " " + unit, VariantType.Resistance));
        }

        // 封装: 0603 / 0805 / 1206
        var packageMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(01005|0201|0402|0603|0805|1206|1210|1812|2010|2512)$");
        if (packageMatch.Success)
        {
            result.Add(new SearchVariant(lower, VariantType.Packaging));
        }

        // 介质: x7r / y5v / npo / c0g
        var dielectricMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(x7r|y5v|z5u|npo|c0g|np0|x5r|x6s|x7s|cog)$");
        if (dielectricMatch.Success)
        {
            result.Add(new SearchVariant(lower.ToUpper(), VariantType.Dielectric));
            // NPO 和 NP0 是同一个
            if (lower == "npo" || lower == "np0")
            {
                result.Add(new SearchVariant("C0G", VariantType.Dielectric));
                result.Add(new SearchVariant("COG", VariantType.Dielectric));
            }
            if (lower == "c0g" || lower == "cog")
            {
                result.Add(new SearchVariant("NPO", VariantType.Dielectric));
                result.Add(new SearchVariant("NP0", VariantType.Dielectric));
            }
        }

        return result;
    }

    /// <summary>
    /// 精度百分比转代号
    /// </summary>
    private string TolerancePercentToCode(string percent)
    {
        switch (percent)
        {
            case "0.1": return "B";
            case "0.25": return "C";
            case "0.5": return "D";
            case "1": return "F";
            case "2": return "G";
            case "5": return "J";
            case "10": return "K";
            case "20": return "M";
            default: return "";
        }
    }

    /// <summary>
    /// 精度代号转百分比
    /// </summary>
    private string ToleranceCodeToPercent(string code)
    {
        switch (code)
        {
            case "B": return "0.1";
            case "C": return "0.25";
            case "D": return "0.5";
            case "F": return "1";
            case "G": return "2";
            case "J": return "5";
            case "K": return "10";
            case "M": return "20";
            default: return "";
        }
    }

    private string TrimZero(double value)
    {
        if (value == Math.Floor(value))
        {
            return ((long)value).ToString();
        }
        return value.ToString("0.##");
    }

    private string ToThreeDigitCode(double pfValue)
    {
        if (pfValue <= 0) return "";
        // 将 pF 值转换为三位数编码
        // 例如 100000 pF = 10 * 10^4 -> 104
        // 例如 10000 pF = 10 * 10^3 -> 103
        // 例如 1000000 pF = 10 * 10^5 -> 105

        // 找到合适的 power，使得 mantissa 在 [1, 100) 之间
        int power = 0;
        double mantissa = pfValue;

        while (mantissa >= 100)
        {
            mantissa /= 10;
            power++;
        }
        while (mantissa < 1)
        {
            mantissa *= 10;
            power--;
        }

        // 现在 mantissa 在 [1, 100) 之间
        // 取前两位作为编码的前两位
        int firstTwo = (int)mantissa;
        string prefix = firstTwo.ToString("D2");

        return prefix + power.ToString();
    }

    /// <summary>
    /// 重新上架商品（将 isSale 设置为 1）
    /// </summary>
    public bool Restock(int goodsId)
    {
        return Restock(goodsId, 0, 0);
    }

    /// <summary>
    /// 重新上架商品（带用户和店铺信息，用于记录操作日志）
    /// </summary>
    public bool Restock(int goodsId, int userId, int shopId)
    {
        try
        {
            string sql = @"UPDATE goods SET isSale = 1, createTime = GETDATE(), updateTime = GETDATE() WHERE goodsId = @goodsId";
            int result = DbHelper.ExecuteNonQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
            
            if (result > 0 && userId > 0)
            {
                LogOperation(goodsId, userId, shopId, "restock", 0, 1);
            }
            
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 检查用户是否频繁操作（120秒内下架3次）
    /// </summary>
    public string CheckFrequentOperation(int userId, int goodsId, string operationType)
    {
        try
        {
            EnsureOperationLogTable();
            
            string sql = @"SELECT COUNT(*) FROM goods_operation_log 
                WHERE userId = @userId AND operationType = @operationType 
                AND createTime >= DATEADD(SECOND, -120, GETDATE())";
            
            object result = DbHelper.ExecuteScalar(sql,
                DbHelper.CreateParameter("@userId", userId),
                DbHelper.CreateParameter("@operationType", operationType));
            
            int count = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            if (count >= 3)
            {
                return "该操作已被系统记录，即将被添加黑名单！";
            }
            
            return "";
        }
        catch (Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// 记录商品操作日志
    /// </summary>
    private void LogOperation(int goodsId, int userId, int shopId, string operationType, int beforeStatus, int afterStatus)
    {
        try
        {
            EnsureOperationLogTable();
            
            string sql = @"INSERT INTO goods_operation_log (goodsId, userId, shopId, operationType, beforeStatus, afterStatus, createTime) 
                VALUES (@goodsId, @userId, @shopId, @operationType, @beforeStatus, @afterStatus, GETDATE())";
            
            DbHelper.ExecuteNonQuery(sql,
                DbHelper.CreateParameter("@goodsId", goodsId),
                DbHelper.CreateParameter("@userId", userId),
                DbHelper.CreateParameter("@shopId", shopId > 0 ? (object)shopId : DBNull.Value),
                DbHelper.CreateParameter("@operationType", operationType),
                DbHelper.CreateParameter("@beforeStatus", beforeStatus),
                DbHelper.CreateParameter("@afterStatus", afterStatus));
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 确保操作日志表存在（自动创建）
    /// </summary>
    private void EnsureOperationLogTable()
    {
        try
        {
            string checkSql = @"IF NOT EXISTS (SELECT * FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[goods_operation_log]') AND type IN ('U'))
                BEGIN
                    CREATE TABLE [dbo].[goods_operation_log] (
                        [logId] int IDENTITY(1,1) NOT NULL,
                        [goodsId] int NOT NULL,
                        [userId] int NOT NULL,
                        [shopId] int NULL,
                        [operationType] varchar(20) NOT NULL,
                        [beforeStatus] int NULL,
                        [afterStatus] int NULL,
                        [createTime] datetime NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT [PK_goods_operation_log] PRIMARY KEY CLUSTERED ([logId])
                    )
                    CREATE INDEX [IX_goods_operation_log_goodsId] ON [dbo].[goods_operation_log] ([goodsId])
                    CREATE INDEX [IX_goods_operation_log_userId] ON [dbo].[goods_operation_log] ([userId])
                    CREATE INDEX [IX_goods_operation_log_createTime] ON [dbo].[goods_operation_log] ([createTime])
                END";
            
            DbHelper.ExecuteNonQuery(checkSql);
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 辅助方法：检查列表中是否包含指定字符串（不区分大小写）
    /// </summary>
    private static bool ListContainsIgnoreCase(System.Collections.Generic.List<string> list, string value)
    {
        if (list == null || list.Count == 0 || string.IsNullOrEmpty(value))
            return false;
        foreach (string s in list)
        {
            if (string.Equals(s, value, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获取搜索关键词的所有参数变体（用于乱序查询）
    /// 输入如 "0603 100nf 50v"，返回所有等价表示的变体列表
    /// </summary>
    public static System.Collections.Generic.List<string> GetSearchKeywordVariants(string keyword)
    {
        var allVariants = new System.Collections.Generic.List<string>();
        if (string.IsNullOrEmpty(keyword) || keyword.Trim().Length == 0)
        {
            return allVariants;
        }

        string kw = keyword.Trim();
        string[] parts = kw.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        var tempService = new GoodsService();
        foreach (string part in parts)
        {
            var partVariants = new System.Collections.Generic.List<string>();
            partVariants.Add(part);

            var paramVariants = tempService.ExpandParamVariantsInternal(part);
            foreach (var v in paramVariants)
            {
                if (!ListContainsIgnoreCase(partVariants, v))
                {
                    partVariants.Add(v);
                }
            }

            allVariants.AddRange(partVariants);
        }

        var uniqueList = new System.Collections.Generic.List<string>();
        foreach (string v in allVariants)
        {
            if (!ListContainsIgnoreCase(uniqueList, v))
            {
                uniqueList.Add(v);
            }
        }

        return uniqueList;
    }

    /// <summary>
    /// 构建带参数变体的搜索SQL条件和参数
    /// 返回 Tuple(whereSql追加部分, 参数列表)
    /// </summary>
    public static System.Tuple<string, System.Collections.Generic.List<SqlParameter>> BuildParametricSearchCondition(
        string keyword, 
        string[] searchFields, 
        string paramPrefix)
    {
        var parameters = new System.Collections.Generic.List<SqlParameter>();
        string whereSql = "";

        if (string.IsNullOrEmpty(keyword) || keyword.Trim().Length == 0 || searchFields == null || searchFields.Length == 0)
        {
            return System.Tuple.Create(whereSql, parameters);
        }

        string kw = keyword.Trim();
        string[] parts = kw.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        var tempService = new GoodsService();
        var keywordGroups = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

        foreach (string part in parts)
        {
            var partVariants = new System.Collections.Generic.List<string>();
            partVariants.Add(part);

            var paramVariants = tempService.ExpandParamVariantsInternal(part);
            foreach (var v in paramVariants)
            {
                if (!ListContainsIgnoreCase(partVariants, v))
                {
                    partVariants.Add(v);
                }
            }

            if (partVariants.Count > 0)
            {
                keywordGroups.Add(partVariants);
            }
        }

        if (keywordGroups.Count == 0)
        {
            return System.Tuple.Create(whereSql, parameters);
        }

        int paramIndex = 0;
        System.Text.StringBuilder groupBuilder = new System.Text.StringBuilder();

        for (int g = 0; g < keywordGroups.Count; g++)
        {
            var group = keywordGroups[g];
            if (group.Count == 0) continue;

            if (groupBuilder.Length > 0) groupBuilder.Append(" AND ");
            groupBuilder.Append("(");

            for (int i = 0; i < group.Count; i++)
            {
                string variant = group[i];
                if (i > 0) groupBuilder.Append(" OR ");
                groupBuilder.Append("(");

                for (int j = 0; j < searchFields.Length; j++)
                {
                    if (j > 0) groupBuilder.Append(" OR ");
                    string paramName = "@" + paramPrefix + paramIndex;
                    groupBuilder.Append(searchFields[j].Trim()).Append(" LIKE ").Append(paramName);
                    parameters.Add(DbHelper.CreateParameter(paramName, "%" + variant + "%"));
                    paramIndex++;
                }

                groupBuilder.Append(")");
            }

            groupBuilder.Append(")");
        }

        if (groupBuilder.Length > 0)
        {
            whereSql = " AND (" + groupBuilder.ToString() + ")";
        }

        return System.Tuple.Create(whereSql, parameters);
    }

    /// <summary>
    /// 内部方法：扩展参数变体，返回所有变体值的字符串列表
    /// </summary>
    private System.Collections.Generic.List<string> ExpandParamVariantsInternal(string input)
    {
        var result = new System.Collections.Generic.List<string>();
        if (string.IsNullOrEmpty(input)) return result;

        string lower = input.ToLower().Trim();

        // 三位数字编码（电容）104 -> 0.1uF
        var threeDigitMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d{3})$");
        if (threeDigitMatch.Success)
        {
            string num = threeDigitMatch.Groups[1].Value;
            int firstTwo = int.Parse(num.Substring(0, 2));
            int multiplier = int.Parse(num.Substring(2, 1));
            double pfValue = firstTwo * Math.Pow(10, multiplier);
            result.Add(num);
            if (pfValue >= 1000000)
            {
                double ufValue = pfValue / 1000000;
                result.Add(TrimZero(ufValue) + "uf");
                result.Add(TrimZero(ufValue) + "uF");
                result.Add(TrimZero(ufValue) + "u");
            }
            else if (pfValue >= 1000)
            {
                double nfValue = pfValue / 1000;
                result.Add(TrimZero(nfValue) + "nf");
                result.Add(TrimZero(nfValue) + "nF");
                result.Add(TrimZero(nfValue) + "n");
            }
            else
            {
                result.Add(TrimZero(pfValue) + "pf");
                result.Add(TrimZero(pfValue) + "pF");
                result.Add(TrimZero(pfValue) + "p");
            }
        }

        // 容值 100nf / 0.1uf / 100n / 0.1u / 100pf / 100p
        var capMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+\.?\d*)(pf|nf|uf|p|n|u)$");
        if (capMatch.Success)
        {
            string numStr = capMatch.Groups[1].Value;
            string unit = capMatch.Groups[2].Value;
            double numVal = double.Parse(numStr);
            result.Add(numStr + unit);
            string unitUpper = unit == "p" ? "pF" : (unit == "n" ? "nF" : (unit == "u" ? "uF" : unit));
            result.Add(numStr + unitUpper);

            double pfValue = 0;
            if (unit == "pf" || unit == "p") pfValue = numVal;
            else if (unit == "nf" || unit == "n") pfValue = numVal * 1000;
            else if (unit == "uf" || unit == "u") pfValue = numVal * 1000000;

            if (pfValue > 0)
            {
                string threeDigit = ToThreeDigitCode(pfValue);
                if (!string.IsNullOrEmpty(threeDigit))
                {
                    result.Add(threeDigit);
                }
                if (pfValue >= 1000000 && unit != "uf" && unit != "u")
                {
                    result.Add(TrimZero(pfValue / 1000000) + "uf");
                    result.Add(TrimZero(pfValue / 1000000) + "uF");
                }
                else if (pfValue >= 1000 && unit != "nf" && unit != "n")
                {
                    result.Add(TrimZero(pfValue / 1000) + "nf");
                    result.Add(TrimZero(pfValue / 1000) + "nF");
                }
                else if (pfValue < 1000 && unit != "pf" && unit != "p")
                {
                    result.Add(TrimZero(pfValue) + "pf");
                    result.Add(TrimZero(pfValue) + "pF");
                }
            }
        }

        // 电压: 50v / 16V
        var voltageMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+\.?\d*)(v|kv|mv)$");
        if (voltageMatch.Success)
        {
            string numStr = voltageMatch.Groups[1].Value;
            string unit = voltageMatch.Groups[2].Value;
            result.Add(numStr + unit);
            result.Add(numStr + unit.ToUpper());
            result.Add(numStr + " " + unit);
            result.Add(numStr + " " + unit.ToUpper());
        }

        // 精度百分比: 5% / 10% / ±5% / ±10% / 1%
        var percentMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^[±]?(\d+\.?\d*)%$");
        if (percentMatch.Success)
        {
            string numStr = percentMatch.Groups[1].Value;
            result.Add("±" + numStr + "%");
            result.Add(numStr + "%");
            string code = TolerancePercentToCode(numStr);
            if (!string.IsNullOrEmpty(code))
            {
                result.Add(code);
            }
        }

        // 精度代号: J(±5%), K(±10%), M(±20%), F(±1%), G(±2%)
        if (lower.Length == 1 && "fjgkdmc".IndexOf(lower[0]) >= 0)
        {
            string code = lower.ToUpper();
            string percent = ToleranceCodeToPercent(code);
            if (!string.IsNullOrEmpty(percent))
            {
                result.Add("±" + percent + "%");
                result.Add(percent + "%");
            }
            result.Add(code);
        }

        // 电阻: 104k / 100k / 4k7 / 4.7k
        var resistorCodeMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d{2,3})k$");
        if (resistorCodeMatch.Success)
        {
            string num = resistorCodeMatch.Groups[1].Value;
            if (num.Length == 3)
            {
                int firstTwo = int.Parse(num.Substring(0, 2));
                int multiplier = int.Parse(num.Substring(2, 1));
                double ohmValue = firstTwo * Math.Pow(10, multiplier);
                result.Add(num + "k");
                result.Add(num + "K");
                if (ohmValue >= 1000)
                {
                    result.Add(TrimZero(ohmValue / 1000) + "k");
                    result.Add(TrimZero(ohmValue / 1000) + "K");
                }
                result.Add(TrimZero(ohmValue) + "ohm");
                result.Add(TrimZero(ohmValue) + "Ω");
            }
        }

        // 4k7 表示法
        var resistorAltMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+)k(\d+)$");
        if (resistorAltMatch.Success)
        {
            string intPart = resistorAltMatch.Groups[1].Value;
            string decPart = resistorAltMatch.Groups[2].Value;
            result.Add(intPart + "." + decPart + "k");
            result.Add(intPart + "." + decPart + "K");
        }

        // 电阻值 100k / 1M / 4.7k / 10K
        var resistorValMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(\d+\.?\d*)(k|m|ohm)$");
        if (resistorValMatch.Success)
        {
            string numStr = resistorValMatch.Groups[1].Value;
            string unit = resistorValMatch.Groups[2].Value;
            result.Add(numStr + unit);
            result.Add(numStr + unit.ToUpper());
            result.Add(numStr + " " + unit);
            result.Add(numStr + " " + unit.ToUpper());
        }

        // 封装: 0603 / 0805 / 1206
        var packageMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(01005|0201|0402|0603|0805|1206|1210|1812|2010|2512)$");
        if (packageMatch.Success)
        {
            result.Add(lower);
            result.Add(lower.ToUpper());
        }

        // 介质: x7r / y5v / npo / c0g
        var dielectricMatch = System.Text.RegularExpressions.Regex.Match(lower, @"^(x7r|y5v|z5u|npo|c0g|np0|x5r|x6s|x7s|cog)$");
        if (dielectricMatch.Success)
        {
            result.Add(lower.ToUpper());
            result.Add(lower.ToLower());
            if (lower == "npo" || lower == "np0")
            {
                result.Add("C0G");
                result.Add("COG");
                result.Add("c0g");
                result.Add("cog");
            }
            if (lower == "c0g" || lower == "cog")
            {
                result.Add("NPO");
                result.Add("NP0");
                result.Add("npo");
                result.Add("np0");
            }
        }

        return result;
    }
}