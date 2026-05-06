/* Admin modal upload + preview flows for WebForms pages.
   Keeps behavior out of .aspx and uses data-* hooks to avoid ClientID coupling. */
(function (window, document) {
    'use strict';

    function ready(fn) {
        if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', fn);
        else fn();
    }

    function byId(id) { return id ? document.getElementById(id) : null; }

    function getValue(selector, fallback) {
        var el = document.querySelector(selector);
        if (!el) return fallback || '';
        if (el.tagName === 'SELECT') return el.options[el.selectedIndex] ? el.options[el.selectedIndex].text : (fallback || '');
        return el.value || fallback || '';
    }

    function decodeHtmlEntities(text) {
        if (!text) return '';
        var textarea = document.createElement('textarea');
        textarea.innerHTML = text;
        return textarea.value;
    }

    function setText(id, value) {
        var el = byId(id);
        if (el) el.textContent = decodeHtmlEntities(value) || '';
    }

    function setHtml(id, value) {
        var el = byId(id);
        if (el) el.innerHTML = decodeHtmlEntities(value) || '';
    }

    function setSrc(id, src) {
        var el = byId(id);
        if (el && src) el.src = src;
    }

    function getTinyMceContent(editorId, fallbackSelector) {
        var fallback = document.querySelector(fallbackSelector || '');
        var resolvedEditorId = editorId || (fallback ? fallback.id : '');
        var content = '';
        if (window.CmsEditor && typeof window.CmsEditor.getContent === 'function' && resolvedEditorId) {
            content = window.CmsEditor.getContent(resolvedEditorId);
        }
        if (!content && window.tinymce && resolvedEditorId && window.tinymce.get(resolvedEditorId)) {
            content = window.tinymce.get(resolvedEditorId).getContent();
        }
        if (!content && fallback) {
            content = fallback.value || fallback.innerHTML || '';
        }
        return decodeHtmlEntities(content);
    }

    function showModal(id) {
        var modalEl = byId(id);
        if (modalEl && window.bootstrap) window.bootstrap.Modal.getOrCreateInstance(modalEl).show();
    }

    function getPreviewImage(previewKey) {
        var img = document.querySelector('[data-preview-target="' + previewKey + '"]');
        if (img && img.src && img.src !== window.location.href && !img.src.endsWith('/')) return img.src;
        return '';
    }

    function initUploadCards() {
        document.querySelectorAll('[data-upload-card]').forEach(function (card) {
            var input = card.querySelector('[data-preview]');
            var key = input ? input.getAttribute('data-preview') : '';
            var img = key ? document.querySelector('[data-preview-target="' + key + '"]') : card.querySelector('[data-preview-target]');
            var wrap = key ? document.querySelector('[data-preview-wrap="' + key + '"]') : card.querySelector('[data-preview-wrap]');
            var fileName = card.querySelector('[data-upload-file-name]');
            var clearBtn = card.querySelector('[data-clear-upload]');
            var originalSrc = img ? (img.getAttribute('src') || img.src || '') : '';
            var hasOriginal = !!originalSrc;

            if (fileName) fileName.textContent = hasOriginal ? 'Đang dùng ảnh hiện có' : 'Chưa chọn ảnh';
            if (wrap && hasOriginal) wrap.style.display = '';

            if (input && !input.dataset.adminUploadBound) {
                input.dataset.adminUploadBound = 'true';
                input.addEventListener('change', function () {
                    var file = input.files && input.files[0];
                    if (!file || !file.type || !file.type.startsWith('image/')) return;
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        if (img) img.src = e.target.result;
                        if (wrap) wrap.style.display = '';
                        if (fileName) fileName.textContent = file.name;
                    };
                    reader.readAsDataURL(file);
                });
            }

            if (clearBtn && input && !clearBtn.dataset.adminUploadBound) {
                clearBtn.dataset.adminUploadBound = 'true';
                clearBtn.addEventListener('click', function () {
                    input.value = '';
                    if (img && originalSrc) img.src = originalSrc;
                    if (wrap) wrap.style.display = originalSrc ? '' : 'none';
                    if (fileName) fileName.textContent = originalSrc ? 'Đã hủy file mới, quay lại ảnh hiện có' : 'Chưa chọn ảnh';
                });
            }
        });
    }

    function initSchoolPreview() {
        var btn = document.querySelector('[data-admin-preview="school"]');
        if (!btn) return;
        btn.addEventListener('click', function () {
            setText('pvSchoolName', getValue('[data-field="ten"]', '(Chưa có tên trường)'));
            setText('pvSchoolCode', getValue('[data-field="slug"]', 'Chưa có slug'));
            setText('pvSchoolType', getValue('[data-preview-source="school-type"]', 'Chưa chọn loại'));
            setText('pvSchoolCity', getValue('[data-preview-source="school-city"]', 'Chưa có tỉnh/TP'));
            setText('pvSchoolWebsite', getValue('[data-preview-source="school-website"]', 'Chưa có website'));
            setText('pvSchoolPhone', getValue('[data-preview-source="school-phone"]', 'Chưa có điện thoại'));
            setHtml('pvSchoolDesc', getTinyMceContent('', '[data-preview-source="school-desc"]') || 'Chưa có mô tả.');
            setSrc('pvSchoolLogo', getPreviewImage('avatar') || '/Resources/Images/no-image.png');
            setSrc('pvSchoolCover', getPreviewImage('bia') || '/Resources/Images/no-image.png');
            showModal('modalPreviewTruong');
        });
    }

    function initArticlePreview() {
        var btn = document.querySelector('[data-admin-preview="article"], [data-preview-baiViet="true"]');
        if (!btn) return;
        btn.addEventListener('click', function () {
            var title = getValue('[data-field="tieude"]', '(Chưa có tiêu đề)');
            var author = getValue('[data-preview-source="article-author"]', 'Ban Biên tập');
            var today = new Date().toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
            var content = getTinyMceContent('txtTinyMCE', '[data-field="noiDung"]');
            var imgSrc = getPreviewImage('anhbia');
            var img = byId('pvPostImg');
            if (img) {
                img.style.display = imgSrc ? '' : 'none';
                if (imgSrc) img.src = imgSrc;
            }
            setText('pvPostTitle', title);
            setText('pvPostAuthor', author || 'Ban Biên tập');
            setText('pvPostDate', today);
            setHtml('pvPostContent', content || '<p class="text-muted">Chưa có nội dung.</p>');
            showModal('modalPreviewBV');
        });
    }

    function initAdmissionPreview() {
        var btn = document.querySelector('[data-admin-preview="admission"]');
        if (!btn) return;
        btn.addEventListener('click', function () {
            var title = getValue('[data-preview-source="admission-title"]', 'Tin tuyển sinh');
            var method = getValue('[data-preview-source="admission-method"]', 'Phương thức');
            var imgSrc = getPreviewImage('tints');
            var img = byId('pvTinImg');
            if (img) {
                img.style.display = imgSrc ? '' : 'none';
                if (imgSrc) img.src = imgSrc;
            }
            setText('pvTinTitle', title);
            setText('pvTinSchool', getValue('[data-preview-source="admission-school"]', 'Chưa chọn trường'));
            setText('pvTinMajor', getValue('[data-preview-source="admission-major"]', 'Chưa chọn ngành'));
            setText('pvTinMethod', method);
            setText('pvTinYear', getValue('[data-preview-source="admission-year"]', '--'));
            setText('pvTinQuota', getValue('[data-preview-source="admission-quota"]', '--'));
            setText('pvTinScore', getValue('[data-preview-source="admission-score"]', '--'));
            setText('pvTinFee', getValue('[data-preview-source="admission-fee"]', '--'));
            setText('pvTinSubject', getValue('[data-preview-source="admission-subject"]', '--'));
            setText('pvTinDeadline', getValue('[data-preview-source="admission-deadline"]', '--'));
            setHtml('pvTinNote', getTinyMceContent('', '[data-preview-source="admission-note"]') || 'Chưa có ghi chú.');
            showModal('modalPreviewTin');
        });
    }

    ready(function () {
        initUploadCards();
        initSchoolPreview();
        initArticlePreview();
        initAdmissionPreview();
    });

    window.AdminModalPreview = {
        initUploadCards: initUploadCards,
        initSchoolPreview: initSchoolPreview,
        initArticlePreview: initArticlePreview,
        initAdmissionPreview: initAdmissionPreview
    };
})(window, document);
