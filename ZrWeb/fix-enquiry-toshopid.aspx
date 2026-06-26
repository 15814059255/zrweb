<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>修复询价 toShopId 问题</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        button { padding: 10px 20px; background: #3498db; color: white; border: none; cursor: pointer; margin: 10px 0; }
        button:hover { background: #2980b9; }
        .result { margin-top: 20px; padding: 15px; border: 1px solid #ddd; }
    </style>
</head>
<body>
    <h1>修复询价 toShopId 问题</h1>
    
    <h2>问题描述</h2>
    <p>询价记录的 <code>toShopId</code> 字段可能为 NULL 或 0，导致供应商在 <code>received-inquiries.aspx</code> 页面无法看到询价信息。</p>
    
    <h2>当前状态检查</h2>
    <%
        string checkSql = "SELECT COUNT(*) FROM enquiryquoteprice WHERE eqType = 1 AND (toShopId IS NULL OR toShopId = 0)";
        object result = DbHelper.ExecuteScalar(checkSql);
        int badCount = result != null ? Convert.ToInt32(result) : 0;
        
        Response.Write("<p>toShopId 为 NULL 或 0 的询价记录数: <span class='" + (badCount > 0 ? "error" : "success") + "'>" + badCount + "</span></p>");
        
        if (badCount > 0) {
            Response.Write("<p class='warning'>需要修复这些记录，为它们补充正确的 toShopId 值。</p>");
        } else {
            Response.Write("<p class='success'>所有询价记录的 toShopId 都正确。</p>");
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
            
            fetch('/fix-enquiry-toshopid.ashx?action=fix', {
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
</body>
</html>
