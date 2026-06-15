<%@ Page Language="C#" AutoEventWireup="true" CodeFile="supply-detail.aspx.cs" Inherits="supply_detail" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

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
                    <h1>供应详情</h1>
                </div>
                <div class="actions">
                    <a class="btn back" href="/search.aspx" data-back>返回搜索</a>
                    <a class="btn soft" href="/merchant-workbench.aspx">商家后台</a>
                </div>
            </header>

            <section class="panel detail-hero">
                <div class="detail-header">
                    <span class="tag blue">供应</span>
                    <h2><%= Model %></h2>
                    <div class="price"><%= PriceDisplay %></div>
                </div>
                <div class="detail-meta">
                    <div class="meta-row"><span class="label">品牌</span><span class="value"><%= Brand %></span></div>
                    <div class="meta-row"><span class="label">封装</span><span class="value"><%= Package %></span></div>
                    <div class="meta-row"><span class="label">参数</span><span class="value"><%= Parameters %></span></div>
                    <div class="meta-row"><span class="label">数量</span><span class="value"><%= Quantity %></span></div>
                    <div class="meta-row"><span class="label">有效期</span><span class="value"><%= Validity %></span></div>
                </div>
            </section>

            <section class="panel detail-company">
                <h3>供应商信息</h3>
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
                    <a class="btn primary" href="#inquiryModal" data-inquiry-open>立即询价</a>
                    <a class="btn soft" href="/merchant-workbench.aspx">查看更多供应</a>
                </div>
            </section>

            <section class="panel site-ad-panel" hidden>
                <a class="search-ad-card" href="/search.aspx" data-ad-slot="DETAIL-S01">
                    <b>白银广告 DETAIL-S01</b>
                    <span>详情页底部，适合相关配件推荐和替代型号。</span>
                </a>
            </section>
        </main>
    </div>
    
    <!-- 询价弹窗 -->
    <div class="modal-backdrop" id="inquiryModal" hidden>
        <div class="modal inquiry-modal" role="dialog" aria-modal="true" aria-label="询价">
            <div class="modal-head">
                <h2>询价 <%= Model %></h2>
                <button class="modal-close" type="button" data-inquiry-close aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <form class="inquiry-form" data-inquiry-form>
                    <div class="form-row">
                        <label>询价数量 <em>*</em>
                            <input class="input" data-inquiry-qty required placeholder="填写您需要的数量">
                        </label>
                    </div>
                    <div class="form-row">
                        <label>期望价格
                            <input class="input" data-inquiry-price placeholder="填写期望单价（可选）">
                        </label>
                    </div>
                    <div class="form-row">
                        <label>备注说明
                            <textarea class="input" data-inquiry-note placeholder="交期要求、包装要求等（可选）"></textarea>
                        </label>
                    </div>
                    <div class="actions">
                        <button class="btn primary" type="button" data-inquiry-submit>提交询价</button>
                        <button class="btn soft" type="button" data-inquiry-close>取消</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <!-- 底部 -->
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>