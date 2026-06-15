using System;
using System.Data;

public partial class test_inquiry : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentType = "application/json";

        if (Request.HttpMethod == "POST")
        {
            InsertTestData();
        }
        else if (Request["action"] == "query")
        {
            QueryData();
        }
    }

    private void InsertTestData()
    {
        try
        {
            string goodsSn = Request["goodsSn"] ?? "TEST-MODEL";
            int quantity = 0;
            int.TryParse(Request["quantity"], out quantity);
            decimal price = 0;
            decimal.TryParse(Request["price"], out price);
            string fromCompany = Request["fromCompany"] ?? "测试公司";
            string fromContact = Request["fromContact"] ?? "";
            string fromTel = Request["fromTel"] ?? "";
            string brandName = Request["brandName"] ?? "";
            string remarks = Request["remarks"] ?? "";

            // 模拟一个采购商 shopId（实际应该从 session 获取）
            int fromShopId = 999;

            // 获取当前商家的 shopId 作为 toShopId（模拟当前用户是供应商）
            int toShopId = 0;
            if (Session["ShopId"] != null)
            {
                int.TryParse(Session["ShopId"].ToString(), out toShopId);
            }
            // 如果没有 session，随机分配一个测试 shopId
            if (toShopId == 0)
            {
                toShopId = 1001;
            }

            string sql = @"INSERT INTO enquiryquoteprice 
                (goodsSn, eqType, fromShopId, toShopId, dataFlag, toDataFlag, fromDataFlag, readStatus,
                createTime, fromQuantity, fromPrice, isIncludingTax, pubSource, fromRemarks,
                fromContact, fromTel, fromCompany, brandName)
                VALUES 
                (@goodsSn, 1, @fromShopId, @toShopId, 1, 1, 1, 0,
                GETDATE(), @quantity, @price, 1, 1, @remarks,
                @fromContact, @fromTel, @fromCompany, @brandName)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@goodsSn", goodsSn),
                new SqlParameter("@fromShopId", fromShopId),
                new SqlParameter("@toShopId", toShopId),
                new SqlParameter("@quantity", quantity),
                new SqlParameter("@price", price),
                new SqlParameter("@remarks", remarks ?? ""),
                new SqlParameter("@fromContact", fromContact ?? ""),
                new SqlParameter("@fromTel", fromTel ?? ""),
                new SqlParameter("@fromCompany", fromCompany),
                new SqlParameter("@brandName", brandName ?? "")
            };

            int result = DbHelper.ExecuteNonQuery(sql, parameters);

            if (result > 0)
            {
                Response.Write("{\"success\":true,\"message\":\"插入成功，共插入 " + result + " 条记录\"}");
            }
            else
            {
                Response.Write("{\"success\":false,\"message\":\"插入失败\"}");
            }
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"错误: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }

    private void QueryData()
    {
        try
        {
            string sql = "SELECT TOP 20 eqId, goodsSn, fromCompany, fromQuantity, fromPrice, createTime FROM enquiryquoteprice WHERE eqType = 1 ORDER BY createTime DESC";
            DataTable dt = DbHelper.ExecuteQuery(sql);

            string json = "{\"success\":true,\"count\":" + dt.Rows.Count + ",\"data\":[";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                json += "{";
                json += "\"eqId\":" + row["eqId"] + ",";
                json += "\"goodsSn\":\"" + row["goodsSn"].ToString().Replace("\"", "\\\"") + "\",";
                json += "\"fromCompany\":\"" + (row["fromCompany"] != DBNull.Value ? row["fromCompany"].ToString().Replace("\"", "\\\"") : "") + "\",";
                json += "\"fromQuantity\":" + (row["fromQuantity"] != DBNull.Value ? row["fromQuantity"] : 0) + ",";
                json += "\"fromPrice\":" + (row["fromPrice"] != DBNull.Value ? row["fromPrice"] : 0) + ",";
                json += "\"createTime\":\"" + Convert.ToDateTime(row["createTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                json += "}";
                if (i < dt.Rows.Count - 1) json += ",";
            }
            json += "]}";
            Response.Write(json);
        }
        catch (Exception ex)
        {
            Response.Write("{\"success\":false,\"message\":\"查询失败: " + ex.Message.Replace("\"", "\\\"") + "\"}");
        }
    }
}