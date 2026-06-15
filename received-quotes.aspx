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
                <div class="tabs quote-tabs" data-quote-tabs>
                    <span class="tab active" data-tab data-filter="new">新报价</span>
                    <span class="tab" data-tab data-filter="recommended">推荐</span>
                    <span class="tab" data-tab data-filter="latest">最新</span>
                    <span class="tab" data-tab data-filter="expiring">即将过期</span>
                    <span class="tab" data-tab data-filter="expired">已过期</span>
                </div>
            </section>
            <section class="panel quote-panel">
                <div class="table-wrap">
                    <table class="table quote-table">
                        <thead><tr><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>报价数量</th><th>单价</th><th>供应商</th><th>报价时间</th><th>有效期</th><th>操作</th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptQuotes" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr data-eq-id="<%# Eval("EqId") %>" data-goods-id="<%# Eval("GoodsId") %>" data-seller-name="<%# Eval("SellerName") %>" data-model="<%# Eval("Model") %>">
                                        <td><span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span></td>
                                        <td><strong><%# Eval("Model") %></strong></td>
                                        <td><%# Eval("BrandParams") %></td>
                                        <td><%# Eval("Quantity") %>&nbsp;<%# Eval("Unit") %></td>
                                        <td><%# Eval("Price") %></td>
                                        <td><%# Eval("SellerName") %></td>
                                        <td><%# Eval("QuoteTime") %></td>
                                        <td><%# Eval("Validity") %></td>
                                        <td><button class="btn mini" data-contact-seller>联系供应商</button></td>
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