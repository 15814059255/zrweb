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
        .quote-list { max-height: 500px; overflow-y: auto; }
        .quote-item { background: #fafafa; border-radius: 8px; padding: 14px; margin-bottom: 12px; border: 1px solid #eee; }
        .quote-item:last-child { margin-bottom: 0; }
        .quote-header { display: flex; justify-content: space-between; margin-bottom: 10px; align-items: center; }
        .quote-name { font-weight: 600; color: #333; font-size: 14px; }
        .quote-price { color: #e74c3c; font-weight: 600; font-size: 18px; }
        .quote-time { font-size: 12px; color: #999; margin-bottom: 8px; }
        .quote-meta { font-size: 12px; color: #666; margin-top: 4px; line-height: 1.6; }
        .quote-meta-row { display: flex; flex-wrap: wrap; gap: 12px; margin-top: 6px; }
        .quote-meta-item { display: flex; }
        .quote-meta-label { color: #999; margin-right: 4px; }
        .quote-meta-value { color: #333; }
        .tag { display: inline-block; padding: 2px 8px; border-radius: 4px; font-size: 11px; }
        .tag-blue { background: #e8f4fd; color: #1890ff; }
        .tag-green { background: #e6f7ed; color: #52c41a; }
        .tag-orange { background: #fff3e0; color: #fa8c16; }
        .empty-tip { text-align: center; color: #999; padding: 40px 0; font-size: 14px; }
        .remarks-box { background: #fff; border: 1px solid #eee; border-radius: 4px; padding: 8px 10px; margin-top: 8px; font-size: 12px; color: #666; line-height: 1.5; }
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
                        <span class="quote-price">
                            <% 
                                decimal price = 0;
                                if (row["Price"] != null && row["Price"] != DBNull.Value && decimal.TryParse(row["Price"].ToString(), out price) && price > 0) {
                                    Response.Write("¥" + price.ToString("0.00"));
                                } else {
                                    Response.Write("面议");
                                }
                            %>
                            <% 
                                int isIncludingTax = 0;
                                if (row.Table.Columns.Contains("isIncludingTax") && row["isIncludingTax"] != null && row["isIncludingTax"] != DBNull.Value) {
                                    int.TryParse(row["isIncludingTax"].ToString(), out isIncludingTax);
                                }
                                if (price > 0) {
                            %>
                            <span class="tag <%= isIncludingTax == 1 ? "tag-blue" : "tag-orange" %>" style="margin-left:6px;font-weight:normal;"><%= isIncludingTax == 1 ? "含税" : "未税" %></span>
                            <% } %>
                        </span>
                    </div>
                    <div class="quote-time">
                        <% 
                            DateTime createTime = DateTime.Now;
                            if (row["CreateTime"] != null && row["CreateTime"] != DBNull.Value && DateTime.TryParse(row["CreateTime"].ToString(), out createTime)) {
                                Response.Write(createTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        %>
                    </div>
                    <div class="quote-meta-row">
                        <% 
                            int fromQuantity = 0;
                            if (row["fromQuantity"] != null && row["fromQuantity"] != DBNull.Value) {
                                int.TryParse(row["fromQuantity"].ToString(), out fromQuantity);
                            }
                        %>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">数量：</span>
                            <span class="quote-meta-value"><%= fromQuantity > 0 ? fromQuantity + " Kpcs" : "按需供应" %></span>
                        </div>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">联系人：</span>
                            <span class="quote-meta-value"><%= row["LinkMan"] %></span>
                        </div>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">电话：</span>
                            <span class="quote-meta-value"><%= row["MobilePhone"] %></span>
                        </div>
                        <% 
                            string shopQQ = "";
                            if (row.Table.Columns.Contains("shopQQ") && row["shopQQ"] != null && row["shopQQ"] != DBNull.Value) {
                                shopQQ = row["shopQQ"].ToString().Trim();
                            }
                            if (!string.IsNullOrEmpty(shopQQ)) {
                        %>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">QQ：</span>
                            <span class="quote-meta-value"><%= shopQQ %></span>
                        </div>
                        <% } %>
                    </div>
                    <% 
                        string brandName = "";
                        if (row.Table.Columns.Contains("brandName") && row["brandName"] != null && row["brandName"] != DBNull.Value) {
                            brandName = row["brandName"].ToString().Trim();
                        }
                        string fromLot = "";
                        if (row.Table.Columns.Contains("fromLot") && row["fromLot"] != null && row["fromLot"] != DBNull.Value) {
                            fromLot = row["fromLot"].ToString().Trim();
                        }
                        string validity = "";
                        if (row.Table.Columns.Contains("validity") && row["validity"] != null && row["validity"] != DBNull.Value) {
                            validity = row["validity"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(brandName) || !string.IsNullOrEmpty(fromLot) || !string.IsNullOrEmpty(validity)) {
                    %>
                    <div class="quote-meta-row">
                        <% if (!string.IsNullOrEmpty(brandName)) { %>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">品牌参数：</span>
                            <span class="quote-meta-value"><%= brandName %></span>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(fromLot)) { %>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">批号：</span>
                            <span class="quote-meta-value"><%= fromLot %></span>
                        </div>
                        <% } %>
                        <% if (!string.IsNullOrEmpty(validity)) { %>
                        <div class="quote-meta-item">
                            <span class="quote-meta-label">有效期：</span>
                            <span class="quote-meta-value"><%= validity %></span>
                        </div>
                        <% } %>
                    </div>
                    <% } %>
                    <% 
                        string remarks = "";
                        if (row["Remarks"] != null && row["Remarks"] != DBNull.Value) {
                            remarks = row["Remarks"].ToString().Trim();
                        }
                        if (!string.IsNullOrEmpty(remarks)) {
                    %>
                    <div class="remarks-box">
                        <strong style="color:#666;">备注：</strong><%= remarks %>
                    </div>
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