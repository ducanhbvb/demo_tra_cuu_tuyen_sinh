<%@ Page Title="Chi tiết bài viết" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="ChiTietBaiViet.aspx.cs" Inherits="ChiTietBaiViet_Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- hero-banner, editorial-content, sidebar-card styles are in Content/Client.css --%>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<asp:Panel ID="pnlBaiViet" runat="server">
    
    <%-- HERO BANNER FULL WIDTH --%>
    <div class="hero-banner" style="background: linear-gradient(to bottom, rgba(0,0,0,0.1) 0%, rgba(0,0,0,0.8) 100%), url('<%= BannerUrl %>') center center / cover no-repeat;">
        <div class="hero-content">
            <span class="hero-badge" style="display:<%= string.IsNullOrEmpty(TheLoaiBadge) ? "none" : "inline-block" %>"><%= TheLoaiBadge %></span>
            <h1 class="hero-title"><asp:Literal ID="litTieuDe" runat="server" /></h1>
            
            <div class="hero-meta">
                <div class="article-author-signature">
                    <img src="<%: ResolveUrl("~/Resources/Images/no-image.png") %>" alt="Author" width="32" height="32" />
                    <span class="fw-medium"><asp:Literal ID="litNguoiDang" runat="server" /></span>
                </div>
                <span><i class="bi bi-calendar3 me-1"></i> <asp:Literal ID="litNgayDang" runat="server" /></span>
                <span><i class="bi bi-eye me-1"></i> <asp:Literal ID="litLuotXem" runat="server" /> lượt đọc</span>
            </div>
        </div>
    </div>

    <%-- ARTICLE LAYOUT --%>
    <div class="article-layout">
        
        <%-- MAIN CONTENT COLUMN --%>
        <div class="article-main">
            
            <%-- Sticky Share Bar --%>
            <div class="social-share-stick">
                <a href="#" class="social-btn" title="Chia sẻ Facebook"><i class="bi bi-facebook"></i></a>
                <a href="#" class="social-btn" title="Chia sẻ Twitter"><i class="bi bi-twitter-x"></i></a>
                <a href="#" class="social-btn" title="Chia sẻ Zalo"><i class="bi bi-chat-fill"></i></a>
                <a href="#" class="social-btn" title="Copy Link"><i class="bi bi-link-45deg"></i></a>
            </div>

            <%-- Editorial Body --%>
            <div class="editorial-content">
                <asp:Literal ID="litNoiDung" runat="server" />
            </div>
            
            <hr class="my-5" />

            <%-- Bottom Interaction Footer --%>
            <div class="d-flex justify-content-between align-items-center mb-5">
                <div class="d-flex gap-2">
                    <asp:Panel ID="pnlTruongBadge" runat="server" Visible="false">
                        <span class="badge bg-secondary"><asp:Literal ID="litTenTruong" runat="server" /></span>
                    </asp:Panel>
                </div>
            </div>

        </div>

        <%-- SIDEBAR COLUMN --%>
        <div class="article-sidebar">
            <asp:Panel ID="pnlLienQuan" runat="server" CssClass="sidebar-card">
                <h5 class="fw-bold mb-4 border-bottom pb-2">Đọc nhiều tuần này</h5>
                <asp:Repeater ID="rptMoiNhat" runat="server">
                    <ItemTemplate>
                        <div class="related-item">
                            <a href='/ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                                <img src='<%# string.IsNullOrEmpty(Convert.ToString(Eval("AnhChinh"))) ? "/Resources/Images/no-image.png" : Eval("AnhChinh") %>' alt="News" loading="lazy" />
                            </a>
                            <div class="related-item-content">
                                <h6><a href='/ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'><%# Eval("TieuDe") %></a></h6>
                                <small class="text-muted"><i class="bi bi-clock me-1"></i><%# FormatDate(Eval("NgayDang")) %></small>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
                <a href="/BaiViet.aspx" class="btn btn-outline-primary w-100 mt-3 rounded-pill fw-medium">Xem tất cả tin tức</a>
            </asp:Panel>

            <div class="sidebar-card text-center" style="background: linear-gradient(135deg, var(--bs-primary), #6f42c1); color: white;">
                <h5 class="fw-bold">Bạn đang chọn trường?</h5>
                <p class="small opacity-75 mb-3">Sử dụng công cụ so sánh điểm chuẩn và gợi ý trường của chúng tôi.</p>
                <a href="/TimKiemTruong.aspx" class="btn btn-light w-100 rounded-pill fw-bold text-primary">Tìm trường ngay</a>
            </div>
        </div>

    </div>
</asp:Panel>

<asp:Literal ID="litNotFound" runat="server" />
</asp:Content>
