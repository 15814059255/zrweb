<%@ Page Language="C#" AutoEventWireup="true" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8">
    <title>品牌数据初始化</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 40px; max-width: 600px; margin: 0 auto; }
        .btn { padding: 12px 30px; background: #007bff; color: white; border: none; border-radius: 6px; cursor: pointer; font-size: 16px; }
        .btn:hover { background: #0056b3; }
        .success { color: green; margin-top: 20px; padding: 15px; border: 1px solid #d4edda; border-radius: 6px; }
        .error { color: red; margin-top: 20px; padding: 15px; border: 1px solid #f8d7da; border-radius: 6px; }
        .info { color: #6c757d; margin-top: 10px; }
    </style>
</head>
<body>
    <h1>品牌数据初始化</h1>
    <p class="info">此页面将从电子元器件选型系统提取品牌数据，并存储到数据库中。</p>
    
    <button class="btn" onclick="initBrands()">开始初始化</button>
    
    <div id="result"></div>

    <script>
    function initBrands() {
        var result = document.getElementById('result');
        result.innerHTML = '<div style="color:#007bff;">正在初始化品牌数据...</div>';
        
        fetch('api/brands.aspx?action=init')
        .then(function(r) { return r.json(); })
        .then(function(data) {
            if (data.success) {
                result.innerHTML = '<div class="success">✓ 品牌数据初始化成功！<br>请访问 <a href="admin-brands.aspx">品牌管理</a> 查看品牌列表<br>或访问 <a href="profile.aspx">个人资料</a> 查看品牌下拉框</div>';
                testApi();
            } else {
                result.innerHTML = '<div class="error">✗ 初始化失败: ' + data.message + '</div>';
            }
        })
        .catch(function(err) {
            result.innerHTML = '<div class="error">✗ 请求失败: ' + err.message + '</div>';
        });
    }
    
    function testApi() {
        fetch('api/brands.aspx')
        .then(function(r) { return r.json(); })
        .then(function(data) {
            if (data.success && data.brands) {
                var result = document.getElementById('result');
                result.innerHTML += '<div class="info" style="margin-top:15px;">API 返回品牌数量: ' + data.brands.length + ' 个</div>';
            }
        });
    }
    </script>
</body>
</html>
