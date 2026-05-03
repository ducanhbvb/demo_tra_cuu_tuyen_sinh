<%@ Page Title="Chi tiết tin tuyển sinh" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="ChiTietTinTuyenSinh.aspx.cs" Inherits="ChiTietTinTuyenSinh" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.3/dist/chart.umd.min.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="text-center py-5">
    <i class="bi bi-exclamation-circle fs-1 text-muted"></i>
    <h4 class="mt-3">Không tìm thấy tin tuyển sinh</h4>
    <a href="index.aspx" class="btn btn-outline-primary mt-2"><i class="bi bi-house me-1"></i>Về trang chủ</a>
</asp:Panel>

<asp:Panel ID="pnlChiTiet" runat="server">

<%-- Breadcrumb --%>
<nav class="mb-3">
    <ol class="breadcrumb mb-0 small">
        <li class="breadcrumb-item"><a href="index.aspx" class="text-decoration-none">Trang chủ</a></li>
        <li class="breadcrumb-item">
            <a href='<%# "ChiTietTruong.aspx?slug=" + TruongSlug %>' class="text-decoration-none">
                <asp:Literal ID="litBreadTruong" runat="server" />
            </a>
        </li>
        <li class="breadcrumb-item active"><asp:Literal ID="litBreadTieuDe" runat="server" /></li>
    </ol>
</nav>

<%-- Header --%>
<div class="card border-0 shadow-sm mb-4">
    <div class="card-body">
        <div class="d-flex align-items-start gap-3">
            <asp:Image ID="imgLogo" runat="server"
                style="width:64px;height:64px;object-fit:contain;background:#f8f9fa;border-radius:8px;padding:4px;border:1px solid #eee;" />
            <div class="flex-grow-1">
                <h3 class="fw-bold mb-1"><asp:Literal ID="litTieuDe" runat="server" /></h3>
                <p class="mb-0 text-muted">
                    <i class="bi bi-building me-1"></i>
                    <a href='<%# "ChiTietTruong.aspx?slug=" + TruongSlug %>' class="text-decoration-none">
                        <asp:Literal ID="litTenTruong" runat="server" />
                    </a>
                </p>
            </div>
            <asp:Panel ID="pnlHanNop" runat="server" Visible="false" CssClass="text-end">
                <span class="badge bg-warning text-dark fs-6">
                    <i class="bi bi-clock me-1"></i><asp:Literal ID="litHanNop" runat="server" />
                </span>
            </asp:Panel>
        </div>
    </div>
</div>

<div class="row g-4">
    <%-- LEFT: Nội dung chi tiết --%>
    <div class="col-lg-8">
        <%-- Thông tin chung --%>
        <div class="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white fw-semibold border-bottom">
                <i class="bi bi-info-circle me-1 text-primary"></i>Thông tin chung
            </div>
            <div class="card-body p-0">
                <table class="table table-sm mb-0">
                    <tr><td class="text-muted ps-3" style="width:180px">Ngành</td>
                        <td class="fw-medium"><asp:Literal ID="litNganh" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Phương thức</td>
                        <td><asp:Literal ID="litPhuongThuc" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Tổ hợp môn</td>
                        <td><asp:Literal ID="litToHop" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Năm tuyển sinh</td>
                        <td><asp:Literal ID="litNam" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Chỉ tiêu</td>
                        <td class="fw-bold"><asp:Literal ID="litChiTieu" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Điểm chuẩn (năm trước)</td>
                        <td class="fw-bold text-danger"><asp:Literal ID="litDiemChuan" runat="server" /></td></tr>
                    <asp:Panel ID="pnlDiemNamNay" runat="server" Visible="false">
                    <tr><td class="text-muted ps-3">Điểm chuẩn (năm nay)</td>
                        <td class="fw-bold text-success"><asp:Literal ID="litDiemNamNay" runat="server" /></td></tr>
                    </asp:Panel>
                    <tr><td class="text-muted ps-3">Học phí</td>
                        <td><asp:Literal ID="litHocPhi" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Loại hình</td>
                        <td><asp:Literal ID="litLoaiHinh" runat="server" /></td></tr>
                    <tr><td class="text-muted ps-3">Cơ sở đào tạo</td>
                        <td><asp:Literal ID="litCoSo" runat="server" /></td></tr>
                </table>
            </div>
        </div>

        <%-- Mô tả chi tiết --%>
        <asp:Panel ID="pnlMoTa" runat="server" Visible="false" CssClass="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white fw-semibold border-bottom">
                <i class="bi bi-file-text me-1 text-primary"></i>Mô tả chi tiết
            </div>
            <div class="card-body">
                <asp:Literal ID="litMoTa" runat="server" />
            </div>
        </asp:Panel>

        <%-- Biểu đồ xu hướng ngành --%>
        <div class="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white fw-semibold border-bottom d-flex justify-content-between align-items-center">
                <span><i class="bi bi-graph-up me-1 text-primary"></i>Xu hướng điểm chuẩn ngành này</span>
                <span class="text-muted small" id="chartStatus">Đang tải...</span>
            </div>
            <div class="card-body">
                <div style="position:relative;height:280px;">
                    <canvas id="chartNganh"></canvas>
                </div>
            </div>
        </div>
    </div>

    <%-- RIGHT: Sidebar --%>
    <div class="col-lg-4">
        <%-- Meta --%>
        <div class="card border-0 shadow-sm mb-3">
            <div class="card-body">
                <div class="d-flex justify-content-between text-muted small mb-2">
                    <span><i class="bi bi-calendar me-1"></i>Ngày đăng: <asp:Literal ID="litNgayDang" runat="server" /></span>
                    <span><i class="bi bi-eye me-1"></i><asp:Literal ID="litLuotXem" runat="server" /> lượt xem</span>
                </div>
                <a href='<%# "ChiTietTruong.aspx?slug=" + TruongSlug + "#tab-tuyensinh" %>'
                   class="btn btn-outline-primary btn-sm w-100">
                    <i class="bi bi-arrow-left me-1"></i>Xem tất cả tin của trường
                </a>
            </div>
        </div>

        <%-- Tin cùng trường --%>
        <div class="card border-0 shadow-sm mb-3">
            <div class="card-header bg-white fw-semibold border-bottom">
                <i class="bi bi-list-ul me-1 text-primary"></i>Tin khác cùng trường
            </div>
            <div class="card-body p-0">
                <asp:Repeater ID="rptTinCungTruong" runat="server">
                    <HeaderTemplate><div class="list-group list-group-flush"></HeaderTemplate>
                    <ItemTemplate>
                        <a href='ChiTietTinTuyenSinh.aspx?id=<%# Eval("MaTin") %>'
                           class="list-group-item list-group-item-action py-2 px-3 <%# Convert.ToInt32(Eval("MaTin")) == MaTinHienTai ? "active" : "" %>">
                            <div class="fw-medium small"><%# Eval("TieuDeHienThi") %></div>
                            <div class="d-flex justify-content-between text-muted" style="font-size:.75rem;">
                                <span>Chỉ tiêu: <%# Eval("ChiTieu") ?? "N/A" %></span>
                                <span><%# Eval("DiemChuanNamTruoc") != null ? string.Format("{0:F2} điểm", Eval("DiemChuanNamTruoc")) : "" %></span>
                            </div>
                        </a>
                    </ItemTemplate>
                    <FooterTemplate></div></FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>

</asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
(function() {
    var maTruong = '<%= MaTruongHienTai %>';
    var maChuyenNganh = '<%= MaChuyenNganhHienTai %>';
    if (!maTruong || maTruong === '0') return;

    var status = document.getElementById('chartStatus');
    fetch('/Handlers/DiemChuanChart.ashx?maTruong=' + maTruong + '&maChuyenNganh=' + maChuyenNganh)
        .then(function(r) { return r.json(); })
        .then(function(data) {
            if (status) status.textContent = '';
            if (!data.labels || data.labels.length === 0) {
                if (status) status.textContent = 'Chưa có dữ liệu lịch sử';
                return;
            }
            var ctx = document.getElementById('chartNganh').getContext('2d');
            new Chart(ctx, {
                type: 'line',
                data: data,
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: { mode: 'index', intersect: false },
                    plugins: {
                        legend: { position: 'bottom', labels: { boxWidth: 12, font: { size: 11 } } }
                    },
                    scales: {
                        y: { title: { display: true, text: 'Điểm chuẩn' }, min: 0, max: 30, ticks: { stepSize: 5 } },
                        x: { title: { display: true, text: 'Năm' } }
                    }
                }
            });
        })
        .catch(function() { if (status) status.textContent = 'Không tải được biểu đồ'; });
})();
</script>
</asp:Content>
