<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PublishModal.ascx.cs" Inherits="UserControls_PublishModal" %>
<div class="modal-backdrop" id="publishModal" hidden>
    <div class="modal publish-form-modal" role="dialog" aria-modal="true" aria-label="发布">
        <div class="modal-head"><h2 data-publish-title>发布供应</h2><button class="modal-close" type="button" data-publish-close aria-label="关闭">×</button></div>
        <div class="modal-body">
            <form class="quick-publish-form" data-quick-publish-form id="commonPublishForm">
                <input type="hidden" name="action" value="publish_goods">
                <input type="hidden" name="pubType" id="pubTypeInput" value="1">
                <input type="hidden" name="isIncludingTax" id="isIncludingTaxInput" value="0">
                <div class="form-row"><div class="segmented"><button class="active" type="button" data-publish-kind="supply">发布供应</button><button type="button" data-publish-kind="demand">发布需求</button></div></div>
                <div class="form-row"><div class="segmented"><button class="active" type="button" data-part-type="capacitor">电容</button><button type="button" data-part-type="resistor">电阻</button></div></div>
                <div class="form-row suggest-wrap inline-row"><label>型号</label><input class="input" name="goodsSn" data-model-input data-clear-on-click autocomplete="off" placeholder="输入型号，如 GRM188R71H104KA93D"><div class="suggest-list" data-suggest-list hidden></div></div>
                <div class="form-row"><label>名称</label><input class="input" name="name" placeholder="商品名称（可选）"></div>
                <div class="form-row"><label>品牌/制造商</label><input class="input" name="manufacturers" placeholder="品牌或制造商（可选）"></div>
                <div class="form-row trade-grid"><label>单价<span class="tax-inline"><span class="price-field is-untaxed"><input class="price-input" name="shopPrice" min="0.0001" step="0.0001" value=""><span>未税</span></span><button class="tax-switch" type="button" data-tax-toggle aria-pressed="false"><span></span></button></span></label><label><span data-qty-label>可供数量</span><span class="qty-unit-inline"><input class="input" name="goodsStock" data-required="数量" placeholder="填写数量"><select class="input unit-inline-input" name="goodsUnit" data-clear-on-click><option>Kpcs</option><option>Pcs</option><option>盘</option><option>卷</option><option>件</option></select></span></label></div>
                <div class="publish-footer"><div class="validity-picker" aria-label="有效期"><span>有效期</span><button type="button" data-validity="24小时">24小时</button><button type="button" data-validity="3天">3天</button><button type="button" data-validity="7天">7天</button><button type="button" data-validity="15天">15天</button><button class="active" type="button" data-validity="1个月">1个月</button><button type="button" data-validity="长期">长期</button></div><button class="btn primary publish-confirm" type="button" data-publish-confirm>确定</button></div>
            </form>
        </div>
    </div>
</div>
<script>
    document.addEventListener('DOMContentLoaded', function() {
        var publishModal = document.getElementById('publishModal');
        var publishForm = document.getElementById('commonPublishForm');
        var publishConfirmBtn = publishModal.querySelector('[data-publish-confirm]');
        var publishCloseBtn = publishModal.querySelector('[data-publish-close]');
        var publishTitle = publishModal.querySelector('[data-publish-title]');
        var taxSwitch = publishModal.querySelector('[data-tax-toggle]');
        var isIncludingTaxInput = document.getElementById('isIncludingTaxInput');
        var pubTypeInput = document.getElementById('pubTypeInput');

        var qtyLabel = publishModal.querySelector('[data-qty-label]');

        document.querySelectorAll('[data-publish-open]').forEach(function(btn) {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                var defaultType = this.getAttribute('data-publish-default') || 'supply';
                openPublishModal(defaultType);
            });
        });

        function openPublishModal(type) {
            publishModal.removeAttribute('hidden');
            var kindBtns = publishModal.querySelectorAll('[data-publish-kind]');
            kindBtns.forEach(function(b) { b.classList.remove('active'); });
            kindBtns.forEach(function(b) {
                if (b.getAttribute('data-publish-kind') === type) {
                    b.classList.add('active');
                }
            });
            publishTitle.textContent = type === 'supply' ? '发布供应' : '发布需求';
            if (qtyLabel) {
                qtyLabel.textContent = type === 'supply' ? '可供数量' : '采购数量';
            }
            if (pubTypeInput) {
                pubTypeInput.value = type === 'supply' ? '1' : '2';
            }
        }

        if (publishCloseBtn) {
            publishCloseBtn.addEventListener('click', function() {
                publishModal.setAttribute('hidden', '');
            });
        }

        publishModal.querySelectorAll('[data-publish-kind]').forEach(function(btn) {
            btn.addEventListener('click', function() {
                var kindBtns = publishModal.querySelectorAll('[data-publish-kind]');
                kindBtns.forEach(function(b) { b.classList.remove('active'); });
                this.classList.add('active');
                var kind = this.getAttribute('data-publish-kind');
                publishTitle.textContent = kind === 'supply' ? '发布供应' : '发布需求';
                if (qtyLabel) {
                    qtyLabel.textContent = kind === 'supply' ? '可供数量' : '采购数量';
                }
                if (pubTypeInput) {
                    pubTypeInput.value = kind === 'supply' ? '1' : '2';
                }
            });
        });

        if (taxSwitch) {
            taxSwitch.addEventListener('click', function() {
                var isOn = taxSwitch.classList.toggle('is-on');
                taxSwitch.setAttribute('aria-pressed', isOn);
                var priceField = taxSwitch.previousElementSibling;
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

        if (publishConfirmBtn) {
            publishConfirmBtn.addEventListener('click', function() {
                var formData = new FormData(publishForm);
                var goodsSn = formData.get('goodsSn');
                var goodsStock = formData.get('goodsStock');
                var shopPrice = formData.get('shopPrice');

                if (!goodsSn || goodsSn.trim() === '') {
                    alert('请输入型号');
                    return;
                }
                if (!goodsStock || goodsStock.trim() === '') {
                    alert('请输入数量');
                    return;
                }
                if (!shopPrice || shopPrice.trim() === '') {
                    alert('请输入单价');
                    return;
                }

                var activeKind = publishModal.querySelector('[data-publish-kind].active');
                var pubType = activeKind.getAttribute('data-publish-kind') === 'supply' ? '1' : '2';
                formData.set('pubType', pubType);
                formData.set('action', pubType === '1' ? 'publish_goods' : 'publish_demand');

                var isLoggedIn = window.ZR_CURRENT_MEMBER && window.ZR_CURRENT_MEMBER.userId;
                if (!isLoggedIn) {
                    var currentPage = window.location.pathname.split('/').pop();
                    var storageKey = currentPage === 'merchant-workbench.aspx' ? 'merchant' : 
                                      currentPage === 'buyer-workbench.aspx' ? 'buyer' : 'index';
                    savePublishData(storageKey, formData);
                    showLoginModal();
                    return;
                }

                submitPublish(formData);
            });
        }

        function submitPublish(formData) {
            publishConfirmBtn.disabled = true;
            publishConfirmBtn.textContent = '发布中...';

            fetch(window.location.pathname, {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert('发布成功！');
                    publishModal.setAttribute('hidden', '');
                    publishForm.reset();
                } else {
                    if (data.message && (data.message.indexOf('请先登录') !== -1 || data.message.indexOf('无法获取店铺信息') !== -1 || data.message.indexOf('请完善店铺资料') !== -1)) {
                        var currentPage = window.location.pathname.split('/').pop();
                        var storageKey = currentPage === 'merchant-workbench.aspx' ? 'merchant' : 
                                          currentPage === 'buyer-workbench.aspx' ? 'buyer' : 'index';
                        savePublishData(storageKey, formData);
                        showLoginModal();
                    } else {
                        alert('发布失败：' + data.message);
                    }
                }
            })
            .catch(error => {
                alert('发布异常：' + error);
            })
            .finally(() => {
                publishConfirmBtn.disabled = false;
                publishConfirmBtn.textContent = '确定';
            });
        }

        function savePublishData(type, formData) {
            var data = {};
            formData.forEach(function(value, key) {
                data[key] = value;
            });
            sessionStorage.setItem('pendingPublish_' + type, JSON.stringify(data));
        }
    });
</script>