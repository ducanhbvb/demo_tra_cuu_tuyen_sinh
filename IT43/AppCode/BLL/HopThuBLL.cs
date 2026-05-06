using System;
using System.Data;

/// <summary>
/// Business Logic Layer — Hộp thư người dùng (HocSinh xem tư vấn + phản hồi).
/// Cache count 1 phút để giảm DB hit cho navbar badge polling mỗi 60s.
/// </summary>
public static class HopThuBLL
{
    private const int CACHE_MINUTES = 1;
    private static string CacheKey(int id) => $"hopthu_count_{id}";

    // ── Đếm tin chưa đọc (có cache) ─────────────────────────────────────────

    /// <summary>
    /// Số phản hồi chưa đọc của HocSinh — cache 1 phút.
    /// Dùng cho navbar bell badge + AJAX polling endpoint.
    /// </summary>
    public static int DemChuaDoc(int maTaiKhoan)
        => CacheHelper.GetOrSet(
               CacheKey(maTaiKhoan),
               () => HopThuDAL.DemChuaDoc(maTaiKhoan),
               minutes: CACHE_MINUTES);

    /// <summary>
    /// Xoá cache count — gọi khi có phản hồi mới (sau GuiPhanHoi) hoặc user đọc xong.
    /// </summary>
    public static void InvalidateCache(int maTaiKhoan)
        => CacheHelper.Remove(CacheKey(maTaiKhoan));

    // ── Danh sách hộp thư (không cache — cần fresh) ──────────────────────────

    /// <summary>
    /// Danh sách toàn bộ tư vấn của HocSinh kèm thống kê phản hồi.
    /// Không cache vì cần hiển thị trạng thái đọc/chưa đọc chính xác.
    /// </summary>
    public static DataTable GetDanhSach(int maTaiKhoan)
        => HopThuDAL.GetByUser(maTaiKhoan);

    /// <summary>
    /// Lấy N tư vấn mới nhất để hiển thị dropdown preview trong navbar.
    /// </summary>
    public static DataTable GetPreview(int maTaiKhoan, int soLuong = 5)
    {
        var dt = HopThuDAL.GetByUser(maTaiKhoan);
        // DataTable đã sắp xếp theo phản hồi mới nhất, lấy N rows đầu
        if (dt.Rows.Count > soLuong)
        {
            var clone = dt.Clone();
            for (int i = 0; i < soLuong; i++)
                clone.ImportRow(dt.Rows[i]);
            return clone;
        }
        return dt;
    }

    // ── Đánh dấu đã đọc + xoá cache ─────────────────────────────────────────

    /// <summary>
    /// Đánh dấu tất cả reply trong 1 tư vấn là đã đọc + xoá cache count.
    /// Gọi khi HocSinh mở modal xem chi tiết thread.
    /// </summary>
    public static void DanhDauDaDoc(int maTuVan, int maTaiKhoan)
    {
        TuVanPhanHoiDAL.DanhDauDaDoc(maTuVan, maTaiKhoan);
        InvalidateCache(maTaiKhoan);
    }
}
