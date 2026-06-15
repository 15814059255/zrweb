<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test-inquiry.aspx.cs" Inherits="test_inquiry" %>

<!DOCTYPE html>
<html>
<head>
    <title>测试询价数据</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .section { margin: 20px 0; padding: 20px; border: 1px solid #ccc; }
        .success { color: green; }
        .error { color: red; }
        input { padding: 8px; margin: 5px; }
        button { padding: 10px 20px; background: #007bff; color: white; border: none; cursor: pointer; }
    </style>
</head>
<body>
    <h1>测试询价数据</h1>
    <p>此页面用于测试询价功能，会在 enquiryquoteprice 表中插入测试数据。</p>
    
    <div class="section">
        <h2>插入测试询价数据</h2>
        <form id="testForm">
            <label>型号: <input type="text" name="goodsSn" value="GRM188R71H104KA93D"></label><br>
            <label>采购数量: <input type="number" name="quantity" value="1000"></label><br>
            <label>期望单价: <input type="number" name="price" value="0.1234" step="0.0001"></label><br>
            <label>采购商公司: <input type="text" name="fromCompany" value="测试采购公司"></label><br>
            <label>联系人: <input type="text" name="fromContact" value="张三"></label><br>
            <label>联系电话: <input type="text" name="fromTel" value="13800138000"></label><br>
            <label>品牌: <input type="text" name="brandName" value="村田 Murata"></label><br>
            <label>备注: <input type="text" name="remarks" value="急需现货"></label><br>
            <button type="submit">插入测试数据</button>
        </form>
        <div id="result"></div>
    </div>

    <div class="section">
        <h2>查询当前询价数据</h2>
        <button onclick="queryData()">查询</button>
        <div id="queryResult"></div>
    </div>

    <script>
        document.getElementById('testForm').addEventListener('submit', function(e) {
            e.preventDefault();
            const formData = new FormData(this);
            
            fetch('test-inquiry.aspx', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                const result = document.getElementById('result');
                if (data.success) {
                    result.innerHTML = '<span class="success">✓ ' + data.message + '</span>';
                    queryData();
                } else {
                    result.innerHTML = '<span class="error">✗ ' + data.message + '</span>';
                }
            })
            .catch(error => {
                document.getElementById('result').innerHTML = '<span class="error">错误: ' + error + '</span>';
            });
        });

        function queryData() {
            fetch('test-inquiry.aspx?action=query')
            .then(response => response.json())
            .then(data => {
                const result = document.getElementById('queryResult');
                if (data.success) {
                    let html = '<p>共 ' + data.count + ' 条记录</p>';
                    html += '<table border="1" cellpadding="5" style="border-collapse:collapse">';
                    html += '<tr><th>ID</th><th>型号</th><th>采购商</th><th>数量</th><th>期望价</th><th>时间</th></tr>';
                    data.data.forEach(item => {
                        html += '<tr>';
                        html += '<td>' + item.eqId + '</td>';
                        html += '<td>' + item.goodsSn + '</td>';
                        html += '<td>' + item.fromCompany + '</td>';
                        html += '<td>' + item.fromQuantity + '</td>';
                        html += '<td>' + item.fromPrice + '</td>';
                        html += '<td>' + item.createTime + '</td>';
                        html += '</tr>';
                    });
                    html += '</table>';
                    result.innerHTML = html;
                } else {
                    result.innerHTML = '<span class="error">查询失败: ' + data.message + '</span>';
                }
            })
            .catch(error => {
                document.getElementById('queryResult').innerHTML = '<span class="error">错误: ' + error + '</span>';
            });
        }

        // 页面加载时自动查询
        queryData();
    </script>
</body>
</html>