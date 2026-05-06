<%@ Page Title="Nhật ký hệ thống" Language="C#" MasterPageFile="~/Admin/Admin.Master"
    CodeBehind="QuanLyLogs.aspx.cs" Inherits="Admin_QuanLyLogs" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server" />

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<%-- ══ Page Header ══════════════════════════════════════════════════════════ --%>
<div class="page-header mb-4">
    <div>
        <h4 class="page-header-title">
            <i class="bi bi-journal-text" style="color:var(--accent)"></i> Nhật ký hệ thống
        </h4>
        <div class="page-header-sub">Theo dõi toàn bộ hoạt động và sự kiện của hệ thống</div>
    </div>
    <asp:Button ID="btnExportCsv" runat="server" Text="⬇ Xuất CSV"
        CssClass="btn btn-outline-success fw-semibold"
        style="border-radius:10px;"
        OnClick="btnExportCsv_Click" UseSubmitBehavior="true" />
</div>

<%-- ══ Stat Cards ════════════════════════════════════════════════════════════ --%>
<div class="row g-3 mb-4">
    <div class="col-sm-4">
        <div class="stat-card grad-indigo">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litTong" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Tổng bản ghi</div>
                </div>
                <i class="bi bi-journal-text stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-sm-4">
        <div class="stat-card grad-green">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litThanhCong" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Thành công</div>
                </div>
                <i class="bi bi-check-circle-fill stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-sm-4">
        <div class="stat-card grad-rose">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litThatBai" runat="server">0</asp:Literal></div>
                    <div class="stat-label">Thất bại</div>
                </div>
                <i class="bi bi-x-circle-fill stat-icon"></i>
            </div>
        </div>
    </div>
</div>

<%-- ══ Bộ lọc ═════════════════════════════════════════════════════════════════ --%>
<div class="admin-card mb-4">
    <div class="admin-card-header">
        <div class="admin-card-title">
            <i class="bi bi-funnel me-1"></i> Bộ lọc
        </div>
    </div>
    <div class="admin-card-body">
        <div class="row g-2 align-items-end">
            <div class="col-6 col-md-3 col-lg-2">
                <label class="form-label mb-1">Hành động</label>
                <asp:DropDownList ID="ddlHanhDong" runat="server" CssClass="form-select form-select-sm">
                    <asp:ListItem Value="">Tất cả</asp:ListItem>
                    <asp:ListItem Value="DANG_NHAP">Đăng nhập</asp:ListItem>
                    <asp:ListItem Value="DOI_MAT_KHAU">Đổi mật khẩu</asp:ListItem>
                    <asp:ListItem Value="UPGRADE_HASH">Nâng cấp hash</asp:ListItem>
                    <asp:ListItem Value="APPLICATION_ERROR">Lỗi hệ thống</asp:ListItem>
                    <asp:ListItem Value="SEND_EMAIL_FAIL">Lỗi gửi email</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-3 col-lg-2">
                <label class="form-label mb-1">Kết quả</label>
                <asp:DropDownList ID="ddlIsSuccess" runat="server" CssClass="form-select form-select-sm">
                    <asp:ListItem Value="">Tất cả</asp:ListItem>
                    <asp:ListItem Value="1">✅ Thành công</asp:ListItem>
                    <asp:ListItem Value="0">❌ Thất bại</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-3 col-lg-2">
                <label class="form-label mb-1">Thiết bị</label>
                <asp:DropDownList ID="ddlThietBi" runat="server" CssClass="form-select form-select-sm">
                    <asp:ListItem Value="">Tất cả</asp:ListItem>
                    <asp:ListItem Value="Desktop">Desktop</asp:ListItem>
                    <asp:ListItem Value="Mobile">Mobile</asp:ListItem>
                    <asp:ListItem Value="Tablet">Tablet</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-3 col-lg-3">
                <label class="form-label mb-1">Email tài khoản</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control form-control-sm"
                    placeholder="Tìm theo email..." />
            </div>
            <div class="col-12 col-lg-3 d-flex gap-2 flex-wrap">
                <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc"
                    CssClass="btn-filter" OnClick="btnLoc_Click" />
                <asp:Button ID="btnReset" runat="server" Text="↺ Reset"
                    CssClass="btn-filter"
                    style="border-color:#94a3b8;color:#64748b;"
                    OnClick="btnReset_Click" />
            </div>
        </div>
    </div>
</div>

<%-- ══ Danh sách log ══════════════════════════════════════════════════════════ --%>
<div class="admin-card">
    <div class="admin-card-header">
        <div class="admin-card-title">
            <i class="bi bi-list-ul me-2"></i>Danh sách nhật ký
        </div>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="false"
            EnableViewState="false"
            CssClass="table table-hover table-sm mb-0 align-middle"
            EmptyDataText="Không có bản ghi nào phù hợp với bộ lọc.">
            <Columns>
                <asp:BoundField DataField="LogID" HeaderText="ID"
                    ItemStyle-CssClass="text-muted small" ItemStyle-Width="60px" />

                <asp:TemplateField HeaderText="Thời gian" ItemStyle-Width="130px">
                    <ItemTemplate>
                        <div class="small fw-500"><%# Eval("ThoiGian", "{0:dd/MM/yyyy}") %></div>
                        <div class="small text-muted"><%# Eval("ThoiGian", "{0:HH:mm:ss}") %></div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Tài khoản">
                    <ItemTemplate>
                        <div class="logs-email-cell">
                            <div class="logs-email-dot" style='<%# "background:" + GetEmailColor(Eval("Email")) %>'></div>
                            <span class="small"><%# FormatEmail(Eval("Email")) %></span>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Hành động" ItemStyle-Width="160px">
                    <ItemTemplate>
                        <span class='badge <%# GetActionBadgeClass(Eval("HanhDong").ToString()) %>'>
                            <%# GetActionLabel(Eval("HanhDong").ToString()) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Kết quả" ItemStyle-Width="90px">
                    <ItemTemplate>
                        <%# Eval("IsSuccess") != DBNull.Value && (bool)Eval("IsSuccess")
                            ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>OK</span>"
                            : "<span class='badge badge-soft-danger'><i class='bi bi-x-circle me-1'></i>Lỗi</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="IP / Thiết bị" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <div class="small text-monospace"><%# Eval("IPAddress") %></div>
                        <span class='badge <%# GetDeviceBadgeClass(Eval("LoaiThietBi").ToString()) %>'>
                            <i class='bi <%# GetDeviceIcon(Eval("LoaiThietBi").ToString()) %> me-1'></i><%# Eval("LoaiThietBi") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Mô tả">
                    <ItemTemplate>
                        <span class="small text-truncate d-inline-block" style="max-width:260px"
                              title='<%# Server.HtmlEncode(Eval("MoTa") == DBNull.Value ? "" : Eval("MoTa").ToString()) %>'>
                            <%# Eval("MoTa") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Mã lỗi" ItemStyle-Width="90px">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("MaLoi") as string) ? ""
                            : "<span class='badge badge-soft-warning'>" + Server.HtmlEncode(Eval("MaLoi").ToString()) + "</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <%-- Phân trang --%>
    <div class="d-flex align-items-center justify-content-between px-3 py-3 border-top" style="flex-wrap:wrap;gap:8px;">
        <div class="small text-muted">
            Trang <strong><asp:Literal ID="litCurrentPage" runat="server">1</asp:Literal></strong>
            / <asp:Literal ID="litTotalPages" runat="server">1</asp:Literal>
            &nbsp;·&nbsp; Tổng <strong><asp:Literal ID="litTongPaging" runat="server">0</asp:Literal></strong> bản ghi
        </div>
        <ul class="pagination pagination-sm mb-0">
            <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
                <ItemTemplate>
                    <li class='page-item <%# (bool)Eval("IsActive") ? "active" : "" %>'>
                        <asp:LinkButton runat="server" CssClass="page-link"
                            CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>'>
                            <%# Eval("PageText") %>
                        </asp:LinkButton>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<%-- Export CSV button fix đã chuyển vào Scripts/site.js --%>
</asp:Content>
