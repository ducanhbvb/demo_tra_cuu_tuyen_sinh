using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Tra cứu điểm chuẩn — lọc theo tỉnh/thành, ngành, phương thức,
/// năm tuyển sinh và khoảng điểm; hiển thị kết quả phân trang.
/// </summary>
public partial class TraCuuDiemChuan : Page
{
    private const int PAGE_SIZE = 20;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage
    {
        get => ViewState["Page"] is int p ? p : 0;
        set => ViewState["Page"] = value;
    }

    /// <summary>Khởi tạo trang: load dropdown bộ lọc và bind dữ liệu lần đầu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropdowns();
            BindData();
        }
    }

    /// <summary>Load tỉnh/thành, ngành, phương thức tuyển sinh và năm vào DropDownList.</summary>
    private void LoadDropdowns()
    {
        foreach (var t in DanhMucDAL.GetTinhThanh())
            ddlTinh.Items.Add(new ListItem(t, t));

        foreach (var n in DanhMucDAL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));

        foreach (var p in DanhMucDAL.GetPhuongThuc())
            ddlPhuongThuc.Items.Add(new ListItem(p.Ten, p.Id.ToString()));

        foreach (var y in DanhMucDAL.GetNamTuyenSinh())
            ddlNam.Items.Add(new ListItem(y.ToString(), y.ToString()));
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>Xử lý nút Reset — xóa tất cả bộ lọc về mặc định.</summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        ddlTinh.SelectedIndex = ddlNganh.SelectedIndex =
            ddlPhuongThuc.SelectedIndex = ddlNam.SelectedIndex = 0;
        txtDiemTu.Text = txtDiemDen.Text = "";
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    /// <summary>
    /// Gọi TinTuyenSinhBLL.TimKiem với bộ lọc nâng cao (ngành, phương thức, năm, khoảng điểm, tỉnh),
    /// bind kết quả vào GridView và tạo nút phân trang.
    /// </summary>
    private void BindData()
    {
        int?    nganh = int.TryParse(ddlNganh.SelectedValue, out int n)    ? n : (int?)null;
        int?    pt    = int.TryParse(ddlPhuongThuc.SelectedValue, out int p) ? p : (int?)null;
        short?  nam   = short.TryParse(ddlNam.SelectedValue, out short y) ? y : (short?)null;
        decimal? tu   = decimal.TryParse(txtDiemTu.Text, out decimal dt)   ? dt : (decimal?)null;
        decimal? den  = decimal.TryParse(txtDiemDen.Text, out decimal dd)  ? dd : (decimal?)null;

        var result = TinTuyenSinhBLL.TimKiem(null, nganh, pt, nam, tu, den,
            ddlTinh.SelectedValue, CurrentPage, PAGE_SIZE);

        litTong.Text = result.TongSo.ToString("N0");
        gvKetQua.DataSource = result.Data;
        gvKetQua.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
