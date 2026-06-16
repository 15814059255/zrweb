<%@ WebHandler Language="C#" Class="CheckLogin" %>

using System;
using System.Web;

public class CheckLogin : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.Charset = "utf-8";
        
        bool isLoggedIn = UserHelper.IsLoggedIn();
        int userId = UserHelper.GetUserId();
        string userName = UserHelper.GetUserName();
        int shopId = UserHelper.GetShopId();
        string shopName = UserHelper.GetShopName();
        
        context.Response.Write("{\"success\":true,\"isLoggedIn\":" + (isLoggedIn ? "true" : "false") + 
            ",\"userId\":" + userId + 
            ",\"userName\":\"" + CleanJson(userName) + "\"" + 
            ",\"shopId\":" + shopId + 
            ",\"shopName\":\"" + CleanJson(shopName) + "\"}");
    }
    
    private string CleanJson(string str)
    {
        if (string.IsNullOrEmpty(str)) return "";
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\r", "\\r")
                  .Replace("\n", "\\n")
                  .Replace("\t", "\\t");
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
