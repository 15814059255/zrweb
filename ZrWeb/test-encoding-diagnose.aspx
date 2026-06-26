<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Text" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>编码诊断</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .hex { font-family: monospace; font-size: 11px; }
    </style>
</head>
<body>
    <h1>编码诊断</h1>
    
    <h2>从数据库读取实际数据</h2>
    <%
        string sql = "SELECT TOP 5 eqId, fromCompany, toCompany, fromContact, toContact FROM enquiryquoteprice WHERE dataFlag = 1 AND fromCompany IS NOT NULL AND fromCompany != '' ORDER BY eqId DESC";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                string fromCompany = row["fromCompany"].ToString();
                string toCompany = row["toCompany"] != DBNull.Value ? row["toCompany"].ToString() : "";
                Response.Write("<h3>eqId: " + row["eqId"] + "</h3>");
                Response.Write("<table>");
                Response.Write("<tr><th>字段</th><th>原始值</th><th>UTF8字节</th><th>GBK解码(UTF8字节)</th><th>GBK字节</th><th>UTF8解码(GBK字节)</th></tr>");
                
                Response.Write("<tr>");
                Response.Write("<td>fromCompany</td>");
                Response.Write("<td>" + Server.HtmlEncode(fromCompany) + "</td>");
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(fromCompany);
                Response.Write("<td class='hex'>" + BitConverter.ToString(utf8Bytes).Replace("-", " ") + "</td>");
                string gbkFromUtf8 = Encoding.GetEncoding("GBK").GetString(utf8Bytes);
                Response.Write("<td>" + Server.HtmlEncode(gbkFromUtf8) + "</td>");
                byte[] gbkBytes = Encoding.GetEncoding("GBK").GetBytes(fromCompany);
                Response.Write("<td class='hex'>" + BitConverter.ToString(gbkBytes).Replace("-", " ") + "</td>");
                string utf8FromGbk = Encoding.UTF8.GetString(gbkBytes);
                Response.Write("<td>" + Server.HtmlEncode(utf8FromGbk) + "</td>");
                Response.Write("</tr>");
                
                if (!string.IsNullOrEmpty(toCompany)) {
                    Response.Write("<tr>");
                    Response.Write("<td>toCompany</td>");
                    Response.Write("<td>" + Server.HtmlEncode(toCompany) + "</td>");
                    byte[] utf8Bytes2 = Encoding.UTF8.GetBytes(toCompany);
                    Response.Write("<td class='hex'>" + BitConverter.ToString(utf8Bytes2).Replace("-", " ") + "</td>");
                    string gbkFromUtf8_2 = Encoding.GetEncoding("GBK").GetString(utf8Bytes2);
                    Response.Write("<td>" + Server.HtmlEncode(gbkFromUtf8_2) + "</td>");
                    byte[] gbkBytes2 = Encoding.GetEncoding("GBK").GetBytes(toCompany);
                    Response.Write("<td class='hex'>" + BitConverter.ToString(gbkBytes2).Replace("-", " ") + "</td>");
                    string utf8FromGbk2 = Encoding.UTF8.GetString(gbkBytes2);
                    Response.Write("<td>" + Server.HtmlEncode(utf8FromGbk2) + "</td>");
                    Response.Write("</tr>");
                }
                
                Response.Write("</table>");
                
                Response.Write("<h4>各种转换方式测试：</h4>");
                Response.Write("<table>");
                Response.Write("<tr><th>转换方式</th><th>结果</th></tr>");
                
                // 方式1: UTF8->GBK->UTF8 (当前FixEncoding使用的)
                byte[] b1 = Encoding.UTF8.GetBytes(fromCompany);
                string s1 = Encoding.GetEncoding("GBK").GetString(b1);
                byte[] b1b = Encoding.GetEncoding("GBK").GetBytes(s1);
                string r1 = Encoding.UTF8.GetString(b1b);
                Response.Write("<tr><td>UTF8->GBK->UTF8</td><td>" + Server.HtmlEncode(r1) + "</td></tr>");
                
                // 方式2: GBK->UTF8->GBK
                byte[] b2 = Encoding.GetEncoding("GBK").GetBytes(fromCompany);
                string s2 = Encoding.UTF8.GetString(b2);
                byte[] b2b = Encoding.UTF8.GetBytes(s2);
                string r2 = Encoding.GetEncoding("GBK").GetString(b2b);
                Response.Write("<tr><td>GBK->UTF8->GBK</td><td>" + Server.HtmlEncode(r2) + "</td></tr>");
                
                // 方式3: 直接Hex转字符串
                Response.Write("<tr><td>Hex值</td><td class='hex'>" + BitConverter.ToString(utf8Bytes).Replace("-", "") + "</td></tr>");
                
                Response.Write("</table>");
            }
        } else {
            Response.Write("<p>没有找到数据</p>");
        }
    %>
    
    <h2>测试已知乱码</h2>
    <%
        string testMojibake = "娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃";
        Response.Write("<p>输入乱码: " + testMojibake + "</p>");
        
        byte[] utf8Bytes_test = Encoding.UTF8.GetBytes(testMojibake);
        string gbk_test = Encoding.GetEncoding("GBK").GetString(utf8Bytes_test);
        Response.Write("<p>UTF8字节->GBK解码: " + gbk_test + "</p>");
        
        byte[] gbkBytes_test = Encoding.GetEncoding("GBK").GetBytes(testMojibake);
        string utf8_test = Encoding.UTF8.GetString(gbkBytes_test);
        Response.Write("<p>GBK字节->UTF8解码: " + utf8_test + "</p>");
    %>
    
    <h2>数据库排序规则</h2>
    <%
        sql = "SELECT SERVERPROPERTY('Collation') as ServerCollation, DATABASEPROPERTYEX(DB_NAME(), 'Collation') as DatabaseCollation";
        dt = DbHelper.ExecuteQuery(sql);
        if (dt != null && dt.Rows.Count > 0) {
            Response.Write("<p>服务器排序规则: " + dt.Rows[0]["ServerCollation"] + "</p>");
            Response.Write("<p>数据库排序规则: " + dt.Rows[0]["DatabaseCollation"] + "</p>");
        }
    %>
</body>
</html>
