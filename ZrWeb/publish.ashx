<%@ WebHandler Language="C#" Class="PublishHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;

public class PublishHandler : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "application/json";
        
        try {
            string action = context.Request["action"];
            
            if (action == "publish_goods") {
                HandlePublishGoods(context);
            } else if (action == "submit_quote") {
                HandleSubmitQuote(context);
            } else if (action == "test_connection") {
                TestDbConnection(context);
            } else if (action == "query_goods") {
                QueryGoods(context);
            } else {
                context.Response.Write("{\"success\":false,\"message\":\"无效的操作\"}");
            }
        } catch (Exception ex) {
            context.Response.Write("{\"success\":false,\"message\":\"服务器错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }
    
    private void HandlePublishGoods(HttpContext context) {
        string goodsSn = context.Request["goodsSn"];
        string name = context.Request["name"];
        string manufacturers = context.Request["manufacturers"];
        
        // 收集参数字段
        string brand = context.Request["attr_品牌"] ?? "";
        string packaging = context.Request["attr_封装"] ?? "";
        string capacity = context.Request["attr_容值"] ?? "";
        string resistance = context.Request["attr_阻值"] ?? "";
        string precision = context.Request["attr_精度"] ?? "";
        string voltage = context.Request["attr_耐压"] ?? "";
        string power = context.Request["attr_功率"] ?? "";
        string medium = context.Request["attr_介质"] ?? "";
        string tcr = context.Request["attr_温漂"] ?? "";
        
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
        
        // 获取有效期
        string validity = context.Request["validity"] ?? "1个月";
        
        int goodsStock = 0;
        int.TryParse(context.Request["goodsStock"], out goodsStock);
        
        string goodsUnit = context.Request["goodsUnit"];
        if (string.IsNullOrEmpty(goodsUnit)) {
            goodsUnit = "Kpcs";
        }
        
        decimal shopPrice = 0;
        decimal.TryParse(context.Request["shopPrice"], out shopPrice);
        
        int isIncludingTax = 0;
        int.TryParse(context.Request["isIncludingTax"], out isIncludingTax);
        
        int pubType = 1;
        int.TryParse(context.Request["pubType"], out pubType);
        
        string remarks = context.Request["remarks"];

        if (context.Session == null) {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.Write("{\"success\":false,\"message\":\"会话超时，请重新登录\"}");
            context.Response.End();
            return;
        }

        int userId = UserHelper.GetUserId();
        if (userId == 0) {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.Write("{\"success\":false,\"message\":\"请先登录\"}");
            context.Response.End();
            return;
        }

        int roseId = GetRoseId(userId);
        if (pubType == 1 && roseId == 2) {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.Write("{\"success\":false,\"message\":\"采购商只能发布采购需求，无法发布供应信息\"}");
            context.Response.End();
            return;
        }

        int shopId = 0;
        // 优先使用前端传递的 shopId
        int.TryParse(context.Request["shopId"], out shopId);
        
        // 如果前端没有传递，从 Session 获取
        if (shopId == 0 && context.Session["ShopId"] != null) {
            int.TryParse(context.Session["ShopId"].ToString(), out shopId);
        }

        // 如果 Session 也没有，从数据库查询
        if (shopId == 0) {
            shopId = GetDefaultShopId(userId);
        }

        if (shopId == 0) {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.Write("{\"success\":false,\"message\":\"无法获取店铺信息，请完善店铺资料后重试\"}");
            context.Response.End();
            return;
        }

        if (string.IsNullOrEmpty(goodsSn)) {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.Write("{\"success\":false,\"message\":\"请输入型号\"}");
            context.Response.End();
            return;
        }

        GoodsService service = new GoodsService();
        bool success = service.InsertGoods(
            goodsSn, name, brandParams, packaging,
            goodsStock, goodsUnit, shopPrice, isIncludingTax,
            pubType, remarks, shopId, userId, validity);

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.Charset = "utf-8";
        
        if (success) {
            context.Response.Write("{\"success\":true,\"message\":\"发布成功\"}");
        } else {
            context.Response.Write("{\"success\":false,\"message\":\"数据库插入失败\"}");
        }
        context.Response.End();
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

        if (toShopId == 0) {
            context.Response.Write("{\"success\":false,\"message\":\"无法获取目标店铺信息\"}");
            return;
        }

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

        EnquiryQuoteService service = new EnquiryQuoteService();
        bool success = service.SubmitQuote(
            goodsId, goodsSn, fromQuantity, fromPrice,
            isIncludingTax, fromCompany, fromContact, fromTel,
            fromRemarks, toCompany, toUserId, fromUserId, brandName,
            fromShopId, toShopId, eqType);

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

    public bool IsReusable {
        get {
            return false;
        }
    }
}