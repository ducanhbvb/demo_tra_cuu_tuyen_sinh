<%@ Page Title="Quản lý tài khoản" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyTaiKhoan.aspx.cs" Inherits="Admin_QuanLyTaiKhoan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<h5 class="fw-bold mb-2">Quản lý tài khoản</h5>

<div class="admin-filter-bar">
    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
        placeholder="Tìm theo email..." style="max-width:240px;" />
    <asp:DropDownList ID="ddlQuyen" runat="server" CssClass="form-select" style="max-width:150px;">
        <asp:ListItem Value="">-- Quyền --</asp:ListItem>
        <asp:ListItem Value="1">Admin</asp:ListItem>
        <asp:ListItem Value="2">Trường học</asp:ListItem>
        <asp:ListItem Value="3">Học sinh</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc"
        CssClass="btn-filter" OnClick="btnLoc_Click" />
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="card border-0 shadow-sm">
    <div class="card-body p-0">
        <asp:GridView ID="gvTK" runat="server"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaTaiKhoan" OnRowCommand="gvTK_RowCommand"
            EmptyDataText="Không có kết quả.">
            <Columns>
                <asp:BoundField DataField="Email"     HeaderText="Email"    HtmlEncode="true" />
                <asp:BoundField DataField="TenQuyen"  HeaderText="Quyền"    HtmlEncode="true" />
                <asp:BoundField DataField="NgayTao"   HeaderText="Ngày tạo" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:TemplateField HeaderText="Xác nhận email">
                    <ItemTemplate>
                        <%# (bool)Eval("EmailDaXacNhan")
                            ? "<span class='badge bg-success'>Đã XN</span>"
                            : "<span class='badge bg-warning text-dark'>Chưa XN</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# (bool)Eval("TrangThai")
                            ? "<span class='badge bg-success'>Hoạt động</span>"
                            : "<span class='badge bg-danger'>Vô hiệu</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:LinkButton runat="server"
                            CommandName="ToggleTT"
                            CommandArgument='<%# Eval("MaTaiKhoan") + "," + Eval("TrangThai") %>'
                            CssClass='<%# (bool)Eval("TrangThai") ? "btn btn-xs btn-warning" : "btn btn-xs btn-success" %>'>
                            <%# (bool)Eval("TrangThai") ? "Vô hiệu hóa" : "Kích hoạt" %>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
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

</asp:Content>
