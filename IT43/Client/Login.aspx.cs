using System;
using System.Web.UI;

/// <summary>
/// Trang Đăng nhập — xác thực email/mật khẩu qua TaiKhoanBLL,
/// xử lý các trường hợp: sai mật khẩu, bị khoá, chưa xác nhận email.
/// </summary>
public partial class Account_Login : Page
{
    /// <summary>Nếu đã đăng nhập → redirect về ReturnUrl hoặc trang tương ứng role.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated) return;

        // Đã đăng nhập → redirect về ReturnUrl an toàn hoặc trang theo role
        string returnUrl = SecurityHelper.GetSafeReturnUrl(
            Request.QueryString["ReturnUrl"] ?? Request.QueryString["returnUrl"]);
        if (!string.IsNullOrEmpty(returnUrl))
        {
            Response.Redirect(returnUrl);
            return;
        }

        // Không có ReturnUrl → redirect về trang chủ theo role
        int maQuyen = SecurityHelper.GetCurrentMaQuyen();
        Response.Redirect(SecurityHelper.GetRedirectUrlByRole(maQuyen));
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

        var (kq, maTK, role, maTruong) = TaiKhoanBLL.DangNhap(email, mk);

        switch (kq)
        {
            case TaiKhoanBLL.KetQuaDangNhap.ThanhCong:
                SecurityHelper.SignIn(maTK, email, role, chkNhoToi.Checked, maTruong);

                // Kiểm tra cờ yêu cầu đổi mật khẩu bắt buộc (admin tạo TK / reset MK)
                if (TaiKhoanDAL.GetYeuCauDoiMatKhau(maTK))
                {
                    Response.Redirect("~/Client/DoiMatKhauBatBuoc.aspx");
                    return;
                }

                // Chỉ redirect ReturnUrl nội bộ an toàn — ngăn Open Redirect attack
                // FormsAuth gửi "ReturnUrl" (uppercase R), cần đọc cả 2 case
                string returnUrl = SecurityHelper.GetSafeReturnUrl(
                    Request.QueryString["ReturnUrl"] ?? Request.QueryString["returnUrl"]);
                if (!string.IsNullOrEmpty(returnUrl))
                    Response.Redirect(returnUrl);
                else
                {
                    // Dùng biến role từ BLL (KHÔNG dùng GetCurrentMaQuyen — cookie chưa được gửi lại
                    // trong cùng request nên FormsIdentity chưa khả dụng → luôn trả 0)
                    int maQuyenLogin = SecurityHelper.RoleToMaQuyen(role);
                    Response.Redirect(SecurityHelper.GetRedirectUrlByRole(maQuyenLogin));
                }
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
