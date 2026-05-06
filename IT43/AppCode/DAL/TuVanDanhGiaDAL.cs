using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Tư vấn, Góp ý và Đánh giá trường.
/// Gửi tư vấn/góp ý (SP), Admin duyệt, đánh giá trường (upsert).
/// </summary>
public static class TuVanDanhGiaDAL
{
    // ── Tư vấn ────────────────────────────────────────────────────────────
    public static int GuiTuVan(TuVanModel m)
    {
        var pID = new SqlParameter("@IDMoi", SqlDbType.Int) { Direction = ParameterDirection.Output };
        DBHelper.ExecSP("sp_GopY_TuVan_Them", new[]
        {
            new SqlParameter("@Loai",       "TUVAN"),
            new SqlParameter("@MaTaiKhoan", m.MaTaiKhoan.HasValue ? (object)m.MaTaiKhoan.Value : DBNull.Value),
            new SqlParameter("@MaTruong",   m.MaTruong),
            new SqlParameter("@HoTen",      m.HoTen),
            new SqlParameter("@Email",      m.Email),
            new SqlParameter("@SoDienThoai",(object)m.SoDienThoai ?? DBNull.Value),
            new SqlParameter("@NoiDung",    m.NoiDung),
            new SqlParameter("@LoaiGopY",   DBNull.Value),
            pID
        });
        return DBHelper.Val<int>(pID.Value);
    }

    // ── Góp ý ─────────────────────────────────────────────────────────────
    public static int GuiGopY(int? maTaiKhoan, int? maTruong, byte loaiGopY, string noiDung)
    {
        var pID = new SqlParameter("@IDMoi", SqlDbType.Int) { Direction = ParameterDirection.Output };
        DBHelper.ExecSP("sp_GopY_TuVan_Them", new[]
        {
            new SqlParameter("@Loai",       "GOPY"),
            new SqlParameter("@MaTaiKhoan", maTaiKhoan.HasValue ? (object)maTaiKhoan.Value : DBNull.Value),
            new SqlParameter("@MaTruong",   maTruong.HasValue   ? (object)maTruong.Value   : DBNull.Value),
            new SqlParameter("@HoTen",      DBNull.Value),
            new SqlParameter("@Email",      DBNull.Value),
            new SqlParameter("@SoDienThoai",DBNull.Value),
            new SqlParameter("@NoiDung",    noiDung),
            new SqlParameter("@LoaiGopY",   loaiGopY),
            pID
        });
        return DBHelper.Val<int>(pID.Value);
    }

    // ── Admin: chi tiết theo ID ───────────────────────────────────────────

    /// <summary>
    /// Lấy chi tiết 1 bản ghi tư vấn hoặc góp ý theo ID.
    /// Trả về DataRow với các cột: HoTen, Email, TenTruong, NgayGui, TrangThai, NoiDung, GhiChuAdmin (chỉ TuVan).
    /// </summary>
    public static DataRow GetChiTiet(string loai, int id)
    {
        string sql = loai == "TUVAN"
            ? @"SELECT tv.*, tr.TenTruong FROM tbl_TuVan tv
                LEFT JOIN tbl_Truong tr ON tr.MaTruong = tv.MaTruong
                WHERE tv.ID = @id"
            : @"SELECT gy.*, tr.TenTruong FROM tbl_GopY gy
                LEFT JOIN tbl_Truong tr ON tr.MaTruong = gy.MaTruong
                WHERE gy.ID = @id";

        var dt = DBHelper.Query(sql, new[] { new SqlParameter("@id", id) });
        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
    }

    // ── Admin: danh sách tư vấn / góp ý ──────────────────────────────────
    public static PagedTable GetDanhSach(string loai, byte? trangThai, int? maTruong, int pageIndex, int pageSize)
    {
        // Helper tạo mới SqlParameter mỗi lần — tránh lỗi "already contained by another collection"
        object ttVal = trangThai.HasValue ? (object)trangThai.Value : DBNull.Value;
        object mtVal = maTruong.HasValue  ? (object)maTruong.Value  : DBNull.Value;

        string countSql, dataSql;
        if (loai == "TUVAN")
        {
            countSql = @"SELECT COUNT(1) FROM tbl_TuVan tv
                         WHERE (@tt IS NULL OR tv.TrangThai=@tt)
                           AND (@mt IS NULL OR tv.MaTruong=@mt)";
            dataSql  = @"SELECT tv.*, tr.TenTruong FROM tbl_TuVan tv
                         JOIN tbl_Truong tr ON tr.MaTruong=tv.MaTruong
                         WHERE (@tt IS NULL OR tv.TrangThai=@tt)
                           AND (@mt IS NULL OR tv.MaTruong=@mt)
                         ORDER BY tv.NgayGui DESC
                         OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        }
        else
        {
            countSql = @"SELECT COUNT(1) FROM tbl_GopY gy
                         WHERE (@tt IS NULL OR gy.TrangThai=@tt)
                           AND (@mt IS NULL OR gy.MaTruong=@mt)";
            // Thêm NULL AS HoTen, NULL AS Email để GridView không crash khi Eval("HoTen")
            dataSql  = @"SELECT gy.*, NULL AS HoTen, NULL AS Email, tr.TenTruong
                         FROM tbl_GopY gy
                         LEFT JOIN tbl_Truong tr ON tr.MaTruong=gy.MaTruong
                         WHERE (@tt IS NULL OR gy.TrangThai=@tt)
                           AND (@mt IS NULL OR gy.MaTruong=@mt)
                         ORDER BY gy.NgayGui DESC
                         OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        }

        // Tạo mới SqlParameter riêng cho count query
        var countPrms = new[]
        {
            new SqlParameter("@tt", ttVal),
            new SqlParameter("@mt", mtVal)
        };
        var count = DBHelper.Val<int>(DBHelper.Scalar(countSql, countPrms));

        // Tạo mới SqlParameter riêng cho data query
        var dataPrms = new[]
        {
            new SqlParameter("@tt",   ttVal),
            new SqlParameter("@mt",   mtVal),
            new SqlParameter("@skip", pageIndex * pageSize),
            new SqlParameter("@take", pageSize)
        };
        var dt = DBHelper.Query(dataSql, dataPrms);

        return new PagedTable { Data = dt, TongSo = count, PageIndex = pageIndex, PageSize = pageSize };
    }

    public static void CapNhatTrangThai(string loai, int id, byte trangThai, string ghiChu = null)
    {
        DBHelper.ExecSP("sp_GopYTuVan_CapNhatTrangThai", new[]
        {
            new SqlParameter("@Loai",      loai),
            new SqlParameter("@ID",        id),
            new SqlParameter("@TrangThai", trangThai),
            new SqlParameter("@GhiChu",    (object)ghiChu ?? DBNull.Value)
        });
    }

    // ── Đánh giá trường ──────────────────────────────────────────────────
    public static DataTable GetDanhGia(int maTruong, byte? trangThai = 1)
    {
        return DBHelper.Query(@"
            SELECT dg.*, tk.Email
            FROM tbl_DanhGiaTruong dg
            JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan = dg.MaTaiKhoan
            WHERE dg.MaTruong=@tr AND (@tt IS NULL OR dg.TrangThai=@tt)
            ORDER BY dg.NgayDang DESC",
            new[]
            {
                new SqlParameter("@tr", maTruong),
                new SqlParameter("@tt", trangThai.HasValue ? (object)trangThai.Value : DBNull.Value)
            });
    }

    public static void GuiDanhGia(int maTruong, int maTaiKhoan, byte diem, string noiDung)
    {
        DBHelper.Execute(@"
            IF NOT EXISTS (SELECT 1 FROM tbl_DanhGiaTruong WHERE MaTruong=@tr AND MaTaiKhoan=@tk)
                INSERT INTO tbl_DanhGiaTruong(MaTruong,MaTaiKhoan,DiemDanhGia,NoiDung,TrangThai)
                VALUES(@tr,@tk,@diem,@nd,0)
            ELSE
                UPDATE tbl_DanhGiaTruong SET DiemDanhGia=@diem, NoiDung=@nd, TrangThai=0
                WHERE MaTruong=@tr AND MaTaiKhoan=@tk",
            new[]
            {
                new SqlParameter("@tr",   maTruong),
                new SqlParameter("@tk",   maTaiKhoan),
                new SqlParameter("@diem", diem),
                new SqlParameter("@nd",   (object)noiDung ?? DBNull.Value)
            });
    }
}
