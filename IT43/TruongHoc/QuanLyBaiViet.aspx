<%@ Page Title="Quản lý bài viết" Language="C#" MasterPageFile="~/TruongHoc/TruongHoc.Master"
   CodeBehind="QuanLyBaiViet.aspx.cs" Inherits="TruongHoc_QuanLyBaiViet"
   ValidateRequest="false" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<!-- Page Header -->
<div class="page-header mb-4">
    <div>
        <h4 class="page-header-title">
            <i class="bi bi-newspaper" style="color: var(--accent);"></i> Bài viết của trường
        </h4>
        <div class="page-header-sub">
            Cổng Trường / <strong>Bài viết</strong> — Quản lý tin tức và bài viết của trường bạn
        </div>
    </div>
    <button type="button" class="btn btn-primary" data-clear-modal="true"
            data-bs-toggle="modal" data-bs-target="#modalBaiViet">
        <i class="bi bi-plus-lg me-1"></i>Thêm mới
    </button>
</div>

<%-- Thống kê --%>
<div class="row g-3 mb-4">
    <div class="col-md-4">
        <div class="stat-card grad-purple">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litTongBV" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Tổng bài viết</div>
                </div>
                <i class="bi bi-journal-text stat-icon"></i>
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
    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-select" style="max-width:160px;">
        <asp:ListItem Value="">-- Trạng thái --</asp:ListItem>
        <asp:ListItem Value="1">Hiển thị</asp:ListItem>
        <asp:ListItem Value="0">Đã ẩn</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc" CssClass="btn-filter" OnClick="btnLoc_Click" />
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="admin-card">
    <div class="admin-card-body p-0">
        <asp:GridView ID="gvBaiViet" runat="server"
            EnableViewState="false"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaBaiViet" OnRowCommand="gvBaiViet_RowCommand"
            UseAccessibleHeader="true"
            EmptyDataText="Chưa có bài viết nào.">
            <Columns>
                <asp:BoundField DataField="TieuDe"   HeaderText="Tiêu đề"   HtmlEncode="true" ItemStyle-Width="350px" />
                <asp:BoundField DataField="NgayDang" HeaderText="Ngày đăng" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="LuotXem"  HeaderText="Lượt xem" />
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
                            CommandArgument='<%# Eval("MaBaiViet") %>'
                            CssClass='<%# (bool)Eval("TrangThai") ? "btn-action btn-toggle" : "btn-action btn-toggle text-success" %>'
                            OnClientClick="return confirm('Thay đổi trạng thái hiển thị của bài viết này?')" title="Ẩn / Hiện">
                            <%# (bool)Eval("TrangThai") ? "<i class='bi bi-eye-slash'></i>" : "<i class='bi bi-eye'></i>" %>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Sua"
                            CommandArgument='<%# Eval("MaBaiViet") %>'
                            CssClass="btn-action btn-edit" title="Sửa bài viết">
                            <i class="bi bi-pencil-square"></i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="Xoa"
                            CommandArgument='<%# Eval("MaBaiViet") %>'
                            CssClass="btn-action btn-delete"
                            OnClientClick="return confirm('CẢNH BÁO: Đây là thao tác XÓA VĨNH VIỄN bài viết này. Bạn đã chắc chắn muốn tiếp tục?')" title="Xóa vĩnh viễn bài viết">
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
<input type="hidden" id="hfMaBaiViet" runat="server" value="0" />
<input type="hidden" id="hfNoiDung" runat="server" data-field="noiDung" />
<div class="modal fade" id="modalBaiViet" tabindex="-1" data-bs-backdrop="static" data-bs-focus="false">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold">Thêm / Sửa bài viết</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="row g-3">
                    <div class="col-md-8">
                        <label class="form-label">Tiêu đề <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtTieuDe" runat="server" CssClass="form-control"
                            placeholder="Tiêu đề bài viết..." oninput="autoGenerateSlug(this)" data-field="tieude"
                            MaxLength="200" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Slug (tự sinh)</label>
                        <div class="slug-copy-wrap">
                            <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control text-muted"
                                placeholder="tu-dong-sinh-tu-tieu-de" data-field="slug" />
                            <button type="button" class="btn-copy-slug" title="Sao chép slug"
                                data-source="<%= txtSlug.ClientID %>">
                                <i class="bi bi-clipboard"></i>
                            </button>
                        </div>
                    </div>
                    <div class="col-md-12">
                        <label class="form-label">Thể loại (Category / Tag)</label>
                        <asp:TextBox ID="txtTheLoai" runat="server" CssClass="form-control"
                            placeholder="VD: Tuyển sinh, Hoạt động, Dành cho tân sinh viên..."
                            list="dlTheLoai" />
                        <datalist id="dlTheLoai">
                            <option value="Tư vấn ngành"></option>
                            <option value="Học bổng"></option>
                            <option value="Sự kiện"></option>
                            <option value="Chính sách mới"></option>
                            <option value="Tuyển sinh"></option>
                        </datalist>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Ảnh bìa</label>
                        <input type="hidden" id="hfAnhChinh" runat="server" />
                        <div class="admin-image-upload admin-image-upload--article" data-upload-card>
                            <div class="admin-image-upload__preview" data-preview-wrap="anhbia" style="display:none;">
                                <asp:Image ID="imgPreviewBV" runat="server" data-preview-target="anhbia"
                                    CssClass="admin-image-upload__thumb" title="Click để xem ảnh đầy đủ" />
                            </div>
                            <div class="admin-image-upload__control">
                                <asp:FileUpload ID="fuAnhChinh" runat="server" data-preview="anhbia"
                                    CssClass="form-control form-control-sm"
                                    accept=".jpg,.jpeg,.png,.gif,.webp" />
                                <div class="form-text">Tối đa 5 MB · Để trống = giữ ảnh cũ.</div>
                                <div class="admin-image-upload__actions">
                                    <button type="button" class="btn btn-outline-danger admin-image-upload__clear" data-clear-upload>
                                        <i class="bi bi-x-circle me-1"></i>Hủy ảnh đã chọn
                                    </button>
                                    <span class="admin-image-upload__filename" data-upload-file-name>Chưa chọn ảnh</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label fw-semibold">Nội dung <span class="text-danger">*</span></label>
                        <%-- TinyMCE Editor --%>
                        <textarea id="txtTinyMCE" name="txtTinyMCE"></textarea>
                    </div>
                </div>
                <div class="form-check mt-3">
                    <asp:CheckBox ID="chkTrangThai" runat="server"
                        CssClass="form-check-input" Checked="true" />
                    <label class="form-check-label">Hiển thị ngay</label>
                </div>
                <asp:Literal ID="litModalLoi" runat="server" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Hủy</button>
                <button type="button" class="btn btn-outline-warning" onclick="resetModalBaiViet()" title="Xóa trắng toàn bộ dữ liệu đã nhập">
                    <i class="bi bi-arrow-counterclockwise me-1"></i>Nhập lại
                </button>
                <button type="button" class="btn btn-outline-info" data-admin-preview="article">
                    <i class="bi bi-eye me-1"></i>Preview
                </button>
                <asp:Button ID="btnLuu" runat="server" Text="Lưu bài viết"
                    CssClass="btn btn-primary" OnClick="btnLuu_Click" CausesValidation="false"
                    OnClientClick="return syncTinyMCEBeforeSubmit();" />
            </div>
        </div>
    </div>
</div>

<%-- ═══ MODAL XEM TRƯỚC BÀI VIẾT ═══ --%>
<div class="modal fade" id="modalPreviewBV" tabindex="-1">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><i class="bi bi-eye me-2"></i>Xem trước bài viết</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <img id="pvPostImg" class="admin-article-preview-cover mb-4" src="/Resources/Images/no-image.png" alt="Preview ảnh bìa" />
                <div class="admin-article-preview-content">
                    <div class="d-flex gap-3 text-muted small mb-3">
                        <span><i class="bi bi-calendar3 me-1"></i><span id="pvPostDate"></span></span>
                        <span class="badge bg-secondary">Xem trước</span>
                    </div>
                    <h2 class="fw-bold mb-4" id="pvPostTitle"></h2>
                    <div class="article-content" id="pvPostContent"></div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                    <i class="bi bi-arrow-left me-1"></i>Quay lại sửa
                </button>
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
    <%-- Load TinyMCE and init script --%>
    <script src="<%: ResolveUrl("~/lib/tinymce/tinymce.min.js") %>"></script>
    <script src="/Scripts/tinymce-init.js"></script>
    <%-- Validate 3 tầng: tầng 2 JS --%>
    <script src="/Scripts/validation_data_input.js"></script>
    <script>
        window.showBaiVietModal = '<%= ShowModal %>';

        /** Reset toàn bộ các ô nhập trong modal Bài viết về trạng thái trống */
        function resetModalBaiViet() {
            if (!confirm('Bạn có chắc muốn xóa trắng toàn bộ dữ liệu đã nhập?')) return;
            document.getElementById('<%= hfMaBaiViet.ClientID %>').value = '0';
            ['<%= txtTieuDe.ClientID %>', '<%= txtSlug.ClientID %>', '<%= txtTheLoai.ClientID %>'].forEach(function(id) {
                var el = document.getElementById(id);
                if (el) el.value = '';
            });
            // Reset TinyMCE
            if (typeof tinymce !== 'undefined' && tinymce.get('txtTinyMCE')) {
                tinymce.get('txtTinyMCE').setContent('');
            }
            // Checkbox hiển thị → mặc định checked
            var chk = document.getElementById('<%= chkTrangThai.ClientID %>');
            if (chk) chk.checked = true;
            // Reset ảnh preview
            var previewWrap = document.querySelector('#modalBaiViet [data-preview-wrap="anhbia"]');
            if (previewWrap) previewWrap.style.display = 'none';
            var fuInput = document.getElementById('<%= fuAnhChinh.ClientID %>');
            if (fuInput) fuInput.value = '';
            var hfAnh = document.getElementById('<%= hfAnhChinh.ClientID %>');
            if (hfAnh) hfAnh.value = '';
            var fileName = document.querySelector('#modalBaiViet [data-upload-file-name]');
            if (fileName) fileName.textContent = 'Chưa chọn ảnh';
            // Xóa trạng thái validate cũ
            document.querySelectorAll('#modalBaiViet .is-invalid, #modalBaiViet .is-valid').forEach(function(el) {
                el.classList.remove('is-invalid', 'is-valid');
            });
            document.querySelectorAll('#modalBaiViet .invalid-feedback').forEach(function(fb) {
                fb.textContent = '';
            });
        }

        function syncTinyMCEBeforeSubmit() {
            return CmsEditor.syncToHiddenField('txtTinyMCE', '<%= hfNoiDung.ClientID %>');
        }

        document.addEventListener('DOMContentLoaded', function() {
            if (typeof CmsEditor !== 'undefined') CmsEditor.init('txtTinyMCE');

            // ── AdminValidator — modal Bài viết ─────────────────────────
            if (window.AdminValidator) {
                AdminValidator.init('form', {
                    '<%= txtTieuDe.ClientID %>': [
                        AdminValidator.rules.required('Vui lòng nhập tiêu đề bài viết.'),
                        AdminValidator.rules.minLen(5, 'Tiêu đề tối thiểu 5 ký tự.'),
                        AdminValidator.rules.maxLen(200, 'Tiêu đề tối đa 200 ký tự.')
                    ],
                    '<%= txtTheLoai.ClientID %>': [
                        AdminValidator.rules.maxLen(100, 'Thể loại tối đa 100 ký tự.')
                    ]
                }, { triggerButtonId: '<%= btnLuu.ClientID %>' });
            }

            // Mở modal sau postback nếu server set ShowModal
            if (window.showBaiVietModal === 'true') {
                var modalEl = document.getElementById('modalBaiViet');
                if (modalEl && typeof bootstrap !== 'undefined') {
                    var bsModal = bootstrap.Modal.getOrCreateInstance(modalEl);
                    bsModal.show();
                    // Load nội dung TinyMCE
                    var hfNd = document.getElementById('<%= hfNoiDung.ClientID %>');
                    if (hfNd && hfNd.value) {
                        setTimeout(function() {
                            CmsEditor.setContent('txtTinyMCE', hfNd.value);
                        }, 500);
                    }
                    // Hiện ảnh cũ nếu có
                    var imgEl = document.getElementById('<%= imgPreviewBV.ClientID %>');
                    if (imgEl && imgEl.src && !imgEl.src.endsWith('/')) {
                        var pw = document.querySelector('#modalBaiViet [data-preview-wrap="anhbia"]');
                        if (pw) pw.style.display = '';
                    }
                }
            }
        });
    </script>
</asp:Content>
