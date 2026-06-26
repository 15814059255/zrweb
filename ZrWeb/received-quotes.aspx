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
        .quote-page { padding: 28px 24px; }
        .quote-page .panel { padding: 20px; }
        .quote-list { display: grid; gap: 12px; }
        .quote-card {
            display: grid;
            grid-template-columns: minmax(360px, 1.2fr) minmax(140px, .4fr) minmax(240px, .8fr) minmax(200px, .6fr);
            gap: 16px;
            align-items: center;
            padding: 16px 20px;
            border: 1px solid #e5e7eb;
            border-radius: 16px;
            background: linear-gradient(180deg, #fff, #f9fbff);
            transition: all 0.25s ease;
        }
        .quote-card:hover {
            border-color: #fca5a5;
            box-shadow: 0 8px 24px rgba(220, 38, 38, 0.08);
            transform: translateY(-1px);
        }
        .quote-card.is-new { box-shadow: inset 4px 0 0 #dc2626; }
        .quote-main { display: grid; gap: 6px; }
        .quote-main h3 { margin: 0; font-size: 16px; font-weight: 700; color: #101828; }
        .quote-main h3 a { color: inherit; text-decoration: none; }
        .quote-main h3 a:hover { color: #dc2626; }
        .quote-main p { margin: 0; color: #475467; font-size: 12px; font-weight: 700; }
        .quote-tags { display: flex; flex-wrap: wrap; gap: 6px; margin-top: 4px; }
        .quote-tags span {
            display: inline-flex;
            align-items: center;
            min-height: 24px;
            padding: 0 8px;
            border-radius: 999px;
            background: #f2f4f7;
            color: #475467;
            font-size: 11px;
            font-weight: 800;
        }
        .quote-price { display: grid; gap: 6px; justify-items: start; }
        .quote-price strong { color: #dc2626; font-size: 22px; font-weight: 800; }
        .quote-price span { color: #98a2b3; font-size: 12px; font-weight: 600; }
        .quote-price .view-price-btn {
            padding: 8px 20px;
            background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
            color: #fff;
            font-size: 13px;
            font-weight: 700;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.2s;
        }
        .quote-price .view-price-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(220, 38, 38, 0.3);
        }
        .quote-price .price-hidden { color: #98a2b3; font-size: 12px; font-weight: 600; }
        .quote-seller { display: grid; gap: 4px; color: #475467; font-size: 12px; line-height: 1.4; }
        .quote-seller b { color: #101828; font-size: 14px; font-weight: 700; }
        .quote-seller span:last-child { color: #2563eb; font-weight: 700; }
        .quote-seller .seller-info-hidden { display: none; }
        .quote-note { display: grid; gap: 6px; color: #475467; font-size: 12px; line-height: 1.4; }
        .quote-note p { margin: 0; }
        .quote-note .remark-label { font-weight: 700; color: #6b7280; }
        .quote-empty {
            text-align: center;
            padding: 60px 20px;
            border: 1px solid #e5e7eb;
            border-radius: 16px;
            background: #fff;
        }
        .quote-empty .empty-icon { font-size: 56px; margin-bottom: 16px; }
        .quote-empty h3 { font-size: 18px; color: #374151; margin: 0 0 8px 0; }
        .quote-empty p { font-size: 14px; color: #9ca3af; margin: 0; }
        .quote-section-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 16px;
        }
        .quote-section-header h2 { font-size: 18px; font-weight: 700; color: #111827; margin: 0; }
        .quote-actions { display: flex; align-items: center; gap: 16px; }
        .quote-filter { display: flex; gap: 4px; padding: 4px; background: #f3f4f6; border-radius: 8px; }
        .quote-filter .filter-btn {
            padding: 6px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #6b7280;
            text-decoration: none;
            border-radius: 6px;
            transition: all 0.2s;
        }
        .quote-filter .filter-btn:hover { background: #e5e7eb; color: #374151; }
        .quote-filter .filter-btn.active {
            background: #dc2626;
            color: #fff;
            box-shadow: 0 2px 8px rgba(220, 38, 38, 0.3);
        }
        .quote-count { font-size: 14px; color: #6b7280; }
        .quote-count b { color: #dc2626; font-weight: 700; font-size: 16px; }
        .topbar h1 .subtitle {
            font-size: 14px;
            font-weight: 400;
            color: #9ca3af;
            margin-left: 8px;
        }
        .topbar {
            padding: 0 0 20px 0;
            border-bottom: 1px solid #e5e7eb;
            margin-bottom: 20px;
        }
        .topbar .actions {
            margin-left: auto;
        }
        .quote-search { 
            display: flex; 
            justify-content: space-between;
            align-items: center;
            gap: 16px; 
            margin-bottom: 20px; 
        }
        .quote-search form { display: flex; gap: 10px; flex: 1; max-width: 520px; min-width: 0; }
        .quote-search .search-input {
            flex: 1;
        }
        .quote-search .btn { white-space: nowrap; }
        .quote-search .quote-filter { display: flex; gap: 4px; padding: 4px; background: #fff; border-radius: 8px; border: 1px solid #e5e7eb; }
        .quote-search .quote-filter .filter-btn {
            padding: 6px 14px;
            font-size: 13px;
            font-weight: 600;
            color: #6b7280;
            text-decoration: none;
            border-radius: 6px;
            transition: all 0.2s;
        }
        .quote-search .quote-filter .filter-btn:hover { background: #e5e7eb; color: #374151; }
        .quote-search .quote-filter .filter-btn.active {
            background: #dc2626;
            color: #fff;
            box-shadow: 0 2px 8px rgba(220, 38, 38, 0.3);
        }
        .quote-card-actions { justify-self: end; display: flex; gap: 8px; }
        .quote-card-actions .btn { white-space: nowrap; }
        .quote-card-actions.seller-info-hidden { display: none; }
    </style>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main quote-page">
            <header class="topbar">
                <div><h1>收到报价 <span class="subtitle">报价列表</span></h1></div>
                <div class="actions"><a class="btn back" href="buyer-workbench.aspx" data-back>返回采购</a></div>
            </header>

            <section class="panel">
                <div class="quote-search">
                    <form id="searchForm" method="get" action="received-quotes.aspx" style="display:flex;gap:10px;flex:1;max-width:520px;min-width:0;align-items:center;">
                        <input type="hidden" name="filter" value="<%= Request.QueryString["filter"] ?? "" %>" />
                        <input type="text" name="keyword" placeholder="搜索型号或参数" value="<%= Request.QueryString["keyword"] ?? "" %>" class="input search-input" />
                        <button type="submit" class="btn primary">搜索</button>
                        <% if (!string.IsNullOrEmpty(Request.QueryString["keyword"])) { %>
                            <a href="received-quotes.aspx<%= !string.IsNullOrEmpty(Request.QueryString["filter"]) ? "?filter=" + Request.QueryString["filter"] : "" %>" class="btn">清除</a>
                        <% } %>
                    </form>
                    <div class="quote-filter">
                        <a href="received-quotes.aspx<%= !string.IsNullOrEmpty(Request.QueryString["keyword"]) ? "?keyword=" + System.Web.HttpUtility.UrlEncode(Request.QueryString["keyword"]) : "" %>" class="filter-btn <%= Request.QueryString["filter"] == null || Request.QueryString["filter"] == "" ? "active" : "" %>">全部</a>
                        <a href="received-quotes.aspx?filter=unread<%= !string.IsNullOrEmpty(Request.QueryString["keyword"]) ? "&keyword=" + System.Web.HttpUtility.UrlEncode(Request.QueryString["keyword"]) : "" %>" class="filter-btn <%= Request.QueryString["filter"] == "unread" ? "active" : "" %>">未查看</a>
                        <a href="received-quotes.aspx?filter=read<%= !string.IsNullOrEmpty(Request.QueryString["keyword"]) ? "&keyword=" + System.Web.HttpUtility.UrlEncode(Request.QueryString["keyword"]) : "" %>" class="filter-btn <%= Request.QueryString["filter"] == "read" ? "active" : "" %>">已查看</a>
                    </div>
                </div>

                <% if (!HasQuoteData) { %>
                <div class="quote-empty">
                    <div class="empty-icon">📬</div>
                    <h3>暂无报价</h3>
                    <p>您还没有收到供应商的报价，去发布询价吧！</p>
                </div>
                <% } else { %>
                <div class="quote-list">
                    <asp:Repeater ID="rptQuotes" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="quote-card <%# (bool)Eval("IsNew") ? "is-new" : "" %>" data-eq-id="<%# Eval("EqId") %>">
                                <div class="quote-main">
                                    <h3>
                                        <%# (bool)Eval("IsViewed") ? "" : "<span class=\"model-text\">" + Eval("Model") + "</span>" %>
                                        <a href="/quote-detail.aspx?id=<%# Eval("EqId") %>" class="model-link" style="<%# (bool)Eval("IsViewed") ? "" : "display:none;" %>"><%# Eval("Model") %></a>
                                    </h3>
                                    <p><%# Eval("Brand") %> · <%# Eval("Quantity") %><%# Eval("Unit") %> · 批次: <%# Eval("Batch") %> · <%# Eval("QuoteTime") %></p>
                                    <div class="quote-tags">
                                        <%# System.Web.HttpUtility.HtmlDecode(Eval("ParamsHtml").ToString()) %>
                                    </div>
                                </div>
                                <div class="quote-price">
                                    <%# System.Web.HttpUtility.HtmlDecode(Eval("PriceAreaHtml").ToString()) %>
                                </div>
                                <div class="quote-seller">
                                    <b class="<%# Eval("SellerInfoHiddenClass") %>" data-seller-info><%# Eval("SellerName") %></b>
                                    <span><%# Eval("Validity") %></span>
                                </div>
                                <div class="quote-card-actions <%# Eval("SellerInfoHiddenClass") %>" data-seller-info>
                                    <a class="btn soft mini" href="tencent://message/?uin=<%# Eval("SellerQQ") %>" target="_blank" data-qq-btn="<%# Eval("SellerQQ") %>">联系供应商</a>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <% if (TotalPages > 1) { %>
                <div class="pagination" style="margin-top:24px; justify-content:center;">
                    <% 
                        string urlParams = "";
                        if (!string.IsNullOrEmpty(Request.QueryString["filter"])) urlParams += "&filter=" + Request.QueryString["filter"];
                        if (!string.IsNullOrEmpty(Request.QueryString["keyword"])) urlParams += "&keyword=" + System.Web.HttpUtility.UrlEncode(Request.QueryString["keyword"]);
                        urlParams = urlParams.TrimStart('&');
                    %>
                    <% if (CurrentPage > 1) { %>
                    <a class="btn" href="received-quotes.aspx?page=<%= CurrentPage - 1 %><%= urlParams.Length > 0 ? "&" + urlParams : "" %>">上一页</a>
                    <% } else { %>
                    <button class="btn" disabled>上一页</button>
                    <% } %>
                    <span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span>
                    <% if (CurrentPage < TotalPages) { %>
                    <a class="btn" href="received-quotes.aspx?page=<%= CurrentPage + 1 %><%= urlParams.Length > 0 ? "&" + urlParams : "" %>">下一页</a>
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
            document.querySelectorAll('[data-view-price]').forEach(function(btn) {
                btn.addEventListener('click', function() {
                    var eqId = this.getAttribute('data-eq-id');
                    var card = this.closest('.quote-card');
                    var priceArea = card.querySelector('.quote-price');
                    var priceDisplay = this.getAttribute('data-price');
                    var self = this;

                    fetch('/api/mark-quote-readed.aspx?eqId=' + eqId, {
                        method: 'GET',
                        credentials: 'same-origin'
                    }).then(function(res) {
                        return res.json();
                    }).then(function(result) {
                        self.style.display = 'none';
                        var priceHidden = priceArea.querySelector('.price-hidden');
                        if (priceHidden) priceHidden.style.display = 'none';

                        var priceDiv = document.createElement('div');
                        priceDiv.innerHTML = '<strong>' + priceDisplay + '</strong><span>点击查看详情</span>';
                        priceArea.appendChild(priceDiv);

                        card.querySelectorAll('[data-seller-info]').forEach(function(el) {
                            el.classList.remove('seller-info-hidden');
                        });

                        var modelText = card.querySelector('.model-text');
                        var modelLink = card.querySelector('.model-link');
                        if (modelText) modelText.style.display = 'none';
                        if (modelLink) modelLink.style.display = 'inline';

                        if (card.classList.contains('is-new')) {
                            card.classList.remove('is-new');
                        }
                    }).catch(function(err) {
                        console.error('标记已查看失败:', err);
                        self.style.display = 'none';
                        var priceHidden = priceArea.querySelector('.price-hidden');
                        if (priceHidden) priceHidden.style.display = 'none';
                        var priceDiv = document.createElement('div');
                        priceDiv.innerHTML = '<strong>' + priceDisplay + '</strong><span>点击查看详情</span>';
                        priceArea.appendChild(priceDiv);
                        
                        card.querySelectorAll('[data-seller-info]').forEach(function(el) {
                            el.classList.remove('seller-info-hidden');
                        });
                        
                        var modelText = card.querySelector('.model-text');
                        var modelLink = card.querySelector('.model-link');
                        if (modelText) modelText.style.display = 'none';
                        if (modelLink) modelLink.style.display = 'inline';
                        
                        if (card.classList.contains('is-new')) {
                            card.classList.remove('is-new');
                        }
                    });
                });
            });

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
