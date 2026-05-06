using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

/// <summary>
/// DAL cho tính năng So sánh trường (tối đa 3 trường).
/// Trả DataTable với 1 row/trường, bao gồm điểm chuẩn gần nhất + số ngành.
/// </summary>
public static class SoSanhDAL
{
    /// <summary>
    /// Lấy thông tin so sánh cho danh sách MaTruong.
    /// Trả DataTable gồm: MaTruong, TenTruong, TinhThanh, LoaiTruong, TenLoaiTruong,
    ///   MaVung, TenVung, KiemDinhChatLuong, Website, QuyMo, Slug, AnhDaiDien,
    ///   DiemDanhGiaTB, SoLuongDanhGia, DiemChuanMin, DiemChuanMax, NamGanNhat, SoNganh
    /// </summary>
    public static DataTable GetDanhSachSoSanh(List<int> danhSachMaTruong)
    {
        if (danhSachMaTruong == null || danhSachMaTruong.Count == 0)
            return new DataTable();

        // Tạo IN clause an toàn — giá trị đều là int
        string inClause = string.Join(",", danhSachMaTruong);

        var dt = DBHelper.Query($@"
            SELECT
                tr.MaTruong,
                tr.TenTruong,
                tr.TinhThanh,
                tr.MaVung,
                tr.LoaiTruong,
                CASE tr.LoaiTruong WHEN 1 THEN N'Công lập' WHEN 2 THEN N'Tư thục'
                                   WHEN 3 THEN N'Quốc tế'  ELSE N'Khác' END AS TenLoaiTruong,
                CASE tr.MaVung WHEN 1 THEN N'Miền Bắc' WHEN 2 THEN N'Miền Trung'
                               WHEN 3 THEN N'Miền Nam'  ELSE N'' END AS TenVung,
                tr.KiemDinhChatLuong,
                tr.Website,
                tr.QuyMo,
                tr.Slug,
                tr.AnhDaiDien,
                -- Điểm đánh giá trung bình
                (SELECT AVG(CAST(DiemDanhGia AS FLOAT))
                 FROM tbl_DanhGiaTruong WHERE MaTruong=tr.MaTruong AND TrangThai=1) AS DiemDanhGiaTB,
                (SELECT COUNT(*)
                 FROM tbl_DanhGiaTruong WHERE MaTruong=tr.MaTruong AND TrangThai=1) AS SoLuongDanhGia,
                -- Điểm chuẩn min/max của năm gần nhất
                (SELECT MIN(t.DiemChuanNamTruoc)
                 FROM tbl_TinTuyenSinh t
                 WHERE t.MaTruong=tr.MaTruong AND t.TrangThai=1
                   AND t.NamTuyenSinh=(
                     SELECT MAX(NamTuyenSinh) FROM tbl_TinTuyenSinh
                     WHERE MaTruong=tr.MaTruong AND TrangThai=1)) AS DiemChuanMin,
                (SELECT MAX(t.DiemChuanNamTruoc)
                 FROM tbl_TinTuyenSinh t
                 WHERE t.MaTruong=tr.MaTruong AND t.TrangThai=1
                   AND t.NamTuyenSinh=(
                     SELECT MAX(NamTuyenSinh) FROM tbl_TinTuyenSinh
                     WHERE MaTruong=tr.MaTruong AND TrangThai=1)) AS DiemChuanMax,
                (SELECT MAX(NamTuyenSinh) FROM tbl_TinTuyenSinh
                 WHERE MaTruong=tr.MaTruong AND TrangThai=1) AS NamGanNhat,
                -- Số ngành đào tạo
                (SELECT COUNT(DISTINCT MaChuyenNganh)
                 FROM tbl_TruongChuyenNganh WHERE MaTruong=tr.MaTruong) AS SoNganh
            FROM tbl_Truong tr
            WHERE tr.MaTruong IN ({inClause})
              AND tr.TrangThai = 1
            ORDER BY CHARINDEX(CAST(tr.MaTruong AS VARCHAR), @thuTu)
        ", new[] { new SqlParameter("@thuTu", string.Join(",", danhSachMaTruong)) });

        // Sắp xếp lại theo thứ tự danhSachMaTruong (SQL CHARINDEX có thể không chính xác)
        var result = new DataTable();
        foreach (DataColumn col in dt.Columns)
            result.Columns.Add(col.ColumnName, col.DataType);

        foreach (int maTruong in danhSachMaTruong)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (DBHelper.Val<int>(r["MaTruong"]) == maTruong)
                {
                    result.ImportRow(r);
                    break;
                }
            }
        }

        return result;
    }
}
