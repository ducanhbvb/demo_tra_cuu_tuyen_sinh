using System;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Hồ sơ cá nhân — cho phép xem/sửa thông tin cá nhân, upload avatar,
/// đổi mật khẩu và xem gợi ý trường phù hợp dựa trên điểm dự kiến.
/// </summary>
public partial class Profile_MyProfile : Page
{
    /// <summary>Mã tài khoản đang đăng nhập.</summary>
    private int MaTK => SecurityHelper.GetCurrentMaTaiKhoan();

    private const string DEFAULT_AVATAR = "/Resources/Images/no-image.png";

    /// <summary>Kiểm tra đăng nhập, load dropdown, bind hồ sơ và gợi ý trường.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated) { Response.Redirect("~/Client/Login.aspx"); return; }

        if (!IsPostBack)
        {
            LoadDropdowns();
            BindProfile();
            LoadGoiY();
        }
    }

    /// <summary>Load danh sách tỉnh/thành và ngành vào dropdown.</summary>
    private void LoadDropdowns()
    {
        foreach (var t in DanhMucDAL.GetTinhThanh())
            ddlTinh.Items.Add(new ListItem(t, t));
        foreach (var n in DanhMucDAL.GetChuyenNganh())
            ddlNganh.Items.Add(new ListItem(n.Ten, n.Id.ToString()));
    }

    /// <summary>Lấy hồ sơ từ DB và bind vào các control form (avatar, họ tên, ngày sinh…).</summary>
    private void BindProfile()
    {
        var tk = TaiKhoanDAL.GetById(MaTK);
        if (tk != null) litEmail.Text = Server.HtmlEncode(tk.Email);

        var p = ProfileBLL.GetProfile(MaTK);

        // Hiển thị avatar
        string avatarPath = p?.AnhDaiDien;
        hfAvatar.Value        = avatarPath ?? "";
        imgAvatar.ImageUrl     = !string.IsNullOrEmpty(avatarPath) ? avatarPath : DEFAULT_AVATAR;

        if (p == null) return;

        txtHoTen.Text   = p.HoTen ?? "";
        litHoTen.Text   = Server.HtmlEncode(p.HoTen ?? "(Chưa cập nhật)");
        if (p.NgaySinh.HasValue) txtNgaySinh.Text = p.NgaySinh.Value.ToString("yyyy-MM-dd");
        ddlTinh.SelectedValue   = p.TinhThanh ?? "";
        ddlKhuVuc.SelectedValue = p.KhuVuc?.ToString() ?? "";
        if (p.DiemDuKien.HasValue) txtDiem.Text = p.DiemDuKien.Value.ToString("F1");
        if (p.MaChuyenNganh.HasValue) ddlNganh.SelectedValue = p.MaChuyenNganh.Value.ToString();
        txtMucTieu.Text = p.MucTieuNghe ?? "";
    }

    /// <summary>
    /// Xử lý nút Lưu hồ sơ — upload avatar (nếu có), thu thập dữ liệu form,
    /// gọi ProfileBLL.LuuProfile và hiển thị kết quả.
    /// </summary>
    protected void btnLuu_Click(object sender, EventArgs e)
    {
        // Xử lý upload avatar
        string avatarPath = hfAvatar.Value;
        if (fuAvatar.HasFile)
        {
            string email = User.Identity.Name ?? "user";
            var (ok, result) = ImageUploadHelper.Upload(
                fuAvatar.PostedFile, "avatar", email + "_avatar", ImageUploadHelper.MAX_AVATAR_SIZE);
            if (ok)
            {
                ImageUploadHelper.DeleteOld(avatarPath);
                avatarPath = result;
            }
            else
            {
                litThongBao.Text = $"<div class='alert alert-warning'><i class='bi bi-exclamation-triangle me-2'></i>Ảnh đại diện: {result}</div>";
            }
        }

        var m = new ProfileHocSinhModel
        {
            MaTaiKhoan    = MaTK,
            HoTen         = txtHoTen.Text.Trim(),
            NgaySinh      = DateTime.TryParse(txtNgaySinh.Text, out DateTime ns) ? ns : (DateTime?)null,
            TinhThanh     = ddlTinh.SelectedValue,
            KhuVuc        = byte.TryParse(ddlKhuVuc.SelectedValue, out byte kv) ? kv : (byte?)null,
            DiemDuKien    = decimal.TryParse(txtDiem.Text, out decimal d) ? d : (decimal?)null,
            MaChuyenNganh = int.TryParse(ddlNganh.SelectedValue, out int n) ? n : (int?)null,
            MucTieuNghe   = txtMucTieu.Text.Trim(),
            AnhDaiDien    = avatarPath
        };

        var (saved, error) = ProfileBLL.LuuProfile(m);

        if (saved)
        {
            hfAvatar.Value    = avatarPath;
            imgAvatar.ImageUrl = !string.IsNullOrEmpty(avatarPath) ? avatarPath : DEFAULT_AVATAR;
            litHoTen.Text     = Server.HtmlEncode(m.HoTen ?? "");
            litThongBao.Text  = "<div class='alert alert-success'><i class='bi bi-check-circle me-2'></i>Lưu hồ sơ thành công!</div>";
        }
        else
        {
            litThongBao.Text = $"<div class='alert alert-danger'>{error}</div>";
        }
    }

    /// <summary>Xử lý nút Đổi mật khẩu — validate xác nhận, gọi TaiKhoanBLL.DoiMatKhau.</summary>
    protected void btnDoiMK_Click(object sender, EventArgs e)
    {
        if (txtMKMoi.Text != txtMKXacNhan.Text)
        {
            litMKThongBao.Text = "<div class='alert alert-danger'>Mật khẩu xác nhận không khớp.</div>";
            return;
        }
        var (ok, error) = TaiKhoanBLL.DoiMatKhau(MaTK, txtMKCu.Text, txtMKMoi.Text);
        litMKThongBao.Text = ok
            ? "<div class='alert alert-success'>Đổi mật khẩu thành công!</div>"
            : $"<div class='alert alert-danger'>{error}</div>";
        if (ok) txtMKCu.Text = txtMKMoi.Text = txtMKXacNhan.Text = "";
    }

    /// <summary>
    /// Load gợi ý trường phù hợp dựa trên điểm dự kiến, ngành và khu vực
    /// từ hồ sơ học sinh (tối đa 6 trường).
    /// </summary>
    private void LoadGoiY()
    {
        var p = ProfileBLL.GetProfile(MaTK);
        if (p == null || !p.DiemDuKien.HasValue)
        {
            if (pnlGoiY != null) pnlGoiY.Visible = false;
            return;
        }

        var dt = GoiYTruongDAL.GoiY(p.DiemDuKien, p.MaChuyenNganh, p.KhuVuc, top: 6);
        if (dt.Rows.Count == 0)
        {
            if (pnlGoiY != null) pnlGoiY.Visible = false;
            return;
        }

        if (pnlGoiY != null)
        {
            pnlGoiY.Visible    = true;
            rptGoiY.DataSource = dt;
            rptGoiY.DataBind();
            litDiemGoiY.Text   = p.DiemDuKien.Value.ToString("F1");
        }
    }
}
