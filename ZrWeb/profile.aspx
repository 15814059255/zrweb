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
                    <div class="avatar"><span class="avatar-placeholder">ZR</span></div>
                    <div class="profile-info">
                        <h2><%= CompanyName %></h2>
                        <p><%= ContactName %> · <%= ContactPhone %></p>
                    </div>
                </div>
                <div class="profile-form">
                    <form id="profileForm" method="post" action="profile.aspx">
                        <input type="hidden" name="action" value="save_profile">
                        <div class="form-section">
                            <h3>公司信息</h3>
                            <div class="form-row"><label>公司名称</label><input class="input" name="companyName" value="<%= CompanyName %>" disabled></div>
                            <div class="form-row"><label>主营品牌</label><input class="input" name="mainBrands" value="<%= MainBrands %>" disabled></div>
                            <div class="form-row"><label>经营能力</label><input class="input" name="businessCapability" value="<%= BusinessCapability %>" disabled></div>
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
                        <div class="form-actions">
                            <button class="btn primary" type="button" id="editBtn">编辑资料</button>
                            <button class="btn primary" type="submit" id="saveBtn" hidden>保存资料</button>
                            <button class="btn soft" type="button" id="cancelBtn" hidden>取消</button>
                            <button class="btn soft" type="button" id="changePwdBtn">修改密码</button>
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
            // 初始化地址选择器
            initAddressSelector({
                province: '<%= Province %>',
                city: '<%= City %>',
                district: '<%= District %>',
                street: '<%= Street %>'
            });

            var editBtn = document.getElementById('editBtn');
            var saveBtn = document.getElementById('saveBtn');
            var cancelBtn = document.getElementById('cancelBtn');
            var form = document.getElementById('profileForm');
            
            var inputs = form.querySelectorAll('input:not([type="hidden"]), textarea');
            
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
                        alert('保存成功！');
                        location.reload();
                    } else {
                        alert('保存失败：' + data.message);
                    }
                })
                .catch(error => {
                    alert('保存异常：' + error);
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
                    alert('请输入原密码');
                    return;
                }
                
                if (!newPassword) {
                    alert('请输入新密码');
                    return;
                }
                
                if (newPassword.length < 6) {
                    alert('新密码长度不能少于6位');
                    return;
                }
                
                if (newPassword !== confirmPassword) {
                    alert('两次输入的密码不一致');
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
                        alert('密码修改成功！请重新登录');
                        window.location.href = 'login.aspx';
                    } else {
                        alert('密码修改失败：' + data.message);
                    }
                })
                .catch(error => {
                    alert('密码修改异常：' + error);
                });
            });
        });
    </script>
</body>
</html>