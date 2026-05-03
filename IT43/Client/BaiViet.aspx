<%@ Page Title="Tin tức & Bài viết" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="BaiViet.aspx.cs" Inherits="BaiViet_Page" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex justify-content-between align-items-center mb-4">
    <h4 class="fw-bold mb-0"><i class="bi bi-newspaper me-2 text-primary"></i>Tin tức &amp; Bài viết</h4>
</div>

<%-- Filter --%>
<div class="card border-0 shadow-sm mb-4">
    <div class="card-body py-2">
        <div class="row g-2 align-items-end">
            <div class="col-md-5">
                <asp:DropDownList ID="ddlTruong" runat="server" CssClass="form-select form-select-sm"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlTruong_Changed">
                    <asp:ListItem Value="">-- Tất cả trường --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-auto">
                <asp:Button ID="btnXemTat" runat="server" Text="Xem tất cả"
                    CssClass="btn btn-sm btn-outline-secondary" OnClick="btnXemTat_Click"
                    CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

<%-- Danh sách bài viết --%>
<asp:Repeater ID="rptBaiViet" runat="server">
    <HeaderTemplate><div class="row g-4"></HeaderTemplate>
    <ItemTemplate>
        <div class="col-md-6 col-lg-4">
            <div class="card h-100 border-0 shadow-sm card-hover">
                <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>'>
                    <img src='<%# (Eval("AnhChinh") == null || Eval("AnhChinh") == DBNull.Value || string.IsNullOrEmpty(Eval("AnhChinh").ToString())) ? "/Resources/Images/no-image.png" : Eval("AnhChinh").ToString() %>'
                         class="card-img-top" style="height:180px;object-fit:cover;"
                         alt='<%# Eval("TieuDe") %>' onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                </a>
                <div class="card-body d-flex flex-column">
                    <div class="d-flex gap-2 mb-2 flex-wrap">
                        <%# (Eval("TenTruong") != DBNull.Value && Eval("TenTruong") != null)
                            ? "<span class='badge bg-primary-subtle text-primary-emphasis small'>" + Server.HtmlEncode(Eval("TenTruong").ToString()) + "</span>"
                            : "" %>
                        <span class="text-muted small"><i class="bi bi-eye me-1"></i><%# Eval("LuotXem") %></span>
                    </div>
                    <h6 class="fw-semibold mb-2">
                        <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="text-body text-decoration-none stretched-link-title">
                            <%# Eval("TieuDe") %>
                        </a>
                    </h6>
                    <div class="mt-auto d-flex justify-content-between align-items-center">
                        <small class="text-muted"><%# ((DateTime)Eval("NgayDang")).ToString("dd/MM/yyyy") %></small>
                        <a href='ChiTietBaiViet.aspx?slug=<%# Eval("Slug") %>' class="btn btn-xs btn-outline-primary btn-sm">Đọc tiếp</a>
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
</asp:Repeater>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="text-center py-5 text-muted">
    <i class="bi bi-newspaper fs-1 opacity-25"></i>
    <p class="mt-2">Chưa có bài viết nào.</p>
</asp:Panel>

<%-- Phân trang --%>
<div class="d-flex justify-content-between align-items-center mt-4">
    <span class="text-muted small">Tổng: <strong><asp:Literal ID="litTong" runat="server" /></strong> bài viết</span>
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
