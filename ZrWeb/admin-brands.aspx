<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-brands.aspx.cs" Inherits="admin_brands" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>品牌管理 - 阻容网</title>
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
                <a href="admin-brands.aspx" class="active">
                    <span class="nav-icon">🏷️</span>
                    <span>品牌管理</span>
                </a>
                <a href="admin-ads.aspx">
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
                    <h1>品牌管理</h1>
                    <p class="admin-breadcrumb">首页 › 品牌管理</p>
                </div>
                <div class="admin-top-actions">
                    <span class="admin-date"><%= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") %></span>
                </div>
            </header>
            <section class="panel admin-panel">
                <div class="section-title">
                    <div>
                        <h2>品牌列表</h2>
                        <span class="admin-table-count">共 <span id="brandCount">0</span> 个品牌</span>
                    </div>
                    <div class="admin-search-actions">
                        <input class="input admin-search" id="txtSearch" placeholder="搜索品牌名称">
                        <button class="btn" onclick="searchBrands()">搜索</button>
                        <button class="btn primary" onclick="showAddModal()">添加品牌</button>
                    </div>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table">
                        <thead>
                            <tr>
                                <th>序号</th>
                                <th>品牌名称</th>
                                <th>品牌描述</th>
                                <th>排序</th>
                                <th>状态</th>
                                <th>操作</th>
                            </tr>
                        </thead>
                        <tbody id="brandsBody">
                            <tr><td colspan="6" style="text-align:center;padding:40px;">加载中...</td></tr>
                        </tbody>
                    </table>
                </div>
            </section>
        </main>
    </div>
    <script src="/assets/js/toast.js"></script>
    <script>
    var allBrands = [];
    
    // 加载品牌列表
    function loadBrands() {
        fetch('api/brands.aspx?action=list')
        .then(r => r.json())
        .then(data => {
            allBrands = data.brands || [];
            document.getElementById('brandCount').textContent = allBrands.length;
            renderBrands(allBrands);
        })
        .catch(err => {
            document.getElementById('brandsBody').innerHTML = '<tr><td colspan="6" style="text-align:center;color:red;padding:40px;">加载失败: ' + err + '</td></tr>';
        });
    }
    
    // 渲染品牌列表
    function renderBrands(brands) {
        var tbody = document.getElementById('brandsBody');
        if (brands.length > 0) {
            tbody.innerHTML = brands.map(function(b, index) {
                var editName = b.BrandName.replace(/'/g, "\\'").replace(/"/g, '\\"');
                var editDesc = (b.BrandDesc || '').replace(/'/g, "\\'").replace(/"/g, '\\"');
                return '<tr>' +
                    '<td>' + (index + 1) + '</td>' +
                    '<td><strong>' + escapeHtml(b.BrandName) + '</strong></td>' +
                    '<td>' + escapeHtml(b.BrandDesc || '-') + '</td>' +
                    '<td>' + b.SortOrder + '</td>' +
                    '<td><span class="tag green">启用</span></td>' +
                    '<td>' +
                    '<button class="btn mini" onclick="editBrand(' + b.BrandId + ',\'' + editName + '\',\'' + editDesc + '\',' + b.SortOrder + ')">编辑</button> ' +
                    '<button class="btn mini danger" onclick="deleteBrand(' + b.BrandId + ')">删除</button>' +
                    '</td>' +
                    '</tr>';
            }).join('');
        } else {
            tbody.innerHTML = '<tr><td colspan="6" style="text-align:center;padding:40px;">暂无品牌数据</td></tr>';
        }
    }
    
    // HTML转义
    function escapeHtml(str) {
        if (!str) return '';
        return str.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }
    
    // 搜索品牌
    function searchBrands() {
        var keyword = document.getElementById('txtSearch').value.toLowerCase().trim();
        if (keyword) {
            var filtered = allBrands.filter(function(b) {
                return b.BrandName.toLowerCase().indexOf(keyword) >= 0;
            });
            renderBrands(filtered);
        } else {
            renderBrands(allBrands);
        }
    }
    
    // 回车搜索
    document.getElementById('txtSearch').addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            searchBrands();
        }
    });
    
    // 显示添加弹窗
    function showAddModal() {
        document.getElementById('modalTitle').textContent = '添加品牌';
        document.getElementById('brandId').value = '0';
        document.getElementById('brandName').value = '';
        document.getElementById('brandDesc').value = '';
        document.getElementById('sortOrder').value = '0';
        document.getElementById('brandModal').hidden = false;
    }
    
    // 编辑品牌
    function editBrand(id, name, desc, order) {
        document.getElementById('modalTitle').textContent = '编辑品牌';
        document.getElementById('brandId').value = id;
        document.getElementById('brandName').value = name;
        document.getElementById('brandDesc').value = desc || '';
        document.getElementById('sortOrder').value = order || 0;
        document.getElementById('brandModal').hidden = false;
    }
    
    // 关闭弹窗
    function closeModal() {
        document.getElementById('brandModal').hidden = true;
    }
    
    // 保存品牌
    document.getElementById('brandForm').addEventListener('submit', function(e) {
        e.preventDefault();
        
        var brandId = document.getElementById('brandId').value;
        var brandName = document.getElementById('brandName').value;
        var brandDesc = document.getElementById('brandDesc').value;
        var sortOrder = document.getElementById('sortOrder').value;
        
        if (!brandName.trim()) {
            Toast.warning('请输入品牌名称');
            return;
        }
        
        var action = brandId === '0' ? 'add' : 'update';
        var formData = new FormData();
        formData.append('action', action);
        if (brandId !== '0') formData.append('brandId', brandId);
        formData.append('brandName', brandName);
        formData.append('brandDesc', brandDesc);
        formData.append('sortOrder', sortOrder);
        
        fetch('api/brands.aspx', { method: 'POST', body: formData })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                Toast.success(data.message);
                closeModal();
                loadBrands();
            } else {
                Toast.error(data.message);
            }
        })
        .catch(err => Toast.error('保存失败: ' + err));
    });
    
    // 删除品牌
    function deleteBrand(id) {
        if (!confirm('确定要删除这个品牌吗？')) return;
        
        var formData = new FormData();
        formData.append('action', 'delete');
        formData.append('brandId', id);
        
        fetch('api/brands.aspx', { method: 'POST', body: formData })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                Toast.success('删除成功');
                loadBrands();
            } else {
                Toast.error(data.message);
            }
        })
        .catch(err => Toast.error('删除失败: ' + err));
    }
    
    // 页面加载
    loadBrands();
    </script>
    
    <!-- 添加/编辑弹窗 -->
    <div class="modal-backdrop" id="brandModal" hidden>
        <div class="modal" role="dialog" aria-modal="true" aria-label="品牌编辑">
            <div class="modal-head">
                <h2 id="modalTitle">添加品牌</h2>
                <button class="modal-close" type="button" onclick="closeModal()">×</button>
            </div>
            <div class="modal-body">
                <form id="brandForm">
                    <input type="hidden" id="brandId" value="0">
                    <div class="form-row">
                        <label>品牌名称 *</label>
                        <input type="text" class="input" id="brandName" required placeholder="请输入品牌名称">
                    </div>
                    <div class="form-row">
                        <label>品牌描述</label>
                        <textarea class="input" id="brandDesc" placeholder="请输入品牌描述（可选）" rows="3"></textarea>
                    </div>
                    <div class="form-row">
                        <label>排序</label>
                        <input type="number" class="input" id="sortOrder" value="0" placeholder="数字越小越靠前">
                    </div>
                    <div class="form-actions" style="justify-content: flex-end;">
                        <button type="button" class="btn" onclick="closeModal()">取消</button>
                        <button type="submit" class="btn primary">保存</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</body>
</html>
