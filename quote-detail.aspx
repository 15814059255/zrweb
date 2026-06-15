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
        <main class="main">
            <header class="topbar">
                <div><h1>报价详情</h1></div>
                <div class="actions"><a class="btn back" href="/quote-records.aspx" data-back>返回列表</a></div>
            </header>

            <section class="panel detail-hero">
                <div class="detail-header">
                    <span class="tag blue">报价</span>
                    <h2><%= Model %></h2>
                    <div class="price"><%= PriceDisplay %></div>
                </div>
                <div class="detail-meta">
                    <div class="meta-row"><span class="label">型号</span><span class="value"><%= Model %></span></div>
                    <div class="meta-row"><span class="label">品牌</span><span class="value"><%= BrandName %></span></div>
                    <div class="meta-row"><span class="label">报价数量</span><span class="value"><%= Quantity %> <%= Unit %></span></div>
                    <div class="meta-row"><span class="label">报价单价</span><span class="value"><%= PriceDisplay %></span></div>
                    <div class="meta-row"><span class="label">采购商</span><span class="value"><%= BuyerName %></span></div>
                    <div class="meta-row"><span class="label">报价时间</span><span class="value"><%= QuoteTime %></span></div>
                    <div class="meta-row"><span class="label">有效期</span><span class="value"><%= Validity %></span></div>
                    <div class="meta-row"><span class="label">状态</span><span class="value"><span class="tag <%= StatusClass %>"><%= Status %></span></span></div>
                </div>
            </section>

            <section class="panel detail-remarks">
                <h3>备注说明</h3>
                <p><%= Remarks %></p>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>