<%@ Page Language="C#" AutoEventWireup="true" CodeFile="received-inquiries.aspx.cs" Inherits="received_inquiries" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
                <div><h1>收到询价</h1></div>
                <div class="actions"><a class="btn back" href="merchant-workbench.aspx" data-back>返回商家</a></div>
            </header>
            <section class="panel">
                <div class="searchbar inventory-searchbar"><input class="input" data-inquiry-search placeholder="搜索型号、采购商"><button class="btn primary" data-inquiry-search-btn>搜索</button></div>
            </section>
            <section class="panel inquiry-panel">
                <div class="inquiry-list" id="inquiryList">
                    <asp:Repeater ID="rptInquiries" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="inquiry-card" data-eq-id="<%# Eval("EqId") %>" data-goods-id="<%# Eval("GoodsId") %>" data-buyer-name="<%# Eval("BuyerName") %>" data-model="<%# Eval("Model") %>" data-to-shop-id="<%# Eval("FromShopId") %>" data-brand-params="<%# Eval("BrandParams") %>" data-quantity="<%# Eval("Quantity") %>" data-unit="<%# Eval("Unit") %>" data-expected-price="<%# Eval("ExpectedPrice") %>">
                                <div class="inquiry-card-header">
                                    <div class="inquiry-status">
                                        <span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span>
                                        <span class="inquiry-time"><%# Eval("InquiryTime") %></span>
                                    </div>
                                    <button class="btn primary mini" data-quote-open <%# Convert.ToBoolean(Eval("IsGoodsActive")) ? "" : "disabled" %>><%# Convert.ToBoolean(Eval("IsGoodsActive")) ? "报价" : "已下架" %></button>
                                </div>
                                <div class="inquiry-card-body">
                                    <div class="inquiry-model">
                                        <strong><%# Eval("Model") %></strong>
                                    </div>
                                    <div class="inquiry-brand-params">
                                        <span class="params-label">品牌参数</span>
                                        <span class="params-value"><%# Eval("BrandParams") %></span>
                                    </div>
                                    <div class="inquiry-specs">
                                        <div class="spec-item">
                                            <span class="spec-label">采购数量</span>
                                            <span class="spec-value"><%# Eval("Quantity") %> <span class="spec-unit"><%# Eval("Unit") %></span></span>
                                        </div>
                                        <div class="spec-item">
                                            <span class="spec-label">期望单价</span>
                                            <span class="spec-value"><%# Eval("ExpectedPrice") %></span>
                                        </div>
                                        <div class="spec-item">
                                            <span class="spec-label">备注</span>
                                            <span class="spec-value remark-value"><%# Eval("Remarks") %></span>
                                        </div>
                                        <div class="spec-item">
                                            <span class="spec-label">有效期</span>
                                            <span class="spec-value"><%# Eval("Validity") %></span>
                                        </div>
                                    </div>
                                </div>
                                <div class="inquiry-card-footer">
                                    <div class="inquiry-buyer <%# Eval("Status").ToString() == "新询价" ? "buyer-hidden" : "" %>">
                                        <span class="buyer-icon">🏢</span>
                                        <span class="buyer-name"><%# Eval("BuyerName") %></span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <div runat="server" ID="divEmpty" visible="false" class="empty-state">
                        <div class="empty-icon">📭</div>
                        <h3><asp:Literal ID="litEmptyMsg" runat="server"></asp:Literal></h3>
                    </div>
                </div>
                <div class="pagination"><button class="btn" disabled>上一页</button><span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span><span class="page-size">每页 50 条</span><button class="btn">下一页</button></div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>