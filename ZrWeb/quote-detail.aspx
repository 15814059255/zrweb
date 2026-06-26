<%@ Page Language="C#" AutoEventWireup="true" CodeFile="quote-detail.aspx.cs" Inherits="quote_detail" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
        <uc1:head runat="server" ID="head" />
        <main class="main detail-page">
            <header class="topbar">
                <div>
                    <h1>报价详情</h1>
                </div>
                <div class="actions">
                    <a class="btn back" href="/quote-records.aspx" data-back>返回列表</a>
                </div>
            </header>

            <div class="detail-container">
                <div class="detail-main">
                    <section class="detail-card detail-hero">
                        <div class="detail-hero-inner">
                            <div class="detail-hero-left">
                                <div class="detail-badge">
                                    <span class="tag blue">报价</span>
                                </div>
                                <h1 class="detail-title"><%= Model %></h1>
                                <% if (!string.IsNullOrEmpty(BrandName)) { %>
                                <div class="detail-subtitle"><%= BrandName %></div>
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
                            <h3>报价信息</h3>
                        </div>
                        <div class="spec-table">
                            <div class="spec-row">
                                <span class="spec-label">型号</span>
                                <span class="spec-value"><%= Model %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">品牌</span>
                                <span class="spec-value"><%= BrandName %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">采购数量</span>
                                <span class="spec-value"><%= PurchaseQuantity %> <%= PurchaseUnit %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">报价数量</span>
                                <span class="spec-value"><%= Quantity %> <%= Unit %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">报价单价</span>
                                <span class="spec-value"><%= PriceDisplay %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">报价时间</span>
                                <span class="spec-value"><%= QuoteTime %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">有效期</span>
                                <span class="spec-value"><%= Validity %></span>
                            </div>
                            <div class="spec-row">
                                <span class="spec-label">状态</span>
                                <span class="spec-value"><span class="tag <%= StatusClass %>"><%= Status %></span></span>
                            </div>
                        </div>
                    </section>

                    <section class="detail-card">
                        <div class="detail-card-header">
                            <h3>采购商信息</h3>
                        </div>
                        <div class="company-card">
                            <div class="company-header">
                                <div class="company-name"><%= BuyerName %></div>
                            </div>
                            <div class="spec-table">
                                <% if (!string.IsNullOrEmpty(BuyerContact)) { %>
                                <div class="spec-row">
                                    <span class="spec-label">联系人</span>
                                    <span class="spec-value"><%= BuyerContact %></span>
                                </div>
                                <% } %>
                                <% if (!string.IsNullOrEmpty(BuyerTel)) { %>
                                <div class="spec-row">
                                    <span class="spec-label">联系电话</span>
                                    <span class="spec-value"><%= BuyerTel %></span>
                                </div>
                                <% } %>
                            </div>
                        </div>
                    </section>

                    <section class="detail-card">
                        <div class="detail-card-header">
                            <h3>供应商信息</h3>
                        </div>
                        <div class="company-card">
                            <div class="company-header">
                                <div class="company-name"><%= SupplierName %></div>
                            </div>
                            <div class="spec-table">
                                <% if (!string.IsNullOrEmpty(SupplierContact)) { %>
                                <div class="spec-row">
                                    <span class="spec-label">联系人</span>
                                    <span class="spec-value"><%= SupplierContact %></span>
                                </div>
                                <% } %>
                                <% if (!string.IsNullOrEmpty(SupplierTel)) { %>
                                <div class="spec-row">
                                    <span class="spec-label">联系电话</span>
                                    <span class="spec-value"><%= SupplierTel %></span>
                                </div>
                                <% } %>
                            </div>
                        </div>
                    </section>

                    <section class="detail-card">
                        <div class="detail-card-header">
                            <h3>备注说明</h3>
                        </div>
                        <div class="remarks-content">
                            <p><%= Remarks %></p>
                        </div>
                    </section>
                </div>
            </div>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>
