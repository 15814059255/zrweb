<%@ Page Language="C#" AutoEventWireup="true" CodeFile="demand-detail.aspx.cs" Inherits="demand_detail" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

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
                    <h1>需求详情</h1>
                </div>
                <div class="actions">
                    <a class="btn back" href="/search.aspx" data-back>返回搜索</a>
                    <a class="btn soft" href="/buyer-workbench.aspx">采购后台</a>
                </div>
            </header>

            <section class="panel detail-hero">
                <input type="hidden" id="hdnGoodsId" name="hdnGoodsId" value="<%= GoodsId %>">
                <input type="hidden" id="hdnGoodsSn" name="hdnGoodsSn" value="<%= GoodsSn %>">
                <div class="detail-header">
                    <span class="tag green">需求</span>
                    <h2><%= Title %></h2>
                    <div class="price <%= PriceClass %>"><%= PriceDisplay %></div>
                </div>
                <div class="detail-meta">
                    <div class="meta-row"><span class="label">型号</span><span class="value"><%= Model %></span></div>
                    <div class="meta-row"><span class="label">品牌要求</span><span class="value"><%= BrandRequirement %></span></div>
                    <div class="meta-row"><span class="label">参数要求</span><span class="value"><%= Parameters %></span></div>
                    <div class="meta-row"><span class="label">需求数量</span><span class="value"><%= Quantity %></span></div>
                    <div class="meta-row"><span class="label">有效期</span><span class="value"><%= Validity %></span></div>
                    <div class="meta-row"><span class="label">交期要求</span><span class="value"><%= DeliveryRequirement %></span></div>
                </div>
            </section>

            <section class="panel detail-company">
                <h3>采购商信息</h3>
                <div class="company-info">
                    <div class="company-name"><%= CompanyName %></div>
                    <div class="company-meta">
                        <span><%= CompanyAddress %></span>
                        <span>认证状态：<%= AuthStatus %></span>
                    </div>
                </div>
            </section>

            <section class="panel detail-actions">
                <div class="actions-grid">
                    <a class="btn primary" href="#quoteModal" data-quote-open>我要报价</a>
                    <a class="btn soft" href="/buyer-workbench.aspx">查看更多需求</a>
                </div>
            </section>

            <section class="panel site-ad-panel" hidden>
                <a class="search-ad-card" href="/search.aspx" data-ad-slot="DEMAND-S01">
                    <b>白银广告 DEMAND-S01</b>
                    <span>需求详情页底部，适合相关供应推荐。</span>
                </a>
            </section>
        </main>
    </div>
    
    <!-- 报价弹窗 -->
    <div class="modal-backdrop" id="quoteModal" hidden>
        <div class="modal quote-modal" role="dialog" aria-modal="true" aria-label="报价">
            <div class="modal-head">
                <h2>报价 <%= Title %></h2>
                <button class="modal-close" type="button" data-quote-close aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <form class="quote-form" data-quote-form>
                    <div class="form-row">
                        <label>报价单价 <em>*</em>
                            <input class="input" data-quote-price required placeholder="填写您的报价单价">
                        </label>
                    </div>
                    <div class="form-row">
                        <label>可供数量 <em>*</em>
                            <input class="input" data-quote-qty required placeholder="填写可供数量">
                        </label>
                    </div>
                    <div class="form-row">
                        <label>交期承诺
                            <input class="input" data-quote-delivery placeholder="填写交期（可选）">
                        </label>
                    </div>
                    <div class="form-row">
                        <label>备注说明
                            <textarea class="input" data-quote-note placeholder="品牌、批次、包装等说明（可选）"></textarea>
                        </label>
                    </div>
                    <div class="actions">
                        <button class="btn primary" type="button" data-quote-submit>提交报价</button>
                        <button class="btn soft" type="button" data-quote-close>取消</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <!-- 底部 -->
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>