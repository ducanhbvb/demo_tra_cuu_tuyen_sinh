using System;
using System.Data;
using System.Web.UI;

/// <summary>
/// Trang Dashboard Admin — hiển thị thống kê tổng quan, trường mới, tư vấn chờ và hoạt động gần đây.
/// </summary>
public partial class Admin_Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadStats();
    }

    private void LoadStats()
    {
        // ── Stat numbers ──────────────────────────────────────────────────
        int soTruong  = ThongKeBLL.SoTruong();
        int soTin     = ThongKeBLL.SoTin();
        int soTuVan   = ThongKeBLL.SoTuVanCho();
        int soTK      = ThongKeBLL.SoTaiKhoan();
        int soNganh   = ThongKeBLL.SoNganh();

        litSoTruong.Text       = soTruong.ToString("N0");
        litSoTin.Text          = soTin.ToString("N0");
        litSoTuVan.Text        = soTuVan.ToString("N0");
        litSoTK.Text           = soTK.ToString("N0");
        litSoNganh.Text        = soNganh.ToString("N0");

        // Banner & button badge
        litSoTuVanBanner.Text  = soTuVan.ToString("N0");
        litSoTuVanBtn.Text     = soTuVan.ToString();
        litTuVanCount.Text     = soTuVan.ToString();

        // ── Logs stat (lấy 1 bản ghi đủ để có TongSo/ThanhCong, không load nặng) ──
        var logsStatResult = LogsBLL.GetDanhSach("", null, "", "", 0, 1);
        litSoLog.Text        = logsStatResult.TongSo.ToString("N0");
        litLogThanhCong.Text = logsStatResult.ThanhCong.ToString("N0");

        // ── Top 5 trường mới cập nhật ────────────────────────────────────
        DataTable dtTruong = ThongKeBLL.GetTopTruong(5);
        rptTruong.DataSource = dtTruong;
        rptTruong.DataBind();

        // ── Top 3 tư vấn chờ xử lý ──────────────────────────────────────
        var tvResult = TuVanDanhGiaBLL.GetDanhSach("TUVAN", 0, null, 0, 3);
        rptTuVan.DataSource = tvResult.Data;
        rptTuVan.DataBind();
        pnlNoTuVan.Visible = (tvResult.Data == null || tvResult.Data.Rows.Count == 0);

        // ── Top 5 hoạt động gần đây (từ logs) ───────────────────────────
        var actResult = LogsBLL.GetDanhSach("", null, "", "", 0, 5);
        rptActivity.DataSource = actResult.Data;
        rptActivity.DataBind();
        pnlNoActivity.Visible = (actResult.Data == null || actResult.Data.Rows.Count == 0);
    }

    // ── Helper: format thời gian tương đối ───────────────────────────────
    protected string FormatTimeAgo(object val)
    {
        if (val == null || val == DBNull.Value) return "";
        DateTime dt;
        if (!DateTime.TryParse(val.ToString(), out dt)) return val.ToString();

        var diff = DateTime.Now - dt;
        if (diff.TotalMinutes < 1)   return "vừa xong";
        if (diff.TotalMinutes < 60)  return (int)diff.TotalMinutes + " phút trước";
        if (diff.TotalHours   < 24)  return (int)diff.TotalHours   + " giờ trước";
        if (diff.TotalDays    < 7)   return (int)diff.TotalDays    + " ngày trước";
        if (diff.TotalDays    < 30)  return (int)(diff.TotalDays/7) + " tuần trước";
        if (diff.TotalDays    < 365) return (int)(diff.TotalDays/30) + " tháng trước";
        return (int)(diff.TotalDays/365) + " năm trước";
    }

    // ── Helper: lấy chữ cái đầu tên ─────────────────────────────────────
    protected string GetInitial(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ');
        return parts[parts.Length - 1].Substring(0, 1).ToUpper();
    }

    // ── Helper: màu avatar theo tên ──────────────────────────────────────
    protected string GetAvatarColor(string name)
    {
        string[] colors = { "#4f46e5","#10b981","#f59e0b","#06b6d4","#8b5cf6","#ef4444","#14b8a6","#f97316" };
        if (string.IsNullOrEmpty(name)) return colors[0];
        return colors[Math.Abs(name.GetHashCode()) % colors.Length];
    }

    // ── Helper: tên hành động ───────────────────────────────────────────
    protected string GetActivityTitle(string hanhDong)
    {
        if (string.IsNullOrEmpty(hanhDong)) return "Hoạt động";
        switch (hanhDong.ToUpper())
        {
            case "DANG_NHAP":         return "Đăng nhập thành công";
            case "DANG_NHAP_FAIL":    return "Đăng nhập thất bại";
            case "DOI_MAT_KHAU":      return "Đổi mật khẩu";
            case "UPGRADE_HASH":      return "Nâng cấp bảo mật";
            case "APPLICATION_ERROR": return "Lỗi hệ thống";
            case "SEND_EMAIL_FAIL":   return "Lỗi gửi email";
            case "CAP_NHAT_TRUONG":   return "Cập nhật thông tin trường";
            case "THEM_TRUONG":       return "Thêm trường mới";
            default: return hanhDong.Replace("_", " ");
        }
    }
}
