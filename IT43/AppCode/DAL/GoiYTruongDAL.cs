using System;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// DAL cho tính năng Gợi ý trường phù hợp dựa trên Profile học sinh.
/// Logic: lấy tin tuyển sinh có DiemChuanNamTruoc ≤ DiemDuKien + ưu tiên cùng ngành quan tâm.
/// </summary>
public static class GoiYTruongDAL
{
    /// <summary>
    /// Gợi ý tối đa <paramref name="top"/> trường phù hợp dựa trên:
    /// - DiemDuKien: lấy trường có DiemChuanNamTruoc ≤ diem + 2 (đảm bảo vào được)
    /// - MaChuyenNganh: ưu tiên ngành quan tâm (ORDER BY ưu tiên)
    /// - KhuVuc: cộng điểm ưu tiên nếu có (KV1=0.75, KV2=0.5, KV2NT=0.25)
    /// </summary>
    public static DataTable GoiY(decimal? diemDuKien, int? maChuyenNganh, byte? khuVuc, int top = 6)
    {
        if (!diemDuKien.HasValue) return new DataTable();

        // Điểm sau khi cộng ưu tiên khu vực
        decimal diemUT = diemDuKien.Value + khuVuc switch
        {
            1 => 0.75m,
            2 => 0.50m,
            3 => 0.25m,
            _ => 0m
        };

        // Lấy năm tuyển sinh gần nhất
        var yearObj = DBHelper.Scalar("SELECT MAX(NamTuyenSinh) FROM tbl_TinTuyenSinh WHERE TrangThai=1");
        if (yearObj == null || yearObj == DBNull.Value) return new DataTable();
        short namGanNhat = DBHelper.Val<short>(yearObj);

        return DBHelper.Query($@"
            SELECT TOP (@top)
                tr.MaTruong,
                tr.TenTruong,
                tr.TinhThanh,
                tr.Slug,
                tr.AnhDaiDien,
                tr.LoaiTruong,
                CASE tr.LoaiTruong WHEN 1 THEN N'Công lập' WHEN 2 THEN N'Tư thục'
                                   WHEN 3 THEN N'Quốc tế'  ELSE N'Khác' END AS TenLoaiTruong,
                tr.KiemDinhChatLuong,
                cn.TenChuyenNganh,
                t.DiemChuanNamTruoc,
                t.NamTuyenSinh,
                t.ToHopMonHoc,
                -- Tỉ lệ phù hợp: khoảng cách giữa điểm user và điểm chuẩn (càng nhỏ càng phù hợp)
                ABS(t.DiemChuanNamTruoc - @diem) AS KhoangCach,
                -- Ưu tiên ngành quan tâm
                CASE WHEN t.MaChuyenNganh = @nganh THEN 0 ELSE 1 END AS UuTienNganh
            FROM tbl_TinTuyenSinh t
            JOIN tbl_Truong           tr ON tr.MaTruong      = t.MaTruong
            JOIN tbl_ChuyenNganh      cn ON cn.MaChuyenNganh = t.MaChuyenNganh
            WHERE t.TrangThai = 1
              AND tr.TrangThai = 1
              AND t.NamTuyenSinh = @nam
              AND t.DiemChuanNamTruoc IS NOT NULL
              AND t.DiemChuanNamTruoc <= @diemMax     -- điểm đủ để vào
              AND t.DiemChuanNamTruoc >= @diemMin     -- không quá dễ (thực tế)
            ORDER BY
                UuTienNganh ASC,       -- ngành quan tâm lên trước
                KhoangCach  ASC,       -- điểm gần nhất
                tr.KiemDinhChatLuong DESC  -- ưu tiên trường có kiểm định",
            new[]
            {
                new SqlParameter("@top",     top),
                new SqlParameter("@diem",    diemUT),
                new SqlParameter("@diemMax", diemUT + 1.5m),   // dư 1.5 điểm là OK
                new SqlParameter("@diemMin", Math.Max(0, diemUT - 3m)), // không quá thấp
                new SqlParameter("@nam",     namGanNhat),
                new SqlParameter("@nganh",   maChuyenNganh.HasValue ? (object)maChuyenNganh.Value : DBNull.Value)
            });
    }
}
