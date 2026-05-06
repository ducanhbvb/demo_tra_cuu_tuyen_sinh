/**
 * validation_data_input.js — Module validate chung cho tất cả Admin modal
 * ===================================================================
 * Cách dùng:
 *
 *   AdminValidator.init('form', {
 *       'ctl00_MainContent_ddlMTruong': [AdminValidator.rules.required('Vui lòng chọn trường')],
 *       'ctl00_MainContent_txtMNam':    [AdminValidator.rules.required('Nhập năm'), AdminValidator.rules.numRange(2020, 2030)],
 *   }, {
 *       triggerButtonId: 'ctl00_MainContent_btnLuuTin'   // chỉ validate khi nhấn nút này
 *   });
 *
 * Rules có sẵn:
 *   required(msg)                  — bắt buộc (cả select + input)
 *   minLen(n, msg)                 — tối thiểu n ký tự
 *   maxLen(n, msg)                 — tối đa n ký tự
 *   numRange(min, max, msg)        — số thực trong khoảng [min, max]
 *   intPositive(msg)               — số nguyên dương > 0
 *   pattern(regex, msg)            — regex match
 *   email(msg)                     — định dạng email
 *   phone(msg)                     — số điện thoại VN 10 hoặc 11 chữ số, bắt đầu bằng 0
 *   url(msg)                       — URL hợp lệ
 *   dateNotPast(msg)               — ngày không được trong quá khứ
 *   toHopMon(msg)                  — tổ hợp môn VD: A00,A01,D01
 *
 * Mỗi rule trả null (pass) hoặc string (error message).
 * Live validate khi blur field, chặn submit khi có lỗi.
 * Hỗ trợ data-skip-validate="true" trên button Hủy để bỏ qua validate.
 */
(function (window) {
    'use strict';

    // ── Helpers ────────────────────────────────────────────────────────────────

    /** Lấy giá trị text của field (input/textarea/select). */
    function getVal(el) {
        if (!el) return '';
        if (el.tagName === 'SELECT') return el.value || '';
        return (el.value || '').trim();
    }

    /** Hiển thị lỗi inline Bootstrap cho 1 field. */
    function showError(el, msg) {
        if (!el) return;
        el.classList.add('is-invalid');
        el.classList.remove('is-valid');
        var fb = el.parentNode.querySelector('.invalid-feedback');
        if (!fb) {
            fb = document.createElement('div');
            fb.className = 'invalid-feedback';
            el.parentNode.appendChild(fb);
        }
        fb.textContent = msg;
    }

    /** Xóa lỗi inline của 1 field. */
    function clearError(el) {
        if (!el) return;
        el.classList.remove('is-invalid');
        el.classList.add('is-valid');
        var fb = el.parentNode.querySelector('.invalid-feedback');
        if (fb) fb.textContent = '';
    }

    /** Xóa toàn bộ trạng thái validate (dùng khi reset form/modal). */
    function clearAll(formEl) {
        if (!formEl) return;
        formEl.querySelectorAll('.is-invalid, .is-valid').forEach(function (el) {
            el.classList.remove('is-invalid', 'is-valid');
        });
        formEl.querySelectorAll('.invalid-feedback').forEach(function (fb) {
            fb.textContent = '';
        });
    }

    // ── Rule factory ───────────────────────────────────────────────────────────

    var rules = {
        /**
         * Bắt buộc — select phải chọn giá trị khác rỗng, input không được trống.
         */
        required: function (msg) {
            msg = msg || 'Trường này là bắt buộc.';
            return function (val) {
                return (val === '' || val === null || val === undefined) ? msg : null;
            };
        },

        /** Tối thiểu n ký tự (bỏ qua nếu trống — kết hợp với required nếu cần). */
        minLen: function (n, msg) {
            msg = msg || ('Nhập ít nhất ' + n + ' ký tự.');
            return function (val) {
                if (!val) return null;
                return val.length < n ? msg : null;
            };
        },

        /** Tối đa n ký tự. */
        maxLen: function (n, msg) {
            msg = msg || ('Tối đa ' + n + ' ký tự.');
            return function (val) {
                if (!val) return null;
                return val.length > n ? msg : null;
            };
        },

        /**
         * Số thực trong khoảng [min, max] (bỏ qua nếu trống).
         * Chỉ chấp nhận dấu PHẨY (,) làm thập phân. Nếu nhập dấu chấm (.) sẽ báo lỗi.
         */
        numRange: function (min, max, msg) {
            msg = msg || ('Giá trị phải từ ' + min + ' đến ' + max + '.');
            return function (val) {
                if (!val) return null;
                // Từ chối nếu dùng dấu chấm làm thập phân
                if (/\d\.\d/.test(val)) {
                    return 'Vui lòng dùng dấu phẩy (,) thay vì dấu chấm (.). Ví dụ: 25,5';
                }
                var n = parseFloat(val.replace(',', '.'));
                if (isNaN(n)) return msg;
                return (n < min || n > max) ? msg : null;
            };
        },

        /** Số nguyên dương > 0 (bỏ qua nếu trống). */
        intPositive: function (msg) {
            msg = msg || 'Phải là số nguyên dương.';
            return function (val) {
                if (!val) return null;
                var n = parseInt(val, 10);
                if (isNaN(n) || String(n) !== val.replace(/\s/g, '') || n < 1)
                    return msg;
                return null;
            };
        },

        /** Khớp regex (bỏ qua nếu trống). */
        pattern: function (regex, msg) {
            msg = msg || 'Định dạng không hợp lệ.';
            return function (val) {
                if (!val) return null;
                return regex.test(val) ? null : msg;
            };
        },

        /** Email hợp lệ (bỏ qua nếu trống). */
        email: function (msg) {
            msg = msg || 'Địa chỉ email không hợp lệ.';
            return rules.pattern(/^[^\s@]+@[^\s@]+\.[^\s@]+$/, msg);
        },

        /** Số điện thoại VN 10 hoặc 11 chữ số, bắt đầu bằng 0 (bỏ qua nếu trống). */
        phone: function (msg) {
            msg = msg || 'Số điện thoại không hợp lệ (10 hoặc 11 chữ số, bắt đầu bằng 0).';
            return rules.pattern(/^0\d{9,10}$/, msg);
        },

        /** URL hợp lệ bắt đầu bằng http/https (bỏ qua nếu trống). */
        url: function (msg) {
            msg = msg || 'URL không hợp lệ (cần bắt đầu bằng http:// hoặc https://).';
            return function (val) {
                if (!val) return null;
                try {
                    var u = new URL(val);
                    return (u.protocol === 'http:' || u.protocol === 'https:') ? null : msg;
                } catch (e) {
                    return msg;
                }
            };
        },

        /**
         * Ngày không được trong quá khứ (yyyy-mm-dd, bỏ qua nếu trống).
         * So sánh với ngày hôm nay (không tính giờ).
         * Nếu window.AppConfig?.AllowPastDates === true → bỏ qua validation.
         */
        dateNotPast: function (msg) {
            msg = msg || 'Ngày không được trong quá khứ.';
            return function (val) {
                if (!val) return null;
                // Cho phép ngày quá khứ nếu config cho phép
                if (window.AppConfig && window.AppConfig.AllowPastDates) {
                    return null;
                }
                var d = new Date(val);
                if (isNaN(d.getTime())) return 'Ngày không hợp lệ.';
                var today = new Date();
                today.setHours(0, 0, 0, 0);
                return d < today ? msg : null;
            };
        },

        /**
         * Tổ hợp môn — định dạng A00,A01,D01 (1 chữ hoa + 2 chữ số, phân cách bằng dấu phẩy).
         * Bỏ qua nếu trống.
         */
        toHopMon: function (msg) {
            msg = msg || 'Sai định dạng tổ hợp môn. VD: A00,A01,D01';
            return rules.pattern(/^[A-Z][0-9]{2}(,[A-Z][0-9]{2})*$/, msg);
        }
    };

    // ── Core: init ─────────────────────────────────────────────────────────────

    /**
     * Khởi tạo validate cho form.
     *
     * @param {string} formSelector   CSS selector trỏ đến thẻ form (VD: 'form')
     * @param {Object} config         Map: { clientId: [rule1, rule2, ...], ... }
     * @param {Object} [options]
     *   - triggerButtonId {string}   ClientID của nút submit cần validate
     *                                 (nếu để trống → validate trên submit của form)
     */
    function init(formSelector, config, options) {
        options = options || {};
        var triggerButtonId = options.triggerButtonId || null;

        var formEl = document.querySelector(formSelector);
        if (!formEl) {
            console.warn('[AdminValidator] Không tìm thấy form:', formSelector);
            return;
        }

        // Xóa lỗi cũ khi modal mở lại
        var modal = formEl.closest('.modal');
        if (modal) {
            modal.addEventListener('show.bs.modal', function () {
                clearAll(formEl);
            });
        }

        // ── Validate 1 field ──────────────────────────────────────────────────
        function validateField(clientId) {
            var el = document.getElementById(clientId);
            if (!el) return true; // field không tồn tại → bỏ qua
            var fieldRules = config[clientId];
            if (!fieldRules || !fieldRules.length) return true;

            var val = getVal(el);
            for (var i = 0; i < fieldRules.length; i++) {
                var err = fieldRules[i](val);
                if (err !== null) {
                    showError(el, err);
                    return false;
                }
            }
            clearError(el);
            return true;
        }

        // ── Live validate on blur ─────────────────────────────────────────────
        Object.keys(config).forEach(function (clientId) {
            var el = document.getElementById(clientId);
            if (!el) return;
            el.addEventListener('blur', function () {
                validateField(clientId);
            });
            // Select: validate ngay khi change
            if (el.tagName === 'SELECT') {
                el.addEventListener('change', function () {
                    validateField(clientId);
                });
            }
        });

        // ── Validate toàn form ────────────────────────────────────────────────
        function validateAll() {
            var valid = true;
            var firstFail = null;
            Object.keys(config).forEach(function (clientId) {
                var ok = validateField(clientId);
                if (!ok && !firstFail) {
                    firstFail = document.getElementById(clientId);
                }
                if (!ok) valid = false;
            });
            if (firstFail) {
                firstFail.focus();
                // Scroll field vào view nếu nằm trong modal
                firstFail.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
            return valid;
        }

        // ── Hook vào trigger button hoặc form submit ──────────────────────────
        if (triggerButtonId) {
            var btn = document.getElementById(triggerButtonId);
            if (btn) {
                btn.addEventListener('click', function (e) {
                    if (!validateAll()) {
                        e.preventDefault();
                        e.stopImmediatePropagation();
                        return false;
                    }
                });
            } else {
                console.warn('[AdminValidator] Không tìm thấy triggerButtonId:', triggerButtonId);
            }
        } else {
            // Fallback: chặn submit form nếu không chỉ định triggerButtonId
            formEl.addEventListener('submit', function (e) {
                // Bỏ qua nếu button submit có data-skip-validate="true"
                var active = document.activeElement;
                if (active && active.getAttribute('data-skip-validate') === 'true') return;
                if (!validateAll()) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    }

    // ── Public API ─────────────────────────────────────────────────────────────
    window.AdminValidator = {
        init: init,
        rules: rules
    };

}(window));

// ── Copy Slug/ID Button — dùng chung cho mọi trang có .btn-copy-slug ────────
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('click', function (e) {
        var btn = e.target.closest('.btn-copy-slug');
        if (!btn) return;
        var val = '';
        // Ưu tiên data-source-text (đọc textContent của element)
        var srcText = btn.getAttribute('data-source-text');
        if (srcText) {
            var el = document.getElementById(srcText);
            if (el) val = el.textContent.trim();
        } else {
            // data-source = input/textarea value
            var sourceId = btn.getAttribute('data-source');
            var input = document.getElementById(sourceId);
            if (input) val = input.value.trim();
        }
        if (!val || val === '—') return;
        navigator.clipboard.writeText(val).then(function () {
            var icon = btn.querySelector('i');
            if (icon) {
                icon.className = 'bi bi-clipboard-check text-success';
                setTimeout(function () { icon.className = 'bi bi-clipboard'; }, 1500);
            }
        }).catch(function () {
            // Fallback
            var ta = document.createElement('textarea');
            ta.value = val; document.body.appendChild(ta);
            ta.select(); document.execCommand('copy');
            document.body.removeChild(ta);
        });
    });
});
