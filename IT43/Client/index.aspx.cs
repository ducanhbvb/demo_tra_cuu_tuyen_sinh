using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

/// <summary>
/// Trang chủ — hiển thị thống kê tổng quan, trường nổi bật,
/// bài viết mới nhất và tin tuyển sinh mới nhất.
/// </summary>
public partial class index_page : Page
{
    /// <summary>Khởi tạo trang chủ: load dữ liệu thống kê và danh sách nổi bật.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadData();
    }

    /// <summary>
    /// Load toàn bộ dữ liệu trang chủ: thống kê (số trường, ngành, tin),
    /// trường nổi bật (có kiểm định), bài viết mới và tin tuyển sinh mới.
    /// </summary>
    private void LoadData()
    {
        LoadHomeTags();

        // Thống kê — dùng ThongKeBLL
        litSoTruong.Text   = ThongKeBLL.SoTruong().ToString("N0");
        litSoNganh.Text    = ThongKeBLL.SoNganh().ToString("N0");
        litSoTin.Text      = ThongKeBLL.SoTinActive().ToString("N0");
        litNamMoiNhat.Text = ThongKeBLL.NamTuyenSinhMoiNhat().ToString();

        // Trường nổi bật — số lượng đọc từ Config (mặc định 8)
        int soTruong = ConfigBLL.GetInt("SoTruongNoiBat", 12);
        rptTruong.DataSource = ThongKeBLL.GetTruongNoiBat(soTruong);
        rptTruong.DataBind();

        // Bài viết mới nhất — số lượng đọc từ Config (mặc định 6)
        int soBaiViet = ConfigBLL.GetInt("SoBaiVietMoi", 9);
        var baiViet = BaiVietBLL.GetDanhSach(null, 0, soBaiViet, chiActive: true);
        AddPostSummaries(baiViet.Data);
        litNewsGroups.Text = BuildNewsGroupsHtml(baiViet.Data);

        // Tin tuyển sinh mới nhất — số lượng đọc từ Config (mặc định 10)
        int soTin = ConfigBLL.GetInt("SoTinMoi", 10);
        var paged = TinTuyenSinhDAL.TimKiem(null, null, null, null, null, null, null, 0, soTin);
        gvTinMoiNhat.DataSource = paged.Data;
        gvTinMoiNhat.DataBind();
    }

    private void LoadHomeTags()
    {
        var dt = new DataTable();
        dt.Columns.Add("Text");
        dt.Columns.Add("Url");
        dt.Columns.Add("Icon");

        AddHomeTag(dt, "HomeTag1Text", "HomeTag1Url", "HomeTag1Icon", "Bách Khoa", "/TimKiemTruong.aspx?q=Bách%20Khoa", "bi bi-fire text-warning");
        AddHomeTag(dt, "HomeTag2Text", "HomeTag2Url", "HomeTag2Icon", "Đại học Công Lập", "/TimKiemTruong.aspx?loai=1", "bi bi-building");
        AddHomeTag(dt, "HomeTag3Text", "HomeTag3Url", "HomeTag3Icon", "Khối ngành IT", "/TimKiemTruong.aspx?q=CNTT", "bi bi-laptop");

        rptHomeTags.DataSource = dt;
        rptHomeTags.DataBind();
    }

    private void AddHomeTag(DataTable dt, string textKey, string urlKey, string iconKey, string defaultText, string defaultUrl, string defaultIcon)
    {
        string text = HttpUtility.HtmlEncode(ConfigBLL.GetString(textKey, defaultText));
        string url = SafeInternalUrl(ConfigBLL.GetString(urlKey, defaultUrl), defaultUrl);
        string icon = SafeIconClass(ConfigBLL.GetString(iconKey, defaultIcon), defaultIcon);
        dt.Rows.Add(text, url, icon);
    }

    private string SafeInternalUrl(string url, string fallback)
    {
        if (string.IsNullOrWhiteSpace(url)) return fallback;
        url = url.Trim();
        if (!url.StartsWith("/") || url.StartsWith("//") || url.IndexOf("javascript:", StringComparison.OrdinalIgnoreCase) >= 0)
            return fallback;
        return HttpUtility.HtmlAttributeEncode(url);
    }

    private string SafeIconClass(string icon, string fallback)
    {
        if (string.IsNullOrWhiteSpace(icon)) return fallback;
        icon = icon.Trim();
        return Regex.IsMatch(icon, @"^[a-zA-Z0-9\-\s]+$") ? HttpUtility.HtmlAttributeEncode(icon) : fallback;
    }

    private string BuildNewsGroupsHtml(DataTable dt)
    {
        if (dt == null || dt.Rows.Count == 0) return "";

        var sb = new StringBuilder();
        const int groupSize = 4;
        int realPostCount = dt.Rows.Count;
        int groupCount = realPostCount >= groupSize
            ? (int)Math.Ceiling(realPostCount / (double)groupSize)
            : 1;

        for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
        {
            int start = groupIndex * groupSize;
            sb.Append("<div class=\"row g-4 js-news-group");
            if (groupIndex == 0) sb.Append(" is-active");
            sb.Append("\">");

            AppendFeaturedPost(sb, dt.Rows[start % realPostCount]);
            sb.Append("<div class=\"col-lg-6 d-flex flex-column gap-4\">");

            for (int offset = 1; offset < groupSize; offset++)
            {
                int rowIndex = start + offset;
                if (realPostCount >= groupSize || rowIndex < realPostCount)
                {
                    AppendSidePost(sb, dt.Rows[rowIndex % realPostCount]);
                }
                else
                {
                    AppendSidePostPlaceholder(sb);
                }
            }

            sb.Append("</div></div>");
        }
        return sb.ToString();
    }

    private void AppendFeaturedPost(StringBuilder sb, DataRow row)
    {
        string slug = Attr(row["Slug"]);
        string title = Text(row["TieuDe"]);
        string image = Attr(GetImageOrFallback(row["AnhChinh"]));
        string category = Text(row["TheLoai"]);
        string summary = Convert.ToString(row["TomTat"]);
        string date = FormatDate(row["NgayDang"]);
        string author = Text(row.Table.Columns.Contains("TenNguoiDangHienThi") && row["TenNguoiDangHienThi"] != DBNull.Value
            ? row["TenNguoiDangHienThi"]
            : "Ban Biên tập");

        sb.Append("<div class=\"col-lg-6\"><div class=\"premium-card h-100\">");
        sb.Append("<div class=\"card-img-wrap\" style=\"padding-top: 50%;\"><a href=\"/ChiTietBaiViet.aspx?slug=").Append(slug).Append("\">");
        sb.Append("<img src=\"").Append(image).Append("\" alt=\"").Append(title).Append("\" loading=\"lazy\" /></a>");
        sb.Append("<span class=\"card-badge bg-primary text-white\"><i class=\"bi bi-star-fill text-warning me-1\"></i>Mới nhất</span></div>");
        sb.Append("<div class=\"premium-card-body\" style=\"padding: 2rem;\">");
        if (!string.IsNullOrWhiteSpace(category)) sb.Append("<span class=\"text-primary fw-bold small text-uppercase tracking-wider mb-2 d-block\">").Append(category).Append("</span>");
        sb.Append("<h3 class=\"premium-card-title\" style=\"font-size: 1.8rem;\"><a href=\"/ChiTietBaiViet.aspx?slug=").Append(slug).Append("\">").Append(title).Append("</a></h3>");
        if (!string.IsNullOrWhiteSpace(summary)) sb.Append("<p class=\"post-excerpt\">").Append(summary).Append("</p>");
        sb.Append("<div class=\"mt-auto d-flex align-items-center justify-content-between border-top pt-3\"><div class=\"d-flex align-items-center gap-2\">");
        sb.Append("<img src=\"").Append(ResolveUrl("~/Resources/Images/no-image.png")).Append("\" width=\"28\" height=\"28\" alt=\"").Append(author).Append("\" />");
        sb.Append("<span class=\"fw-medium small\">").Append(author).Append("</span></div><span class=\"text-muted small\"><i class=\"bi bi-clock me-1\"></i>").Append(date).Append("</span></div>");
        sb.Append("</div></div></div>");
    }

    private void AppendSidePost(StringBuilder sb, DataRow row)
    {
        string slug = Attr(row["Slug"]);
        string title = Text(row["TieuDe"]);
        string image = Attr(GetImageOrFallback(row["AnhChinh"]));
        string category = Text(row["TheLoai"]);
        string summary = Convert.ToString(row["TomTat"]);
        string date = FormatDate(row["NgayDang"]);

        sb.Append("<div class=\"d-flex gap-3 align-items-center mb-0\">");
        sb.Append("<div style=\"width: 150px; height: 100px; border-radius: 12px; overflow: hidden; flex-shrink: 0; box-shadow: 0 4px 10px rgba(0,0,0,0.1);\">");
        sb.Append("<a href=\"/ChiTietBaiViet.aspx?slug=").Append(slug).Append("\"><img src=\"").Append(image).Append("\" style=\"width: 100%; height: 100%; object-fit: cover;\" alt=\"").Append(title).Append("\" loading=\"lazy\" /></a></div><div>");
        if (!string.IsNullOrWhiteSpace(category)) sb.Append("<span class=\"text-success fw-bold small text-uppercase mb-1 d-block\">").Append(category).Append("</span>");
        sb.Append("<h4 class=\"font-outfit fw-bold lh-base\" style=\"font-size: 1.1rem; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden;\"><a href=\"/ChiTietBaiViet.aspx?slug=").Append(slug).Append("\" class=\"text-body text-decoration-none hover-primary\">").Append(title).Append("</a></h4>");
        if (!string.IsNullOrWhiteSpace(summary)) sb.Append("<p class=\"news-list-excerpt\">").Append(summary).Append("</p>");
        sb.Append("<div class=\"mt-2 text-muted small\"><i class=\"bi bi-clock me-1\"></i>").Append(date).Append("</div></div></div>");
    }

    private void AppendSidePostPlaceholder(StringBuilder sb)
    {
        sb.Append("<div class=\"d-flex gap-3 align-items-center mb-0 opacity-25\">");
        sb.Append("<div style=\"width: 150px; height: 100px; border-radius: 12px; overflow: hidden; flex-shrink: 0; background:#e2e8f0;\"></div>");
        sb.Append("<div class=\"flex-grow-1\"><div style=\"height:18px;width:75%;background:#e2e8f0;border-radius:8px;margin-bottom:10px;\"></div>");
        sb.Append("<div style=\"height:14px;width:92%;background:#e2e8f0;border-radius:8px;margin-bottom:8px;\"></div>");
        sb.Append("<div style=\"height:12px;width:35%;background:#e2e8f0;border-radius:8px;\"></div></div></div>");
    }

    private string GetImageOrFallback(object value)
    {
        string image = Convert.ToString(value);
        return string.IsNullOrWhiteSpace(image) ? "/Resources/Images/no-image.png" : image;
    }

    private string FormatDate(object value)
    {
        return value is DateTime dt ? dt.ToString("dd/MM/yyyy") : "";
    }

    private string Text(object value) => HttpUtility.HtmlEncode(Convert.ToString(value) ?? "");

    private string Attr(object value) => HttpUtility.HtmlAttributeEncode(Convert.ToString(value) ?? "");

    private void AddPostSummaries(DataTable dt)
    {
        if (dt == null) return;
        if (!dt.Columns.Contains("TomTat")) dt.Columns.Add("TomTat", typeof(string));
        foreach (DataRow row in dt.Rows)
        {
            string raw = dt.Columns.Contains("NoiDung") && row["NoiDung"] != DBNull.Value ? row["NoiDung"].ToString() : "";
            row["TomTat"] = BuildSummary(raw, 170);
        }
    }

    private string BuildSummary(string html, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(html)) return "";
        string text = Regex.Replace(html, "<.*?>", " ");
        text = HttpUtility.HtmlDecode(text);
        text = Regex.Replace(text ?? "", @"\s+", " ").Trim();
        if (text.Length <= maxLength) return HttpUtility.HtmlEncode(text);
        return HttpUtility.HtmlEncode(text.Substring(0, maxLength).TrimEnd()) + "...";
    }

    protected string GetLoaiTruongText(object value)
    {
        string v = Convert.ToString(value);
        return v == "1" ? "Công lập" : v == "2" ? "Tư thục" : v == "3" ? "Quốc tế" : "Khác";
    }

    protected string GetLoaiTruongBadgeClass(object value)
    {
        string v = Convert.ToString(value);
        return v == "1" ? "badge-public" : v == "2" ? "badge-private" : v == "3" ? "badge-international" : "badge-level-university";
    }

    protected string GetCapBacText(object value)
    {
        string v = Convert.ToString(value);
        return v == "1" ? "Đại học" : v == "2" ? "Cao Đẳng" : v == "3" ? "Trường nghề" : "";
    }

    protected string GetCapBacBadgeClass(object value)
    {
        string v = Convert.ToString(value);
        return v == "1" ? "badge-level-university" : v == "2" ? "badge-level-college" : v == "3" ? "badge-level-vocational" : "badge-level-university";
    }

    /// <summary>Xử lý ô tìm kiếm nhanh trên trang chủ — redirect sang trang tìm kiếm trường.</summary>
    protected void btnTimKiem_Click(object sender, EventArgs e)
    {
        string kw = Server.UrlEncode(txtTimKiem.Text.Trim());
        Response.Redirect($"TimKiemTruong.aspx?q={kw}");
    }
}
