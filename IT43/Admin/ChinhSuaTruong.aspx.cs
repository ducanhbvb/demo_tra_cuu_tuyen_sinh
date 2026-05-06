using System;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Thêm / Chỉnh sửa trường (Admin).
/// - Tab 1 (thongtin): form thông tin cơ bản + upload ảnh.
/// - Tab 2 (diemchuan): quản lý lịch sử điểm chuẩn (GridView + modal CRUD + nút Đồng bộ).
///   Tab 2 chỉ hiển thị khi IsEdit = true.
/// </summary>
public partial class Admin_ChinhSuaTruong : Page
{
    // ── QueryString ──────────────────────────────────────────────────────────
    /// <summary>Mã trường từ QueryString (0 = thêm mới).</summary>
    public new int ID => int.TryParse(Request.QueryString["id"], out int i) ? i : 0;

    /// <summary>Tab đang active: "thongtin" (mặc định) hoặc "diemchuan".</summary>
    public string ActiveTab
    {
        get
        {
            var t = (Request.QueryString["tab"] ?? "").ToLower();
            return (t == "diemchuan" && IsEdit) ? "diemchuan" : "thongtin";
        }
    }

    /// <summary>True nếu đang chỉnh sửa (có ID), false nếu thêm mới.</summary>
    public bool IsEdit => ID > 0;

    // ── Flags mở modal từ server ─────────────────────────────────────────────
    /// <summary>Flag mở lại modal điểm chuẩn sau PostBack lỗi.</summary>
    public string ShowDiemChuanModal { get; private set; } = "false";

    // ── Page_Load ────────────────────────────────────────────────────────────
    protected void Page_Load(object sender, EventArgs e)
    {
        litTieuDe.Text = IsEdit ? "Chỉnh sửa trường" : "Thêm trường mới";

        // Hiện tab điểm chuẩn chỉ khi sửa
        phTabDiemChuan.Visible = IsEdit;

        if (ActiveTab == "diemchuan")
        {
            panelThongTin.Visible  = false;
            panelDiemChuan.Visible = true;

            if (!IsPostBack)
            {
                LoadDiemChuanDropdowns();
                BindDiemChuan();
            }
        }
        else
        {
            panelThongTin.Visible  = true;
            panelDiemChuan.Visible = false;

            if (!IsPostBack)
            {
                // Load danh sách tỉnh/TP vào dropdown
                foreach (var t in DanhMucDAL.GetTinhThanh())
                    ddlTinhThanh.Items.Add(new ListItem(t, t));

                if (IsEdit)
                    LoadTruong();
            }
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // TAB 1 — THÔNG TIN CHUNG
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>Load thông tin trường từ DB vào các control form và preview ảnh.</summary>
    private void LoadTruong()
    {
        var m = TruongBLL.LayChiTiet(ID);
        if (m == null) { Response.Redirect("QuanLyTruong.aspx"); return; }

        txtTen.Text           = m.TenTruong;
        txtSlug.Text          = m.Slug;
        txtDiaChi.Text        = m.DiaChi;
        ddlTinhThanh.SelectedValue = m.TinhThanh ?? "";
        ddlVung.SelectedValue = m.MaVung?.ToString() ?? "";
        ddlLoai.SelectedValue = m.LoaiTruong?.ToString() ?? "";
        ddlCapBacDaoTao.SelectedValue = m.CapBacDaoTao?.ToString() ?? "";
        txtSdt.Text           = m.SoDienThoai;
        txtWebsite.Text       = m.Website;
        txtQuyMo.Text         = m.QuyMo;
        txtMoTa.Text          = System.Web.HttpUtility.HtmlDecode(m.MoTa ?? "");
        chkKiemDinh.Checked   = m.KiemDinhChatLuong;

        hfAnhDaiDien.Value = m.AnhDaiDien ?? "";
        hfAnhBia.Value     = m.AnhBia ?? "";

        string fallback = ResolveUrl("~/Resources/Images/no-image.png");
        imgPreviewAvatar.ImageUrl = !string.IsNullOrEmpty(m.AnhDaiDien) ? m.AnhDaiDien : fallback;
        imgPreviewBia.ImageUrl    = !string.IsNullOrEmpty(m.AnhBia)     ? m.AnhBia     : fallback;
    }

    /// <summary>Xử lý nút Lưu thông tin trường.</summary>
    protected void btnLuu_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        string anhDaiDien = hfAnhDaiDien.Value;
        string anhBia     = hfAnhBia.Value;
        string slug       = txtSlug.Text.Trim();
        if (string.IsNullOrEmpty(slug))
            slug = SlugHelper.ToSlug(txtTen.Text.Trim());

        if (fuAnhDaiDien.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuAnhDaiDien.PostedFile, "truong", slug + "_logo", ImageUploadHelper.MAX_AVATAR_SIZE);
            if (!ok) { litThongBao.Text = $"<div class='alert alert-danger'>Ảnh đại diện: {result}</div>"; return; }
            ImageUploadHelper.DeleteOld(anhDaiDien);
            anhDaiDien = result;
        }

        if (fuAnhBia.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuAnhBia.PostedFile, "truong", slug + "_cover", ImageUploadHelper.MAX_COVER_SIZE);
            if (!ok) { litThongBao.Text = $"<div class='alert alert-danger'>Ảnh bìa: {result}</div>"; return; }
            ImageUploadHelper.DeleteOld(anhBia);
            anhBia = result;
        }

        var m = new TruongModel
        {
            MaTruong          = ID,
            TenTruong         = txtTen.Text.Trim(),
            Slug              = slug,
            DiaChi            = txtDiaChi.Text.Trim(),
            TinhThanh         = ddlTinhThanh.SelectedValue,
            MaVung            = byte.TryParse(ddlVung.SelectedValue, out byte v) ? v : (byte?)null,
            LoaiTruong        = byte.TryParse(ddlLoai.SelectedValue, out byte l) ? l : (byte?)null,
            CapBacDaoTao      = byte.TryParse(ddlCapBacDaoTao.SelectedValue, out byte c) ? c : (byte?)null,
            SoDienThoai       = txtSdt.Text.Trim(),
            Website           = txtWebsite.Text.Trim(),
            QuyMo             = txtQuyMo.Text.Trim(),
            AnhDaiDien        = anhDaiDien,
            AnhBia            = anhBia,
            MoTa              = txtMoTa.Text.Trim(),
            KiemDinhChatLuong = chkKiemDinh.Checked,
            MaTaiKhoan        = SecurityHelper.GetCurrentMaTaiKhoan()
        };

        var (saved, error) = IsEdit ? TruongBLL.CapNhat(m) : TruongBLL.Them(m);

        if (saved)
        {
            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, IsEdit ? "CAP_NHAT_TRUONG" : "THEM_TRUONG",
                $"{(IsEdit ? "Cập nhật" : "Thêm")} trường: {m.TenTruong} (MaTruong={m.MaTruong})",
                bangTacDong: "tbl_Truong");

            Response.Redirect("QuanLyTruong.aspx?msg=saved");
        }
        else
            litThongBao.Text = $"<div class='alert alert-danger'>{error}</div>";
    }

    // ════════════════════════════════════════════════════════════════════════
    // TAB 2 — ĐIỂM CHUẨN LỊCH SỬ
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>Load dropdown ngành, năm, phương thức cho filter và modal.</summary>
    private void LoadDiemChuanDropdowns()
    {
        // Filter dropdowns
        foreach (var n in DanhMucBLL.GetChuyenNganh())
        {
            ddlFNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));
            ddlMDCNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));
        }
        foreach (var y in DanhMucBLL.GetNamTuyenSinh())
            ddlFNam.Items.Add(new ListItem(y.ToString(), y.ToString()));

        foreach (var pt in DanhMucBLL.GetPhuongThuc())
        {
            ddlFPhuongThuc.Items.Add(new ListItem(pt.Ten, pt.Id.ToString()));
            ddlMDCPhuongThuc.Items.Add(new ListItem(pt.Ten, pt.Id.ToString()));
        }
    }

    /// <summary>Bind GridView điểm chuẩn lịch sử theo bộ lọc hiện tại.</summary>
    private void BindDiemChuan()
    {
        int?   cn  = int.TryParse(ddlFNganh.SelectedValue,      out int n)  ? n  : (int?)null;
        short? nam = short.TryParse(ddlFNam.SelectedValue,      out short y) ? y : (short?)null;
        int?   pt  = int.TryParse(ddlFPhuongThuc.SelectedValue, out int p)  ? p  : (int?)null;

        var list = DiemChuanLichSuBLL.GetDanhSachTheoTruong(ID, cn, nam, pt);
        litTongDC.Text       = list.Count.ToString("N0");
        gvDiemChuan.DataSource = list;
        gvDiemChuan.DataBind();
    }

    /// <summary>Xử lý nút Lọc trong Tab 2.</summary>
    protected void btnLocDC_Click(object sender, EventArgs e) => BindDiemChuan();

    /// <summary>Đồng bộ toàn bộ từ tbl_TinTuyenSinh → tbl_DiemChuanLichSu.</summary>
    protected void btnDongBo_Click(object sender, EventArgs e)
    {
        DiemChuanLichSuBLL.SyncFromTinTuyenSinh(ID);
        litThongBaoDC.Text = "<div class='alert alert-success'>Đồng bộ thành công!</div>";
        BindDiemChuan();
    }

    /// <summary>
    /// Xử lý RowCommand trong GridView điểm chuẩn:
    /// SuaDC — load bản ghi vào modal form.
    /// XoaDC — xóa bản ghi.
    /// </summary>
    protected void gvDiemChuan_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = int.Parse(e.CommandArgument.ToString());

        if (e.CommandName == "XoaDC")
        {
            DiemChuanLichSuBLL.Xoa(id);

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, "XOA_DIEM_CHUAN",
                $"Xóa điểm chuẩn lịch sử (ID={id}, MaTruong={ID})",
                bangTacDong: "tbl_DiemChuanLichSu");

            litThongBaoDC.Text = "<div class='alert alert-success'>Đã xóa.</div>";
            BindDiemChuan();
        }
        else if (e.CommandName == "SuaDC")
        {
            var m = DiemChuanLichSuBLL.GetById(id);
            if (m == null) return;
            hfIDDC.Value = id.ToString();
            ddlMDCNganh.SelectedValue      = m.MaChuyenNganh.ToString();
            ddlMDCPhuongThuc.SelectedValue = m.MaPhuongThuc.ToString();
            txtMDCNam.Text     = m.NamTuyenSinh.ToString();
            txtMDCDiem.Text    = m.DiemChuan?.ToString("F2") ?? "";
            txtMDCChiTieu.Text = m.ChiTieu?.ToString() ?? "";
            txtMDCGhiChu.Text  = m.GhiChu ?? "";
            ShowDiemChuanModal = "true";
            BindDiemChuan();
        }
    }

    /// <summary>Xử lý nút Lưu trong modal điểm chuẩn (thêm hoặc cập nhật).</summary>
    protected void btnLuuDC_Click(object sender, EventArgs e)
    {
        int idDC = int.TryParse(hfIDDC.Value, out int i) ? i : 0;

        var m = new DiemChuanLichSuModel
        {
            ID            = idDC,
            MaTruong      = ID,
            MaChuyenNganh = int.TryParse(ddlMDCNganh.SelectedValue,      out int cn) ? cn : 0,
            MaPhuongThuc  = int.TryParse(ddlMDCPhuongThuc.SelectedValue, out int pt) ? pt : 0,
            NamTuyenSinh  = short.TryParse(txtMDCNam.Text,   out short nam) ? nam : (short)DateTime.Now.Year,
            DiemChuan     = decimal.TryParse(txtMDCDiem.Text, out decimal d) ? d : (decimal?)null,
            ChiTieu       = int.TryParse(txtMDCChiTieu.Text,  out int ct)   ? ct : (int?)null,
            GhiChu        = string.IsNullOrWhiteSpace(txtMDCGhiChu.Text) ? null : txtMDCGhiChu.Text.Trim()
        };

        var (ok, error) = idDC > 0
            ? DiemChuanLichSuBLL.CapNhat(m)
            : DiemChuanLichSuBLL.Them(m);

        if (ok)
        {
            hfIDDC.Value       = "0";
            litThongBaoDC.Text = "<div class='alert alert-success'>Lưu thành công!</div>";
            BindDiemChuan();
        }
        else
        {
            litThongBaoDC.Text = $"<div class='alert alert-danger'>{error}</div>";
            ShowDiemChuanModal = "true";
            BindDiemChuan();
        }
    }
}
