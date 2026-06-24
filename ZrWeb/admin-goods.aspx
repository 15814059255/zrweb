<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-goods.aspx.cs" Inherits="admin_goods" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>供需管理 - 阻容网</title>
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
                <a class="active" href="admin-goods.aspx">
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
                    <h1>供需管理</h1>
                    <p class="admin-breadcrumb">首页 › 供需管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="section-title">
                    <div>
                        <h2>供需列表</h2>
                        <span class="admin-table-count">共 <%= TotalGoods %> 条记录</span>
                    </div>
                    <div class="admin-search-actions">
                        <select class="input" id="selPubType" runat="server">
                            <option value="">全部类型</option>
                            <option value="1">供应</option>
                            <option value="2">需求</option>
                        </select>
                        <input class="input admin-search" id="txtSearch" runat="server" placeholder="搜索型号">
                        <button class="btn" onclick="searchGoods()">搜索</button>
                    </div>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th><input type="checkbox" onchange="toggleSelectAll(this)"></th>
                                <th>型号</th>
                                <th>发布者</th>
                                <th>类型</th>
                                <th>价格</th>
                                <th>库存</th>
                                <th>单位</th>
                                <th>发布时间</th>
                                <th>状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (GoodsList != null && GoodsList.Rows.Count > 0) { %>
                            <asp:Repeater ID="rptGoods" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td><input type="checkbox" name="goodsIds" value="<%# Eval("goodsId") %>"></td>
                                        <td><a href="javascript:void(0)" class="goods-link" onclick="showGoodsDetail(<%# Eval("goodsId") %>, '<%# Eval("goodsSn") %>', <%# Eval("pubType") %>)"><%# Eval("goodsSn") %></a></td>
                                        <td><%# Eval("PublisherName") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("pubType")) == 1 ? "blue" : "orange" %>"><%# Convert.ToInt32(Eval("pubType")) == 1 ? "供应" : "需求" %></span></td>
                                        <td>¥<%# Eval("shopPrice") %></td>
                                        <td><%# Eval("goodsStock") %></td>
                                        <td><%# Eval("goodsUnit") %></td>
                                        <td><%# Convert.ToDateTime(Eval("createTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("isSale")) == 1 ? "green" : "gray" %>"><%# Convert.ToInt32(Eval("isSale")) == 1 ? "在线" : "已下架" %></span></td>
                                        <td>
                                            <button class="btn mini" onclick="toggleGoodsStatus(<%# Eval("goodsId") %>, <%# Convert.ToInt32(Eval("isSale")) == 1 ? 0 : 1 %>)">
                                                <%# Convert.ToInt32(Eval("isSale")) == 1 ? "下架" : "上架" %>
                                            </button>
                                            <button class="btn mini" onclick="deleteGoods(<%# Eval("goodsId") %>)">删除</button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } else { %>
                            <tr><td colspan="10" style="text-align:center;padding:40px;">暂无数据</td></tr>
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
    <div class="modal-backdrop" id="goodsDetailModal" hidden>
        <div class="modal" role="dialog" aria-modal="true" aria-label="商品详情">
            <div class="modal-head">
                <h2 id="goodsDetailTitle">商品详情</h2>
                <button class="modal-close" type="button" onclick="closeGoodsDetail()" aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <div id="goodsDetailContent">
                    <div class="loading">加载中...</div>
                </div>
            </div>
        </div>
    </div>
    <script>
        function toggleSelectAll(checkbox) {
            var checkboxes = document.querySelectorAll('input[name="goodsIds"]');
            checkboxes.forEach(function(cb) {
                cb.checked = checkbox.checked;
            });
        }
        
        function showGoodsDetail(goodsId, goodsSn, pubType) {
            var modal = document.getElementById('goodsDetailModal');
            var title = document.getElementById('goodsDetailTitle');
            var content = document.getElementById('goodsDetailContent');
            
            title.textContent = goodsSn + ' - ' + (pubType == 1 ? '供应' : '需求') + '详情';
            content.innerHTML = '<div class="loading">加载中...</div>';
            modal.hidden = false;
            
            fetch('/api/goods-detail.aspx?id=' + goodsId + '&pubType=' + pubType)
                .then(response => response.text())
                .then(html => {
                    content.innerHTML = html;
                })
                .catch(error => {
                    content.innerHTML = '<div style="text-align:center;color:#999;">加载失败，请重试</div>';
                });
        }
        
        function closeGoodsDetail() {
            document.getElementById('goodsDetailModal').hidden = true;
        }
        
        function searchGoods() {
            var keyword = document.getElementById('<%= txtSearch.ClientID %>').value;
            var pubType = document.getElementById('<%= selPubType.ClientID %>').value;
            var url = '/admin-goods.aspx?keyword=' + encodeURIComponent(keyword);
            if (pubType) url += '&pubType=' + pubType;
            window.location.href = url;
        }
        
        function prevPage() {
            window.location.href = '<%= GetPageUrl(CurrentPage - 1) %>';
        }
        
        function nextPage() {
            window.location.href = '<%= GetPageUrl(CurrentPage + 1) %>';
        }
        
        function toggleGoodsStatus(goodsId, status) {
            if (!confirm(status == 1 ? '确定上架该商品？' : '确定下架该商品？')) return;
            window.location.href = '/admin-goods.aspx?action=toggleStatus&goodsId=' + goodsId + '&status=' + status;
        }
        
        function deleteGoods(goodsId) {
            if (!confirm('确定删除该商品？此操作不可恢复！')) return;
            window.location.href = '/admin-goods.aspx?action=delete&goodsId=' + goodsId;
        }
    </script>
</body>
</html>