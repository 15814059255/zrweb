<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
    <script>
    window.ZrToast = function(message, type, duration) {
        type = type || 'info';
        duration = duration || 3000;
        var c = document.querySelector('.zr-toast-container');
        if (!c) {
            c = document.createElement('div');
            c.className = 'zr-toast-container';
            document.body.appendChild(c);
        }
        var toast = document.createElement('div');
        toast.className = 'zr-toast zr-toast-' + type;
        toast.innerHTML = '<span>' + message + '</span>';
        c.appendChild(toast);
        setTimeout(function() {
            toast.classList.add('toast-exit');
            setTimeout(function() { toast.remove(); }, 250);
        }, duration);
    };
    window.ZrToast.success = function(msg, dur) { ZrToast(msg, 'success', dur); };
    window.ZrToast.error = function(msg, dur) { ZrToast(msg, 'error', dur); };
    window.ZrToast.warning = function(msg, dur) { ZrToast(msg, 'warning', dur); };
    window.ZrToast.info = function(msg, dur) { ZrToast(msg, 'info', dur); };
    </script>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div><h1>登录</h1></div>
                <div class="actions"><a class="btn back" href="/index.aspx" data-back>返回</a></div>
            </header>
            <section class="login-shell">
                <div class="login-panel-card">
                    <div class="login-tabs" role="tablist">
                        <button class="active" type="button" data-login-method="account">账号登录</button>
                        <button type="button" data-login-method="sms">手机验证码</button>
                    </div>
                    <div class="login-method-panel active" data-login-panel="account">
                        <form id="loginForm" runat="server">
                            <asp:Panel ID="pnlAccountLogin" runat="server" DefaultButton="btnLogin">
                                <div class="form-row"><label>手机号</label><asp:TextBox ID="txtUserName" runat="server" CssClass="input" placeholder="请输入手机号"></asp:TextBox></div>
                                <div class="form-row"><label>密码</label><asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" placeholder="请输入密码"></asp:TextBox></div>
                                <asp:Label ID="lblError" runat="server" CssClass="error-msg"></asp:Label>
                                <label class="privacy-check"><input type="checkbox" id="chkPrivacy" runat="server"> <span>已阅读并同意 <a href="/about-us.aspx">《隐私政策》</a></span></label>
                                <asp:Button ID="btnLogin" runat="server" CssClass="btn primary login-submit" Text="登录" OnClick="btnLogin_Click" OnClientClick="return validateLoginForm();" />
                            </asp:Panel>
                        </form>
                        <p class="login-tip">使用手机号和密码登录，登录后可管理供需信息。</p>
                        <p class="register-link">没有账号？<a href="/register.aspx">立即注册</a></p>
                    </div>
                    <div class="login-method-panel" data-login-panel="sms" hidden>
                        <div class="sms-login-form sms-code-row">
                            <label>手机号<input class="input" data-login-phone inputmode="numeric" placeholder="请输入手机号"></label>
                            <label>验证码<input class="input" data-login-code inputmode="numeric" placeholder="6 位验证码"></label>
                            <button class="btn soft" type="button" data-ui-toast="验证码已发送">获取验证码</button>
                        </div>
                        <label class="privacy-check"><input type="checkbox" data-login-privacy> <span>已阅读并同意 <a href="#" data-policy-link>《隐私政策》</a></span></label>
                        <button class="btn primary login-submit" type="button" data-login-submit>登录并继续</button>
                        <p class="login-tip">使用手机验证码登录，无需记住密码。</p>
                    </div>
                </div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
    <script>
    function validateLoginForm() {
        var userName = document.getElementById('<%= txtUserName.ClientID %>');
        var password = document.getElementById('<%= txtPassword.ClientID %>');
        var privacy = document.getElementById('<%= chkPrivacy.ClientID %>');
        
        if (!userName.value.trim()) {
            userName.classList.add('input-error');
            ZrToast.error('请输入手机号');
            return false;
        }
        if (!/^1[3-9]\d{9}$/.test(userName.value.trim())) {
            userName.classList.add('input-error');
            ZrToast.error('请输入正确的11位手机号');
            return false;
        }
        userName.classList.remove('input-error');
        
        if (!password.value.trim()) {
            password.classList.add('input-error');
            ZrToast.error('请输入密码');
            return false;
        }
        password.classList.remove('input-error');
        
        if (!privacy.checked) {
            ZrToast.warning('请阅读并同意隐私政策');
            return false;
        }
        
        return true;
    }
    </script>
</body>
</html>