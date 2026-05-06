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
        byte? loaiTruong, int? maChuyenNganh, short? nam, decimal? diemMin, decimal? diemMax,
        string sortBy = "TenTruong", string sortDirection = "ASC",
        int pageIndex = 0, int pageSize = 12, bool? trangThai = null,
        byte? capBacDaoTao = null)
    {
        // Normalize về NFC để tránh lỗi Unicode NFD/NFC khi browser gõ có dấu tiếng Việt
        // (ví dụ: "đại học mở" từ IME có thể gửi dạng NFD = decomposed codepoints)
        if (!string.IsNullOrEmpty(tenTruong))
            tenTruong = tenTruong.Normalize(System.Text.NormalizationForm.FormC);

        var pTong = new SqlParameter("@TongSo", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var prms = new[]
        {
            new SqlParameter("@TenTruong",    string.IsNullOrEmpty(tenTruong)  ? (object)DBNull.Value : tenTruong),
            new SqlParameter("@TinhThanh",    string.IsNullOrEmpty(tinhThanh)  ? (object)DBNull.Value : tinhThanh),
            new SqlParameter("@MaVung",       maVung.HasValue       ? (object)maVung.Value       : DBNull.Value),
            new SqlParameter("@LoaiTruong",   loaiTruong.HasValue   ? (object)loaiTruong.Value   : DBNull.Value),
            new SqlParameter("@MaChuyenNganh",maChuyenNganh.HasValue? (object)maChuyenNganh.Value: DBNull.Value),
            new SqlParameter("@Nam",          nam.HasValue          ? (object)nam.Value          : DBNull.Value),
            new SqlParameter("@DiemMin",      diemMin.HasValue      ? (object)diemMin.Value      : DBNull.Value),
            new SqlParameter("@DiemMax",      diemMax.HasValue      ? (object)diemMax.Value      : DBNull.Value),
            new SqlParameter("@SortBy",       string.IsNullOrEmpty(sortBy) ? (object)"TenTruong" : sortBy),
            new SqlParameter("@SortDirection",string.IsNullOrEmpty(sortDirection) ? (object)"ASC" : sortDirection),
            new SqlParameter("@TrangThai",    trangThai.HasValue    ? (object)trangThai.Value    : DBNull.Value),
            new SqlParameter("@PageIndex",    pageIndex),
            new SqlParameter("@PageSize",     pageSize),
            new SqlParameter("@CapBacDaoTao", capBacDaoTao.HasValue ? (object)capBacDaoTao.Value : DBNull.Value),
            pTong
        };
        // sp_Truong_TimKiem đã trả về CapBacDaoTao + TenCapBacDaoTao trực tiếp.
        // Không cần EnsureCapBacDaoTao() (N+1 bug đã được fix trong SP).
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

    /// <summary>
    /// Lấy chi tiết trường cho màn Admin, bao gồm cả trường đã ẩn TrangThai=0.
    /// Không dùng sp_Truong_LayChiTiet vì stored procedure đó lọc cứng TrangThai=1 cho phía Client.
    /// </summary>
    public static TruongModel LayChiTietAdmin(int maTruong)
    {
        var dt = DBHelper.Query(@"
            SELECT
                tr.*,
                (SELECT AVG(CAST(DiemDanhGia AS FLOAT))
                 FROM tbl_DanhGiaTruong WHERE MaTruong = tr.MaTruong AND TrangThai = 1) AS DiemDanhGiaTB,
                (SELECT COUNT(*)
                 FROM tbl_DanhGiaTruong WHERE MaTruong = tr.MaTruong AND TrangThai = 1) AS SoLuongDanhGia
            FROM tbl_Truong tr
            WHERE tr.MaTruong = @id",
            new[] { new SqlParameter("@id", maTruong) });

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
            INSERT INTO tbl_Truong(TenTruong,DiaChi,TinhThanh,MaVung,LoaiTruong,CapBacDaoTao,SoDienThoai,
                Website,AnhDaiDien,AnhBia,MoTa,QuyMo,KiemDinhChatLuong,Slug,MaTaiKhoan)
            VALUES(@ten,@dc,@tt,@vung,@loai,@capbac,@sdt,@web,@anh,@bia,@mo,@qm,@kd,@slug,@tk)
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
                LoaiTruong=@loai,CapBacDaoTao=@capbac,SoDienThoai=@sdt,Website=@web,AnhDaiDien=@anh,AnhBia=@bia,
                MoTa=@mo,QuyMo=@qm,KiemDinhChatLuong=@kd,Slug=@slug,ThoiGianCapNhat=GETDATE()
            WHERE MaTruong=@id",
            list.ToArray());
    }

    public static void Xoa(int maTruong)
    {
        // Xóa tạm đúng 1 record trường theo MaTruong, KHÔNG xóa toàn bộ bảng tbl_Truong.
        // Giữ record trường và tài khoản trường để có thể khôi phục khi khiếu nại thành công.
        DBHelper.Execute(@"
            UPDATE tbl_Truong SET TrangThai = 0 WHERE MaTruong=@id;
            UPDATE tbl_TaiKhoan SET TrangThai = 0 WHERE MaTruong=@id;",
            new[] { new SqlParameter("@id", maTruong) });
    }

    public static void KhoiPhuc(int maTruong)
    {
        // Khôi phục đúng 1 record trường theo MaTruong và mở lại các tài khoản trường liên quan.
        DBHelper.Execute(@"
            UPDATE tbl_Truong SET TrangThai = 1 WHERE MaTruong=@id;
            UPDATE tbl_TaiKhoan SET TrangThai = 1 WHERE MaTruong=@id;",
            new[] { new SqlParameter("@id", maTruong) });
    }

    public static Dictionary<string, int> GetDeleteDependencies(int maTruong)
    {
        var result = new Dictionary<string, int>();
        using var conn = DBHelper.GetConnection();
        conn.Open();

        void Count(string label, string sql)
        {
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@id", maTruong));
            cmd.Parameters.Add(new SqlParameter("@pattern", $"%MaTruong={maTruong}%"));
            result[label] = DBHelper.Val<int>(cmd.ExecuteScalar());
        }

        Count("Bài viết", "SELECT COUNT(1) FROM tbl_BaiViet WHERE MaTruong=@id");
        Count("Tin tuyển sinh", "SELECT COUNT(1) FROM tbl_TinTuyenSinh WHERE MaTruong=@id");
        Count("Ngành/chương trình của trường", "SELECT COUNT(1) FROM tbl_TruongChuyenNganh WHERE MaTruong=@id");
        Count("Tài khoản liên kết trường", "SELECT COUNT(1) FROM tbl_TaiKhoan WHERE MaTruong=@id");
        Count("Đánh giá trường", "SELECT COUNT(1) FROM tbl_DanhGiaTruong WHERE MaTruong=@id");
        Count("Điểm chuẩn lịch sử", "SELECT COUNT(1) FROM tbl_DiemChuanLichSu WHERE MaTruong=@id");
        Count("Góp ý", "SELECT COUNT(1) FROM tbl_GopY WHERE MaTruong=@id");
        Count("Lịch sử tìm kiếm", "SELECT COUNT(1) FROM tbl_SearchHistory WHERE MaTruong=@id");
        Count("Tư vấn", "SELECT COUNT(1) FROM tbl_TuVan WHERE MaTruong=@id");
        Count("Phản hồi tư vấn", "SELECT COUNT(1) FROM tbl_TuVanPhanHoi WHERE MaTuVan IN (SELECT ID FROM tbl_TuVan WHERE MaTruong=@id)");
        Count("Wishlist", "SELECT COUNT(1) FROM tbl_WishList WHERE MaTruong=@id");
        Count("Log/audit liên quan", "SELECT COUNT(1) FROM tbl_Logs WHERE BangTacDong='tbl_Truong' AND MoTa LIKE @pattern");

        return result;
    }

    public static bool HasDeleteDependencies(int maTruong, out string reason)
    {
        var dependencies = GetDeleteDependencies(maTruong);
        var parts = new List<string>();
        foreach (var item in dependencies)
        {
            if (item.Value > 0)
                parts.Add($"{item.Key}: {item.Value:N0}");
        }

        reason = parts.Count == 0
            ? null
            : "Không thể xóa vĩnh viễn vì trường vẫn đang được liên kết bởi: " + string.Join("; ", parts) + ". Vui lòng xử lý/gỡ các dữ liệu phụ thuộc trước.";
        return parts.Count > 0;
    }

    public static List<string> GetLocalImagePathsForHardDelete(int maTruong)
    {
        var paths = new List<string>();
        var dt = DBHelper.Query(@"
            SELECT AnhDaiDien, AnhBia, MoTa
            FROM tbl_Truong
            WHERE MaTruong=@id",
            new[] { new SqlParameter("@id", maTruong) });

        if (dt.Rows.Count == 0) return paths;

        var row = dt.Rows[0];
        void AddPath(object value)
        {
            if (value == null || value == DBNull.Value) return;
            string path = value.ToString();
            if (!string.IsNullOrWhiteSpace(path)
                && path.StartsWith("/Resources/Images/", StringComparison.OrdinalIgnoreCase)
                && !ImageUploadHelper.IsExternalUrl(path)
                && !paths.Exists(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase)))
            {
                paths.Add(path);
            }
        }

        AddPath(row["AnhDaiDien"]);
        AddPath(row["AnhBia"]);

        foreach (var path in ImageUploadHelper.ExtractContentImagePaths(row["MoTa"] == DBNull.Value ? null : row["MoTa"].ToString()))
        {
            if (!paths.Exists(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase)))
                paths.Add(path);
        }

        var baiVietDt = DBHelper.Query(@"
            SELECT AnhChinh, NoiDung
            FROM tbl_BaiViet
            WHERE MaTruong=@id",
            new[] { new SqlParameter("@id", maTruong) });
        foreach (DataRow bv in baiVietDt.Rows)
        {
            AddPath(bv["AnhChinh"]);
            foreach (var path in ImageUploadHelper.ExtractContentImagePaths(bv["NoiDung"] == DBNull.Value ? null : bv["NoiDung"].ToString()))
            {
                if (!paths.Exists(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase)))
                    paths.Add(path);
            }
        }

        var tinDt = DBHelper.Query(@"
            SELECT MoTa
            FROM tbl_TinTuyenSinh
            WHERE MaTruong=@id",
            new[] { new SqlParameter("@id", maTruong) });
        foreach (DataRow tin in tinDt.Rows)
        {
            foreach (var path in ImageUploadHelper.ExtractContentImagePaths(tin["MoTa"] == DBNull.Value ? null : tin["MoTa"].ToString()))
            {
                if (!paths.Exists(x => string.Equals(x, path, StringComparison.OrdinalIgnoreCase)))
                    paths.Add(path);
            }
        }

        return paths;
    }

    public static void XoaVinhVien(int maTruong)
    {
        // Hard delete đúng 1 record trường theo MaTruong và xóa dây chuyền dữ liệu con của riêng trường đó.
        using var conn = DBHelper.GetConnection();
        conn.Open();
        using var tran = conn.BeginTransaction();
        try
        {
            using (var checkCmd = new SqlCommand("SELECT COUNT(1) FROM tbl_Truong WHERE MaTruong=@id", conn, tran))
            {
                checkCmd.Parameters.Add(new SqlParameter("@id", maTruong));
                if (DBHelper.Val<int>(checkCmd.ExecuteScalar()) == 0)
                    throw new InvalidOperationException("Không tìm thấy trường cần xóa vĩnh viễn.");
            }

            void Execute(string sql)
            {
                using var cmd = new SqlCommand(sql, conn, tran);
                cmd.Parameters.Add(new SqlParameter("@id", maTruong));
                cmd.ExecuteNonQuery();
            }

            Execute("DELETE FROM tbl_TuVanPhanHoi WHERE MaTuVan IN (SELECT ID FROM tbl_TuVan WHERE MaTruong=@id)");
            Execute("DELETE FROM tbl_TuVan WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_WishList WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_SearchHistory WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_GopY WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_DanhGiaTruong WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_DiemChuanLichSu WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_TruongChuyenNganh WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_TinTuyenSinh WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_BaiViet WHERE MaTruong=@id");
            Execute("UPDATE tbl_TaiKhoan SET MaTruong = NULL, TrangThai = 0 WHERE MaTruong=@id");
            Execute("DELETE FROM tbl_Truong WHERE MaTruong=@id");

            tran.Commit();
        }
        catch
        {
            tran.Rollback();
            throw;
        }
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
        new SqlParameter("@capbac", m.CapBacDaoTao.HasValue ? (object)m.CapBacDaoTao.Value : DBNull.Value),
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
        CapBacDaoTao      = r.Table.Columns.Contains("CapBacDaoTao") ? DBHelper.ValN<byte>(r["CapBacDaoTao"]) : null,
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
