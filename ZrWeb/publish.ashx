<%@ WebHandler Language="C#" Class="PublishHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;

public class PublishHandler : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {
        try {
            Log("PublishHandler.ProcessRequest called");
            
            string action = context.Request["action"];
            Log("Action: " + (action ?? "null"));
            
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            
            if (action == "publish_goods") {
                HandlePublishGoods(context);
            } else if (action == "submit_quote") {
                HandleSubmitQuote(context);
            } else if (action == "test_connection") {
                TestDbConnection(context);
            } else if (action == "query_goods") {
                QueryGoods(context);
            } else if (action == "test_publish") {
                TestPublish(context);
            } else {
                context.Response.Write("{\"success\":false,\"message\":\"无效的操作\"}");
            }
        } catch (Exception ex) {
            Log("Exception in ProcessRequest: " + ex.Message);
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write("{\"success\":false,\"message\":\"服务器错误: " + ex.Message.Replace("\"", "\\\"").Replace("\r\n", " ") + "\"}");
        }
    }
    
    private void Log(string message) {
        try {
            string logPath = System.Web.HttpContext.Current.Server.MapPath("~/publish_log.txt");
            using (System.IO.StreamWriter writer = System.IO.File.AppendText(logPath)) {
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message);
            }
        } catch { }
    }
    
    private void HandlePublishGoods(HttpContext context) {
        try {
            Log("HandlePublishGoods started");
            
            string goodsSn = context.Request["goodsSn"];
            Log("goodsSn: " + (goodsSn ?? "null"));
            
            string name = context.Request["name"];
            string manufacturers = context.Request["manufacturers"];
            
            string brand = context.Request["attr_品牌"] ?? "";
            string packaging = context.Request["attr_封装"] ?? "";
            string capacity = context.Request["attr_容值"] ?? "";
            string resistance = context.Request["attr_阻值"] ?? "";
            string precision = context.Request["attr_精度"] ?? "";
            string voltage = context.Request["attr_耐压"] ?? "";
            string power = context.Request["attr_功率"] ?? "";
            string medium = context.Request["attr_介质"] ?? "";
            string tcr = context.Request["attr_温漂"] ?? "";
            
            string brandParams = "";
            if (!string.IsNullOrEmpty(brand))
            {
                brandParams = brand;
            }
            
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
            
            if (string.IsNullOrEmpty(brandParams))
            {
                brandParams = manufacturers;
            }
            
            string validity = context.Request["validity"] ?? "1个月";
            Log("validity: " + validity);
            
            int goodsStock = 0;
            int.TryParse(context.Request["goodsStock"], out goodsStock);
            Log("goodsStock: " + goodsStock);
            
            string goodsUnit = context.Request["goodsUnit"];
            if (string.IsNullOrEmpty(goodsUnit)) {
                goodsUnit = "Kpcs";
            }
            Log("goodsUnit: " + goodsUnit);
            
            decimal shopPrice = 0;
            decimal.TryParse(context.Request["shopPrice"], out shopPrice);
            Log("shopPrice: " + shopPrice);
            
            int isIncludingTax = 0;
            int.TryParse(context.Request["isIncludingTax"], out isIncludingTax);
            Log("isIncludingTax: " + isIncludingTax);
            
            int pubType = 1;
            int.TryParse(context.Request["pubType"], out pubType);
            Log("pubType: " + pubType);
            
            string remarks = context.Request["remarks"];

            Log("Session null check: " + (context.Session == null));
            if (context.Session == null) {
                Log("Session is null, returning error");
                context.Response.Write("{\"success\":false,\"message\":\"会话超时，请重新登录\"}");
                return;
            }

            int userId = UserHelper.GetUserId();
            Log("userId: " + userId);
            
            if (userId == 0) {
                Log("userId is 0, returning error");
                context.Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
                return;
            }

            int roseId = GetRoseId(userId);
            Log("roseId: " + roseId);
            
            if (pubType == 1 && roseId == 2) {
                context.Response.Write("{\"success\":false,\"message\":\"采购商只能发布采购需求，无法发布供应信息\"}");
                return;
            }

            int shopId = 0;
            int.TryParse(context.Request["shopId"], out shopId);
            Log("shopId from request: " + shopId);
            
            if (shopId == 0 && context.Session["ShopId"] != null) {
                int.TryParse(context.Session["ShopId"].ToString(), out shopId);
                Log("shopId from session: " + shopId);
            }

            if (shopId == 0) {
                shopId = GetDefaultShopId(userId);
                Log("shopId from db: " + shopId);
            }

            if (shopId == 0) {
                Log("shopId is 0, returning error");
                context.Response.Write("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
                return;
            }

            if (string.IsNullOrEmpty(goodsSn)) {
                context.Response.Write("{\"success\":false,\"message\":\"请输入型号\"}");
                return;
            }

            Log("Calling service method with pubType=" + pubType);
            GoodsService service = new GoodsService();
            bool success = false;
            
            if (pubType == 1) {
                success = service.InsertGoods(
                    goodsSn, name, brandParams, packaging,
                    goodsStock, goodsUnit, shopPrice, isIncludingTax,
                    pubType, remarks, shopId, userId, validity,
                    brand, capacity, resistance, precision, voltage, medium, power, tcr);
            } else {
                success = service.PublishDemand(
                    goodsSn, name, brandParams, goodsStock, goodsUnit, shopPrice, isIncludingTax, userId, shopId, validity,
                    brand, capacity, resistance, precision, voltage, medium, power, tcr);
            }
            
            Log("Service call result: " + success);
            
            if (success) {
                context.Response.Write("{\"success\":true,\"message\":\"发布成功\"}");
            } else {
                string debugInfo = "pubType=" + pubType + ", goodsSn=" + (goodsSn ?? "null") + ", shopId=" + shopId + ", userId=" + userId;
                context.Response.Write("{\"success\":false,\"message\":\"数据库插入失败\",\"debug\":\"" + debugInfo + "\"}");
            }
            Log("HandlePublishGoods completed");
        } catch (Exception ex) {
            Log("Exception in HandlePublishGoods: " + ex.Message);
            string errorMsg = ex.Message.Replace("\"", "'").Replace("\r\n", " ");
            context.Response.Write("{\"success\":false,\"message\":\"发布异常: " + errorMsg + "\"}");
        }
    }
    
    private int GetDefaultShopId(int userId) {
        try {
            string sql = "SELECT TOP 1 shopId FROM shops WHERE userId = @userId AND dataFlag = 1";
            object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@userId", userId));
            if (result != null && result != DBNull.Value) {
                return Convert.ToInt32(result);
            }
        }
        catch {
            // 忽略错误
        }
        return 0;
    }
    
    private int GetRoseId(int userId) {
        try {
            string sql = "SELECT RoseID FROM userinfo WHERE UserID = @userId";
            object result = DbHelper.ExecuteScalar(sql, DbHelper.CreateParameter("@userId", userId));
            if (result != null && result != DBNull.Value) {
                return Convert.ToInt32(result);
            }
        }
        catch {
            // 忽略错误
        }
        return 1;
    }
    
    private void HandleSubmitQuote(HttpContext context) {
        if (context.Session == null) {
            context.Response.Write("{\"success\":false,\"message\":\"会话超时，请重新登录\"}");
            return;
        }

        int goodsId = 0;
        int.TryParse(context.Request["goodsId"], out goodsId);
        
        string goodsSn = context.Request["goodsSn"];
        int fromQuantity = 0;
        int.TryParse(context.Request["quantity"], out fromQuantity);
        
        decimal fromPrice = 0;
        decimal.TryParse(context.Request["price"], out fromPrice);
        
        int isIncludingTax = 0;
        int.TryParse(context.Request["isIncludingTax"], out isIncludingTax);
        
        string brandName = context.Request["brandName"];
        string fromRemarks = context.Request["remarks"];
        string fromLot = context.Request["fromLot"];
        string validity = context.Request["validity"];
        
        int eqType = 2;
        int.TryParse(context.Request["eqType"], out eqType);
        if (eqType != 1 && eqType != 2) eqType = 2;

        int fromUserId = UserHelper.GetUserId();
        if (fromUserId == 0) {
            context.Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
            return;
        }

        int fromShopId = 0;
        if (context.Session["ShopId"] != null) {
            int.TryParse(context.Session["ShopId"].ToString(), out fromShopId);
        }

        if (fromShopId == 0) {
            fromShopId = GetDefaultShopId(fromUserId);
        }

        if (fromShopId == 0) {
            context.Response.Write("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
            return;
        }

        int toShopId = 0;
        int.TryParse(context.Request["toShopId"], out toShopId);

        string fromCompany = "";
        string fromContact = "";
        string fromTel = "";
        
        if (context.Session["ShopCompany"] != null) {
            fromCompany = context.Session["ShopCompany"].ToString();
        }
        if (string.IsNullOrEmpty(fromCompany) && context.Session["ShopName"] != null) {
            fromCompany = context.Session["ShopName"].ToString();
        }
        if (string.IsNullOrEmpty(fromCompany)) {
            fromCompany = "匿名用户";
        }
        
        if (context.Session["LinkMan"] != null) {
            fromContact = context.Session["LinkMan"].ToString();
        }
        if (context.Session["MobilePhone"] != null) {
            fromTel = context.Session["MobilePhone"].ToString();
        }

        string toCompany = "";
        int toUserId = 0;
        try {
            string shopSql = "SELECT s.shopCompany, s.shopName, s.userId FROM shops s WHERE s.shopId = @shopId";
            DataTable shopDt = DbHelper.ExecuteQuery(shopSql, DbHelper.CreateParameter("@shopId", toShopId));
            if (shopDt != null && shopDt.Rows.Count > 0) {
                toCompany = shopDt.Rows[0]["shopCompany"] != DBNull.Value ? shopDt.Rows[0]["shopCompany"].ToString() : "";
                if (string.IsNullOrEmpty(toCompany) && shopDt.Rows[0]["shopName"] != DBNull.Value) {
                    toCompany = shopDt.Rows[0]["shopName"].ToString();
                }
                if (shopDt.Rows[0]["userId"] != DBNull.Value) {
                    int.TryParse(shopDt.Rows[0]["userId"].ToString(), out toUserId);
                }
            }
        } catch {
        }

        string fromManufacturers = "";
        string fromPackaging = "";
        string fromGoodsDesc = "";
        try {
            string goodsSql = "SELECT Manufacturers, Packaging, goodsDesc FROM goods WHERE goodsId = @goodsId";
            DataTable goodsDt = DbHelper.ExecuteQuery(goodsSql, DbHelper.CreateParameter("@goodsId", goodsId));
            if (goodsDt != null && goodsDt.Rows.Count > 0) {
                fromManufacturers = goodsDt.Rows[0]["Manufacturers"] != DBNull.Value ? goodsDt.Rows[0]["Manufacturers"].ToString() : "";
                fromPackaging = goodsDt.Rows[0]["Packaging"] != DBNull.Value ? goodsDt.Rows[0]["Packaging"].ToString() : "";
                fromGoodsDesc = goodsDt.Rows[0]["goodsDesc"] != DBNull.Value ? goodsDt.Rows[0]["goodsDesc"].ToString() : "";
            }
        } catch {
        }

        int sourceEqId = 0;
        int.TryParse(context.Request["sourceEqId"], out sourceEqId);

        EnquiryQuoteService service = new EnquiryQuoteService();
        bool success = false;
        try {
            success = service.SubmitQuote(
                goodsId, goodsSn, fromQuantity, fromPrice,
                isIncludingTax, fromCompany, fromContact, fromTel,
                fromRemarks, toCompany, toUserId, fromUserId, brandName,
                fromShopId, toShopId, eqType, sourceEqId, fromLot,
                fromManufacturers, fromPackaging, fromGoodsDesc, validity);
        } catch (Exception ex) {
            context.Response.Write("{\"success\":false,\"message\":\"提交异常: " + ex.Message.Replace("\"", "\\\"").Replace("\r\n", " ") + "\"}");
            return;
        }

        if (success) {
            context.Response.Write("{\"success\":true,\"message\":\"提交成功\"}");
        } else {
            context.Response.Write("{\"success\":false,\"message\":\"提交失败\"}");
        }
    }
    
    private void TestDbConnection(HttpContext context) {
        try {
            using (var conn = DbHelper.GetConnection()) {
                conn.Open();
                context.Response.Write("{\"success\":true,\"message\":\"数据库连接成功\"}");
            }
        } catch (Exception ex) {
            context.Response.Write("{\"success\":false,\"message\":\"数据库连接失败: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }
    
    private void QueryGoods(HttpContext context) {
        try {
            string sql = "SELECT TOP 10 goodsId, goodsSn, [Name], pubType, shopPrice, createTime FROM goods ORDER BY createTime DESC";
            var dt = DbHelper.ExecuteQuery(sql);
            
            string json = "{\"success\":true,\"count\":" + dt.Rows.Count + ",\"data\":[";
            for (int i = 0; i < dt.Rows.Count; i++) {
                var row = dt.Rows[i];
                json += "{";
                json += "\"goodsId\":" + row["goodsId"] + ",";
                json += "\"goodsSn\":\"" + (row["goodsSn"] != DBNull.Value ? row["goodsSn"].ToString().Replace("\"", "\\\"") : "") + "\",";
                json += "\"name\":\"" + (row["Name"] != DBNull.Value ? row["Name"].ToString().Replace("\"", "\\\"") : "") + "\",";
                json += "\"pubType\":" + row["pubType"] + ",";
                json += "\"shopPrice\":" + row["shopPrice"] + ",";
                json += "\"createTime\":\"" + Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                json += "}";
                if (i < dt.Rows.Count - 1) json += ",";
            }
            json += "]}";
            context.Response.Write(json);
        } catch (Exception ex) {
            context.Response.Write("{\"success\":false,\"message\":\"查询失败: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }
    
    private void TestPublish(HttpContext context) {
        try {
            Log("TestPublish called");
            
            int userId = UserHelper.GetUserId();
            Log("Test userId: " + userId);
            
            int shopId = 0;
            if (context.Session != null && context.Session["ShopId"] != null) {
                int.TryParse(context.Session["ShopId"].ToString(), out shopId);
            }
            if (shopId == 0 && userId > 0) {
                shopId = GetDefaultShopId(userId);
            }
            Log("Test shopId: " + shopId);
            
            GoodsService service = new GoodsService();
            bool success = service.PublishDemand(
                "TEST-MODEL-X", "测试需求", "测试品牌", 1000, "Kpcs", 1.50m, 0, userId, shopId, "1个月",
                "测试品牌", "", "", "", "", "", "", "");
            
            Log("Test publish result: " + success);
            
            if (success) {
                context.Response.Write("{\"success\":true,\"message\":\"测试发布成功\"}");
            } else {
                context.Response.Write("{\"success\":false,\"message\":\"测试发布失败\"}");
            }
        } catch (Exception ex) {
            Log("TestPublish exception: " + ex.Message);
            context.Response.Write("{\"success\":false,\"message\":\"测试异常: " + ex.Message.Replace("\"", "'").Replace("\r\n", " ") + "\"}");
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}