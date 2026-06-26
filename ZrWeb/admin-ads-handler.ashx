<%@ WebHandler Language="C#" Class="AdminAdsHandler" %>

using System;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class AdminAdsHandler : IHttpHandler, IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {
        try {
            string action = context.Request["action"];
            
            context.Response.ClearContent();
            context.Response.ClearHeaders();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "UTF-8";
            
            if (action == "addAd") {
                HandleAddAd(context);
            } else {
                WriteJson(context, "{\"success\":false,\"message\":\"无效的操作\"}");
            }
        } catch (System.Threading.ThreadAbortException) {
        } catch (Exception ex) {
            context.Response.ClearContent();
            context.Response.ClearHeaders();
            context.Response.ContentType = "application/json";
            context.Response.Charset = "UTF-8";
            WriteJson(context, "{\"success\":false,\"message\":\"服务器错误: " + ex.Message.Replace("\"", "\\\"").Replace("\r\n", " ") + "\"}");
        }
    }
    
    private void EnsureAdTableExists() {
        try {
            string checkSql = "SELECT COUNT(*) FROM sys.all_objects WHERE object_id = OBJECT_ID(N'[dbo].[ads]') AND type IN ('U')";
            int count = Convert.ToInt32(DbHelper.ExecuteScalar(checkSql));
            if (count == 0) {
                string createSql = @"CREATE TABLE [dbo].[ads] (
                    [AdID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [AdSlot] nvarchar(50) COLLATE Chinese_PRC_CI_AS NULL,
                    [Title] nvarchar(100) COLLATE Chinese_PRC_CI_AS NULL,
                    [Position] nvarchar(20) COLLATE Chinese_PRC_CI_AS NULL,
                    [LinkUrl] nvarchar(255) COLLATE Chinese_PRC_CI_AS NULL,
                    [StartDate] date NULL,
                    [EndDate] date NULL,
                    [Status] int NULL DEFAULT 1,
                    [CreateTime] datetime NULL DEFAULT GETDATE()
                )";
                DbHelper.ExecuteNonQuery(createSql);
            }
        } catch { }
    }

    private void HandleAddAd(HttpContext context) {
        if (context.Session == null || context.Session["AdminID"] == null) {
            WriteJson(context, "{\"success\":false,\"message\":\"会话超时，请重新登录\"}");
            return;
        }
        
        string adSlot = context.Request["adSlot"];
        string title = context.Request["title"];
        string position = context.Request["position"];
        string linkUrl = context.Request["linkUrl"];
        string startDate = context.Request["startDate"];
        string endDate = context.Request["endDate"];
        int status = 0;
        int.TryParse(context.Request["status"], out status);
        
        if (string.IsNullOrEmpty(adSlot)) {
            WriteJson(context, "{\"success\":false,\"message\":\"请选择广告位\"}");
            return;
        }
        if (string.IsNullOrEmpty(title)) {
            WriteJson(context, "{\"success\":false,\"message\":\"请输入广告标题\"}");
            return;
        }
        if (string.IsNullOrEmpty(linkUrl)) {
            WriteJson(context, "{\"success\":false,\"message\":\"请输入链接地址\"}");
            return;
        }
        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate)) {
            WriteJson(context, "{\"success\":false,\"message\":\"请选择开始时间和结束时间\"}");
            return;
        }
        
        try {
            EnsureAdTableExists();
            DbHelper.ExecuteNonQuery("INSERT INTO ads (keyWord, adName, adPositionId, adURL, adStartDate, adEndDate, dataFlag, createTime) VALUES (@adSlot, @title, @position, @linkUrl, @startDate, @endDate, @status, GETDATE())",
                DbHelper.CreateParameter("@adSlot", adSlot),
                DbHelper.CreateParameter("@title", title),
                DbHelper.CreateParameter("@position", position),
                DbHelper.CreateParameter("@linkUrl", linkUrl),
                DbHelper.CreateParameter("@startDate", startDate),
                DbHelper.CreateParameter("@endDate", endDate),
                DbHelper.CreateParameter("@status", status));
            
            WriteJson(context, "{\"success\":true,\"message\":\"广告添加成功\"}");
        } catch (Exception ex) {
            WriteJson(context, "{\"success\":false,\"message\":\"添加失败: " + ex.Message.Replace("\"", "\\\"").Replace("\r\n", " ") + "\"}");
        }
    }
    
    private void WriteJson(HttpContext context, string json) {
        try {
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.Flush();
            context.Response.End();
        } catch (System.Threading.ThreadAbortException) {
        }
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }
}