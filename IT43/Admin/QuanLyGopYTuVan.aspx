<%@ Page Title="Góp ý / Tư vấn" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="QuanLyGopYTuVan.aspx.cs" Inherits="Admin_QuanLyGopYTuVan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:HiddenField ID="hfLoai" runat="server" Value="TUVAN" />
<asp:Button ID="btnDoTab" runat="server" CssClass="d-none"
    OnClick="btnDoTab_Click" CausesValidation="false" />

<%-- Browser-style tabs --%>
<div class="bstab-bar">
    <button type="button" class='bstab <%: hfLoai.Value == "TUVAN" ? "active" : "" %>'
        onclick="switchTab('TUVAN')">
        <i class="bi bi-chat-dots-fill"></i>
        <span>Tư vấn</span>
        <asp:Literal ID="litSoTuVan" runat="server" />
    </button>
    <button type="button" class='bstab <%: hfLoai.Value == "GOPY" ? "active" : "" %>'
        onclick="switchTab('GOPY')">
        <i class="bi bi-flag-fill"></i>
        <span>Góp ý</span>
        <asp:Literal ID="litSoGopY" runat="server" />
    </button>
</div>

<%-- Filter trạng thái --%>
<div class="bstab-filter">
    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-select form-select-sm"
        AutoPostBack="true" OnSelectedIndexChanged="ddlTrangThai_Changed"
        style="min-width:180px;max-width:220px;">
        <asp:ListItem Value="">-- Tất cả trạng thái --</asp:ListItem>
        <asp:ListItem Value="0">Chờ xử lý</asp:ListItem>
        <asp:ListItem Value="1">Đã phản hồi</asp:ListItem>
        <asp:ListItem Value="2">Đã đóng</asp:ListItem>
    </asp:DropDownList>
</div>

<asp:Literal ID="litThongBao" runat="server" />

<div class="card border-0 shadow-sm">
    <div class="card-body p-0">
        <asp:GridView ID="gvDanhSach" runat="server"
            CssClass="table table-hover table-sm align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="ID" OnRowCommand="gvDanhSach_RowCommand"
            EmptyDataText="Chưa có dữ liệu.">
            <Columns>
                <%-- Cột Họ tên: chỉ hiện ở tab Tư vấn --%>
                <asp:TemplateField HeaderText="Họ tên">
                    <ItemTemplate>
                        <%# hfLoai.Value == "TUVAN"
                            ? Server.HtmlEncode(Eval("HoTen") == DBNull.Value ? "" : Eval("HoTen").ToString())
                            : "<span class='text-muted fst-italic small'>Ẩn danh</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <%-- Cột Email: chỉ tư vấn mới có --%>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# hfLoai.Value == "TUVAN"
                            ? Server.HtmlEncode(Eval("Email") == DBNull.Value ? "" : Eval("Email").ToString())
                            : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TenTruong" HeaderText="Trường"   HtmlEncode="true" />
                <asp:BoundField DataField="NgayGui"   HeaderText="Ngày gửi" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# Convert.ToByte(Eval("TrangThai")) == 0
                            ? "<span class='badge bg-warning text-dark'>Chờ xử lý</span>"
                            : Convert.ToByte(Eval("TrangThai")) == 1
                                ? "<span class='badge bg-success'>Đã phản hồi</span>"
                                : "<span class='badge bg-secondary'>Đã đóng</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Nội dung">
                    <ItemTemplate>
                        <span class="text-truncate d-inline-block" style="max-width:220px;"
                              title='<%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString()) %>'>
                            <%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" :
                                Eval("NoiDung").ToString().Length > 50
                                    ? Eval("NoiDung").ToString().Substring(0, 50) + "..."
                                    : Eval("NoiDung").ToString()) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="200px" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <%-- Xem chi tiết --%>
                        <asp:LinkButton runat="server" CommandName="XemChiTiet"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn btn-xs btn-outline-info me-1">
                            <i class="bi bi-eye"></i>
                        </asp:LinkButton>
                        <%-- Phản hồi (chỉ khi Chờ xử lý) --%>
                        <asp:LinkButton runat="server" CommandName="PhanHoi"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn btn-xs btn-outline-success me-1"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) == 0 %>'>
                            <i class="bi bi-check2"></i> Phản hồi
                        </asp:LinkButton>
                        <%-- Roll back: đưa về Chờ xử lý (khi đã phản hồi) --%>
                        <asp:LinkButton runat="server" CommandName="RollBack"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn btn-xs btn-outline-warning me-1"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) == 1 %>'
                            OnClientClick="return confirm('Hoàn tác về Chờ xử lý?')">
                            <i class="bi bi-arrow-counterclockwise"></i> Hoàn tác
                        </asp:LinkButton>
                        <%-- Đóng (khi chưa đóng) --%>
                        <asp:LinkButton runat="server" CommandName="Dong"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn btn-xs btn-outline-secondary"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) != 2 %>'
                            OnClientClick="return confirm('Đóng yêu cầu này?')">
                            Đóng
                        </asp:LinkButton>
                        <%-- Mở lại (khi đã đóng) --%>
                        <asp:LinkButton runat="server" CommandName="MoLai"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn btn-xs btn-outline-primary"
                            Visible='<%# Convert.ToByte(Eval("TrangThai")) == 2 %>'>
                            <i class="bi bi-arrow-repeat"></i> Mở lại
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

<%-- MODAL CHI TIẾT --%>
<div class="modal fade" id="modalChiTiet" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title fw-bold">
                    <i class="bi bi-chat-text me-2"></i>Chi tiết nội dung
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="row g-3 mb-3">
                    <div class="col-md-6">
                        <label class="text-muted small fw-semibold">HỌ TÊN</label>
                        <div><asp:Literal ID="litCTHoTen" runat="server" /></div>
                    </div>
                    <div class="col-md-6">
                        <label class="text-muted small fw-semibold">EMAIL</label>
                        <div><asp:Literal ID="litCTEmail" runat="server" /></div>
                    </div>
                    <div class="col-md-6">
                        <label class="text-muted small fw-semibold">TRƯỜNG</label>
                        <div><asp:Literal ID="litCTTruong" runat="server" /></div>
                    </div>
                    <div class="col-md-3">
                        <label class="text-muted small fw-semibold">NGÀY GỬI</label>
                        <div><asp:Literal ID="litCTNgay" runat="server" /></div>
                    </div>
                    <div class="col-md-3">
                        <label class="text-muted small fw-semibold">TRẠNG THÁI</label>
                        <div><asp:Literal ID="litCTTrangThai" runat="server" /></div>
                    </div>
                </div>
                <div>
                    <label class="text-muted small fw-semibold">NỘI DUNG ĐẦY ĐỦ</label>
                    <div class="border rounded p-3 bg-light mt-1" style="white-space:pre-wrap; min-height:100px;">
                        <asp:Literal ID="litCTNoiDung" runat="server" />
                    </div>
                </div>
                <asp:Panel ID="pnlGhiChu" runat="server" CssClass="mt-3">
                    <label class="text-muted small fw-semibold">GHI CHÚ ADMIN</label>
                    <div class="border rounded p-3 bg-light mt-1">
                        <asp:Literal ID="litCTGhiChu" runat="server" />
                    </div>
                </asp:Panel>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Đóng</button>
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// Chuyển tab: set hfLoai rồi click btnDoTab ẩn
function switchTab(loai) {
    document.getElementById('<%= hfLoai.ClientID %>').value = loai;
    document.getElementById('<%= btnDoTab.ClientID %>').click();
}

// Mở modal chi tiết nếu server báo
if ('<%= ShowDetail %>' === 'true') {
    var m = new bootstrap.Modal(document.getElementById('modalChiTiet'));
    m.show();
}
</script>
</asp:Content>
