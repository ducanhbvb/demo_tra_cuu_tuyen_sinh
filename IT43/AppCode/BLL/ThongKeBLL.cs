using System.Data;

/// <summary>Business Logic Layer cho thống kê — trang chủ và admin dashboard.
/// Áp dụng caching 10 phút cho các số liệu thống kê.</summary>
public static class ThongKeBLL
{
    // ── Trang chủ (cache 10 phút) ─────────────────────────────────────────
    public static int SoTruong()
        => CacheHelper.GetOrSet(CacheHelper.KEY_SO_TRUONG,
            () => ThongKeDAL.SoTruong(), 10);

    public static int SoNganh()
        => CacheHelper.GetOrSet(CacheHelper.KEY_SO_NGANH,
            () => ThongKeDAL.SoNganh(), 10);

    public static int SoTinActive()
        => CacheHelper.GetOrSet(CacheHelper.KEY_SO_TIN_ACTIVE,
            () => ThongKeDAL.SoTinActive(), 10);

    public static int NamTuyenSinhMoiNhat()
    {
        // Ưu tiên lấy từ Config admin nếu đã được thiết lập (khác 0)
        int configNam = ConfigBLL.GetInt("NamTuyenSinhHienTai", 0);
        if (configNam > 0) return configNam;

        // Fallback: lấy MAX(NamTuyenSinh) từ DB với cache 10 phút
        return CacheHelper.GetOrSet(CacheHelper.KEY_NAM_MOI_NHAT,
            () => ThongKeDAL.NamTuyenSinhMoiNhat(), 10);
    }

    // ── Admin dashboard (không cache — cần số liệu realtime) ─────────────
    public static int SoTin()                 => ThongKeDAL.SoTin();
    public static int SoTuVanCho()            => ThongKeDAL.SoTuVanCho();
    public static int SoGopYCho()             => ThongKeDAL.SoGopYCho();
    public static int SoTaiKhoan()            => ThongKeDAL.SoTaiKhoan();

    // ── Module Stats ────────────────────────────────────────────────────────
    public static (int tong, int qAdmin, int qTruong, int qUser, int chuaXacNhan, int dangHoatDong) ThongKeTaiKhoan()
        => ThongKeDAL.ThongKeTaiKhoan();

    public static (int tong, int hienThi, int luotXem) ThongKeBaiViet()
        => ThongKeDAL.ThongKeBaiViet();

    public static (int tong, int hienThi, int luotXem) ThongKeTinTuyenSinh()
        => ThongKeDAL.ThongKeTinTuyenSinh();

    // ── Cổng Trường (lọc theo MaTruong) ──────────────────────────────────
    /// <summary>Số tư vấn chờ xử lý của trường — dùng cho badge sidebar TruongHoc.</summary>
    public static int SoTuVanChoCuaTruong(int maTruong)
    {
        var v = DBHelper.Scalar(
            "SELECT COUNT(*) FROM tbl_TuVan WHERE MaTruong=@mt AND TrangThai=0",
            new[] { new System.Data.SqlClient.SqlParameter("@mt", maTruong) });
        return v == null ? 0 : (int)v;
    }

    /// <summary>Số góp ý chờ xử lý của trường — dùng cho badge sidebar TruongHoc.</summary>
    public static int SoGopYChoCuaTruong(int maTruong)
    {
        var v = DBHelper.Scalar(
            "SELECT COUNT(*) FROM tbl_GopY WHERE MaTruong=@mt AND TrangThai=0",
            new[] { new System.Data.SqlClient.SqlParameter("@mt", maTruong) });
        return v == null ? 0 : (int)v;
    }

    /// <summary>Top N trường mới cập nhật — dùng cho Admin dashboard (không cache).</summary>
    public static DataTable GetTopTruong(int top = 5)
        => DBHelper.Query($"SELECT TOP {top} MaTruong,TenTruong,TinhThanh,Website,LoaiTruong,ThoiGianCapNhat FROM tbl_Truong ORDER BY ThoiGianCapNhat DESC");

    /// <summary>Top N trường nổi bật có kiểm định — dùng cho trang chủ (cache 10 phút).</summary>
    public static DataTable GetTruongNoiBat(int top = 8)
        => CacheHelper.GetOrSet(CacheHelper.KEY_TRUONG_NOI_BAT,
            () => DBHelper.Query(
                $"SELECT TOP {top} MaTruong,TenTruong,TinhThanh,AnhDaiDien,Slug,LoaiTruong,CapBacDaoTao " +
                "FROM tbl_Truong WHERE KiemDinhChatLuong=1 AND TrangThai=1 ORDER BY ThoiGianCapNhat DESC"),
            10);

    /// <summary>Xóa cache thống kê trang chủ (gọi khi thêm/sửa/xóa trường, tin tuyển sinh).</summary>
    public static void InvalidateCache()
        => CacheHelper.RemoveByPrefix(CacheHelper.PREFIX_THONGKE);
}
