<%@ Page Language="C#" AutoEventWireup="true" CodeFile="register.aspx.cs" Inherits="register" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
                <div><h1>注册会员</h1></div>
                <div class="actions"><a class="btn back" href="/index.aspx" data-back>返回</a></div>
            </header>
            <section class="login-shell">
                <div class="login-panel-card register-panel">
                    <h2>注册新会员</h2>
                    <p class="register-tip">注册后可发布供需信息、参与询价报价。<em class="required-mark">*</em> 为必填字段</p>
                    <form id="registerForm" runat="server">
                        <asp:Panel ID="pnlRegister" runat="server" DefaultButton="btnRegister">
                            <div class="form-row"><label>手机号 <em class="required-mark">*</em></label><asp:TextBox ID="txtMobilePhone" runat="server" CssClass="input" placeholder="11位手机号"></asp:TextBox></div>
                            <div class="form-row"><label>密码 <em class="required-mark">*</em></label><asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" placeholder="至少6个字符"></asp:TextBox></div>
                            <div class="form-row"><label>确认密码 <em class="required-mark">*</em></label><asp:TextBox ID="txtPassword2" runat="server" CssClass="input" TextMode="Password" placeholder="再次输入密码"></asp:TextBox></div>
                            <div class="form-row"><label>联系人 <em class="required-mark">*</em></label><asp:TextBox ID="txtLinkMan" runat="server" CssClass="input" placeholder="您的姓名"></asp:TextBox></div>
                            <asp:Label ID="lblError" runat="server" CssClass="error-msg"></asp:Label>
                            <asp:Label ID="lblSuccess" runat="server" CssClass="success-msg"></asp:Label>
                            <label class="privacy-check"><input type="checkbox" id="chkPrivacy" runat="server"> <span>已阅读并同意 <a href="/about-us.aspx">《隐私政策》</a></span></label>
                            <asp:Button ID="btnRegister" runat="server" CssClass="btn primary register-submit" Text="立即注册" OnClick="btnRegister_Click" OnClientClick="return validateRegisterForm();" />
                        </asp:Panel>
                    </form>
                    <p class="login-link">已有账号？<a href="/login.aspx">立即登录</a></p>
                </div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
    <script>
    function validateRegisterForm() {
        var mobilePhone = document.getElementById('<%= txtMobilePhone.ClientID %>');
        var password = document.getElementById('<%= txtPassword.ClientID %>');
        var password2 = document.getElementById('<%= txtPassword2.ClientID %>');
        var linkMan = document.getElementById('<%= txtLinkMan.ClientID %>');
        var privacy = document.getElementById('<%= chkPrivacy.ClientID %>');
        
        // 验证手机号
        if (!mobilePhone.value.trim()) {
            mobilePhone.classList.add('input-error');
            ZrToast.error('请输入手机号');
            return false;
        }
        if (!/^1[3-9]\d{9}$/.test(mobilePhone.value.trim())) {
            mobilePhone.classList.add('input-error');
            ZrToast.error('请输入正确的11位手机号');
            return false;
        }
        mobilePhone.classList.remove('input-error');
        
        // 验证密码
        if (!password.value.trim()) {
            password.classList.add('input-error');
            ZrToast.error('请输入密码');
            return false;
        }
        if (password.value.trim().length < 6) {
            password.classList.add('input-error');
            ZrToast.error('密码长度至少6个字符');
            return false;
        }
        password.classList.remove('input-error');
        
        // 验证确认密码
        if (!password2.value.trim()) {
            password2.classList.add('input-error');
            ZrToast.error('请再次输入密码');
            return false;
        }
        if (password.value.trim() !== password2.value.trim()) {
            password2.classList.add('input-error');
            ZrToast.error('两次输入的密码不一致');
            return false;
        }
        password2.classList.remove('input-error');
        
        // 验证联系人
        if (!linkMan.value.trim()) {
            linkMan.classList.add('input-error');
            ZrToast.error('请输入联系人姓名');
            return false;
        }
        linkMan.classList.remove('input-error');
        
        // 验证隐私政策
        if (!privacy.checked) {
            ZrToast.warning('请阅读并同意隐私政策');
            return false;
        }
        
        return true;
    }
    </script>
</body>
</html>