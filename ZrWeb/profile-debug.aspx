<%@ Page Language="C#" AutoEventWireup="true" CodeFile="profile-debug.aspx.cs" Inherits="profile_debug" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>收货地址诊断工具</title>
    <style>
        body { font-family: Arial, sans-serif; padding: 20px; }
        .section { background: #f5f5f5; padding: 15px; margin: 10px 0; border-radius: 5px; }
        .success { background: #d4edda; color: #155724; }
        .error { background: #f8d7da; color: #721c24; }
        .warning { background: #fff3cd; color: #856404; }
        h2 { margin-top: 0; }
        pre { background: #333; color: #0f0; padding: 10px; overflow-x: auto; }
        .btn { padding: 10px 20px; background: #007bff; color: white; border: none; cursor: pointer; margin: 5px; }
        .btn:hover { background: #0056b3; }
    </style>
</head>
<body>
    <h1>收货地址诊断工具</h1>
    
    <div class="section">
        <h2>1. 检查数据库字段</h2>
        <div id="checkFieldsResult"></div>
        <button class="btn" onclick="checkFields()">检查字段</button>
    </div>
    
    <div class="section">
        <h2>2. 测试保存地址</h2>
        <form id="saveForm">
            <label>省: <input type="text" name="province" value="广东省" /></label><br><br>
            <label>市: <input type="text" name="city" value="深圳市" /></label><br><br>
            <label>区/县: <input type="text" name="district" value="南山区" /></label><br><br>
            <label>街道: <input type="text" name="street" value="粤海街道" /></label><br><br>
            <label>详细地址: <input type="text" name="address" value="科技园路1号" /></label><br><br>
            <button type="button" class="btn" onclick="testSave()">保存测试</button>
        </form>
        <div id="saveResult"></div>
    </div>
    
    <div class="section">
        <h2>3. 读取当前地址</h2>
        <button class="btn" onclick="loadAddress()">读取地址</button>
        <div id="loadResult"></div>
    </div>
    
    <div class="section">
        <h2>4. 完整测试（保存后读取）</h2>
        <button class="btn" onclick="fullTest()">完整测试</button>
        <div id="fullResult"></div>
    </div>

    <script src="/assets/js/toast.js"></script>
    <script>
    function checkFields() {
        fetch('profile-debug.aspx?action=check_fields')
            .then(r => r.json())
            .then(data => {
                var html = '';
                if (data.hasFields) {
                    html = '<p class="success">字段已存在: Province, City, District, Street, Address</p>';
                    html += '<p>当前用户地址:</p>';
                    html += '<pre>' + JSON.stringify(data.currentAddress, null, 2) + '</pre>';
                } else {
                    html = '<p class="error">字段不存在！需要执行以下SQL：</p>';
                    html += '<pre>ALTER TABLE [dbo].[userinfo] ADD [Province] nvarchar(50) NULL\n';
                    html += 'ALTER TABLE [dbo].[userinfo] ADD [City] nvarchar(50) NULL\n';
                    html += 'ALTER TABLE [dbo].[userinfo] ADD [District] nvarchar(50) NULL\n';
                    html += 'ALTER TABLE [dbo].[userinfo] ADD [Street] nvarchar(50) NULL\n';
                    html += 'ALTER TABLE [dbo].[userinfo] ADD [Address] nvarchar(255) NULL</pre>';
                }
                document.getElementById('checkFieldsResult').innerHTML = html;
            })
            .catch(err => {
                document.getElementById('checkFieldsResult').innerHTML = 
                    '<p class="error">检查失败: ' + err + '</p>';
            });
    }
    
    function testSave() {
        var form = document.getElementById('saveForm');
        var formData = new FormData(form);
        formData.append('action', 'test_save');
        
        fetch('profile-debug.aspx', {
            method: 'POST',
            body: formData
        })
        .then(r => r.json())
        .then(data => {
            var html = '<p class="' + (data.success ? 'success' : 'error') + '">' + data.message + '</p>';
            html += '<pre>' + JSON.stringify(data, null, 2) + '</pre>';
            document.getElementById('saveResult').innerHTML = html;
        })
        .catch(err => {
            document.getElementById('saveResult').innerHTML = 
                '<p class="error">保存失败: ' + err + '</p>';
        });
    }
    
    function loadAddress() {
        fetch('profile-debug.aspx?action=load_address')
            .then(r => r.json())
            .then(data => {
                var html = '<p>当前地址:</p>';
                html += '<pre>' + JSON.stringify(data, null, 2) + '</pre>';
                document.getElementById('loadResult').innerHTML = html;
            })
            .catch(err => {
                document.getElementById('loadResult').innerHTML = 
                    '<p class="error">读取失败: ' + err + '</p>';
            });
    }
    
    function fullTest() {
        document.getElementById('fullResult').innerHTML = '<p>测试中...</p>';
        
        // 1. 先检查字段
        fetch('profile-debug.aspx?action=check_fields')
            .then(r => r.json())
            .then(data => {
                if (!data.hasFields) {
                    document.getElementById('fullResult').innerHTML = 
                        '<p class="error">数据库字段不存在，请先执行SQL创建字段</p>';
                    return;
                }
                
                // 2. 保存测试数据
                var formData = new FormData();
                formData.append('action', 'test_save');
                formData.append('province', '广东省');
                formData.append('city', '深圳市');
                formData.append('district', '南山区');
                formData.append('street', '粤海街道');
                formData.append('address', '科技园路100号');
                
                return fetch('profile-debug.aspx', { method: 'POST', body: formData });
            })
            .then(r => r.json())
            .then(data => {
                if (!data.success) {
                    document.getElementById('fullResult').innerHTML = 
                        '<p class="error">保存失败: ' + data.message + '</p>';
                    return;
                }
                
                // 3. 读取验证
                return fetch('profile-debug.aspx?action=load_address');
            })
            .then(r => r.json())
            .then(data => {
                var html = '<p class="success">完整测试成功！</p>';
                html += '<p>保存并读取到的地址:</p>';
                html += '<pre>' + JSON.stringify(data, null, 2) + '</pre>';
                
                // 验证数据是否正确
                if (data.Province === '广东省' && data.City === '深圳市' && 
                    data.District === '南山区' && data.Street === '粤海街道' && data.Address === '科技园路100号') {
                    html += '<p class="success">数据验证通过！地址保存和读取功能正常。</p>';
                } else {
                    html += '<p class="warning">数据不匹配，请检查保存逻辑</p>';
                }
                
                document.getElementById('fullResult').innerHTML = html;
            })
            .catch(err => {
                document.getElementById('fullResult').innerHTML = 
                    '<p class="error">测试失败: ' + err + '</p>';
            });
    }
    
    // 页面加载时自动检查
    checkFields();
    </script>
</body>
</html>
