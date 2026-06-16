<%@ Page Language="C#" AutoEventWireup="true" CodeFile="my-inquiries.aspx.cs" Inherits="my_inquiries" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
                <div><h1>我的询价</h1></div>
                <div class="actions"><a class="btn back" href="buyer-workbench.aspx" data-back>返回采购</a></div>
            </header>
            <section class="panel quote-panel">
                <div class="table-wrap">
                    <table class="table quote-table">
                        <thead><tr><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>询价数量</th><th>期望价</th><th>目标供应商</th><th>询价时间</th><th>有效期</th><th>回复数</th></tr></thead>
                        <tbody>
                            <% if (!HasData) { %>
                            <tr><td colspan="9" style="text-align:center;padding:40px;">
                                <div class="empty-state" style="display:inline-block;padding:30px 40px;">
                                    <div class="empty-icon">📋</div>
                                    <h3>暂无询价记录</h3>
                                    <p>发布采购需求后，可向供应商发起询价</p>
                                </div>
                            </td></tr>
                            <% } else { %>
                            <asp:Repeater ID="rptInquiries" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr data-eq-id="<%# Eval("EqId") %>" data-goods-id="<%# Eval("GoodsId") %>" data-model="<%# Eval("Model") %>">
                                        <td><span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span></td>
                                        <td><strong><%# Eval("Model") %></strong></td>
                                        <td><%# Eval("BrandParams") %></td>
                                        <td><%# Eval("Quantity") %>&nbsp;<%# Eval("Unit") %></td>
                                        <td><%# Eval("Price") %></td>
                                        <td><%# Eval("TargetCompany") %></td>
                                        <td><%# Eval("InquiryTime") %></td>
                                        <td><%# Eval("Validity") %></td>
                                        <td><span class="badge"><%# Eval("ReplyCount") %></span></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } %>
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