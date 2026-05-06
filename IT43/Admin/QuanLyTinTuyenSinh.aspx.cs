using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

// Culture vi-VN: dấu phẩy (,) là thập phân, dấu chấm (.) là phân cách nghìn
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
        gvTin.PreRender += gvTin_PreRender;
        if (!IsPostBack)
        {
            LoadDropdowns();
            BindStats();
            BindData();
        }
        else
        {
            // EnableViewState="false" → rebind để LinkButton trong GridView tồn tại trước RowCommand
            BindData();
        }
    }

    /// <summary>Load dropdown cho cả filter bar và modal form (trường, ngành, năm, phương thức).</summary>
    private void LoadDropdowns()
    {
        // Filter
        var truongs = TruongBLL.GetDanhSachDropdown();
        foreach (var t in truongs)
            ddlTruong.Items.Add(new ListItem(t.Ten, t.Id.ToString()));

        foreach (var n in DanhMucBLL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));

        // Lấy danh sách năm từ DB + đảm bảo năm cài đặt của admin luôn có trong list
        var years = DanhMucBLL.GetNamTuyenSinh();
        int configNam = ConfigBLL.GetInt("NamTuyenSinhHienTai", 0);
        if (configNam > 0 && !years.Contains(configNam))
            years.Insert(0, configNam); // Thêm vào đầu nếu chưa có

        foreach (var y in years)
            ddlNam.Items.Add(new ListItem(y.ToString(), y.ToString()));

        // Modal
        foreach (var t in truongs)
            ddlMTruong.Items.Add(new ListItem(t.Ten, t.Id.ToString()));

        foreach (var n in DanhMucBLL.GetChuyenNganh())
            ddlMNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));

        foreach (var pt in DanhMucBLL.GetPhuongThuc())
            ddlMPhuongThuc.Items.Add(new ListItem(pt.Ten, pt.Id.ToString()));
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>
    /// Xử lý command trên GridView: XoaTin, ToggleTin (ẩn/hiện),
    /// SuaTin (load dữ liệu vào modal form).
    /// TuVanVien chỉ được xem — không thể XoaTin, ToggleTin, SuaTin.
    /// </summary>
    protected void gvTin_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // TuVanVien chỉ xem, chặn mọi write op
        if (!SecurityHelper.CanManageContent())
        {
            litThongBao.Text = "<div class='alert alert-danger py-2'><i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền thực hiện thao tác này.</div>";
            return;
        }

        int maTin = int.Parse(e.CommandArgument.ToString());

        if (e.CommandName == "XoaTin")
        {
            TinTuyenSinhBLL.Xoa(maTin);

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, "XOA_TIN_TUYEN_SINH",
                $"Xóa tin tuyển sinh (MaTin={maTin})",
                bangTacDong: "tbl_TinTuyenSinh");

            litThongBao.Text = "<div class='alert alert-success'>Đã xóa.</div>";
            BindData();
        }
        else if (e.CommandName == "ToggleTin")
        {
            TinTuyenSinhBLL.ToggleTrangThai(maTin);

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, "TOGGLE_TIN_TUYEN_SINH",
                $"Thay đổi trạng thái tin tuyển sinh (MaTin={maTin})",
                bangTacDong: "tbl_TinTuyenSinh");

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
            txtMHocPhi.Text       = m.HocPhi ?? "";  // Sprint 1: HocPhi là string
            txtMLoaiHinh.Text     = m.LoaiHinhDaoTao ?? "";
            txtMCoSo.Text         = m.CoSoDaoTao ?? "";
            txtMTieuDe.Text       = m.TieuDe ?? "";
            txtMHanNop.Text       = m.HanNop.HasValue ? m.HanNop.Value.ToString("yyyy-MM-dd") : "";
            txtMMoTa.Text         = System.Web.HttpUtility.HtmlDecode(m.MoTa ?? "");
            chkMActive.Checked    = m.TrangThai;
            ShowModal = "true";
        }
    }

    /// <summary>
    /// Xử lý nút Lưu trong modal — thu thập dữ liệu form,
    /// gọi BLL Thêm hoặc Cập nhật tin tuyển sinh.
    /// TuVanVien không được phép lưu.
    /// </summary>
    protected void btnLuuTin_Click(object sender, EventArgs e)
    {
        if (!SecurityHelper.CanManageContent())
        {
            litThongBao.Text = "<div class='alert alert-danger py-2'><i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền thực hiện thao tác này.</div>";
            return;
        }

        int maTin = int.TryParse(hfMaTin.Value, out int i) ? i : 0;

        // Tạo model từ form input
        var m = new TinTuyenSinhModel
        {
            MaTin             = maTin,
            MaTruong          = int.TryParse(ddlMTruong.SelectedValue, out int tr) ? tr : 0,
            MaChuyenNganh     = int.TryParse(ddlMNganh.SelectedValue, out int cn)  ? cn : 0,
            MaPhuongThuc      = int.TryParse(ddlMPhuongThuc.SelectedValue, out int pt) ? pt : 0,
            NamTuyenSinh      = short.TryParse(txtMNam.Text, out short nam) ? nam : (short)DateTime.Now.Year,
            ChiTieu           = int.TryParse(txtMChiTieu.Text, out int ct)   ? ct : (int?)null,
            DiemChuanNamTruoc = decimal.TryParse(txtMDiemTruoc.Text, NumberStyles.Any, new CultureInfo("vi-VN"), out decimal d1) ? d1 : (decimal?)null,
            DiemChuanNamNay   = decimal.TryParse(txtMDiemNay.Text,   NumberStyles.Any, new CultureInfo("vi-VN"), out decimal d2) ? d2 : (decimal?)null,
            ToHopMonHoc       = txtMToHop.Text.Trim(),
            HocPhi            = string.IsNullOrWhiteSpace(txtMHocPhi.Text) ? null : txtMHocPhi.Text.Trim(),
            LoaiHinhDaoTao    = txtMLoaiHinh.Text.Trim(),
            CoSoDaoTao        = txtMCoSo.Text.Trim(),
            TieuDe            = string.IsNullOrWhiteSpace(txtMTieuDe.Text) ? null : txtMTieuDe.Text.Trim(),
            HanNop            = DateTime.TryParse(txtMHanNop.Text, out DateTime hn) ? hn : (DateTime?)null,
            MoTa              = string.IsNullOrWhiteSpace(txtMMoTa.Text) ? null : txtMMoTa.Text.Trim(),
            TrangThai         = chkMActive.Checked
        };

        // Server-side validation: AllowPastDates check
        bool allowPastDates = ConfigBLL.GetBool("AllowPastDates", false);
        if (!allowPastDates && m.HanNop.HasValue && m.HanNop.Value.Date < DateTime.Today)
        {
            litThongBao.Text = "<div class='alert alert-danger'>Ngày hạn nộp không được trong quá khứ.</div>";
            ShowModal = "true";
            return;
        }

        var (ok, error) = maTin > 0 ? TinTuyenSinhBLL.CapNhat(m) : TinTuyenSinhBLL.Them(m);

        if (ok)
        {
            // Auto-sync điểm chuẩn năm nay vào bảng lịch sử
            if (m.DiemChuanNamNay.HasValue)
                DiemChuanLichSuBLL.SyncFromTinTuyenSinh(m.MaTruong);

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, maTin > 0 ? "CAP_NHAT_TIN_TUYEN_SINH" : "THEM_TIN_TUYEN_SINH",
                $"{(maTin > 0 ? "Cập nhật" : "Thêm")} tin tuyển sinh: Năm={m.NamTuyenSinh}, Trường={m.MaTruong}, Ngành={m.MaChuyenNganh}",
                bangTacDong: "tbl_TinTuyenSinh");

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

    protected void gvTin_PreRender(object sender, EventArgs e)
    {
        if (gvTin.HeaderRow != null)
            gvTin.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    protected bool SafeGetBool(object value)
    {
        return value != DBNull.Value && value != null && (bool)value;
    }

    protected string GetTrangThaiBadge(object value)
    {
        if (value == DBNull.Value || value == null) return "";
        bool trangThai = (bool)value;
        return trangThai
            ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Hiện</span>"
            : "<span class='badge badge-soft-secondary'><i class='bi bi-eye-slash me-1'></i>Ẩn</span>";
    }

    private void BindStats()
    {
        var stats = ThongKeBLL.ThongKeTinTuyenSinh();
        litTongTin.Text = stats.tong.ToString("N0");
        litHienThi.Text = stats.hienThi.ToString("N0");
        litLuotXem.Text = stats.luotXem.ToString("N0");
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
