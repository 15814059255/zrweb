<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addr-simple.aspx.cs" Inherits="addr_simple" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>地址保存测试</title>
    <style>
        body { font-family: Arial; padding: 20px; }
        .box { border: 1px solid #ccc; padding: 15px; margin: 10px 0; }
        .success { background: #d4edda; border-color: #28a745; }
        .error { background: #f8d7da; border-color: #dc3545; }
        input { padding: 8px; width: 200px; margin: 5px; }
        button { padding: 10px 20px; margin: 5px; cursor: pointer; }
    </style>
</head>
<body>
    <h1>地址保存测试</h1>
    
    <div class="box">
        <h3>当前保存的地址：</h3>
        <p id="currentAddr">加载中...</p>
        <button onclick="loadAddr()">刷新</button>
    </div>
    
    <div class="box">
        <h3>保存新地址：</h3>
        <input type="text" id="txtProvince" placeholder="省"><br>
        <input type="text" id="txtCity" placeholder="市"><br>
        <input type="text" id="txtDistrict" placeholder="区/县"><br>
        <input type="text" id="txtStreet" placeholder="街道"><br>
        <input type="text" id="txtAddress" placeholder="详细地址"><br><br>
        <button onclick="saveAddr()">保存地址</button>
    </div>
    
    <div id="result"></div>

    <script>
    function loadAddr() {
        fetch('addr-simple.aspx?action=load')
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                document.getElementById('currentAddr').innerHTML = 
                    data.province + ' ' + data.city + ' ' + data.district + ' ' + data.street + '<br>' + data.address;
                document.getElementById('result').innerHTML = '';
            } else {
                document.getElementById('currentAddr').innerHTML = '未保存或保存失败';
                document.getElementById('result').innerHTML = '<div class="error">' + data.message + '</div>';
            }
        });
    }
    
    function saveAddr() {
        var province = document.getElementById('txtProvince').value;
        var city = document.getElementById('txtCity').value;
        var district = document.getElementById('txtDistrict').value;
        var street = document.getElementById('txtStreet').value;
        var address = document.getElementById('txtAddress').value;
        
        var formData = new FormData();
        formData.append('province', province);
        formData.append('city', city);
        formData.append('district', district);
        formData.append('street', street);
        formData.append('address', address);
        
        fetch('addr-simple.aspx', { method: 'POST', body: formData })
        .then(r => r.json())
        .then(data => {
            var div = document.getElementById('result');
            div.className = data.success ? 'box success' : 'box error';
            div.innerHTML = data.message;
            if (data.success) {
                loadAddr();
            }
        });
    }
    
    // 加载当前地址
    loadAddr();
    </script>
</body>
</html>
