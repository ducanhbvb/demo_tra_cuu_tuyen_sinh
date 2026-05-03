<%@ Page Title="Tìm kiếm trường" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="TimKiemTruong.aspx.cs" Inherits="TimKiemTruong" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<h3 class="fw-bold mb-4"><i class="bi bi-search text-primary me-2"></i>Tìm kiếm trường đại học</h3>

<%-- BỘ LỌC --%>
<div class="card border-0 shadow-sm mb-4">
    <div class="card-body">
        <div class="row g-3 align-items-end">
            <div class="col-12 col-md-4">
                <label class="form-label fw-semibold">Tên trường</label>
                <asp:TextBox ID="txtTen" runat="server" CssClass="form-control"
                    placeholder="Nhập tên trường..." />
            </div>
            <div class="col-6 col-md-3">
                <label class="form-label fw-semibold">Tỉnh / Thành phố</label>
                <asp:DropDownList ID="ddlTinh" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-2">
                <label class="form-label fw-semibold">Loại trường</label>
                <asp:DropDownList ID="ddlLoai" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                    <asp:ListItem Value="1">Công lập</asp:ListItem>
                    <asp:ListItem Value="2">Tư thục</asp:ListItem>
                    <asp:ListItem Value="3">Quốc tế</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-12 col-md-3">
                <label class="form-label fw-semibold">Ngành học</label>
                <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả ngành --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-12 d-flex gap-2">
                <asp:Button ID="btnLoc" runat="server" Text="Tìm kiếm"
                    CssClass="btn btn-primary" OnClick="btnLoc_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Xóa bộ lọc"
                    CssClass="btn btn-outline-secondary" OnClick="btnReset_Click"
                    CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

<%-- KẾT QUẢ --%>
<div class="d-flex justify-content-between align-items-center mb-3">
    <span class="text-muted">
        Tìm thấy <strong><asp:Literal ID="litTong" runat="server" /></strong> trường
    </span>
</div>

<div class="row g-3" id="truong-list">
    <asp:Repeater ID="rptKetQua" runat="server">
        <ItemTemplate>
            <div class="col-sm-6 col-md-4 col-xl-3">
                <div class="card border-0 shadow-sm h-100 truong-card">
                    <a href='ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'>
                        <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string) ? "/Resources/Images/no-image.png" : Eval("AnhDaiDien") %>'
                             class="card-img-top" style="height:140px;object-fit:cover;"
                             alt='<%# Eval("TenTruong") %>'
                             onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                    </a>
                    <div class="card-body p-3">
                        <h6 class="card-title fw-semibold mb-1">
                            <a href='ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'
                               class="text-decoration-none text-body"><%# Eval("TenTruong") %></a>
                        </h6>
                        <p class="text-muted small mb-1">
                            <i class="bi bi-geo-alt me-1"></i><%# Eval("TinhThanh") %>
                        </p>
                        <span class='badge <%# (byte?)Eval("LoaiTruong") == 1 ? "bg-primary" : (byte?)Eval("LoaiTruong") == 2 ? "bg-success" : "bg-warning text-dark" %>'>
                            <%# (byte?)Eval("LoaiTruong") == 1 ? "Công lập" : (byte?)Eval("LoaiTruong") == 2 ? "Tư thục" : "Quốc tế" %>
                        </span>
                        <%# (bool)Eval("KiemDinhChatLuong") ? "<span class='badge bg-info ms-1'>Kiểm định</span>" : "" %>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<%-- PHÂN TRANG --%>
<div class="d-flex justify-content-center mt-4">
    <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
        <HeaderTemplate><ul class="pagination"></HeaderTemplate>
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
