using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// DAL cho tính năng So sánh ngành — so sánh tối đa 4 tin tuyển sinh (MaTin) song song.
/// Trả DataTable với 1 row/tin, gồm đầy đủ thông tin ngành, trường, điểm chuẩn, học phí…
/// </summary>
public static class SoSanhNganhDAL
{
    /// <summary>
    /// Lấy thông tin so sánh cho danh sách MaTin.
    /// Trả DataTable gồm: MaTin, MaTruong, TenTruong, TinhThanh, Slug, AnhDaiDien,
    ///   MaChuyenNganh, TenChuyenNganh, TenDanhMuc, TenCapBac, TenPhuongThuc,
    ///   NamTuyenSinh, DiemChuanNamTruoc, HocPhi, ChiTieu, ToHopMonHoc
    /// </summary>
    public static DataTable GetDanhSachSoSanhNganh(List<int> danhSachMaTin)
    {
        if (danhSachMaTin == null || danhSachMaTin.Count == 0)
            return new DataTable();

        // Tạo IN clause an toàn — giá trị đều là int
        string inClause = string.Join(",", danhSachMaTin);

        var dt = DBHelper.Query($@"
            SELECT
                t.MaTin,
                tr.MaTruong,
                tr.TenTruong,
                tr.TinhThanh,
                tr.Slug,
                tr.AnhDaiDien,
                t.MaChuyenNganh,
                cn.TenChuyenNganh,
                dmn.TenDanhMuc       AS TenDanhMuc,
                cb.TenCapBac,
                pt.TenPhuongThuc,
                t.NamTuyenSinh,
                t.DiemChuanNamTruoc,
                t.DiemChuanNamNay,
                t.HocPhi,
                t.ChiTieu,
                t.ToHopMonHoc
            FROM tbl_TinTuyenSinh t
            JOIN tbl_Truong              tr  ON tr.MaTruong      = t.MaTruong
            JOIN tbl_ChuyenNganh         cn  ON cn.MaChuyenNganh = t.MaChuyenNganh
            JOIN tbl_DanhMucNganh        dmn ON dmn.MaDanhMuc    = cn.MaDanhMuc
            LEFT JOIN tbl_CapBac         cb  ON cb.MaCapBac      = (
                SELECT TOP 1 MaCapBac
                FROM tbl_TruongChuyenNganh
                WHERE MaTruong = t.MaTruong AND MaChuyenNganh = t.MaChuyenNganh)
            JOIN tbl_PhuongThucXetTuyen  pt  ON pt.MaPhuongThuc  = t.MaPhuongThuc
            WHERE t.MaTin IN ({inClause})
              AND t.TrangThai = 1
              AND tr.TrangThai = 1
        ");

        // Sắp xếp lại theo thứ tự danhSachMaTin (giữ đúng thứ tự người dùng thêm vào)
        var result = new DataTable();
        foreach (DataColumn col in dt.Columns)
            result.Columns.Add(col.ColumnName, col.DataType);

        foreach (int maTin in danhSachMaTin)
        {
            foreach (DataRow r in dt.Rows)
            {
                if (DBHelper.Val<int>(r["MaTin"]) == maTin)
                {
                    result.ImportRow(r);
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Lấy danh sách tin tuyển sinh của một trường (để dùng trong dropdown cascading).
    /// Trả DataTable: MaTin, TenChuyenNganh, NamTuyenSinh, DiemChuanNamTruoc, TenPhuongThuc
    /// Sắp xếp: năm giảm dần → tên ngành tăng dần.
    /// </summary>
    public static DataTable GetNganhCuaTruong(int maTruong, int? nam = null)
    {
        return DBHelper.Query("sp_SoSanhNganh_LayTinTheoTruong", new[]
        {
            new SqlParameter("@MaTruong", maTruong),
            new SqlParameter("@NamTuyenSinh", nam.HasValue ? (object)nam.Value : DBNull.Value)
        }, isSP: true);
    }
}
