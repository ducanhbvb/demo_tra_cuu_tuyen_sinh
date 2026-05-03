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

    public static int SoTaiKhoan()
        => DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_TaiKhoan"));
}
