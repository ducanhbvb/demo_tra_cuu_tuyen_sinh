<%@ Page Title="Góp ý / Tư vấn" Language="C#" MasterPageFile="~/TruongHoc/TruongHoc.Master"
   CodeBehind="GopYTuVan.aspx.cs" Inherits="TruongHoc_GopYTuVan" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<asp:HiddenField ID="hfLoai" runat="server" Value="TUVAN" />
<asp:Button ID="btnDoTab" runat="server" CssClass="d-none"
    OnClick="btnDoTab_Click" CausesValidation="false" />

<!-- Page Header -->
<div class="page-header mb-4">
    <div>
        <h4 class="page-header-title">
            <i class="bi bi-chat-dots-fill" style="color: var(--accent);"></i> Góp ý / Tư vấn
        </h4>
        <div class="page-header-sub">
            Cổng Trường / <strong>Tư vấn</strong> — Xem và phản hồi tư vấn, góp ý từ học sinh
        </div>
    </div>
</div>

<asp:Literal ID="litThongBao" runat="server" />

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
<div class="admin-filter-bar mt-3 mb-4">
    <asp:DropDownList ID="ddlTrangThai" runat="server" CssClass="form-select form-select-sm"
        AutoPostBack="true" OnSelectedIndexChanged="ddlTrangThai_Changed"
        style="min-width:180px;max-width:220px;">
        <asp:ListItem Value="">-- Tất cả trạng thái --</asp:ListItem>
        <asp:ListItem Value="0">Chờ xử lý</asp:ListItem>
        <asp:ListItem Value="1">Đã phản hồi</asp:ListItem>
        <asp:ListItem Value="2">Đã đóng</asp:ListItem>
    </asp:DropDownList>
</div>

<div class="alert-admin info py-3 small mb-4">
    <i class="bi bi-info-circle-fill"></i>
    <span>Danh sách tư vấn/góp ý gửi đến trường bạn. Bạn có thể <strong>xem chi tiết và phản hồi</strong> trực tiếp các yêu cầu tư vấn.</span>
</div>

<div class="admin-card">
    <div class="admin-card-body p-0">
        <div class="table-responsive">
        <asp:GridView ID="gvDanhSach" runat="server"
            EnableViewState="false"
            CssClass="table table-hover align-middle mb-0"
            AutoGenerateColumns="false" GridLines="None"
            DataKeyNames="ID" OnRowCommand="gvDanhSach_RowCommand"
            EmptyDataText="Chưa có dữ liệu.">
            <Columns>
                <%-- Họ tên: chỉ hiện ở tab Tư vấn --%>
                <asp:TemplateField HeaderText="Họ tên">
                    <ItemTemplate>
                        <%# hfLoai.Value == "TUVAN"
                            ? Server.HtmlEncode(Eval("HoTen") == DBNull.Value ? "" : Eval("HoTen").ToString())
                            : "<span class='text-muted fst-italic small'>Ẩn danh</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <%-- Email: chỉ tư vấn --%>
                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <%# hfLoai.Value == "TUVAN"
                            ? Server.HtmlEncode(Eval("Email") == DBNull.Value ? "" : Eval("Email").ToString())
                            : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="NgayGui"   HeaderText="Ngày gửi" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="Trạng thái">
                    <ItemTemplate>
                        <%# Convert.ToByte(Eval("TrangThai")) == 0
                            ? "<span class='badge badge-soft-warning'><i class='bi bi-hourglass-split me-1'></i>Chờ xử lý</span>"
                            : Convert.ToByte(Eval("TrangThai")) == 1
                                ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Đã phản hồi</span>"
                                : "<span class='badge badge-soft-secondary'><i class='bi bi-x-circle me-1'></i>Đã đóng</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Nội dung">
                    <ItemTemplate>
                        <span class="text-truncate d-inline-block" style="max-width:260px;"
                              title='<%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString()) %>'>
                            <%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" :
                                Eval("NoiDung").ToString().Length > 60
                                    ? Eval("NoiDung").ToString().Substring(0, 60) + "..."
                                    : Eval("NoiDung").ToString()) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Thao tác" ItemStyle-Width="100px">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="XemChiTiet"
                            CommandArgument='<%# Eval("ID") %>'
                            CssClass="btn-action btn-chart" title="Xem chi tiết">
                            <i class="bi bi-eye"></i>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        </div>
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

<%-- MODAL CHI TIẾT + REPLY --%>
<asp:HiddenField ID="hfMaTuVanHienTai" runat="server" Value="0" />
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
                <asp:Literal ID="litReplyError" runat="server" />
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
                        <label class="text-muted small fw-semibold">NGÀY GỬI</label>
                        <div><asp:Literal ID="litCTNgay" runat="server" /></div>
                    </div>
                    <div class="col-md-6">
                        <label class="text-muted small fw-semibold">TRẠNG THÁI</label>
                        <div><asp:Literal ID="litCTTrangThai" runat="server" /></div>
                    </div>
                </div>
                <div>
                    <label class="text-muted small fw-semibold">NỘI DUNG ĐẦY ĐỦ</label>
                    <div class="border rounded p-3 bg-light mt-1" style="white-space:pre-wrap; min-height:80px;">
                        <asp:Literal ID="litCTNoiDung" runat="server" />
                    </div>
                </div>
                <asp:Panel ID="pnlGhiChu" runat="server" CssClass="mt-3" Visible="false">
                    <label class="text-muted small fw-semibold">GHI CHÚ / PHẢN HỒI</label>
                    <div class="border rounded p-3 bg-light mt-1">
                        <asp:Literal ID="litCTGhiChu" runat="server" />
                    </div>
                </asp:Panel>

                <%-- Thread tư vấn (chỉ hiện tab TUVAN) --%>
                <asp:Panel ID="pnlThread" runat="server" CssClass="mt-3" Visible="false">
                    <label class="text-muted small fw-semibold">LỊCH SỬ PHẢN HỒI</label>
                    <div class="border rounded p-3 mt-1" style="max-height:260px;overflow-y:auto;">
                        <asp:Repeater ID="rptThread" runat="server">
                            <ItemTemplate>
                                <div class="mb-2 pb-2 border-bottom">
                                    <div class="d-flex justify-content-between align-items-center mb-1">
                                        <strong class="small"><%# Server.HtmlEncode(Eval("HoTen") == DBNull.Value ? "" : Eval("HoTen").ToString()) %></strong>
                                        <span class="text-muted x-small"><%# Eval("NgayGui") != DBNull.Value ? ((DateTime)Eval("NgayGui")).ToString("dd/MM/yyyy HH:mm") : "" %></span>
                                    </div>
                                    <div class="small" style="white-space:pre-wrap;"><%# Server.HtmlEncode(Eval("NoiDung") == DBNull.Value ? "" : Eval("NoiDung").ToString()) %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>

                <%-- Reply box (chỉ tab TUVAN, chưa đóng) --%>
                <asp:Panel ID="pnlReply" runat="server" CssClass="mt-3" Visible="false">
                    <label class="text-muted small fw-semibold">PHẢN HỒI CỦA TRƯỜNG</label>
                    <asp:TextBox ID="txtReply" runat="server" TextMode="MultiLine" Rows="3"
                        CssClass="form-control mt-1" placeholder="Nhập nội dung phản hồi…" />
                    <div class="form-check mt-2">
                        <asp:CheckBox ID="chkGuiEmail" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label small">Gửi email thông báo cho học sinh</label>
                    </div>
                </asp:Panel>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnDongYeuCau" runat="server" Text="Đóng yêu cầu"
                    CssClass="btn btn-outline-secondary btn-sm"
                    OnClick="btnDongYeuCau_Click" CausesValidation="false" Visible="false" />
                <asp:Button ID="btnGuiPhanHoi" runat="server" Text="Gửi phản hồi"
                    CssClass="btn btn-primary btn-sm"
                    OnClick="btnGuiPhanHoi_Click" CausesValidation="false" Visible="false" />
                <button type="button" class="btn btn-outline-secondary btn-sm" data-bs-dismiss="modal">Đóng</button>
            </div>
        </div>
    </div>
</div>

</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
function switchTab(loai) {
    document.getElementById('<%= hfLoai.ClientID %>').value = loai;
    document.getElementById('<%= btnDoTab.ClientID %>').click();
}

if ('<%= ShowDetail %>' === 'true') {
    var m = new bootstrap.Modal(document.getElementById('modalChiTiet'));
    m.show();
}
</script>
</asp:Content>
