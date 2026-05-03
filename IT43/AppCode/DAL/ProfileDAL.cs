using System;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Hồ sơ học sinh (tbl_ProfileHocSinh).
/// Lấy và lưu (upsert) hồ sơ cá nhân theo mã tài khoản.
/// </summary>
public static class ProfileDAL
{
    public static ProfileHocSinhModel GetByTaiKhoan(int maTaiKhoan)
    {
        var dt = DBHelper.Query(@"
            SELECT p.*, cn.TenChuyenNganh
            FROM tbl_ProfileHocSinh p
            LEFT JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = p.MaChuyenNganh
            WHERE p.MaTaiKhoan=@id",
            new[] { new SqlParameter("@id", maTaiKhoan) });
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    public static void LuuProfile(ProfileHocSinhModel m)
    {
        int exists = DBHelper.Val<int>(DBHelper.Scalar(
            "SELECT COUNT(1) FROM tbl_ProfileHocSinh WHERE MaTaiKhoan=@id",
            new[] { new SqlParameter("@id", m.MaTaiKhoan) }));

        if (exists > 0)
        {
            DBHelper.Execute(@"
                UPDATE tbl_ProfileHocSinh SET HoTen=@ht,NgaySinh=@ns,TinhThanh=@tt,
                    DiemDuKien=@dk,DiemMonHoc=@dmh,MaChuyenNganh=@cn,MucTieuNghe=@mt,
                    KhuVuc=@kv,AnhDaiDien=@anh,NgayCapNhat=GETDATE()
                WHERE MaTaiKhoan=@id",
                BuildParams(m));
        }
        else
        {
            DBHelper.Execute(@"
                INSERT INTO tbl_ProfileHocSinh(MaTaiKhoan,HoTen,NgaySinh,TinhThanh,
                    DiemDuKien,DiemMonHoc,MaChuyenNganh,MucTieuNghe,KhuVuc,AnhDaiDien)
                VALUES(@id,@ht,@ns,@tt,@dk,@dmh,@cn,@mt,@kv,@anh)",
                BuildParams(m));
        }
    }

    private static SqlParameter[] BuildParams(ProfileHocSinhModel m) => new[]
    {
        new SqlParameter("@id",  m.MaTaiKhoan),
        new SqlParameter("@ht",  (object)m.HoTen ?? DBNull.Value),
        new SqlParameter("@ns",  m.NgaySinh.HasValue ? (object)m.NgaySinh.Value : DBNull.Value),
        new SqlParameter("@tt",  (object)m.TinhThanh ?? DBNull.Value),
        new SqlParameter("@dk",  m.DiemDuKien.HasValue ? (object)m.DiemDuKien.Value : DBNull.Value),
        new SqlParameter("@dmh", (object)m.DiemMonHoc ?? DBNull.Value),
        new SqlParameter("@cn",  m.MaChuyenNganh.HasValue ? (object)m.MaChuyenNganh.Value : DBNull.Value),
        new SqlParameter("@mt",  (object)m.MucTieuNghe ?? DBNull.Value),
        new SqlParameter("@kv",  m.KhuVuc.HasValue ? (object)m.KhuVuc.Value : DBNull.Value),
        new SqlParameter("@anh", (object)m.AnhDaiDien ?? DBNull.Value)
    };

    private static ProfileHocSinhModel MapRow(DataRow r) => new ProfileHocSinhModel
    {
        ID             = DBHelper.Val<int>(r["ID"]),
        MaTaiKhoan     = DBHelper.Val<int>(r["MaTaiKhoan"]),
        HoTen          = r["HoTen"] == DBNull.Value ? null : r["HoTen"].ToString(),
        NgaySinh       = DBHelper.ValN<DateTime>(r["NgaySinh"]),
        TinhThanh      = r["TinhThanh"] == DBNull.Value ? null : r["TinhThanh"].ToString(),
        DiemDuKien     = DBHelper.ValN<decimal>(r["DiemDuKien"]),
        DiemMonHoc     = r["DiemMonHoc"] == DBNull.Value ? null : r["DiemMonHoc"].ToString(),
        MaChuyenNganh  = DBHelper.ValN<int>(r["MaChuyenNganh"]),
        TenChuyenNganh = r.Table.Columns.Contains("TenChuyenNganh") && r["TenChuyenNganh"] != DBNull.Value
                         ? r["TenChuyenNganh"].ToString() : null,
        MucTieuNghe    = r["MucTieuNghe"] == DBNull.Value ? null : r["MucTieuNghe"].ToString(),
        KhuVuc         = DBHelper.ValN<byte>(r["KhuVuc"]),
        AnhDaiDien     = r["AnhDaiDien"] == DBNull.Value ? null : r["AnhDaiDien"].ToString(),
        NgayCapNhat    = DBHelper.Val<DateTime>(r["NgayCapNhat"])
    };
}
