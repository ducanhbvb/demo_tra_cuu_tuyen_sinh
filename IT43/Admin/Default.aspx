<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Admin/Admin.Master"
   CodeBehind="Default.aspx.cs" Inherits="Admin_Default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- Stat Cards --%>
<div class="row g-4 mb-4">
    <div class="col-6 col-xl-3">
        <div class="stat-card grad-blue">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litSoTruong" runat="server" /></div>
                    <div class="stat-label">Trường đại học</div>
                </div>
                <i class="bi bi-building-fill stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-6 col-xl-3">
        <div class="stat-card grad-green">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litSoTin" runat="server" /></div>
                    <div class="stat-label">Tin tuyển sinh</div>
                </div>
                <i class="bi bi-file-earmark-text-fill stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-6 col-xl-3">
        <div class="stat-card grad-amber">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litSoTuVan" runat="server" /></div>
                    <div class="stat-label">Tư vấn chờ xử lý</div>
                </div>
                <i class="bi bi-chat-dots-fill stat-icon"></i>
            </div>
        </div>
    </div>
    <div class="col-6 col-xl-3">
        <div class="stat-card grad-cyan">
            <div class="stat-card-body">
                <div>
                    <div class="stat-number"><asp:Literal ID="litSoTK" runat="server" /></div>
                    <div class="stat-label">Tài khoản</div>
                </div>
                <i class="bi bi-people-fill stat-icon"></i>
            </div>
        </div>
    </div>
</div>

<%-- Tables Row --%>
<div class="row g-4">
    <div class="col-lg-6">
        <div class="admin-card">
            <div class="admin-card-header">
                <div class="admin-card-title"><i class="bi bi-building me-2 text-primary"></i>Trường mới nhất</div>
                <a href="QuanLyTruong.aspx" class="btn btn-sm btn-admin-primary">Xem tất cả</a>
            </div>
            <asp:GridView ID="gvTruong" runat="server"
                CssClass="table table-hover mb-0"
                AutoGenerateColumns="false" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="TenTruong" HeaderText="Tên trường" HtmlEncode="true" />
                    <asp:BoundField DataField="TinhThanh" HeaderText="Tỉnh/TP" HtmlEncode="true" />
                    <asp:HyperLinkField Text="Sửa" HeaderText=""
                        DataNavigateUrlFields="MaTruong"
                        DataNavigateUrlFormatString="ChinhSuaTruong.aspx?id={0}"
                        ControlStyle-CssClass="btn btn-xs btn-outline-secondary rounded-pill px-3" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="col-lg-6">
        <div class="admin-card">
            <div class="admin-card-header">
                <div class="admin-card-title"><i class="bi bi-chat-dots me-2 text-warning"></i>Tư vấn chờ xử lý</div>
                <a href="QuanLyGopYTuVan.aspx" class="btn btn-sm btn-admin-primary">Xem tất cả</a>
            </div>
            <asp:GridView ID="gvTuVan" runat="server"
                CssClass="table table-hover mb-0"
                AutoGenerateColumns="false" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="HoTen"    HeaderText="Họ tên"   HtmlEncode="true" />
                    <asp:BoundField DataField="TenTruong" HeaderText="Trường"   HtmlEncode="true" />
                    <asp:BoundField DataField="NgayGui"  HeaderText="Ngày gửi"  DataFormatString="{0:dd/MM}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>

</asp:Content>
