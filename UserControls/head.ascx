<%@ Control Language="C#" AutoEventWireup="true" CodeFile="head.ascx.cs" Inherits="UserControls_head" %>

<script>
    window.ZR_CURRENT_MEMBER = <%= IsLoggedIn ? "{ userId: '" + Session["UserID"] + "', userName: '" + CurrentUserName + "', role: '" + (Session["RoseID"] ?? "") + "', shopId: '" + (Session["ShopId"] ?? "0") + "', shopName: '" + (Session["ShopName"] ?? "") + "' }" : "null" %>;
</script>

<!-- 侧边栏头部 -->
<aside class="sidebar">
    <a class="brand brand-v2" href="/index.aspx" aria-label="阻容网首页">
        <div class="logo logo-v2"><span>ZR</span></div>
        <div class="brand-copy">
            <strong><%= SiteName %></strong>
            <span><%= SiteDomain %></span>
            <small>阻容元件供需撮合</small>
        </div>
    </a>
    <nav class="nav">
        <a class="<%= GetNavClass("index") %>" href="/index.aspx"><span>广场</span><small>›</small></a>
        <a class="<%= GetNavClass("merchant") %>" href="/merchant-workbench.aspx"><span>商家</span><small>›</small></a>
        <a class="<%= GetNavClass("buyer") %>" href="/buyer-workbench.aspx"><span>采购</span><small>›</small></a>
        <% if (IsLoggedIn) { %>
        <a class="<%= GetNavClass("profile") %>" href="/profile.aspx"><span>我的</span><small>›</small></a>
        <% } else { %>
        <a class="<%= GetNavClass("login") %>" href="/login.aspx"><span>登录</span><small>›</small></a>
        <% } %>
    </nav>
    <% if (IsLoggedIn) { %>
    <div class="user-info">
        <div class="user-name"><%= CurrentUserName %></div>
        <div class="user-actions"><a href="/logout.aspx">退出登录</a></div>
    </div>
    <% } %>
    <div class="sidebar-footer">
        <a class="sidebar-footer-link" href="/about-us.aspx">关于我们</a>
        <a class="sidebar-footer-link" href="/help-center.aspx">网站帮助</a>
        <div class="sidebar-legal">
            <span>© <%= SiteName %> <%= SiteDomain %></span>
            <span><a class="icp-link" href="https://beian.miit.gov.cn/" target="_blank" rel="noopener"><%= ICP %></a></span>
        </div>
    </div>
</aside>