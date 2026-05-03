using System;

/// <summary>
/// Business Logic Layer — Trường đại học/cao đẳng.
/// Validate dữ liệu, tự tạo slug và gọi DAL cho CRUD trường.
/// </summary>
public static class TruongBLL
{
    /// <summary>Tìm kiếm trường với nhiều bộ lọc, phân trang.</summary>
    public static PagedTable TimKiem(string tenTruong, string tinhThanh, byte? maVung,
        byte? loaiTruong, int? maChuyenNganh, int pageIndex = 0, int pageSize = 12,
        bool? trangThai = null)
        => TruongDAL.TimKiem(tenTruong, tinhThanh, maVung, loaiTruong, maChuyenNganh, pageIndex, pageSize, trangThai);

    /// <summary>Lấy chi tiết trường theo mã hoặc slug.</summary>
    public static TruongModel LayChiTiet(int? maTruong = null, string slug = null)
        => TruongDAL.LayChiTiet(maTruong, slug);

    /// <summary>Validate tên trường, tự tạo slug nếu thiếu, và thêm trường mới.</summary>
    public static (bool ok, string error) Them(TruongModel m)
    {
        if (string.IsNullOrWhiteSpace(m.TenTruong))
            return (false, "Tên trường không được để trống.");

        if (string.IsNullOrEmpty(m.Slug))
            m.Slug = SlugHelper.ToSlug(m.TenTruong);

        TruongDAL.Them(m);
        return (true, null);
    }

    /// <summary>Validate tên trường, tự tạo slug nếu thiếu, và cập nhật trường.</summary>
    public static (bool ok, string error) CapNhat(TruongModel m)
    {
        if (string.IsNullOrWhiteSpace(m.TenTruong))
            return (false, "Tên trường không được để trống.");

        if (string.IsNullOrEmpty(m.Slug))
            m.Slug = SlugHelper.ToSlug(m.TenTruong);

        TruongDAL.CapNhat(m);
        return (true, null);
    }

    /// <summary>Xóa trường theo mã trường.</summary>
    public static void Xoa(int maTruong) => TruongDAL.Xoa(maTruong);
}
