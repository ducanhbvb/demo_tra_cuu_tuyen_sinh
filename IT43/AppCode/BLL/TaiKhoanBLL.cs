using System;
using System.Configuration;

/// <summary>
/// Business Logic Layer — Tài khoản người dùng.
/// Xử lý đăng nhập (brute-force protect, auto upgrade hash), đăng ký,
/// xác nhận email, quên/đặt lại mật khẩu, đổi mật khẩu.
/// </summary>
public static class TaiKhoanBLL
{
    /// <summary>Kết quả trả về từ hàm đăng nhập.</summary>
    public enum KetQuaDangNhap { ThanhCong = 0, SaiMatKhau = 1, BiKhoa = 2, ChuaXacNhanEmail = 3, KhongTonTai = 4 }

    /// <summary>Kiểm tra SMTP đã cấu hình trong Web.config hay chưa (SmtpUser, SmtpPass, SmtpFrom).</summary>
    public static bool SmtpDaCauHinh =>
        !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SmtpUser"])
     && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SmtpPass"])
     && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SmtpFrom"]);

    /// <summary>
    /// Đăng nhập — verify mật khẩu (hỗ trợ SHA256 cũ + PBKDF2 mới),
    /// gọi SP brute-force protect, auto upgrade hash nếu cần.
    /// </summary>
    /// <returns>Tuple (kết quả, mã tài khoản, role name).</returns>
    public static (KetQuaDangNhap ketQua, int maTaiKhoan, string role) DangNhap(string email, string matKhauPlain)
    {
        var tk = TaiKhoanDAL.GetByEmail(email);

        if (tk == null)
        {
            TaiKhoanDAL.DangNhap(email, "");
            LogHelper.Ghi(null, "DANG_NHAP",
                moTa: $"Đăng nhập thất bại — email không tồn tại ({email})",
                isSuccess: false, maLoi: "KhongTonTai");
            return (KetQuaDangNhap.KhongTonTai, 0, "");
        }

        bool matKhauDung = SecurityHelper.VerifyPassword(matKhauPlain, tk.MatKhau);

        // Với PBKDF2 (per-user salt) không thể truyền hash vào SP để so sánh.
        // Nếu đúng → truyền hash hiện tại DB (luôn khớp), sai → truyền giả (SP tăng counter).
        string hashChoSP = matKhauDung ? tk.MatKhau : SecurityHelper.HashPassword(matKhauPlain);

        var (kq, maTK, maQuyen) = TaiKhoanDAL.DangNhap(email, hashChoSP);

        string role   = maQuyen switch { 1 => "Admin", 2 => "TruongHoc", _ => "HocSinh" };
        var ketQua    = (KetQuaDangNhap)kq;
        bool ok       = ketQua == KetQuaDangNhap.ThanhCong;
        int? logMaTK  = maTK > 0 ? maTK : (int?)null;

        LogHelper.Ghi(logMaTK, "DANG_NHAP",
            moTa: ok ? $"Đăng nhập thành công ({email})" : $"Đăng nhập thất bại ({email}) - {ketQua}",
            isSuccess: ok,
            maLoi: ok ? null : kq.ToString());

        // Auto upgrade SHA256 cũ → PBKDF2 khi đăng nhập thành công
        if (ok && SecurityHelper.IsLegacyHash(tk.MatKhau))
        {
            try
            {
                string newHash = SecurityHelper.HashPassword(matKhauPlain);
                TaiKhoanDAL.DoiMatKhau(maTK, newHash);
                LogHelper.Ghi(maTK, "UPGRADE_HASH", "Tự động nâng cấp hash mật khẩu SHA256 → PBKDF2");
            }
            catch { /* không để lỗi upgrade làm gián đoạn đăng nhập */ }
        }

        return (ketQua, maTK, role);
    }

    /// <summary>
    /// Đăng ký tài khoản mới — hash PBKDF2, gửi email xác nhận (nếu SMTP có)
    /// hoặc tự kích hoạt.
    /// </summary>
    /// <returns>Tuple (thành công, thông báo/lỗi — "CHECK_EMAIL" nếu cần xác nhận).</returns>
    public static (bool ok, string error) DangKy(string email, string matKhau, int maQuyen = 3)
    {
        if (TaiKhoanDAL.EmailTonTai(email))
            return (false, "Email này đã được đăng ký.");

        string hash = SecurityHelper.HashPassword(matKhau);

        if (SmtpDaCauHinh)
        {
            string token = SecurityHelper.GenerateToken();
            int maTK = TaiKhoanDAL.DangKy(email, hash, maQuyen, token);
            try { EmailHelper.GuiXacNhanEmail(email, token); }
            catch {
                LogHelper.Ghi(maTK, "SEND_EMAIL_FAIL", $"Không gửi được email xác nhận tới {email}", isSuccess: false);
            }
            return (true, "CHECK_EMAIL");
        }
        else
        {
            TaiKhoanDAL.DangKy(email, hash, maQuyen);
            return (true, null);
        }
    }

    /// <summary>Xác nhận email bằng token — kích hoạt tài khoản.</summary>
    public static bool XacNhanEmail(string token) => TaiKhoanDAL.XacNhanEmail(token);

    /// <summary>Gửi email chứa link đặt lại mật khẩu.</summary>
    public static (bool ok, string error) GuiEmailDatLai(string email)
    {
        string token = SecurityHelper.GenerateToken();
        bool ok = TaiKhoanDAL.TaoTokenDatLai(email, token);
        if (!ok) return (false, "Email không tồn tại trong hệ thống.");

        try { EmailHelper.GuiDatLaiMatKhau(email, token); }
        catch { return (false, "Không gửi được email. Vui lòng thử lại sau."); }

        return (true, null);
    }

    /// <summary>Đặt lại mật khẩu bằng token reset — validate độ dài, hash PBKDF2.</summary>
    public static (bool ok, string error) DatLaiMatKhau(string token, string matKhauMoi)
    {
        if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 6)
            return (false, "Mật khẩu phải có ít nhất 6 ký tự.");

        bool ok = TaiKhoanDAL.DatLaiMatKhau(token, SecurityHelper.HashPassword(matKhauMoi));
        return ok ? (true, null) : (false, "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.");
    }

    /// <summary>Đổi mật khẩu — verify mật khẩu cũ, validate mới, hash PBKDF2.</summary>
    public static (bool ok, string error) DoiMatKhau(int maTaiKhoan, string matKhauCu, string matKhauMoi)
    {
        var tk = TaiKhoanDAL.GetById(maTaiKhoan);
        if (tk == null) return (false, "Tài khoản không tồn tại.");
        if (!SecurityHelper.VerifyPassword(matKhauCu, tk.MatKhau))
            return (false, "Mật khẩu hiện tại không đúng.");
        if (matKhauMoi.Length < 6)
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");

        TaiKhoanDAL.DoiMatKhau(maTaiKhoan, SecurityHelper.HashPassword(matKhauMoi));
        LogHelper.Ghi(maTaiKhoan, "DOI_MAT_KHAU", "Đổi mật khẩu thành công");
        return (true, null);
    }
}
