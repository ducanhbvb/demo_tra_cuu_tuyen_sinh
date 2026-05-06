<%@ Page Title="Hộp thư của tôi" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="HopThu.aspx.cs" Inherits="Client_HopThu" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="server">

<link href="/Content/HopThu.css" rel="stylesheet" />

</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="row justify-content-center">
<div class="col-lg-10">

<%-- Breadcrumb + header --%>
<div class="ht-page-header">
    <div class="ht-breadcrumb mb-2">
        <a href="/index.aspx"><i class="bi bi-house me-1"></i>Trang chủ</a><span class="sep">/</span>
        <a href="/my-profile.aspx">Hồ sơ</a><span class="sep">/</span>
        <span style="color:var(--bs-body-color);font-weight:600;">Hộp thư</span>
    </div>
    <div class="d-flex align-items-center gap-3 flex-wrap">
        <div class="ht-header-icon">
            <i class="bi bi-envelope-fill"></i>
        </div>
        <div>
            <h2 class="fw-bold mb-0" style="font-size:1.25rem;">
                Hộp thư của tôi
                <asp:Literal ID="litBadgeHopThu" runat="server" />
            </h2>
            <p class="mb-0 text-muted" style="font-size:.8rem;">Tổng hợp các cuộc tư vấn với trường đại học</p>
        </div>
        <div class="ms-auto d-flex align-items-center gap-2 flex-wrap">
            <div class="ht-kpi-bar">
                <div class="ht-kpi clr-purple">
                    <i class="bi bi-chat-left-text"></i>
                    <asp:Literal ID="litTongCuoc" runat="server">0</asp:Literal> cuộc
                </div>
                <div class="ht-kpi clr-yellow">
                    <i class="bi bi-dot" style="font-size:1.1rem;line-height:1;"></i>
                    <asp:Literal ID="litChuaDoc" runat="server">0</asp:Literal> chưa đọc
                </div>
                <div class="ht-kpi clr-green">
                    <i class="bi bi-check2-circle"></i>
                    <asp:Literal ID="litTongPhanHoi" runat="server">0</asp:Literal> phản hồi
                </div>
            </div>
            <a href="/my-profile.aspx" class="btn btn-sm btn-outline-secondary fw-semibold"
               style="border-radius:50rem;font-size:.8rem;padding:.32rem .9rem;">
                <i class="bi bi-person me-1"></i>Hồ sơ
            </a>
        </div>
    </div>
</div>

<%-- Main wrapper --%>
<div class="mt-3">
<div class="ht-wrapper" style="background:var(--bs-body-bg);border-radius:16px;border:1px solid var(--bs-border-color,#e5e7eb);box-shadow:0 2px 12px rgba(0,0,0,.06);">

    <%-- Toolbar (sticky) --%>
    <div class="ht-toolbar">
        <%-- Search --%>
        <div class="ht-search-wrap">
            <i class="bi bi-search si"></i>
            <input type="search" id="htSearch" placeholder="Tìm tên trường..." oninput="htFilter()" autocomplete="off"/>
        </div>

        <%-- Filter tabs --%>
        <div class="ht-tabs">
            <button class="ht-tab active" data-status="all" onclick="htSetStatus(this)">Tất cả</button>
            <button class="ht-tab" data-status="unread" onclick="htSetStatus(this)">
                <i class="bi bi-circle-fill" style="font-size:.45rem;vertical-align:middle;"></i> Chưa đọc
            </button>
            <button class="ht-tab" data-status="replied" onclick="htSetStatus(this)">Đã phản hồi</button>
            <button class="ht-tab" data-status="waiting" onclick="htSetStatus(this)">Chờ phản hồi</button>
        </div>

        <%-- Time filter --%>
        <div class="dropdown ms-auto">
            <button class="btn ht-sort-btn dropdown-toggle" type="button" id="htTimeBtnLabel" data-bs-toggle="dropdown">
                <i class="bi bi-calendar3"></i>
                <span id="htTimeLbl">Tất cả</span>
            </button>
            <ul class="dropdown-menu dropdown-menu-end shadow-sm" style="border-radius:12px;font-size:.82rem;min-width:155px;">
                <li><a class="dropdown-item active" href="#" onclick="htSetTime(this,'all','Tất cả');return false;">Tất cả thời gian</a></li>
                <li><a class="dropdown-item" href="#" onclick="htSetTime(this,'today','Hôm nay');return false;">Hôm nay</a></li>
                <li><a class="dropdown-item" href="#" onclick="htSetTime(this,'7d','7 ngày');return false;">7 ngày qua</a></li>
                <li><a class="dropdown-item" href="#" onclick="htSetTime(this,'30d','30 ngày');return false;">30 ngày qua</a></li>
                <li><a class="dropdown-item" href="#" onclick="htSetTime(this,'3m','3 tháng');return false;">3 tháng qua</a></li>
            </ul>
        </div>
    </div>

    <%-- Empty state --%>
    <asp:Literal ID="litEmpty" runat="server" />

    <%-- No filter result --%>
    <div id="noFilterResult">
        <i class="bi bi-funnel fs-3 d-block mb-2 opacity-25"></i>
        Không có kết quả phù hợp với bộ lọc.
    </div>

    <%-- Danh sách thư --%>
    <div class="ht-list">
    <asp:Repeater ID="rptHopThu" runat="server" OnItemCommand="rptHopThu_ItemCommand">
        <ItemTemplate>
            <div class='ht-row <%# Convert.ToInt32(Eval("SoChuaDoc")) > 0 ? "unread" : "" %>'
                 data-status='<%# GetItemStatus(Convert.ToInt32(Eval("SoChuaDoc")), Convert.ToInt32(Eval("SoLuotPhanHoi")), Convert.ToByte(Eval("TrangThai"))) %>'
                 data-date='<%# GetItemDateIso(Eval("PhanHoiCuoi"), Eval("NgayGui")) %>'
                 data-truong='<%# Server.HtmlEncode(Convert.ToString(Eval("TenTruong")).ToLower()) %>'
                 onclick="this.querySelector('.ht-trigger').click()">
                <%-- Avatar --%>
                <div class="ht-avatar" style='<%# GetAvatarStyle(Convert.ToString(Eval("TenTruong"))) %>'>
                    <%# GetAvatarChar(Convert.ToString(Eval("TenTruong"))) %>
                </div>
                <%-- Body --%>
                <div class="ht-row-body">
                    <div class="ht-row-top">
                        <div class="ht-row-name"><%# Server.HtmlEncode(Convert.ToString(Eval("TenTruong"))) %></div>
                        <span class="ht-row-time">
                            <%# Eval("PhanHoiCuoi") != DBNull.Value
                                ? RelativeTime.From((DateTime)Eval("PhanHoiCuoi"))
                                : RelativeTime.From((DateTime)Eval("NgayGui")) %>
                        </span>
                    </div>
                    <div class="ht-row-preview">
                        <%# Server.HtmlEncode(Convert.ToString(Eval("CauHoiGoc")).Length > 95
                            ? Convert.ToString(Eval("CauHoiGoc")).Substring(0,95) + "…"
                            : Convert.ToString(Eval("CauHoiGoc"))) %>
                    </div>
                </div>
                <%-- Badges phải --%>
                <div class="ht-row-right">
                    <%# GetBadgeHtml(Convert.ToInt32(Eval("SoChuaDoc")), Convert.ToInt32(Eval("SoLuotPhanHoi")), Convert.ToByte(Eval("TrangThai"))) %>
                </div>
                <%-- Hidden trigger --%>
                <asp:LinkButton runat="server" CommandName="XemThread"
                    CommandArgument='<%# Eval("ID") %>'
                    CssClass="ht-trigger" />
            </div>
        </ItemTemplate>
    </asp:Repeater>
    </div><%-- /.ht-list --%>

</div><%-- /.ht-wrapper --%>
</div><%-- /mt-3 --%>
</div><%-- /col-lg-10 --%>
</div><%-- /row --%>

<%-- ========== Modal xem thread ========== --%>
<asp:HiddenField ID="hfMaTuVan" runat="server" Value="0" />
<div class="modal fade" id="modalHopThuThread" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content" style="border:none;border-radius:16px;overflow:hidden;">
            <%-- Header gradient --%>
            <div class="ht-modal-header d-flex align-items-center justify-content-between">
                <div class="d-flex align-items-center gap-3">
                    <div style="width:34px;height:34px;background:rgba(255,255,255,.2);border-radius:9px;display:flex;align-items:center;justify-content:center;flex-shrink:0;">
                        <i class="bi bi-chat-left-dots-fill text-white" style="font-size:.9rem;"></i>
                    </div>
                    <div>
                        <h5 class="modal-title fw-bold mb-0 text-white" style="font-size:.95rem;">Chi tiết tư vấn</h5>
                        <small class="text-white" style="opacity:.8;font-size:.73rem;">
                            <asp:Literal ID="litModalTruong" runat="server" />
                        </small>
                    </div>
                </div>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body" style="padding:1.1rem 1.25rem;">
                <%-- Meta info chips --%>
                <div class="ht-modal-meta">
                    <asp:Literal ID="litHopThuMeta" runat="server" />
                </div>
                <%-- Nội dung câu hỏi gốc: hiển thị đầy đủ, không cắt ngắn --%>
                <div class="ht-original-question">
                    <div class="ht-original-question-label">
                        <i class="bi bi-chat-quote me-1"></i>Nội dung câu hỏi gốc
                    </div>
                    <div class="ht-original-question-text">
                        <asp:Literal ID="litCauHoiGoc" runat="server" />
                    </div>
                </div>
                <%-- Bubble chat timeline --%>
                <div class="fw-semibold mb-2 text-muted small text-uppercase" style="letter-spacing:.05em;font-size:.72rem;">
                    <i class="bi bi-clock-history me-1"></i>Lịch sử trao đổi
                </div>
                <div class="ht-timeline">
                    <asp:Repeater ID="rptThread" runat="server">
                        <ItemTemplate>
                            <%# Convert.ToString(Eval("LoaiNguoi")) == "System"
                                ? "<div class='ht-bubble-sys-wrap'><span class='ht-bubble-sys'>" +
                                  Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString()) +
                                  "</span></div>"
                                : GetBubbleHtml(
                                    Convert.ToString(Eval("LoaiNguoi")),
                                    Eval("HoTen") == DBNull.Value ? "?" : Eval("HoTen").ToString(),
                                    Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString(),
                                    Eval("NgayPhanHoi") == DBNull.Value ? (DateTime?)null : (DateTime?)Eval("NgayPhanHoi"),
                                    Convert.ToString(Eval("LoaiNguoi"))) %>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
            <%-- Reply box --%>
            <asp:Panel ID="pnlReply" runat="server" CssClass="modal-reply-box">
                <asp:Literal ID="litReplyMsg" runat="server" />
                <div class="d-flex flex-column gap-2">
                    <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="5"
                        CssClass="form-control form-control-sm"
                        placeholder="Nhập phản hồi của bạn tại đây…"
                        style="border-radius:10px;resize:vertical;border-color:#e5e7eb;font-size:.85rem;min-height:100px;" />
                    <div class="d-flex justify-content-between align-items-center">
                        <small class="text-muted" style="font-size:.74rem;">
                            <i class="bi bi-info-circle me-1"></i>Tối đa 10 lượt phản hồi / 24 giờ
                        </small>
                        <asp:Button ID="btnGuiReply" runat="server" Text="Gửi phản hồi"
                            CssClass="btn btn-sm btn-primary fw-semibold"
                            style="border-radius:50rem;padding:.4rem 1.2rem;background:linear-gradient(135deg,#7c3aed,#4f46e5);border:none;"
                            OnClick="btnGuiReply_Click" />
                    </div>
                </div>
            </asp:Panel>
            <div class="modal-footer" style="background:var(--bs-tertiary-bg,#f9fafb);padding:.55rem 1rem;">
                <button type="button" class="btn btn-sm btn-outline-secondary" data-bs-dismiss="modal" style="border-radius:50rem;">Đóng</button>
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">

<script>window.hopThuThreadHiddenFieldId = '<%= hfMaTuVan.ClientID %>';</script>

<script src="/Scripts/hopthu.js"></script>

</asp:Content>
