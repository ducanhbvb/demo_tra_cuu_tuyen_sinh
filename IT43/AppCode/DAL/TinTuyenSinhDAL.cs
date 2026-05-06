using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Data Access Layer — Tin tuyển sinh (tbl_TinTuyenSinh).
/// Tìm kiếm (SP phân trang), CRUD, toggle trạng thái, lấy theo trường.
/// </summary>
public static class TinTuyenSinhDAL
{
    // ── Tìm kiếm (SP có phân trang) ───────────────────────────────────────
    public static PagedTable TimKiem(int? maTruong, int? maChuyenNganh, int? maPhuongThuc,
        short? nam, decimal? diemTu, decimal? diemDen, string tinhThanh,
        int pageIndex, int pageSize)
    {
        var pTong = new SqlParameter("@TongSo", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var prms = new[]
        {
            new SqlParameter("@MaTruong",      maTruong.HasValue       ? (object)maTruong.Value       : DBNull.Value),
            new SqlParameter("@MaChuyenNganh", maChuyenNganh.HasValue  ? (object)maChuyenNganh.Value  : DBNull.Value),
            new SqlParameter("@MaPhuongThuc",  maPhuongThuc.HasValue   ? (object)maPhuongThuc.Value   : DBNull.Value),
            new SqlParameter("@NamTuyenSinh",  nam.HasValue            ? (object)nam.Value             : DBNull.Value),
            new SqlParameter("@DiemTu",        diemTu.HasValue         ? (object)diemTu.Value          : DBNull.Value),
            new SqlParameter("@DiemDen",       diemDen.HasValue        ? (object)diemDen.Value         : DBNull.Value),
            new SqlParameter("@TinhThanh",     string.IsNullOrEmpty(tinhThanh) ? (object)DBNull.Value  : tinhThanh),
            new SqlParameter("@PageIndex",     pageIndex),
            new SqlParameter("@PageSize",      pageSize),
            pTong
        };
        var dt = DBHelper.Query("sp_TinTuyenSinh_TimKiem", prms, isSP: true);
        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(pTong.Value), PageIndex = pageIndex, PageSize = pageSize };
    }

    // ── Tin tuyển sinh của 1 trường (hiển thị trên trang chi tiết) ────────
    public static DataTable GetTheoTruong(int maTruong, short? nam = null)
    {
        string sql = @"
            SELECT tts.*, cn.TenChuyenNganh, pt.TenPhuongThuc
            FROM tbl_TinTuyenSinh tts
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh = tts.MaChuyenNganh
            JOIN tbl_PhuongThucXetTuyen pt ON pt.MaPhuongThuc = tts.MaPhuongThuc
            WHERE tts.MaTruong=@mt AND tts.TrangThai=1";
        var prms = new List<SqlParameter>
            { new SqlParameter("@mt", maTruong) };
        if (nam.HasValue) { sql += " AND tts.NamTuyenSinh=@nam"; prms.Add(new SqlParameter("@nam", nam.Value)); }
        sql += " ORDER BY tts.NamTuyenSinh DESC, cn.TenChuyenNganh";
        return DBHelper.Query(sql, prms.ToArray());
    }

    // ── Tìm kiếm cho Admin (gọi sp_Admin_TinTuyenSinh_TimKiem — không filter TrangThai) ───
    public static PagedTable TimKiemAdmin(int? maTruong, int? maChuyenNganh,
        short? nam, int pageIndex, int pageSize)
    {
        var pTong = new SqlParameter("@TongSo", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var prms = new[]
        {
            new SqlParameter("@MaTruong",      maTruong.HasValue      ? (object)maTruong.Value      : DBNull.Value),
            new SqlParameter("@MaChuyenNganh", maChuyenNganh.HasValue ? (object)maChuyenNganh.Value : DBNull.Value),
            new SqlParameter("@NamTuyenSinh",  nam.HasValue           ? (object)nam.Value            : DBNull.Value),
            new SqlParameter("@TrangThai",     DBNull.Value),   // NULL = lấy tất cả
            new SqlParameter("@PageIndex",     pageIndex),
            new SqlParameter("@PageSize",      pageSize),
            pTong
        };
        var dt = DBHelper.Query("sp_Admin_TinTuyenSinh_TimKiem", prms, isSP: true);
        return new PagedTable { Data = dt, TongSo = DBHelper.Val<int>(pTong.Value), PageIndex = pageIndex, PageSize = pageSize };
    }

    // ── CRUD Admin ────────────────────────────────────────────────────────
    public static int Them(TinTuyenSinhModel m)
    {
        var id = DBHelper.Scalar(@"
            INSERT INTO tbl_TinTuyenSinh(MaTruong,MaChuyenNganh,MaPhuongThuc,NamTuyenSinh,ChiTieu,
                HocPhi,ToHopMonHoc,DiemChuanNamTruoc,DiemChuanNamNay,ChenhLechDiem,
                LoaiHinhDaoTao,CoSoDaoTao,MoTa,TieuDe,HanNop,TrangThai)
            VALUES(@tr,@cn,@pt,@nam,@ct,@hp,@tohop,@dc1,@dc2,@cl,@loai,@cs,@mo,@tieuDe,@hanNop,@tt)
            SELECT SCOPE_IDENTITY()",
            BuildParams(m));
        return DBHelper.Val<int>(id);
    }

    public static void CapNhat(TinTuyenSinhModel m)
    {
        var list = new System.Collections.Generic.List<SqlParameter>(BuildParams(m))
            { new SqlParameter("@id", m.MaTin) };
        DBHelper.Execute(@"
            UPDATE tbl_TinTuyenSinh SET MaTruong=@tr,MaChuyenNganh=@cn,MaPhuongThuc=@pt,
                NamTuyenSinh=@nam,ChiTieu=@ct,HocPhi=@hp,ToHopMonHoc=@tohop,
                DiemChuanNamTruoc=@dc1,DiemChuanNamNay=@dc2,ChenhLechDiem=@cl,
                LoaiHinhDaoTao=@loai,CoSoDaoTao=@cs,MoTa=@mo,TieuDe=@tieuDe,HanNop=@hanNop,TrangThai=@tt
            WHERE MaTin=@id",
            list.ToArray());
    }

    public static void Xoa(int maTin)
    {
        // Hard delete đúng 1 record tin tuyển sinh theo MaTin, KHÔNG xóa toàn bộ bảng tbl_TinTuyenSinh.
        DBHelper.Execute("DELETE FROM tbl_TinTuyenSinh WHERE MaTin=@id",
            new[] { new SqlParameter("@id", maTin) });
    }

    public static void ToggleTrangThai(int maTin)
    {
        DBHelper.Execute(
            "UPDATE tbl_TinTuyenSinh SET TrangThai = CASE WHEN TrangThai=1 THEN 0 ELSE 1 END WHERE MaTin=@id",
            new[] { new SqlParameter("@id", maTin) });
    }

    /// <summary>Lấy tin theo ID cho Admin/TruongHoc — bao gồm cả tin đang ẩn.</summary>
    public static TinTuyenSinhModel GetById(int maTin)
        => GetByIdInternal(maTin, onlyActive: false);

    /// <summary>Lấy tin theo ID cho Client — chỉ trả tin đang hiển thị.</summary>
    public static TinTuyenSinhModel GetByIdPublic(int maTin)
        => GetByIdInternal(maTin, onlyActive: true);

    private static TinTuyenSinhModel GetByIdInternal(int maTin, bool onlyActive)
    {
        string sql = @"
            SELECT tts.*, tr.TenTruong, cn.TenChuyenNganh, pt.TenPhuongThuc
            FROM tbl_TinTuyenSinh tts
            JOIN tbl_Truong tr ON tr.MaTruong=tts.MaTruong
            JOIN tbl_ChuyenNganh cn ON cn.MaChuyenNganh=tts.MaChuyenNganh
            JOIN tbl_PhuongThucXetTuyen pt ON pt.MaPhuongThuc=tts.MaPhuongThuc
            WHERE tts.MaTin=@id";
        if (onlyActive)
            sql += " AND tts.TrangThai=1";

        var dt = DBHelper.Query(sql, new[] { new SqlParameter("@id", maTin) });
        return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
    }

    private static SqlParameter[] BuildParams(TinTuyenSinhModel m)
    {
        // Helper tạo decimal param với precision/scale rõ ràng — tránh Arithmetic overflow
        SqlParameter DecimalParam(string name, decimal? val, byte precision = 10, byte scale = 2)
        {
            var p = new SqlParameter(name, SqlDbType.Decimal)
            {
                Precision = precision,
                Scale     = scale,
                Value     = val.HasValue ? (object)val.Value : DBNull.Value
            };
            return p;
        }

        return new[]
        {
            new SqlParameter("@tr",    m.MaTruong),
            new SqlParameter("@cn",    m.MaChuyenNganh),
            new SqlParameter("@pt",    m.MaPhuongThuc),
            new SqlParameter("@nam",   m.NamTuyenSinh),
            new SqlParameter("@ct",    m.ChiTieu.HasValue ? (object)m.ChiTieu.Value : DBNull.Value),
            // Sprint 1: HocPhi đổi thành NVARCHAR(100) — text tự do "10-20 triệu"
            new SqlParameter("@hp", SqlDbType.NVarChar) { Size = 100, Value = (object)m.HocPhi ?? DBNull.Value },
            new SqlParameter("@tohop", (object)m.ToHopMonHoc ?? DBNull.Value),
            DecimalParam("@dc1", m.DiemChuanNamTruoc,    precision: 5,  scale: 2),
            DecimalParam("@dc2", m.DiemChuanNamNay,      precision: 5,  scale: 2),
            DecimalParam("@cl",  m.ChenhLechDiem,        precision: 5,  scale: 2),
            new SqlParameter("@loai",  (object)m.LoaiHinhDaoTao ?? DBNull.Value),
            new SqlParameter("@cs",    (object)m.CoSoDaoTao ?? DBNull.Value),
            new SqlParameter("@mo",    (object)m.MoTa ?? DBNull.Value),
            new SqlParameter("@tieuDe",(object)m.TieuDe ?? DBNull.Value),
            new SqlParameter("@hanNop", m.HanNop.HasValue ? (object)m.HanNop.Value : DBNull.Value),
            new SqlParameter("@tt",    m.TrangThai)
        };
    }

    private static TinTuyenSinhModel MapRow(DataRow r) => new TinTuyenSinhModel
    {
        MaTin              = DBHelper.Val<int>(r["MaTin"]),
        MaTruong           = DBHelper.Val<int>(r["MaTruong"]),
        TenTruong          = r.Table.Columns.Contains("TenTruong") ? r["TenTruong"].ToString() : "",
        MaChuyenNganh      = DBHelper.Val<int>(r["MaChuyenNganh"]),
        TenChuyenNganh     = r.Table.Columns.Contains("TenChuyenNganh") ? r["TenChuyenNganh"].ToString() : "",
        MaPhuongThuc       = DBHelper.Val<int>(r["MaPhuongThuc"]),
        TenPhuongThuc      = r.Table.Columns.Contains("TenPhuongThuc") ? r["TenPhuongThuc"].ToString() : "",
        NamTuyenSinh       = DBHelper.Val<short>(r["NamTuyenSinh"]),
        ChiTieu            = DBHelper.ValN<int>(r["ChiTieu"]),
        HocPhi             = r["HocPhi"] == DBNull.Value ? null : r["HocPhi"].ToString(),  // Sprint 1: NVARCHAR(100)
        ToHopMonHoc        = r["ToHopMonHoc"] == DBNull.Value ? null : r["ToHopMonHoc"].ToString(),
        DiemChuanNamTruoc  = DBHelper.ValN<decimal>(r["DiemChuanNamTruoc"]),
        DiemChuanNamNay    = DBHelper.ValN<decimal>(r["DiemChuanNamNay"]),
        ChenhLechDiem      = DBHelper.ValN<decimal>(r["ChenhLechDiem"]),
        LoaiHinhDaoTao     = r["LoaiHinhDaoTao"] == DBNull.Value ? null : r["LoaiHinhDaoTao"].ToString(),
        CoSoDaoTao         = r["CoSoDaoTao"] == DBNull.Value ? null : r["CoSoDaoTao"].ToString(),
        MoTa               = r["MoTa"] == DBNull.Value ? null : r["MoTa"].ToString(),
        TieuDe             = r.Table.Columns.Contains("TieuDe") && r["TieuDe"] != DBNull.Value ? r["TieuDe"].ToString() : null,
        HanNop             = r.Table.Columns.Contains("HanNop") && r["HanNop"] != DBNull.Value ? (DateTime?)DBHelper.Val<DateTime>(r["HanNop"]) : null,
        NgayDang           = DBHelper.Val<DateTime>(r["NgayDang"]),
        LuotXem            = DBHelper.Val<int>(r["LuotXem"]),
        TrangThai          = DBHelper.Val<bool>(r["TrangThai"])
    };

    // ── Tăng lượt xem ────────────────────────────────────────────────────
    public static void TangLuotXem(int maTin)
    {
        DBHelper.Execute("UPDATE tbl_TinTuyenSinh SET LuotXem=LuotXem+1 WHERE MaTin=@id",
            new[] { new SqlParameter("@id", maTin) });
    }

    // ── Lấy danh sách tin theo trường (dạng Model) ──────────────────────
    public static List<TinTuyenSinhModel> GetListTheoTruong(int maTruong, short? nam = null)
    {
        var dt = GetTheoTruong(maTruong, nam);
        var list = new List<TinTuyenSinhModel>();
        foreach (DataRow r in dt.Rows) list.Add(MapRow(r));
        return list;
    }
}
