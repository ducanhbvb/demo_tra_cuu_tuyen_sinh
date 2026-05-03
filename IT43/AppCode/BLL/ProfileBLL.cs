using System;

/// <summary>
/// Business Logic Layer — Hồ sơ học sinh.
/// Xử lý validate và gọi DAL để lấy/lưu hồ sơ cá nhân.
/// </summary>
public static class ProfileBLL
{
    /// <summary>Lấy hồ sơ học sinh theo mã tài khoản.</summary>
    public static ProfileHocSinhModel GetProfile(int maTaiKhoan)
        => ProfileDAL.GetByTaiKhoan(maTaiKhoan);

    /// <summary>Validate và lưu hồ sơ học sinh vào DB.</summary>
    public static (bool ok, string error) LuuProfile(ProfileHocSinhModel m)
    {
        if (m.MaTaiKhoan == 0) return (false, "Phiên đăng nhập không hợp lệ.");
        ProfileDAL.LuuProfile(m);
        return (true, null);
    }
}
