<%@ Page Title="Tra cứu điểm chuẩn" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="TraCuuDiemChuan.aspx.cs" Inherits="TraCuuDiemChuan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="page-header-bar">
    <h3 class="fw-bold mb-0"><i class="bi bi-bar-chart-line text-primary me-2"></i>Tra cứu điểm chuẩn</h3>
    <p class="page-header-sub">Tra cứu điểm chuẩn tuyển sinh theo trường, ngành học và năm.</p>
</div>

<%-- BỘ LỌC --%>
<div class="card border-0 shadow-sm mb-4">
    <div class="card-body">
        <div class="row g-3 align-items-end">
            <div class="col-6 col-md-3">
                <label class="form-label fw-semibold">Tỉnh / TP</label>
                <asp:DropDownList ID="ddlTinh" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-3">
                <label class="form-label fw-semibold">Ngành học</label>
                <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả ngành --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-2">
                <label class="form-label fw-semibold">Phương thức</label>
                <asp:DropDownList ID="ddlPhuongThuc" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-2">
                <label class="form-label fw-semibold">Năm</label>
                <asp:DropDownList ID="ddlNam" runat="server" CssClass="form-select">
                    <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-6 col-md-1">
                <label class="form-label fw-semibold">Điểm từ</label>
                <asp:TextBox ID="txtDiemTu" runat="server" CssClass="form-control" placeholder="0" />
            </div>
            <div class="col-6 col-md-1">
                <label class="form-label fw-semibold">Đến</label>
                <asp:TextBox ID="txtDiemDen" runat="server" CssClass="form-control" placeholder="30" />
            </div>
            <div class="col-12 d-flex gap-2">
                <asp:Button ID="btnLoc" runat="server" Text="Tra cứu"
                    CssClass="btn btn-primary" OnClick="btnLoc_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Xóa bộ lọc"
                    CssClass="btn btn-outline-secondary" OnClick="btnReset_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
</div>

<%-- KẾT QUẢ --%>
<div class="d-flex justify-content-between mb-3">
    <span class="text-muted">
        Tìm thấy <strong><asp:Literal ID="litTong" runat="server" /></strong> kết quả
    </span>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false">
    <div class="text-center py-5 text-muted">
        <i class="bi bi-inbox display-4 d-block mb-2 opacity-50"></i>
        <p class="mb-0">Không tìm thấy dữ liệu phù hợp với bộ lọc.</p>
    </div>
</asp:Panel>

<asp:Panel ID="pnlResult" runat="server">
<div class="table-responsive">
    <asp:GridView ID="gvKetQua" runat="server"
        CssClass="table table-hover table-bordered align-middle"
        AutoGenerateColumns="false" GridLines="None"
        EnableViewState="false"
        EmptyDataText="">
        <Columns>
            <asp:TemplateField HeaderText="Trường">
                <ItemTemplate>
                    <a href='<%# ResolveUrl("~/Client/ChiTietTruong.aspx?slug=" + Eval("TruongSlug")) %>'
                       class="fw-semibold text-decoration-none"><%# Eval("TenTruong") %></a>
                    <div class="small text-muted"><%# Eval("TinhThanh") %></div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TenChuyenNganh"     HeaderText="Ngành"       HtmlEncode="true" />
            <asp:BoundField DataField="TenPhuongThuc"      HeaderText="Phương thức" HtmlEncode="true"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:BoundField DataField="ToHopMonHoc"        HeaderText="Tổ hợp"      HtmlEncode="true"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:TemplateField HeaderText="Năm">
                <ItemTemplate>
                    <span class="badge year-pill"><%# Eval("NamTuyenSinh") %></span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Điểm chuẩn">
                <ItemTemplate>
                    <%# Eval("DiemChuanNamTruoc") == DBNull.Value
                        ? "<span class='text-muted'>&#8212;</span>"
                        : "<b class='text-danger'>" + string.Format("{0:F2}", Eval("DiemChuanNamTruoc")) + "</b>" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ChiTieu" HeaderText="Chỉ tiêu"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:HyperLinkField Text="Chi tiết" HeaderText=""
                DataNavigateUrlFields="TruongSlug"
                DataNavigateUrlFormatString="ChiTietTruong.aspx?slug={0}"
                ControlStyle-CssClass="btn btn-sm btn-outline-primary" />
        </Columns>
    </asp:GridView>
</div>
</asp:Panel>

<%-- PHÂN TRANG --%>
<nav class="mt-3">
    <ul class="pagination pagination-sm justify-content-center flex-wrap gap-1">
        <asp:Repeater ID="rptPaging" runat="server" OnItemCommand="rptPaging_ItemCommand">
            <ItemTemplate>
                <li class='page-item <%# (bool)Eval("IsActive") ? "active" : (bool)Eval("IsDisabled") ? "disabled" : "" %>'>
                    <%# (bool)Eval("IsDisabled")
                        ? "<span class=\"page-link px-2\">" + Eval("PageText") + "</span>"
                        : "<span style='display:none'></span>" %>
                    <asp:LinkButton runat="server" CssClass="page-link px-2"
                        Visible='<%# !(bool)Eval("IsDisabled") %>'
                        CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>'><%# Eval("PageText") %></asp:LinkButton>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</nav>

</asp:Content>
