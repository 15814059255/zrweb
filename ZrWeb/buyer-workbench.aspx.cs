using System;
using System.Data;
using System.Threading;
using System.Web;
using System.Text;

public partial class buyer_workbench : System.Web.UI.Page
{
    public string PageTitle = "我是采购 - 电子元器件 B2B 平台";
    public string PageKeywords = "阻容网,采购工作台,需求管理,电子元器件采购";
    public string PageDescription = "阻容网采购工作台，管理采购需求、查看供应商报价、跟踪询价进度，一站式采购电子元器件。";
    
    public int OnlineDemandCount = 0;
    public int QuoteCount = 0;
    public int NewQuoteCount = 0;
    public int InquiryCount = 0;
    public int NewInquiryCount = 0;
    public int ExpiredCount = 0;
    public int CurrentPage = 1;
    public int TotalPages = 1;
    public bool HasDemandData = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "POST")
        {
            string action = Request["action"];
            
            if (action == "publish_demand")
            {
                HandlePublishDemand();
                return;
            }
            else if (action == "take_off")
            {
                HandleTakeOff();
                return;
            }
            else if (action == "restock")
            {
                HandleRestock();
                return;
            }
            else if (action == "batch_restock")
            {
                HandleBatchRestock();
                return;
            }
            else if (action == "batch_take_off")
            {
                HandleBatchTakeOff();
                return;
            }
            else if (action == "batch_publish")
            {
                HandleBatchPublish();
                return;
            }
            else if (action == "update_demand")
            {
                HandleUpdateDemand();
                return;
            }
            else if (action == "update_goods_attr")
            {
                HandleUpdateGoodsAttr();
                return;
            }
        }

        if (!IsPostBack)
        {
            LoadStats();
            BindDemand();
            BindExpiredDemand();
        }
    }

    private void HandlePublishDemand()
    {
        try
        {
            string goodsSn = Request["goodsSn"] ?? "";
            string name = Request["name"] ?? "";
            string manufacturers = Request["manufacturers"] ?? "";
            int quantity = 0;
            string unit = Request["unit"] ?? "Kpcs";
            decimal price = 0;
            int isIncludingTax = 0;
            string validity = Request["validity"] ?? "30天";
            string batchNo = Request["batchNo"] ?? "2年内";

            int.TryParse(Request["quantity"], out quantity);
            decimal.TryParse(Request["price"], out price);
            int.TryParse(Request["isIncludingTax"], out isIncludingTax);

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
                    }
                }
            }

            if (shopId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.PublishDemand(goodsSn, name, brandParams, quantity, unit, price, isIncludingTax, userId, shopId, validity,
                brand, capacity, resistance, precision, voltage, medium, power, tcr, batchNo);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"发布成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"发布失败\"}");
            }
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"发布异常:" + ex.Message.Replace("\"", "'") + "\"}");
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
                    
                    decimal shopPrice = 0;
                    if (dict.ContainsKey("shopPrice") && dict["shopPrice"] != null)
                    {
                        decimal.TryParse(dict["shopPrice"].ToString(), out shopPrice);
                    }
                    
                    int isIncludingTax = 0;
                    if (dict.ContainsKey("isIncludingTax"))
                    {
                        isIncludingTax = Convert.ToInt32(dict["isIncludingTax"]);
                    }
                    
                    if (string.IsNullOrEmpty(goodsSn)) continue;
                    
                    bool success = service.PublishDemand(
                        goodsSn, "", "", goodsStock, goodsUnit, shopPrice, isIncludingTax,
                        userId, shopId, "3天",
                        brand, capacitance, resistance, precision, voltage, medium, power, tcr, "2年内");
                    
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    lastError = ex.Message;
                }
            }
            
            if (successCount > 0)
            {
                WriteJson("{\"success\":true,\"message\":\"成功发布 " + successCount + " 条需求" + (failCount > 0 ? "，失败 " + failCount + " 条" : "") + "\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"发布失败\"}");
            }
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"批量发布异常:" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }

    private void HandleTakeOff()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId <= 0)
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
                WriteJson("{\"success\":false,\"message\":\"" + warning.Replace("\"", "'") + "\"}");
                return;
            }
            
            bool success = service.TakeOff(goodsId, userId, shopId);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"下架成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"下架失败\"}");
            }
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"下架异常:" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }

    private void HandleRestock()
    {
        try
        {
            int goodsId = 0;
            int.TryParse(Request["goodsId"], out goodsId);

            if (goodsId <= 0)
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
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"重新上架异常:" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }

    private void HandleBatchRestock()
    {
        try
        {
            string goodsIdsStr = Request["goodsIds"] ?? "";
            if (string.IsNullOrEmpty(goodsIdsStr))
            {
                WriteJson("{\"success\":false,\"message\":\"请选择要上架的商品\"}");
                return;
            }

            string[] goodsIdArr = goodsIdsStr.Split(',');
            int successCount = 0;
            int failCount = 0;
            GoodsService service = new GoodsService();

            foreach (string idStr in goodsIdArr)
            {
                int goodsId = 0;
                if (int.TryParse(idStr, out goodsId) && goodsId > 0)
                {
                    if (service.Restock(goodsId))
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

            WriteJson("{\"success\":true,\"message\":\"批量重新上架完成，成功" + successCount + "条，失败" + failCount + "条\"}");
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"批量重新上架异常:" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }

    private void HandleBatchTakeOff()
    {
        try
        {
            string goodsIdsStr = Request["goodsIds"] ?? "";
            if (string.IsNullOrEmpty(goodsIdsStr))
            {
                WriteJson("{\"success\":false,\"message\":\"请选择要下架的需求\"}");
                return;
            }

            string[] goodsIdArr = goodsIdsStr.Split(',');
            int successCount = 0;
            int failCount = 0;
            GoodsService service = new GoodsService();

            foreach (string idStr in goodsIdArr)
            {
                int goodsId = 0;
                if (int.TryParse(idStr, out goodsId) && goodsId > 0)
                {
                    if (service.TakeOff(goodsId, 0, 0, true))
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

            WriteJson("{\"success\":true,\"message\":\"批量下架完成，成功" + successCount + "条，失败" + failCount + "条\"}");
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"批量下架异常:" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }

    private void HandleUpdateDemand()
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
            string batchNo = Request["batchNo"] ?? "2年内";

            if (goodsId <= 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            GoodsService service = new GoodsService();
            bool success = service.UpdateDemand(goodsId, quantity, unit, price, isIncludingTax, batchNo);

            if (success)
            {
                WriteJson("{\"success\":true,\"message\":\"修改成功\"}");
            }
            else
            {
                WriteJson("{\"success\":false,\"message\":\"修改失败\"}");
            }
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"修改异常:" + ex.Message.Replace("\"", "'") + "\"}");
        }
    }

    private void HandleUpdateGoodsAttr()
    {
        try
        {
            string goodsIdStr = Request["goodsId"] ?? "";
            int goodsId = 0;
            if (!int.TryParse(goodsIdStr, out goodsId) || goodsId <= 0)
            {
                WriteJson("{\"success\":false,\"message\":\"无效的商品ID\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            if (userId == 0)
            {
                WriteJson("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }

            string brand = Request["brand"] ?? "";
            string packaging = Request["packaging"] ?? "";
            string capacitance = Request["capacitance"] ?? "";
            string resistance = Request["resistance"] ?? "";
            string tolerance = Request["tolerance"] ?? "";
            string voltage = Request["voltage"] ?? "";
            string dielectric = Request["dielectric"] ?? "";
            string power = Request["power"] ?? "";
            string tempCoefficient = Request["tempCoefficient"] ?? "";

            GoodsService service = new GoodsService();
            service.UpdateGoodsParams(goodsId, brand, capacitance, resistance, tolerance, voltage, dielectric, power, tempCoefficient);

            if (!string.IsNullOrEmpty(packaging))
            {
                string sql = "UPDATE goods SET Packaging = @Packaging WHERE goodsId = @goodsId";
                DbHelper.ExecuteNonQuery(sql,
                    DbHelper.CreateParameter("@Packaging", packaging),
                    DbHelper.CreateParameter("@goodsId", goodsId));
            }

            WriteJson("{\"success\":true,\"message\":\"保存成功\"}");
        }
        catch (Exception ex)
        {
            WriteJson("{\"success\":false,\"message\":\"保存失败: " + ex.Message.Replace("\"", "'") + "\"}");
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
            OnlineDemandCount = shopId > 0 ? service.GetOnlineSupplyCount(2, shopId) : 0;
            ExpiredCount = shopId > 0 ? service.GetExpiredCount(2, shopId) : 0;

            if (shopId > 0)
            {
                // 查询采购方收到的报价（商家回复的报价）
                string sql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 2 AND dataFlag = 1";
                object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@shopId", shopId));
                QuoteCount = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                string sqlNew = "SELECT COUNT(*) FROM enquiryquoteprice WHERE toShopId = @shopId AND eqType = 2 AND dataFlag = 1 AND readStatus = 0";
                object resultNew = DbHelper.ExecuteScalar(sqlNew, DbHelper.CreateParameter("@shopId", shopId));
                NewQuoteCount = resultNew != DBNull.Value ? Convert.ToInt32(resultNew) : 0;

                // 查询采购方发出的询价（等待商家回复）
                string sqlInquiry = "SELECT COUNT(*) FROM enquiryquoteprice WHERE fromShopID = @shopId AND eqType = 1 AND dataFlag = 1";
                object resultInquiry = DbHelper.ExecuteScalar(sqlInquiry, DbHelper.CreateParameter("@shopId", shopId));
                InquiryCount = resultInquiry != DBNull.Value ? Convert.ToInt32(resultInquiry) : 0;

                string sqlNewInquiry = "SELECT COUNT(*) FROM enquiryquoteprice WHERE fromShopID = @shopId AND eqType = 1 AND dataFlag = 1 AND readStatus = 0";
                object resultNewInquiry = DbHelper.ExecuteScalar(sqlNewInquiry, DbHelper.CreateParameter("@shopId", shopId));
                NewInquiryCount = resultNewInquiry != DBNull.Value ? Convert.ToInt32(resultNewInquiry) : 0;
            }
        }
        catch
        {
            OnlineDemandCount = 3;
            ExpiredCount = 4;
            QuoteCount = 0;
            NewQuoteCount = 0;
        }
    }

    private void BindDemand()
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
                        Session["ShopName"] = userCookie["ShopName"] ?? "";
                        Session["ShopCompany"] = userCookie["ShopCompany"] ?? "";
                    }
                }
            }

            GoodsService service = new GoodsService();
            dt = shopId > 0 ? service.GetDemandList(2, shopId) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            HasDemandData = false;
            dt = new DataTable();
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
            dt.Columns.Add("Brand", typeof(string));
            dt.Columns.Add("Packaging", typeof(string));
            dt.Columns.Add("Capacitance", typeof(string));
            dt.Columns.Add("Resistance", typeof(string));
            dt.Columns.Add("Tolerance", typeof(string));
            dt.Columns.Add("Voltage", typeof(string));
            dt.Columns.Add("Dielectric", typeof(string));
            dt.Columns.Add("Power", typeof(string));
            dt.Columns.Add("TempCoefficient", typeof(string));
            dt.Columns.Add("BatchNo", typeof(string));
        }
        else
        {
            HasDemandData = true;
        }

        TotalPages = dt.Rows.Count > 0 ? (int)Math.Ceiling((double)dt.Rows.Count / 25) : 1;

        rptDemand.DataSource = dt;
        rptDemand.DataBind();
    }

    private void BindExpiredDemand()
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
            dt = shopId > 0 ? service.GetExpiredDemandList(2, shopId) : null;
        }
        catch
        {
            dt = null;
        }

        if (dt == null || dt.Rows.Count == 0)
        {
            dt = new DataTable();
            dt.Columns.Add("goodsId", typeof(int));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("BrandParams", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("IsTaxed", typeof(bool));
            dt.Columns.Add("OfflineTime", typeof(string));
            dt.Columns.Add("Brand", typeof(string));
            dt.Columns.Add("Packaging", typeof(string));
            dt.Columns.Add("Capacitance", typeof(string));
            dt.Columns.Add("Resistance", typeof(string));
            dt.Columns.Add("Tolerance", typeof(string));
            dt.Columns.Add("Voltage", typeof(string));
            dt.Columns.Add("Dielectric", typeof(string));
            dt.Columns.Add("Power", typeof(string));
            dt.Columns.Add("TempCoefficient", typeof(string));
            dt.Columns.Add("BatchNo", typeof(string));
        }

        rptExpiredDemand.DataSource = dt;
        rptExpiredDemand.DataBind();
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