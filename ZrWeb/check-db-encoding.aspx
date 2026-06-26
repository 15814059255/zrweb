<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>检查数据库编码</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
    </style>
</head>
<body>
    <h1>检查数据库编码</h1>
    
    <h2>数据库排序规则</h2>
    <%
        string sql = "SELECT SERVERPROPERTY('Collation') as ServerCollation, DATABASEPROPERTYEX(DB_NAME(), 'Collation') as DatabaseCollation";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        if (dt != null && dt.Rows.Count > 0) {
            Response.Write("<table>");
            Response.Write("<tr><th>服务器排序规则</th><th>数据库排序规则</th></tr>");
            Response.Write("<tr>");
            Response.Write("<td>" + dt.Rows[0]["ServerCollation"] + "</td>");
            Response.Write("<td>" + dt.Rows[0]["DatabaseCollation"] + "</td>");
            Response.Write("</tr>");
            Response.Write("</table>");
            
            string dbCollation = dt.Rows[0]["DatabaseCollation"].ToString();
            if (dbCollation.Contains("UTF8")) {
                Response.Write("<p class='success'>数据库使用 UTF-8 编码</p>");
            } else if (dbCollation.Contains("Chinese") || dbCollation.Contains("PRC") || dbCollation.Contains("GBK") || dbCollation.Contains("GB2312")) {
                Response.Write("<p class='warning'>数据库使用 GBK/GB2312 编码，可能导致中文乱码</p>");
            } else {
                Response.Write("<p class='warning'>数据库排序规则: " + dbCollation + "</p>");
            }
        }
    %>
    
    <h2>enquiryquoteprice 表字段类型</h2>
    <%
        sql = @"SELECT 
            c.name as ColumnName, 
            t.name as DataType, 
            c.max_length as MaxLength,
            c.collation_name as Collation
            FROM sys.columns c
            JOIN sys.types t ON c.system_type_id = t.system_type_id
            WHERE c.object_id = OBJECT_ID('enquiryquoteprice')
            AND c.name IN ('fromCompany', 'toCompany', 'fromContact', 'toContact')";
        dt = DbHelper.ExecuteQuery(sql);
        if (dt != null && dt.Rows.Count > 0) {
            Response.Write("<table>");
            Response.Write("<tr><th>字段名</th><th>数据类型</th><th>最大长度</th><th>排序规则</th></tr>");
            foreach (DataRow row in dt.Rows) {
                Response.Write("<tr>");
                Response.Write("<td>" + row["ColumnName"] + "</td>");
                Response.Write("<td>" + row["DataType"] + "</td>");
                Response.Write("<td>" + row["MaxLength"] + "</td>");
                string collation = row["Collation"] != DBNull.Value ? row["Collation"].ToString() : "NULL";
                string collClass = collation.Contains("UTF8") ? "success" : (collation.Contains("Chinese") || collation.Contains("PRC") || collation.Contains("GBK") ? "warning" : "");
                Response.Write("<td class='" + collClass + "'>" + collation + "</td>");
                Response.Write("</tr>");
            }
            Response.Write("</table>");
        }
    %>
    
    <h2>测试编码转换</h2>
    <%
        string[] testCases = { "娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃", "深圳市浩之东科技有限公司" };
        Response.Write("<table>");
        Response.Write("<tr><th>原始字符串</th><th>UTF8->GBK->UTF8</th><th>GBK->UTF8->GBK</th><th>是否有变化</th></tr>");
        foreach (string tc in testCases) {
            string result1 = ConvertEncoding(tc, "UTF-8", "GBK", "UTF-8");
            string result2 = ConvertEncoding(tc, "GBK", "UTF-8", "GBK");
            bool changed = tc != result1;
            Response.Write("<tr>");
            Response.Write("<td>" + Server.HtmlEncode(tc) + "</td>");
            Response.Write("<td>" + Server.HtmlEncode(result1) + "</td>");
            Response.Write("<td>" + Server.HtmlEncode(result2) + "</td>");
            Response.Write("<td>" + (changed ? "<span class='error'>是</span>" : "<span class='success'>否</span>") + "</td>");
            Response.Write("</tr>");
        }
        Response.Write("</table>");
    %>
    
    <h2>数据库中乱码数据测试</h2>
    <%
        sql = "SELECT TOP 1 fromCompany, toCompany FROM enquiryquoteprice WHERE dataFlag = 1 AND fromCompany IS NOT NULL AND fromCompany != ''";
        dt = DbHelper.ExecuteQuery(sql);
        if (dt != null && dt.Rows.Count > 0) {
            string fc = dt.Rows[0]["fromCompany"].ToString();
            string fcFixed = ConvertEncoding(fc, "UTF-8", "GBK", "UTF-8");
            Response.Write("<p>原始: " + Server.HtmlEncode(fc) + "</p>");
            Response.Write("<p>修复后: " + Server.HtmlEncode(fcFixed) + "</p>");
            Response.Write("<p>是否相同: " + (fc == fcFixed ? "<span class='success'>是</span>" : "<span class='error'>否</span>") + "</p>");
        }
    %>
    
    <%
        string ConvertEncoding(string input, string from, string middle, string to) {
            if (string.IsNullOrEmpty(input)) return input;
            try {
                byte[] fromBytes = System.Text.Encoding.GetEncoding(from).GetBytes(input);
                string middleStr = System.Text.Encoding.GetEncoding(middle).GetString(fromBytes);
                byte[] middleBytes = System.Text.Encoding.GetEncoding(middle).GetBytes(middleStr);
                return System.Text.Encoding.GetEncoding(to).GetString(middleBytes);
            } catch {
                return input;
            }
        }
    %>
</body>
</html>
