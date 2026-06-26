<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>诊断 goodsId=27801</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .key { color: #e74c3c; font-weight: bold; }
    </style>
</head>
<body>
    <h1>诊断 goodsId=27801</h1>
    
    <%
        int goodsId = 27801;
        string sql = @"SELECT g.*, s.shopName, s.shopCompany, s.shopAddress, s.shopTel, s.userId 
                      FROM goods g 
                      LEFT JOIN shops s ON g.shopId = s.shopId 
                      WHERE g.goodsId = @goodsId";
        DataTable dt = DbHelper.ExecuteQuery(sql, DbHelper.CreateParameter("@goodsId", goodsId));
        
        if (dt != null && dt.Rows.Count > 0) {
            DataRow row = dt.Rows[0];
            
            Response.Write("<h2>关键字段值</h2>");
            Response.Write("<table>");
            Response.Write("<tr><th>字段名</th><th>值</th><th>说明</th></tr>");
            Response.Write("<tr><td class='key'>shopId</td><td>" + GetValue(row["shopId"]) + "</td><td>店铺ID</td></tr>");
            Response.Write("<tr><td class='key'>shopCompany</td><td>" + GetValue(row["shopCompany"]) + "</td><td>公司名称（应该显示的）</td></tr>");
            Response.Write("<tr><td class='key'>shopName</td><td>" + GetValue(row["shopName"]) + "</td><td>主营品牌（多个用|分隔）</td></tr>");
            Response.Write("<tr><td class='key'>shopAddress</td><td>" + GetValue(row["shopAddress"]) + "</td><td>公司地址</td></tr>");
            Response.Write("<tr><td class='key'>shopTel</td><td>" + GetValue(row["shopTel"]) + "</td><td>优势型号（多个用|分隔）</td></tr>");
            Response.Write("<tr><td class='key'>userId</td><td>" + GetValue(row["userId"]) + "</td><td>用户ID</td></tr>");
            Response.Write("</table>");
            
            string shopCompany = GetValue(row["shopCompany"]);
            string shopName = GetValue(row["shopName"]);
            string shopAddress = GetValue(row["shopAddress"]);
            string shopTel = GetValue(row["shopTel"]);
            
            Response.Write("<h2>当前代码逻辑计算结果</h2>");
            Response.Write("<p>CompanyName = " + (!string.IsNullOrEmpty(shopCompany) ? shopCompany : (!string.IsNullOrEmpty(shopName) ? shopName : "匿名供应商")) + "</p>");
            Response.Write("<p>CompanyAddress = " + (!string.IsNullOrEmpty(shopAddress) ? shopAddress : "未填写") + "</p>");
            Response.Write("<p>MainBrands = " + shopName + "</p>");
            Response.Write("<p>AdvantageModels = " + shopTel + "</p>");
            
            int userId = row["userId"] != DBNull.Value ? Convert.ToInt32(row["userId"]) : 0;
            if (userId > 0) {
                string userSql = "SELECT CompanyName, Province, City, District, Street, Address, IsCertified FROM userinfo WHERE UserID = @userId";
                DataTable userDt = DbHelper.ExecuteQuery(userSql, DbHelper.CreateParameter("@userId", userId));
                if (userDt != null && userDt.Rows.Count > 0) {
                    DataRow userRow = userDt.Rows[0];
                    Response.Write("<h2>userinfo 表数据</h2>");
                    Response.Write("<table>");
                    Response.Write("<tr><th>字段名</th><th>值</th></tr>");
                    Response.Write("<tr><td>CompanyName</td><td>" + GetValue(userRow["CompanyName"]) + "</td></tr>");
                    Response.Write("<tr><td>Province</td><td>" + GetValue(userRow["Province"]) + "</td></tr>");
                    Response.Write("<tr><td>City</td><td>" + GetValue(userRow["City"]) + "</td></tr>");
                    Response.Write("<tr><td>District</td><td>" + GetValue(userRow["District"]) + "</td></tr>");
                    Response.Write("<tr><td>Street</td><td>" + GetValue(userRow["Street"]) + "</td></tr>");
                    Response.Write("<tr><td>Address</td><td>" + GetValue(userRow["Address"]) + "</td></tr>");
                    Response.Write("<tr><td>IsCertified</td><td>" + GetValue(userRow["IsCertified"]) + "</td></tr>");
                    Response.Write("</table>");
                }
            }
        } else {
            Response.Write("<p>没有找到 goodsId=27801 的数据</p>");
        }
        
        string GetValue(object val) {
            return val != DBNull.Value && val != null ? val.ToString() : "NULL";
        }
    %>
</body>
</html>
