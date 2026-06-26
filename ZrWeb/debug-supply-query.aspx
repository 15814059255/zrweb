<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>诊断供应详情查询</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .success { color: #27ae60; font-weight: bold; }
        .warning { color: #f39c12; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .query { background-color: #f8f9fa; padding: 10px; border-radius: 4px; margin-bottom: 10px; }
    </style>
</head>
<body>
    <h1>诊断供应详情查询 (goodsId=27801)</h1>
    
    <%
        int goodsId = 27801;
        
        Response.Write("<h2>1. 原始数据查询（无任何条件）</h2>");
        string sql1 = "SELECT goodsId, goodsSn, shopId, pubType, dataFlag, isSale FROM goods WHERE goodsId = @goodsId";
        DataTable dt1 = DbHelper.ExecuteQuery(sql1, DbHelper.CreateParameter("@goodsId", goodsId));
        if (dt1 != null && dt1.Rows.Count > 0) {
            DataRow row1 = dt1.Rows[0];
            Response.Write("<table>");
            Response.Write("<tr><th>字段名</th><th>值</th><th>是否满足条件</th></tr>");
            Response.Write("<tr><td>goodsId</td><td>" + row1["goodsId"] + "</td><td>-</td></tr>");
            Response.Write("<tr><td>goodsSn</td><td>" + row1["goodsSn"] + "</td><td>-</td></tr>");
            Response.Write("<tr><td>shopId</td><td>" + row1["shopId"] + "</td><td>-</td></tr>");
            Response.Write("<tr><td>pubType</td><td>" + row1["pubType"] + "</td><td>" + (Convert.ToInt32(row1["pubType"]) == 1 ? "<span class='success'>OK</span>" : "<span class='error'>需为1</span>") + "</td></tr>");
            Response.Write("<tr><td>dataFlag</td><td>" + row1["dataFlag"] + "</td><td>" + (Convert.ToInt32(row1["dataFlag"]) == 1 ? "<span class='success'>OK</span>" : "<span class='error'>需为1</span>") + "</td></tr>");
            Response.Write("<tr><td>isSale</td><td>" + row1["isSale"] + "</td><td>" + (Convert.ToInt32(row1["isSale"]) == 1 ? "<span class='success'>OK</span>" : "<span class='warning'>需为1（当前为0，已下架）</span>") + "</td></tr>");
            Response.Write("</table>");
            
            int shopId = Convert.ToInt32(row1["shopId"]);
            
            Response.Write("<h2>2. 店铺信息查询 (shopId=" + shopId + ")</h2>");
            string sql2 = "SELECT shopId, shopName, shopCompany, shopAddress, shopTel, userId FROM shops WHERE shopId = @shopId";
            DataTable dt2 = DbHelper.ExecuteQuery(sql2, DbHelper.CreateParameter("@shopId", shopId));
            if (dt2 != null && dt2.Rows.Count > 0) {
                DataRow row2 = dt2.Rows[0];
                Response.Write("<table>");
                Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                Response.Write("<tr><td>shopId</td><td>" + row2["shopId"] + "</td></tr>");
                Response.Write("<tr><td>shopName</td><td>" + row2["shopName"] + "</td></tr>");
                Response.Write("<tr><td>shopCompany</td><td>" + row2["shopCompany"] + "</td></tr>");
                Response.Write("<tr><td>shopAddress</td><td>" + row2["shopAddress"] + "</td></tr>");
                Response.Write("<tr><td>shopTel</td><td>" + row2["shopTel"] + "</td></tr>");
                Response.Write("<tr><td>userId</td><td>" + row2["userId"] + "</td></tr>");
                Response.Write("</table>");
                
                int userId = Convert.ToInt32(row2["userId"]);
                Response.Write("<h2>3. 用户认证信息查询 (userId=" + userId + ")</h2>");
                string sql3 = "SELECT UserID, CompanyName, IsCertified, Province, City, District, Street, Address FROM userinfo WHERE UserID = @userId";
                DataTable dt3 = DbHelper.ExecuteQuery(sql3, DbHelper.CreateParameter("@userId", userId));
                if (dt3 != null && dt3.Rows.Count > 0) {
                    DataRow row3 = dt3.Rows[0];
                    Response.Write("<table>");
                    Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                    Response.Write("<tr><td>UserID</td><td>" + row3["UserID"] + "</td></tr>");
                    Response.Write("<tr><td>CompanyName</td><td>" + row3["CompanyName"] + "</td></tr>");
                    Response.Write("<tr><td>IsCertified</td><td>" + row3["IsCertified"] + "</td></tr>");
                    Response.Write("<tr><td>Province</td><td>" + row3["Province"] + "</td></tr>");
                    Response.Write("<tr><td>City</td><td>" + row3["City"] + "</td></tr>");
                    Response.Write("<tr><td>District</td><td>" + row3["District"] + "</td></tr>");
                    Response.Write("<tr><td>Street</td><td>" + row3["Street"] + "</td></tr>");
                    Response.Write("<tr><td>Address</td><td>" + row3["Address"] + "</td></tr>");
                    Response.Write("</table>");
                } else {
                    Response.Write("<p class='warning'>userinfo 表中没有找到 userId=" + userId + " 的记录</p>");
                }
            } else {
                Response.Write("<p class='error'>shops 表中没有找到 shopId=" + shopId + " 的记录</p>");
            }
            
            Response.Write("<h2>4. 当前实际查询（包含所有条件）</h2>");
            Response.Write("<div class='query'>SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.userId FROM goods g LEFT JOIN shops s ON g.shopId = s.shopId WHERE g.goodsId = 27801 AND g.dataFlag = 1 AND g.pubType = 1 AND g.isSale = 1</div>");
            string sql4 = @"SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.userId 
                          FROM goods g 
                          LEFT JOIN shops s ON g.shopId = s.shopId 
                          WHERE g.goodsId = @goodsId AND g.dataFlag = 1 AND g.pubType = 1 AND g.isSale = 1";
            DataTable dt4 = DbHelper.ExecuteQuery(sql4, DbHelper.CreateParameter("@goodsId", goodsId));
            if (dt4 != null && dt4.Rows.Count > 0) {
                Response.Write("<p class='success'>查询成功！找到 " + dt4.Rows.Count + " 条记录</p>");
            } else {
                Response.Write("<p class='error'>查询失败！未找到记录，将显示默认数据</p>");
                Response.Write("<p>原因分析：</p>");
                Response.Write("<ul>");
                if (Convert.ToInt32(row1["isSale"]) != 1) Response.Write("<li>isSale = " + row1["isSale"] + "，商品已下架</li>");
                if (Convert.ToInt32(row1["pubType"]) != 1) Response.Write("<li>pubType = " + row1["pubType"] + "，不是供应商品</li>");
                if (Convert.ToInt32(row1["dataFlag"]) != 1) Response.Write("<li>dataFlag = " + row1["dataFlag"] + "，商品已删除</li>");
                Response.Write("</ul>");
            }
        } else {
            Response.Write("<p class='error'>goods 表中没有找到 goodsId=27801 的记录</p>");
        }
    %>
</body>
</html>
