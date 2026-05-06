 <%@ Page Title="Chi tiết trường" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="ChiTietTruong.aspx.cs" Inherits="ChiTietTruong" %>
<%-- KHÔNG dùng OutputCache: trang phụ thuộc data động (tin tuyển sinh, đánh giá, điểm chuẩn).
     Khi admin/trường học CRUD tin → MaTin mới (IDENTITY không reuse), HTML cache cũ sẽ render
     link ?id=<MaTin_cũ> dẫn đến trang "Không tìm thấy". Cache tầng BLL/SP đã đủ giảm tải DB. --%>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="<%: ResolveUrl("~/lib/chartjs/chart.umd.min.js") %>"></script>
    <%-- school-cover, sticky-tabs, premium-card, info-list-item styles are in Content/Client.css --%>
    <style>
        /* Floating Tu Van Widget */
        .floating-tuvan-widget {
            position: fixed;
            bottom: 2rem;
            right: 2rem;
            z-index: 1050;
            display: flex;
            flex-direction: column;
            align-items: flex-end;
        }
        .btn-tuvan-toggle {
            background: linear-gradient(135deg, #0d6efd, #6f42c1);
            color: white;
            border: none;
            border-radius: 50rem;
            padding: 0.75rem 1.5rem;
            font-weight: 600;
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
        }
        .btn-tuvan-toggle:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.25);
        }
        .tuvan-panel {
            width: 350px;
            margin-bottom: 1rem;
            display: none;
            transform-origin: bottom right;
            animation: scaleUp 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275) forwards;
            box-shadow: 0 10px 30px rgba(0,0,0,0.15);
        }
        .floating-tuvan-widget.open .tuvan-panel {
            display: block;
        }
        .floating-tuvan-widget.open .btn-tuvan-toggle {
            display: none;
        }
        @keyframes scaleUp {
            from { opacity: 0; transform: scale(0.9); }
            to { opacity: 1; transform: scale(1); }
        }
        @media (max-width: 576px) {
            .floating-tuvan-widget {
                bottom: 1rem;
                right: 1rem;
            }
            .tuvan-panel {
                width: calc(100vw - 2rem);
            }
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:Literal ID="litNotFound" runat="server" />

<asp:Panel ID="pnlTruong" runat="server">

<%-- BIA & LIGHTBOX MODAL --%>
<div class="school-cover mb-4">
    <asp:Image ID="imgBia" runat="server"
        AlternateText="Ảnh bìa trường"
        data-bs-toggle="modal" data-bs-target="#imageLightboxModal"
        onclick="document.getElementById('lightboxImage').src = this.src" />
    <div class="school-header-info">
        <asp:Image ID="imgLogo" runat="server" CssClass="school-logo"
            data-bs-toggle="modal" data-bs-target="#imageLightboxModal"
            onclick="document.getElementById('lightboxImage').src = this.src" />
        <div>
            <h1 class="mb-1 fw-bold fs-3" style="font-family: 'Outfit', sans-serif;"><asp:Literal ID="litTenTruong" runat="server" /></h1>
            <p class="mb-0 opacity-75 fs-6">
                <i class="bi bi-geo-alt-fill text-danger me-1"></i><asp:Literal ID="litDiaChi" runat="server" />
            </p>
        </div>
    </div>
</div>

<!-- Lightbox Modal -->
<div class="modal fade" id="imageLightboxModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-xl">
        <div class="modal-content bg-transparent border-0">
            <div class="modal-body text-center p-0 position-relative">
                <button type="button" class="btn-close btn-close-white position-absolute top-0 end-0 m-3" data-bs-dismiss="modal" aria-label="Close" style="z-index: 10;"></button>
                <img id="lightboxImage" src="" alt="View Full Screen" class="img-fluid rounded shadow-lg" />
            </div>
        </div>
    </div>
</div>

<%-- THÔNG TIN + NAV TABS --%>
<div class="row g-4">
    <%-- LEFT: Chi tiết --%>
    <div class="col-lg-8">
<div class="sticky-tabs">
            <ul class="nav nav-tabs border-0" id="tabTruong">
                <li class="nav-item"><a class="nav-link active" data-bs-toggle="tab" href="#tab-gioithieu">Giới thiệu</a></li>
                <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-nganh">Ngành đào tạo</a></li>
                <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-tuyensinh">Tuyển sinh</a></li>
                <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-baiviet">Câu Chuyện & Tin Tức</a></li>
                <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-danhgia">Đánh giá</a></li>
            </ul>
        </div>

        <div class="tab-content">
            <%-- Tab giới thiệu --%>
            <div class="tab-pane fade show active editorial-content" id="tab-gioithieu" style="padding: 1.5rem 0;">
                <asp:Literal ID="litMoTa" runat="server" />
            </div>

            <%-- Tab ngành đào tạo (biểu đồ + bảng điểm chuẩn) --%>
            <div class="tab-pane fade" id="tab-nganh">
                <%-- Biểu đồ xu hướng điểm chuẩn --%>
                <div class="card border-0 shadow-sm mb-3 overflow-hidden">
                    <div style="height:4px;background:linear-gradient(90deg,#4e79a7,#f28e2b,#e15759,#76b7b2);"></div>
                    <div class="card-body p-3">
                        <div class="d-flex justify-content-between align-items-center mb-3">
                            <h6 class="mb-0 fw-semibold"><i class="bi bi-graph-up me-1 text-primary"></i>Xu hướng điểm chuẩn qua các năm</h6>
                            <span class="text-muted small" id="chartStatus"><i class="bi bi-hourglass-split me-1"></i>Đang tải...</span>
                        </div>
                        <div style="position:relative;height:350px;">
                            <canvas id="chartDiemChuan"></canvas>
                        </div>
                    </div>
                </div>

                <%-- Bảng điểm chuẩn --%>
                <asp:GridView ID="gvDiemChuan" runat="server"
                    CssClass="table table-hover table-sm align-middle"
                    AutoGenerateColumns="false" GridLines="None"
                    EnableViewState="false"
                    EmptyDataText="Chưa có thông tin điểm chuẩn.">
                    <Columns>
                        <asp:BoundField DataField="NamTuyenSinh"   HeaderText="Năm"          />
                        <asp:BoundField DataField="TenChuyenNganh" HeaderText="Ngành"        HtmlEncode="true" />
                        <asp:BoundField DataField="TenPhuongThuc"  HeaderText="Phương thức"  HtmlEncode="true" />
                        <asp:BoundField DataField="ToHopMonHoc"    HeaderText="Tổ hợp"       HtmlEncode="true" />
                        <asp:BoundField DataField="DiemChuanNamTruoc" HeaderText="Điểm chuẩn" DataFormatString="{0:F2}" />
                        <asp:BoundField DataField="ChiTieu"        HeaderText="Chỉ tiêu"     />
                    </Columns>
                </asp:GridView>
            </div>

            <%-- Tab tin tuyển sinh --%>
            <div class="tab-pane fade" id="tab-tuyensinh" style="padding-top: 1.5rem;">
                <asp:Repeater ID="rptTinTuyenSinh" runat="server">
                    <HeaderTemplate><div class="row g-4"></HeaderTemplate>
                    <ItemTemplate>
                        <div class="col-12">
                            <a href='ChiTietTinTuyenSinh.aspx?id=<%# Eval("MaTin") %>'
                               class="premium-card text-decoration-none text-body d-block p-4">
                                <div class="d-flex justify-content-between align-items-start flex-column flex-md-row gap-3">
                                    <div class="flex-grow-1">
                                        <h5 class="fw-bold mb-3 text-primary" style="font-family: 'Outfit', sans-serif;">
                                            <i class="bi bi-mortarboard-fill me-2 text-warning"></i><%# Eval("TieuDeHienThi") %>
                                        </h5>
                                        <div class="d-flex flex-wrap gap-2 mb-3">
                                            <span class="badge rounded-pill bg-light text-dark border px-3 py-2">
                                                <i class="bi bi-people me-1"></i>Chỉ tiêu: <%# (Eval("ChiTieu") != DBNull.Value && Eval("ChiTieu") != null) ? Eval("ChiTieu") : "N/A" %>
                                            </span>
                                            <span class="badge rounded-pill bg-danger-subtle text-danger border border-danger-subtle px-3 py-2 fw-bold">
                                                <i class="bi bi-bullseye me-1"></i>Điểm chuẩn: <%# (Eval("DiemChuanNamTruoc") != DBNull.Value && Eval("DiemChuanNamTruoc") != null) ? FormatDecimal(Eval("DiemChuanNamTruoc")) : "N/A" %>
                                            </span>
                                            <span class="badge rounded-pill bg-light text-dark border px-3 py-2">
                                                <%-- Sprint 1: HocPhi là NVARCHAR text tự do --%>
                                                <i class="bi bi-cash me-1"></i>Học phí: <%# Eval("HocPhi") != null && Eval("HocPhi") != DBNull.Value && !string.IsNullOrWhiteSpace(Eval("HocPhi").ToString()) ? System.Web.HttpUtility.HtmlEncode(Eval("HocPhi").ToString()) : "N/A" %>
                                            </span>
                                            <%# (Eval("ToHopMonHoc") != DBNull.Value && Eval("ToHopMonHoc") != null) ? "<span class='badge rounded-pill bg-light text-dark border px-3 py-2'><i class='bi bi-book me-1'></i>" + Eval("ToHopMonHoc") + "</span>" : "" %>
                                        </div>
                                        <div class="text-muted small">
                                            <span class="me-3"><i class="bi bi-clock me-1"></i>Đăng ngày: <%# FormatDate(Eval("NgayDang")) %></span>
                                            <span><i class="bi bi-eye me-1"></i><%# Eval("LuotXem") %> lượt đọc</span>
                                        </div>
                                    </div>
                                    <div class="text-md-end d-flex flex-row flex-md-column align-items-center align-items-md-end justify-content-between w-100 w-md-auto mt-3 mt-md-0">
                                        <%# GetHanNopBadge(Eval("HanNop")) %>
                                        <span class="fw-semibold mt-auto btn btn-sm btn-primary rounded-pill px-4">Xem chi tiết <i class="bi bi-arrow-right"></i></span>
                                    </div>
                                </div>
                            </a>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoTinTuyenSinh" runat="server" Visible="false" CssClass="text-center py-5 text-muted bg-light rounded-4 border border-dashed">
                    <i class="bi bi-megaphone fs-1 text-secondary opacity-50"></i>
                    <p class="mt-3 mb-0 fw-medium">Chưa có tin tuyển sinh nào được công bố.</p>
                </asp:Panel>
            </div>

            <%-- Tab bài viết --%>
            <div class="tab-pane fade" id="tab-baiviet" style="padding-top: 1.5rem;">
                <asp:Repeater ID="rptBaiVietTruong" runat="server">
                    <HeaderTemplate><div class="row g-4"></HeaderTemplate>
                    <ItemTemplate>
                        <div class="col-md-6">
                            <div class="premium-card h-100 d-flex flex-column">
                                <div class="card-img-wrap" style="padding-top: 55%; position: relative; overflow: hidden;">
                                    <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                                        <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("AnhChinh")))
                                                       ? "/Resources/Images/no-image.png"
                                                       : Eval("AnhChinh") %>'
                                             style="position: absolute; top:0; left:0; width:100%; height:100%; object-fit:cover; transition: transform 0.5s;"
                                             onmouseover="this.style.transform='scale(1.05)'" onmouseout="this.style.transform='scale(1)'"
                                             alt='<%# Eval("TieuDe") %>' loading="lazy" />
                                    </a>
                                </div>
                                <div class="card-body p-4 d-flex flex-column flex-grow-1">
                                    <h5 class="fw-bold mb-3 lh-base" style="font-family: 'Outfit', sans-serif;">
                                        <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'
                                           class="text-decoration-none text-body hover-primary"><%# Eval("TieuDe") %></a>
                                    </h5>
                                    <div class="mt-auto d-flex justify-content-between align-items-center border-top pt-3">
                                        <small class="text-muted"><i class="bi bi-clock me-1"></i><%# FormatDate(Eval("NgayDang")) %></small>
                                        <small class="text-muted"><i class="bi bi-eye me-1"></i><%# Eval("LuotXem") %></small>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoBaiViet" runat="server" Visible="false" CssClass="text-center py-5 text-muted bg-light rounded-4 border border-dashed">
                    <i class="bi bi-newspaper fs-1 text-secondary opacity-50"></i>
                    <p class="mt-3 mb-0 fw-medium">Chưa có bài viết nào về trường này.</p>
                </asp:Panel>
            </div>

            <%-- Tab đánh giá --%>
            <div class="tab-pane fade" id="tab-danhgia">
                <asp:Literal ID="litDiemTB" runat="server" />
                <asp:Repeater ID="rptDanhGia" runat="server">
                    <HeaderTemplate><div class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <div class="list-group-item px-0">
                            <div class="d-flex justify-content-between">
                                <strong><%# Eval("Email") %></strong>
                                <span class="text-warning"><%# FormatStars(Eval("DiemDanhGia")) %></span>
                            </div>
                            <p class="mb-0 text-muted small"><%# Eval("NoiDung") %></p>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>

                <%-- Form đánh giá (chỉ hiện khi đăng nhập) --%>
                <asp:Panel ID="pnlFormDanhGia" runat="server" CssClass="mt-4" Visible="false">
                    <h6 class="fw-bold">Viết đánh giá của bạn</h6>
                    <div class="mb-2">
                        <label class="form-label">Điểm đánh giá</label>
                        <asp:DropDownList ID="ddlDiem" runat="server" CssClass="form-select w-auto">
                            <asp:ListItem Value="5">⭐⭐⭐⭐⭐ Xuất sắc</asp:ListItem>
                            <asp:ListItem Value="4">⭐⭐⭐⭐ Tốt</asp:ListItem>
                            <asp:ListItem Value="3">⭐⭐⭐ Bình thường</asp:ListItem>
                            <asp:ListItem Value="2">⭐⭐ Không tốt</asp:ListItem>
                            <asp:ListItem Value="1">⭐ Rất kém</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="mb-2">
                        <asp:TextBox ID="txtDanhGia" runat="server" CssClass="form-control"
                            TextMode="MultiLine" Rows="3" placeholder="Nhận xét của bạn..." />
                    </div>
                    <asp:Button ID="btnGuiDanhGia" runat="server" Text="Gửi đánh giá"
                        CssClass="btn btn-sm btn-primary" OnClick="btnGuiDanhGia_Click" />
                </asp:Panel>
            </div>
        </div>
    </div>

    <%-- RIGHT: Sidebar --%>
    <div class="col-lg-4">
        <div class="premium-card mb-4">
            <div class="card-body p-4">
                <h5 class="fw-bold mb-4" style="font-family: 'Outfit', sans-serif;"><i class="bi bi-info-circle-fill text-primary me-2"></i>Tổng quan</h5>
                <div class="info-list-item">
                    <div class="info-icon"><i class="bi bi-building"></i></div>
                    <div>
                        <div class="text-muted small">Loại trường</div>
                        <div class="fw-semibold"><asp:Literal ID="litLoai" runat="server" /></div>
                    </div>
                </div>
                <div class="info-list-item">
                    <div class="info-icon bg-primary-subtle text-primary"><i class="bi bi-mortarboard"></i></div>
                    <div>
                        <div class="text-muted small">Cấp bậc đào tạo</div>
                        <div class="fw-semibold"><asp:Literal ID="litCapBacDaoTao" runat="server" /></div>
                    </div>
                </div>
                <div class="info-list-item">
                    <div class="info-icon bg-success-subtle text-success"><i class="bi bi-geo-alt"></i></div>
                    <div>
                        <div class="text-muted small">Khu vực</div>
                        <div class="fw-semibold"><asp:Literal ID="litVung" runat="server" /></div>
                    </div>
                </div>
                <div class="info-list-item">
                    <div class="info-icon bg-warning-subtle text-warning"><i class="bi bi-telephone"></i></div>
                    <div>
                        <div class="text-muted small">Điện thoại</div>
                        <div class="fw-semibold"><asp:Literal ID="litSdt" runat="server" /></div>
                    </div>
                </div>
                <div class="info-list-item">
                    <div class="info-icon bg-info-subtle text-info"><i class="bi bi-globe"></i></div>
                    <div>
                        <div class="text-muted small">Website</div>
                        <div class="fw-semibold"><asp:HyperLink ID="hlkWeb" runat="server" Target="_blank" CssClass="text-decoration-none" /></div>
                    </div>
                </div>
                <div class="info-list-item">
                    <div class="info-icon bg-danger-subtle text-danger"><i class="bi bi-star-fill"></i></div>
                    <div>
                        <div class="text-muted small">Đánh giá</div>
                        <div class="fw-semibold"><asp:Literal ID="litDanhGiaSidebar" runat="server" /></div>
                    </div>
                </div>
                <div class="info-list-item">
                    <div class="info-icon bg-secondary-subtle text-secondary"><i class="bi bi-patch-check-fill"></i></div>
                    <div>
                        <div class="text-muted small">Kiểm định</div>
                        <div class="fw-semibold"><asp:Literal ID="litKiemDinh" runat="server" /></div>
                    </div>
                </div>
            </div>
        </div>

        <%-- Yêu thích --%>
        <asp:Panel ID="pnlWishlist" runat="server" Visible="false" CssClass="mb-4">
            <asp:Button ID="btnWishList" runat="server"
                CssClass="btn btn-outline-danger w-100 rounded-pill fw-bold py-2 shadow-sm"
                OnClick="btnWishList_Click" />
        </asp:Panel>

    </div>
</div>

<%-- Form tư vấn nổi (Floating Chat Widget) --%>
<div class="floating-tuvan-widget" id="floatingTuVan">
    <button type="button" class="btn-tuvan-toggle" id="btnTuVanToggle" onclick="document.getElementById('floatingTuVan').classList.add('open')">
        <i class="bi bi-chat-dots-fill me-2 fs-5"></i><span>Đăng ký tư vấn</span>
    </button>
    <div class="tuvan-panel premium-card">
        <div class="card-body p-4" style="background: linear-gradient(135deg, rgba(13, 110, 253, 0.05), rgba(111, 66, 193, 0.05)); position: relative;">
            <button type="button" class="btn-close position-absolute top-0 end-0 m-3" aria-label="Close" onclick="document.getElementById('floatingTuVan').classList.remove('open')"></button>
            <h5 class="fw-bold mb-3 pe-4" style="font-family: 'Outfit', sans-serif;"><i class="bi bi-chat-dots-fill me-2 text-primary"></i>Đăng ký tư vấn</h5>
            <p class="text-muted small mb-4">Để lại thông tin, nhà trường sẽ liên hệ hỗ trợ bạn sớm nhất.</p>
            <div class="mb-3">
                <asp:TextBox ID="txtTVHoTen" runat="server" CssClass="form-control rounded-3"
                    placeholder="Họ tên của bạn" />
            </div>
            <div class="mb-3">
                <asp:TextBox ID="txtTVEmail" runat="server" CssClass="form-control rounded-3"
                    placeholder="Địa chỉ Email" TextMode="Email" />
            </div>
            <div class="mb-3">
                <asp:TextBox ID="txtTVSdt" runat="server" CssClass="form-control rounded-3"
                    placeholder="Số điện thoại Zalo" />
            </div>
            <div class="mb-4">
                <asp:TextBox ID="txtTVNoiDung" runat="server" CssClass="form-control rounded-3"
                    TextMode="MultiLine" Rows="3" placeholder="Bạn muốn hỏi về ngành nào?..." />
            </div>
            <asp:Button ID="btnGuiTuVan" runat="server" Text="Gửi yêu cầu ngay"
                CssClass="btn btn-primary w-100 rounded-pill fw-bold shadow-sm py-2" OnClick="btnGuiTuVan_Click"
                CausesValidation="false" />
            <div class="mt-3 text-center" id="divTVThongBao"><asp:Literal ID="litTVThongBao" runat="server" /></div>
        </div>
    </div>
</div>

</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// Vẽ biểu đồ xu hướng điểm chuẩn khi người dùng click tab "Ngành đào tạo"
(function() {
    var maTruong = '<%= MaTruongHienTai %>';
    if (!maTruong || maTruong === '0') return;

    var chartInited = false;
    var tabBtn = document.querySelector('a[href="#tab-nganh"]');
    if (!tabBtn) return;

    // Detect dark mode từ Bootstrap theme
    function isDark() {
        return document.documentElement.getAttribute('data-bs-theme') === 'dark';
    }
    function getColors() {
        var dark = isDark();
        return {
            gridColor: dark ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.06)',
            textColor: dark ? '#adb5bd' : '#6c757d',
            titleColor: dark ? '#e9ecef' : '#212529',
            bgColor: dark ? '#212529' : '#ffffff',
            tooltipBg: dark ? '#343a40' : '#ffffff',
            tooltipText: dark ? '#f8f9fa' : '#212529',
            tooltipBorder: dark ? '#495057' : '#dee2e6'
        };
    }

    tabBtn.addEventListener('shown.bs.tab', function() {
        if (chartInited) return;
        chartInited = true;

        var status = document.getElementById('chartStatus');
        fetch('/Handlers/DiemChuanChart.ashx?maTruong=' + maTruong)
            .then(function(r) { return r.json(); })
            .then(function(data) {
                if (status) status.textContent = '';
                if (!data.labels || data.labels.length === 0) {
                    if (status) status.textContent = 'Chưa có dữ liệu lịch sử điểm chuẩn';
                    return;
                }

                // Enhance datasets: thêm pointRadius, borderWidth, pointHoverRadius
                if (data.datasets) {
                    data.datasets.forEach(function(ds) {
                        ds.borderWidth = 2.5;
                        ds.pointRadius = 4;
                        ds.pointHoverRadius = 7;
                        ds.pointBackgroundColor = ds.borderColor;
                        ds.pointBorderColor = '#fff';
                        ds.pointBorderWidth = 2;
                        ds.fill = true;
                    });
                }

                // Tính toán min/max y-axis tự động
                var allVals = [];
                data.datasets.forEach(function(ds) {
                    ds.data.forEach(function(v) { if (v !== null) allVals.push(v); });
                });
                var minVal = Math.max(0, Math.floor(Math.min.apply(null, allVals) - 2));
                var maxVal = Math.ceil(Math.max.apply(null, allVals) + 2);

                var colors = getColors();
                var ctx = document.getElementById('chartDiemChuan').getContext('2d');
                new Chart(ctx, {
                    type: 'line',
                    data: data,
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        animation: {
                            duration: 1200,
                            easing: 'easeInOutQuart',
                            delay: function(context) {
                                return context.dataIndex * 100;
                            }
                        },
                        interaction: { mode: 'index', intersect: false },
                        plugins: {
                            legend: {
                                position: 'bottom',
                                labels: {
                                    boxWidth: 14,
                                    boxHeight: 14,
                                    borderRadius: 3,
                                    useBorderRadius: true,
                                    font: { size: 12, family: "'Be Vietnam Pro', sans-serif" },
                                    color: colors.textColor,
                                    padding: 16
                                }
                            },
                            tooltip: {
                                backgroundColor: colors.tooltipBg,
                                titleColor: colors.tooltipText,
                                bodyColor: colors.tooltipText,
                                borderColor: colors.tooltipBorder,
                                borderWidth: 1,
                                cornerRadius: 8,
                                padding: 12,
                                titleFont: { size: 13, weight: 'bold', family: "'Be Vietnam Pro', sans-serif" },
                                bodyFont: { size: 12, family: "'Be Vietnam Pro', sans-serif" },
                                displayColors: true,
                                boxWidth: 10,
                                boxHeight: 10,
                                callbacks: {
                                    title: function(items) {
                                        return 'Năm tuyển sinh ' + items[0].label;
                                    },
                                    label: function(ctx) {
                                        var val = ctx.raw;
                                        return ' ' + ctx.dataset.label + ': ' + (val !== null ? val + ' điểm' : 'N/A');
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                title: {
                                    display: true,
                                    text: 'Điểm chuẩn',
                                    font: { size: 12, weight: '600', family: "'Be Vietnam Pro', sans-serif" },
                                    color: colors.textColor
                                },
                                min: minVal,
                                max: maxVal,
                                grid: { color: colors.gridColor },
                                ticks: {
                                    color: colors.textColor,
                                    font: { size: 11 },
                                    stepSize: maxVal <= 10 ? 1 : (maxVal <= 20 ? 2 : 5)
                                }
                            },
                            x: {
                                title: {
                                    display: true,
                                    text: 'Năm tuyển sinh',
                                    font: { size: 12, weight: '600', family: "'Be Vietnam Pro', sans-serif" },
                                    color: colors.textColor
                                },
                                grid: { display: false },
                                ticks: {
                                    color: colors.textColor,
                                    font: { size: 11 }
                                }
                            }
                        }
                    }
                });

                // Cập nhật status
                if (status) {
                    status.innerHTML = '<span class="badge bg-success-subtle text-success"><i class="bi bi-check-circle me-1"></i>' +
                        data.datasets.length + ' ngành · ' + data.labels.length + ' năm</span>';
                }
            })
            .catch(function() {
                if (status) status.textContent = 'Không tải được dữ liệu biểu đồ';
            });
    });

    // Auto open TuVan form if there is a message (e.g., after submit)
    var tvMsg = document.getElementById('divTVThongBao');
    if (tvMsg && tvMsg.innerText.trim() !== '') {
        document.getElementById('floatingTuVan').classList.add('open');
    }
})();
</script>
</asp:Content>
