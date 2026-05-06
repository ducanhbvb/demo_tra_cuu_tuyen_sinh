<%@ Page Title="Thông tin tuyển sinh" Language="C#" MasterPageFile="~/TruongHoc/TruongHoc.Master"
   CodeBehind="QuanLyTinTuyenSinh.aspx.cs" Inherits="TruongHoc_QuanLyTinTuyenSinh"
   ValidateRequest="false" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<!-- Page Header -->
<div class="page-header mb-4">
    <div>
        <h4 class="page-header-title">
            <i class="bi bi-megaphone" style="color: var(--accent);"></i> Thông tin tuyển sinh
        </h4>
        <div class="page-header-sub">
            Cổng Trường / <strong>Tuyển sinh</strong> — Quản lý thông tin tuyển sinh của trường bạn
        </div>
    </div>
    <button type="button" class="btn btn-success" data-bs-toggle="modal" data-bs-target="#modalTin">
        <i class="bi bi-plus-lg me-1"></i>Thêm mới
    </button>
</div>

<%-- Thống kê --%>
<div class="row g-3 mb-4">
    <div class="col-md-4">
        <div class="stat-card grad-purple">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litTongTin" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Tổng tin tuyển sinh</div>
                </div>
                <i class="bi bi-newspaper stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="stat-card grad-green">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litHienThi" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Hiển thị công khai</div>
                </div>
                <i class="bi bi-eye stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="stat-card grad-blue">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litLuotXem" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Tổng lượt xem</div>
                </div>
                <i class="bi bi-graph-up-arrow stat-icon"></i>
            </div>
        </div>
    </div>
</div>

<%-- Filter --%>
<div class="admin-filter-bar mb-4">
    <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select" style="max-width:200px;">
        <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlNam" runat="server" CssClass="form-select" style="max-width:110px;">
        <asp:ListItem Value="">-- Năm --</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc" CssClass="btn-filter" OnClick="btnLoc_Click" />
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="admin-card">
    <div class="admin-card-body p-0">
        <asp:GridView ID="gvTin" runat="server"
            EnableViewState="false"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaTin" OnRowCommand="gvTin_RowCommand"
            UseAccessibleHeader="true"
            EmptyDataText="Chưa có tin tuyển sinh nào.">
            <Columns>
                <asp:BoundField DataField="TenChuyenNganh"    HeaderText="Ngành"        HtmlEncode="true" />
                <asp:BoundField DataField="TenPhuongThuc"     HeaderText="Phương thức"  HtmlEncode="true" />
                <asp:BoundField DataField="NamTuyenSinh"      HeaderText="Năm"          />
                <asp:BoundField DataField="DiemChuanNamTruoc" HeaderText="Điểm chuẩn"  DataFormatString="{0:F2}" />
                <asp:BoundField DataField="ChiTieu"           HeaderText="Chỉ tiêu"    />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# (bool)Eval("TrangThai")
                            ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hiện</span>"
                            : "<span class='badge badge-soft-secondary'><i class='bi bi-eye-slash me-1'></i>Ẩn</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="Toggle"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass='<%# (bool)Eval("TrangThai") ? "btn-action btn-toggle" : "btn-action btn-toggle text-success" %>'
                            OnClientClick="return confirm('Thay đổi trạng thái hiển thị của tin này?')" title="Ẩn / Hiện">
                            <%# (bool)Eval("TrangThai") ? "<i class='bi bi-eye-slash'></i>" : "<i class='bi bi-eye'></i>" %>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Sua"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass="btn-action btn-edit" title="Sửa tin tuyển sinh">
                            <i class="bi bi-pencil-square"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Xoa"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass="btn-action btn-delete"
                            OnClientClick="return confirm('CẢNH BÁO: Đây là thao tác XÓA VĨNH VIỄN tin tuyển sinh này. Bạn đã chắc chắn muốn tiếp tục?')"
                            title="Xóa vĩnh viễn">
                            <i class="bi bi-trash"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<div class="d-flex justify-content-between mt-3">
    <span class="text-muted small">Tổng: <strong><asp:Literal ID="litTong" runat="server" /></strong></span>
    <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
        <HeaderTemplate><ul class="pagination pagination-sm mb-0"></HeaderTemplate>
        <ItemTemplate>
            <li class='page-item <%# (bool)Eval("IsActive") ? "active" : "" %>'>
                <asp:LinkButton runat="server" CssClass="page-link"
                    CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>'>
                    <%# Eval("PageText") %>
                </asp:LinkButton>
            </li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
</div>

<%-- MODAL THÊM/SỬA --%>
<asp:HiddenField ID="hfMaTin" runat="server" />
<div class="modal fade" id="modalTin" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold">Thêm / Sửa tin tuyển sinh</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <%-- Mã tin (ID) — chỉ hiện khi sửa --%>
                <div id="panelMaTin" class="d-none mb-3">
                    <div class="d-flex align-items-center gap-2 flex-wrap" style="background:var(--bs-tertiary-bg,#f8fafc);border-radius:10px;padding:.6rem .9rem;">
                        <span class="text-muted small">Mã tin:</span>
                        <span class="fw-bold" id="lblMaTinDisplay" style="font-size:1rem;letter-spacing:.02em">—</span>
                        <button type="button" class="btn-copy-slug" id="btnCopyMaTin" title="Sao chép ID"
                            data-source-text="lblMaTinDisplay">
                            <i class="bi bi-clipboard"></i>
                        </button>
                        <span class="text-muted" style="font-size:.75rem">·</span>
                        <a id="lnkChiTiet" href="#" target="_blank" class="small text-primary text-decoration-none" style="font-family:monospace">
                            /Client/ChiTietTinTuyenSinh.aspx?id=<span id="lblMaTinInUrl">—</span>
                        </a>
                        <button type="button" class="btn-copy-slug" id="btnCopyLink" title="Sao chép link"
                            data-source-text="lnkChiTiet">
                            <i class="bi bi-clipboard"></i>
                        </button>
                    </div>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Chuyên ngành <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMNganh" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Phương thức xét tuyển <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMPhuongThuc" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Năm tuyển sinh <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtMNam" runat="server" CssClass="form-control"
                            TextMode="Number" MaxLength="4" placeholder="2025" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Chỉ tiêu</label>
                        <asp:TextBox ID="txtMChiTieu" runat="server" CssClass="form-control"
                            TextMode="Number" placeholder="200" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Điểm chuẩn năm trước</label>
                        <asp:TextBox ID="txtMDiemTruoc" runat="server" CssClass="form-control"
                            TextMode="SingleLine" placeholder="VD: 25,50" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Điểm chuẩn năm nay</label>
                        <asp:TextBox ID="txtMDiemNay" runat="server" CssClass="form-control"
                            TextMode="SingleLine" placeholder="VD: 25,50" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Tổ hợp môn</label>
                        <asp:TextBox ID="txtMToHop" runat="server" CssClass="form-control"
                            MaxLength="200" placeholder="A00,A01,D01" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Học phí</label>
                        <asp:TextBox ID="txtMHocPhi" runat="server" CssClass="form-control"
                            MaxLength="100" placeholder="VD: 15 triệu/năm hoặc 10-20 triệu" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Loại hình đào tạo</label>
                        <asp:TextBox ID="txtMLoaiHinh" runat="server" CssClass="form-control"
                            MaxLength="100" placeholder="Chính quy" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Hạn nộp hồ sơ</label>
                        <asp:TextBox ID="txtMHanNop" runat="server" CssClass="form-control date-input"
                            TextMode="Date" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Tiêu đề bài đăng</label>
                        <asp:TextBox ID="txtMTieuDe" runat="server" CssClass="form-control"
                            MaxLength="200"
                            placeholder="Để trống sẽ tự sinh: Tuyển sinh 2025 - Ngành - Phương thức" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Mô tả chi tiết</label>
                        <asp:TextBox ID="txtMMoTa" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="4" placeholder="Mô tả yêu cầu, điều kiện xét tuyển..." />
                    </div>
                    <div class="col-md-4 d-flex align-items-end">
                        <div class="form-check">
                            <asp:CheckBox ID="chkTrangThai" runat="server" CssClass="form-check-input" Checked="true" />
                            <label class="form-check-label">Hiển thị tin này</label>
                        </div>
                    </div>
                </div>
                <asp:Literal ID="litModalLoi" runat="server" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary rounded-pill px-4" data-bs-dismiss="modal">Hủy</button>
                <button type="button" class="btn btn-outline-warning rounded-pill px-4" onclick="resetModalTin()" title="Xóa trắng toàn bộ dữ liệu đã nhập">
                    <i class="bi bi-arrow-counterclockwise me-1"></i>Nhập lại
                </button>
                <asp:Button ID="btnLuu" runat="server" Text="Lưu tin"
                    CssClass="btn btn-success rounded-pill px-4" OnClick="btnLuu_Click"
                    CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
    <script src="/Scripts/validation_data_input.js"></script>
    <script>
    function resetModalTin() {
        if (!confirm('Bạn có chắc muốn xóa trắng toàn bộ dữ liệu đã nhập?')) return;
        document.getElementById('<%= hfMaTin.ClientID %>').value = '0';
        // Dropdowns về option đầu tiên
        ['<%= ddlMNganh.ClientID %>', '<%= ddlMPhuongThuc.ClientID %>'].forEach(function(id) {
            var el = document.getElementById(id);
            if (el) el.selectedIndex = 0;
        });
        // Text inputs
        ['<%= txtMNam.ClientID %>', '<%= txtMChiTieu.ClientID %>', '<%= txtMDiemTruoc.ClientID %>',
         '<%= txtMDiemNay.ClientID %>', '<%= txtMToHop.ClientID %>', '<%= txtMHocPhi.ClientID %>',
         '<%= txtMLoaiHinh.ClientID %>', '<%= txtMHanNop.ClientID %>', '<%= txtMTieuDe.ClientID %>',
         '<%= txtMMoTa.ClientID %>'].forEach(function(id) {
            var el = document.getElementById(id);
            if (el) el.value = '';
        });
        // Năm về năm hiện tại
        document.getElementById('<%= txtMNam.ClientID %>').value = '<%= DateTime.Now.Year %>';
        // Checkbox hiển thị → mặc định checked
        var chk = document.getElementById('<%= chkTrangThai.ClientID %>');
        if (chk) chk.checked = true;
        // Xóa trạng thái validate cũ
        document.querySelectorAll('#modalTin .is-invalid, #modalTin .is-valid').forEach(function(el) {
            el.classList.remove('is-invalid', 'is-valid');
        });
        document.querySelectorAll('#modalTin .invalid-feedback').forEach(function(fb) {
            fb.textContent = '';
        });
    }

    document.addEventListener('DOMContentLoaded', function() {
        // ── AdminValidator ──────────────────────────────────────────────────
        if (window.AdminValidator) {
            var R = AdminValidator.rules;
            AdminValidator.init('form', {
                '<%= ddlMNganh.ClientID %>':      [R.required('Vui lòng chọn ngành')],
                '<%= ddlMPhuongThuc.ClientID %>': [R.required('Vui lòng chọn phương thức xét tuyển')],
                '<%= txtMNam.ClientID %>':        [R.required('Nhập năm tuyển sinh'),
                                                   R.numRange(2020, new Date().getFullYear() + 1, 'Năm tuyển sinh không hợp lệ')],
                '<%= txtMChiTieu.ClientID %>':    [R.intPositive('Chỉ tiêu phải là số nguyên dương')],
                '<%= txtMDiemTruoc.ClientID %>':  [R.numRange(0, 40, 'Điểm chuẩn phải trong khoảng 0 – 40')],
                '<%= txtMDiemNay.ClientID %>':    [R.numRange(0, 40, 'Điểm chuẩn phải trong khoảng 0 – 40')],
                '<%= txtMHocPhi.ClientID %>':     [R.maxLen(100, 'Học phí tối đa 100 ký tự')],
                '<%= txtMToHop.ClientID %>':      [R.toHopMon('Sai định dạng. VD: A00,A01,D01')],
                '<%= txtMTieuDe.ClientID %>':     [R.maxLen(200, 'Tiêu đề tối đa 200 ký tự')],
                '<%= txtMLoaiHinh.ClientID %>':   [R.maxLen(100, 'Loại hình đào tạo tối đa 100 ký tự')]
            }, { triggerButtonId: '<%= btnLuu.ClientID %>' });
        }

        // ── Hiện/ẩn panel Mã tin dựa vào hfMaTin ─────────────────────────
        function syncMaTinPanel() {
            var maTin = document.getElementById('<%= hfMaTin.ClientID %>').value;
            var panel    = document.getElementById('panelMaTin');
            var lblDisp  = document.getElementById('lblMaTinDisplay');
            var lblInUrl = document.getElementById('lblMaTinInUrl');
            var lnk      = document.getElementById('lnkChiTiet');
            if (!panel) return;
            if (maTin && maTin !== '0' && maTin !== '') {
                panel.classList.remove('d-none');
                if (lblDisp)  lblDisp.textContent  = maTin;
                if (lblInUrl) lblInUrl.textContent = maTin;
                if (lnk)      lnk.href = '/Client/ChiTietTinTuyenSinh.aspx?id=' + maTin;
            } else {
                panel.classList.add('d-none');
            }
        }

        // Mở modal sau postback
        if ('<%= ShowModal %>' === 'true') {
            var modalEl = document.getElementById('modalTin');
            if (modalEl && typeof bootstrap !== 'undefined') {
                bootstrap.Modal.getOrCreateInstance(modalEl).show();
            }
        }

        // Đồng bộ panel khi modal được show (cả thêm mới lẫn sửa)
        var modalTinEl = document.getElementById('modalTin');
        if (modalTinEl) {
            modalTinEl.addEventListener('show.bs.modal', syncMaTinPanel);
        }
        syncMaTinPanel(); // chạy ngay nếu modal đã mở sau postback
    });
    </script>
</asp:Content>
