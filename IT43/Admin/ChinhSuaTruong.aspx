<%@ Page Title="Thêm / Sửa trường" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="ChinhSuaTruong.aspx.cs" Inherits="Admin_ChinhSuaTruong" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex gap-2 align-items-center mb-3">
    <a href="QuanLyTruong.aspx" class="btn btn-sm btn-outline-secondary">
        <i class="bi bi-arrow-left me-1"></i>Quay lại
    </a>
    <h5 class="fw-bold mb-0 ms-2">
        <asp:Literal ID="litTieuDe" runat="server" />
    </h5>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<%-- ── TABS ──────────────────────────────────────────────────── --%>
<ul class="nav nav-tabs mb-3" id="truongTabs">
    <li class="nav-item">
        <a class="nav-link <%: ActiveTab == "diemchuan" ? "" : "active" %>"
           href='<%: IsEdit ? "ChinhSuaTruong.aspx?id=" + ID + "&tab=thongtin" : "ChinhSuaTruong.aspx" %>'>
            <i class="bi bi-info-circle me-1"></i>Thông tin chung
        </a>
    </li>
    <asp:PlaceHolder ID="phTabDiemChuan" runat="server" Visible="false">
    <li class="nav-item">
        <a class="nav-link <%: ActiveTab == "diemchuan" ? "active" : "" %>"
           href='<%: "ChinhSuaTruong.aspx?id=" + ID + "&tab=diemchuan" %>'>
            <i class="bi bi-graph-up me-1"></i>Điểm chuẩn các năm
        </a>
    </li>
    </asp:PlaceHolder>
</ul>

<%-- ══════════════════════════════════════════════════════════════
     TAB 1 — THÔNG TIN CHUNG
     ══════════════════════════════════════════════════════════════ --%>
<asp:Panel ID="panelThongTin" runat="server">
<div class="admin-card">
    <div class="admin-card-body">
        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label fw-semibold">Tên trường <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtTen" runat="server" CssClass="form-control" MaxLength="200" />
                <asp:RequiredFieldValidator ControlToValidate="txtTen" runat="server"
                    CssClass="text-danger small" ErrorMessage="Bắt buộc." Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label class="form-label fw-semibold">Slug (SEO)</label>
                <div class="slug-copy-wrap">
                    <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control"
                        placeholder="tu-dong-tao-neu-de-trong" />
                    <button type="button" class="btn-copy-slug" title="Sao chép slug"
                        data-source="<%= txtSlug.ClientID %>">
                        <i class="bi bi-clipboard"></i>
                    </button>
                </div>
            </div>
            <div class="col-md-6">
                <label class="form-label fw-semibold">Địa chỉ</label>
                <asp:TextBox ID="txtDiaChi" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Tỉnh / Thành phố</label>
                <asp:DropDownList ID="ddlTinhThanh" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Chọn tỉnh/TP --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Khu vực</label>
                <asp:DropDownList ID="ddlVung" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                    <asp:ListItem Value="1">Miền Bắc</asp:ListItem>
                    <asp:ListItem Value="2">Miền Trung</asp:ListItem>
                    <asp:ListItem Value="3">Miền Nam</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Loại trường</label>
                <asp:DropDownList ID="ddlLoai" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                    <asp:ListItem Value="1">Công lập</asp:ListItem>
                    <asp:ListItem Value="2">Tư thục</asp:ListItem>
                    <asp:ListItem Value="3">Quốc tế</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Cấp bậc đào tạo</label>
                <asp:DropDownList ID="ddlCapBacDaoTao" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                    <asp:ListItem Value="1">Đại học</asp:ListItem>
                    <asp:ListItem Value="2">Cao Đẳng</asp:ListItem>
                    <asp:ListItem Value="3">Trường nghề</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Điện thoại</label>
                <asp:TextBox ID="txtSdt" runat="server" CssClass="form-control"
                    TextMode="Phone" MaxLength="20" placeholder="0xxxxxxxxx" />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Website</label>
                <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control"
                    TextMode="SingleLine" MaxLength="200" placeholder="https://..." />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Quy mô</label>
                <asp:TextBox ID="txtQuyMo" runat="server" CssClass="form-control" placeholder="VD: 30.000 SV" />
            </div>
            <%-- Ảnh đại diện --%>
            <div class="col-md-6">
                <label class="form-label fw-semibold">Ảnh đại diện</label>
                <asp:HiddenField ID="hfAnhDaiDien" runat="server" />
                <div class="mb-2" data-preview-wrap="avatar" style="display:none;">
                    <asp:Image ID="imgPreviewAvatar" data-preview-target="avatar" runat="server"
                        CssClass="img-thumbnail"
                        style="width:120px;height:120px;object-fit:cover;cursor:pointer;border-radius:8px;"
                        AlternateText="Ảnh đại diện"
                        onclick="viewImageFull(this)" title="Click để xem ảnh đầy đủ" />
                </div>
                <asp:FileUpload ID="fuAnhDaiDien" runat="server" data-preview="avatar"
                    CssClass="form-control form-control-sm"
                    accept=".jpg,.jpeg,.png,.gif,.webp" />
                <div class="form-text">Tối đa 5 MB · JPG/PNG/GIF/WebP. Để trống = giữ ảnh cũ.</div>
            </div>
            <%-- Ảnh bìa --%>
            <div class="col-md-6">
                <label class="form-label fw-semibold">Ảnh bìa</label>
                <asp:HiddenField ID="hfAnhBia" runat="server" />
                <div class="mb-2">
                    <asp:Image ID="imgPreviewBia" runat="server"
                        CssClass="img-thumbnail"
                        style="max-width:100%;max-height:130px;width:auto;height:auto;object-fit:cover;cursor:pointer;border-radius:8px;"
                        AlternateText="Ảnh bìa"
                        onclick="viewImageFull(this)" title="Click để xem ảnh đầy đủ" />
                </div>
                <asp:FileUpload ID="fuAnhBia" runat="server" CssClass="form-control form-control-sm"
                    accept=".jpg,.jpeg,.png,.gif,.webp"
                    onchange="previewImgById(this, 'imgPreviewBia')" />
                <div class="form-text">Tối đa 10 MB · Khuyến nghị 1200×400 px. Để trống = giữ ảnh cũ.</div>
            </div>
            <div class="col-md-3 d-flex align-items-center pt-3">
                <div class="form-check">
                    <input type="checkbox" id="chkKiemDinh" runat="server" class="form-check-input" />
                    <label class="form-check-label">Đã kiểm định chất lượng</label>
                </div>
            </div>
            <div class="col-12">
                <label class="form-label fw-semibold">Mô tả</label>
                <asp:TextBox ID="txtMoTa" runat="server" CssClass="form-control"
                    TextMode="MultiLine" Rows="6" />
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <asp:Button ID="btnLuu" runat="server" Text="Lưu trường"
                CssClass="btn btn-primary" OnClick="btnLuu_Click" />
            <a href="QuanLyTruong.aspx" class="btn btn-outline-secondary">Hủy</a>
        </div>
    </div>
</div>
</asp:Panel>

<%-- ══════════════════════════════════════════════════════════════
     TAB 2 — ĐIỂM CHUẨN LỊCH SỬ
     ══════════════════════════════════════════════════════════════ --%>
<asp:Panel ID="panelDiemChuan" runat="server" Visible="false">

<%-- Toolbar --%>
<div class="d-flex justify-content-between align-items-center mb-3">
    <asp:Button ID="btnDongBo" runat="server" Text="🔄 Đồng bộ từ Tin tuyển sinh"
        CssClass="btn btn-outline-secondary btn-sm" OnClick="btnDongBo_Click"
        OnClientClick="return confirm('Đồng bộ toàn bộ điểm chuẩn từ Tin tuyển sinh sang bảng lịch sử?');"
        CausesValidation="false" />
            <button type="button" class="btn btn-primary btn-sm"
        data-clear-modal="diemchuan" data-bs-toggle="modal" data-bs-target="#modalDiemChuan">
        <i class="bi bi-plus-lg me-1"></i>Thêm điểm chuẩn
    </button>
</div>

<%-- Filter --%>
<div class="admin-filter-bar mb-3">
    <asp:DropDownList ID="ddlFNganh" runat="server" CssClass="form-select" style="max-width:220px;">
        <asp:ListItem Value="">-- Tất cả ngành --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlFNam" runat="server" CssClass="form-select" style="max-width:130px;">
        <asp:ListItem Value="">-- Tất cả năm --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlFPhuongThuc" runat="server" CssClass="form-select" style="max-width:200px;">
        <asp:ListItem Value="">-- Tất cả phương thức --</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLocDC" runat="server" Text="🔍 Lọc"
        CssClass="btn-filter" OnClick="btnLocDC_Click" CausesValidation="false" />
</div>

<asp:Literal ID="litThongBaoDC" runat="server" />

<div class="admin-card">
    <div class="admin-card-body p-0">
        <asp:GridView ID="gvDiemChuan" runat="server"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="ID"
            OnRowCommand="gvDiemChuan_RowCommand"
            EmptyDataText="Chưa có dữ liệu điểm chuẩn. Bấm 'Đồng bộ' hoặc 'Thêm điểm chuẩn'.">
            <Columns>
                <asp:BoundField DataField="TenChuyenNganh" HeaderText="Ngành học"   HtmlEncode="true" />
                <asp:BoundField DataField="NamTuyenSinh"   HeaderText="Năm" />
                <asp:BoundField DataField="TenPhuongThuc"  HeaderText="Phương thức" HtmlEncode="true" />
                <asp:TemplateField HeaderText="Điểm chuẩn">
                    <ItemTemplate><%# Eval("DiemChuan") != null ? string.Format("{0:F2}", Eval("DiemChuan")) : "—" %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Chỉ tiêu">
                    <ItemTemplate><%# Eval("ChiTieu") != null ? Eval("ChiTieu").ToString() : "—" %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="GhiChu" HeaderText="Ghi chú" HtmlEncode="true" />
                <asp:TemplateField HeaderText="Thao tác">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="SuaDC"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-edit"
                            title="Sửa rạng mục">
                            <i class="bi bi-pencil-square"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="XoaDC"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-delete"
                            OnClientClick="return confirm('Xóa bản ghi điểm chuẩn này?')"
                            title="Xóa">
                            <i class="bi bi-trash"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<%-- Tổng số --%>
<div class="mt-2 text-muted small">
    Tổng: <strong><asp:Literal ID="litTongDC" runat="server" /></strong> bản ghi
</div>

<%-- MODAL THÊM / SỬA ĐIỂM CHUẨN --%>
<asp:HiddenField ID="hfIDDC" runat="server" Value="0" />
<div class="modal fade" id="modalDiemChuan" tabindex="-1" data-bs-backdrop="static">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold">Thêm / Sửa điểm chuẩn</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="row g-3">
                    <div class="col-12">
                        <label class="form-label fw-semibold">Ngành học <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMDCNganh" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-12">
                        <label class="form-label fw-semibold">Phương thức <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMDCPhuongThuc" runat="server" CssClass="form-select" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Năm tuyển sinh <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtMDCNam" runat="server" CssClass="form-control"
                            placeholder="VD: 2024" TextMode="Number" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Điểm chuẩn</label>
                        <asp:TextBox ID="txtMDCDiem" runat="server" CssClass="form-control"
                            placeholder="VD: 27.50" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Chỉ tiêu</label>
                        <asp:TextBox ID="txtMDCChiTieu" runat="server" CssClass="form-control"
                            placeholder="VD: 200" TextMode="Number" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Ghi chú</label>
                        <asp:TextBox ID="txtMDCGhiChu" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnLuuDC" runat="server" Text="Lưu"
                    CssClass="btn btn-primary" OnClick="btnLuuDC_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

</asp:Panel>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script src='<%: ResolveUrl("~/lib/tinymce/tinymce.min.js") %>'></script>
<script src="/Scripts/tinymce-init.js"></script>
<script src="/Scripts/validation_data_input.js"></script>
<script>
// Server flag: báo site.js mở modal điểm chuẩn
window.showDiemChuanModal = '<%= ShowDiemChuanModal %>';

// Reset modal điểm chuẩn khi thêm mới (cần ClientID nên giữ inline)
function clearDiemChuanModal() {
    var hf = document.getElementById('<%= hfIDDC.ClientID %>');
    if (hf) hf.value = '0';
    var ids = [
        '<%= txtMDCNam.ClientID %>',
        '<%= txtMDCDiem.ClientID %>',
        '<%= txtMDCChiTieu.ClientID %>',
        '<%= txtMDCGhiChu.ClientID %>',
        '<%= ddlMDCNganh.ClientID %>',
        '<%= ddlMDCPhuongThuc.ClientID %>'
    ];
    ids.forEach(function(id) {
        var el = document.getElementById(id);
        if (!el) return;
        if (el.tagName === 'SELECT') el.selectedIndex = 0;
        else el.value = '';
    });
}

document.addEventListener('DOMContentLoaded', function () {
    if (typeof CmsEditor !== 'undefined') {
        CmsEditor.init('<%= txtMoTa.ClientID %>');
    }

    // ── AdminValidator — Form thông tin chung ─────────────────────────────
    AdminValidator.init('form', {
        '<%= txtTen.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập tên trường.'),
            AdminValidator.rules.minLen(3, 'Tên trường tối thiểu 3 ký tự.'),
            AdminValidator.rules.maxLen(200, 'Tên trường tối đa 200 ký tự.')
        ],
        '<%= txtSdt.ClientID %>': [
            AdminValidator.rules.phone('Số điện thoại không hợp lệ (10 hoặc 11 chữ số bắt đầu 0).')
        ],
        '<%= txtWebsite.ClientID %>': [
            AdminValidator.rules.url('Website không hợp lệ (cần http:// hoặc https://).')
        ],
        '<%= txtDiaChi.ClientID %>': [
            AdminValidator.rules.maxLen(300, 'Địa chỉ tối đa 300 ký tự.')
        ],
        '<%= ddlTinhThanh.ClientID %>': [
            AdminValidator.rules.required('Vui lòng chọn tỉnh/TP.')
        ]
    }, { triggerButtonId: '<%= btnLuu.ClientID %>' });

    // ── AdminValidator — Modal Điểm chuẩn ────────────────────────────────
    AdminValidator.init('form', {
        '<%= ddlMDCNganh.ClientID %>': [
            AdminValidator.rules.required('Vui lòng chọn ngành học.')
        ],
        '<%= ddlMDCPhuongThuc.ClientID %>': [
            AdminValidator.rules.required('Vui lòng chọn phương thức.')
        ],
        '<%= txtMDCNam.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập năm tuyển sinh.'),
            AdminValidator.rules.numRange(2000, 2100, 'Năm tuyển sinh không hợp lệ.')
        ],
        '<%= txtMDCDiem.ClientID %>': [
            AdminValidator.rules.numRange(0, 40, 'Điểm chuẩn phải từ 0 đến 40.')
        ],
        '<%= txtMDCChiTieu.ClientID %>': [
            AdminValidator.rules.intPositive('Chỉ tiêu phải là số nguyên dương.')
        ]
    }, { triggerButtonId: '<%= btnLuuDC.ClientID %>' });
});
</script>
</asp:Content>
