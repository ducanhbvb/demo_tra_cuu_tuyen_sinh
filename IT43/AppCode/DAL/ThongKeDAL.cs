/// <summary>
/// Tổng hợp các câu query thống kê — tách khỏi Presentation Layer,
/// đảm bảo kiến trúc 3-layer nhất quán.
/// Dùng cho: index.aspx, Admin/Default.aspx
/// </summary>
public static class ThongKeDAL
{
    // ── Thống kê trang chủ ────────────────────────────────────────────────
    public static int SoTruong()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_Truong WHERE TrangThai=1"));

    public static int SoNganh()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_ChuyenNganh"));

    public static int SoTinActive()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_TinTuyenSinh WHERE TrangThai=1"));

    public static int NamTuyenSinhMoiNhat()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT ISNULL(MAX(NamTuyenSinh),0) FROM tbl_TinTuyenSinh"));

    // ── Thống kê Admin dashboard ──────────────────────────────────────────
    public static int SoTin()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_TinTuyenSinh"));

    public static int SoTuVanCho()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_TuVan WHERE TrangThai=0"));

    public static int SoGopYCho()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_GopY WHERE TrangThai=0"));

    public static int SoTaiKhoan()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_TaiKhoan"));

    // ── Thống kê Module quản lý (Tài khoản, Bài viết, Tin) ───────────────────
    public static (int tong, int qAdmin, int qTruong, int qUser, int chuaXacNhan, int dangHoatDong) ThongKeTaiKhoan()
    {
        var dt = DBHelper.Query(@"
            SELECT 
                COUNT(1) as Tong,
                SUM(CASE WHEN MaQuyen=1 THEN 1 ELSE 0 END) as Q1,
                SUM(CASE WHEN MaQuyen=2 THEN 1 ELSE 0 END) as Q2,
                SUM(CASE WHEN MaQuyen=3 THEN 1 ELSE 0 END) as Q3,
                SUM(CASE WHEN EmailDaXacNhan=0 THEN 1 ELSE 0 END) as ChuaXN,
                SUM(CASE WHEN TrangThai=1 THEN 1 ELSE 0 END) as HoatDong
            FROM tbl_TaiKhoan");
        if (dt == null || dt.Rows.Count == 0) return (0, 0, 0, 0, 0, 0);
        var r = dt.Rows[0];
        return (
            DBHelper.Val<int>(r["Tong"]),
            DBHelper.Val<int>(r["Q1"]),
            DBHelper.Val<int>(r["Q2"]),
            DBHelper.Val<int>(r["Q3"]),
            DBHelper.Val<int>(r["ChuaXN"]),
            DBHelper.Val<int>(r["HoatDong"])
        );
    }

    public static (int tong, int hienThi, int luotXem) ThongKeBaiViet()
    {
        var dt = DBHelper.Query(@"
            SELECT
                COUNT(1) as Tong,
                SUM(CASE WHEN TrangThai=1 THEN 1 ELSE 0 END) as HienThi,
                SUM(ISNULL(LuotXem, 0)) as LuotXem
            FROM tbl_BaiViet");
        if (dt == null || dt.Rows.Count == 0) return (0, 0, 0);
        var r = dt.Rows[0];
        return (
            DBHelper.Val<int>(r["Tong"]),
            DBHelper.Val<int>(r["HienThi"]),
            DBHelper.Val<int>(r["LuotXem"])
        );
    }

    public static (int tong, int hienThi, int luotXem) ThongKeTinTuyenSinh()
    {
        var dt = DBHelper.Query(@"
            SELECT
                COUNT(1) as Tong,
                SUM(CASE WHEN TrangThai=1 THEN 1 ELSE 0 END) as HienThi,
                SUM(ISNULL(LuotXem, 0)) as LuotXem
            FROM tbl_TinTuyenSinh");
        if (dt == null || dt.Rows.Count == 0) return (0, 0, 0);
        var r = dt.Rows[0];
        return (
            DBHelper.Val<int>(r["Tong"]),
            DBHelper.Val<int>(r["HienThi"]),
            DBHelper.Val<int>(r["LuotXem"])
        );
    }
}
