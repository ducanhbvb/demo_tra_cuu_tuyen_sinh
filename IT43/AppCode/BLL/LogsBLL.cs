/// <summary>Business Logic Layer cho Logs hệ thống.</summary>
public static class LogsBLL
{
    public static LogsResult GetDanhSach(
        string hanhDong, bool? isSuccess, string loaiThietBi, string email,
        int pageIndex, int pageSize)
        => LogsDAL.GetDanhSach(hanhDong, isSuccess, loaiThietBi, email, pageIndex, pageSize);
}
