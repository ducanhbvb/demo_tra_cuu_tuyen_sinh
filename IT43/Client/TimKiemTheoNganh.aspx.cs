using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Tìm kiếm theo ngành — cho phép lọc theo danh mục ngành, ngành cụ thể,
/// năm tuyển sinh; hiển thị kết quả phân trang và hỗ trợ thêm trường vào danh sách so sánh.
/// </summary>
public partial class TimKiemTheoNganh_Page : Page
{
    private const int PAGE_SIZE = 20;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: load bộ lọc và dữ liệu lần đầu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFilters();
            LoadData();
        }
    }

    /// <summary>Load danh mục ngành, ngành con và năm tuyển sinh vào DropDownList.</summary>
    private void LoadFilters()
    {
        // Nhóm ngành
        var dtDanhMuc = DBHelper.Query(
            "SELECT MaDanhMuc, TenDanhMuc FROM tbl_DanhMucNganh ORDER BY ThuTu, TenDanhMuc");
        foreach (DataRow r in dtDanhMuc.Rows)
            ddlDanhMuc.Items.Add(new ListItem(r["TenDanhMuc"].ToString(), r["MaDanhMuc"].ToString()));

        // Ngành
        LoadNganh(null);

        // Năm tuyển sinh
        var dtNam = DBHelper.Query(
            "SELECT DISTINCT NamTuyenSinh FROM tbl_TinTuyenSinh ORDER BY NamTuyenSinh DESC");
        foreach (DataRow r in dtNam.Rows)
            ddlNam.Items.Add(new ListItem(r["NamTuyenSinh"].ToString(), r["NamTuyenSinh"].ToString()));
    }

    /// <summary>Load danh sách ngành theo danh mục đã chọn (null = tất cả).</summary>
    private void LoadNganh(int? maDanhMuc)
    {
        ddlNganh.Items.Clear();
        ddlNganh.Items.Add(new ListItem("-- Tất cả ngành --", "0"));

        string sql = maDanhMuc.HasValue
            ? "SELECT MaChuyenNganh, TenChuyenNganh FROM tbl_ChuyenNganh WHERE MaDanhMuc=@dm ORDER BY TenChuyenNganh"
            : "SELECT MaChuyenNganh, TenChuyenNganh FROM tbl_ChuyenNganh ORDER BY TenChuyenNganh";

        var prms = maDanhMuc.HasValue
            ? new[] { new SqlParameter("@dm", maDanhMuc.Value) }
            : null;

        var dt = DBHelper.Query(sql, prms);
        foreach (DataRow r in dt.Rows)
            ddlNganh.Items.Add(new ListItem(
                r["TenChuyenNganh"].ToString(),
                r["MaChuyenNganh"].ToString()));
    }

    /// <summary>Truy vấn dữ liệu tìm kiếm theo ngành, năm và bind vào GridView + phân trang.</summary>
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

    /// <summary>Tạo danh sách nút phân trang dựa trên tổng số bản ghi.</summary>
    private void BindPaging(int tongSo)
    {
        int total = (int)Math.Ceiling((double)tongSo / PAGE_SIZE);
        var pages = new List<object>();
        for (int i = 0; i < total; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    /// <summary>Khi thay đổi nhóm ngành → reload ngành con và dữ liệu.</summary>
    protected void ddlDanhMuc_Changed(object sender, EventArgs e)
    {
        int? maDanhMuc = null;
        if (int.TryParse(ddlDanhMuc.SelectedValue, out int dm) && dm > 0) maDanhMuc = dm;
        LoadNganh(maDanhMuc);
        CurrentPage = 0;
        LoadData();
    }

    /// <summary>Xử lý nút Tìm kiếm — reset về trang đầu và tải lại dữ liệu.</summary>
    protected void btnTim_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
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

    /// <summary>
    /// Xử lý nút "Thêm so sánh" trên GridView — thêm MaTruong vào Session["SoSanh"]
    /// (tối đa 3 trường).
    /// </summary>
    protected void gvKetQua_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "ThemSoSanh" && int.TryParse(e.CommandArgument?.ToString(), out int maTruong))
        {
            if (!(Session["SoSanh"] is List<int> ds))
                ds = new List<int>();

            if (!ds.Contains(maTruong) && ds.Count < 3)
            {
                ds.Add(maTruong);
                Session["SoSanh"] = ds;
                litThongBao.Text = $"<div class='alert alert-success alert-dismissible mt-2 py-1 small fade show'>" +
                    $"Đã thêm vào danh sách so sánh ({ds.Count}/3). " +
                    $"<a href='{ResolveUrl("~/Client/SoSanhTruong.aspx")}' class='alert-link'>Xem so sánh</a></div>";
            }
            else if (ds.Contains(maTruong))
            {
                litThongBao.Text = "<div class='alert alert-info mt-2 py-1 small'>Trường này đã có trong danh sách so sánh.</div>";
            }
            else
            {
                litThongBao.Text = "<div class='alert alert-warning mt-2 py-1 small'>Đã đủ 3 trường. <a href='" +
                    ResolveUrl("~/Client/SoSanhTruong.aspx") + "' class='alert-link'>Xem so sánh</a></div>";
            }
        }
    }
}
