<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LoginModal.ascx.cs" Inherits="UserControls_LoginModal" %>
<div class="modal-backdrop" id="loginModal" hidden style="z-index: 1000;">
    <div class="modal login-modal" role="dialog" aria-modal="true" aria-label="登录">
        <div class="modal-head">
            <h2>登录</h2>
            <button class="modal-close" type="button" data-modal-close aria-label="关闭">×</button>
        </div>
        <div class="modal-body">
            <div class="login-tabs" role="tablist">
                <button class="active" type="button" data-login-method="account">账号登录</button>
                <button type="button" data-login-method="sms">手机验证码</button>
            </div>
            <div class="login-method-panel active" data-login-panel="account">
                <form id="loginForm">
                    <div class="form-row"><label>手机号</label><input type="text" name="txtUserName" id="txtUserName" class="input" placeholder="请输入手机号" /></div>
                    <div class="form-row"><label>密码</label><input type="password" name="txtPassword" id="txtPassword" class="input" placeholder="请输入密码" /></div>
                    <span id="loginError" class="error-msg"></span>
                    <label class="privacy-check"><input type="checkbox" id="chkPrivacy" name="chkPrivacy"> <span>已阅读并同意 <a href="/about-us.aspx">《隐私政策》</a></span></label>
                    <button type="button" class="btn primary login-submit" onclick="submitLoginForm();">登录</button>
                </form>
                <p class="login-tip">使用手机号和密码登录，登录后可管理供需信息。</p>
                <p class="register-link">没有账号？<a href="/register.aspx" target="_blank">立即注册</a></p>
            </div>
            <div class="login-method-panel" data-login-panel="sms" hidden>
                <div class="sms-login-form sms-code-row">
                    <label>手机号<input class="input" data-login-phone inputmode="numeric" placeholder="请输入手机号"></label>
                    <label>验证码<input class="input" data-login-code inputmode="numeric" placeholder="6 位验证码"></label>
                    <button class="btn soft" type="button" data-ui-toast="验证码已发送">获取验证码</button>
                </div>
                <label class="privacy-check"><input type="checkbox" data-login-privacy> <span>已阅读并同意 <a href="#" data-policy-link>《隐私政策》</a></span></label>
                <button class="btn primary login-submit" type="button" data-login-submit>登录并继续</button>
                <p class="login-tip">使用手机验证码登录，无需记住密码。</p>
            </div>
        </div>
    </div>
</div>
<script>
    (function() {
        var loginModal = document.getElementById('loginModal');
        
        if (!loginModal) return;

        loginModal.querySelector('[data-modal-close]').addEventListener('click', function() {
            loginModal.hidden = true;
        });

        loginModal.querySelectorAll('[data-login-method]').forEach(function(btn) {
            btn.addEventListener('click', function() {
                var method = this.getAttribute('data-login-method');
                loginModal.querySelectorAll('[data-login-method]').forEach(function(b) { b.classList.remove('active'); });
                this.classList.add('active');
                loginModal.querySelectorAll('[data-login-panel]').forEach(function(panel) {
                    panel.hidden = panel.getAttribute('data-login-panel') !== method;
                    panel.classList.toggle('active', panel.getAttribute('data-login-panel') === method);
                });
            });
        });

        var loginForm = loginModal.querySelector('#loginForm');
        if (loginForm) {
            loginForm.addEventListener('submit', function(e) {
                e.preventDefault();
                if (validateLoginForm()) {
                    var formData = new FormData(loginForm);
                    
                    fetch('/login.aspx', {
                        method: 'POST',
                        body: formData
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            loginModal.hidden = true;
                            handleLoginSuccess();
                        } else {
                            document.getElementById('loginError').textContent = data.message;
                        }
                    })
                    .catch(error => {
                        console.error('登录异常:', error);
                    });
                }
            });
        }
});
    

    function handleLoginSuccess() {
        fetch('/check-login.ashx')
        .then(function(response) {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(function(loginData) {
            if (loginData.success && loginData.isLoggedIn) {
                window.ZR_CURRENT_MEMBER = {
                    userId: loginData.userId,
                    userName: loginData.userName,
                    shopId: loginData.shopId,
                    shopName: loginData.shopName
                };
            }
        })
        .catch(function(error) {
            console.log('更新登录状态失败:', error);
        })
        .finally(function() {
            var merchantData = sessionStorage.getItem('pendingPublish_merchant');
            if (merchantData) {
                sessionStorage.removeItem('pendingPublish_merchant');
                var data = JSON.parse(merchantData);
                restoreAndSubmitMerchantPublish(data);
                return;
            }

            var buyerData = sessionStorage.getItem('pendingPublish_buyer');
            if (buyerData) {
                sessionStorage.removeItem('pendingPublish_buyer');
                var data = JSON.parse(buyerData);
                restoreAndSubmitBuyerPublish(data);
                return;
            }

            var indexData = sessionStorage.getItem('pendingPublish_index');
            if (indexData) {
                sessionStorage.removeItem('pendingPublish_index');
                var data = JSON.parse(indexData);
                restoreAndSubmitIndexPublish(data);
                return;
            }

            location.reload();
        });
    }

    function restoreAndSubmitMerchantPublish(data) {
        var publishModal = document.getElementById('publishModal');
        if (publishModal) {
            publishModal.hidden = false;
            
            for (var key in data) {
                var input = publishModal.querySelector('[name="' + key + '"]') || publishModal.querySelector('#' + key);
                if (input) {
                    input.value = data[key];
                }
            }
            
            var taxSwitch = publishModal.querySelector('[data-tax-toggle]');
            if (taxSwitch && data.isIncludingTax === '1') {
                taxSwitch.classList.add('is-on');
                taxSwitch.setAttribute('aria-pressed', 'true');
                var priceField = taxSwitch.closest('.price-field');
                if (priceField) {
                    priceField.classList.remove('is-untaxed');
                    priceField.classList.add('is-taxed');
                    priceField.querySelector('span').textContent = '含税';
                }
            }
            
            var publishConfirmBtn = publishModal.querySelector('[data-publish-confirm]');
            if (publishConfirmBtn) {
                var formData = new FormData(publishModal.querySelector('form'));
                formData.set('action', data.action || 'publish_goods');
                formData.set('pubType', data.pubType || '1');
                
                publishConfirmBtn.disabled = true;
                publishConfirmBtn.textContent = '发布中...';
                
                fetch('merchant-workbench.aspx', {
                    method: 'POST',
                    body: formData
                })
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        alert('发布成功！');
                        publishModal.hidden = true;
                        publishModal.querySelector('form').reset();
                    } else {
                        alert('发布失败：' + result.message);
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
        }
    }

    function restoreAndSubmitBuyerPublish(data) {
        var publishModal = document.getElementById('publishModal');
        if (publishModal) {
            publishModal.hidden = false;
            
            for (var key in data) {
                var input = publishModal.querySelector('[name="' + key + '"]') || publishModal.querySelector('#' + key);
                if (input) {
                    input.value = data[key];
                }
            }
            
            var taxSwitch = publishModal.querySelector('[data-tax-toggle]');
            if (taxSwitch && data.isIncludingTax === '1') {
                taxSwitch.classList.add('is-on');
                taxSwitch.setAttribute('aria-pressed', 'true');
                var priceField = taxSwitch.closest('.price-field');
                if (priceField) {
                    priceField.classList.remove('is-untaxed');
                    priceField.classList.add('is-taxed');
                    priceField.querySelector('span').textContent = '含税';
                }
            }
            
            var publishConfirmBtn = publishModal.querySelector('[data-publish-confirm]');
            if (publishConfirmBtn) {
                var formData = new FormData(publishModal.querySelector('form'));
                formData.set('action', data.action || 'publish_demand');
                formData.set('pubType', data.pubType || '2');
                
                publishConfirmBtn.disabled = true;
                publishConfirmBtn.textContent = '发布中...';
                
                fetch('buyer-workbench.aspx', {
                    method: 'POST',
                    body: formData
                })
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        alert('发布成功！');
                        publishModal.hidden = true;
                        publishModal.querySelector('form').reset();
                    } else {
                        alert('发布失败：' + result.message);
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
        }
    }

    function restoreAndSubmitIndexPublish(data) {
        var publishModal = document.getElementById('publishModal');
        if (publishModal) {
            publishModal.hidden = false;
            
            var kindBtns = publishModal.querySelectorAll('[data-publish-kind]');
            kindBtns.forEach(function(b) { b.classList.remove('active'); });
            var kind = data.pubType === '1' ? 'supply' : 'demand';
            kindBtns.forEach(function(b) {
                if (b.getAttribute('data-publish-kind') === kind) {
                    b.classList.add('active');
                }
            });
            
            var title = publishModal.querySelector('[data-publish-title]');
            if (title) {
                title.textContent = kind === 'supply' ? '发布供应' : '发布需求';
            }

            for (var key in data) {
                var input = publishModal.querySelector('[name="' + key + '"]') || publishModal.querySelector('#' + key);
                if (input) {
                    input.value = data[key];
                }
            }

            var taxSwitch = publishModal.querySelector('[data-tax-toggle]');
            if (taxSwitch && data.isIncludingTax === '1') {
                taxSwitch.classList.add('is-on');
                taxSwitch.setAttribute('aria-pressed', 'true');
                var priceField = taxSwitch.previousElementSibling;
                if (priceField) {
                    priceField.classList.remove('is-untaxed');
                    priceField.classList.add('is-taxed');
                    priceField.querySelector('span').textContent = '含税';
                }
            }

            var publishConfirmBtn = publishModal.querySelector('[data-publish-confirm]');
            if (publishConfirmBtn) {
                var formData = new FormData(publishModal.querySelector('form'));
                formData.set('action', data.action || 'publish_goods');
                formData.set('pubType', data.pubType || '1');

                publishConfirmBtn.disabled = true;
                publishConfirmBtn.textContent = '发布中...';

                fetch('index.aspx', {
                    method: 'POST',
                    body: formData
                })
                .then(response => response.json())
                .then(result => {
                    if (result.success) {
                        alert('发布成功！');
                        publishModal.hidden = true;
                        publishModal.querySelector('form').reset();
                    } else {
                        alert('发布失败：' + result.message);
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
        }
    }

    function validateLoginForm() {
        var loginModal = document.getElementById('loginModal');
        if (!loginModal) return false;
        
        var userName = loginModal.querySelector('#txtUserName');
        var password = loginModal.querySelector('#txtPassword');
        var privacy = loginModal.querySelector('#chkPrivacy');
        var errorSpan = loginModal.querySelector('#loginError');
        
        errorSpan.textContent = '';
        
        if (!userName.value.trim()) {
            userName.classList.add('input-error');
            errorSpan.textContent = '请输入手机号';
            return false;
        }
        if (!/^1[3-9]\d{9}$/.test(userName.value.trim())) {
            userName.classList.add('input-error');
            errorSpan.textContent = '请输入正确的11位手机号';
            return false;
        }
        userName.classList.remove('input-error');
        
        if (!password.value.trim()) {
            password.classList.add('input-error');
            errorSpan.textContent = '请输入密码';
            return false;
        }
        password.classList.remove('input-error');
        
        if (!privacy.checked) {
            errorSpan.textContent = '请阅读并同意隐私政策';
            return false;
        }
        
        return true;
    }

    function submitLoginForm() {
        if (!validateLoginForm()) return;
        
        var loginModal = document.getElementById('loginModal');
        var loginForm = loginModal.querySelector('#loginForm');
        var submitBtn = loginModal.querySelector('.login-submit');
        var errorSpan = loginModal.querySelector('#loginError');
        
        var formData = new FormData(loginForm);
        formData.append('action', 'ajax_login');
        
        submitBtn.disabled = true;
        submitBtn.textContent = '登录中...';
        
        fetch('/login.aspx', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                loginModal.hidden = true;
                handleLoginSuccess();
            } else {
                errorSpan.textContent = data.message || '登录失败';
            }
        })
        .catch(error => {
            errorSpan.textContent = '登录异常，请重试';
        })
        .finally(() => {
            submitBtn.disabled = false;
            submitBtn.textContent = '登录';
        });
    }

    var loginCheckInterval = null;
    
    function startLoginCheck() {
        if (loginCheckInterval) return;
        loginCheckInterval = setInterval(function() {
            fetch('/check-login.ashx')
            .then(function(response) {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(function(data) {
                if (data.success && !data.isLoggedIn) {
                    window.ZR_CURRENT_MEMBER = null;
                    showLoginModal();
                    stopLoginCheck();
                } else if (data.success && data.isLoggedIn) {
                    window.ZR_CURRENT_MEMBER = {
                        userId: data.userId,
                        userName: data.userName,
                        shopId: data.shopId,
                        shopName: data.shopName
                    };
                }
            })
            .catch(function(error) {
                console.log('登录状态检查失败:', error);
            });
        }, 60000);
    }
    
    function stopLoginCheck() {
        if (loginCheckInterval) {
            clearInterval(loginCheckInterval);
            loginCheckInterval = null;
        }
    }
    
    startLoginCheck();
</script>