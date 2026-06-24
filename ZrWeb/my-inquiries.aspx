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
    <style>
        .main {
            background: #f5f7fa;
        }
        .inquiry-list {
            display: flex;
            flex-direction: column;
            gap: 14px;
        }
        .inquiry-card {
            background: #fff;
            border: 1px solid #e5e7eb;
            border-radius: 14px;
            padding: 20px 24px;
            transition: all 0.25s ease;
            position: relative;
            overflow: hidden;
        }
        .inquiry-card::before {
            content: '';
            position: absolute;
            left: 0;
            top: 0;
            bottom: 0;
            width: 4px;
            background: linear-gradient(180deg, #3b82f6 0%, #06b6d4 100%);
            opacity: 0;
            transition: opacity 0.25s;
        }
        .inquiry-card:hover {
            box-shadow: 0 8px 24px rgba(0,0,0,0.08);
            border-color: #bfdbfe;
            transform: translateY(-1px);
        }
        .inquiry-card:hover::before {
            opacity: 1;
        }
        .inquiry-card-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 14px;
        }
        .inquiry-model {
            font-size: 19px;
            font-weight: 700;
            color: #111827;
            letter-spacing: 0.3px;
        }
        .inquiry-brand {
            font-size: 13px;
            color: #dc2626;
            font-weight: 600;
            margin-top: 5px;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }
        .inquiry-brand::before {
            content: '';
            display: inline-block;
            width: 6px;
            height: 6px;
            background: #dc2626;
            border-radius: 50%;
        }
        .inquiry-status {
            flex-shrink: 0;
        }
        .inquiry-params {
            display: flex;
            flex-wrap: wrap;
            gap: 7px;
            margin-bottom: 14px;
        }
        .param-tag {
            display: inline-block;
            padding: 4px 12px;
            background: linear-gradient(135deg, #f3f4f6 0%, #e5e7eb 100%);
            color: #4b5563;
            font-size: 12px;
            border-radius: 6px;
            line-height: 1.6;
            font-weight: 500;
            transition: all 0.2s;
        }
        .param-tag:hover {
            background: linear-gradient(135deg, #e5e7eb 0%, #d1d5db 100%);
        }
        .inquiry-info-row {
            display: flex;
            flex-wrap: wrap;
            gap: 28px;
            padding-top: 14px;
            border-top: 1px solid #f3f4f6;
            font-size: 13px;
            color: #6b7280;
        }
        .inquiry-info-item {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .inquiry-info-item .label {
            color: #9ca3af;
            font-size: 12px;
        }
        .inquiry-info-item .value {
            color: #374151;
            font-weight: 500;
        }
        .inquiry-price {
            color: #dc2626;
            font-weight: 700;
            font-size: 16px;
        }
        .inquiry-remarks {
            margin-top: 10px;
            padding-top: 10px;
            border-top: 1px dashed #f3f4f6;
            font-size: 13px;
            color: #6b7280;
        }
        .inquiry-remarks .label {
            color: #9ca3af;
            margin-right: 6px;
        }
        .buyer-demand-panel {
            background: transparent !important;
            border: none !important;
            box-shadow: none !important;
            padding: 0 !important;
        }
        .section-title {
            margin-bottom: 14px !important;
        }
    </style>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div><h1>我的询价</h1></div>
                <div class="actions"><a class="btn back" href="buyer-workbench.aspx" data-back>返回采购</a></div>
            </header>
            <section class="panel buyer-demand-panel" id="inquiryPanel" style="margin-top:18px">
                <div class="section-title"><div><h2>询价记录</h2></div></div>
                <div class="inquiry-list">
                    <% if (!HasInquiryData) { %>
                    <div style="text-align:center;padding:40px;">
                        <div class="empty-state" style="display:inline-block;padding:30px 40px;">
                            <div class="empty-icon">📋</div>
                            <h3>暂无询价记录</h3>
                            <p>您还没有发出过询价</p>
                        </div>
                    </div>
                    <% } else { %>
                    <asp:Repeater ID="rptInquiries" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="inquiry-card">
                                <div class="inquiry-card-header">
                                    <div>
                                        <div class="inquiry-model"><%# Eval("Model") %></div>
                                        <div class="inquiry-brand"><%# Eval("Brand") %></div>
                                    </div>
                                    <div class="inquiry-status">
                                        <span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span>
                                    </div>
                                </div>
                                <div class="inquiry-params">
                                    <%# System.Web.HttpUtility.HtmlDecode(Eval("ParamsHtml").ToString()) %>
                                </div>
                                <div class="inquiry-info-row">
                                    <div class="inquiry-info-item">
                                        <span class="label">数量</span>
                                        <span class="value"><%# Eval("Quantity") %> <%# Eval("Unit") %></span>
                                    </div>
                                    <div class="inquiry-info-item">
                                        <span class="label">期望单价</span>
                                        <span class="value inquiry-price"><%# Eval("ExpectedPrice") %></span>
                                    </div>
                                    <div class="inquiry-info-item">
                                        <span class="label">询价对象</span>
                                        <span class="value"><%# Eval("ShopName") %></span>
                                    </div>
                                    <div class="inquiry-info-item">
                                        <span class="label">询价时间</span>
                                        <span class="value"><%# Eval("InquiryTime") %></span>
                                    </div>
                                </div>
                                <%# System.Web.HttpUtility.HtmlDecode(Eval("RemarksDisplay").ToString()) %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <% } %>
                </div>
                <% if (HasInquiryData) { %>
                <div class="pagination" style="margin-top:20px;">
                    <% if (CurrentPage > 1) { %>
                    <a class="btn" href="my-inquiries.aspx?page=<%= CurrentPage - 1 %>">上一页</a>
                    <% } else { %>
                    <button class="btn" disabled>上一页</button>
                    <% } %>
                    <span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span>
                    <% if (CurrentPage < TotalPages) { %>
                    <a class="btn" href="my-inquiries.aspx?page=<%= CurrentPage + 1 %>">下一页</a>
                    <% } else { %>
                    <button class="btn" disabled>下一页</button>
                    <% } %>
                </div>
                <% } %>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>
