using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>Load dữ liệu danh mục dùng cho dropdown toàn trang.</summary>
public static class DanhMucDAL
{
    public static List<LookupModel> GetQuyen()
    {
        var dt = DBHelper.Query("SELECT MaQuyen AS Id, TenQuyen AS Ten, 0 AS ThuTu FROM tbl_Quyen ORDER BY MaQuyen");
        return MapList(dt);
    }

    public static List<LookupModel> GetCapBac()
    {
        var dt = DBHelper.Query("SELECT MaCapBac AS Id, TenCapBac AS Ten, 0 AS ThuTu FROM tbl_CapBac ORDER BY MaCapBac");
        return MapList(dt);
    }

    public static List<LookupModel> GetDanhMucNganh()
    {
        var dt = DBHelper.Query("SELECT MaDanhMuc AS Id, TenDanhMuc AS Ten, ThuTu FROM tbl_DanhMucNganh ORDER BY ThuTu, TenDanhMuc");
        return MapList(dt);
    }

    public static List<LookupModel> GetChuyenNganh(int? maDanhMuc = null)
    {
        string sql;
        DataTable dt;
        if (maDanhMuc.HasValue)
        {
            sql = "SELECT MaChuyenNganh AS Id, TenChuyenNganh AS Ten, 0 AS ThuTu FROM tbl_ChuyenNganh WHERE MaDanhMuc=@p ORDER BY TenChuyenNganh";
            dt = DBHelper.Query(sql, new[] { new SqlParameter("@p", maDanhMuc.Value) });
        }
        else
        {
            sql = "SELECT MaChuyenNganh AS Id, TenChuyenNganh AS Ten, 0 AS ThuTu FROM tbl_ChuyenNganh ORDER BY TenChuyenNganh";
            dt = DBHelper.Query(sql);
        }
        return MapList(dt);
    }

    public static List<LookupModel> GetPhuongThuc()
    {
        var dt = DBHelper.Query("SELECT MaPhuongThuc AS Id, TenPhuongThuc AS Ten, ThuTu FROM tbl_PhuongThucXetTuyen ORDER BY ThuTu");
        return MapList(dt);
    }

    /// <summary>
    /// Danh sách 34 tỉnh/TP dự kiến sau sáp nhập 2025.
    /// Hardcoded để không phụ thuộc vào dữ liệu tbl_Truong có thể chưa cập nhật.
    /// </summary>
    public static List<string> GetTinhThanh()
    {
        return new List<string>
        {
            // ── Miền Bắc ──────────────────────────────────────────────────────
            "TP Hà Nội",
            "Tỉnh Cao Bằng",
            "Tỉnh Điện Biên",
            "Tỉnh Hà Tĩnh",
            "Tỉnh Hưng Yên",
            "Tỉnh Lai Châu",
            "Tỉnh Lạng Sơn",
            "Tỉnh Lào Cai",
            "Tỉnh Nghệ An",
            "Tỉnh Ninh Bình",
            "Tỉnh Phú Thọ",
            "Tỉnh Quảng Ninh",
            "Tỉnh Bắc Ninh",
            "Tỉnh Sơn La",
            "Tỉnh Thái Nguyên",
            "Tỉnh Thanh Hoá",
            "Tỉnh Tuyên Quang",
            "TP Hải Phòng",
            // ── Miền Trung ────────────────────────────────────────────────────
            "TP Huế",
            "TP Đà Nẵng",
            "Tỉnh Đắk Lắk",
            "Tỉnh Gia Lai",
            "Tỉnh Khánh Hoà",
            "Tỉnh Lâm Đồng",
            "Tỉnh Quảng Ngãi",
            "Tỉnh Quảng Trị",
            // ── Miền Nam ──────────────────────────────────────────────────────
            "TP Hồ Chí Minh",
            "TP Cần Thơ",
            "Tỉnh An Giang",
            "Tỉnh Cà Mau",
            "Tỉnh Đồng Nai",
            "Tỉnh Đồng Tháp",
            "Tỉnh Tây Ninh",
            "Tỉnh Vĩnh Long",
        };
    }

    public static List<int> GetNamTuyenSinh()
    {
        var dt = DBHelper.Query("SELECT DISTINCT NamTuyenSinh FROM tbl_TinTuyenSinh ORDER BY NamTuyenSinh DESC");
        var list = new List<int>();
        foreach (DataRow r in dt.Rows)
            list.Add(DBHelper.Val<int>(r[0]));
        return list;
    }

    private static List<LookupModel> MapList(DataTable dt)
    {
        var list = new List<LookupModel>();
        foreach (DataRow r in dt.Rows)
            list.Add(new LookupModel
            {
                Id    = DBHelper.Val<int>(r["Id"]),
                Ten   = r["Ten"].ToString(),
                ThuTu = DBHelper.Val<int>(r["ThuTu"])
            });
        return list;
    }
}
