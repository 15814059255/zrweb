<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>诊断供应详情数据</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .error { color: #e74c3c; font-weight: bold; }
        .success { color: #27ae60; font-weight: bold; }
        .warning { color: #f39c12; }
    </style>
</head>
<body>
    <h1>诊断供应详情数据</h1>
    
    <%
        int goodsId = 0;
        if (Request.QueryString["id"] != null) {
            int.TryParse(Request.QueryString["id"], out goodsId);
        }
        
        Response.Write("<p>当前 goodsId: " + goodsId + "</p>");
        
        if (goodsId > 0) {
            string sql = @"SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.userId 
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          WHERE g.goodsId = @goodsId";
            DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
            
            if (dt != null && dt.Rows.Count > 0) {
                DataRow row = dt.Rows[0];
                
                Response.Write("<h2>goods 表数据</h2>");
                Response.Write("<table>");
                Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                foreach (DataColumn col in dt.Columns) {
                    string val = row[col.ColumnName] != DBNull.Value ? row[col.ColumnName].ToString() : "NULL";
                    string highlight = "";
                    if (col.ColumnName == "shopId" && (string.IsNullOrEmpty(val) || val == "0")) highlight = "class='warning'";
                    if (col.ColumnName == "pubType" && val != "1") highlight = "class='warning'";
                    if (col.ColumnName == "dataFlag" && val != "1") highlight = "class='warning'";
                    Response.Write("<tr " + highlight + "><td>" + col.ColumnName + "</td><td>" + Server.HtmlEncode(val) + "</td></tr>");
                }
                Response.Write("</table>");
                
                int shopId = row["shopId"] != DBNull.Value ? Convert.ToInt32(row["shopId"]) : 0;
                if (shopId > 0) {
                    string shopSql = "SELECT * FROM shops WHERE shopId = @shopId";
                    DataTable shopDt = DbHelper.ExecuteQuery(shopSql, DbHelper.CreateParameter("@shopId", shopId));
                    
                    Response.Write("<h2>shops 表数据 (shopId=" + shopId + ")</h2>");
                    if (shopDt != null && shopDt.Rows.Count > 0) {
                        DataRow shopRow = shopDt.Rows[0];
                        Response.Write("<table>");
                        Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                        foreach (DataColumn col in shopDt.Columns) {
                            string val = shopRow[col.ColumnName] != DBNull.Value ? shopRow[col.ColumnName].ToString() : "NULL";
                            Response.Write("<tr><td>" + col.ColumnName + "</td><td>" + Server.HtmlEncode(val) + "</td></tr>");
                        }
                        Response.Write("</table>");
                    } else {
                        Response.Write("<p class='error'>shops 表中没有找到 shopId=" + shopId + " 的记录</p>");
                    }
                } else {
                    Response.Write("<p class='warning'>goods 表中的 shopId 为空或 0，无法关联到供应商</p>");
                }
                
                int userId = row["userId"] != DBNull.Value ? Convert.ToInt32(row["userId"]) : 0;
                if (userId > 0) {
                    string userSql = "SELECT * FROM userinfo WHERE UserID = @userId";
                    DataTable userDt = DbHelper.ExecuteQuery(userSql, DbHelper.CreateParameter("@userId", userId));
                    
                    Response.Write("<h2>userinfo 表数据 (userId=" + userId + ")</h2>");
                    if (userDt != null && userDt.Rows.Count > 0) {
                        DataRow userRow = userDt.Rows[0];
                        Response.Write("<table>");
                        Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                        foreach (DataColumn col in userDt.Columns) {
                            string val = userRow[col.ColumnName] != DBNull.Value ? userRow[col.ColumnName].ToString() : "NULL";
                            Response.Write("<tr><td>" + col.ColumnName + "</td><td>" + Server.HtmlEncode(val) + "</td></tr>");
                        }
                        Response.Write("</table>");
                    }
                }
            } else {
                Response.Write("<p class='error'>goods 表中没有找到 goodsId=" + goodsId + " 的记录</p>");
                
                string allSql = "SELECT TOP 10 goodsId, goodsSn, shopId, pubType, dataFlag, isSale FROM goods WHERE pubType = 1 AND dataFlag = 1 ORDER BY goodsId DESC";
                DataTable allDt = DbHelper.ExecuteQuery(allSql);
                if (allDt != null && allDt.Rows.Count > 0) {
                    Response.Write("<h2>所有供应商品列表</h2>");
                    Response.Write("<table>");
                    Response.Write("<tr><th>goodsId</th><th>goodsSn</th><th>shopId</th><th>pubType</th><th>dataFlag</th><th>isSale</th></tr>");
                    foreach (DataRow row in allDt.Rows) {
                        Response.Write("<tr>");
                        Response.Write("<td><a href='/debug-supply-detail.aspx?id=" + row["goodsId"] + "'>" + row["goodsId"] + "</a></td>");
                        Response.Write("<td>" + row["goodsSn"] + "</td>");
                        Response.Write("<td>" + row["shopId"] + "</td>");
                        Response.Write("<td>" + row["pubType"] + "</td>");
                        Response.Write("<td>" + row["dataFlag"] + "</td>");
                        Response.Write("<td>" + row["isSale"] + "</td>");
                        Response.Write("</tr>");
                    }
                    Response.Write("</table>");
                }
            }
        } else {
            string allSql = "SELECT TOP 10 goodsId, goodsSn, shopId, pubType, dataFlag, isSale FROM goods WHERE pubType = 1 AND dataFlag = 1 ORDER BY goodsId DESC";
            DataTable allDt = DbHelper.ExecuteQuery(allSql);
            
            Response.Write("<h2>所有供应商品列表</h2>");
            if (allDt != null && allDt.Rows.Count > 0) {
                Response.Write("<table>");
                Response.Write("<tr><th>goodsId</th><th>goodsSn</th><th>shopId</th><th>pubType</th><th>dataFlag</th><th>isSale</th></tr>");
                foreach (DataRow row in allDt.Rows) {
                    Response.Write("<tr>");
                    Response.Write("<td><a href='/debug-supply-detail.aspx?id=" + row["goodsId"] + "'>" + row["goodsId"] + "</a></td>");
                    Response.Write("<td>" + row["goodsSn"] + "</td>");
                    Response.Write("<td>" + row["shopId"] + "</td>");
                    Response.Write("<td>" + row["pubType"] + "</td>");
                    Response.Write("<td>" + row["dataFlag"] + "</td>");
                    Response.Write("<td>" + row["isSale"] + "</td>");
                    Response.Write("</tr>");
                }
                Response.Write("</table>");
            } else {
                Response.Write("<p class='error'>没有找到任何供应商品</p>");
            }
        }
    %>
</body>
</html>
