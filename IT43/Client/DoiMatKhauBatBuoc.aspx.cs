using System;
using System.Text.RegularExpressions;
using System.Web.UI;

/// <summary>
/// Trang đổi mật khẩu bắt buộc — hiển thị khi YeuCauDoiMatKhau = true.
/// User PHẢI đổi xong mới được tiếp tục dùng hệ thống.
/// </summary>
public partial class Account_DoiMatKhauBatBuoc : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Chưa đăng nhập → về login
        if (!User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/Client/Login.aspx");
            return;
        }

        // Nếu không còn cần đổi MK (flag đã 0) → về trang chủ theo role
        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (!TaiKhoanDAL.GetYeuCauDoiMatKhau(maTK))
        {
            int maQuyen = SecurityHelper.GetCurrentMaQuyen();
            Response.Redirect(SecurityHelper.GetRedirectUrlByRole(maQuyen));
        }
    }

    protected void btnLuu_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        string mk  = txtMK.Text;
        string mk2 = txtMK2.Text;

        // Validate độ mạnh mật khẩu
        if (mk.Length < 8)
        {
            ShowError("Mật khẩu phải có ít nhất 8 ký tự.");
            return;
        }
        if (!Regex.IsMatch(mk, @"[A-Z]") || !Regex.IsMatch(mk, @"[0-9]"))
        {
            ShowError("Mật khẩu phải chứa ít nhất 1 chữ hoa và 1 chữ số.");
            return;
        }

        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();

        // Không cho dùng lại mật khẩu cũ
        var tk = TaiKhoanDAL.GetById(maTK);
        if (tk != null && SecurityHelper.VerifyPassword(mk, tk.MatKhau))
        {
            ShowError("Mật khẩu mới không được trùng mật khẩu cũ.");
            return;
        }

        string hash = SecurityHelper.HashPassword(mk);
        TaiKhoanDAL.DoiMatKhauBatBuoc(maTK, hash);

        // Redirect về trang chủ theo role
        int maQuyen = SecurityHelper.GetCurrentMaQuyen();
        Response.Redirect(SecurityHelper.GetRedirectUrlByRole(maQuyen));
    }

    private void ShowError(string msg)
    {
        litThongBao.Text = $"<div class='alert alert-danger'><i class='bi bi-exclamation-triangle me-2'></i>{msg}</div>";
    }
}
