<%@ Page Title="Đăng ký" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="DangKy.aspx.cs" Inherits="Account_DangKy" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="row justify-content-center">
  <div class="col-md-6 col-lg-5">
    <div class="card shadow-sm border-0 mt-3">
      <div class="card-body p-4">
        <h4 class="card-title text-center mb-4 fw-bold">
          <i class="bi bi-person-plus text-primary me-2"></i>Tạo tài khoản
        </h4>

        <asp:Literal ID="litThongBao" runat="server" />

        <div class="mb-3">
          <label class="form-label fw-semibold">Email <span class="text-danger">*</span></label>
          <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
            placeholder="you@example.com" TextMode="Email" />
          <asp:RequiredFieldValidator ControlToValidate="txtEmail" runat="server"
            CssClass="text-danger small" ErrorMessage="Vui lòng nhập email." Display="Dynamic" />
          <asp:RegularExpressionValidator ControlToValidate="txtEmail" runat="server"
            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
            CssClass="text-danger small" ErrorMessage="Email không hợp lệ." Display="Dynamic" />
        </div>

        <div class="mb-3">
          <label class="form-label fw-semibold">Mật khẩu <span class="text-danger">*</span></label>
          <asp:TextBox ID="txtMatKhau" runat="server" CssClass="form-control"
            placeholder="Ít nhất 6 ký tự" TextMode="Password" />
          <asp:RequiredFieldValidator ControlToValidate="txtMatKhau" runat="server"
            CssClass="text-danger small" ErrorMessage="Vui lòng nhập mật khẩu." Display="Dynamic" />
          <asp:RegularExpressionValidator ControlToValidate="txtMatKhau" runat="server"
            ValidationExpression=".{6,}"
            CssClass="text-danger small" ErrorMessage="Mật khẩu phải có ít nhất 6 ký tự." Display="Dynamic" />
        </div>

        <div class="mb-4">
          <label class="form-label fw-semibold">Xác nhận mật khẩu <span class="text-danger">*</span></label>
          <asp:TextBox ID="txtXacNhan" runat="server" CssClass="form-control"
            placeholder="Nhập lại mật khẩu" TextMode="Password" />
          <asp:CompareValidator ControlToValidate="txtXacNhan" ControlToCompare="txtMatKhau"
            runat="server" CssClass="text-danger small"
            ErrorMessage="Mật khẩu xác nhận không khớp." Display="Dynamic" />
        </div>

        <div class="d-grid">
          <asp:Button ID="btnDangKy" runat="server" Text="Đăng ký"
            CssClass="btn btn-primary" OnClick="btnDangKy_Click" />
        </div>

        <hr />
        <p class="text-center small mb-0">
          Đã có tài khoản? <a href="/login.aspx">Đăng nhập</a>
        </p>
      </div>
    </div>
  </div>
</div>
</asp:Content>
