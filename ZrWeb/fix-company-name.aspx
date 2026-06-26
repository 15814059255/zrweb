<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>修复公司名称乱码</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        button { padding: 10px 20px; background: #3498db; color: white; border: none; cursor: pointer; margin: 10px 0; }
        button:hover { background: #2980b9; }
        .result { margin-top: 20px; padding: 15px; border: 1px solid #ddd; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .problem { background-color: #ffebee; }
    </style>
</head>
<body>
    <h1>修复公司名称乱码</h1>
    
    <h2>问题描述</h2>
    <p>由于编码不一致，通过 URL 参数传递中文公司名称时可能导致乱码。此工具可以检测并修复数据库中已有的乱码数据。</p>
    
    <h2>检测结果</h2>
    <%
        string checkSql = @"SELECT UserID, UserName, CompanyName FROM [dbo].[userinfo] WHERE CompanyName IS NOT NULL AND CompanyName != ''";
        DataTable dt = DbHelper.ExecuteQuery(checkSql);
        int problemCount = 0;
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                string name = row["CompanyName"].ToString();
                if (ContainsInvalidChars(name)) {
                    problemCount++;
                }
            }
        }
        
        Response.Write("<p>userinfo 表中有问题的公司名称: <span class='" + (problemCount > 0 ? "error" : "success") + "'>" + problemCount + "</span> 条</p>");
        
        string checkShopSql = @"SELECT shopId, userId, shopCompany FROM [dbo].[shops] WHERE shopCompany IS NOT NULL AND shopCompany != ''";
        DataTable shopDt = DbHelper.ExecuteQuery(checkShopSql);
        int shopProblemCount = 0;
        if (shopDt != null && shopDt.Rows.Count > 0) {
            foreach (DataRow row in shopDt.Rows) {
                string name = row["shopCompany"].ToString();
                if (ContainsInvalidChars(name)) {
                    shopProblemCount++;
                }
            }
        }
        
        Response.Write("<p>shops 表中有问题的公司名称: <span class='" + (shopProblemCount > 0 ? "error" : "success") + "'>" + shopProblemCount + "</span> 条</p>");
    %>
    
    <h2>有问题的数据预览</h2>
    <%
        if (problemCount > 0 || shopProblemCount > 0) {
    %>
    <table>
        <tr>
            <th>表名</th>
            <th>ID</th>
            <th>原始名称</th>
            <th>修复后</th>
        </tr>
        <%
            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow row in dt.Rows) {
                    string name = row["CompanyName"].ToString();
                    if (ContainsInvalidChars(name)) {
        %>
        <tr class="problem">
            <td>userinfo</td>
            <td>U<%= row["UserID"] %></td>
            <td><%= Server.HtmlEncode(name) %></td>
            <td><%= Server.HtmlEncode(CleanStringValue(name)) %></td>
        </tr>
        <%
                    }
                }
            }
            
            if (shopDt != null && shopDt.Rows.Count > 0) {
                foreach (DataRow row in shopDt.Rows) {
                    string name = row["shopCompany"].ToString();
                    if (ContainsInvalidChars(name)) {
        %>
        <tr class="problem">
            <td>shops</td>
            <td>S<%= row["shopId"] %></td>
            <td><%= Server.HtmlEncode(name) %></td>
            <td><%= Server.HtmlEncode(CleanStringValue(name)) %></td>
        </tr>
        <%
                    }
                }
            }
        %>
    </table>
    <%
        } else {
            Response.Write("<p class='success'>所有公司名称数据正常，无需修复。</p>");
        }
    %>
    
    <h2>修复操作</h2>
    <button onclick="doFix()">执行修复</button>
    
    <div id="fixResult" class="result" style="display:none;"></div>
    
    <script>
        function doFix() {
            var resultDiv = document.getElementById('fixResult');
            resultDiv.style.display = 'block';
            resultDiv.innerHTML = '<p>正在修复...</p>';
            
            fetch('/fix-company-name.ashx?action=fix', {
                method: 'POST'
            })
            .then(function(response) {
                return response.text();
            })
            .then(function(data) {
                resultDiv.innerHTML = data;
                setTimeout(function() {
                    window.location.reload();
                }, 3000);
            })
            .catch(function(error) {
                resultDiv.innerHTML = '<p class="error">修复失败: ' + error + '</p>';
            });
        }
    </script>
    
    <%
        bool ContainsInvalidChars(string str) {
            if (string.IsNullOrEmpty(str)) return false;
            foreach (char c in str) {
                if (c < 0x20 || (c > 0x7E && c < 0x4E00) || (c > 0x9FFF && c < 0x3000)) {
                    return true;
                }
            }
            return false;
        }
        
        string CleanStringValue(string value) {
            if (string.IsNullOrEmpty(value)) return "";
            char[] cleanChars = new char[value.Length];
            int idx = 0;
            foreach (char c in value) {
                if (c >= 0x20 && c <= 0x7E || 
                    c >= 0x4E00 && c <= 0x9FFF || 
                    c >= 0x3000 && c <= 0x303F ||
                    c >= 0xFF00 && c <= 0xFFEF) {
                    cleanChars[idx++] = c;
                }
            }
            return new string(cleanChars, 0, idx).Trim();
        }
    %>
</body>
</html>
