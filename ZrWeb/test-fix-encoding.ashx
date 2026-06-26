<%@ WebHandler Language="C#" Class="TestFixEncodingHandler" %>

using System;
using System.Web;

public class TestFixEncodingHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        
        try {
            string action = context.Request["action"];
            string str = context.Request["str"] ?? "";
            
            if (action == "fix") {
                str = HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8);
                string fixedStr = DbHelper.FixEncoding(str);
                context.Response.Write(fixedStr);
            }
        } catch (Exception ex) {
            context.Response.Write("Error: " + ex.Message);
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}
