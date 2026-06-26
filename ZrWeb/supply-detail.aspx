<%@ Page Language="C#" AutoEventWireup="true" CodeFile="supply-detail.aspx.cs" Inherits="supply_detail" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

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
        
        <main class="main detail-page">
            <header class="topbar">
                <div>
                    <h1>供应详情</h1>
                </div>
                <div class="actions">
                    <a class="btn back" href="/search.aspx" data-back>返回搜索</a>
                    <% if (IsLoggedIn && IsMerchant) { %>
                    <a class="btn soft" href="/merchant-workbench.aspx">商家后台</a>
                    <% } %>
                </div>
            </header>

            <div class="detail-container" data-goods-id="<%= GoodsId %>" data-shop-id="<%= ShopId %>" data-goods-sn="<%= Model %>" data-brand="<%= Brand %>">
                <div class="detail-main">
                    <section class="detail-card detail-hero">
                        <div class="detail-hero-inner">
                            <div class="detail-hero-left">
                                <div class="detail-badge">
                                    <span class="tag blue">供应</span>
                                </div>
                                <h1 class="detail-title"><%= Model %></h1>
                                <% if (!string.IsNullOrEmpty(ParametersSummary)) { %>
                                <div class="detail-subtitle"><%= ParametersSummary %></div>
                                <% } %>
                            </div>
                            <div class="detail-hero-right">
                                <div class="detail-price-wrap">
                                    <span class="detail-price-label">报价</span>
                                    <div class="detail-price"><%= PriceDisplay %></div>
                                </div>
                            </div>
                        </div>
                    </section>

                    <section class="detail-card">
                        <div class="detail-card-header">
                            <h3>供应信息</h3>
                        </div>
                        <div class="spec-table">
                            <% if (!string.IsNullOrEmpty(Brand)) { %>
                            <div class="spec-row">
                                <span class="spec-label">品牌</span>
                                <span class="spec-value"><%= Brand %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Package)) { %>
                            <div class="spec-row">
                                <span class="spec-label">封装</span>
                                <span class="spec-value"><%= Package %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Capacitance)) { %>
                            <div class="spec-row">
                                <span class="spec-label">容值</span>
                                <span class="spec-value"><%= Capacitance %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Resistance)) { %>
                            <div class="spec-row">
                                <span class="spec-label">阻值</span>
                                <span class="spec-value"><%= Resistance %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Tolerance)) { %>
                            <div class="spec-row">
                                <span class="spec-label">精度</span>
                                <span class="spec-value"><%= Tolerance %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Voltage)) { %>
                            <div class="spec-row">
                                <span class="spec-label">耐压</span>
                                <span class="spec-value"><%= Voltage %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Dielectric)) { %>
                            <div class="spec-row">
                                <span class="spec-label">介质</span>
                                <span class="spec-value"><%= Dielectric %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(Power)) { %>
                            <div class="spec-row">
                                <span class="spec-label">功率</span>
                                <span class="spec-value"><%= Power %></span>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(TempCoefficient)) { %>
                            <div class="spec-row">
                                <span class="spec-label">温漂</span>
                                <span class="spec-value"><%= TempCoefficient %></span>
                            </div>
                            <% } %>
                            <div class="spec-row">
                                <span class="spec-label">数量</span>
                                <span class="spec-value"><%= Quantity %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">有效期</span>
                                <span class="spec-value"><%= Validity %></span>
                            </div>
                            <% if (!string.IsNullOrEmpty(Remarks)) { %>
                            <div class="spec-row">
                                <span class="spec-label">备注</span>
                                <span class="spec-value"><%= Remarks %></span>
                            </div>
                            <% } %>
                        </div>
                    </section>

                    <section class="detail-card detail-company">
                        <div class="detail-card-header">
                            <h3>供应商信息</h3>
                        </div>
                        <div class="company-card">
                            <div class="company-header">
                                <div class="company-name"><%= CompanyName %></div>
                                <div class="company-tags">
                                    <span class="company-auth tag <%= AuthStatus == "已认证" ? "green" : "orange" %>"><%= AuthStatus %></span>
                                    <% if (AuthStatus == "已认证") { %>
                                    <span class="company-seal">📜 公章</span>
                                    <% } %>
                                </div>
                            </div>
                            <div class="company-address">📍 <%= CompanyAddress %></div>
                        </div>
                    </section>

                    <section class="detail-card">
                        <div class="detail-card-header">
                            <h3>业务信息</h3>
                        </div>
                        <div class="supplier-capabilities">
                            <% if (!string.IsNullOrEmpty(MainBrands)) { %>
                            <div class="capability-item">
                                <span class="capability-label">主营品牌</span>
                                <div class="capability-value">
                                    <% foreach (string brand in MainBrands.Split('|')) { %>
                                    <span class="brand-chip"><%= brand %></span>
                                    <% } %>
                                </div>
                            </div>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(AdvantageModels)) { %>
                            <div class="capability-item">
                                <span class="capability-label">优势型号</span>
                                <div class="capability-value models-list">
                                    <%= AdvantageModels.Replace("|", " · ") %>
                                </div>
                            </div>
                            <% } %>
                        </div>
                    </section>

                    <% if (IsLoggedIn && IsMerchant) { %>
                    <section class="detail-card detail-actions">
                        <div class="actions-grid">
                            <a class="btn soft" href="/merchant-workbench.aspx">查看更多供应</a>
                        </div>
                    </section>
                    <% } %>
                </div>

                <aside class="detail-sidebar">
                    <div class="detail-card sidebar-card">
                        <h3>快速导航</h3>
                        <div class="sidebar-actions">
                            <a class="btn block primary" href="#tradeInteractionModal">立即询价</a>
                            <a class="btn block soft" href="/search.aspx">🔍 返回搜索</a>
                            <% if (IsLoggedIn && IsMerchant) { %>
                            <a class="btn block soft" href="/merchant-workbench.aspx">📋 商家后台</a>
                            <% } %>
                        </div>
                    </div>
                    <div class="detail-card sidebar-card">
                        <h3>询价须知</h3>
                        <ul class="tips-list">
                            <li>请确认询价数量准确无误</li>
                            <li>询价后可在采购商后台查看</li>
                            <li>供应商将收到询价通知</li>
                        </ul>
                    </div>
                </aside>
            </div>

            <section class="panel site-ad-panel" hidden>
                <a class="search-ad-card" href="/search.aspx" data-ad-slot="DETAIL-S01">
                    <b>白银广告 DETAIL-S01</b>
                    <span>详情页底部，适合相关配件推荐和替代型号。</span>
                </a>
            </section>
        </main>
    </div>
    
    <!-- 底部 -->
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>