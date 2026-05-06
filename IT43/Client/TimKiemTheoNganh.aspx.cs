using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Tìm kiếm theo ngành — cho phép lọc theo danh mục ngành, ngành cụ thể,
/// năm tuyển sinh; hiển thị kết quả phân trang và hỗ trợ:
///   - Thêm trường vào Session["SoSanh"]   (nút SS trường)
///   - Thêm ngành vào Session["SoSanhNganh"] (nút SS ngành — mới)
/// Thông báo hiển thị qua Bootstrap Toast góc trên-phải (không còn litThongBao cuối trang).
/// </summary>
public partial class TimKiemTheoNganh_Page : Page
{
    private const int PAGE_SIZE = 20;

    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFilters();
            LoadData();
        }
    }

    private void LoadFilters()
    {
        foreach (var dm in DanhMucBLL.GetDanhMucNganh())
            ddlDanhMuc.Items.Add(new ListItem(dm.Ten, dm.Id.ToString()));

        LoadNganh(null);

        foreach (var y in DanhMucBLL.GetNamTuyenSinh())
            ddlNam.Items.Add(new ListItem(y.ToString(), y.ToString()));
    }

    private void LoadNganh(int? maDanhMuc)
    {
        ddlNganh.Items.Clear();
        ddlNganh.Items.Add(new ListItem("-- Tất cả ngành --", "0"));
        foreach (var n in DanhMucBLL.GetChuyenNganh(maDanhMuc))
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));
    }

    private void LoadData()
    {
        int? maNganh = null;
        if (int.TryParse(ddlNganh.SelectedValue, out int ng) && ng > 0) maNganh = ng;

        int? nam = null;
        if (int.TryParse(ddlNam.SelectedValue, out int n) && n > 0) nam = n;

        var paged = TimKiemTheoNganhDAL.TimKiem(maNganh, nam, CurrentPage, PAGE_SIZE);

        litTong.Text = paged.TongSo.ToString("N0");

        if (paged.TongSo == 0)
        {
            pnlEmpty.Visible  = true;
            pnlResult.Visible = false;
        }
        else
        {
            pnlEmpty.Visible  = false;
            pnlResult.Visible = true;
            gvKetQua.DataSource = paged.Data;
            gvKetQua.DataBind();
        }

        BindPaging(paged.TongSo);
    }

    private void BindPaging(int tongSo)
    {
        int total = (int)Math.Ceiling((double)tongSo / PAGE_SIZE);
        if (total <= 1) { rptPaging.DataSource = null; rptPaging.DataBind(); return; }

        const int WING = 2;
        var pages = new List<object>();

        Func<int, string, bool, bool, object> item =
            (idx, text, active, disabled) =>
                new { PageIndex = idx, PageText = text, IsActive = active, IsDisabled = disabled };

        pages.Add(item(Math.Max(0, CurrentPage - 1), "‹", false, CurrentPage == 0));

        int winStart = Math.Max(0, CurrentPage - WING);
        int winEnd   = Math.Min(total - 1, CurrentPage + WING);

        if (winStart > 1)  { pages.Add(item(0, "1", false, false)); pages.Add(item(-1, "…", false, true)); }
        else if (winStart == 1) pages.Add(item(0, "1", false, false));

        for (int i = winStart; i <= winEnd; i++)
            pages.Add(item(i, (i + 1).ToString(), i == CurrentPage, false));

        if (winEnd < total - 2)  { pages.Add(item(-1, "…", false, true)); pages.Add(item(total - 1, total.ToString(), false, false)); }
        else if (winEnd == total - 2) pages.Add(item(total - 1, total.ToString(), false, false));

        pages.Add(item(Math.Min(total - 1, CurrentPage + 1), "›", false, CurrentPage >= total - 1));

        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    protected void ddlDanhMuc_Changed(object sender, EventArgs e)
    {
        int? maDanhMuc = null;
        if (int.TryParse(ddlDanhMuc.SelectedValue, out int dm) && dm > 0) maDanhMuc = dm;
        LoadNganh(maDanhMuc);
        CurrentPage = 0;
        LoadData();
    }

    protected void btnTim_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        LoadData();
    }

    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            LoadData();
        }
    }

    /// <summary>
    /// Xử lý nút "+ So sánh" trên GridView:
    ///   "ThemSoSanhNganh" — thêm MaTin vào Session["SoSanhNganh"] (tối đa 4)
    /// Thông báo qua Bootstrap Toast.
    /// </summary>
    protected void gvKetQua_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName != "ThemSoSanhNganh") return;

        string soSanhNganhUrl = ResolveUrl("~/Client/SoSanhTruong.aspx?tab=nganh");

        if (!int.TryParse(e.CommandArgument?.ToString(), out int maTin)) return;

        if (!(Session["SoSanhNganh"] is List<int> ds)) ds = new List<int>();

        if (ds.Contains(maTin))
            ShowToast("Ngành này đã có trong danh sách so sánh.", "info");
        else if (ds.Count >= 4)
            ShowToast($"Đã đủ 4 ngành. <a href='{soSanhNganhUrl}' class='toast-link'>Xem so sánh ›</a>", "warning");
        else
        {
            ds.Add(maTin);
            Session["SoSanhNganh"] = ds;
            ShowToast($"Đã thêm vào so sánh ({ds.Count}/4). <a href='{soSanhNganhUrl}' class='toast-link'>Xem so sánh ›</a>", "success");
        }

        // Reload lại data để giữ kết quả tìm kiếm
        LoadData();
    }

    /// <summary>Hiển thị Bootstrap Toast góc trên-phải qua RegisterStartupScript.</summary>
    private void ShowToast(string htmlMessage, string type = "success")
    {
        string bgClass = $"text-bg-{type}";
        string script  = $@"(function(){{
            var el  = document.getElementById('toastSoSanh');
            var msg = document.getElementById('toastSoSanhMsg');
            if (!el || !msg) return;
            el.className = 'toast align-items-center border-0 {bgClass}';
            msg.innerHTML = {System.Web.HttpUtility.JavaScriptStringEncode(htmlMessage, addDoubleQuotes: true)};
            bootstrap.Toast.getOrCreateInstance(el).show();
        }})();";

        Page.ClientScript.RegisterStartupScript(
            GetType(), "toast_ss_" + DateTime.Now.Ticks, script, addScriptTags: true);
    }
}
