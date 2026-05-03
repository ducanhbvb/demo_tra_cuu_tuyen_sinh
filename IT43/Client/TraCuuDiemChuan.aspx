<%@ Page Title="Tra cứu điểm chuẩn" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="TraCuuDiemChuan.aspx.cs" Inherits="TraCuuDiemChuan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<h3 class="fw-bold mb-4"><i class="bi bi-bar-chart-line text-primary me-2"></i>Tra cứu điểm chuẩn</h3>

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

<div class="table-responsive">
    <asp:GridView ID="gvKetQua" runat="server"
        CssClass="table table-hover table-bordered align-middle"
        AutoGenerateColumns="false" GridLines="None"
        EmptyDataText="Không tìm thấy dữ liệu phù hợp với bộ lọc.">
        <Columns>
            <asp:BoundField DataField="TenTruong"          HeaderText="Trường"          HtmlEncode="true" />
            <asp:BoundField DataField="TinhThanh"          HeaderText="Tỉnh/TP"         HtmlEncode="true"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:BoundField DataField="TenChuyenNganh"     HeaderText="Ngành"           HtmlEncode="true" />
            <asp:BoundField DataField="TenPhuongThuc"      HeaderText="Phương thức"     HtmlEncode="true"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:BoundField DataField="ToHopMonHoc"        HeaderText="Tổ hợp"          HtmlEncode="true"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:BoundField DataField="NamTuyenSinh"       HeaderText="Năm"             />
            <asp:BoundField DataField="DiemChuanNamTruoc"  HeaderText="Điểm chuẩn"
                DataFormatString="{0:F2}" ItemStyle-CssClass="fw-bold text-danger" />
            <asp:BoundField DataField="ChiTieu"            HeaderText="Chỉ tiêu"
                HeaderStyle-CssClass="col-hide-mobile" ItemStyle-CssClass="col-hide-mobile" />
            <asp:HyperLinkField Text="Chi tiết" HeaderText=""
                DataNavigateUrlFields="TruongSlug"
                DataNavigateUrlFormatString="ChiTietTruong.aspx?slug={0}"
                ControlStyle-CssClass="btn btn-sm btn-outline-primary" />
        </Columns>
    </asp:GridView>
</div>

<%-- PHÂN TRANG --%>
<div class="d-flex justify-content-center mt-3">
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
