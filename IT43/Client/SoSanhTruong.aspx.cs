using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang So sánh v2 — hỗ trợ 2 tab:
///   Tab "truong": so sánh tối đa 3 trường theo tiêu chí cấp trường
///   Tab "nganh" : so sánh tối đa 4 ngành (tin tuyển sinh) song song
/// Tab đang active được lưu trong Session["SoSanhTab"] và HiddenField hfActiveTab.
/// </summary>
public partial class SoSanhTruong_Page : Page
{
    private const int MAX_TRUONG = 3;
    private const int MAX_NGANH  = SoSanhNganhBLL.MAX_NGANH; // = 4

    // ── Session helpers ────────────────────────────────────────────────────────

    private List<int> DanhSachSoSanh
    {
        get
        {
            if (Session["SoSanh"] is List<int> list) return list;
            var newList = new List<int>();
            Session["SoSanh"] = newList;
            return newList;
        }
    }

    private List<int> DanhSachSoSanhNganh
    {
        get
        {
            if (Session["SoSanhNganh"] is List<int> list) return list;
            var newList = new List<int>();
            Session["SoSanhNganh"] = newList;
            return newList;
        }
    }

    // ── Page lifecycle ─────────────────────────────────────────────────────────

    protected void Page_Load(object sender, EventArgs e)
    {
        // Đọc tab từ HiddenField (đã được JS cập nhật trước PostBack)
        // hoặc từ QueryString ?tab=nganh
        string tab = Request.QueryString["tab"] ?? "";
        if (!string.IsNullOrEmpty(tab))
            hfActiveTab.Value = tab == "nganh" ? "nganh" : "truong";

        if (!IsPostBack)
        {
            LoadDropDownTruong();
            LoadDropDownTruongChoNganh();
            LoadNganhCuaTruong(0);
        }
        else
        {
            // Khi PostBack từ __doPostBack(hfActiveTab, …) tab đã được JS cập nhật
            // hfActiveTab.Value đã được framework restore từ ViewState/postback value
        }

        RenderActiveTab();
        ApplyActiveTabCssClass();
    }

    /// <summary>
    /// Load dữ liệu cho cả 2 tab — panel luôn được render ra HTML,
    /// JS phía client sẽ ẩn/hiện bằng display:none (không cần PostBack chỉ để đổi tab).
    /// </summary>
    private void RenderActiveTab()
    {
        // Luôn load cả 2 để tránh lỗi "tab không truy cập được khi chưa có dữ liệu"
        LoadBangTruong();
        LoadBangNganh();

        // Cập nhật badge số đếm cả 2 tab
        litSoTruong.Text = DanhSachSoSanh.Count.ToString();
        litSoNganh.Text  = DanhSachSoSanhNganh.Count.ToString();
    }

    /// <summary>Set class active server-side để tab đúng hiển thị ngay cả trước khi JS chạy.</summary>
    private void ApplyActiveTabCssClass()
    {
        bool isNganh = hfActiveTab.Value == "nganh";
        pnlTabTruong.CssClass = isNganh ? "compare-tab-panel" : "compare-tab-panel active";
        pnlTabNganh.CssClass  = isNganh ? "compare-tab-panel active" : "compare-tab-panel";
    }

    // ── TAB TRƯỜNG ─────────────────────────────────────────────────────────────

    private void LoadDropDownTruong()
    {
        ddlChonTruong.Items.Clear();
        ddlChonTruong.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn trường để thêm --", "0"));
        foreach (var t in TruongBLL.GetDanhSachDropdown())
            ddlChonTruong.Items.Add(new System.Web.UI.WebControls.ListItem(t.Ten, t.Id.ToString()));
    }

    private void LoadBangTruong()
    {
        var ds = DanhSachSoSanh;
        litSoTruong.Text = ds.Count.ToString();

        if (ds.Count == 0)
        {
            pnlBang.Visible  = false;
            pnlEmpty.Visible = true;
            return;
        }

        pnlBang.Visible  = true;
        pnlEmpty.Visible = false;

        var data = SoSanhDAL.GetDanhSachSoSanh(ds);
        rptHeader.DataSource    = data; rptHeader.DataBind();
        rptLoai.DataSource      = data; rptLoai.DataBind();
        rptVung.DataSource      = data; rptVung.DataBind();
        rptKiemDinh.DataSource  = data; rptKiemDinh.DataBind();
        rptDanhGia.DataSource   = data; rptDanhGia.DataBind();
        rptWebsite.DataSource   = data; rptWebsite.DataBind();
        rptQuyMo.DataSource     = data; rptQuyMo.DataBind();
        rptDiemChuan.DataSource = data; rptDiemChuan.DataBind();
        rptSoNganh.DataSource   = data; rptSoNganh.DataBind();
        rptChiTiet.DataSource   = data; rptChiTiet.DataBind();
    }

    protected void btnThem_Click(object sender, EventArgs e)
    {
        hfActiveTab.Value = "truong";
        if (!int.TryParse(ddlChonTruong.SelectedValue, out int maTruong) || maTruong == 0)
        {
            ShowToast("Vui lòng chọn trường.", "warning");
            LoadBangTruong();
            return;
        }

        var ds = DanhSachSoSanh;
        if (ds.Contains(maTruong))
            ShowToast("Trường này đã có trong danh sách so sánh.", "info");
        else if (ds.Count >= MAX_TRUONG)
            ShowToast($"Chỉ có thể so sánh tối đa {MAX_TRUONG} trường. " +
                      $"<a href='#' class='toast-link' onclick=\"switchTab('truong');return false;\">Xem so sánh ›</a>", "warning");
        else
        {
            ds.Add(maTruong);
            Session["SoSanh"] = ds;
            ShowToast($"Đã thêm trường vào so sánh ({ds.Count}/{MAX_TRUONG}).", "success");
        }

        LoadBangTruong();
    }

    protected void rptHeader_Command(object sender, CommandEventArgs e)
    {
        hfActiveTab.Value = "truong";
        if (e.CommandName == "Xoa" && int.TryParse(e.CommandArgument?.ToString(), out int maTruong))
        {
            var ds = DanhSachSoSanh;
            ds.Remove(maTruong);
            Session["SoSanh"] = ds;
        }
        LoadBangTruong();
    }

    protected void btnXoaTatCaTruong_Click(object sender, EventArgs e)
    {
        hfActiveTab.Value = "truong";
        Session.Remove("SoSanh");
        LoadBangTruong();
    }

    // ── TAB NGÀNH ──────────────────────────────────────────────────────────────

    private void LoadDropDownTruongChoNganh()
    {
        ddlChonTruongNganh.Items.Clear();
        ddlChonTruongNganh.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn trường --", "0"));
        foreach (var t in TruongBLL.GetDanhSachDropdown())
            ddlChonTruongNganh.Items.Add(new System.Web.UI.WebControls.ListItem(t.Ten, t.Id.ToString()));
    }

    private void LoadNganhCuaTruong(int maTruong)
    {
        ddlChonNganhTin.Items.Clear();
        if (maTruong <= 0)
        {
            ddlChonNganhTin.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn trường trước --", "0"));
            return;
        }

        ddlChonNganhTin.Items.Add(new System.Web.UI.WebControls.ListItem("-- Chọn ngành & năm --", "0"));
        var dt = SoSanhNganhBLL.GetNganhCuaTruong(maTruong);
        foreach (DataRow r in dt.Rows)
        {
            string tenNganh = r["TenChuyenNganh"].ToString();
            int    nam      = DBHelper.Val<int>(r["NamTuyenSinh"]);
            object diem     = r["DiemChuanNamTruoc"];
            string diemStr  = diem == DBNull.Value ? "Thi riêng" : $"ĐC: {string.Format("{0:F2}", diem)}";
            string maTin    = r["MaTin"].ToString();

            ddlChonNganhTin.Items.Add(
                new System.Web.UI.WebControls.ListItem($"{tenNganh} — {nam}  ({diemStr})", maTin));
        }
    }

    protected void ddlChonTruongNganh_Changed(object sender, EventArgs e)
    {
        hfActiveTab.Value = "nganh";
        int.TryParse(ddlChonTruongNganh.SelectedValue, out int maTruong);
        LoadNganhCuaTruong(maTruong);
        LoadBangNganh();
    }

    protected void btnThemNganh_Click(object sender, EventArgs e)
    {
        hfActiveTab.Value = "nganh";

        // Phải đọc MaTin từ Request.Form TRƯỚC khi bind lại ddlChonNganhTin.
        // Nếu LoadNganhCuaTruong() chạy trước, SelectedValue sẽ bị reset về item "0".
        string postedMaTin = Request.Form[ddlChonNganhTin.UniqueID];
        if (!int.TryParse(postedMaTin, out int maTin) || maTin == 0)
        {
            ShowToast("Vui lòng chọn ngành.", "warning");
            LoadBangNganh();
            return;
        }

        // Restore dropdown ngành sau khi đã đọc MaTin để giao diện giữ đúng danh sách ngành.
        if (int.TryParse(ddlChonTruongNganh.SelectedValue, out int maTruongSel) && maTruongSel > 0)
            LoadNganhCuaTruong(maTruongSel);

        var ds = DanhSachSoSanhNganh;
        if (ds.Contains(maTin))
            ShowToast("Ngành này đã có trong danh sách so sánh.", "info");
        else if (ds.Count >= MAX_NGANH)
            ShowToast($"Chỉ có thể so sánh tối đa {MAX_NGANH} ngành.", "warning");
        else
        {
            ds.Add(maTin);
            Session["SoSanhNganh"] = ds;
            ShowToast($"Đã thêm ngành vào so sánh ({ds.Count}/{MAX_NGANH}).", "success");
        }

        LoadBangNganh();
    }

    protected void rptNganhHeader_Command(object sender, CommandEventArgs e)
    {
        hfActiveTab.Value = "nganh";
        if (e.CommandName == "XoaNganh" && int.TryParse(e.CommandArgument?.ToString(), out int maTin))
        {
            var ds = DanhSachSoSanhNganh;
            ds.Remove(maTin);
            Session["SoSanhNganh"] = ds;
        }
        LoadBangNganh();
    }

    protected void btnXoaTatCaNganh_Click(object sender, EventArgs e)
    {
        hfActiveTab.Value = "nganh";
        Session.Remove("SoSanhNganh");
        LoadBangNganh();
    }

    private void LoadBangNganh()
    {
        var ds = DanhSachSoSanhNganh;
        litSoNganh.Text = ds.Count.ToString();

        if (ds.Count == 0)
        {
            pnlBangNganh.Visible  = false;
            pnlEmptyNganh.Visible = true;
            return;
        }

        pnlBangNganh.Visible  = true;
        pnlEmptyNganh.Visible = false;

        var data = SoSanhNganhBLL.GetDanhSachSoSanh(ds);

        // Tính highlight điểm chuẩn: cao nhất = vàng, thấp nhất = đỏ nhạt
        data = AddDiemChuanHighlight(data);

        rptNganhHeader.DataSource       = data; rptNganhHeader.DataBind();
        rptNganhDanhMuc.DataSource      = data; rptNganhDanhMuc.DataBind();
        rptNganhTruong.DataSource       = data; rptNganhTruong.DataBind();
        rptNganhCapBac.DataSource       = data; rptNganhCapBac.DataBind();
        rptNganhPhuongThuc.DataSource   = data; rptNganhPhuongThuc.DataBind();
        rptNganhToHop.DataSource        = data; rptNganhToHop.DataBind();
        rptNganhDiemChuan.DataSource    = data; rptNganhDiemChuan.DataBind();
        rptNganhHocPhi.DataSource       = data; rptNganhHocPhi.DataBind();
        rptNganhChiTieu.DataSource      = data; rptNganhChiTieu.DataBind();
        rptNganhChiTietTruong.DataSource= data; rptNganhChiTietTruong.DataBind();
    }

    /// <summary>
    /// Thêm cột "DiemChuanCssClass" vào DataTable để tô màu:
    ///   cao nhất → "compare-highlight-best"
    ///   thấp nhất → "compare-highlight-low"
    ///   (chỉ khi có ít nhất 2 hàng có dữ liệu điểm)
    /// </summary>
    private static DataTable AddDiemChuanHighlight(DataTable dt)
    {
        if (!dt.Columns.Contains("DiemChuanCssClass"))
            dt.Columns.Add("DiemChuanCssClass", typeof(string));

        // Khởi tạo mặc định rỗng
        foreach (DataRow r in dt.Rows)
            r["DiemChuanCssClass"] = "";

        // Thu thập các giá trị hợp lệ
        double? maxVal = null, minVal = null;
        foreach (DataRow r in dt.Rows)
        {
            if (r["DiemChuanNamTruoc"] == DBNull.Value) continue;
            double v = Convert.ToDouble(r["DiemChuanNamTruoc"]);
            if (maxVal == null || v > maxVal) maxVal = v;
            if (minVal == null || v < minVal) minVal = v;
        }

        if (maxVal == null || maxVal == minVal) return dt; // không đủ để highlight

        foreach (DataRow r in dt.Rows)
        {
            if (r["DiemChuanNamTruoc"] == DBNull.Value) continue;
            double v = Convert.ToDouble(r["DiemChuanNamTruoc"]);
            if (v == maxVal) r["DiemChuanCssClass"] = "compare-highlight-best";
            else if (v == minVal) r["DiemChuanCssClass"] = "compare-highlight-low";
        }

        return dt;
    }

    // ── Toast helper ───────────────────────────────────────────────────────────

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
