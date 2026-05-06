using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý Logs (Admin) — hiển thị lịch sử hoạt động hệ thống
/// với bộ lọc, thống kê, phân trang và xuất CSV.
/// </summary>
public partial class Admin_QuanLyLogs : Page
{
    private const int PAGE_SIZE = 30;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage
    {
        get => ViewState["Page"] is int p ? p : 0;
        set => ViewState["Page"] = value;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Chỉ Admin được xem logs hệ thống
        if (!SecurityHelper.IsAdmin())
        {
            Response.Redirect("~/Admin/Default.aspx");
            return;
        }

        if (!IsPostBack)
        {
            // Render icon vào nút xuất CSV ở page header
            btnExportCsv.Text = "⬇ Xuất CSV";
            LoadData();
        }
        else
        {
            // EnableViewState="false" → rebind để LinkButton trong GridView tồn tại trước RowCommand
            LoadData();
        }
    }

    /// <summary>
    /// Thu thập bộ lọc từ UI, gọi LogsBLL.GetDanhSach, bind GridView và phân trang.
    /// </summary>
    private void LoadData()
    {
        bool? ok = null;
        if (!string.IsNullOrEmpty(ddlIsSuccess.SelectedValue))
            ok = ddlIsSuccess.SelectedValue == "1";

        var result = LogsBLL.GetDanhSach(
            hanhDong:    ddlHanhDong.SelectedValue,
            isSuccess:   ok,
            loaiThietBi: ddlThietBi.SelectedValue,
            email:       txtEmail.Text.Trim(),
            pageIndex:   CurrentPage,
            pageSize:    PAGE_SIZE);

        // Stat cards
        litTong.Text      = result.TongSo.ToString("N0");
        litThanhCong.Text = result.ThanhCong.ToString("N0");
        litThatBai.Text   = result.ThatBai.ToString("N0");

        // Table
        gvLogs.DataSource = result.Data;
        gvLogs.DataBind();

        // Paging info
        int totalPages = Math.Max(1, result.TongTrang);
        litCurrentPage.Text = (CurrentPage + 1).ToString();
        litTotalPages.Text  = totalPages.ToString();
        litTongPaging.Text  = result.TongSo.ToString("N0");

        BindPaging(totalPages);
    }

    /// <summary>Tạo danh sách nút phân trang (tối đa 10 nút, có "..." rút gọn).</summary>
    private void BindPaging(int tongTrang)
    {
        var pages = new List<object>();
        int start = Math.Max(0, CurrentPage - 4);
        int end   = Math.Min(tongTrang - 1, start + 9);
        start = Math.Max(0, end - 9);

        if (start > 0)
            pages.Add(new { PageIndex = 0, PageText = "1", IsActive = false });
        if (start > 1)
            pages.Add(new { PageIndex = -1, PageText = "…", IsActive = false });

        for (int i = start; i <= end; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });

        if (end < tongTrang - 2)
            pages.Add(new { PageIndex = -1, PageText = "…", IsActive = false });
        if (end < tongTrang - 1)
            pages.Add(new { PageIndex = tongTrang - 1, PageText = tongTrang.ToString(), IsActive = false });

        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    // ── Button handlers ───────────────────────────────────────────────────

    protected void btnLoc_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        LoadData();
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlHanhDong.SelectedIndex  = 0;
        ddlIsSuccess.SelectedIndex = 0;
        ddlThietBi.SelectedIndex   = 0;
        txtEmail.Text = "";
        CurrentPage = 0;
        LoadData();
    }

    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" &&
            int.TryParse(e.CommandArgument?.ToString(), out int pg) && pg >= 0)
        {
            CurrentPage = pg;
            LoadData();
        }
    }

    // ── CSV Export ────────────────────────────────────────────────────────

    /// <summary>
    /// Xuất toàn bộ logs (theo bộ lọc hiện tại) ra file CSV UTF-8 BOM
    /// để Excel đọc được tiếng Việt.
    /// </summary>
    protected void btnExportCsv_Click(object sender, EventArgs e)
    {
        bool? ok = null;
        if (!string.IsNullOrEmpty(ddlIsSuccess.SelectedValue))
            ok = ddlIsSuccess.SelectedValue == "1";

        // Lấy toàn bộ (pageSize lớn, không phân trang)
        var result = LogsBLL.GetDanhSach(
            hanhDong:    ddlHanhDong.SelectedValue,
            isSuccess:   ok,
            loaiThietBi: ddlThietBi.SelectedValue,
            email:       txtEmail.Text.Trim(),
            pageIndex:   0,
            pageSize:    100000);

        var sb = new StringBuilder();

        // Header row
        sb.AppendLine("ID,Thời gian,Email,Hành động,Kết quả,IP,Thiết bị,Mô tả,Mã lỗi");

        if (result.Data != null)
        {
            foreach (DataRow row in result.Data.Rows)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                    CsvCell(row["LogID"]),
                    CsvCell(row["ThoiGian"] is DateTime dt ? dt.ToString("dd/MM/yyyy HH:mm:ss") : ""),
                    CsvCell(row["Email"]),
                    CsvCell(row["HanhDong"]),
                    CsvCell(row["IsSuccess"] != DBNull.Value && (bool)row["IsSuccess"] ? "Thành công" : "Thất bại"),
                    CsvCell(row["IPAddress"]),
                    CsvCell(row["LoaiThietBi"]),
                    CsvCell(row["MoTa"]),
                    CsvCell(row["MaLoi"])
                }));
            }
        }

        string fileName = "logs_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";

        Response.Clear();
        Response.ContentType    = "text/csv";
        Response.Charset        = "UTF-8";
        Response.ContentEncoding = Encoding.UTF8;
        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");

        // UTF-8 BOM — giúp Excel nhận diện encoding
        Response.BinaryWrite(Encoding.UTF8.GetPreamble());
        Response.Write(sb.ToString());
        Response.Flush();
        Response.End();
    }

    /// <summary>Bọc giá trị vào ô CSV (escape dấu nháy kép, bọc trong nháy kép nếu cần).</summary>
    private static string CsvCell(object val)
    {
        if (val == null || val == DBNull.Value) return "";
        string s = val.ToString().Replace("\"", "\"\"");
        // Bọc nháy kép nếu có dấu phẩy, dấu xuống dòng hoặc nháy kép
        if (s.Contains(",") || s.Contains("\n") || s.Contains("\""))
            return "\"" + s + "\"";
        return s;
    }

    // ── UI Helpers ────────────────────────────────────────────────────────

    protected string FormatEmail(object val)
    {
        if (val == null || val == DBNull.Value || string.IsNullOrEmpty(val.ToString()))
            return "<span class='text-muted fst-italic'>Ẩn danh</span>";
        return HttpUtility.HtmlEncode(val.ToString());
    }

    protected string GetEmailColor(object val)
    {
        if (val == null || val == DBNull.Value || string.IsNullOrEmpty(val.ToString()))
            return "#94a3b8";
        string[] colors = { "#4f46e5","#10b981","#f59e0b","#06b6d4","#8b5cf6","#ef4444","#14b8a6","#f97316" };
        return colors[Math.Abs(val.ToString().GetHashCode()) % colors.Length];
    }

    protected string GetActionLabel(string hanhDong)
    {
        if (string.IsNullOrEmpty(hanhDong)) return "—";
        switch (hanhDong.ToUpper())
        {
            case "DANG_NHAP":         return "Đăng nhập";
            case "DOI_MAT_KHAU":      return "Đổi mật khẩu";
            case "UPGRADE_HASH":      return "Nâng cấp hash";
            case "APPLICATION_ERROR": return "Lỗi hệ thống";
            case "SEND_EMAIL_FAIL":   return "Lỗi gửi email";
            default: return hanhDong.Replace("_", " ");
        }
    }

    protected string GetActionBadgeClass(string hanhDong)
    {
        if (string.IsNullOrEmpty(hanhDong)) return "badge-soft-secondary";
        switch (hanhDong.ToUpper())
        {
            case "DANG_NHAP":         return "badge-soft-primary";
            case "DOI_MAT_KHAU":      return "badge-soft-warning";
            case "UPGRADE_HASH":      return "badge-soft-primary";
            case "APPLICATION_ERROR": return "badge-soft-danger";
            case "SEND_EMAIL_FAIL":   return "badge-soft-danger";
            default:                  return "badge-soft-secondary";
        }
    }

    protected string GetDeviceBadgeClass(string device)
    {
        switch ((device ?? "").ToLower())
        {
            case "mobile":  return "badge-soft-success";
            case "tablet":  return "badge-soft-warning";
            default:        return "badge-soft-secondary";
        }
    }

    protected string GetDeviceIcon(string device)
    {
        switch ((device ?? "").ToLower())
        {
            case "mobile":  return "bi-phone";
            case "tablet":  return "bi-tablet";
            default:        return "bi-display";
        }
    }
}
