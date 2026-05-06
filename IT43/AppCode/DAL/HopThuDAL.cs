using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Hộp thư người dùng (HocSinh xem lịch sử tư vấn + phản hồi).
/// Gọi các stored procedures đã tạo trong migration_phase_b.sql.
/// </summary>
public static class HopThuDAL
{
    /// <summary>
    /// Lấy danh sách toàn bộ tư vấn của 1 HocSinh kèm thống kê phản hồi.
    /// Sắp xếp: tư vấn có phản hồi mới nhất lên đầu.
    /// Cột trả về: ID, MaTruong, TenTruong, CauHoiGoc, NgayGui, TrangThai, DaXoa,
    ///             SoLuotPhanHoi, PhanHoiCuoi, SoChuaDoc.
    /// </summary>
    public static DataTable GetByUser(int maTaiKhoan)
        => DBHelper.Query("sp_HopThu_DanhSachByUser",
               new[] { new SqlParameter("@MaTaiKhoan", maTaiKhoan) }, isSP: true);

    /// <summary>
    /// Đếm số phản hồi chưa đọc của 1 HocSinh.
    /// Dùng cho navbar bell badge — được cache 1 phút ở BLL.
    /// </summary>
    public static int DemChuaDoc(int maTaiKhoan)
        => DBHelper.Val<int>(DBHelper.Scalar("sp_HopThu_DemChuaDoc",
               new[] { new SqlParameter("@MaTaiKhoan", maTaiKhoan) }, isSP: true));
}
