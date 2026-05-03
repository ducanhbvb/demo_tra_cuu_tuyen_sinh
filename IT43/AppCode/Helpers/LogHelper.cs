using System.Data.SqlClient;
using System.Web;

/// <summary>
/// Ghi log hành động người dùng vào tbl_Logs.
/// Dùng cho: đăng nhập, đăng xuất, đổi mật khẩu, v.v.
/// </summary>
public static class LogHelper
{
    public static void Ghi(int? maTaiKhoan, string hanhDong, string moTa = null, bool isSuccess = true, string maLoi = null)
    {
        try
        {
            var ctx = HttpContext.Current;
            string ip         = ctx?.Request?.UserHostAddress;
            string userAgent  = ctx?.Request?.UserAgent;
            string trangUrl   = ctx?.Request?.Url?.PathAndQuery;
            string sessionId  = ctx?.Session?.SessionID;
            string loaiThietBi = DetectDevice(userAgent);

            DBHelper.Execute(@"
                INSERT INTO tbl_Logs
                    (MaTaiKhoan, HanhDong, MoTa, IPAddress, UserAgent, TrangUrl,
                     SessionID, LoaiThietBi, IsSuccess, MaLoi)
                VALUES
                    (@maTK, @hd, @moTa, @ip, @ua, @url,
                     @sid, @ltd, @ok, @ml)",
                new[]
                {
                    new SqlParameter("@maTK", maTaiKhoan.HasValue ? (object)maTaiKhoan.Value : System.DBNull.Value),
                    new SqlParameter("@hd",   hanhDong),
                    new SqlParameter("@moTa", (object)moTa    ?? System.DBNull.Value),
                    new SqlParameter("@ip",   (object)ip       ?? System.DBNull.Value),
                    new SqlParameter("@ua",   (object)userAgent ?? System.DBNull.Value),
                    new SqlParameter("@url",  (object)trangUrl  ?? System.DBNull.Value),
                    new SqlParameter("@sid",  (object)sessionId ?? System.DBNull.Value),
                    new SqlParameter("@ltd",  (object)loaiThietBi ?? System.DBNull.Value),
                    new SqlParameter("@ok",   isSuccess),
                    new SqlParameter("@ml",   (object)maLoi   ?? System.DBNull.Value)
                });
        }
        catch
        {
            // Không để lỗi log làm crash ứng dụng
        }
    }

    private static string DetectDevice(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "Unknown";
        string ua = userAgent.ToLower();
        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            return "Mobile";
        if (ua.Contains("tablet") || ua.Contains("ipad"))
            return "Tablet";
        return "Desktop";
    }
}
