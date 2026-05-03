<%@ Page Title="Quản lý trường" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyTruong.aspx.cs" Inherits="Admin_QuanLyTruong" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex justify-content-between align-items-center mb-2">
    <h5 class="fw-bold mb-0">Danh sách trường</h5>
</div>

<%-- Filter + Thêm mới --%>
<div class="admin-filter-bar">
    <asp:TextBox ID="txtFilter" runat="server" CssClass="form-control"
        placeholder="Tên trường..." style="max-width:220px;" />
    <asp:DropDownList ID="ddlTinh" runat="server" CssClass="form-select" style="max-width:180px;">
        <asp:ListItem Value="">-- Tỉnh/TP --</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-select" style="max-width:160px;">
        <asp:ListItem Value="">-- Trạng thái --</asp:ListItem>
        <asp:ListItem Value="1">Hiển thị</asp:ListItem>
        <asp:ListItem Value="0">Đã ẩn</asp:ListItem>
    </asp:DropDownList>
    <asp:Button ID="btnLoc" runat="server" Text="🔍 Lọc"
        CssClass="btn-filter" OnClick="btnLoc_Click" />
    <span class="filter-spacer"></span>
    <a href="ChinhSuaTruong.aspx" class="btn-add-new">
        <i class="bi bi-plus-lg"></i>Thêm trường mới
    </a>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="card border-0 shadow-sm">
    <div class="card-body p-0">
        <asp:GridView ID="gvTruong" runat="server"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="MaTruong"
            OnRowCommand="gvTruong_RowCommand"
            EmptyDataText="Chưa có dữ liệu.">
            <Columns>
                <asp:BoundField DataField="MaTruong"  HeaderText="ID" />
                <asp:BoundField DataField="TenTruong" HeaderText="Tên trường"   HtmlEncode="true" />
                <asp:BoundField DataField="TinhThanh" HeaderText="Tỉnh/TP"     HtmlEncode="true" />
                <asp:BoundField DataField="QuyMo"     HeaderText="Quy mô"      HtmlEncode="true" />
                <asp:TemplateField HeaderText="Kiểm định">
                    <ItemTemplate>
                        <%# (bool)Eval("KiemDinhChatLuong")
                            ? "<span class='badge bg-success'>Có</span>"
                            : "<span class='badge bg-secondary'>Không</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))
                            ? "<span class='badge bg-success'>Hiện</span>"
                            : "<span class='badge bg-secondary'>Ẩn</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Thao tác">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="ToggleTruong"
                            CommandArgument='<%# Eval("MaTruong") %>'
                            CssClass='<%# (Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))) ? "btn btn-xs btn-warning me-1" : "btn btn-xs btn-success me-1" %>'
                            OnClientClick="return confirm('Thay đổi trạng thái hiển thị trường này?')">
                            <%# (Eval("TrangThai") != DBNull.Value && Convert.ToBoolean(Eval("TrangThai"))) ? "Ẩn" : "Hiện" %>
                        </asp:LinkButton>
                        <a href='ChinhSuaTruong.aspx?id=<%# Eval("MaTruong") %>'
                           class="btn btn-xs btn-outline-primary me-1">Sửa</a>
                        <asp:LinkButton runat="server" CommandName="XoaTruong"
                            CommandArgument='<%# Eval("MaTruong") %>'
                            CssClass="btn btn-xs btn-outline-danger"
                            OnClientClick="return confirm('Xóa trường này? Toàn bộ dữ liệu liên quan sẽ bị xóa.')">
                            Xóa
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>

<%-- Phân trang --%>
<div class="d-flex justify-content-between align-items-center mt-3">
    <span class="text-muted small">
        Tổng: <strong><asp:Literal ID="litTong" runat="server" /></strong> trường
    </span>
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
