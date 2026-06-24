<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-user-detail.aspx.cs" Inherits="admin_user_detail" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>会员详情 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
</head>
<body class="admin-page">
    <div class="admin-app">
        <aside class="admin-sidebar">
            <div class="admin-brand">
                <div class="admin-logo-icon">ZR</div>
                <div class="admin-brand-text">
                    <strong>阻容网</strong>
                    <span>管理员后台</span>
                </div>
            </div>
            <nav class="admin-nav">
                <a href="admin-console.aspx">
                    <span class="nav-icon">📊</span>
                    <span>控制台</span>
                </a>
                <a href="admin-users.aspx">
                    <span class="nav-icon">👥</span>
                    <span>用户管理</span>
                </a>
                <a href="admin-goods.aspx">
                    <span class="nav-icon">📦</span>
                    <span>供需管理</span>
                </a>
                <a href="admin-quotes.aspx">
                    <span class="nav-icon">💰</span>
                    <span>报价管理</span>
                </a>
                <a href="admin-ads.aspx">
                    <span class="nav-icon">📢</span>
                    <span>广告管理</span>
                </a>
            </nav>
            <div class="admin-sidebar-footer">
                <div class="admin-user-info">
                    <span><%= AdminName %></span>
                    <small>超级管理员</small>
                </div>
                <a href="admin-login.aspx?action=logout" class="admin-logout">退出登录</a>
            </div>
        </aside>
        <main class="admin-main">
            <header class="admin-topbar">
                <div>
                    <h1>会员详情</h1>
                    <p class="admin-breadcrumb"><a href="admin-users.aspx">用户管理</a> › 会员详情</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="admin-detail-header">
                    <div class="admin-detail-title">
                        <h2>U<%= UserID %></h2>
                        <span class="tag <%= IsCheck == 1 ? "green" : "orange" %>"><%= IsCheck == 1 ? "已审核" : "待审核" %></span>
                        <span class="tag <%= SysStatus == 0 ? "green" : "red" %>"><%= SysStatus == 0 ? "正常" : "已删除" %></span>
                    </div>
                    <div class="admin-detail-actions">
                        <button class="btn" onclick="window.location.href='admin-users.aspx'">返回列表</button>
                        <button class="btn mini" onclick="checkUser(<%= UserID %>, <%= IsCheck == 1 ? 0 : 1 %>)">
                            <%= IsCheck == 1 ? "取消审核" : "审核通过" %>
                        </button>
                        <button class="btn mini" onclick="toggleUserStatus(<%= UserID %>, <%= SysStatus == 0 ? 1 : 0 %>)">
                            <%= SysStatus == 0 ? "删除" : "恢复" %>
                        </button>
                    </div>
                </div>
                
                <div class="admin-detail-grid">
                    <div class="admin-detail-section">
                        <h3>基本信息</h3>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">会员ID</span>
                            <span class="admin-detail-value">U<%= UserID %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">用户名</span>
                            <span class="admin-detail-value"><%= UserName %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">注册时间</span>
                            <span class="admin-detail-value"><%= CreateTime %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">审核状态</span>
                            <span class="admin-detail-value"><%= IsCheck == 1 ? "已审核" : "待审核" %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">系统状态</span>
                            <span class="admin-detail-value"><%= SysStatus == 0 ? "正常" : "已删除" %></span>
                        </div>
                    </div>
                    
                    <div class="admin-detail-section">
                        <h3>联系人信息</h3>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">联系人</span>
                            <span class="admin-detail-value"><%= LinkMan %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">手机号</span>
                            <span class="admin-detail-value"><%= MobilePhone %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">QQ</span>
                            <span class="admin-detail-value"><%= QQ %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">邮箱</span>
                            <span class="admin-detail-value"><%= Email %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">重置密码</span>
                            <span class="admin-detail-value" style="display:flex;align-items:center;gap:8px;flex-wrap:nowrap;">
                                <input type="text" id="txtNewPassword" class="input" style="width:150px;" placeholder="输入新密码">
                                <button class="btn mini" onclick="resetPassword(<%= UserID %>)">重置</button>
                            </span>
                        </div>
                    </div>
                    
                    <div class="admin-detail-section">
                        <h3>公司信息</h3>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">公司名称</span>
                            <span class="admin-detail-value">
                                <input type="text" id="txtCompanyName" class="input" style="width:250px;display:inline-block;" value="<%= CompanyName %>" placeholder="公司名称">
                                <button class="btn mini" onclick="saveCompanyName(<%= UserID %>)">保存</button>
                            </span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">地址</span>
                            <span class="admin-detail-value"><%= Province %> <%= City %> <%= District %> <%= Street %> <%= Address %></span>
                        </div>
                    </div>
                    
                    <div class="admin-detail-section">
                        <h3>资质认证</h3>
                        <div class="admin-cert-grid">
                            <div class="admin-cert-item">
                                <div class="admin-cert-label">营业执照</div>
                                <div class="admin-cert-preview">
                                    <% if (!string.IsNullOrEmpty(BusinessLicense)) { %>
                                    <img src="<%= BusinessLicense %>" alt="营业执照" class="admin-cert-img" onclick="previewImage('<%= BusinessLicense %>')">
                                    <% } else { %>
                                    <div class="admin-cert-empty">暂无上传</div>
                                    <% } %>
                                </div>
                            </div>
                            <div class="admin-cert-item">
                                <div class="admin-cert-label">身份证</div>
                                <div class="admin-cert-preview">
                                    <% if (!string.IsNullOrEmpty(IDCard)) { %>
                                    <img src="<%= IDCard %>" alt="身份证" class="admin-cert-img" onclick="previewImage('<%= IDCard %>')">
                                    <% } else { %>
                                    <div class="admin-cert-empty">暂无上传</div>
                                    <% } %>
                                </div>
                            </div>
                        </div>
                        <div class="admin-cert-status">
                            <span class="admin-cert-label">认证状态：</span>
                            <span class="tag <%= IsCertified == 1 ? "green" : "orange" %>"><%= IsCertified == 1 ? "已认证" : "待审核" %></span>
                        </div>
                        <div class="admin-cert-actions">
                            <% if (IsCertified == 0) { %>
                            <button class="btn mini" onclick="certifyUser(<%= UserID %>, 1)">审核通过</button>
                            <button class="btn mini" onclick="certifyUser(<%= UserID %>, 0)">审核不通过</button>
                            <% } else { %>
                            <button class="btn mini" onclick="certifyUser(<%= UserID %>, 0)">取消认证</button>
                            <% } %>
                        </div>
                        <div class="admin-cert-tip">
                            <span class="tip-icon">💡</span>
                            <span>审核通过后，会员无法修改上传的证照；审核不通过，会员可重新上传一次</span>
                        </div>
                    </div>
                    
                    <div class="admin-detail-section">
                        <h3>账号信息</h3>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">用户类型</span>
                            <span class="admin-detail-value">
                                <select id="ddlRole" class="input" style="width:auto;display:inline-block;">
                                    <option value="1" <%= RoseID == 1 ? "selected" : "" %>>普通用户（未选择类型）</option>
                                    <option value="2" <%= RoseID == 2 ? "selected" : "" %>>采购商（只能发布采购需求）</option>
                                    <option value="3" <%= RoseID == 3 ? "selected" : "" %>>供应商（可发布供应和采购）</option>
                                </select>
                                <button class="btn mini" onclick="changeRole(<%= UserID %>)">修改</button>
                            </span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">注册IP</span>
                            <span class="admin-detail-value"><%= Source %></span>
                        </div>
                        <div class="admin-detail-row">
                            <span class="admin-detail-label">身份证信息</span>
                            <span class="admin-detail-value" style="display:flex;align-items:center;gap:8px;flex-wrap:nowrap;">
                                <input type="text" id="txtIDCardName" class="input" style="width:120px;" value="<%= IDCardName %>" placeholder="姓名">
                                <input type="text" id="txtIDCardNumber" class="input" style="width:180px;" value="<%= IDCardNumber %>" placeholder="身份证号码">
                                <button class="btn mini" onclick="saveIDCardInfo(<%= UserID %>)">保存</button>
                            </span>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    </div>
    <div class="modal-backdrop" id="imageModal" hidden>
        <div class="modal image-modal" role="dialog" aria-modal="true">
            <div class="modal-head"><h2>图片预览</h2><button class="modal-close" type="button" onclick="closeImageModal()" aria-label="关闭">×</button></div>
            <div class="modal-body image-modal-body">
                <img id="previewImage" src="" alt="预览">
            </div>
        </div>
    </div>
    <script>
        function checkUser(userId, isCheck) {
            if (!confirm(isCheck == 1 ? '确定审核通过该用户？' : '确定取消该用户的审核？')) return;
            window.location.href = '/admin-users.aspx?action=check&userId=' + userId + '&isCheck=' + isCheck;
        }
        
        function toggleUserStatus(userId, status) {
            if (!confirm(status == 1 ? '确定删除该用户？' : '确定恢复该用户？')) return;
            window.location.href = '/admin-users.aspx?action=toggleStatus&userId=' + userId + '&status=' + status;
        }
        
        function certifyUser(userId, isCertified) {
            if (isCertified == 1) {
                if (!confirm('确定审核通过该会员的资质认证？通过后会员将无法修改上传的证照')) return;
            } else {
                if (!confirm('确定审核不通过/取消认证？会员将可以重新上传证照')) return;
            }
            window.location.href = '/admin-user-detail.aspx?action=certify&id=' + userId + '&isCertified=' + isCertified;
        }
        
        function previewImage(src) {
            document.getElementById('previewImage').src = src;
            document.getElementById('imageModal').hidden = false;
        }
        
        function closeImageModal() {
            document.getElementById('imageModal').hidden = true;
        }
        
        function changeRole(userId) {
            var role = document.getElementById('ddlRole').value;
            var roleName = role == 2 ? '采购商' : '供应商';
            if (!confirm('确定将该用户修改为 ' + roleName + '？')) return;
            window.location.href = '/admin-user-detail.aspx?action=changeRole&id=' + userId + '&role=' + role;
        }
        
        function saveIDCardInfo(userId) {
            var name = document.getElementById('txtIDCardName').value.trim();
            var number = document.getElementById('txtIDCardNumber').value.trim();
            if (!confirm('确定保存身份证信息？')) return;
            window.location.href = '/admin-user-detail.aspx?action=saveIDCard&id=' + userId + '&name=' + encodeURIComponent(name) + '&number=' + encodeURIComponent(number);
        }
        
        function saveCompanyName(userId) {
            var name = document.getElementById('txtCompanyName').value.trim();
            if (!confirm('确定保存公司名称？')) return;
            window.location.href = '/admin-user-detail.aspx?action=saveCompanyName&id=' + userId + '&name=' + encodeURIComponent(name);
        }
        
        function resetPassword(userId) {
            var pwd = document.getElementById('txtNewPassword').value.trim();
            if (!pwd) {
                alert('请输入新密码');
                return;
            }
            if (pwd.length < 6) {
                alert('密码长度不能少于6位');
                return;
            }
            if (!confirm('确定将该用户的密码重置为：' + pwd + '？')) return;
            window.location.href = '/admin-user-detail.aspx?action=resetPassword&id=' + userId + '&pwd=' + encodeURIComponent(pwd);
        }
    </script>
</body>
</html>