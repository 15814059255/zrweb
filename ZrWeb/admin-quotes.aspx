<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-quotes.aspx.cs" Inherits="admin_quotes" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>报价管理 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
    <style>
        .admin-table-wrap { max-height: none !important; overflow-y: visible !important; }
    </style>
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
                <a class="active" href="admin-quotes.aspx">
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
                    <h1>报价管理</h1>
                    <p class="admin-breadcrumb">首页 › 报价管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="section-title">
                    <div>
                        <h2>报价列表</h2>
                        <span class="admin-table-count">共 <%= TotalQuotes %> 条记录</span>
                    </div>
                    <div class="admin-search-actions">
                        <input class="input admin-search" id="txtSearch" runat="server" placeholder="搜索型号、供应方、采购方">
                        <button class="btn" onclick="searchQuotes()">搜索</button>
                    </div>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>报价ID</th>
                                <th>型号</th>
                                <th>供应方</th>
                                <th>采购方</th>
                                <th>报价</th>
                                <th>数量</th>
                                <th>报价时间</th>
                                <th>状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (QuoteList != null && QuoteList.Rows.Count > 0) { %>
                            <asp:Repeater ID="rptQuotes" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td>Q<%# Eval("eqId") %></td>
                                        <td><%# Eval("goodsSn") %></td>
                                        <td><%# Convert.ToInt32(Eval("eqType")) == 1 ? Eval("toCompany") : Eval("fromCompany") %></td>
                                        <td><%# Convert.ToInt32(Eval("eqType")) == 1 ? Eval("fromCompany") : Eval("toCompany") %></td>
                                        <td>¥<%# Eval("fromPrice") %></td>
                                        <td><%# Eval("fromQuantity") %></td>
                                        <td><%# Convert.ToDateTime(Eval("createTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("eqType")) == 1 ? "blue" : "green" %>"><%# Convert.ToInt32(Eval("eqType")) == 1 ? "询价" : "报价" %></span></td>
                                        <td><button class="btn mini" onclick="deleteQuote(<%# Eval("eqId") %>)">删除</button></td>
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
        function searchQuotes() {
            var keyword = document.getElementById('<%= txtSearch.ClientID %>').value;
            window.location.href = '/admin-quotes.aspx?keyword=' + encodeURIComponent(keyword);
        }
        
        function prevPage() {
            window.location.href = '<%= GetPageUrl(CurrentPage - 1) %>';
        }
        
        function nextPage() {
            window.location.href = '<%= GetPageUrl(CurrentPage + 1) %>';
        }
        
        function deleteQuote(eqId) {
            if (!confirm('确定删除该报价？')) return;
            window.location.href = '/admin-quotes.aspx?action=delete&eqId=' + eqId;
        }
    </script>
</body>
</html>