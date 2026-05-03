<%@ Page Title="Hồ sơ của tôi" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="MyProfile.aspx.cs" Inherits="Profile_MyProfile" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="row g-4">
    <div class="col-lg-8">
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-white fw-bold">
                <i class="bi bi-person-circle text-primary me-2"></i>Thông tin cá nhân
            </div>
            <div class="card-body">
                <asp:Literal ID="litThongBao" runat="server" />

                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Họ và tên</label>
                        <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" placeholder="Nguyễn Văn A" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Ngày sinh</label>
                        <asp:TextBox ID="txtNgaySinh" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Tỉnh / Thành phố</label>
                        <asp:DropDownList ID="ddlTinh" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn tỉnh --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Khu vực ưu tiên</label>
                        <asp:DropDownList ID="ddlKhuVuc" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                            <asp:ListItem Value="1">Khu vực 1 (KV1)</asp:ListItem>
                            <asp:ListItem Value="2">Khu vực 2 (KV2)</asp:ListItem>
                            <asp:ListItem Value="3">Khu vực 3 (KV3)</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Điểm dự kiến (tổng 3 môn)</label>
                        <asp:TextBox ID="txtDiem" runat="server" CssClass="form-control"
                            placeholder="VD: 25.5" TextMode="Number" />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Ngành quan tâm</label>
                        <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12">
                        <label class="form-label fw-semibold">Định hướng nghề nghiệp</label>
                        <asp:TextBox ID="txtMucTieu" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="3" placeholder="Mô tả mục tiêu nghề nghiệp của bạn..." />
                    </div>
                </div>

                <div class="mt-4 d-flex gap-2">
                    <asp:Button ID="btnLuu" runat="server" Text="Lưu hồ sơ"
                        CssClass="btn btn-primary" OnClick="btnLuu_Click" CausesValidation="false" />
                </div>
            </div>
        </div>

        <%-- Đổi mật khẩu --%>
        <div class="card border-0 shadow-sm mt-4">
            <div class="card-header bg-white fw-bold">
                <i class="bi bi-shield-lock text-warning me-2"></i>Đổi mật khẩu
            </div>
            <div class="card-body">
                <asp:Literal ID="litMKThongBao" runat="server" />
                <div class="mb-3">
                    <label class="form-label">Mật khẩu hiện tại</label>
                    <asp:TextBox ID="txtMKCu" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Mật khẩu mới</label>
                    <asp:TextBox ID="txtMKMoi" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Xác nhận mật khẩu mới</label>
                    <asp:TextBox ID="txtMKXacNhan" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <asp:Button ID="btnDoiMK" runat="server" Text="Đổi mật khẩu"
                    CssClass="btn btn-warning" OnClick="btnDoiMK_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <div class="col-lg-4">
        <div class="card border-0 shadow-sm text-center p-4">
            <%-- Avatar --%>
            <asp:HiddenField ID="hfAvatar" runat="server" />
            <div class="mb-2">
                <asp:Image ID="imgAvatar" runat="server"
                    CssClass="rounded-circle border"
                    style="width:90px;height:90px;object-fit:cover;cursor:pointer;"
                    AlternateText="Avatar"
                    onclick="if(this.src)window.open(this.src,'_blank')"
                    title="Click để xem ảnh đầy đủ" />
            </div>
            <h5 class="mb-0"><asp:Literal ID="litHoTen" runat="server" /></h5>
            <p class="text-muted small mb-2"><asp:Literal ID="litEmail" runat="server" /></p>
            <asp:FileUpload ID="fuAvatar" runat="server" CssClass="form-control form-control-sm mb-1"
                accept=".jpg,.jpeg,.png,.gif,.webp" />
            <div class="form-text text-center">Tối đa 5 MB · Bấm "Lưu hồ sơ" để lưu ảnh</div>
        </div>

        <%-- 4.3: Gợi ý trường phù hợp --%>
        <asp:Panel ID="pnlGoiY" runat="server" Visible="false" CssClass="mt-3">
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-white fw-bold py-2">
                <i class="bi bi-stars text-warning me-1"></i>
                Gợi ý cho điểm <b><asp:Literal ID="litDiemGoiY" runat="server" /></b>
            </div>
            <div class="card-body p-2">
                <asp:Repeater ID="rptGoiY" runat="server">
                    <ItemTemplate>
                        <div class="d-flex align-items-start gap-2 py-2 border-bottom">
                            <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string) ? "/Resources/Images/no-image.png" : Eval("AnhDaiDien") %>'
                                 width="40" height="40" style="object-fit:cover;border-radius:6px;"
                                 onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                            <div class="flex-grow-1 min-w-0">
                                <a href='<%# ResolveUrl("~/Client/ChiTietTruong.aspx?slug=" + Eval("Slug")) %>'
                                   class="fw-semibold small text-decoration-none d-block text-truncate">
                                    <%# Eval("TenTruong") %>
                                </a>
                                <div class="text-muted" style="font-size:0.75rem;">
                                    <%# Eval("TenChuyenNganh") %>
                                </div>
                                <div style="font-size:0.75rem;">
                                    <span class="text-danger fw-bold"><%# Eval("DiemChuanNamTruoc", "{0:F2}") %></span>
                                    <span class="text-muted ms-1">đ chuẩn</span>
                                    <%# (bool)Eval("KiemDinhChatLuong")
                                        ? "<span class='badge bg-success ms-1' style='font-size:0.65rem;'>KĐ</span>" : "" %>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="pt-2 text-center">
                    <a href='<%: ResolveUrl("~/Client/TimKiemTheoNganh.aspx") %>' class="small text-primary">
                        <i class="bi bi-arrow-right-circle me-1"></i>Xem thêm
                    </a>
                </div>
            </div>
        </div>
        </asp:Panel>
    </div>
</div>
</asp:Content>
