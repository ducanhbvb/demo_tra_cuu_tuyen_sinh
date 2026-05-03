<%@ Page Title="Quên mật khẩu" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="QuenMatKhau.aspx.cs" Inherits="Account_QuenMatKhau" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="row justify-content-center">
  <div class="col-md-5 col-lg-4">
    <div class="card shadow-sm border-0 mt-3">
      <div class="card-body p-4">
        <h4 class="fw-bold text-center mb-1"><i class="bi bi-key text-primary me-2"></i>Quên mật khẩu</h4>
        <p class="text-muted text-center small mb-4">Nhập email, chúng tôi sẽ gửi link đặt lại mật khẩu.</p>

        <asp:Literal ID="litThongBao" runat="server" />

        <div class="mb-3">
          <label class="form-label fw-semibold">Email đã đăng ký</label>
          <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
            placeholder="you@example.com" TextMode="Email" />
          <asp:RequiredFieldValidator ControlToValidate="txtEmail" runat="server"
            CssClass="text-danger small" ErrorMessage="Vui lòng nhập email." Display="Dynamic" />
        </div>

        <div class="d-grid">
          <asp:Button ID="btnGui" runat="server" Text="Gửi link đặt lại"
            CssClass="btn btn-primary" OnClick="btnGui_Click" />
        </div>

        <p class="text-center small mt-3 mb-0">
          <a href="/login.aspx">Quay lại đăng nhập</a>
        </p>
      </div>
    </div>
  </div>
</div>
</asp:Content>
