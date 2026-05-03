<%@ Page Title="Danh sách yêu thích" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="WishList.aspx.cs" Inherits="Profile_WishList" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<h3 class="fw-bold mb-4"><i class="bi bi-heart-fill text-danger me-2"></i>Trường yêu thích</h3>

<asp:Literal ID="litThongBao" runat="server" />

<div class="row g-3">
    <asp:Repeater ID="rptWishList" runat="server" OnItemCommand="rptWishList_ItemCommand">
        <ItemTemplate>
            <div class="col-sm-6 col-md-4 col-lg-3">
                <div class="card border-0 shadow-sm h-100">
                    <a href='<%# "~/Client/ChiTietTruong.aspx?slug=" + Eval("TruongSlug") %>'>
                        <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string) ? ResolveUrl("~/Content/img/school-default.png") : Eval("AnhDaiDien") %>'
                             class="card-img-top" style="height:130px;object-fit:cover;" />
                    </a>
                    <div class="card-body p-3">
                        <h6 class="card-title fw-semibold mb-1">
                            <a href='<%# "~/Client/ChiTietTruong.aspx?slug=" + Eval("TruongSlug") %>'
                               class="text-decoration-none text-body"><%# Eval("TenTruong") %></a>
                        </h6>
                        <p class="text-muted small mb-2">
                            <i class="bi bi-geo-alt me-1"></i><%# Eval("TinhThanh") %>
                        </p>
                        <%# !string.IsNullOrEmpty(Eval("TenChuyenNganh") as string)
                            ? "<p class='text-muted small mb-2'><i class='bi bi-book me-1'></i>" + Eval("TenChuyenNganh") + "</p>"
                            : "" %>
                        <asp:LinkButton runat="server" CommandName="XoaWish"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn btn-sm btn-outline-danger w-100"
                            OnClientClick="return confirm('Xóa khỏi danh sách yêu thích?')">
                            <i class="bi bi-trash me-1"></i>Xóa
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false">
    <div class="col-12 text-center text-muted py-5">
        <i class="bi bi-heart display-4 d-block mb-2"></i>
        <p>Chưa có trường nào trong danh sách yêu thích.</p>
        <a href="~/Client/TimKiemTruong.aspx" class="btn btn-primary">Tìm kiếm trường</a>
    </div>
</asp:Panel>
</div>
</asp:Content>
