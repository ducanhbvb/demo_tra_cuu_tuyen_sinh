using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Danh sách bài viết — tìm kiếm, bài nổi bật, grid + sidebar, phân trang.
/// </summary>
public partial class BaiViet_Page : Page
{
    private const int PAGE_SIZE = 9;

    /// <summary>Trang hiện tại (lưu ViewState để persist qua PostBack).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: load bộ lọc trường, bài nổi bật, sidebar và dữ liệu chính.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadTruongFilter();
            LoadNoiBat();
            LoadSidebar();
            LoadData();
        }
    }

    // ── Event handlers ─────────────────────────────────────────────────────

    /// <summary>Nút Tìm kiếm — reset trang và tìm theo từ khóa + trường.</summary>
    protected void btnTimKiem_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        LoadData();
    }

    /// <summary>Nút "Xem tất cả" — xóa bộ lọc và hiển thị toàn bộ bài viết.</summary>
    protected void btnXemTat_Click(object sender, EventArgs e)
    {
        txtTimKiem.Text = "";
        ddlTruong.SelectedIndex = 0;
        CurrentPage = 0;
        LoadNoiBat();
        LoadData();
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            LoadData();
        }
    }

    // ── Data loading ───────────────────────────────────────────────────────

    /// <summary>Load danh sách trường vào DropDownList bộ lọc.</summary>
    private void LoadTruongFilter()
    {
        foreach (var t in TruongBLL.GetDanhSachDropdown())
            ddlTruong.Items.Add(new ListItem(t.Ten, t.Id.ToString()));
    }

    /// <summary>Load bài viết nổi bật (1 bài, lượt xem cao nhất).</summary>
    private void LoadNoiBat()
    {
        var dt = BaiVietBLL.GetBaiVietNoiBat();
        if (dt != null && dt.Rows.Count > 0)
        {
            pnlNoiBat.Visible = true;
            rptNoiBat.DataSource = dt;
            rptNoiBat.DataBind();
        }
        else
        {
            pnlNoiBat.Visible = false;
        }
    }

    /// <summary>Load sidebar: bài mới nhất + bài xem nhiều nhất.</summary>
    private void LoadSidebar()
    {
        var moiNhat = BaiVietBLL.GetMoiNhat(5);
        rptMoiNhat.DataSource = moiNhat;
        rptMoiNhat.DataBind();

        var xemNhieu = BaiVietBLL.GetXemNhieuNhat(5);
        rptXemNhieu.DataSource = xemNhieu;
        rptXemNhieu.DataBind();
    }

    /// <summary>Truy vấn bài viết theo bộ lọc (từ khóa + trường) và bind vào Repeater + phân trang.</summary>
    private void LoadData()
    {
        string tuKhoa = txtTimKiem.Text.Trim();
        int? maTruong = null;
        if (int.TryParse(ddlTruong.SelectedValue, out int mt) && mt > 0)
            maTruong = mt;

        // Nếu có từ khóa hoặc filter trường → ẩn featured
        bool hasFilter = !string.IsNullOrEmpty(tuKhoa) || maTruong.HasValue;
        if (hasFilter)
            pnlNoiBat.Visible = false;

        PagedTable paged;
        if (!string.IsNullOrEmpty(tuKhoa) || maTruong.HasValue)
            paged = BaiVietBLL.SearchBaiViet(tuKhoa, maTruong, CurrentPage, PAGE_SIZE);
        else
            paged = BaiVietBLL.GetDanhSach(null, CurrentPage, PAGE_SIZE, chiActive: true);

        if (paged.TongSo == 0)
        {
            rptBaiViet.DataSource = null;
            rptBaiViet.DataBind();
            pnlEmpty.Visible = true;
        }
        else
        {
            pnlEmpty.Visible = false;
            rptBaiViet.DataSource = paged.Data;
            rptBaiViet.DataBind();
        }

        litTong.Text = paged.TongSo.ToString("N0");
        BindPaging(paged.TongSo);
    }

    /// <summary>Tạo danh sách nút phân trang dựa trên tổng số bài viết.</summary>
    private void BindPaging(int tongSo)
    {
        int totalPages = (int)Math.Ceiling((double)tongSo / PAGE_SIZE);
        var pages = new List<object>();
        for (int i = 0; i < totalPages; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });

        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    // ── Helper ─────────────────────────────────────────────────────────────

    /// <summary>Cắt nội dung HTML thành plain text, giới hạn ký tự + thêm "..."</summary>
    protected static string TruncateText(object raw, int maxLength)
    {
        if (raw == null || raw == DBNull.Value) return "";
        string text = Regex.Replace(raw.ToString(), "<.*?>", " ");  // strip HTML
        text = Regex.Replace(text, @"\s+", " ").Trim();              // collapse whitespace
        if (text.Length <= maxLength) return System.Web.HttpUtility.HtmlEncode(text);
        return System.Web.HttpUtility.HtmlEncode(text.Substring(0, maxLength).TrimEnd()) + "...";
    }

    protected string FormatDate(object value)
    {
        return value != DBNull.Value && value != null
            ? ((DateTime)value).ToString("dd/MM/yyyy")
            : "";
    }

    protected bool SafeGetBool(object value)
    {
        return value != DBNull.Value && value != null && (bool)value;
    }

    protected string FormatInt(object value, string format = "N0")
    {
        return value != DBNull.Value && value != null
            ? Convert.ToInt64(value).ToString(format)
            : "";
    }
}
