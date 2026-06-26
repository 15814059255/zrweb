<%@ WebHandler Language="C#" Class="FixEnquiryToShopIdHandler" %>

using System;
using System.Web;
using System.Data;

public class FixEnquiryToShopIdHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        
        try {
            string action = context.Request["action"];
            
            if (action == "fix") {
                FixToShopId();
                context.Response.Write("<p class='success'>修复完成！页面将在 3 秒后刷新。</p>");
            } else {
                context.Response.Write("<p class='error'>无效操作</p>");
            }
        } catch (Exception ex) {
            context.Response.Write("<p class='error'>修复失败: " + ex.Message + "</p>");
        }
    }
    
    private void FixToShopId() {
        string sql = @"SELECT eqId, goodsId, goodsSn 
                      FROM enquiryquoteprice 
                      WHERE eqType = 1 AND (toShopId IS NULL OR toShopId = 0)";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                int eqId = Convert.ToInt32(row["eqId"]);
                int goodsId = row["goodsId"] != DBNull.Value ? Convert.ToInt32(row["goodsId"]) : 0;
                string goodsSn = row["goodsSn"] != DBNull.Value ? row["goodsSn"].ToString() : "";
                
                int shopId = 0;
                
                if (goodsId > 0) {
                    string goodsSql = "SELECT TOP 1 shopId FROM goods WHERE goodsId = @goodsId AND dataFlag = 1";
                    object result = DbHelper.ExecuteScalar(goodsSql, DbHelper.CreateParameter("@goodsId", goodsId));
                    if (result != null && result != DBNull.Value) {
                        shopId = Convert.ToInt32(result);
                    }
                }
                
                if (shopId == 0 && !string.IsNullOrEmpty(goodsSn)) {
                    string goodsSql = "SELECT TOP 1 shopId FROM goods WHERE goodsSn = @goodsSn AND pubType = 1 AND dataFlag = 1";
                    object result = DbHelper.ExecuteScalar(goodsSql, DbHelper.CreateParameter("@goodsSn", goodsSn));
                    if (result != null && result != DBNull.Value) {
                        shopId = Convert.ToInt32(result);
                    }
                }
                
                if (shopId > 0) {
                    string updateSql = "UPDATE enquiryquoteprice SET toShopId = @shopId WHERE eqId = @eqId";
                    DbHelper.ExecuteNonQuery(updateSql, 
                        DbHelper.CreateParameter("@shopId", shopId),
                        DbHelper.CreateParameter("@eqId", eqId));
                }
            }
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}
