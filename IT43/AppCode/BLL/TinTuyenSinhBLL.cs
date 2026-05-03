using System;

/// <summary>
/// Business Logic Layer — Tin tuyển sinh.
/// Validate dữ liệu và gọi DAL cho CRUD tin tuyển sinh (client + admin).
/// </summary>
public static class TinTuyenSinhBLL
{
    /// <summary>Tìm kiếm tin tuyển sinh phía Client (chỉ tin đang hiển thị).</summary>
    public static PagedTable TimKiem(int? maTruong, int? maChuyenNganh, int? maPhuongThuc,
        short? nam, decimal? diemTu, decimal? diemDen, string tinhThanh,
        int pageIndex = 0, int pageSize = 20)
        => TinTuyenSinhDAL.TimKiem(maTruong, maChuyenNganh, maPhuongThuc,
            nam, diemTu, diemDen, tinhThanh, pageIndex, pageSize);

    /// <summary>Tìm kiếm tin tuyển sinh phía Admin (bao gồm cả tin đang ẩn).</summary>
    public static PagedTable TimKiemAdmin(int? maTruong, int? maChuyenNganh,
        short? nam, int pageIndex = 0, int pageSize = 20)
        => TinTuyenSinhDAL.TimKiemAdmin(maTruong, maChuyenNganh, nam, pageIndex, pageSize);

    /// <summary>Validate và thêm tin tuyển sinh mới.</summary>
    public static (bool ok, string error) Them(TinTuyenSinhModel m)
    {
        if (m.MaTruong == 0 || m.MaChuyenNganh == 0 || m.MaPhuongThuc == 0)
            return (false, "Vui lòng chọn đầy đủ Trường, Ngành và Phương thức xét tuyển.");
        if (m.NamTuyenSinh < 2000 || m.NamTuyenSinh > 2100)
            return (false, "Năm tuyển sinh không hợp lệ.");

        TinTuyenSinhDAL.Them(m);
        return (true, null);
    }

    /// <summary>Validate và cập nhật tin tuyển sinh.</summary>
    public static (bool ok, string error) CapNhat(TinTuyenSinhModel m)
    {
        if (m.MaTruong == 0 || m.MaChuyenNganh == 0)
            return (false, "Dữ liệu không hợp lệ.");

        TinTuyenSinhDAL.CapNhat(m);
        return (true, null);
    }

    /// <summary>Xóa tin tuyển sinh theo mã tin.</summary>
    public static void Xoa(int maTin) => TinTuyenSinhDAL.Xoa(maTin);

    /// <summary>Toggle trạng thái hiển thị (ẩn/hiện) tin tuyển sinh.</summary>
    public static void ToggleTrangThai(int maTin) => TinTuyenSinhDAL.ToggleTrangThai(maTin);
}
