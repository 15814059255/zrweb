<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test-publish-diagnosis.aspx.cs" Inherits="test_publish_diagnosis" %>

<!DOCTYPE html>
<html>
<head>
    <title>发布功能诊断</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .section { margin: 20px 0; padding: 20px; border: 1px solid #ccc; }
        .success { color: green; }
        .error { color: red; }
        .info { color: blue; }
        .test-btn { padding: 10px 20px; background: #007bff; color: white; border: none; cursor: pointer; }
        .test-btn:hover { background: #0056b3; }
        textarea { width: 100%; height: 100px; }
    </style>
</head>
<body>
    <h1>发布供采功能诊断工具</h1>
    
    <div class="section">
        <h2>1. 数据库连接测试</h2>
        <button class="test-btn" onclick="testDbConnection()">测试数据库连接</button>
        <div id="dbResult"></div>
    </div>
    
    <div class="section">
        <h2>2. 直接插入测试</h2>
        <form id="testForm">
            <label>型号: <input type="text" name="goodsSn" value="TEST-MODEL-001"></label><br>
            <label>数量: <input type="text" name="goodsStock" value="1000"></label><br>
            <label>单位: <input type="text" name="goodsUnit" value="Kpcs"></label><br>
            <label>单价: <input type="text" name="shopPrice" value="0.1234"></label><br>
            <label>含税: <input type="checkbox" name="isIncludingTax" checked></label><br>
            <label>类型: 
                <select name="pubType">
                    <option value="1">供应</option>
                    <option value="2">需求</option>
                </select>
            </label><br>
            <button type="submit" class="test-btn">提交测试数据</button>
        </form>
        <div id="insertResult"></div>
    </div>
    
    <div class="section">
        <h2>3. 数据库查询验证</h2>
        <button class="test-btn" onclick="queryGoods()">查询最新数据</button>
        <div id="queryResult"></div>
    </div>

    <script>
        function testDbConnection() {
            const xhr = new XMLHttpRequest();
            xhr.open('GET', '/publish.ashx?action=test_connection', true);
            xhr.onload = function() {
                const result = document.getElementById('dbResult');
                try {
                    const data = JSON.parse(xhr.responseText);
                    if (data.success) {
                        result.innerHTML = '<span class="success">✓ 数据库连接成功！</span>';
                    } else {
                        result.innerHTML = '<span class="error">✗ 数据库连接失败: ' + data.message + '</span>';
                    }
                } catch (e) {
                    result.innerHTML = '<span class="error">✗ 服务器响应异常: ' + xhr.responseText + '</span>';
                }
            };
            xhr.onerror = function() {
                document.getElementById('dbResult').innerHTML = '<span class="error">✗ 网络请求失败</span>';
            };
            xhr.send();
        }

        document.getElementById('testForm').addEventListener('submit', function(e) {
            e.preventDefault();
            const formData = new FormData(this);
            formData.append('action', 'publish_goods');
            formData.append('isIncludingTax', this.isIncludingTax.checked ? '1' : '0');
            
            const xhr = new XMLHttpRequest();
            xhr.open('POST', '/publish.ashx', true);
            xhr.onload = function() {
                const result = document.getElementById('insertResult');
                try {
                    const data = JSON.parse(xhr.responseText);
                    if (data.success) {
                        result.innerHTML = '<span class="success">✓ 插入成功！</span>';
                    } else {
                        result.innerHTML = '<span class="error">✗ 插入失败: ' + data.message + '</span>';
                    }
                } catch (e) {
                    result.innerHTML = '<span class="error">✗ 服务器响应异常: ' + xhr.responseText + '</span>';
                }
            };
            xhr.onerror = function() {
                document.getElementById('insertResult').innerHTML = '<span class="error">✗ 网络请求失败</span>';
            };
            xhr.send(formData);
        });

        function queryGoods() {
            const xhr = new XMLHttpRequest();
            xhr.open('GET', '/publish.ashx?action=query_goods', true);
            xhr.onload = function() {
                const result = document.getElementById('queryResult');
                try {
                    const data = JSON.parse(xhr.responseText);
                    if (data.success) {
                        let html = '<span class="success">✓ 查询成功！共 ' + data.count + ' 条记录</span><br>';
                        html += '<textarea readonly>' + JSON.stringify(data.data, null, 2) + '</textarea>';
                        result.innerHTML = html;
                    } else {
                        result.innerHTML = '<span class="error">✗ 查询失败: ' + data.message + '</span>';
                    }
                } catch (e) {
                    result.innerHTML = '<span class="error">✗ 服务器响应异常: ' + xhr.responseText + '</span>';
                }
            };
            xhr.onerror = function() {
                document.getElementById('queryResult').innerHTML = '<span class="error">✗ 网络请求失败</span>';
            };
            xhr.send();
        }
    </script>
</body>
</html>