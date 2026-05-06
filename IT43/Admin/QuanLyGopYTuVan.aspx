<%@ Page Title="Góp ý / Tư vấn" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyGopYTuVan.aspx.cs" Inherits="Admin_QuanLyGopYTuVan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:HiddenField ID="hfLoai" runat="server" Value="TUVAN" />
<asp:Button ID="btnDoTab" runat="server" CssClass="d-none"
    OnClick="btnDoTab_Click" CausesValidation="false" />

<%-- Browser-style tabs --%>
<div class="bstab-bar">
    <button type="button" class='bstab <%: hfLoai.Value == "TUVAN" ? "active" : "" %>'
        onclick="switchTab('TUVAN', '<%= hfLoai.ClientID %>', '<%= btnDoTab.ClientID %>')">
        <i class="bi bi-chat-dots-fill"></i>
        <span>Tư vấn</span>
        <asp:Literal ID="litSoTuVan" runat="server" />
    </button>
    <button type="button" class='bstab <%: hfLoai.Value == "GOPY" ? "active" : "" %>'
        onclick="switchTab('GOPY', '<%= hfLoai.ClientID %>', '<%= btnDoTab.ClientID %>')">
        <i class="bi bi-flag-fill"></i>
        <span>Góp ý</span>
        <asp:Literal ID="litSoGopY" runat="server" />
    </button>
</div>

<%-- Filter bar: trạng thái + trường (chỉ hiện khi ở tab Tư vấn) --%>
<div class="admin-filter-bar mb-3 d-flex flex-wrap gap-2 align-items-center">
    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-select form-select-sm"
        AutoPostBack="true" OnSelectedIndexChanged="ddlTrangThai_Changed"
        style="min-width:180px;max-width:220px;">
        <asp:ListItem Value="">-- Tất cả trạng thái --</asp:ListItem>
        <asp:ListItem Value="0">Chờ xử lý</asp:ListItem>
        <asp:ListItem Value="1">Đã phản hồi</asp:ListItem>
        <asp:ListItem Value="2">Đã đóng</asp:ListItem>
    </asp:DropDownList>

    <%-- Dropdown lọc theo trường — chỉ hữu ích ở tab Tư vấn --%>
    <asp:Panel ID="pnlFilterTruong" runat="server">
        <asp:DropDownList ID="ddlTruong" runat="server" CssClass="form-select form-select-sm"
            AutoPostBack="true" OnSelectedIndexChanged="ddlTruong_Changed"
            style="min-width:220px;max-width:300px;">
            <asp:ListItem Value="">-- Tất cả trường --</asp:ListItem>
        </asp:DropDownList>
    </asp:Panel>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="admin-card">
    <div class="admin-card-body p-0">
        <div class="table-responsive">
        <asp:GridView ID="gvDanhSach" runat="server"
                    EnableViewState="false"
                    CssClass="table table-hover align-middle mb-0"
                    AutoGenerateColumns="false" GridLines="None"
                    DataKeyNames="ID" OnRowCommand="gvDanhSach_RowCommand"
                    OnRowDataBound="gvDanhSach_RowDataBound"
                    UseAccessibleHeader="true">
            <EmptyDataTemplate>
                <div class="text-center py-5 text-muted">
                    <i class="bi bi-inbox fs-1 d-block mb-2 opacity-50"></i>
                    <div class="fw-semibold">Không có dữ liệu</div>
                    <div class="small mt-1">Thử thay đổi bộ lọc hoặc kiểm tra lại sau.</div>
                </div>
            </EmptyDataTemplate>
            <Columns>
                <%-- Cột Họ tên: chỉ hiện ở tab Tư vấn --%>
                <asp:TemplateField HeaderText="Họ tên">
                    <ItemTemplate>
                        <%# hfLoai.Value == "TUVAN"
                            ? Server.HtmlEncode(Eval("HoTen") == DBNull.Value ? "" : Eval("HoTen").ToString())
                            : "<span class='text-muted fst-italic small'>Ẩn danh</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <%-- Cột Email: chỉ tư vấn mới có --%>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# hfLoai.Value == "TUVAN"
                            ? Server.HtmlEncode(Eval("Email") == DBNull.Value ? "" : Eval("Email").ToString())
                            : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TenTruong" HeaderText="Trường"   HtmlEncode="true" />
                <asp:BoundField DataField="NgayGui"   HeaderText="Ngày gửi" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# Convert.ToByte(Eval("TrangThai")) == 0
                            ? "<span class='badge badge-soft-warning'><i class='bi bi-hourglass-split me-1'></i>Chờ xử lý</span>"
                            : Convert.ToByte(Eval("TrangThai")) == 1
                                ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Đã phản hồi</span>"
                                : "<span class='badge badge-soft-secondary'><i class='bi bi-x-circle me-1'></i>Đã đóng</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Nội dung">
                    <ItemTemplate>
                        <span class="text-truncate d-inline-block" style="max-width:220px;"
                              title='<%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString()) %>'>
                            <%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" :
                                Eval("NoiDung").ToString().Length > 50
                                    ? Eval("NoiDung").ToString().Substring(0, 50) + "..."
                                    : Eval("NoiDung").ToString()) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="200px" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%-- Xem chi tiết --%>
                        <asp:LinkButton runat="server" CommandName="XemChiTiet"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-chart" title="Xem chi tiết">
                            <i class="bi bi-eye"></i>
                        </asp:LinkButton>
                        <%-- Phản hồi (chỉ khi Chờ xử lý) --%>
                        <asp:LinkButton runat="server" CommandName="PhanHoi"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-success"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) == 0 %>' title="Phản hồi">
                            <i class="bi bi-check2"></i>
                        </asp:LinkButton>
                        <%-- Roll back: đưa về Chờ xử lý (khi đã phản hồi) --%>
                        <asp:LinkButton runat="server" CommandName="RollBack"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-toggle"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) == 1 %>'
                            OnClientClick="return confirm('Hoàn tác về Chờ xử lý?')" title="Hoàn tác">
                            <i class="bi bi-arrow-counterclockwise"></i>
                        </asp:LinkButton>
                        <%-- Đóng (khi chưa đóng) --%>
                        <asp:LinkButton runat="server" CommandName="Dong"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-secondary"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) != 2 %>'
                            OnClientClick="return confirm('Đóng yêu cầu này?')" title="Đóng">
                            <i class="bi bi-x"></i>
                        </asp:LinkButton>
                        <%-- Mở lại (khi đã đóng) --%>
                        <asp:LinkButton runat="server" CommandName="MoLai"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-edit"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) == 2 %>' title="Mở lại">
                            <i class="bi bi-arrow-repeat"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        </div>
    </div>
</div>

<div class="d-flex justify-content-between mt-3">
    <span class="text-muted small">Tổng: <strong><asp:Literal ID="litTong" runat="server" /></strong></span>
    <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
        <HeaderTemplate><ul class="pagination pagination-sm mb-0"></HeaderTemplate>
        <ItemTemplate>
            <li class='page-item <%# (bool)Eval("IsActive") ? "active" : "" %>'>
                <asp:LinkButton runat="server" CssClass="page-link"
                    CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>'>
                    <%# Eval("PageText") %>
                </asp:LinkButton>
            </li>
        </ItemTemplate>
        <FooterTemplate></ul></FooterTemplate>
    </asp:Repeater>
</div>

<%-- HIDDEN FIELDS phục vụ modal thread --%>
<asp:HiddenField ID="hfMaTuVanHienTai" runat="server" Value="0" />

<%-- MODAL THREAD — Timeline trao đổi tư vấn (theo mockup bugs/mockup-quanlygopytuvan.html) --%>
<div class="modal fade" id="modalChiTiet" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <%-- Header gradient động theo trạng thái (M5) --%>
            <div class="modal-header border-0 pb-2" style="border-radius:12px 12px 0 0">
                <asp:Literal ID="litModalHeader" runat="server" />
                <button type="button" class="btn-close btn-close-white ms-auto" data-bs-dismiss="modal"></button>
            </div>

            <div class="modal-body pt-2">
                <%-- Info bar: Họ tên · Trạng thái · Email · Trường · Ngày gửi --%>
                <table class="w-100 mb-3 pb-2" style="border-bottom:1px solid var(--border-light);font-size:13px;">
                    <tr>
                        <td class="fw-semibold align-middle" style="font-size:14px;">
                            <i class="bi bi-person text-primary me-1"></i><asp:Literal ID="litCTHoTen" runat="server" />
                        </td>
                        <td class="text-end align-middle" style="white-space:nowrap;">
                            <asp:Literal ID="litCTTrangThai" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="pt-1" style="color:var(--bs-secondary-color);">
                            <i class="bi bi-envelope me-1"></i><asp:Literal ID="litCTEmail" runat="server" />
                            &nbsp;&nbsp;
                            <i class="bi bi-building me-1"></i><asp:Literal ID="litCTTruong" runat="server" />
                            &nbsp;&nbsp;
                            <i class="bi bi-clock me-1"></i><asp:Literal ID="litCTNgay" runat="server" />
                        </td>
                    </tr>
                </table>

                <%-- Nội dung câu hỏi gốc — luôn hiện (cả TUVAN lẫn GOPY) --%>
                <div class="mb-3">
                    <div class="mb-1" style="font-size:12px;font-weight:700;color:var(--bs-secondary-color);text-transform:uppercase;letter-spacing:.04em">
                        <i class="bi bi-chat-quote me-1"></i>Nội dung
                    </div>
                    <div class="border rounded p-3 bg-light" style="white-space:pre-wrap;min-height:60px;font-size:14px;line-height:1.6">
                        <asp:Literal ID="litCTNoiDung" runat="server" />
                    </div>
                </div>

                <%-- Timeline thread — chỉ hiện với TUVAN (Góp ý không có thread) --%>
                <asp:Panel ID="pnlThread" runat="server">
                    <div class="mb-2" style="font-size:13px;font-weight:700;color:var(--bs-secondary-color)">
                        <i class="bi bi-clock-history"></i> Lịch sử trao đổi
                    </div>
                    <div class="timeline mb-3">
                        <asp:Repeater ID="rptThread" runat="server">
                            <ItemTemplate>
                                <div class="timeline-item">
                                    <div class='timeline-dot <%# GetDotClass(Convert.ToString(Eval("LoaiNguoi"))) %>'></div>
                                    <div class='timeline-content <%# Convert.ToString(Eval("LoaiNguoi")) == "System" ? "timeline-system" : "" %>'>
                                        <div class="timeline-meta d-flex align-items-center gap-2">
                                            <%# AvatarHelper.GetHtml(
                                                    Eval("HoTen") == DBNull.Value ? "?" : Eval("HoTen").ToString(),
                                                    28, Convert.ToString(Eval("LoaiNguoi"))) %>
                                            <span>
                                                <strong><%# Server.HtmlEncode(Eval("HoTen") == DBNull.Value ? "Hệ thống" : Eval("HoTen").ToString()) %></strong>
                                                <%# GetLoaiNguoiLabel(Convert.ToString(Eval("LoaiNguoi"))) %>
                                                <span class="text-muted ms-1">—</span>
                                                <%# Eval("NgayPhanHoi") == DBNull.Value ? "" :
                                                    RelativeTime.FromWithTitle((DateTime)Eval("NgayPhanHoi")) %>
                                            </span>
                                        </div>
                                        <%# Convert.ToString(Eval("LoaiNguoi")) != "System" ? "<div class='timeline-text mt-1'>" + Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString()) + "</div>" : "" %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>

                <%-- pnlNoiDungGopY đã được thay bằng litCTNoiDung ở trên modal-body --%>
                <asp:Panel ID="pnlNoiDungGopY" runat="server" Visible="false" />

                <%-- Panel ẩn tương thích ngược --%>
                <asp:Panel ID="pnlGhiChu" runat="server" Visible="false">
                    <asp:Literal ID="litCTGhiChu" runat="server" />
                </asp:Panel>

                <%-- Reply box — chỉ hiện với TUVAN và khi chưa đóng --%>
                <asp:Panel ID="pnlReply" runat="server">
                    <div class="mt-3 pt-3" style="border-top:1px solid var(--border-light)">
                        <label class="form-label fw-semibold"><i class="bi bi-reply me-1"></i>Phản hồi</label>
                        <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="4"
                            CssClass="form-control" placeholder="Nhập nội dung phản hồi cho học sinh..." MaxLength="4000" />
                        <div class="mt-2">
                            <asp:CheckBox ID="chkGuiEmail" runat="server" Checked="true"
                                CssClass="form-check-input me-2" />
                            <label class="form-check-label small text-muted">
                                <i class="bi bi-envelope me-1"></i>Gửi email thông báo đến học sinh
                            </label>
                        </div>
                        <asp:Literal ID="litReplyError" runat="server" />
                    </div>
                </asp:Panel>
            </div>

            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">
                    <i class="bi bi-x me-1"></i>Đóng
                </button>
                <%-- Nút Đóng yêu cầu (vàng) — chỉ hiện khi chưa đóng --%>
                <asp:Button ID="btnDongYeuCau" runat="server" CssClass="btn"
                    Text="Đóng yêu cầu" OnClick="btnDongYeuCau_Click" CausesValidation="false"
                    style="background:#f59e0b;color:#fff;border:none;border-radius:10px;padding:8px 18px;font-weight:600" />
                <%-- Nút Gửi phản hồi (xanh) — chỉ hiện với TUVAN --%>
                <asp:Button ID="btnGuiPhanHoi" runat="server" CssClass="btn btn-primary"
                    Text="Gửi phản hồi" OnClick="btnGuiPhanHoi_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script src="/Scripts/validation_data_input.js"></script>
<script>
// Server flag: báo site.js mở modal chi tiết
window.showGopYTuvanDetail = '<%= ShowDetail %>';

// ── AdminValidator — modal Phản hồi tư vấn ────────────────────────────────
document.addEventListener('DOMContentLoaded', function () {
    AdminValidator.init('form', {
        '<%= txtReply.ClientID %>': [
            AdminValidator.rules.required('Vui lòng nhập nội dung phản hồi.'),
            AdminValidator.rules.minLen(10, 'Nội dung phản hồi tối thiểu 10 ký tự.'),
            AdminValidator.rules.maxLen(4000, 'Nội dung phản hồi tối đa 4000 ký tự.')
        ]
    }, { triggerButtonId: '<%= btnGuiPhanHoi.ClientID %>' });
});
</script>
</asp:Content>
