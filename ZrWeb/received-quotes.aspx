<%@ Page Language="C#" AutoEventWireup="true" CodeFile="received-quotes.aspx.cs" Inherits="received_quotes" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
        .quote-section-title {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 16px;
        }
        .quote-section-title h2 {
            font-size: 18px;
            font-weight: 600;
            color: #111827;
            margin: 0;
        }
        .quote-count {
            font-size: 13px;
            color: #9ca3af;
        }
        .quote-count b {
            color: #dc2626;
            font-weight: 600;
        }
        .quote-card-list {
            display: flex;
            flex-direction: column;
            gap: 12px;
        }
        .quote-item {
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 10px;
            padding: 16px 20px;
            transition: all 0.2s ease;
            position: relative;
        }
        .quote-item:hover {
            border-color: #fca5a5;
            box-shadow: 0 4px 12px rgba(220, 38, 38, 0.08);
        }
        .quote-item.is-new::after {
            content: 'NEW';
            position: absolute;
            top: 0;
            right: 16px;
            background: linear-gradient(135deg, #dc2626 0%, #f97316 100%);
            color: #fff;
            font-size: 10px;
            font-weight: 700;
            padding: 2px 8px;
            border-radius: 0 0 6px 6px;
            letter-spacing: 0.5px;
        }
        .quote-item-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 20px;
        }
        .quote-item-main {
            flex: 1;
            min-width: 0;
        }
        .quote-item-title {
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 8px;
        }
        .quote-item-title .tag {
            font-size: 11px;
            padding: 2px 8px;
            border-radius: 4px;
            font-weight: 600;
        }
        .quote-item-title .tag.supply {
            background: #eff6ff;
            color: #2563eb;
        }
        .quote-item-title h3 {
            margin: 0;
            font-size: 17px;
            font-weight: 600;
            color: #111827;
            line-height: 1.3;
        }
        .quote-item-title h3 a {
            color: inherit;
            text-decoration: none;
        }
        .quote-item-title h3 a:hover {
            color: #dc2626;
        }
        .quote-price-area {
            flex-shrink: 0;
            text-align: right;
            min-width: 160px;
        }
        .quote-price {
            font-size: 24px;
            font-weight: 700;
            color: #dc2626;
            line-height: 1.2;
        }
        .quote-price .tax-label {
            font-size: 11px;
            font-weight: 500;
            padding: 2px 6px;
            border-radius: 3px;
            margin-left: 6px;
            vertical-align: middle;
        }
        .quote-price .tax-label.tax {
            background: #ecfdf5;
            color: #059669;
        }
        .quote-price .tax-label.notax {
            background: #fef3c7;
            color: #d97706;
        }
        .view-price-btn {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 8px 18px;
            background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
            color: #fff;
            font-size: 13px;
            font-weight: 600;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.2s;
        }
        .view-price-btn:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(220, 38, 38, 0.3);
        }
        .view-price-btn:active {
            transform: translateY(0);
        }
        .price-hidden {
            font-size: 14px;
            color: #9ca3af;
            margin-bottom: 6px;
        }
        .quote-item-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 16px;
            font-size: 13px;
            color: #6b7280;
            margin-top: 8px;
        }
        .quote-item-meta span {
            display: inline-flex;
            align-items: center;
        }
        .quote-item-meta .meta-brand {
            color: #dc2626;
            font-weight: 500;
        }
        .quote-params {
            display: flex;
            flex-wrap: wrap;
            gap: 6px;
            margin-top: 10px;
        }
        .param-chip {
            display: inline-block;
            padding: 3px 10px;
            background: #f3f4f6;
            color: #4b5563;
            font-size: 12px;
            border-radius: 4px;
            font-weight: 500;
        }
        .quote-item-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 14px;
            padding-top: 12px;
            border-top: 1px solid #f3f4f6;
        }
        .quote-seller-info {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 13px;
            color: #6b7280;
        }
        .quote-seller-info .company-name {
            color: #374151;
            font-weight: 500;
            max-width: 240px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }
        .quote-seller-info .dot {
            color: #d1d5db;
        }
        .quote-seller-info .validity {
            color: #2563eb;
            font-weight: 500;
        }
        .quote-item-actions {
            display: flex;
            gap: 8px;
        }
        .quote-empty {
            text-align: center;
            padding: 60px 20px;
        }
        .quote-empty .empty-icon {
            font-size: 48px;
            margin-bottom: 16px;
        }
        .quote-empty h3 {
            font-size: 16px;
            color: #374151;
            margin: 0 0 8px 0;
        }
        .quote-empty p {
            font-size: 13px;
            color: #9ca3af;
            margin: 0;
        }
        .quote-remarks {
            margin-top: 12px;
            padding: 10px 14px;
            background: #fffbeb;
            border: 1px solid #fef3c7;
            border-radius: 6px;
            font-size: 12px;
            color: #92400e;
            line-height: 1.6;
        }
        .quote-remarks b {
            color: #d97706;
            font-weight: 600;
        }
        .tab-bar {
            display: flex;
            gap: 4px;
            background: #f3f4f6;
            padding: 4px;
            border-radius: 8px;
            margin-bottom: 16px;
            width: fit-content;
        }
        .tab-bar .tab-item {
            padding: 8px 18px;
            font-size: 13px;
            font-weight: 500;
            color: #6b7280;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.2s;
        }
        .tab-bar .tab-item:hover {
            color: #374151;
        }
        .tab-bar .tab-item.active {
            background: #fff;
            color: #111827;
            font-weight: 600;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }
    </style>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div><h1>收到报价</h1></div>
                <div class="actions"><a class="btn back" href="buyer-workbench.aspx" data-back>返回采购</a></div>
            </header>
            <section class="panel">
                <div class="quote-section-title">
                    <h2>报价列表</h2>
                    <span class="quote-count">共 <b><%= TotalCount %></b> 条报价</span>
                </div>
                <% if (!HasQuoteData) { %>
                <div class="quote-empty">
                    <div class="empty-icon">📬</div>
                    <h3>暂无报价</h3>
                    <p>您还没有收到供应商的报价，去发布询价吧！</p>
                </div>
                <% } else { %>
                <div class="quote-card-list">
                    <asp:Repeater ID="rptQuotes" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="quote-item <%# (bool)Eval("IsNew") ? "is-new" : "" %>" data-eq-id="<%# Eval("EqId") %>">
                                <div class="quote-item-header">
                                    <div class="quote-item-main">
                                        <div class="quote-item-title">
                                            <span class="tag supply">供应</span>
                                            <h3><a href="/quote-detail.aspx?id=<%# Eval("EqId") %>"><%# Eval("Model") %></a></h3>
                                        </div>
                                        <div class="quote-item-meta">
                                            <span class="meta-brand"><%# Eval("Brand") %></span>
                                            <span>报价数量：<%# Eval("Quantity") %> <%# Eval("Unit") %></span>
                                        </div>
                                        <div class="quote-params">
                                            <%# System.Web.HttpUtility.HtmlDecode(Eval("ParamsHtml").ToString()) %>
                                        </div>
                                        <%# System.Web.HttpUtility.HtmlDecode(Eval("RemarksHtml").ToString()) %>
                                    </div>
                                    <div class="quote-price-area">
                                        <%# System.Web.HttpUtility.HtmlDecode(Eval("PriceAreaHtml").ToString()) %>
                                    </div>
                                </div>
                                <div class="quote-item-footer">
                                    <div class="quote-seller-info">
                                        <span class="company-name"><%# Eval("SellerName") %></span>
                                        <span class="dot">·</span>
                                        <span><%# Eval("QuoteTime") %></span>
                                        <span class="dot">·</span>
                                        <span class="validity">有效期 <%# Eval("Validity") %></span>
                                    </div>
                                    <div class="quote-item-actions">
                                        <a class="btn soft mini" href="tencent://message/?uin=<%# Eval("SellerQQ") %>" target="_blank" data-qq-btn="<%# Eval("SellerQQ") %>">联系供应商</a>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <% if (TotalPages > 1) { %>
                <div class="pagination" style="margin-top:24px; justify-content: center;">
                    <% if (CurrentPage > 1) { %>
                    <a class="btn" href="received-quotes.aspx?page=<%= CurrentPage - 1 %>">上一页</a>
                    <% } else { %>
                    <button class="btn" disabled>上一页</button>
                    <% } %>
                    <span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span>
                    <% if (CurrentPage < TotalPages) { %>
                    <a class="btn" href="received-quotes.aspx?page=<%= CurrentPage + 1 %>">下一页</a>
                    <% } else { %>
                    <button class="btn" disabled>下一页</button>
                    <% } %>
                </div>
                <% } %>
                <% } %>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />

    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // 查看报价按钮
            document.querySelectorAll('[data-view-price]').forEach(function(btn) {
                btn.addEventListener('click', function() {
                    var eqId = this.getAttribute('data-eq-id');
                    var card = this.closest('.quote-item');
                    var priceArea = card.querySelector('.quote-price-area');
                    var priceDisplay = this.getAttribute('data-price');
                    var self = this;

                    // 标记为已查看
                    fetch('/api/mark-quote-readed.aspx?eqId=' + eqId, {
                        method: 'GET',
                        credentials: 'same-origin'
                    }).then(function(res) {
                        return res.json();
                    }).then(function(result) {
                        // 显示价格
                        self.style.display = 'none';
                        var priceHidden = priceArea.querySelector('.price-hidden');
                        if (priceHidden) priceHidden.style.display = 'none';

                        // 创建价格元素
                        var priceDiv = document.createElement('div');
                        priceDiv.className = 'quote-price';
                        priceDiv.innerHTML = priceDisplay;
                        priceArea.appendChild(priceDiv);

                        // 移除 NEW 标签
                        if (card.classList.contains('is-new')) {
                            card.classList.remove('is-new');
                        }
                    }).catch(function(err) {
                        console.error('标记已查看失败:', err);
                        // 即使失败也显示价格
                        self.style.display = 'none';
                        var priceHidden = priceArea.querySelector('.price-hidden');
                        if (priceHidden) priceHidden.style.display = 'none';
                        var priceDiv = document.createElement('div');
                        priceDiv.className = 'quote-price';
                        priceDiv.innerHTML = priceDisplay;
                        priceArea.appendChild(priceDiv);
                        if (card.classList.contains('is-new')) {
                            card.classList.remove('is-new');
                        }
                    });
                });
            });

            // 联系供应商QQ按钮 - 空QQ禁用
            document.querySelectorAll('[data-qq-btn]').forEach(function(btn) {
                var qq = btn.getAttribute('data-qq-btn');
                if (!qq || qq === '') {
                    btn.classList.add('disabled');
                    btn.style.opacity = '0.5';
                    btn.style.cursor = 'not-allowed';
                    btn.addEventListener('click', function(e) {
                        e.preventDefault();
                        Toast.warning('该供应商未设置QQ号');
                    });
                }
            });
        });
    </script>
</body>
</html>
