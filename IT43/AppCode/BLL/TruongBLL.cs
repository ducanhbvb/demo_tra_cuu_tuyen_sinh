using System;
using System.Collections.Generic;

/// <summary>
/// Business Logic Layer — Trường đại học/cao đẳng.
/// Validate dữ liệu, tự tạo slug và gọi DAL cho CRUD trường.
/// </summary>
public static class TruongBLL
{
    /// <summary>Lấy danh sách trường đang hiển thị dạng dropdown (Id + Ten), dùng cho filter bar / modal form.
    /// Cache 15 phút.</summary>
    public static List<LookupModel> GetDanhSachDropdown()
        => CacheHelper.GetOrSet(CacheHelper.KEY_TRUONG_DROPDOWN, () =>
        {
            var dt = DBHelper.Query("SELECT MaTruong AS Id, TenTruong AS Ten, 0 AS ThuTu FROM tbl_Truong WHERE TrangThai = 1 ORDER BY TenTruong");
            var list = new List<LookupModel>();
            foreach (System.Data.DataRow r in dt.Rows)
                list.Add(new LookupModel
                {
                    Id    = DBHelper.Val<int>(r["Id"]),
                    Ten   = r["Ten"].ToString(),
                    ThuTu = 0
                });
            return list;
        }, 15);

    /// <summary>Tìm kiếm trường với nhiều bộ lọc, phân trang.</summary>
    public static PagedTable TimKiem(string tenTruong, string tinhThanh, byte? maVung,
        byte? loaiTruong, int? maChuyenNganh, short? nam, decimal? diemMin, decimal? diemMax,
        string sortBy = "TenTruong", string sortDirection = "ASC",
        int pageIndex = 0, int pageSize = 12, bool? trangThai = null,
        byte? capBacDaoTao = null)
        => TruongDAL.TimKiem(tenTruong, tinhThanh, maVung, loaiTruong, maChuyenNganh, nam, diemMin, diemMax, sortBy, sortDirection, pageIndex, pageSize, trangThai, capBacDaoTao);

    /// <summary>Lấy chi tiết trường đang hiển thị theo mã hoặc slug.</summary>
    public static TruongModel LayChiTiet(int? maTruong = null, string slug = null)
        => TruongDAL.LayChiTiet(maTruong, slug);

    /// <summary>Lấy chi tiết trường cho Admin, bao gồm cả trường đã ẩn.</summary>
    public static TruongModel LayChiTietAdmin(int maTruong)
        => TruongDAL.LayChiTietAdmin(maTruong);

    /// <summary>Validate tên trường, tự tạo slug nếu thiếu, và thêm trường mới.</summary>
    public static (bool ok, string error) Them(TruongModel m)
    {
        if (string.IsNullOrWhiteSpace(m.TenTruong))
            return (false, "Tên trường không được để trống.");

        if (string.IsNullOrEmpty(m.Slug))
            m.Slug = SlugHelper.ToSlug(m.TenTruong);

        m.MoTa = HtmlSanitizerHelper.SanitizeRichText(m.MoTa);

        TruongDAL.Them(m);
        InvalidateCache();
        return (true, null);
    }

    /// <summary>Validate tên trường, tự tạo slug nếu thiếu, và cập nhật trường.</summary>
    public static (bool ok, string error) CapNhat(TruongModel m)
    {
        if (string.IsNullOrWhiteSpace(m.TenTruong))
            return (false, "Tên trường không được để trống.");

        if (string.IsNullOrEmpty(m.Slug))
            m.Slug = SlugHelper.ToSlug(m.TenTruong);

        m.MoTa = HtmlSanitizerHelper.SanitizeRichText(m.MoTa);

        TruongDAL.CapNhat(m);
        InvalidateCache();
        return (true, null);
    }

    /// <summary>Xóa tạm đúng 1 record trường theo mã trường và khóa tài khoản trường liên quan.</summary>
    public static void Xoa(int maTruong)
    {
        TruongDAL.Xoa(maTruong);
        InvalidateCache();
    }

    /// <summary>Khôi phục đúng 1 record trường theo mã trường và mở lại tài khoản trường liên quan.</summary>
    public static void KhoiPhuc(int maTruong)
    {
        TruongDAL.KhoiPhuc(maTruong);
        InvalidateCache();
    }

    /// <summary>Lấy danh sách dữ liệu đang ràng buộc trường trước khi hard-delete.</summary>
    public static Dictionary<string, int> GetDeleteDependencies(int maTruong)
        => TruongDAL.GetDeleteDependencies(maTruong);

    /// <summary>Xóa vĩnh viễn trường theo mã trường sau khi xác thực lại mật khẩu admin.</summary>
    public static (bool ok, string error) XoaVinhVien(int maTruong, int adminId, string adminPassword)
    {
        if (adminId <= 0)
            return (false, "Phiên đăng nhập admin không hợp lệ. Vui lòng đăng nhập lại.");

        if (string.IsNullOrWhiteSpace(adminPassword))
            return (false, "Vui lòng nhập mật khẩu admin hiện tại để xác nhận xóa vĩnh viễn.");

        var admin = TaiKhoanDAL.GetById(adminId);
        if (admin == null || admin.MaQuyen != 1)
            return (false, "Chỉ tài khoản Admin mới được xóa vĩnh viễn trường.");

        if (!SecurityHelper.VerifyPassword(adminPassword, admin.MatKhau))
            return (false, "Mật khẩu admin không đúng. Trường chưa bị xóa vĩnh viễn.");

        var truong = TruongDAL.LayChiTietAdmin(maTruong);
        if (truong == null)
            return (false, "Không tìm thấy trường cần xóa vĩnh viễn.");

        if (truong.TrangThai)
            return (false, "Chỉ được xóa vĩnh viễn trường đang ở trạng thái Đã ẩn. Vui lòng xóa tạm trường trước.");

        try
        {
            var imagePaths = TruongDAL.GetLocalImagePathsForHardDelete(maTruong);
            TruongDAL.XoaVinhVien(maTruong);

            foreach (var path in imagePaths)
                ImageUploadHelper.DeleteOld(path);

            InvalidateCache();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, "Không thể xóa vĩnh viễn trường: " + ex.Message);
        }
    }

    /// <summary>Toggle trạng thái hiển thị trường và xóa cache liên quan.</summary>
    public static void ToggleTrangThai(int maTruong)
    {
        TruongDAL.ToggleTrangThai(maTruong);
        InvalidateCache();
    }

    /// <summary>Xóa cache liên quan đến trường (Dropdown + ThongKe trang chủ).</summary>
    public static void InvalidateCache()
    {
        CacheHelper.Remove(CacheHelper.KEY_TRUONG_DROPDOWN);
        // Xóa thêm ThongKe vì số lượng trường có thể thay đổi
        ThongKeBLL.InvalidateCache();
    }
}
