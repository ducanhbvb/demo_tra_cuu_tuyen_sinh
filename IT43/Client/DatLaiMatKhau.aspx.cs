using System;
using System.Web.UI;

/// <summary>
/// Trang Đặt lại mật khẩu — xác thực token từ email reset,
/// cho phép người dùng nhập mật khẩu mới qua TaiKhoanBLL.DatLaiMatKhau.
/// </summary>
public partial class Account_DatLaiMatKhau : Page
{
    /// <summary>Token reset mật khẩu từ QueryString.</summary>
    private string Token => Request.QueryString["token"];

    /// <summary>Kiểm tra token hợp lệ khi load trang, ẩn form nếu thiếu token.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Token))
        {
            pnlForm.Visible = false;
            litThongBao.Text = "<div class='alert alert-danger'>Link không hợp lệ.</div>";
        }
    }

    /// <summary>
    /// Xử lý nút Lưu — gọi BLL đặt lại mật khẩu với token,
    /// ẩn form và hiển thị kết quả (thành công / token hết hạn).
    /// </summary>
    protected void btnLuu_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        var (ok, error) = TaiKhoanBLL.DatLaiMatKhau(Token, txtMK1.Text);

        if (ok)
        {
            pnlForm.Visible = false;
            litThongBao.Text = @"<div class='alert alert-success'>
                <i class='bi bi-check-circle me-2'></i>
                Đặt lại mật khẩu thành công! <a href='Login.aspx'>Đăng nhập ngay</a>
            </div>";
        }
        else
        {
            litThongBao.Text = $"<div class='alert alert-danger'>{error}</div>";
        }
    }
}
