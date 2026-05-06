// ── Filter logic ─────────────────────────────────────────────────────────────
var _status = 'all';
var _time   = 'all';

function htFilter() {
    var search = document.getElementById('htSearch');
    var q = ((search && search.value) || '').toLowerCase().trim();
    var rows = document.querySelectorAll('.ht-row');
    var visible = 0;
    var now = Date.now();
    var cutoff = 0;
    if (_time === 'today') cutoff = now - 86400000;
    else if (_time === '7d')  cutoff = now - 7*86400000;
    else if (_time === '30d') cutoff = now - 30*86400000;
    else if (_time === '3m')  cutoff = now - 90*86400000;

    rows.forEach(function(el) {
        var matchStatus = (_status === 'all') || (el.dataset.status === _status);
        var matchSearch = !q || (el.dataset.truong && el.dataset.truong.includes(q));
        var matchTime   = true;
        if (cutoff > 0 && el.dataset.date) {
            var t = new Date(el.dataset.date).getTime();
            matchTime = (t >= cutoff);
        }
        if (matchStatus && matchSearch && matchTime) {
            el.classList.remove('ht-row-hidden');
            visible++;
        } else {
            el.classList.add('ht-row-hidden');
        }
    });

    var noResult = document.getElementById('noFilterResult');
    if (noResult) {
        noResult.style.display = (visible === 0 && rows.length > 0) ? 'block' : 'none';
    }
}

function htSetStatus(el) {
    document.querySelectorAll('.ht-tab').forEach(function(x){x.classList.remove('active');});
    el.classList.add('active');
    _status = el.dataset.status || 'all';
    htFilter();
}

function htSetTime(el, val, label) {
    document.querySelectorAll('#htTimeBtnLabel + .dropdown-menu .dropdown-item').forEach(function(x){x.classList.remove('active');});
    el.classList.add('active');
    _time = val;
    var lbl = document.getElementById('htTimeLbl');
    if (lbl) lbl.textContent = label;
    var btn = document.getElementById('htTimeBtnLabel');
    if (btn) btn.classList.toggle('time-active', val !== 'all');
    htFilter();
}

// ── Auto-open modal nếu server yêu cầu ───────────────────────────────────────
document.addEventListener('DOMContentLoaded', function(){
    var threadId = window.hopThuThreadHiddenFieldId;
    var hf = threadId ? document.getElementById(threadId) : null;
    if (hf && hf.value && hf.value !== '0') {
        var modalEl = document.getElementById('modalHopThuThread');
        if (modalEl && window.bootstrap && bootstrap.Modal) {
            var m = new bootstrap.Modal(modalEl);
            m.show();
        }
    }
});
