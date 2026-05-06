<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="Default.aspx.cs" Inherits="Admin_Default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- ══════════════════════════════════════════════
     WELCOME BANNER
══════════════════════════════════════════════ --%>
<div class="db-welcome-banner mb-4">
    <div class="db-welcome-text">
        <div class="db-welcome-title">
            Chào buổi tối, Admin! 👋
        </div>
        <div class="db-welcome-sub">
            Hệ thống đang hoạt động bình thường.
            Có <strong><asp:Literal ID="litSoTuVanBanner" runat="server">0</asp:Literal> tư vấn</strong> chờ xử lý.
        </div>
        <div class="db-welcome-date" id="dbBannerDate"></div>
    </div>
</div>

<%-- ══════════════════════════════════════════════
     QUICK ACTION BUTTONS
══════════════════════════════════════════════ --%>
<div class="db-quick-actions mb-4">
    <a href="ChinhSuaTruong.aspx?id=0" class="db-action-btn btn-purple">
        <i class="bi bi-building-add"></i> Thêm trường
    </a>
    <a href="QuanLyTinTuyenSinh.aspx?action=new" class="db-action-btn btn-green">
        <i class="bi bi-file-earmark-plus"></i> Thêm tin tuyển sinh
    </a>
    <a href="QuanLyBaiViet.aspx?action=new" class="db-action-btn btn-teal">
        <i class="bi bi-pencil-square"></i> Viết bài mới
    </a>
    <a href="QuanLyGopYTuVan.aspx" class="db-action-btn btn-orange">
        <i class="bi bi-chat-dots"></i> Xem tư vấn
        <span class="db-action-badge"><asp:Literal ID="litSoTuVanBtn" runat="server">0</asp:Literal></span>
    </a>
    <a href="QuanLyLogs.aspx" class="db-action-btn btn-slate">
        <i class="bi bi-journal-text"></i> Nhật ký hệ thống
    </a>
</div>

<%-- ══════════════════════════════════════════════
     STAT CARDS (6 cards)
══════════════════════════════════════════════ --%>
<div class="row g-3 mb-4">
    <%-- Trường đại học --%>
    <div class="col-6 col-xl-2">
        <div class="db-stat-card grad-blue">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoTruong" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-building-fill"></i></div>
            </div>
            <div class="db-stat-label">Trường đại học</div>
            <div class="db-stat-sub">+ 2 tháng này</div>
        </div>
    </div>
    <%-- Tin tuyển sinh --%>
    <div class="col-6 col-xl-2">
        <div class="db-stat-card grad-green">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoTin" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-file-earmark-text-fill"></i></div>
            </div>
            <div class="db-stat-label">Tin tuyển sinh</div>
            <div class="db-stat-sub">+ 5 tuần này</div>
        </div>
    </div>
    <%-- Tư vấn chờ --%>
    <div class="col-6 col-xl-2">
        <div class="db-stat-card grad-amber">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoTuVan" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-chat-dots-fill"></i></div>
            </div>
            <div class="db-stat-label">Tư vấn chờ xử lý</div>
            <div class="db-stat-sub db-stat-warn">⚠ Cần xử lý ngay</div>
        </div>
    </div>
    <%-- Tài khoản --%>
    <div class="col-6 col-xl-2">
        <div class="db-stat-card grad-cyan">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoTK" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-people-fill"></i></div>
            </div>
            <div class="db-stat-label">Tài khoản</div>
            <div class="db-stat-sub">+ 1 mới nhất</div>
        </div>
    </div>
    <%-- Ngành học --%>
    <div class="col-6 col-xl-2">
        <div class="db-stat-card grad-teal">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoNganh" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-book-fill"></i></div>
            </div>
            <div class="db-stat-label">Ngành học</div>
            <div class="db-stat-sub">Đang cập nhật</div>
        </div>
    </div>
    <%-- Nhật ký --%>
    <div class="col-6 col-xl-2">
        <div class="db-stat-card grad-rose">
            <div class="db-stat-top">
                <div class="db-stat-num"><asp:Literal ID="litSoLog" runat="server">0</asp:Literal></div>
                <div class="db-stat-icon"><i class="bi bi-shield-check-fill"></i></div>
            </div>
            <div class="db-stat-label">Nhật ký hệ thống</div>
            <div class="db-stat-sub db-stat-ok">✓ <asp:Literal ID="litLogThanhCong" runat="server">0</asp:Literal> thành công</div>
        </div>
    </div>
</div>

<%-- ══════════════════════════════════════════════
     BOTTOM ROW: Trường mới + Tư vấn / Hoạt động
══════════════════════════════════════════════ --%>
<div class="row g-4">

    <%-- LEFT: Trường mới cập nhật --%>
    <div class="col-lg-7">
        <div class="admin-card h-100">
            <div class="admin-card-header">
                <div class="admin-card-title">
                    <i class="bi bi-building me-2"></i>Trường mới cập nhật
                </div>
                <a href="QuanLyTruong.aspx" class="db-view-all-btn">
                    → Xem tất cả
                </a>
            </div>
            <div class="table-responsive">
                <table class="table table-hover db-table mb-0">
                    <thead>
                        <tr>
                            <th>TRƯỜNG</th>
                            <th>TỈNH/TP</th>
                            <th>LOẠI</th>
                            <th>CẬP NHẬT</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptTruong" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <div class="db-truong-cell">
                                            <div class="db-truong-icon">
                                                <i class="bi bi-building"></i>
                                            </div>
                                            <div>
                                                <div class="db-truong-name"><%# Server.HtmlEncode(Eval("TenTruong").ToString()) %></div>
                                                <div class="db-truong-url"><%# Eval("Website") != DBNull.Value ? Server.HtmlEncode(Eval("Website").ToString()) : "" %></div>
                                            </div>
                                        </div>
                                    </td>
                                    <td><span class="db-badge-city"><%# Server.HtmlEncode(Eval("TinhThanh").ToString()) %></span></td>
                                    <td><span class="db-badge-type"><%# Eval("LoaiTruong") != DBNull.Value ? Server.HtmlEncode(Eval("LoaiTruong").ToString()) : "Công lập" %></span></td>
                                    <td><span class="db-time-ago"><%# FormatTimeAgo(Eval("ThoiGianCapNhat")) %></span></td>
                                    <td>
                                        <a href='<%# "ChinhSuaTruong.aspx?id=" + Eval("MaTruong") %>' class="db-btn-edit">Sửa</a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <%-- RIGHT: Tư vấn chờ + Hoạt động gần đây --%>
    <div class="col-lg-5">

        <%-- Tư vấn chờ xử lý --%>
        <div class="admin-card mb-4">
            <div class="admin-card-header">
                <div class="admin-card-title">
                    <i class="bi bi-chat-dots text-warning me-2"></i>Tư vấn chờ xử lý
                    <span class="db-badge-count"><asp:Literal ID="litTuVanCount" runat="server">0</asp:Literal></span>
                </div>
                <a href="QuanLyGopYTuVan.aspx" class="db-view-all-btn">Xem tất cả</a>
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
                                <div class="db-tuvan-school"><%# Eval("TenTruong") != DBNull.Value ? Server.HtmlEncode(Eval("TenTruong").ToString()) : "" %> • <%# FormatTimeAgo(Eval("NgayGui")) %></div>
                            </div>
                            <a href='<%# "QuanLyGopYTuVan.aspx?id=" + Eval("ID") %>' class="db-btn-reply">Trả lời</a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoTuVan" runat="server" Visible="false" CssClass="db-empty-state">
                    <i class="bi bi-chat-square-dots"></i>
                    <span>Không có tư vấn nào chờ xử lý</span>
                </asp:Panel>
            </div>
        </div>

        <%-- Hoạt động gần đây --%>
        <div class="admin-card">
            <div class="admin-card-header">
                <div class="admin-card-title">
                    <i class="bi bi-activity text-success me-2"></i>Hoạt động gần đây
                </div>
                <a href="QuanLyLogs.aspx" class="db-view-all-btn db-view-all-btn--purple">Nhật ký đầy đủ</a>
            </div>
            <div class="db-activity-list">
                <asp:Repeater ID="rptActivity" runat="server">
                    <ItemTemplate>
                        <div class="db-activity-item">
                            <div class='db-activity-dot <%# (bool)Eval("IsSuccess") ? "dot-success" : "dot-danger" %>'>
                                <i class='bi <%# (bool)Eval("IsSuccess") ? "bi-check-lg" : "bi-x-lg" %>'></i>
                            </div>
                            <div class="db-activity-info">
                                <div class="db-activity-title"><%# GetActivityTitle(Eval("HanhDong").ToString()) %></div>
                                <div class="db-activity-sub"><%# Eval("Email") != DBNull.Value ? Server.HtmlEncode(Eval("Email").ToString()) : "Hệ thống" %> • <%# FormatTimeAgo(Eval("ThoiGian")) %></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoActivity" runat="server" Visible="false" CssClass="db-empty-state">
                    <i class="bi bi-journal-x"></i>
                    <span>Chưa có hoạt động nào</span>
                </asp:Panel>
            </div>
        </div>

    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<%-- Dashboard behaviors (banner, stat count-up) đã chuyển vào Scripts/site.js --%>
</asp:Content>
