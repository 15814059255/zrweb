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
    <style>
        .profile-card.business-info,
        .profile-card.cert-info {
            grid-column: 1 / -1;
        }
        .basic-info .profile-form-row-5 {
            display: flex;
            flex-wrap: nowrap;
            gap: 16px;
            align-items: center;
        }
        .basic-info .profile-form-item {
            display: flex;
            align-items: center;
            flex-direction: row !important;
            gap: 0;
            flex: 0 0 auto;
        }
        .basic-info .profile-form-item label {
            flex: 0 0 auto;
            text-align: right;
            font-size: 12px;
            color: #64748b;
            font-weight: 500;
            margin-right: 8px;
            white-space: nowrap;
        }
        .basic-info .profile-form-item .input {
            flex: 0 0 auto;
            width: auto !important;
            height: 40px;
            font-size: 12px;
            font-weight: 500;
        }
        .basic-info .profile-form-item .input.input-company { width: 200px !important; }
        .basic-info .profile-form-item .input.input-contact { width: 120px !important; }
        .basic-info .profile-form-item .input.input-phone { width: 160px !important; }
        .basic-info .profile-form-item .input.input-qq { width: 170px !important; }
        .basic-info .profile-form-item .input.input-email { width: 180px !important; }
    </style>
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
                        <div class="profile-info-item">
                            <span class="info-label">公司名称</span>
                            <span class="info-value"><%= CompanyName %></span>
                        </div>
                    </div>
                    <div class="profile-actions-top">
                        <button class="btn primary" type="button" id="editBtn">编辑资料</button>
                        <button class="btn primary" type="submit" id="saveBtn" form="profileForm" hidden>保存资料</button>
                        <button class="btn soft" type="button" id="cancelBtn" hidden>取消</button>
                        <button class="btn soft" type="button" id="changePwdBtn">修改密码</button>
                    </div>
                </div>
                <div class="profile-form">
                    <form id="profileForm" method="post" action="profile.aspx" enctype="multipart/form-data">
                        <input type="hidden" name="action" value="save_profile">
                        
                        <div class="profile-grid">
                            <div class="profile-card basic-info">
                                <div class="card-title">
                                    <span class="card-icon">🏢</span>
                                    <h3>基本信息</h3>
                                </div>
                                <div class="card-body">
                                    <div class="profile-form-row profile-form-row-5">
                                        <div class="profile-form-item"><label>公司名称</label><input class="input input-company" name="companyName" value="<%= CompanyName %>" disabled></div>
                                        <div class="profile-form-item"><label>联系人</label><input class="input input-contact" name="contactName" value="<%= ContactName %>" disabled></div>
                                        <div class="profile-form-item"><label>联系电话</label><input class="input input-phone" name="contactPhone" value="<%= OriginalContactPhone %>" disabled></div>
                                        <div class="profile-form-item"><label>QQ</label><input class="input input-qq" name="contactQQ" value="<%= ContactQQ %>" placeholder="请输入QQ号码" disabled></div>
                                        <div class="profile-form-item"><label>邮箱</label><input class="input input-email" name="contactEmail" value="<%= ContactEmail %>" placeholder="请输入邮箱地址" disabled></div>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="profile-card business-info">
                                <div class="card-title">
                                    <span class="card-icon">🏷️</span>
                                    <h3>业务信息</h3>
                                </div>
                                <div class="card-body">
                                    <div class="profile-form-group">
                                        <label>主营品牌</label>
                                        <div class="brands-select-grid">
                                            <select class="select" name="mainBrand1" id="mainBrand1" disabled><option value="">品牌1</option></select>
                                            <select class="select" name="mainBrand2" id="mainBrand2" disabled><option value="">品牌2</option></select>
                                            <select class="select" name="mainBrand3" id="mainBrand3" disabled><option value="">品牌3</option></select>
                                            <select class="select" name="mainBrand4" id="mainBrand4" disabled><option value="">品牌4</option></select>
                                            <select class="select" name="mainBrand5" id="mainBrand5" disabled><option value="">品牌5</option></select>
                                        </div>
                                    </div>
                                    <div class="profile-form-group">
                                        <label>优势型号</label>
                                        <div class="brands-select-grid">
                                            <input class="input" name="model1" id="model1" value="<%= Model1 %>" placeholder="型号1" disabled>
                                            <input class="input" name="model2" id="model2" value="<%= Model2 %>" placeholder="型号2" disabled>
                                            <input class="input" name="model3" id="model3" value="<%= Model3 %>" placeholder="型号3" disabled>
                                            <input class="input" name="model4" id="model4" value="<%= Model4 %>" placeholder="型号4" disabled>
                                            <input class="input" name="model5" id="model5" value="<%= Model5 %>" placeholder="型号5" disabled>
                                        </div>
                                    </div>
                                    <div class="profile-form-group">
                                        <label>公司简介</label>
                                        <textarea name="companyDescription" disabled><%= CompanyDescription %></textarea>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="profile-card cert-info">
                                <div class="card-title">
                                    <span class="card-icon">📄</span>
                                    <h3>资质认证</h3>
                                </div>
                                <div class="card-body">
                                    <div class="cert-grid">
                                        <div class="cert-item">
                                            <div class="cert-label">营业执照<% if (IsCertified) { %><span class="certified-badge">已认证</span><% } %></div>
                                            <div class="cert-upload" data-cert="business">
                                                <input type="file" name="businessLicense" accept="image/*" class="cert-file" disabled <%= IsCertified ? "readonly" : "" %>>
                                                <div class="cert-placeholder" <%= (IsCertified || !string.IsNullOrEmpty(BusinessLicense)) ? "hidden" : "" %>>
                                                    <span class="cert-icon">📄</span>
                                                    <span class="cert-text">上传营业执照</span>
                                                    <span class="cert-hint">支持 JPG/PNG/PDF，大小不超过 5MB</span>
                                                </div>
                                                <div class="cert-preview" <%= string.IsNullOrEmpty(BusinessLicense) ? "hidden" : "" %>>
                                                    <img src="<%= BusinessLicense %>" alt="营业执照" class="cert-img">
                                                    <% if (!IsCertified) { %><button class="cert-remove" type="button">移除</button><% } %>
                                                    <% if (IsCertified) { %><span class="certified-stamp">✓ 已认证</span><% } %>
                                                </div>
                                            </div>
                                            <% if (IsCertified) { %><span class="field-tip">认证后不可修改</span><% } %>
                                        </div>
                                        <div class="cert-item">
                                            <div class="cert-label">身份证<% if (IsCertified) { %><span class="certified-badge">已认证</span><% } %></div>
                                            <div class="cert-upload" data-cert="idcard">
                                                <input type="file" name="idCard" accept="image/*" class="cert-file" disabled <%= IsCertified ? "readonly" : "" %>>
                                                <div class="cert-placeholder" <%= (IsCertified || !string.IsNullOrEmpty(IDCard)) ? "hidden" : "" %>>
                                                    <span class="cert-icon">🆔</span>
                                                    <span class="cert-text">上传身份证</span>
                                                    <span class="cert-hint">支持 JPG/PNG，大小不超过 5MB</span>
                                                </div>
                                                <div class="cert-preview" <%= string.IsNullOrEmpty(IDCard) ? "hidden" : "" %>>
                                                    <img src="<%= IDCard %>" alt="身份证" class="cert-img">
                                                    <% if (!IsCertified) { %><button class="cert-remove" type="button">移除</button><% } %>
                                                    <% if (IsCertified) { %><span class="certified-stamp">✓ 已认证</span><% } %>
                                                </div>
                                            </div>
                                            <% if (IsCertified) { %><span class="field-tip">认证后不可修改</span><% } %>
                                        </div>
                                    </div>
                                    <div class="cert-safety-tip">
                                        <span class="tip-icon">🔒</span>
                                        <span class="tip-text">安全提示：您的证照信息将被严格保密，仅用于身份验证</span>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="profile-card">
                                <div class="card-title">
                                    <span class="card-icon">📍</span>
                                    <h3>收货地址</h3>
                                </div>
                                <div class="card-body">
                                    <div class="address-select-grid">
                                        <div class="form-row"><label>省</label><select class="select" name="province" id="selProvince" disabled><option value="">请选择省</option></select></div>
                                        <div class="form-row"><label>市</label><select class="select" name="city" id="selCity" disabled><option value="">请选择市</option></select></div>
                                        <div class="form-row"><label>区/县</label><select class="select" name="district" id="selDistrict" disabled><option value="">请选择区/县</option></select></div>
                                        <div class="form-row"><label>街道</label><select class="select" name="street" id="selStreet" disabled><option value="">请选择街道</option></select></div>
                                    </div>
                                    <div class="profile-form-group">
                                        <label>详细地址</label>
                                        <input class="input" name="address" value="<%= Address %>" placeholder="请输入详细地址，如街道、门牌号等" disabled>
                                    </div>
                                </div>
                            </div>
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
                try {
                    fetch('api/brands.aspx')
                    .then(r => r.json())
                    .then(data => {
                        if (data.success && data.brands) {
                            var selects = ['mainBrand1', 'mainBrand2', 'mainBrand3', 'mainBrand4', 'mainBrand5'];
                            var savedBrands = ['<%= jsEncode(MainBrand1) %>', '<%= jsEncode(MainBrand2) %>', '<%= jsEncode(MainBrand3) %>', '<%= jsEncode(MainBrand4) %>', '<%= jsEncode(MainBrand5) %>'];
                            
                            selects.forEach(function(selectId, index) {
                                var select = document.getElementById(selectId);
                                if (select) {
                                    select.innerHTML = '<option value="">品牌' + (index + 1) + '</option>';
                                    data.brands.forEach(function(brand) {
                                        var selected = (brand.BrandName === savedBrands[index]) ? ' selected' : '';
                                        select.innerHTML += '<option value="' + brand.BrandName + '"' + selected + '>' + brand.BrandName + '</option>';
                                    });
                                }
                            });
                        }
                    })
                    .catch(function(err) {
                        console.log('加载品牌失败:', err);
                    });
                } catch(e) {
                    console.log('loadBrands错误:', e);
                }
            }
            loadBrands();

            // 初始化地址选择器
            try {
                var addrOptions = {
                    province: '<%= jsEncode(Province) %>',
                    city: '<%= jsEncode(City) %>',
                    district: '<%= jsEncode(District) %>',
                    street: '<%= jsEncode(Street) %>'
                };
                
                if (typeof initAddressSelector === 'function') {
                    initAddressSelector(addrOptions);
                }
            } catch(e) {
                console.log('地址选择器初始化错误:', e);
            }

            // 证照上传预览
            var certUploads = document.querySelectorAll('.cert-upload');
            certUploads.forEach(function(upload) {
                var fileInput = upload.querySelector('.cert-file');
                var placeholder = upload.querySelector('.cert-placeholder');
                var preview = upload.querySelector('.cert-preview');
                var img = upload.querySelector('.cert-img');
                var removeBtn = upload.querySelector('.cert-remove');
                
                if (fileInput) {
                    fileInput.addEventListener('change', function(e) {
                        var file = e.target.files[0];
                        if (file) {
                            var reader = new FileReader();
                            reader.onload = function(event) {
                                if (img) img.src = event.target.result;
                                if (placeholder) placeholder.hidden = true;
                                if (preview) preview.hidden = false;
                            };
                            reader.readAsDataURL(file);
                        }
                    });
                }
                
                if (removeBtn) {
                    removeBtn.addEventListener('click', function(e) {
                        e.preventDefault();
                        if (fileInput) fileInput.value = '';
                        if (placeholder) placeholder.hidden = false;
                        if (preview) preview.hidden = true;
                    });
                }
            });

            var editBtn = document.getElementById('editBtn');
            var saveBtn = document.getElementById('saveBtn');
            var cancelBtn = document.getElementById('cancelBtn');
            var form = document.getElementById('profileForm');
            
            var isCertified = <%= IsCertified ? "true" : "false" %>;
            var companyModified = <%= CompanyModified ? "true" : "false" %>;
            
            editBtn.addEventListener('click', function() {
                var inputs = form.querySelectorAll('input:not([type="hidden"]):not([name="contactPhone"]), textarea, select');
                inputs.forEach(function(input) {
                    if (input.name === 'companyName' && companyModified) {
                        return;
                    }
                    if ((input.name === 'businessLicense' || input.name === 'idCard') && isCertified) {
                        return;
                    }
                    input.removeAttribute('disabled');
                });
                editBtn.hidden = true;
                saveBtn.hidden = false;
                cancelBtn.hidden = false;
            });
            
            cancelBtn.addEventListener('click', function() {
                var inputs = form.querySelectorAll('input:not([type="hidden"]):not([name="contactPhone"]), textarea, select');
                inputs.forEach(function(input) {
                    if (input.name === 'companyName' && companyModified) {
                        input.setAttribute('disabled', 'disabled');
                        return;
                    }
                    if ((input.name === 'businessLicense' || input.name === 'idCard') && isCertified) {
                        input.setAttribute('disabled', 'disabled');
                        return;
                    }
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
                .then(response => {
                    if (!response.ok) {
                        throw new Error('HTTP ' + response.status);
                    }
                    var contentType = response.headers.get('content-type');
                    return response.text().then(text => {
                        return { text: text, isJson: contentType && contentType.includes('application/json') };
                    });
                })
                .then(result => {
                    var text = result.text;
                    if (!text || text.trim() === '') {
                        throw new Error('空响应');
                    }
                    if (!result.isJson) {
                        throw new Error('非JSON响应');
                    }
                    try {
                        return JSON.parse(text);
                    } catch (parseError) {
                        throw new Error('JSON解析失败: ' + text.substring(0, 100));
                    }
                })
                .then(data => {
                    if (data.success) {
                        Toast.success('保存成功！');
                        setTimeout(function() { location.reload(); }, 1500);
                    } else {
                        Toast.error('保存失败：' + data.message);
                    }
                })
                .catch(error => {
                    Toast.error('保存异常：' + error.message);
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