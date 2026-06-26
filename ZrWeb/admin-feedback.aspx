<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-feedback.aspx.cs" Inherits="admin_feedback" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>反馈管理 - 阻容网</title>
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
                <a href="admin-brands.aspx">
                    <span class="nav-icon">🏷️</span>
                    <span>品牌管理</span>
                </a>
                <a href="admin-ads.aspx">
                    <span class="nav-icon">📢</span>
                    <span>广告管理</span>
                </a>
                <a class="active" href="admin-feedback.aspx">
                    <span class="nav-icon">💬</span>
                    <span>反馈管理</span>
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
                    <h1>反馈管理</h1>
                    <p class="admin-breadcrumb">首页 › 反馈管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            
            <section class="panel admin-panel">
                <div class="section-title">
                    <h2>反馈列表</h2>
                    <div class="admin-filter">
                        <a href="/admin-feedback.aspx" class="btn mini <%= StatusFilter == null ? "primary" : "" %>">全部</a>
                        <a href="/admin-feedback.aspx?status=0" class="btn mini <%= StatusFilter == "0" ? "primary" : "" %>">待处理</a>
                        <a href="/admin-feedback.aspx?status=1" class="btn mini <%= StatusFilter == "1" ? "primary" : "" %>">已回复</a>
                    </div>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>称呼</th>
                                <th>联系方式</th>
                                <th>内容</th>
                                <th>提交时间</th>
                                <th>状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% if (FeedbackList != null && FeedbackList.Rows.Count > 0) { %>
                            <asp:Repeater ID="rptFeedbacks" runat="server" EnableViewState="false">
                                <ItemTemplate>
                                    <tr>
                                        <td>F<%# Eval("feedbackId") %></td>
                                        <td><%# Eval("name") %></td>
                                        <td><%# Eval("contact") %></td>
                                        <td class="text-truncate"><%# Eval("content") %></td>
                                        <td><%# Convert.ToDateTime(Eval("createTime")).ToString("yyyy-MM-dd HH:mm") %></td>
                                        <td><span class="tag <%# Convert.ToInt32(Eval("status")) == 1 ? "green" : "orange" %>"><%# Convert.ToInt32(Eval("status")) == 1 ? "已回复" : "待处理" %></span></td>
                                        <td>
                                            <a href="#replyModal" class="btn mini" data-reply-id="<%# Eval("feedbackId") %>" data-reply-content="<%# Eval("content") %>">回复</a>
                                            <a href="/admin-feedback.aspx?action=delete&id=<%# Eval("feedbackId") %>" class="btn mini danger" onclick="return confirm('确定删除这条反馈？')">删除</a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <% } else { %>
                            <tr><td colspan="7" style="text-align:center;padding:20px;">暂无数据</td></tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
                
                <% if (TotalPages > 1) { %>
                <div class="pagination">
                    <% if (CurrentPage > 1) { %>
                    <a href="<%= GetPageUrl(CurrentPage - 1) %>" class="page-btn">上一页</a>
                    <% } %>
                    <% for (int i = 1; i <= TotalPages; i++) { %>
                    <a href="<%= GetPageUrl(i) %>" class="page-btn <%= i == CurrentPage ? "active" : "" %>"><%= i %></a>
                    <% } %>
                    <% if (CurrentPage < TotalPages) { %>
                    <a href="<%= GetPageUrl(CurrentPage + 1) %>" class="page-btn">下一页</a>
                    <% } %>
                </div>
                <% } %>
            </section>
        </main>
    </div>
    
    <div class="modal-backdrop" id="replyModal" hidden>
        <div class="modal feedback-modal" role="dialog" aria-modal="true" aria-label="回复反馈">
            <div class="modal-head">
                <h2>回复反馈</h2>
                <button class="modal-close" type="button" data-reply-close aria-label="关闭">×</button>
            </div>
            <div class="modal-body">
                <div class="feedback-content-preview" id="feedbackContentPreview"></div>
                <form class="reply-form" data-reply-form>
                    <input type="hidden" id="replyId" name="replyId">
                    <label class="feedback-full">回复内容<textarea class="input" id="replyContent" name="replyContent" rows="5" placeholder="请输入回复内容"></textarea></label>
                    <div class="actions feedback-actions">
                        <button class="btn primary" type="button" data-reply-submit>提交回复</button>
                        <button class="btn soft" type="button" data-reply-close>取消</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    
    <script>
        (function() {
            var modal = document.getElementById('replyModal');
            var closeBtns = document.querySelectorAll('[data-reply-close]');
            var replyLinks = document.querySelectorAll('[data-reply-id]');
            var submitBtn = document.querySelector('[data-reply-submit]');
            var preview = document.getElementById('feedbackContentPreview');
            var replyId = document.getElementById('replyId');
            var replyContent = document.getElementById('replyContent');
            
            replyLinks.forEach(function(link) {
                link.addEventListener('click', function(e) {
                    e.preventDefault();
                    var id = this.getAttribute('data-reply-id');
                    var content = this.getAttribute('data-reply-content');
                    replyId.value = id;
                    preview.innerHTML = '<p><strong>反馈内容：</strong></p><p>' + (content || '') + '</p>';
                    replyContent.value = '';
                    modal.removeAttribute('hidden');
                });
            });
            
            closeBtns.forEach(function(btn) {
                btn.addEventListener('click', function() {
                    modal.setAttribute('hidden', '');
                });
            });
            
            modal.addEventListener('click', function(e) {
                if (e.target === modal) {
                    modal.setAttribute('hidden', '');
                }
            });
            
            submitBtn.addEventListener('click', function() {
                var id = replyId.value;
                var content = replyContent.value.trim();
                
                if (!content) {
                    alert('请填写回复内容');
                    return;
                }
                
                var xhr = new XMLHttpRequest();
                xhr.open('POST', '/admin-feedback.aspx', true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.onreadystatechange = function() {
                    if (xhr.readyState === 4) {
                        try {
                            var result = JSON.parse(xhr.responseText);
                            if (result.success) {
                                alert('回复成功！');
                                window.location.reload();
                            } else {
                                alert('回复失败: ' + (result.message || '未知错误'));
                            }
                        } catch (e) {
                            alert('回复失败: 服务器响应异常');
                        }
                    }
                };
                
                xhr.send('action=reply&id=' + id + '&content=' + encodeURIComponent(content));
            });
        })();
    </script>
</body>
</html>