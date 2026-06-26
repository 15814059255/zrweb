<%@ WebHandler Language="C#" Class="RepairDbEncodingHandler" %>

using System;
using System.Web;
using System.Data;

public class RepairDbEncodingHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        
        try {
            string action = context.Request["action"];
            
            if (action == "repair") {
                int fixedCount = RepairAllTables();
                context.Response.Write("<p class='success'>修复完成！共修复 " + fixedCount + " 条记录。页面将在 3 秒后刷新。</p>");
            } else {
                context.Response.Write("<p class='error'>无效操作</p>");
            }
        } catch (Exception ex) {
            context.Response.Write("<p class='error'>修复失败: " + ex.Message + "</p>");
        }
    }
    
    private int RepairAllTables() {
        int totalCount = 0;
        
        string[] tables = { "enquiryquoteprice", "userinfo", "shops" };
        string[][] fields = {
            new string[] { "fromCompany", "toCompany", "fromContact", "toContact" },
            new string[] { "CompanyName", "IDCardName" },
            new string[] { "shopCompany" }
        };
        string[] idFields = { "eqId", "UserID", "shopId" };
        
        for (int i = 0; i < tables.Length; i++) {
            totalCount += RepairTable(tables[i], fields[i], idFields[i]);
        }
        
        return totalCount;
    }
    
    private int RepairTable(string table, string[] fields, string idField) {
        int count = 0;
        
        string selectSql = "SELECT " + idField + ", " + string.Join(", ", fields) + " FROM " + table + " WHERE dataFlag = 1";
        DataTable dt = DbHelper.ExecuteQuery(selectSql);
        
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                int id = Convert.ToInt32(row[idField]);
                bool changed = false;
                string updateSql = "UPDATE " + table + " SET ";
                System.Collections.Generic.List<object> paramsList = new System.Collections.Generic.List<object>();
                
                foreach (string field in fields) {
                    if (row[field] != DBNull.Value) {
                        string original = row[field].ToString();
                        string fixedVal = DbHelper.FixEncoding(original);
                        
                        if (original != fixedVal) {
                            if (changed) updateSql += ", ";
                            updateSql += field + " = @" + field;
                            paramsList.Add(DbHelper.CreateParameter("@" + field, fixedVal));
                            changed = true;
                        }
                    }
                }
                
                if (changed) {
                    updateSql += " WHERE " + idField + " = @id";
                    paramsList.Add(DbHelper.CreateParameter("@id", id));
                    
                    DbHelper.ExecuteNonQuery(updateSql, paramsList.ToArray());
                    count++;
                }
            }
        }
        
        return count;
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}
