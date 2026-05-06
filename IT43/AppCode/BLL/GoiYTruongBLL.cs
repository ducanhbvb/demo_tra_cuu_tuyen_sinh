using System.Data;

/// <summary>Business Logic Layer cho tính năng Gợi ý trường phù hợp.</summary>
public static class GoiYTruongBLL
{
    // ── Widget đơn giản trong MyProfile ─────────────────────────────────────
    public static DataTable GoiY(decimal? diemDuKien, int? maChuyenNganh, byte? khuVuc, int top = 6)
        => GoiYTruongDAL.GoiY(diemDuKien, maChuyenNganh, khuVuc, top);

    // ── Trang GoiYTruong.aspx — đầy đủ tiêu chí ─────────────────────────────
    /// <summary>
    /// Gợi ý nâng cao: điểm dự kiến, ngành, khu vực, loại trường, tỉnh/thành, kiểm định.
    /// </summary>
    public static DataTable GoiYNangCao(
        decimal? diemDuKien,
        int?     maChuyenNganh,
        byte?    khuVuc,
        byte?    loaiTruong,
        string   tinhThanh,
        bool?    kiemDinh,
        byte?    capBacDaoTao,
        string   toHopXetTuyen,
        int      top = 12)
        => GoiYTruongDAL.GoiYNangCao(diemDuKien, maChuyenNganh, khuVuc, loaiTruong, tinhThanh, kiemDinh, capBacDaoTao, toHopXetTuyen, top);
}
