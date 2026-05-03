using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Trường đại học/cao đẳng (tbl_Truong).
/// Tìm kiếm (SP phân trang), chi tiết, CRUD, danh sách ngành của trường.
/// </summary>
public static class TruongDAL
{
    // ── Tìm kiếm + phân trang (dùng SP) ──────────────────────────────────
    public static PagedTable TimKiem(string tenTruong, string tinhThanh, byte? maVung,
        byte? loaiTruong, int? maChuyenNganh, int pageIndex, int pageSize,
        bool? trangThai = null)
    {
        var pTong = new SqlParameter("@TongSo", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var prms = new[]
        {
            new SqlParameter("@TenTruong",    string.IsNullOrEmpty(tenTruong)  ? (object)DBNull.Value : tenTruong),
            new SqlParameter("@TinhThanh",    string.IsNullOrEmpty(tinhThanh)  ? (object)DBNull.Value : tinhThanh),
            new SqlParameter("@MaVung",       maVung.HasValue       ? (object)maVung.Value       : DBNull.Value),
            new SqlParameter("@LoaiTruong",   loaiTruong.HasValue   ? (object)loaiTruong.Value   : DBNull.Value),
            new SqlParameter("@MaChuyenNganh",maChuyenNganh.HasValue? (object)maChuyenNganh.Value: DBNull.Value),
            new SqlParameter("@TrangThai",    trangThai.HasValue    ? (object)trangThai.Value    : DBNull.Value),
            new SqlParameter("@PageIndex",    pageIndex),
            new SqlParameter("@PageSize",     pageSize),
            pTong
        };
        var dt = DBHelper.Query("sp_Truong_TimKiem", prms, isSP: true);
        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(pTong.Value), PageIndex = pageIndex, PageSize = pageSize };
    }

    // ── Chi tiết trường ───────────────────────────────────────────────────
    public static TruongModel LayChiTiet(int? maTruong = null, string slug = null)
    {
        var dt = DBHelper.Query("sp_Truong_LayChiTiet", new[]
        {
            new SqlParameter("@MaTruong", maTruong.HasValue ? (object)maTruong.Value : DBNull.Value),
            new SqlParameter("@Slug",     slug != null      ? (object)slug           : DBNull.Value)
        }, isSP: true);
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    // ── Danh sách ngành của trường ────────────────────────────────────────
    public static DataTable GetNganhCuaTruong(int maTruong)
    {
        return DBHelper.Query(@"
            SELECT tcn.*, cn.TenChuyenNganh, dmn.TenDanhMuc, cb.TenCapBac
            FROM tbl_TruongChuyenNganh tcn
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = tcn.MaChuyenNganh
            JOIN tbl_DanhMucNganh dmn ON dmn.MaDanhMuc = cn.MaDanhMuc
            JOIN tbl_CapBac cb ON cb.MaCapBac = tcn.MaCapBac
            WHERE tcn.MaTruong = @id
            ORDER BY dmn.TenDanhMuc, cn.TenChuyenNganh",
            new[] { new SqlParameter("@id", maTruong) });
    }

    // ── CRUD Admin ────────────────────────────────────────────────────────
    public static int Them(TruongModel m)
    {
        var id = DBHelper.Scalar(@"
            INSERT INTO tbl_Truong(TenTruong,DiaChi,TinhThanh,MaVung,LoaiTruong,SoDienThoai,
                Website,AnhDaiDien,AnhBia,MoTa,QuyMo,KiemDinhChatLuong,Slug,MaTaiKhoan)
            VALUES(@ten,@dc,@tt,@vung,@loai,@sdt,@web,@anh,@bia,@mo,@qm,@kd,@slug,@tk)
            SELECT SCOPE_IDENTITY()",
            BuildParams(m));
        return DBHelper.Val<int>(id);
    }

    public static void CapNhat(TruongModel m)
    {
        var list = new List<SqlParameter>(BuildParams(m))
        {
            new SqlParameter("@id", m.MaTruong)
        };
        DBHelper.Execute(@"
            UPDATE tbl_Truong SET TenTruong=@ten,DiaChi=@dc,TinhThanh=@tt,MaVung=@vung,
                LoaiTruong=@loai,SoDienThoai=@sdt,Website=@web,AnhDaiDien=@anh,AnhBia=@bia,
                MoTa=@mo,QuyMo=@qm,KiemDinhChatLuong=@kd,Slug=@slug,ThoiGianCapNhat=GETDATE()
            WHERE MaTruong=@id",
            list.ToArray());
    }

    public static void Xoa(int maTruong)
    {
        DBHelper.Execute("DELETE FROM tbl_Truong WHERE MaTruong=@id",
            new[] { new SqlParameter("@id", maTruong) });
    }

    public static void ToggleTrangThai(int maTruong)
    {
        DBHelper.Execute(
            "UPDATE tbl_Truong SET TrangThai = CASE WHEN TrangThai=1 THEN 0 ELSE 1 END WHERE MaTruong=@id",
            new[] { new SqlParameter("@id", maTruong) });
    }

    private static SqlParameter[] BuildParams(TruongModel m) => new[]
    {
        new SqlParameter("@ten",  m.TenTruong ?? ""),
        new SqlParameter("@dc",   (object)m.DiaChi   ?? DBNull.Value),
        new SqlParameter("@tt",   (object)m.TinhThanh ?? DBNull.Value),
        new SqlParameter("@vung", m.MaVung.HasValue  ? (object)m.MaVung.Value : DBNull.Value),
        new SqlParameter("@loai", m.LoaiTruong.HasValue ? (object)m.LoaiTruong.Value : DBNull.Value),
        new SqlParameter("@sdt",  (object)m.SoDienThoai ?? DBNull.Value),
        new SqlParameter("@web",  (object)m.Website ?? DBNull.Value),
        new SqlParameter("@anh",  (object)m.AnhDaiDien ?? DBNull.Value),
        new SqlParameter("@bia",  (object)m.AnhBia ?? DBNull.Value),
        new SqlParameter("@mo",   (object)m.MoTa ?? DBNull.Value),
        new SqlParameter("@qm",   (object)m.QuyMo ?? DBNull.Value),
        new SqlParameter("@kd",   m.KiemDinhChatLuong),
        new SqlParameter("@slug", (object)m.Slug ?? DBNull.Value),
        new SqlParameter("@tk",   m.MaTaiKhoan)
    };

    private static TruongModel MapRow(DataRow r) => new TruongModel
    {
        MaTruong          = DBHelper.Val<int>(r["MaTruong"]),
        TenTruong         = r["TenTruong"].ToString(),
        DiaChi            = r["DiaChi"] == DBNull.Value ? null : r["DiaChi"].ToString(),
        TinhThanh         = r["TinhThanh"] == DBNull.Value ? null : r["TinhThanh"].ToString(),
        MaVung            = DBHelper.ValN<byte>(r["MaVung"]),
        LoaiTruong        = DBHelper.ValN<byte>(r["LoaiTruong"]),
        SoDienThoai       = r["SoDienThoai"] == DBNull.Value ? null : r["SoDienThoai"].ToString(),
        Website           = r["Website"] == DBNull.Value ? null : r["Website"].ToString(),
        AnhDaiDien        = r["AnhDaiDien"] == DBNull.Value ? null : r["AnhDaiDien"].ToString(),
        AnhBia            = r["AnhBia"] == DBNull.Value ? null : r["AnhBia"].ToString(),
        MoTa              = r["MoTa"] == DBNull.Value ? null : r["MoTa"].ToString(),
        QuyMo             = r["QuyMo"] == DBNull.Value ? null : r["QuyMo"].ToString(),
        KiemDinhChatLuong = DBHelper.Val<bool>(r["KiemDinhChatLuong"]),
        TrangThai         = r.Table.Columns.Contains("TrangThai") ? DBHelper.Val<bool>(r["TrangThai"]) : true,
        Slug              = r["Slug"] == DBNull.Value ? null : r["Slug"].ToString(),
        MaTaiKhoan        = r.Table.Columns.Contains("MaTaiKhoan") ? DBHelper.Val<int>(r["MaTaiKhoan"]) : 0,
        ThoiGianCapNhat   = DBHelper.Val<DateTime>(r["ThoiGianCapNhat"]),
        DiemDanhGiaTB     = r.Table.Columns.Contains("DiemDanhGiaTB") && r["DiemDanhGiaTB"] != DBNull.Value
                            ? DBHelper.ValN<double>(r["DiemDanhGiaTB"]) : null,
        SoLuongDanhGia    = r.Table.Columns.Contains("SoLuongDanhGia") ? DBHelper.Val<int>(r["SoLuongDanhGia"]) : 0
    };
}
