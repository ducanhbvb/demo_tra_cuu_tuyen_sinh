/**
 * perf.js — Performance utilities cho Tra Cứu Tuyển Sinh
 * 
 * Tự động thêm loading="lazy" cho tất cả ảnh trong nội dung bài viết
 * và áp dụng các tối ưu hiệu suất phía client.
 */
(function () {
    'use strict';

    // ── Lazy load ảnh trong .article-content (nội dung TinyMCE) ──────────
    document.addEventListener('DOMContentLoaded', function () {
        var containers = document.querySelectorAll('.article-content, .tab-content, .card-body');
        containers.forEach(function (container) {
            var imgs = container.querySelectorAll('img:not([loading])');
            imgs.forEach(function (img) {
                img.setAttribute('loading', 'lazy');
            });
        });
    });

    // ── Prefetch trang tiếp theo khi hover link (tăng tốc navigation) ────
    var prefetched = {};
    document.addEventListener('mouseover', function (e) {
        var link = e.target.closest('a[href]');
        if (!link) return;
        var href = link.href;
        if (prefetched[href]) return;
        if (!href.startsWith(window.location.origin)) return;
        if (href.includes('#') || href.includes('javascript:')) return;

        prefetched[href] = true;
        var prefetchLink = document.createElement('link');
        prefetchLink.rel = 'prefetch';
        prefetchLink.href = href;
        document.head.appendChild(prefetchLink);
    }, { passive: true });
})();
