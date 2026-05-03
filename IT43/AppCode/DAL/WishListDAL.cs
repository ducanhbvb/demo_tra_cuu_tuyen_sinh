using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Danh sách yêu thích (tbl_WishList).
/// Lấy, thêm, xóa trường yêu thích theo tài khoản.
/// </summary>
public static class WishListDAL
{
    public static List<WishListModel> GetByTaiKhoan(int maTaiKhoan)
    {
        var dt = DBHelper.Query(@"
            SELECT wl.*, tr.TenTruong, tr.Slug AS TruongSlug, tr.AnhDaiDien, tr.TinhThanh,
                   cn.TenChuyenNganh
            FROM tbl_WishList wl
            JOIN tbl_Truong tr ON tr.MaTruong = wl.MaTruong
            LEFT JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = wl.MaChuyenNganh
            WHERE wl.MaTaiKhoan=@id
            ORDER BY wl.NgayThem DESC",
            new[] { new SqlParameter("@id", maTaiKhoan) });

        var list = new List<WishListModel>();
        foreach (DataRow r in dt.Rows)
            list.Add(new WishListModel
            {
                ID             = DBHelper.Val<int>(r["ID"]),
                MaTaiKhoan     = DBHelper.Val<int>(r["MaTaiKhoan"]),
                MaTruong       = DBHelper.Val<int>(r["MaTruong"]),
                TenTruong      = r["TenTruong"].ToString(),
                TruongSlug     = r["TruongSlug"] == DBNull.Value ? null : r["TruongSlug"].ToString(),
                AnhDaiDien     = r["AnhDaiDien"] == DBNull.Value ? null : r["AnhDaiDien"].ToString(),
                TinhThanh      = r["TinhThanh"] == DBNull.Value ? null : r["TinhThanh"].ToString(),
                MaChuyenNganh  = DBHelper.ValN<int>(r["MaChuyenNganh"]),
                TenChuyenNganh = r["TenChuyenNganh"] == DBNull.Value ? null : r["TenChuyenNganh"].ToString(),
                GhiChu         = r["GhiChu"] == DBNull.Value ? null : r["GhiChu"].ToString(),
                NgayThem       = DBHelper.Val<DateTime>(r["NgayThem"])
            });
        return list;
    }

    public static bool DaThem(int maTaiKhoan, int maTruong, int? maChuyenNganh)
    {
        var r = DBHelper.Scalar(@"
            SELECT COUNT(1) FROM tbl_WishList
            WHERE MaTaiKhoan=@tk AND MaTruong=@tr
              AND ((@cn IS NULL AND MaChuyenNganh IS NULL) OR MaChuyenNganh=@cn)",
            new[]
            {
                new SqlParameter("@tk", maTaiKhoan),
                new SqlParameter("@tr", maTruong),
                new SqlParameter("@cn", maChuyenNganh.HasValue ? (object)maChuyenNganh.Value : DBNull.Value)
            });
        return DBHelper.Val<int>(r) > 0;
    }

    public static void Them(int maTaiKhoan, int maTruong, int? maChuyenNganh, string ghiChu = null)
    {
        DBHelper.Execute(@"
            IF NOT EXISTS (SELECT 1 FROM tbl_WishList WHERE MaTaiKhoan=@tk AND MaTruong=@tr
                           AND ((@cn IS NULL AND MaChuyenNganh IS NULL) OR MaChuyenNganh=@cn))
            INSERT INTO tbl_WishList(MaTaiKhoan,MaTruong,MaChuyenNganh,GhiChu)
            VALUES(@tk,@tr,@cn,@gh)",
            new[]
            {
                new SqlParameter("@tk", maTaiKhoan),
                new SqlParameter("@tr", maTruong),
                new SqlParameter("@cn", maChuyenNganh.HasValue ? (object)maChuyenNganh.Value : DBNull.Value),
                new SqlParameter("@gh", (object)ghiChu ?? DBNull.Value)
            });
    }

    public static void Xoa(int id, int maTaiKhoan)
    {
        DBHelper.Execute("DELETE FROM tbl_WishList WHERE ID=@id AND MaTaiKhoan=@tk",
            new[] { new SqlParameter("@id", id), new SqlParameter("@tk", maTaiKhoan) });
    }
}
