<%@ Page Language="C#" AutoEventWireup="true" CodeFile="index.aspx.cs" Inherits="index" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

<%@ Register Src="~/UserControls/head.ascx" TagPrefix="uc1" TagName="head" %>
<%@ Register Src="~/UserControls/bottom.ascx" TagPrefix="uc1" TagName="bottom" %>
<%@ Register Src="~/UserControls/PublishModal.ascx" TagPrefix="uc1" TagName="PublishModal" %>
<%@ Register Src="~/UserControls/LoginModal.ascx" TagPrefix="uc1" TagName="LoginModal" %>

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
            <section class="panel">
                <div class="searchbar market-toolbar">
                    <input class="input" placeholder="搜索型号、品牌、参数，如 0603 100nF X7R" id="searchInput">
                    <div class="actions toolbar-actions">
                        <a class="btn primary" href="/search.aspx">搜索</a>
                        <button class="btn soft" type="button" data-publish-open data-publish-default="supply">发布供采</button>
                    </div>
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
                            <div class="item" data-type="<%# Eval("ItemType") %>">
                                <div>
                                    <h3><span class="tag <%# Eval("TagClass") %>"><%# Eval("TypeLabel") %></span><a href="<%# Eval("DetailUrl") %>"><%# Eval("Model") %></a></h3>
                                    <div class="price"><%# Eval("PriceDisplay") %></div>
                                    <div class="meta"><span><%# Eval("BrandParams") %></span><span><%# Eval("QuantityDisplay") %></span><span class="validity-line">有效期 <%# Eval("Validity") %></span></div>
                                </div>
                                <div class="item-footer"><div class="company"><%# Eval("CompanyName") %></div><a class="btn soft mini" data-action="<%# Eval("ActionText") %>" data-goods-id="<%# Eval("GoodsId") %>" data-goods-sn="<%# Eval("GoodsSn") %>" data-shop-id="<%# Eval("ShopId") %>" href="#tradeInteractionModal"><%# Eval("ActionText") %></a></div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </section>
        </main>
    </div>
    <uc1:PublishModal runat="server" ID="PublishModal" />
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
    <uc1:LoginModal runat="server" ID="LoginModal" />
    
    <div class="modal-backdrop" id="tradeInteractionModal" hidden>
        <div class="modal trade-modal" role="dialog" aria-modal="true" aria-label="交易交互">
            <div class="modal-head">
                <h2 id="tradeModalTitle">交易交互</h2>
                <button class="modal-close" type="button" data-trade-close aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <form id="tradeForm">
                    <input type="hidden" name="action" id="tradeAction">
                    <input type="hidden" name="goodsId" id="tradeGoodsId">
                    <input type="hidden" name="goodsSn" id="tradeGoodsSn">
                    <input type="hidden" name="toShopId" id="tradeToShopId">
                    
                    <div class="form-row">
                        <label>型号</label>
                        <input class="input" id="tradeModel" readonly>
                    </div>
                    
                    <div class="form-row trade-grid">
                        <label>数量
                            <input class="input" name="quantity" data-required="数量" placeholder="填写数量">
                        </label>
                        <label>单价
                            <span class="price-field is-untaxed">
                                <input class="price-input" name="price" min="0.0001" step="0.0001" value="">
                                <span>未税</span>
                            </span>
                            <button class="tax-switch" type="button" data-trade-tax-toggle aria-pressed="false"><span></span></button>
                            <input type="hidden" name="isIncludingTax" value="0">
                        </label>
                    </div>
                    
                    <div class="form-row">
                        <label>备注</label>
                        <textarea name="remarks" placeholder="输入备注信息（可选）"></textarea>
                    </div>
                    
                    <div class="form-row">
                        <button class="btn primary" type="button" id="tradeSubmit">提交</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function() {
            var tradeModal = document.getElementById('tradeInteractionModal');
            var tradeForm = document.getElementById('tradeForm');
            var tradeSubmitBtn = document.getElementById('tradeSubmit');
            var tradeTaxSwitch = tradeModal.querySelector('[data-trade-tax-toggle]');
            var tradeIsIncludingTaxInput = tradeModal.querySelector('input[name="isIncludingTax"]');

            // 税赋切换
            if (tradeTaxSwitch) {
                tradeTaxSwitch.addEventListener('click', function() {
                    var isOn = tradeTaxSwitch.classList.toggle('is-on');
                    tradeTaxSwitch.setAttribute('aria-pressed', isOn);
                    var priceField = tradeTaxSwitch.previousElementSibling;
                    if (isOn) {
                        priceField.classList.remove('is-untaxed');
                        priceField.classList.add('is-taxed');
                        priceField.querySelector('span').textContent = '含税';
                        if (tradeIsIncludingTaxInput) tradeIsIncludingTaxInput.value = '1';
                    } else {
                        priceField.classList.remove('is-taxed');
                        priceField.classList.add('is-untaxed');
                        priceField.querySelector('span').textContent = '未税';
                        if (tradeIsIncludingTaxInput) tradeIsIncludingTaxInput.value = '0';
                    }
                });
            }

            // 点击"我要报价"或"立即询价"按钮
            document.querySelectorAll('[data-action]').forEach(function(btn) {
                btn.addEventListener('click', function(e) {
                    e.preventDefault();
                    var actionText = this.getAttribute('data-action');
                    var goodsId = this.getAttribute('data-goods-id');
                    var goodsSn = this.getAttribute('data-goods-sn');
                    var shopId = this.getAttribute('data-shop-id');
                    
                    // 设置表单数据
                    document.getElementById('tradeGoodsId').value = goodsId || '';
                    document.getElementById('tradeGoodsSn').value = goodsSn || '';
                    document.getElementById('tradeToShopId').value = shopId || '';
                    document.getElementById('tradeModel').value = goodsSn || '';
                    
                    // 根据按钮文字判断是询价还是报价
                    if (actionText === '我要报价') {
                        document.getElementById('tradeModalTitle').textContent = '我要报价';
                        document.getElementById('tradeAction').value = 'submit_quote';
                    } else {
                        document.getElementById('tradeModalTitle').textContent = '立即询价';
                        document.getElementById('tradeAction').value = 'submit_inquiry';
                    }
                    
                    tradeModal.removeAttribute('hidden');
                });
            });

            // 关闭模态框
            document.querySelector('[data-trade-close]').addEventListener('click', function() {
                tradeModal.setAttribute('hidden', '');
            });

            // 提交表单
            if (tradeSubmitBtn) {
                tradeSubmitBtn.addEventListener('click', function() {
                    var formData = new FormData(tradeForm);
                    var quantity = formData.get('quantity');
                    var price = formData.get('price');

                    if (!quantity || quantity.trim() === '') {
                        alert('请输入数量');
                        return;
                    }
                    if (!price || price.trim() === '') {
                        alert('请输入单价');
                        return;
                    }

                    tradeSubmitBtn.disabled = true;
                    tradeSubmitBtn.textContent = '提交中...';

                    fetch('index.aspx', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            alert(data.message);
                            tradeModal.setAttribute('hidden', '');
                            tradeForm.reset();
                        } else {
                            alert('提交失败：' + data.message);
                        }
                    })
                    .catch(error => {
                        alert('提交异常：' + error);
                    })
                    .finally(() => {
                        tradeSubmitBtn.disabled = false;
                        tradeSubmitBtn.textContent = '提交';
                    });
                });
            }
        });
    </script>
</body>
</html>