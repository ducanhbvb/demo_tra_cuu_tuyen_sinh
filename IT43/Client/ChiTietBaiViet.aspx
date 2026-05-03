<%@ Page Title="Bài viết" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="ChiTietBaiViet.aspx.cs" Inherits="ChiTietBaiViet_Page" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:Literal ID="litNotFound" runat="server" />

<asp:Panel ID="pnlBaiViet" runat="server">
<div class="row g-4">
    <%-- NỘI DUNG CHÍNH --%>
    <div class="col-lg-8">
        <article>
            <%-- Ảnh bìa --%>
            <asp:Panel ID="pnlAnhBia" runat="server" CssClass="mb-4">
                <asp:Image ID="imgAnhBia" runat="server"
                    CssClass="w-100 rounded-3"
                    style="max-height:400px;object-fit:cover;cursor:pointer;"
                    onclick="if(this.src)window.open(this.src,'_blank')"
                    title="Click để xem ảnh đầy đủ" />
            </asp:Panel>

            <%-- Meta --%>
            <div class="d-flex flex-wrap gap-3 text-muted small mb-3">
                <span><i class="bi bi-calendar3 me-1"></i><asp:Literal ID="litNgayDang" runat="server" /></span>
                <span><i class="bi bi-eye me-1"></i><asp:Literal ID="litLuotXem" runat="server" /> lượt xem</span>
                <asp:Panel ID="pnlTruongBadge" runat="server">
                    <span><i class="bi bi-building me-1"></i><asp:Literal ID="litTenTruong" runat="server" /></span>
                </asp:Panel>
            </div>

            <%-- Tiêu đề --%>
            <h2 class="fw-bold mb-4"><asp:Literal ID="litTieuDe" runat="server" /></h2>

            <%-- Nội dung --%>
            <div class="article-content">
                <asp:Literal ID="litNoiDung" runat="server" />
            </div>
        </article>

        <%-- Bài viết liên quan --%>
        <asp:Panel ID="pnlLienQuan" runat="server" CssClass="mt-5">
            <h5 class="fw-bold mb-3 border-bottom pb-2">Bài viết liên quan</h5>
            <asp:Repeater ID="rptLienQuan" runat="server">
                <HeaderTemplate><div class="row g-3"></HeaderTemplate>
                <ItemTemplate>
                            <div class="col-md-4">
                        <div class="card border-0 shadow-sm h-100">
                            <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                                <img src='<%# string.IsNullOrEmpty(Eval("AnhChinh") as string) ? "/Resources/Images/no-image.png" : Eval("AnhChinh") %>'
                                     class="card-img-top" style="height:120px;object-fit:cover;"
                                     onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                            </a>
                            <div class="card-body p-2">
                                <small class="fw-semibold">
                                    <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'
                                       class="text-body text-decoration-none"><%# Eval("TieuDe") %></a>
                                </small>
                                <div class="text-muted" style="font-size:.75rem;">
                                    <%# ((DateTime)Eval("NgayDang")).ToString("dd/MM/yyyy") %>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate></div></FooterTemplate>
            </asp:Repeater>
        </asp:Panel>
    </div>

    <%-- SIDEBAR --%>
    <div class="col-lg-4">
        <div class="card border-0 shadow-sm mb-3">
            <div class="card-body">
                <h6 class="fw-bold mb-3">Bài viết mới nhất</h6>
                <asp:Repeater ID="rptMoiNhat" runat="server">
                    <ItemTemplate>
                        <div class="d-flex gap-2 mb-3 align-items-start">
                            <img src='<%# string.IsNullOrEmpty(Eval("AnhChinh") as string) ? "/Resources/Images/no-image.png" : Eval("AnhChinh") %>'
                                 style="width:60px;height:50px;object-fit:cover;border-radius:6px;flex-shrink:0;"
                                 onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                            <div>
                                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'
                                   class="text-body text-decoration-none small fw-semibold lh-sm">
                                    <%# Eval("TieuDe") %>
                                </a>
                                <div class="text-muted" style="font-size:.72rem;">
                                    <%# ((DateTime)Eval("NgayDang")).ToString("dd/MM/yyyy") %>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="card border-0 shadow-sm">
            <div class="card-body">
                <h6 class="fw-bold mb-2">Xem tất cả bài viết</h6>
                <a href="/BaiViet.aspx" class="btn btn-outline-primary btn-sm w-100">
                    <i class="bi bi-newspaper me-1"></i>Danh sách bài viết
                </a>
            </div>
        </div>
    </div>
</div>
</asp:Panel>

</asp:Content>
