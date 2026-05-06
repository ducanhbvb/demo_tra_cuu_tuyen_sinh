<%@ Page Title="Quản lý trường" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyTruong.aspx.cs" Inherits="Admin_QuanLyTruong" ValidateRequest="false" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

	<!-- Stats Cards -->
	<div class="row g-3 mb-4">
		<div class="col-md-4">
			<div class="stat-card grad-blue">
				<div class="stat-card-body">
					<div>
						<div class="stat-number"><asp:Literal ID="litTongTruong" runat="server" /></div>
						<div class="stat-label">Tổng số trường</div>
					</div>
					<i class="bi bi-building stat-icon"></i>
				</div>
			</div>
		</div>
		<div class="col-md-4">
			<div class="stat-card grad-green">
				<div class="stat-card-body">
					<div>
						<div class="stat-number"><asp:Literal ID="litCongTru" runat="server" /></div>
						<div class="stat-label">Công lập / Tư thục</div>
					</div>
					<i class="bi bi-pie-chart stat-icon"></i>
				</div>
			</div>
		</div>
		<div class="col-md-4">
			<div class="stat-card grad-amber">
				<div class="stat-card-body">
					<div>
						<div class="stat-number"><asp:Literal ID="litMoiTuan" runat="server" /></div>
						<div class="stat-label">Mới thêm tuần này</div>
					</div>
					<i class="bi bi-calendar-plus stat-icon"></i>
				</div>
			</div>
		</div>
	</div>

	<!-- Page Header -->
	<div class="page-header">
		<div>
			<h1 class="page-header-title">
				<i class="bi bi-building" style="color: var(--accent);"></i> Quản lý Trường đại học
			</h1>
			<p class="page-header-sub">
				<i class="bi bi-house-door"></i> Dashboard / <strong>Quản lý Trường</strong> — Thêm, sửa, quản lý thông tin các trường đại học
			</p>
		</div>
	</div>

	<%-- Filter + Thêm mới --%>
	<div class="admin-filter-bar">
		<div class="search-live" style="flex: 1; max-width: 280px;">
			<i class="bi bi-search"></i>
			<asp:TextBox ID="txtFilter" runat="server" data-filter-input="true" CssClass="form-control" placeholder="Tìm kiếm nhanh..." />
		</div>
		<asp:DropDownList ID="ddlTinh" runat="server" data-filter-tinh="true" CssClass="form-select" style="max-width: 180px;">
			<asp:ListItem Value="">-- Tỉnh/TP --</asp:ListItem>
		</asp:DropDownList>
		<asp:DropDownList ID="ddlTrangThai" runat="server" data-filter-trangthai="true" CssClass="form-select" style="max-width: 160px;">
			<asp:ListItem Value="">-- Trạng thái --</asp:ListItem>
			<asp:ListItem Value="1">Hiển thị</asp:ListItem>
			<asp:ListItem Value="0">Đã ẩn</asp:ListItem>
		</asp:DropDownList>
		<asp:Button ID="btnLoc" runat="server" Text="Lọc" CssClass="btn-filter" OnClick="btnLoc_Click" />
		<button class="btn-filter" style="border-color: #94a3b8; color: #94a3b8;" title="Xoá bộ lọc" data-clear-filter="true">
			<i class="bi bi-x-lg"></i>
		</button>
		<asp:Button ID="btnXoaLoc" runat="server" data-clear-filter-trigger="true" Text="Xoá bộ lọc" Style="display:none" OnClick="btnXoaLoc_Click" />
		<button type="button" class="btn-add-new" data-clear-modal="true" data-bs-toggle="modal" data-bs-target="#modalTruong">
			<i class="bi bi-plus-lg"></i> Thêm trường mới
		</button>
	</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="admin-card">
	<div class="admin-card-body p-0">
		<asp:GridView ID="gvTruong" runat="server"
			EnableViewState="false"
			CssClass="table table-hover table-sm align-middle mb-0"
			AutoGenerateColumns="false" GridLines="None"
			DataKeyNames="MaTruong"
			OnRowCommand="gvTruong_RowCommand"
			OnRowDataBound="gvTruong_RowDataBound"
			UseAccessibleHeader="true"
			EmptyDataText="Chưa có dữ liệu.">
			<Columns>
				<asp:TemplateField HeaderText="#" HeaderStyle-Width="50px">
					<ItemTemplate>
						<%# Container.DataItemIndex + 1 + CurrentPage * 15 %>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="TRƯỜNG">
					<ItemTemplate>
						<div class="truong-cell">
							<div class="truong-avatar">
								<i class="bi bi-building"></i>
							</div>
							<div>
								<div class="truong-name"><%# Eval("TenTruong") %></div>
								<div class="truong-url"><%# GetSafeWebsite(Eval("Website")) %></div>
							</div>
						</div>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:BoundField DataField="TinhThanh" HeaderText="Tỉnh/TP" HtmlEncode="true" />
				<asp:BoundField DataField="QuyMo" HeaderText="Quy mô" HtmlEncode="true" />
				<asp:TemplateField HeaderText="Kiểm định">
					<ItemTemplate>
						<%# GetKiemDinhBadge(Eval("KiemDinhChatLuong")) %>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Trạng thái">
					<ItemTemplate>
						<%# Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))
							? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hiển thị</span>"
							: "<span class='badge badge-soft-secondary'><i class='bi bi-eye-slash me-1'></i>Đã ẩn</span>" %>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:TemplateField HeaderText="Thao tác" HeaderStyle-Width="180px" ItemStyle-CssClass="action-cell">
					<ItemTemplate>
						<asp:LinkButton runat="server" CommandName="SuaTruong"
							CommandArgument='<%# Eval("MaTruong") %>'
							CssClass="btn-action btn-edit" title="Chỉnh sửa">
							<i class="bi bi-pencil-square"></i>
						</asp:LinkButton>
						<a href='<%# "ChinhSuaTruong.aspx?id=" + Eval("MaTruong") %>'
						   class="btn-action btn-chart" title="Xem biểu đồ điểm chuẩn">
							<i class="bi bi-bar-chart-line"></i>
						</a>
						<asp:LinkButton runat="server" CommandName="ToggleTruong"
							CommandArgument='<%# Eval("MaTruong") %>'
							CssClass='<%# (Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))) ? "btn-action btn-toggle" : "btn-action btn-toggle text-success" %>'
							OnClientClick="return confirm('Thay đổi trạng thái hiển thị trường này?')"
							title="Ẩn / Hiện">
							<%# (Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))) ? "<i class='bi bi-eye-slash'></i>" : "<i class='bi bi-eye'></i>" %>
						</asp:LinkButton>
						<asp:LinkButton runat="server" CommandName="KhoiPhucTruong"
							CommandArgument='<%# Eval("MaTruong") %>'
							CssClass="btn-action btn-edit text-success"
							Visible='<%# Eval("TrangThai") != DBNull.Value && !Convert.ToBoolean(Eval("TrangThai")) %>'
							OnClientClick="return confirm('Khôi phục 1 bản ghi trường này và mở lại tài khoản trường liên quan?')"
							title="Khôi phục trường">
							<i class="bi bi-arrow-counterclockwise"></i>
						</asp:LinkButton>
						<asp:LinkButton runat="server" CommandName="XoaTruong"
							CommandArgument='<%# Eval("MaTruong") %>'
							CssClass="btn-action btn-delete"
							Visible='<%# Eval("TrangThai") == DBNull.Value || Convert.ToBoolean(Eval("TrangThai")) %>'
							OnClientClick="return confirm('Xóa tạm 1 bản ghi trường này khỏi danh sách hiển thị và khóa tài khoản trường liên quan? Có thể khôi phục sau.')"
							title="Xóa tạm trường">
							<i class="bi bi-trash3"></i>
						</asp:LinkButton>
						<asp:LinkButton runat="server" CommandName="XoaVinhVienTruong"
							CommandArgument='<%# Eval("MaTruong") %>'
							CssClass="btn-action btn-delete"
							Visible='<%# Eval("TrangThai") != DBNull.Value && !Convert.ToBoolean(Eval("TrangThai")) %>'
							OnClientClick="return confirm('CẢNH BÁO LẦN 1: Bạn đang yêu cầu xóa vĩnh viễn đúng 1 bản ghi trường. Sau khi bấm OK, server sẽ kiểm tra dữ liệu phụ thuộc rồi hiển thị cảnh báo lần 2. Tiếp tục?')"
							title="Xóa vĩnh viễn trường">
							<i class="bi bi-exclamation-triangle-fill"></i>
						</asp:LinkButton>
					</ItemTemplate>
				</asp:TemplateField>
			</Columns>
		</asp:GridView>
	</div>
</div>

<%-- Phân trang --%>
<div class="d-flex justify-content-between align-items-center mt-3">
    <span class="text-muted small">
        Tổng: <strong><asp:Literal ID="litTong" runat="server" /></strong> trường
    </span>
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

<%-- MODAL THÊM TRƯỜNG MỚI --%>
<input type="hidden" id="hfMaTruong" runat="server" value="0" />
<div class="modal fade" id="modalTruong" tabindex="-1" data-bs-backdrop="static">
    <div class="modal-dialog modal-xl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold"><asp:Literal ID="litModalTitle" runat="server" Text="Thêm trường mới" /></h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="row g-3">
                    <div class="col-md-8">
                        <label class="form-label fw-semibold">Tên trường <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtMTen" runat="server" CssClass="form-control"
                            placeholder="VD: Đại học Bách khoa Hà Nội" MaxLength="200" data-field="ten" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label fw-semibold">Slug (SEO)</label>
                        <div class="slug-copy-wrap">
                            <asp:TextBox ID="txtMSlug" runat="server" CssClass="form-control text-muted"
                                placeholder="tu-dong-tao-neu-de-trong" />
                            <button type="button" class="btn-copy-slug" title="Sao chép slug"
                                data-source="<%= txtMSlug.ClientID %>">
                                <i class="bi bi-clipboard"></i>
                            </button>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Địa chỉ</label>
                        <asp:TextBox ID="txtMDiaChi" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Tỉnh / Thành phố</label>
                        <asp:DropDownList ID="ddlMTinhThanh" runat="server" CssClass="form-select" data-preview-source="school-city">
                            <asp:ListItem Value="">-- Chọn tỉnh/TP --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Khu vực</label>
                        <asp:DropDownList ID="ddlMVung" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                            <asp:ListItem Value="1">Miền Bắc</asp:ListItem>
                            <asp:ListItem Value="2">Miền Trung</asp:ListItem>
                            <asp:ListItem Value="3">Miền Nam</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Loại trường</label>
                        <asp:DropDownList ID="ddlMLoai" runat="server" CssClass="form-select" data-preview-source="school-type">
                            <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                            <asp:ListItem Value="1">Công lập</asp:ListItem>
                            <asp:ListItem Value="2">Tư thục</asp:ListItem>
                            <asp:ListItem Value="3">Quốc tế</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Cấp bậc đào tạo</label>
                        <asp:DropDownList ID="ddlMCapBacDaoTao" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                            <asp:ListItem Value="1">Đại học</asp:ListItem>
                            <asp:ListItem Value="2">Cao Đẳng</asp:ListItem>
                            <asp:ListItem Value="3">Trường nghề</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Điện thoại</label>
                        <asp:TextBox ID="txtMSdt" runat="server" CssClass="form-control"
                            TextMode="Phone" MaxLength="20" placeholder="0xxxxxxxxx" data-preview-source="school-phone" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Website</label>
                        <asp:TextBox ID="txtMWebsite" runat="server" CssClass="form-control"
                            TextMode="SingleLine" MaxLength="200" placeholder="https://..." data-preview-source="school-website" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-semibold">Quy mô</label>
                        <asp:TextBox ID="txtMQuyMo" runat="server" CssClass="form-control"
                            placeholder="VD: Hơn 30.000 sinh viên" />
                    </div>
                    <div class="w-100"></div>
                    <%-- Ảnh đại diện --%>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Ảnh đại diện</label>
                        <input type="hidden" id="hfMAnhDaiDien" runat="server" />
                        <div class="admin-image-upload admin-image-upload--avatar" data-upload-card>
                            <div class="admin-image-upload__preview" data-preview-wrap="avatar" style="display:none;">
                                <asp:Image ID="imgPreviewMAvatar" runat="server" data-preview-target="avatar"
                                    CssClass="admin-image-upload__thumb" />
                            </div>
                            <div class="admin-image-upload__control">
                                <asp:FileUpload ID="fuMAnhDaiDien" runat="server" data-preview="avatar"
                                    CssClass="form-control form-control-sm"
                                    accept=".jpg,.jpeg,.png,.gif,.webp" />
                                <div class="form-text">Tối đa 5 MB · JPG/PNG/GIF/WebP.</div>
                                <div class="admin-image-upload__actions">
                                    <button type="button" class="btn btn-outline-danger admin-image-upload__clear" data-clear-upload>
                                        <i class="bi bi-x-circle me-1"></i>Hủy ảnh đã chọn
                                    </button>
                                    <span class="admin-image-upload__filename" data-upload-file-name>Chưa chọn ảnh</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%-- Ảnh bìa --%>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Ảnh bìa</label>
                        <input type="hidden" id="hfMAnhBia" runat="server" />
                        <div class="admin-image-upload admin-image-upload--cover" data-upload-card>
                            <div class="admin-image-upload__preview" data-preview-wrap="bia" style="display:none;">
                                <asp:Image ID="imgPreviewMBia" runat="server" data-preview-target="bia"
                                    CssClass="admin-image-upload__thumb" />
                            </div>
                            <div class="admin-image-upload__control">
                                <asp:FileUpload ID="fuMAnhBia" runat="server" data-preview="bia"
                                    CssClass="form-control form-control-sm"
                                    accept=".jpg,.jpeg,.png,.gif,.webp" />
                                <div class="form-text">Tối đa 10 MB · Khuyến nghị 1200×400 px.</div>
                                <div class="admin-image-upload__actions">
                                    <button type="button" class="btn btn-outline-danger admin-image-upload__clear" data-clear-upload>
                                        <i class="bi bi-x-circle me-1"></i>Hủy ảnh đã chọn
                                    </button>
                                    <span class="admin-image-upload__filename" data-upload-file-name>Chưa chọn ảnh</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="w-100"></div>
                    <div class="col-12 d-flex align-items-center pt-1">
                        <div class="form-check">
                            <input type="checkbox" id="chkMKiemDinh" runat="server" class="form-check-input" />
                            <label class="form-check-label">Đã kiểm định chất lượng</label>
                        </div>
                    </div>
                    <div class="col-12">
                        <label class="form-label fw-semibold">Mô tả</label>
                        <asp:TextBox ID="txtMMoTa" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="5"
                            placeholder="Giới thiệu về trường..." data-preview-source="school-desc" />
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Hủy</button>
                <button type="button" class="btn btn-outline-info" data-admin-preview="school">
                    <i class="bi bi-eye me-1"></i>Preview
                </button>
                <asp:Button ID="btnLuuTruong" runat="server" Text="Lưu trường"
                    CssClass="btn btn-primary" OnClick="btnLuuTruong_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

<%-- MODAL PREVIEW TRƯỜNG --%>
<div class="modal fade" id="modalPreviewTruong" tabindex="-1">
    <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold"><i class="bi bi-eye me-2 text-info"></i>Preview hồ sơ trường</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="admin-preview-hero mb-3">
                    <img id="pvSchoolCover" src="/Resources/Images/no-image.png" alt="Preview ảnh bìa" />
                </div>
                <div class="d-flex align-items-end gap-3 mb-3" style="margin-top:-58px;padding-left:18px;">
                    <img id="pvSchoolLogo" class="admin-preview-logo" src="/Resources/Images/no-image.png" alt="Preview logo" />
                    <div class="pb-2"><span id="pvSchoolType" class="admin-preview-chip">Loại trường</span></div>
                </div>
                <div class="admin-preview-info-card">
                    <h4 id="pvSchoolName" class="fw-bold mb-1">Tên trường</h4>
                    <div class="text-muted mb-3"><span id="pvSchoolCode">Slug</span> · <span id="pvSchoolCity">Tỉnh/TP</span></div>
                    <p id="pvSchoolDesc" class="mb-3">Mô tả trường.</p>
                    <div class="row g-2 small">
                        <div class="col-md-6"><i class="bi bi-globe2 me-1 text-primary"></i><span id="pvSchoolWebsite">Website</span></div>
                        <div class="col-md-6"><i class="bi bi-telephone me-1 text-primary"></i><span id="pvSchoolPhone">Điện thoại</span></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<%-- MODAL XÁC NHẬN XÓA VĨNH VIỄN TRƯỜNG --%>
<input type="hidden" id="hfXoaVinhVienMaTruong" runat="server" value="" />
<input type="hidden" id="hfCanOpenDeletePasswordModal" runat="server" value="" />
<div class="modal fade" id="modalXoaVinhVienTruong" tabindex="-1" data-bs-backdrop="static">
    <div class="modal-dialog modal-lg">
        <div class="modal-content border-danger">
            <div class="modal-header bg-danger text-white">
                <h5 class="modal-title fw-bold"><i class="bi bi-exclamation-triangle-fill me-2"></i>Xác nhận xóa vĩnh viễn trường</h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="alert alert-danger">
                    <div class="fw-bold mb-1">Thao tác nguy hiểm, không thể khôi phục bằng nút Khôi phục.</div>
                    Hệ thống sẽ hard-delete trường, dữ liệu liên quan và ảnh local trong Resources sau khi server xác thực mật khẩu admin hiện tại.
                </div>
                <p class="mb-2">Trường cần xóa vĩnh viễn: <strong><asp:Literal ID="litXoaVinhVienTenTruong" runat="server" /></strong></p>
                <ul class="small text-muted">
                    <li>Xóa dây chuyền bài viết, tin tuyển sinh, ngành/chương trình, đợt/điểm chuẩn, đánh giá, góp ý, tư vấn, wishlist và lịch sử tìm kiếm của đúng trường này.</li>
                    <li>Tài khoản liên kết trường được gỡ liên kết, khóa lại để giữ audit; ảnh logo/cover và ảnh nội dung local được dọn khỏi Resources.</li>
                </ul>
                <label class="form-label fw-semibold">Nhập mật khẩu admin hiện tại <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtXoaVinhVienMatKhau" runat="server" CssClass="form-control" TextMode="Password" autocomplete="current-password" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnXacNhanXoaVinhVien" runat="server"
                    Text="Xác nhận xóa vĩnh viễn"
                    CssClass="btn btn-danger"
                    OnClick="btnXacNhanXoaVinhVien_Click"
                    OnClientClick="return confirm('Bạn đã nhập mật khẩu admin và chắc chắn muốn bắt đầu xóa vĩnh viễn đúng 1 bản ghi trường này? Nếu chọn Không, hệ thống sẽ hủy thao tác và không xóa dữ liệu.');"
                    CausesValidation="false" />
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
	    window.showModalOnLoad = '<%= ShowModal %>';
	    window.showDeleteModalOnLoad = '<%= ShowDeleteModal %>';
	    window.showDeleteSecondWarningOnLoad = '<%= ShowDeleteSecondWarning %>';
	    window.deleteSecondWarningMessage = '<%= HttpUtility.JavaScriptStringEncode(DeleteSecondWarningMessage) %>';
        document.addEventListener('DOMContentLoaded', function() {
            if (typeof CmsEditor !== 'undefined') {
                CmsEditor.init('<%= txtMMoTa.ClientID %>');
            }

            // ── AdminValidator — modal Trường ──────────────────────────────
            AdminValidator.init('form', {
                '<%= txtMTen.ClientID %>': [
                    AdminValidator.rules.required('Vui lòng nhập tên trường.'),
                    AdminValidator.rules.minLen(3, 'Tên trường tối thiểu 3 ký tự.'),
                    AdminValidator.rules.maxLen(200, 'Tên trường tối đa 200 ký tự.')
                ],
                '<%= txtMSdt.ClientID %>': [
                    AdminValidator.rules.phone('Số điện thoại không hợp lệ (10 hoặc 11 chữ số bắt đầu 0).')
                ],
                '<%= txtMWebsite.ClientID %>': [
                    AdminValidator.rules.url('Website không hợp lệ (cần bắt đầu bằng http:// hoặc https://).')
                ],
                '<%= txtMDiaChi.ClientID %>': [
                    AdminValidator.rules.maxLen(300, 'Địa chỉ tối đa 300 ký tự.')
                ],
                '<%= ddlMTinhThanh.ClientID %>': [
                    AdminValidator.rules.required('Vui lòng chọn tỉnh/TP.')
                ]
            }, { triggerButtonId: '<%= btnLuuTruong.ClientID %>' });
            var openDeletePasswordModal = function () {
                var deleteModalEl = document.getElementById('modalXoaVinhVienTruong');
                if (deleteModalEl && typeof bootstrap !== 'undefined') {
                    new bootstrap.Modal(deleteModalEl).show();
                }
            };

            if (window.showDeleteSecondWarningOnLoad === 'true') {
                if (confirm(window.deleteSecondWarningMessage || 'CẢNH BÁO LẦN 2: Thao tác xóa vĩnh viễn không thể hoàn tác. Tiếp tục đến bước nhập mật khẩu?')) {
                    openDeletePasswordModal();
                }
            } else if (window.showDeleteModalOnLoad === 'true') {
                openDeletePasswordModal();
            }
        });
 </script>
</asp:Content>
