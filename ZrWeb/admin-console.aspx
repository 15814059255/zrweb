<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-console.aspx.cs" Inherits="admin_console" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>管理员控制台 - 阻容网</title>
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
                <a class="active" href="admin-console.aspx">
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
                    <h1>控制台</h1>
                    <p class="admin-breadcrumb">首页 › 控制台</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <div class="admin-kpi-grid">
                <div class="admin-kpi">
                    <div class="kpi-icon">👥</div>
                    <div class="kpi-content">
                        <span class="kpi-label">会员总数</span>
                        <strong class="kpi-value"><%= MemberCount %></strong>
                        <small class="kpi-change">今日新增 <%= TodayNewMember %></small>
                    </div>
                </div>
                <div class="admin-kpi">
                    <div class="kpi-icon">📦</div>
                    <div class="kpi-content">
                        <span class="kpi-label">在线供应</span>
                        <strong class="kpi-value"><%= SupplyCount %></strong>
                        <small class="kpi-change">今日新增 <%= TodayNewSupply %></small>
                    </div>
                </div>
                <div class="admin-kpi">
                    <div class="kpi-icon">🛒</div>
                    <div class="kpi-content">
                        <span class="kpi-label">在线需求</span>
                        <strong class="kpi-value"><%= DemandCount %></strong>
                        <small class="kpi-change">今日新增 <%= TodayNewDemand %></small>
                    </div>
                </div>
                <div class="admin-kpi">
                    <div class="kpi-icon">💰</div>
                    <div class="kpi-content">
                        <span class="kpi-label">报价总数</span>
                        <strong class="kpi-value"><%= QuoteCount %></strong>
                        <small class="kpi-change">今日新增 <%= TodayNewQuote %></small>
                    </div>
                </div>
            </div>
            <div class="admin-section-grid">
                <section class="panel admin-panel">
                    <div class="section-title">
                        <h2>最近注册会员</h2>
                        <a href="admin-users.aspx" class="btn mini">查看全部</a>
                    </div>
                    <div class="admin-table-wrap">
                        <table class="table admin-table">
                            <thead><tr><th>会员ID</th><th>用户名</th><th>联系人</th><th>手机号</th><th>注册时间</th><th>状态</th></tr></thead>
                            <tbody>
                                <% if (RecentMembers != null && RecentMembers.Rows.Count > 0) { %>
                                <asp:Repeater ID="rptRecentMembers" runat="server" EnableViewState="false">
                                    <ItemTemplate>
                                        <tr><td>U<%# Eval("UserID") %></td><td><%# Eval("UserName") %></td><td><%# Eval("LinkMan") %></td><td><%# Eval("MobilePhone") %></td><td><%# Convert.ToDateTime(Eval("CreateTime")).ToString("yyyy-MM-dd HH:mm") %></td><td><span class="tag <%# Convert.ToInt32(Eval("IsCheck")) == 1 ? "green" : "orange" %>"><%# Convert.ToInt32(Eval("IsCheck")) == 1 ? "已审核" : "待审核" %></span></td></tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <% } else { %>
                                <tr><td colspan="6" style="text-align:center;padding:20px;">暂无数据</td></tr>
                                <% } %>
                            </tbody>
                        </table>
                    </div>
                </section>
                <section class="panel admin-panel">
                    <div class="section-title">
                        <h2>最近发布供需</h2>
                        <a href="admin-goods.aspx" class="btn mini">查看全部</a>
                    </div>
                    <div class="admin-table-wrap">
                        <table class="table admin-table">
                            <thead><tr><th>商品ID</th><th>型号</th><th>类型</th><th>价格</th><th>发布时间</th><th>状态</th></tr></thead>
                            <tbody>
                                <% if (RecentGoods != null && RecentGoods.Rows.Count > 0) { %>
                                <asp:Repeater ID="rptRecentGoods" runat="server" EnableViewState="false">
                                    <ItemTemplate>
                                        <tr><td>G<%# Eval("goodsId") %></td><td><%# Eval("goodsSn") %></td><td><span class="tag <%# Convert.ToInt32(Eval("pubType")) == 1 ? "blue" : "orange" %>"><%# Convert.ToInt32(Eval("pubType")) == 1 ? "供应" : "需求" %></span></td><td>¥<%# Eval("shopPrice") %>/<%# Eval("goodsUnit") %></td><td><%# Convert.ToDateTime(Eval("createTime")).ToString("yyyy-MM-dd HH:mm") %></td><td><span class="tag <%# Convert.ToInt32(Eval("isSale")) == 1 ? "green" : "gray" %>"><%# Convert.ToInt32(Eval("isSale")) == 1 ? "在线" : "已下架" %></span></td></tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <% } else { %>
                                <tr><td colspan="6" style="text-align:center;padding:20px;">暂无数据</td></tr>
                                <% } %>
                            </tbody>
                        </table>
                    </div>
                </section>
            </div>
        </main>
    </div>
</body>
</html>