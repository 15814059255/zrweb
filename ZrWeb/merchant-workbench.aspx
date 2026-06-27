<%@ Page Language="C#" AutoEventWireup="true" CodeFile="merchant-workbench.aspx.cs" Inherits="merchant_workbench" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
        .confirm-btn.is-hidden { display: none !important; }
        .bom-tag {
            display: inline-block;
            font-size: 10px;
            color: #fff;
            background: #10b981;
            padding: 1px 4px;
            border-radius: 3px;
            margin-left: 4px;
            font-weight: normal;
            vertical-align: top;
        }
    </style>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div>
                    <h1>我是商家</h1>
                    <div class="grid cols-4 merchant-stats" style="margin:18px 0" data-merchant-stats-menu>
                        <div class="stat" data-merchant-stat="online"><strong><%= OnlineSupplyCount %></strong><span>在线供应</span></div>
                        <a class="stat stat-link inquiry-stat-link" href="received-inquiries.aspx" data-merchant-stat="inquiries"><div><strong><%= InquiryCount %></strong><span>收到询价</span></div><em data-db-source="inquiries" data-db-metric="COUNT(*) WHERE seller_id=:current_member AND read_at IS NULL">新询价 <%= NewInquiryCount %></em><small>查看询价 ›</small></a>
                        <a class="stat stat-link quote-record-stat-link" href="quote-records.aspx" data-merchant-stat="quotes"><div><strong><%= QuoteCount %></strong><span>我的报价</span></div><small>查看记录 ›</small></a>
                        <button class="stat stat-action" type="button" data-expired-stat data-merchant-stat="expired"><strong><%= ExpiredCount %></strong><span>到期数据</span><small>查看已下架 ›</small></button>
                    </div>
                </div>
                <div class="actions"><a class="btn back" href="merchant-workbench.aspx" data-back>返回商家</a>
                    <button class="btn soft" type="button" data-quick-import-open>快捷发布</button>
                </div>
            </header>
            <section class="panel">
                <form method="get" action="merchant-workbench.aspx" class="searchbar inventory-searchbar">
                    <input class="input" name="keyword" placeholder="搜索型号或参数" value="<%= SearchKeyword %>">
                    <div style="display:flex;gap:8px;align-items:center;">
                        <button class="btn primary" type="submit">搜索</button>
                        <% if (!string.IsNullOrEmpty(SearchKeyword)) { %>
                            <a class="btn" href="merchant-workbench.aspx">清除</a>
                        <% } %>
                    </div>
                </form>
            </section>
            
            <section class="panel" style="margin-top:18px"><div class="section-title"><div><h2>库存管理</h2></div><div class="actions"><button class="btn primary merchant-publish-fab" type="button" data-publish-open data-publish-default="supply" data-publish-part-default="capacitor">发布供采</button></div></div>
                <div class="table-wrap">
                    <table class="table inventory-table">
                        <thead><tr><th></th><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>数量</th><th>单位</th><th>单价</th><th>税赋</th><th>剩余时间</th><th>操作</th></tr></thead>
                        <tbody>
                            <% if (!HasInventoryData) { %>
                            <tr><td colspan="10" style="text-align:center;padding:40px;">
                                <div class="empty-state" style="display:inline-block;padding:30px 40px;">
                                    <div class="empty-icon">📦</div>
                                    <h3>暂无库存</h3>
                                    <p>发布库存供应，让采购商找到您</p>
                                </div>
                            </td></tr>
                            <% } else { %>
                            <asp:Repeater ID="rptInventory" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr class="inventory-item" data-goods-id="<%# Eval("GoodsId") %>"><td><input type="checkbox"></td><td><span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span></td><td><strong><%# Eval("Model") %><%# Convert.ToInt32(Eval("PubSource")) == 1 ? "<span class=\"bom-tag\">BOM</span>" : "" %></strong></td><td><%# Eval("BrandParams") %></td><td><input class="qty-input" inputmode="numeric" pattern="[1-9][0-9]*" value="<%# Eval("Quantity") %>"></td><td><select class="unit-select"><option <%# Eval("Unit") == "Kpcs" ? "selected" : "" %>>Kpcs</option><option <%# Eval("Unit") == "Pcs" ? "selected" : "" %>>Pcs</option><option <%# Eval("Unit") == "盘" ? "selected" : "" %>>盘</option><option <%# Eval("Unit") == "卷" ? "selected" : "" %>>卷</option><option <%# Eval("Unit") == "件" ? "selected" : "" %>>件</option></select></td><td><label class="price-field <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-taxed" : "is-untaxed" %>"><input class="price-input" min="0.0001" step="0.0001" value="<%# Eval("Price") %>"><span><%# Convert.ToBoolean(Eval("IsTaxed")) ? "含税" : "未税" %></span></label></td><td><button class="tax-switch <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-on" : "" %>" type="button" data-tax-toggle aria-pressed="<%# Convert.ToBoolean(Eval("IsTaxed")) %>"><span></span></button></td><td><%# Eval("RemainingTime") %></td><td><button class="btn mini primary confirm-btn is-hidden" onclick="saveSupplyChange(<%# Eval("GoodsId") %>, this)">确定</button><button class="btn mini take-off" data-toggle-stock>下架</button></td></tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } %>
                        </tbody>
                    </table>
                </div>
                <div class="pagination" data-inventory-pagination data-page-size="25"><div class="batch-actions"><label class="select-all"><input type="checkbox" data-select-all></label><button class="btn mini" data-batch-takeoff>批量下架</button></div><button class="btn" disabled data-page-prev>上一页</button><span>第 <span data-page-current><%= CurrentPage %></span> / <span data-page-total><%= TotalPages %></span> 页</span><span class="page-size">每页 25 条</span><button class="btn" data-page-next>下一页</button></div></section>
            <section class="panel expired-panel" id="expiredPanel" hidden style="margin-top:18px">
                <div class="section-title"><div><h2>到期信息 · 已下架商品</h2></div><div class="actions"><button class="btn" data-toggle-expired>收起</button><button class="btn" data-batch-restock>批量重新上架</button><label class="select-all expired-select-all"><input type="checkbox" data-select-all><span>全选</span></label></div></div>
                <div class="table-wrap">
                    <table class="table inventory-table">
                        <thead><tr><th></th><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>数量</th><th>单位</th><th>单价</th><th>税赋</th><th>下架时间</th><th>操作</th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptExpiredInventory" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr class="inventory-item is-offline" data-goods-id="<%# Eval("GoodsId") %>"><td><input type="checkbox"></td><td><span class="tag orange">已下架</span></td><td><strong><%# Eval("Model") %><%# Convert.ToInt32(Eval("PubSource")) == 1 ? "<span class=\"bom-tag\">BOM</span>" : "" %></strong></td><td><%# Eval("BrandParams") %></td><td><input class="qty-input" inputmode="numeric" pattern="[1-9][0-9]*" value="<%# Eval("Quantity") %>"></td><td><select class="unit-select"><option <%# Eval("Unit") == "Kpcs" ? "selected" : "" %>>Kpcs</option><option <%# Eval("Unit") == "Pcs" ? "selected" : "" %>>Pcs</option><option <%# Eval("Unit") == "盘" ? "selected" : "" %>>盘</option><option <%# Eval("Unit") == "卷" ? "selected" : "" %>>卷</option><option <%# Eval("Unit") == "件" ? "selected" : "" %>>件</option></select></td><td><label class="price-field <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-taxed" : "is-untaxed" %>"><input class="price-input" min="0.0001" step="0.0001" value="<%# Eval("Price") %>"><span><%# Convert.ToBoolean(Eval("IsTaxed")) ? "含税" : "未税" %></span></label></td><td><button class="tax-switch <%# Convert.ToBoolean(Eval("IsTaxed")) ? "is-on" : "" %>" type="button" data-tax-toggle aria-pressed="<%# Convert.ToBoolean(Eval("IsTaxed")) %>"><span></span></button></td><td><%# Eval("OfflineTime") %></td><td><button class="btn mini restock" data-toggle-stock>重新上架</button></td></tr>
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
                <form class="quick-publish-form" data-quick-publish-form id="merchantPublishForm">
                    <input type="hidden" name="action" value="publish_goods">
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
                    <div class="form-row trade-grid"><label>单价<span class="tax-inline"><span class="price-field is-untaxed"><input class="price-input" name="shopPrice" min="0.0001" step="0.0001" value=""><span>未税</span></span><button class="tax-switch" type="button" data-tax-toggle aria-pressed="false"><span></span></button><input type="hidden" name="isIncludingTax" value="0"></span></label><label><span data-qty-label>可供数量</span><span class="qty-unit-inline"><input class="input" name="goodsStock" data-required="数量" placeholder="填写数量"><select class="input unit-inline-input" name="goodsUnit" data-clear-on-click><option>Kpcs</option><option>Pcs</option><option>盘</option><option>卷</option><option>件</option></select></span></label></div>
                    <input type="hidden" name="pubType" id="pubTypeInput" value="1">
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
        // 全局函数：保存供应修改（数量/单位/单价/税赋），不影响历史交互记录
        function saveSupplyChange(goodsId, btn) {
            if (!goodsId) return;
            var row = btn.closest('tr');
            if (!row) return;

            var quantity = row.querySelector('.qty-input')?.value || '';
            var unit = row.querySelector('.unit-select')?.value || 'Kpcs';
            var price = row.querySelector('.price-input')?.value || '';
            var taxSwitch = row.querySelector('[data-tax-toggle]');
            var isIncludingTax = taxSwitch?.classList.contains('is-on') ? 1 : 0;

            if (!quantity || quantity.trim() === '') {
                Toast.warning('请输入数量');
                return;
            }

            var formData = new FormData();
            formData.append('action', 'update_supply');
            formData.append('goodsId', goodsId);
            formData.append('quantity', quantity);
            formData.append('unit', unit);
            formData.append('price', price);
            formData.append('isIncludingTax', isIncludingTax);

            btn.disabled = true;
            btn.textContent = '提交中...';

            fetch('merchant-workbench.aspx', {
                method: 'POST',
                body: formData
            })
            .then(function(response) { return response.json(); })
            .then(function(data) {
                if (data.success) {
                    Toast.success('修改成功！');
                    btn.classList.add('is-hidden');
                } else {
                    Toast.error('修改失败：' + data.message);
                }
            })
            .catch(function(error) {
                Toast.error('提交异常：' + error);
            })
            .finally(function() {
                btn.disabled = false;
                btn.textContent = '确定';
            });
        }

        document.addEventListener('DOMContentLoaded', function() {
            try {
                // 监听库存列表输入变化，触发时显示确定按钮
                var inventoryPanel = document.querySelector('.inventory-table');
                if (inventoryPanel) {
                    inventoryPanel.querySelectorAll('tbody tr.inventory-item').forEach(function(row) {
                        var qtyInput = row.querySelector('.qty-input');
                        var unitSelect = row.querySelector('.unit-select');
                        var priceInput = row.querySelector('.price-input');
                        var taxSwitch = row.querySelector('[data-tax-toggle]');
                        var confirmBtn = row.querySelector('.confirm-btn');

                        function showConfirm() {
                            if (confirmBtn) confirmBtn.classList.remove('is-hidden');
                        }

                        if (qtyInput) qtyInput.addEventListener('input', showConfirm);
                        if (unitSelect) unitSelect.addEventListener('change', showConfirm);
                        if (priceInput) priceInput.addEventListener('input', showConfirm);
                        if (taxSwitch) taxSwitch.addEventListener('click', showConfirm);
                    });
                }
            } catch(e) {
                console.error('merchant-workbench init error:', e);
            }

            var publishModal = document.getElementById('publishModal');
            var publishForm = document.getElementById('merchantPublishForm');
            var pubTypeInput = document.getElementById('pubTypeInput');

            // 初始渲染参数输入框
            if (publishForm) renderQuickPublishAttrs(publishForm);

            // 类型切换按钮（电容/电阻）
            var partTypeBtns = publishModal.querySelectorAll('[data-part-type]');
            partTypeBtns.forEach(function(btn) {
                btn.addEventListener('click', function() {
                    partTypeBtns.forEach(function(b) { b.classList.remove('active'); });
                    this.classList.add('active');
                    // 重新渲染参数输入框
                    if (publishForm) renderQuickPublishAttrs(publishForm);
                });
            });

            var kindBtns = publishModal.querySelectorAll('[data-publish-kind]');
            kindBtns.forEach(function(btn) {
                btn.addEventListener('click', function() {
                    kindBtns.forEach(function(b) { b.classList.remove('active'); });
                    this.classList.add('active');
                    var kind = this.getAttribute('data-publish-kind');
                    if (pubTypeInput) {
                        pubTypeInput.value = kind === 'supply' ? '1' : '2';
                    }
                });
            });

        });
    </script>
</body>
</html>