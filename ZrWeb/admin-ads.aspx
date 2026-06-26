<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-ads.aspx.cs" Inherits="admin_ads" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>广告管理 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
</head>
<body class="admin-page">
    <div class="admin-app">
        <aside class="admin-sidebar">
            <div class="admin-brand">
                <div class="admin-logo-icon">ZR</div>
                <div class="admin-brand-text">
                    <strong>阻容网</strong>
                    <span>管理员后台</span>
                </div>
            </div>
            <nav class="admin-nav">
                <a href="admin-console.aspx">
                    <span class="nav-icon">📊</span>
                    <span>控制台</span>
                </a>
                <a href="admin-users.aspx">
                    <span class="nav-icon">👥</span>
                    <span>用户管理</span>
                </a>
                <a href="admin-goods.aspx">
                    <span class="nav-icon">📦</span>
                    <span>供需管理</span>
                </a>
                <a href="admin-quotes.aspx">
                    <span class="nav-icon">💰</span>
                    <span>报价管理</span>
                </a>
                <a class="active" href="admin-ads.aspx">
                    <span class="nav-icon">📢</span>
                    <span>广告管理</span>
                </a>
            </nav>
            <div class="admin-sidebar-footer">
                <div class="admin-user-info">
                    <span><%= AdminName %></span>
                    <small>超级管理员</small>
                </div>
                <a href="admin-login.aspx?action=logout" class="admin-logout">退出登录</a>
            </div>
        </aside>
        <main class="admin-main">
            <header class="admin-topbar">
                <div>
                    <h1>广告管理</h1>
                    <p class="admin-breadcrumb">首页 › 广告管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="section-title">
                    <div>
                        <h2>广告位管理</h2>
                        <span class="admin-table-count">共 <%= TotalAds %> 条记录</span>
                        <span style="color:red;font-size:12px;margin-left:10px;">DEBUG: AdList=<%= AdList == null ? "null" : "rows=" + AdList.Rows.Count %></span>
                    </div>
                    <button class="btn primary" onclick="showAddAd()">新增广告</button>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>广告位</th>
                                <th>标题</th>
                                <th>位置</th>
                                <th>链接</th>
                                <th>开始时间</th>
                                <th>结束时间</th>
                                <th>状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (AdList != null && AdList.Rows.Count > 0) { %>
                            <asp:Repeater ID="rptAds" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("AdSlot") %></td>
                                        <td><%# Eval("Title") %></td>
                                        <td><%# Eval("Position") %></td>
                                        <td><a href="<%# Eval("LinkUrl") %>" target="_blank"><%# Eval("LinkUrl") %></a></td>
                                        <td><%# Eval("StartDate") %></td>
                                        <td><%# Eval("EndDate") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("Status")) == 1 ? "green" : "gray" %>"><%# Convert.ToInt32(Eval("Status")) == 1 ? "启用" : "停用" %></span></td>
                                        <td>
                                            <button class="btn mini" onclick="toggleAdStatus(<%# Eval("AdID") %>, <%# Convert.ToInt32(Eval("Status")) == 1 ? 0 : 1 %>)">
                                                <%# Convert.ToInt32(Eval("Status")) == 1 ? "停用" : "启用" %>
                                            </button>
                                            <button class="btn mini" onclick="deleteAd(<%# Eval("AdID") %>)">删除</button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } else { %>
                            <tr><td colspan="8" style="text-align:center;padding:40px;">暂无数据</td></tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
            </section>
        </main>
    </div>
    
    <!-- 新增广告模态框 -->
    <div class="modal-backdrop" id="addAdModal" hidden>
        <div class="modal" role="dialog" aria-modal="true" aria-label="新增广告">
            <div class="modal-head">
                <h2>新增广告</h2>
                <button class="modal-close" type="button" onclick="closeAddAd()" aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <form id="addAdForm" method="post" action="/admin-ads.aspx">
                    <input type="hidden" name="action" value="addAd">
                    <div class="form-row">
                        <label>广告位 <em class="required-mark">*</em></label>
                        <select class="select" name="adSlot" required>
                            <option value="">请选择广告位</option>
                            <option value="HOME-D01">首页钻石位 (HOME-D01)</option>
                            <option value="HOME-G01">首页黄金位 (HOME-G01)</option>
                            <option value="SR-D01">搜索钻石位1 (SR-D01)</option>
                            <option value="SR-D02">搜索钻石位2 (SR-D02)</option>
                            <option value="SR-G01">搜索黄金位1 (SR-G01)</option>
                            <option value="SR-G02">搜索黄金位2 (SR-G02)</option>
                            <option value="SUP-G01">供应详情 (SUP-G01)</option>
                            <option value="DEM-G01">需求详情 (DEM-G01)</option>
                        </select>
                    </div>
                    <div class="form-row">
                        <label>广告标题 <em class="required-mark">*</em></label>
                        <input class="input" name="title" placeholder="请输入广告标题" maxlength="50" required>
                    </div>
                    <div class="form-row">
                        <label>广告位置</label>
                        <input class="input" name="position" placeholder="请输入位置标识，如：1、2、3" maxlength="20">
                    </div>
                    <div class="form-row">
                        <label>链接地址 <em class="required-mark">*</em></label>
                        <input class="input" name="linkUrl" type="url" placeholder="请输入广告链接地址" maxlength="255" required>
                    </div>
                    <div class="form-row-inline">
                        <div class="form-row">
                            <label>开始时间 <em class="required-mark">*</em></label>
                            <input class="input" name="startDate" type="date" required>
                        </div>
                        <div class="form-row">
                            <label>结束时间 <em class="required-mark">*</em></label>
                            <input class="input" name="endDate" type="date" required>
                        </div>
                    </div>
                    <div class="form-row">
                        <label>状态</label>
                        <label class="radio-row">
                            <input type="radio" name="status" value="1" checked> <span>启用</span>
                        </label>
                        <label class="radio-row">
                            <input type="radio" name="status" value="0"> <span>停用</span>
                        </label>
                    </div>
                    <div class="form-actions" style="justify-content: flex-end;">
                        <button class="btn" type="button" onclick="closeAddAd()">取消</button>
                        <button class="btn primary" type="submit">保存</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <script src="/assets/js/toast.js"></script>
    <script>
        function showAddAd() {
            document.getElementById('addAdModal').hidden = false;
        }
        
        window.onload = function() {
            var urlParams = new URLSearchParams(window.location.search);
            var msg = urlParams.get('msg');
            if (msg == 'addSuccess') {
                Toast.success('广告添加成功');
            } else if (msg == 'addError') {
                var err = urlParams.get('err');
                Toast.error('添加失败：' + (err || '未知错误'));
            }
        };
        
        function closeAddAd() {
            document.getElementById('addAdModal').hidden = true;
        }
        
        function toggleAdStatus(adId, status) {
            ConfirmDialog.show(status == 1 ? '确定启用该广告？' : '确定停用该广告？', function() {
                window.location.href = '/admin-ads.aspx?action=toggleStatus&adId=' + adId + '&status=' + status;
            });
        }
        
        function deleteAd(adId) {
            ConfirmDialog.show('确定删除该广告？此操作不可恢复。', function() {
                window.location.href = '/admin-ads.aspx?action=delete&adId=' + adId;
            });
        }
        
        document.getElementById('addAdForm').addEventListener('submit', function(e) {
            e.preventDefault();
            
            var adSlot = this.adSlot.value;
            var title = this.title.value.trim();
            var linkUrl = this.linkUrl.value.trim();
            var startDate = this.startDate.value;
            var endDate = this.endDate.value;
            
            if (!adSlot) {
                Toast.error('请选择广告位');
                return;
            }
            if (!title) {
                Toast.error('请输入广告标题');
                return;
            }
            if (!linkUrl) {
                Toast.error('请输入链接地址');
                return;
            }
            if (!startDate || !endDate) {
                Toast.error('请选择开始时间和结束时间');
                return;
            }
            if (new Date(startDate) > new Date(endDate)) {
                Toast.error('结束时间不能早于开始时间');
                return;
            }
            
            this.submit();
        });
    </script>
</body>
</html>