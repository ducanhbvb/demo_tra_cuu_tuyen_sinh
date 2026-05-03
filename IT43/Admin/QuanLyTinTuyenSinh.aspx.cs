using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý tin tuyển sinh (Admin) — CRUD tin tuyển sinh với modal form,
/// bộ lọc (trường, ngành, năm), phân trang, toggle trạng thái hiển thị.
/// </summary>
public partial class Admin_QuanLyTinTuyenSinh : Page
{
    private const int PAGE_SIZE = 20;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Flag điều khiển hiển thị modal Bootstrap từ server-side.</summary>
    public string ShowModal { get; private set; } = "false";

    /// <summary>Khởi tạo trang: load dropdown bộ lọc + modal và bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadDropdowns();
            BindData();
        }
    }

    /// <summary>Load dropdown cho cả filter bar và modal form (trường, ngành, năm, phương thức).</summary>
    private void LoadDropdowns()
    {
        // Filter
        var truongs = DBHelper.Query("SELECT MaTruong, TenTruong FROM tbl_Truong ORDER BY TenTruong");
        foreach (DataRow r in truongs.Rows)
            ddlTruong.Items.Add(new ListItem(r["TenTruong"].ToString(), r["MaTruong"].ToString()));

        foreach (var n in DanhMucDAL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));

        foreach (var y in DanhMucDAL.GetNamTuyenSinh())
            ddlNam.Items.Add(new ListItem(y.ToString(), y.ToString()));

        // Modal
        foreach (DataRow r in truongs.Rows)
            ddlMTruong.Items.Add(new ListItem(r["TenTruong"].ToString(), r["MaTruong"].ToString()));

        foreach (var n in DanhMucDAL.GetChuyenNganh())
            ddlMNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));

        foreach (var pt in DanhMucDAL.GetPhuongThuc())
            ddlMPhuongThuc.Items.Add(new ListItem(pt.Ten, pt.Id.ToString()));
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>
    /// Xử lý command trên GridView: XoaTin, ToggleTin (ẩn/hiện),
    /// SuaTin (load dữ liệu vào modal form).
    /// </summary>
    protected void gvTin_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int maTin = int.Parse(e.CommandArgument.ToString());

        if (e.CommandName == "XoaTin")
        {
            TinTuyenSinhBLL.Xoa(maTin);
            litThongBao.Text = "<div class='alert alert-success'>Đã xóa.</div>";
            BindData();
        }
        else if (e.CommandName == "ToggleTin")
        {
            TinTuyenSinhBLL.ToggleTrangThai(maTin);
            litThongBao.Text = "<div class='alert alert-success'>Đã cập nhật trạng thái.</div>";
            BindData();
        }
        else if (e.CommandName == "SuaTin")
        {
            var m = TinTuyenSinhDAL.GetById(maTin);
            if (m == null) return;
            hfMaTin.Value         = maTin.ToString();
            ddlMTruong.SelectedValue    = m.MaTruong.ToString();
            ddlMNganh.SelectedValue     = m.MaChuyenNganh.ToString();
            ddlMPhuongThuc.SelectedValue = m.MaPhuongThuc.ToString();
            txtMNam.Text          = m.NamTuyenSinh.ToString();
            txtMChiTieu.Text      = m.ChiTieu?.ToString() ?? "";
            txtMDiemTruoc.Text    = m.DiemChuanNamTruoc?.ToString("F2") ?? "";
            txtMDiemNay.Text      = m.DiemChuanNamNay?.ToString("F2") ?? "";
            txtMToHop.Text        = m.ToHopMonHoc ?? "";
            txtMHocPhi.Text       = m.HocPhi?.ToString("F2") ?? "";
            txtMLoaiHinh.Text     = m.LoaiHinhDaoTao ?? "";
            txtMCoSo.Text         = m.CoSoDaoTao ?? "";
            txtMTieuDe.Text       = m.TieuDe ?? "";
            txtMHanNop.Text       = m.HanNop.HasValue ? m.HanNop.Value.ToString("yyyy-MM-dd") : "";
            txtMMoTa.Text         = m.MoTa ?? "";
            chkMActive.Checked    = m.TrangThai;
            ShowModal = "true";
        }
    }

    /// <summary>
    /// Xử lý nút Lưu trong modal — thu thập dữ liệu form,
    /// gọi BLL Thêm hoặc Cập nhật tin tuyển sinh.
    /// </summary>
    protected void btnLuuTin_Click(object sender, EventArgs e)
    {
        int maTin = int.TryParse(hfMaTin.Value, out int i) ? i : 0;
        var m = new TinTuyenSinhModel
        {
            MaTin             = maTin,
            MaTruong          = int.TryParse(ddlMTruong.SelectedValue, out int tr) ? tr : 0,
            MaChuyenNganh     = int.TryParse(ddlMNganh.SelectedValue, out int cn)  ? cn : 0,
            MaPhuongThuc      = int.TryParse(ddlMPhuongThuc.SelectedValue, out int pt) ? pt : 0,
            NamTuyenSinh      = short.TryParse(txtMNam.Text, out short nam) ? nam : (short)DateTime.Now.Year,
            ChiTieu           = int.TryParse(txtMChiTieu.Text, out int ct)   ? ct : (int?)null,
            DiemChuanNamTruoc = decimal.TryParse(txtMDiemTruoc.Text, out decimal d1) ? d1 : (decimal?)null,
            DiemChuanNamNay   = decimal.TryParse(txtMDiemNay.Text, out decimal d2)   ? d2 : (decimal?)null,
            ToHopMonHoc       = txtMToHop.Text.Trim(),
            HocPhi            = decimal.TryParse(txtMHocPhi.Text, out decimal hp) ? hp : (decimal?)null,
            LoaiHinhDaoTao    = txtMLoaiHinh.Text.Trim(),
            CoSoDaoTao        = txtMCoSo.Text.Trim(),
            TieuDe            = string.IsNullOrWhiteSpace(txtMTieuDe.Text) ? null : txtMTieuDe.Text.Trim(),
            HanNop            = DateTime.TryParse(txtMHanNop.Text, out DateTime hn) ? hn : (DateTime?)null,
            MoTa              = string.IsNullOrWhiteSpace(txtMMoTa.Text) ? null : txtMMoTa.Text.Trim(),
            TrangThai         = chkMActive.Checked
        };

        var (ok, error) = maTin > 0 ? TinTuyenSinhBLL.CapNhat(m) : TinTuyenSinhBLL.Them(m);

        if (ok)
        {
            hfMaTin.Value = "";
            litThongBao.Text = "<div class='alert alert-success'>Lưu thành công!</div>";
            BindData();
        }
        else
        {
            litThongBao.Text = $"<div class='alert alert-danger'>{error}</div>";
            ShowModal = "true";
        }
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    /// <summary>
    /// Gọi TinTuyenSinhBLL.TimKiemAdmin (bao gồm cả tin ẩn) với bộ lọc,
    /// bind vào GridView và tạo nút phân trang.
    /// </summary>
    private void BindData()
    {
        int?   tr  = int.TryParse(ddlTruong.SelectedValue, out int t) ? t : (int?)null;
        int?   cn  = int.TryParse(ddlNganh.SelectedValue, out int n)  ? n : (int?)null;
        short? nam = short.TryParse(ddlNam.SelectedValue, out short y) ? y : (short?)null;

        var result = TinTuyenSinhBLL.TimKiemAdmin(tr, cn, nam, CurrentPage, PAGE_SIZE);

        litTong.Text = result.TongSo.ToString("N0");
        gvTin.DataSource = result.Data;
        gvTin.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
