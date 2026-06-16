<%@ Page Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write("<h3>Redis连接测试</h3>");
        Response.Write("<hr/>");
        
        // 测试Redis配置
        Response.Write("<h4>1. Redis配置信息:</h4>");
        var config = RedisHelper.Config;
        if (config != null)
        {
            Response.Write("<p>配置名称: " + config.Name + "</p>");
            Response.Write("<p>写服务器: " + config.WriteServerConStr + "</p>");
            Response.Write("<p>读服务器: " + config.ReadServerConStr + "</p>");
            Response.Write("<p>数据库编号: " + config.DbNum + "</p>");
        }
        else
        {
            Response.Write("<p style='color:red'>Redis配置加载失败！</p>");
        }
        
        Response.Write("<hr/>");
        
        // 测试Redis缓存用户信息
        Response.Write("<h4>2. Redis缓存测试:</h4>");
        try
        {
            int testUserId = 1;
            Response.Write("<p>测试用户ID: " + testUserId + "</p>");
            
            // 先缓存用户信息
            RedisHelper.CacheUserInfo(testUserId, "testuser", "测试用户", "13800138000", 1, Guid.NewGuid().ToString(), 100, "测试店铺", "测试公司");
            Response.Write("<p style='color:green'>缓存用户信息成功</p>");
            
            // 再读取用户信息
            var userData = RedisHelper.GetUserInfo(testUserId);
            if (userData != null && userData.Count > 0)
            {
                Response.Write("<p style='color:green'>从Redis读取用户信息成功</p>");
                Response.Write("<table border='1' cellpadding='5'>");
                Response.Write("<tr><th>字段</th><th>值</th></tr>");
                foreach (var kvp in userData)
                {
                    Response.Write("<tr><td>" + kvp.Key + "</td><td>" + kvp.Value + "</td></tr>");
                }
                Response.Write("</table>");
            }
            else
            {
                Response.Write("<p style='color:red'>从Redis读取用户信息失败！</p>");
            }
        }
        catch (Exception ex)
        {
            Response.Write("<p style='color:red'>Redis操作异常: " + ex.Message + "</p>");
            Response.Write("<p style='color:red'>堆栈: " + ex.StackTrace + "</p>");
        }
        
        Response.Write("<hr/>");
        
        // 测试Cookie
        Response.Write("<h4>3. Cookie测试:</h4>");
        HttpCookie cookie = Request.Cookies["ZrWebUser"];
        if (cookie != null)
        {
            Response.Write("<p style='color:green'>找到ZrWebUser Cookie</p>");
            Response.Write("<table border='1' cellpadding='5'>");
            Response.Write("<tr><th>字段</th><th>值</th></tr>");
            Response.Write("<tr><td>UserID</td><td>" + (cookie["UserID"] ?? "空") + "</td></tr>");
            Response.Write("<tr><td>UserName</td><td>" + (cookie["UserName"] ?? "空") + "</td></tr>");
            Response.Write("<tr><td>Expires</td><td>" + cookie.Expires.ToString() + "</td></tr>");
            Response.Write("<tr><td>HttpOnly</td><td>" + cookie.HttpOnly + "</td></tr>");
            Response.Write("</table>");
        }
        else
        {
            Response.Write("<p style='color:red'>未找到ZrWebUser Cookie</p>");
        }
        
        Response.Write("<hr/>");
        
        // 测试Session
        Response.Write("<h4>4. Session测试:</h4>");
        if (Session["UserID"] != null)
        {
            Response.Write("<p style='color:green'>Session[UserID]: " + Session["UserID"] + "</p>");
            Response.Write("<p>Session[UserName]: " + (Session["UserName"] ?? "空") + "</p>");
            Response.Write("<p>Session[ShopId]: " + (Session["ShopId"] ?? "空") + "</p>");
            Response.Write("<p>Session超时时间: " + Session.Timeout + " 分钟</p>");
        }
        else
        {
            Response.Write("<p style='color:red'>Session[UserID]为空</p>");
        }
        
        Response.Write("<hr/>");
        
        // 测试UserHelper
        Response.Write("<h4>5. UserHelper测试:</h4>");
        int userId = UserHelper.GetUserId();
        Response.Write("<p>UserHelper.GetUserId(): " + userId + "</p>");
        Response.Write("<p>UserHelper.IsLoggedIn(): " + UserHelper.IsLoggedIn() + "</p>");
        Response.Write("<p>UserHelper.GetUserName(): " + UserHelper.GetUserName() + "</p>");
        Response.Write("<p>UserHelper.GetShopId(): " + UserHelper.GetShopId() + "</p>");
        
        Response.Write("<hr/>");
    }
</script>
