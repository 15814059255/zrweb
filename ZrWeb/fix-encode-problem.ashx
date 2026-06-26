<%@ WebHandler Language="C#" Class="FixEncodeProblemHandler" %>

using System;
using System.Web;
using System.Data;

public class FixEncodeProblemHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        
        try {
            string action = context.Request["action"];
            
            if (action == "fix") {
                int fixedCount = FixEncodingProblem();
                context.Response.Write("<p class='success'>修复完成！共修复 " + fixedCount + " 条记录。页面将在 3 秒后刷新。</p>");
            } else {
                context.Response.Write("<p class='error'>无效操作</p>");
            }
        } catch (Exception ex) {
            context.Response.Write("<p class='error'>修复失败: " + ex.Message + "</p>");
        }
    }
    
    private int FixEncodingProblem() {
        int count = 0;
        
        string sql = @"SELECT eqId, fromCompany, toCompany, fromContact, toContact 
                      FROM enquiryquoteprice 
                      WHERE dataFlag = 1 AND (fromCompany IS NOT NULL OR toCompany IS NOT NULL)";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                int eqId = Convert.ToInt32(row["eqId"]);
                string fromCompany = row["fromCompany"] != DBNull.Value ? row["fromCompany"].ToString() : "";
                string toCompany = row["toCompany"] != DBNull.Value ? row["toCompany"].ToString() : "";
                string fromContact = row["fromContact"] != DBNull.Value ? row["fromContact"].ToString() : "";
                string toContact = row["toContact"] != DBNull.Value ? row["toContact"].ToString() : "";
                
                string fromCompanyFixed = FixEncoding(fromCompany);
                string toCompanyFixed = FixEncoding(toCompany);
                string fromContactFixed = FixEncoding(fromContact);
                string toContactFixed = FixEncoding(toContact);
                
                bool changed = false;
                if (fromCompany != fromCompanyFixed) {
                    fromCompany = fromCompanyFixed;
                    changed = true;
                }
                if (toCompany != toCompanyFixed) {
                    toCompany = toCompanyFixed;
                    changed = true;
                }
                if (fromContact != fromContactFixed) {
                    fromContact = fromContactFixed;
                    changed = true;
                }
                if (toContact != toContactFixed) {
                    toContact = toContactFixed;
                    changed = true;
                }
                
                if (changed) {
                    DbHelper.ExecuteNonQuery("UPDATE enquiryquoteprice SET fromCompany = @fromCompany, toCompany = @toCompany, fromContact = @fromContact, toContact = @toContact WHERE eqId = @eqId",
                        DbHelper.CreateParameter("@fromCompany", fromCompany),
                        DbHelper.CreateParameter("@toCompany", toCompany),
                        DbHelper.CreateParameter("@fromContact", fromContact),
                        DbHelper.CreateParameter("@toContact", toContact),
                        DbHelper.CreateParameter("@eqId", eqId));
                    count++;
                }
            }
        }
        
        return count;
    }
    
    private string FixEncoding(string str) {
        if (string.IsNullOrEmpty(str)) return str;
        try {
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);
            string gbkStr = System.Text.Encoding.GetEncoding("GBK").GetString(utf8Bytes);
            byte[] gbkBytes = System.Text.Encoding.GetEncoding("GBK").GetBytes(gbkStr);
            string utf8Str = System.Text.Encoding.UTF8.GetString(gbkBytes);
            if (utf8Str != str) {
                return utf8Str;
            }
        } catch { }
        return str;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}
