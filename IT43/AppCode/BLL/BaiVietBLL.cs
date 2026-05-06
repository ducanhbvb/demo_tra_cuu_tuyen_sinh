using System;
using System.Data.SqlClient;
using System.Web;

/// <summary>
/// Business Logic Layer — Bài viết (tbl_BaiViet).
/// Validate dữ liệu, tự tạo slug, quản lý ảnh và gọi DAL cho CRUD bài viết.
/// </summary>
public static class BaiVietBLL
{
    // ── Đọc ───────────────────────────────────────────────────────────────

    /// <summary>Lấy danh sách bài viết với phân trang, dùng cho cả Client và Admin.</summary>
    public static PagedTable GetDanhSach(int? maTruong, int pageIndex, int pageSize,
                                         bool chiActive = true, bool? trangThai = null)
        => BaiVietDAL.GetDanhSach(maTruong, pageIndex, pageSize, chiActive, trangThai);

    /// <summary>Lấy chi tiết bài viết theo slug (tăng lượt xem tự động).</summary>
    public static BaiVietModel GetChiTiet(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return null;
        return BaiVietDAL.GetChiTiet(slug);
    }

    /// <summary>
    /// Lấy slug theo MaBaiViet — dùng khi cần slug để xóa ảnh trước khi xóa bài viết.
    /// </summary>
    public static string GetSlug(int maBaiViet)
    {
        var obj = DBHelper.Scalar(
            "SELECT Slug FROM tbl_BaiViet WHERE MaBaiViet=@id",
            new[] { new SqlParameter("@id", maBaiViet) });
        return obj == null || obj == DBNull.Value ? null : obj.ToString();
    }

    // ── Thêm / Cập nhật ───────────────────────────────────────────────────

    /// <summary>
    /// Validate và thêm bài viết mới.
    /// Tự sinh slug nếu không nhập, không ghi đè ảnh nếu không có file mới.
    /// </summary>
    public static (bool ok, string error) Them(BaiVietModel m)
    {
        if (string.IsNullOrWhiteSpace(m.TieuDe))
            return (false, "Tiêu đề bài viết không được để trống.");

        if (m.MaTacGia <= 0)
            return (false, "Phiên đăng nhập không hợp lệ.");

        if (string.IsNullOrWhiteSpace(m.Slug))
            m.Slug = SlugHelper.ToSlug(m.TieuDe);

        m.NoiDung = HtmlSanitizerHelper.SanitizeRichText(m.NoiDung);

        BaiVietDAL.Them(m);
        return (true, null);
    }

    /// <summary>
    /// Validate và cập nhật bài viết.
    /// Tự động xóa ảnh nội dung bị loại bỏ so với phiên bản cũ.
    /// </summary>
    public static (bool ok, string error) CapNhat(BaiVietModel m)
    {
        if (string.IsNullOrWhiteSpace(m.TieuDe))
            return (false, "Tiêu đề bài viết không được để trống.");

        if (m.MaBaiViet <= 0)
            return (false, "Mã bài viết không hợp lệ.");

        if (string.IsNullOrWhiteSpace(m.Slug))
            m.Slug = SlugHelper.ToSlug(m.TieuDe);

        m.NoiDung = HtmlSanitizerHelper.SanitizeRichText(m.NoiDung);

        // Lấy nội dung cũ để so sánh ảnh trước khi ghi đè
        var old = BaiVietDAL.GetById(m.MaBaiViet);
        BaiVietDAL.CapNhat(m);

        // Xóa ảnh có trong bài cũ nhưng không còn trong bài mới
        if (old != null)
            ImageUploadHelper.DeleteRemovedContentImages(old.NoiDung, m.NoiDung);

        return (true, null);
    }

    // ── Xóa ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Xóa bài viết, ảnh bìa và tất cả ảnh nội dung (nếu có).
    /// </summary>
    public static void Xoa(int maBaiViet)
    {
        // Dùng GetById để lấy ảnh mà không tăng LuotXem (GetChiTiet có side-effect)
        var bv = BaiVietDAL.GetById(maBaiViet);
        if (bv != null)
        {
            // Xóa ảnh bìa
            ImageUploadHelper.DeleteOld(bv.AnhChinh);
            // Xóa toàn bộ ảnh trong nội dung bài viết
            ImageUploadHelper.DeleteContentImages(bv.NoiDung);
        }

        BaiVietDAL.Xoa(maBaiViet);
    }

    // ── Toggle trạng thái ────────────────────────────────────────────────

    /// <summary>Ẩn/hiện bài viết.</summary>
    public static void ToggleTrangThai(int maBaiViet)
        => BaiVietDAL.ToggleTrangThai(maBaiViet);

    // ── Load dropdown ────────────────────────────────────────────────────

    /// <summary>Lấy danh sách tên trường để bind vào dropdown bộ lọc bài viết.</summary>
    public static System.Data.DataTable GetDanhSachTruongFilter()
        => DBHelper.Query("SELECT MaTruong, TenTruong FROM tbl_Truong ORDER BY TenTruong");

    /// <summary>Tìm kiếm bài viết theo từ khóa + trường, phân trang.</summary>
    public static PagedTable SearchBaiViet(string tuKhoa, int? maTruong, int pageIndex, int pageSize)
        => BaiVietDAL.SearchBaiViet(tuKhoa, maTruong, pageIndex, pageSize);

    /// <summary>Lấy 1 bài viết nổi bật (lượt xem cao nhất).</summary>
    public static System.Data.DataTable GetBaiVietNoiBat()
        => BaiVietDAL.GetBaiVietNoiBat();

    /// <summary>Lấy N bài viết mới nhất (sidebar).</summary>
    public static System.Data.DataTable GetMoiNhat(int top = 5)
        => BaiVietDAL.GetMoiNhat(top);

    /// <summary>Lấy N bài viết xem nhiều nhất (sidebar).</summary>
    public static System.Data.DataTable GetXemNhieuNhat(int top = 5)
        => BaiVietDAL.GetXemNhieuNhat(top);
}
