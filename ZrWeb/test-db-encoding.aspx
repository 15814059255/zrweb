<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Text" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>测试数据库编码</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .hex { font-family: monospace; font-size: 10px; }
        .problem-row { background-color: #ffebee; }
        .fixed-row { background-color: #e8f5e9; }
    </style>
</head>
<body>
    <h1>测试数据库编码</h1>
    
    <h2>数据库中的询价报价记录</h2>
    <%
        string sql = "SELECT TOP 10 eqId, fromCompany, toCompany, fromContact, toContact FROM enquiryquoteprice WHERE dataFlag = 1 AND (fromCompany IS NOT NULL OR toCompany IS NOT NULL) ORDER BY eqId DESC";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        
        if (dt != null && dt.Rows.Count > 0) {
            Response.Write("<table>");
            Response.Write("<tr><th>eqId</th><th>fromCompany (原始)</th><th>fromCompany (修复后)</th><th>toCompany (原始)</th><th>toCompany (修复后)</th><th>是否修复</th></tr>");
            
            foreach (DataRow row in dt.Rows) {
                string fc = row["fromCompany"] != DBNull.Value ? row["fromCompany"].ToString() : "";
                string tc = row["toCompany"] != DBNull.Value ? row["toCompany"].ToString() : "";
                string fcFixed = DbHelper.FixEncoding(fc);
                string tcFixed = DbHelper.FixEncoding(tc);
                
                bool fcChanged = fc != fcFixed;
                bool tcChanged = tc != tcFixed;
                string rowClass = (fcChanged || tcChanged) ? "fixed-row" : "";
                
                Response.Write("<tr class='" + rowClass + "'>");
                Response.Write("<td>" + row["eqId"] + "</td>");
                Response.Write("<td>" + Server.HtmlEncode(fc) + "</td>");
                Response.Write("<td>" + Server.HtmlEncode(fcFixed) + "</td>");
                Response.Write("<td>" + Server.HtmlEncode(tc) + "</td>");
                Response.Write("<td>" + Server.HtmlEncode(tcFixed) + "</td>");
                Response.Write("<td>" + ((fcChanged || tcChanged) ? "<span class='success'>是</span>" : "<span class='success'>无需修复</span>") + "</td>");
                Response.Write("</tr>");
            }
            
            Response.Write("</table>");
        } else {
            Response.Write("<p>没有找到数据</p>");
        }
    %>
    
    <h2>编码转换测试</h2>
    <%
        string[] testCases = {
            "娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃",
            "深圳市浩之东科技有限公司",
            "姹借溅",
            "汽车",
            "鍖椾含",
            "中国"
        };
        
        Response.Write("<table>");
        Response.Write("<tr><th>输入</th><th>FixEncoding结果</th><th>UTF-8字节</th><th>GBK解码(UTF-8字节)</th><th>GBK字节</th><th>UTF-8解码(GBK字节)</th></tr>");
        
        foreach (string tc in testCases) {
            Response.Write("<tr>");
            Response.Write("<td>" + Server.HtmlEncode(tc) + "</td>");
            Response.Write("<td>" + Server.HtmlEncode(DbHelper.FixEncoding(tc)) + "</td>");
            
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(tc);
            Response.Write("<td class='hex'>" + BitConverter.ToString(utf8Bytes) + "</td>");
            
            try {
                string gbkFromUtf8 = Encoding.GetEncoding("GBK").GetString(utf8Bytes);
                Response.Write("<td>" + Server.HtmlEncode(gbkFromUtf8) + "</td>");
                
                byte[] gbkBytes = Encoding.GetEncoding("GBK").GetBytes(gbkFromUtf8);
                Response.Write("<td class='hex'>" + BitConverter.ToString(gbkBytes) + "</td>");
                
                string utf8FromGbk = Encoding.UTF8.GetString(gbkBytes);
                Response.Write("<td>" + Server.HtmlEncode(utf8FromGbk) + "</td>");
            } catch (Exception ex) {
                Response.Write("<td colspan='3' class='error'>错误: " + ex.Message + "</td>");
            }
            
            Response.Write("</tr>");
        }
        
        Response.Write("</table>");
    %>
    
    <h2>数据库排序规则</h2>
    <%
        string sql2 = "SELECT SERVERPROPERTY('Collation') as ServerCollation, DATABASEPROPERTYEX(DB_NAME(), 'Collation') as DatabaseCollation";
        DataTable dt2 = DbHelper.ExecuteQuery(sql2);
        if (dt2 != null && dt2.Rows.Count > 0) {
            Response.Write("<p>服务器排序规则: " + dt2.Rows[0]["ServerCollation"] + "</p>");
            Response.Write("<p>数据库排序规则: " + dt2.Rows[0]["DatabaseCollation"] + "</p>");
        }
    %>
    
    <h2>enquiryquoteprice 表字段排序规则</h2>
    <%
        string sql3 = @"SELECT c.name as ColumnName, c.collation_name as Collation 
                       FROM sys.columns c 
                       WHERE c.object_id = OBJECT_ID('enquiryquoteprice') 
                       AND c.name IN ('fromCompany', 'toCompany', 'fromContact', 'toContact')";
        DataTable dt3 = DbHelper.ExecuteQuery(sql3);
        if (dt3 != null && dt3.Rows.Count > 0) {
            Response.Write("<table>");
            Response.Write("<tr><th>字段名</th><th>排序规则</th></tr>");
            foreach (DataRow row in dt3.Rows) {
                string collation = row["Collation"].ToString();
                string collClass = collation.Contains("UTF8") ? "success" : (collation.Contains("Chinese") || collation.Contains("PRC") || collation.Contains("GBK") ? "warning" : "");
                Response.Write("<tr><td>" + row["ColumnName"] + "</td><td class='" + collClass + "'>" + collation + "</td></tr>");
            }
            Response.Write("</table>");
        }
    %>
</body>
</html>
