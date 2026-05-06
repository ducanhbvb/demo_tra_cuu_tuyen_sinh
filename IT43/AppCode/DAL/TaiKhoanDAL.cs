using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Tài khoản (tbl_TaiKhoan).
/// Đăng nhập (SP brute-force), đăng ký, xác nhận email, đặt lại mật khẩu,
/// quản lý danh sách tài khoản (Admin).
/// </summary>
public static class TaiKhoanDAL
{
    // ── Đăng nhập (dùng SP có brute-force protect) ────────────────────────
    public static (byte ketQua, int maTaiKhoan, int maQuyen, int maTruong) DangNhap(string email, string matKhauHash)
    {
        var pKetQua   = new SqlParameter("@KetQua",     SqlDbType.TinyInt) { Direction = ParameterDirection.Output };
        var pMaTK     = new SqlParameter("@MaTaiKhoan", SqlDbType.Int)     { Direction = ParameterDirection.Output };
        var pQuyen    = new SqlParameter("@MaQuyen",    SqlDbType.Int)     { Direction = ParameterDirection.Output };
        var pMaTruong = new SqlParameter("@MaTruong",   SqlDbType.Int)     { Direction = ParameterDirection.Output };

        DBHelper.ExecSP("sp_TaiKhoan_DangNhap", new[]
        {
            new SqlParameter("@Email",   email),
            new SqlParameter("@MatKhau", matKhauHash),
            pKetQua, pMaTK, pQuyen, pMaTruong
        });

        return (
            DBHelper.Val<byte>(pKetQua.Value),
            DBHelper.Val<int>(pMaTK.Value),
            DBHelper.Val<int>(pQuyen.Value),
            pMaTruong.Value == DBNull.Value ? 0 : DBHelper.Val<int>(pMaTruong.Value)
        );
    }

    // ── Đăng ký ───────────────────────────────────────────────────────────
    public static bool EmailTonTai(string email)
    {
        var r = DBHelper.Scalar(
            "SELECT COUNT(1) FROM tbl_TaiKhoan WHERE Email=@e",
            new[] { new SqlParameter("@e", email) });
        return DBHelper.Val<int>(r) > 0;
    }

    // Đăng ký KHÔNG cần xác nhận email (dùng khi chưa cấu hình SMTP)
    public static int DangKy(string email, string matKhauHash, int maQuyen)
    {
        var id = DBHelper.Scalar(@"
            INSERT INTO tbl_TaiKhoan(Email,MatKhau,MaQuyen,EmailDaXacNhan,TrangThai)
            VALUES(@e,@mk,@q,1,1)
            SELECT SCOPE_IDENTITY()",
            new[]
            {
                new SqlParameter("@e",   email),
                new SqlParameter("@mk",  matKhauHash),
                new SqlParameter("@q",   maQuyen)
            });
        return DBHelper.Val<int>(id);
    }

    // Đăng ký CÓ xác nhận email (dùng khi SMTP đã cấu hình)
    public static int DangKy(string email, string matKhauHash, int maQuyen, string tokenXacNhan)
    {
        var id = DBHelper.Scalar(@"
            INSERT INTO tbl_TaiKhoan(Email,MatKhau,MaQuyen,EmailDaXacNhan,TokenXacNhanEmail)
            VALUES(@e,@mk,@q,0,@tok)
            SELECT SCOPE_IDENTITY()",
            new[]
            {
                new SqlParameter("@e",   email),
                new SqlParameter("@mk",  matKhauHash),
                new SqlParameter("@q",   maQuyen),
                new SqlParameter("@tok", tokenXacNhan)
            });
        return DBHelper.Val<int>(id);
    }

    // ── Xác nhận email ────────────────────────────────────────────────────
    public static bool XacNhanEmail(string token)
    {
        int rows = DBHelper.Execute(@"
            UPDATE tbl_TaiKhoan
            SET EmailDaXacNhan=1, TokenXacNhanEmail=NULL
            WHERE TokenXacNhanEmail=@tok AND EmailDaXacNhan=0",
            new[] { new SqlParameter("@tok", token) });
        return rows > 0;
    }

    // ── Quên mật khẩu ─────────────────────────────────────────────────────
    public static bool TaoTokenDatLai(string email, string token)
    {
        var pKQ = new SqlParameter("@KetQua", SqlDbType.TinyInt) { Direction = ParameterDirection.Output };
        DBHelper.ExecSP("sp_TaiKhoan_DatLaiMatKhau", new[]
        {
            new SqlParameter("@HanhDong", "TAO_TOKEN"),
            new SqlParameter("@Email",    email),
            new SqlParameter("@Token",    token),
            new SqlParameter("@MatKhauMoi", DBNull.Value),
            pKQ
        });
        return DBHelper.Val<byte>(pKQ.Value) == 0;
    }

    public static bool DatLaiMatKhau(string token, string matKhauMoiHash)
    {
        var pKQ = new SqlParameter("@KetQua", SqlDbType.TinyInt) { Direction = ParameterDirection.Output };
        DBHelper.ExecSP("sp_TaiKhoan_DatLaiMatKhau", new[]
        {
            new SqlParameter("@HanhDong",   "DAT_LAI"),
            new SqlParameter("@Email",      DBNull.Value),
            new SqlParameter("@Token",      token),
            new SqlParameter("@MatKhauMoi", matKhauMoiHash),
            pKQ
        });
        return DBHelper.Val<byte>(pKQ.Value) == 0;
    }

    // ── Lấy thông tin tài khoản ───────────────────────────────────────────
    /// <summary>Lấy theo Email — dùng cho PBKDF2 verify + auto-upgrade hash.</summary>
    public static TaiKhoanModel GetByEmail(string email)
    {
        var dt = DBHelper.Query(
            "SELECT tk.*, q.TenQuyen FROM tbl_TaiKhoan tk JOIN tbl_Quyen q ON q.MaQuyen=tk.MaQuyen WHERE tk.Email=@e",
            new[] { new SqlParameter("@e", email) });
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    public static TaiKhoanModel GetById(int maTaiKhoan)
    {
        var dt = DBHelper.Query(
            "SELECT tk.*, q.TenQuyen FROM tbl_TaiKhoan tk JOIN tbl_Quyen q ON q.MaQuyen=tk.MaQuyen WHERE tk.MaTaiKhoan=@id",
            new[] { new SqlParameter("@id", maTaiKhoan) });
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    // ── Cập nhật mật khẩu (đổi mật khẩu) ────────────────────────────────
    public static void DoiMatKhau(int maTaiKhoan, string matKhauMoiHash)
    {
        DBHelper.Execute(
            "UPDATE tbl_TaiKhoan SET MatKhau=@mk WHERE MaTaiKhoan=@id",
            new[] { new SqlParameter("@mk", matKhauMoiHash), new SqlParameter("@id", maTaiKhoan) });
    }

    // ── Admin: danh sách tài khoản ────────────────────────────────────────
    public static PagedTable GetDanhSach(int pageIndex, int pageSize, int? maQuyen = null, string keyword = null)
    {
        string where = "WHERE 1=1";
        // Tạo 2 bộ params riêng biệt (count + data) tránh lỗi "SqlParameter already contained"
        var countPrms = new List<SqlParameter>();
        var dataPrms  = new List<SqlParameter>();

        if (maQuyen.HasValue)
        {
            where += " AND tk.MaQuyen=@q";
            countPrms.Add(new SqlParameter("@q", maQuyen.Value));
            dataPrms.Add(new SqlParameter("@q",  maQuyen.Value));
        }
        if (!string.IsNullOrEmpty(keyword))
        {
            where += " AND tk.Email LIKE @kw";
            countPrms.Add(new SqlParameter("@kw", $"%{keyword}%"));
            dataPrms.Add(new SqlParameter("@kw",  $"%{keyword}%"));
        }

        // Thêm @skip/@take vào dataPrms (không dùng string interpolation cho OFFSET)
        dataPrms.Add(new SqlParameter("@skip", pageIndex * pageSize));
        dataPrms.Add(new SqlParameter("@take", pageSize));

        var count = DBHelper.Scalar($"SELECT COUNT(1) FROM tbl_TaiKhoan tk {where}", countPrms.ToArray());
        var sql = $@"SELECT tk.*, q.TenQuyen, tr.TenTruong FROM tbl_TaiKhoan tk
                     JOIN tbl_Quyen q ON q.MaQuyen=tk.MaQuyen
                     LEFT JOIN tbl_Truong tr ON tr.MaTruong=tk.MaTruong
                     {where} ORDER BY tk.NgayTao DESC
                     OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";
        var dt = DBHelper.Query(sql, dataPrms.ToArray());
        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(count), PageIndex = pageIndex, PageSize = pageSize };
    }

    public static void CapNhatTrangThai(int maTaiKhoan, bool trangThai)
    {
        DBHelper.Execute(
            "UPDATE tbl_TaiKhoan SET TrangThai=@tt WHERE MaTaiKhoan=@id",
            new[] { new SqlParameter("@tt", trangThai), new SqlParameter("@id", maTaiKhoan) });
    }

    // ── Admin: Thêm tài khoản mới ─────────────────────────────────────────
    /// <summary>Admin tạo tài khoản mới — kích hoạt ngay, set YeuCauDoiMatKhau=1.</summary>
    public static int AdminTaoTaiKhoan(string email, string matKhauHash, int maQuyen, int? maTruong)
    {
        var id = DBHelper.Scalar(@"
            INSERT INTO tbl_TaiKhoan(Email,MatKhau,MaQuyen,MaTruong,EmailDaXacNhan,TrangThai,YeuCauDoiMatKhau)
            VALUES(@e,@mk,@q,@mt,1,1,1)
            SELECT SCOPE_IDENTITY()",
            new[]
            {
                new SqlParameter("@e",  email),
                new SqlParameter("@mk", matKhauHash),
                new SqlParameter("@q",  maQuyen),
                new SqlParameter("@mt", maTruong.HasValue ? (object)maTruong.Value : DBNull.Value)
            });
        return DBHelper.Val<int>(id);
    }

    // ── Admin: Cập nhật quyền + MaTruong ─────────────────────────────────
    /// <summary>Admin đổi quyền và gán/hủy trường của tài khoản.</summary>
    public static void AdminCapNhatQuyen(int maTaiKhoan, int maQuyen, int? maTruong)
    {
        DBHelper.Execute(
            "UPDATE tbl_TaiKhoan SET MaQuyen=@q, MaTruong=@mt WHERE MaTaiKhoan=@id",
            new[]
            {
                new SqlParameter("@q",   maQuyen),
                new SqlParameter("@mt",  maTruong.HasValue ? (object)maTruong.Value : DBNull.Value),
                new SqlParameter("@id",  maTaiKhoan)
            });
    }

    // ── Admin: Đặt mật khẩu trực tiếp (Cấp 3 reset) ─────────────────────
    /// <summary>Admin reset MK trực tiếp — set YeuCauDoiMatKhau=1 để bắt user đổi ngay.</summary>
    public static void AdminDatMatKhau(int maTaiKhoan, string matKhauMoiHash)
    {
        DBHelper.Execute(
            "UPDATE tbl_TaiKhoan SET MatKhau=@mk, YeuCauDoiMatKhau=1 WHERE MaTaiKhoan=@id",
            new[] { new SqlParameter("@mk", matKhauMoiHash), new SqlParameter("@id", maTaiKhoan) });
    }

    // ── Xóa cờ yêu cầu đổi mật khẩu sau khi user đổi xong ───────────────
    /// <summary>Đổi MK mới + clear flag YeuCauDoiMatKhau trong một UPDATE.</summary>
    public static void DoiMatKhauBatBuoc(int maTaiKhoan, string matKhauMoiHash)
    {
        DBHelper.Execute(
            "UPDATE tbl_TaiKhoan SET MatKhau=@mk, YeuCauDoiMatKhau=0 WHERE MaTaiKhoan=@id",
            new[] { new SqlParameter("@mk", matKhauMoiHash), new SqlParameter("@id", maTaiKhoan) });
    }

    /// <summary>Lấy flag YeuCauDoiMatKhau — dùng sau khi đăng nhập thành công để check.</summary>
    public static bool GetYeuCauDoiMatKhau(int maTaiKhoan)
    {
        var r = DBHelper.Scalar(
            "SELECT YeuCauDoiMatKhau FROM tbl_TaiKhoan WHERE MaTaiKhoan=@id",
            new[] { new SqlParameter("@id", maTaiKhoan) });
        return r != null && DBHelper.Val<bool>(r);
    }

    private static TaiKhoanModel MapRow(DataRow r) => new TaiKhoanModel
    {
        MaTaiKhoan        = DBHelper.Val<int>(r["MaTaiKhoan"]),
        Email             = r["Email"].ToString(),
        MatKhau           = r["MatKhau"].ToString(),
        MaQuyen           = DBHelper.Val<int>(r["MaQuyen"]),
        TenQuyen          = r.Table.Columns.Contains("TenQuyen") ? r["TenQuyen"].ToString() : "",
        NgayTao           = DBHelper.Val<DateTime>(r["NgayTao"]),
        TrangThai         = DBHelper.Val<bool>(r["TrangThai"]),
        SoLanDangNhapSai  = DBHelper.Val<int>(r["SoLanDangNhapSai"]),
        KhoaTaiKhoanDen   = DBHelper.ValN<DateTime>(r["KhoaTaiKhoanDen"]),
        LanDangNhapCuoi   = DBHelper.ValN<DateTime>(r["LanDangNhapCuoi"]),
        EmailDaXacNhan    = DBHelper.Val<bool>(r["EmailDaXacNhan"]),
        TokenXacNhanEmail = r["TokenXacNhanEmail"] == DBNull.Value ? null : r["TokenXacNhanEmail"].ToString(),
        TokenDatLaiMatKhau= r["TokenDatLaiMatKhau"] == DBNull.Value ? null : r["TokenDatLaiMatKhau"].ToString(),
        TokenHetHan       = DBHelper.ValN<DateTime>(r["TokenHetHan"]),
        YeuCauDoiMatKhau  = r.Table.Columns.Contains("YeuCauDoiMatKhau")
                            ? DBHelper.Val<bool>(r["YeuCauDoiMatKhau"]) : false,
        MaTruong          = r.Table.Columns.Contains("MaTruong") && r["MaTruong"] != DBNull.Value
                            ? (int?)DBHelper.Val<int>(r["MaTruong"]) : null
    };
}
