<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-ads.aspx.cs" Inherits="admin_ads" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>广告管理 - 阻容网</title>
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
                <a class="active" href="admin-ads.aspx">
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
                    <h1>广告管理</h1>
                    <p class="admin-breadcrumb">首页 › 广告管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="section-title">
                    <div>
                        <h2>广告位管理</h2>
                        <span class="admin-table-count">共 <%= TotalAds %> 条记录</span>
                    </div>
                    <button class="btn primary" onclick="showAddAd()">新增广告</button>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>广告位</th>
                                <th>标题</th>
                                <th>位置</th>
                                <th>链接</th>
                                <th>开始时间</th>
                                <th>结束时间</th>
                                <th>状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (AdList != null && AdList.Rows.Count > 0) { %>
                            <asp:Repeater ID="rptAds" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("AdSlot") %></td>
                                        <td><%# Eval("Title") %></td>
                                        <td><%# Eval("Position") %></td>
                                        <td><a href="<%# Eval("LinkUrl") %>" target="_blank"><%# Eval("LinkUrl") %></a></td>
                                        <td><%# Eval("StartDate") %></td>
                                        <td><%# Eval("EndDate") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("Status")) == 1 ? "green" : "gray" %>"><%# Convert.ToInt32(Eval("Status")) == 1 ? "启用" : "停用" %></span></td>
                                        <td>
                                            <button class="btn mini" onclick="toggleAdStatus(<%# Eval("AdID") %>, <%# Convert.ToInt32(Eval("Status")) == 1 ? 0 : 1 %>)">
                                                <%# Convert.ToInt32(Eval("Status")) == 1 ? "停用" : "启用" %>
                                            </button>
                                            <button class="btn mini" onclick="deleteAd(<%# Eval("AdID") %>)">删除</button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } else { %>
                            <tr><td colspan="8" style="text-align:center;padding:40px;">暂无数据</td></tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
            </section>
        </main>
    </div>
    <script>
        function showAddAd() {
            alert('新增广告功能正在开发中，敬请期待！');
        }
        
        function toggleAdStatus(adId, status) {
            if (!confirm(status == 1 ? '确定启用该广告？' : '确定停用该广告？')) return;
            window.location.href = '/admin-ads.aspx?action=toggleStatus&adId=' + adId + '&status=' + status;
        }
        
        function deleteAd(adId) {
            if (!confirm('确定删除该广告？')) return;
            window.location.href = '/admin-ads.aspx?action=delete&adId=' + adId;
        }
    </script>
</body>
</html>