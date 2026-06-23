<%@ Page Language="C#" AutoEventWireup="true" CodeFile="about-us.aspx.cs" Inherits="about_us" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>

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
        <!-- 头部侧边栏2 -->
        <uc1:head runat="server" ID="head" />
        
        <main class="main doc-page">
            <header class="topbar">
                <div>
                    <h1>关于我们</h1>
                    <p class="lead">阻容网 <%= SiteDomain %> 面向阻容元件供需双方，提供供应、需求、询价、报价和管理闭环。</p>
                </div>
                <div class="actions">
                    <a class="btn back" href="/index.aspx" data-back>返回广场</a>
                    <a class="btn soft" href="/help-center.aspx">网站帮助</a>
                </div>
            </header>

            <section class="doc-hero panel">
                <span class="tag green">平台定位</span>
                <h2>专注阻容元件供需信息撮合</h2>
                <p>阻容网聚焦电阻、电容等被动元件现货供应、采购需求、询报价与信息管理。平台帮助供应商更快展示库存，也帮助采购商更快找到可报价的供应资源。</p>
            </section>

            <section class="grid cols-2">
                <div class="panel doc-card">
                    <h2>我们服务谁</h2>
                    <div class="doc-flow-grid single">
                        <div><b>供应商</b><span>发布库存、维护价格和有效期，接收采购商询价并提交报价。</span></div>
                        <div><b>采购商</b><span>发布需求、搜索型号、发起询价，统一查看供应商报价。</span></div>
                        <div><b>平台管理员</b><span>审核认证、管理发布信息、处理投诉、维护风控规则和白名单。</span></div>
                    </div>
                </div>
                <div class="panel doc-card">
                    <h2>平台能力</h2>
                    <div class="doc-flow-grid single">
                        <div><b>供需广场</b><span>集中展示供应和需求信息，支持按供应、需求切换查看。</span></div>
                        <div><b>交易互动</b><span>供应详情可询价，需求详情可报价，形成双方互动记录。</span></div>
                        <div><b>会员后台</b><span>供应商、采购商分别管理发布数据、收到询价、收到报价和历史记录。</span></div>
                    </div>
                </div>
            </section>

            <section class="panel doc-card">
                <h2>业务边界</h2>
                <p>平台以供需信息展示和询报价撮合为核心，不直接替代双方的合同、付款、发货、验货和售后责任。正式交易前，双方应自行确认型号、品牌、批次、数量、价格、税赋、交期和资质信息。</p>
                <div class="doc-flow-grid">
                    <div><b>信息审核</b><span>发布信息会经过平台审核和风控规则检查，白名单会员可免人工审核但仍受风控约束。</span></div>
                    <div><b>数据留痕</b><span>询价、报价、发布、上下架、投诉和管理员操作均应形成记录，方便追溯。</span></div>
                    <div><b>风险处理</b><span>异常价格、敏感词、重复发布、投诉集中等情况可进入复核或限制流程。</span></div>
                    <div><b>持续完善</b><span>平台会根据会员使用反馈继续完善搜索、推荐、发布和后台管理体验。</span></div>
                </div>
            </section>

            <section class="panel doc-card">
                <h2>联系我们</h2>
                <div class="doc-table-wrap">
                    <table class="table">
                        <tbody>
                            <tr><th>公司地址</th><td><%= CompanyAddress %></td></tr>
                            <tr><th>服务邮箱</th><td><a href="mailto:<%= ServiceEmail %>"><%= ServiceEmail %></a></td></tr>
                            <tr><th>反馈建议</th><td><a href="#feedbackModal" data-feedback-open>在线留言与建议</a></td></tr>
                        </tbody>
                    </table>
                </div>
            </section>
            
            <section class="panel site-ad-panel workbench-ad-panel" hidden>
                <a class="search-ad-card" href="/help-center.aspx" data-ad-slot="HELP-S01">
                    <b>白银广告 HELP-S01</b>
                    <span>关于页底部低干扰位置，适合平台公告、合作入口和服务说明。</span>
                </a>
            </section>
        </main>
    </div>
    
    <!-- 反馈弹窗 -->
    <div class="modal-backdrop" id="feedbackModal" hidden>
        <div class="modal feedback-modal" role="dialog" aria-modal="true" aria-label="在线留言与建议">
            <div class="modal-head">
                <h2>在线留言与建议</h2>
                <button class="modal-close" type="button" data-feedback-close aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <form class="feedback-form" data-feedback-form>
                    <label>您的称呼 <em>*</em><input class="input" data-feedback-name required placeholder="如 张先生 / 李小姐"></label>
                    <label>联系方式 <em>*</em><input class="input" data-feedback-contact required placeholder="手机 / 微信 / 邹箱，便于回复"></label>
                    <label class="feedback-full">留言或建议<textarea class="input" data-feedback-content placeholder="请填写您遇到的问题、改进建议或合作需求"></textarea></label>
                    <div class="actions feedback-actions">
                        <button class="btn primary" type="button" data-feedback-submit>提交留言</button>
                        <button class="btn soft" type="button" data-feedback-close>取消</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <!-- 底部 -->
    <uc1:bottom runat="server" ID="bottom" />
</body>
</html>