<%@ Page Title="Tin tức & Bài viết" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="BaiViet.aspx.cs" Inherits="BaiViet_Page" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex justify-content-between align-items-center mb-4">
    <h4 class="fw-bold mb-0"><i class="bi bi-newspaper me-2 text-primary"></i>Tin tức &amp; Bài viết</h4>
</div>

<%-- ═══════ BỘ LỌC + TÌM KIẾM ═══════ --%>
<div class="card border-0 shadow-sm mb-4">
    <div class="card-body py-2">
        <div class="row g-2 align-items-end">
            <div class="col-md-5">
                <asp:TextBox ID="txtTimKiem" runat="server" CssClass="form-control form-control-sm"
                    placeholder="🔍 Tìm kiếm bài viết..." />
            </div>
            <div class="col-md-4">
                <asp:DropDownList ID="ddlTruong" runat="server" CssClass="form-select form-select-sm">
                    <asp:ListItem Value="">-- Tất cả trường --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-auto d-flex gap-2">
                <asp:Button ID="btnTimKiem" runat="server" Text="Tìm kiếm"
                    CssClass="btn btn-sm btn-primary" OnClick="btnTimKiem_Click"
                    CausesValidation="false" />
                <asp:Button ID="btnXemTat" runat="server" Text="Xem tất cả"
                    CssClass="btn btn-sm btn-outline-secondary" OnClick="btnXemTat_Click"
                    CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

<%-- ═══════ BÀI VIẾT NỔI BẬT ═══════ --%>
<asp:Panel ID="pnlNoiBat" runat="server" Visible="false" CssClass="mb-4">
    <asp:Repeater ID="rptNoiBat" runat="server">
        <ItemTemplate>
            <div class="card border-0 shadow-sm overflow-hidden featured-card">
                <div class="row g-0">
                    <div class="col-md-5">
                        <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                            <img src='<%# (Eval("AnhChinh") == null || Eval("AnhChinh") == DBNull.Value || string.IsNullOrEmpty(Eval("AnhChinh").ToString())) ? "/Resources/Images/no-image.png" : Eval("AnhChinh").ToString() %>'
                                 class="w-100 h-100" style="object-fit:cover;min-height:240px;"
                                 alt='<%# Eval("TieuDe") %>' loading="lazy" onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                        </a>
                    </div>
                    <div class="col-md-7 d-flex flex-column">
                        <div class="card-body d-flex flex-column h-100">
                            <div class="d-flex gap-2 mb-2 flex-wrap align-items-center">
                                <span class="badge bg-warning text-dark"><i class="bi bi-star-fill me-1"></i>Nổi bật</span>
                                <%# (Eval("TenTruong") != DBNull.Value && Eval("TenTruong") != null)
                                    ? "<span class='badge bg-primary-subtle text-primary-emphasis'>" + Server.HtmlEncode(Eval("TenTruong").ToString()) + "</span>"
                                    : "" %>
                            </div>
                            <h5 class="fw-bold mb-2">
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="text-body text-decoration-none">
                                    <%# Eval("TieuDe") %>
                                </a>
                            </h5>
                            <p class="text-muted small mb-3 flex-grow-1"><%# TruncateText(Eval("NoiDung"), 200) %></p>
                            <div class="d-flex justify-content-between align-items-center mt-auto">
                                <small class="text-muted">
                                    <i class="bi bi-calendar3 me-1"></i><%# FormatDate(Eval("NgayDang")) %>
                                    <span class="ms-3"><i class="bi bi-eye me-1"></i><%# FormatInt(Eval("LuotXem")) %></span>
                                </small>
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="btn btn-primary btn-sm">Đọc tiếp <i class="bi bi-arrow-right"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>

<%-- ═══════ GRID + SIDEBAR ═══════ --%>
<div class="row g-4">
    <%-- Danh sách bài viết (col-8) --%>
    <div class="col-lg-8">
        <asp:Repeater ID="rptBaiViet" runat="server">
            <HeaderTemplate><div class="row g-3"></HeaderTemplate>
            <ItemTemplate>
                <div class="col-md-6 col-lg-4">
                    <div class="card h-100 border-0 shadow-sm card-hover card-baiviet">
                        <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                            <img src='<%# (Eval("AnhChinh") == null || Eval("AnhChinh") == DBNull.Value || string.IsNullOrEmpty(Eval("AnhChinh").ToString())) ? "/Resources/Images/no-image.png" : Eval("AnhChinh").ToString() %>'
                                 class="card-img-top" style="height:160px;object-fit:cover;"
                                 alt='<%# Eval("TieuDe") %>' loading="lazy" onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                        </a>
                        <div class="card-body d-flex flex-column p-3">
                            <div class="d-flex gap-2 mb-2 flex-wrap">
                                <%# (Eval("TenTruong") != DBNull.Value && Eval("TenTruong") != null)
                                    ? "<span class='badge bg-primary-subtle text-primary-emphasis small'>" + Server.HtmlEncode(Eval("TenTruong").ToString()) + "</span>"
                                    : "" %>
                                <span class="text-muted small"><i class="bi bi-eye me-1"></i><%# FormatInt(Eval("LuotXem")) %></span>
                            </div>
                            <h6 class="fw-semibold mb-2 card-title-clamp">
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="text-body text-decoration-none">
                                    <%# Eval("TieuDe") %>
                                </a>
                            </h6>
                            <p class="text-muted small mb-2 card-excerpt"><%# TruncateText(Eval("NoiDung"), 100) %></p>
                            <div class="mt-auto d-flex justify-content-between align-items-center">
                                <small class="text-muted"><%# FormatDate(Eval("NgayDang")) %></small>
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="btn btn-xs btn-outline-primary btn-sm">Đọc tiếp</a>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate></div></FooterTemplate>
        </asp:Repeater>

        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="text-center py-5 text-muted">
            <i class="bi bi-newspaper fs-1 opacity-25"></i>
            <p class="mt-2">Chưa có bài viết nào.</p>
        </asp:Panel>

        <%-- Phân trang --%>
        <div class="d-flex justify-content-between align-items-center mt-4">
            <span class="text-muted small">Tổng: <strong><asp:Literal ID="litTong" runat="server" /></strong> bài viết</span>
            <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
                <HeaderTemplate><ul class="pagination pagination-sm mb-0"></HeaderTemplate>
                <ItemTemplate>
                    <li class='page-item <%# SafeGetBool(Eval("IsActive")) ? "active" : "" %>'>
                        <asp:LinkButton runat="server" CssClass="page-link"
                            CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>'>
                            <%# Eval("PageText") %>
                        </asp:LinkButton>
                    </li>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <%-- SIDEBAR (col-4) --%>
    <div class="col-lg-4">
        <%-- Bài viết mới nhất --%>
        <div class="card border-0 shadow-sm mb-3">
            <div class="card-header bg-transparent border-0 fw-semibold">
                <i class="bi bi-clock-history me-1 text-primary"></i>Bài viết mới nhất
            </div>
            <div class="card-body pt-0">
                <asp:Repeater ID="rptMoiNhat" runat="server">
                    <ItemTemplate>
                        <div class="d-flex align-items-start mb-2 sidebar-item">
                            <span class="badge bg-primary-subtle text-primary me-2 mt-1"><%# Container.ItemIndex + 1 %></span>
                            <div>
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="text-body text-decoration-none small fw-medium sidebar-link">
                                    <%# Eval("TieuDe") %>
                                </a>
                                <div class="text-muted" style="font-size:0.75rem;"><%# FormatDate(Eval("NgayDang")) %></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <%-- Bài xem nhiều nhất --%>
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-transparent border-0 fw-semibold">
                <i class="bi bi-fire me-1 text-danger"></i>Xem nhiều nhất
            </div>
            <div class="card-body pt-0">
                <asp:Repeater ID="rptXemNhieu" runat="server">
                    <ItemTemplate>
                        <div class="d-flex align-items-start mb-2 sidebar-item">
                            <span class="badge bg-danger-subtle text-danger me-2 mt-1"><%# Container.ItemIndex + 1 %></span>
                            <div class="flex-grow-1">
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="text-body text-decoration-none small fw-medium sidebar-link">
                                    <%# Eval("TieuDe") %>
                                </a>
                                <div class="text-muted" style="font-size:0.75rem;">
                                    <i class="bi bi-eye me-1"></i><%# FormatInt(Eval("LuotXem")) %> lượt xem
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>

</asp:Content>
