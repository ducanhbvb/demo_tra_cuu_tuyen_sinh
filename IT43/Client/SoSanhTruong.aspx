<%@ Page Title="So sánh" Language="C#" MasterPageFile="~/Client/Site.Master"
    CodeBehind="SoSanhTruong.aspx.cs" Inherits="SoSanhTruong_Page" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- compare-table / compare-major styles are in Content/Client.css --%>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

<%-- Hidden field giữ tab đang active sau PostBack --%>
<asp:HiddenField ID="hfActiveTab" runat="server" Value="truong" />

<%-- Toast thông báo — dùng chung, đặt ngoài bảng để không bị re-render --%>
<div class="toast-container position-fixed top-0 end-0 p-3" style="z-index:9050;">
    <div id="toastSoSanh" class="toast align-items-center border-0"
         role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="4000">
        <div class="d-flex">
            <div class="toast-body fw-semibold" id="toastSoSanhMsg"></div>
            <button type="button" class="btn-close me-2 m-auto"
                    data-bs-dismiss="toast" aria-label="Đóng"></button>
        </div>
    </div>
</div>

<%-- PAGE HEADER --%>
<div class="page-header-bar mb-3">
    <div class="d-flex align-items-center gap-3 flex-wrap">
        <h4 class="mb-0"><i class="bi bi-bar-chart-steps me-2 text-primary"></i>So sánh</h4>
        <span class="badge year-pill">
            Trường: <asp:Literal ID="litSoTruong" runat="server">0</asp:Literal> / 3
        </span>
        <span class="badge major-pill">
            Ngành: <asp:Literal ID="litSoNganh" runat="server">0</asp:Literal> / 4
        </span>
        <div class="ms-auto d-flex gap-2">
            <asp:Button ID="btnXoaTatCaTruong" runat="server" Text="Xóa trường"
                CssClass="btn btn-sm btn-outline-danger"
                OnClick="btnXoaTatCaTruong_Click"
                OnClientClick="return confirm('Xóa hết danh sách so sánh trường?');" />
            <asp:Button ID="btnXoaTatCaNganh" runat="server" Text="Xóa ngành"
                CssClass="btn btn-sm btn-outline-danger"
                OnClick="btnXoaTatCaNganh_Click"
                OnClientClick="return confirm('Xóa hết danh sách so sánh ngành?');" />
        </div>
    </div>
    <p class="page-header-sub mt-1">So sánh trường đại học hoặc so sánh ngành học song song theo nhiều tiêu chí.</p>
</div>

<%-- BOOTSTRAP TABS --%>
<ul class="nav nav-tabs compare-tabs mb-4" id="compareTabs">
    <li class="nav-item">
        <a class="nav-link" href='<%: ResolveUrl("~/Client/SoSanhTruong.aspx?tab=truong") %>'
           id="tabLinkTruong" data-tab="truong">
            <i class="bi bi-buildings me-1"></i>So sánh trường
        </a>
    </li>
    <li class="nav-item">
        <a class="nav-link" href='<%: ResolveUrl("~/Client/SoSanhTruong.aspx?tab=nganh") %>'
           id="tabLinkNganh" data-tab="nganh">
            <i class="bi bi-diagram-3 me-1"></i>So sánh ngành
        </a>
    </li>
</ul>

<%-- ════════════════════════════════════════════════════════
     TAB 1: SO SÁNH TRƯỜNG
════════════════════════════════════════════════════════ --%>
<asp:Panel ID="pnlTabTruong" runat="server" CssClass="compare-tab-panel">

    <%-- Thêm trường --%>
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
        </div>
    </div>

    <%-- Bảng so sánh trường --%>
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
            <tr>
                <th>Loại trường</th>
                <asp:Repeater ID="rptLoai" runat="server">
                    <ItemTemplate><td class="text-center"><%# Eval("TenLoaiTruong") %></td></ItemTemplate>
                </asp:Repeater>
            </tr>
            <tr>
                <th>Vùng</th>
                <asp:Repeater ID="rptVung" runat="server">
                    <ItemTemplate><td class="text-center"><%# Eval("TenVung") %></td></ItemTemplate>
                </asp:Repeater>
            </tr>
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
            <tr>
                <th>Quy mô</th>
                <asp:Repeater ID="rptQuyMo" runat="server">
                    <ItemTemplate><td class="text-center"><%# Eval("QuyMo") == DBNull.Value ? "&#8212;" : Eval("QuyMo") %></td></ItemTemplate>
                </asp:Repeater>
            </tr>
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
            <tr>
                <th>Số ngành đào tạo</th>
                <asp:Repeater ID="rptSoNganh" runat="server">
                    <ItemTemplate><td class="text-center fw-bold text-primary"><%# Eval("SoNganh") %></td></ItemTemplate>
                </asp:Repeater>
            </tr>
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
            <i class="bi bi-buildings display-4 d-block mb-3 opacity-50"></i>
            <h5>Chưa có trường nào trong danh sách so sánh</h5>
            <p class="small">Thêm tối đa 3 trường để so sánh các tiêu chí song song.</p>
            <a href="<%: ResolveUrl("~/Client/TimKiemTruong.aspx") %>" class="btn btn-primary btn-sm">
                <i class="bi bi-search me-1"></i>Tìm kiếm trường
            </a>
        </div>
    </asp:Panel>

</asp:Panel>
<%-- END TAB TRƯỜNG --%>


<%-- ════════════════════════════════════════════════════════
     TAB 2: SO SÁNH NGÀNH (MỚI)
════════════════════════════════════════════════════════ --%>
<asp:Panel ID="pnlTabNganh" runat="server" CssClass="compare-tab-panel">

    <%-- Chọn ngành cascading --%>
    <div class="card shadow-sm mb-4">
        <div class="card-body">
            <p class="small text-muted mb-2">
                <i class="bi bi-info-circle me-1 text-primary"></i>
                Chọn trường, sau đó chọn ngành &amp; năm để thêm vào bảng so sánh (tối đa 4 mục).
            </p>
            <div class="row g-2 align-items-end">
                <div class="col-md-4">
                    <label class="form-label small fw-semibold">1. Chọn trường</label>
                    <asp:DropDownList ID="ddlChonTruongNganh" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlChonTruongNganh_Changed">
                        <asp:ListItem Value="0">-- Chọn trường --</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-5">
                    <label class="form-label small fw-semibold">2. Chọn ngành &amp; năm tuyển sinh</label>
                    <asp:DropDownList ID="ddlChonNganhTin" runat="server" CssClass="form-select">
                        <asp:ListItem Value="0">-- Chọn trường trước --</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <asp:Button ID="btnThemNganh" runat="server" Text="Thêm so sánh"
                        CssClass="btn btn-primary w-100" OnClick="btnThemNganh_Click" />
                </div>
            </div>
        </div>
    </div>

    <%-- Bảng so sánh ngành --%>
    <asp:Panel ID="pnlBangNganh" runat="server" Visible="false">
    <div class="table-responsive">
    <table class="table table-bordered compare-table">
        <thead>
            <tr>
                <th class="align-middle" style="min-width:155px;">Tiêu chí</th>
                <asp:Repeater ID="rptNganhHeader" runat="server">
                    <ItemTemplate>
                        <th class="text-center p-0" style="min-width:215px;">
                            <div class="compare-major-header p-3">
                                <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string)
                                    ? "/Resources/Images/no-image.png"
                                    : Eval("AnhDaiDien") %>'
                                    class="compare-major-img mb-2 d-block mx-auto"
                                    onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                                <div class="compare-major-name"><%# Eval("TenChuyenNganh") %></div>
                                <div class="compare-major-school">
                                    <i class="bi bi-building me-1"></i><%# Eval("TenTruong") %> · <%# Eval("NamTuyenSinh") %>
                                </div>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-light mt-2 remove-col"
                                    CommandName="XoaNganh" CommandArgument='<%# Eval("MaTin") %>'
                                    OnCommand="rptNganhHeader_Command">
                                    <i class="bi bi-x-circle me-1"></i>Bỏ
                                </asp:LinkButton>
                            </div>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
        </thead>
        <tbody>

            <%-- Nhóm ngành --%>
            <tr>
                <th>Nhóm ngành<span class="compare-tieuchi-label">Danh mục</span></th>
                <asp:Repeater ID="rptNganhDanhMuc" runat="server">
                    <ItemTemplate><td class="text-center small"><%# Eval("TenDanhMuc") %></td></ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Trường --%>
            <tr>
                <th>Trường<span class="compare-tieuchi-label">Địa điểm</span></th>
                <asp:Repeater ID="rptNganhTruong" runat="server">
                    <ItemTemplate>
                        <td class="text-center small">
                            <div class="fw-semibold"><%# Eval("TenTruong") %></div>
                            <div class="text-muted"><%# Eval("TinhThanh") %></div>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Cấp bậc --%>
            <tr>
                <th>Cấp bậc đào tạo</th>
                <asp:Repeater ID="rptNganhCapBac" runat="server">
                    <ItemTemplate>
                        <td class="text-center">
                            <%# string.IsNullOrEmpty(Eval("TenCapBac") as string) ? "<span class='text-muted'>&#8212;</span>"
                                : "<span class='badge bg-secondary-subtle text-secondary-emphasis'>" + Eval("TenCapBac") + "</span>" %>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Phương thức --%>
            <tr>
                <th>Phương thức xét tuyển</th>
                <asp:Repeater ID="rptNganhPhuongThuc" runat="server">
                    <ItemTemplate>
                        <td class="text-center">
                            <span class="badge-phuongthuc"><%# Eval("TenPhuongThuc") %></span>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Tổ hợp môn --%>
            <tr>
                <th>Tổ hợp môn<span class="compare-tieuchi-label">Xét tuyển</span></th>
                <asp:Repeater ID="rptNganhToHop" runat="server">
                    <ItemTemplate>
                        <td class="text-center small text-muted">
                            <%# string.IsNullOrEmpty(Eval("ToHopMonHoc") as string) ? "<span>&#8212;</span>" : Eval("ToHopMonHoc").ToString() %>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Điểm chuẩn — css class được gán từ code-behind qua DataTable extra column --%>
            <tr>
                <th>Điểm chuẩn<span class="compare-tieuchi-label">Năm trước</span></th>
                <asp:Repeater ID="rptNganhDiemChuan" runat="server">
                    <ItemTemplate>
                        <td class='text-center <%# Eval("DiemChuanCssClass") %>'>
                            <%# Eval("DiemChuanNamTruoc") == DBNull.Value
                                ? "<span class='text-muted score-big'>&#8212;</span>"
                                : "<span class='score-big'>" + string.Format("{0:F2}", Eval("DiemChuanNamTruoc")) + "</span>" %>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Học phí --%>
            <tr>
                <th>Học phí/năm<span class="compare-tieuchi-label">Dự kiến</span></th>
                <asp:Repeater ID="rptNganhHocPhi" runat="server">
                    <ItemTemplate>
                        <td class="text-center small">
                            <%# Eval("HocPhi") == DBNull.Value || string.IsNullOrWhiteSpace(Eval("HocPhi").ToString())
                                ? "<span class='text-muted'>&#8212;</span>"
                                : System.Web.HttpUtility.HtmlEncode(Eval("HocPhi").ToString()) %>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Chỉ tiêu --%>
            <tr>
                <th>Chỉ tiêu<span class="compare-tieuchi-label">Năm tuyển sinh</span></th>
                <asp:Repeater ID="rptNganhChiTieu" runat="server">
                    <ItemTemplate>
                        <td class="text-center fw-bold text-primary">
                            <%# Eval("ChiTieu") == DBNull.Value || (int)Eval("ChiTieu") == 0
                                ? "<span class='text-muted fw-normal'>&#8212;</span>"
                                : Eval("ChiTieu").ToString() %>
                        </td>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>

            <%-- Xem chi tiết trường --%>
            <tr>
                <th>Xem chi tiết trường</th>
                <asp:Repeater ID="rptNganhChiTietTruong" runat="server">
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

    <asp:Panel ID="pnlEmptyNganh" runat="server" Visible="true">
        <div class="text-center py-5 text-muted">
            <i class="bi bi-diagram-3 display-4 d-block mb-3 opacity-50"></i>
            <h5>Chưa có ngành nào trong danh sách so sánh</h5>
            <p class="small">Chọn trường rồi chọn ngành để thêm vào (tối đa 4 mục).</p>
            <a href="<%: ResolveUrl("~/Client/TimKiemTheoNganh.aspx") %>" class="btn btn-primary btn-sm">
                <i class="bi bi-search me-1"></i>Tìm kiếm theo ngành
            </a>
        </div>
    </asp:Panel>

</asp:Panel>
<%-- END TAB NGÀNH --%>


<%-- Script: active tab — ẩn/hiện bằng CSS class, KHÔNG PostBack chỉ để đổi tab --%>
<script>
(function () {
    var hfId = '<%= hfActiveTab.ClientID %>';
    var hf   = document.getElementById(hfId);

    var panels = {
        truong: document.getElementById('<%= pnlTabTruong.ClientID %>'),
        nganh:  document.getElementById('<%= pnlTabNganh.ClientID %>')
    };

    function activateTab(name) {
        name = (name === 'nganh') ? 'nganh' : 'truong';

        // Cập nhật active class trên nav-link
        ['Truong', 'Nganh'].forEach(function (t) {
            var link = document.getElementById('tabLink' + t);
            if (link) link.classList.toggle('active', t.toLowerCase() === name);
        });

        // Ẩn/hiện panel bằng class active (không PostBack)
        Object.keys(panels).forEach(function (key) {
            if (panels[key]) panels[key].classList.toggle('active', key === name);
        });

        // Lưu tab đang active vào HiddenField để server biết khi có PostBack thực sự
        if (hf) hf.value = name;
    }

    // Restore tab khi trang load (server đã set hfActiveTab.Value đúng tab)
    var initial = (hf && hf.value) ? hf.value : 'truong';
    activateTab(initial);

    // Enhancement: nếu cần gọi từ code khác thì vẫn đổi tab client-side được.
    // Hai tab phía trên KHÔNG phụ thuộc hàm này nữa vì đã dùng href thật.
    window.switchTab = function (name) {
        activateTab(name);
        return false;
    };
})();
</script>

</asp:Content>
