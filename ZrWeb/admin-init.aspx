<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>管理员初始化 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
    <style>
        body { padding: 40px; font-family: inherit; }
        .panel { background: #fff; border: 1px solid #ffe1bd; border-radius: 16px; padding: 24px; max-width: 600px; margin: 0 auto; }
        h1 { margin: 0 0 20px; font-size: 24px; color: #101828; text-align: center; }
        .result { padding: 12px; border-radius: 10px; margin: 8px 0; }
        .success { background: #ecfdf3; border: 1px solid #bbf7d0; color: #047857; }
        .error { background: #fff1f2; border: 1px solid #fecdd3; color: #be123c; }
        .info { background: #eff6ff; border: 1px solid #bfdbfe; color: #1d4ed8; }
        .btn { display: inline-flex; align-items: center; justify-content: center; min-height: 40px; padding: 0 16px; border-radius: 12px; border: 1px solid #e4e7ec; background: #fff; color: #344054; font-weight: 700; cursor: pointer; text-decoration: none; font-size: 14px; }
        .btn.primary { background: #f97316; color: #fff; border-color: #f97316; }
        .actions { margin-top: 20px; display: flex; gap: 12px; justify-content: center; }
        .account-box { background: #fff7ed; border: 1px solid #fed7aa; border-radius: 12px; padding: 20px; margin: 20px 0; text-align: center; }
        .account-box h2 { margin: 0 0 12px; color: #c2410c; font-size: 16px; }
        .account-box p { margin: 8px 0; font-size: 14px; }
        .account-box strong { font-size: 18px; color: #101828; }
    </style>
</head>
<body>
    <div class="panel">
        <h1>管理员账号初始化</h1>
        
        <asp:Panel ID="pnlInitForm" runat="server">
            <p style="text-align:center; color:#475467;">点击下方按钮创建/修复管理员账号</p>
            <div class="actions" style="margin-top:24px;">
                <asp:Button ID="btnInit" runat="server" CssClass="btn primary" Text="初始化管理员账号" OnClick="btnInit_Click" />
            </div>
        </asp:Panel>
        
        <asp:Panel ID="pnlResult" runat="server" Visible="false">
            <asp:Literal ID="litResult" runat="server" />
        </asp:Panel>
        
        <div class="account-box" style="display:none;" id="divSuccess">
            <h2>初始化成功！</h2>
            <p>管理员账号信息：</p>
            <p><strong>账号：superadmin</strong></p>
            <p><strong>密码：ZrAdmin@2026</strong></p>
            <div class="actions">
                <a href="/admin-login.aspx" class="btn primary">立即登录</a>
                <a href="/admin-diagnose.aspx" class="btn">诊断工具</a>
            </div>
        </div>
    </div>
</body>
</html>
