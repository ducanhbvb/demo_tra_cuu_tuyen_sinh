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

    /// <summary>Lấy danh sách lịch sử điểm chuẩn theo trường (có filter ngành/năm/phương thức).</summary>
    public static List<DiemChuanLichSuModel> GetDanhSachTheoTruong(
        int maTruong, int? maChuyenNganh = null, short? nam = null, int? maPhuongThuc = null)
    {
        string sql = @"
            SELECT ls.*, cn.TenChuyenNganh, pt.TenPhuongThuc
            FROM tbl_DiemChuanLichSu ls
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = ls.MaChuyenNganh
            JOIN tbl_PhuongThucXetTuyen pt ON pt.MaPhuongThuc = ls.MaPhuongThuc
            WHERE ls.MaTruong = @tr";
        var prms = new List<SqlParameter> { new SqlParameter("@tr", maTruong) };
        if (maChuyenNganh.HasValue) { sql += " AND ls.MaChuyenNganh=@cn"; prms.Add(new SqlParameter("@cn", maChuyenNganh.Value)); }
        if (nam.HasValue)           { sql += " AND ls.NamTuyenSinh=@nam"; prms.Add(new SqlParameter("@nam", nam.Value)); }
        if (maPhuongThuc.HasValue)  { sql += " AND ls.MaPhuongThuc=@pt";  prms.Add(new SqlParameter("@pt", maPhuongThuc.Value)); }
        sql += " ORDER BY ls.NamTuyenSinh DESC, cn.TenChuyenNganh ASC";
        var dt = DBHelper.Query(sql, prms.ToArray());
        var list = new List<DiemChuanLichSuModel>();
        foreach (DataRow r in dt.Rows)
            list.Add(MapRow(r));
        return list;
    }

    /// <summary>Lấy 1 bản ghi theo ID.</summary>
    public static DiemChuanLichSuModel GetById(int id)
    {
        var dt = DBHelper.Query(@"
            SELECT ls.*, cn.TenChuyenNganh, pt.TenPhuongThuc
            FROM tbl_DiemChuanLichSu ls
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = ls.MaChuyenNganh
            JOIN tbl_PhuongThucXetTuyen pt ON pt.MaPhuongThuc = ls.MaPhuongThuc
            WHERE ls.ID = @id",
            new[] { new SqlParameter("@id", id) });
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    /// <summary>Thêm mới 1 bản ghi điểm chuẩn lịch sử.</summary>
    public static (bool ok, string error) Them(DiemChuanLichSuModel m)
    {
        try
        {
            DBHelper.Execute(@"
                IF NOT EXISTS (SELECT 1 FROM tbl_DiemChuanLichSu
                               WHERE MaTruong=@tr AND MaChuyenNganh=@cn
                                 AND MaPhuongThuc=@pt AND NamTuyenSinh=@nam)
                INSERT INTO tbl_DiemChuanLichSu
                    (MaTruong, MaChuyenNganh, MaPhuongThuc, NamTuyenSinh, DiemChuan, ChiTieu, GhiChu)
                VALUES (@tr, @cn, @pt, @nam, @diem, @ct, @gc)",
                new SqlParameter[]
                {
                    new SqlParameter("@tr",  m.MaTruong),
                    new SqlParameter("@cn",  m.MaChuyenNganh),
                    new SqlParameter("@pt",  m.MaPhuongThuc),
                    new SqlParameter("@nam", m.NamTuyenSinh),
                    new SqlParameter("@diem", m.DiemChuan.HasValue ? (object)m.DiemChuan.Value : DBNull.Value),
                    new SqlParameter("@ct",   m.ChiTieu.HasValue   ? (object)m.ChiTieu.Value   : DBNull.Value),
                    new SqlParameter("@gc",   string.IsNullOrEmpty(m.GhiChu) ? (object)DBNull.Value : m.GhiChu)
                });
            return (true, null);
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    /// <summary>Cập nhật 1 bản ghi điểm chuẩn lịch sử.</summary>
    public static (bool ok, string error) CapNhat(DiemChuanLichSuModel m)
    {
        try
        {
            DBHelper.Execute(@"
                UPDATE tbl_DiemChuanLichSu
                SET MaChuyenNganh=@cn, MaPhuongThuc=@pt, NamTuyenSinh=@nam,
                    DiemChuan=@diem, ChiTieu=@ct, GhiChu=@gc
                WHERE ID=@id",
                new SqlParameter[]
                {
                    new SqlParameter("@id",  m.ID),
                    new SqlParameter("@cn",  m.MaChuyenNganh),
                    new SqlParameter("@pt",  m.MaPhuongThuc),
                    new SqlParameter("@nam", m.NamTuyenSinh),
                    new SqlParameter("@diem", m.DiemChuan.HasValue ? (object)m.DiemChuan.Value : DBNull.Value),
                    new SqlParameter("@ct",   m.ChiTieu.HasValue   ? (object)m.ChiTieu.Value   : DBNull.Value),
                    new SqlParameter("@gc",   string.IsNullOrEmpty(m.GhiChu) ? (object)DBNull.Value : m.GhiChu)
                });
            return (true, null);
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    /// <summary>Xóa 1 bản ghi điểm chuẩn lịch sử theo ID.</summary>
    public static void Xoa(int id)
    {
        DBHelper.Execute("DELETE FROM tbl_DiemChuanLichSu WHERE ID=@id",
            new[] { new SqlParameter("@id", id) });
    }

    // ── private helper ──────────────────────────────────────────────────────
    private static DiemChuanLichSuModel MapRow(DataRow r) => new DiemChuanLichSuModel
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
    };

    // Lưu hàng loạt từ tbl_TinTuyenSinh -> tbl_DiemChuanLichSu (admin dùng)
    public static void SyncFromTinTuyenSinh(int maTruong)
    {
        // Dùng GROUP BY để dedup source — tránh lỗi "MERGE UPDATE same row more than once"
        // khi có nhiều bản ghi TinTuyenSinh cùng (Truong+Nganh+PhuongThuc+Nam)
        DBHelper.Execute(@"
            MERGE tbl_DiemChuanLichSu AS target
            USING (
                SELECT MaTruong, MaChuyenNganh, MaPhuongThuc, NamTuyenSinh,
                       MAX(DiemChuanNamTruoc) AS DiemChuan,
                       MAX(ChiTieu)           AS ChiTieu
                FROM tbl_TinTuyenSinh
                WHERE MaTruong=@tr AND TrangThai=1
                  AND DiemChuanNamTruoc IS NOT NULL
                GROUP BY MaTruong, MaChuyenNganh, MaPhuongThuc, NamTuyenSinh
            ) AS src
            ON  target.MaTruong      = src.MaTruong
            AND target.MaChuyenNganh = src.MaChuyenNganh
            AND target.MaPhuongThuc  = src.MaPhuongThuc
            AND target.NamTuyenSinh  = src.NamTuyenSinh
            WHEN MATCHED THEN
                UPDATE SET target.DiemChuan = src.DiemChuan,
                           target.ChiTieu   = src.ChiTieu
            WHEN NOT MATCHED BY TARGET THEN
                INSERT (MaTruong, MaChuyenNganh, MaPhuongThuc, NamTuyenSinh, DiemChuan, ChiTieu)
                VALUES (src.MaTruong, src.MaChuyenNganh, src.MaPhuongThuc, src.NamTuyenSinh, src.DiemChuan, src.ChiTieu);",
            new[] { new SqlParameter("@tr", maTruong) });
    }
}
