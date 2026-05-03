<%@ Page Title="Lỗi" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="Error.aspx.cs" Inherits="ErrorPage" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="text-center py-5">
    <i class="bi bi-exclamation-triangle-fill text-warning display-1"></i>
    <h2 class="mt-3 fw-bold"><asp:Literal ID="litCode" runat="server" /></h2>
    <p class="text-muted"><asp:Literal ID="litMsg" runat="server" /></p>
    <a href="<%: ResolveUrl("~/Client/index.aspx") %>" class="btn btn-primary mt-2">
        <i class="bi bi-house me-1"></i>Về trang chủ
    </a>
</div>
</asp:Content>
