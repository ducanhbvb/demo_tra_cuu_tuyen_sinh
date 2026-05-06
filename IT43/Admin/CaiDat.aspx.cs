using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Cài đặt hệ thống Admin — load/save config qua ConfigBLL.
/// Chỉ Admin mới được truy cập (kiểm tra bằng SecurityHelper.IsAdmin()).
/// </summary>
public partial class Admin_CaiDat : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Bảo mật: Chỉ Admin
        if (!SecurityHelper.IsAdmin())
        {
            Response.Redirect("~/Admin/Default.aspx");
            return;
        }

        if (!IsPostBack)
            LoadConfigSettings();
    }

    // ── Load giá trị hiện tại từ ConfigBLL ───────────────────────────────
    private void LoadConfigSettings()
    {
        int nam     = ConfigBLL.GetInt("NamTuyenSinhHienTai", 0);
        int truong  = ConfigBLL.GetInt("SoTruongNoiBat",      8);
        int baiViet = ConfigBLL.GetInt("SoBaiVietMoi",        6);
        int tin     = ConfigBLL.GetInt("SoTinMoi",            10);
        bool allowPastDates = ConfigBLL.GetBool("AllowPastDates", false);

        // Badge preview
        litNamHienTai.Text = nam > 0 ? nam.ToString() : "Tự động";
        litSoTruong.Text   = truong.ToString();
        litSoBaiViet.Text  = baiViet.ToString();
        litSoTin.Text      = tin.ToString();

        // Điền vào TextBox
        txtNamTuyenSinh.Text   = nam > 0 ? nam.ToString() : "";
        txtSoTruongNoiBat.Text = truong.ToString();
        txtSoBaiViet.Text      = baiViet.ToString();
        txtSoTin.Text          = tin.ToString();

        LoadHomeTagsConfig();

        // AllowPastDates
        chkAllowPastDates.Checked = allowPastDates;
        UpdateAllowPastDatesPreview();
    }

    private void LoadHomeTagsConfig()
    {
        txtHomeTag1Text.Text = ConfigBLL.GetString("HomeTag1Text", "Bách Khoa");
        txtHomeTag1Url.Text = ConfigBLL.GetString("HomeTag1Url", "/TimKiemTruong.aspx?q=Bách%20Khoa");
        SetDropDownValue(ddlHomeTag1Icon, ConfigBLL.GetString("HomeTag1Icon", "bi bi-fire text-warning"));

        txtHomeTag2Text.Text = ConfigBLL.GetString("HomeTag2Text", "Đại học Công Lập");
        txtHomeTag2Url.Text = ConfigBLL.GetString("HomeTag2Url", "/TimKiemTruong.aspx?loai=1");
        SetDropDownValue(ddlHomeTag2Icon, ConfigBLL.GetString("HomeTag2Icon", "bi bi-building"));

        txtHomeTag3Text.Text = ConfigBLL.GetString("HomeTag3Text", "Khối ngành IT");
        txtHomeTag3Url.Text = ConfigBLL.GetString("HomeTag3Url", "/TimKiemTruong.aspx?q=CNTT");
        SetDropDownValue(ddlHomeTag3Icon, ConfigBLL.GetString("HomeTag3Icon", "bi bi-laptop"));
    }

    private void SetDropDownValue(DropDownList ddl, string value)
    {
        if (ddl.Items.FindByValue(value) != null)
            ddl.SelectedValue = value;
        else if (ddl.Items.Count > 0)
            ddl.SelectedIndex = 0;
    }

    private void UpdateAllowPastDatesPreview()
    {
        lblAllowPastDatesPreview.InnerText = chkAllowPastDates.Checked ? "Đã bật" : "Đã tắt";
        lblAllowPastDatesPreview.Attributes["class"] = chkAllowPastDates.Checked
            ? "status-pill is-on"
            : "status-pill is-off";
    }

    // ── Lưu: Năm tuyển sinh ──────────────────────────────────────────────
    protected void btnSaveNamTuyenSinh_Click(object sender, EventArgs e)
    {
        string raw = txtNamTuyenSinh.Text.Trim();

        // Cho phép xóa trắng → về chế độ "tự động"
        if (string.IsNullOrEmpty(raw))
        {
            ConfigBLL.SetValue("NamTuyenSinhHienTai", "0");
            ThongKeBLL.InvalidateCache();
            ShowSuccess("Đã reset năm tuyển sinh về <b>Tự động</b> (lấy từ dữ liệu mới nhất).");
            LoadConfigSettings();
            return;
        }

        if (!int.TryParse(raw, out int nam))
        {
            ShowError("Năm phải là số nguyên hợp lệ.");
            return;
        }
        if (nam < 2015 || nam > DateTime.Now.Year + 2)
        {
            ShowError($"Năm tuyển sinh không hợp lý. Hãy nhập trong khoảng 2015 – {DateTime.Now.Year + 2}.");
            return;
        }

        ConfigBLL.SetValue("NamTuyenSinhHienTai", nam.ToString());
        ThongKeBLL.InvalidateCache(); // Trang chủ cập nhật ngay
        ShowSuccess($"Đã lưu năm tuyển sinh hiển thị: <b>{nam}</b>.");
        LoadConfigSettings();
    }

    // ── Lưu: Số trường nổi bật ──────────────────────────────────────────
    protected void btnSaveSoTruong_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtSoTruongNoiBat.Text.Trim(), out int so) || so < 1 || so > 20)
        {
            ShowError("Số trường nổi bật phải là số nguyên từ 1 đến 20.");
            return;
        }

        ConfigBLL.SetValue("SoTruongNoiBat", so.ToString());
        ThongKeBLL.InvalidateCache();
        ShowSuccess($"Đã lưu số trường nổi bật trang chủ: <b>{so} trường</b>.");
        LoadConfigSettings();
    }

    // ── Lưu: Số bài viết mới ────────────────────────────────────────────
    protected void btnSaveBaiViet_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtSoBaiViet.Text.Trim(), out int so) || so < 1 || so > 15)
        {
            ShowError("Số bài viết phải là số nguyên từ 1 đến 15.");
            return;
        }

        ConfigBLL.SetValue("SoBaiVietMoi", so.ToString());
        ThongKeBLL.InvalidateCache(); // Đảm bảo trang chủ reload ngay
        ShowSuccess($"Đã lưu số bài viết Slider trang chủ: <b>{so} bài</b>.");
        LoadConfigSettings();
    }

    // ── Lưu: Số tin tuyển sinh mới ───────────────────────────────────────
    protected void btnSaveSoTin_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtSoTin.Text.Trim(), out int so) || so < 1 || so > 20)
        {
            ShowError("Số tin tuyển sinh phải là số nguyên từ 1 đến 20.");
            return;
        }

        ConfigBLL.SetValue("SoTinMoi", so.ToString());
        ThongKeBLL.InvalidateCache(); // Đảm bảo trang chủ reload ngay
        ShowSuccess($"Đã lưu số tin tuyển sinh mới nhất: <b>{so} tin</b>.");
        LoadConfigSettings();
    }

    // ── Lưu: 3 nút gợi ý trang chủ ───────────────────────────────────────
    protected void btnSaveHomeTags_Click(object sender, EventArgs e)
    {
        string tag1Text = NormalizeTagText(txtHomeTag1Text.Text, "Bách Khoa", "Nhãn nút 1");
        string tag1Url = NormalizeInternalUrl(txtHomeTag1Url.Text, "/TimKiemTruong.aspx?q=Bách%20Khoa", "Đường dẫn nút 1");
        if (tag1Text == null || tag1Url == null) return;

        string tag2Text = NormalizeTagText(txtHomeTag2Text.Text, "Đại học Công Lập", "Nhãn nút 2");
        string tag2Url = NormalizeInternalUrl(txtHomeTag2Url.Text, "/TimKiemTruong.aspx?loai=1", "Đường dẫn nút 2");
        if (tag2Text == null || tag2Url == null) return;

        string tag3Text = NormalizeTagText(txtHomeTag3Text.Text, "Khối ngành IT", "Nhãn nút 3");
        string tag3Url = NormalizeInternalUrl(txtHomeTag3Url.Text, "/TimKiemTruong.aspx?q=CNTT", "Đường dẫn nút 3");
        if (tag3Text == null || tag3Url == null) return;

        ConfigBLL.SetValue("HomeTag1Text", tag1Text);
        ConfigBLL.SetValue("HomeTag1Url", tag1Url);
        ConfigBLL.SetValue("HomeTag1Icon", ddlHomeTag1Icon.SelectedValue);
        ConfigBLL.SetValue("HomeTag2Text", tag2Text);
        ConfigBLL.SetValue("HomeTag2Url", tag2Url);
        ConfigBLL.SetValue("HomeTag2Icon", ddlHomeTag2Icon.SelectedValue);
        ConfigBLL.SetValue("HomeTag3Text", tag3Text);
        ConfigBLL.SetValue("HomeTag3Url", tag3Url);
        ConfigBLL.SetValue("HomeTag3Icon", ddlHomeTag3Icon.SelectedValue);

        CacheHelper.Clear();
        ShowSuccess("Đã lưu <b>3 nút gợi ý dưới ô tìm kiếm</b> cho trang chủ.");
        LoadConfigSettings();
    }

    protected void btnResetHomeTags_Click(object sender, EventArgs e)
    {
        ResetHomeTagsDefault();
        CacheHelper.Clear();
        ShowSuccess("Đã khôi phục mặc định <b>3 nút gợi ý dưới thanh tìm kiếm</b>.");
        LoadConfigSettings();
    }

    private string NormalizeTagText(string raw, string fallback, string label)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value)) value = fallback;
        if (value.Length > 40)
        {
            ShowError($"{HttpUtility.HtmlEncode(label)} không được dài quá 40 ký tự.");
            return null;
        }
        return value;
    }

    private string NormalizeInternalUrl(string raw, string fallback, string label)
    {
        string value = (raw ?? "").Trim();
        if (string.IsNullOrEmpty(value)) value = fallback;
        if (!value.StartsWith("/") || value.StartsWith("//") || value.IndexOf("javascript:", StringComparison.OrdinalIgnoreCase) >= 0 || Regex.IsMatch(value, @"\s"))
        {
            ShowError($"{HttpUtility.HtmlEncode(label)} phải là URL nội bộ hợp lệ, bắt đầu bằng dấu / và không chứa khoảng trắng.");
            return null;
        }
        return value;
    }

    // ── Lưu: Cho phép nhập ngày quá khứ ───────────────────────────────────
    protected void btnSaveAllowPastDates_Click(object sender, EventArgs e)
    {
        bool allowPast = chkAllowPastDates.Checked;
        ConfigBLL.SetValue("AllowPastDates", allowPast.ToString().ToLower());

        string msg = allowPast
            ? "Đã bật cho phép nhập ngày quá khứ. Admin có thể nhập hạn nộp/ngày bắt đầu bất kỳ."
            : "Đã tắt cho phép nhập ngày quá khứ. Hạn nộp phải từ hôm nay trở về sau.";
        ShowSuccess(msg);

        UpdateAllowPastDatesPreview();
    }

    // ── Reset về mặc định ────────────────────────────────────────────────
    protected void btnResetDefault_Click(object sender, EventArgs e)
    {
        ConfigBLL.SetValue("NamTuyenSinhHienTai", "2025");
        ConfigBLL.SetValue("SoTruongNoiBat",      "8");
        ConfigBLL.SetValue("SoBaiVietMoi",        "6");
        ConfigBLL.SetValue("SoTinMoi",            "10");
        ConfigBLL.SetValue("AllowPastDates",      "false");
        ResetHomeTagsDefault();
        ThongKeBLL.InvalidateCache();
        ShowSuccess("Đã <b>khôi phục tất cả cài đặt về mặc định</b>: Năm=2025, Trường=8, Bài viết=6, Tin=10, 3 nút gợi ý mặc định, Cho phép ngày quá khứ=Tắt.");
        LoadConfigSettings();
    }

    private void ResetHomeTagsDefault()
    {
        ConfigBLL.SetValue("HomeTag1Text", "Bách Khoa");
        ConfigBLL.SetValue("HomeTag1Url", "/TimKiemTruong.aspx?q=Bách%20Khoa");
        ConfigBLL.SetValue("HomeTag1Icon", "bi bi-fire text-warning");
        ConfigBLL.SetValue("HomeTag2Text", "Đại học Công Lập");
        ConfigBLL.SetValue("HomeTag2Url", "/TimKiemTruong.aspx?loai=1");
        ConfigBLL.SetValue("HomeTag2Icon", "bi bi-building");
        ConfigBLL.SetValue("HomeTag3Text", "Khối ngành IT");
        ConfigBLL.SetValue("HomeTag3Url", "/TimKiemTruong.aspx?q=CNTT");
        ConfigBLL.SetValue("HomeTag3Icon", "bi bi-laptop");
    }

    // ── Xóa toàn bộ Cache ─────────────────────────────────────────────────
    protected void btnClearCache_Click(object sender, EventArgs e)
    {
        CacheHelper.Clear();
        ShowSuccess("Đã <b>xóa toàn bộ Cache</b>. Hệ thống sẽ tải lại dữ liệu mới từ Database.");
    }

    // ── Helpers hiển thị thông báo ────────────────────────────────────────
    private void ShowSuccess(string msg)
    {
        litThongBao.Text =
            $"<div class='alert alert-success alert-dismissible fade show shadow-sm mb-4' role='alert'>" +
            $"<i class='bi bi-check-circle-fill me-2'></i>{msg}" +
            $"<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
    }

    private void ShowError(string msg)
    {
        litThongBao.Text =
            $"<div class='alert alert-danger alert-dismissible fade show shadow-sm mb-4' role='alert'>" +
            $"<i class='bi bi-exclamation-triangle-fill me-2'></i>{msg}" +
            $"<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
    }
}
