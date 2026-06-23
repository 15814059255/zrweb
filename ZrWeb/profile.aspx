<%@ Page Language="C#" AutoEventWireup="true" CodeFile="profile.aspx.cs" Inherits="profile" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<%@ Register Src="~/UserControls/head.ascx" TagPrefix="uc1" TagName="head" %>
<%@ Register Src="~/UserControls/bottom.ascx" TagPrefix="uc1" TagName="bottom" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><%= PageTitle %></title>
    <meta name="keywords" content="<%= PageKeywords %>">
    <meta name="description" content="<%= PageDescription %>">
    <link rel="stylesheet" href="/assets/css/styles.css">
    <script src="/assets/js/area-data.js"></script>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div><h1>个人资料</h1></div>
                <div class="actions"><a class="btn back" href="/index.aspx" data-back>返回</a></div>
            </header>
            <section class="panel profile-panel">
                <div class="profile-header">
                    <div class="profile-info">
                        <h2><%= CompanyName %></h2>
                        <p><%= ContactName %> · <%= ContactPhone %></p>
                    </div>
                    <div class="profile-actions-top">
                        <button class="btn primary" type="button" id="editBtn">编辑资料</button>
                        <button class="btn primary" type="submit" id="saveBtn" form="profileForm" hidden>保存资料</button>
                        <button class="btn soft" type="button" id="cancelBtn" hidden>取消</button>
                        <button class="btn soft" type="button" id="changePwdBtn">修改密码</button>
                    </div>
                </div>
                <div class="profile-form">
                    <form id="profileForm" method="post" action="profile.aspx">
                        <input type="hidden" name="action" value="save_profile">
                        <div class="form-section">
                            <h3>公司信息</h3>
                            <div class="form-row"><label>公司名称</label><input class="input" name="companyName" value="<%= CompanyName %>" disabled></div>
                            <div class="form-row">
                                <label>主营品牌</label>
                                <div class="brands-select-grid">
                                    <select class="select" name="mainBrand1" id="mainBrand1" disabled><option value="">品牌1</option></select>
                                    <select class="select" name="mainBrand2" id="mainBrand2" disabled><option value="">品牌2</option></select>
                                    <select class="select" name="mainBrand3" id="mainBrand3" disabled><option value="">品牌3</option></select>
                                    <select class="select" name="mainBrand4" id="mainBrand4" disabled><option value="">品牌4</option></select>
                                    <select class="select" name="mainBrand5" id="mainBrand5" disabled><option value="">品牌5</option></select>
                                </div>
                            </div>
                            <div class="form-row"><label>公司简介</label><textarea name="companyDescription" disabled><%= CompanyDescription %></textarea></div>
                        </div>
                        <div class="form-section">
                            <h3>联系人信息</h3>
                            <div class="form-row"><label>联系人</label><input class="input" name="contactName" value="<%= ContactName %>" disabled></div>
                            <div class="form-row"><label>联系电话</label><input class="input" name="contactPhone" value="<%= ContactPhone %>" disabled></div>
                        </div>
                        <div class="form-section">
                            <h3>收货地址</h3>
                            <div class="address-select-grid">
                                <div class="form-row"><label>省</label><select class="select" name="province" id="selProvince" disabled><option value="">请选择省</option></select></div>
                                <div class="form-row"><label>市</label><select class="select" name="city" id="selCity" disabled><option value="">请选择市</option></select></div>
                                <div class="form-row"><label>区/县</label><select class="select" name="district" id="selDistrict" disabled><option value="">请选择区/县</option></select></div>
                                <div class="form-row"><label>街道</label><select class="select" name="street" id="selStreet" disabled><option value="">请选择街道</option></select></div>
                            </div>
                            <div class="form-row address-detail"><label>详细地址</label><input class="input" name="address" value="<%= Address %>" placeholder="请输入详细地址，如街道、门牌号等" disabled></div>
                        </div>
                    </form>
                </div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
    <div class="modal-backdrop" id="changePwdModal" hidden>
        <div class="modal" role="dialog" aria-modal="true" aria-label="修改密码">
            <div class="modal-head"><h2>修改密码</h2><button class="modal-close" type="button" id="closePwdModal" aria-label="关闭">×</button></div>
            <div class="modal-body">
                <form id="changePwdForm">
                    <input type="hidden" name="action" value="change_password">
                    <div class="form-row"><label>原密码</label><input class="input" type="password" name="oldPassword" placeholder="请输入原密码"></div>
                    <div class="form-row"><label>新密码</label><input class="input" type="password" name="newPassword" placeholder="请输入新密码"></div>
                    <div class="form-row"><label>确认密码</label><input class="input" type="password" name="confirmPassword" placeholder="请确认新密码"></div>
                    <div class="form-actions" style="justify-content: flex-end;">
                        <button class="btn" type="button" id="cancelPwdBtn">取消</button>
                        <button class="btn primary" type="submit" id="submitPwdBtn">确认修改</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // 安全转义 JavaScript 字符串
            function jsEncode(str) {
                if (!str) return '';
                return str.replace(/\\/g, '\\\\').replace(/'/g, "\\'").replace(/"/g, '\\"').replace(/\n/g, '\\n').replace(/\r/g, '\\r');
            }
            
            // 加载品牌列表
            function loadBrands() {
                fetch('api/brands.aspx')
                .then(r => r.json())
                .then(data => {
                    if (data.success && data.brands) {
                        var selects = ['mainBrand1', 'mainBrand2', 'mainBrand3', 'mainBrand4', 'mainBrand5'];
                        var savedBrands = [<%= jsEncode(MainBrand1) %>, <%= jsEncode(MainBrand2) %>, <%= jsEncode(MainBrand3) %>, <%= jsEncode(MainBrand4) %>, <%= jsEncode(MainBrand5) %>];
                        
                        selects.forEach(function(selectId, index) {
                            var select = document.getElementById(selectId);
                            select.innerHTML = '<option value="">品牌' + (index + 1) + '</option>';
                            data.brands.forEach(function(brand) {
                                var selected = (brand.BrandName === savedBrands[index]) ? ' selected' : '';
                                select.innerHTML += '<option value="' + brand.BrandName + '"' + selected + '>' + brand.BrandName + '</option>';
                            });
                        });
                    }
                });
            }
            loadBrands();

            // 初始化地址选择器
            var addrOptions = {
                province: '<%= jsEncode(Province) %>',
                city: '<%= jsEncode(City) %>',
                district: '<%= jsEncode(District) %>',
                street: '<%= jsEncode(Street) %>'
            };
            
            initAddressSelector(addrOptions);

            var editBtn = document.getElementById('editBtn');
            var saveBtn = document.getElementById('saveBtn');
            var cancelBtn = document.getElementById('cancelBtn');
            var form = document.getElementById('profileForm');
            
            // 获取所有表单元素（包括 input、textarea、select）
            var inputs = form.querySelectorAll('input:not([type="hidden"]), textarea, select');
            
            editBtn.addEventListener('click', function() {
                inputs.forEach(function(input) {
                    input.removeAttribute('disabled');
                });
                editBtn.hidden = true;
                saveBtn.hidden = false;
                cancelBtn.hidden = false;
            });
            
            cancelBtn.addEventListener('click', function() {
                inputs.forEach(function(input) {
                    input.setAttribute('disabled', 'disabled');
                });
                editBtn.hidden = false;
                saveBtn.hidden = true;
                cancelBtn.hidden = true;
                location.reload();
            });
            
            form.addEventListener('submit', function(e) {
                e.preventDefault();
                
                var formData = new FormData(form);
                
                fetch('profile.aspx', {
                    method: 'POST',
                    body: formData
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        Toast.success('保存成功！');
                        setTimeout(function() { location.reload(); }, 1500);
                    } else {
                        Toast.error('保存失败：' + data.message);
                    }
                })
                .catch(error => {
                    Toast.error('保存异常：' + error);
                });
            });
            
            var changePwdBtn = document.getElementById('changePwdBtn');
            var changePwdModal = document.getElementById('changePwdModal');
            var closePwdModal = document.getElementById('closePwdModal');
            var cancelPwdBtn = document.getElementById('cancelPwdBtn');
            var changePwdForm = document.getElementById('changePwdForm');
            
            changePwdBtn.addEventListener('click', function() {
                changePwdModal.hidden = false;
            });
            
            closePwdModal.addEventListener('click', function() {
                changePwdModal.hidden = true;
            });
            
            cancelPwdBtn.addEventListener('click', function() {
                changePwdModal.hidden = true;
            });
            
            changePwdForm.addEventListener('submit', function(e) {
                e.preventDefault();
                
                var oldPassword = changePwdForm.querySelector('input[name="oldPassword"]').value;
                var newPassword = changePwdForm.querySelector('input[name="newPassword"]').value;
                var confirmPassword = changePwdForm.querySelector('input[name="confirmPassword"]').value;
                
                if (!oldPassword) {
                    Toast.warning('请输入原密码');
                    return;
                }
                
                if (!newPassword) {
                    Toast.warning('请输入新密码');
                    return;
                }
                
                if (newPassword.length < 6) {
                    Toast.warning('新密码长度不能少于6位');
                    return;
                }
                
                if (newPassword !== confirmPassword) {
                    Toast.error('两次输入的密码不一致');
                    return;
                }
                
                var formData = new FormData(changePwdForm);
                
                fetch('profile.aspx', {
                    method: 'POST',
                    body: formData
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        Toast.success('密码修改成功！请重新登录');
                        setTimeout(function() { window.location.href = 'login.aspx'; }, 1500);
                    } else {
                        Toast.error('密码修改失败：' + data.message);
                    }
                })
                .catch(error => {
                    Toast.error('密码修改异常：' + error);
                });
            });
        });
    </script>
</body>
</html>