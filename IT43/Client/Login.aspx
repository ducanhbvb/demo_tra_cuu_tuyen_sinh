<%@ Page Title="Đăng nhập" Language="C#" MasterPageFile="~/Client/Site.Master"
   CodeBehind="Login.aspx.cs" Inherits="Account_Login" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="row justify-content-center">
  <div class="col-md-5 col-lg-4">
    <div class="card shadow-sm border-0 mt-3">
      <div class="card-body p-4">
        <h4 class="card-title text-center mb-4 fw-bold">
          <i class="bi bi-box-arrow-in-right text-primary me-2"></i>Đăng nhập
        </h4>

        <asp:Literal ID="litThongBao" runat="server" />

        <div class="mb-3">
          <label class="form-label fw-semibold">Email</label>
          <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
            placeholder="you@example.com" TextMode="Email" />
          <asp:RequiredFieldValidator ControlToValidate="txtEmail" runat="server"
            CssClass="text-danger small" ErrorMessage="Vui lòng nhập email." Display="Dynamic" />
        </div>

        <div class="mb-3">
          <label class="form-label fw-semibold">Mật khẩu</label>
          <div class="pass-input-wrap">
            <asp:TextBox ID="txtMatKhau" runat="server" CssClass="form-control"
              placeholder="••••••" TextMode="Password" />
            <button type="button" class="pass-eye-btn" onclick="togglePassword(this)" tabindex="-1">
              <i class="bi bi-eye"></i>
            </button>
          </div>
          <asp:RequiredFieldValidator ControlToValidate="txtMatKhau" runat="server"
            CssClass="text-danger small" ErrorMessage="Vui lòng nhập mật khẩu." Display="Dynamic" />
        </div>

        <div class="d-flex justify-content-between align-items-center mb-3">
          <div class="form-check">
            <input type="checkbox" id="chkNhoToi" runat="server" class="form-check-input" />
            <label class="form-check-label small" for="<%= chkNhoToi.ClientID %>">Nhớ đăng nhập</label>
          </div>
          <a href="/quen-mat-khau.aspx" class="small text-decoration-none">Quên mật khẩu?</a>
        </div>

        <div class="d-grid">
          <asp:Button ID="btnDangNhap" runat="server" Text="Đăng nhập"
            CssClass="btn btn-primary" OnClick="btnDangNhap_Click" />
        </div>

        <hr />
        <p class="text-center small mb-0">
          Chưa có tài khoản? <a href="/dang-ky.aspx">Đăng ký ngay</a>
        </p>
      </div>
    </div>
  </div>
</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
function togglePassword(btn) {
    var wrap = btn.parentElement;
    var inp = wrap.querySelector('input');
    if (!inp) return;
    var icon = btn.querySelector('i');
    if (inp.type === 'password') {
        inp.type = 'text';
        icon.className = 'bi bi-eye-slash';
    } else {
        inp.type = 'password';
        icon.className = 'bi bi-eye';
    }
    inp.focus();
}
</script>
</asp:Content>
