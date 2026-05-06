using System;
using System.Data;
using System.Web.UI;

/// <summary>
/// Dashboard Cổng Trường — hiển thị thống kê + bài viết/tin tuyển sinh/tư vấn chờ
/// của trường đang đăng nhập.
/// </summary>
public partial class TruongHoc_Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadStats();
    }

    private void LoadStats()
    {
        int maTruong = SecurityHelper.GetCurrentMaTruong();

        // ── Tên trường lên banner ─────────────────────────────────────
        if (maTruong > 0)
        {
            var truong = TruongBLL.LayChiTiet(maTruong);
            if (truong != null)
                litTenTruongBanner.Text = Server.HtmlEncode(truong.TenTruong);
        }

        if (maTruong <= 0)
        {
            litSoBaiViet.Text         = "0";
            litSoTin.Text             = "0";
            litSoBaiVietHoatDong.Text = "0";
            litNamTuyenSinh.Text      = DateTime.Now.Year.ToString();
            litSoTuVan.Text           = "0";
            litSoTuVanBanner.Text     = "0";
            litSoTuVanBtn.Text        = "0";
            litTuVanCount.Text        = "0";
            pnlNoTuVan.Visible        = true;
            return;
        }

        // ── Bài viết của trường ───────────────────────────────────────
        var bvAll    = BaiVietBLL.GetDanhSach(maTruong, 0, 5, chiActive: false);
        var bvActive = BaiVietBLL.GetDanhSach(maTruong, 0, 1, chiActive: true);

        litSoBaiViet.Text         = bvAll.TongSo.ToString("N0");
        litSoBaiVietHoatDong.Text = bvActive.TongSo.ToString("N0");

        gvBaiViet.DataSource = bvAll.Data;
        gvBaiViet.DataBind();

        // ── Tin tuyển sinh của trường ─────────────────────────────────
        var tinResult = TinTuyenSinhBLL.TimKiemAdmin(maTruong, null, null, 0, 5);
        litSoTin.Text        = tinResult.TongSo.ToString("N0");
        litNamTuyenSinh.Text = DateTime.Now.Year.ToString();

        gvTin.DataSource = tinResult.Data;
        gvTin.DataBind();

        // ── Tư vấn chờ của trường ─────────────────────────────────────
        int soTuVan = ThongKeBLL.SoTuVanChoCuaTruong(maTruong);
        litSoTuVan.Text       = soTuVan.ToString("N0");
        litSoTuVanBanner.Text = soTuVan.ToString("N0");
        litSoTuVanBtn.Text    = soTuVan.ToString();
        litTuVanCount.Text    = soTuVan.ToString();

        // ── Top 3 tư vấn chờ xử lý ───────────────────────────────────
        var tvResult = TuVanDanhGiaBLL.GetDanhSach("TUVAN", 0, maTruong, 0, 3);
        rptTuVan.DataSource = tvResult.Data;
        rptTuVan.DataBind();
        pnlNoTuVan.Visible = (tvResult.Data == null || tvResult.Data.Rows.Count == 0);
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
}
