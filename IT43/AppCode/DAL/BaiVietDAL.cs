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
    /// <param name="trangThai">null = không lọc; true/false = lọc theo TrangThai</param>
    public static PagedTable GetDanhSach(int? maTruong, int pageIndex, int pageSize,
                                         bool chiActive = true, bool? trangThai = null)
    {
        // Xây WHERE clause — lọc điều kiện trực tiếp ở SQL
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
            SELECT bv.MaBaiViet, bv.TieuDe, bv.Slug, bv.AnhChinh, bv.NoiDung, bv.TenNguoiDang, bv.NgayDang, bv.LuotXem,
                   bv.TrangThai, bv.TheLoai, bv.MaTruong, tr.TenTruong,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong = bv.MaTruong
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
            {where} ORDER BY bv.NgayDang DESC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", dataPrms.ToArray());

        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(count), PageIndex = pageIndex, PageSize = pageSize };
    }

    /// <summary>Lấy bài viết theo MaBaiViet — dùng để kiểm tra IDOR trước khi sửa/xóa.</summary>
    public static BaiVietModel GetById(int maBaiViet)
    {
        var dt = DBHelper.Query(@"
            SELECT bv.*, tr.TenTruong,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong = bv.MaTruong
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
            WHERE bv.MaBaiViet = @id",
            new[] { new SqlParameter("@id", maBaiViet) });
        if (dt == null || dt.Rows.Count == 0) return null;
        return MapRow(dt.Rows[0]);
    }

    public static BaiVietModel GetChiTiet(string slug)
    {
        var dt = DBHelper.Query(@"
            SELECT bv.*, tr.TenTruong,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong=bv.MaTruong
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
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
            INSERT INTO tbl_BaiViet(TieuDe,Slug,AnhChinh,NoiDung,MaTacGia,TrangThai,MaTruong,TheLoai,TenNguoiDang)
            VALUES(@td,@slug,@anh,@nd,@tg,@tt,@tr,@tl,@tnd)
            SELECT SCOPE_IDENTITY()",
            BuildParams(m));
        return DBHelper.Val<int>(id);
    }

    public static void CapNhat(BaiVietModel m)
    {
        var list = new System.Collections.Generic.List<SqlParameter>(BuildParams(m))
            { new SqlParameter("@id", m.MaBaiViet) };
        DBHelper.Execute(@"UPDATE tbl_BaiViet SET TieuDe=@td,Slug=@slug,AnhChinh=@anh,
            NoiDung=@nd,TrangThai=@tt,MaTruong=@tr,TheLoai=@tl,TenNguoiDang=@tnd WHERE MaBaiViet=@id", list.ToArray());
    }

    public static void Xoa(int maBaiViet)
    {
        // Hard delete đúng 1 record bài viết theo MaBaiViet, KHÔNG xóa toàn bộ bảng tbl_BaiViet.
        DBHelper.Execute("DELETE FROM tbl_BaiViet WHERE MaBaiViet=@id",
            new[] { new SqlParameter("@id", maBaiViet) });
    }

    public static void ToggleTrangThai(int maBaiViet)
    {
        DBHelper.Execute(
            "UPDATE tbl_BaiViet SET TrangThai = CASE WHEN TrangThai=1 THEN 0 ELSE 1 END WHERE MaBaiViet=@id",
            new[] { new SqlParameter("@id", maBaiViet) });
    }

    /// <summary>Tìm kiếm bài viết theo từ khóa và tên trường, có phân trang.</summary>
    public static PagedTable SearchBaiViet(string tuKhoa, int? maTruong, int pageIndex, int pageSize)
    {
        string where = "WHERE bv.TrangThai=1";
        var countPrms = new List<SqlParameter>();
        var dataPrms  = new List<SqlParameter>();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            where += " AND (bv.TieuDe LIKE @kw OR bv.NoiDung LIKE @kw)";
            string kwVal = "%" + tuKhoa.Trim() + "%";
            countPrms.Add(new SqlParameter("@kw", kwVal));
            dataPrms.Add(new SqlParameter("@kw", kwVal));
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
            SELECT bv.MaBaiViet, bv.TieuDe, bv.Slug, bv.AnhChinh, bv.NoiDung, bv.TenNguoiDang,
                   bv.NgayDang, bv.LuotXem, bv.TrangThai, bv.TheLoai, bv.MaTruong, tr.TenTruong,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong = bv.MaTruong
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
            {where} ORDER BY bv.NgayDang DESC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", dataPrms.ToArray());

        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(count), PageIndex = pageIndex, PageSize = pageSize };
    }

    /// <summary>Lấy 1 bài viết nổi bật (mới nhất có lượt xem cao nhất trong 30 ngày).</summary>
    public static DataTable GetBaiVietNoiBat()
    {
        return DBHelper.Query(@"
            SELECT TOP 1 bv.MaBaiViet, bv.TieuDe, bv.Slug, bv.AnhChinh, bv.NoiDung, bv.TenNguoiDang,
                   bv.NgayDang, bv.LuotXem, bv.TheLoai, bv.MaTruong, tr.TenTruong,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_Truong tr ON tr.MaTruong = bv.MaTruong
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
            WHERE bv.TrangThai = 1
            ORDER BY bv.LuotXem DESC, bv.NgayDang DESC");
    }

    /// <summary>Lấy N bài viết mới nhất (cho sidebar).</summary>
    public static DataTable GetMoiNhat(int top = 5)
    {
        return DBHelper.Query($@"
            SELECT TOP {top} bv.MaBaiViet, bv.TieuDe, bv.Slug, bv.NgayDang, bv.AnhChinh, bv.TheLoai, bv.TenNguoiDang,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
            WHERE bv.TrangThai = 1
            ORDER BY bv.NgayDang DESC");
    }

    /// <summary>Lấy N bài viết xem nhiều nhất (cho sidebar).</summary>
    public static DataTable GetXemNhieuNhat(int top = 5)
    {
        return DBHelper.Query($@"
            SELECT TOP {top} bv.MaBaiViet, bv.TieuDe, bv.Slug, bv.LuotXem, bv.AnhChinh, bv.TheLoai, bv.TenNguoiDang,
                   COALESCE(NULLIF(bv.TenNguoiDang, ''),
                       CASE tk.MaQuyen
                           WHEN 1 THEN N'Admin'
                           WHEN 2 THEN COALESCE(NULLIF(trTk.TenTruong, ''), N'Trường học')
                           WHEN 4 THEN N'Moderator'
                           WHEN 5 THEN N'Tư vấn viên'
                           ELSE N'Ban Biên tập'
                       END,
                       N'Ban Biên tập') AS TenNguoiDangHienThi
            FROM tbl_BaiViet bv
            LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = bv.MaTacGia
            LEFT JOIN tbl_Truong trTk ON trTk.MaTruong = tk.MaTruong
            WHERE bv.TrangThai = 1
            ORDER BY bv.LuotXem DESC");
    }

    private static SqlParameter[] BuildParams(BaiVietModel m) => new[]
    {
        new SqlParameter("@td",   m.TieuDe),
        new SqlParameter("@slug", (object)m.Slug ?? DBNull.Value),
        new SqlParameter("@anh",  (object)m.AnhChinh ?? DBNull.Value),
        new SqlParameter("@nd",   (object)m.NoiDung ?? DBNull.Value),
        new SqlParameter("@tg",   m.MaTacGia),
        new SqlParameter("@tt",   m.TrangThai),
        new SqlParameter("@tr",   m.MaTruong.HasValue ? (object)m.MaTruong.Value : DBNull.Value),
        new SqlParameter("@tl",   (object)m.TheLoai ?? DBNull.Value),
        new SqlParameter("@tnd",  string.IsNullOrWhiteSpace(m.TenNguoiDang) ? (object)DBNull.Value : m.TenNguoiDang.Trim())
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
        TheLoai   = r.Table.Columns.Contains("TheLoai") && r["TheLoai"] != DBNull.Value ? r["TheLoai"].ToString() : null,
        TenTruong = r.Table.Columns.Contains("TenTruong") && r["TenTruong"] != DBNull.Value
                    ? r["TenTruong"].ToString() : null,
        TenNguoiDang = r.Table.Columns.Contains("TenNguoiDang") && r["TenNguoiDang"] != DBNull.Value
                    ? r["TenNguoiDang"].ToString() : null,
        TenNguoiDangHienThi = r.Table.Columns.Contains("TenNguoiDangHienThi") && r["TenNguoiDangHienThi"] != DBNull.Value && !string.IsNullOrWhiteSpace(r["TenNguoiDangHienThi"].ToString())
                    ? r["TenNguoiDangHienThi"].ToString() : "Ban Biên tập"
    };
}
