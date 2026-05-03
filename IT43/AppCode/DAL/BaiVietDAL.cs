using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Bài viết (tbl_BaiViet).
/// CRUD bài viết, phân trang, toggle trạng thái, tăng lượt xem.
/// </summary>
public static class BaiVietDAL
{
    /// <param name="chiActive">true = chỉ lấy active; false = lấy tất cả (admin)</param>
    /// <param name="trangThai">null = không lọc; true/false = lọc theo TrangThai (Bug #6 fix)</param>
    public static PagedTable GetDanhSach(int? maTruong, int pageIndex, int pageSize,
                                         bool chiActive = true, bool? trangThai = null)
    {
        // Xây WHERE clause đúng cách — lọc ở SQL thay vì phía C# (fix Bug #6)
        string where = "WHERE 1=1";
        var countPrms = new System.Collections.Generic.List<SqlParameter>();
        var dataPrms  = new System.Collections.Generic.List<SqlParameter>();

        if (chiActive)
        {
            where += " AND bv.TrangThai=1";
        }
        else if (trangThai.HasValue)
        {
            // Admin filter: lọc theo trạng thái cụ thể
            where += " AND bv.TrangThai=@tt";
            countPrms.Add(new SqlParameter("@tt", trangThai.Value));
            dataPrms.Add(new SqlParameter("@tt",  trangThai.Value));
        }

        if (maTruong.HasValue)
        {
            where += " AND bv.MaTruong=@mt";
            countPrms.Add(new SqlParameter("@mt", maTruong.Value));
            dataPrms.Add(new SqlParameter("@mt", maTruong.Value));
        }

        var count = DBHelper.Scalar($"SELECT COUNT(1) FROM tbl_BaiViet bv {where}", countPrms.ToArray());

        dataPrms.Add(new SqlParameter("@skip", pageIndex * pageSize));
        dataPrms.Add(new SqlParameter("@take", pageSize));

        var dt = DBHelper.Query($@"
            SELECT bv.MaBaiViet, bv.TieuDe, bv.Slug, bv.AnhChinh, bv.NgayDang, bv.LuotXem,
                   bv.TrangThai, bv.MaTruong, tr.TenTruong
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong = bv.MaTruong
            {where} ORDER BY bv.NgayDang DESC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", dataPrms.ToArray());

        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(count), PageIndex = pageIndex, PageSize = pageSize };
    }

    public static BaiVietModel GetChiTiet(string slug)
    {
        var dt = DBHelper.Query(@"
            SELECT bv.*, tr.TenTruong FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong=bv.MaTruong
            WHERE bv.Slug=@slug AND bv.TrangThai=1",
            new[] { new SqlParameter("@slug", slug) });
        if (dt.Rows.Count == 0) return null;
        // tăng lượt xem
        DBHelper.Execute("UPDATE tbl_BaiViet SET LuotXem=LuotXem+1 WHERE Slug=@slug",
            new[] { new SqlParameter("@slug", slug) });
        return MapRow(dt.Rows[0]);
    }

    public static int Them(BaiVietModel m)
    {
        var id = DBHelper.Scalar(@"
            INSERT INTO tbl_BaiViet(TieuDe,Slug,AnhChinh,NoiDung,MaTacGia,TrangThai,MaTruong)
            VALUES(@td,@slug,@anh,@nd,@tg,@tt,@tr)
            SELECT SCOPE_IDENTITY()",
            BuildParams(m));
        return DBHelper.Val<int>(id);
    }

    public static void CapNhat(BaiVietModel m)
    {
        var list = new System.Collections.Generic.List<SqlParameter>(BuildParams(m))
            { new SqlParameter("@id", m.MaBaiViet) };
        DBHelper.Execute(@"UPDATE tbl_BaiViet SET TieuDe=@td,Slug=@slug,AnhChinh=@anh,
            NoiDung=@nd,TrangThai=@tt,MaTruong=@tr WHERE MaBaiViet=@id", list.ToArray());
    }

    public static void Xoa(int maBaiViet)
    {
        DBHelper.Execute("DELETE FROM tbl_BaiViet WHERE MaBaiViet=@id",
            new[] { new SqlParameter("@id", maBaiViet) });
    }

    public static void ToggleTrangThai(int maBaiViet)
    {
        DBHelper.Execute(
            "UPDATE tbl_BaiViet SET TrangThai = CASE WHEN TrangThai=1 THEN 0 ELSE 1 END WHERE MaBaiViet=@id",
            new[] { new SqlParameter("@id", maBaiViet) });
    }

    private static SqlParameter[] BuildParams(BaiVietModel m) => new[]
    {
        new SqlParameter("@td",   m.TieuDe),
        new SqlParameter("@slug", (object)m.Slug ?? DBNull.Value),
        new SqlParameter("@anh",  (object)m.AnhChinh ?? DBNull.Value),
        new SqlParameter("@nd",   (object)m.NoiDung ?? DBNull.Value),
        new SqlParameter("@tg",   m.MaTacGia),
        new SqlParameter("@tt",   m.TrangThai),
        new SqlParameter("@tr",   m.MaTruong.HasValue ? (object)m.MaTruong.Value : DBNull.Value)
    };

    private static BaiVietModel MapRow(DataRow r) => new BaiVietModel
    {
        MaBaiViet = DBHelper.Val<int>(r["MaBaiViet"]),
        TieuDe    = r["TieuDe"].ToString(),
        Slug      = r["Slug"] == DBNull.Value ? null : r["Slug"].ToString(),
        AnhChinh  = r["AnhChinh"] == DBNull.Value ? null : r["AnhChinh"].ToString(),
        NoiDung   = r["NoiDung"] == DBNull.Value ? null : r["NoiDung"].ToString(),
        MaTacGia  = DBHelper.Val<int>(r["MaTacGia"]),
        NgayDang  = DBHelper.Val<DateTime>(r["NgayDang"]),
        LuotXem   = DBHelper.Val<int>(r["LuotXem"]),
        TrangThai = DBHelper.Val<bool>(r["TrangThai"]),
        MaTruong  = DBHelper.ValN<int>(r["MaTruong"]),
        TenTruong = r.Table.Columns.Contains("TenTruong") && r["TenTruong"] != DBNull.Value
                    ? r["TenTruong"].ToString() : null
    };
}
