<%@ Page Title="Tin tuyển sinh" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyTinTuyenSinh.aspx.cs" Inherits="Admin_QuanLyTinTuyenSinh" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex justify-content-between align-items-center mb-2">
    <h5 class="fw-bold mb-0">Quản lý tin tuyển sinh</h5>
</div>

<%-- Filter + Thêm mới --%>
<div class="admin-filter-bar">
    <asp:DropDownList ID="ddlTruong" runat="server" CssClass="form-select" style="max-width:220px;">
        <asp:ListItem Value="">-- Chọn trường --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select" style="max-width:180px;">
        <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlNam" runat="server" CssClass="form-select" style="max-width:110px;">
        <asp:ListItem Value="">-- Năm --</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc"
        CssClass="btn-filter" OnClick="btnLoc_Click" />
    <span class="filter-spacer"></span>
    <button class="btn-add-new" data-bs-toggle="modal" data-bs-target="#modalThem">
        <i class="bi bi-plus-lg"></i>Thêm mới
    </button>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="card border-0 shadow-sm">
    <div class="card-body p-0">
        <asp:GridView ID="gvTin" runat="server"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaTin" OnRowCommand="gvTin_RowCommand"
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
                        <%# Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))
                            ? "<span class='badge bg-success'>Hiện</span>"
                            : "<span class='badge bg-secondary'>Ẩn</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="ToggleTin"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass='<%# (Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))) ? "btn btn-xs btn-warning me-1" : "btn btn-xs btn-success me-1" %>'
                            OnClientClick="return confirm('Thay đổi trạng thái hiển thị tin này?')">
                            <%# (Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))) ? "Ẩn" : "Hiện" %>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="SuaTin"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass="btn btn-xs btn-outline-primary me-1">Sửa</asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="XoaTin"
                            CommandArgument='<%# Eval("MaTin") %>'
                            CssClass="btn btn-xs btn-outline-danger"
                            OnClientClick="return confirm('Xóa tin này?')">Xóa</asp:LinkButton>
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
<div class="modal fade" id="modalThem" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold">Thêm / Sửa tin tuyển sinh</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label">Trường <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMTruong" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn trường --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Ngành <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMNganh" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Phương thức <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlMPhuongThuc" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Năm tuyển sinh</label>
                        <asp:TextBox ID="txtMNam" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Chỉ tiêu</label>
                        <asp:TextBox ID="txtMChiTieu" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Điểm chuẩn năm trước</label>
                        <asp:TextBox ID="txtMDiemTruoc" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-3">
                        <label class="form-label">Điểm chuẩn năm nay</label>
                        <asp:TextBox ID="txtMDiemNay" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Tổ hợp môn</label>
                        <asp:TextBox ID="txtMToHop" runat="server" CssClass="form-control" placeholder="A00,A01,D01" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Học phí (triệu/năm)</label>
                        <asp:TextBox ID="txtMHocPhi" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Loại hình đào tạo</label>
                        <asp:TextBox ID="txtMLoaiHinh" runat="server" CssClass="form-control" placeholder="Chính quy" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Cơ sở đào tạo</label>
                        <asp:TextBox ID="txtMCoSo" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Tiêu đề bài đăng</label>
                        <asp:TextBox ID="txtMTieuDe" runat="server" CssClass="form-control"
                            placeholder="Để trống sẽ tự sinh: Tuyển sinh 2026 - Ngành - Phương thức" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Hạn nộp hồ sơ</label>
                        <asp:TextBox ID="txtMHanNop" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-12">
                        <label class="form-label">Mô tả chi tiết</label>
                        <asp:TextBox ID="txtMMoTa" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="4" placeholder="Mô tả yêu cầu, điều kiện xét tuyển..." />
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
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnLuuTin" runat="server" Text="Lưu"
                    CssClass="btn btn-primary" OnClick="btnLuuTin_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// Mở lại modal nếu có lỗi (sau postback)
if ('<%= ShowModal %>' === 'true') {
    var modal = new bootstrap.Modal(document.getElementById('modalThem'));
    modal.show();
}
</script>
</asp:Content>
