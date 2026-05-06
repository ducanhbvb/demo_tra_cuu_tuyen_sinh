<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="index.aspx.cs" Inherits="index_page" %>
<%-- KHÔNG dùng OutputCache ở đây: tầng BLL (ThongKeBLL) đã cache query DB 10 phút rồi.
   Nếu bật OutputCache, admin sửa config sẽ không phản ánh ngay vì HTML render bị cache
   (không thể invalidate xuyên app domain Admin ↔ Client). --%>
<%-- KHÔNG dùng OutputCache ở đây: tầng BLL (ThongKeBLL) đã cache query DB 10 phút rồi.
     Nếu bật OutputCache, admin sửa config sẽ không phản ánh ngay vì HTML render bị cache
     (không thể invalidate xuyên app domain Admin ↔ Client). --%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%: ResolveUrl("~/Content/HomePage.css?v=2") %>" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <%-- HERO SECTION --%>
    <section class="home-hero">
        <div class="hero-wave" aria-hidden="true"></div>
        <div class="hero-content">
            <h1 class="hero-title">Khởi đầu tương lai ngay hôm nay.</h1>
            <p class="hero-subtitle">Nền tảng tra cứu dữ liệu tuyển sinh Việt Nam. Phân tích điểm chuẩn, định hướng ngành nghề và gợi ý trường đại học phù hợp nhất với năng lực của bạn.</p>
            
            <div class="search-glass">
                <asp:TextBox ID="txtTimKiem" runat="server" CssClass="form-control" placeholder="Nhập tên trường, ngành học (VD: Bách Khoa, CNTT)..." />
                <asp:Button ID="btnTimKiem" runat="server" Text="Tra Cứu" CssClass="btn-search" OnClick="btnTimKiem_Click" />
            </div>

            <div class="hero-tags">
                <asp:Repeater ID="rptHomeTags" runat="server">
                    <ItemTemplate>
                        <a href='<%# Eval("Url") %>' class="tag-btn">
                            <i class='<%# Eval("Icon") %>'></i><%# Eval("Text") %>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <%-- QUICK STATS --%>
    <div class="container stats-container">
        <div class="row g-4">
            <div class="col-6 col-lg-3">
                <div class="stat-card">
                    <div class="stat-icon bg-icon-1"><i class="bi bi-bank"></i></div>
                    <div class="stat-number text-primary"><asp:Literal ID="litSoTruong" runat="server" /></div>
                    <div class="stat-label">Trường Đại Học</div>
                </div>
            </div>
            <div class="col-6 col-lg-3">
                <div class="stat-card">
                    <div class="stat-icon bg-icon-2"><i class="bi bi-journal-bookmark-fill"></i></div>
                    <div class="stat-number text-success"><asp:Literal ID="litSoNganh" runat="server" /></div>
                    <div class="stat-label">Ngành Học</div>
                </div>
            </div>
            <div class="col-6 col-lg-3">
                <div class="stat-card">
                    <div class="stat-icon bg-icon-3"><i class="bi bi-graph-up-arrow"></i></div>
                    <div class="stat-number text-warning"><asp:Literal ID="litNamMoiNhat" runat="server" /></div>
                    <div class="stat-label">Năm Tuyển Sinh</div>
                </div>
            </div>
            <div class="col-6 col-lg-3">
                <div class="stat-card">
                    <div class="stat-icon bg-icon-4"><i class="bi bi-people-fill"></i></div>
                    <div class="stat-number text-purple" style="color: #6f42c1;"><asp:Literal ID="litSoTin" runat="server" /></div>
                    <div class="stat-label">Tin Tuyển Sinh</div>
                </div>
            </div>
        </div>
    </div>

    <%-- FEATURED SCHOOLS --%>
    <div class="container mb-5 pb-4">
        <div class="section-header">
            <div>
                <h2 class="section-title">Trường Gợi Ý Hàng Đầu</h2>
                <p class="text-muted mt-2 mb-0">Những ngôi trường được đánh giá cao và nhiều thí sinh quan tâm nhất.</p>
            </div>
            <div class="section-actions">
                <a href="/TimKiemTruong.aspx" class="view-all-link">Xem tất cả <i class="bi bi-arrow-right"></i></a>
                <div class="home-slider-controls d-none d-md-inline-flex" data-slider="schools" aria-label="Trường nổi bật auto slider">
                    <button type="button" class="home-slider-arrow" aria-label="Trường trước"><i class="bi bi-chevron-left"></i></button>
                    <button type="button" class="home-slider-arrow" aria-label="Trường kế tiếp"><i class="bi bi-chevron-right"></i></button>
                </div>
            </div>
        </div>

        <div class="school-slider-window">
        <div class="school-slider-track js-school-slider-track">
            <asp:Repeater ID="rptTruong" runat="server">
                <ItemTemplate>
                    <div class="js-school-slide">
                        <div class="premium-card">
                            <div class="card-img-wrap">
                                <a href='/ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'>
                                    <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("AnhDaiDien"))) ? "/Resources/Images/no-image.png" : Eval("AnhDaiDien") %>' 
                                         alt='<%# Eval("TenTruong") %>' loading="lazy" />
                                </a>
                            </div>
                            <div class="premium-card-body">
                                <div class="school-badge-stack">
                                    <span class='school-card-badge <%# GetLoaiTruongBadgeClass(Eval("LoaiTruong")) %>'><%# GetLoaiTruongText(Eval("LoaiTruong")) %></span>
                                    <span class='school-card-badge <%# GetCapBacBadgeClass(Eval("CapBacDaoTao")) %>' style='display:<%# string.IsNullOrEmpty(GetCapBacText(Eval("CapBacDaoTao"))) ? "none" : "inline-flex" %>'><%# GetCapBacText(Eval("CapBacDaoTao")) %></span>
                                </div>
                                <h3 class="premium-card-title">
                                    <a href='/ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'><%# Eval("TenTruong") %></a>
                                </h3>
                                <p class="text-muted small mb-3"><i class="bi bi-geo-alt-fill text-danger opacity-75 me-1"></i><%# Eval("TinhThanh") %></p>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        </div>
    </div>

    <%-- LATEST POSTS --%>
    <div class="container mb-5 pb-5">
        <div class="section-header">
            <div>
                <h2 class="section-title">Tin Tức Tuyển Sinh</h2>
                <p class="text-muted mt-2 mb-0">Cập nhật thông tin mới nhất về kỳ thi THPT Quốc Gia và xét tuyển Đại học.</p>
            </div>
            <div class="section-actions">
                <a href="/BaiViet.aspx" class="view-all-link">Xem tất cả tin tức <i class="bi bi-arrow-right"></i></a>
                <div class="home-slider-controls d-none d-md-inline-flex" data-slider="news" aria-label="Bài viết auto slider">
                    <button type="button" class="home-slider-arrow" aria-label="Nhóm bài trước"><i class="bi bi-chevron-left"></i></button>
                    <button type="button" class="home-slider-arrow" aria-label="Nhóm bài kế tiếp"><i class="bi bi-chevron-right"></i></button>
                </div>
            </div>
        </div>

        <div class="news-slider-stage">
            <asp:Literal ID="litNewsGroups" runat="server" />
        </div>
    </div>

    <%-- LATEST ADMISSIONS GRID (Brought from previous layout and styled) --%>
    <div class="container mb-5">
        <div class="section-header">
            <div>
                <h2 class="section-title">Thông Tin Xét Tuyển Nổi Bật</h2>
            </div>
            <a href="/TraCuuDiemChuan.aspx" class="view-all-link">Tra Cứu Full <i class="bi bi-arrow-right"></i></a>
        </div>
        <div class="card premium-card border-0 mb-5 border">
            <div class="table-responsive">
                <asp:GridView ID="gvTinMoiNhat" runat="server"
                    CssClass="table table-hover align-middle mb-0"
                    AutoGenerateColumns="false" GridLines="None"
                    EnableViewState="false"
                    EmptyDataText="Chưa có tin tuyển sinh.">
                    <HeaderStyle CssClass="bg-light text-secondary font-outfit" Font-Bold="true" />
                    <RowStyle CssClass="border-bottom" />
                    <Columns>
                        <asp:BoundField DataField="TenTruong"      HeaderText="Trường"         HtmlEncode="true" ItemStyle-CssClass="fw-medium text-dark" />
                        <asp:BoundField DataField="TenChuyenNganh" HeaderText="Ngành"          HtmlEncode="true" />
                        <asp:BoundField DataField="TenPhuongThuc"  HeaderText="Phương thức"    HtmlEncode="true" />
                        <asp:BoundField DataField="NamTuyenSinh"   HeaderText="Năm"            ItemStyle-CssClass="text-center" HeaderStyle-CssClass="text-center" />
                        <asp:BoundField DataField="DiemChuanNamTruoc" HeaderText="Điểm chuẩn"  DataFormatString="{0:F2}" ItemStyle-CssClass="fw-bold text-danger text-end pe-4" HeaderStyle-CssClass="text-end pe-4" />
                        <asp:HyperLinkField Text="Xem" HeaderText=""
                            DataNavigateUrlFields="TruongSlug"
                            DataNavigateUrlFormatString="/ChiTietTruong.aspx?slug={0}"
                            ControlStyle-CssClass="btn btn-sm btn-outline-primary rounded-pill px-3" ItemStyle-CssClass="text-center" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="ScriptContent" ContentPlaceHolderID="ScriptContent" runat="server">
    <script src="<%: ResolveUrl("~/Scripts/homepage-client.js?v=2") %>"></script>
</asp:Content>
