using System;
using System.Web.UI;

/// <summary>
/// Trang Đăng ký tài khoản — tạo tài khoản mới qua TaiKhoanBLL.DangKy,
/// gửi email xác nhận (nếu SMTP đã cấu hình) hoặc tự kích hoạt.
/// </summary>
public partial class Account_DangKy : Page
{
    /// <summary>Nếu đã đăng nhập → redirect về trang chủ.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (User.Identity.IsAuthenticated)
            Response.Redirect("~/Client/index.aspx");
    }

    /// <summary>
    /// Xử lý nút Đăng ký — validate, gọi BLL tạo tài khoản,
    /// hiển thị hướng dẫn xác nhận email hoặc thông báo lỗi.
    /// </summary>
    protected void btnDangKy_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        string email = txtEmail.Text.Trim().ToLower();
        string mk    = txtMatKhau.Text;

        var (ok, error) = TaiKhoanBLL.DangKy(email, mk, maQuyen: 3);

        if (ok)
        {
            if (error == "CHECK_EMAIL")
            {
                litThongBao.Text = @"<div class='alert alert-success'>
                    <i class='bi bi-envelope-check me-2'></i>
                    <strong>Đăng ký thành công!</strong> Chúng tôi đã gửi email xác nhận tới <b>" + email + @"</b>.
                    Vui lòng kiểm tra hộp thư (kể cả thư mục Spam) và nhấn vào đường dẫn để kích hoạt tài khoản.
                </div>";
            }
            else
            {
                litThongBao.Text = @"<div class='alert alert-success'>
                    <i class='bi bi-check-circle me-2'></i>
                    <strong>Đăng ký thành công!</strong> Tài khoản của bạn đã được kích hoạt.
                    <a href='/login.aspx' class='alert-link'>Đăng nhập ngay</a>
                </div>";
            }
            btnDangKy.Enabled = false;
        }
        else
        {
            litThongBao.Text = $"<div class='alert alert-danger'><i class='bi bi-exclamation-triangle me-2'></i>{error}</div>";
        }
    }
}
