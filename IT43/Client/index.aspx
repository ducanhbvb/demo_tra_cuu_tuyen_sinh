<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="index.aspx.cs" Inherits="index_page" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- HERO --%>
<div class="hero-section text-center mb-5">
    <div class="hero-overlay"></div>
    <div class="hero-content">
        <h1 class="font-outfit fw-bolder mb-3">Tra Cứu Thông Tin Tuyển Sinh</h1>
        <p class="lead text-white-50 mb-4" style="font-size: 1.15rem;">Tìm trường, tra cứu điểm chuẩn, khám phá ngành học phù hợp với tương lai của bạn</p>

        <%-- Quick search --%>
        <div class="row justify-content-center mt-4 mb-4">
            <div class="col-md-8 col-lg-6">
                <div class="input-group input-group-lg shadow-lg" style="border-radius: 50px; overflow: hidden;">
                    <asp:TextBox ID="txtTimKiem" runat="server" CssClass="form-control border-0 px-4"
                        placeholder="Nhập tên trường, ngành học..." style="font-size: 1rem;" />
                    <asp:Button ID="btnTimKiem" runat="server" Text="Tìm kiếm"
                        CssClass="btn btn-warning fw-bold px-4" OnClick="btnTimKiem_Click" />
                </div>
            </div>
        </div>

        <%-- Quick links --%>
        <div class="mt-4 d-flex flex-wrap gap-3 justify-content-center">
            <a href="/TimKiemTruong.aspx?loai=1" class="btn btn-outline-light rounded-pill px-4"><i class="bi bi-bank me-2"></i>Công lập</a>
            <a href="/TimKiemTruong.aspx?loai=2" class="btn btn-outline-light rounded-pill px-4"><i class="bi bi-building me-2"></i>Tư thục</a>
            <a href="/TraCuuDiemChuan.aspx"       class="btn btn-warning rounded-pill px-4 fw-bold text-dark"><i class="bi bi-bar-chart-fill me-2"></i>Điểm chuẩn 2024</a>
        </div>
    </div>
</div>

<%-- THỐNG KÊ NHANH --%>
<div class="row g-3 mb-5">
    <div class="col-6 col-md-3">
        <div class="card border-0 shadow-sm text-center p-4">
            <div class="fs-1 fw-bold text-primary lh-1 mb-1"><asp:Literal ID="litSoTruong" runat="server" /></div>
            <div class="text-muted">Trường đại học</div>
        </div>
    </div>
    <div class="col-6 col-md-3">
        <div class="card border-0 shadow-sm text-center p-4">
            <div class="fs-1 fw-bold text-success lh-1 mb-1"><asp:Literal ID="litSoNganh" runat="server" /></div>
            <div class="text-muted">Ngành học</div>
        </div>
    </div>
    <div class="col-6 col-md-3">
        <div class="card border-0 shadow-sm text-center p-4">
            <div class="fs-1 fw-bold text-warning lh-1 mb-1"><asp:Literal ID="litSoTin" runat="server" /></div>
            <div class="text-muted">Tin tuyển sinh</div>
        </div>
    </div>
    <div class="col-6 col-md-3">
        <div class="card border-0 shadow-sm text-center p-4">
            <div class="fs-1 fw-bold text-info lh-1 mb-1"><asp:Literal ID="litNamMoiNhat" runat="server" /></div>
            <div class="text-muted">Năm tuyển sinh</div>
        </div>
    </div>
</div>

<%-- TRƯỜNG NỔI BẬT --%>
<div class="d-flex justify-content-between align-items-end mb-4">
    <h3 class="font-outfit fw-bold mb-0"><i class="bi bi-award-fill text-warning me-2"></i>Trường Gợi Ý</h3>
    <a href="/TimKiemTruong.aspx" class="text-decoration-none fw-medium">Xem tất cả <i class="bi bi-arrow-right"></i></a>
</div>
<div class="row g-4 mb-5">
    <asp:Repeater ID="rptTruong" runat="server">
        <ItemTemplate>
            <div class="col-sm-6 col-md-4 col-lg-3">
                <div class="card truong-card h-100">
                    <div class="img-wrapper position-relative">
                        <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("AnhDaiDien")))
                                       ? "/Resources/Images/no-image.png"
                                       : Eval("AnhDaiDien") %>'
                             class="card-img-top" style="height:160px;object-fit:cover;"
                             alt='<%# Eval("TenTruong") %>'
                             onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                        <span class="badge bg-primary position-absolute top-0 start-0 m-3 shadow-sm rounded-pill px-3 py-2 text-white">
                           <%# Convert.ToString(Eval("LoaiTruong")) == "1" ? "Công lập" : "Tư thục" %>
                        </span>
                    </div>
                    <div class="card-body p-4 d-flex flex-column">
                        <h5 class="card-title font-outfit fw-bold mb-2 lh-sm">
                            <a href='/ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'
                               class="text-decoration-none text-body stretched-link">
                                <%# Eval("TenTruong") %>
                            </a>
                        </h5>
                        <p class="text-muted small mt-auto mb-0 pt-2 border-top border-light">
                            <i class="bi bi-geo-alt-fill text-danger opacity-75 me-1"></i><%# Eval("TinhThanh") %>
                        </p>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<%-- BÀI VIẾT MỚI NHẤT --%>
<div class="d-flex justify-content-between align-items-end mb-4">
    <h3 class="font-outfit fw-bold mb-0"><i class="bi bi-pen-fill text-info me-2"></i>Bài viết mới nhất</h3>
    <a href="/BaiViet.aspx" class="text-decoration-none fw-medium">Xem tất cả <i class="bi bi-arrow-right"></i></a>
</div>
<div class="row g-4 mb-5">
    <asp:Repeater ID="rptBaiViet" runat="server">
        <ItemTemplate>
            <div class="col-sm-6 col-md-4 col-lg-3">
                <div class="card h-100 border-0 shadow-sm card-hover">
                    <a href='/ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                        <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("AnhChinh")))
                                       ? "/Resources/Images/no-image.png"
                                       : Eval("AnhChinh") %>'
                             class="card-img-top" style="height:160px;object-fit:cover;"
                             alt='<%# Eval("TieuDe") %>'
                             onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                    </a>
                    <div class="card-body p-3 d-flex flex-column">
                        <h6 class="fw-semibold mb-2 lh-sm">
                            <a href='/ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'
                               class="text-decoration-none text-body">
                                <%# Eval("TieuDe") %>
                            </a>
                        </h6>
                        <div class="mt-auto d-flex justify-content-between align-items-center">
                            <small class="text-muted"><%# ((DateTime)Eval("NgayDang")).ToString("dd/MM/yyyy") %></small>
                            <small class="text-muted"><i class="bi bi-eye me-1"></i><%# Eval("LuotXem") %></small>
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<%-- TIN TUYỂN SINH MỚI NHẤT --%>
<div class="d-flex justify-content-between align-items-end mb-4">
    <h3 class="font-outfit fw-bold mb-0"><i class="bi bi-newspaper text-success me-2"></i>Tin tuyển sinh mới nhất</h3>
    <a href="/TraCuuDiemChuan.aspx" class="text-decoration-none fw-medium">Xem tất cả <i class="bi bi-arrow-right"></i></a>
</div>
<div class="card border-0 shadow-sm mb-5" style="border-radius: var(--card-radius); overflow: hidden;">
    <div class="table-responsive">
        <asp:GridView ID="gvTinMoiNhat" runat="server"
            CssClass="table table-hover align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            EmptyDataText="Chưa có tin tuyển sinh.">
            <HeaderStyle CssClass="bg-light text-secondary font-outfit" Font-Bold="true" />
            <RowStyle CssClass="border-bottom" />
            <Columns>
                <asp:BoundField DataField="TenTruong"      HeaderText="Trường"         HtmlEncode="true" />
                <asp:BoundField DataField="TenChuyenNganh" HeaderText="Ngành"          HtmlEncode="true" />
                <asp:BoundField DataField="TenPhuongThuc"  HeaderText="Phương thức"    HtmlEncode="true" />
                <asp:BoundField DataField="NamTuyenSinh"   HeaderText="Năm"            ItemStyle-CssClass="fw-medium text-center" HeaderStyle-CssClass="text-center" />
                <asp:BoundField DataField="DiemChuanNamTruoc" HeaderText="Điểm chuẩn"  DataFormatString="{0:F2}" ItemStyle-CssClass="fw-bold text-danger text-end pe-4" HeaderStyle-CssClass="text-end pe-4" />
                <asp:HyperLinkField Text="Chi tiết" HeaderText=""
                    DataNavigateUrlFields="TruongSlug"
                    DataNavigateUrlFormatString="/ChiTietTruong.aspx?slug={0}"
                    ControlStyle-CssClass="btn btn-sm btn-light rounded-pill px-3" ItemStyle-CssClass="text-center" />
            </Columns>
        </asp:GridView>
    </div>
</div>

</asp:Content>
