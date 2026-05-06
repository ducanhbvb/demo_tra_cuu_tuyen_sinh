<%@ Page Title="Tin tuyển sinh" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyTinTuyenSinh.aspx.cs" Inherits="Admin_QuanLyTinTuyenSinh" ValidateRequest="false" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<!-- Page Header -->
<div class="page-header mb-4">
    <div>
        <h4 class="page-header-title">
            <i class="bi bi-megaphone" style="color: var(--accent);"></i> Quản lý Tin tuyển sinh
        </h4>
        <div class="page-header-sub">
            Dashboard / <strong>Tuyển sinh</strong> — Đăng thông tin tuyển sinh định mức của các trường
        </div>
    </div>
    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalThem">
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
    <asp:DropDownList ID="ddlTruong" runat="server" CssClass="form-select" style="max-width:220px;">
        <asp:ListItem Value="">-- Chọn trường --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select" style="max-width:180px;">
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
            EmptyDataText="Chưa có dữ liệu.">
            <Columns>
                <asp:BoundField DataField="TenTruong"          HeaderText="Trường"          HtmlEncode="true" />
                <asp:BoundField DataField="TenChuyenNganh"     HeaderText="Ngành"           HtmlEncode="true" />
                <asp:BoundField DataField="TenPhuongThuc"      HeaderText="Phương thức"     HtmlEncode="true" />
                <asp:BoundField DataField="NamTuyenSinh"       HeaderText="Năm"             />
                <asp:BoundField DataField="DiemChuanNamTruoc"  HeaderText="Điểm chuẩn"
                    DataFormatString="{0:F2}" />
                <asp:BoundField DataField="ChiTieu"            HeaderText="Chỉ tiêu"        />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# GetTrangThaiBadge(Eval("TrangThai")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="ToggleTin"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass='<%# SafeGetBool(Eval("TrangThai")) ? "btn-action btn-toggle" : "btn-action btn-toggle text-success" %>'
                            OnClientClick="return confirm('Thay đổi trạng thái hiển thị tin này?')"
                            title="Ẩn / Hiện tin tuyển sinh">
                            <%# SafeGetBool(Eval("TrangThai")) ? "<i class='bi bi-eye-slash'></i>" : "<i class='bi bi-eye'></i>" %>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="SuaTin"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass="btn-action btn-edit"
                            title="Sửa tin tuyển sinh">
                            <i class="bi bi-pencil-square"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="XoaTin"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass="btn-action btn-delete"
                            OnClientClick="return confirm('CẢNH BÁO: Đây là thao tác XÓA VĨNH VIỄN tin tuyển sinh này. Tin đã xóa sẽ không khôi phục được từ giao diện. Bạn đã chắc chắn muốn tiếp tục?')"
                            title="Xóa vĩnh viễn tin tuyển sinh">
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
            <li class='page-item <%# SafeGetBool(Eval("IsActive")) ? "active" : "" %>'>
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
<div class="modal fade" id="modalThem" tabindex="-1">
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
                            /ChiTietTinTuyenSinh.aspx?id=<span id="lblMaTinInUrl">—</span>
                        </a>
                        <button type="button" class="btn-copy-slug" id="btnCopyLink" title="Sao chép link"
                            data-source-text="lnkChiTiet">
                            <i class="bi bi-clipboard"></i>
                        </button>
                    </div>
                </div>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">Trường <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMTruong" runat="server" CssClass="form-select" data-preview-source="admission-school">
                            <asp:ListItem Value="">-- Chọn trường --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Ngành <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMNganh" runat="server" CssClass="form-select" data-preview-source="admission-major">
                            <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Phương thức <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMPhuongThuc" runat="server" CssClass="form-select" data-preview-source="admission-method">
                            <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Năm tuyển sinh <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtMNam" runat="server" CssClass="form-control"
                            TextMode="Number" MaxLength="4" placeholder="2025" data-preview-source="admission-year" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Chỉ tiêu</label>
                        <asp:TextBox ID="txtMChiTieu" runat="server" CssClass="form-control"
                            TextMode="Number" placeholder="200" data-preview-source="admission-quota" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Điểm chuẩn năm trước</label>
                        <asp:TextBox ID="txtMDiemTruoc" runat="server" CssClass="form-control"
                            TextMode="SingleLine" placeholder="VD: 25,50" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Điểm chuẩn năm nay</label>
                        <asp:TextBox ID="txtMDiemNay" runat="server" CssClass="form-control"
                            TextMode="SingleLine" placeholder="VD: 25,50" data-preview-source="admission-score" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Tổ hợp môn</label>
                        <asp:TextBox ID="txtMToHop" runat="server" CssClass="form-control"
                            MaxLength="200" placeholder="A00,A01,D01" data-preview-source="admission-subject" />
                    </div>
                    <div class="col-md-6">
                        <%-- Sprint 1: Học phí đổi sang text tự do --%>
                        <label class="form-label">Học phí</label>
                        <asp:TextBox ID="txtMHocPhi" runat="server" CssClass="form-control"
                            MaxLength="100" placeholder="VD: 15 triệu/năm hoặc 10-20 triệu" data-preview-source="admission-fee" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Loại hình đào tạo</label>
                        <asp:TextBox ID="txtMLoaiHinh" runat="server" CssClass="form-control"
                            MaxLength="100" placeholder="Chính quy" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Cơ sở đào tạo</label>
                        <asp:TextBox ID="txtMCoSo" runat="server" CssClass="form-control"
                            MaxLength="200" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Tiêu đề bài đăng</label>
                        <asp:TextBox ID="txtMTieuDe" runat="server" CssClass="form-control"
                            MaxLength="200"
                            placeholder="Để trống sẽ tự sinh: Tuyển sinh 2026 - Ngành - Phương thức" data-preview-source="admission-title" />
                    </div>
                    <div class="col-md-6 admission-deadline-field">
                        <label class="form-label">Hạn nộp hồ sơ</label>
                        <asp:TextBox ID="txtMHanNop" runat="server" CssClass="form-control date-input" TextMode="Date" data-preview-source="admission-deadline" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Mô tả chi tiết</label>
                        <asp:TextBox ID="txtMMoTa" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="4" placeholder="Mô tả yêu cầu, điều kiện xét tuyển..." data-preview-source="admission-note" />
                    </div>
                    <div class="col-md-4 d-flex align-items-end">
                        <div class="form-check">
                            <input type="checkbox" id="chkMActive" runat="server" class="form-check-input" checked />
                            <label class="form-check-label">Hiển thị</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary rounded-pill px-4" data-bs-dismiss="modal">Hủy</button>
                <button type="button" class="btn btn-outline-warning rounded-pill px-4" onclick="resetModalThem()" title="Xóa trắng toàn bộ dữ liệu đã nhập">
                    <i class="bi bi-arrow-counterclockwise me-1"></i>Nhập lại
                </button>
                <button type="button" class="btn btn-outline-info rounded-pill px-4" data-admin-preview="admission">
                    <i class="bi bi-eye me-1"></i>Preview
                </button>
                <asp:Button ID="btnLuuTin" runat="server" Text="Lưu thông tin"
                    CssClass="btn btn-admin-primary rounded-pill px-4" OnClick="btnLuuTin_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

<%-- MODAL PREVIEW TIN TUYỂN SINH --%>
<div class="modal fade" id="modalPreviewTin" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold"><i class="bi bi-eye me-2 text-info"></i>Preview tin tuyển sinh</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="admin-admission-preview-card">
                    <img id="pvTinImg" src="/Resources/Images/no-image.png" alt="Preview tin tuyển sinh" style="display:none;" />
                    <div class="admin-admission-preview-body">
                        <span id="pvTinMethod" class="admin-admission-preview-badge">Phương thức</span>
                        <h4 id="pvTinTitle" class="fw-bold mb-2">Tin tuyển sinh</h4>
                        <div class="text-muted mb-2"><span id="pvTinSchool">Trường</span> · <span id="pvTinMajor">Ngành</span></div>
                        <div id="pvTinNote" class="mb-3">Ghi chú.</div>
                        <div class="admin-admission-preview-grid">
                            <div class="admin-admission-preview-item"><span>Năm</span><strong id="pvTinYear">--</strong></div>
                            <div class="admin-admission-preview-item"><span>Chỉ tiêu</span><strong id="pvTinQuota">--</strong></div>
                            <div class="admin-admission-preview-item"><span>Điểm dự kiến</span><strong id="pvTinScore">--</strong></div>
                            <div class="admin-admission-preview-item"><span>Học phí</span><strong id="pvTinFee">--</strong></div>
                            <div class="admin-admission-preview-item"><span>Tổ hợp</span><strong id="pvTinSubject">--</strong></div>
                            <div class="admin-admission-preview-item"><span>Hạn nộp</span><strong id="pvTinDeadline">--</strong></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
    <%-- Load TinyMCE and init script --%>
    <script src="<%: ResolveUrl("~/lib/tinymce/tinymce.min.js") %>"></script>
    <script src="/Scripts/tinymce-init.js"></script>
    <%-- Sprint 1: Module validate chung cho Admin modal --%>
    <script src="/Scripts/validation_data_input.js"></script>
    <script>
        window.AppConfig = window.AppConfig || {};
        window.AppConfig.AllowPastDates = <%= ConfigBLL.GetBool("AllowPastDates", false).ToString().ToLower() %>;
        window.showTinTuyenSinhModal = '<%= ShowModal %>';

        /** Reset toàn bộ các ô nhập trong modal Thêm/Sửa về trạng thái trống */
        function resetModalThem() {
            if (!confirm('Bạn có chắc muốn xóa trắng toàn bộ dữ liệu đã nhập?')) return;
            // Hidden field
            document.getElementById('<%= hfMaTin.ClientID %>').value = '';
            // Dropdowns → về option đầu tiên (rỗng)
            ['<%= ddlMTruong.ClientID %>', '<%= ddlMNganh.ClientID %>', '<%= ddlMPhuongThuc.ClientID %>'].forEach(function(id) {
                var el = document.getElementById(id);
                if (el) el.selectedIndex = 0;
            });
            // Text inputs
            ['<%= txtMNam.ClientID %>', '<%= txtMChiTieu.ClientID %>', '<%= txtMDiemTruoc.ClientID %>',
             '<%= txtMDiemNay.ClientID %>', '<%= txtMToHop.ClientID %>', '<%= txtMHocPhi.ClientID %>',
             '<%= txtMLoaiHinh.ClientID %>', '<%= txtMCoSo.ClientID %>', '<%= txtMTieuDe.ClientID %>',
             '<%= txtMHanNop.ClientID %>'].forEach(function(id) {
                var el = document.getElementById(id);
                if (el) el.value = '';
            });
            // Textarea / TinyMCE mô tả
            var moTaId = '<%= txtMMoTa.ClientID %>';
            var ta = document.getElementById(moTaId);
            if (ta) ta.value = '';
            if (typeof tinymce !== 'undefined' && tinymce.get(moTaId)) {
                tinymce.get(moTaId).setContent('');
            }
            // Checkbox hiển thị → mặc định checked
            var chk = document.getElementById('<%= chkMActive.ClientID %>');
            if (chk) chk.checked = true;
            // Xóa trạng thái validate cũ
            document.querySelectorAll('#modalThem .is-invalid, #modalThem .is-valid').forEach(function(el) {
                el.classList.remove('is-invalid', 'is-valid');
            });
            document.querySelectorAll('#modalThem .invalid-feedback').forEach(function(fb) {
                fb.textContent = '';
            });
        }

        // Khởi tạo TinyMCE cho mô tả tin tuyển sinh
        document.addEventListener('DOMContentLoaded', function() {
            if (typeof CmsEditor !== 'undefined') {
                CmsEditor.init('<%= txtMMoTa.ClientID %>');
            }

            // Mở lại modal sau postback nếu server set ShowModal = "true"
            // (VD: validation BLL thất bại → giữ modal mở để user sửa lỗi)
            if (window.showTinTuyenSinhModal === 'true') {
                var modalEl = document.getElementById('modalThem');
                if (modalEl && typeof bootstrap !== 'undefined') {
                    var bsModal = bootstrap.Modal.getOrCreateInstance(modalEl);
                    bsModal.show();
                }
            }

            // ── AdminValidator: 3-tầng validate cho modal Thêm/Sửa tin tuyển sinh ──
            // Chỉ chạy khi click nút Lưu, không validate khi Filter/Paging/Xóa
            if (window.AdminValidator) {
                var R = AdminValidator.rules;
                AdminValidator.init('form', {
                    '<%= ddlMTruong.ClientID %>':     [R.required('Vui lòng chọn trường')],
                    '<%= ddlMNganh.ClientID %>':      [R.required('Vui lòng chọn ngành')],
                    '<%= ddlMPhuongThuc.ClientID %>': [R.required('Vui lòng chọn phương thức xét tuyển')],
                    '<%= txtMNam.ClientID %>':        [R.required('Nhập năm tuyển sinh'), R.numRange(2020, new Date().getFullYear() + 1, 'Năm tuyển sinh không hợp lệ')],
                    '<%= txtMChiTieu.ClientID %>':    [R.intPositive('Chỉ tiêu phải là số nguyên dương')],
                    '<%= txtMDiemTruoc.ClientID %>':  [R.numRange(0, 40, 'Điểm chuẩn phải trong khoảng 0 – 40')],
                    '<%= txtMDiemNay.ClientID %>':    [R.numRange(0, 40, 'Điểm chuẩn phải trong khoảng 0 – 40')],
                    '<%= txtMHocPhi.ClientID %>':     [R.maxLen(100, 'Học phí tối đa 100 ký tự')],
                    '<%= txtMToHop.ClientID %>':      [R.toHopMon('Sai định dạng. VD: A00,A01,D01')],
                    '<%= txtMTieuDe.ClientID %>':     [R.maxLen(200, 'Tiêu đề tối đa 200 ký tự')],
                    '<%= txtMHanNop.ClientID %>':     [R.dateNotPast('Hạn nộp hồ sơ không được trong quá khứ')],
                    '<%= txtMLoaiHinh.ClientID %>':   [R.maxLen(100, 'Loại hình đào tạo tối đa 100 ký tự')],
                    '<%= txtMCoSo.ClientID %>':       [R.maxLen(200, 'Cơ sở đào tạo tối đa 200 ký tự')]
                }, {
                    // Chỉ validate khi nhấn nút Lưu thông tin, không trigger khi Filter/Paging
                    triggerButtonId: '<%= btnLuuTin.ClientID %>'
                });
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

            // Đồng bộ panel khi modal show
            var modalEl = document.getElementById('modalThem');
            if (modalEl) {
                modalEl.addEventListener('show.bs.modal', syncMaTinPanel);
            }
            syncMaTinPanel();
        });
    </script>
</asp:Content>
