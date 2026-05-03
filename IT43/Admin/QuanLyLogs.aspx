<%@ Page Title="Quản lý Logs" Language="C#" MasterPageFile="~/Admin/Admin.Master"
    CodeBehind="QuanLyLogs.aspx.cs" Inherits="Admin_QuanLyLogs" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server" />

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<h5 class="fw-bold mb-2"><i class="bi bi-journal-text me-2 text-primary"></i>Nhật ký hệ thống</h5>

<%-- BỘ LỌC --%>
<div class="admin-filter-bar flex-wrap mb-3">
    <asp:DropDownList ID="ddlHanhDong" runat="server" CssClass="form-select" style="max-width:180px;">
        <asp:ListItem Value="">-- Hành động --</asp:ListItem>
        <asp:ListItem Value="DANG_NHAP">Đăng nhập</asp:ListItem>
        <asp:ListItem Value="DOI_MAT_KHAU">Đổi mật khẩu</asp:ListItem>
        <asp:ListItem Value="UPGRADE_HASH">Nâng cấp hash</asp:ListItem>
        <asp:ListItem Value="APPLICATION_ERROR">Lỗi hệ thống</asp:ListItem>
        <asp:ListItem Value="SEND_EMAIL_FAIL">Lỗi gửi email</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlIsSuccess" runat="server" CssClass="form-select" style="max-width:140px;">
        <asp:ListItem Value="">-- Kết quả --</asp:ListItem>
        <asp:ListItem Value="1">✅ Thành công</asp:ListItem>
        <asp:ListItem Value="0">❌ Thất bại</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlThietBi" runat="server" CssClass="form-select" style="max-width:130px;">
        <asp:ListItem Value="">-- Thiết bị --</asp:ListItem>
        <asp:ListItem Value="Desktop">Desktop</asp:ListItem>
        <asp:ListItem Value="Mobile">Mobile</asp:ListItem>
        <asp:ListItem Value="Tablet">Tablet</asp:ListItem>
    </asp:DropDownList>
    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
        placeholder="Email tài khoản..." style="max-width:200px;" />
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc"
        CssClass="btn-filter" OnClick="btnLoc_Click" />
    <asp:Button ID="btnReset" runat="server" Text="↺ Reset"
        CssClass="btn-filter" OnClick="btnReset_Click"
        style="border-color:#94a3b8;color:#64748b;" />
</div>

<%-- THỐNG KÊ --%>
<div class="row g-3 mb-3">
    <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center py-2">
            <div class="fw-bold text-primary fs-5"><asp:Literal ID="litTong" runat="server">0</asp:Literal></div>
            <div class="small text-muted">Tổng bản ghi</div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center py-2">
            <div class="fw-bold text-success fs-5"><asp:Literal ID="litThanhCong" runat="server">0</asp:Literal></div>
            <div class="small text-muted">Thành công</div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card border-0 shadow-sm text-center py-2">
            <div class="fw-bold text-danger fs-5"><asp:Literal ID="litThatBai" runat="server">0</asp:Literal></div>
            <div class="small text-muted">Thất bại</div>
        </div>
    </div>
</div>

<%-- DANH SÁCH LOG --%>
<div class="card border-0 shadow-sm">
    <div class="card-body p-0">
        <div class="table-responsive">
            <asp:GridView ID="gvLogs" runat="server" AutoGenerateColumns="false"
                CssClass="table table-hover table-sm mb-0 align-middle"
                EmptyDataText="Không có bản ghi nào.">
                <Columns>
                    <asp:BoundField DataField="LogID"    HeaderText="ID"    ItemStyle-CssClass="text-muted small" />
                    <asp:TemplateField HeaderText="Thời gian">
                        <ItemTemplate>
                            <span class="small"><%# Eval("ThoiGian", "{0:dd/MM/yyyy HH:mm:ss}") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tài khoản">
                        <ItemTemplate>
                            <%# Eval("Email") == DBNull.Value || string.IsNullOrEmpty(Eval("Email") as string)
                                ? "<span class='text-muted small fst-italic'>Ẩn danh</span>"
                                : "<span class='small'>" + Server.HtmlEncode(Eval("Email").ToString()) + "</span>" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Hành động">
                        <ItemTemplate>
                            <span class="badge bg-secondary"><%# Eval("HanhDong") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Kết quả">
                        <ItemTemplate>
                            <%# Eval("IsSuccess") != DBNull.Value && (bool)Eval("IsSuccess")
                                ? "<span class='badge bg-success'>✓ OK</span>"
                                : "<span class='badge bg-danger'>✗ Lỗi</span>" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="IP / Thiết bị">
                        <ItemTemplate>
                            <span class="small"><%# Eval("IPAddress") %></span>
                            <span class="badge bg-light text-dark ms-1"><%# Eval("LoaiThietBi") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Mô tả">
                        <ItemTemplate>
                            <span class="small text-truncate d-inline-block" style="max-width:280px"
                                  title='<%# Server.HtmlEncode(Eval("MoTa") == DBNull.Value ? "" : Eval("MoTa").ToString()) %>'>
                                <%# Eval("MoTa") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Mã lỗi">
                        <ItemTemplate>
                            <%# string.IsNullOrEmpty(Eval("MaLoi") as string) ? ""
                                : "<span class='badge bg-warning text-dark'>" + Server.HtmlEncode(Eval("MaLoi").ToString()) + "</span>" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>

<%-- PHÂN TRANG --%>
<div class="d-flex justify-content-center mt-3">
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

</asp:Content>
