using System;
using System.Web.UI;

/// <summary>
/// Trang Thêm/Chỉnh sửa trường (Admin) — form nhập liệu thông tin trường,
/// upload ảnh đại diện/ảnh bìa, lưu qua TruongBLL.
/// </summary>
public partial class Admin_ChinhSuaTruong : Page
{
    /// <summary>Mã trường từ QueryString (0 = thêm mới).</summary>
    private new int ID => int.TryParse(Request.QueryString["id"], out int i) ? i : 0;

    /// <summary>True nếu đang chỉnh sửa (có ID), false nếu thêm mới.</summary>
    private bool IsEdit => ID > 0;

    /// <summary>Thiết lập tiêu đề trang và load dữ liệu trường nếu đang chỉnh sửa.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        litTieuDe.Text = IsEdit ? "Chỉnh sửa trường" : "Thêm trường mới";
        if (!IsPostBack && IsEdit) LoadTruong();
    }

    /// <summary>Load thông tin trường từ DB vào các control form và preview ảnh.</summary>
    private void LoadTruong()
    {
        var m = TruongBLL.LayChiTiet(ID);
        if (m == null) { Response.Redirect("QuanLyTruong.aspx"); return; }

        txtTen.Text       = m.TenTruong;
        txtSlug.Text      = m.Slug;
        txtDiaChi.Text    = m.DiaChi;
        txtTinhThanh.Text = m.TinhThanh;
        ddlVung.SelectedValue = m.MaVung?.ToString() ?? "";
        ddlLoai.SelectedValue = m.LoaiTruong?.ToString() ?? "";
        txtSdt.Text       = m.SoDienThoai;
        txtWebsite.Text   = m.Website;
        txtQuyMo.Text     = m.QuyMo;
        txtMoTa.Text      = m.MoTa;
        chkKiemDinh.Checked = m.KiemDinhChatLuong;

        // Lưu path hiện tại vào HiddenField để giữ lại nếu không upload mới
        hfAnhDaiDien.Value = m.AnhDaiDien ?? "";
        hfAnhBia.Value     = m.AnhBia ?? "";

        // Hiển thị preview ảnh hiện tại
        string fallback = ResolveUrl("~/Resources/Images/no-image.png");
        imgPreviewAvatar.ImageUrl = !string.IsNullOrEmpty(m.AnhDaiDien) ? m.AnhDaiDien : fallback;
        imgPreviewBia.ImageUrl    = !string.IsNullOrEmpty(m.AnhBia)     ? m.AnhBia     : fallback;
    }

    /// <summary>
    /// Xử lý nút Lưu — upload ảnh mới (nếu có), thu thập dữ liệu form,
    /// gọi TruongBLL.Them hoặc TruongBLL.CapNhat tùy theo chế độ.
    /// </summary>
    protected void btnLuu_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;

        // Bắt đầu với ảnh cũ (từ HiddenField)
        string anhDaiDien = hfAnhDaiDien.Value;
        string anhBia     = hfAnhBia.Value;

        string slug = txtSlug.Text.Trim();
        if (string.IsNullOrEmpty(slug)) slug = "truong";

        // Upload ảnh đại diện mới nếu có
        if (fuAnhDaiDien.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuAnhDaiDien.PostedFile, "truong", slug + "_logo", ImageUploadHelper.MAX_AVATAR_SIZE);
            if (!ok)
            {
                litThongBao.Text = $"<div class='alert alert-danger'><i class='bi bi-exclamation-triangle me-2'></i>Ảnh đại diện: {result}</div>";
                return;
            }
            ImageUploadHelper.DeleteOld(anhDaiDien);
            anhDaiDien = result;
        }

        // Upload ảnh bìa mới nếu có
        if (fuAnhBia.HasFile)
        {
            var (ok, result) = ImageUploadHelper.Upload(
                fuAnhBia.PostedFile, "truong", slug + "_cover", ImageUploadHelper.MAX_COVER_SIZE);
            if (!ok)
            {
                litThongBao.Text = $"<div class='alert alert-danger'><i class='bi bi-exclamation-triangle me-2'></i>Ảnh bìa: {result}</div>";
                return;
            }
            ImageUploadHelper.DeleteOld(anhBia);
            anhBia = result;
        }

        var m = new TruongModel
        {
            MaTruong          = ID,
            TenTruong         = txtTen.Text.Trim(),
            Slug              = txtSlug.Text.Trim(),
            DiaChi            = txtDiaChi.Text.Trim(),
            TinhThanh         = txtTinhThanh.Text.Trim(),
            MaVung            = byte.TryParse(ddlVung.SelectedValue, out byte v) ? v : (byte?)null,
            LoaiTruong        = byte.TryParse(ddlLoai.SelectedValue, out byte l) ? l : (byte?)null,
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
            Response.Redirect("QuanLyTruong.aspx?msg=saved");
        else
            litThongBao.Text = $"<div class='alert alert-danger'>{error}</div>";
    }
}
