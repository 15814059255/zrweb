using System;
using System.Data;
using System.Web;
using System.Text;

public partial class merchant_workbench : System.Web.UI.Page
{
    public string PageTitle = "我是商家 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,商家工作台,库存管理,电子元器件";
    public string PageDescription = "阻容网商家工作台，管理库存、查看询价、提交报价，一站式管理您的电子元器件供应业务。";
    
    public int OnlineSupplyCount = 3;
    public int InquiryCount = 7;
    public int NewInquiryCount = 7;
    public int QuoteCount = 3;
    public int ExpiredCount = 4;
    public int CurrentPage = 1;
    public int TotalPages = 49;
    public bool HasInventoryData = false;
    public string SearchKeyword = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        // 处理AJAX请求
        if (Request["action"] == "publish_goods")
        {
            if (Session != null && Session["IsPublishing"] != null && (bool)Session["IsPublishing"])
            {
                WriteJson("{\"success\":false,\"message\":\"正在发布中，请稍候\"}");
                return;
            }
            if (Session != null)
            {
                Session["IsPublishing"] = true;
            }
            HandlePublishGoods();
            if (Session != null)
            {
                Session["IsPublishing"] = false;
            }
            return;
        }

        if (Request["action"] == "take_off")
        {
            HandleTakeOff();
            return;
        }

        if (Request["action"] == "batch_take_off")
        {
            HandleBatchTakeOff();
            return;
        }

        if (Request["action"] == "batch_restock")
        {
            HandleBatchRestock();
            return;
        }

        if (Request["action"] == "restock")
        {
            HandleRestock();
            return;
        }

        if (Request["action"] == "update_supply")
        {
            HandleUpdateSupply();
            return;
        }

        if (Request["action"] == "batch_publish")
        {
            HandleBatchPublish();
            return;
        }

        LoadStats();
        BindInventory();
        BindExpiredInventory();
    }

    private void HandlePublishGoods()
    {
        Response.ContentType = "application/json";
        try
        {
            if (Session == null)
            {
                WriteJson("{\"success\":false,\"message\":\"会话超时，请重新登录\"}");
                return;
            }
            
            string goodsSn = Request["goodsSn"];
            string name = Request["name"];
            string manufacturers = Request["manufacturers"];
            
            int goodsStock = 0;
            int.TryParse(Request["goodsStock"], out goodsStock);
            
            string goodsUnit = Request["goodsUnit"];
            if (string.IsNullOrEmpty(goodsUnit)) goodsUnit = "Kpcs";
            
            decimal? shopPrice = null;
            string priceStr = Request["shopPrice"];
            if (!string.IsNullOrEmpty(priceStr)) {
                decimal price;
                if (decimal.TryParse(priceStr, out price)) {
                    shopPrice = price;
                }
            }
            
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);
            
            int pubType = 1;
            int.TryParse(Request["pubType"], out pubType);
            
            string validity = Request["validity"] ?? "30天";

            // 收集参数字段
            string brand = Request["attr_品牌"] ?? "";
            string packaging = Request["attr_封装"] ?? "";
            string capacity = Request["attr_容值"] ?? "";
            string resistance = Request["attr_阻值"] ?? "";
            string precision = Request["attr_精度"] ?? "";
            string voltage = Request["attr_耐压"] ?? "";
            string power = Request["attr_功率"] ?? "";
            string medium = Request["attr_介质"] ?? "";
            string tcr = Request["attr_温漂"] ?? "";

            // 组合品牌和参数信息
            string brandParams = "";
            if (!string.IsNullOrEmpty(brand))
            {
                brandParams = brand;
            }
            
            // 添加参数信息
            System.Collections.Generic.List<string> paramsList = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(packaging)) paramsList.Add(packaging);
            if (!string.IsNullOrEmpty(capacity)) paramsList.Add(capacity);
            if (!string.IsNullOrEmpty(resistance)) paramsList.Add(resistance);
            if (!string.IsNullOrEmpty(precision)) paramsList.Add(precision);
            if (!string.IsNullOrEmpty(voltage)) paramsList.Add(voltage);
            if (!string.IsNullOrEmpty(power)) paramsList.Add(power);
            if (!string.IsNullOrEmpty(medium)) paramsList.Add(medium);
            if (!string.IsNullOrEmpty(tcr)) paramsList.Add(tcr);
            
            if (paramsList.Count > 0)
            {
                if (!string.IsNullOrEmpty(brandParams))
                {
                    brandParams += " · " + string.Join(" · ", paramsList);
                }
                else
                {
                    brandParams = string.Join(" · ", paramsList);
                }
            }

            // 如果没有参数信息，使用原来的 manufacturers 字段
            if (string.IsNullOrEmpty(brandParams))
            {
                brandParams = manufacturers;
            }

            if (string.IsNullOrEmpty(goodsSn))
            {
                WriteJson("{\"success\":false,\"message\":\"请输入型号\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }

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
                        Session["ShopName"] = userCookie["ShopName"] ?? "";
                        Session["ShopCompany"] = userCookie["ShopCompany"] ?? "";
                    }
                }
            }

            if (shopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.InsertGoods(
                goodsSn, name, brandParams, packaging,
                goodsStock, goodsUnit, shopPrice, isIncludingTax,
                pubType, "", shopId, userId, validity,
                brand, capacity, resistance, precision, voltage, medium, power, tcr);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"发布成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"发布失败\"}");
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandleTakeOff()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            int shopId = 0;
            
            GoodsService service = new GoodsService();
            
            string warning = service.CheckFrequentOperation(userId, goodsId, "takeoff");
            if (!string.IsNullOrEmpty(warning))
            {
                WriteJson("{\"success\":false,\"message\":\"" + warning.Replace("\"", "\\\"") + "\"}");
                return;
            }
            
            bool success = service.TakeOff(goodsId, userId, shopId);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"下架成功\"}");
            }
            else
            {
                // 获取商品信息用于调试
                string debugSql = "SELECT goodsId, goodsSn, isSale, shopId FROM goods WHERE goodsId = @goodsId";
                var debugDt = DbHelper.ExecuteQuery(debugSql, DbHelper.CreateParameter("@goodsId", goodsId));
                string debugInfo = "";
                if (debugDt != null && debugDt.Rows.Count > 0)
                {
                    debugInfo = string.Format("goodsId={0}, goodsSn={1}, isSale={2}, shopId={3}",
                        debugDt.Rows[0]["goodsId"], debugDt.Rows[0]["goodsSn"],
                        debugDt.Rows[0]["isSale"], debugDt.Rows[0]["shopId"]);
                }
                WriteJson("{\"success\":false,\"message\":\"下架失败，请重试\",\"debug\":\"" + debugInfo + "\"}");
            }
        }
        catch (System.Threading.ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandleBatchTakeOff()
    {
        try
        {
            string goodsIdsStr = Request["goodsIds"] ?? "";
            if (string.IsNullOrEmpty(goodsIdsStr))
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID列表\"}");
                return;
            }

            string[] goodsIdArr = goodsIdsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (goodsIdArr.Length == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID列表\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            int shopId = 0;
            
            GoodsService service = new GoodsService();
            int successCount = 0;
            int failCount = 0;
            
            foreach (string goodsIdStr in goodsIdArr)
            {
                int goodsId = 0;
                if (int.TryParse(goodsIdStr.Trim(), out goodsId) && goodsId > 0)
                {
                    bool success = service.TakeOff(goodsId, userId, shopId, true); // 跳过频繁操作检查
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                else
                {
                    failCount++;
                }
            }

            WriteJson("{\"success\":true,\"message\":\"批量下架完成\",\"successCount\":" + successCount + ",\"failCount\":" + failCount + "}");
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandleBatchRestock()
    {
        try
        {
            string goodsIdsStr = Request["goodsIds"] ?? "";
            if (string.IsNullOrEmpty(goodsIdsStr))
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID列表\"}");
                return;
            }

            string[] goodsIdArr = goodsIdsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (goodsIdArr.Length == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID列表\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            int shopId = 0;
            
            GoodsService service = new GoodsService();
            int successCount = 0;
            int failCount = 0;
            
            foreach (string goodsIdStr in goodsIdArr)
            {
                int goodsId = 0;
                if (int.TryParse(goodsIdStr.Trim(), out goodsId) && goodsId > 0)
                {
                    bool success = service.Restock(goodsId, userId, shopId);
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                else
                {
                    failCount++;
                }
            }

            WriteJson("{\"success\":true,\"message\":\"批量上架完成\",\"successCount\":" + successCount + ",\"failCount\":" + failCount + "}");
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandleRestock()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            int shopId = 0;
            
            GoodsService service = new GoodsService();
            bool success = service.Restock(goodsId, userId, shopId);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"重新上架成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"重新上架失败\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void HandleUpdateSupply()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            string unit = Request["unit"] ?? "Kpcs";
            decimal price = 0;
            decimal.TryParse(Request["price"], out price);
            int isIncludingTax = 0;
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);

            if (goodsId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.UpdateSupply(goodsId, quantity, unit, price, isIncludingTax);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"修改成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"修改失败\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }
    
    private void HandleBatchPublish()
    {
        Response.ContentType = "application/json";
        try
        {
            if (Session == null)
            {
                WriteJson("{\"success\":false,\"message\":\"会话超时，请重新登录\"}");
                return;
            }
            
            string data = Request["data"];
            if (string.IsNullOrEmpty(data))
            {
                WriteJson("{\"success\":false,\"message\":\"请提供发布数据\"}");
                return;
            }
            
            int pubType = 1;
            int.TryParse(Request["pubType"], out pubType);
            
            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }
            
            int shopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out shopId);
            }
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
            
            if (shopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
                return;
            }
            
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            dynamic items = serializer.DeserializeObject(data);
            
            int successCount = 0;
            int failCount = 0;
            string lastError = "";
            GoodsService service = new GoodsService();
            
            foreach (var item in (System.Collections.IEnumerable)items)
            {
                try
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object>)item;
                    string goodsSn = "";
                    if (dict.ContainsKey("goodsSn") && dict["goodsSn"] != null)
                        goodsSn = dict["goodsSn"].ToString().Trim();
                    
                    string brand = "";
                    if (dict.ContainsKey("attr_品牌") && dict["attr_品牌"] != null)
                        brand = dict["attr_品牌"].ToString();
                    
                    string packaging = "";
                    if (dict.ContainsKey("attr_封装") && dict["attr_封装"] != null)
                        packaging = dict["attr_封装"].ToString();
                    
                    string capacitance = "";
                    if (dict.ContainsKey("attr_容值") && dict["attr_容值"] != null)
                        capacitance = dict["attr_容值"].ToString();
                    
                    string resistance = "";
                    if (dict.ContainsKey("attr_阻值") && dict["attr_阻值"] != null)
                        resistance = dict["attr_阻值"].ToString();
                    
                    string precision = "";
                    if (dict.ContainsKey("attr_精度") && dict["attr_精度"] != null)
                        precision = dict["attr_精度"].ToString();
                    
                    string voltage = "";
                    if (dict.ContainsKey("attr_耐压") && dict["attr_耐压"] != null)
                        voltage = dict["attr_耐压"].ToString();
                    
                    string power = "";
                    if (dict.ContainsKey("attr_功率") && dict["attr_功率"] != null)
                        power = dict["attr_功率"].ToString();
                    
                    string medium = "";
                    if (dict.ContainsKey("attr_介质") && dict["attr_介质"] != null)
                        medium = dict["attr_介质"].ToString();
                    
                    string tcr = "";
                    if (dict.ContainsKey("attr_温漂") && dict["attr_温漂"] != null)
                        tcr = dict["attr_温漂"].ToString();
                    
                    int goodsStock = 0;
                    if (dict.ContainsKey("goodsStock") && dict["goodsStock"] != null)
                    {
                        int.TryParse(dict["goodsStock"].ToString(), out goodsStock);
                    }
                    
                    string goodsUnit = "Kpcs";
                    if (dict.ContainsKey("goodsUnit") && dict["goodsUnit"] != null)
                        goodsUnit = dict["goodsUnit"].ToString();
                    if (string.IsNullOrEmpty(goodsUnit)) goodsUnit = "Kpcs";
                    
                    decimal? shopPrice = null;
                    if (dict.ContainsKey("shopPrice") && dict["shopPrice"] != null)
                    {
                        string priceStr = dict["shopPrice"].ToString();
                        if (!string.IsNullOrEmpty(priceStr))
                        {
                            decimal price;
                            if (decimal.TryParse(priceStr, out price))
                            {
                                shopPrice = price;
                            }
                        }
                    }
                    
                    int isIncludingTax = 0;
                    if (dict.ContainsKey("isIncludingTax"))
                    {
                        isIncludingTax = Convert.ToInt32(dict["isIncludingTax"]);
                    }
                    
                    if (string.IsNullOrEmpty(goodsSn)) continue;
                    
                    bool success = service.InsertGoods(
                        goodsSn, "", "", packaging,
                        goodsStock, goodsUnit, shopPrice, isIncludingTax,
                        pubType, "", shopId, userId, "3天",
                        brand, capacitance, resistance, precision, voltage, medium, power, tcr);
                    
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        lastError = "InsertGoods returned false for: " + goodsSn;
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    lastError = "Exception for item: " + ex.Message;
                }
            }
            
            if (successCount > 0)
            {
                WriteJson("{\"success\":true,\"message\":\"发布成功，共 " + successCount + " 条，失败 " + failCount + " 条\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"发布失败，全部 " + failCount + " 条均未成功。最后错误: " + lastError.Replace("\"", "\\\"") + "\"}");
            }
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void LoadStats()
    {
        try
        {
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

            GoodsService service = new GoodsService();
            OnlineSupplyCount = shopId > 0 ? service.GetOnlineSupplyCount(1, shopId) : 0;
            ExpiredCount = shopId > 0 ? service.GetExpiredCount(1, shopId) : 0;

            if (shopId > 0)
            {
                string sql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 1 AND dataFlag = 1";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@shopId", shopId));
                InquiryCount = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                string sqlNew = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 1 AND dataFlag = 1 AND readStatus = 0";
                object resultNew = DbHelper.ExecuteScalar(sqlNew, DbHelper.CreateParameter("@shopId", shopId));
                NewInquiryCount = resultNew != DBNull.Value ? Convert.ToInt32(resultNew) : 0;

                string sqlQuote = "SELECT COUNT(*) FROM enquiryquoteprice WHERE fromShopId = @shopId AND eqType = 2 AND dataFlag = 1";
                object resultQuote = DbHelper.ExecuteScalar(sqlQuote, DbHelper.CreateParameter("@shopId", shopId));
                QuoteCount = resultQuote != DBNull.Value ? Convert.ToInt32(resultQuote) : 0;
            }
        }
        catch
        {
            OnlineSupplyCount = 3;
            ExpiredCount = 4;
            InquiryCount = 0;
            NewInquiryCount = 0;
            QuoteCount = 0;
        }
    }

    private void BindInventory()
    {
        DataTable dt = null;
        try
        {
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

            SearchKeyword = Request.QueryString["keyword"] ?? "";

            GoodsService service = new GoodsService();
            dt = shopId > 0 ? service.GetInventoryList(1, shopId, SearchKeyword) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasInventoryData = false;
            dt = new DataTable();
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
        }
        else
        {
            HasInventoryData = true;
        }

        TotalPages = dt.Rows.Count > 0 ? (int)Math.Ceiling((double)dt.Rows.Count / 25) : 1;

        rptInventory.DataSource = dt;
        rptInventory.DataBind();
    }

    private void BindExpiredInventory()
    {
        DataTable dt = null;
        try
        {
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

            GoodsService service = new GoodsService();
            dt = shopId > 0 ? service.GetExpiredInventoryList(1, shopId) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            dt = new DataTable();
            dt.Columns.Add("GoodsId", typeof(int));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("IsTaxed", typeof(bool));
            dt.Columns.Add("OfflineTime", typeof(string));
        }

        rptExpiredInventory.DataSource = dt;
        rptExpiredInventory.DataBind();
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
    
    private void WriteJson(string json)
    {
        try {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/json";
            Response.Charset = "UTF-8";
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
        } finally {
            Response.SuppressContent = true;
            ApplicationInstance.CompleteRequest();
        }
    }
}