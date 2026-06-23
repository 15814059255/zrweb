<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>管理员诊断 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
    <style>
        body { padding: 40px; font-family: inherit; }
        .panel { background: #fff; border: 1px solid #ffe1bd; border-radius: 16px; padding: 24px; margin-bottom: 20px; }
        h1 { margin: 0 0 20px; font-size: 24px; color: #101828; }
        h2 { margin: 0 0 12px; font-size: 16px; color: #101828; }
        .result { padding: 12px; border-radius: 10px; margin: 8px 0; }
        .success { background: #ecfdf3; border: 1px solid #bbf7d0; color: #047857; }
        .error { background: #fff1f2; border: 1px solid #fecdd3; color: #be123c; }
        .info { background: #eff6ff; border: 1px solid #bfdbfe; color: #1d4ed8; }
        pre { background: #f8fafc; padding: 12px; border-radius: 8px; overflow-x: auto; font-size: 12px; margin: 8px 0; }
        .btn { display: inline-flex; align-items: center; justify-content: center; min-height: 40px; padding: 0 16px; border-radius: 12px; border: 1px solid #e4e7ec; background: #fff; color: #344054; font-weight: 700; cursor: pointer; text-decoration: none; }
        .btn.primary { background: #f97316; color: #fff; border-color: #f97316; }
        .actions { margin-top: 20px; display: flex; gap: 12px; }
    </style>
</head>
<body>
    <div class="panel">
        <h1>管理员后台 - 诊断工具</h1>
        
        <h2>1. 数据库连接测试</h2>
        <asp:Literal ID="litDbTest" runat="server" />
        
        <h2>2. 管理员表检查</h2>
        <asp:Literal ID="litTableCheck" runat="server" />
        
        <h2>3. 用户数据检查</h2>
        <asp:Literal ID="litUserData" runat="server" />
        
        <h2>4. 密码验证测试</h2>
        <asp:Literal ID="litPasswordTest" runat="server" />
        
        <div class="actions">
            <a href="/admin-login.aspx" class="btn primary">前往登录页面</a>
            <a href="/admin-init.aspx" class="btn">重新初始化数据库</a>
        </div>
    </div>
</body>
</html>
