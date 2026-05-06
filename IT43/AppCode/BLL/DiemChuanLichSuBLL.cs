using System.Collections.Generic;

/// <summary>Business Logic Layer cho điểm chuẩn lịch sử.</summary>
public static class DiemChuanLichSuBLL
{
    /// <summary>Lấy điểm chuẩn theo trường + ngành (dùng cho biểu đồ Chart.js).</summary>
    public static List<DiemChuanLichSuModel> GetTheoTruongNganh(int maTruong, int maChuyenNganh, int? maPhuongThuc = null)
        => DiemChuanLichSuDAL.GetTheoTruongNganh(maTruong, maChuyenNganh, maPhuongThuc);

    /// <summary>Lấy danh sách lịch sử điểm chuẩn của 1 trường, có filter.</summary>
    public static List<DiemChuanLichSuModel> GetDanhSachTheoTruong(
        int maTruong, int? maChuyenNganh = null, short? nam = null, int? maPhuongThuc = null)
        => DiemChuanLichSuDAL.GetDanhSachTheoTruong(maTruong, maChuyenNganh, nam, maPhuongThuc);

    /// <summary>Lấy 1 bản ghi theo ID.</summary>
    public static DiemChuanLichSuModel GetById(int id)
        => DiemChuanLichSuDAL.GetById(id);

    /// <summary>Thêm mới 1 bản ghi điểm chuẩn lịch sử.</summary>
    public static (bool ok, string error) Them(DiemChuanLichSuModel m)
        => DiemChuanLichSuDAL.Them(m);

    /// <summary>Cập nhật 1 bản ghi điểm chuẩn lịch sử.</summary>
    public static (bool ok, string error) CapNhat(DiemChuanLichSuModel m)
        => DiemChuanLichSuDAL.CapNhat(m);

    /// <summary>Xóa 1 bản ghi điểm chuẩn lịch sử.</summary>
    public static void Xoa(int id)
        => DiemChuanLichSuDAL.Xoa(id);

    /// <summary>Đồng bộ toàn bộ điểm chuẩn từ tbl_TinTuyenSinh → tbl_DiemChuanLichSu.</summary>
    public static void SyncFromTinTuyenSinh(int maTruong)
        => DiemChuanLichSuDAL.SyncFromTinTuyenSinh(maTruong);
}
