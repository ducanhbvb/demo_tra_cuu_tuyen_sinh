using System;
using System.Web.UI;

/// <summary>
/// Trang Quên mật khẩu — gửi email chứa link đặt lại mật khẩu
/// qua TaiKhoanBLL.GuiEmailDatLai.
/// </summary>
public partial class Account_QuenMatKhau : Page
{
    /// <summary>Xử lý nút Gửi — gọi BLL gửi email reset và hiển thị kết quả.</summary>
    protected void btnGui_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        var (ok, error) = TaiKhoanBLL.GuiEmailDatLai(txtEmail.Text.Trim().ToLower());

        litThongBao.Text = ok
            ? "<div class='alert alert-success'><i class='bi bi-check-circle me-2'></i>Đã gửi email! Vui lòng kiểm tra hộp thư (cả spam).</div>"
            : $"<div class='alert alert-danger'><i class='bi bi-exclamation-triangle me-2'></i>{error}</div>";

        if (ok) btnGui.Enabled = false;
    }
}
