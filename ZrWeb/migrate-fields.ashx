<%@ WebHandler Language="C#" Class="migrate_fields" %>

using System;
using System.Web;
using System.Text;

public class migrate_fields : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/html; charset=utf-8";
        
        StringBuilder sb = new StringBuilder();
        sb.Append("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>数据库字段迁移</title></head><body>");
        sb.Append("<h2>数据库字段迁移 - 扩大字段长度</h2>");
        
        string[] sqlStatements = new string[]
        {
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopName] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopCompany] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopTel] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopImg] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopQQ] nvarchar(255) NULL",
            "ALTER TABLE [dbo].[shops] ALTER COLUMN [shopAddress] nvarchar(1000) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [CompanyName] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Province] nvarchar(100) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [City] nvarchar(100) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [District] nvarchar(100) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Street] nvarchar(200) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Address] nvarchar(500) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [LinkMan] nvarchar(100) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [QQ] nvarchar(50) NULL",
            "ALTER TABLE [dbo].[userinfo] ALTER COLUMN [Email] nvarchar(100) NULL"
        };
        
        int success = 0;
        int failed = 0;
        
        foreach (string sql in sqlStatements)
        {
            try
            {
                DbHelper.ExecuteNonQuery(sql);
                sb.Append("<div style='color:green'>✓ 成功: " + sql + "</div>");
                success++;
            }
            catch (Exception ex)
            {
                sb.Append("<div style='color:red'>✗ 失败: " + sql + "<br>错误: " + ex.Message + "</div>");
                failed++;
            }
        }
        
        sb.Append("<br><p><strong>总计: " + sqlStatements.Length + " 条, 成功: " + success + " 条, 失败: " + failed + " 条</strong></p>");
        sb.Append("<p><a href='/profile.aspx'>返回个人资料页面</a></p>");
        sb.Append("</body></html>");
        
        context.Response.Write(sb.ToString());
    }
 
    public bool IsReusable
    {
        get { return false; }
    }
}
