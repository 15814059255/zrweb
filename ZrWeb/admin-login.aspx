<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-login.aspx.cs" Inherits="admin_login" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>管理员登录 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
</head>
<body class="admin-login-page">
    <div class="admin-login-shell">
        <div class="admin-login-card">
            <div class="admin-login-logo">
                <div class="admin-logo-icon">ZR</div>
                <div class="admin-logo-text">
                    <strong>阻容网</strong>
                    <span>ZR.net.cn</span>
                    <small>超级管理员后台</small>
                </div>
            </div>
            <form id="adminLoginForm" runat="server">
                <asp:Panel ID="pnlLogin" runat="server" DefaultButton="btnAdminLogin">
                    <div class="form-row">
                        <label>管理员账号</label>
                        <asp:TextBox ID="txtAdminName" runat="server" CssClass="input" placeholder="请输入管理员账号"></asp:TextBox>
                    </div>
                    <div class="form-row">
                        <label>登录密码</label>
                        <asp:TextBox ID="txtAdminPassword" runat="server" CssClass="input" TextMode="Password" placeholder="请输入登录密码"></asp:TextBox>
                    </div>
                    <asp:Label ID="lblLoginError" runat="server" CssClass="error-msg"></asp:Label>
                    <asp:Button ID="btnAdminLogin" runat="server" CssClass="btn primary admin-login-btn" Text="登录系统" OnClick="btnAdminLogin_Click" />
                </asp:Panel>
            </form>
            <p class="admin-login-tip">默认账号: superadmin / 默认密码: ZrAdmin@2026</p>
        </div>
    </div>
</body>
</html>