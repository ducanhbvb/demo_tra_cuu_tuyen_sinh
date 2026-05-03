<%@ Page Title="Quản lý bài viết" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyBaiViet.aspx.cs" Inherits="Admin_QuanLyBaiViet" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex justify-content-between align-items-center mb-2">
    <h5 class="fw-bold mb-0">Quản lý bài viết</h5>
</div>

<%-- Filter + Thêm mới --%>
<div class="admin-filter-bar">
    <asp:DropDownList ID="ddlTruong" runat="server" CssClass="form-select" style="max-width:220px;">
        <asp:ListItem Value="">-- Tất cả trường --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-select" style="max-width:160px;">
        <asp:ListItem Value="">-- Trạng thái --</asp:ListItem>
        <asp:ListItem Value="1">Hiển thị</asp:ListItem>
        <asp:ListItem Value="0">Đã ẩn</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc"
        CssClass="btn-filter" OnClick="btnLoc_Click" />
    <span class="filter-spacer"></span>
    <button class="btn-add-new" data-bs-toggle="modal" data-bs-target="#modalBaiViet"
            onclick="clearModal()">
        <i class="bi bi-plus-lg"></i>Thêm mới
    </button>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="card border-0 shadow-sm">
    <div class="card-body p-0">
        <asp:GridView ID="gvBaiViet" runat="server"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaBaiViet" OnRowCommand="gvBaiViet_RowCommand"
            EmptyDataText="Chưa có bài viết nào.">
            <Columns>
                <asp:BoundField DataField="TieuDe"    HeaderText="Tiêu đề"  HtmlEncode="true" ItemStyle-Width="300px" />
                <asp:BoundField DataField="TenTruong" HeaderText="Trường"   HtmlEncode="true" />
                <asp:BoundField DataField="NgayDang"  HeaderText="Ngày đăng"
                    DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="LuotXem"   HeaderText="Lượt xem" />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# Convert.ToBoolean(Eval("TrangThai"))
                            ? "<span class='badge bg-success'>Hiện</span>"
                            : "<span class='badge bg-secondary'>Ẩn</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="ToggleBV"
                            CommandArgument='<%# Eval("MaBaiViet") %>'
                            CssClass='<%# Convert.ToBoolean(Eval("TrangThai")) ? "btn btn-xs btn-warning me-1" : "btn btn-xs btn-success me-1" %>'
                            OnClientClick="return confirm('Thay đổi trạng thái hiển thị bài viết này?')">
                            <%# Convert.ToBoolean(Eval("TrangThai")) ? "Ẩn" : "Hiện" %>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="SuaBV"
                            CommandArgument='<%# Eval("MaBaiViet") %>'
                            CssClass="btn btn-xs btn-outline-primary me-1">Sửa</asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="XoaBV"
                            CommandArgument='<%# Eval("MaBaiViet") %>'
                            CssClass="btn btn-xs btn-outline-danger"
                            OnClientClick="return confirm('Xóa bài viết này?')">Xóa</asp:LinkButton>
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
<asp:HiddenField ID="hfMaBaiViet" runat="server" Value="0" />
<div class="modal fade" id="modalBaiViet" tabindex="-1" data-bs-backdrop="static">
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
                            placeholder="Tiêu đề bài viết..." oninput="autoSlug(this)" />
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Slug (tự sinh)</label>
                        <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control text-muted"
                            placeholder="tu-dong-sinh-tu-tieu-de" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Trường liên quan</label>
                        <asp:DropDownList ID="ddlMTruong" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Không liên kết trường --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label">Ảnh bìa</label>
                        <asp:HiddenField ID="hfAnhChinh" runat="server" />
                        <div class="mb-1" id="divPreviewAnhBV" style="display:none;">
                            <asp:Image ID="imgPreviewBV" runat="server"
                                CssClass="img-thumbnail"
                                style="max-height:120px;max-width:100%;width:auto;height:auto;object-fit:cover;cursor:pointer;border-radius:8px;"
                                onclick="viewImg(this)" title="Click để xem ảnh đầy đủ" />
                        </div>
                        <asp:FileUpload ID="fuAnhChinh" runat="server" CssClass="form-control form-control-sm"
                            accept=".jpg,.jpeg,.png,.gif,.webp"
                            onchange="previewBVImg(this)" />
                        <div class="form-text">Tối đa 5 MB · Để trống = giữ ảnh cũ.</div>
                    </div>
                    <div class="col-12">
                        <label class="form-label">Nội dung <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtNoiDung" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="12"
                            placeholder="Nội dung bài viết (hỗ trợ HTML)..." />
                        <div class="form-text">Hỗ trợ HTML cơ bản: &lt;p&gt;, &lt;b&gt;, &lt;i&gt;, &lt;h3&gt;, &lt;ul&gt;, &lt;img&gt;...</div>
                    </div>
                    <div class="col-auto d-flex align-items-end">
                        <div class="form-check">
                            <asp:CheckBox ID="chkTrangThai" runat="server" CssClass="form-check-input"
                                Checked="true" />
                            <label class="form-check-label">Hiển thị ngay</label>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Hủy</button>
                <asp:Button ID="btnLuuBV" runat="server" Text="Lưu bài viết"
                    CssClass="btn btn-primary" OnClick="btnLuuBV_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// Tự sinh slug từ tiêu đề
function autoSlug(input) {
    var slug = input.value
        .toLowerCase()
        .replace(/đ/g, 'd')
        .normalize('NFD').replace(/[\u0300-\u036f]/g, '')
        .replace(/[^a-z0-9\s-]/g, '')
        .trim().replace(/\s+/g, '-').replace(/-+/g, '-');
    document.getElementById('<%= txtSlug.ClientID %>').value = slug;
}

function clearModal() {
    document.getElementById('<%= txtTieuDe.ClientID %>').value = '';
    document.getElementById('<%= txtSlug.ClientID %>').value = '';
    document.getElementById('<%= hfAnhChinh.ClientID %>').value = '';
    document.getElementById('<%= txtNoiDung.ClientID %>').value = '';
    document.getElementById('<%= hfMaBaiViet.ClientID %>').value = '0';
    // ẩn preview ảnh cũ
    document.getElementById('divPreviewAnhBV').style.display = 'none';
}

// Preview ảnh bài viết local trước khi upload
function previewBVImg(input) {
    if (!input.files || !input.files[0]) return;
    var file = input.files[0];
    if (!file.type.startsWith('image/')) return;
    var reader = new FileReader();
    reader.onload = function(e) {
        var img  = document.getElementById('<%= imgPreviewBV.ClientID %>');
        var wrap = document.getElementById('divPreviewAnhBV');
        if (img)  img.src = e.target.result;
        if (wrap) wrap.style.display = '';
    };
    reader.readAsDataURL(file);
}
// Mở ảnh full trong tab mới
function viewImg(el) {
    if (!el || !el.src || el.src === window.location.href) return;
    window.open(el.src, '_blank');
}

// Mở lại modal sau postback nếu cần
if ('<%= ShowModal %>' === 'true') {
    var modal = new bootstrap.Modal(document.getElementById('modalBaiViet'));
    modal.show();
}
</script>
</asp:Content>
