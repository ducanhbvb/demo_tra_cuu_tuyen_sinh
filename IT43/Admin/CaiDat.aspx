<%@ Page Title="Cài đặt hệ thống" Language="C#" MasterPageFile="~/Admin/Admin.Master"
    AutoEventWireup="true" CodeBehind="CaiDat.aspx.cs" Inherits="Admin_CaiDat" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<link href="/Content/admin-setting.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="settings-page mx-auto">
    <%-- Page Header --%>
    <div class="page-header">
        <div>
            <h4 class="page-header-title">
                <i class="bi bi-gear-fill"></i> Cài đặt Hệ thống
            </h4>
            <div class="page-header-sub">
                Dashboard / <a href="/admin/index.aspx">Admin</a> / Cài đặt
            </div>
        </div>
    </div>

    <%-- Thông báo kết quả --%>
    <asp:Literal ID="litThongBao" runat="server" />

    <!-- ══ SECTION 1: Trang chủ ══════════════════════════════════════════════ -->
    <div class="admin-card mb-4">
        <div class="admin-card-header">
            <div class="admin-card-title">
                <i class="bi bi-layout-wtf text-primary"></i>
                Hiển thị Trang chủ
            </div>
        </div>
        <div class="admin-card-body">
            <div class="setting-group">

                <%-- Năm tuyển sinh --%>
                <div class="setting-item">
                    <div class="setting-icon bg-primary-bg text-primary">
                        <i class="bi bi-calendar-check"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Năm tuyển sinh hiện tại</div>
                        <div class="setting-desc">
                            Năm mặc định được chọn sẵn ở bộ lọc tìm kiếm trường và Banner trang chủ.
                            Để trống để tự động lấy năm mới nhất từ dữ liệu.
                        </div>
                    </div>
                    <div class="setting-control">
                        <span class="setting-badge-preview">
                            Hiện tại: <strong><asp:Literal ID="litNamHienTai" runat="server" /></strong>
                        </span>
                        <asp:TextBox ID="txtNamTuyenSinh" runat="server"
                            CssClass="form-control" MaxLength="4"
                            placeholder="VD: 2025" />
                        <asp:Button ID="btnSaveNamTuyenSinh" runat="server" Text="Lưu"
                            CssClass="btn btn-primary btn-sm px-3"
                            OnClick="btnSaveNamTuyenSinh_Click" />
                    </div>
                </div>

                <%-- Số trường nổi bật --%>
                <div class="setting-item">
                    <div class="setting-icon bg-success-bg text-success">
                        <i class="bi bi-building"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Số trường nổi bật</div>
                        <div class="setting-desc">
                            Số lượng thẻ trường đại học hiển thị ở mục <b>Trường nổi bật</b> trang chủ (1–20).
                        </div>
                    </div>
                    <div class="setting-control">
                        <span class="setting-badge-preview">
                            Hiện tại: <strong><asp:Literal ID="litSoTruong" runat="server" /></strong> trường
                        </span>
                        <asp:TextBox ID="txtSoTruongNoiBat" runat="server"
                            CssClass="form-control" TextMode="Number"
                            placeholder="8" />
                        <asp:Button ID="btnSaveSoTruong" runat="server" Text="Lưu"
                            CssClass="btn btn-success btn-sm px-3"
                            OnClick="btnSaveSoTruong_Click" />
                    </div>
                </div>

                <%-- Số bài viết mới --%>
                <div class="setting-item">
                    <div class="setting-icon bg-info-bg text-info">
                        <i class="bi bi-newspaper"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Số bài viết mới nhất</div>
                        <div class="setting-desc">
                            Số bài viết tự động xuất hiện ở <b>Slider</b> cuối trang chủ (1–15).
                        </div>
                    </div>
                    <div class="setting-control">
                        <span class="setting-badge-preview">
                            Hiện tại: <strong><asp:Literal ID="litSoBaiViet" runat="server" /></strong> bài
                        </span>
                        <asp:TextBox ID="txtSoBaiViet" runat="server"
                            CssClass="form-control" TextMode="Number"
                            placeholder="6" />
                        <asp:Button ID="btnSaveBaiViet" runat="server" Text="Lưu"
                            CssClass="btn btn-info btn-sm px-3"
                            OnClick="btnSaveBaiViet_Click" />
                    </div>
                </div>

                <%-- Số tin tuyển sinh mới --%>
                <div class="setting-item">
                    <div class="setting-icon bg-warning-bg text-warning">
                        <i class="bi bi-megaphone"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Số tin tuyển sinh mới nhất</div>
                        <div class="setting-desc">
                            Số tin tuyển sinh hiển thị ở trang danh sách tin và trang chủ (1–20).
                        </div>
                    </div>
                    <div class="setting-control">
                        <span class="setting-badge-preview">
                            Hiện tại: <strong><asp:Literal ID="litSoTin" runat="server" /></strong> tin
                        </span>
                        <asp:TextBox ID="txtSoTin" runat="server"
                            CssClass="form-control" TextMode="Number"
                            placeholder="10" />
                        <asp:Button ID="btnSaveSoTin" runat="server" Text="Lưu"
                            CssClass="btn btn-warning btn-sm px-3"
                            OnClick="btnSaveSoTin_Click" />
                    </div>
                </div>


            </div><!-- /.setting-group -->
        </div>
    </div>

    <!-- ══ SECTION 2: 3 nút gợi ý dưới thanh tìm kiếm ═══════════════════════════ -->
    <div class="admin-card mb-4">
        <div class="admin-card-header d-flex justify-content-between align-items-center flex-wrap gap-2">
            <div class="admin-card-title">
                <i class="bi bi-tags-fill text-primary"></i>
                3 nút gợi ý dưới thanh tìm kiếm
            </div>
            <asp:Button ID="btnResetHomeTags" runat="server" Text="↺ Khôi phục mặc định 3 nút"
                CssClass="btn btn-outline-secondary btn-sm"
                OnClick="btnResetHomeTags_Click" CausesValidation="false" />
        </div>
        <div class="admin-card-body pb-3">
            <div class="tag-settings-layout">
            <div>
            <div class="home-tag-grid">
                <%-- NÚT 1 --%>
                <div class="home-tag-card">
                    <div class="home-tag-head">
                        <span class="home-tag-index">1</span>
                        <span class="preview-hero-tag"><i class="bi bi-fire text-warning"></i>Nút 1</span>
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Tên nút</label>
                        <asp:TextBox ID="txtHomeTag1Text" runat="server" CssClass="form-control" MaxLength="40" placeholder="VD: Bách Khoa" />
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Chọn chủ đề</label>
                        <div class="url-builder" data-target="<%= txtHomeTag1Url.ClientID %>">
                            <select class="form-select url-type-sel">
                                <option data-base="" data-param="" data-slug-type="">— Chọn chủ đề —</option>
                                <option data-base="/Client/ChiTietTruong.aspx" data-param="slug" data-slug-type="slug" data-hint="Dán slug trường (lấy từ ô Slug khi chỉnh sửa trường)">🏛 Trang chi tiết Trường</option>
                                <option data-base="/Client/ChiTietBaiViet.aspx" data-param="slug" data-slug-type="slug" data-hint="Dán slug bài viết (lấy từ ô Slug khi quản lý bài viết)">📰 Trang chi tiết Bài viết</option>
                                <option data-base="/Client/ChiTietTinTuyenSinh.aspx" data-param="id" data-slug-type="id" data-hint="Dán mã ID của tin (lấy từ modal sửa tin → xem Mã tin)">📢 Trang chi tiết Tin tuyển sinh</option>
                                <option data-base="" data-param="" data-slug-type="divider" disabled>──────────────────</option>
                                <option data-base="/TimKiemTruong.aspx" data-param="">🔍 Tìm kiếm tất cả trường</option>
                                <option data-base="/TimKiemTruong.aspx?loai=1" data-param="">🏫 Trường Đại học Công lập</option>
                                <option data-base="/TimKiemTruong.aspx?loai=2" data-param="">🏢 Trường Đại học Tư thục</option>
                                <option data-base="/TimKiemTruong.aspx?loai=3" data-param="">🎓 Trường Cao đẳng</option>
                                <option data-base="/TimKiemTruong.aspx?loai=4" data-param="">🛠 Trường Nghề / Trung cấp</option>
                                <option data-base="/TimKiemTheoNganh.aspx" data-param="">📚 Tìm kiếm theo ngành</option>
                                <option data-base="/TraCuuDiemChuan.aspx" data-param="">📊 Tra cứu điểm chuẩn</option>
                                <option data-base="/BaiViet.aspx" data-param="">📋 Danh sách bài viết</option>
                                <option data-base="/SoSanhTruong.aspx" data-param="">⚖️ So sánh trường</option>
                                <option data-base="/GoiYTruong.aspx" data-param="">💡 Gợi ý trường phù hợp</option>
                            </select>
                            <div class="url-keyword-wrap d-none mt-2">
                                <div class="input-group input-group-sm">
                                    <input type="text" class="form-control url-keyword-input" placeholder="Dán slug hoặc mã ID..." />
                                    <button type="button" class="btn btn-success url-apply-btn fw-bold px-3">✓ Áp dụng</button>
                                </div>
                                <div class="form-hint url-preview-hint mt-1"></div>
                            </div>
                        </div>
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Đường dẫn</label>
                        <asp:TextBox ID="txtHomeTag1Url" runat="server" CssClass="form-control url-result-box" MaxLength="180" placeholder="Tự điền sau khi áp dụng..." />
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Biểu tượng</label>
                        <asp:DropDownList ID="ddlHomeTag1Icon" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">Không có biểu tượng</asp:ListItem>
                            <asp:ListItem Value="bi bi-fire text-warning">🔥 Nổi bật / Hot</asp:ListItem>
                            <asp:ListItem Value="bi bi-mortarboard">🎓 Đại học</asp:ListItem>
                            <asp:ListItem Value="bi bi-award">🏅 Cao đẳng</asp:ListItem>
                            <asp:ListItem Value="bi bi-tools">🛠 Trường nghề</asp:ListItem>
                            <asp:ListItem Value="bi bi-building">🏛 Công lập</asp:ListItem>
                            <asp:ListItem Value="bi bi-house-heart">🏢 Tư thục</asp:ListItem>
                            <asp:ListItem Value="bi bi-globe2">🌐 Quốc tế</asp:ListItem>
                            <asp:ListItem Value="bi bi-laptop">💻 Công nghệ thông tin</asp:ListItem>
                            <asp:ListItem Value="bi bi-graph-up-arrow">📈 Kinh tế</asp:ListItem>
                            <asp:ListItem Value="bi bi-heart-pulse">🩺 Y dược</asp:ListItem>
                            <asp:ListItem Value="bi bi-bank">🏦 Tài chính - Ngân hàng</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%-- NÚT 2 --%>
                <div class="home-tag-card">
                    <div class="home-tag-head">
                        <span class="home-tag-index">2</span>
                        <span class="preview-hero-tag"><i class="bi bi-building"></i>Nút 2</span>
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Tên nút</label>
                        <asp:TextBox ID="txtHomeTag2Text" runat="server" CssClass="form-control" MaxLength="40" placeholder="VD: Công Lập" />
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Chọn chủ đề</label>
                        <div class="url-builder" data-target="<%= txtHomeTag2Url.ClientID %>">
                            <select class="form-select url-type-sel">
                                <option data-base="" data-param="" data-slug-type="">— Chọn chủ đề —</option>
                                <option data-base="/Client/ChiTietTruong.aspx" data-param="slug" data-slug-type="slug" data-hint="Dán slug trường (lấy từ ô Slug khi chỉnh sửa trường)">🏛 Trang chi tiết Trường</option>
                                <option data-base="/Client/ChiTietBaiViet.aspx" data-param="slug" data-slug-type="slug" data-hint="Dán slug bài viết (lấy từ ô Slug khi quản lý bài viết)">📰 Trang chi tiết Bài viết</option>
                                <option data-base="/Client/ChiTietTinTuyenSinh.aspx" data-param="id" data-slug-type="id" data-hint="Dán mã ID của tin (lấy từ modal sửa tin → xem Mã tin)">📢 Trang chi tiết Tin tuyển sinh</option>
                                <option data-base="" data-param="" data-slug-type="divider" disabled>──────────────────</option>
                                <option data-base="/TimKiemTruong.aspx" data-param="">🔍 Tìm kiếm tất cả trường</option>
                                <option data-base="/TimKiemTruong.aspx?loai=1" data-param="">🏫 Trường Đại học Công lập</option>
                                <option data-base="/TimKiemTruong.aspx?loai=2" data-param="">🏢 Trường Đại học Tư thục</option>
                                <option data-base="/TimKiemTruong.aspx?loai=3" data-param="">🎓 Trường Cao đẳng</option>
                                <option data-base="/TimKiemTruong.aspx?loai=4" data-param="">🛠 Trường Nghề / Trung cấp</option>
                                <option data-base="/TimKiemTheoNganh.aspx" data-param="">📚 Tìm kiếm theo ngành</option>
                                <option data-base="/TraCuuDiemChuan.aspx" data-param="">📊 Tra cứu điểm chuẩn</option>
                                <option data-base="/BaiViet.aspx" data-param="">📋 Danh sách bài viết</option>
                                <option data-base="/SoSanhTruong.aspx" data-param="">⚖️ So sánh trường</option>
                                <option data-base="/GoiYTruong.aspx" data-param="">💡 Gợi ý trường phù hợp</option>
                            </select>
                            <div class="url-keyword-wrap d-none mt-2">
                                <div class="input-group input-group-sm">
                                    <input type="text" class="form-control url-keyword-input" placeholder="Dán slug hoặc mã ID..." />
                                    <button type="button" class="btn btn-success url-apply-btn fw-bold px-3">✓ Áp dụng</button>
                                </div>
                                <div class="form-hint url-preview-hint mt-1"></div>
                            </div>
                        </div>
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Đường dẫn</label>
                        <asp:TextBox ID="txtHomeTag2Url" runat="server" CssClass="form-control url-result-box" MaxLength="180" placeholder="Tự điền sau khi áp dụng..." />
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Biểu tượng</label>
                        <asp:DropDownList ID="ddlHomeTag2Icon" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">Không có biểu tượng</asp:ListItem>
                            <asp:ListItem Value="bi bi-fire text-warning">🔥 Nổi bật / Hot</asp:ListItem>
                            <asp:ListItem Value="bi bi-mortarboard">🎓 Đại học</asp:ListItem>
                            <asp:ListItem Value="bi bi-award">🏅 Cao đẳng</asp:ListItem>
                            <asp:ListItem Value="bi bi-tools">🛠 Trường nghề</asp:ListItem>
                            <asp:ListItem Value="bi bi-building">🏛 Công lập</asp:ListItem>
                            <asp:ListItem Value="bi bi-house-heart">🏢 Tư thục</asp:ListItem>
                            <asp:ListItem Value="bi bi-globe2">🌐 Quốc tế</asp:ListItem>
                            <asp:ListItem Value="bi bi-laptop">💻 Công nghệ thông tin</asp:ListItem>
                            <asp:ListItem Value="bi bi-graph-up-arrow">📈 Kinh tế</asp:ListItem>
                            <asp:ListItem Value="bi bi-heart-pulse">🩺 Y dược</asp:ListItem>
                            <asp:ListItem Value="bi bi-bank">🏦 Tài chính - Ngân hàng</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <%-- NÚT 3 --%>
                <div class="home-tag-card">
                    <div class="home-tag-head">
                        <span class="home-tag-index">3</span>
                        <span class="preview-hero-tag"><i class="bi bi-laptop"></i>Nút 3</span>
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Tên nút</label>
                        <asp:TextBox ID="txtHomeTag3Text" runat="server" CssClass="form-control" MaxLength="40" placeholder="VD: Khối IT" />
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Chọn chủ đề</label>
                        <div class="url-builder" data-target="<%= txtHomeTag3Url.ClientID %>">
                            <select class="form-select url-type-sel">
                                <option data-base="" data-param="" data-slug-type="">— Chọn chủ đề —</option>
                                <option data-base="/Client/ChiTietTruong.aspx" data-param="slug" data-slug-type="slug" data-hint="Dán slug trường (lấy từ ô Slug khi chỉnh sửa trường)">🏛 Trang chi tiết Trường</option>
                                <option data-base="/Client/ChiTietBaiViet.aspx" data-param="slug" data-slug-type="slug" data-hint="Dán slug bài viết (lấy từ ô Slug khi quản lý bài viết)">📰 Trang chi tiết Bài viết</option>
                                <option data-base="/Client/ChiTietTinTuyenSinh.aspx" data-param="id" data-slug-type="id" data-hint="Dán mã ID của tin (lấy từ modal sửa tin → xem Mã tin)">📢 Trang chi tiết Tin tuyển sinh</option>
                                <option data-base="" data-param="" data-slug-type="divider" disabled>──────────────────</option>
                                <option data-base="/TimKiemTruong.aspx" data-param="">🔍 Tìm kiếm tất cả trường</option>
                                <option data-base="/TimKiemTruong.aspx?loai=1" data-param="">🏫 Trường Đại học Công lập</option>
                                <option data-base="/TimKiemTruong.aspx?loai=2" data-param="">🏢 Trường Đại học Tư thục</option>
                                <option data-base="/TimKiemTruong.aspx?loai=3" data-param="">🎓 Trường Cao đẳng</option>
                                <option data-base="/TimKiemTruong.aspx?loai=4" data-param="">🛠 Trường Nghề / Trung cấp</option>
                                <option data-base="/TimKiemTheoNganh.aspx" data-param="">📚 Tìm kiếm theo ngành</option>
                                <option data-base="/TraCuuDiemChuan.aspx" data-param="">📊 Tra cứu điểm chuẩn</option>
                                <option data-base="/BaiViet.aspx" data-param="">📋 Danh sách bài viết</option>
                                <option data-base="/SoSanhTruong.aspx" data-param="">⚖️ So sánh trường</option>
                                <option data-base="/GoiYTruong.aspx" data-param="">💡 Gợi ý trường phù hợp</option>
                            </select>
                            <div class="url-keyword-wrap d-none mt-2">
                                <div class="input-group input-group-sm">
                                    <input type="text" class="form-control url-keyword-input" placeholder="Dán slug hoặc mã ID..." />
                                    <button type="button" class="btn btn-success url-apply-btn fw-bold px-3">✓ Áp dụng</button>
                                </div>
                                <div class="form-hint url-preview-hint mt-1"></div>
                            </div>
                        </div>
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Đường dẫn</label>
                        <asp:TextBox ID="txtHomeTag3Url" runat="server" CssClass="form-control url-result-box" MaxLength="180" placeholder="Tự điền sau khi áp dụng..." />
                    </div>
                    <div class="home-tag-field">
                        <label class="form-label">Biểu tượng</label>
                        <asp:DropDownList ID="ddlHomeTag3Icon" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">Không có biểu tượng</asp:ListItem>
                            <asp:ListItem Value="bi bi-fire text-warning">🔥 Nổi bật / Hot</asp:ListItem>
                            <asp:ListItem Value="bi bi-mortarboard">🎓 Đại học</asp:ListItem>
                            <asp:ListItem Value="bi bi-award">🏅 Cao đẳng</asp:ListItem>
                            <asp:ListItem Value="bi bi-tools">🛠 Trường nghề</asp:ListItem>
                            <asp:ListItem Value="bi bi-building">🏛 Công lập</asp:ListItem>
                            <asp:ListItem Value="bi bi-house-heart">🏢 Tư thục</asp:ListItem>
                            <asp:ListItem Value="bi bi-globe2">🌐 Quốc tế</asp:ListItem>
                            <asp:ListItem Value="bi bi-laptop">💻 Công nghệ thông tin</asp:ListItem>
                            <asp:ListItem Value="bi bi-graph-up-arrow">📈 Kinh tế</asp:ListItem>
                            <asp:ListItem Value="bi bi-heart-pulse">🩺 Y dược</asp:ListItem>
                            <asp:ListItem Value="bi bi-bank">🏦 Tài chính - Ngân hàng</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <%-- Action bar nằm trong cột trái, dưới grid --%>
            <div class="tag-action-bar">
                <a href="/index.aspx" target="_blank" rel="noopener"
                   class="btn btn-outline-primary btn-sm px-3">
                    <i class="bi bi-eye me-1"></i>Xem trang chủ
                </a>
                <asp:Button ID="btnSaveHomeTags" runat="server" Text="💾 Lưu 3 nút gợi ý"
                    CssClass="btn btn-primary btn-sm px-4"
                    OnClick="btnSaveHomeTags_Click" />
            </div>
            </div>
            <%-- Help panel (cột phải) --%>
            <div class="tag-help-panel">
                <div class="tag-help-title">
                    <i class="bi bi-lightbulb-fill text-warning me-2"></i>Hướng dẫn cài đặt nút
                </div>
                <ol class="tag-help-steps">
                    <li>
                        <span class="tag-help-badge">Tên nút</span>
                        Nhập nhãn hiển thị trên nút, ví dụ: <em>Bách Khoa</em>, <em>Công Lập</em>…
                    </li>
                    <li>
                        <span class="tag-help-badge">Chọn chủ đề</span>
                        Chọn loại trang muốn liên kết (trang trường, tìm kiếm, điểm chuẩn…).
                    </li>
                    <li>
                        <span class="tag-help-badge">Dán slug / ID</span>
                        Nếu cần (trang chi tiết trường/bài viết/tin), dán slug hoặc mã ID vào ô rồi bấm <b>✓ Áp dụng</b>.
                    </li>
                    <li>
                        <span class="tag-help-badge">Biểu tượng</span>
                        Chọn icon phù hợp hoặc để trống nếu chỉ hiển thị chữ.
                    </li>
                    <li>
                        <span class="tag-help-badge">Lưu</span>
                        Bấm <b>💾 Lưu 3 nút gợi ý</b> để áp dụng thay đổi lên trang chủ.
                    </li>
                </ol>
                <div class="tag-help-note">
                    <i class="bi bi-info-circle me-1"></i>
                    Slug trường lấy từ ô <b>Slug</b> khi chỉnh sửa trường. Mã ID tin lấy từ modal sửa tin → xem <b>Mã tin</b>.
                </div>
            </div>
            </div>
        </div>
    </div>

    <!-- ══ SECTION 4: Cài đặt ngày tháng ════════════════════════════════════════ -->
    <div class="admin-card mb-4">
        <div class="admin-card-header">
            <div class="admin-card-title">
                <i class="bi bi-calendar-date text-primary"></i>
                Cài đặt ngày tháng
            </div>
        </div>
        <div class="admin-card-body">
            <div class="setting-group">

                <!-- Cho phép nhập ngày quá khứ -->
                <div class="setting-item">
                    <div class="setting-icon bg-warning-bg text-warning">
                        <i class="bi bi-clock-history"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Cho phép nhập ngày quá khứ</div>
                        <div class="setting-desc">
                            Bỏ qua kiểm tra “ngày không được trong quá khứ” khi nhập hạn nộp,
                            ngày bắt đầu trong các form quản lý. Khi bật, admin có thể nhập
                            bất kỳ ngày nào (kể cả ngày cũ, quá khứ).
                        </div>
                    </div>
                    <div class="setting-control">
                        <span class="setting-badge-preview">
                            Trạng thái: <strong id="lblAllowPastDatesPreview" runat="server" class="status-pill is-off">Đang tải...</strong>
                        </span>
                        <div class="form-check form-switch setting-switch-wrap">
                            <asp:CheckBox ID="chkAllowPastDates" runat="server" CssClass="setting-switch" />
                            <label class="setting-switch-label" for="<%= chkAllowPastDates.ClientID %>">Bật/Tắt</label>
                        </div>
                        <asp:Button ID="btnSaveAllowPastDates" runat="server" Text="Lưu"
                            CssClass="btn btn-warning btn-sm px-3"
                            OnClick="btnSaveAllowPastDates_Click" />
                    </div>
                </div>

            </div>
        </div>
    </div>

    <!-- ══ SECTION 3: Thông tin website (Giai đoạn 3 — Preview) ══════════════ -->
    <div class="admin-card mb-4">
        <div class="admin-card-header opacity-75">
            <div class="admin-card-title text-muted">
                <i class="bi bi-globe text-secondary"></i>
                Thông tin Website & Liên hệ
                <span class="badge badge-soon ms-1">Sắp ra mắt</span>
            </div>
        </div>
        <div class="admin-card-body bg-light bg-opacity-50" style="opacity:.6; pointer-events:none;">
            <div class="setting-group">
                <div class="setting-item">
                    <div class="setting-icon bg-secondary bg-opacity-10 text-secondary">
                        <i class="bi bi-type-h1"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Tiêu đề Website (Site Title)</div>
                        <div class="setting-desc">Hiển thị trên tab trình duyệt và thẻ meta SEO.</div>
                    </div>
                    <div class="setting-control">
                        <input type="text" class="form-control" style="width:200px" placeholder="Tra Cứu Tuyển Sinh" disabled />
                        <button class="btn btn-secondary btn-sm px-3" disabled>Khóa</button>
                    </div>
                </div>
                <div class="setting-item">
                    <div class="setting-icon bg-secondary bg-opacity-10 text-secondary">
                        <i class="bi bi-envelope"></i>
                    </div>
                    <div class="setting-body">
                        <div class="setting-label">Email hỗ trợ</div>
                        <div class="setting-desc">Hiển thị ở Footer và trang Liên hệ.</div>
                    </div>
                    <div class="setting-control">
                        <input type="text" class="form-control" style="width:200px" placeholder="hotro@tracuutuyensinh.vn" disabled />
                        <button class="btn btn-secondary btn-sm px-3" disabled>Khóa</button>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- ══ SECTION 4: Thao tác nhanh ════════════════════════════════════════ -->
    <div class="admin-card mb-4">
        <div class="admin-card-header">
            <div class="admin-card-title">
                <i class="bi bi-lightning-charge text-warning"></i>
                Thao tác nhanh
            </div>
        </div>
        <div class="admin-card-body">
            <div class="d-flex gap-3 flex-wrap quick-actions">
                <asp:Button ID="btnResetDefault" runat="server" Text="↺ Reset về mặc định"
                    CssClass="btn btn-outline-secondary"
                    OnClick="btnResetDefault_Click"
                    OnClientClick="return confirm('Bạn có chắc muốn khôi phục tất cả cài đặt về mặc định?');" />
                <asp:Button ID="btnClearCache" runat="server" Text="🗑 Xóa toàn bộ Cache"
                    CssClass="btn btn-outline-danger"
                    OnClick="btnClearCache_Click"
                    OnClientClick="return confirm('Xóa cache sẽ làm website tải chậm hơn trong vài giây đầu. Tiếp tục?');" />
            </div>
            <small class="text-muted d-block mt-3">
                <i class="bi bi-info-circle me-1"></i>
                <b>Reset về mặc định</b>: Năm=2025, Trường nổi bật=8, Bài viết=6, Tin=10, Cho phép ngày quá khứ=Tắt. —
                <b>Xóa Cache</b>: Buộc hệ thống tải lại toàn bộ dữ liệu từ Database.
            </small>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptContent" runat="server">
<script src="/Scripts/validation_data_input.js"></script>
<script>
// ── URL Builder ────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {

    // ── Restore dropdown từ URL đã lưu ────────────────────────────────────
    function restoreBuilderState(builder, savedUrl) {
        if (!savedUrl) return;
        var typeSel = builder.querySelector('.url-type-sel');
        var kwWrap  = builder.querySelector('.url-keyword-wrap');
        var kwInput = builder.querySelector('.url-keyword-input');
        var hintEl  = builder.querySelector('.url-preview-hint');

        for (var i = 0; i < typeSel.options.length; i++) {
            var opt      = typeSel.options[i];
            var base     = opt.getAttribute('data-base') || '';
            var param    = opt.getAttribute('data-param') || '';
            var slugType = opt.getAttribute('data-slug-type') || '';
            if (!base || slugType === 'divider') continue;

            if (param) {
                // Chủ đề có param → kiểm tra savedUrl bắt đầu bằng base?param=
                var prefix = base + (base.indexOf('?') >= 0 ? '&' : '?') + param + '=';
                if (savedUrl.indexOf(prefix) === 0) {
                    typeSel.selectedIndex = i;
                    var val = decodeURIComponent(savedUrl.slice(prefix.length));
                    kwWrap.classList.remove('d-none');
                    kwInput.placeholder = slugType === 'id' ? 'Dán mã ID vào đây...' : 'Dán slug vào đây...';
                    kwInput.value = val;
                    if (hintEl) hintEl.textContent = opt.getAttribute('data-hint') || '';
                    break;
                }
            } else {
                // URL tĩnh → so khớp exact hoặc prefix
                if (savedUrl === base || (base && savedUrl.indexOf(base) === 0)) {
                    typeSel.selectedIndex = i;
                    kwWrap.classList.add('d-none');
                    break;
                }
            }
        }
    }

    document.querySelectorAll('.url-builder').forEach(function (builder) {
        var targetId  = builder.getAttribute('data-target');
        var resultBox = document.getElementById(targetId);
        var typeSel   = builder.querySelector('.url-type-sel');
        var kwWrap    = builder.querySelector('.url-keyword-wrap');
        var kwInput   = builder.querySelector('.url-keyword-input');
        var applyBtn  = builder.querySelector('.url-apply-btn');
        var hintEl    = builder.querySelector('.url-preview-hint');

        // Restore state từ giá trị hiện tại của ô URL
        if (resultBox && resultBox.value) {
            restoreBuilderState(builder, resultBox.value.trim());
        }

        typeSel.addEventListener('change', function () {
            var opt      = typeSel.options[typeSel.selectedIndex];
            var base     = opt.getAttribute('data-base') || '';
            var param    = opt.getAttribute('data-param') || '';
            var slugType = opt.getAttribute('data-slug-type') || '';
            var hint     = opt.getAttribute('data-hint') || '';

            if (!base) { kwWrap.classList.add('d-none'); return; }

            if (param && slugType !== 'divider') {
                kwWrap.classList.remove('d-none');
                kwInput.placeholder = slugType === 'id' ? 'Dán mã ID vào đây...' : 'Dán slug vào đây...';
                kwInput.value = '';
                if (hintEl) hintEl.textContent = hint;
                kwInput.focus();
            } else {
                kwWrap.classList.add('d-none');
                applyToBox(resultBox, base);
            }
        });

        applyBtn.addEventListener('click', function () {
            var opt   = typeSel.options[typeSel.selectedIndex];
            var base  = opt.getAttribute('data-base') || '';
            var param = opt.getAttribute('data-param') || '';
            var val   = kwInput.value.trim();
            if (!base || !param) return;
            if (!val) { kwInput.focus(); kwInput.classList.add('is-invalid'); return; }
            kwInput.classList.remove('is-invalid');
            var url = base + (base.indexOf('?') >= 0 ? '&' : '?') + param + '=' + encodeURIComponent(val);
            applyToBox(resultBox, url);
            if (hintEl) hintEl.textContent = '✓ ' + url;
        });

        kwInput.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') { e.preventDefault(); applyBtn.click(); }
        });
        kwInput.addEventListener('input', function () { kwInput.classList.remove('is-invalid'); });
    });

    function applyToBox(box, url) {
        if (!box) return;
        box.value = url;
        box.style.transition = 'background .3s';
        box.style.background = '#dcfce7';
        setTimeout(function () { box.style.background = ''; }, 1000);
    }
});
</script>
<script>
window.AppConfig = window.AppConfig || {};
document.addEventListener('DOMContentLoaded', function () {
    var allowPastCheckbox = document.getElementById('<%= chkAllowPastDates.ClientID %>');
    var allowPastPreview = document.getElementById('<%= lblAllowPastDatesPreview.ClientID %>');
    var syncAllowPastState = function () {
        if (!allowPastCheckbox || !allowPastPreview) return;
        var isOn = allowPastCheckbox.checked;
        window.AppConfig.AllowPastDates = isOn;
        allowPastPreview.textContent = isOn ? 'Đã bật' : 'Đã tắt';
        allowPastPreview.className = isOn ? 'status-pill is-on' : 'status-pill is-off';
    };
    syncAllowPastState();
    if (allowPastCheckbox) {
        allowPastCheckbox.addEventListener('change', syncAllowPastState);
    }
});
</script>
<script>
document.addEventListener('DOMContentLoaded', function () {

    // ── Năm tuyển sinh (4 chữ số, 2020..2099) ────────────────────────────
    AdminValidator.init('form', {
        '<%= txtNamTuyenSinh.ClientID %>': [
            AdminValidator.rules.pattern(/^$|^20[2-9][0-9]$/, 'Năm phải có 4 chữ số trong khoảng 2020–2099.')
        ]
    }, { triggerButtonId: '<%= btnSaveNamTuyenSinh.ClientID %>' });

    // ── Số trường nổi bật (1..20) ─────────────────────────────────────────
    AdminValidator.init('form', {
        '<%= txtSoTruongNoiBat.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập số trường nổi bật.'),
            AdminValidator.rules.numRange(1, 20, 'Số trường nổi bật phải từ 1 đến 20.')
        ]
    }, { triggerButtonId: '<%= btnSaveSoTruong.ClientID %>' });

    // ── Số bài viết mới nhất (1..15) ─────────────────────────────────────
    AdminValidator.init('form', {
        '<%= txtSoBaiViet.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập số bài viết.'),
            AdminValidator.rules.numRange(1, 15, 'Số bài viết mới phải từ 1 đến 15.')
        ]
    }, { triggerButtonId: '<%= btnSaveBaiViet.ClientID %>' });

    // ── 3 nút gợi ý trang chủ ─────────────────────────────────────────────
    AdminValidator.init('form', {
        '<%= txtHomeTag1Text.ClientID %>': [AdminValidator.rules.required('Vui lòng nhập nhãn nút 1.')],
        '<%= txtHomeTag1Url.ClientID %>': [AdminValidator.rules.pattern(/^\/[^\s]*$/, 'Đường dẫn nút 1 phải là URL nội bộ bắt đầu bằng /.')],
        '<%= txtHomeTag2Text.ClientID %>': [AdminValidator.rules.required('Vui lòng nhập nhãn nút 2.')],
        '<%= txtHomeTag2Url.ClientID %>': [AdminValidator.rules.pattern(/^\/[^\s]*$/, 'Đường dẫn nút 2 phải là URL nội bộ bắt đầu bằng /.')],
        '<%= txtHomeTag3Text.ClientID %>': [AdminValidator.rules.required('Vui lòng nhập nhãn nút 3.')],
        '<%= txtHomeTag3Url.ClientID %>': [AdminValidator.rules.pattern(/^\/[^\s]*$/, 'Đường dẫn nút 3 phải là URL nội bộ bắt đầu bằng /.')]
    }, { triggerButtonId: '<%= btnSaveHomeTags.ClientID %>' });

    // ── Số tin tuyển sinh (1..20) ─────────────────────────────────────────
    AdminValidator.init('form', {
        '<%= txtSoTin.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập số tin tuyển sinh.'),
            AdminValidator.rules.numRange(1, 20, 'Số tin tuyển sinh phải từ 1 đến 20.')
        ]
    }, { triggerButtonId: '<%= btnSaveSoTin.ClientID %>' });

});
</script>
</asp:Content>
