using System.Collections.Generic;

/// <summary>Business Logic Layer cho danh mục lookup (ngành, phương thức, tỉnh thành...).
/// Áp dụng caching 30 phút cho các danh mục ít thay đổi.</summary>
public static class DanhMucBLL
{
    public static List<LookupModel> GetQuyen()  => DanhMucDAL.GetQuyen();
    public static List<LookupModel> GetCapBac() => DanhMucDAL.GetCapBac();

    /// <summary>Danh mục nhóm ngành — cache 30 phút.</summary>
    public static List<LookupModel> GetDanhMucNganh()
        => CacheHelper.GetOrSet(CacheHelper.KEY_DANH_MUC_NGANH,
            () => DanhMucDAL.GetDanhMucNganh(), 30);

    /// <summary>Danh sách chuyên ngành (tất cả hoặc theo danh mục) — cache 30 phút.
    /// Cache riêng cho từng maDanhMuc.</summary>
    public static List<LookupModel> GetChuyenNganh(int? maDanhMuc = null)
    {
        string key = maDanhMuc.HasValue
            ? $"{CacheHelper.PREFIX_DANHMUC}ChuyenNganh_{maDanhMuc}"
            : CacheHelper.KEY_CHUYEN_NGANH_ALL;
        return CacheHelper.GetOrSet(key, () => DanhMucDAL.GetChuyenNganh(maDanhMuc), 30);
    }

    /// <summary>Phương thức tuyển sinh — cache 30 phút.</summary>
    public static List<LookupModel> GetPhuongThuc()
        => CacheHelper.GetOrSet(CacheHelper.KEY_PHUONG_THUC,
            () => DanhMucDAL.GetPhuongThuc(), 30);

    /// <summary>Tỉnh thành — không cache (nhỏ + hiếm dùng).</summary>
    public static List<string> GetTinhThanh() => DanhMucDAL.GetTinhThanh();

    /// <summary>Năm tuyển sinh — cache 30 phút.</summary>
    public static List<int> GetNamTuyenSinh()
        => CacheHelper.GetOrSet(CacheHelper.KEY_NAM_TUYEN_SINH,
            () => DanhMucDAL.GetNamTuyenSinh(), 30);

    /// <summary>Xóa toàn bộ cache DanhMuc (gọi khi thêm/sửa/xóa ngành, phương thức...).</summary>
    public static void InvalidateCache()
        => CacheHelper.RemoveByPrefix(CacheHelper.PREFIX_DANHMUC);
}
