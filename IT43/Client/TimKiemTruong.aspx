<%@ Page Title="Tìm kiếm trường" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="TimKiemTruong.aspx.cs" Inherits="TimKiemTruong" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<!-- ScriptManager cho UpdatePanel -->
<asp:ScriptManager ID="ScriptManager1" runat="server" />

<h3 class="fw-bold mb-4"><i class="bi bi-search text-primary me-2"></i>Tìm kiếm trường đại học</h3>

<!-- UpdateProgress: loading indicator -->
<asp:UpdateProgress ID="updProgress" runat="server" AssociatedUpdatePanelID="updSearch">
    <ProgressTemplate>
        <div class="position-fixed top-50 start-50 translate-middle bg-white p-4 rounded shadow border" style="z-index: 9999;">
            <div class="text-center">
                <div class="spinner-border text-primary" role="status"></div>
                <div class="mt-2">Đang tìm kiếm...</div>
            </div>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>

<!-- UpdatePanel cho toàn bộ tìm kiếm -->
<asp:UpdatePanel ID="updSearch" runat="server">
    <ContentTemplate>

        <%-- BỘ LỌC --%>
        <div class="card border-0 shadow-sm mb-4">
            <div class="card-body">
                <div class="school-search-glass mb-3">
                    <i class="bi bi-search school-search-icon" aria-hidden="true"></i>
                    <asp:TextBox ID="txtTen" runat="server" CssClass="form-control"
                        placeholder="Nhập tên trường, tỉnh thành hoặc từ khóa..." />
                    <asp:Button ID="btnLoc" runat="server" Text="Tìm kiếm"
                        CssClass="btn-search" OnClick="btnLoc_Click" />
                </div>

                <div class="row g-3 align-items-end">
                    <div class="col-6 col-md-2">
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
                    <div class="col-6 col-md-2">
                        <label class="form-label fw-semibold">Cấp bậc đào tạo</label>
                        <asp:DropDownList ID="ddlCapBac" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                            <asp:ListItem Value="1">Đại học</asp:ListItem>
                            <asp:ListItem Value="2">Cao Đẳng</asp:ListItem>
                            <asp:ListItem Value="3">Trường nghề</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-3">
                        <label class="form-label fw-semibold">Ngành học</label>
                        <asp:DropDownList ID="ddlNganh" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Tất cả ngành --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label fw-semibold">Năm tuyển sinh</label>
                        <asp:DropDownList ID="ddlNam" runat="server" CssClass="form-select">
                            <asp:ListItem Value="">-- Tất cả --</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row g-3 align-items-end mt-0">
                    <div class="col-6 col-md-2">
                        <label class="form-label fw-semibold">Điểm từ</label>
                        <asp:TextBox ID="txtDiemMin" runat="server" CssClass="form-control"
                            placeholder="VD: 15.0" TextMode="Number" step="0.1" />
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label fw-semibold">Điểm đến</label>
                        <asp:TextBox ID="txtDiemMax" runat="server" CssClass="form-control"
                            placeholder="VD: 30.0" TextMode="Number" step="0.1" />
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label fw-semibold">Sắp xếp theo</label>
                        <asp:DropDownList ID="ddlSort" runat="server" CssClass="form-select">
                            <asp:ListItem Value="TenTruong">Tên trường</asp:ListItem>
                            <asp:ListItem Value="TinhThanh">Tỉnh thành</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-6 col-md-2">
                        <label class="form-label fw-semibold">Thứ tự</label>
                        <asp:DropDownList ID="ddlSortDir" runat="server" CssClass="form-select">
                            <asp:ListItem Value="ASC">Tăng dần</asp:ListItem>
                            <asp:ListItem Value="DESC">Giảm dần</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 col-md-8 d-flex gap-2">
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

        <%-- Toast thông báo so sánh — góc trên-phải, đặt ngoài UpdatePanel --%>
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

        <div class="row g-3" id="truong-list">
            <asp:Repeater ID="rptKetQua" runat="server" OnItemCommand="rptKetQua_Command">
                <ItemTemplate>
                    <div class="col-sm-6 col-md-4 col-xl-3">
                        <div class="card border-0 shadow-sm h-100 truong-card">
                            <a href='ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'>
                                <img src='<%# string.IsNullOrEmpty(Eval("AnhDaiDien") as string) ? "/Resources/Images/no-image.png" : Eval("AnhDaiDien") %>'
                                     class="card-img-top" style="height:140px;object-fit:cover;"
                                     alt='<%# Eval("TenTruong") %>' loading="lazy"
                                     onerror="this.onerror=null;this.src='/Resources/Images/no-image.png'" />
                            </a>
                            <div class="card-body p-3 position-relative">
                                <%-- Nút + so sánh — góc trên-phải phần nội dung card --%>
                                <asp:LinkButton runat="server"
                                    CssClass="compare-card-add-btn position-absolute top-0 end-0 m-2"
                                    CommandName="ThemSoSanh"
                                    CommandArgument='<%# Eval("MaTruong") %>'
                                    ToolTip="Thêm trường vào so sánh">
                                    <i class="bi bi-plus-circle-fill"></i>
                                </asp:LinkButton>
                                <h6 class="card-title fw-semibold mb-1" style="padding-right:32px;">
                                    <a href='ChiTietTruong.aspx?slug=<%# Eval("Slug") %>'
                                       class="text-decoration-none text-body"><%# Eval("TenTruong") %></a>
                                </h6>
                                <p class="text-muted small mb-1">
                                    <i class="bi bi-geo-alt me-1"></i><%# Eval("TinhThanh") %>
                                </p>
                                <div class="school-badge-stack mt-2">
                                    <span class='badge <%# (byte?)Eval("LoaiTruong") == 1 ? "bg-primary" : (byte?)Eval("LoaiTruong") == 2 ? "bg-success" : "bg-warning text-dark" %>'>
                                        <%# (byte?)Eval("LoaiTruong") == 1 ? "Công lập" : (byte?)Eval("LoaiTruong") == 2 ? "Tư thục" : "Quốc tế" %>
                                    </span>
                                    <%# GetCapBacBadge(Eval("CapBacDaoTao")) %>
                                    <%# (bool)Eval("KiemDinhChatLuong") ? "<span class='badge bg-info'>Kiểm định</span>" : "" %>
                                </div>
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

    </ContentTemplate>
</asp:UpdatePanel>

</asp:Content>
