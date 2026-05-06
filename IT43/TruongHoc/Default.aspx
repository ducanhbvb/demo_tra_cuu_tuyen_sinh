<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/TruongHoc/TruongHoc.Master"
   CodeBehind="Default.aspx.cs" Inherits="TruongHoc_Default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- ══════════════════════════════════════════════
     WELCOME BANNER
══════════════════════════════════════════════ --%>
<div class="db-welcome-banner mb-4">
    <div class="db-welcome-text">
        <div class="db-welcome-title">
            Xin chào, <asp:Literal ID="litTenTruongBanner" runat="server">Trường học</asp:Literal>! 👋
        </div>
        <div class="db-welcome-sub">
            Cổng trường đang hoạt động.
            Có <strong><asp:Literal ID="litSoTuVanBanner" runat="server">0</asp:Literal> tư vấn</strong> chờ phản hồi.
        </div>
        <div class="db-welcome-date" id="dbBannerDate"></div>
    </div>
</div>

<%-- ══════════════════════════════════════════════
     QUICK ACTION BUTTONS
══════════════════════════════════════════════ --%>
<div class="db-quick-actions mb-4">
    <a href="/TruongHoc/QuanLyTinTuyenSinh.aspx" class="db-action-btn btn-green">
        <i class="bi bi-file-earmark-plus"></i> Thêm tin tuyển sinh
    </a>
    <a href="/TruongHoc/QuanLyBaiViet.aspx" class="db-action-btn btn-teal">
        <i class="bi bi-pencil-square"></i> Viết bài mới
    </a>
    <a href="/TruongHoc/GopYTuVan.aspx" class="db-action-btn btn-orange">
        <i class="bi bi-chat-dots"></i> Xem tư vấn
        <span class="db-action-badge"><asp:Literal ID="litSoTuVanBtn" runat="server">0</asp:Literal></span>
    </a>
</div>

<%-- ══════════════════════════════════════════════
     STAT CARDS (4 cards)
══════════════════════════════════════════════ --%>
<div class="row g-3 mb-4">
    <%-- Tổng bài viết --%>
    <div class="col-6 col-xl-3">
        <div class="db-stat-card grad-blue">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoBaiViet" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-newspaper"></i></div>
            </div>
            <div class="db-stat-label">Tổng Bài Viết</div>
            <div class="db-stat-sub">Của trường bạn</div>
        </div>
    </div>
    <%-- Bài viết đang hiện --%>
    <div class="col-6 col-xl-3">
        <div class="db-stat-card grad-green">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoBaiVietHoatDong" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-eye"></i></div>
            </div>
            <div class="db-stat-label">Bài Viết Đang Hiện</div>
            <div class="db-stat-sub">Công khai</div>
        </div>
    </div>
    <%-- Tin tuyển sinh --%>
    <div class="col-6 col-xl-3">
        <div class="db-stat-card grad-amber">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoTin" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-file-earmark-text"></i></div>
            </div>
            <div class="db-stat-label">Tin Tuyển Sinh</div>
            <div class="db-stat-sub">Năm <asp:Literal ID="litNamTuyenSinh" runat="server" /></div>
        </div>
    </div>
    <%-- Tư vấn chờ --%>
    <div class="col-6 col-xl-3">
        <div class="db-stat-card grad-rose">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoTuVan" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-chat-dots-fill"></i></div>
            </div>
            <div class="db-stat-label">Tư Vấn Chờ</div>
            <div class="db-stat-sub db-stat-warn">⚠ Cần xử lý</div>
        </div>
    </div>
</div>

<%-- ══════════════════════════════════════════════
     BOTTOM ROW: Bài viết + Tin tuyển sinh mới nhất
══════════════════════════════════════════════ --%>
<div class="row g-4">

    <%-- LEFT: Bài viết mới nhất --%>
    <div class="col-lg-6">
        <div class="admin-card h-100">
            <div class="admin-card-header">
                <div class="admin-card-title">
                    <i class="bi bi-newspaper me-2"></i>Bài viết mới nhất
                </div>
                <a href="/TruongHoc/QuanLyBaiViet.aspx" class="db-view-all-btn">
                    → Xem tất cả
                </a>
            </div>
            <div class="admin-card-body p-0">
                <div class="table-responsive">
                <asp:GridView ID="gvBaiViet" runat="server"
                    EnableViewState="false"
                    CssClass="table table-hover align-middle mb-0"
                    AutoGenerateColumns="false" GridLines="None"
                    EmptyDataText="Chưa có bài viết nào.">
                    <Columns>
                        <asp:BoundField DataField="TieuDe"  HeaderText="Tiêu đề"   HtmlEncode="true" />
                        <asp:BoundField DataField="NgayDang" HeaderText="Ngày đăng" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:TemplateField HeaderText="Trạng thái">
                            <ItemTemplate>
                                <%# (bool)Eval("TrangThai")
                                    ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hiển thị</span>"
                                    : "<span class='badge badge-soft-secondary'>Ẩn</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <%-- RIGHT: Tin tuyển sinh + Tư vấn chờ --%>
    <div class="col-lg-6">

        <%-- Tin tuyển sinh mới nhất --%>
        <div class="admin-card mb-4">
            <div class="admin-card-header">
                <div class="admin-card-title">
                    <i class="bi bi-file-earmark-text me-2"></i>Tin tuyển sinh mới nhất
                </div>
                <a href="/TruongHoc/QuanLyTinTuyenSinh.aspx" class="db-view-all-btn">→ Xem tất cả</a>
            </div>
            <div class="admin-card-body p-0">
                <div class="table-responsive">
                <asp:GridView ID="gvTin" runat="server"
                    EnableViewState="false"
                    CssClass="table table-hover align-middle mb-0"
                    AutoGenerateColumns="false" GridLines="None"
                    EmptyDataText="Chưa có tin tuyển sinh nào.">
                    <Columns>
                        <asp:BoundField DataField="TenChuyenNganh" HeaderText="Ngành"            HtmlEncode="true" />
                        <asp:BoundField DataField="NamTuyenSinh"   HeaderText="Năm"              />
                        <asp:BoundField DataField="DiemChuanNamTruoc" HeaderText="Điểm chuẩn"    DataFormatString="{0:N1}" />
                        <asp:TemplateField HeaderText="Trạng thái">
                            <ItemTemplate>
                                <%# (bool)Eval("TrangThai")
                                    ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hiển thị</span>"
                                    : "<span class='badge badge-soft-secondary'>Ẩn</span>" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                </div>
            </div>
        </div>

        <%-- Tư vấn chờ xử lý --%>
        <div class="admin-card">
            <div class="admin-card-header">
                <div class="admin-card-title">
                    <i class="bi bi-chat-dots text-warning me-2"></i>Tư vấn chờ xử lý
                    <span class="db-badge-count"><asp:Literal ID="litTuVanCount" runat="server">0</asp:Literal></span>
                </div>
                <a href="/TruongHoc/GopYTuVan.aspx" class="db-view-all-btn">Xem tất cả</a>
            </div>
            <div class="db-tuvan-list">
                <asp:Repeater ID="rptTuVan" runat="server">
                    <ItemTemplate>
                        <div class="db-tuvan-item">
                            <div class="db-tuvan-avatar" style='<%# "background:" + GetAvatarColor(Eval("HoTen").ToString()) %>'>
                                <%# GetInitial(Eval("HoTen").ToString()) %>
                            </div>
                            <div class="db-tuvan-info">
                                <div class="db-tuvan-name"><%# Server.HtmlEncode(Eval("HoTen").ToString()) %></div>
                                <div class="db-tuvan-school"><%# FormatTimeAgo(Eval("NgayGui")) %></div>
                            </div>
                            <a href='<%# "/TruongHoc/GopYTuVan.aspx" %>' class="db-btn-reply">Trả lời</a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoTuVan" runat="server" Visible="false" CssClass="db-empty-state">
                    <i class="bi bi-chat-square-dots"></i>
                    <span>Không có tư vấn nào chờ xử lý</span>
                </asp:Panel>
            </div>
        </div>

    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<%-- Dashboard behaviors (banner, stat count-up) đã có trong Scripts/site.js --%>
</asp:Content>
