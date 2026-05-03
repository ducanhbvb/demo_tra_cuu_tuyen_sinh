var THEME_KEY = 'tcts-theme';

/* Theme helpers */
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

/* Listen system preference changes */
if (window.matchMedia) {
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', function () {
        var stored = getStoredTheme();
        if (!stored) {
            setTheme(getPreferredTheme());
            syncThemeIcon();
        }
    });
}

/* DOMContentLoaded */
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

        document.querySelectorAll('.alert.alert-success, .alert.alert-info').forEach(function (el) {
            setTimeout(function () {
                el.style.transition = 'opacity .5s';
                el.style.opacity = '0';
                setTimeout(function () { el.remove(); }, 500);
            }, 5000);
        });

        var path = window.location.pathname.toLowerCase();
        document.querySelectorAll('.navbar-nav .nav-link').forEach(function (a) {
            var href = (a.getAttribute('href') || '').toLowerCase();
            if (href && href !== '/' && path.indexOf(href.split('?')[0]) !== -1) {
                a.classList.add('active');
            }
        });

        if (typeof bootstrap !== 'undefined') {
            document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
                new bootstrap.Tooltip(el);
            });
        }

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

        document.querySelectorAll('[data-confirm]').forEach(function (el) {
            el.addEventListener('click', function (e) {
                if (!confirm(el.dataset.confirm)) e.preventDefault();
            });
        });
    });
})();
