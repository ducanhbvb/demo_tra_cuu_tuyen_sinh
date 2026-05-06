using System;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// DAL cho tính năng Gợi ý trường phù hợp dựa trên Profile học sinh.
/// Logic: lấy tin tuyển sinh có DiemChuanNamTruoc trong khoảng phù hợp + ưu tiên ngành quan tâm.
/// </summary>
public static class GoiYTruongDAL
{
    // ── Helpers nội bộ ──────────────────────────────────────────────────────
    private static decimal DiemKhuVuc(byte? kv) => kv switch { 1 => 0.75m, 2 => 0.50m, 3 => 0.25m, _ => 0m };

    private static short GetNamHienTai()
    {
        // Lấy năm lớn nhất có dữ liệu từ hệ thống
        var o = DBHelper.Scalar("SELECT MAX(NamTuyenSinh) FROM tbl_TinTuyenSinh WHERE TrangThai=1");
        return (o == null || o == DBNull.Value) ? (short)0 : DBHelper.Val<short>(o);
    }

    // ── Gợi ý cơ bản — dùng trong widget sidebar MyProfile ─────────────────
    /// <summary>
    /// Gợi ý tối đa <paramref name="top"/> trường dựa trên điểm, ngành, khu vực, và tỉnh (nếu có).
    /// Cải tiến: ưu tiên trường cùng tỉnh, tăng top mặc định, weighting tốt hơn.
    /// </summary>
    public static DataTable GoiY(decimal? diemDuKien, int? maChuyenNganh, byte? khuVuc, int top = 12, string tinhThanh = null)
    {
        if (!diemDuKien.HasValue) return new DataTable();

        decimal diemUT     = diemDuKien.Value + DiemKhuVuc(khuVuc);
        short   namHienTai = GetNamHienTai();
        if (namHienTai == 0) return new DataTable();

        var prms = new System.Collections.Generic.List<SqlParameter>
        {
            new SqlParameter("@top",     top),
            new SqlParameter("@diem",    diemUT),
            new SqlParameter("@diemMax", diemUT + 4.0m),
            new SqlParameter("@diemMin", Math.Max(0, diemUT - 8.0m)),
            new SqlParameter("@nam",     namHienTai),
            new SqlParameter("@nganh",   maChuyenNganh.HasValue ? (object)maChuyenNganh.Value : DBNull.Value)
        };

        string whereExtra = "";
        if (!string.IsNullOrEmpty(tinhThanh))
        {
            whereExtra = " AND tr.TinhThanh = @tinh";
            prms.Add(new SqlParameter("@tinh", tinhThanh));
        }

        return DBHelper.Query($@"
            SELECT TOP (@top)
                tr.MaTruong, tr.TenTruong, tr.TinhThanh, tr.Slug, tr.AnhDaiDien, tr.LoaiTruong, tr.CapBacDaoTao,
                CASE tr.LoaiTruong WHEN 1 THEN N'Công lập' WHEN 2 THEN N'Tư thục'
                                   WHEN 3 THEN N'Quốc tế'  ELSE N'Khác' END AS TenLoaiTruong,
                CASE tr.CapBacDaoTao WHEN 1 THEN N'Đại học' WHEN 2 THEN N'Cao Đẳng'
                                      WHEN 3 THEN N'Trường nghề' ELSE N'' END AS TenCapBacDaoTao,
                tr.KiemDinhChatLuong,
                cn.TenChuyenNganh,
                t.DiemChuanNamTruoc, t.NamTuyenSinh, t.ToHopMonHoc,
                ABS(t.DiemChuanNamTruoc - @diem) AS KhoangCach,
                CASE WHEN t.MaChuyenNganh = @nganh THEN 0 ELSE 1 END AS UuTienNganh,
                CASE WHEN tr.TinhThanh = @tinh THEN 0 ELSE 1 END AS UuTienTinh
            FROM tbl_TinTuyenSinh t
            JOIN tbl_Truong      tr ON tr.MaTruong      = t.MaTruong
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = t.MaChuyenNganh
            WHERE t.TrangThai = 1 AND tr.TrangThai = 1
              AND t.NamTuyenSinh <= @nam
              AND t.DiemChuanNamTruoc IS NOT NULL
              AND t.DiemChuanNamTruoc <= @diemMax
              AND t.DiemChuanNamTruoc >= @diemMin
              {whereExtra}
            ORDER BY UuTienNganh ASC, UuTienTinh ASC, KhoangCach ASC, tr.KiemDinhChatLuong DESC",
            prms.ToArray());
    }

    // ── Gợi ý nâng cao — dùng trong trang GoiYTruong.aspx riêng ─────────────
    /// <summary>
    /// Gợi ý trường với đầy đủ tiêu chí tuỳ chọn:<br/>
    /// điểm, ngành, khu vực, loại trường, tỉnh/thành, kiểm định chất lượng.
    /// </summary>
    public static DataTable GoiYNangCao(
        decimal? diemDuKien,
        int?     maChuyenNganh,
        byte?    khuVuc,
        byte?    loaiTruong,   // null=tất cả | 1=Công lập | 2=Tư thục | 3=Quốc tế
        string   tinhThanh,    // null=tất cả
        bool?    kiemDinh,     // null=tất cả | true=chỉ trường đã kiểm định
        byte?    capBacDaoTao, // null=tất cả | 1=Đại học | 2=Cao Đẳng | 3=Trường nghề
        string   toHopXetTuyen,// null=tất cả | VD: "A00", "A01"...
        int      top = 12)
    {
        if (!diemDuKien.HasValue) return new DataTable();

        decimal diemUT     = diemDuKien.Value + DiemKhuVuc(khuVuc);
        short   namHienTai = GetNamHienTai();
        if (namHienTai == 0) return new DataTable();

        var prms = new System.Collections.Generic.List<SqlParameter>
        {
            new SqlParameter("@top",     top),
            new SqlParameter("@diem",    diemUT),
            new SqlParameter("@diemMax", diemUT + 2.0m),   // Nới 1-2 điểm trên
            new SqlParameter("@nam",     namHienTai),
            new SqlParameter("@nganh",   maChuyenNganh.HasValue ? (object)maChuyenNganh.Value : DBNull.Value)
        };

        string w = "";
        if (loaiTruong.HasValue)              { w += " AND tr.LoaiTruong=@loai"; prms.Add(new SqlParameter("@loai", loaiTruong.Value)); }
        if (!string.IsNullOrEmpty(tinhThanh)) { w += " AND tr.TinhThanh=@tinh";  prms.Add(new SqlParameter("@tinh", tinhThanh)); }
        if (kiemDinh == true)                   w += " AND tr.KiemDinhChatLuong=1";
        if (capBacDaoTao.HasValue)            { w += " AND tr.CapBacDaoTao=@capBac"; prms.Add(new SqlParameter("@capBac", capBacDaoTao.Value)); }
        if (!string.IsNullOrEmpty(toHopXetTuyen)) { w += " AND t.ToHopMonHoc LIKE @toHop"; prms.Add(new SqlParameter("@toHop", "%" + toHopXetTuyen.Trim().ToUpper() + "%")); }

        return DBHelper.Query($@"
            SELECT TOP (@top)
                tr.MaTruong, tr.TenTruong, tr.TinhThanh, tr.Slug, tr.AnhDaiDien, tr.LoaiTruong, tr.CapBacDaoTao,
                CASE tr.LoaiTruong WHEN 1 THEN N'Công lập' WHEN 2 THEN N'Tư thục'
                                   WHEN 3 THEN N'Quốc tế'  ELSE N'Khác' END AS TenLoaiTruong,
                CASE tr.CapBacDaoTao WHEN 1 THEN N'Đại học' WHEN 2 THEN N'Cao Đẳng'
                                      WHEN 3 THEN N'Trường nghề' ELSE N'' END AS TenCapBacDaoTao,
                tr.KiemDinhChatLuong,
                cn.TenChuyenNganh,
                t.DiemChuanNamTruoc, t.NamTuyenSinh, t.ToHopMonHoc, t.HocPhi,
                ABS(t.DiemChuanNamTruoc - @diem) AS KhoangCach,
                CASE WHEN t.MaChuyenNganh = @nganh THEN 0 ELSE 1 END AS UuTienNganh,
                ISNULL(CAST(ROUND(@diem / NULLIF(t.DiemChuanNamTruoc,0) * 100, 0) AS INT), 0) AS TiLePhuHop
            FROM tbl_TinTuyenSinh t
            JOIN tbl_Truong      tr ON tr.MaTruong      = t.MaTruong
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = t.MaChuyenNganh
            WHERE t.TrangThai = 1 AND tr.TrangThai = 1
              AND t.NamTuyenSinh <= @nam
              AND t.DiemChuanNamTruoc IS NOT NULL
              AND t.DiemChuanNamTruoc <= @diemMax
              {w}
            ORDER BY UuTienNganh ASC, KhoangCach ASC, tr.KiemDinhChatLuong DESC",
            prms.ToArray());
    }
}
