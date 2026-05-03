using System;
using System.Web.UI;

/// <summary>
/// Trang Đăng nhập — xác thực email/mật khẩu qua TaiKhoanBLL,
/// xử lý các trường hợp: sai mật khẩu, bị khoá, chưa xác nhận email.
/// </summary>
public partial class Account_Login : Page
{
    /// <summary>Nếu đã đăng nhập → redirect về trang chủ.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.Identity.IsAuthenticated)
            Response.Redirect("~/Client/index.aspx");
    }

    /// <summary>
    /// Xử lý nút Đăng nhập — gọi TaiKhoanBLL.DangNhap, tạo cookie xác thực
    /// và redirect về returnUrl (nếu hợp lệ) hoặc trang chủ.
    /// </summary>
    protected void btnDangNhap_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        string email = txtEmail.Text.Trim().ToLower();
        string mk    = txtMatKhau.Text;

        var (kq, maTK, role) = TaiKhoanBLL.DangNhap(email, mk);

        switch (kq)
        {
            case TaiKhoanBLL.KetQuaDangNhap.ThanhCong:
                SecurityHelper.SignIn(maTK, email, role, chkNhoToi.Checked);
                // Chỉ redirect URL nội bộ (bắt đầu bằng "/") — ngăn Open Redirect attack
                string returnUrl = Request.QueryString["returnUrl"];
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
                    Response.Redirect(returnUrl);
                else
                    Response.Redirect("~/Client/index.aspx");
                break;

            case TaiKhoanBLL.KetQuaDangNhap.SaiMatKhau:
                ShowError("Email hoặc mật khẩu không đúng.");
                break;

            case TaiKhoanBLL.KetQuaDangNhap.BiKhoa:
                ShowError("Tài khoản đã bị khoá tạm thời do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau 30 phút.");
                break;

            case TaiKhoanBLL.KetQuaDangNhap.ChuaXacNhanEmail:
                ShowError("Bạn chưa xác nhận email. Vui lòng kiểm tra hộp thư và nhấn vào link xác nhận.");
                break;

            case TaiKhoanBLL.KetQuaDangNhap.KhongTonTai:
                ShowError("Email hoặc mật khẩu không đúng.");
                break;
        }
    }

    /// <summary>Hiển thị thông báo lỗi dạng alert Bootstrap.</summary>
    private void ShowError(string msg)
    {
        litThongBao.Text = $"<div class='alert alert-danger'><i class='bi bi-exclamation-triangle me-2'></i>{msg}</div>";
    }
}
