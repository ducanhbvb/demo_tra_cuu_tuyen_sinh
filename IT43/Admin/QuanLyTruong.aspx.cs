using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý trường (Admin) — danh sách trường với bộ lọc, phân trang,
/// toggle trạng thái hiển thị, xóa trường và modal thêm/sửa nhanh.
/// </summary>
public partial class Admin_QuanLyTruong : Page
{
    private const int PAGE_SIZE = 15;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    protected int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Flag điều khiển hiển thị modal Bootstrap từ server-side.</summary>
    public string ShowModal { get; private set; } = "false";

    /// <summary>Flag điều khiển hiển thị modal xác nhận xóa vĩnh viễn từ server-side.</summary>
    public string ShowDeleteModal { get; private set; } = "false";

    /// <summary>Flag điều khiển hiển thị cảnh báo lần 2 từ server-side sau khi đã kiểm tra phụ thuộc.</summary>
    public string ShowDeleteSecondWarning { get; private set; } = "false";

    /// <summary>Nội dung cảnh báo lần 2 sau khi kiểm tra phụ thuộc server-side.</summary>
    public string DeleteSecondWarningMessage { get; private set; } = "";

    /// <summary>Khởi tạo trang: load dropdown tỉnh/thành và bind dữ liệu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Luôn thêm data-field attributes cho controls (cho JavaScript)
        AddDataFieldAttributes();
        gvTruong.PreRender += gvTruong_PreRender;

        // Bind dropdown tỉnh thành cho filter (ddlTinh) và modal (ddlMTinhThanh) - luôn bind vì static data
        if (ddlTinh.Items.Count <= 1 || ddlMTinhThanh.Items.Count <= 1)
        {
            var tinhThanhList = DanhMucDAL.GetTinhThanh();

            // Clear và bind cho cả 2 dropdown
            ddlTinh.Items.Clear();
            ddlMTinhThanh.Items.Clear();

            foreach (var t in tinhThanhList)
            {
                ddlTinh.Items.Add(new ListItem(t, t));
                ddlMTinhThanh.Items.Add(new ListItem(t, t));
            }

            // Thêm lại mục mặc định cho cả 2
            ddlTinh.Items.Insert(0, new ListItem("-- Tỉnh/TP --", ""));
            ddlMTinhThanh.Items.Insert(0, new ListItem("-- Chọn tỉnh/TP --", ""));
        }

        if (!IsPostBack)
        {
            BindData();
            CalculateStats();

            // Hiển thị thông báo sau redirect từ ChinhSuaTruong
            if (Request.QueryString["msg"] == "saved")
                litThongBao.Text = "<div class='alert alert-success'>Lưu trường thành công!</div>";
        }
        else
        {
            // EnableViewState="false" → rebind để LinkButton trong GridView tồn tại trước RowCommand
            BindData();
        }
    }

    /// <summary>Tính toán và hiển thị các thống kê cards.</summary>
    private void CalculateStats()
    {
        // Tổng số trường
        var all = TruongBLL.TimKiem("", "", null, null, null, null, null, null, "TenTruong", "ASC", 0, 1, null);
        litTongTruong.Text = all.TongSo.ToString("N0");

        // Công lập / Tư thục
        var cong = TruongBLL.TimKiem("", "", null, 1, null, null, null, null, "TenTruong", "ASC", 0, 1, null);
        var tu = TruongBLL.TimKiem("", "", null, 2, null, null, null, null, "TenTruong", "ASC", 0, 1, null);
        litCongTru.Text = $"{cong.TongSo:N0} / {tu.TongSo:N0}";

        // Mới thêm (placeholder - cần thêm filter ngày trong BLL/DAL)
        // TODO: Implement date filtering in TruongBLL.TimKiem for recent additions
        litMoiTuan.Text = "0";
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>Xử lý nút Xóa bộ lọc — reset tất cả filter.</summary>
    protected void btnXoaLoc_Click(object sender, EventArgs e)
    {
        txtFilter.Text = "";
        ddlTinh.SelectedIndex = 0;
        ddlTrangThai.SelectedIndex = 0;
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý RowDataBound để thêm row-warning class cho trường chưa kiểm định.</summary>
    protected void gvTruong_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;

        var row = e.Row.DataItem as System.Data.DataRowView;
        if (row != null && row.Row.Table.Columns.Contains("KiemDinhChatLuong"))
        {
            bool kiemDinh = false;
            if (row["KiemDinhChatLuong"] != DBNull.Value)
                bool.TryParse(row["KiemDinhChatLuong"].ToString(), out kiemDinh);

            if (!kiemDinh)
            {
                e.Row.CssClass = "row-warning";
            }
        }
    }

    /// <summary>
    /// Xử lý command trên GridView:
    /// ToggleTruong (ẩn/hiện), XoaTruong (xóa), SuaTruong (load modal).
    /// </summary>
    protected void gvTruong_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!int.TryParse(e.CommandArgument?.ToString(), out int id)) return;

        if (e.CommandName == "ToggleTruong")
        {
            TruongBLL.ToggleTrangThai(id);

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, "TOGGLE_TRUONG",
                $"Thay đổi trạng thái hiển thị trường (MaTruong={id})",
                bangTacDong: "tbl_Truong");

            litThongBao.Text = "<div class='alert alert-success alert-dismissible fade show py-2'>Đã thay đổi trạng thái hiển thị trường.</div>";
            BindData();
            return;
        }

        if (e.CommandName == "XoaTruong")
        {
            try
            {
                // Lấy tên trường trước khi xóa tạm để ghi log chi tiết
                var truongInfo = TruongBLL.LayChiTiet(id);
                string tenTruong = truongInfo?.TenTruong ?? $"ID={id}";

                TruongBLL.Xoa(id);

                int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
                LogHelper.Ghi(adminId, "XOA_TAM_TRUONG",
                    $"Xóa tạm 1 bản ghi trường: {tenTruong} (MaTruong={id}); giữ record và khóa tài khoản trường liên quan",
                    bangTacDong: "tbl_Truong");

                litThongBao.Text = "<div class='alert-admin success fade show'><i class='bi bi-check-circle'></i> Đã xóa tạm trường khỏi danh sách hiển thị và khóa tài khoản trường liên quan. Có thể khôi phục khi cần.</div>";
            }
            catch (Exception)
            {
                litThongBao.Text = "<div class='alert-admin danger fade show'><i class='bi bi-exclamation-triangle'></i> Không thể xóa tạm trường này. Vui lòng thử lại hoặc kiểm tra log hệ thống.</div>";
            }
            BindData();
            return;
        }

        if (e.CommandName == "KhoiPhucTruong")
        {
            try
            {
                var truongInfo = TruongBLL.LayChiTiet(id);
                string tenTruong = truongInfo?.TenTruong ?? $"ID={id}";

                TruongBLL.KhoiPhuc(id);

                int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
                LogHelper.Ghi(adminId, "KHOI_PHUC_TRUONG",
                    $"Khôi phục 1 bản ghi trường: {tenTruong} (MaTruong={id}); mở lại tài khoản trường liên quan",
                    bangTacDong: "tbl_Truong");

                litThongBao.Text = "<div class='alert-admin success fade show'><i class='bi bi-check-circle'></i> Đã khôi phục trường và mở lại tài khoản trường liên quan.</div>";
            }
            catch (Exception)
            {
                litThongBao.Text = "<div class='alert-admin danger fade show'><i class='bi bi-exclamation-triangle'></i> Không thể khôi phục trường này. Vui lòng thử lại hoặc kiểm tra log hệ thống.</div>";
            }
            BindData();
            return;
        }

        if (e.CommandName == "XoaVinhVienTruong")
        {
            var truongInfo = TruongBLL.LayChiTietAdmin(id);
            if (truongInfo == null)
            {
                litThongBao.Text = "<div class='alert-admin danger fade show'><i class='bi bi-exclamation-triangle'></i> Không tìm thấy trường cần xóa vĩnh viễn.</div>";
                BindData();
                return;
            }

            hfXoaVinhVienMaTruong.Value = id.ToString();
            litXoaVinhVienTenTruong.Text = Server.HtmlEncode(truongInfo.TenTruong);
            txtXoaVinhVienMatKhau.Text = "";

            string dependencyMessage = BuildDeleteDependencyMessage(id);
            hfCanOpenDeletePasswordModal.Value = "1";
            DeleteSecondWarningMessage = dependencyMessage;
            ShowDeleteSecondWarning = "true";
            return;
        }

        if (e.CommandName == "SuaTruong")
        {
            var m = TruongBLL.LayChiTiet(id);
            if (m == null) return;

            hfMaTruong.Value          = id.ToString();
            txtMTen.Text              = m.TenTruong;
            txtMSlug.Text             = m.Slug;
            txtMDiaChi.Text           = m.DiaChi;
            // Set SelectedValue an toàn: kiểm tra tồn tại trong dropdown trước
            if (!string.IsNullOrEmpty(m.TinhThanh) && ddlMTinhThanh.Items.FindByValue(m.TinhThanh) != null)
                ddlMTinhThanh.SelectedValue = m.TinhThanh;
            else
                ddlMTinhThanh.SelectedIndex = 0;
            if (m.MaVung.HasValue && ddlMVung.Items.FindByValue(m.MaVung.Value.ToString()) != null)
                ddlMVung.SelectedValue = m.MaVung.Value.ToString();
            else
                ddlMVung.SelectedIndex = 0;
            if (m.LoaiTruong.HasValue && ddlMLoai.Items.FindByValue(m.LoaiTruong.Value.ToString()) != null)
                ddlMLoai.SelectedValue = m.LoaiTruong.Value.ToString();
            else
                ddlMLoai.SelectedIndex = 0;
            if (m.CapBacDaoTao.HasValue && ddlMCapBacDaoTao.Items.FindByValue(m.CapBacDaoTao.Value.ToString()) != null)
                ddlMCapBacDaoTao.SelectedValue = m.CapBacDaoTao.Value.ToString();
            else
                ddlMCapBacDaoTao.SelectedIndex = 0;
            txtMSdt.Text              = m.SoDienThoai;
            txtMWebsite.Text          = m.Website;
            txtMQuyMo.Text            = m.QuyMo;
            txtMMoTa.Text             = System.Web.HttpUtility.HtmlDecode(m.MoTa ?? "");
            chkMKiemDinh.Checked     = m.KiemDinhChatLuong;
            hfMAnhDaiDien.Value       = m.AnhDaiDien ?? "";
            hfMAnhBia.Value           = m.AnhBia ?? "";

            // Hiển thị ảnh hiện tại trong preview
            string fallback = ResolveUrl("~/Resources/Images/no-image.png");
            if (!string.IsNullOrEmpty(m.AnhDaiDien))
            {
                imgPreviewMAvatar.ImageUrl = m.AnhDaiDien;
                imgPreviewMAvatar.Style["display"] = "";
            }
            if (!string.IsNullOrEmpty(m.AnhBia))
            {
                imgPreviewMBia.ImageUrl = m.AnhBia;
                imgPreviewMBia.Style["display"] = "";
            }

            litModalTitle.Text = "Chỉnh sửa trường";
            ShowModal = "true";
        }
    }

    /// <summary>
    /// Xử lý nút Lưu trong modal — upload ảnh (nếu có),
    /// thu thập dữ liệu form, gọi TruongBLL.Them hoặc CapNhat.
    /// </summary>
    protected void btnLuuTruong_Click(object sender, EventArgs e)
    {
        string tenTruong = txtMTen.Text.Trim();
        if (string.IsNullOrEmpty(tenTruong))
        {
            litThongBao.Text = "<div class='alert alert-danger'>Vui lòng nhập tên trường.</div>";
            ShowModal = "true";
            return;
        }

        int maTruong = int.TryParse(hfMaTruong.Value, out int i) ? i : 0;
        bool isEdit  = maTruong > 0;

        string slug = txtMSlug.Text.Trim();
        if (string.IsNullOrEmpty(slug))
            slug = SlugHelper.ToSlug(tenTruong);

        // Ảnh cũ (từ HiddenField, giữ lại nếu không upload mới)
        string anhDaiDien = hfMAnhDaiDien.Value;
        string anhBia     = hfMAnhBia.Value;

        // Upload ảnh đại diện mới nếu có
        if (fuMAnhDaiDien.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuMAnhDaiDien.PostedFile, "truong", slug + "_logo", ImageUploadHelper.MAX_AVATAR_SIZE);
            if (!ok)
            {
                litThongBao.Text = $"<div class='alert alert-danger'>Ảnh đại diện: {result}</div>";
                ShowModal = "true";
                return;
            }
            if (isEdit) ImageUploadHelper.DeleteOld(anhDaiDien);
            anhDaiDien = result;
        }

        // Upload ảnh bìa mới nếu có
        if (fuMAnhBia.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuMAnhBia.PostedFile, "truong", slug + "_cover", ImageUploadHelper.MAX_COVER_SIZE);
            if (!ok)
            {
                litThongBao.Text = $"<div class='alert alert-danger'>Ảnh bìa: {result}</div>";
                ShowModal = "true";
                return;
            }
            if (isEdit) ImageUploadHelper.DeleteOld(anhBia);
            anhBia = result;
        }

        var m = new TruongModel
        {
            MaTruong          = maTruong,
            TenTruong         = tenTruong,
            Slug              = slug,
            DiaChi            = txtMDiaChi.Text.Trim(),
            TinhThanh         = ddlMTinhThanh.SelectedValue,
            MaVung            = byte.TryParse(ddlMVung.SelectedValue, out byte v) ? v : (byte?)null,
            LoaiTruong        = byte.TryParse(ddlMLoai.SelectedValue, out byte l) ? l : (byte?)null,
            CapBacDaoTao      = byte.TryParse(ddlMCapBacDaoTao.SelectedValue, out byte c) ? c : (byte?)null,
            SoDienThoai       = txtMSdt.Text.Trim(),
            Website           = txtMWebsite.Text.Trim(),
            QuyMo             = txtMQuyMo.Text.Trim(),
            AnhDaiDien        = anhDaiDien,
            AnhBia            = anhBia,
            MoTa              = txtMMoTa.Text.Trim(),
            KiemDinhChatLuong = chkMKiemDinh.Checked,
            MaTaiKhoan        = SecurityHelper.GetCurrentMaTaiKhoan()
        };

        var (saved, error) = isEdit ? TruongBLL.CapNhat(m) : TruongBLL.Them(m);

        if (saved)
        {
            string action = isEdit ? "Cập nhật" : "Thêm";

            int adminId = SecurityHelper.GetCurrentMaTaiKhoan();
            LogHelper.Ghi(adminId, isEdit ? "CAP_NHAT_TRUONG" : "THEM_TRUONG",
                $"{action} trường: {tenTruong} (MaTruong={maTruong})",
                bangTacDong: "tbl_Truong");

            litThongBao.Text = $"<div class='alert alert-success'><i class='bi bi-check-circle me-2'></i>{action} trường thành công!</div>";
            // Reset form
            hfMaTruong.Value = "0";
            txtMTen.Text = txtMSlug.Text = txtMDiaChi.Text = "";
            ddlMTinhThanh.SelectedIndex = 0;
            txtMSdt.Text = txtMWebsite.Text = txtMQuyMo.Text = txtMMoTa.Text = "";
            ddlMVung.SelectedIndex = 0;
            ddlMLoai.SelectedIndex = 0;
            ddlMCapBacDaoTao.SelectedIndex = 0;
            chkMKiemDinh.Checked = false;
            hfMAnhDaiDien.Value = hfMAnhBia.Value = "";
            litModalTitle.Text = "Thêm trường mới";
            BindData();
        }
        else
        {
            litThongBao.Text = $"<div class='alert alert-danger'>{error}</div>";
            litModalTitle.Text = isEdit ? "Chỉnh sửa trường" : "Thêm trường mới";
            ShowModal = "true";
        }
    }

    private string BuildDeleteDependencyMessage(int maTruong)
    {
        var dependencies = TruongBLL.GetDeleteDependencies(maTruong);
        var parts = new List<string>();
        foreach (var item in dependencies)
        {
            if (item.Value > 0)
                parts.Add($"{item.Key}: {item.Value:N0}");
        }

        if (parts.Count > 0)
            return "CẢNH BÁO LẦN 2: Server đã kiểm tra và phát hiện trường vẫn còn dữ liệu phụ thuộc: " +
                string.Join("; ", parts) +
                ". Thao tác xóa vĩnh viễn không thể hoàn tác. Nếu tiếp tục, hệ thống sẽ yêu cầu nhập mật khẩu admin rồi từ chối xóa cho tới khi xử lý hết dữ liệu phụ thuộc. Tiếp tục đến bước nhập mật khẩu?";

        return "CẢNH BÁO LẦN 2: Server đã kiểm tra và chưa phát hiện dữ liệu phụ thuộc đối với trường này. Thao tác xóa vĩnh viễn không thể hoàn tác. Nếu tiếp tục, hệ thống sẽ yêu cầu nhập mật khẩu admin hiện tại trước khi xóa đúng 1 bản ghi trường. Tiếp tục đến bước nhập mật khẩu?";
    }

    /// <summary>Xác nhận xóa vĩnh viễn trường: bắt buộc nhập lại mật khẩu admin và hard-delete dữ liệu liên quan.</summary>
    protected void btnXacNhanXoaVinhVien_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(hfXoaVinhVienMaTruong.Value, out int id) || id <= 0)
        {
            litThongBao.Text = "<div class='alert-admin danger fade show'><i class='bi bi-exclamation-triangle'></i> Dữ liệu xác nhận xóa vĩnh viễn không hợp lệ.</div>";
            BindData();
            return;
        }

        var truongInfo = TruongBLL.LayChiTietAdmin(id);
        string tenTruong = truongInfo?.TenTruong ?? $"ID={id}";
        int adminId = SecurityHelper.GetCurrentMaTaiKhoan();

        var (ok, error) = TruongBLL.XoaVinhVien(id, adminId, txtXoaVinhVienMatKhau.Text);
        if (ok)
        {
            LogHelper.Ghi(adminId, "XOA_VINH_VIEN_TRUONG",
                $"Xóa vĩnh viễn trường và dữ liệu liên quan: {tenTruong} (MaTruong={id}); đã xác thực mật khẩu admin",
                bangTacDong: "tbl_Truong");

            hfXoaVinhVienMaTruong.Value = "";
            hfCanOpenDeletePasswordModal.Value = "";
            txtXoaVinhVienMatKhau.Text = "";
            litThongBao.Text = "<div class='alert-admin success fade show'><i class='bi bi-check-circle'></i> Đã xóa vĩnh viễn trường, dữ liệu liên quan.</div>";
        }
        else
        {
            LogHelper.Ghi(adminId, "XOA_VINH_VIEN_TRUONG_TU_CHOI",
                $"Từ chối xóa vĩnh viễn trường: {tenTruong} (MaTruong={id}). Lý do: {error}",
                bangTacDong: "tbl_Truong");

            hfCanOpenDeletePasswordModal.Value = "";
            txtXoaVinhVienMatKhau.Text = "";
            litThongBao.Text = $"<div class='alert-admin danger fade show'><i class='bi bi-exclamation-triangle'></i> {Server.HtmlEncode(error)}</div>";
        }

        BindData();
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    /// <summary>Ép GridView render thẻ thead (để CSS match).</summary>
    protected void gvTruong_PreRender(object sender, EventArgs e)
    {
        if (gvTruong.HeaderRow != null)
            gvTruong.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    /// <summary>
    /// Gọi TruongBLL.TimKiem với bộ lọc (tên, tỉnh, trạng thái),
    /// bind vào GridView và tạo nút phân trang.
    /// </summary>
    private void BindData()
    {
        bool? trangThai = null;
        if (ddlTrangThai.SelectedValue == "1")      trangThai = true;
        else if (ddlTrangThai.SelectedValue == "0") trangThai = false;

        var result = TruongBLL.TimKiem(txtFilter.Text.Trim(), ddlTinh.SelectedValue,
            null, null, null, null, null, null, "TenTruong", "ASC", CurrentPage, PAGE_SIZE, trangThai);

        litTong.Text = result.TongSo.ToString("N0");
        gvTruong.DataSource = result.Data;
        gvTruong.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();

        // Cập nhật stats sau mỗi lần bind
        CalculateStats();
    }

    /// <summary>Helper để safely get Website value from DataRowView.</summary>
    protected string GetSafeWebsite(object websiteValue)
    {
        if (websiteValue == null || websiteValue == DBNull.Value)
            return "Chưa có website";
        var url = websiteValue.ToString();
        return string.IsNullOrWhiteSpace(url) ? "Chưa có website" : url;
    }

    /// <summary>Thêm data-field attributes cho các controls trong modal (cho JavaScript).</summary>
    private void AddDataFieldAttributes()
    {
        hfMaTruong.Attributes["data-field"] = "id";
        txtMTen.Attributes["data-field"] = "ten";
        txtMSlug.Attributes["data-field"] = "slug";
        txtMDiaChi.Attributes["data-field"] = "diachi";
        ddlMTinhThanh.Attributes["data-field"] = "tinhthanh";
        ddlMVung.Attributes["data-field"] = "vung";
        ddlMLoai.Attributes["data-field"] = "loai";
        txtMSdt.Attributes["data-field"] = "sdt";
        txtMWebsite.Attributes["data-field"] = "website";
        txtMQuyMo.Attributes["data-field"] = "quymo";
        hfMAnhDaiDien.Attributes["data-field"] = "anhdaiDien";
        hfMAnhBia.Attributes["data-field"] = "anhbia";
        chkMKiemDinh.Attributes["data-field"] = "kiemDinh";
        txtMMoTa.Attributes["data-field"] = "moTa";
    }

    protected string GetKiemDinhBadge(object value)
    {
        if (value == DBNull.Value || value == null) return "";
        bool kd = (bool)value;
        return kd
            ? "<span class='badge badge-soft-success'><i class='bi bi-patch-check-fill me-1'></i>Đạt</span>"
            : "<span class='badge badge-soft-warning'><i class='bi bi-hourglass-split me-1'></i>Chưa</span>";
    }

    protected bool SafeGetBool(object value)
    {
        return value != DBNull.Value && value != null && (bool)value;
    }
}
