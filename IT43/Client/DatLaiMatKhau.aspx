<%@ Page Title="Đặt lại mật khẩu" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="DatLaiMatKhau.aspx.cs" Inherits="Account_DatLaiMatKhau" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="row justify-content-center">
  <div class="col-md-5 col-lg-4">
    <div class="card shadow-sm border-0 mt-3">
      <div class="card-body p-4">
        <h4 class="fw-bold text-center mb-4"><i class="bi bi-shield-lock text-primary me-2"></i>Đặt lại mật khẩu</h4>

        <asp:Literal ID="litThongBao" runat="server" />

        <asp:Panel ID="pnlForm" runat="server">
          <div class="mb-3">
            <label class="form-label fw-semibold">Mật khẩu mới</label>
            <asp:TextBox ID="txtMK1" runat="server" CssClass="form-control"
              placeholder="Ít nhất 6 ký tự" TextMode="Password" />
            <asp:RequiredFieldValidator ControlToValidate="txtMK1" runat="server"
              CssClass="text-danger small" ErrorMessage="Vui lòng nhập mật khẩu mới." Display="Dynamic" />
          </div>
          <div class="mb-4">
            <label class="form-label fw-semibold">Xác nhận mật khẩu</label>
            <asp:TextBox ID="txtMK2" runat="server" CssClass="form-control"
              placeholder="Nhập lại mật khẩu" TextMode="Password" />
            <asp:CompareValidator ControlToValidate="txtMK2" ControlToCompare="txtMK1"
              runat="server" CssClass="text-danger small"
              ErrorMessage="Mật khẩu không khớp." Display="Dynamic" />
          </div>
          <div class="d-grid">
            <asp:Button ID="btnLuu" runat="server" Text="Lưu mật khẩu mới"
              CssClass="btn btn-primary" OnClick="btnLuu_Click" />
          </div>
        </asp:Panel>
      </div>
    </div>
  </div>
</div>
</asp:Content>
