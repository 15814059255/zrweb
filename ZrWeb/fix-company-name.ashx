<%@ WebHandler Language="C#" Class="FixCompanyNameHandler" %>

using System;
using System.Web;
using System.Data;

public class FixCompanyNameHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        
        try {
            string action = context.Request["action"];
            
            if (action == "fix") {
                int fixedCount = FixCompanyNames();
                context.Response.Write("<p class='success'>修复完成！共修复 " + fixedCount + " 条记录。页面将在 3 秒后刷新。</p>");
            } else {
                context.Response.Write("<p class='error'>无效操作</p>");
            }
        } catch (Exception ex) {
            context.Response.Write("<p class='error'>修复失败: " + ex.Message + "</p>");
        }
    }
    
    private int FixCompanyNames() {
        int count = 0;
        
        string sql = "SELECT UserID, CompanyName FROM [dbo].[userinfo] WHERE CompanyName IS NOT NULL AND CompanyName != ''";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                int userId = Convert.ToInt32(row["UserID"]);
                string originalName = row["CompanyName"].ToString();
                string cleanedName = CleanStringValue(originalName);
                
                if (originalName != cleanedName) {
                    DbHelper.ExecuteNonQuery("UPDATE [dbo].[userinfo] SET CompanyName = @name WHERE UserID = @userId",
                        DbHelper.CreateParameter("@name", cleanedName),
                        DbHelper.CreateParameter("@userId", userId));
                    count++;
                }
            }
        }
        
        string shopSql = "SELECT shopId, shopCompany FROM [dbo].[shops] WHERE shopCompany IS NOT NULL AND shopCompany != ''";
        DataTable shopDt = DbHelper.ExecuteQuery(shopSql);
        
        if (shopDt != null && shopDt.Rows.Count > 0) {
            foreach (DataRow row in shopDt.Rows) {
                int shopId = Convert.ToInt32(row["shopId"]);
                string originalName = row["shopCompany"].ToString();
                string cleanedName = CleanStringValue(originalName);
                
                if (originalName != cleanedName) {
                    DbHelper.ExecuteNonQuery("UPDATE [dbo].[shops] SET shopCompany = @name WHERE shopId = @shopId",
                        DbHelper.CreateParameter("@name", cleanedName),
                        DbHelper.CreateParameter("@shopId", shopId));
                    count++;
                }
            }
        }
        
        return count;
    }
    
    private string CleanStringValue(string value) {
        if (string.IsNullOrEmpty(value)) return "";
        char[] cleanChars = new char[value.Length];
        int idx = 0;
        foreach (char c in value) {
            if (c >= 0x20 && c <= 0x7E || 
                c >= 0x4E00 && c <= 0x9FFF || 
                c >= 0x3000 && c <= 0x303F ||
                c >= 0xFF00 && c <= 0xFFEF) {
                cleanChars[idx++] = c;
            }
        }
        return new string(cleanChars, 0, idx).Trim();
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}
