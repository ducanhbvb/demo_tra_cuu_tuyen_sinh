<%@ Page Title="Tìm trường theo ngành" Language="C#" MasterPageFile="~/Client/Site.Master"
    CodeBehind="TimKiemTheoNganh.aspx.cs" Inherits="TimKiemTheoNganh_Page" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server" />

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="mb-3">
    <h4 class="mb-1"><i class="bi bi-diagram-3 me-2 text-primary"></i>Tìm trường theo ngành học</h4>
    <p class="text-muted small mb-0">Chọn ngành học để xem tất cả các trường đào tạo và điểm chuẩn tương ứng.</p>
</div>

<%-- BỘ LỌC --%>
<div class="card shadow-sm mb-4">
    <div class="card-body">
        <div class="row g-2 align-items-end">
            <div class="col-md-4">
                <label class="form-label small fw-semibold">Nhóm ngành</label>
                <asp:DropDownList ID="ddlDanhMuc" runat="server" CssClass="form-select"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlDanhMuc_Changed">
                    <asp:ListItem Value="0">-- Tất cả nhóm ngành --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-semibold">Ngành học</label>
                <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                    <asp:ListItem Value="0">-- Tất cả ngành --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label class="form-label small fw-semibold">Năm tuyển sinh</label>
                <asp:DropDownList ID="ddlNam" runat="server" CssClass="form-select">
                    <asp:ListItem Value="0">-- Tất cả --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnTim" runat="server" Text="Tìm kiếm"
                    CssClass="btn btn-primary w-100" OnClick="btnTim_Click" />
            </div>
        </div>
    </div>
</div>

<%-- KẾT QUẢ --%>
<div class="d-flex justify-content-between align-items-center mb-2">
    <span class="text-muted small">
        Tìm thấy <b><asp:Literal ID="litTong" runat="server">0</asp:Literal></b> kết quả
    </span>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false">
    <div class="text-center py-5 text-muted">
        <i class="bi bi-inbox display-4 d-block mb-2 opacity-50"></i>
        <p>Không tìm thấy kết quả phù hợp.</p>
    </div>
</asp:Panel>

<asp:Panel ID="pnlResult" runat="server">
<div class="table-responsive">
<asp:GridView ID="gvKetQua" runat="server" AutoGenerateColumns="false"
    CssClass="table table-hover align-middle"
    EmptyDataText="Không có dữ liệu.">
    <Columns>
        <asp:TemplateField HeaderText="Trường">
            <ItemTemplate>
                <a href='<%# ResolveUrl("~/Client/ChiTietTruong.aspx?slug=" + Eval("TruongSlug")) %>'
                   class="fw-semibold text-decoration-none">
                    <%# Eval("TenTruong") %>
                </a>
                <div class="small text-muted"><%# Eval("TinhThanh") %></div>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="TenChuyenNganh" HeaderText="Ngành học" />
        <asp:BoundField DataField="TenCapBac"      HeaderText="Cấp bậc" />
        <asp:BoundField DataField="TenPhuongThuc"  HeaderText="Phương thức xét tuyển" />
        <asp:BoundField DataField="TenPhuongThuc1"  HeaderText="Phương thức xét tuyển" />
        <asp:TemplateField HeaderText="Năm">
            <ItemTemplate><span class="badge bg-secondary"><%# Eval("NamTuyenSinh") %></span></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Điểm chuẩn">
            <ItemTemplate>
                <%# Eval("DiemChuanNamTruoc") == DBNull.Value
                    ? "<span class='text-muted'>&#8212;</span>"
                    : "<b class='text-danger'>" + string.Format("{0:F2}", Eval("DiemChuanNamTruoc")) + "</b>" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Học phí/năm">
            <ItemTemplate>
                <%# Eval("HocPhi") == DBNull.Value
                    ? "<span class='text-muted'>&#8212;</span>"
                    : string.Format("{0:N0}", Eval("HocPhi")) + " đ" %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Tổ hợp">
            <ItemTemplate>
                <span class="small text-muted"><%# Eval("ToHopMonHoc") %></span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="So sánh">
            <ItemTemplate>
                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-secondary"
                    CommandName="ThemSoSanh"
                    CommandArgument='<%# Eval("MaTruong") %>'
                    OnCommand="gvKetQua_Command"
                    ToolTip="Thêm vào so sánh">
                    <i class="bi bi-plus-circle"></i>
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
</div>
</asp:Panel>

<%-- PHÂN TRANG --%>
<nav class="mt-2">
    <ul class="pagination pagination-sm justify-content-center flex-wrap">
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
</nav>

<asp:Literal ID="litThongBao" runat="server" />

</asp:Content>
