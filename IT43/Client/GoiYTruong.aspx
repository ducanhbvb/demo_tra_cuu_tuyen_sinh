<%@ Page Title="Gợi ý trường phù hợp" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="GoiYTruong.aspx.cs" Inherits="Client_GoiYTruong" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
<style>
/* ── Hero ─────────────────────────────────────────────────────────────── */
.goiy-hero {
    background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 60%, #a855f7 100%);
    border-radius: 20px;
    padding: 40px 36px 32px;
    color: #fff;
    margin-bottom: 28px;
    position: relative;
    overflow: hidden;
}
.goiy-hero::before {
    content: '';
    position: absolute;
    inset: -60px;
    background: radial-gradient(circle at 80% 20%, rgba(255,255,255,.12) 0%, transparent 60%);
}
.goiy-hero h1 { font-size: clamp(1.5rem, 3vw, 2.1rem); font-weight: 800; margin-bottom: 6px; }

/* ── Filter form ─────────────────────────────────────────────────────── */
.filter-card {
    background: var(--bs-body-bg, #fff);
    border: 1px solid var(--bs-border-color, #e2e8f0);
    border-radius: 16px;
    padding: 28px;
    box-shadow: 0 2px 12px rgba(0,0,0,.06);
    margin-bottom: 28px;
}
.filter-section-title {
    font-size: 13px; font-weight: 700; text-transform: uppercase;
    letter-spacing:.5px; color: #6366f1; margin-bottom: 12px;
}
.form-label { font-size: 13.5px; font-weight: 600; }

/* ── Score input group ───────────────────────────────────────────────── */
.score-group { position: relative; }
.score-group input[type=range] { width: 100%; accent-color: #6366f1; cursor: pointer; }
.score-display {
    display: inline-block; min-width: 60px; text-align: center;
    background: #6366f1; color: #fff; border-radius: 8px;
    font-weight: 800; font-size: 1.15rem; padding: 4px 12px;
}


/* ── Chip group (Khu vực, Loại trường) ──────────────────────────────── */
.chip-group { display: flex; gap: 6px; flex-wrap: wrap; }
.chip-group input[type=radio] { display: none; }
.chip-group label {
    display: inline-flex; align-items: center; gap: 5px;
    padding: 5px 14px; border-radius: 20px; cursor: pointer;
    font-size: 13px; font-weight: 600;
    border: 1.5px solid #e2e8f0;
    background: var(--bs-secondary-bg, #f8fafc);
    transition: all .15s ease;
}
.chip-group input[type=radio]:checked + label {
    background: #6366f1; color: #fff; border-color: #6366f1;
    box-shadow: 0 2px 8px rgba(99,102,241,.3);
}

/* ── Result cards ────────────────────────────────────────────────────── */
.truong-card {
    background: var(--bs-body-bg, #fff);
    border: 1px solid var(--bs-border-color, #e8ecf0);
    border-radius: 16px;
    overflow: hidden;
    box-shadow: 0 2px 12px rgba(0,0,0,.06);
    transition: transform .2s ease, box-shadow .2s ease;
    display: flex; flex-direction: column; height: 100%;
}
.truong-card:hover { transform: translateY(-4px); box-shadow: 0 8px 28px rgba(0,0,0,.12); }
.truong-card-img {
    width: 100%; height: 140px; object-fit: cover; background: #f1f5f9;
}
.truong-card-body { padding: 16px; flex: 1; }
.truong-card-name { font-weight: 700; font-size: 15px; line-height: 1.35; margin-bottom: 6px; color: var(--bs-body-color); }
.truong-card-meta { font-size: 12.5px; color: #64748b; margin-bottom: 8px; }
.badge-diem {
    display: inline-block; padding: 3px 10px; border-radius: 20px;
    font-size: 12px; font-weight: 700;
    background: #fee2e2; color: #b91c1c;
    border: 1px solid #fca5a5;
}
.badge-kd { background: #d1fae5; color: #065f46; border: 1px solid #6ee7b7; }
.school-level-badge {
    display: inline-flex; align-items: center; border-radius: 999px;
    padding: 3px 10px; font-size: 12px; font-weight: 700; color: #fff;
}
.school-level-university { background: rgba(15, 23, 42, .88); }
.school-level-college { background: rgba(124, 58, 237, .92); }
.school-level-vocational { background: rgba(8, 145, 178, .92); }

/* ── Load from Profile button ──────────────────────────────────────────── */
.goiy-hero .btn-light {
    background: rgba(255,255,255,.95);
    border: none;
    color: #4f46e5;
    padding: 8px 16px;
    border-radius: 10px;
    transition: all .2s ease;
}
.goiy-hero .btn-light:hover {
    background: #fff;
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(0,0,0,.15);
}
.fit-bar-wrap { background: #e2e8f0; border-radius: 10px; height: 6px; margin-top: 8px; }
.fit-bar { background: linear-gradient(90deg,#6366f1,#a855f7); border-radius: 10px; height: 100%; transition: width .5s ease; }
.truong-card-footer { padding: 10px 16px 14px; }
.truong-card-footer a { width: 100%; }

/* ── Empty state ─────────────────────────────────────────────────────── */
.empty-state {
    text-align: center; padding: 60px 20px;
    color: #94a3b8; font-size: 15px;
}
.empty-state i { font-size: 3.5rem; display: block; margin-bottom: 16px; color: #c7d2fe; }
</style>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- ══ Hero ════════════════════════════════════════════════════════════════ --%>
<div class="goiy-hero">
    <div style="position:relative;z-index:1;">
        <h1><i class="bi bi-stars me-2"></i>Gợi ý trường phù hợp</h1>
        <p class="mb-0 opacity-75" style="font-size:15px;">
            Nhập thông tin của bạn để chúng tôi tìm trường phù hợp nhất dựa trên điểm, ngành và sở thích.
        </p>
    </div>
</div>

<%-- ══ Form tiêu chí ════════════════════════════════════════════════════════ --%>
<div class="filter-card">
    <div class="row gy-4">
        <%-- Điểm dự kiến --%>
        <div class="col-12">
            <div class="filter-section-title"><i class="bi bi-calculator me-1"></i>Điểm dự kiến</div>
            <div class="row align-items-center g-3">
                <div class="col">
                    <div class="score-group">
                        <asp:TextBox ID="txtDiemSlider" runat="server" TextMode="Number"
                            CssClass="form-range" style="display:none;" min="0" max="30" step="0.25" />
                        <input type="range" id="sliderDiem" min="0" max="30" step="0.25" value="20"
                               oninput="syncDiem(this.value)" style="width:100%;accent-color:#6366f1;" />
                    </div>
                    <div class="d-flex justify-content-between mt-1" style="font-size:11px;color:#94a3b8;">
                        <span>0</span><span>10</span><span>20</span><span>30</span>
                    </div>
                </div>
                <div class="col-auto" style="min-width:120px;">
                    <asp:TextBox ID="txtDiem" runat="server" CssClass="form-control text-center fw-bold"
                        style="font-size:1.4rem;border-radius:12px;border-color:#6366f1;color:#4f46e5;width:110px;"
                        placeholder="0" />
                    <asp:HiddenField ID="hfDiemGoc" runat="server" />
                    <div style="font-size:11px;color:#94a3b8;margin-top:4px;text-align:center;">tổng 3 môn</div>
                </div>
            </div>
        </div>

        <%-- Hàng 2: Ngành quan tâm + Khu vực ưu tiên --%>
        <div class="col-md-5">
            <div class="filter-section-title"><i class="bi bi-diagram-3 me-1"></i>Ngành quan tâm</div>
            <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="col-md-7">
            <div class="filter-section-title"><i class="bi bi-geo-alt me-1"></i>Khu vực ưu tiên</div>
            <div class="chip-group">
                <input type="radio" name="khuVuc" id="kv0" value="" checked /><label for="kv0">Không có</label>
                <input type="radio" name="khuVuc" id="kv1" value="1" /><label for="kv1">KV1 (+0.75)</label>
                <input type="radio" name="khuVuc" id="kv2" value="2" /><label for="kv2">KV2 (+0.50)</label>
                <input type="radio" name="khuVuc" id="kv3" value="3" /><label for="kv3">KV3 (+0.25)</label>
            </div>
            <asp:HiddenField ID="hfKhuVuc" runat="server" />
        </div>

        <%-- Hàng 3: Loại trường + Cấp bậc + Tỉnh/TP --%>
        <div class="col-md-4">
            <div class="filter-section-title"><i class="bi bi-building me-1"></i>Loại trường</div>
            <asp:DropDownList ID="ddlLoaiTruong" runat="server" CssClass="form-select">
                <asp:ListItem Value="">Tất cả</asp:ListItem>
                <asp:ListItem Value="1">🏛 Công lập</asp:ListItem>
                <asp:ListItem Value="2">🏢 Tư thục</asp:ListItem>
                <asp:ListItem Value="3">🌐 Quốc tế</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="col-md-4">
            <div class="filter-section-title"><i class="bi bi-mortarboard me-1"></i>Cấp bậc đào tạo</div>
            <asp:DropDownList ID="ddlCapBacDaoTao" runat="server" CssClass="form-select">
                <asp:ListItem Value="">Tất cả</asp:ListItem>
                <asp:ListItem Value="1">🎓 Đại học</asp:ListItem>
                <asp:ListItem Value="2">📚 Cao đẳng</asp:ListItem>
                <asp:ListItem Value="3">🔧 Trường nghề</asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="col-md-4">
            <div class="filter-section-title"><i class="bi bi-map me-1"></i>Tỉnh / Thành phố</div>
            <asp:DropDownList ID="ddlTinh" runat="server" CssClass="form-select">
                <asp:ListItem Value="">Toàn quốc</asp:ListItem>
            </asp:DropDownList>
        </div>

        <%-- Hàng 4: Tổ hợp xét tuyển + Checkbox kiểm định --%>
        <div class="col-md-6">
            <div class="filter-section-title"><i class="bi bi-grid-3x3 me-1"></i>Tổ hợp xét tuyển</div>
            <asp:TextBox ID="txtToHopXetTuyen" runat="server" CssClass="form-control"
                placeholder="VD: A00, A01..." MaxLength="10"
                style="text-transform:uppercase;" />
            <small class="text-muted" style="font-size:11px;">Nhập mã tổ hợp (A00, A01, C00...)</small>
        </div>

        <div class="col-md-6">
            <div class="filter-section-title" style="opacity:0;">.</div>
            <div class="form-check mt-2">
                <asp:CheckBox ID="chkKiemDinh" runat="server" />
                <label class="form-check-label" for="<%: chkKiemDinh.ClientID %>">
                    <i class="bi bi-shield-check text-success me-1"></i>
                    Chỉ hiển thị trường đã kiểm định chất lượng (KĐCLGD)
                </label>
            </div>
        </div>

        <%-- Hàng 5: Các nút action --%>
        <div class="col-12">
            <div class="d-flex flex-wrap align-items-center justify-content-end gap-2">
                <%-- Chỉ hiển thị khi đã đăng nhập --%>
                <asp:LinkButton ID="btnLoadProfile" runat="server"
                    CssClass="btn btn-outline-secondary btn-sm"
                    OnClick="btnLoadProfile_Click"
                    Visible="false"
                    style="border-radius:8px;padding:6px 14px;">
                    <i class="bi bi-person-circle me-1"></i> Từ Profile
                </asp:LinkButton>
                
                <div class="d-flex align-items-center gap-2">
                    <label class="form-label mb-0" style="white-space:nowrap;font-size:13px;">Số kết quả:</label>
                    <asp:DropDownList ID="ddlTop" runat="server" CssClass="form-select form-select-sm" style="width:70px;">
                        <asp:ListItem Value="6">6</asp:ListItem>
                        <asp:ListItem Value="12" Selected="True">12</asp:ListItem>
                        <asp:ListItem Value="20">20</asp:ListItem>
                        <asp:ListItem Value="30">30</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <asp:Button ID="btnGoiY" runat="server" Text="✨ Gợi ý ngay"
                    CssClass="btn btn-primary fw-bold px-4"
                    style="border-radius:10px;background:linear-gradient(135deg,#4f46e5,#7c3aed);border:none;"
                    OnClick="btnGoiY_Click" />
            </div>
        </div>
    </div>
</div>

<%-- ══ Kết quả ════════════════════════════════════════════════════════════════ --%>
<asp:Panel ID="pnlKetQua" runat="server" Visible="false">
    <div class="d-flex align-items-center justify-content-between mb-3">
        <h5 class="fw-bold mb-0">
            <i class="bi bi-stars text-warning me-2"></i>
            <asp:Literal ID="litKetQuaTitle" runat="server" />
        </h5>
        <span class="badge rounded-pill" style="background:#eef2ff;color:#4f46e5;font-size:13px;padding:6px 14px;">
            <asp:Literal ID="litSoKetQua" runat="server" /> trường phù hợp
        </span>
    </div>
    <div class="row g-3">
        <asp:Repeater ID="rptKetQua" runat="server">
            <ItemTemplate>
                <div class="col-sm-6 col-lg-4">
                    <div class="truong-card">
                        <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string) ? "/Resources/Images/no-image.png" : Eval("AnhDaiDien") %>'
                             class="truong-card-img"
                             onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                        <div class="truong-card-body">
                            <div class="truong-card-name"><%# Eval("TenTruong") %></div>
                            <div class="truong-card-meta">
                                <i class="bi bi-geo-alt me-1"></i><%# Eval("TinhThanh") %> &nbsp;·&nbsp;
                                <span><%# Eval("TenLoaiTruong") %></span>
                            </div>
                            <div class="mb-2" style="font-size:13px;color:#475569;">
                                <i class="bi bi-diagram-3 me-1"></i><%# Eval("TenChuyenNganh") %>
                                <%# !string.IsNullOrEmpty(Eval("ToHopMonHoc") as string) ? "<br><i class='bi bi-grid-3x3 me-1'></i>" + Eval("ToHopMonHoc") : "" %>
                            </div>
                            <div class="d-flex gap-2 flex-wrap align-items-center">
                                <span class="badge-diem"><%# Eval("DiemChuanNamTruoc", "{0:F2}") %> điểm</span>
                                <%# GetCapBacBadge(Eval("CapBacDaoTao")) %>
                                <%# (bool)Eval("KiemDinhChatLuong") ? "<span class='badge-diem badge-kd'><i class='bi bi-shield-check me-1'></i>KĐ</span>" : "" %>
                            </div>
                            <%-- Progress bar tỉ lệ phù hợp --%>
                            <div class="mt-2" style="font-size:11.5px;color:#94a3b8;">
                                Độ phù hợp: <strong style="color:#6366f1;"><%# GetTiLePhuHop(Eval("TiLePhuHop")) %>%</strong>
                            </div>
                            <div class="fit-bar-wrap">
                                <div class="fit-bar" style="width:<%# GetTiLePhuHop(Eval("TiLePhuHop")) %>%;"></div>
                            </div>
                        </div>
                        <div class="truong-card-footer">
                            <a href='<%# ResolveUrl("~/Client/ChiTietTruong.aspx?slug=" + Eval("Slug")) %>'
                               class="btn btn-sm btn-outline-primary w-100 fw-semibold" style="border-radius:10px;">
                                Xem chi tiết <i class="bi bi-arrow-right ms-1"></i>
                            </a>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>

<%-- Empty state --%>
<asp:Panel ID="pnlEmpty" runat="server" Visible="false">
    <div class="empty-state">
        <i class="bi bi-search-heart"></i>
        Không tìm thấy trường phù hợp với tiêu chí đã chọn.<br>
        Thử điều chỉnh điểm dự kiến hoặc bỏ bớt bộ lọc nhé!
    </div>
</asp:Panel>

<%-- Default state --%>
<asp:Panel ID="pnlDefault" runat="server">
    <div class="empty-state">
        <i class="bi bi-stars" style="color:#c7d2fe;"></i>
        Nhập điểm dự kiến và nhấn <strong>Gợi ý ngay</strong> để xem các trường phù hợp với bạn!
    </div>
</asp:Panel>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// ── Điểm gốc (không có ưu tiên) ─────────────────────────────────
var _hfDiemGoc = document.getElementById('<%: hfDiemGoc.ClientID %>');
var _txtDiem   = document.getElementById('<%: txtDiem.ClientID %>');
var _slider    = document.getElementById('sliderDiem');
var _hfKhuVuc  = document.getElementById('<%: hfKhuVuc.ClientID %>');

function getKvBonus() {
    var el = document.querySelector('input[name="khuVuc"]:checked');
    if (!el || !el.value) return 0;
    switch (parseInt(el.value)) {
        case 1: return 0.75;
        case 2: return 0.50;
        case 3: return 0.25;
        default: return 0;
    }
}

// Cập nhật ô điểm = điểm gốc + bonus khu vực
function applyBonus() {
    var goc   = parseFloat(_hfDiemGoc.value) || 0;
    var bonus = getKvBonus();
    _txtDiem.value = (goc + bonus).toFixed(2);
}

// Khi kéo slider → lưu điểm gốc → cập nhật ô điểm
function syncDiem(val) {
    _hfDiemGoc.value = parseFloat(val).toFixed(2);
    applyBonus();
}

// Khi nhập tay vào ô điểm → tính lại điểm gốc (trừ bonus)
_txtDiem.addEventListener('input', function () {
    var v     = parseFloat(this.value) || 0;
    var bonus = getKvBonus();
    var goc   = Math.max(0, v - bonus);
    _hfDiemGoc.value = goc.toFixed(2);
    if (goc >= 0 && goc <= 30) _slider.value = goc;
});

// Khi chọn khu vực → lưu hidden, cập nhật ô điểm
document.querySelectorAll('input[name="khuVuc"]').forEach(function (el) {
    el.addEventListener('change', function () {
        _hfKhuVuc.value = this.value;
        applyBonus();
    });
});

// ── Khởi tạo ─────────────────────────────────────────────────────
(function init() {
    var initialDiem = <%: PreFillDiem > 0 ? PreFillDiem.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) : "20" %>;
    var currentTxt  = _txtDiem.value;

    // Restore KV từ hidden field (sau postback)
    var savedKv = _hfKhuVuc.value;
    if (savedKv) {
        var radio = document.querySelector('input[name="khuVuc"][value="' + savedKv + '"]');
        if (radio) radio.checked = true;
    }

    if (currentTxt && currentTxt !== "") {
        // Sau postback: điểm trong txtDiem đã là điểm gốc (server trả về)
        _hfDiemGoc.value = currentTxt;
        _slider.value    = Math.min(30, parseFloat(currentTxt) || 20);
        applyBonus();
    } else {
        _hfDiemGoc.value = parseFloat(initialDiem).toFixed(2);
        _slider.value    = initialDiem;
        applyBonus();
    }
})();
</script>
</asp:Content>
