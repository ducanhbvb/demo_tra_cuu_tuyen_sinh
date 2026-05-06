using System;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Lịch sử phản hồi tư vấn (tbl_TuVanPhanHoi).
/// Mỗi row = 1 lượt nói trong timeline thread: câu hỏi gốc, reply admin, system event, reply học sinh.
/// </summary>
public static class TuVanPhanHoiDAL
{
    /// <summary>
    /// Thêm 1 lượt phản hồi vào timeline của tư vấn.
    /// Trả về ID mới được insert.
    /// </summary>
    /// <param name="maTuVan">ID của bản ghi tbl_TuVan.</param>
    /// <param name="maTaiKhoan">NULL nếu là System event hoặc người dùng anonymous.</param>
    /// <param name="loaiNguoi">HocSinh | Admin | Moderator | TuVanVien | TruongHoc | System</param>
    /// <param name="hoTen">Cache tên để render timeline mà không JOIN thêm.</param>
    /// <param name="noiDung">Nội dung phản hồi.</param>
    public static int Them(int maTuVan, int? maTaiKhoan, string loaiNguoi, string hoTen, string noiDung)
    {
        const string SQL = @"
            INSERT INTO tbl_TuVanPhanHoi (MaTuVan, MaTaiKhoan, LoaiNguoi, HoTen, NoiDung)
            VALUES (@mt, @tk, @loai, @hoTen, @noi);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        var result = DBHelper.Scalar(SQL, new SqlParameter[] {
            new SqlParameter("@mt",    maTuVan),
            new SqlParameter("@tk",    maTaiKhoan.HasValue ? (object)maTaiKhoan.Value : DBNull.Value),
            new SqlParameter("@loai",  loaiNguoi),
            new SqlParameter("@hoTen", (object)hoTen ?? DBNull.Value),
            new SqlParameter("@noi",   noiDung),
        });
        return DBHelper.Val<int>(result);
    }

    /// <summary>
    /// Lấy toàn bộ timeline của 1 tư vấn, sắp xếp cũ → mới.
    /// Cột trả về: ID, MaTaiKhoan, LoaiNguoi, HoTen, NoiDung, NgayPhanHoi, DaDocBoiUser, DaGuiEmail.
    /// </summary>
    public static DataTable GetByMaTuVan(int maTuVan)
        => DBHelper.Query(
            "SELECT * FROM tbl_TuVanPhanHoi WHERE MaTuVan = @id ORDER BY NgayPhanHoi ASC, ID ASC",
            new[] { new SqlParameter("@id", maTuVan) });

    /// <summary>
    /// Lấy timeline của 1 tư vấn cho HocSinh hiện tại, có kiểm tra ownership để tránh xem nhầm thread.
    /// </summary>
    public static DataTable GetByMaTuVan(int maTuVan, int maTaiKhoanHocSinh)
    {
        const string SQL = @"
            SELECT ph.ID, ph.MaTuVan, ph.MaTaiKhoan, ph.LoaiNguoi, ph.HoTen,
                   ph.NoiDung, ph.NgayPhanHoi, ph.DaDocBoiUser, ph.DaGuiEmail
            FROM tbl_TuVanPhanHoi ph
            INNER JOIN tbl_TuVan tv ON tv.ID = ph.MaTuVan
            WHERE ph.MaTuVan = @id
              AND tv.MaTaiKhoan = @tk
              AND ISNULL(tv.DaXoa, 0) = 0
            ORDER BY ph.NgayPhanHoi ASC, ph.ID ASC";

        return DBHelper.Query(SQL, new[] {
            new SqlParameter("@id", maTuVan),
            new SqlParameter("@tk", maTaiKhoanHocSinh)
        });
    }

    /// <summary>
    /// Đánh dấu tất cả reply từ phía khác (không phải HocSinh) là đã đọc.
    /// Chỉ có owner (MaTaiKhoan khớp tbl_TuVan) mới được phép — stored proc bảo vệ IDOR.
    /// </summary>
    public static void DanhDauDaDoc(int maTuVan, int maTaiKhoan)
        => DBHelper.ExecSP("sp_HopThu_DanhDauDaDoc", new[] {
            new SqlParameter("@MaTuVan",    maTuVan),
            new SqlParameter("@MaTaiKhoan", maTaiKhoan)
        });

    /// <summary>
    /// Đánh dấu email của 1 phản hồi là đã gửi.
    /// Gọi sau khi EmailHelper.GuiPhanHoiTuVan() thành công.
    /// </summary>
    public static void DanhDauDaGuiEmail(int phanHoiId)
        => DBHelper.Execute(
            "UPDATE tbl_TuVanPhanHoi SET DaGuiEmail = 1 WHERE ID = @id",
            new[] { new SqlParameter("@id", phanHoiId) });

    /// <summary>
    /// Đếm số lần HocSinh đã gửi reply trong 24 giờ qua — dùng cho rate-limit chống spam.
    /// </summary>
    public static int DemReplyTrongNgay(int maTaiKhoan)
    {
        const string SQL = @"
            SELECT COUNT(*) FROM tbl_TuVanPhanHoi
            WHERE MaTaiKhoan = @tk
              AND LoaiNguoi = 'HocSinh'
              AND NgayPhanHoi >= DATEADD(HOUR, -24, GETDATE())";
        return DBHelper.Val<int>(DBHelper.Scalar(SQL, new[] { new SqlParameter("@tk", maTaiKhoan) }));
    }
}
