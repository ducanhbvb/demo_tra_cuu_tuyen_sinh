using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// DAL cho trang Tìm kiếm theo Ngành học — trả kết quả phân trang.
/// </summary>
public static class TimKiemTheoNganhDAL
{
    /// <summary>
    /// Tìm tất cả tin tuyển sinh theo ngành + năm, kết hợp thông tin trường.
    /// </summary>
    public static PagedTable TimKiem(int? maChuyenNganh, int? namTuyenSinh, int pageIndex, int pageSize)
    {
        string where = "WHERE t.TrangThai=1 AND tr.TrangThai=1";
        var countPrms = new List<SqlParameter>();
        var dataPrms  = new List<SqlParameter>();

        if (maChuyenNganh.HasValue)
        {
            where += " AND t.MaChuyenNganh=@cn";
            countPrms.Add(new SqlParameter("@cn", maChuyenNganh.Value));
            dataPrms.Add(new SqlParameter("@cn",  maChuyenNganh.Value));
        }
        if (namTuyenSinh.HasValue)
        {
            where += " AND t.NamTuyenSinh=@nam";
            countPrms.Add(new SqlParameter("@nam", namTuyenSinh.Value));
            dataPrms.Add(new SqlParameter("@nam",  namTuyenSinh.Value));
        }

        var count = DBHelper.Scalar(
            $@"SELECT COUNT(1)
               FROM tbl_TinTuyenSinh t
               JOIN tbl_Truong tr ON tr.MaTruong=t.MaTruong
               {where}",
            countPrms.ToArray());

        dataPrms.Add(new SqlParameter("@skip", pageIndex * pageSize));
        dataPrms.Add(new SqlParameter("@take", pageSize));

        var dt = DBHelper.Query($@"
            SELECT
                t.MaTin,
                tr.MaTruong,
                tr.TenTruong,
                tr.TinhThanh,
                tr.Slug        AS TruongSlug,
                cn.TenChuyenNganh,
                dmn.TenDanhMuc AS NhomNganh,
                cb.TenCapBac,
                pt.TenPhuongThuc,
                t.NamTuyenSinh,
                t.ChiTieu,
                t.HocPhi,
                t.ToHopMonHoc,
                t.DiemChuanNamTruoc,
                t.DiemChuanNamNay,
                t.ChenhLechDiem,
                t.LoaiHinhDaoTao
            FROM tbl_TinTuyenSinh t
            JOIN tbl_Truong              tr  ON tr.MaTruong      = t.MaTruong
            JOIN tbl_ChuyenNganh         cn  ON cn.MaChuyenNganh = t.MaChuyenNganh
            JOIN tbl_DanhMucNganh        dmn ON dmn.MaDanhMuc     = cn.MaDanhMuc
            LEFT JOIN tbl_CapBac         cb  ON cb.MaCapBac       = (
                SELECT TOP 1 MaCapBac FROM tbl_TruongChuyenNganh
                WHERE MaTruong=t.MaTruong AND MaChuyenNganh=t.MaChuyenNganh)
            JOIN tbl_PhuongThucXetTuyen  pt  ON pt.MaPhuongThuc   = t.MaPhuongThuc
            {where}
            ORDER BY t.NamTuyenSinh DESC, t.DiemChuanNamTruoc DESC, tr.TenTruong ASC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
            dataPrms.ToArray());

        return new PagedTable
        {
            Data      = dt,
            TongSo    = DBHelper.Val<int>(count),
            PageIndex = pageIndex,
            PageSize  = pageSize
        };
    }
}
