<%@ Page Language="C#" AutoEventWireup="true" CodeFile="received-inquiries.aspx.cs" Inherits="received_inquiries" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
                <div><h1>收到询价</h1></div>
                <div class="actions"><a class="btn back" href="merchant-workbench.aspx" data-back>返回商家</a></div>
            </header>
            <section class="panel">
                <div class="searchbar inventory-searchbar"><input class="input" data-inquiry-search placeholder="搜索型号、采购商"><button class="btn primary" data-inquiry-search-btn>搜索</button></div>
            </section>
            <section class="panel inquiry-panel">
                <div class="table-wrap">
                    <table class="table inquiry-table">
                        <thead><tr><th>状态</th><th>型号</th><th>品牌 / 参数</th><th>采购数量</th><th>期望单价</th><th>采购商</th><th>询价时间</th><th>操作</th></tr></thead>
                        <tbody>
                            <asp:Repeater ID="rptInquiries" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr data-eq-id="<%# Eval("EqId") %>" data-goods-id="<%# Eval("GoodsId") %>" data-buyer-name="<%# Eval("BuyerName") %>" data-model="<%# Eval("Model") %>">
                                        <td><span class="tag <%# Eval("StatusClass") %>"><%# Eval("Status") %></span></td>
                                        <td><strong><%# Eval("Model") %></strong></td>
                                        <td><%# Eval("BrandParams") %></td>
                                        <td><%# Eval("Quantity") %>&nbsp;<%# Eval("Unit") %></td>
                                        <td><%# Eval("ExpectedPrice") %></td>
                                        <td><%# Eval("BuyerName") %></td>
                                        <td><%# Eval("InquiryTime") %></td>
                                        <td><button class="btn mini" data-quote-open>报价</button></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                <div class="pagination"><button class="btn" disabled>上一页</button><span>第 <%= CurrentPage %> / <%= TotalPages %> 页</span><span class="page-size">每页 50 条</span><button class="btn">下一页</button></div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
    <div class="modal-backdrop" id="quoteModal" hidden>
        <div class="modal quote-form-modal" role="dialog" aria-modal="true" aria-label="报价">
            <div class="modal-head"><h2>报价</h2><button class="modal-close" type="button" data-quote-close aria-label="关闭">×</button></div>
            <div class="modal-body">
                <form class="quote-form" id="quoteForm">
                    <input type="hidden" name="eqId" id="quoteEqId">
                    <input type="hidden" name="goodsId" id="quoteGoodsId">
                    <div class="form-row"><label>采购商</label><input class="input" id="quoteBuyerName" readonly></div>
                    <div class="form-row"><label>型号</label><input class="input" id="quoteModel" readonly></div>
                    <div class="form-row trade-grid">
                        <label>报价单价<span class="tax-inline">
                            <span class="price-field is-untaxed">
                                <input class="price-input" name="quotePrice" id="quotePrice" min="0.0001" step="0.0001" value="">
                                <span>未税</span>
                            </span>
                            <button class="tax-switch" type="button" data-quote-tax-toggle aria-pressed="false"><span></span></button>
                            <input type="hidden" name="isIncludingTax" id="quoteIsIncludingTax" value="0">
                        </span></label>
                        <label>供货数量<span class="qty-unit-inline">
                            <input class="input" name="quoteQuantity" id="quoteQuantity" placeholder="填写供货数量" value="">
                            <select class="input unit-inline-input" name="quoteUnit">
                                <option>Kpcs</option>
                                <option>Pcs</option>
                                <option>盘</option>
                                <option>卷</option>
                                <option>件</option>
                            </select>
                        </span></label>
                    </div>
                    <div class="form-row"><label>备注</label><textarea class="input" name="quoteRemarks" id="quoteRemarks" rows="3" placeholder="填写报价备注（可选）"></textarea></div>
                    <div class="form-row"><button class="btn primary" type="button" id="quoteSubmit">提交报价</button></div>
                </form>
            </div>
        </div>
    </div>
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            var quoteModal = document.getElementById('quoteModal');
            var quoteForm = document.getElementById('quoteForm');
            var quoteSubmit = document.getElementById('quoteSubmit');
            var taxSwitch = quoteModal.querySelector('[data-quote-tax-toggle]');
            var isIncludingTaxInput = document.getElementById('quoteIsIncludingTax');

            // 报价按钮点击
            document.querySelectorAll('[data-quote-open]').forEach(function(btn) {
                btn.addEventListener('click', function() {
                    var tr = this.closest('tr');
                    var eqId = tr.getAttribute('data-eq-id');
                    var goodsId = tr.getAttribute('data-goods-id');
                    var buyerName = tr.getAttribute('data-buyer-name');
                    var model = tr.getAttribute('data-model');

                    document.getElementById('quoteEqId').value = eqId;
                    document.getElementById('quoteGoodsId').value = goodsId;
                    document.getElementById('quoteBuyerName').value = buyerName;
                    document.getElementById('quoteModel').value = model;
                    document.getElementById('quotePrice').value = '';
                    document.getElementById('quoteQuantity').value = '';
                    document.getElementById('quoteRemarks').value = '';
                    
                    quoteModal.hidden = false;
                });
            });

            // 关闭弹窗
            quoteModal.querySelector('[data-quote-close]').addEventListener('click', function() {
                quoteModal.hidden = true;
            });

            // 税赋切换
            if (taxSwitch) {
                taxSwitch.addEventListener('click', function() {
                    var isOn = taxSwitch.classList.toggle('is-on');
                    taxSwitch.setAttribute('aria-pressed', isOn);
                    var priceField = quoteModal.querySelector('.price-field');
                    if (isOn) {
                        priceField.classList.remove('is-untaxed');
                        priceField.classList.add('is-taxed');
                        priceField.querySelector('span').textContent = '含税';
                        isIncludingTaxInput.value = '1';
                    } else {
                        priceField.classList.remove('is-taxed');
                        priceField.classList.add('is-untaxed');
                        priceField.querySelector('span').textContent = '未税';
                        isIncludingTaxInput.value = '0';
                    }
                });
            }

            // 提交报价
            quoteSubmit.addEventListener('click', function() {
                var eqId = document.getElementById('quoteEqId').value;
                var price = document.getElementById('quotePrice').value;
                var quantity = document.getElementById('quoteQuantity').value;

                if (!price || price.trim() === '') {
                    alert('请输入报价金额');
                    return;
                }
                if (!quantity || quantity.trim() === '') {
                    alert('请输入供货数量');
                    return;
                }

                quoteSubmit.disabled = true;
                quoteSubmit.textContent = '提交中...';

                var formData = new FormData(quoteForm);
                formData.append('action', 'submit_quote');

                fetch('received-inquiries.aspx', {
                    method: 'POST',
                    body: formData
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert('报价成功！');
                        quoteModal.hidden = true;
                        location.reload();
                    } else {
                        alert('报价失败：' + data.message);
                    }
                })
                .catch(error => {
                    alert('报价异常：' + error);
                })
                .finally(() => {
                    quoteSubmit.disabled = false;
                    quoteSubmit.textContent = '提交报价';
                });
            });
        });
    </script>
</body>
</html>