<%@ Page Language="C#" AutoEventWireup="true" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8">
    <title>品牌数据测试</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .test-section { margin: 20px 0; padding: 20px; border: 1px solid #ccc; border-radius: 8px; }
        .success { color: green; }
        .error { color: red; }
        .code { background: #f5f5f5; padding: 10px; border-radius: 4px; font-family: monospace; }
        button { padding: 10px 20px; background: #007bff; color: white; border: none; border-radius: 4px; cursor: pointer; }
        button:hover { background: #0056b3; }
    </style>
</head>
<body>
    <h1>品牌数据测试</h1>
    
    <div class="test-section">
        <h3>1. 检查数据库连接</h3>
        <div id="dbTest"></div>
    </div>
    
    <div class="test-section">
        <h3>2. 检查 brands 表是否存在</h3>
        <div id="tableTest"></div>
    </div>
    
    <div class="test-section">
        <h3>3. API 返回数据</h3>
        <button onclick="testApi()">测试 API</button>
        <div id="apiResult"></div>
    </div>
    
    <div class="test-section">
        <h3>4. 创建/初始化品牌数据</h3>
        <button onclick="initBrands()">初始化品牌数据</button>
        <div id="initResult"></div>
    </div>

    <script>
    function testApi() {
        var result = document.getElementById('apiResult');
        result.innerHTML = '<div class="code">正在请求 api/brands.aspx...</div>';
        
        fetch('api/brands.aspx')
        .then(function(r) {
            return r.text().then(function(text) {
                return { status: r.status, text: text };
            });
        })
        .then(function(data) {
            result.innerHTML = '<div class="code">状态码: ' + data.status + '<br>响应: ' + data.text + '</div>';
            try {
                var json = JSON.parse(data.text);
                if (json.success) {
                    result.innerHTML += '<div class="success">API 返回成功，品牌数量: ' + (json.brands ? json.brands.length : 0) + '</div>';
                }
            } catch(e) {
                result.innerHTML += '<div class="error">JSON 解析失败: ' + e.message + '</div>';
            }
        })
        .catch(function(err) {
            result.innerHTML = '<div class="error">请求失败: ' + err.message + '</div>';
        });
    }
    
    function initBrands() {
        var result = document.getElementById('initResult');
        result.innerHTML = '<div class="code">正在初始化品牌数据...</div>';
        
        fetch('api/brands.aspx', { method: 'POST', body: 'action=init' })
        .then(function(r) { return r.json(); })
        .then(function(data) {
            if (data.success) {
                result.innerHTML = '<div class="success">' + data.message + '</div>';
                testApi();
            } else {
                result.innerHTML = '<div class="error">' + data.message + '</div>';
            }
        })
        .catch(function(err) {
            result.innerHTML = '<div class="error">初始化失败: ' + err.message + '</div>';
        });
    }
    
    window.onload = function() {
        testApi();
    };
    </script>
</body>
</html>
