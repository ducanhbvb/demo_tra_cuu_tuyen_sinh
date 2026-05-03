using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Điểm chuẩn lịch sử (tbl_DiemChuanLichSu).
/// Lấy điểm chuẩn qua các năm theo trường/ngành, sync dữ liệu từ tin tuyển sinh.
/// </summary>
public static class DiemChuanLichSuDAL
{
    public static List<DiemChuanLichSuModel> GetTheoTruongNganh(int maTruong, int maChuyenNganh, int? maPhuongThuc = null)
    {
        string sql = @"
            SELECT ls.*, cn.TenChuyenNganh, pt.TenPhuongThuc
            FROM tbl_DiemChuanLichSu ls
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = ls.MaChuyenNganh
            JOIN tbl_PhuongThucXetTuyen pt ON pt.MaPhuongThuc = ls.MaPhuongThuc
            WHERE ls.MaTruong=@tr AND ls.MaChuyenNganh=@cn";
        var prms = new System.Collections.Generic.List<SqlParameter>
        {
            new SqlParameter("@tr", maTruong),
            new SqlParameter("@cn", maChuyenNganh)
        };
        if (maPhuongThuc.HasValue)
        {
            sql += " AND ls.MaPhuongThuc=@pt";
            prms.Add(new SqlParameter("@pt", maPhuongThuc.Value));
        }
        sql += " ORDER BY ls.NamTuyenSinh ASC";
        var dt = DBHelper.Query(sql, prms.ToArray());
        var list = new List<DiemChuanLichSuModel>();
        foreach (DataRow r in dt.Rows)
            list.Add(new DiemChuanLichSuModel
            {
                ID             = DBHelper.Val<int>(r["ID"]),
                MaTruong       = DBHelper.Val<int>(r["MaTruong"]),
                MaChuyenNganh  = DBHelper.Val<int>(r["MaChuyenNganh"]),
                TenChuyenNganh = r["TenChuyenNganh"].ToString(),
                MaPhuongThuc   = DBHelper.Val<int>(r["MaPhuongThuc"]),
                TenPhuongThuc  = r["TenPhuongThuc"].ToString(),
                NamTuyenSinh   = DBHelper.Val<short>(r["NamTuyenSinh"]),
                DiemChuan      = DBHelper.ValN<decimal>(r["DiemChuan"]),
                ChiTieu        = DBHelper.ValN<int>(r["ChiTieu"]),
                GhiChu         = r["GhiChu"] == DBNull.Value ? null : r["GhiChu"].ToString()
            });
        return list;
    }

    // Lưu hàng loạt từ tbl_TinTuyenSinh -> tbl_DiemChuanLichSu (admin dùng)
    public static void SyncFromTinTuyenSinh(int maTruong)
    {
        DBHelper.Execute(@"
            MERGE tbl_DiemChuanLichSu AS target
            USING (
                SELECT MaTruong, MaChuyenNganh, MaPhuongThuc, NamTuyenSinh,
                       DiemChuanNamTruoc AS DiemChuan, ChiTieu
                FROM tbl_TinTuyenSinh
                WHERE MaTruong=@tr AND TrangThai=1
            ) AS src
            ON target.MaTruong=src.MaTruong AND target.MaChuyenNganh=src.MaChuyenNganh
               AND target.MaPhuongThuc=src.MaPhuongThuc AND target.NamTuyenSinh=src.NamTuyenSinh
            WHEN MATCHED THEN UPDATE SET target.DiemChuan=src.DiemChuan, target.ChiTieu=src.ChiTieu
            WHEN NOT MATCHED THEN INSERT(MaTruong,MaChuyenNganh,MaPhuongThuc,NamTuyenSinh,DiemChuan,ChiTieu)
                VALUES(src.MaTruong,src.MaChuyenNganh,src.MaPhuongThuc,src.NamTuyenSinh,src.DiemChuan,src.ChiTieu);",
            new[] { new SqlParameter("@tr", maTruong) });
    }
}
