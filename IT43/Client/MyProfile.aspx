<%@ Page Title="Hồ sơ của tôi" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="MyProfile-new.aspx.cs" Inherits="Profile_MyProfile" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%: ResolveUrl("~/Content/myprofile.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="row g-4">
    <!-- Left Sidebar: Profile Info (3 columns) -->
    <div class="col-lg-3">
        <div class="profile-sidebar">
            <div class="glass-card p-4 text-center">
                <!-- Avatar -->
                <div class="avatar-container mb-3">
                    <div class="avatar-ring">
                        <asp:Image ID="imgAvatar" runat="server"
                            CssClass="avatar-img avatar-preview"
                            AlternateText="Avatar" />
                    </div>
                    <button type="button" class="avatar-edit-btn" onclick="document.getElementById('avatarUpload').click();" title="Thay đổi ảnh đại diện">
                        <i class="bi bi-camera-fill"></i>
                    </button>
                    <asp:HiddenField ID="hfAvatar" runat="server" />
                </div>

                <!-- Name & Email -->
                <h5 class="fw-bold mb-1"><asp:Literal ID="litHoTen" runat="server" /></h5>
                <p class="text-muted small mb-2"><asp:Literal ID="litEmail" runat="server" /></p>

                <!-- Role Badge -->
                <asp:Literal ID="litVaiTroBadge" runat="server" />

                <!-- Stats -->
                <div class="profile-stats">
                    <div class="stat-item">
                        <div class="stat-label"><i class="bi bi-123"></i> Điểm dự kiến</div>
                        <div class="stat-value"><asp:Literal ID="litDiemDuKien" runat="server" /></div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-label"><i class="bi bi-geo-alt"></i> Khu vực</div>
                        <div class="stat-value"><asp:Literal ID="litKhuVuc" runat="server" /></div>
                    </div>
                    <div class="stat-item stat-item-wide">
                        <div class="stat-label"><i class="bi bi-briefcase"></i> Ngành quan tâm</div>
                        <div class="stat-value"><asp:Literal ID="litNganhQuanTam" runat="server" /></div>
                    </div>
                </div>

                <!-- Quick Actions -->
                <asp:Panel ID="pnlQuickActions" runat="server" CssClass="quick-actions text-start">
                    <asp:Literal ID="litQuickActions" runat="server" />
                </asp:Panel>
            </div>
        </div>
    </div>

    <!-- Main Content: Tabs + Forms (6 columns) -->
    <div class="col-lg-6">
        <div class="glass-card p-4">
            <!-- Tabs -->
            <div class="profile-tabs">
                <button type="button" class="profile-tab active" onclick="switchTab('info')">
                    <i class="bi bi-person"></i>
                    Thông tin cá nhân
                </button>
                <button type="button" class="profile-tab" onclick="switchTab('password')">
                    <i class="bi bi-shield-lock"></i>
                    Đổi mật khẩu
                </button>
                <button type="button" class="profile-tab" onclick="switchTab('avatar')">
                    <i class="bi bi-image"></i>
                    Ảnh đại diện
                </button>
            </div>

            <!-- Alert Messages -->
            <asp:Literal ID="litThongBao" runat="server" />
            <asp:Literal ID="litMKThongBao" runat="server" />

            <!-- Tab Content: Personal Info -->
            <div id="tab-info" class="form-section active">
                <h5 class="section-title">
                    <i class="bi bi-person-vcard"></i>
                    Thông tin cá nhân
                </h5>

                <div class="row g-3">
                    <div class="col-md-12">
                        <div class="form-group-custom">
                            <label><i class="bi bi-person"></i> Họ và tên</label>
                            <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" placeholder="Nhập họ và tên" />
                        </div>
                    </div>

                    <asp:Panel ID="pnlThongTinHocSinh" runat="server" CssClass="col-md-12">
                        <div class="row g-3">
                            <div class="col-md-6">
                                <div class="form-group-custom">
                                    <label><i class="bi bi-calendar"></i> Ngày sinh</label>
                                    <asp:TextBox ID="txtNgaySinh" runat="server" CssClass="form-control" TextMode="Date" />
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="form-group-custom">
                                    <label><i class="bi bi-geo-alt"></i> Tỉnh / Thành phố</label>
                                    <asp:DropDownList ID="ddlTinh" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="">-- Chọn tỉnh --</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="form-group-custom">
                                    <label><i class="bi bi-map"></i> Khu vực ưu tiên</label>
                                    <asp:DropDownList ID="ddlKhuVuc" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="">-- Chọn --</asp:ListItem>
                                        <asp:ListItem Value="1">Khu vực 1 (KV1)</asp:ListItem>
                                        <asp:ListItem Value="2">Khu vực 2 (KV2)</asp:ListItem>
                                        <asp:ListItem Value="3">Khu vực 3 (KV3)</asp:ListItem>
                                    </asp:DropDownList>
                                    <small class="text-muted d-block mt-1" style="font-size: 0.75rem;">
                                        <a href="#" class="text-primary text-decoration-none" data-bs-toggle="modal" data-bs-target="#modalKhuVuc">
                                            <i class="bi bi-eye me-1"></i>Xem thêm
                                        </a>
                                    </small>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="form-group-custom">
                                    <label><i class="bi bi-123"></i> Điểm dự kiến (tổng 3 môn)</label>
                                    <asp:TextBox ID="txtDiem" runat="server" CssClass="form-control"
                                        placeholder="VD: 25.5" TextMode="Number" step="0.1" />
                                </div>
                            </div>

                            <div class="col-md-12">
                                <div class="form-group-custom">
                                    <label><i class="bi bi-briefcase"></i> Ngành quan tâm</label>
                                    <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                                        <asp:ListItem Value="">-- Chọn ngành --</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-md-12">
                                <div class="form-group-custom">
                                    <label><i class="bi bi-bullseye"></i> Định hướng nghề nghiệp</label>
                                    <asp:TextBox ID="txtMucTieu" runat="server" CssClass="form-control"
                                        TextMode="MultiLine" Rows="3" placeholder="Mô tả mục tiêu nghề nghiệp của bạn..." />
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnLuu" runat="server" Text="Lưu thay đổi"
                        CssClass="btn-primary-custom" OnClick="btnLuu_Click" CausesValidation="false" />
                </div>
            </div>

            <!-- Tab Content: Change Password -->
            <div id="tab-password" class="form-section">
                <h5 class="section-title">
                    <i class="bi bi-shield-lock"></i>
                    Đổi mật khẩu
                </h5>

                <div class="row g-3">
                    <div class="col-md-12">
                        <div class="form-group-custom">
                            <label><i class="bi bi-lock"></i> Mật khẩu hiện tại</label>
                            <div class="password-field-wrapper">
                                <asp:TextBox ID="txtMKCu" runat="server" CssClass="form-control" TextMode="Password" />
                                <button type="button" class="password-toggle" onclick="togglePassword('txtMKCu', this)">
                                    <i class="bi bi-eye"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-12">
                        <div class="form-group-custom">
                            <label><i class="bi bi-key"></i> Mật khẩu mới</label>
                            <div class="password-field-wrapper">
                                <asp:TextBox ID="txtMKMoi" runat="server" CssClass="form-control" TextMode="Password" />
                                <button type="button" class="password-toggle" onclick="togglePassword('txtMKMoi', this)">
                                    <i class="bi bi-eye"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-12">
                        <div class="form-group-custom">
                            <label><i class="bi bi-key-fill"></i> Xác nhận mật khẩu mới</label>
                            <div class="password-field-wrapper">
                                <asp:TextBox ID="txtMKXacNhan" runat="server" CssClass="form-control" TextMode="Password" />
                                <button type="button" class="password-toggle" onclick="togglePassword('txtMKXacNhan', this)">
                                    <i class="bi bi-eye"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnDoiMK" runat="server" Text="Cập nhật mật khẩu"
                        CssClass="btn-primary-custom" style="background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);"
                        OnClick="btnDoiMK_Click" CausesValidation="false" />
                </div>
            </div>

            <!-- Tab Content: Avatar Upload -->
            <div id="tab-avatar" class="form-section">
                <h5 class="section-title">
                    <i class="bi bi-image"></i>
                    Cập nhật ảnh đại diện
                </h5>

                <div class="upload-area" onclick="document.getElementById('avatarUpload').click();">
                    <i class="bi bi-cloud-arrow-up"></i>
                    <div class="upload-text">
                        Kéo thả ảnh vào đây hoặc <span>chọn file</span>
                    </div>
                    <div class="text-muted mt-2" style="font-size: 0.8rem;">
                        Hỗ trợ: JPG, PNG, GIF, WEBP (Tối đa 5MB)
                    </div>
                    <asp:FileUpload ID="avatarUpload" runat="server" CssClass="avatar-upload" ClientIDMode="Static" accept=".jpg,.jpeg,.png,.gif,.webp" style="display: none;" />
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnLuuAvatar" runat="server" Text="Lưu ảnh đại diện"
                        CssClass="btn-primary-custom" OnClick="btnLuu_Click" CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>

    <!-- Right Sidebar: Tips (3 columns) -->
    <div class="col-lg-3">
        <!-- Tips Card -->
        <div class="glass-card p-4">
            <h6 class="fw-bold mb-3">
                <i class="bi bi-lightbulb text-warning me-2"></i>
                Mẹo nhỏ
            </h6>
            <ul class="list-unstyled mb-0" style="font-size: 0.85rem; color: #64748b;">
                <li class="mb-2">
                    <i class="bi bi-check-circle-fill text-success me-2" style="font-size: 0.7rem;"></i>
                    Cập nhật điểm dự kiến để nhận gợi ý chính xác hơn
                </li>
                <li class="mb-2">
                    <i class="bi bi-check-circle-fill text-success me-2" style="font-size: 0.7rem;"></i>
                    Chọn đúng khu vực ưu tiên để tính điểm chuẩn
                </li>
                <li>
                    <i class="bi bi-check-circle-fill text-success me-2" style="font-size: 0.7rem;"></i>
                    Thêm ngành quan tâm để lọc gợi ý phù hợp
                </li>
            </ul>
        </div>
    </div>
</div>

<!-- Modal Khu Vuc Detail -->
<div class="modal fade" id="modalKhuVuc" tabindex="-1" aria-labelledby="modalKhuVucLabel" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="modalKhuVucLabel">
          <i class="bi bi-info-circle text-primary me-2"></i>Thông tin khu vực & điểm ưu tiên
        </h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <asp:Repeater ID="rptKhuVucInfo" runat="server">
          <HeaderTemplate>
            <table class="table table-sm table-hover align-middle">
              <thead class="table-light">
                <tr>
                  <th>Mã KV</th>
                  <th>Tên khu vực</th>
                  <th>Điểm ưu tiên</th>
                  <th>Mô tả</th>
                </tr>
              </thead>
              <tbody>
          </HeaderTemplate>
          <ItemTemplate>
                <tr>
                  <td><span class="badge bg-primary"><%# Eval("MaKV") %></span></td>
                  <td class="fw-semibold"><%# Eval("TenKV") %></td>
                  <td>
                    <span class="badge <%# Convert.ToDecimal(Eval("DiemCong")) > 0 ? "bg-success" : "bg-secondary" %>">
                      <%# Convert.ToDecimal(Eval("DiemCong")) > 0 ? "+" + Eval("DiemCong") + " điểm" : "Không tính" %>
                    </span>
                  </td>
                  <td class="text-muted small" style="max-width: 300px;"><%# Eval("MoTa") %></td>
                </tr>
          </ItemTemplate>
          <FooterTemplate>
              </tbody>
            </table>
          </FooterTemplate>
        </asp:Repeater>
        <asp:Panel ID="pnlKhuVucEmpty" runat="server" Visible="false">
          <div class="text-center text-muted py-3">
            <i class="bi bi-database-exclamation display-4"></i>
            <p class="mb-0 mt-2">Chưa có dữ liệu khu vực.</p>
          </div>
        </asp:Panel>
      </div>
    </div>
  </div>
</div>
</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptContent" runat="server">
    <script src="<%: ResolveUrl("~/Script/myprofile.js") %>"></script>
</asp:Content>
