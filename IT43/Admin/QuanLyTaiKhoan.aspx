<%@ Page Title="Quản lý tài khoản" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyTaiKhoan.aspx.cs" Inherits="Admin_QuanLyTaiKhoan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex align-items-center justify-content-between mb-3">
    <h5 class="fw-bold mb-0">Quản lý tài khoản</h5>
    <asp:Panel ID="pnlBtnTaoTK" runat="server">
        <button type="button" class="btn btn-primary btn-sm"
                data-bs-toggle="modal" data-bs-target="#modalTaoTK">
            <i class="bi bi-person-plus me-1"></i>Tạo tài khoản
        </button>
    </asp:Panel>
</div>

<%-- Thống kê --%>
<div class="row g-3 mb-4">
    <div class="col-md-3">
        <div class="stat-card grad-purple">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litTongTK" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Tổng tài khoản</div>
                </div>
                <i class="bi bi-people stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="stat-card" style="background: linear-gradient(135deg, #8b5cf6, #7c3aed); color: white;">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litPhanQuyen" runat="server">0 / 0 / 0</asp:Literal></div>
                    <div class="stat-label">Admin / Trường / User</div>
                </div>
                <i class="bi bi-shield-check stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="stat-card grad-amber">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litChuaXN" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Chưa xác nhận email</div>
                </div>
                <i class="bi bi-envelope-exclamation stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-md-3">
        <div class="stat-card grad-green">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litHoatDong" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Đang hoạt động</div>
                </div>
                <i class="bi bi-person-check stat-icon"></i>
            </div>
        </div>
    </div>
</div>

<div class="admin-filter-bar mb-4">
    <div class="search-live" style="flex: 1; max-width: 280px;">
        <i class="bi bi-search"></i>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Tìm email..."
            autocomplete="off" TextMode="Search" />
    </div>
    <asp:DropDownList ID="ddlQuyen" runat="server" CssClass="form-select" style="max-width:160px;">
        <asp:ListItem Value="">-- Quyền --</asp:ListItem>
        <asp:ListItem Value="1">Admin</asp:ListItem>
        <asp:ListItem Value="2">Trường học</asp:ListItem>
        <asp:ListItem Value="3">Học sinh</asp:ListItem>
        <asp:ListItem Value="4">Moderator</asp:ListItem>
        <asp:ListItem Value="5">Tư vấn viên</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc" CssClass="btn-filter" OnClick="btnLoc_Click" />
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="admin-card">
    <div class="admin-card-body p-0">
        <asp:GridView ID="gvTK" runat="server"
            EnableViewState="false"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaTaiKhoan" OnRowCommand="gvTK_RowCommand"
            UseAccessibleHeader="true"
            EmptyDataText="Không có kết quả.">
            <Columns>
                <%-- Email + badge "(Bạn)" nếu là tài khoản đang đăng nhập --%>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# Server.HtmlEncode(Eval("Email").ToString()) %>
                        <%# (int)Eval("MaTaiKhoan") == SecurityHelper.GetCurrentMaTaiKhoan()
                            ? "<span class='badge bg-info ms-1' title='Đây là tài khoản của bạn'>Bạn</span>"
                            : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TenQuyen"  HeaderText="Quyền"    HtmlEncode="true" />
                <asp:BoundField DataField="TenTruong" HeaderText="Trường"   HtmlEncode="true"
                    HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile text-muted small" />
                <asp:BoundField DataField="NgayTao"   HeaderText="Ngày tạo" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:TemplateField HeaderText="Xác nhận email">
                    <ItemTemplate>
                        <%# (bool)Eval("EmailDaXacNhan")
                            ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Đã XN</span>"
                            : "<span class='badge badge-soft-warning'><i class='bi bi-exclamation-triangle me-1'></i>Chưa XN</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# (bool)Eval("TrangThai")
                            ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hoạt động</span>"
                            : "<span class='badge badge-soft-danger'><i class='bi bi-ban me-1'></i>Vô hiệu</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Thao tác">
                    <ItemTemplate>
                        <%-- Toggle kích hoạt/khóa — ẩn nếu là tài khoản đang đăng nhập --%>
                        <asp:LinkButton runat="server"
                            CommandName="ToggleTT"
                            CommandArgument='<%# Eval("MaTaiKhoan") + "," + Eval("TrangThai") %>'
                            CssClass='<%# (bool)Eval("TrangThai") ? "btn-action btn-toggle" : "btn-action btn-toggle text-success" %>'
                            Visible='<%# (int)Eval("MaTaiKhoan") != SecurityHelper.GetCurrentMaTaiKhoan() %>'
                            OnClientClick="return confirm('Thay đổi trạng thái kích hoạt tài khoản này?')"
                            title="Kích hoạt / Vô hiệu">
                            <%# (bool)Eval("TrangThai") ? "<i class='bi bi-lock'></i>" : "<i class='bi bi-unlock'></i>" %>
                        </asp:LinkButton>
                        <%-- Sửa quyền — ẩn nếu là tài khoản đang đăng nhập --%>
                        <asp:LinkButton runat="server"
                            CommandName="SuaQuyen"
                            CommandArgument='<%# Eval("MaTaiKhoan") + "|" + Eval("Email") + "|" + Eval("MaQuyen") %>'
                            CssClass="btn-action btn-edit"
                            Visible='<%# (int)Eval("MaTaiKhoan") != SecurityHelper.GetCurrentMaTaiKhoan() %>'
                            title="Sửa quyền">
                            <i class="bi bi-shield-lock"></i>
                        </asp:LinkButton>
                        <%-- Gửi email reset mật khẩu (Cấp 2) --%>
                        <asp:LinkButton runat="server"
                            CommandName="GuiEmailReset"
                            CommandArgument='<%# Eval("MaTaiKhoan") + "|" + Eval("Email") %>'
                            CssClass="btn-action btn-chart"
                            title="Gửi email reset mật khẩu">
                            <i class="bi bi-envelope-arrow-up"></i>
                        </asp:LinkButton>
                        <%-- Đặt mật khẩu trực tiếp (Cấp 3) --%>
                        <asp:LinkButton runat="server"
                            CommandName="ResetMK"
                            CommandArgument='<%# Eval("MaTaiKhoan") + "|" + Eval("Email") %>'
                            CssClass="btn-action btn-delete"
                            title="Đặt mật khẩu trực tiếp">
                            <i class="bi bi-key"></i>
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

<%-- ═══════════════ MODAL TẠO TÀI KHOẢN ═══════════════ --%>
<div class="modal fade" id="modalTaoTK" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><i class="bi bi-person-plus me-2"></i>Tạo tài khoản mới</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label fw-semibold">Email <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTaoEmail" runat="server" CssClass="form-control"
                        placeholder="email@example.com" TextMode="Email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTaoEmail"
                        ErrorMessage="Email không được trống." CssClass="text-danger small"
                        ValidationGroup="TaoTK" Display="Dynamic" />
                </div>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Mật khẩu <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTaoMK" runat="server" CssClass="form-control"
                        TextMode="Password" placeholder="Tối thiểu 8 ký tự" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTaoMK"
                        ErrorMessage="Mật khẩu không được trống." CssClass="text-danger small"
                        ValidationGroup="TaoTK" Display="Dynamic" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtTaoMK"
                        ValidationExpression=".{8,}"
                        ErrorMessage="Mật khẩu tối thiểu 8 ký tự." CssClass="text-danger small"
                        ValidationGroup="TaoTK" Display="Dynamic" />
                </div>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Quyền <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlTaoQuyen" runat="server" CssClass="form-select"
                            data-toggle-truong="divTaoTruong">
                        <asp:ListItem Value="3">Học sinh</asp:ListItem>
                        <asp:ListItem Value="4">Moderator</asp:ListItem>
                        <asp:ListItem Value="5">Tư vấn viên</asp:ListItem>
                        <asp:ListItem Value="2">Trường học</asp:ListItem>
                        <asp:ListItem Value="1">Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="mb-3" id="divTaoTruong" style="display:none;">
                    <label class="form-label fw-semibold">Trường gắn kết <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlTaoTruong" runat="server" CssClass="form-select">
                        <asp:ListItem Value="">-- Chọn trường --</asp:ListItem>
                    </asp:DropDownList>
                    <div class="form-text text-muted">Bắt buộc khi quyền = Trường học</div>
                </div>
                <asp:Literal ID="litTaoLoi" runat="server" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnTaoTK" runat="server" Text="Tạo tài khoản"
                    CssClass="btn btn-primary" OnClick="btnTaoTK_Click"
                    ValidationGroup="TaoTK" />
            </div>
        </div>
    </div>
</div>

<%-- ═══════════════ MODAL SỬA QUYỀN ═══════════════ --%>
<div class="modal fade" id="modalSuaQuyen" tabindex="-1">
    <div class="modal-dialog modal-sm">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><i class="bi bi-shield-lock me-2"></i>Sửa quyền</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hfSuaMaTK" runat="server" />
                <p class="text-muted small mb-2">Tài khoản: <strong><asp:Literal ID="litSuaEmail" runat="server" /></strong></p>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Quyền mới</label>
                        <asp:DropDownList ID="ddlSuaQuyen" runat="server" CssClass="form-select"
                            data-toggle-truong="divSuaTruong">
                        <asp:ListItem Value="3">Học sinh</asp:ListItem>
                        <asp:ListItem Value="4">Moderator</asp:ListItem>
                        <asp:ListItem Value="5">Tư vấn viên</asp:ListItem>
                        <asp:ListItem Value="2">Trường học</asp:ListItem>
                        <asp:ListItem Value="1">Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="mb-3" id="divSuaTruong" style="display:none;">
                    <label class="form-label fw-semibold">Trường gắn kết <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlSuaTruong" runat="server" CssClass="form-select">
                        <asp:ListItem Value="">-- Chọn trường --</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Literal ID="litSuaLoi" runat="server" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnSuaQuyen" runat="server" Text="Lưu"
                    CssClass="btn btn-info" OnClick="btnSuaQuyen_Click" />
            </div>
        </div>
    </div>
</div>

<%-- ═══════════════ MODAL RESET MẬT KHẨU ═══════════════ --%>
<div class="modal fade" id="modalResetMK" tabindex="-1">
    <div class="modal-dialog modal-sm">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><i class="bi bi-key me-2"></i>Reset mật khẩu</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <asp:HiddenField ID="hfResetMaTK" runat="server" />
                <p class="text-muted small mb-2">Tài khoản: <strong><asp:Literal ID="litResetEmail" runat="server" /></strong></p>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Mật khẩu mới <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtResetMK" runat="server" CssClass="form-control"
                        TextMode="Password" placeholder="Tối thiểu 8 ký tự" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtResetMK"
                        ErrorMessage="Mật khẩu không được trống." CssClass="text-danger small"
                        ValidationGroup="ResetMK" Display="Dynamic" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtResetMK"
                        ValidationExpression=".{8,}"
                        ErrorMessage="Mật khẩu tối thiểu 8 ký tự." CssClass="text-danger small"
                        ValidationGroup="ResetMK" Display="Dynamic" />
                </div>
                <div class="alert alert-warning py-2 small">
                    <i class="bi bi-exclamation-triangle me-1"></i>
                    Mật khẩu sẽ được đặt trực tiếp — không gửi email. Hành động được ghi log. 
                    User sẽ bị yêu cầu đổi mật khẩu lần đăng nhập tiếp theo.
                </div>
                <asp:Literal ID="litResetLoi" runat="server" />
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnResetMK" runat="server" Text="🔑 Đặt mật khẩu"
                    CssClass="btn btn-warning" OnClick="btnResetMK_Click"
                    ValidationGroup="ResetMK" />
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script src="/Scripts/validation_data_input.js"></script>
<script>
// Server flags cho site.js
window.openSuaQuyen = '<%= hfSuaMaTK.Value %>';
window.openResetMK  = '<%= hfResetMaTK.Value %>';

// ── AdminValidator — modal Tạo tài khoản ──────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    AdminValidator.init('form', {
        '<%= txtTaoEmail.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập email.'),
            AdminValidator.rules.email('Địa chỉ email không hợp lệ.')
        ],
        '<%= txtTaoMK.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập mật khẩu.'),
            AdminValidator.rules.minLen(8, 'Mật khẩu tối thiểu 8 ký tự.')
        ]
    }, { triggerButtonId: '<%= btnTaoTK.ClientID %>' });

    // ── AdminValidator — modal Reset mật khẩu ─────────────────────────────
    AdminValidator.init('form', {
        '<%= txtResetMK.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập mật khẩu mới.'),
            AdminValidator.rules.minLen(8, 'Mật khẩu mới tối thiểu 8 ký tự.')
        ]
    }, { triggerButtonId: '<%= btnResetMK.ClientID %>' });
});
</script>
</asp:Content>
