<%@ Page Language="C#" %>
<%@ Import Namespace="System.Text" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>测试编码修复</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .success { color: #27ae60; font-weight: bold; font-size: 18px; }
        .error { color: #e74c3c; font-weight: bold; }
        .warning { color: #f39c12; }
        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; font-size: 12px; }
        th { background-color: #f5f5f5; }
        .hex { font-family: monospace; font-size: 11px; }
        textarea { width: 100%; height: 100px; margin-bottom: 10px; }
        button { padding: 10px 20px; background: #3498db; color: white; border: none; cursor: pointer; }
        button:hover { background: #2980b9; }
    </style>
</head>
<body>
    <h1>测试编码修复</h1>
    
    <h2>1. 手动测试乱码转换</h2>
    <textarea id="inputStr">娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃</textarea>
    <button onclick="testEncoding()">测试转换</button>
    
    <div id="result"></div>
    
    <h2>2. 已知乱码测试</h2>
    <table>
        <tr><th>乱码</th><th>期望结果</th><th>实际修复结果</th><th>是否成功</th></tr>
        <tr>
            <td>娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃</td>
            <td>深圳市浩之东科技有限公司</td>
            <td><%= DbHelper.FixEncoding("娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃") %></td>
            <td><%= DbHelper.FixEncoding("娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃") == "深圳市浩之东科技有限公司" ? "<span class='success'>是</span>" : "<span class='error'>否</span>" %></td>
        </tr>
        <tr>
            <td>姹借溅</td>
            <td>汽车</td>
            <td><%= DbHelper.FixEncoding("姹借溅") %></td>
            <td><%= DbHelper.FixEncoding("姹借溅") == "汽车" ? "<span class='success'>是</span>" : "<span class='error'>否</span>" %></td>
        </tr>
    </table>
    
    <h2>3. 编码转换过程详解</h2>
    <%
        string testStr = "娣卞湷甯傛旦涔嬩笢绉戞妧鏈夐檺鍏徃";
        Response.Write("<p>输入: " + testStr + "</p>");
        
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(testStr);
        Response.Write("<p>UTF-8 字节: " + BitConverter.ToString(utf8Bytes) + "</p>");
        
        string gbkFromUtf8 = Encoding.GetEncoding("GBK").GetString(utf8Bytes);
        Response.Write("<p>UTF-8字节 → GBK解码: " + gbkFromUtf8 + "</p>");
        
        byte[] gbkBytes = Encoding.GetEncoding("GBK").GetBytes(gbkFromUtf8);
        Response.Write("<p>GBK字符串 → GBK字节: " + BitConverter.ToString(gbkBytes) + "</p>");
        
        string utf8FromGbk = Encoding.UTF8.GetString(gbkBytes);
        Response.Write("<p>GBK字节 → UTF-8解码: " + utf8FromGbk + "</p>");
        
        Response.Write("<p class='" + (utf8FromGbk == "深圳市浩之东科技有限公司" ? "success" : "error") + "'>最终结果: " + utf8FromGbk + "</p>");
    %>
    
    <h2>4. 当前FixEncoding方法输出</h2>
    <%
        string fixedResult = DbHelper.FixEncoding(testStr);
        Response.Write("<p>DbHelper.FixEncoding('" + testStr + "') = '" + fixedResult + "'</p>");
        Response.Write("<p>是否等于 '深圳市浩之东科技有限公司': " + (fixedResult == "深圳市浩之东科技有限公司" ? "<span class='success'>是</span>" : "<span class='error'>否</span>") + "</p>");
    %>
    
    <script>
        function testEncoding() {
            var input = document.getElementById('inputStr').value;
            var resultDiv = document.getElementById('result');
            
            fetch('/test-fix-encoding.ashx?action=fix&str=' + encodeURIComponent(input))
            .then(function(response) {
                return response.text();
            })
            .then(function(data) {
                resultDiv.innerHTML = '<p class="success">修复结果: ' + data + '</p>';
            })
            .catch(function(error) {
                resultDiv.innerHTML = '<p class="error">错误: ' + error + '</p>';
            });
        }
    </script>
</body>
</html>
