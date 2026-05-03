<%@ Page Title="So sánh trường" Language="C#" MasterPageFile="~/Client/Site.Master"
    CodeBehind="SoSanhTruong.aspx.cs" Inherits="SoSanhTruong_Page" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
.compare-table th { background: #f8f9fa; font-weight: 600; white-space: nowrap; width: 160px; }
.compare-table td { vertical-align: middle; }
.compare-school-header { background: linear-gradient(135deg,#1a73e8,#0d47a1); color:#fff; border-radius:8px 8px 0 0; }
.compare-school-img { width:64px; height:64px; object-fit:cover; border-radius:50%; border:3px solid #fff; }
.badge-kd  { background:#28a745; }
.badge-nkd { background:#6c757d; }
.star-gold { color:#f5a623; }
.remove-col { cursor:pointer; }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<div class="d-flex align-items-center mb-3 gap-3">
    <h4 class="mb-0"><i class="bi bi-bar-chart-steps me-2 text-primary"></i>So sánh trường</h4>
    <span class="badge bg-secondary">
        Đang so sánh: <asp:Literal ID="litSoTruong" runat="server">0</asp:Literal> / 3 trường
    </span>
    <asp:Button ID="btnXoaTatCa" runat="server" Text="Xóa tất cả" CssClass="btn btn-sm btn-outline-danger ms-auto"
        OnClick="btnXoaTatCa_Click" OnClientClick="return confirm('Xóa hết danh sách so sánh?');" />
</div>

<%-- THÊM TRƯỜNG --%>
<div class="card shadow-sm mb-4">
    <div class="card-body">
        <div class="row g-2 align-items-end">
            <div class="col-md-8">
                <label class="form-label small fw-semibold">Thêm trường vào danh sách so sánh (tối đa 3)</label>
                <asp:DropDownList ID="ddlChonTruong" runat="server" CssClass="form-select">
                    <asp:ListItem Value="0">-- Chọn trường để thêm --</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <asp:Button ID="btnThem" runat="server" Text="Thêm vào so sánh"
                    CssClass="btn btn-primary w-100" OnClick="btnThem_Click" />
            </div>
        </div>
        <asp:Literal ID="litThongBao" runat="server" />
    </div>
</div>

<%-- BẢNG SO SÁNH --%>
<asp:Panel ID="pnlBang" runat="server" Visible="false">
<div class="table-responsive">
<table class="table table-bordered compare-table">
    <thead>
        <tr>
            <th class="align-middle">Tiêu chí</th>
            <asp:Repeater ID="rptHeader" runat="server">
                <ItemTemplate>
                    <th class="text-center p-0" style="min-width:200px;">
                        <div class="compare-school-header p-3">
                            <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string)
                                ? "/Resources/Images/no-image.png"
                                : Eval("AnhDaiDien") %>'
                                class="compare-school-img mb-2 d-block mx-auto"
                                onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                            <div class="fw-bold small"><%# Eval("TenTruong") %></div>
                            <div class="small opacity-75"><%# Eval("TinhThanh") %></div>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-light mt-2 remove-col"
                                CommandName="Xoa" CommandArgument='<%# Eval("MaTruong") %>'
                                OnCommand="rptHeader_Command">
                                <i class="bi bi-x-circle me-1"></i>Bỏ
                            </asp:LinkButton>
                        </div>
                    </th>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
    </thead>
    <tbody>
        <%-- Loại trường --%>
        <tr>
            <th>Loại trường</th>
            <asp:Repeater ID="rptLoai" runat="server">
                <ItemTemplate><td class="text-center"><%# Eval("TenLoaiTruong") %></td></ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Vùng --%>
        <tr>
            <th>Vùng</th>
            <asp:Repeater ID="rptVung" runat="server">
                <ItemTemplate><td class="text-center"><%# Eval("TenVung") %></td></ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Kiểm định --%>
        <tr>
            <th>Kiểm định CL</th>
            <asp:Repeater ID="rptKiemDinh" runat="server">
                <ItemTemplate>
                    <td class="text-center">
                        <%# (bool)Eval("KiemDinhChatLuong")
                            ? "<span class='badge badge-kd'>✓ Đã kiểm định</span>"
                            : "<span class='badge badge-nkd'>Chưa</span>" %>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Đánh giá --%>
        <tr>
            <th>Đánh giá TB</th>
            <asp:Repeater ID="rptDanhGia" runat="server">
                <ItemTemplate>
                    <td class="text-center">
                        <%# Eval("DiemDanhGiaTB") == DBNull.Value ? "<span class='text-muted'>&#8212;</span>"
                            : "<span class='star-gold'>&#9733;</span> " + string.Format("{0:F1}", Eval("DiemDanhGiaTB")) + " / 5" %>
                        <div class="small text-muted">(<%# Eval("SoLuongDanhGia") %> đánh giá)</div>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Website --%>
        <tr>
            <th>Website</th>
            <asp:Repeater ID="rptWebsite" runat="server">
                <ItemTemplate>
                    <td class="text-center small">
                        <%# string.IsNullOrEmpty(Eval("Website") as string) ? "&#8212;"
                            : "<a href='" + Eval("Website") + "' target='_blank' rel='noopener'>" + Eval("Website") + "</a>" %>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Quy mô --%>
        <tr>
            <th>Quy mô</th>
            <asp:Repeater ID="rptQuyMo" runat="server">
                <ItemTemplate><td class="text-center"><%# Eval("QuyMo") == DBNull.Value ? "&#8212;" : Eval("QuyMo") %></td></ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Điểm chuẩn gần nhất --%>
        <tr>
            <th>Điểm chuẩn<br/><small class="text-muted fw-normal">(gần nhất)</small></th>
            <asp:Repeater ID="rptDiemChuan" runat="server">
                <ItemTemplate>
                    <td class="text-center">
                        <%# Eval("DiemChuanMin") == DBNull.Value ? "<span class='text-muted'>&#8212;</span>"
                            : "<b>" + string.Format("{0:F2}", Eval("DiemChuanMin")) + "</b> &#8211; <b>" + string.Format("{0:F2}", Eval("DiemChuanMax")) + "</b>" %>
                        <div class="small text-muted"><%# Eval("NamGanNhat") %></div>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Số ngành --%>
        <tr>
            <th>Số ngành đào tạo</th>
            <asp:Repeater ID="rptSoNganh" runat="server">
                <ItemTemplate><td class="text-center fw-bold text-primary"><%# Eval("SoNganh") %></td></ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Chi tiết --%>
        <tr>
            <th>Xem chi tiết</th>
            <asp:Repeater ID="rptChiTiet" runat="server">
                <ItemTemplate>
                    <td class="text-center">
                        <a href='<%# ResolveUrl("~/Client/ChiTietTruong.aspx?slug=" + Eval("Slug")) %>'
                           class="btn btn-sm btn-outline-primary" target="_blank">
                            <i class="bi bi-eye me-1"></i>Xem
                        </a>
                    </td>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
    </tbody>
</table>
</div>
</asp:Panel>

<asp:Panel ID="pnlEmpty" runat="server" Visible="true">
    <div class="text-center py-5 text-muted">
        <i class="bi bi-bar-chart-steps display-4 d-block mb-3 opacity-50"></i>
        <h5>Chưa có trường nào trong danh sách so sánh</h5>
        <p class="small">Thêm tối đa 3 trường để so sánh các tiêu chí song song.</p>
        <a href="<%: ResolveUrl("~/Client/TimKiemTruong.aspx") %>" class="btn btn-primary">
            <i class="bi bi-search me-1"></i>Tìm kiếm trường
        </a>
    </div>
</asp:Panel>

</asp:Content>
