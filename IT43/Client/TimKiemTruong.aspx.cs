using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Tìm kiếm trường — lọc theo tên, tỉnh/thành, loại trường, ngành đào tạo;
/// hiển thị danh sách trường dạng card với phân trang.
/// </summary>
public partial class TimKiemTruong : Page
{
    private const int PAGE_SIZE = 12;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage
    {
        get => ViewState["Page"] is int p ? p : 0;
        set => ViewState["Page"] = value;
    }

    /// <summary>Khởi tạo trang: load dropdown, đọc QueryString bộ lọc và bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropdowns();
            // Lấy param từ URL
            txtTen.Text = Request.QueryString["q"] ?? "";
            if (byte.TryParse(Request.QueryString["loai"], out byte loai))
                ddlLoai.SelectedValue = loai.ToString();
            if (!string.IsNullOrEmpty(Request.QueryString["tinh"]))
                ddlTinh.SelectedValue = Request.QueryString["tinh"];
            if (int.TryParse(Request.QueryString["nganh"], out int nganh))
                ddlNganh.SelectedValue = nganh.ToString();
            if (decimal.TryParse(Request.QueryString["diemMin"], out decimal dmin))
                txtDiemMin.Text = dmin.ToString("F1");
            if (decimal.TryParse(Request.QueryString["diemMax"], out decimal dmax))
                txtDiemMax.Text = dmax.ToString("F1");
            if (short.TryParse(Request.QueryString["nam"], out short nam))
                ddlNam.SelectedValue = nam.ToString();
            if (!string.IsNullOrEmpty(Request.QueryString["sortBy"]))
                ddlSort.SelectedValue = Request.QueryString["sortBy"];
            if (!string.IsNullOrEmpty(Request.QueryString["sortDir"]))
                ddlSortDir.SelectedValue = Request.QueryString["sortDir"];
            CurrentPage = 0;
            BindData();
        }
    }

    /// <summary>Load danh sách tỉnh/thành và ngành đào tạo vào DropDownList.</summary>
    private void LoadDropdowns()
    {
        foreach (var t in DanhMucDAL.GetTinhThanh())
            ddlTinh.Items.Add(new ListItem(t, t));

        foreach (var n in DanhMucDAL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));

        foreach (var y in DanhMucBLL.GetNamTuyenSinh())
            ddlNam.Items.Add(new ListItem(y.ToString(), y.ToString()));

        // Sort options
        ddlSort.Items.Add(new ListItem("Tên trường", "TenTruong"));
        ddlSort.Items.Add(new ListItem("Tỉnh thành", "TinhThanh"));
        // Có thể thêm "Điểm chuẩn" nếu muốn, nhưng cần handle trong SP vì điểm nằm trong tbl_TinTuyenSinh
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý nút Reset — xóa tất cả bộ lọc và tải lại toàn bộ dữ liệu.</summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        txtTen.Text = "";
        ddlTinh.SelectedIndex = 0;
        ddlLoai.SelectedIndex = 0;
        ddlCapBac.SelectedIndex = 0;
        ddlNganh.SelectedIndex = 0;
        ddlNam.SelectedIndex = 0;
        txtDiemMin.Text = "";
        txtDiemMax.Text = "";
        ddlSort.SelectedIndex = 0;
        ddlSortDir.SelectedIndex = 0;
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page")
        {
            CurrentPage = int.Parse(e.CommandArgument.ToString());
            BindData();
        }
    }

    /// <summary>
    /// Gọi TruongBLL.TimKiem với các bộ lọc, bind kết quả vào Repeater
    /// và tạo danh sách nút phân trang.
    /// </summary>
    private void BindData()
    {
        byte? loai       = byte.TryParse(ddlLoai.SelectedValue,   out byte l)  && l  > 0 ? l  : (byte?)null;
        byte? capBac     = byte.TryParse(ddlCapBac.SelectedValue,  out byte cb) && cb > 0 ? cb : (byte?)null;
        int?  nganh      = int.TryParse(ddlNganh.SelectedValue,   out int n)   && n  > 0 ? n  : (int?)null;
        short? nam       = short.TryParse(ddlNam.SelectedValue,   out short ny) && ny > 0 ? ny : (short?)null;
        decimal? diemMin = decimal.TryParse(txtDiemMin.Text.Trim(), out decimal dm) ? dm : (decimal?)null;
        decimal? diemMax = decimal.TryParse(txtDiemMax.Text.Trim(), out decimal dx) ? dx : (decimal?)null;

        string sortBy  = string.IsNullOrEmpty(ddlSort.SelectedValue)    ? "TenTruong" : ddlSort.SelectedValue;
        string sortDir = string.IsNullOrEmpty(ddlSortDir.SelectedValue) ? "ASC"       : ddlSortDir.SelectedValue;

        // Phía client: chỉ hiển thị trường đang active (TrangThai=1)
        var result = TruongBLL.TimKiem(
            txtTen.Text.Trim(),
            string.IsNullOrEmpty(ddlTinh.SelectedValue) ? null : ddlTinh.SelectedValue,
            null, // maVung - chưa dùng
            loai,
            nganh,
            nam,
            diemMin,
            diemMax,
            sortBy,
            sortDir,
            CurrentPage,
            PAGE_SIZE,
            trangThai:    true,
            capBacDaoTao: capBac);

        litTong.Text = result.TongSo.ToString("N0");
        rptKetQua.DataSource = result.Data;
        rptKetQua.DataBind();

        // Phân trang
        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    protected string GetCapBacBadge(object value)
    {
        string v = Convert.ToString(value);
        string text = v == "1" ? "Đại học" : v == "2" ? "Cao Đẳng" : v == "3" ? "Trường nghề" : "";
        if (string.IsNullOrEmpty(text)) return "";
        string css = v == "1" ? "school-level-university" : v == "2" ? "school-level-college" : "school-level-vocational";
        return $"<span class='school-level-badge {css}'>{text}</span>";
    }

    /// <summary>
    /// Xử lý nút "So sánh" trên card trường — thêm MaTruong vào Session["SoSanh"] (tối đa 3).
    /// Thông báo qua Bootstrap Toast góc trên-phải (dùng ScriptManager vì trang có UpdatePanel).
    /// </summary>
    protected void rptKetQua_Command(object sender, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "ThemSoSanh") return;
        if (!int.TryParse(e.CommandArgument?.ToString(), out int maTruong)) return;

        string soSanhUrl = ResolveUrl("~/Client/SoSanhTruong.aspx");

        if (!(Session["SoSanh"] is List<int> ds)) ds = new List<int>();

        if (ds.Contains(maTruong))
            ShowToast("Trường này đã có trong danh sách so sánh.", "info");
        else if (ds.Count >= 3)
            ShowToast($"Đã đủ 3 trường. <a href='{soSanhUrl}' class='toast-link'>Xem so sánh ›</a>", "warning");
        else
        {
            ds.Add(maTruong);
            Session["SoSanh"] = ds;
            ShowToast($"Đã thêm vào so sánh ({ds.Count}/3). <a href='{soSanhUrl}' class='toast-link'>Xem so sánh ›</a>", "success");
        }

        BindData();
    }

    /// <summary>
    /// Hiển thị Bootstrap Toast góc trên-phải qua ScriptManager.RegisterStartupScript.
    /// Toast container đặt ngoài UpdatePanel trong ASPX nên script kích hoạt được sau partial PostBack.
    /// </summary>
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

        ScriptManager.RegisterStartupScript(
            this, GetType(), "toast_ss_" + DateTime.Now.Ticks, script, addScriptTags: true);
    }
}
