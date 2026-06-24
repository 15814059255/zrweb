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
                <div class="table-wrap">
                    <table class="table inquiry-table">
                        <thead><tr><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>采购数量</th><th>期望单价</th><th>采购商</th><th>询价时间</th><th>操作</th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptInquiries" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr data-eq-id="<%# Eval("EqId") %>" data-goods-id="<%# Eval("GoodsId") %>" data-buyer-name="<%# Eval("BuyerName") %>" data-model="<%# Eval("Model") %>" data-to-shop-id="<%# Eval("FromShopId") %>" data-brand-params="<%# Eval("BrandParams") %>" data-quantity="<%# Eval("Quantity") %>" data-unit="<%# Eval("Unit") %>" data-expected-price="<%# Eval("ExpectedPrice") %>">
                                        <td><span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span></td>
                                        <td><strong><%# Eval("Model") %></strong></td>
                                        <td><%# Eval("BrandParams") %></td>
                                        <td><%# Eval("Quantity") %>&nbsp;<%# Eval("Unit") %></td>
                                        <td><%# Eval("ExpectedPrice") %></td>
                                        <td><%# Eval("BuyerName") %></td>
                                        <td><%# Eval("InquiryTime") %></td>
                                        <td><button class="btn mini" data-quote-open>报价</button></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <div class="pagination"><button class="btn" disabled>上一页</button><span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span><span class="page-size">每页 50 条</span><button class="btn">下一页</button></div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>