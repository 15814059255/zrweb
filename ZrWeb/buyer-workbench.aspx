<%@ Page Language="C#" AutoEventWireup="true" CodeFile="buyer-workbench.aspx.cs" Inherits="buyer_workbench" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
                <div><h1>我是采购</h1></div>
                <div class="actions"><a class="btn back" href="buyer-workbench.aspx" data-back>返回采购</a></div>
            </header>
            <div class="grid cols-3 buyer-stats" data-db-name="zr_platform" data-current-member-id="member_10031">
                <button class="stat stat-action" type="button" data-demand-stat data-db-source="demand_items" data-db-metric="COUNT(*) WHERE member_id=:current_member AND status=在线"><strong><%= OnlineDemandCount %></strong><span>在线需求</span><small>查看当前发布 ›</small></button>
                <a class="stat stat-link quote-stat-link" href="received-quotes.aspx" data-db-source="quotes" data-db-metric="COUNT(*) WHERE buyer_id=:current_member"><div><strong><%= QuoteCount %></strong><span>收到报价</span></div><em>新报价 <%= NewQuoteCount %></em><small>查看报价 ›</small></a>
                <button class="stat stat-action" type="button" data-expired-stat data-db-source="demand_items" data-db-metric="COUNT(*) WHERE member_id=:current_member AND status IN (已下架,已过期)"><strong><%= ExpiredCount %></strong><span>到期数据</span><small>查看已下架 ›</small></button>
            </div>
            <section class="panel site-ad-panel workbench-ad-panel" hidden>
                <a class="search-ad-card" href="received-quotes.html" data-ad-slot="BW-S01"><b>白银广告 BW-S01</b><span>采购工作台轻提示位，适合报价服务、会员权益和工具提醒。</span></a>
            </section>
            <section class="panel buyer-demand-panel" id="demandPanel" style="margin-top:18px"><div class="section-title"><div><h2>我的需求信息</h2></div><div class="actions"><button class="btn primary buyer-publish-btn" type="button" data-publish-open data-publish-default="demand" data-publish-lock-kind="demand">发布采购</button></div></div>
                <div class="table-wrap">
                    <table class="table inventory-table">
                        <thead><tr><th></th><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>数量</th><th>单位</th><th>期望价</th><th>税赋</th><th>剩余时间</th><th>操作</th></tr></thead>
                        <tbody>
                            <% if (!HasDemandData) { %>
                            <tr><td colspan="10" style="text-align:center;padding:40px;">
                                <div class="empty-state" style="display:inline-block;padding:30px 40px;">
                                    <div class="empty-icon">📋</div>
                                    <h3>暂无采购需求</h3>
                                    <p>发布采购需求，让供应商主动联系您</p>
                                </div>
                            </td></tr>
                            <% } else { %>
                            <asp:Repeater ID="rptDemand" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr class="inventory-item" data-goods-id="<%# Eval("goodsId") %>"><td><input type="checkbox"></td><td><span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span></td><td><strong><%# Eval("Model") %></strong></td><td><%# Eval("BrandParams") %></td><td><input class="qty-input" inputmode="numeric" pattern="[1-9][0-9]*" value="<%# Eval("Quantity") %>"></td><td><select class="unit-select"><option <%# Eval("Unit") == "Kpcs" ? "selected" : "" %>>Kpcs</option><option <%# Eval("Unit") == "Pcs" ? "selected" : "" %>>Pcs</option><option <%# Eval("Unit") == "盘" ? "selected" : "" %>>盘</option><option <%# Eval("Unit") == "卷" ? "selected" : "" %>>卷</option><option <%# Eval("Unit") == "件" ? "selected" : "" %>>件</option></select></td><td><label class="price-field <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-taxed" : "is-untaxed" %>"><input class="price-input" min="0.0001" step="0.0001" value="<%# Eval("Price") %>"><span><%# Convert.ToBoolean(Eval("IsTaxed")) ? "含税" : "未税" %></span></label></td><td><button class="tax-switch <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-on" : "" %>" type="button" data-tax-toggle aria-pressed="<%# Convert.ToBoolean(Eval("IsTaxed")) %>"><span></span></button></td><td><%# Eval("RemainingTime") %></td><td><button class="btn mini take-off" data-toggle-stock data-goods-id="<%# Eval("goodsId") %>">下架</button><button class="btn mini restock" data-toggle-stock data-goods-id="<%# Eval("goodsId") %>">重新上架</button></td></tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } %>
                        </tbody>
                    </table>
                </div>
                <div class="pagination"><div class="batch-actions"><label class="select-all"><input type="checkbox" data-select-all></label><button class="btn mini" data-ui-toast="已批量下架选中库存">批量下架</button></div><button class="btn" disabled>上一页</button><span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span><span class="page-size">每页 50 条</span><button class="btn">下一页</button></div></section>
            <section class="panel expired-panel" id="expiredPanel" hidden style="margin-top:18px">
                <div class="section-title"><div><h2>到期信息 · 已下架</h2></div><div class="actions"><button class="btn" data-toggle-expired>收起</button><button class="btn" data-ui-toast="已批量重新上架选中库存">批量重新上架</button><label class="select-all expired-select-all"><input type="checkbox" data-select-all><span>全选</span></label></div></div>
                <div class="table-wrap">
                    <table class="table inventory-table">
                        <thead><tr><th></th><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>数量</th><th>单位</th><th>单价</th><th>税赋</th><th>下架时间</th><th>操作</th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptExpiredDemand" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr class="inventory-item is-offline" data-goods-id="<%# Eval("goodsId") %>"><td><input type="checkbox"></td><td><span class="tag orange">已下架</span></td><td><strong><%# Eval("Model") %></strong></td><td><%# Eval("BrandParams") %></td><td><input class="qty-input" inputmode="numeric" pattern="[1-9][0-9]*" value="<%# Eval("Quantity") %>"></td><td><select class="unit-select"><option <%# Eval("Unit") == "Kpcs" ? "selected" : "" %>>Kpcs</option><option <%# Eval("Unit") == "Pcs" ? "selected" : "" %>>Pcs</option><option <%# Eval("Unit") == "盘" ? "selected" : "" %>>盘</option><option <%# Eval("Unit") == "卷" ? "selected" : "" %>>卷</option><option <%# Eval("Unit") == "件" ? "selected" : "" %>>件</option></select></td><td><label class="price-field <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-taxed" : "is-untaxed" %>"><input class="price-input" min="0.0001" step="0.0001" value="<%# Eval("Price") %>"><span><%# Convert.ToBoolean(Eval("IsTaxed")) ? "含税" : "未税" %></span></label></td><td><button class="tax-switch <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-on" : "" %>" type="button" data-tax-toggle aria-pressed="<%# Convert.ToBoolean(Eval("IsTaxed")) %>"><span></span></button></td><td><%# Eval("OfflineTime") %></td><td><button class="btn mini restock" data-toggle-stock data-goods-id="<%# Eval("goodsId") %>">重新上架</button></td></tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <div class="pagination"><button class="btn" disabled>上一页</button><span>第 1 / 12 页</span><button class="btn">下一页</button></div>
            </section>
        </main>
    </div>
    <div class="modal-backdrop" id="publishModal" hidden>
        <div class="modal publish-form-modal" role="dialog" aria-modal="true" aria-label="发布">
            <div class="modal-head"><h2 data-publish-title>发布</h2><button class="modal-close" type="button" data-publish-close aria-label="关闭">×</button></div>
            <div class="modal-body">
                <form class="quick-publish-form" data-quick-publish-form id="publishDemandForm">
                    <input type="hidden" name="action" value="publish_demand">
                    <div class="form-row"><div class="segmented"><button class="active" type="button" data-publish-kind="supply">发布供应</button><button type="button" data-publish-kind="demand">发布需求</button></div></div>
                    <div class="form-row"><div class="segmented"><button class="active" type="button" data-part-type="capacitor">电容</button><button type="button" data-part-type="resistor">电阻</button></div></div>
                    <div class="form-row suggest-wrap inline-row">
                        <label>型号</label>
                        <input class="input" name="goodsSn" data-model-input data-clear-on-click autocomplete="off" placeholder="输入型号，如 GRM188R71H104KA93D" onblur="validateAndFillPartNumber(this)">
                        <div class="suggest-list" data-suggest-list hidden></div>
                    </div>
                    <div id="pnr-result" style="display:none;padding:12px;background:rgba(34,197,94,0.05);border-left:3px solid #22c55e;margin-bottom:12px;"></div>
                    <div class="form-row">
                        <div class="attr-grid" data-attr-grid></div>
                    </div>
                    <div class="form-row trade-grid"><label>期望单价<span class="tax-inline"><span class="price-field is-untaxed"><input class="price-input" name="price" min="0.0001" step="0.0001" value=""><span>未税</span></span><button class="tax-switch" type="button" data-tax-toggle aria-pressed="false"><span></span></button><input type="hidden" name="isIncludingTax" value="0"></span></label><label><span data-qty-label>采购数量</span><span class="qty-unit-inline"><input class="input" name="quantity" data-required="数量" placeholder="填写数量" value=""><select class="input unit-inline-input" name="unit" data-clear-on-click><option>Kpcs</option><option>Pcs</option><option>盘</option><option>卷</option><option>件</option></select></span></label></div>
                    <div class="publish-footer"><div class="validity-picker" aria-label="有效期"><span>有效期</span><button type="button" data-validity="24小时">24小时</button><button type="button" data-validity="3天">3天</button><button type="button" data-validity="7天">7天</button><button type="button" data-validity="15天">15天</button><button class="active" type="button" data-validity="1个月">1个月</button><button type="button" data-validity="长期">长期</button></div><button class="btn primary publish-confirm" type="button" data-publish-confirm>确定</button></div>
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
                    <div class="actions" style="margin-top:16px"><a class="btn primary" href="parse-confirm.aspx" data-quick-parse>开始解析</a></div>
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
        document.addEventListener('DOMContentLoaded', function() {
            var publishModal = document.getElementById('publishModal');
            var publishForm = document.getElementById('publishDemandForm');
            var publishConfirmBtn = publishModal.querySelector('[data-publish-confirm]');
            var taxSwitch = publishModal.querySelector('[data-tax-toggle]');
            var isIncludingTaxInput = publishModal.querySelector('input[name="isIncludingTax"]');

            // 初始渲染参数输入框
            if (publishForm && typeof renderQuickPublishAttrs === 'function') {
                renderQuickPublishAttrs(publishForm);
            }

            // 类型切换时重新渲染参数输入框
            publishModal.querySelectorAll('[data-part-type]').forEach(function(btn) {
                btn.addEventListener('click', function() {
                    if (publishForm && typeof renderQuickPublishAttrs === 'function') {
                        renderQuickPublishAttrs(publishForm);
                    }
                });
            });

            // 税赋切换
            if (taxSwitch) {
                taxSwitch.addEventListener('click', function() {
                    var isOn = taxSwitch.classList.toggle('is-on');
                    taxSwitch.setAttribute('aria-pressed', isOn);
                    var priceField = taxSwitch.closest('.price-field');
                    if (isOn) {
                        priceField.classList.remove('is-untaxed');
                        priceField.classList.add('is-taxed');
                        priceField.querySelector('span').textContent = '含税';
                        if (isIncludingTaxInput) isIncludingTaxInput.value = '1';
                    } else {
                        priceField.classList.remove('is-taxed');
                        priceField.classList.add('is-untaxed');
                        priceField.querySelector('span').textContent = '未税';
                        if (isIncludingTaxInput) isIncludingTaxInput.value = '0';
                    }
                });
            }

            // 发布确认
            if (publishConfirmBtn) {
                publishConfirmBtn.addEventListener('click', function() {
                    var formData = new FormData(publishForm);
                    var goodsSn = formData.get('goodsSn');
                    var quantity = formData.get('quantity');

                    if (!goodsSn || goodsSn.trim() === '') {
                        alert('请输入型号');
                        return;
                    }
                    if (!quantity || quantity.trim() === '') {
                        alert('请输入采购数量');
                        return;
                    }

                    publishConfirmBtn.disabled = true;
                    publishConfirmBtn.textContent = '发布中...';

                    fetch('buyer-workbench.aspx', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            alert('发布成功！');
                            publishModal.hidden = true;
                            location.reload();
                        } else {
                            alert('发布失败：' + data.message);
                        }
                    })
                    .catch(error => {
                        alert('发布异常：' + error);
                    })
                    .finally(() => {
                        publishConfirmBtn.disabled = false;
                        publishConfirmBtn.textContent = '确定';
                    });
                });
            }

            // 下架按钮点击
            document.querySelectorAll('.take-off[data-goods-id]').forEach(function(btn) {
                btn.addEventListener('click', function() {
                    var goodsId = this.getAttribute('data-goods-id');
                    if (!goodsId) return;

                    if (!confirm('确定要下架此需求吗？')) return;

                    var formData = new FormData();
                    formData.append('action', 'take_off');
                    formData.append('goodsId', goodsId);

                    this.disabled = true;
                    this.textContent = '下架中...';

                    fetch('buyer-workbench.aspx', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            alert('下架成功！');
                            location.reload();
                        } else {
                            alert('下架失败：' + data.message);
                        }
                    })
                    .catch(error => {
                        alert('下架异常：' + error);
                    })
                    .finally(() => {
                        this.disabled = false;
                        this.textContent = '下架';
                    });
                });
            });

            // 重新上架按钮点击
            document.querySelectorAll('.restock[data-goods-id]').forEach(function(btn) {
                btn.addEventListener('click', function() {
                    var goodsId = this.getAttribute('data-goods-id');
                    if (!goodsId) return;

                    if (!confirm('确定要重新上架此需求吗？')) return;

                    var formData = new FormData();
                    formData.append('action', 'restock');
                    formData.append('goodsId', goodsId);

                    this.disabled = true;
                    this.textContent = '上架中...';

                    fetch('buyer-workbench.aspx', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            alert('重新上架成功！');
                            location.reload();
                        } else {
                            alert('重新上架失败：' + data.message);
                        }
                    })
                    .catch(error => {
                        alert('重新上架异常：' + error);
                    })
                    .finally(() => {
                        this.disabled = false;
                        this.textContent = '重新上架';
                    });
                });
            });
        });
    </script>
</body>
</html>