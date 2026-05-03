<%@ Page Title="Chi tiết trường" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="ChiTietTruong.aspx.cs" Inherits="ChiTietTruong" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.3/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:Literal ID="litNotFound" runat="server" />

<asp:Panel ID="pnlTruong" runat="server">

<%-- BIA --%>
<div class="position-relative mb-4" style="height:220px;overflow:hidden;border-radius:12px;">
    <asp:Image ID="imgBia" runat="server" CssClass="w-100 h-100"
        style="object-fit:cover;cursor:pointer;" AlternateText="Ảnh bìa trường"
        onclick="if(this.src)window.open(this.src,'_blank')" title="Click để xem ảnh đầy đủ" />
    <div class="position-absolute bottom-0 start-0 p-3 text-white"
         style="background:linear-gradient(transparent,rgba(0,0,0,.65));width:100%;">
        <div class="d-flex align-items-center gap-3">
            <asp:Image ID="imgLogo" runat="server"
                style="width:80px;height:80px;object-fit:contain;background:#fff;border-radius:8px;padding:4px;cursor:pointer;"
                onclick="if(this.src)window.open(this.src,'_blank')" title="Click để xem logo đầy đủ" />
            <div>
                <h2 class="mb-0 fw-bold"><asp:Literal ID="litTenTruong" runat="server" /></h2>
                <p class="mb-0 opacity-75">
                    <i class="bi bi-geo-alt me-1"></i><asp:Literal ID="litDiaChi" runat="server" />
                </p>
            </div>
        </div>
    </div>
</div>

<%-- THÔNG TIN + NAV TABS --%>
<div class="row g-4">
    <%-- LEFT: Chi tiết --%>
    <div class="col-lg-8">
        <ul class="nav nav-tabs mb-3" id="tabTruong">
            <li class="nav-item"><a class="nav-link active" data-bs-toggle="tab" href="#tab-gioithieu">Giới thiệu</a></li>
            <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-nganh">Ngành đào tạo</a></li>
            <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-diem">Điểm chuẩn</a></li>
            <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-tuyensinh">Tin tuyển sinh</a></li>
            <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-baiviet">Bài viết</a></li>
            <li class="nav-item"><a class="nav-link" data-bs-toggle="tab" href="#tab-danhgia">Đánh giá</a></li>
        </ul>

        <div class="tab-content">
            <%-- Tab giới thiệu --%>
            <div class="tab-pane fade show active" id="tab-gioithieu">
                <asp:Literal ID="litMoTa" runat="server" />
            </div>

            <%-- Tab ngành đào tạo --%>
            <div class="tab-pane fade" id="tab-nganh">
                <asp:GridView ID="gvNganh" runat="server"
                    CssClass="table table-hover table-sm align-middle"
                    AutoGenerateColumns="false" GridLines="None"
                    EmptyDataText="Chưa có thông tin ngành.">
                    <Columns>
                        <asp:BoundField DataField="TenDanhMuc"    HeaderText="Lĩnh vực"     HtmlEncode="true" />
                        <asp:BoundField DataField="TenChuyenNganh" HeaderText="Tên ngành"   HtmlEncode="true" />
                        <asp:BoundField DataField="TenCapBac"     HeaderText="Bậc đào tạo"  HtmlEncode="true" />
                        <asp:BoundField DataField="ChiTieu"       HeaderText="Chỉ tiêu"     />
                    </Columns>
                </asp:GridView>
            </div>

            <%-- Tab điểm chuẩn --%>
            <div class="tab-pane fade" id="tab-diem">
                <%-- Biểu đồ xu hướng --%>
                <div class="card border-0 bg-light mb-3 p-3">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                        <h6 class="mb-0 fw-semibold"><i class="bi bi-graph-up me-1 text-primary"></i>Xu hướng điểm chuẩn qua các năm</h6>
                        <span class="text-muted small" id="chartStatus">Đang tải...</span>
                    </div>
                    <div style="position:relative;height:300px;">
                        <canvas id="chartDiemChuan"></canvas>
                    </div>
                </div>
                <%-- Bảng điểm --%>
                <asp:GridView ID="gvDiemChuan" runat="server"
                    CssClass="table table-hover table-sm align-middle"
                    AutoGenerateColumns="false" GridLines="None"
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
            <div class="tab-pane fade" id="tab-tuyensinh">
                <asp:Repeater ID="rptTinTuyenSinh" runat="server">
                    <HeaderTemplate><div class="row g-3"></HeaderTemplate>
                    <ItemTemplate>
                        <div class="col-12">
                            <a href='ChiTietTinTuyenSinh.aspx?id=<%# Eval("MaTin") %>'
                               class="card border-0 shadow-sm text-decoration-none text-body card-hover d-block">
                                <div class="card-body py-3">
                                    <div class="d-flex justify-content-between align-items-start">
                                        <div>
                                            <h6 class="fw-bold mb-1 text-primary">
                                                <i class="bi bi-mortarboard me-1"></i><%# Eval("TieuDeHienThi") %>
                                            </h6>
                                            <div class="d-flex flex-wrap gap-2 mb-2">
                                                <span class="badge bg-light text-dark border">
                                                    <i class="bi bi-people me-1"></i>Chỉ tiêu: <%# Eval("ChiTieu") ?? "N/A" %>
                                                </span>
                                                <span class="badge bg-light text-danger border fw-bold">
                                                    <i class="bi bi-bullseye me-1"></i>Điểm chuẩn: <%# Eval("DiemChuanNamTruoc") != null ? string.Format("{0:F2}", Eval("DiemChuanNamTruoc")) : "N/A" %>
                                                </span>
                                                <span class="badge bg-light text-dark border">
                                                    <i class="bi bi-cash me-1"></i>Học phí: <%# Eval("HocPhi") != null ? string.Format("{0:N0} VNĐ", Eval("HocPhi")) : "N/A" %>
                                                </span>
                                                <%# Eval("ToHopMonHoc") != null ? "<span class='badge bg-light text-dark border'><i class='bi bi-book me-1'></i>" + Eval("ToHopMonHoc") + "</span>" : "" %>
                                            </div>
                                            <div class="text-muted small">
                                                <span class="me-3"><i class="bi bi-calendar me-1"></i><%# ((DateTime)Eval("NgayDang")).ToString("dd/MM/yyyy") %></span>
                                                <span><i class="bi bi-eye me-1"></i><%# Eval("LuotXem") %> lượt xem</span>
                                            </div>
                                        </div>
                                        <div class="text-end ms-3">
                                            <%# Eval("HanNop") != null && ((DateTime)Eval("HanNop")) > DateTime.Now
                                                ? "<span class='badge bg-success'><i class='bi bi-clock me-1'></i>Còn " + ((DateTime)Eval("HanNop") - DateTime.Now).Days + " ngày</span>"
                                                : (Eval("HanNop") != null ? "<span class='badge bg-secondary'>Hết hạn</span>" : "") %>
                                            <div class="mt-2">
                                                <span class="text-primary small">Xem chi tiết <i class="bi bi-arrow-right"></i></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </a>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoTinTuyenSinh" runat="server" Visible="false" CssClass="text-center py-4 text-muted">
                    <i class="bi bi-megaphone fs-3 opacity-25"></i>
                    <p class="mt-1 mb-0">Chưa có tin tuyển sinh nào.</p>
                </asp:Panel>
            </div>

            <%-- Tab bài viết --%>
            <div class="tab-pane fade" id="tab-baiviet">
                <asp:Repeater ID="rptBaiVietTruong" runat="server">
                    <HeaderTemplate><div class="row g-3"></HeaderTemplate>
                    <ItemTemplate>
                        <div class="col-md-6">
                            <div class="card border-0 shadow-sm h-100 card-hover">
                                <div class="row g-0">
                                    <div class="col-4">
                                        <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("AnhChinh")))
                                                       ? "/Resources/Images/no-image.png"
                                                       : Eval("AnhChinh") %>'
                                             class="img-fluid rounded-start h-100" style="object-fit:cover;min-height:120px;"
                                             alt='<%# Eval("TieuDe") %>'
                                             onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                                    </div>
                                    <div class="col-8">
                                        <div class="card-body py-2 px-3 d-flex flex-column h-100">
                                            <h6 class="fw-semibold mb-1 lh-sm">
                                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'
                                                   class="text-decoration-none text-body"><%# Eval("TieuDe") %></a>
                                            </h6>
                                            <div class="mt-auto d-flex justify-content-between align-items-center">
                                                <small class="text-muted"><%# ((DateTime)Eval("NgayDang")).ToString("dd/MM/yyyy") %></small>
                                                <small class="text-muted"><i class="bi bi-eye me-1"></i><%# Eval("LuotXem") %></small>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoBaiViet" runat="server" Visible="false" CssClass="text-center py-4 text-muted">
                    <i class="bi bi-newspaper fs-3 opacity-25"></i>
                    <p class="mt-1 mb-0">Chưa có bài viết nào về trường này.</p>
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
                                <span class="text-warning">
                                    <%# new string('★', (byte)Eval("DiemDanhGia")) %>
                                    <%# new string('☆', 5 - (byte)Eval("DiemDanhGia")) %>
                                </span>
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
        <div class="card border-0 shadow-sm mb-3">
            <div class="card-body">
                <h6 class="fw-bold mb-3">Thông tin trường</h6>
                <table class="table table-sm table-borderless mb-0">
                    <tr><td class="text-muted">Loại trường</td><td><asp:Literal ID="litLoai" runat="server" /></td></tr>
                    <tr><td class="text-muted">Khu vực</td><td><asp:Literal ID="litVung" runat="server" /></td></tr>
                    <tr><td class="text-muted">Điện thoại</td><td><asp:Literal ID="litSdt" runat="server" /></td></tr>
                    <tr><td class="text-muted">Website</td><td><asp:HyperLink ID="hlkWeb" runat="server" Target="_blank" /></td></tr>
                    <tr><td class="text-muted">Đánh giá</td><td><asp:Literal ID="litDanhGiaSidebar" runat="server" /></td></tr>
                    <tr><td class="text-muted">Kiểm định</td><td><asp:Literal ID="litKiemDinh" runat="server" /></td></tr>
                </table>
            </div>
        </div>

        <%-- Yêu thích --%>
        <asp:Panel ID="pnlWishlist" runat="server" Visible="false">
            <asp:Button ID="btnWishList" runat="server"
                CssClass="btn btn-outline-danger w-100 mb-3"
                OnClick="btnWishList_Click" />
        </asp:Panel>

        <%-- Form tư vấn --%>
        <div class="card border-0 shadow-sm">
            <div class="card-body">
                <h6 class="fw-bold mb-3"><i class="bi bi-chat-dots me-1 text-primary"></i>Đăng ký tư vấn</h6>
                <div class="mb-2">
                    <asp:TextBox ID="txtTVHoTen" runat="server" CssClass="form-control form-control-sm"
                        placeholder="Họ tên" />
                </div>
                <div class="mb-2">
                    <asp:TextBox ID="txtTVEmail" runat="server" CssClass="form-control form-control-sm"
                        placeholder="Email" TextMode="Email" />
                </div>
                <div class="mb-2">
                    <asp:TextBox ID="txtTVSdt" runat="server" CssClass="form-control form-control-sm"
                        placeholder="Số điện thoại" />
                </div>
                <div class="mb-3">
                    <asp:TextBox ID="txtTVNoiDung" runat="server" CssClass="form-control form-control-sm"
                        TextMode="MultiLine" Rows="3" placeholder="Nội dung cần tư vấn..." />
                </div>
                <asp:Button ID="btnGuiTuVan" runat="server" Text="Gửi yêu cầu"
                    CssClass="btn btn-primary btn-sm w-100" OnClick="btnGuiTuVan_Click"
                    CausesValidation="false" />
                <asp:Literal ID="litTVThongBao" runat="server" />
            </div>
        </div>
    </div>
</div>

</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// Vẽ biểu đồ xu hướng điểm chuẩn khi người dùng click tab "Điểm chuẩn"
(function() {
    var maTruong = '<%= MaTruongHienTai %>';
    if (!maTruong || maTruong === '0') return;

    var chartInited = false;
    var tabBtn = document.querySelector('a[href="#tab-diem"]');
    if (!tabBtn) return;

    tabBtn.addEventListener('shown.bs.tab', function() {
        if (chartInited) return;
        chartInited = true;

        var status = document.getElementById('chartStatus');
        fetch('/Handlers/DiemChuanChart.ashx?maTruong=' + maTruong)
            .then(function(r) { return r.json(); })
            .then(function(data) {
                if (status) status.textContent = '';
                if (!data.labels || data.labels.length === 0) {
                    if (status) status.textContent = 'Chưa có dữ liệu lịch sử';
                    return;
                }
                var ctx = document.getElementById('chartDiemChuan').getContext('2d');
                new Chart(ctx, {
                    type: 'line',
                    data: data,
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        interaction: { mode: 'index', intersect: false },
                        plugins: {
                            legend: { position: 'bottom', labels: { boxWidth: 12, font: { size: 11 } } },
                            tooltip: {
                                callbacks: {
                                    label: function(ctx) {
                                        return ctx.dataset.label + ': ' + (ctx.raw !== null ? ctx.raw : 'N/A');
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                title: { display: true, text: 'Điểm chuẩn' },
                                min: 0, max: 30,
                                ticks: { stepSize: 5 }
                            },
                            x: { title: { display: true, text: 'Năm tuyển sinh' } }
                        }
                    }
                });
            })
            .catch(function() {
                if (status) status.textContent = 'Không tải được dữ liệu biểu đồ';
            });
    });
})();
</script>
</asp:Content>
