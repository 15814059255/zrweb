<%@ Page Language="C#" AutoEventWireup="true" CodeFile="help-center.aspx.cs" Inherits="help_center" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
        <main class="main doc-page">
            <header class="topbar">
                <div><h1>使用帮助</h1><p class="lead">从注册会员、完善资料，到发布供应或需求、询价报价、管理信息和查看记录。</p></div>
                <div class="actions"><a class="btn back" href="/index.aspx" data-back>返回</a></div>
            </header>
            <section class="doc-hero panel">
                <span class="tag green">新手入口</span>
                <h2>先登录并完善资料，再发布或互动</h2>
                <p>未登录时可以浏览广场、搜索型号和查看详情。发送询价、提交报价和管理供需信息需要先登录；登录完成后需要再次点击原按钮，系统才会提交询价或报价。</p>
            </section>
            <section class="help-steps">
                <article class="panel help-step"><span>01</span><h2>注册和登录会员</h2><p>进入登录页，可选择 QQ、微信或手机验证码。勾选隐私政策后完成登录，系统会进入会员资料页。</p></article>
                <article class="panel help-step"><span>02</span><h2>完善公司资料</h2><p>在“我的”页面填写公司名称、联系人、电话、主营品牌、经营能力和公司简介。已填写资料会锁定，避免误改。</p></article>
                <article class="panel help-step"><span>03</span><h2>选择身份入口</h2><p>供应商进入“商家工作台”，采购商进入“采购工作台”。两个工作台分别管理库存和需求。</p></article>
                <article class="panel help-step"><span>04</span><h2>发布供应或需求</h2><p>商家点击“发布供应”，采购点击“发布需求”。填写型号、品牌、参数、数量、单位、单价、税赋和有效期后发布。</p></article>
            </section>
            <section class="grid cols-2">
                <div class="panel doc-card">
                    <h2>供应商怎么使用</h2>
                    <div class="doc-flow-grid single">
                        <div><b>管理库存</b><span>在商家工作台查看在线供应、库存数量、单价、税赋和有效期。到期信息会进入“已下架”区域，可重新上架。</span></div>
                        <div><b>收到询价</b><span>点击“收到询价”查看采购商需求，填写可供数量、单位、批次、报价和备注后提交报价。</span></div>
                        <div><b>报价记录</b><span>进入“我的报价”查看已提交报价，跟进采购商是否查看、是否推荐和是否过期。</span></div>
                        <div><b>快捷发布</b><span>点击“快捷发布”，粘贴多行型号和数量，系统会进入解析预览，可批量确认发布。</span></div>
                    </div>
                </div>
                <div class="panel doc-card">
                    <h2>采购商怎么使用</h2>
                    <div class="doc-flow-grid single">
                        <div><b>管理需求</b><span>在采购工作台查看在线需求、收到报价和到期数据。需求到期后可以重新上架。</span></div>
                        <div><b>发布需求</b><span>填写型号、品牌、参数、需求数量、期望单价、税赋、有效期和备注，发布后供应商可报价。</span></div>
                        <div><b>收到报价</b><span>进入“收到报价”按新报价、推荐、最新、即将过期和已过期筛选报价，选择供应商沟通。</span></div>
                        <div><b>发起询价</b><span>在广场或供应详情页点击“立即询价”，填写采购数量和期望单价后发送给供应商。</span></div>
                    </div>
                </div>
            </section>
            <section class="panel doc-card">
                <h2>从广场找到供需</h2>
                <p>广场展示供应和需求混合信息。蓝色标签代表供应，绿色标签代表需求，橙色有效期标签提示信息仍在有效期内。搜索框支持输入型号、品牌和参数，例如 `0603 100nF X7R`。</p>
                <div class="doc-flow-grid">
                    <div><b>供应卡片</b><span>显示型号、价格、品牌参数、现货数量、有效期和供应商。点击“立即询价”联系供应商。</span></div>
                    <div><b>需求卡片</b><span>显示求购型号、期望价格、需求数量、有效期和采购商。点击“我要报价”向采购商提交报价。</span></div>
                    <div><b>详情页</b><span>供应详情展示库存和供应商信息，需求详情展示采购数量和采购商信息。上架状态显示普通标签；已售罄或已采购时显示盖章。</span></div>
                    <div><b>登录弹窗</b><span>未登录时提交询价或报价会弹出登录窗。完成登录后再次点击原按钮，才会真正提交。</span></div>
                </div>
            </section>
            <section class="panel doc-card">
                <h2>信息状态说明</h2>
                <div class="doc-table-wrap"><table class="table status-doc-table"><tr><th>状态</th><th>含义</th><th>用户能做什么</th></tr><tr><td><span class="tag green">现货供应</span></td><td>供应仍在线，可被采购商询价。</td><td>采购商可点击“立即询价”。</td></tr><tr><td><span class="tag orange">急采</span></td><td>采购商希望尽快收到报价。</td><td>供应商可点击“我要报价”。</td></tr><tr><td><span class="tag gray">已下架</span></td><td>信息到期或被手动下架。</td><td>发布方可在工作台重新上架。</td></tr><tr><td><span class="sold-out-stamp">已售罄</span></td><td>供应已卖完，保留展示记录。</td><td>不能继续询价，需发布新供应。</td></tr><tr><td><span class="purchased-stamp">已采购</span></td><td>需求已完成采购，保留记录。</td><td>不能继续报价，需发布新需求。</td></tr></table></div>
            </section>
            <section class="panel doc-card">
                <h2>常见问题</h2>
                <div class="faq-grid">
                    <details><summary>为什么点击提交会弹登录窗</summary><p>询价和报价会产生交易互动记录，因此需要先登录。登录后再次点击原按钮即可提交。</p></details>
                    <details><summary>有效期到了怎么办</summary><p>到期信息会进入工作台的“已下架”区域，可勾选后重新上架，也可以修改数量、价格和税赋。</p></details>
                    <details><summary>供应和需求怎么区分</summary><p>供应通常显示“现货”数量和供应商，需求显示“需求”数量和采购商。广场也提供“找供应”和“找需求”筛选。</p></details>
                    <details><summary>报价后在哪里查看</summary><p>供应商在“我的报价”查看报价记录，采购商在“收到报价”查看供应商报价。</p></details>
                </div>
            </section>
            <section class="panel site-ad-panel workbench-ad-panel" hidden>
                <a class="search-ad-card" href="about-us.aspx" data-ad-slot="HELP-S01"><b>白银广告 HELP-S01</b><span>帮助页底部低干扰位置，适合平台公告、合作入口和服务说明。</span></a>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>