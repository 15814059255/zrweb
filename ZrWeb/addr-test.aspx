<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addr-test.aspx.cs" Inherits="addr_test" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>地址保存测试</title>
    <style>
        body { font-family: Arial; padding: 20px; max-width: 600px; margin: 0 auto; }
        h1 { color: #333; }
        .form-group { margin: 15px 0; }
        label { display: block; margin-bottom: 5px; font-weight: bold; }
        input, select { width: 100%; padding: 8px; box-sizing: border-box; }
        .btn { background: #007bff; color: white; padding: 10px 20px; border: none; cursor: pointer; margin-top: 20px; }
        .btn:hover { background: #0056b3; }
        .result { margin-top: 20px; padding: 15px; border-radius: 5px; }
        .success { background: #d4edda; color: #155724; }
        .error { background: #f8d7da; color: #721c24; }
        pre { background: #f4f4f4; padding: 10px; overflow-x: auto; }
    </style>
</head>
<body>
    <h1>地址保存测试</h1>
    
    <form id="addrForm">
        <div class="form-group">
            <label>省:</label>
            <select name="province" id="province">
                <option value="">请选择省</option>
            </select>
        </div>
        <div class="form-group">
            <label>市:</label>
            <select name="city" id="city">
                <option value="">请选择市</option>
            </select>
        </div>
        <div class="form-group">
            <label>区/县:</label>
            <select name="district" id="district">
                <option value="">请选择区/县</option>
            </select>
        </div>
        <div class="form-group">
            <label>街道:</label>
            <select name="street" id="street">
                <option value="">请选择街道</option>
            </select>
        </div>
        <div class="form-group">
            <label>详细地址:</label>
            <input type="text" name="address" placeholder="请输入详细地址" value="测试地址123号">
        </div>
        <button type="button" class="btn" onclick="saveAddress()">保存地址</button>
        <button type="button" class="btn" onclick="loadAddress()">加载地址</button>
    </form>
    
    <div id="result"></div>

    <script src="/assets/js/area-data.js"></script>
    <script>
    function saveAddress() {
        var form = document.getElementById('addrForm');
        var formData = new FormData(form);
        
        // 调试：显示要提交的数据
        console.log('提交的数据:');
        for (var pair of formData.entries()) {
            console.log(pair[0] + ': ' + pair[1]);
        }
        
        document.getElementById('result').innerHTML = '<p>正在保存...</p>';
        
        fetch('addr-test.aspx', {
            method: 'POST',
            body: formData
        })
        .then(r => r.json())
        .then(data => {
            var html = '<div class="result ' + (data.success ? 'success' : 'error') + '">';
            html += '<h3>保存结果</h3>';
            html += '<p>' + data.message + '</p>';
            if (data.debug) {
                html += '<pre>' + JSON.stringify(data.debug, null, 2) + '</pre>';
            }
            html += '</div>';
            document.getElementById('result').innerHTML = html;
        })
        .catch(err => {
            document.getElementById('result').innerHTML = '<div class="result error">请求失败: ' + err + '</div>';
        });
    }
    
    function loadAddress() {
        document.getElementById('result').innerHTML = '<p>正在加载...</p>';
        
        fetch('addr-test.aspx?action=load')
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                document.getElementById('province').value = data.province || '';
                // 触发省份change事件来加载城市
                document.getElementById('province').dispatchEvent(new Event('change'));
                setTimeout(function() {
                    document.getElementById('city').value = data.city || '';
                    document.getElementById('city').dispatchEvent(new Event('change'));
                    setTimeout(function() {
                        document.getElementById('district').value = data.district || '';
                        document.getElementById('district').dispatchEvent(new Event('change'));
                        setTimeout(function() {
                            document.getElementById('street').value = data.street || '';
                        }, 100);
                    }, 100);
                }, 100);
                document.querySelector('input[name="address"]').value = data.address || '';
                
                document.getElementById('result').innerHTML = '<div class="result success"><p>加载成功!</p></div>';
            } else {
                document.getElementById('result').innerHTML = '<div class="result error"><p>' + data.message + '</p></div>';
            }
        })
        .catch(err => {
            document.getElementById('result').innerHTML = '<div class="result error">加载失败: ' + err + '</div>';
        });
    }
    
    // 初始化地址选择器
    document.addEventListener('DOMContentLoaded', function() {
        initAddressSelector({});
    });
    </script>
</body>
</html>
