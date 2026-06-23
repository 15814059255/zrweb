<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-users.aspx.cs" Inherits="admin_users" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>用户管理 - 阻容网</title>
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
                <a class="active" href="admin-users.aspx">
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
                    <h1>用户管理</h1>
                    <p class="admin-breadcrumb">首页 › 用户管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="section-title">
                    <div>
                        <h2>会员列表</h2>
                        <span class="admin-table-count">共 <%= TotalUsers %> 条记录</span>
                    </div>
                    <div class="admin-search-actions">
                        <input class="input admin-search" id="txtSearch" runat="server" placeholder="搜索用户名、手机号、联系人">
                        <button class="btn" onclick="searchUsers()">搜索</button>
                    </div>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th><input type="checkbox" onchange="toggleSelectAll(this)"></th>
                                <th>会员ID</th>
                                <th>用户名</th>
                                <th>联系人</th>
                                <th>手机号</th>
                                <th>注册时间</th>
                                <th>审核状态</th>
                                <th>系统状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (UserList != null && UserList.Rows.Count > 0) { %>
                            <asp:Repeater ID="rptUsers" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td><input type="checkbox" name="userIds" value="<%# Eval("UserID") %>"></td>
                                        <td>U<%# Eval("UserID") %></td>
                                        <td><%# Eval("UserName") %></td>
                                        <td><%# Eval("LinkMan") %></td>
                                        <td><%# Eval("MobilePhone") %></td>
                                        <td><%# Convert.ToDateTime(Eval("CreateTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("IsCheck")) == 1 ? "green" : "orange" %>"><%# Convert.ToInt32(Eval("IsCheck")) == 1 ? "已审核" : "待审核" %></span></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("SysStatus")) == 0 ? "green" : "red" %>"><%# Convert.ToInt32(Eval("SysStatus")) == 0 ? "正常" : "已删除" %></span></td>
                                        <td>
                                            <button class="btn mini" onclick="checkUser(<%# Eval("UserID") %>, <%# Convert.ToInt32(Eval("IsCheck")) == 1 ? 0 : 1 %>)">
                                                <%# Convert.ToInt32(Eval("IsCheck")) == 1 ? "取消审核" : "审核通过" %>
                                            </button>
                                            <button class="btn mini" onclick="toggleUserStatus(<%# Eval("UserID") %>, <%# Convert.ToInt32(Eval("SysStatus")) == 0 ? 1 : 0 %>)">
                                                <%# Convert.ToInt32(Eval("SysStatus")) == 0 ? "删除" : "恢复" %>
                                            </button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } else { %>
                            <tr><td colspan="9" style="text-align:center;padding:40px;">暂无数据</td></tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
                <div class="admin-pagination">
                    <button class="btn" onclick="prevPage()" <%= CurrentPage <= 1 ? "disabled" : "" %>>上一页</button>
                    <span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span>
                    <button class="btn" onclick="nextPage()" <%= CurrentPage >= TotalPages ? "disabled" : "" %>>下一页</button>
                </div>
            </section>
        </main>
    </div>
    <script>
        function toggleSelectAll(checkbox) {
            var checkboxes = document.querySelectorAll('input[name="userIds"]');
            checkboxes.forEach(function(cb) {
                cb.checked = checkbox.checked;
            });
        }
        
        function searchUsers() {
            var keyword = document.getElementById('<%= txtSearch.ClientID %>').value;
            window.location.href = '/admin-users.aspx?keyword=' + encodeURIComponent(keyword);
        }
        
        function prevPage() {
            window.location.href = '<%= GetPageUrl(CurrentPage - 1) %>';
        }
        
        function nextPage() {
            window.location.href = '<%= GetPageUrl(CurrentPage + 1) %>';
        }
        
        function checkUser(userId, isCheck) {
            if (!confirm(isCheck == 1 ? '确定审核通过该用户？' : '确定取消该用户的审核？')) return;
            window.location.href = '/admin-users.aspx?action=check&userId=' + userId + '&isCheck=' + isCheck;
        }
        
        function toggleUserStatus(userId, status) {
            if (!confirm(status == 1 ? '确定删除该用户？' : '确定恢复该用户？')) return;
            window.location.href = '/admin-users.aspx?action=toggleStatus&userId=' + userId + '&status=' + status;
        }
    </script>
</body>
</html>