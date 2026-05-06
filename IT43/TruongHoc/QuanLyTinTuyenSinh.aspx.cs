using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý tin tuyển sinh cho tài khoản TruongHoc.
/// Chỉ hiển thị và thao tác tin của trường mình.
/// Hỗ trợ đầy đủ fields giống Admin: điểm chuẩn, học phí, tổ hợp môn, hạn nộp, mô tả.
/// </summary>
public partial class TruongHoc_QuanLyTinTuyenSinh : Page
{
    private const int PAGE_SIZE = 15;
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }
    private int MaTruong    => SecurityHelper.GetCurrentMaTruong();
    private int MaTK        => SecurityHelper.GetCurrentMaTaiKhoan();

    /// <summary>Flag điều khiển hiển thị modal Bootstrap từ server-side.</summary>
    public string ShowModal = "false";

    protected void Page_Load(object sender, EventArgs e)
    {
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

    protected void btnLoc_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    protected void gvTin_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int maTin = int.Parse(e.CommandArgument.ToString());

        switch (e.CommandName)
        {
            case "Sua":
            {
                var tin = TinTuyenSinhDAL.GetById(maTin);
                if (tin == null || tin.MaTruong != MaTruong) break; // bảo vệ IDOR

                hfMaTin.Value                = maTin.ToString();
                ddlMNganh.SelectedValue      = tin.MaChuyenNganh.ToString();
                ddlMPhuongThuc.SelectedValue = tin.MaPhuongThuc.ToString();
                txtMNam.Text                 = tin.NamTuyenSinh.ToString();
                txtMChiTieu.Text             = tin.ChiTieu.HasValue ? tin.ChiTieu.Value.ToString() : "";
                txtMDiemTruoc.Text           = tin.DiemChuanNamTruoc?.ToString("F2") ?? "";
                txtMDiemNay.Text             = tin.DiemChuanNamNay?.ToString("F2") ?? "";
                txtMToHop.Text               = tin.ToHopMonHoc ?? "";
                txtMHocPhi.Text              = tin.HocPhi ?? "";
                txtMLoaiHinh.Text            = tin.LoaiHinhDaoTao ?? "";
                txtMHanNop.Text              = tin.HanNop.HasValue ? tin.HanNop.Value.ToString("yyyy-MM-dd") : "";
                txtMTieuDe.Text              = tin.TieuDe ?? "";
                txtMMoTa.Text                = System.Web.HttpUtility.HtmlDecode(tin.MoTa ?? "");
                chkTrangThai.Checked         = tin.TrangThai;

                ShowModal = "true";
                BindStats();
                BindData();
                break;
            }
            case "Toggle":
            {
                var tin = TinTuyenSinhDAL.GetById(maTin);
                if (tin == null || tin.MaTruong != MaTruong) break;
                TinTuyenSinhBLL.ToggleTrangThai(maTin);

                LogHelper.Ghi(MaTK, "TOGGLE_TIN_TUYEN_SINH",
                    $"Thay đổi trạng thái tin tuyển sinh (MaTin={maTin})",
                    bangTacDong: "tbl_TinTuyenSinh");

                ShowMsg("success", "Đã cập nhật trạng thái tin.");
                BindStats();
                BindData();
                break;
            }
            case "Xoa":
            {
                var tin = TinTuyenSinhDAL.GetById(maTin);
                if (tin == null || tin.MaTruong != MaTruong) break;
                TinTuyenSinhBLL.Xoa(maTin);

                LogHelper.Ghi(MaTK, "XOA_TIN_TUYEN_SINH",
                    $"Xóa tin tuyển sinh (MaTin={maTin}, MaTruong={MaTruong})",
                    bangTacDong: "tbl_TinTuyenSinh");

                ShowMsg("success", "Đã xóa tin tuyển sinh.");
                BindStats();
                BindData();
                break;
            }
        }
    }

    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    protected void btnLuu_Click(object sender, EventArgs e)
    {
        int maTin = int.TryParse(hfMaTin.Value, out int i) ? i : 0;

        // Validate cơ bản
        if (ddlMNganh.SelectedIndex <= 0)
        {
            litModalLoi.Text = Alert("danger", "Vui lòng chọn chuyên ngành.");
            ShowModal = "true"; BindStats(); BindData(); return;
        }
        if (ddlMPhuongThuc.SelectedIndex <= 0)
        {
            litModalLoi.Text = Alert("danger", "Vui lòng chọn phương thức xét tuyển.");
            ShowModal = "true"; BindStats(); BindData(); return;
        }
        if (!short.TryParse(txtMNam.Text, out short nam) || nam < 2020)
        {
            litModalLoi.Text = Alert("danger", "Năm tuyển sinh không hợp lệ.");
            ShowModal = "true"; BindStats(); BindData(); return;
        }

        var m = new TinTuyenSinhModel
        {
            MaTin             = maTin,
            MaTruong          = MaTruong,
            MaChuyenNganh     = int.Parse(ddlMNganh.SelectedValue),
            MaPhuongThuc      = int.Parse(ddlMPhuongThuc.SelectedValue),
            NamTuyenSinh      = nam,
            ChiTieu           = int.TryParse(txtMChiTieu.Text.Trim(), out int ct) ? ct : (int?)null,
            DiemChuanNamTruoc = decimal.TryParse(txtMDiemTruoc.Text.Trim(), NumberStyles.Any,
                                    new CultureInfo("vi-VN"), out decimal d1) ? d1 : (decimal?)null,
            DiemChuanNamNay   = decimal.TryParse(txtMDiemNay.Text.Trim(), NumberStyles.Any,
                                    new CultureInfo("vi-VN"), out decimal d2) ? d2 : (decimal?)null,
            ToHopMonHoc       = string.IsNullOrWhiteSpace(txtMToHop.Text) ? null : txtMToHop.Text.Trim(),
            HocPhi            = string.IsNullOrWhiteSpace(txtMHocPhi.Text) ? null : txtMHocPhi.Text.Trim(),
            LoaiHinhDaoTao    = string.IsNullOrWhiteSpace(txtMLoaiHinh.Text) ? null : txtMLoaiHinh.Text.Trim(),
            HanNop            = DateTime.TryParse(txtMHanNop.Text, out DateTime hn) ? hn : (DateTime?)null,
            TieuDe            = string.IsNullOrWhiteSpace(txtMTieuDe.Text) ? null : txtMTieuDe.Text.Trim(),
            MoTa              = string.IsNullOrWhiteSpace(txtMMoTa.Text) ? null : txtMMoTa.Text.Trim(),
            TrangThai         = chkTrangThai.Checked,
        };

        (bool ok, string err) result;
        if (maTin == 0)
        {
            result = TinTuyenSinhBLL.Them(m);
        }
        else
        {
            // Kiểm tra IDOR
            var existing = TinTuyenSinhDAL.GetById(maTin);
            if (existing == null || existing.MaTruong != MaTruong)
            {
                litModalLoi.Text = Alert("danger", "Không có quyền chỉnh sửa tin này.");
                ShowModal = "true"; BindStats(); BindData(); return;
            }
            result = TinTuyenSinhBLL.CapNhat(m);
        }

        if (result.ok)
        {
            // Auto-sync điểm chuẩn năm nay vào bảng lịch sử
            if (m.DiemChuanNamNay.HasValue)
                DiemChuanLichSuBLL.SyncFromTinTuyenSinh(MaTruong);

            LogHelper.Ghi(MaTK, maTin == 0 ? "THEM_TIN_TUYEN_SINH" : "CAP_NHAT_TIN_TUYEN_SINH",
                $"{(maTin == 0 ? "Thêm" : "Cập nhật")} tin tuyển sinh: Năm={m.NamTuyenSinh}, Ngành={m.MaChuyenNganh}",
                bangTacDong: "tbl_TinTuyenSinh");

            hfMaTin.Value = "0";
            ShowMsg("success", maTin == 0 ? "Đã thêm tin tuyển sinh." : "Đã cập nhật tin tuyển sinh.");
            BindStats();
            BindData();
        }
        else
        {
            litModalLoi.Text = Alert("danger", Server.HtmlEncode(result.err));
            ShowModal = "true";
            BindStats();
            BindData();
        }
    }

    private void LoadDropdowns()
    {
        // Filter: Ngành
        ddlNganh.Items.Clear();
        ddlNganh.Items.Add(new ListItem("-- Chọn ngành --", ""));
        foreach (var item in DanhMucBLL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(item.Ten, item.Id.ToString()));

        // Filter: Năm
        ddlNam.Items.Clear();
        ddlNam.Items.Add(new ListItem("-- Năm --", ""));
        var years = DanhMucBLL.GetNamTuyenSinh();
        int configNam = ConfigBLL.GetInt("NamTuyenSinhHienTai", 0);
        if (configNam > 0 && !years.Contains(configNam))
            years.Insert(0, configNam);
        foreach (var y in years)
            ddlNam.Items.Add(new ListItem(y.ToString(), y.ToString()));

        // Modal: Ngành
        ddlMNganh.Items.Clear();
        ddlMNganh.Items.Add(new ListItem("-- Chọn ngành --", "0"));
        foreach (var item in DanhMucBLL.GetChuyenNganh())
            ddlMNganh.Items.Add(new ListItem(item.Ten, item.Id.ToString()));

        // Modal: Phương thức
        ddlMPhuongThuc.Items.Clear();
        ddlMPhuongThuc.Items.Add(new ListItem("-- Chọn phương thức --", "0"));
        foreach (var item in DanhMucBLL.GetPhuongThuc())
            ddlMPhuongThuc.Items.Add(new ListItem(item.Ten, item.Id.ToString()));
    }

    private void BindStats()
    {
        // Lấy thống kê của riêng trường này
        var all    = TinTuyenSinhBLL.TimKiemAdmin(MaTruong, null, null, 0, 1);
        litTongTin.Text = all.TongSo.ToString("N0");

        // Hiển thị: đếm trực tiếp
        var active = DBHelper.Scalar(
            "SELECT COUNT(*) FROM tbl_TinTuyenSinh WHERE MaTruong=@mt AND TrangThai=1",
            new[] { new System.Data.SqlClient.SqlParameter("@mt", MaTruong) });
        litHienThi.Text = (active == null ? 0 : System.Convert.ToInt32(active)).ToString("N0");

        // Lượt xem
        var views = DBHelper.Scalar(
            "SELECT ISNULL(SUM(LuotXem),0) FROM tbl_TinTuyenSinh WHERE MaTruong=@mt",
            new[] { new System.Data.SqlClient.SqlParameter("@mt", MaTruong) });
        litLuotXem.Text = (views == null ? 0L : System.Convert.ToInt64(views)).ToString("N0");
    }

    private void BindData()
    {
        int? cn  = int.TryParse(ddlNganh.SelectedValue, out int n) && !string.IsNullOrEmpty(ddlNganh.SelectedValue) ? n : (int?)null;
        short? y = short.TryParse(ddlNam.SelectedValue, out short yr) && !string.IsNullOrEmpty(ddlNam.SelectedValue) ? yr : (short?)null;

        var result = TinTuyenSinhBLL.TimKiemAdmin(MaTruong, cn, y, CurrentPage, PAGE_SIZE);

        litTong.Text    = result.TongSo.ToString("N0");
        gvTin.DataSource = result.Data;
        gvTin.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    private void ShowMsg(string type, string msg) => litThongBao.Text = Alert(type, msg);
    private static string Alert(string type, string msg)
        => $"<div class='alert alert-{type} alert-dismissible fade show'>{msg}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
}
