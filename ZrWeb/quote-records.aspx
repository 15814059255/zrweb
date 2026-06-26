<%@ Page Language="C#" AutoEventWireup="true" CodeFile="quote-records.aspx.cs" Inherits="quote_records" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
    <style>
        .main { padding: 28px 24px; }
        .inquiry-panel { padding: 0; }
    </style>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div><h1>我的报价</h1></div>
                <div class="actions"><a class="btn back" href="merchant-workbench.aspx" data-back>返回商家</a></div>
            </header>
            <section class="panel">
                <form method="get" action="quote-records.aspx" class="searchbar inventory-searchbar">
                    <input class="input" name="keyword" placeholder="搜索型号或参数" value="<%= SearchKeyword %>">
                    <div style="display:flex;gap:8px;align-items:center;">
                        <button class="btn primary" type="submit">搜索</button>
                        <% if (!string.IsNullOrEmpty(SearchKeyword)) { %>
                            <a class="btn" href="quote-records.aspx">清除</a>
                        <% } %>
                    </div>
                </form>
            </section>
            <section class="panel inquiry-panel">
                <div class="inquiry-list" id="quoteList">
                    <asp:Repeater ID="rptQuoteRecords" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="inquiry-card">
                                <div class="inquiry-card-header">
                                    <div class="inquiry-status">
                                        <span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span>
                                        <span class="inquiry-time"><%# Eval("QuoteTime") %></span>
                                    </div>
                                    <div class="quote-validity-tag">有效期: <span><%# Eval("Validity") %></span></div>
                                </div>
                                <div class="inquiry-card-body">
                                    <div class="inquiry-model">
                                        <strong><%# Eval("Model") %></strong>
                                    </div>
                                    <div class="inquiry-brand-params">
                                        <span class="params-label">品牌参数</span>
                                        <span class="params-value"><%# Eval("BrandParams") %></span>
                                    </div>
                                    <div class="quote-compare-grid">
                                        <div class="quote-side inquiry-side">
                                            <div class="side-header">
                                                <span class="side-icon">👤</span>
                                                <span class="side-title">询价方</span>
                                            </div>
                                            <div class="inquiry-specs">
                                                <div class="spec-item">
                                                    <span class="spec-label">采购数量</span>
                                                    <span class="spec-value"><%# Eval("InquiryQuantity") %> <span class="spec-unit"><%# Eval("InquiryUnit") %></span></span>
                                                </div>
                                                <div class="spec-item">
                                                    <span class="spec-label">期望单价</span>
                                                    <span class="spec-value"><%# Eval("InquiryPrice") %></span>
                                                </div>
                                                <div class="spec-item">
                                                    <span class="spec-label">备注</span>
                                                    <span class="spec-value remark-value"><%# Eval("InquiryRemarks") %></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="quote-side my-side">
                                            <div class="side-header">
                                                <span class="side-icon">🏢</span>
                                                <span class="side-title">我的报价</span>
                                            </div>
                                            <div class="inquiry-specs">
                                                <div class="spec-item">
                                                    <span class="spec-label">报价数量</span>
                                                    <span class="spec-value"><%# Eval("MyQuantity") %> <span class="spec-unit"><%# Eval("MyUnit") %></span></span>
                                                </div>
                                                <div class="spec-item">
                                                    <span class="spec-label">报价单价</span>
                                                    <span class="spec-value"><%# Eval("MyPrice") %></span>
                                                </div>
                                                <div class="spec-item">
                                                    <span class="spec-label">批次</span>
                                                    <span class="spec-value"><%# Eval("MyBatch") %></span>
                                                </div>
                                                <div class="spec-item">
                                                    <span class="spec-label">备注</span>
                                                    <span class="spec-value remark-value"><%# Eval("MyRemarks") %></span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="inquiry-card-footer">
                                    <div class="inquiry-buyer">
                                        <span class="buyer-icon">🏢</span>
                                        <span class="buyer-name"><%# Eval("BuyerName") %></span>
                                    </div>
                                    <a class="btn soft mini" href="tencent://message/?uin=<%# Eval("BuyerQQ") %>" target="_blank" data-qq-btn="<%# Eval("BuyerQQ") %>">联系采购商</a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div class="pagination"><button class="btn" disabled>上一页</button><span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span><span class="page-size">每页 50 条</span><button class="btn">下一页</button></div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />

    <script>
        document.addEventListener('DOMContentLoaded', function() {
            document.querySelectorAll('[data-qq-btn]').forEach(function(btn) {
                var qq = btn.getAttribute('data-qq-btn');
                if (!qq || qq === '') {
                    btn.classList.add('disabled');
                    btn.style.opacity = '0.5';
                    btn.style.cursor = 'not-allowed';
                    btn.addEventListener('click', function(e) {
                        e.preventDefault();
                        alert('该采购商未设置QQ号');
                    });
                }
            });
        });
    </script>
</body>
</html>