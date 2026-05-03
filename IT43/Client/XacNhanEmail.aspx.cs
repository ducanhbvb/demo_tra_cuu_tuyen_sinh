using System;
using System.Web.UI;

/// <summary>
/// Trang Xác nhận email — kích hoạt tài khoản qua token từ QueryString,
/// hiển thị kết quả thành công hoặc lỗi (token hết hạn / đã dùng).
/// </summary>
public partial class Account_XacNhanEmail : Page
{
    /// <summary>Đọc token từ URL, gọi TaiKhoanBLL.XacNhanEmail và hiển thị kết quả.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string token = Request.QueryString["token"];
        if (string.IsNullOrEmpty(token))
        {
            litThongBao.Text = "<div class='alert alert-danger'>Link không hợp lệ.</div>";
            return;
        }

        bool ok = TaiKhoanBLL.XacNhanEmail(token);
        litThongBao.Text = ok
            ? @"<div class='alert alert-success display-icon'>
                <i class='bi bi-patch-check-fill text-success fs-1 d-block mb-3'></i>
                <h5>Xác nhận thành công!</h5>
                <p class='mb-2'>Tài khoản của bạn đã được kích hoạt.</p>
                <a href='Login.aspx' class='btn btn-primary'>Đăng nhập ngay</a>
               </div>"
            : "<div class='alert alert-danger'>Link xác nhận không hợp lệ hoặc đã được sử dụng.</div>";
    }
}
