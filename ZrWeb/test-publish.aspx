<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test-publish.aspx.cs" Inherits="test_publish" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8">
    <title>测试发布功能</title>
</head>
<body>
    <h1>测试发布功能</h1>
    
    <form method="post" action="/index.aspx">
        <input type="hidden" name="action" value="publish_goods">
        
        <div>
            <label>型号：</label>
            <input type="text" name="goodsSn" value="GRM188R71H104KA93D">
        </div>
        <div>
            <label>数量：</label>
            <input type="text" name="goodsStock" value="1000">
        </div>
        <div>
            <label>单位：</label>
            <input type="text" name="goodsUnit" value="Kpcs">
        </div>
        <div>
            <label>单价：</label>
            <input type="text" name="shopPrice" value="0.01">
        </div>
        <div>
            <label>含税：</label>
            <input type="text" name="isIncludingTax" value="0">
        </div>
        <div>
            <label>类型(1=供应, 2=需求)：</label>
            <input type="text" name="pubType" value="1">
        </div>
        <br>
        <input type="submit" value="提交测试">
    </form>
    
    <div id="result"></div>

    <script>
        document.querySelector('form').addEventListener('submit', function(e) {
            e.preventDefault();
            
            const formData = new FormData(this);
            const xhr = new XMLHttpRequest();
            xhr.open('POST', '/index.aspx', true);
            
            xhr.onload = function() {
                document.getElementById('result').innerHTML = '<h3>服务器响应：</h3>' + xhr.responseText;
            };
            
            xhr.onerror = function() {
                document.getElementById('result').innerHTML = '<h3>网络错误</h3>';
            };
            
            xhr.send(formData);
        });
    </script>
</body>
</html>