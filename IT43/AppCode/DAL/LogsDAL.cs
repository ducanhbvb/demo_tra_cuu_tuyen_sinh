using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// DAL cho tbl_Logs — truy vấn lịch sử hoạt động hệ thống với bộ lọc động.
/// </summary>
public static class LogsDAL
{
    /// <summary>
    /// Lấy danh sách logs với bộ lọc động (hành động, trạng thái, thiết bị, email), phân trang.
    /// Trả về PagedTable kèm thống kê tổng/thành công/thất bại.
    /// </summary>
    public static LogsResult GetDanhSach(
        string hanhDong, bool? isSuccess, string loaiThietBi, string email,
        int pageIndex, int pageSize)
    {
        string where = "WHERE 1=1";
        var countPrms = new List<SqlParameter>();
        var dataPrms  = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(hanhDong))
        {
            where += " AND l.HanhDong=@hd";
            countPrms.Add(new SqlParameter("@hd", hanhDong));
            dataPrms.Add(new SqlParameter("@hd",  hanhDong));
        }
        if (isSuccess.HasValue)
        {
            where += " AND l.IsSuccess=@ok";
            countPrms.Add(new SqlParameter("@ok", isSuccess.Value));
            dataPrms.Add(new SqlParameter("@ok",  isSuccess.Value));
        }
        if (!string.IsNullOrEmpty(loaiThietBi))
        {
            where += " AND l.LoaiThietBi=@ltd";
            countPrms.Add(new SqlParameter("@ltd", loaiThietBi));
            dataPrms.Add(new SqlParameter("@ltd",  loaiThietBi));
        }
        if (!string.IsNullOrWhiteSpace(email))
        {
            where += " AND tk.Email LIKE @em";
            string kw = $"%{email.Trim()}%";
            countPrms.Add(new SqlParameter("@em", kw));
            dataPrms.Add(new SqlParameter("@em",  kw));
        }

        string baseQuery = $"FROM tbl_Logs l LEFT JOIN tbl_TaiKhoan tk ON tk.MaTaiKhoan=l.MaTaiKhoan {where}";

        int tong = DBHelper.Val<int>(DBHelper.Scalar($"SELECT COUNT(1) {baseQuery}", countPrms.ToArray()));

        // Clone params cho query thành công
        var statPrms = new List<SqlParameter>();
        foreach (var p in countPrms)
            statPrms.Add(new SqlParameter(p.ParameterName, p.Value));

        int thanhCong = DBHelper.Val<int>(DBHelper.Scalar(
            $"SELECT COUNT(1) {baseQuery} AND l.IsSuccess=1", statPrms.ToArray()));

        dataPrms.Add(new SqlParameter("@skip", pageIndex * pageSize));
        dataPrms.Add(new SqlParameter("@take", pageSize));

        var dt = DBHelper.Query($@"
            SELECT l.LogID, l.HanhDong, l.ThoiGian, l.IPAddress, l.LoaiThietBi,
                   l.MoTa, l.IsSuccess, l.MaLoi, l.TrangUrl, l.SessionID,
                   tk.Email
            {baseQuery}
            ORDER BY l.ThoiGian DESC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
            dataPrms.ToArray());

        return new LogsResult
        {
            Data       = dt,
            TongSo     = tong,
            ThanhCong  = thanhCong,
            ThatBai    = tong - thanhCong,
            PageIndex  = pageIndex,
            PageSize   = pageSize,
            TongTrang  = (int)System.Math.Ceiling((double)tong / pageSize)
        };
    }
}

/// <summary>Kết quả trả về từ LogsDAL.GetDanhSach.</summary>
public class LogsResult
{
    public DataTable Data      { get; set; }
    public int TongSo          { get; set; }
    public int ThanhCong       { get; set; }
    public int ThatBai         { get; set; }
    public int PageIndex       { get; set; }
    public int PageSize        { get; set; }
    public int TongTrang       { get; set; }
}
