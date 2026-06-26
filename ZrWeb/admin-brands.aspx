<%@ Page Language="C#" AutoEventWireup="true" CodeFile="admin-brands.aspx.cs" Inherits="admin_brands" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
<!DOCTYPE html>
<html lang="zh-CN">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>品牌管理 - 阻容网</title>
    <link rel="stylesheet" href="/assets/css/styles.css">
    <style>
        .admin-table-wrap { max-height: none !important; overflow-y: visible !important; }
    </style>
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
                        <span class="admin-table-count">共 <%= BrandCount %> 个品牌</span>
                    </div>
                    <div class="admin-search-actions">
                        <input class="input admin-search" id="txtSearch" placeholder="搜索品牌名称" onkeyup="filterBrands()">
                        <button class="btn" onclick="filterBrands()">搜索</button>
                        <button class="btn primary" onclick="showAddModal()">添加品牌</button>
                    </div>
                </div>
                <div class="admin-table-wrap">
                    <table class="table admin-table" id="brandsTable">
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
                        <tbody>
                            <%= BrandsHtml %>
                        </tbody>
                    </table>
                </div>
            </section>
        </main>
    </div>
    <script src="/assets/js/toast.js"></script>
    <script>
    function filterBrands() {
        var keyword = document.getElementById('txtSearch').value.toLowerCase().trim();
        var rows = document.querySelectorAll('#brandsTable tbody tr');
        rows.forEach(function(row) {
            var name = row.querySelector('td:nth-child(2)').textContent.toLowerCase();
            row.style.display = keyword && name.indexOf(keyword) < 0 ? 'none' : '';
        });
    }
    
    function showAddModal() {
        document.getElementById('modalTitle').textContent = '添加品牌';
        document.getElementById('brandId').value = '0';
        document.getElementById('brandName').value = '';
        document.getElementById('brandDesc').value = '';
        document.getElementById('sortOrder').value = '0';
        document.getElementById('brandModal').hidden = false;
    }
    
    function editBrand(id, name, desc, order) {
        document.getElementById('modalTitle').textContent = '编辑品牌';
        document.getElementById('brandId').value = id;
        document.getElementById('brandName').value = name;
        document.getElementById('brandDesc').value = desc || '';
        document.getElementById('sortOrder').value = order || 0;
        document.getElementById('brandModal').hidden = false;
    }
    
    function closeModal() {
        document.getElementById('brandModal').hidden = true;
    }
    
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
                location.reload();
            } else {
                Toast.error(data.message);
            }
        })
        .catch(err => Toast.error('保存失败: ' + err));
    });
    
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
                location.reload();
            } else {
                Toast.error(data.message);
            }
        })
        .catch(err => Toast.error('删除失败: ' + err));
    }
    </script>
    
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
