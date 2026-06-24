<%@ Page Language="C#" AutoEventWireup="true" CodeFile="search.aspx.cs" Inherits="search" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

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
</head>
<body>
    <div class="app">
        <!-- 头部侧边栏 -->
        <uc1:head runat="server" ID="head" />
        
        <main class="main">
            <header class="topbar">
                <div>
                    <h1>搜索结果</h1>
                </div>
                <div class="actions">
                    <a class="btn back" href="/index.aspx" data-back>返回</a>
                    <button class="btn soft" type="button" data-quick-import-open>快捷发布</button>
                </div>
            </header>

            <!-- 搜索广告位 -->
            <section class="panel search-ad-panel" hidden data-ad-group="search-before">
                <div class="search-ad-grid">
                    <a class="search-ad-card" href="/supply-detail.aspx" data-ad-slot="SR-D01">
                        <b>钻石广告 SR-D01</b>
                        <span>搜索前黄金视线位，适合重点库存和品牌曝光。</span>
                    </a>
                    <a class="search-ad-card" href="/supply-detail.aspx" data-ad-slot="SR-D02">
                        <b>钻石广告 SR-D02</b>
                        <span>搜索前高曝光位，适合现货专场和活动入口。</span>
                    </a>
                </div>
            </section>

            <!-- 搜索筛选 -->
            <section class="panel search-filter-panel">
                <div class="form-grid search-filter-grid">
                    <div class="search-keyword-field">
                        <label>搜索</label>
                        <input class="input" data-page-search-input value="<%= SearchKeyword %>" placeholder="输入型号、品牌、参数，如 GRM188 / 0603 / 100nF">
                    </div>
                </div>
                <div class="actions search-filter-actions" style="margin-top:16px">
                    <button class="btn primary" type="button" data-page-search-submit>查询</button>
                </div>
            </section>

            <!-- 搜索后广告位 -->
            <section class="panel search-ad-panel" hidden data-ad-group="search-after">
                <div class="search-ad-grid">
                    <a class="search-ad-card" href="/demand-detail.aspx" data-ad-slot="SR-G01">
                        <b>黄金广告 SR-G01</b>
                        <span>搜索后承接位，适合精准报价和急采服务。</span>
                    </a>
                    <a class="search-ad-card" href="/merchant-workbench.aspx" data-ad-slot="SR-G02">
                        <b>黄金广告 SR-G02</b>
                        <span>搜索后推荐位，适合供应商入驻和推广。</span>
                    </a>
                </div>
            </section>

            <!-- 搜索结果 -->
            <section class="panel search-results-panel" style="margin-top:18px">
                <div class="section-title">
                    <div>
                        <h2>搜索结果</h2>
                        <p>找到 <%= ResultCount %> 条匹配信息，优先展示有效期内和资料完善公司。</p>
                    </div>
                </div>
                
                <div class="card-list">
                    <!-- 搜索结果列表 - 后端绑定 -->
                    <asp:Repeater ID="rptSearchResults" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="item" data-shop-id="<%# Eval("ShopId") %>">
                                <div>
                                    <h3><a href="<%# Eval("DetailUrl") %>"><%# Eval("Model") %></a></h3>
                                    <div class="meta">
                                        <span class="tag <%# Eval("TagClass") %>"><%# Eval("TypeLabel") %></span>
                                        <span class="company"><%# Eval("CompanyName") %></span>
                                        <span><%# Eval("BrandParams") %></span>
                                        <span><%# Eval("QuantityDisplay") %></span>
                                        <span class="validity-line">有效期 <%# Eval("Validity") %></span>
                                    </div>
                                </div>
                                <div>
                                    <div class="price <%# Eval("PriceClass") %>"><%# Eval("PriceDisplay") %></div>
                                    <div class="actions search-result-actions">
                                        <a class="btn soft mini <%# Eval("ActionClass") %>" href="#tradeInteractionModal"><%# Eval("ActionText") %></a>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                
                <!-- 分页 -->
                <div class="pagination">
                    <%= PaginationHtml %>
                </div>
            </section>
        </main>
    </div>
    
    <!-- 底部 -->
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>