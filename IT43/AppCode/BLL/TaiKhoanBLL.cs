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
    /// <returns>Tuple (kết quả, mã tài khoản, role name, maTruong).</returns>
    public static (KetQuaDangNhap ketQua, int maTaiKhoan, string role, int maTruong) DangNhap(string email, string matKhauPlain)
    {
        var tk = TaiKhoanDAL.GetByEmail(email);

        if (tk == null)
        {
            TaiKhoanDAL.DangNhap(email, "");
            LogHelper.Ghi(null, "DANG_NHAP",
                moTa: $"Đăng nhập thất bại — email không tồn tại ({email})",
                isSuccess: false, maLoi: "KhongTonTai");
            return (KetQuaDangNhap.KhongTonTai, 0, "", 0);
        }

        bool matKhauDung = SecurityHelper.VerifyPassword(matKhauPlain, tk.MatKhau);

        // Với PBKDF2 (per-user salt) không thể truyền hash vào SP để so sánh.
        // Nếu đúng → truyền hash hiện tại DB (luôn khớp), sai → truyền giả (SP tăng counter).
        string hashChoSP = matKhauDung ? tk.MatKhau : SecurityHelper.HashPassword(matKhauPlain);

        var (kq, maTK, maQuyen, maTruong) = TaiKhoanDAL.DangNhap(email, hashChoSP);

        string role   = SecurityHelper.MaQuyenToRole(maQuyen);
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

        return (ketQua, maTK, role, maTruong);
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

    // ── Admin: danh sách & quản lý tài khoản ─────────────────────────────

    /// <summary>Lấy thông tin tài khoản theo ID — dùng cho Admin (preset dropdown Trường).</summary>
    public static TaiKhoanModel GetById(int maTaiKhoan) => TaiKhoanDAL.GetById(maTaiKhoan);

    /// <summary>
    /// Đặt flag YeuCauDoiMatKhau trong DB.
    /// Lưu ý: AdminDatMatKhau đã tự set flag = 1, method này dùng khi cần clear flag (yeuCau=false)
    /// hoặc set lại độc lập mà không thay đổi mật khẩu.
    /// </summary>
    public static void SetForceChangePassword(int maTaiKhoan, bool yeuCau)
    {
        // AdminDatMatKhau đã set YeuCauDoiMatKhau=1 — chỉ cần set=0 khi clear
        DBHelper.Execute(
            "UPDATE tbl_TaiKhoan SET YeuCauDoiMatKhau=@f WHERE MaTaiKhoan=@id",
            new System.Data.SqlClient.SqlParameter[]
            {
                new System.Data.SqlClient.SqlParameter("@f",   yeuCau ? 1 : 0),
                new System.Data.SqlClient.SqlParameter("@id",  maTaiKhoan)
            });

        int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
        LogHelper.Ghi(adminId, "ADMIN_SET_FORCE_PWD",
            $"Set YeuCauDoiMatKhau={yeuCau} cho tài khoản ID={maTaiKhoan}");
    }

    /// <summary>Lấy danh sách tài khoản có phân trang, lọc theo quyền và email — dùng cho Admin.</summary>
    public static PagedTable GetDanhSach(int pageIndex, int pageSize, int? maQuyen = null, string keyword = null)
        => TaiKhoanDAL.GetDanhSach(pageIndex, pageSize, maQuyen, keyword);

    /// <summary>Kích hoạt hoặc khóa tài khoản — dùng cho Admin.</summary>
    /// <exception cref="InvalidOperationException">Khi admin cố tự khóa chính mình.</exception>
    public static void CapNhatTrangThai(int maTaiKhoan, bool trangThai)
    {
        // ★ LỚP 3 — Phòng thủ sâu: chặn admin tự vô hiệu hóa chính mình
        if (maTaiKhoan == SecurityHelper.GetCurrentMaTaiKhoan())
            throw new InvalidOperationException("Không thể thay đổi trạng thái tài khoản của chính bạn.");

        TaiKhoanDAL.CapNhatTrangThai(maTaiKhoan, trangThai);
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

    /// <summary>
    /// Admin tạo tài khoản mới — kích hoạt ngay (không cần xác nhận email).
    /// Dùng cho trang QuanLyTaiKhoan.
    /// </summary>
    public static (bool ok, string error) AdminTaoTaiKhoan(string email, string matKhau, int maQuyen, int? maTruong)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "Email không được để trống.");
        if (TaiKhoanDAL.EmailTonTai(email))
            return (false, "Email này đã được đăng ký.");
        if (string.IsNullOrEmpty(matKhau) || matKhau.Length < 6)
            return (false, "Mật khẩu phải có ít nhất 6 ký tự.");
        if (maQuyen == 2 && (!maTruong.HasValue || maTruong.Value <= 0))
            return (false, "Tài khoản TruongHoc phải được gán trường.");

        string hash = SecurityHelper.HashPassword(matKhau);
        int newId = TaiKhoanDAL.AdminTaoTaiKhoan(email, hash, maQuyen, maQuyen == 2 ? maTruong : null);

        int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
        LogHelper.Ghi(adminId, "ADMIN_TAO_TK",
            $"Admin tạo tài khoản mới: {email} (MaQuyen={maQuyen}, MaTruong={maTruong})");

        return (true, null);
    }

    /// <summary>
    /// Admin cập nhật quyền + MaTruong của tài khoản.
    /// </summary>
    public static (bool ok, string error) AdminCapNhatQuyen(int maTaiKhoan, int maQuyen, int? maTruong)
    {
        // ★ LỚP 3 — Phòng thủ sâu: chặn admin tự hạ/sửa quyền chính mình
        if (maTaiKhoan == SecurityHelper.GetCurrentMaTaiKhoan())
            return (false, "Không thể sửa quyền tài khoản của chính bạn.");

        if (maQuyen == 2 && (!maTruong.HasValue || maTruong.Value <= 0))
            return (false, "Tài khoản TruongHoc phải được gán trường.");

        TaiKhoanDAL.AdminCapNhatQuyen(maTaiKhoan, maQuyen, maQuyen == 2 ? maTruong : null);

        int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
        LogHelper.Ghi(adminId, "ADMIN_CAP_NHAT_QUYEN",
            $"Admin cập nhật quyền tài khoản ID={maTaiKhoan}: MaQuyen={maQuyen}, MaTruong={maTruong}");

        return (true, null);
    }

    /// <summary>
    /// Admin gửi email link reset mật khẩu — Cấp 2 reset.
    /// Tái sử dụng logic GuiEmailDatLai nhưng ghi log admin.
    /// </summary>
    public static (bool ok, string error) AdminGuiEmailReset(int maTaiKhoan)
    {
        var tk = TaiKhoanDAL.GetById(maTaiKhoan);
        if (tk == null) return (false, "Tài khoản không tồn tại.");

        var result = GuiEmailDatLai(tk.Email);

        int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
        LogHelper.Ghi(adminId, "ADMIN_GUI_EMAIL_RESET",
            moTa: $"Admin gửi email reset MK cho tài khoản: {tk.Email}",
            isSuccess: result.ok,
            maLoi: result.ok ? null : result.error);

        return result;
    }

    /// <summary>
    /// Admin đặt mật khẩu trực tiếp — Cấp 3 reset (user mất email).
    /// Không gửi thông báo qua email. Ghi log để audit.
    /// </summary>
    public static (bool ok, string error) AdminDatMatKhauTrucTiep(int maTaiKhoan, string matKhauMoi)
    {
        if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 6)
            return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");

        var tk = TaiKhoanDAL.GetById(maTaiKhoan);
        if (tk == null) return (false, "Tài khoản không tồn tại.");

        TaiKhoanDAL.AdminDatMatKhau(maTaiKhoan, SecurityHelper.HashPassword(matKhauMoi));

        int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
        LogHelper.Ghi(adminId, "ADMIN_DAT_MAT_KHAU",
            $"Admin đặt lại MK trực tiếp cho tài khoản: {tk.Email} (ID={maTaiKhoan})");

        return (true, null);
    }
}
