<%@ Page Title="Thêm / Sửa trường" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="ChinhSuaTruong.aspx.cs" Inherits="Admin_ChinhSuaTruong" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex gap-2 mb-3">
    <a href="QuanLyTruong.aspx" class="btn btn-sm btn-outline-secondary">
        <i class="bi bi-arrow-left me-1"></i>Quay lại
    </a>
    <h5 class="fw-bold mb-0 ms-2">
        <asp:Literal ID="litTieuDe" runat="server" />
    </h5>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="card border-0 shadow-sm">
    <div class="card-body">
        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label fw-semibold">Tên trường <span class="text-danger">*</span></label>
                <asp:TextBox ID="txtTen" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ControlToValidate="txtTen" runat="server"
                    CssClass="text-danger small" ErrorMessage="Bắt buộc." Display="Dynamic" />
            </div>
            <div class="col-md-4">
                <label class="form-label fw-semibold">Slug (SEO)</label>
                <asp:TextBox ID="txtSlug" runat="server" CssClass="form-control"
                    placeholder="tu-dong-tao-neu-de-trong" />
            </div>
            <div class="col-md-6">
                <label class="form-label fw-semibold">Địa chỉ</label>
                <asp:TextBox ID="txtDiaChi" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Tỉnh / Thành phố</label>
                <asp:TextBox ID="txtTinhThanh" runat="server" CssClass="form-control" />
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
                <label class="form-label fw-semibold">Điện thoại</label>
                <asp:TextBox ID="txtSdt" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Website</label>
                <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label fw-semibold">Quy mô</label>
                <asp:TextBox ID="txtQuyMo" runat="server" CssClass="form-control" placeholder="VD: 30.000 SV" />
            </div>
            <%-- Ảnh đại diện --%>
            <div class="col-md-6">
                <label class="form-label fw-semibold">Ảnh đại diện</label>
                <asp:HiddenField ID="hfAnhDaiDien" runat="server" />
                <div class="mb-2">
                    <asp:Image ID="imgPreviewAvatar" runat="server"
                        CssClass="img-thumbnail"
                        style="width:120px;height:120px;object-fit:cover;cursor:pointer;border-radius:8px;"
                        AlternateText="Ảnh đại diện"
                        onclick="viewImg(this)" title="Click để xem ảnh đầy đủ" />
                </div>
                <asp:FileUpload ID="fuAnhDaiDien" runat="server" CssClass="form-control form-control-sm"
                    accept=".jpg,.jpeg,.png,.gif,.webp"
                    onchange="previewImg(this, '<%= imgPreviewAvatar.ClientID %>')" />
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
                        onclick="viewImg(this)" title="Click để xem ảnh đầy đủ" />
                </div>
                <asp:FileUpload ID="fuAnhBia" runat="server" CssClass="form-control form-control-sm"
                    accept=".jpg,.jpeg,.png,.gif,.webp"
                    onchange="previewImg(this, '<%= imgPreviewBia.ClientID %>')" />
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

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// Preview ảnh local trước khi upload
function previewImg(input, imgId) {
    if (!input.files || !input.files[0]) return;
    var file = input.files[0];
    if (!file.type.startsWith('image/')) return;
    var reader = new FileReader();
    reader.onload = function(e) {
        var img = document.getElementById(imgId);
        if (img) { img.src = e.target.result; img.style.display = ''; }
    };
    reader.readAsDataURL(file);
}
// Mở ảnh full trong tab mới
function viewImg(el) {
    if (!el || !el.src || el.src === window.location.href) return;
    window.open(el.src, '_blank');
}
</script>
</asp:Content>
