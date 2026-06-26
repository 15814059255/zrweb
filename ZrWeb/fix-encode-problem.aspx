<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>修复编码乱码问题</title>
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
        .fixed { color: #27ae60; font-weight: bold; }
    </style>
</head>
<body>
    <h1>修复编码乱码问题</h1>
    
    <h2>问题分析</h2>
    <p>您看到的乱码"娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃"是典型的 <strong>GBK 编码被当作 UTF-8 解码</strong> 的结果。</p>
    <p>正确的公司名称应该是：<strong>深圳市浩之东科技有限公司</strong></p>
    
    <h2>检测结果</h2>
    <%
        string testStr = "娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃";
        string fixedStr = FixEncoding(testStr);
        Response.Write("<p>测试修复：</p>");
        Response.Write("<p>乱码：" + testStr + "</p>");
        Response.Write("<p>修复后：<span class='success'>" + fixedStr + "</span></p>");
    %>
    
    <h2>enquiryquoteprice 表中的乱码数据</h2>
    <%
        string sql = @"SELECT eqId, fromCompany, toCompany, fromContact, toContact 
                      FROM enquiryquoteprice 
                      WHERE dataFlag = 1 AND (fromCompany IS NOT NULL OR toCompany IS NOT NULL)";
        DataTable dt = DbHelper.ExecuteQuery(sql);
        int problemCount = 0;
        if (dt != null && dt.Rows.Count > 0) {
            foreach (DataRow row in dt.Rows) {
                string fc = row["fromCompany"] != DBNull.Value ? row["fromCompany"].ToString() : "";
                string tc = row["toCompany"] != DBNull.Value ? row["toCompany"].ToString() : "";
                string fcFixed = FixEncoding(fc);
                string tcFixed = FixEncoding(tc);
                if (fc != fcFixed || tc != tcFixed) {
                    problemCount++;
                }
            }
        }
        Response.Write("<p>有问题的记录数: <span class='" + (problemCount > 0 ? "error" : "success") + "'>" + problemCount + "</span> 条</p>");
    %>
    
    <h2>乱码数据预览</h2>
    <%
        if (problemCount > 0) {
    %>
    <table>
        <tr>
            <th>eqId</th>
            <th>fromCompany (原始)</th>
            <th>fromCompany (修复后)</th>
            <th>toCompany (原始)</th>
            <th>toCompany (修复后)</th>
        </tr>
        <%
            if (dt != null && dt.Rows.Count > 0) {
                foreach (DataRow row in dt.Rows) {
                    string fc = row["fromCompany"] != DBNull.Value ? row["fromCompany"].ToString() : "";
                    string tc = row["toCompany"] != DBNull.Value ? row["toCompany"].ToString() : "";
                    string fcFixed = FixEncoding(fc);
                    string tcFixed = FixEncoding(tc);
                    if (fc != fcFixed || tc != tcFixed) {
        %>
        <tr class="problem">
            <td><%= row["eqId"] %></td>
            <td><%= Server.HtmlEncode(fc) %></td>
            <td><span class="fixed"><%= Server.HtmlEncode(fcFixed) %></span></td>
            <td><%= Server.HtmlEncode(tc) %></td>
            <td><span class="fixed"><%= Server.HtmlEncode(tcFixed) %></span></td>
        </tr>
        <%
                    }
                }
            }
        %>
    </table>
    <%
        } else {
            Response.Write("<p class='success'>没有发现乱码数据。</p>");
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
            
            fetch('/fix-encode-problem.ashx?action=fix', {
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
        string FixEncoding(string str) {
            if (string.IsNullOrEmpty(str)) return str;
            try {
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);
                string gbkStr = System.Text.Encoding.GetEncoding("GBK").GetString(utf8Bytes);
                byte[] gbkBytes = System.Text.Encoding.GetEncoding("GBK").GetBytes(gbkStr);
                string utf8Str = System.Text.Encoding.UTF8.GetString(gbkBytes);
                if (utf8Str != str) {
                    return utf8Str;
                }
            } catch { }
            return str;
        }
    %>
</body>
</html>
