using System;
using System.Collections.Generic;
using System.Linq;
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

    /// <summary>Model cho thông tin khu vực trong modal.</summary>
    public class KhuVucInfoModel
    {
        public string MaKV { get; set; }
        public string TenKV { get; set; }
        public decimal DiemCong { get; set; }
        public string MoTa { get; set; }
    }

    /// <summary>Kiểm tra đăng nhập, load dropdown, bind hồ sơ và gợi ý trường.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!User.Identity.IsAuthenticated) { Response.Redirect("~/Client/Login.aspx"); return; }

        if (!IsPostBack)
        {
            LoadDropdowns();
            BindProfile();
            LoadKhuVucInfo();
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

    /// <summary>Load thông tin khu vực vào modal từ dữ liệu hardcode (template CSV).</summary>
    private void LoadKhuVucInfo()
    {
        var data = new List<KhuVucInfoModel>
        {
            new KhuVucInfoModel
            {
                MaKV = "KV1",
                TenKV = "Khu vực 1",
                DiemCong = 0.75m,
                MoTa = "Các xã vùng đồng bào dân tộc thiểu số và miền núi, xã khu vực I, II, III; các xã có thôn vùng đồng bào dân tộc thiểu số và miền núi, thôn đặc biệt khó khăn; các xã/phường hải đảo/đặc khu; các xã/phường biên giới đất liền."
            },
            new KhuVucInfoModel
            {
                MaKV = "KV2-NT",
                TenKV = "Khu vực 2 nông thôn",
                DiemCong = 0.50m,
                MoTa = "Các địa phương không thuộc KV1, KV2, KV3."
            },
            new KhuVucInfoModel
            {
                MaKV = "KV2",
                TenKV = "Khu vực 2",
                DiemCong = 0.25m,
                MoTa = "Các phường thuộc tỉnh; các xã của thành phố trực thuộc Trung ương (trừ các xã thuộc KV1)."
            },
            new KhuVucInfoModel
            {
                MaKV = "KV3",
                TenKV = "Khu vực 3",
                DiemCong = 0m,
                MoTa = "Các phường của thành phố trực thuộc Trung ương."
            }
        };

        rptKhuVucInfo.DataSource = data;
        rptKhuVucInfo.DataBind();
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

        // Xử lý huy hiệu vài trò và Form nhập
        if (tk != null) 
        {
            string badgeClass = "bg-primary";
            if (tk.IsAdmin) badgeClass = "bg-danger";
            else if (tk.IsTruongHoc) badgeClass = "bg-success";
            else if (tk.IsModerator) badgeClass = "bg-warning text-dark";
            else if (tk.IsTuVanVien) badgeClass = "bg-info text-dark";
            else badgeClass = "bg-secondary";
            
            litVaiTroBadge.Text = $"<span class='badge {badgeClass} fw-normal px-3 py-1 mt-2'>{tk.TenQuyen}</span>";

            // Giới hạn form thông tin riêng của học sinh
            pnlThongTinHocSinh.Visible = tk.IsHocSinh;

            // Xử lý thẻ Quick Actions
            litQuickActions.Text = "";
            pnlQuickActions.Visible = true;
            if (tk.IsAdmin || tk.IsModerator) {
                litQuickActions.Text += $"<a href='/Admin/Default.aspx' class='btn btn-sm btn-outline-danger w-100 mb-2'><i class='bi bi-gear me-1'></i>Control Panel (Admin)</a>";
            }
            else if (tk.IsTruongHoc) {
                litQuickActions.Text += $"<a href='/TruongHoc/Default.aspx' class='btn btn-sm btn-outline-success w-100 mb-2'><i class='bi bi-building me-1'></i>Quản lý trường</a>";
                if (tk.MaTruong.HasValue) {
                    var tr = TruongDAL.LayChiTiet(tk.MaTruong.Value);
                    if (tr != null && !string.IsNullOrEmpty(tr.Slug)) {
                        litQuickActions.Text += $"<a href='/Client/ChiTietTruong.aspx?slug={tr.Slug}' class='btn btn-sm btn-primary w-100 mb-2'><i class='bi bi-eye me-1'></i>Xem trang Public</a>";
                    }
                }
            }
            else if (tk.IsTuVanVien) {
                litQuickActions.Text += $"<a href='/TruongHoc/GopYTuVan.aspx' class='btn btn-sm btn-outline-info w-100 mb-2 text-dark'><i class='bi bi-chat-dots me-1'></i>Quản lý Tư vấn</a>";
            }
            else {
                // For students, show standard quick actions from mockup
                pnlQuickActions.Visible = true;
                litQuickActions.Text = $@"
                    <a href='/Client/WishList.aspx' class='quick-action-btn'>
                        <i class='bi bi-heart text-danger'></i>
                        <span>Trường yêu thích</span>
                    </a>
                    <a href='/Client/HopThu.aspx' class='quick-action-btn'>
                        <i class='bi bi-chat-dots text-success'></i>
                        <span>Tin nhắn</span>
                    </a>
                    <a href='/Client/MyProfile.aspx' class='quick-action-btn'>
                        <i class='bi bi-gear text-secondary'></i>
                        <span>Cài đặt</span>
                    </a>";
            }
        }

        if (p == null) return;

        txtHoTen.Text   = p.HoTen ?? "";
        litHoTen.Text   = Server.HtmlEncode(p.HoTen ?? "(Chưa cập nhật)");
        if (pnlThongTinHocSinh.Visible)
        {
            if (p.NgaySinh.HasValue) txtNgaySinh.Text = p.NgaySinh.Value.ToString("yyyy-MM-dd");
            ddlTinh.SelectedValue   = p.TinhThanh ?? "";
            ddlKhuVuc.SelectedValue = p.KhuVuc?.ToString() ?? "";
            // Luôn hiển thị điểm dự kiến với InvariantCulture (dấu chấm) để input type="number" nhận đúng
            txtDiem.Text = p.DiemDuKien.HasValue
                ? p.DiemDuKien.Value.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)
                : "";
            if (p.MaChuyenNganh.HasValue) ddlNganh.SelectedValue = p.MaChuyenNganh.Value.ToString();
            txtMucTieu.Text = p.MucTieuNghe ?? "";
        }

        // Set sidebar stats for student users
        if (tk.IsHocSinh && p != null)
        {
            litDiemDuKien.Text = p.DiemDuKien.HasValue ? p.DiemDuKien.Value.ToString("F1") : "Chưa có";

            if (p.KhuVuc.HasValue)
            {
                litKhuVuc.Text = p.KhuVuc.Value switch
                {
                    1 => "KV1",
                    2 => "KV2",
                    3 => "KV3",
                    _ => "Không xác định"
                };
            }
            else
            {
                litKhuVuc.Text = "Chưa có";
            }

            if (p.MaChuyenNganh.HasValue)
            {
                var nganhList = DanhMucDAL.GetChuyenNganh();
                var nganh = nganhList.FirstOrDefault(n => n.Id == p.MaChuyenNganh.Value);
                litNganhQuanTam.Text = nganh?.Ten ?? "Không xác định";
            }
            else
            {
                litNganhQuanTam.Text = "Chưa có";
            }
        }
    }

    /// <summary>
    /// Xử lý nút Lưu hồ sơ — upload avatar (nếu có), thu thập dữ liệu form,
    /// gọi ProfileBLL.LuuProfile và hiển thị kết quả.
    /// </summary>
    protected void btnLuu_Click(object sender, EventArgs e)
    {
        // Xử lý upload avatar
        string avatarPath = hfAvatar.Value;
        if (avatarUpload.HasFile)
        {
            string email = User.Identity.Name ?? "user";
            var (ok, result) = ImageUploadHelper.Upload(
                avatarUpload.PostedFile, "avatar", email + "_avatar", ImageUploadHelper.MAX_AVATAR_SIZE);
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
            NgaySinh      = pnlThongTinHocSinh.Visible && DateTime.TryParse(txtNgaySinh.Text, out DateTime ns) ? ns : (DateTime?)null,
            TinhThanh     = pnlThongTinHocSinh.Visible ? ddlTinh.SelectedValue : null,
            KhuVuc        = pnlThongTinHocSinh.Visible && byte.TryParse(ddlKhuVuc.SelectedValue, out byte kv) ? kv : (byte?)null,
            DiemDuKien    = pnlThongTinHocSinh.Visible && decimal.TryParse(txtDiem.Text,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out decimal d)
                                ? d : (decimal?)null,
            MaChuyenNganh = pnlThongTinHocSinh.Visible && int.TryParse(ddlNganh.SelectedValue, out int n) ? n : (int?)null,
            MucTieuNghe   = pnlThongTinHocSinh.Visible ? txtMucTieu.Text.Trim() : null,
            AnhDaiDien    = avatarPath
        };

        var (saved, error) = ProfileBLL.LuuProfile(m);

        if (saved)
        {
            hfAvatar.Value    = avatarPath;
            imgAvatar.ImageUrl = !string.IsNullOrEmpty(avatarPath) ? avatarPath : DEFAULT_AVATAR;
            litHoTen.Text     = Server.HtmlEncode(m.HoTen ?? "");
            litThongBao.Text  = "<div class='alert alert-success'><i class='bi bi-check-circle me-2'></i>Lưu hồ sơ thành công!</div>";

            // Refresh all profile data after save
            BindProfile();
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

}
