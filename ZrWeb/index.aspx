<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

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
            <section class="panel search-filter-panel">
                <div class="form-grid search-filter-grid">
                    <div class="search-keyword-field">
                        <label>搜索</label>
                        <input class="input" id="indexSearchInput" placeholder="输入型号、品牌、参数，如 GRM188 / 0603 / 100nF">
                    </div>
                </div>
                <div class="actions search-filter-actions" style="margin-top:16px">
                    <button class="btn primary" type="button" onclick="doIndexSearch()">查询</button>
                </div>
            </section>
            <section class="panel site-ad-panel diamond-ad-panel" hidden>
                <div class="search-ad-grid">
                    <a class="search-ad-card" href="/search.aspx" data-ad-slot="HOME-D01"><b>钻石广告 HOME-D01</b><span>首页搜索下方，适合全站重点品牌和现货活动。</span></a>
                </div>
            </section>
            <section class="panel feed-panel">
                <div class="section-title feed-section-title">
                    <div class="feed-section-actions">
                        <div class="tabs" data-feed-tabs>
                            <span class="tab active" data-tab data-filter="all">全部</span>
                            <span class="tab" data-tab data-filter="supply">找供应</span>
                            <span class="tab" data-tab data-filter="demand">找需求</span>
                        </div>
                    </div>
                </div>
                <div class="card-list mixed-feed" data-feed-list>
                    <div class="site-ad-panel inline-feed-ad" hidden>
                        <a class="search-ad-card" href="/search.aspx" data-ad-slot="HOME-G01"><b>黄金广告 HOME-G01</b><span>信息流中段，适合轻量文字推广，不打断供需浏览。</span></a>
                    </div>
                    <% if (!HasData) { %>
                    <div class="empty-state" id="emptyState">
                        <div class="empty-icon">📦</div>
                        <h3>暂无供需信息</h3>
                        <p>当前没有供应或需求信息，</p>
                        <p>成为第一个发布者，开启交易之旅！</p>
                        <a class="btn primary" href="#" data-publish-open data-publish-default="supply">发布供应</a>
                        <span style="margin: 0 10px; color: #999;">或</span>
                        <a class="btn soft" href="#" data-publish-open data-publish-default="demand">发布需求</a>
                    </div>
                    <% } %>
                    <asp:Repeater ID="rptSupplyList" runat="server" EnableViewState="false">
                        <ItemTemplate>
                            <div class="item" data-type="<%# Eval("ItemType") %>" data-shop-id="<%# Eval("ShopId") %>">
                                <div>
                                    <h3><span class="tag <%# Eval("TagClass") %>"><%# Eval("TypeLabel") %></span><a href="<%# Eval("DetailUrl") %>"><%# Eval("Model") %></a></h3>
                                    <div class="price"><%# Eval("PriceDisplay") %></div>
                                    <div class="meta"><span><%# Eval("BrandParams") %></span><span><%# Eval("QuantityDisplay") %></span><span class="validity-line">有效期 <%# Eval("Validity") %></span></div>
                                </div>
                                <div class="item-footer"><div class="company"><%# Eval("CompanyName") %></div><a class="btn soft mini" data-action="<%# Eval("ActionText") %>" data-goods-id="<%# Eval("GoodsId") %>" data-goods-sn="<%# Eval("GoodsSn") %>" href="#tradeInteractionModal"><%# Eval("ActionText") %></a></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div class="pagination"><button class="btn" type="button" data-page-prev<% if (CurrentPage <= 1) { %> disabled<% } %>>上一页</button><span data-page-info>第 <%= CurrentPage %> / <%= TotalPages %> 页</span><span class="page-size">每页 45 条</span><button class="btn" type="button" data-page-next<% if (CurrentPage >= TotalPages) { %> disabled<% } %>>下一页</button></div>
            </section>
        </main>
    </div>
    <div class="modal-backdrop" id="publishModal" hidden>
        <div class="modal publish-form-modal" role="dialog" aria-modal="true" aria-label="发布">
            <div class="modal-head"><h2 data-publish-title>发布供应</h2><button class="modal-close" type="button" data-publish-close aria-label="关闭">×</button></div>
            <div class="modal-body">
                <form class="quick-publish-form" data-quick-publish-form>
                    <div class="form-row"><div class="segmented"><button class="active" type="button" data-publish-kind="supply">发布供应</button><button type="button" data-publish-kind="demand">发布需求</button></div></div>
                    <div class="form-row"><div class="segmented"><button class="active" type="button" data-part-type="capacitor">电容</button><button type="button" data-part-type="resistor">电阻</button></div></div>
                    <div class="form-row suggest-wrap inline-row"><label>型号</label><input class="input" data-model-input data-clear-on-click autocomplete="off" placeholder="输入型号，如 GRM188R71H104KA93D"><div class="suggest-list" data-suggest-list hidden></div></div>
                    <div class="form-row"><div class="attr-grid" data-attr-grid></div></div>
                    <div class="form-row trade-grid"><label>单价<span class="tax-inline"><span class="price-field is-untaxed"><input class="price-input" min="0.0001" step="0.0001" value=""><span>未税</span></span><button class="tax-switch" type="button" data-tax-toggle aria-pressed="false"><span></span></button></span></label><label><span data-qty-label>可供数量</span><span class="qty-unit-inline"><input class="input" data-required="数量" placeholder="填写数量"><select class="input unit-inline-input" data-clear-on-click><option>Kpcs</option><option>Pcs</option><option>盘</option><option>卷</option><option>件</option></select></span></label></div>
                    <div class="publish-footer"><div class="validity-picker" aria-label="有效期"><span>有效期</span><button type="button" data-validity="1天">1天</button><button type="button" data-validity="3天">3天</button><button type="button" data-validity="7天">7天</button><button type="button" data-validity="15天">15天</button><button class="active" type="button" data-validity="30天">30天</button><button type="button" data-validity="长期">长期</button></div><button class="btn primary publish-confirm" type="button" data-publish-confirm>确定</button></div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop" id="quickImportModal" hidden>
        <div class="modal quick-import-modal" role="dialog" aria-modal="true" aria-label="快捷发布">
            <div class="modal-head"><h2>快捷发布 <small data-quick-import-count></small></h2><button class="modal-close" type="button" data-quick-import-close aria-label="关闭">×</button></div>
            <div class="modal-body">
                <section class="panel quick-paste-panel" data-quick-paste-panel>
                    <div class="section-title"><div><h2>批量粘贴</h2><p>一行一条 料号+数量(发布后有效期为3天、下架后可以重新上架)</p></div></div>
                    <textarea data-quick-paste-text></textarea>
                    <div class="actions" style="margin-top:16px"><a class="btn primary" href="/parse-confirm.aspx" data-quick-parse>开始解析</a></div>
                </section>
                <section class="panel quick-preview-panel" data-quick-preview-panel hidden>
                    <div class="table-wrap">
                        <table class="table inventory-table quick-preview-table">
                            <thead><tr><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>数量</th><th>单位</th><th class="price-tax-head">单价 <button class="tax-switch is-on" type="button" data-quick-tax-master aria-pressed="true"><span></span></button></th></tr></thead>
                            <tbody data-quick-preview-body></tbody>
                        </table>
                    </div>
                    <div class="quick-preview-footer"><button class="btn primary" type="button" data-quick-confirm-release>确定</button><div class="quick-type-toggle"><button class="active" type="button" data-quick-kind="supply">发布供应</button><button type="button" data-quick-kind="demand">发布需求</button></div><div class="pagination"><button class="btn" type="button" data-quick-page-prev>上一页</button><span data-quick-page-info>第 1 / 1 页</span><span class="page-size">每页 30 条</span><button class="btn" type="button" data-quick-page-next>下一页</button></div></div>
                </section>
            </div>
        </div>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
    
    <script>
        function doIndexSearch() {
            var input = document.getElementById('indexSearchInput');
            var query = input ? input.value.trim() : '';
            var target = query ? 'search.aspx?q=' + encodeURIComponent(query) : 'search.aspx';
            window.location.href = target;
        }
        document.getElementById('indexSearchInput')?.addEventListener('keydown', function(e) {
            if (e.key === 'Enter') doIndexSearch();
        });

        // 首页分页功能
        (function() {
            var currentPage = <%= CurrentPage %>;
            var totalPages = <%= TotalPages %>;
            
            document.querySelector('[data-page-prev]')?.addEventListener('click', function() {
                if (currentPage > 1) {
                    window.location.href = '?page=' + (currentPage - 1);
                }
            });
            
            document.querySelector('[data-page-next]')?.addEventListener('click', function() {
                if (currentPage < totalPages) {
                    window.location.href = '?page=' + (currentPage + 1);
                }
            });
        })();

        // 发布弹窗初始化
        document.addEventListener('DOMContentLoaded', function() {
            var publishModal = document.getElementById('publishModal');
            if (publishModal) {
                var publishForm = publishModal.querySelector('[data-quick-publish-form]');
                
                // 型号输入框添加料号验证
                var modelInput = publishModal.querySelector('[data-model-input]');
                if (modelInput) {
                    modelInput.setAttribute('onblur', 'validateAndFillPartNumber(this)');
                }
                
                // 添加验证结果显示区域
                var formRow = modelInput ? modelInput.closest('.form-row') : null;
                if (formRow && !publishModal.querySelector('#pnr-result')) {
                    var pnrResult = document.createElement('div');
                    pnrResult.id = 'pnr-result';
                    pnrResult.style.cssText = 'display:none;padding:12px;background:rgba(34,197,94,0.05);border-left:3px solid #22c55e;margin-bottom:12px;';
                    formRow.insertAdjacentElement('afterend', pnrResult);
                }
            }
        });
    </script>
</body>
</html>