<%@ Page Language="C#" AutoEventWireup="true" CodeFile="register.aspx.cs" Inherits="register" Culture="zh-CN" UICulture="zh-CN" ResponseEncoding="utf-8" %>
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
    <script>
    window.ZrToast = function(message, type, duration) {
        type = type || 'info';
        duration = duration || 3000;
        var c = document.querySelector('.zr-toast-container');
        if (!c) {
            c = document.createElement('div');
            c.className = 'zr-toast-container';
            document.body.appendChild(c);
        }
        var toast = document.createElement('div');
        toast.className = 'zr-toast zr-toast-' + type;
        toast.innerHTML = '<span>' + message + '</span>';
        c.appendChild(toast);
        setTimeout(function() {
            toast.classList.add('toast-exit');
            setTimeout(function() { toast.remove(); }, 250);
        }, duration);
    };
    window.ZrToast.success = function(msg, dur) { ZrToast(msg, 'success', dur); };
    window.ZrToast.error = function(msg, dur) { ZrToast(msg, 'error', dur); };
    window.ZrToast.warning = function(msg, dur) { ZrToast(msg, 'warning', dur); };
    window.ZrToast.info = function(msg, dur) { ZrToast(msg, 'info', dur); };
    </script>
    <style>
        .register-panel {
            max-width: 720px;
            padding: 36px 44px;
        }
        .register-wizard {
            display: flex;
            justify-content: space-between;
            margin: 18px 0 28px;
            position: relative;
        }
        .register-wizard::before {
            content: '';
            position: absolute;
            top: 18px;
            left: 14%;
            right: 14%;
            height: 2px;
            background: linear-gradient(to right, #22c55e var(--progress, 0%), #e5e7eb var(--progress, 0%));
            z-index: 0;
        }
        .wizard-step {
            position: relative;
            z-index: 1;
            text-align: center;
            flex: 1;
        }
        .wizard-step .step-num {
            width: 36px;
            height: 36px;
            line-height: 36px;
            margin: 0 auto 6px;
            border-radius: 50%;
            background: #e5e7eb;
            color: #94a3b8;
            font-weight: 600;
            transition: all .25s;
        }
        .wizard-step.active .step-num {
            background: #22c55e;
            color: #fff;
            box-shadow: 0 0 0 4px rgba(34, 197, 94, 0.18);
        }
        .wizard-step.done .step-num {
            background: #22c55e;
            color: #fff;
        }
        .wizard-step .step-label {
            font-size: 13px;
            color: #64748b;
        }
        .wizard-step.active .step-label {
            color: #0f172a;
            font-weight: 500;
        }
        .register-section {
            display: none;
        }
        .register-section.active {
            display: block;
            animation: fadeIn .3s ease;
        }
        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(8px); }
            to { opacity: 1; transform: translateY(0); }
        }
        .role-selector {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 14px;
            margin: 16px 0 22px;
        }
        .role-card {
            border: 2px solid #e2e8f0;
            border-radius: 12px;
            padding: 20px 18px;
            cursor: pointer;
            transition: all .2s;
            background: #fafbfc;
        }
        .role-card:hover {
            border-color: #22c55e;
            background: #fff;
        }
        .role-card.selected {
            border-color: #22c55e;
            background: rgba(34, 197, 94, 0.05);
            box-shadow: 0 0 0 3px rgba(34, 197, 94, 0.12);
        }
        .role-card .role-icon {
            font-size: 28px;
            margin-bottom: 8px;
        }
        .role-card .role-title {
            font-size: 16px;
            font-weight: 600;
            color: #0f172a;
            margin-bottom: 4px;
        }
        .role-card .role-desc {
            font-size: 12px;
            color: #64748b;
            line-height: 1.5;
        }
        .input-with-icon {
            position: relative;
        }
        .input-with-icon .input {
            padding-right: 80px;
        }
        .input-with-icon .input-action {
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            color: #22c55e;
            cursor: pointer;
            font-size: 13px;
        }
        .password-strength {
            display: flex;
            gap: 4px;
            margin-top: 6px;
        }
        .password-strength .bar {
            height: 3px;
            flex: 1;
            background: #e5e7eb;
            border-radius: 2px;
            transition: background .2s;
        }
        .password-strength .bar.weak { background: #ef4444; }
        .password-strength .bar.medium { background: #f59e0b; }
        .password-strength .bar.strong { background: #22c55e; }
        .password-strength-text {
            font-size: 12px;
            margin-top: 4px;
            color: #94a3b8;
        }
        .password-strength-text.weak { color: #ef4444; }
        .password-strength-text.medium { color: #f59e0b; }
        .password-strength-text.strong { color: #22c55e; }
        .field-hint {
            font-size: 12px;
            color: #94a3b8;
            margin-top: 4px;
        }
        .field-hint.error {
            color: #ef4444;
        }
        .field-hint.success {
            color: #22c55e;
        }
        .input-feedback {
            display: inline-block;
            margin-left: 8px;
            font-size: 12px;
        }
        .input-feedback.ok { color: #22c55e; }
        .input-feedback.fail { color: #ef4444; }
        .checkbox-row {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 13px;
            color: #475569;
            margin: 8px 0;
        }
        .checkbox-row a { color: #22c55e; text-decoration: none; }
        .form-actions {
            display: flex;
            gap: 12px;
            margin-top: 24px;
        }
        .form-actions .btn {
            flex: 1;
            padding: 12px 0;
            font-size: 15px;
        }
        .register-success-state {
            text-align: center;
            padding: 30px 0;
        }
        .register-success-state .success-icon {
            width: 72px;
            height: 72px;
            margin: 0 auto 18px;
            background: rgba(34, 197, 94, 0.12);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 36px;
            color: #22c55e;
        }
        .register-success-state h3 {
            font-size: 20px;
            color: #0f172a;
            margin-bottom: 8px;
        }
        .register-success-state p {
            color: #64748b;
            margin-bottom: 6px;
        }
        .register-success-state .countdown {
            color: #22c55e;
            font-weight: 500;
        }
        .step-actions {
            display: flex;
            gap: 12px;
            margin-top: 20px;
        }
        .step-actions .btn-prev {
            flex: 0 0 100px;
        }
        .step-actions .btn-next {
            flex: 1;
        }
        @media (max-width: 600px) {
            .register-panel { padding: 24px 18px; }
            .role-selector { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <div class="app">
        <uc1:head runat="server" ID="head" />
        <main class="main">
            <header class="topbar">
                <div><h1>注册会员</h1></div>
                <div class="actions"><a class="btn back" href="/index.aspx" data-back>返回</a></div>
            </header>
            <section class="login-shell">
                <div class="login-panel-card register-panel">
                    <h2>注册新会员</h2>
                    <p class="register-tip">注册后可发布供需信息、参与询价报价。<em class="required-mark">*</em> 为必填字段</p>

                    <div class="register-wizard" id="registerWizard">
                        <div class="wizard-step active" data-step="1">
                            <div class="step-num">1</div>
                            <div class="step-label">选择角色</div>
                        </div>
                        <div class="wizard-step" data-step="2">
                            <div class="step-num">2</div>
                            <div class="step-label">账号信息</div>
                        </div>
                        <div class="wizard-step" data-step="3">
                            <div class="step-num">3</div>
                            <div class="step-label">企业资料</div>
                        </div>
                        <div class="wizard-step" data-step="4">
                            <div class="step-num">4</div>
                            <div class="step-label">注册成功</div>
                        </div>
                    </div>

                    <form id="registerForm" runat="server">
                        <asp:Panel ID="pnlRegister" runat="server" DefaultButton="btnRegister">

                            <!-- Step 1: 选择角色 -->
                            <div class="register-section active" data-section="1">
                                <h3 style="font-size:16px;color:#0f172a;margin-bottom:8px;">请选择您的身份</h3>
                                <p style="color:#64748b;font-size:13px;margin-bottom:16px;">不同身份对应不同的功能权限，注册后可在个人中心调整</p>
                                <div class="role-selector">
                                    <div class="role-card" data-role="buyer">
                                        <div class="role-icon">🛒</div>
                                        <div class="role-title">我是采购商</div>
                                        <div class="role-desc">发布采购需求、查看报价、管理订单</div>
                                    </div>
                                    <div class="role-card" data-role="seller">
                                        <div class="role-icon">🏪</div>
                                        <div class="role-title">我是供应商</div>
                                        <div class="role-desc">发布库存、响应询价、拓展客户</div>
                                    </div>
                                </div>
                                <asp:HiddenField ID="hfRole" runat="server" Value="buyer" />
                                <div class="step-actions">
                                    <button type="button" class="btn btn-prev" disabled>上一步</button>
                                    <button type="button" class="btn primary btn-next" data-next-step>下一步</button>
                                </div>
                            </div>

                            <!-- Step 2: 账号信息 -->
                            <div class="register-section" data-section="2">
                                <div class="form-row">
                                    <label>手机号 <em class="required-mark">*</em></label>
                                    <div class="input-with-icon">
                                        <asp:TextBox ID="txtMobilePhone" runat="server" CssClass="input" placeholder="11位手机号" MaxLength="11"></asp:TextBox>
                                    </div>
                                    <div class="field-hint" id="mobileHint">将作为您的登录账号</div>
                                </div>
                                <div class="form-row">
                                    <label>密码 <em class="required-mark">*</em></label>
                                    <div class="input-with-icon">
                                        <asp:TextBox ID="txtPassword" runat="server" CssClass="input" TextMode="Password" placeholder="6-20位字符" MaxLength="20"></asp:TextBox>
                                        <button type="button" class="input-action" data-toggle-pwd="txtPassword">显示</button>
                                    </div>
                                    <div class="password-strength" id="pwdStrength">
                                        <div class="bar"></div>
                                        <div class="bar"></div>
                                        <div class="bar"></div>
                                        <div class="bar"></div>
                                    </div>
                                    <div class="password-strength-text" id="pwdStrengthText">请输入密码</div>
                                </div>
                                <div class="form-row">
                                    <label>确认密码 <em class="required-mark">*</em></label>
                                    <div class="input-with-icon">
                                        <asp:TextBox ID="txtPassword2" runat="server" CssClass="input" TextMode="Password" placeholder="再次输入密码" MaxLength="20"></asp:TextBox>
                                        <button type="button" class="input-action" data-toggle-pwd="txtPassword2">显示</button>
                                    </div>
                                    <div class="field-hint" id="pwd2Hint">两次密码需保持一致</div>
                                </div>
                                <!-- 短信验证码（暂隐藏，保留代码以便后续使用）
                                <div class="form-row">
                                    <label>短信验证码 <em class="required-mark">*</em></label>
                                    <div style="display:flex;gap:8px;">
                                        <asp:TextBox ID="txtSmsCode" runat="server" CssClass="input" placeholder="6位验证码" MaxLength="6" style="flex:1;"></asp:TextBox>
                                        <button type="button" class="btn" id="btnSendCode" style="flex:0 0 120px;">获取验证码</button>
                                    </div>
                                    <div class="field-hint">完成上方验证后即可获取（演示版固定为 123456）</div>
                                </div>
                                -->
                                <div class="step-actions">
                                    <button type="button" class="btn btn-prev" data-prev-step>上一步</button>
                                    <button type="button" class="btn primary btn-next" data-next-step>下一步</button>
                                </div>
                            </div>

                            <!-- Step 3: 企业资料 -->
                            <div class="register-section" data-section="3">
                                <div class="form-row">
                                    <label>联系人 <em class="required-mark">*</em></label>
                                    <asp:TextBox ID="txtLinkMan" runat="server" CssClass="input" placeholder="您的姓名" MaxLength="20"></asp:TextBox>
                                </div>
                                <div class="form-row">
                                    <label>公司名称</label>
                                    <asp:TextBox ID="txtShopCompany" runat="server" CssClass="input" placeholder="选填，便于客户识别" MaxLength="60"></asp:TextBox>
                                </div>
                                <div class="form-row">
                                    <label>QQ</label>
                                    <asp:TextBox ID="txtQQ" runat="server" CssClass="input" placeholder="选填" MaxLength="15"></asp:TextBox>
                                </div>
                                <div class="form-row">
                                    <label>邮箱</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="input" placeholder="选填" MaxLength="60"></asp:TextBox>
                                </div>
                                <label class="privacy-check checkbox-row">
                                    <input type="checkbox" id="chkPrivacy" runat="server">
                                    <span>已阅读并同意 <a href="/about-us.aspx" target="_blank">《用户协议》</a> 与 <a href="/about-us.aspx" target="_blank">《隐私政策》</a></span>
                                </label>
                                <asp:Label ID="lblError" runat="server" CssClass="error-msg"></asp:Label>
                                <asp:Label ID="lblSuccess" runat="server" CssClass="success-msg"></asp:Label>
                                <div class="step-actions">
                                    <button type="button" class="btn btn-prev" data-prev-step>上一步</button>
                                    <asp:Button ID="btnRegister" runat="server" CssClass="btn primary btn-next" Text="立即注册" OnClick="btnRegister_Click" OnClientClick="return validateRegisterFormFinal();" />
                                </div>
                            </div>

                            <!-- Step 4: 注册成功 -->
                            <div class="register-section" data-section="4">
                                <div class="register-success-state">
                                    <div class="success-icon">✓</div>
                                    <h3>注册成功！</h3>
                                    <p>账号已创建，欢迎加入 <span style="color:#22c55e;font-weight:500;">阻容网</span></p>
                                    <p>使用您的手机号即可登录</p>
                                    <p style="margin-top:18px;font-size:13px;color:#94a3b8;">
                                        <span class="countdown" id="countdown">3</span> 秒后自动跳转，或
                                        <a href="/index.aspx" style="color:#22c55e;text-decoration:none;">立即进入首页</a>
                                    </p>
                                </div>
                            </div>

                        </asp:Panel>
                    </form>
                    <p class="login-link">已有账号？<a href="/login.aspx">立即登录</a></p>
                </div>
            </section>
        </main>
    </div>
    <uc1:bottom runat="server" ID="bottom" />
    <script>
    var currentStep = 1;
    var smsCooldown = 0;
    var smsInterval = null;

    function $(id) {
        return document.querySelector('[id$="' + id + '"]');
    }

    // 角色选择
    document.querySelectorAll('.role-card').forEach(function(card) {
        card.addEventListener('click', function() {
            document.querySelectorAll('.role-card').forEach(function(c) { c.classList.remove('selected'); });
            this.classList.add('selected');
            $('hfRole').value = this.dataset.role;
        });
    });
    document.querySelector('.role-card[data-role="buyer"]').classList.add('selected');

    // 步骤切换
    function goToStep(step) {
        if (step < 1 || step > 4) return;
        if (step > currentStep) {
            var result = validateCurrentStep(function(valid) {
                if (valid) {
                    doGoToStep(step);
                }
            });
            if (result === false) return;
            if (result === null) return;
        }
        doGoToStep(step);
    }

    function doGoToStep(step) {
        document.querySelectorAll('.register-section').forEach(function(sec) {
            sec.classList.toggle('active', parseInt(sec.dataset.section) === step);
        });
        document.querySelectorAll('.wizard-step').forEach(function(s) {
            var n = parseInt(s.dataset.step);
            s.classList.remove('active', 'done');
            if (n === step) s.classList.add('active');
            else if (n < step) s.classList.add('done');
        });
        var progress = ((step - 1) / 3) * 100;
        document.getElementById('registerWizard').style.setProperty('--progress', progress + '%');
        currentStep = step;
    }

    document.querySelectorAll('[data-next-step]').forEach(function(btn) {
        btn.addEventListener('click', function() { goToStep(currentStep + 1); });
    });
    document.querySelectorAll('[data-prev-step]').forEach(function(btn) {
        btn.addEventListener('click', function() { goToStep(currentStep - 1); });
    });

    // 密码显示/隐藏
    document.querySelectorAll('[data-toggle-pwd]').forEach(function(btn) {
        btn.addEventListener('click', function() {
            var id = this.dataset.togglePwd;
            var input = $(id);
            if (!input) return;
            if (input.type === 'password') {
                input.type = 'text';
                this.textContent = '隐藏';
            } else {
                input.type = 'password';
                this.textContent = '显示';
            }
        });
    });

    // 密码强度检测
    function checkPwdStrength(pwd) {
        var score = 0;
        if (pwd.length >= 6) score++;
        if (pwd.length >= 10) score++;
        if (/[A-Z]/.test(pwd) && /[a-z]/.test(pwd)) score++;
        if (/[0-9]/.test(pwd)) score++;
        if (/[^A-Za-z0-9]/.test(pwd)) score++;
        if (score <= 2) return { level: 0, text: '弱', cls: 'weak' };
        if (score <= 3) return { level: 1, text: '中等', cls: 'medium' };
        return { level: 2, text: '强', cls: 'strong' };
    }
    var pwdInput = $('txtPassword');
    if (pwdInput) {
        pwdInput.addEventListener('input', function() {
            var pwd = this.value;
            var bars = document.querySelectorAll('#pwdStrength .bar');
            var txt = document.getElementById('pwdStrengthText');
            bars.forEach(function(b) { b.className = 'bar'; });
            if (!pwd) { txt.textContent = '请输入密码'; txt.className = 'password-strength-text'; return; }
            var r = checkPwdStrength(pwd);
            var fillCount = r.level === 0 ? 1 : (r.level === 1 ? 2 : 4);
            for (var i = 0; i < fillCount; i++) bars[i].classList.add(r.cls);
            txt.textContent = '密码强度：' + r.text;
            txt.className = 'password-strength-text ' + r.cls;
        });
    }

    // 手机号实时验证
    var mobileInput = $('txtMobilePhone');
    if (mobileInput) {
        mobileInput.addEventListener('input', function() {
            var v = this.value.trim();
            var hint = document.getElementById('mobileHint');
            if (!v) { hint.textContent = '将作为您的登录账号'; hint.className = 'field-hint'; return; }
            if (/^1[3-9]\d{9}$/.test(v)) {
                hint.textContent = '✓ 手机号格式正确'; hint.className = 'field-hint success';
            } else {
                hint.textContent = '请输入正确的11位手机号'; hint.className = 'field-hint error';
            }
        });
    }

    // 确认密码实时验证
    var pwd2Input = $('txtPassword2');
    if (pwd2Input) {
        pwd2Input.addEventListener('input', function() {
            var hint = document.getElementById('pwd2Hint');
            if (!this.value) { hint.textContent = '两次密码需保持一致'; hint.className = 'field-hint'; return; }
            if (this.value === pwdInput.value) {
                hint.textContent = '✓ 两次密码一致'; hint.className = 'field-hint success';
            } else {
                hint.textContent = '两次密码不一致'; hint.className = 'field-hint error';
            }
        });
    }

    // 发送验证码
    var btnSendCode = document.getElementById('btnSendCode');
    if (btnSendCode) {
        btnSendCode.addEventListener('click', function() {
            var mobile = mobileInput.value.trim();
            if (!/^1[3-9]\d{9}$/.test(mobile)) {
                ZrToast.error('请先输入正确的手机号');
                mobileInput.focus();
                return;
            }
            if (smsCooldown > 0) return;
            ZrToast.success('验证码已发送（演示版：123456）');
            smsCooldown = 60;
            btnSendCode.disabled = true;
            smsInterval = setInterval(function() {
                smsCooldown--;
                btnSendCode.textContent = smsCooldown + 's 后重发';
                if (smsCooldown <= 0) {
                    clearInterval(smsInterval);
                    btnSendCode.disabled = false;
                    btnSendCode.textContent = '获取验证码';
                }
            }, 1000);
        });
    }

    var validatingMobile = false;

    function validateCurrentStep(callback) {
        if (currentStep === 1) {
            var hf = $('hfRole');
            if (!hf || !hf.value) {
                ZrToast.warning('请选择身份');
                if (callback) callback(false);
                return false;
            }
            if (callback) callback(true);
            return true;
        }
        if (currentStep === 2) {
            var mobile = mobileInput ? mobileInput.value.trim() : '';
            if (!/^1[3-9]\d{9}$/.test(mobile)) {
                ZrToast.error('请输入正确的11位手机号');
                if (mobileInput) mobileInput.focus();
                if (callback) callback(false);
                return false;
            }
            var pwd = pwdInput ? pwdInput.value : '';
            if (pwd.length < 6) {
                ZrToast.error('密码长度至少6个字符');
                if (pwdInput) pwdInput.focus();
                if (callback) callback(false);
                return false;
            }
            if (pwd2Input && pwd !== pwd2Input.value) {
                ZrToast.error('两次输入的密码不一致');
                pwd2Input.focus();
                if (callback) callback(false);
                return false;
            }
            // 短信验证码验证（暂跳过，对应隐藏的验证码输入框）
            // var codeInput = $('txtSmsCode');
            // var code = codeInput ? codeInput.value.trim() : '';
            // if (!/^\d{6}$/.test(code)) {
            //     ZrToast.error('请输入6位短信验证码');
            //     if (codeInput) codeInput.focus();
            //     if (callback) callback(false);
            //     return false;
            // }

            if (validatingMobile) {
                return false;
            }
            validatingMobile = true;
            
            var xhr = new XMLHttpRequest();
            xhr.open('GET', '/register.aspx?action=checkMobile&mobile=' + encodeURIComponent(mobile), true);
            xhr.onload = function() {
                validatingMobile = false;
                if (xhr.status === 200) {
                    try {
                        var result = JSON.parse(xhr.responseText);
                        if (result.exists) {
                            ZrToast.error('该手机号已注册，请直接登录');
                            if (mobileInput) mobileInput.focus();
                            if (callback) callback(false);
                        } else {
                            if (callback) callback(true);
                        }
                    } catch (e) {
                        ZrToast.error('验证失败，请重试');
                        if (callback) callback(false);
                    }
                } else {
                    ZrToast.error('验证失败，请重试');
                    if (callback) callback(false);
                }
            };
            xhr.onerror = function() {
                validatingMobile = false;
                ZrToast.error('网络错误，请重试');
                if (callback) callback(false);
            };
            xhr.send();
            return null;
        }
        if (callback) callback(true);
        return true;
    }

    // 最终提交验证
    function validateRegisterFormFinal() {
        var chk = $('chkPrivacy');
        if (!chk || !chk.checked) {
            ZrToast.warning('请阅读并同意用户协议与隐私政策');
            return false;
        }
        var linkManInput = $('txtLinkMan');
        var linkMan = linkManInput ? linkManInput.value.trim() : '';
        if (!linkMan) {
            ZrToast.error('请输入联系人姓名');
            if (linkManInput) linkManInput.focus();
            return false;
        }
        var btn = $('btnRegister');
        if (btn) { btn.value = '注册中...'; }
        return true;
    }

    // 注册成功后显示成功页
    function showSuccess() {
        goToStep(4);
        var n = 3;
        var cd = document.getElementById('countdown');
        var timer = setInterval(function() {
            n--;
            if (cd) cd.textContent = n;
            if (n <= 0) {
                clearInterval(timer);
                window.location.href = '/login.aspx';
            }
        }, 1000);
    }
    window.showRegisterSuccess = showSuccess;
    </script>
</body>
</html>
