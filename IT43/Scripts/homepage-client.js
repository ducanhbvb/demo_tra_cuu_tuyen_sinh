(function () {
    'use strict';

    function setDots(dots, activeIndex) {
        dots.forEach(function (dot, index) {
            dot.classList.toggle('active', index === activeIndex);
        });
    }

    function buildDots(controls, count, labelPrefix, onClick) {
        if (!controls) return [];
        var oldDots = Array.prototype.slice.call(controls.querySelectorAll('.home-slider-dot'));
        var nextArrow = controls.querySelectorAll('.home-slider-arrow')[1];
        oldDots.forEach(function (dot) { dot.parentNode.removeChild(dot); });

        var dots = [];
        for (var i = 0; i < count; i += 1) {
            var dot = document.createElement('button');
            dot.type = 'button';
            dot.className = 'home-slider-dot';
            dot.setAttribute('aria-label', labelPrefix + ' ' + (i + 1));
            dot.addEventListener('click', onClick.bind(null, i));
            controls.insertBefore(dot, nextArrow || null);
            dots.push(dot);
        }
        return dots;
    }

    function initSchoolSlider() {
        var windowEl = document.querySelector('.school-slider-window');
        var track = document.querySelector('.js-school-slider-track');
        if (!windowEl || !track) return;

        var slides = Array.prototype.slice.call(track.querySelectorAll('.js-school-slide'));
        var controls = document.querySelector('[data-slider="schools"]');
        var dots = [];
        var arrows = controls ? Array.prototype.slice.call(controls.querySelectorAll('.home-slider-arrow')) : [];
        var index = 0;
        var timer = null;
        var lastDotCount = 0;

        function visibleCount() {
            if (window.innerWidth < 768) return 1;
            if (window.innerWidth < 992) return 2;
            return 4;
        }

        function maxIndex() {
            return Math.max(0, slides.length - visibleCount());
        }

        function ensureDots(count) {
            if (count === lastDotCount) return;
            lastDotCount = count;
            dots = buildDots(controls, count, 'Slide trường', function (dotIndex) {
                render(dotIndex);
                restart();
            });
        }

        function slideStep() {
            if (!slides.length) return 0;
            var gap = parseFloat(getComputedStyle(track).columnGap || getComputedStyle(track).gap || 0) || 0;
            return slides[0].offsetWidth + gap;
        }

        function render(nextIndex) {
            if (!slides.length) return;
            var max = maxIndex();
            if (max <= 0) {
                if (controls) controls.style.display = 'none';
                track.style.transform = 'translate3d(0,0,0)';
                index = 0;
                return;
            }

            index = nextIndex > max ? 0 : (nextIndex < 0 ? max : nextIndex);
            ensureDots(max + 1);
            if (controls) controls.style.display = '';
            track.style.transform = 'translate3d(' + (-index * slideStep()) + 'px,0,0)';
            if (dots.length) setDots(dots, index);
        }

        function restart() {
            window.clearInterval(timer);
            if (maxIndex() <= 0) return;
            timer = window.setInterval(function () { render(index + 1); }, 4200);
        }

        if (arrows[0]) arrows[0].addEventListener('click', function () { render(index - 1); restart(); });
        if (arrows[1]) arrows[1].addEventListener('click', function () { render(index + 1); restart(); });
        windowEl.addEventListener('mouseenter', function () { window.clearInterval(timer); });
        windowEl.addEventListener('mouseleave', restart);
        window.addEventListener('resize', function () { lastDotCount = 0; render(Math.min(index, maxIndex())); restart(); });

        render(0);
        restart();
    }

    function initNewsSlider() {
        var stage = document.querySelector('.news-slider-stage');
        var groups = Array.prototype.slice.call(document.querySelectorAll('.js-news-group'));
        var controls = document.querySelector('[data-slider="news"]');
        var dots = [];
        var arrows = controls ? Array.prototype.slice.call(controls.querySelectorAll('.home-slider-arrow')) : [];
        var index = 0;
        var timer = null;
        var isPaused = false;

        function clearTimer() {
            window.clearInterval(timer);
            timer = null;
        }

        function normalizeIndex(nextIndex) {
            if (!groups.length) return 0;
            return (nextIndex + groups.length) % groups.length;
        }

        function render(nextIndex) {
            if (!groups.length) return;

            index = normalizeIndex(nextIndex);

            if (groups.length <= 1) {
                if (controls) controls.style.display = 'none';
                groups[0].classList.add('is-active');
                groups[0].setAttribute('aria-hidden', 'false');
                if (stage) stage.style.minHeight = groups[0].offsetHeight + 'px';
                clearTimer();
                return;
            }

            if (controls) controls.style.display = '';
            groups.forEach(function (group, groupIndex) {
                var active = groupIndex === index;
                group.classList.toggle('is-active', active);
                group.setAttribute('aria-hidden', active ? 'false' : 'true');
            });
            if (dots.length) setDots(dots, index);
            syncStageHeight();
        }

        function syncStageHeight() {
            if (!stage || !groups.length) return;
            var activeGroup = groups[index];
            if (!activeGroup) return;
            stage.style.minHeight = activeGroup.offsetHeight + 'px';
        }

        function restart() {
            clearTimer();
            if (isPaused || groups.length <= 1) return;
            timer = window.setInterval(function () { render(index + 1); }, 5600);
        }

        if (arrows[0]) arrows[0].addEventListener('click', function () { render(index - 1); restart(); });
        if (arrows[1]) arrows[1].addEventListener('click', function () { render(index + 1); restart(); });
        dots = buildDots(controls, groups.length, 'Nhóm bài', function (dotIndex) {
            render(dotIndex);
            restart();
        });

        if (stage) {
            stage.addEventListener('mouseenter', function () { isPaused = true; clearTimer(); });
            stage.addEventListener('mouseleave', function () { isPaused = false; restart(); });
            stage.addEventListener('focusin', function () { isPaused = true; clearTimer(); });
            stage.addEventListener('focusout', function () { isPaused = false; restart(); });
        }
        window.addEventListener('resize', syncStageHeight);

        render(0);
        restart();
    }

    document.addEventListener('DOMContentLoaded', function () {
        initSchoolSlider();
        initNewsSlider();
    });
})();
