// 自定义提示框组件 - 与网站风格一致
var Toast = {
    container: null,
    
    // 初始化容器
    init: function() {
        if (this.container) return;
        
        this.container = document.createElement('div');
        this.container.id = 'toast-container';
        this.container.style.cssText = 'position:fixed;top:20px;left:50%;transform:translateX(-50%);z-index:9999;display:flex;flex-direction:column;gap:10px;pointer-events:none;';
        document.body.appendChild(this.container);
    },
    
    // 显示提示
    show: function(message, type, duration) {
        this.init();
        
        type = type || 'info';
        duration = duration || 3000;
        
        var toast = document.createElement('div');
        toast.className = 'toast-item toast-' + type;
        
        var icon = '';
        var bgColor = '';
        var borderColor = '';
        
        switch(type) {
            case 'success':
                icon = '✓';
                bgColor = 'linear-gradient(135deg, #ecfdf3, #d1fae5)';
                borderColor = '#10b981';
                break;
            case 'error':
                icon = '✕';
                bgColor = 'linear-gradient(135deg, #fef2f2, #fee2e2)';
                borderColor = '#ef4444';
                break;
            case 'warning':
                icon = '⚠';
                bgColor = 'linear-gradient(135deg, #fffbeb, #fef3c7)';
                borderColor = '#f59e0b';
                break;
            default:
                icon = 'ℹ';
                bgColor = 'linear-gradient(135deg, #eff6ff, #dbeafe)';
                borderColor = '#3b82f6';
        }
        
        toast.style.cssText = 'pointer-events:auto;display:flex;align-items:center;gap:12px;padding:14px 20px;border-radius:16px;background:' + bgColor + ';border:1px solid ' + borderColor + ';box-shadow:0 12px 28px rgba(16,24,40,.12);font-size:14px;color:#344054;font-weight:700;max-width:400px;animation:toastIn 0.3s ease;pointer-events:auto;';
        
        toast.innerHTML = '<span style="width:28px;height:28px;border-radius:10px;background:' + borderColor + ';color:#fff;display:grid;place-items:center;font-size:14px;font-weight:950;">' + icon + '</span><span style="flex:1;line-height:1.5;">' + message + '</span>';
        
        this.container.appendChild(toast);
        
        // 自动消失
        setTimeout(function() {
            toast.style.animation = 'toastOut 0.3s ease forwards';
            setTimeout(function() {
                if (toast.parentNode) {
                    toast.parentNode.removeChild(toast);
                }
            }, 300);
        }, duration);
        
        return toast;
    },
    
    // 成功提示
    success: function(message, duration) {
        return this.show(message, 'success', duration);
    },
    
    // 错误提示
    error: function(message, duration) {
        return this.show(message, 'error', duration);
    },
    
    // 警告提示
    warning: function(message, duration) {
        return this.show(message, 'warning', duration);
    },
    
    // 信息提示
    info: function(message, duration) {
        return this.show(message, 'info', duration);
    }
};

// 确认对话框
var ConfirmDialog = {
    show: function(message, onConfirm, onCancel) {
        var existing = document.getElementById('confirm-dialog-overlay');
        if (existing) existing.remove();
        
        var overlay = document.createElement('div');
        overlay.id = 'confirm-dialog-overlay';
        overlay.style.cssText = 'position:fixed;top:0;left:0;right:0;bottom:0;z-index:9998;background:rgba(16,24,40,.4);display:grid;place-items:center;';
        
        var dialog = document.createElement('div');
        dialog.style.cssText = 'width:min(380px,calc(100vw - 32px));background:rgba(255,255,255,.96);border:1px solid #f0dfd0;border-radius:20px;padding:24px;box-shadow:0 24px 64px rgba(16,24,40,.18);';
        
        dialog.innerHTML = '<div style="display:flex;align-items:center;gap:14px;margin-bottom:18px;"><span style="width:48px;height:48px;border-radius:16px;background:linear-gradient(135deg,#fff7ed,#fef3c7);display:grid;place-items:center;font-size:24px;color:#f59e0b;">⚠</span><h3 style="font-size:18px;font-weight:950;color:#101828;margin:0;">确认操作</h3></div><p style="font-size:14px;color:#475467;line-height:1.6;margin:0 0 20px;">' + message + '</p><div style="display:flex;gap:12px;justify-content:flex-end;"><button id="confirm-cancel-btn" style="min-height:40px;padding:0 18px;border-radius:12px;border:1px solid #d8deea;background:#fff;color:#344054;font-size:14px;font-weight:800;cursor:pointer;">取消</button><button id="confirm-ok-btn" style="min-height:40px;padding:0 18px;border-radius:12px;border:1px solid #f97316;background:linear-gradient(135deg,#f97316,#fb923c);color:#fff;font-size:14px;font-weight:800;cursor:pointer;">确认</button></div>';
        
        overlay.appendChild(dialog);
        document.body.appendChild(overlay);
        
        var cancelBtn = document.getElementById('confirm-cancel-btn');
        var okBtn = document.getElementById('confirm-ok-btn');
        
        cancelBtn.addEventListener('click', function() {
            overlay.remove();
            if (onCancel) onCancel();
        });
        
        okBtn.addEventListener('click', function() {
            overlay.remove();
            if (onConfirm) onConfirm();
        });
        
        overlay.addEventListener('click', function(e) {
            if (e.target === overlay) {
                overlay.remove();
                if (onCancel) onCancel();
            }
        });
    }
};

// 添加动画样式
(function() {
    var style = document.createElement('style');
    style.textContent = '@keyframes toastIn{from{opacity:0;transform:translateY(-20px)}to{opacity:1;transform:translateY(0)}}@keyframes toastOut{from{opacity:1;transform:translateY(0)}to{opacity:0;transform:translateY(-20px)}}';
    document.head.appendChild(style);
})();