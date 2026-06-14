// Toast 提示系统
(function() {
  // 创建 Toast 容器
  if (!document.querySelector('.zr-toast-container')) {
    const container = document.createElement('div');
    container.className = 'zr-toast-container';
    document.body.appendChild(container);
  }

  // Toast 函数
  window.ZrToast = function(message, type, duration) {
    type = type || 'info';
    duration = duration || 3000;
    
    const container = document.querySelector('.zr-toast-container');
    const toast = document.createElement('div');
    toast.className = 'zr-toast zr-toast-' + type;
    toast.innerHTML = '<span>' + message + '</span>';
    container.appendChild(toast);

    // 自动消失
    setTimeout(function() {
      toast.classList.add('toast-exit');
      setTimeout(function() { toast.remove(); }, 250);
    }, duration);
  };

  // 快捷方法
  window.ZrToast.success = function(msg, dur) { ZrToast(msg, 'success', dur); };
  window.ZrToast.error = function(msg, dur) { ZrToast(msg, 'error', dur); };
  window.ZrToast.warning = function(msg, dur) { ZrToast(msg, 'warning', dur); };
  window.ZrToast.info = function(msg, dur) { ZrToast(msg, 'info', dur); };
})();

// 表单验证辅助
function showFieldError(input, message) {
  input.classList.add('input-error');
  input.classList.remove('input-success');
  ZrToast.error(message);
  setTimeout(function() { input.classList.remove('input-error'); }, 2000);
}

function showFieldSuccess(input) {
  input.classList.add('input-success');
  input.classList.remove('input-error');
}

document.querySelectorAll('[data-publish-open]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const modal = document.getElementById('publishModal');
    if (!modal) return;
    modal.removeAttribute('hidden');
    const form = modal.querySelector('[data-quick-publish-form]');
    const lockedKind = btn.dataset.publishLockKind;
    const cannotPublishSupply = lockedKind === 'demand' || isNonSupplierContext();
    const defaultKind = cannotPublishSupply ? 'demand' : btn.dataset.publishDefault;
    const publishTitle = modal.querySelector('[data-publish-title]');
    if (publishTitle) publishTitle.textContent = lockedKind === 'demand' ? '发布采购' : (defaultKind === 'demand' ? '发布需求' : '发布供应');
    modal.classList.toggle('is-buyer-publish', lockedKind === 'demand');
    modal.classList.toggle('cannot-publish-supply', cannotPublishSupply);
    if (form && defaultKind) {
      form.querySelectorAll('[data-publish-kind]').forEach((item) => item.classList.remove('active'));
      form.querySelector(`[data-publish-kind="${defaultKind}"]`)?.classList.add('active');
      const defaultPart = btn.dataset.publishPartDefault;
      if (defaultPart) {
        form.querySelectorAll('[data-part-type]').forEach((item) => item.classList.remove('active'));
        form.querySelector(`[data-part-type="${defaultPart}"]`)?.classList.add('active');
        renderQuickPublishAttrs(form);
      }
      const qtyLabel = form.querySelector('[data-qty-label]');
      if (qtyLabel) qtyLabel.textContent = defaultKind === 'supply' ? '可供数量' : '需求数量';
      setDefaultValidity(form, defaultKind);
    } else if (form) {
      const kind = form.querySelector('[data-publish-kind].active')?.dataset.publishKind || 'supply';
      setDefaultValidity(form, kind);
    }
  });
});

document.querySelectorAll('table').forEach((table) => {
  const labels = [...table.querySelectorAll('thead th')].map((th) => th.textContent.trim().replace(/\s*↕\s*/g, ''));
  table.querySelectorAll('tbody tr').forEach((row) => {
    [...row.children].forEach((cell, index) => {
      if (!cell.hasAttribute('data-label') && labels[index]) cell.setAttribute('data-label', labels[index]);
    });
  });
});

function updateAdminAdStatus(card) {
  const endInput = card.querySelector('[data-ad-end]');
  const status = card.querySelector('[data-ad-status]');
  const endDate = endInput?.value || status?.dataset.adEnd || '';
  if (!status || !endDate) return;
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const expire = new Date(`${endDate}T00:00:00`);
  const expired = expire < today;
  const title = card.querySelector('b')?.textContent?.trim() || '广告位';
  card.classList.toggle('is-paused', expired);
  status.textContent = expired ? `${title} · 已暂停（已到期）` : `${title} · 待开启`;
}
document.querySelectorAll('[data-ad-card]').forEach((card) => {
  updateAdminAdStatus(card);
  card.querySelector('[data-ad-end]')?.addEventListener('change', () => updateAdminAdStatus(card));
});

const AD_SLOT_DEFAULTS = {
  'SR-D01': { tier: 'diamond', page: '搜索结果页', position: '搜索框前 1', format: '图文', start: '2026-06-13', end: '2026-07-13' },
  'SR-D02': { tier: 'diamond', page: '搜索结果页', position: '搜索框前 2', format: '图文', start: '2026-06-13', end: '2026-07-13' },
  'SR-G01': { tier: 'gold', page: '搜索结果页', position: '搜索框后 1', format: '文字', start: '2026-06-13', end: '2026-07-13' },
  'SR-G02': { tier: 'gold', page: '搜索结果页', position: '搜索框后 2', format: '文字', start: '2026-06-13', end: '2026-07-13' },
  'HOME-D01': { tier: 'diamond', page: '首页广场', position: '搜索栏下方', format: '图文', start: '2026-06-13', end: '2026-07-13' },
  'HOME-G01': { tier: 'gold', page: '首页广场', position: '信息流中段', format: '文字', start: '2026-06-13', end: '2026-07-13' },
  'SUP-G01': { tier: 'gold', page: '供应详情', position: '供应商侧栏下方', format: '图文', start: '2026-06-13', end: '2026-07-13' },
  'DEM-G01': { tier: 'gold', page: '需求详情', position: '采购商侧栏下方', format: '图文', start: '2026-06-13', end: '2026-07-13' },
  'MW-S01': { tier: 'silver', page: '商家工作台', position: '搜索栏上方', format: '文字', start: '2026-06-13', end: '2026-07-13' },
  'BW-S01': { tier: 'silver', page: '采购工作台', position: '数据概览下方', format: '文字', start: '2026-06-13', end: '2026-07-13' },
  'HELP-S01': { tier: 'silver', page: '帮助/关于页', position: '页面底部', format: '文字', start: '2026-06-13', end: '2026-07-13' }
};
const getAdStorageKey = (slot) => `zr-ad-slot-${slot}`;
const getAdDateStorageKey = (slot, type) => `zr-ad-slot-${slot}-${type}`;
function getAdDateConfig(slot, element) {
  const defaults = AD_SLOT_DEFAULTS[slot] || {};
  return {
    start: localStorage.getItem(getAdDateStorageKey(slot, 'start')) || element?.dataset.adStart || defaults.start || '',
    end: localStorage.getItem(getAdDateStorageKey(slot, 'end')) || element?.dataset.adEnd || defaults.end || ''
  };
}
function isAdSlotInDateWindow(slot, element) {
  const { start, end } = getAdDateConfig(slot, element);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  if (start && new Date(`${start}T00:00:00`) > today) return false;
  if (end && new Date(`${end}T00:00:00`) < today) return false;
  return true;
}
function isAdSlotActive(slot) {
  return localStorage.getItem(getAdStorageKey(slot)) === 'on' && isAdSlotInDateWindow(slot);
}
function paintAdToggle(toggle, active) {
  const slot = toggle.dataset.adSlot;
  const storedOn = localStorage.getItem(getAdStorageKey(slot)) === 'on';
  const inDate = isAdSlotInDateWindow(slot, toggle);
  if (storedOn && !inDate) {
    toggle.textContent = '已到期';
    toggle.classList.remove('green');
    toggle.classList.add('gray');
    return;
  }
  toggle.textContent = active ? '启用' : '关停';
  toggle.classList.toggle('green', active);
  toggle.classList.toggle('gray', !active);
}
function applySearchAdVisibility() {
  document.querySelectorAll('.search-ad-panel, .site-ad-panel').forEach((panel) => {
    const cards = [...panel.querySelectorAll('[data-ad-slot].search-ad-card')];
    if (!cards.length) return;
    let activeCount = 0;
    cards.forEach((card) => {
      const active = localStorage.getItem(getAdStorageKey(card.dataset.adSlot)) === 'on' && isAdSlotInDateWindow(card.dataset.adSlot, card);
      card.hidden = !active;
      if (active) activeCount += 1;
    });
    panel.hidden = activeCount === 0;
  });
}
document.querySelectorAll('[data-ad-state-toggle]').forEach((toggle) => {
  const slot = toggle.dataset.adSlot;
  paintAdToggle(toggle, isAdSlotActive(slot));
  const toggleState = () => {
    const nextActive = localStorage.getItem(getAdStorageKey(slot)) !== 'on';
    localStorage.setItem(getAdStorageKey(slot), nextActive ? 'on' : 'off');
    paintAdToggle(toggle, nextActive && isAdSlotInDateWindow(slot, toggle));
    applySearchAdVisibility();
  };
  toggle.addEventListener('click', toggleState);
  toggle.addEventListener('keydown', (event) => {
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault();
      toggleState();
    }
  });
});
document.querySelectorAll('[data-ad-card][data-ad-slot]').forEach((card) => {
  const slot = card.dataset.adSlot;
  const start = card.querySelector('[data-ad-start]');
  const end = card.querySelector('[data-ad-end]');
  const config = getAdDateConfig(slot, card);
  if (start && config.start) start.value = config.start;
  if (end && config.end) end.value = config.end;
  [start, end].forEach((input) => {
    input?.addEventListener('change', () => {
      const type = input.matches('[data-ad-start]') ? 'start' : 'end';
      localStorage.setItem(getAdDateStorageKey(slot, type), input.value);
      document.querySelectorAll(`[data-ad-state-toggle][data-ad-slot="${slot}"]`).forEach((toggle) => paintAdToggle(toggle, isAdSlotActive(slot)));
      updateAdminAdStatus(card);
      applySearchAdVisibility();
    });
  });
});
applySearchAdVisibility();

function createAdDraftCard(index) {
  const article = document.createElement('article');
  article.className = 'admin-ad-card is-paused';
  article.setAttribute('data-ad-card', '');
  article.innerHTML = `
    <span>新增广告位 ${index}</span>
    <label>广告位置<select class="select"><option>搜索页顶部</option><option>首页信息流间</option><option>供应详情侧栏</option><option>需求详情侧栏</option><option>工作台顶部</option></select></label>
    <label>广告类型<select class="select" data-ad-type><option>文字广告</option><option>图片广告</option></select></label>
    <label>显示内容<textarea class="input" data-ad-content>这个位置很适合您的产品！</textarea></label>
    <label>图片广告<input class="input" type="file" accept="image/*" data-ad-image></label>
    <label>链接地址<input class="input" value="search.html" data-ad-link></label>
    <div class="admin-ad-dates"><label>开始日期<input class="input" type="date" data-ad-start></label><label>到期日期<input class="input" type="date" data-ad-end></label></div>
    <small>草稿 · 未开启，不会在前台显示</small>
    <button class="btn mini" data-ui-toast="广告草稿已保存">保存广告</button>
  `;
  return article;
}
document.querySelectorAll('[data-ad-create]').forEach((button) => {
  button.addEventListener('click', () => {
    const grid = document.querySelector('.admin-ad-grid');
    if (!grid) return;
    const index = grid.querySelectorAll('[data-ad-card]').length + 1;
    grid.appendChild(createAdDraftCard(index));
    grid.lastElementChild?.scrollIntoView({ behavior: 'smooth', block: 'center' });
  });
});

const SEARCH_MAX_RESULTS = 25 * 20;
const PACKAGE_ALIASES = {
  '0201': ['0201', '0603metric'],
  '0402': ['0402', '1005', '1005metric'],
  '0603': ['0603', '1608', '1608metric'],
  '0805': ['0805', '2012', '2012metric'],
  '1206': ['1206', '3216', '3216metric'],
  '1210': ['1210', '3225', '3225metric'],
  '1812': ['1812', '4532', '4532metric'],
  '2220': ['2220', '5750', '5750metric']
};
const TOLERANCE_ALIASES = {
  f: ['f', '1%', '±1%'],
  j: ['j', '5%', '±5%'],
  k: ['k', '10%', '±10%'],
  m: ['m', '20%', '±20%']
};
function normalizeParamText(text) {
  return String(text || '')
    .toLowerCase()
    .replace(/μ/g, 'u')
    .replace(/欧姆|Ω|ω/g, 'ohm')
    .replace(/±\s*/g, '±')
    .replace(/([0-9])\s*([a-z]+)/g, '$1$2')
    .replace(/([a-z])([0-9])/g, '$1 $2')
    .replace(/([0-9]+(?:\.[0-9]+)?)(v|kv|mv|pf|nf|uf|ohm|kohm|mohm|k(?![a-z])|m(?![a-z]))/g, ' $1$2 ')
    .replace(/([a-z]+)([0-9]+(?:\.[0-9]+)?)/g, ' $1 $2')
    .replace(/[·•・,，\/|+()（）\[\]【】_-]+/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}
function trimNumber(value) {
  return Number(value).toFixed(6).replace(/\.?0+$/, '');
}
function formatCapacitanceVariants(pfValue) {
  const variants = new Set();
  if (!Number.isFinite(pfValue) || pfValue <= 0) return [];
  variants.add(`${trimNumber(pfValue)}pf`);
  if (pfValue >= 1000) variants.add(`${trimNumber(pfValue / 1000)}nf`);
  if (pfValue >= 1000000) variants.add(`${trimNumber(pfValue / 1000000)}uf`);
  if (pfValue < 1000) variants.add(`${trimNumber(pfValue / 1000)}nf`);
  if (pfValue < 1000000) variants.add(`${trimNumber(pfValue / 1000000)}uf`);
  const exp = Math.floor(Math.log10(pfValue));
  const base = pfValue / (10 ** Math.max(exp - 1, 0));
  if (Number.isInteger(base) && base >= 10 && base <= 99 && exp >= 1) variants.add(`${base}${exp - 1}`);
  return [...variants];
}
function capacitanceTokenToPf(token) {
  const cap = token.match(/^([0-9]+(?:\.[0-9]+)?)(pf|nf|uf)$/);
  if (cap) {
    const value = Number(cap[1]);
    if (cap[2] === 'pf') return value;
    if (cap[2] === 'nf') return value * 1000;
    if (cap[2] === 'uf') return value * 1000000;
  }
  const code = token.match(/^([0-9]{3})(?:$|[a-z]{2,}$)/);
  if (code) {
    const firstTwo = Number(code[1].slice(0, 2));
    const multiplier = Number(code[1][2]);
    if (firstTwo > 0 && multiplier <= 9) return firstTwo * (10 ** multiplier);
  }
  return null;
}
function expandSearchToken(token) {
  const clean = token.replace(/[^a-z0-9.%]/g, '');
  const set = new Set([clean]);
  Object.values(PACKAGE_ALIASES).forEach((aliases) => {
    if (aliases.includes(clean)) aliases.forEach((alias) => set.add(alias));
  });
  Object.entries(TOLERANCE_ALIASES).forEach(([letter, aliases]) => {
    if (aliases.includes(clean)) aliases.forEach((alias) => set.add(alias));
    if (clean.endsWith(letter) && clean.length > 1) aliases.forEach((alias) => set.add(clean.replace(new RegExp(`${letter}$`), alias)));
  });
  const capPf = capacitanceTokenToPf(clean);
  if (capPf) formatCapacitanceVariants(capPf).forEach((variant) => set.add(variant));
  const resistor = clean.match(/^([0-9]+(?:\.[0-9]+)?)(k|m|ohm|kohm|mohm)$/);
  if (resistor) {
    const value = Number(resistor[1]);
    if (resistor[2] === 'k' || resistor[2] === 'kohm') {
      set.add(`${trimNumber(value)}k`);
      set.add(`${trimNumber(value)}kohm`);
      set.add(`${trimNumber(value * 1000)}ohm`);
    }
    if (resistor[2] === 'm' || resistor[2] === 'mohm') {
      set.add(`${trimNumber(value)}m`);
      set.add(`${trimNumber(value)}mohm`);
      set.add(`${trimNumber(value * 1000000)}ohm`);
    }
  }
  if (clean === '50v') set.add('50 v');
  return [...set].filter(Boolean);
}
function getSearchTokens(text) {
  const normalized = normalizeParamText(text);
  const basicTokens = normalized.split(/\s+/).filter(Boolean);
  const compactTokens = [...String(text || '').toLowerCase().replace(/μ/g, 'u').matchAll(/[0-9]+(?:\.[0-9]+)?(?:pf|nf|uf|v|kv|mv|ohm|kohm|mohm|k(?![a-z])|m(?![a-z]))|[0-9]{3,}[a-z]*/g)].map((match) => match[0]);
  return [...new Set([...basicTokens, ...compactTokens])].filter(Boolean);
}
function itemMatchesSearch(item, queryTokens) {
  if (!queryTokens.length) return true;
  const haystack = ` ${normalizeParamText(item.textContent)} ${getSearchTokens(item.textContent).join(' ')} `;
  return queryTokens.every((token) => expandSearchToken(token).some((candidate) => haystack.includes(` ${normalizeParamText(candidate)} `) || haystack.includes(normalizeParamText(candidate))));
}
function runPageSearch() {
  const input = document.querySelector('[data-page-search-input]');
  const query = (input?.value || '').trim();
  const queryTokens = getSearchTokens(query);
  const items = [...document.querySelectorAll('.search-results-panel .item')];
  let visibleCount = 0;
  items.forEach((item) => {
    const matched = itemMatchesSearch(item, queryTokens);
    const withinLimit = visibleCount < SEARCH_MAX_RESULTS;
    item.hidden = !(matched && withinLimit);
    if (matched) visibleCount += 1;
  });
  const renderedCount = Math.min(visibleCount, SEARCH_MAX_RESULTS);
  const summary = document.querySelector('.search-results-panel .section-title p');
  if (summary) summary.textContent = query ? `找到 ${renderedCount} 条匹配信息${visibleCount > SEARCH_MAX_RESULTS ? '，已按最新发布显示前 20 页' : ''}。` : '找到 126 条匹配信息，优先展示有效期内和资料完善公司。';
  const panel = document.querySelector('.search-results-panel');
  let empty = panel?.querySelector('[data-search-empty]');
  if (panel && !empty) {
    empty = document.createElement('div');
    empty.className = 'empty-state';
    empty.setAttribute('data-search-empty', '');
    empty.textContent = '找不到哦！这样吧，请您发布一条！';
    panel.querySelector('.card-list')?.after(empty);
  }
  if (empty) empty.hidden = renderedCount !== 0;
  document.querySelector('.search-results-panel .pagination')?._updateBasicPagination?.(1);
}
document.querySelector('[data-page-search-submit]')?.addEventListener('click', runPageSearch);
document.querySelector('[data-page-search-input]')?.addEventListener('keydown', (event) => {
  if (event.key === 'Enter') runPageSearch();
});
function navigateToSearchFrom(input) {
  const query = (input?.value || '').trim();
  const target = query ? `search.html?q=${encodeURIComponent(query)}` : 'search.html';
  window.location.href = target;
}
document.querySelectorAll('.market-toolbar a[href="search.html"]').forEach((link) => {
  link.addEventListener('click', (event) => {
    const input = link.closest('.searchbar')?.querySelector('input');
    if (!input) return;
    event.preventDefault();
    navigateToSearchFrom(input);
  });
});
document.querySelectorAll('.market-toolbar input[placeholder^="搜索"]').forEach((input) => {
  input.addEventListener('keydown', (event) => {
    if (event.key === 'Enter') {
      event.preventDefault();
      navigateToSearchFrom(input);
    }
  });
});
const pageSearchInput = document.querySelector('[data-page-search-input]');
if (pageSearchInput) {
  const initialQuery = new URLSearchParams(window.location.search).get('q') || '';
  if (initialQuery) {
    pageSearchInput.value = initialQuery;
    runPageSearch();
  }
}
document.querySelectorAll('[data-quote-search], [data-inquiry-search], [data-record-search], [data-list-search], [data-admin-member-search], [data-admin-table-search], [data-admin-supply-search]').forEach((input) => {
  input.addEventListener('keydown', (event) => {
    if (event.key !== 'Enter') return;
    event.preventDefault();
    input.dispatchEvent(new Event('input', { bubbles: true }));
  });
});

document.querySelectorAll('[data-feedback-open]').forEach((link) => {
  link.addEventListener('click', (event) => {
    event.preventDefault();
    const modal = document.getElementById('feedbackModal');
    modal?.removeAttribute('hidden');
    modal?.querySelector('[data-feedback-content]')?.focus();
  });
});
document.querySelectorAll('[data-feedback-close]').forEach((btn) => {
  btn.addEventListener('click', () => {
    document.getElementById('feedbackModal')?.setAttribute('hidden', '');
  });
});
document.querySelectorAll('[data-feedback-submit]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const modal = document.getElementById('feedbackModal');
    const form = modal?.querySelector('[data-feedback-form]');
    const name = form?.querySelector('[data-feedback-name]');
    const contact = form?.querySelector('[data-feedback-contact]');
    const content = form?.querySelector('[data-feedback-content]');
    if (!name?.value.trim()) {
      markInteractionInputError(name);
      return;
    }
    if (!contact?.value.trim()) {
      markInteractionInputError(contact);
      return;
    }
    if (!content?.value.trim()) {
      markInteractionInputError(content);
      return;
    }
    form?.reset();
    modal?.setAttribute('hidden', '');
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = '留言已提交，我们会尽快处理';
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1400);
  });
});
document.querySelectorAll('[data-quick-import-open]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const modal = document.getElementById('quickImportModal');
    if (!modal) return;
    modal.removeAttribute('hidden');
    modal._quickImportRows = [];
    modal._quickImportPage = 1;
    modal.querySelector('[data-quick-preview-body]')?.replaceChildren();
    const pasteText = modal.querySelector('[data-quick-paste-text]');
    if (pasteText) pasteText.value = '';
    modal.querySelector('[data-quick-paste-panel]')?.removeAttribute('hidden');
    modal.querySelector('[data-quick-preview-panel]')?.setAttribute('hidden', '');
    const count = modal.querySelector('[data-quick-import-count]');
    if (count) count.textContent = '';
    modal.querySelectorAll('[data-quick-kind]').forEach((btn) => btn.classList.remove('active'));
    modal.querySelector(`[data-quick-kind="${isNonSupplierContext() ? 'demand' : 'supply'}"]`)?.classList.add('active');
    applyRolePublishVisibility();
    setQuickImportTax(modal, true);
  });
});
document.querySelectorAll('[data-quick-import-close]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const modal = document.getElementById('quickImportModal');
    if (modal) modal.setAttribute('hidden', '');
  });
});
document.querySelectorAll('[data-publish-close]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const modal = document.getElementById('publishModal');
    if (modal) modal.setAttribute('hidden', '');
  });
});
document.querySelectorAll('.modal-backdrop').forEach((mask) => {
  mask.addEventListener('click', (e) => {
    if (e.target === mask) mask.setAttribute('hidden', '');
  });
});
document.addEventListener('keydown', (e) => {
  if (e.key !== 'Escape') return;
  document.querySelectorAll('.modal-backdrop').forEach((m) => {
    if (m.id === 'tradeInteractionModal') resetTradeInteractionModal(m);
    m.setAttribute('hidden', '');
  });
});

document.querySelectorAll('a.btn.back').forEach((link) => {
  link.addEventListener('click', (event) => {
    event.preventDefault();
    const fallbackUrl = link.getAttribute('href') || 'index.html';
    window.location.href = fallbackUrl;
  });
});

document.addEventListener('click', (event) => {
  const policyLink = event.target.closest('[data-policy-link]');
  if (policyLink) event.preventDefault();
});

const LOGIN_GATE_ENABLED = false;

function getCurrentMember() {
  return window.ZR_CURRENT_MEMBER || null;
}

function isCurrentMemberLoggedIn() {
  return Boolean(getCurrentMember());
}

function getCurrentMemberRole() {
  return getCurrentMember()?.role || '';
}

function isSupplierRole(role = getCurrentMemberRole()) {
  return /供应商/.test(role);
}

function isBuyerOnlyPage() {
  const page = location.pathname.split('/').pop();
  return ['buyer-workbench.html', 'received-quotes.html', 'demand-detail.html'].includes(page);
}

function isMerchantWorkbenchPage() {
  const page = location.pathname.split('/').pop();
  return page === 'merchant-workbench.aspx';
}

function isNonSupplierContext() {
  if (isMerchantWorkbenchPage()) return false;
  return (isCurrentMemberLoggedIn() && !isSupplierRole()) || isBuyerOnlyPage();
}

function guardWorkbenchAccess() {
  if (!LOGIN_GATE_ENABLED) return;
  const page = location.pathname.split('/').pop();
  if (!['merchant-workbench.html', 'buyer-workbench.html'].includes(page)) return;
  if (!isCurrentMemberLoggedIn()) {
    location.replace(`login.html?redirect=${encodeURIComponent(page)}`);
    return;
  }
  if (page === 'merchant-workbench.html' && !isSupplierRole()) {
    location.replace('buyer-workbench.html?denied=merchant');
  }
}

guardWorkbenchAccess();

function syncMemberWorkspaceNav() {
  document.querySelectorAll('.nav a[href="login.html"]').forEach((link) => {
    const label = link.querySelector('span') || link;
    if (!isCurrentMemberLoggedIn()) {
      label.textContent = '登录';
      link.setAttribute('href', 'login.html');
      return;
    }
    const member = getCurrentMember();
    label.textContent = member?.companyName || member?.company || member?.company_name || '请补充完整公司名';
    link.setAttribute('href', isSupplierRole() ? 'merchant-workbench.html' : 'buyer-workbench.html');
  });
}

syncMemberWorkspaceNav();

function applyRolePublishVisibility() {
  if (!isNonSupplierContext()) return;
  document.querySelectorAll('[data-publish-open][data-publish-default="supply"]').forEach((button) => {
    button.hidden = true;
  });
  document.querySelectorAll('[data-publish-kind="supply"], [data-quick-kind="supply"]').forEach((button) => {
    button.hidden = true;
    button.classList.remove('active');
  });
  document.querySelectorAll('[data-publish-kind="demand"], [data-quick-kind="demand"]').forEach((button) => {
    button.classList.add('active');
  });
}

applyRolePublishVisibility();

document.querySelectorAll('[data-merchant-stat="online"]').forEach((item) => {
  item.addEventListener('click', () => focusPanel(item.closest('main')?.querySelector('.panel:nth-of-type(2)')));
});

function ensureAuthModal() {
  let modal = document.getElementById('authLoginModal');
  if (modal) return modal;
  modal = document.createElement('div');
  modal.className = 'modal-backdrop auth-login-backdrop';
  modal.id = 'authLoginModal';
  modal.hidden = true;
  modal.innerHTML = `
    <div class="modal auth-login-modal" role="dialog" aria-modal="true" aria-label="登录后继续">
      <div class="modal-head"><h2>登录后继续</h2><button class="modal-close" type="button" data-auth-close aria-label="关闭">×</button></div>
      <div class="modal-body">
        <div class="auth-login-card">
          <p>发送询价或提交报价前需要先登录。登录完成后，请再次点击原按钮提交。</p>
          <div class="auth-login-options"><button class="active" type="button" data-auth-method="qq">QQ 登录</button><button type="button" data-auth-method="wechat">微信登录</button><button type="button" data-auth-method="sms">手机验证码</button></div>
          <div class="auth-login-hint" data-auth-hint>当前选择 QQ 登录。</div>
          <label class="privacy-check"><input type="checkbox" data-auth-privacy><span>已阅读并同意 <a href="#" data-policy-link>《隐私政策》</a></span></label>
          <button class="btn primary auth-login-submit" type="button" data-auth-login>完成登录</button>
        </div>
      </div>
    </div>`;
  document.body.appendChild(modal);
  return modal;
}

function openAuthModal() {
  const modal = ensureAuthModal();
  modal.removeAttribute('hidden');
}

function isLoginRequiredSubmit(target) {
  const button = target.closest('button');
  if (!button) return false;
  if (button.matches('[data-interaction-submit]')) return true;
  const text = button.textContent.trim();
  const feedbackText = button.getAttribute('data-ui-toast') || '';
  return ['发送询价', '提交报价'].includes(text) || /询价已发送|报价已提交/.test(feedbackText);
}

document.addEventListener('click', (event) => {
  if (!LOGIN_GATE_ENABLED) return;
  if (!isLoginRequiredSubmit(event.target) || isCurrentMemberLoggedIn()) return;
  event.preventDefault();
  event.stopPropagation();
  event.stopImmediatePropagation();
  openAuthModal();
}, true);

document.addEventListener('click', (event) => {
  const modal = document.getElementById('authLoginModal');
  if (!modal) return;
  if (event.target === modal || event.target.closest('[data-auth-close]')) {
    modal.setAttribute('hidden', '');
    return;
  }
  const method = event.target.closest('[data-auth-method]');
  if (method) {
    modal.querySelectorAll('[data-auth-method]').forEach((btn) => btn.classList.toggle('active', btn === method));
    const hint = modal.querySelector('[data-auth-hint]');
    if (hint) hint.textContent = `当前选择 ${method.textContent.trim()}。`;
    return;
  }
  const login = event.target.closest('[data-auth-login]');
  if (!login) return;
  const privacy = modal.querySelector('[data-auth-privacy]');
  if (!privacy?.checked) {
    privacy?.closest('.privacy-check')?.classList.add('input-error-shake');
    setTimeout(() => privacy?.closest('.privacy-check')?.classList.remove('input-error-shake'), 480);
    return;
  }
  window.location.href = `login.html?redirect=${encodeURIComponent(location.pathname.split('/').pop() || 'index.html')}`;
});

function normalizeSearchInputValue(value) {
  return String(value || '');
}

function isSearchLikeInput(input) {
  return input instanceof HTMLInputElement && input.matches('input[placeholder^="搜索"], [data-page-search-input], [data-inventory-search], [data-quote-search], [data-inquiry-search], [data-record-search], [data-list-search], [data-admin-member-search], [data-admin-table-search], [data-admin-supply-search]');
}

function normalizeNonNegativeNumber(value) {
  const cleaned = String(value || '').replace(/[^\d.]/g, '');
  const parts = cleaned.split('.');
  return parts.length > 1 ? `${parts.shift()}.${parts.join('')}` : cleaned;
}

function normalizeTradePriceInput(value) {
  const cleaned = normalizeNonNegativeNumber(value);
  if (!cleaned) return '';
  const [integer = '', decimal = ''] = cleaned.split('.');
  const normalizedInteger = integer.replace(/^0+(?=\d)/, '') || (cleaned.startsWith('.') ? '0' : integer);
  const normalizedDecimal = decimal ? decimal.slice(0, 4) : '';
  return cleaned.includes('.') ? `${normalizedInteger || '0'}.${normalizedDecimal}` : normalizedInteger;
}

function isValidTradePrice(value) {
  const text = String(value || '').trim();
  if (text === '') return true;
  if (!/^(?:[1-9]\d*|0)(?:\.\d{1,4})?$/.test(text)) return false;
  return Number(text) >= 0.0001;
}

function isPriceInput(input) {
  if (!(input instanceof HTMLInputElement)) return false;
  if (input.readOnly || input.disabled || input.matches('[data-lock-once], [data-bind-contact], [data-required-address]')) return false;
  if (isSearchLikeInput(input)) return false;
  const text = `${input.getAttribute('data-required') || ''} ${input.placeholder || ''} ${input.closest('label')?.textContent || ''}`;
  return input.matches('.price-input, [data-interaction-price]') || /价格|单价|报价|期望价/.test(text);
}

function normalizePositiveQuantity(value) {
  const digits = String(value || '').replace(/[^\d]/g, '');
  const withoutLeadingZero = digits.replace(/^0+/, '');
  if (!withoutLeadingZero) return '';
  return withoutLeadingZero;
}

function isQuantityInput(input) {
  if (!(input instanceof HTMLInputElement)) return false;
  if (['checkbox', 'radio', 'button', 'submit', 'reset', 'hidden'].includes(input.type)) return false;
  if (input.readOnly || input.disabled || input.matches('[data-lock-once], [data-bind-contact], [data-required-address]')) return false;
  if (isSearchLikeInput(input)) return false;
  const text = `${input.getAttribute('data-required') || ''} ${input.placeholder || ''} ${input.closest('label')?.textContent || ''}`;
  return input.matches('[data-interaction-qty], [data-quote-qty], .qty-input') || /数量|库存|现货|需求数量|可供数量|采购数量/.test(text);
}

function ensureSearchDatalist() {
  let list = document.getElementById('zrSearchSuggestions');
  if (list) return list;
  list = document.createElement('datalist');
  list.id = 'zrSearchSuggestions';
  list.innerHTML = ['GRM188R71H104KA93D', 'CL10B104KB8NNNC', 'RC0603FR-0710KL', 'RC0402FR-0710KL', '100K 0603', '0603 100nF X7R', '村田 Murata', '三星 Samsung', '国巨 Yageo', '品牌不限', '深圳市华南电子有限公司', '东莞智造电子科技有限公司'].map((value) => `<option value="${value}"></option>`).join('');
  document.body.appendChild(list);
  return list;
}

function attachSearchAutocomplete() {
  const list = ensureSearchDatalist();
  document.querySelectorAll('input[placeholder^="搜索"], [data-page-search-input], [data-inventory-search], [data-quote-search], [data-inquiry-search], [data-record-search], [data-list-search], [data-admin-member-search], [data-admin-table-search], [data-admin-supply-search]').forEach((input) => {
    if (input instanceof HTMLInputElement) input.setAttribute('list', list.id);
  });
}

attachSearchAutocomplete();

document.querySelectorAll('.price-input, [data-interaction-price]').forEach((input) => {
  input.setAttribute('inputmode', 'decimal');
  input.setAttribute('min', '0.0001');
  input.setAttribute('step', '0.0001');
});

document.addEventListener('input', (event) => {
  if (event.isComposing) return;
  const input = event.target;
  if (!(input instanceof HTMLInputElement)) return;
  if (!isSearchLikeInput(input)) return;
  input.classList.remove('input-error', 'shake-error', 'input-error-shake');
  const nextValue = normalizeSearchInputValue(input.value);
  if (nextValue === input.value) return;
  const start = input.selectionStart;
  const end = input.selectionEnd;
  input.value = nextValue;
  input.setSelectionRange(start, end);
}, true);

document.addEventListener('focusin', (event) => {
  const input = event.target;
  if (isSearchLikeInput(input)) {
    input.classList.remove('input-error', 'shake-error', 'input-error-shake');
  }
});

document.addEventListener('change', (event) => {
  const input = event.target;
  if (isSearchLikeInput(input)) {
    input.classList.remove('input-error', 'shake-error', 'input-error-shake');
  }
}, true);

document.addEventListener('input', (event) => {
  if (event.isComposing) return;
  const input = event.target;
  if (!(input instanceof HTMLInputElement)) return;
  if (!isQuantityInput(input) && !isPriceInput(input)) return;
  const nextValue = isQuantityInput(input) ? normalizePositiveQuantity(input.value) : normalizeTradePriceInput(input.value);
  if (nextValue === input.value) return;
  const start = Math.min(input.selectionStart || nextValue.length, nextValue.length);
  input.value = nextValue;
  input.setSelectionRange(start, start);
}, true);

document.addEventListener('blur', (event) => {
  const input = event.target;
  if (!(input instanceof HTMLInputElement)) return;
  input.classList.remove('input-error-shake');
  if (isQuantityInput(input) && !normalizePositiveQuantity(input.value)) {
    if (!input.value.trim()) return;
    input.value = '';
    markInteractionInputError(input);
    return;
  }
  if (isPriceInput(input)) {
    input.value = normalizeTradePriceInput(input.value);
    if (!isValidTradePrice(input.value)) {
      input.value = '';
      markInteractionInputError(input);
    }
  }
}, true);

function lockFilledProfileInput(input) {
  if (!input?.matches?.('[data-lock-once]')) return;
  if (!input.value.trim()) return;
  input.readOnly = true;
  input.classList.add('is-locked-once');
  input.title = '已填写，不可修改';
}

document.querySelectorAll('[data-lock-once]').forEach((input) => lockFilledProfileInput(input));

document.addEventListener('blur', (event) => {
  lockFilledProfileInput(event.target);
}, true);

function setLoginMethod(card, method) {
  const tipMap = {
    qq: '当前选择 QQ 登录，请扫码后继续。',
    wechat: '当前选择微信登录，请扫码后继续。',
    sms: '当前选择手机验证码登录，请填写手机号和验证码。',
  };
  card.querySelectorAll('[data-login-method]').forEach((item) => {
    const active = item.dataset.loginMethod === method;
    item.classList.toggle('active', active);
    item.setAttribute('aria-selected', active ? 'true' : 'false');
  });
  card.querySelectorAll('[data-login-panel]').forEach((panel) => {
    const active = panel.dataset.loginPanel === method;
    panel.hidden = !active;
    panel.classList.toggle('active', active);
  });
  const tip = card.querySelector('[data-login-tip]');
  if (tip) tip.textContent = tipMap[method] || '可任选 QQ、微信或手机验证码任一种方式登录。';
}

document.querySelectorAll('.login-panel-card').forEach((card) => {
  const initial = card.querySelector('[data-login-method].active')?.dataset.loginMethod || 'qq';
  setLoginMethod(card, initial);
});

document.addEventListener('click', (event) => {
  const btn = event.target.closest('[data-login-method]');
  if (!btn) return;
  const card = btn.closest('.login-panel-card');
  if (!card) return;
  setLoginMethod(card, btn.dataset.loginMethod);
});

document.querySelector('[data-login-submit]')?.addEventListener('click', () => {
  const activeMethod = document.querySelector('[data-login-method].active')?.dataset.loginMethod || 'qq';
  const privacy = document.querySelector('[data-login-privacy]');
  if (!privacy?.checked) {
    privacy?.closest('.privacy-check')?.classList.add('input-error-shake');
    setTimeout(() => privacy?.closest('.privacy-check')?.classList.remove('input-error-shake'), 480);
    return;
  }
  if (activeMethod === 'sms') {
    const phone = document.querySelector('[data-login-phone]');
    const code = document.querySelector('[data-login-code]');
    if (!phone?.value.trim()) return markInteractionInputError(phone);
    if (!code?.value.trim()) return markInteractionInputError(code);
  }
  const toast = document.createElement('div');
  toast.className = 'publish-toast';
  toast.textContent = '请接入后端登录接口，登录成功后由服务端回跳';
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 1800);
});

const QUICK_IMPORT_PAGE_SIZE = 30;

function quickParseLine(line) {
  const parts = line.trim().split(/\s+/);
  const model = parts[0] || 'GRM188R71H104KA93D';
  const rest = parts.slice(1);
  const qtyRaw = rest.find((part) => /^[1-9]\d*(?:Kpcs|K|Pcs|PCS|盘|卷|件)?$/i.test(part)) || '850Kpcs';
  const priceRaw = rest.find((part) => /[¥￥/]|价格|期望/.test(part)) || '';
  const qtyMatch = qtyRaw.match(/^([1-9]\d*)(Kpcs|K|Pcs|PCS|盘|卷|件)?$/i);
  const qty = qtyMatch?.[1] || '850';
  const unit = (qtyMatch?.[2] || 'Kpcs').replace(/^K$/i, 'Kpcs');
  const price = (priceRaw.match(/\d+(?:\.\d+)?/) || [''])[0];
  return { model, qty, unit, price };
}

function quickPreviewRow(item, isTaxed = true) {
  const options = ['Kpcs', 'Pcs', '盘', '卷', '件'].map((unitOption) => `<option${unitOption.toLowerCase() === item.unit.toLowerCase() ? ' selected' : ''}>${unitOption}</option>`).join('');
  const taxClass = isTaxed ? 'is-taxed' : 'is-untaxed';
  const taxText = isTaxed ? '含税' : '未税';
  return `<tr class="inventory-item"><td><span class="tag orange">核对中</span></td><td><strong>${item.model}</strong></td><td>自动解析 · 待核对参数</td><td><input class="qty-input" value="${item.qty}"></td><td><select class="unit-select">${options}</select></td><td><label class="price-field ${taxClass}"><input class="price-input" min="0.0001" step="0.0001" placeholder="如 0.0001" value="${item.price}"><span>${taxText}</span></label></td></tr>`;
}

function renderQuickImportPage(modal) {
  const rows = modal._quickImportRows || [];
  const page = modal._quickImportPage || 1;
  const pageCount = Math.max(1, Math.ceil(rows.length / QUICK_IMPORT_PAGE_SIZE));
  const start = (page - 1) * QUICK_IMPORT_PAGE_SIZE;
  const currentRows = rows.slice(start, start + QUICK_IMPORT_PAGE_SIZE);
  const isTaxed = modal.querySelector('[data-quick-tax-master]')?.getAttribute('aria-pressed') !== 'false';
  const body = modal.querySelector('[data-quick-preview-body]');
  if (body) body.innerHTML = currentRows.map((row) => quickPreviewRow(row, isTaxed)).join('');
  const pageInfo = modal.querySelector('[data-quick-page-info]');
  if (pageInfo) pageInfo.textContent = `第 ${page} / ${pageCount} 页`;
  const prev = modal.querySelector('[data-quick-page-prev]');
  const next = modal.querySelector('[data-quick-page-next]');
  if (prev) prev.disabled = page <= 1;
  if (next) next.disabled = page >= pageCount;
}

function setQuickImportTax(modal, checked) {
  const master = modal.querySelector('[data-quick-tax-master]');
  if (master) {
    master.setAttribute('aria-pressed', checked ? 'true' : 'false');
    master.classList.toggle('is-on', checked);
  }
  modal.querySelectorAll('.quick-preview-table .price-field').forEach((field) => {
    const label = field.querySelector('span');
    field.classList.toggle('is-taxed', checked);
    field.classList.toggle('is-untaxed', !checked);
    if (label) label.textContent = checked ? '含税' : '未税';
  });
}

document.querySelectorAll('[data-quick-parse]').forEach((link) => {
  link.addEventListener('click', (event) => {
    event.preventDefault();
    const modal = link.closest('#quickImportModal');
    if (!modal) return;
    const text = modal.querySelector('[data-quick-paste-text]')?.value || '';
    const lines = text.split('\n').map((line) => line.trim()).filter(Boolean);
    modal._quickImportRows = (lines.length ? lines : ['GRM188R71H104KA93D 850Kpcs']).map(quickParseLine);
    modal._quickImportPage = 1;
    const count = modal.querySelector('[data-quick-import-count]');
    if (count) count.textContent = `成功导入 ${modal._quickImportRows.length} 条数据`;
    modal.querySelector('[data-quick-paste-panel]')?.setAttribute('hidden', '');
    modal.querySelector('[data-quick-preview-panel]')?.removeAttribute('hidden');
    renderQuickImportPage(modal);
  });
});

document.addEventListener('click', (event) => {
  const prev = event.target.closest('[data-quick-page-prev]');
  const next = event.target.closest('[data-quick-page-next]');
  const masterTax = event.target.closest('[data-quick-tax-master]');
  const confirm = event.target.closest('[data-quick-confirm-release]');
  const quickKind = event.target.closest('[data-quick-kind]');
  const modal = event.target.closest('#quickImportModal');
  if (!modal) return;
  if (prev) {
    modal._quickImportPage = Math.max(1, (modal._quickImportPage || 1) - 1);
    renderQuickImportPage(modal);
  }
  if (next) {
    const rows = modal._quickImportRows || [];
    const pageCount = Math.max(1, Math.ceil(rows.length / QUICK_IMPORT_PAGE_SIZE));
    modal._quickImportPage = Math.min(pageCount, (modal._quickImportPage || 1) + 1);
    renderQuickImportPage(modal);
  }
  if (masterTax) {
    const checked = masterTax.getAttribute('aria-pressed') !== 'true';
    setQuickImportTax(modal, checked);
  }
  if (quickKind) {
    quickKind.closest('.quick-type-toggle')?.querySelectorAll('[data-quick-kind]').forEach((btn) => btn.classList.remove('active'));
    quickKind.classList.add('active');
  }
  if (confirm) {
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = '已经成功发布！';
    document.body.appendChild(toast);
    setTimeout(() => {
      toast.remove();
      modal.setAttribute('hidden', '');
      modal.querySelector('[data-quick-paste-panel]')?.removeAttribute('hidden');
      modal.querySelector('[data-quick-preview-panel]')?.setAttribute('hidden', '');
      const count = modal.querySelector('[data-quick-import-count]');
      if (count) count.textContent = '';
    }, 2000);
  }
});

const commonPartAttrs = ['品牌', '封装', '精度'];
const partSpecificAttrs = {
  capacitor: ['容值', '耐压', '介质'],
  resistor: ['阻值', '功率', '温漂']
};
function getPartAttrs(type) {
  return [...commonPartAttrs, ...(partSpecificAttrs[type] || [])];
}
const quickPublishModels = {
  capacitor: ['GRM188R71H104KA93D', 'CL10B104KB8NNNC', 'GRM21BR71H105KA12L', 'CC0603KRX7R9BB104'],
  resistor: ['RC0603FR-0710KL', 'RC0402FR-0710KL', 'ERJ3EKF1002V', 'CRCW060310K0FKEA']
};
const quickPublishAttrOptions = {
  品牌: ['Murata', 'Samsung', 'Yageo', 'TDK', '国巨'],
  封装: ['0201', '0402', '0603', '0805', '1206'],
  容值: ['100nF', '1uF', '10uF', '22uF', '47uF'],
  精度: ['±1%', '±5%', '±10%', '±20%'],
  耐压: ['6.3V', '10V', '16V', '25V', '50V'],
  介质: ['X5R', 'X7R', 'C0G', 'Y5V'],
  阻值: ['10Ω', '100Ω', '1KΩ', '10KΩ', '100KΩ'],
  功率: ['1/20W', '1/16W', '1/10W', '1/8W', '1/4W'],
  温漂: ['±50ppm', '±100ppm', '±200ppm']
};

function renderQuickPublishAttrs(form) {
  const activeType = form.querySelector('[data-part-type].active')?.dataset.partType || 'capacitor';
  const grid = form.querySelector('[data-attr-grid]');
  if (!grid) return;
  const requiredMap = { capacitor: ['封装', '容值'], resistor: ['封装', '阻值'] };
  const attrs = getPartAttrs(activeType);
  grid.innerHTML = attrs.map((name) => (
    `<label>${name}<input class="input" data-clear-on-click data-attr-name="${name}" ${(requiredMap[activeType] || []).includes(name) ? `data-required="${name}"` : ''} list="attr-${name}" placeholder="填写${name}"></label>`
  )).join('') + attrs.map((name) => (
    `<datalist id="attr-${name}">${(quickPublishAttrOptions[name] || []).map((value) => `<option value="${value}"></option>`).join('')}</datalist>`
  )).join('');
}

function normalizePrecisionValue(input) {
  const value = input.value.trim();
  if (!value) return;
  if (!value.startsWith('±')) {
    input.value = value.endsWith('%') ? `±${value}` : `±${value}%`;
  }
}

function normalizeResistanceValue(input, live = false) {
  let value = input.value.trim();
  if (!value) return;
  const form = input.closest('[data-quick-publish-form]');
  const precisionInput = form?.querySelector('[data-attr-name="精度"]');
  const tolerance = value.match(/([JF])$/i)?.[1]?.toUpperCase();
  if (tolerance && precisionInput && !precisionInput.value.trim()) {
    precisionInput.value = tolerance === 'J' ? '±5%' : '±1%';
    value = value.slice(0, -1);
  }
  const hasOhm = /Ω$/i.test(value);
  const hasRNotation = /r/i.test(value);
  if (!hasOhm && !hasRNotation) value = `${value}Ω`;
  input.value = value;
  if (live) {
    const pos = Math.max(0, value.length - (value.endsWith('Ω') ? 1 : 0));
    input.setSelectionRange?.(pos, pos);
  }
}

function resetQuickPublishForm(form) {
  if (!form) return;
  form.querySelectorAll('input').forEach((input) => {
    input.value = '';
    input.classList.remove('input-error', 'shake-error');
  });
  form.querySelectorAll('[data-publish-kind], [data-part-type]').forEach((btn) => btn.classList.remove('active'));
  form.querySelector('[data-publish-kind="supply"]')?.classList.add('active');
  form.querySelector('[data-part-type="capacitor"]')?.classList.add('active');
  const qtyLabel = form.querySelector('[data-qty-label]');
  if (qtyLabel) qtyLabel.textContent = '可供数量';
  setDefaultValidity(form, 'supply');
  form.querySelector('[data-suggest-list]')?.setAttribute('hidden', '');
  const taxToggle = form.querySelector('[data-tax-toggle]');
  const priceField = form.querySelector('.price-field');
  const taxLabel = priceField?.querySelector('span');
  taxToggle?.classList.remove('is-on');
  taxToggle?.setAttribute('aria-pressed', 'false');
  priceField?.classList.remove('is-taxed');
  priceField?.classList.add('is-untaxed');
  if (taxLabel) taxLabel.textContent = '未税';
  renderQuickPublishAttrs(form);
}

function setDefaultValidity(form, kind) {
  const target = kind === 'demand' ? '3天' : '1个月';
  form.querySelectorAll('[data-validity]').forEach((btn) => {
    btn.classList.toggle('active', btn.dataset.validity === target);
  });
}

document.querySelectorAll('[data-quick-publish-form]').forEach((form) => {
  renderQuickPublishAttrs(form);

  form.querySelectorAll('[data-publish-kind], [data-part-type]').forEach((btn) => {
    btn.addEventListener('click', () => {
      const selector = btn.hasAttribute('data-publish-kind') ? '[data-publish-kind]' : '[data-part-type]';
      form.querySelectorAll(selector).forEach((item) => item.classList.remove('active'));
      btn.classList.add('active');
      if (selector === '[data-publish-kind]') {
        if (btn.dataset.publishKind === 'supply' && btn.closest('.publish-form-modal')?.classList.contains('cannot-publish-supply')) return;
        const qtyLabel = form.querySelector('[data-qty-label]');
        if (qtyLabel) qtyLabel.textContent = btn.dataset.publishKind === 'supply' ? '可供数量' : '需求数量';
        const modal = form.closest('.publish-form-modal');
        const title = modal?.querySelector('[data-publish-title]');
        if (title) title.textContent = modal.classList.contains('is-buyer-publish') ? '发布采购' : (btn.dataset.publishKind === 'supply' ? '发布供应' : '发布需求');
        setDefaultValidity(form, btn.dataset.publishKind);
      }
      if (selector === '[data-part-type]') {
        form.querySelector('[data-model-input]').value = '';
        form.querySelector('[data-suggest-list]').setAttribute('hidden', '');
        renderQuickPublishAttrs(form);
      }
    });
  });

  const input = form.querySelector('[data-model-input]');
  const list = form.querySelector('[data-suggest-list]');
  input?.addEventListener('input', () => {
    const activeType = form.querySelector('[data-part-type].active')?.dataset.partType || 'capacitor';
    const keyword = input.value.trim().toLowerCase();
    const matches = quickPublishModels[activeType].filter((model) => model.toLowerCase().includes(keyword)).slice(0, 5);
    if (!keyword || matches.length === 0) {
      list.setAttribute('hidden', '');
      return;
    }
    list.innerHTML = matches.map((model) => `<button type="button">${model}</button>`).join('');
    list.removeAttribute('hidden');
  });

  list?.addEventListener('click', (event) => {
    const option = event.target.closest('button');
    if (!option) return;
    input.value = option.textContent;
    list.setAttribute('hidden', '');
  });
});

document.addEventListener('pointerdown', (event) => {
  const input = event.target.closest('input[list]');
  if (!input) return;
  input.dataset.wasFocusedBeforeClick = document.activeElement === input ? 'true' : 'false';
});

document.addEventListener('click', (event) => {
  const input = event.target.closest('input[list]');
  if (!input) return;
  const shouldClear = input.dataset.wasFocusedBeforeClick === 'true' && input.value.trim();
  delete input.dataset.wasFocusedBeforeClick;
  if (!shouldClear) return;
  input.value = '';
  input.classList.remove('input-error', 'shake-error', 'input-error-shake');
  input.dispatchEvent(new Event('input', { bubbles: true }));
  const form = input.closest('[data-quick-publish-form]');
  form?.querySelector('[data-suggest-list]')?.setAttribute('hidden', '');
});

document.addEventListener('input', (event) => {
  const input = event.target.closest('[data-required]');
  if (input && input.value.trim()) input.classList.remove('input-error', 'shake-error');
  const attrInput = event.target.closest('[data-attr-name]');
  if (!attrInput) return;
  if (attrInput.dataset.attrName === '阻值' && /^\d+(\.\d+)?$/i.test(attrInput.value.trim())) {
    normalizeResistanceValue(attrInput, true);
  }
  if (attrInput.dataset.attrName === '精度') {
    attrInput.classList.remove('input-error', 'shake-error');
  }
});

document.addEventListener('change', (event) => {
  const attrInput = event.target.closest('[data-attr-name]');
  if (!attrInput) return;
  if (attrInput.dataset.attrName === '阻值') normalizeResistanceValue(attrInput);
  if (attrInput.dataset.attrName === '精度') normalizePrecisionValue(attrInput);
});

document.addEventListener('blur', (event) => {
  const attrInput = event.target.closest?.('[data-attr-name]');
  if (!attrInput) return;
  if (attrInput.dataset.attrName === '阻值') normalizeResistanceValue(attrInput);
  if (attrInput.dataset.attrName === '精度') normalizePrecisionValue(attrInput);
}, true);

document.addEventListener('focusin', (event) => {
  const form = event.target.closest?.('[data-quick-publish-form]');
  if (!form) return;
  const resistorInput = form.querySelector('[data-attr-name="阻值"]');
  if (resistorInput && event.target !== resistorInput) normalizeResistanceValue(resistorInput);
});

document.addEventListener('click', (event) => {
  const validity = event.target.closest('[data-validity]');
  if (!validity) return;
  const picker = validity.closest('.validity-picker');
  picker?.querySelectorAll('[data-validity]').forEach((btn) => btn.classList.remove('active'));
  validity.classList.add('active');
});

document.querySelectorAll('[data-publish-confirm]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const form = btn.closest('[data-quick-publish-form]');
    const modal = btn.closest('.modal-backdrop');
    if (!form) return;
    const kind = form.querySelector('[data-publish-kind].active')?.dataset.publishKind || 'supply';

    if (!isCurrentMemberLoggedIn()) {
      ZrToast.error('请先登录后再发布');
      return;
    }
    
    const requiredInputs = [...form.querySelectorAll('[data-required]')];
    const emptyInputs = requiredInputs.filter((input) => !input.value.trim());
    requiredInputs.forEach((input) => input.classList.remove('input-error'));
    if (emptyInputs.length) {
      emptyInputs.forEach((input) => {
        input.classList.remove('shake-error');
        void input.offsetWidth;
        input.classList.add('input-error', 'shake-error');
      });
      emptyInputs[0].focus();
      return;
    }
    
    const priceInput = form.querySelector('.price-input');
    if (priceInput) {
      if (!(kind === 'demand' && priceInput.value.trim() === '不限')) {
        priceInput.value = normalizeTradePriceInput(priceInput.value);
        if (!isValidTradePrice(priceInput.value)) {
          markInteractionInputError(priceInput);
          return;
        }
      }
      if (kind === 'supply' && !priceInput.value.trim()) {
        markInteractionInputError(priceInput);
        return;
      }
    }
    
    const model = form.querySelector('[data-model-input]')?.value || '';
    const qtyInput = form.querySelector('[data-required="数量"]')?.closest('label')?.querySelector('input');
    const qty = qtyInput?.value || '0';
    const qtyUnit = form.querySelector('.unit-inline-input')?.value || 'Kpcs';
    const price = priceInput?.value || '0';
    const isTaxed = form.querySelector('[data-tax-toggle]')?.getAttribute('aria-pressed') === 'true';
    
    const data = new FormData();
    data.append('action', 'publish_goods');
    data.append('goodsSn', model);
    data.append('goodsStock', qty);
    data.append('goodsUnit', qtyUnit);
    data.append('shopPrice', price);
    data.append('isIncludingTax', isTaxed ? '1' : '0');
    data.append('pubType', kind === 'supply' ? '1' : '2');
    if (window.ZR_CURRENT_MEMBER && window.ZR_CURRENT_MEMBER.shopId) {
      data.append('shopId', window.ZR_CURRENT_MEMBER.shopId);
    }
    
    const xhr = new XMLHttpRequest();
    xhr.open('POST', '/publish.ashx', true);
    xhr.onload = function() {
      let result;
      try {
        result = JSON.parse(xhr.responseText);
      } catch (e) {
        result = { success: false, message: '服务器返回无效数据' };
      }
      
      const toast = document.createElement('div');
      toast.className = 'publish-toast';
      if (result.success) {
        toast.textContent = kind === 'demand' ? '需求发布成功！请到后台查看' : '供应发布成功！请到后台查看';
      } else {
        toast.textContent = result.message || '发布失败，请重试';
        toast.style.backgroundColor = '#e53935';
      }
      document.body.appendChild(toast);
      
      setTimeout(() => {
        toast.remove();
        if (result.success) {
          resetQuickPublishForm(form);
          modal?.setAttribute('hidden', '');
        }
      }, 2000);
    };
    xhr.onerror = function() {
      const toast = document.createElement('div');
      toast.className = 'publish-toast';
      toast.textContent = '网络错误，请重试';
      toast.style.backgroundColor = '#e53935';
      document.body.appendChild(toast);
      setTimeout(() => toast.remove(), 2000);
    };
    xhr.send(data);
  });
});

document.querySelectorAll('[data-ui-toast]').forEach((el) => {
  el.addEventListener('click', (event) => {
    const quoteRow = el.closest('.quote-submit-row');
    if (quoteRow) {
      const required = [...quoteRow.querySelectorAll('[data-required-quote]')];
      const empty = required.filter((input) => !input.value.trim());
      required.forEach((input) => input.classList.remove('input-error-shake'));
      if (empty.length) {
        event.preventDefault();
        empty.forEach((input) => markInteractionInputError(input));
        empty[0].focus();
        return;
      }
    }
    const text = el.getAttribute('data-ui-toast') || '操作已记录，后续由接口提交。';
    const toast = document.createElement('div');
    toast.className = 'publish-toast success-toast';
    toast.innerHTML = `<b>${text}</b><span>报价信息已进入采购方记录</span>`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1500);
  });
});

function parseTradeQuantity(text) {
  const match = (text || '').match(/([1-9]\d*)\s*(?:\/|单位)?\s*(Kpcs|K|Pcs|PCS|盘|卷|件)?/i);
  if (!match) return { qty: '', unit: 'Kpcs' };
  const unit = (match[2] || 'Kpcs').replace(/^K$/i, 'Kpcs').replace(/^PCS$/i, 'Pcs');
  return { qty: match[1], unit };
}

function parseTradePrice(text) {
  return ((text || '').match(/\d+(?:\.\d+)?/) || [''])[0];
}

function getTradeContext(trigger) {
  const item = trigger.closest('.item');
  const parseBrand = (text) => {
    if (!text) return '';
    const segs = text.split(/[·•・,，\s\/|]+/).map((s) => s.trim()).filter(Boolean);
    const seg = segs.find((s) => /品牌|不限|其它|其他|未知/.test(s)) || '';
    if (/不限|其它|其他|未知/.test(seg)) return '不限';
    const m = seg.match(/品牌[:：]?\s*(.+)/);
    if (m && m[1]) return m[1].trim();
    return segs[0] && !/^\d/.test(segs[0]) ? segs[0] : '';
  };
  if (item) {
    const title = item.querySelector('h3 a')?.textContent.replace(/^求购\s*/, '').trim() || '未命名型号';
    const meta = [...item.querySelectorAll('.meta span')].map((span) => span.textContent.trim());
    const qtyText = meta.find((text) => /^(需求|现货)\s*\d/i.test(text)) || '';
    const { qty, unit } = parseTradeQuantity(qtyText);
    const priceText = item.querySelector('.price')?.textContent.trim() || '';
    return {
      model: title,
      attrs: meta[0] || '参数待确认',
      brand: parseBrand(meta[0] || ''),
      company: item.querySelector('.company')?.textContent.trim() || '对方公司',
      qty,
      unit,
      price: priceText.includes('面议') ? '' : parseTradePrice(priceText),
      taxed: !/未税|不含税/.test(item.textContent),
      valid: meta.find((text) => /有效期/.test(text)) || '有效期待确认',
    };
  }
  const detailLayout = trigger.closest('.detail-layout');
  if (detailLayout) {
    const isQuoteAction = trigger.textContent.trim() === '我要报价';
    const mainPanel = trigger.closest('.detail-main-card') || detailLayout.querySelector('.detail-main-card');
    const hero = detailLayout.previousElementSibling?.matches('.detail-hero') ? detailLayout.previousElementSibling : document.querySelector('.detail-hero');
    const sidePanel = detailLayout.querySelector('aside.panel');
    const readDetail = (label) => {
      const row = [...(mainPanel?.querySelectorAll('.detail-metrics div, .detail-spec-grid div') || [])].find((node) => node.querySelector('span')?.textContent.trim().includes(label));
      if (!row) return { value: '', unit: '' };
      return {
        value: (row.querySelector('strong, b')?.textContent || '').trim(),
        unit: (row.querySelector('small')?.textContent || '').trim(),
      };
    };
    const qtyInfo = readDetail(isQuoteAction ? '需求数量' : '库存');
    const fallbackQtyInfo = readDetail(isQuoteAction ? '库存' : '需求数量');
    const qtyText = `${qtyInfo.value || fallbackQtyInfo.value} ${qtyInfo.unit || fallbackQtyInfo.unit}`.trim();
    const { qty, unit } = parseTradeQuantity(qtyText);
    const validInfo = readDetail('有效期');
    const validText = hero?.querySelector('.validity-line')?.textContent.trim() || (validInfo.value ? `有效期 ${validInfo.value}` : (isQuoteAction ? '有效期待确认' : '有效期 72 小时'));
    const priceText = hero?.querySelector('.detail-price-card strong')?.textContent.trim() || readDetail(isQuoteAction ? '期望单价' : '单价').value;
    const attrs = hero?.querySelector('.detail-hero-main p')?.textContent.replace(/\s*·\s*(电容|电阻|采购需求)\s*/g, ' · ').replace(/\s*·\s*·\s*/g, ' · ').trim() || '参数待确认';
    return {
      model: hero?.querySelector('.detail-hero-main h2')?.textContent.trim() || '未命名型号',
      attrs,
      brand: readDetail('品牌').value || parseBrand(attrs),
      company: sidePanel?.querySelector('h2')?.textContent.trim() || '对方公司',
      qty,
      unit,
      price: parseTradePrice(priceText),
      taxed: !/否|未税|不含税/.test(readDetail('是否含税').value || mainPanel?.textContent || ''),
      valid: validText,
    };
  }
  const panel = trigger.closest('.panel');
  const title = panel?.querySelector('.section-title h2')?.textContent.trim() || '未命名型号';
  const attrs = panel?.querySelector('.section-title p')?.textContent.trim() || '参数待确认';
  const cells = [...(panel?.querySelectorAll('th,td') || [])].map((cell) => cell.textContent.trim());
  const findAfter = (label) => {
    const index = cells.findIndex((cell) => cell.includes(label));
    return index >= 0 ? cells[index + 1] || '' : '';
  };
  const qtyText = findAfter('库存') || findAfter('需求数量');
  const { qty, unit } = parseTradeQuantity(qtyText);
  const priceText = findAfter('单价') || findAfter('期望单价');
  const sidePanel = panel?.parentElement?.querySelector('aside.panel');
  return {
    model: title,
    attrs,
    brand: findAfter('品牌') || parseBrand(attrs),
    company: sidePanel?.querySelector('.section-title p')?.textContent.trim() || '对方公司',
    qty,
    unit,
    price: parseTradePrice(priceText),
    taxed: !/否|未税|不含税/.test(findAfter('是否含税') || panel?.textContent || ''),
    valid: findAfter('有效期') ? `有效期 ${findAfter('有效期')}` : '有效期待确认',
  };
}

function ensureTradeModal() {
  let modal = document.getElementById('tradeInteractionModal');
  if (modal) return modal;
  modal = document.createElement('div');
  modal.className = 'modal-backdrop';
  modal.id = 'tradeInteractionModal';
  modal.hidden = true;
  modal.innerHTML = `
    <div class="modal trade-interaction-modal" role="dialog" aria-modal="true" aria-label="交易互动">
      <div class="modal-head"><h2><span data-trade-title>我要报价</span><small data-trade-subtitle></small></h2><button class="modal-close" type="button" data-interaction-close aria-label="关闭">×</button></div>
      <div class="modal-body">
        <div class="trade-context-card">
          <div><strong data-trade-model></strong><p data-trade-attrs></p></div>
        </div>
        <datalist id="interactionUnitOptions"><option value="Kpcs"></option><option value="Pcs"></option><option value="盘"></option><option value="卷"></option><option value="件"></option></datalist>
        <datalist id="interactionBrandOptions"><option value="三星 Samsung"></option><option value="村田 Murata"></option><option value="国巨 Yageo"></option><option value="华新科 Walsin"></option><option value="风华"></option><option value="厚声 Uniohm"></option><option value="旺诠 RALEC"></option><option value="TDK"></option><option value="KEMET"></option><option value="AVX"></option><option value="Panasonic"></option><option value="Vishay"></option><option value="ROHM"></option></datalist>
        <div class="interaction-form">
          <div class="trade-party-strip"><span data-trade-company></span><small data-trade-qty></small><small data-trade-valid></small></div>
          <label data-interaction-qty-field><span data-interaction-qty-label>数量</span><input class="input" data-interaction-qty inputmode="numeric" pattern="[1-9][0-9]*" placeholder="填写数量"></label>
          <label data-interaction-unit-field>单位<select class="input unit-inline-input" data-interaction-unit><option>Kpcs</option><option>Pcs</option><option>盘</option><option>卷</option><option>件</option></select></label>
          <label data-interaction-batch-field>批次<input class="input" data-interaction-batch placeholder="如 2025+ 或 2633"></label>
          <label data-interaction-price-field><span data-interaction-price-label>我的期望价</span><span class="tax-inline"><span class="price-field is-untaxed"><input class="price-input" data-interaction-price inputmode="decimal" min="0.0001" step="0.0001" placeholder="优势价格"><span>未税</span></span><button class="tax-switch" type="button" data-tax-toggle aria-pressed="false"><span></span></button></span></label>
          <div class="interaction-validity" data-interaction-validity-row hidden><span>有效期</span><button type="button" data-interaction-validity="24小时">24小时</button><button class="active" type="button" data-interaction-validity="3天">3天</button><button type="button" data-interaction-validity="7天">7天</button><button type="button" data-interaction-validity="15天">15天</button><button type="button" data-interaction-validity="1个月">1个月</button></div>
          <label class="interaction-note"><span>备注</span><input class="input" placeholder="如交期、包装、分批等"><span class="trade-brand-row" data-interaction-brand-row hidden><span>品牌</span><input class="input" data-interaction-brand list="interactionBrandOptions" autocomplete="on" placeholder="请提供品牌"><small class="interaction-brand-hint" data-interaction-brand-hint></small></span></label>
          <button class="btn primary" type="button" data-interaction-submit>提交</button>
        </div>
      </div>
    </div>`;
  document.body.appendChild(modal);
  return modal;
}

function setTradeModalTax(modal, checked) {
  const toggle = modal.querySelector('[data-tax-toggle]');
  const priceField = modal.querySelector('.price-field');
  const label = priceField?.querySelector('span');
  toggle?.setAttribute('aria-pressed', checked ? 'true' : 'false');
  toggle?.classList.toggle('is-on', checked);
  priceField?.classList.toggle('is-taxed', checked);
  priceField?.classList.toggle('is-untaxed', !checked);
  if (label) label.textContent = checked ? '含税' : '未税';
}

function resetTradeInteractionModal(modal) {
  if (!modal) return;
  modal.querySelectorAll('.input-error-shake').forEach((input) => input.classList.remove('input-error-shake'));
  modal.querySelectorAll('[data-interaction-qty], [data-interaction-price], [data-interaction-batch], [data-interaction-brand], .interaction-note input').forEach((input) => {
    input.value = '';
  });
  const brandRow = modal.querySelector('[data-interaction-brand-row]');
  const brandInput = modal.querySelector('[data-interaction-brand]');
  const brandHint = modal.querySelector('[data-interaction-brand-hint]');
  const noteRow = modal.querySelector('.interaction-note');
  modal.querySelector('[data-interaction-validity-row]')?.setAttribute('hidden', '');
  modal.querySelectorAll('[data-interaction-validity]').forEach((button) => button.classList.toggle('active', button.dataset.interactionValidity === '3天'));
  noteRow?.classList.remove('interaction-note-brand');
  brandRow?.setAttribute('hidden', '');
  brandInput?.removeAttribute('readonly');
  brandInput?.classList.remove('is-locked');
  if (brandInput) brandInput.placeholder = '请提供品牌';
  if (brandHint) brandHint.textContent = '';
  modal.dataset.tradeMode = '';
}

function markInteractionInputError(input) {
  if (!input) return;
  input.classList.remove('input-error-shake');
  void input.offsetWidth;
  input.classList.add('input-error-shake');
  input.focus();
}

document.addEventListener('click', (event) => {
  const trigger = event.target.closest('a');
  if (!trigger || !['我要报价', '立即询价'].includes(trigger.textContent.trim())) return;
  event.preventDefault();
  const isQuote = trigger.textContent.trim() === '我要报价';
  const context = getTradeContext(trigger);
  const modal = ensureTradeModal();
  resetTradeInteractionModal(modal);
  modal.dataset.tradeMode = isQuote ? 'quote' : 'inquiry';
  modal.querySelector('[data-trade-title]').textContent = isQuote ? '向采购商报价' : '向供应商询价';
  modal.querySelector('[data-trade-subtitle]').textContent = isQuote ? '填写你可供应的数量和报价，采购商收到后可对比选择。' : '填写您的采购数量、期望价格；供应商尽快回复报价！';
  modal.querySelector('[data-trade-model]').textContent = context.model;
  modal.querySelector('[data-trade-attrs]').textContent = context.attrs;
  modal.querySelector('[data-trade-company]').textContent = context.company;
  modal.querySelector('[data-trade-qty]').textContent = context.qty ? `${isQuote ? '采购商需求' : '供应商可供'}：${context.qty}/${context.unit}` : `${isQuote ? '采购商需求' : '供应商可供'}待确认`;
  modal.querySelector('[data-trade-valid]').textContent = context.valid;
  modal.querySelector('[data-interaction-qty-label]').textContent = isQuote ? '我可供应' : '我要采购';
  modal.querySelector('[data-interaction-price-label]').textContent = isQuote ? '我的报价' : '期望单价';
  modal.querySelector('[data-interaction-qty]').value = isQuote ? context.qty : '';
  modal.querySelector('[data-interaction-unit]').value = context.unit;
  modal.querySelector('[data-interaction-validity-row]')?.removeAttribute('hidden');
  const interactionPrice = modal.querySelector('[data-interaction-price]');
  if (interactionPrice) {
    interactionPrice.value = '';
    interactionPrice.placeholder = isQuote ? '如 0.0001' : '';
  }
  const brandRow = modal.querySelector('[data-interaction-brand-row]');
  const brandInput = modal.querySelector('[data-interaction-brand]');
  const brandHint = modal.querySelector('[data-interaction-brand-hint]');
  const noteRow = modal.querySelector('.interaction-note');
  if (brandRow && brandInput && brandHint) {
    if (isQuote) {
      const brand = (context.brand || '').trim();
      const isOpen = !brand || /不限|其它|其他|未知/.test(brand);
      if (isOpen) {
        brandRow.removeAttribute('hidden');
        noteRow?.classList.add('interaction-note-brand');
        brandInput.value = '';
        brandInput.removeAttribute('readonly');
        brandInput.classList.remove('is-locked');
        brandInput.placeholder = '请提供品牌';
        brandHint.textContent = '';
      } else {
        brandRow.setAttribute('hidden', '');
        noteRow?.classList.remove('interaction-note-brand');
        brandInput.value = '';
        brandInput.removeAttribute('readonly');
        brandInput.classList.remove('is-locked');
        brandHint.textContent = '';
      }
    } else {
      brandRow.setAttribute('hidden', '');
      noteRow?.classList.remove('interaction-note-brand');
      brandInput.value = '';
      brandInput.removeAttribute('readonly');
      brandInput.classList.remove('is-locked');
      brandHint.textContent = '';
    }
  }
  modal.querySelector('[data-interaction-submit]').textContent = isQuote ? '提交报价' : '发送询价';
  modal.querySelector('[data-interaction-submit]').setAttribute('data-ui-toast', isQuote ? '报价已提交给采购商' : '您已成功发布询价！');
  setTradeModalTax(modal, false);
  modal.removeAttribute('hidden');
});

document.addEventListener('click', (event) => {
  const validity = event.target.closest('[data-interaction-validity]');
  if (validity) {
    const row = validity.closest('[data-interaction-validity-row]');
    row?.querySelectorAll('[data-interaction-validity]').forEach((button) => button.classList.toggle('active', button === validity));
    return;
  }
  if (event.target?.id === 'tradeInteractionModal') {
    resetTradeInteractionModal(event.target);
    event.target.setAttribute('hidden', '');
  }
  if (event.target.closest('[data-interaction-close]')) {
    const modal = document.getElementById('tradeInteractionModal');
    resetTradeInteractionModal(modal);
    modal?.setAttribute('hidden', '');
  }
  const submit = event.target.closest('[data-interaction-submit]');
  if (!submit) return;
  const modal = document.getElementById('tradeInteractionModal');
  const qtyInput = modal?.querySelector('[data-interaction-qty]');
  if (qtyInput && !qtyInput.value.trim()) {
    markInteractionInputError(qtyInput);
    return;
  }
  const priceInput = modal?.querySelector('[data-interaction-price]');
  if (priceInput) {
    priceInput.value = normalizeTradePriceInput(priceInput.value);
    if (!isValidTradePrice(priceInput.value)) {
      markInteractionInputError(priceInput);
      return;
    }
  }
  if (modal?.dataset.tradeMode === 'quote' && priceInput && !priceInput.value.trim()) {
    markInteractionInputError(priceInput);
    return;
  }
  const brandInput = modal?.querySelector('[data-interaction-brand]');
  const brandRow = modal?.querySelector('[data-interaction-brand-row]');
  const needBrand = modal?.dataset.tradeMode === 'quote' && brandRow && !brandRow.hasAttribute('hidden');
  if (needBrand && brandInput && !brandInput.value.trim()) {
    markInteractionInputError(brandInput);
    return;
  }
  const toast = document.createElement('div');
  toast.className = 'publish-toast';
  const isInquiry = modal?.dataset.tradeMode === 'inquiry';
  toast.textContent = isInquiry ? '您已成功发布询价！' : (submit.getAttribute('data-ui-toast') || '已提交');
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 2000);
  modal?.setAttribute('hidden', '');
  resetTradeInteractionModal(modal);
});

function getResponsivePageSize() {
  return window.matchMedia('(max-width: 760px)').matches ? 10 : 25;
}

function applyQuoteFilter(panel) {
  const keyword = (panel.querySelector('[data-quote-search]')?.value || '').trim().toLowerCase();
  const filter = panel.querySelector('[data-quote-filter].active')?.dataset.quoteFilter || 'all';
  const isFiltering = Boolean(keyword) || filter !== 'all';
  const matchedGroups = [];
  panel.querySelectorAll('.quote-group').forEach((group) => {
    group.classList.toggle('is-filtering', isFiltering);
    let visibleRows = 0;
    const groupText = group.querySelector('.quote-group-head')?.textContent.toLowerCase() || '';
    group.querySelectorAll('.quote-row:not(.quote-row-head)').forEach((row) => {
      const rowText = row.textContent.toLowerCase();
      const marks = row.dataset.quoteMark || '';
      const keywordMatch = !keyword || groupText.includes(keyword) || rowText.includes(keyword);
      const filterMatch = filter === 'all' || marks.includes(filter);
      const visible = keywordMatch && filterMatch;
      row.classList.toggle('is-hidden', !visible);
      if (visible) visibleRows += 1;
    });
    const more = group.querySelector('[data-quote-more]');
    if (more) {
      more.hidden = isFiltering || visibleRows <= 3;
      if (isFiltering || visibleRows <= 3) {
        group.classList.remove('is-expanded');
        more.textContent = '---展现更多---';
      }
    }
    if (visibleRows > 0) matchedGroups.push(group);
  });
  const pageSize = getResponsivePageSize();
  const pageCount = Math.max(1, Math.ceil(matchedGroups.length / pageSize));
  panel._quotePage = Math.min(Math.max(panel._quotePage || 1, 1), pageCount);
  const start = (panel._quotePage - 1) * pageSize;
  const end = start + pageSize;
  panel.querySelectorAll('.quote-group').forEach((group) => group.classList.add('is-hidden'));
  matchedGroups.forEach((group, index) => {
    group.classList.toggle('is-hidden', index < start || index >= end);
  });
  const empty = panel.querySelector('[data-quote-empty]');
  if (empty) empty.hidden = matchedGroups.length !== 0;
  const pagination = panel.querySelector('[data-quote-pagination]');
  if (pagination) pagination.hidden = matchedGroups.length === 0;
  const pageInfo = panel.querySelector('[data-quote-page-info]');
  if (pageInfo) pageInfo.textContent = `第 ${panel._quotePage} / ${pageCount} 页`;
  const pageSizeText = panel.querySelector('.page-size');
  if (pageSizeText) pageSizeText.textContent = `每页 ${pageSize} 个型号`;
  const prev = panel.querySelector('[data-quote-page-prev]');
  const next = panel.querySelector('[data-quote-page-next]');
  if (prev) prev.disabled = panel._quotePage <= 1;
  if (next) next.disabled = panel._quotePage >= pageCount;
}

document.querySelectorAll('.received-quotes').forEach((panel) => {
  panel._quotePage = 1;
  panel.querySelector('[data-quote-search]')?.addEventListener('input', () => {
    panel._quotePage = 1;
    applyQuoteFilter(panel);
  });
  panel.querySelectorAll('[data-quote-filter]').forEach((btn) => {
    btn.addEventListener('click', () => {
      panel.querySelectorAll('[data-quote-filter]').forEach((item) => item.classList.remove('active'));
      btn.classList.add('active');
      panel._quotePage = 1;
      applyQuoteFilter(panel);
    });
  });
  panel.querySelector('[data-quote-show-new]')?.addEventListener('click', () => {
    panel.querySelector('[data-quote-search]').value = '';
    panel.querySelectorAll('[data-quote-filter]').forEach((item) => item.classList.remove('active'));
    panel.querySelector('[data-quote-filter="new"]')?.classList.add('active');
    panel._quotePage = 1;
    applyQuoteFilter(panel);
  });
  panel.querySelector('[data-quote-refresh]')?.addEventListener('click', () => {
    const refresh = panel.querySelector('[data-quote-refresh]');
    if (!refresh) return;
    refresh.textContent = '正在刷新...';
    setTimeout(() => {
      refresh.textContent = '最近更新：刚刚';
      const toast = document.createElement('div');
      toast.className = 'publish-toast';
      toast.textContent = '报价已刷新';
      document.body.appendChild(toast);
      setTimeout(() => toast.remove(), 1200);
    }, 500);
  });
  panel.querySelectorAll('[data-quote-more]').forEach((btn) => {
    btn.addEventListener('click', () => {
      const group = btn.closest('.quote-group');
      if (!group) return;
      const expanded = group.classList.toggle('is-expanded');
      btn.textContent = expanded ? '---收起更多---' : '---展现更多---';
    });
  });
  panel.querySelector('[data-quote-page-prev]')?.addEventListener('click', () => {
    panel._quotePage = Math.max(1, (panel._quotePage || 1) - 1);
    applyQuoteFilter(panel);
  });
  panel.querySelector('[data-quote-page-next]')?.addEventListener('click', () => {
    panel._quotePage = (panel._quotePage || 1) + 1;
    applyQuoteFilter(panel);
  });
  applyQuoteFilter(panel);
});

function applyInquiryFilter(panel) {
  const pageRoot = panel.closest('.main') || document;
  const keyword = (pageRoot.querySelector('[data-inquiry-search]')?.value || '').trim().toLowerCase();
  const filter = pageRoot.querySelector('.inquiry-tabs button[data-inquiry-filter].active')?.dataset.inquiryFilter || 'all';
  let visible = 0;
  panel.querySelectorAll('.inquiry-card').forEach((card) => {
    const text = `${card.dataset.inquiryText || ''} ${card.textContent}`.toLowerCase();
    const marks = card.dataset.inquiryMark || '';
    const matchesKeyword = !keyword || text.includes(keyword);
    const matchesFilter = filter === 'all' || marks.includes(filter);
    const show = matchesKeyword && matchesFilter;
    card.classList.toggle('is-hidden', !show);
    if (show) visible += 1;
  });
  const empty = panel.querySelector('[data-inquiry-empty]');
  if (empty) empty.hidden = visible !== 0;
  panel.querySelector('.pagination')?._updateBasicPagination?.(1);
}

document.querySelectorAll('.received-inquiries').forEach((panel) => {
  const pageRoot = panel.closest('.main') || document;
  pageRoot.querySelector('[data-inquiry-search]')?.addEventListener('input', () => applyInquiryFilter(panel));
  pageRoot.querySelectorAll('.inquiry-tabs button[data-inquiry-filter]').forEach((btn) => {
    btn.addEventListener('click', () => {
      pageRoot.querySelectorAll('.inquiry-tabs button[data-inquiry-filter]').forEach((item) => item.classList.remove('active'));
      btn.classList.add('active');
      applyInquiryFilter(panel);
    });
  });
  pageRoot.querySelector('[data-inquiry-show-new]')?.addEventListener('click', () => {
    const search = pageRoot.querySelector('[data-inquiry-search]');
    if (search) search.value = '';
    pageRoot.querySelectorAll('.inquiry-tabs button[data-inquiry-filter]').forEach((item) => item.classList.remove('active'));
    pageRoot.querySelector('.inquiry-tabs button[data-inquiry-filter="waiting"]')?.classList.add('active');
    applyInquiryFilter(panel);
  });
  applyInquiryFilter(panel);
});

function applyRecordFilter(panel) {
  const keyword = (panel.querySelector('[data-record-search]')?.value || '').trim().toLowerCase();
  const start = panel.querySelector('[data-record-date-start]')?.value || '';
  const end = panel.querySelector('[data-record-date-end]')?.value || '';
  const pageRoot = panel.closest('.main') || document;
  const category = pageRoot.querySelector('.record-tabs button[data-record-category].active')?.dataset.recordCategory || 'all';
  let visible = 0;
  panel.querySelectorAll('.record-card').forEach((card) => {
    const text = `${card.dataset.recordText || ''} ${card.textContent}`.toLowerCase();
    const date = card.dataset.recordDate || '';
    const matchesKeyword = !keyword || text.includes(keyword);
    const matchesCategory = category === 'all' || card.dataset.recordCategory === category;
    const matchesStart = !start || date >= start;
    const matchesEnd = !end || date <= end;
    const show = matchesKeyword && matchesCategory && matchesStart && matchesEnd;
    card.classList.toggle('is-hidden', !show);
    if (show) visible += 1;
  });
  const empty = panel.querySelector('[data-record-empty]');
  if (empty) empty.hidden = visible !== 0;
  panel.querySelector('.pagination')?._updateBasicPagination?.(1);
}

document.querySelectorAll('.quote-records-panel').forEach((panel) => {
  const pageRoot = panel.closest('.main') || document;
  pageRoot.querySelector('[data-record-search]')?.addEventListener('input', () => applyRecordFilter(panel));
  pageRoot.querySelector('[data-record-date-start]')?.addEventListener('change', () => applyRecordFilter(panel));
  pageRoot.querySelector('[data-record-date-end]')?.addEventListener('change', () => applyRecordFilter(panel));
  pageRoot.querySelectorAll('.record-tabs button[data-record-category]').forEach((btn) => {
    btn.addEventListener('click', () => {
      pageRoot.querySelectorAll('.record-tabs button[data-record-category]').forEach((item) => item.classList.remove('active'));
      btn.classList.add('active');
      applyRecordFilter(panel);
    });
  });
  applyRecordFilter(panel);
});

document.addEventListener('click', (event) => {
  const badge = event.target.closest('.quote-new-badge, .inquiry-stat-link em, .quote-stat-link em');
  if (!badge) return;
  badge.hidden = true;
});

document.querySelectorAll('[data-admin-table-search]').forEach((input) => {
  input.addEventListener('input', () => {
    const panel = input.closest('.admin-panel');
    const table = panel?.querySelector('[data-admin-table]');
    const keyword = input.value.trim().toLowerCase();
    table?.querySelectorAll('tbody tr').forEach((row) => {
      row.dataset.filteredOut = keyword && !row.textContent.toLowerCase().includes(keyword) ? '1' : '';
    });
    if (table) {
      table.dataset.page = '1';
      updateAdminPagination(table);
    }
  });
});

document.querySelectorAll('[data-admin-supply-search]').forEach((input) => {
  input.addEventListener('input', () => {
    const list = input.closest('.admin-panel')?.querySelector('[data-admin-supply-list]');
    const table = list?.querySelector('table');
    const keyword = input.value.trim().toLowerCase();
    list?.querySelectorAll('tbody tr').forEach((item) => {
      item.dataset.filteredOut = keyword && !item.textContent.toLowerCase().includes(keyword) ? '1' : '';
    });
    if (table) {
      table.dataset.page = '1';
      updateAdminPagination(table);
    }
  });
});

function updateAdminPagination(table) {
  if (!table) return;
  const pageSize = Number(table.getAttribute('data-admin-page-size') || 100);
  const rows = [...table.querySelectorAll('tbody tr')].filter((row) => !row.classList.contains('admin-quote-detail-row'));
  const visibleRows = rows.filter((row) => row.dataset.filteredOut !== '1');
  const totalPages = Math.max(1, Math.ceil(visibleRows.length / pageSize));
  const currentPage = Math.min(Math.max(1, Number(table.dataset.page || 1)), totalPages);
  table.dataset.page = String(currentPage);
  rows.forEach((row) => {
    const index = visibleRows.indexOf(row);
    row.hidden = index === -1 || index < (currentPage - 1) * pageSize || index >= currentPage * pageSize;
  });
  const panel = table.closest('.admin-panel');
  const pagination = panel?.querySelector('[data-admin-pagination]');
  if (!pagination) return;
  const info = pagination.querySelector('[data-page-info]');
  const prev = pagination.querySelector('[data-page-prev]');
  const next = pagination.querySelector('[data-page-next]');
  if (info) info.textContent = `第 ${currentPage} / ${totalPages} 页，共 ${visibleRows.length} 条`;
  if (prev) prev.disabled = currentPage <= 1;
  if (next) next.disabled = currentPage >= totalPages;
}

document.querySelectorAll('[data-admin-pagination]').forEach((pagination) => {
  pagination.addEventListener('click', (event) => {
    const table = pagination.closest('.admin-panel')?.querySelector('table[data-admin-page-size]');
    if (!table) return;
    const current = Number(table.dataset.page || 1);
    if (event.target.closest('[data-page-prev]')) table.dataset.page = String(Math.max(1, current - 1));
    if (event.target.closest('[data-page-next]')) table.dataset.page = String(current + 1);
    updateAdminPagination(table);
  });
});

document.querySelectorAll('table[data-admin-page-size]').forEach((table) => updateAdminPagination(table));

function showProfileToast(text) {
  const toast = document.createElement('div');
  toast.className = 'publish-toast';
  toast.textContent = text;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 1400);
}

document.querySelectorAll('[data-profile-role]').forEach((select) => {
  const currentRole = getCurrentMemberRole();
  if (currentRole && [...select.options].some((option) => option.value === currentRole)) select.value = currentRole;
  select.dataset.previousRole = select.value;
  select.addEventListener('change', () => {
    const nextValue = select.value;
    const prevValue = select.dataset.previousRole || '';
    if (/供应/.test(nextValue) && nextValue !== prevValue) {
      showProfileToast('请与客服联系核实！');
      select.value = prevValue || '采购商';
      syncProfileBadges();
      return;
    }
    select.dataset.previousRole = select.value;
    syncProfileBadges();
  });
});

function syncProfileBadges() {
  const roleBadge = document.querySelector('[data-profile-badge="role"]');
  const locationBadge = document.querySelector('[data-profile-badge="location"]');
  const deliveryBadge = document.querySelector('[data-profile-badge="delivery"]');
  const taxBadge = document.querySelector('[data-profile-badge="tax"]');
  const role = document.querySelector('[data-profile-role]')?.value || '';
  const city = (document.querySelector('[data-profile-city]')?.value || '').replace(/市$/, '');
  const district = (document.querySelector('[data-profile-district]')?.value || '').replace(/区$/, '');
  const delivery = document.querySelector('[data-profile-delivery]')?.value || '';
  const tax = document.querySelector('[data-profile-tax]')?.value || '';
  if (roleBadge && role) roleBadge.textContent = role;
  if (locationBadge) locationBadge.textContent = [city, district].filter(Boolean).join(' · ') || '未选择地区';
  if (deliveryBadge && delivery) deliveryBadge.textContent = delivery;
  if (taxBadge && tax) taxBadge.textContent = tax;
}

document.querySelectorAll('[data-profile-city], [data-profile-district], [data-profile-delivery], [data-profile-tax]').forEach((select) => {
  select.addEventListener('change', syncProfileBadges);
});
syncProfileBadges();

document.querySelectorAll('[data-bind-contact]').forEach((input) => {
  const type = input.getAttribute('data-bind-contact');
  const status = document.querySelector(`[data-bind-status="${type}"]`);
  const update = () => {
    const bound = Boolean(input.value.trim());
    if (!status) return;
    status.textContent = bound ? '已绑定' : '未绑定';
    status.classList.toggle('bound', bound);
    status.classList.toggle('unbound', !bound);
  };
  input.addEventListener('input', update);
  update();
});

document.querySelectorAll('[data-profile-save]').forEach((button) => {
  button.addEventListener('click', () => {
    const address = document.querySelector('[data-required-address]');
    if (address && !address.value.trim()) {
      address.classList.add('input-error-shake');
      showProfileToast('请填写详细地址');
      setTimeout(() => address.classList.remove('input-error-shake'), 500);
      address.focus();
      return;
    }
    showProfileToast('资料已保存');
  });
});

document.querySelectorAll('[data-profile-short-name]').forEach((input) => {
  const avatar = document.querySelector('[data-profile-avatar]');
  const updateAvatar = () => {
    const shortName = input.value.trim();
    avatar.textContent = shortName ? shortName[0] : 'MLCC';
    avatar.classList.toggle('is-default', !shortName);
  };
  input.addEventListener('input', updateAvatar);
  updateAvatar();
});

const PROFILE_BRANDS = ['Murata', 'Yageo', 'Samsung', 'TDK', 'Vishay', '风华高科', 'Panasonic', 'Walsin', 'Taiyo Yuden', 'AVX'];

document.querySelectorAll('[data-brand-picker]').forEach((picker) => {
  const search = picker.querySelector('[data-brand-search]');
  const dropdown = picker.querySelector('[data-brand-dropdown]');
  const selectedBox = picker.querySelector('[data-brand-selected]');
  const selected = new Set(['Murata', 'Yageo', 'Samsung']);
  const renderSelected = () => {
    if (!selectedBox) return;
    selectedBox.innerHTML = [...selected].map((brand) => `<button type="button" data-brand-remove="${brand}">${brand}<span>×</span></button>`).join('');
  };
  const renderDropdown = () => {
    if (!dropdown || !search) return;
    const keyword = search.value.trim().toLowerCase();
    const list = PROFILE_BRANDS.filter((brand) => brand.toLowerCase().includes(keyword));
    dropdown.innerHTML = list.map((brand) => {
      const checked = selected.has(brand);
      return `<button type="button" data-brand-option="${brand}" class="${checked ? 'checked' : ''}"><span>${checked ? '✓' : '+'}</span>${brand}</button>`;
    }).join('');
    dropdown.hidden = false;
  };
  search?.addEventListener('focus', renderDropdown);
  search?.addEventListener('input', renderDropdown);
  picker.addEventListener('click', (event) => {
    const option = event.target.closest('[data-brand-option]');
    const remove = event.target.closest('[data-brand-remove]');
    if (option) {
      const brand = option.getAttribute('data-brand-option');
      if (selected.has(brand)) selected.delete(brand);
      else if (selected.size >= 6) showProfileToast('主营品牌最多选择 6 个');
      else selected.add(brand);
      renderSelected();
      renderDropdown();
      return;
    }
    if (remove) {
      selected.delete(remove.getAttribute('data-brand-remove'));
      renderSelected();
      renderDropdown();
    }
  });
  document.addEventListener('click', (event) => {
    if (!picker.contains(event.target) && dropdown) dropdown.hidden = true;
  });
  renderSelected();
});

document.querySelectorAll('[data-admin-tab]').forEach((button) => {
  button.addEventListener('click', () => {
    const target = button.getAttribute('data-admin-tab');
    document.querySelectorAll('[data-admin-tab]').forEach((item) => item.classList.toggle('active', item === button));
    document.querySelectorAll('[data-admin-view]').forEach((panel) => {
      const active = panel.getAttribute('data-admin-view') === target;
      panel.hidden = !active;
      panel.classList.toggle('active', active);
    });
    document.querySelectorAll('[data-admin-view]:not([hidden]) table[data-admin-page-size]').forEach((table) => updateAdminPagination(table));
  });
});

function parseAdminNumber(text) {
  const match = String(text || '').replace(/,/g, '').match(/\d+(?:\.\d+)?/);
  return match ? Number(match[0]) : 0;
}

document.addEventListener('click', (event) => {
  const sortButton = event.target.closest('[data-sort-table]');
  if (!sortButton) return;
  const table = sortButton.closest('table');
  const tbody = table?.querySelector('tbody');
  if (!table || !tbody) return;
  const col = Number(sortButton.getAttribute('data-sort-col'));
  const nextDirection = sortButton.dataset.sortDirection === 'desc' ? 'asc' : 'desc';
  sortButton.dataset.sortDirection = nextDirection;
  const rows = [...tbody.querySelectorAll('tr')].filter((row) => !row.classList.contains('admin-quote-detail-row'));
  rows.sort((a, b) => {
    const av = parseAdminNumber(a.children[col]?.textContent || '');
    const bv = parseAdminNumber(b.children[col]?.textContent || '');
    return nextDirection === 'desc' ? bv - av : av - bv;
  });
  rows.forEach((row) => tbody.appendChild(row));
  sortButton.textContent = sortButton.textContent.replace(/[↑↓↕]/g, nextDirection === 'desc' ? '↓' : '↑');
  updateAdminPagination(table);
});

document.addEventListener('click', (event) => {
  const qqBtn = event.target.closest('.qq-contact');
  if (!qqBtn) return;
  event.stopPropagation();
  window.alert(`正在通过 QQ 联系：${qqBtn.dataset.qq || '未填写 QQ'}`);
});

document.addEventListener('click', async (event) => {
  const copySpan = event.target.closest('.rank-list span');
  if (!copySpan) return;
  const text = copySpan.textContent.trim();
  if (!text) return;
  try {
    if (navigator.clipboard?.writeText) {
      await navigator.clipboard.writeText(text);
    } else {
      const input = document.createElement('input');
      input.value = text;
      input.style.position = 'fixed';
      input.style.opacity = '0';
      document.body.appendChild(input);
      input.select();
      document.execCommand('copy');
      input.remove();
    }
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = `已复制：${text}`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1200);
  } catch (err) {
    window.alert(`复制内容：${text}`);
  }
});

function ensureAdminPasswordModal() {
  let modal = document.getElementById('adminPasswordModal');
  if (modal) return modal;
  modal = document.createElement('div');
  modal.className = 'modal-backdrop';
  modal.id = 'adminPasswordModal';
  modal.hidden = true;
  modal.innerHTML = `
    <div class="modal admin-password-modal" role="dialog" aria-modal="true" aria-label="管理员密码确认">
      <div class="modal-head"><h2>管理员密码确认</h2><button class="modal-close" type="button" data-admin-password-cancel aria-label="关闭">×</button></div>
      <div class="modal-body">
        <input class="input" type="password" data-admin-password-input placeholder="请输入管理员密码">
        <div class="actions" style="margin-top:14px;justify-content:flex-end">
          <button class="btn" type="button" data-admin-password-cancel>取消</button>
          <button class="btn primary" type="button" data-admin-password-confirm>确认</button>
        </div>
      </div>
    </div>`;
  document.body.appendChild(modal);
  return modal;
}

function requestAdminPassword(action, onPass) {
  const modal = ensureAdminPasswordModal();
  const input = modal.querySelector('[data-admin-password-input]');
  input.value = '';
  modal.removeAttribute('hidden');
  setTimeout(() => input.focus(), 30);
  const cleanup = () => {
    modal.setAttribute('hidden', '');
    modal._onPass = null;
  };
  modal._onPass = () => {
    if (input.value.trim().length < 6) {
      markInteractionInputError(input);
      return;
    }
    cleanup();
    onPass();
  };
}

document.addEventListener('click', (event) => {
  if (event.target.closest('[data-admin-password-cancel]')) {
    document.getElementById('adminPasswordModal')?.setAttribute('hidden', '');
  }
  if (event.target.closest('[data-admin-password-confirm]')) {
    document.getElementById('adminPasswordModal')?._onPass?.();
  }
});

document.addEventListener('keydown', (event) => {
  if (event.key !== 'Enter') return;
  const modal = document.getElementById('adminPasswordModal');
  if (!modal || modal.hidden || !event.target.closest('#adminPasswordModal')) return;
  modal._onPass?.();
});

function ensureAdminProfileModal() {
  let modal = document.getElementById('adminProfileModal');
  if (modal) return modal;
  modal = document.createElement('div');
  modal.className = 'modal-backdrop';
  modal.id = 'adminProfileModal';
  modal.hidden = true;
  modal.innerHTML = `
    <div class="modal admin-profile-modal" role="dialog" aria-modal="true" aria-label="会员资料编辑">
      <div class="modal-head"><h2>会员资料编辑</h2><button class="modal-close" type="button" data-admin-profile-close aria-label="关闭">×</button></div>
      <div class="modal-body">
        <div class="admin-profile-form">
          <section>
            <h3>注册信息</h3>
            <div class="form-grid compact-form-grid">
              <div><label>会员 ID</label><input class="input" data-admin-profile-field="memberId" readonly></div>
              <div><label>注册时间</label><input class="input" data-admin-profile-field="registeredAt"></div>
              <div><label>最近登录（时间 IP）</label><input class="input" data-admin-profile-field="loginLog" placeholder="2026-06-12 17:12 113.87.22.18"></div>
              <div><label>状态</label><select class="select" data-admin-profile-field="status"><option>启用</option><option>禁用</option></select></div>
            </div>
          </section>
          <section>
            <h3>联系人信息</h3>
            <div class="form-grid compact-form-grid">
              <div><label>姓名</label><input class="input" data-admin-profile-field="contact"></div>
              <div><label>手机号</label><input class="input" data-admin-profile-field="phone" inputmode="numeric"></div>
              <div><label>微信 / QQ</label><input class="input" data-admin-profile-field="qq"></div>
              <div><label>职位</label><input class="input" data-admin-profile-field="position"></div>
            </div>
          </section>
          <section>
            <h3>公司资料</h3>
            <div class="form-grid compact-form-grid">
              <div><label>公司名称</label><input class="input" data-admin-profile-field="company"></div>
              <div><label>公司简称</label><input class="input" data-admin-profile-field="shortName"></div>
              <div><label>所在城市</label><input class="input" data-admin-profile-field="city"></div>
              <div><label>经营身份</label><select class="select" data-admin-profile-field="role"><option>采购商</option><option>供应商</option><option>供应商 + 采购商</option></select></div>
              <div class="full"><label>公司地址</label><input class="input" data-admin-profile-field="address"></div>
              <div class="full"><label>公司简介</label><textarea class="profile-textarea" data-admin-profile-field="intro"></textarea></div>
            </div>
          </section>
          <section>
            <h3>经营能力</h3>
            <div class="form-grid compact-form-grid">
              <div><label>主营品类</label><input class="input" data-admin-profile-field="categories"></div>
              <div><label>交易方式</label><select class="select" data-admin-profile-field="tradeMode"><option>现货交易</option><option>期货订货</option><option>现货 + 订货</option></select></div>
              <div><label>发货时效</label><select class="select" data-admin-profile-field="delivery"><option>当天可发</option><option>次日可发</option><option>按订单确认</option></select></div>
              <div><label>是否支持含税</label><select class="select" data-admin-profile-field="taxSupport"><option>支持含税 / 未税</option><option>只支持含税</option><option>只支持未税</option></select></div>
              <div class="full"><label>主营品牌</label><input class="input" data-admin-profile-field="brands"></div>
              <div class="full"><label>优势型号</label><textarea class="profile-textarea small" data-admin-profile-field="models"></textarea></div>
            </div>
          </section>
          <section>
            <h3>认证与形象资料</h3>
            <div class="admin-cert-grid">
              <div class="admin-cert-card" data-cert-card="license"><b>营业执照</b><span>已有图片时显示预览缩略图</span><i>证</i><select class="select" data-admin-profile-field="license"><option>未上传</option><option>已上传</option><option>已审核</option></select></div>
              <div class="admin-cert-card" data-cert-card="officePhoto"><b>公司门头 / 办公室</b><span>已有图片时显示预览缩略图</span><i>门</i><select class="select" data-admin-profile-field="officePhoto"><option>未上传</option><option>已上传</option><option>已审核</option></select></div>
              <div class="admin-cert-card" data-cert-card="warehousePhoto"><b>仓库 / 库存照片</b><span>已有图片时显示预览缩略图</span><i>仓</i><select class="select" data-admin-profile-field="warehousePhoto"><option>未上传</option><option>已上传</option><option>已审核</option></select></div>
              <div class="admin-cert-card" data-cert-card="authDoc"><b>授权 / 代理证明</b><span>已有图片时显示预览缩略图</span><i>授</i><select class="select" data-admin-profile-field="authDoc"><option>未上传</option><option>已上传</option><option>已审核</option></select></div>
            </div>
          </section>
        </div>
        <div class="actions admin-profile-actions">
          <button class="btn" type="button" data-admin-profile-close>取消</button>
          <button class="btn primary" type="button" data-admin-profile-save>保存资料</button>
        </div>
      </div>
    </div>`;
  document.body.appendChild(modal);
  return modal;
}

function getAdminMemberProfile(row) {
  const cells = row.querySelectorAll('td');
  const company = row.dataset.company || cells[1]?.textContent.trim() || '';
  return {
    memberId: cells[0]?.textContent.trim() || '',
    company,
    contact: row.dataset.contact || '',
    phone: row.dataset.phone || '',
    qq: row.dataset.qq || '',
    address: row.dataset.address || '',
    registeredAt: cells[3]?.textContent.trim() || '',
    loginLog: cells[4]?.textContent.trim().replace(/\s+/g, ' ') || '',
    status: cells[7]?.textContent.trim() || '启用',
    position: company.includes('智造') ? '采购经理' : company.includes('精密') ? '供应链经理' : '销售经理',
    shortName: company.includes('华南') ? '华南现货' : company.includes('智造') ? '东莞智造' : '苏州精密',
    city: company.includes('深圳') ? '深圳 · 福田' : company.includes('东莞') ? '东莞 · 松山湖' : '苏州 · 工业园',
    role: row.dataset.role || localStorage.getItem('zrDefaultMemberType') || '采购商',
    intro: '主营村田、国巨、三星被动件，支持现货快速交付，可提供原装追溯资料。',
    categories: '电容、电阻',
    tradeMode: '现货交易',
    delivery: '当天可发',
    taxSupport: '支持含税 / 未税',
    brands: 'Murata, Yageo, Samsung',
    models: 'GRM188R71H104KA93D、RC0603FR-0710KL、CL10B104KB8NNNC',
    license: company.includes('华南') ? '已审核' : '未上传',
    officePhoto: company.includes('华南') ? '已上传' : '未上传',
    warehousePhoto: company.includes('华南') ? '已上传' : '未上传',
    authDoc: '未上传',
  };
}

function fillAdminProfileModal(modal, data) {
  modal.querySelectorAll('[data-admin-profile-field]').forEach((field) => {
    const key = field.getAttribute('data-admin-profile-field');
    field.value = data[key] || '';
  });
  modal.querySelectorAll('.admin-cert-card').forEach((card) => {
    const key = card.getAttribute('data-cert-card');
    const value = data[key] || '未上传';
    card.classList.toggle('has-image', value !== '未上传');
  });
}

document.addEventListener('change', (event) => {
  const select = event.target.closest('[data-admin-profile-field]');
  if (!select) return;
  const card = select.closest('.admin-cert-card');
  if (!card) return;
  card.classList.toggle('has-image', select.value !== '未上传');
});

document.addEventListener('click', (event) => {
  const companyCell = event.target.closest('.admin-member-row td:nth-child(2)');
  if (!companyCell) return;
  const row = companyCell.closest('.admin-member-row');
  const modal = ensureAdminProfileModal();
  modal._memberRow = row;
  fillAdminProfileModal(modal, getAdminMemberProfile(row));
  modal.removeAttribute('hidden');
});

document.addEventListener('click', (event) => {
  if (event.target.closest('[data-admin-profile-close]')) {
    document.getElementById('adminProfileModal')?.setAttribute('hidden', '');
  }
  const save = event.target.closest('[data-admin-profile-save]');
  if (!save) return;
  const modal = document.getElementById('adminProfileModal');
  const row = modal?._memberRow;
  if (!modal || !row) return;
  requestAdminPassword('保存会员资料', () => {
    const getValue = (key) => modal.querySelector(`[data-admin-profile-field="${key}"]`)?.value.trim() || '';
    row.dataset.company = getValue('company');
    row.dataset.contact = getValue('contact');
    row.dataset.phone = getValue('phone');
    row.dataset.qq = getValue('qq');
    row.dataset.address = getValue('address');
    row.dataset.role = getValue('role');
    const cells = row.querySelectorAll('td');
    if (cells[1]) cells[1].textContent = getValue('company');
    if (cells[2]) cells[2].innerHTML = `${getValue('contact')}<br>${getValue('phone')}<br>${getValue('qq')}`;
    if (cells[3]) cells[3].textContent = getValue('registeredAt');
    if (cells[4]) cells[4].textContent = getValue('loginLog');
    if (cells[7]) cells[7].innerHTML = getValue('status') === '禁用' ? '<span class="tag gray">禁用</span>' : '<span class="tag green">启用</span>';
    modal.setAttribute('hidden', '');
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = '会员资料已保存';
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1200);
  });
});

function ensureDefaultMemberModal() {
  let modal = document.getElementById('defaultMemberModal');
  if (modal) return modal;
  modal = document.createElement('div');
  modal.className = 'modal-backdrop';
  modal.id = 'defaultMemberModal';
  modal.hidden = true;
  modal.innerHTML = `
    <div class="modal default-member-modal" role="dialog" aria-modal="true" aria-label="新注册会员入口身份">
      <div class="modal-head"><h2>新注册会员入口身份</h2><button class="modal-close" type="button" data-default-member-close aria-label="关闭">×</button></div>
      <div class="modal-body">
        <div class="default-member-options">
          <button type="button" data-default-member-type="采购商"><b>1</b><span>采购</span></button>
          <button type="button" data-default-member-type="供应商"><b>2</b><span>供应</span></button>
          <button type="button" data-default-member-type="供应商 + 采购商"><b>3</b><span>供应商 + 采购商</span></button>
        </div>
      </div>
    </div>`;
  document.body.appendChild(modal);
  return modal;
}

function refreshDefaultMemberModal(modal) {
  const current = localStorage.getItem('zrDefaultMemberType') || '采购商';
  modal.querySelectorAll('[data-default-member-type]').forEach((button) => {
    button.classList.toggle('active', button.getAttribute('data-default-member-type') === current);
  });
}

document.addEventListener('click', (event) => {
  if (event.target.closest('[data-default-member-open]')) {
    requestAdminPassword('打开新注册会员入口身份设置', () => {
      const modal = ensureDefaultMemberModal();
      refreshDefaultMemberModal(modal);
      modal.removeAttribute('hidden');
    });
    return;
  }
  if (event.target.closest('[data-default-member-close]')) {
    document.getElementById('defaultMemberModal')?.setAttribute('hidden', '');
    return;
  }
  const typeButton = event.target.closest('[data-default-member-type]');
  if (!typeButton) return;
  const type = typeButton.getAttribute('data-default-member-type') || '采购商';
  localStorage.setItem('zrDefaultMemberType', type);
  refreshDefaultMemberModal(typeButton.closest('#defaultMemberModal'));
  const toast = document.createElement('div');
  toast.className = 'publish-toast';
  toast.textContent = `设置后新注册用户入口身份：${type}`;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 1200);
});

document.addEventListener('click', (event) => {
  const row = event.target.closest('.admin-quote-row');
  if (!row || event.target.closest('button')) return;
  const next = row.nextElementSibling;
  if (next?.classList.contains('admin-quote-detail-row')) {
    next.remove();
    row.classList.remove('is-open');
    return;
  }
  row.parentElement?.querySelectorAll('.admin-quote-detail-row').forEach((item) => item.remove());
  row.parentElement?.querySelectorAll('.admin-quote-row.is-open').forEach((item) => item.classList.remove('is-open'));
  row.classList.add('is-open');
  const status = row.getAttribute('data-status') || '启用';
  const action = status === '禁用' ? '启用该会员' : '禁用该会员';
  const buttonClass = status === '禁用' ? 'btn primary mini' : 'btn soft mini';
  const tradePrice = row.children[3]?.textContent.trim() || '-';
  const tradeQty = row.children[4]?.textContent.trim() || '-';
  const taxText = tradePrice.includes('未税') ? '未税' : tradePrice.includes('含税') ? '含税' : '税别未标注';
  const detail = document.createElement('tr');
  detail.className = 'admin-quote-detail-row';
  detail.innerHTML = `
    <td colspan="7">
      <div class="admin-quote-detail-card">
        <div class="party-card supplier-party"><i>供</i><b>供应方</b><strong>${row.dataset.company || '-'} <em>发布采购 ${row.dataset.purchase || '0'}</em><em>发布供应 ${row.dataset.supply || '0'}</em></strong><span>注册时间：${row.dataset.register || '-'}</span><span>${row.dataset.contact || '-'} · ${row.dataset.phone || '-'}</span><small>${row.dataset.address || '-'}</small><button type="button" class="qq-contact" data-qq="${row.dataset.qq || ''}"><i>QQ</i> 联系供应方</button></div>
        <div class="party-card buyer-party"><i>采</i><b>采购方</b><strong>${row.dataset.buyerCompany || row.children[2]?.textContent.trim() || '-'} <em>发布采购 ${row.dataset.buyerPurchase || row.dataset.purchase || '0'}</em><em>发布供应 ${row.dataset.buyerSupply || row.dataset.supply || '0'}</em></strong><span>注册时间：${row.dataset.buyerRegister || row.dataset.register || '-'}</span><span>${row.dataset.buyerContact || '-'} · ${row.dataset.buyerPhone || '-'}</span><small>${row.dataset.buyerAddress || '-'}</small><button type="button" class="qq-contact" data-qq="${row.dataset.buyerQq || ''}"><i>QQ</i> 联系采购方</button><div class="buyer-trade-meta"><span>需采购数量 ${tradeQty}</span><span>期望价 ${tradePrice.replace(/ · (含税|未税)/, '')}</span><span>${taxText}</span></div></div>
      </div>
    </td>`;
  row.insertAdjacentElement('afterend', detail);
});

document.addEventListener('click', (event) => {
  const actionButton = event.target.closest('[data-password-action]');
  if (!actionButton) return;
  const action = actionButton.getAttribute('data-password-action') || '执行该操作';
  requestAdminPassword(action, () => {
  const row = actionButton.closest('tr');
  const detailRow = actionButton.closest('.admin-quote-detail-row');
  const quoteRow = detailRow?.previousElementSibling?.classList.contains('admin-quote-row') ? detailRow.previousElementSibling : null;
  if (quoteRow && /禁用该会员/.test(action)) {
    quoteRow.dataset.status = '禁用';
    actionButton.textContent = '启用';
    actionButton.setAttribute('data-password-action', '启用该会员');
    actionButton.classList.remove('soft');
    actionButton.classList.add('primary');
  } else if (quoteRow && /启用该会员/.test(action)) {
    quoteRow.dataset.status = '启用';
    actionButton.textContent = '禁用';
    actionButton.setAttribute('data-password-action', '禁用该会员');
    actionButton.classList.remove('primary');
    actionButton.classList.add('soft');
  } else if (row && /禁用该会员/.test(action)) {
    const status = row.querySelector('td:nth-child(8)');
    if (status) status.innerHTML = '<span class="tag gray">禁用</span>';
    actionButton.textContent = '启用';
    actionButton.setAttribute('data-password-action', '启用该会员');
    actionButton.classList.remove('soft');
    actionButton.classList.add('primary');
  } else if (row && /启用该会员/.test(action)) {
    const status = row.querySelector('td:nth-child(8)');
    if (status) status.innerHTML = '<span class="tag green">启用</span>';
    actionButton.textContent = '禁用';
    actionButton.setAttribute('data-password-action', '禁用该会员');
    actionButton.classList.remove('primary');
    actionButton.classList.add('soft');
  } else if (/下架/.test(action)) {
    const checked = [...document.querySelectorAll('[data-admin-supply-list] input[type="checkbox"]:checked')];
    if (!checked.length) {
      window.alert('请先勾选需要下架的供需信息');
      return;
    }
    checked.forEach((checkbox) => {
      const row = checkbox.closest('tr');
      if (row) {
        row.classList.add('is-offline');
        const status = row.querySelector('td:last-child');
        if (status) status.innerHTML = '<span class="tag gray">已下架</span>';
        checkbox.checked = false;
        checkbox.disabled = true;
      }
    });
  }
  const toast = document.createElement('div');
  toast.className = 'publish-toast';
  toast.textContent = `${action}已完成`;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 1200);
  });
});

function renderAdminKline(panel, seed = 0) {
  const poly = panel?.querySelector('[data-k-poly]');
  const candles = panel?.querySelector('[data-k-candles]');
  if (!poly || !candles) return;
  const values = Array.from({ length: 13 }, (_, index) => {
    const wave = Math.sin((index + seed) * .9) * 34;
    const trend = index * (7 + seed);
    return Math.max(38, Math.min(245, 220 - trend + wave));
  });
  const points = values.map((value, index) => `${50 + index * 46},${Math.round(value)}`).join(' ');
  poly.setAttribute('points', points);
  candles.innerHTML = values.slice(1, 7).map((value, index) => {
    const x = 90 + index * 90;
    const high = Math.max(42, value - 34);
    const low = Math.min(258, value + 38);
    const open = Math.max(50, value - 14);
    const close = Math.min(252, value + 18);
    return `<path d="M${x} ${Math.round(high)}V${Math.round(low)}M${x - 12} ${Math.round(open)}H${x + 12}V${Math.round(close)}H${x - 12}Z"></path>`;
  }).join('');
}

function applyRankPage(panel, page) {
  panel.dataset.rankPage = String(page);
  panel.querySelectorAll('[data-rank-page]').forEach((row) => {
    row.hidden = row.getAttribute('data-rank-page') !== String(page);
  });
}

document.querySelectorAll('[data-rank-page-switch]').forEach((button) => {
  button.addEventListener('click', () => {
    const panel = button.closest('.admin-panel');
    const nextPage = panel?.dataset.rankPage === '2' ? 1 : 2;
    applyRankPage(panel, nextPage);
    button.textContent = nextPage === 1 ? 'TOP1-10 ⇄ TOP11-20' : 'TOP11-20 ⇄ TOP1-10';
  });
});

document.querySelectorAll('.admin-rank-tabs button').forEach((button) => {
  button.addEventListener('click', () => {
    const tabs = button.closest('.admin-rank-tabs');
    tabs?.querySelectorAll('button').forEach((item) => item.classList.remove('active'));
    button.classList.add('active');
    const panel = button.closest('.admin-panel');
    const label = button.textContent.trim();
    const heatBase = { '12小时': 98, '24小时': 94, '1周': 88, '15天': 82, '30天': 76 }[label] || 90;
    panel?.querySelectorAll('.rank-list div:not([hidden])').forEach((row, index) => {
      const heat = Math.max(42, heatBase - index * 4);
      const em = row.querySelector('em');
      if (em) em.textContent = `热度 ${heat}`;
    });
    renderAdminKline(panel, { '12小时': 0, '24小时': 1, '1周': 2, '15天': 3, '30天': 4 }[label] || 0);
  });
});

document.querySelectorAll('.admin-panel').forEach((panel) => {
  if (panel.querySelector('[data-rank-page]')) {
    applyRankPage(panel, 1);
    renderAdminKline(panel, 0);
  }
});

document.querySelectorAll('[data-tab]').forEach((tab) => {
  tab.addEventListener('click', () => {
    const group = tab.parentElement;
    group.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
    tab.classList.add('active');

    const filter = tab.getAttribute('data-filter');
    const panel = group.closest('.panel');
    if (!filter || !panel) return;

    panel.querySelectorAll('[data-type]').forEach((item) => {
      item.classList.toggle('is-hidden', filter !== 'all' && item.getAttribute('data-type') !== filter);
    });
  });
});

document.querySelectorAll('[data-feed-tabs]').forEach((group) => {
  const activeTab = group.querySelector('.tab.active[data-filter]');
  if (activeTab) activeTab.click();
});

document.querySelectorAll('[data-feed-list]').forEach((list) => {
  const panel = list.closest('.panel');
  if (!panel) return;
  const totalEl = panel.querySelector('[data-feed-total]');
  const refreshTotal = () => {
    if (!totalEl) return;
    totalEl.textContent = String(list.querySelectorAll('.item:not(.is-hidden)').length);
  };
  panel.querySelectorAll('[data-feed-tabs] [data-tab]').forEach((tab) => {
    tab.addEventListener('click', () => setTimeout(refreshTotal, 0));
  });
  refreshTotal();
});

document.querySelectorAll('[data-toggle-stock]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const item = btn.closest('.inventory-item');
    if (!item) return;
    const offline = btn.textContent.trim() === '下架';
    item.classList.toggle('is-offline', offline);
    const tag = item.querySelector('.tag');
    if (tag) {
      tag.classList.toggle('blue', !offline);
      tag.classList.toggle('orange', offline);
      tag.textContent = offline ? '已下架' : '供应';
    }
  });
});

document.querySelectorAll('[data-select-all]').forEach((checkbox) => {
  checkbox.addEventListener('change', () => {
    const panel = checkbox.closest('.panel');
    if (!panel) return;
    panel.querySelectorAll('.inventory-item input[type="checkbox"]').forEach((itemCheckbox) => {
      itemCheckbox.checked = checkbox.checked;
    });
    panel.querySelectorAll('[data-select-all]').forEach((otherCheckbox) => {
      otherCheckbox.checked = checkbox.checked;
    });
  });
});

document.querySelectorAll('[data-toggle-expired]').forEach((btn) => {
  btn.addEventListener('click', () => {
    const panel = document.getElementById('expiredPanel');
    if (!panel) return;
    const willShow = panel.hasAttribute('hidden');
    if (willShow) {
      panel.removeAttribute('hidden');
      panel.scrollIntoView({ behavior: 'smooth', block: 'start' });
    } else {
      panel.setAttribute('hidden', '');
    }
  });
});

function focusPanel(panel) {
  if (!panel) return;
  panel.removeAttribute('hidden');
  panel.scrollIntoView({ behavior: 'smooth', block: 'start' });
  panel.classList.remove('panel-focus-pulse');
  void panel.offsetWidth;
  panel.classList.add('panel-focus-pulse');
}

document.querySelectorAll('[data-demand-stat]').forEach((btn) => {
  btn.addEventListener('click', () => focusPanel(document.getElementById('demandPanel')));
});

document.querySelectorAll('[data-expired-stat]').forEach((btn) => {
  btn.addEventListener('click', () => focusPanel(document.getElementById('expiredPanel')));
});

document.addEventListener('click', (event) => {
  const toggle = event.target.closest('[data-tax-toggle]');
  if (!toggle) return;
  const scope = toggle.closest('tr') || toggle.closest('.tax-inline') || toggle.parentElement;
  const priceField = scope?.querySelector('.price-field');
  const label = priceField?.querySelector('span');
  if (!priceField || !label) return;
  const checked = toggle.getAttribute('aria-pressed') !== 'true';
  toggle.setAttribute('aria-pressed', checked ? 'true' : 'false');
  toggle.classList.toggle('is-on', checked);
  priceField.classList.toggle('is-taxed', checked);
  priceField.classList.toggle('is-untaxed', !checked);
  label.textContent = checked ? '含税' : '未税';
});

function searchInventory(input) {
  const keyword = input.value.trim().toLowerCase();
  document.querySelectorAll('.inventory-table tbody tr').forEach((row) => {
    row.classList.toggle('is-hidden', Boolean(keyword) && !row.textContent.toLowerCase().includes(keyword));
  });
  document.querySelectorAll('.inventory-table').forEach((table) => {
    table.closest('.panel')?.querySelector('.pagination')?._updateBasicPagination?.(1);
  });
}

document.querySelectorAll('[data-inventory-search]').forEach((input) => {
  input.addEventListener('keydown', (event) => {
    if (event.key === 'Enter') searchInventory(input);
  });
});

document.addEventListener('click', (event) => {
  const tab = event.target.closest('[data-admin-tab]');
  if (!tab) return;
  const panel = tab.closest('.panel');
  const filter = tab.dataset.adminTab || 'all';
  panel?.querySelectorAll('[data-admin-tab]').forEach((btn) => btn.classList.toggle('active', btn === tab));
  panel?.querySelectorAll('[data-admin-module]').forEach((card) => {
    card.classList.toggle('is-hidden', filter !== 'all' && card.dataset.adminModule !== filter);
  });
});

document.addEventListener('click', (event) => {
  const action = event.target.closest('[data-admin-member-action]');
  if (!action) return;
  const row = action.closest('tr');
  const status = row?.querySelector('[data-member-status]');
  const type = action.dataset.adminMemberAction;
  if (!status) return;
  const company = row?.querySelector('.admin-member-company b, td b')?.textContent.trim() || '该会员';
  status.classList.remove('green', 'orange', 'gray');
  if (type === 'approve' || type === 'enable') {
    status.textContent = '已通过';
    status.classList.add('green');
    row.dataset.status = '已通过';
    action.textContent = '禁用';
    action.dataset.adminMemberAction = 'disable';
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = `${company} 已启用/通过，操作已写入日志`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1400);
  } else {
    status.textContent = '已禁用';
    status.classList.add('gray');
    row.dataset.status = '已禁用';
    action.textContent = '启用';
    action.dataset.adminMemberAction = 'enable';
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = `${company} 已禁用，发布和互动权限将被限制`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1400);
  }
  applyAdminMemberFilter();
});

document.addEventListener('click', (event) => {
  const action = event.target.closest('[data-admin-whitelist-action]');
  if (!action) return;
  const row = action.closest('[data-member-row]');
  const status = row?.querySelector('[data-member-status]')?.textContent.trim() || '';
  const whitelist = row?.querySelector('[data-whitelist-status]');
  const company = row?.querySelector('.admin-member-company b, td b')?.textContent.trim() || '该会员';
  if (!row || !whitelist) return;
  if (status !== '已通过') {
    const toast = document.createElement('div');
    toast.className = 'publish-toast';
    toast.textContent = `${company} 未通过认证，不能加入免审白名单`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 1400);
    return;
  }
  const enabled = row.dataset.whitelist === 'true';
  row.dataset.whitelist = enabled ? 'false' : 'true';
  whitelist.classList.remove('green', 'orange', 'gray');
  if (enabled) {
    whitelist.textContent = '需审核';
    whitelist.classList.add('orange');
    action.textContent = '白名单';
    action.classList.remove('active');
  } else {
    whitelist.textContent = '免审';
    whitelist.classList.add('green');
    action.textContent = '移出';
    action.classList.add('active');
  }
  const toast = document.createElement('div');
  toast.className = 'publish-toast';
  toast.textContent = enabled ? `${company} 已移出白名单，发布恢复人工审核` : `${company} 已加入白名单，发布默认免人工审核`;
  document.body.appendChild(toast);
  setTimeout(() => toast.remove(), 1400);
});

function applyAdminMemberFilter() {
  const root = document.querySelector('.admin-dashboard');
  if (!root) return;
  const keyword = (root.querySelector('[data-admin-member-search]')?.value || '').trim().toLowerCase();
  const role = root.querySelector('[data-admin-member-role]')?.value || 'all';
  const status = root.querySelector('[data-admin-member-status-filter]')?.value || 'all';
  root.querySelectorAll('[data-member-row]').forEach((row) => {
    const text = row.textContent.toLowerCase();
    const okKeyword = !keyword || text.includes(keyword);
    const okRole = role === 'all' || row.dataset.role === role;
    const rowStatus = row.querySelector('[data-member-status]')?.textContent.trim() || row.dataset.status || '';
    const okStatus = status === 'all' || rowStatus === status;
    row.classList.toggle('is-hidden', !(okKeyword && okRole && okStatus));
  });
}

document.querySelectorAll('[data-admin-member-search], [data-admin-member-role], [data-admin-member-status-filter]').forEach((item) => {
  item.addEventListener('input', applyAdminMemberFilter);
  item.addEventListener('change', applyAdminMemberFilter);
});

document.addEventListener('click', (event) => {
  const reset = event.target.closest('[data-admin-reset-filter]');
  if (!reset) return;
  const root = reset.closest('.panel');
  root?.querySelectorAll('[data-admin-member-search]').forEach((input) => input.value = '');
  root?.querySelectorAll('[data-admin-member-role], [data-admin-member-status-filter]').forEach((select) => select.value = 'all');
  applyAdminMemberFilter();
});

document.addEventListener('click', (event) => {
  const detail = event.target.closest('[data-admin-member-detail]');
  if (!detail) return;
  const row = detail.closest('[data-member-row]');
  const inspector = document.querySelector('[data-member-inspector]');
  if (!row || !inspector) return;
  const cells = row.querySelectorAll('td');
  inspector.querySelector('[data-inspector-company]').textContent = cells[0]?.querySelector('b')?.textContent.trim() || '会员详情';
  inspector.querySelector('[data-inspector-contact]').textContent = cells[0]?.querySelector('small')?.textContent.trim() || '联系人未填写';
  inspector.querySelector('[data-inspector-role]').textContent = cells[1]?.textContent.trim() || '-';
  inspector.querySelector('[data-inspector-status]').textContent = cells[2]?.textContent.trim() || '-';
  inspector.querySelector('[data-inspector-login]').textContent = cells[3]?.textContent.trim() || '-';
  inspector.querySelector('[data-inspector-ip]').textContent = cells[4]?.textContent.trim() || '-';
  inspector.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
});

document.addEventListener('click', (event) => {
  const filter = event.target.closest('[data-admin-audit-filter]');
  if (!filter) return;
  const panel = filter.closest('.panel');
  const value = filter.dataset.adminAuditFilter || 'all';
  panel?.querySelectorAll('[data-admin-audit-filter]').forEach((btn) => btn.classList.toggle('active', btn === filter));
  panel?.querySelectorAll('[data-audit-row]').forEach((row) => {
    row.classList.toggle('is-hidden', value !== 'all' && row.dataset.auditKind !== value);
  });
});

document.addEventListener('click', (event) => {
  const audit = event.target.closest('[data-audit-action]');
  if (!audit) return;
  const row = audit.closest('[data-audit-row]');
  if (!row) return;
  audit.textContent = '已通过';
  audit.disabled = true;
  row.style.opacity = '.72';
});

document.querySelectorAll('[data-inventory-search-btn]').forEach((button) => {
  button.addEventListener('click', () => {
    const input = button.closest('.searchbar')?.querySelector('[data-inventory-search]');
    if (input) searchInventory(input);
  });
});

function setupBasicPagination(pagination) {
  if (pagination.dataset.basicPaginationReady || pagination.querySelector('[data-quick-page-prev], [data-page-prev], [data-quote-page-prev]')) return;
  const buttons = [...pagination.querySelectorAll('button')];
  const prev = buttons.find((btn) => btn.textContent.includes('上一页'));
  const next = buttons.find((btn) => btn.textContent.includes('下一页'));
  const info = [...pagination.querySelectorAll('span, em')].find((el) => /第\s*\d+\s*\/\s*\d+\s*页/.test(el.textContent));
  if (!prev || !next || !info) return;
  const panel = pagination.closest('.panel') || document;
  const getItems = () => {
    if (panel.querySelector('.record-list')) return [...panel.querySelectorAll('.record-card')];
    if (panel.querySelector('.inquiry-list')) return [...panel.querySelectorAll('.inquiry-card')];
    const table = panel.querySelector('.inventory-table tbody');
    if (table) return [...table.querySelectorAll('tr')];
    return [];
  };
  let page = 1;
  const update = (nextPage = page) => {
    const pageSize = getResponsivePageSize();
    const items = getItems();
    const visibleItems = items.filter((item) => !item.classList.contains('is-hidden') && !item.hidden);
    const pageCount = Math.max(1, Math.ceil(visibleItems.length / pageSize));
    page = Math.min(Math.max(nextPage, 1), pageCount);
    const start = (page - 1) * pageSize;
    const end = start + pageSize;
    items.forEach((item) => item.classList.remove('is-page-hidden'));
    visibleItems.forEach((item, index) => item.classList.toggle('is-page-hidden', index < start || index >= end));
    info.textContent = `第 ${page} / ${pageCount} 页`;
    const pageSizeText = pagination.querySelector('.page-size');
    if (pageSizeText) pageSizeText.textContent = `每页 ${pageSize} 条`;
    pagination.hidden = visibleItems.length <= pageSize;
    prev.disabled = page <= 1;
    next.disabled = page >= pageCount;
  };
  prev.addEventListener('click', () => {
    if (page <= 1) return;
    update(page - 1);
  });
  next.addEventListener('click', () => {
    update(page + 1);
  });
  pagination._updateBasicPagination = update;
  pagination.dataset.basicPaginationReady = 'true';
  update();
}

document.querySelectorAll('.pagination').forEach(setupBasicPagination);
window.addEventListener('resize', () => {
  document.querySelectorAll('.pagination').forEach((pagination) => pagination._updateBasicPagination?.());
  document.querySelectorAll('.received-quotes').forEach((panel) => applyQuoteFilter(panel));
});

function applyAdminListFilter(scope) {
  if (!scope) return;
  const keyword = (scope.querySelector('[data-list-search]')?.value || '').trim().toLowerCase();
  const cat = scope.querySelector('[data-list-category]');
  const catVal = cat ? cat.value : 'all';
  const stat = scope.querySelector('[data-list-status]');
  const statVal = stat ? stat.value : 'all';
  const kindVal = scope.dataset.activeKind || 'all';
  const rows = scope.querySelectorAll('[data-list-row], [data-audit-row]');
  let visible = 0;
  rows.forEach((row) => {
    const text = (row.textContent || '').toLowerCase();
    const okKeyword = !keyword || text.includes(keyword);
    const okCat = catVal === 'all' || (row.dataset.category || '').split(',').includes(catVal);
    const okStat = statVal === 'all' || (row.dataset.status || '') === statVal;
    const okKind = kindVal === 'all' || (row.dataset.auditKind || row.dataset.kind || '') === kindVal;
    const show = okKeyword && okCat && okStat && okKind;
    row.classList.toggle('is-hidden', !show);
    if (show) visible += 1;
  });
  const countLabel = scope.querySelector('[data-list-count]');
  if (countLabel) countLabel.textContent = String(visible);
}

document.addEventListener('input', (event) => {
  const target = event.target.closest('[data-list-search], [data-list-category], [data-list-status]');
  if (!target) return;
  const scope = target.closest('[data-list-scope]') || target.closest('.panel') || document;
  applyAdminListFilter(scope);
});

document.addEventListener('change', (event) => {
  const target = event.target.closest('[data-list-category], [data-list-status]');
  if (!target) return;
  const scope = target.closest('[data-list-scope]') || target.closest('.panel') || document;
  applyAdminListFilter(scope);
});

document.addEventListener('click', (event) => {
  const reset = event.target.closest('[data-list-reset]');
  if (!reset) return;
  const scope = reset.closest('[data-list-scope]') || reset.closest('.panel') || document;
  scope.querySelectorAll('[data-list-search]').forEach((input) => { input.value = ''; });
  scope.querySelectorAll('[data-list-category], [data-list-status]').forEach((sel) => { sel.value = 'all'; });
  applyAdminListFilter(scope);
});

document.querySelectorAll('[data-list-scope]').forEach((scope) => applyAdminListFilter(scope));
