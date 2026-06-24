<%@ Page Language="C#" AutoEventWireup="true" CodeFile="goods-detail.aspx.cs" Inherits="api_goods_detail" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="utf-8">
    <style>
        .detail-section { margin-bottom: 20px; }
        .detail-section h3 { margin: 0 0 12px 0; font-size: 14px; color: #333; border-bottom: 1px solid #eee; padding-bottom: 8px; }
        .detail-row { display: flex; padding: 8px 0; border-bottom: 1px solid #f5f5f5; }
        .detail-label { width: 100px; flex-shrink: 0; color: #666; font-size: 13px; }
        .detail-value { flex: 1; color: #333; font-size: 13px; }
        .quote-list { max-height: 400px; overflow-y: auto; }
        .quote-item { background: #fafafa; border-radius: 8px; padding: 12px; margin-bottom: 10px; border: 1px solid #eee; }
        .quote-item:last-child { margin-bottom: 0; }
        .quote-header { display: flex; justify-content: space-between; margin-bottom: 8px; }
        .quote-name { font-weight: 600; color: #333; }
        .quote-price { color: #e74c3c; font-weight: 600; font-size: 16px; }
        .quote-time { font-size: 12px; color: #999; }
        .quote-meta { font-size: 12px; color: #666; margin-top: 4px; }
        .empty-tip { text-align: center; color: #999; padding: 40px 0; font-size: 14px; }
    </style>
</head>
<body>
    <div class="detail-section">
        <h3>商品基本信息</h3>
        <div class="detail-row"><span class="detail-label">型号</span><span class="detail-value"><%= GoodsSn %></span></div>
        <div class="detail-row"><span class="detail-label">发布者</span><span class="detail-value"><%= PublisherName %></span></div>
        <div class="detail-row"><span class="detail-label">类型</span><span class="detail-value"><%= PubTypeText %></span></div>
        <div class="detail-row"><span class="detail-label">价格</span><span class="detail-value">¥<%= ShopPrice %></span></div>
        <div class="detail-row"><span class="detail-label">库存</span><span class="detail-value"><%= GoodsStock %></span></div>
        <div class="detail-row"><span class="detail-label">单位</span><span class="detail-value"><%= GoodsUnit %></span></div>
        <div class="detail-row"><span class="detail-label">发布时间</span><span class="detail-value"><%= CreateTime %></span></div>
    </div>
    
    <div class="detail-section">
        <h3><%= PubType == 1 ? "询价方" : "报价方" %>列表（共 <%= QuoteCount %> 条）</h3>
        <div class="quote-list">
            <% if (QuoteList != null && QuoteList.Rows.Count > 0) { %>
                <% foreach (System.Data.DataRow row in QuoteList.Rows) { %>
                <div class="quote-item">
                    <div class="quote-header">
                        <span class="quote-name"><%= row["CompanyName"] %></span>
                        <span class="quote-price">¥<%= row["Price"] %></span>
                    </div>
                    <div class="quote-time"><%= row["CreateTime"] %></div>
                    <div class="quote-meta">
                        联系人：<%= row["LinkMan"] %> | 电话：<%= row["MobilePhone"] %>
                    </div>
                    <% if (!string.IsNullOrEmpty(row["Remarks"].ToString())) { %>
                    <div class="quote-meta" style="margin-top: 6px;">备注：<%= row["Remarks"] %></div>
                    <% } %>
                </div>
                <% } %>
            <% } else { %>
                <div class="empty-tip">暂无<%= PubType == 1 ? "询价" : "报价" %>信息</div>
            <% } %>
        </div>
    </div>
</body>
</html>