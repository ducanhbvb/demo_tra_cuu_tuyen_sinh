/* =========================================================
   site.js — Shared helpers for Admin + Client pages
   ========================================================= */

var THEME_KEY = 'tcts-theme';

/* ── Theme helpers ──────────────────────────────────────── */
function getStoredTheme() {
    try { return localStorage.getItem(THEME_KEY); } catch (e) { return null; }
}
function setStoredTheme(theme) {
    try { localStorage.setItem(THEME_KEY, theme); } catch (e) {}
}
function getPreferredTheme() {
    var stored = getStoredTheme();
    if (stored) return stored;
    return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}
function setTheme(theme) {
    document.documentElement.setAttribute('data-bs-theme', theme);
}
function syncThemeIcon() {
    var icon = document.getElementById('themeIcon');
    if (!icon) return;
    var isDark = document.documentElement.getAttribute('data-bs-theme') === 'dark';
    icon.className = isDark ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
}
function toggleTheme() {
    var isDark = document.documentElement.getAttribute('data-bs-theme') === 'dark';
    var next = isDark ? 'light' : 'dark';
    setTheme(next);
    setStoredTheme(next);
    syncThemeIcon();
}
if (window.matchMedia) {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function () {
        if (!getStoredTheme()) {
            setTheme(getPreferredTheme());
            syncThemeIcon();
        }
    });
}

/* ── Admin sidebar helpers ──────────────────────────────── */
function openSidebar() {
    var sidebar = document.getElementById('adminSidebar');
    var overlay  = document.getElementById('sidebarOverlay');
    var mainArea = document.querySelector('.admin-main');
    if (!sidebar) return;
    var DESKTOP = window.innerWidth >= 992;
    sidebar.classList.remove('collapsed');
    sidebar.classList.add('open');
    if (overlay) overlay.classList.add('show');
    if (mainArea && DESKTOP) mainArea.style.marginLeft = 'var(--sidebar-w)';
    try { localStorage.setItem('admin-sidebar-state', 'open'); } catch(e) {}
}
function closeSidebar() {
    var sidebar = document.getElementById('adminSidebar');
    var overlay  = document.getElementById('sidebarOverlay');
    var mainArea = document.querySelector('.admin-main');
    if (!sidebar) return;
    var DESKTOP = window.innerWidth >= 992;
    sidebar.classList.remove('open');
    if (overlay) overlay.classList.remove('show');
    if (DESKTOP) {
        sidebar.classList.add('collapsed');
        if (mainArea) mainArea.style.marginLeft = '0';
    }
    try { localStorage.setItem('admin-sidebar-state', 'closed'); } catch(e) {}
}
function toggleSidebar() {
    var sidebar = document.getElementById('adminSidebar');
    if (!sidebar) return;
    var DESKTOP = window.innerWidth >= 992;
    if (DESKTOP) {
        sidebar.classList.contains('collapsed') ? openSidebar() : closeSidebar();
    } else {
        sidebar.classList.contains('open') ? closeSidebar() : openSidebar();
    }
}
function initAdminSidebar() {
    var toggleBtn = document.getElementById('sidebarToggleBtn');
    var closeBtn  = document.getElementById('sidebarCloseBtn');
    var overlay   = document.getElementById('sidebarOverlay');
    if (toggleBtn) toggleBtn.addEventListener('click', toggleSidebar);
    if (closeBtn)  closeBtn.addEventListener('click', closeSidebar);
    if (overlay)   overlay.addEventListener('click', closeSidebar);

    // Bug fix: Always collapse on small/medium screens (< 1024px) or if user previously collapsed it
    var savedState = '';
    try { savedState = localStorage.getItem('admin-sidebar-state'); } catch(e) {}
    
    if (window.innerWidth < 1024) {
        // Auto hide for screens < 1024px as requested
        closeSidebar();
    } else if (savedState === 'closed') {
        // Restore user preference
        closeSidebar();
    }

    window.addEventListener('resize', function() {
        var sidebar = document.getElementById('adminSidebar');
        var overlay = document.getElementById('sidebarOverlay');
        var mainArea = document.querySelector('.admin-main');
        var DESKTOP = window.innerWidth >= 992;
        if (DESKTOP) {
            if (overlay) overlay.classList.remove('show');
            if (!sidebar.classList.contains('collapsed')) {
                sidebar.classList.remove('open');
                if (mainArea) mainArea.style.marginLeft = 'var(--sidebar-w)';
            }
        } else {
            // Auto-close sidebar on mobile/tablet when shrinking from desktop
            if (sidebar && !sidebar.classList.contains('open')) {
                closeSidebar();
            }
        }
    });
}

/* ── Admin clock ───────────────────────────────────────── */
function tickClock() {
    var el = document.getElementById('topbarClock');
    if (!el) return;
    var now  = new Date();
    var time = now.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
    var date = now.toLocaleDateString('vi-VN', { weekday: 'short', day: '2-digit', month: '2-digit', year: 'numeric' });
    el.innerHTML = '<span class="clock-time">' + time + '</span><span class="clock-date">' + date + '</span>';
}
function initAdminClock() {
    tickClock();
    setInterval(tickClock, 1000);
}

/* ── Admin theme toggle ─────────────────────────────────── */
function syncAdminThemeIcon() {
    var icon = document.getElementById('adminThemeIcon');
    if (!icon) return;
    var isDark = document.documentElement.getAttribute('data-bs-theme') === 'dark';
    icon.className = isDark ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
}
function toggleAdminTheme() {
    var isDark = document.documentElement.getAttribute('data-bs-theme') === 'dark';
    var next   = isDark ? 'light' : 'dark';
    document.documentElement.setAttribute('data-bs-theme', next);
    try { localStorage.setItem('tcts-theme', next); } catch(e) {}
    syncAdminThemeIcon();
}

/* ── Stat count-up animation ────────────────────────────── */
function countUp(el) {
    var target = parseInt(el.textContent.replace(/\D/g, '')) || 0;
    if (target === 0) return;
    el.textContent = '0';
    var cur = 0;
    var step = Math.max(1, Math.ceil(target / 50));
    var timer = setInterval(function () {
        cur = Math.min(cur + step, target);
        el.textContent = cur.toLocaleString('vi-VN');
        if (cur >= target) clearInterval(timer);
    }, 25);
}
function initStatCountUp() {
    document.querySelectorAll('.stat-number, .db-stat-num').forEach(function (el) {
        countUp(el);
    });
}

/* ── Greeting & banner date (Admin Default) ────────────── */
function initDashboardBanner() {
    var el = document.getElementById('dbBannerDate');
    if (el) {
        el.textContent = new Date().toLocaleDateString('vi-VN', {
            weekday: 'long', day: 'numeric', month: 'long', year: 'numeric'
        });
    }
    var titleEl = document.querySelector('.db-welcome-title');
    if (titleEl) {
        var h = new Date().getHours();
        var greeting = h < 12 ? 'Chào buổi sáng' : h < 18 ? 'Chào buổi chiều' : 'Chào buổi tối';
        titleEl.innerHTML = greeting + ', <strong>Admin!</strong> 👋';
    }
}

/* ── Export CSV button text ─────────────────────────────── */
function initExportCsvBtn() {
    document.querySelectorAll('.btn-export-csv').forEach(function (btn) {
        if (!btn.value && !btn.textContent.trim().includes('CSV')) {
            btn.innerHTML = '<i class="bi bi-file-earmark-spreadsheet me-1"></i> Xuất CSV';
        }
    });
}

/* ── QuanLyTaiKhoan: toggle trường dropdown ────────────── */
function toggleTruongDropdown(ddl, divId) {
    var div = document.getElementById(divId);
    if (div) div.style.display = (ddl.value === '2') ? '' : 'none';
}

/* ── QuanLyGopYTuVan: switch tab ────────────────────────── */
function switchTab(loai, hfId, btnId) {
    var hf  = document.getElementById(hfId);
    var btn = document.getElementById(btnId);
    if (hf)  hf.value  = loai;
    if (btn) btn.click();
}

/* ── Filter helpers (QuanLyTruong) ─────────────────────── */
function clearFilter() {
    var filterInput  = document.querySelector('[data-filter-input="true"]');
    var tinhSelect   = document.querySelector('[data-filter-tinh="true"]');
    var trangThaiSel = document.querySelector('[data-filter-trangthai="true"]');
    var clearTrigger = document.querySelector('[data-clear-filter-trigger="true"]');
    if (filterInput)  filterInput.value = '';
    if (tinhSelect)   tinhSelect.selectedIndex = 0;
    if (trangThaiSel) trangThaiSel.selectedIndex = 0;
    if (clearTrigger) __doPostBack(clearTrigger.name, '');
}

/* ── Modal helpers ─────────────────────────────────────── */
function clearModalByPrefix(prefix) {
    var selector = prefix
        ? '[data-field-' + prefix + ']'
        : '[data-field]';
    document.querySelectorAll(selector).forEach(function(el) {
        if (el.tagName === 'INPUT' && el.type === 'checkbox') {
            el.checked = false;
        } else if (el.tagName === 'SELECT' || el.tagName === 'INPUT' || el.tagName === 'TEXTAREA') {
            el.value = '';
        }
    });
    document.querySelectorAll('[data-preview-wrap]').forEach(function(el) {
        el.style.display = 'none';
    });
}

function previewImage(input) {
    if (!input.files || !input.files[0]) return;
    var file = input.files[0];
    if (!file.type.startsWith('image/')) return;
    var reader    = new FileReader();
    var previewType = input.dataset.preview;
    var targetImg = document.querySelector('[data-preview-target="' + previewType + '"]');
    var wrapDiv   = document.querySelector('[data-preview-wrap="' + previewType + '"]');
    reader.onload = function(e) {
        if (targetImg) targetImg.src = e.target.result;
        if (wrapDiv)   wrapDiv.style.display = '';
    };
    reader.readAsDataURL(file);
}

/* ChinhSuaTruong: preview ảnh với id chỉ định */
function previewImgById(input, imgId) {
    if (!input.files || !input.files[0]) return;
    var file = input.files[0];
    if (!file.type.startsWith('image/')) return;
    var reader = new FileReader();
    reader.onload = function(e) {
        var img = document.getElementById(imgId);
        if (img) { img.src = e.target.result; img.style.display = ''; }
    };
    reader.readAsDataURL(file);
}

/* Xem ảnh full trong tab mới */
function viewImageFull(el) {
    if (!el || !el.src || el.src === window.location.href) return;
    window.open(el.src, '_blank');
}

/* Auto-generate slug từ tiêu đề */
function autoGenerateSlug(input) {
    var slug = input.value
        .toLowerCase()
        .replace(/đ/g, 'd')
        .normalize('NFD').replace(/[\u0300-\u036f]/g, '')
        .replace(/[^a-z0-9\s-]/g, '')
        .trim().replace(/\s+/g, '-').replace(/-+/g, '-');
    var slugInput = document.querySelector('[data-field="slug"]');
    if (slugInput) slugInput.value = slug;
}

/* Sync TinyMCE before submit */
function syncTinyMCEBeforeSubmit() {
    var hf = document.querySelector('[data-field="noiDung"]');
    return hf && typeof CmsEditor !== 'undefined' ? CmsEditor.syncToHiddenField('txtTinyMCE', hf.id) : true;
}

/* Preview bài viết (QuanLyBaiViet) */
function previewBaiViet() {
    var titleInput = document.querySelector('[data-field="tieude"]');
    var title = titleInput ? titleInput.value : '(Chưa có tiêu đề)';
    var imgEl = document.querySelector('[data-preview-target="anhbia"]');
    var imgSrc = (imgEl && imgEl.src && !imgEl.src.endsWith('/') && imgEl.src !== window.location.href) ? imgEl.src : null;
    var today = new Date().toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
    var content = (typeof CmsEditor !== 'undefined') ? CmsEditor.getContent('txtTinyMCE') : '';
    var imgWrap   = document.getElementById('pvImgWrap');
    var titleEl   = document.getElementById('pvTitle');
    var dateEl    = document.getElementById('pvDate');
    var contentEl = document.getElementById('pvContent');
    if (imgWrap)   imgWrap.innerHTML   = imgSrc ? '<img src="' + imgSrc + '" class="w-100 rounded-3 mb-4" style="max-height:350px;object-fit:cover;" />' : '';
    if (titleEl)   titleEl.innerText   = title;
    if (dateEl)    dateEl.innerText    = today;
    if (contentEl) contentEl.innerHTML = content;
    var modalEl = document.getElementById('modalPreviewBV');
    if (modalEl && typeof bootstrap !== 'undefined') {
        new bootstrap.Modal(modalEl).show();
    }
}

/* ChinhSuaTruong: clear modal điểm chuẩn */
function clearDiemChuanModal() {
    var hf = document.getElementById('hfIDDC');
    if (hf) hf.value = '0';
    var ids = ['txtMDCNam', 'txtMDCDiem', 'txtMDCChiTieu', 'txtMDCGhiChu', 'ddlMDCNganh', 'ddlMDCPhuongThuc'];
    ids.forEach(function(id) {
        var el = document.getElementById(id);
        if (!el) return;
        if (el.tagName === 'SELECT') el.selectedIndex = 0;
        else el.value = '';
    });
}

/* ── Confirm delete links ──────────────────────────────── */
document.querySelectorAll('[data-confirm]').forEach(function (el) {
    el.addEventListener('click', function (e) {
        if (!confirm(el.dataset.confirm)) e.preventDefault();
    });
});

/* ── DOMContentLoaded: apply all behaviors ─────────────── */
(function () {
    function ready(fn) {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', fn);
        } else {
            fn();
        }
    }

    ready(function () {
        syncThemeIcon();

        /* Auto-dismiss success/info alerts */
        document.querySelectorAll('.alert.alert-success, .alert.alert-info').forEach(function (el) {
            setTimeout(function () {
                el.style.transition = 'opacity .5s';
                el.style.opacity = '0';
                setTimeout(function () { el.remove(); }, 500);
            }, 5000);
        });

        /* Highlight active nav link */
        var path = window.location.pathname.toLowerCase();
        document.querySelectorAll('.navbar-nav .nav-link').forEach(function (a) {
            var href = (a.getAttribute('href') || '').toLowerCase();
            if (href && href !== '/' && path.indexOf(href.split('?')[0]) !== -1) {
                a.classList.add('active');
            }
        });

        /* Bootstrap tooltips */
        if (typeof bootstrap !== 'undefined') {
            document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
                new bootstrap.Tooltip(el);
            });
        }

        /* Star rating (Client pages) */
        document.querySelectorAll('.star-rating').forEach(function (container) {
            var stars = container.querySelectorAll('.star');
            var input = container.querySelector('input[type="hidden"]');
            if (!input) return;
            stars.forEach(function (star, idx) {
                star.addEventListener('click', function () {
                    input.value = idx + 1;
                    stars.forEach(function (s, i) { s.classList.toggle('active', i <= idx); });
                });
            });
        });

        /* ── QuanLyTruong behaviors ── */
        // Auto slug from tên trường
        var tenInput = document.querySelector('[data-field="ten"]');
        if (tenInput) tenInput.addEventListener('input', function () { autoGenerateSlug(this); });

        // Clear filter button
        var clearFilterBtn = document.querySelector('[data-clear-filter="true"]');
        if (clearFilterBtn) clearFilterBtn.addEventListener('click', clearFilter);

        // Add new → clear modal
        var addNewBtn = document.querySelector('[data-clear-modal="true"]');
        if (addNewBtn) addNewBtn.addEventListener('click', function () { clearModalByPrefix(); });

        // Image preview on change
        document.querySelectorAll('[data-preview]').forEach(function (input) {
            input.addEventListener('change', function () { previewImage(this); });
        });

        // Show modal on load if server flag set
        if (window.showModalOnLoad === 'true') {
            var modalEl = document.getElementById('modalTruong');
            if (modalEl && typeof bootstrap !== 'undefined') new bootstrap.Modal(modalEl).show();
        }

        /* ── QuanLyBaiViet behaviors ── */
        // Auto slug from tiêu đề
        var tenBVInput = document.querySelector('[data-field="tieude"]');
        if (tenBVInput) tenBVInput.addEventListener('input', function () { autoGenerateSlug(this); });

        // Clear modal
        document.querySelectorAll('[data-clear-modal]').forEach(function(btn) {
            btn.addEventListener('click', function () { clearModalByPrefix(); });
        });

        // Image preview
        document.querySelectorAll('[data-preview]').forEach(function (input) {
            input.addEventListener('change', function () { previewImage(this); });
        });

        // View full image
        document.querySelectorAll('[data-preview-target]').forEach(function (img) {
            img.style.cursor = 'pointer';
            img.addEventListener('click', function () { viewImageFull(this); });
        });

        // Preview bài viết
        var previewBtn = document.querySelector('[data-preview-baiViet="true"]');
        if (previewBtn) previewBtn.addEventListener('click', previewBaiViet);

        // Show BaiViet modal on load
        if (window.showBaiVietModal === 'true') {
            var bvModal = document.getElementById('modalBaiViet');
            if (bvModal && typeof bootstrap !== 'undefined') {
                var modal = new bootstrap.Modal(bvModal);
                modal.show();
                var hfNoiDung = document.querySelector('[data-field="noiDung"]');
                if (hfNoiDung && hfNoiDung.value) {
                    setTimeout(function () {
                        if (typeof CmsEditor !== 'undefined') CmsEditor.setContent('txtTinyMCE', hfNoiDung.value);
                    }, 500);
                }
            }
        }

        /* ── QuanLyTinTuyenSinh: show modal on load ── */
        if (window.showTinTuyenSinhModal === 'true') {
            var tinModal = document.getElementById('modalThem');
            if (tinModal && typeof bootstrap !== 'undefined') new bootstrap.Modal(tinModal).show();
        }

        /* ── QuanLyTaiKhoan behaviors ── */
        // Toggle trường dropdown theo role
        document.querySelectorAll('select[data-toggle-truong]').forEach(function(ddl) {
            ddl.addEventListener('change', function() {
                var targetId = this.dataset.toggleTruong;
                if (targetId) toggleTruongDropdown(this, targetId);
            });
        });

        // Mở modal SuaQuyen / ResetMK nếu server báo (value = MaTaiKhoan > 0)
        if (window.openSuaQuyen && window.openSuaQuyen !== '' && window.openSuaQuyen !== '0') {
            var m = document.getElementById('modalSuaQuyen');
            if (m && typeof bootstrap !== 'undefined') new bootstrap.Modal(m).show();
            var ddlSua = document.querySelector('#modalSuaQuyen select[data-toggle-truong]');
            if (ddlSua) toggleTruongDropdown(ddlSua, 'divSuaTruong');
        }
        if (window.openResetMK && window.openResetMK !== '' && window.openResetMK !== '0') {
            var mR = document.getElementById('modalResetMK');
            if (mR && typeof bootstrap !== 'undefined') new bootstrap.Modal(mR).show();
        }

        // Reset TaoTruong when opening modal Tạo TK
        var modalTao = document.getElementById('modalTaoTK');
        if (modalTao) {
            modalTao.addEventListener('show.bs.modal', function () {
                var ddlTao = modalTao.querySelector('select[data-toggle-truong]');
                if (ddlTao) toggleTruongDropdown(ddlTao, 'divTaoTruong');
            });
        }

        /* ── QuanLyGopYTuVan: show detail modal ── */
        if (window.showGopYTuvanDetail === 'true') {
            var ctModal = document.getElementById('modalChiTiet');
            if (ctModal && typeof bootstrap !== 'undefined') new bootstrap.Modal(ctModal).show();
        }

        /* ── QuanLyLogs: export CSV button ── */
        initExportCsvBtn();

        /* ── ChinhSuaTruong: show modal on load ── */
        if (window.showDiemChuanModal === 'true') {
            var dcModal = document.getElementById('modalDiemChuan');
            if (dcModal && typeof bootstrap !== 'undefined') new bootstrap.Modal(dcModal).show();
        }

        /* ── QuanLyTruong / QuanLyBaiViet: image click to full ── */
        document.querySelectorAll('img[data-preview-target]').forEach(function (img) {
            img.style.cursor = 'pointer';
            img.addEventListener('click', function() { viewImageFull(this); });
        });
    });

    /* ── HopThu bell badge polling — mỗi 60s khi HocSinh đăng nhập ── */
    (function initHopThuPolling() {
        var bellLink = document.querySelector('a[href*="hop-thu"]');
        if (!bellLink) return;  // không phải HocSinh hoặc không có bell

        function pollHopThu() {
            fetch('/Handlers/HopThuCount.ashx?_=' + Date.now(), { credentials: 'same-origin' })
                .then(function(r) { return r.json(); })
                .then(function(data) {
                    var count = data && data.chuaDoc ? data.chuaDoc : 0;
                    var badge = bellLink.querySelector('.badge');
                    if (count > 0) {
                        if (!badge) {
                            badge = document.createElement('span');
                            badge.className = 'position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger';
                            badge.style.fontSize = '.6rem';
                            badge.style.minWidth = '16px';
                            bellLink.style.position = 'relative';
                            bellLink.appendChild(badge);
                        }
                        badge.textContent = count > 99 ? '99+' : count;
                    } else if (badge) {
                        badge.remove();
                    }
                })
                .catch(function() { /* ignore network errors */ });
        }

        // Chạy lần đầu sau 5s (tránh chặn load trang), sau đó mỗi 60s
        setTimeout(function() {
            pollHopThu();
            setInterval(pollHopThu, 60000);
        }, 5000);
    })();
})();