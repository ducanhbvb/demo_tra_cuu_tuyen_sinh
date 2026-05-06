using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý tài khoản (Admin) — danh sách tài khoản với bộ lọc quyền/email,
/// phân trang, toggle trạng thái, tạo tài khoản mới, sửa quyền, reset mật khẩu.
/// Chỉ Admin mới được tạo/sửa/reset. Trang này nằm trong /Admin/ đã được bảo vệ.
/// </summary>
public partial class Admin_QuanLyTaiKhoan : Page
{
    private const int PAGE_SIZE = 20;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Khởi tạo trang: Admin toàn quyền, Moderator chỉ xem danh sách.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Moderator được xem danh sách tài khoản nhưng không thao tác
        // (Web.config đã cho phép Moderator vào /Admin/, page-level guard ở đây)
        gvTK.PreRender += gvTK_PreRender;
        if (!IsPostBack)
        {
            // Nút tạo tài khoản chỉ hiện với Admin
            pnlBtnTaoTK.Visible = SecurityHelper.IsAdmin();
            BindTruongDropdowns();
            BindStats();
            BindData();
        }
        else
        {
            // EnableViewState="false" → rebind để LinkButton trong GridView tồn tại trước RowCommand
            BindData();
        }
    }

    /// <summary>Xử lý nút Lọc — reset về trang đầu và tìm kiếm lại.</summary>
    protected void btnLoc_Click(object sender, EventArgs e) { CurrentPage = 0; BindData(); }

    /// <summary>
    /// Xử lý các command trên GridView:
    /// ToggleTT      — kích hoạt/khóa tài khoản (chỉ Admin).
    /// SuaQuyen      — mở modal sửa quyền (chỉ Admin).
    /// GuiEmailReset — gửi email reset mật khẩu qua link (Cấp 2).
    /// ResetMK       — mở modal đặt MK trực tiếp (Cấp 3, chỉ Admin).
    /// </summary>
    protected void gvTK_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        bool isAdmin     = SecurityHelper.IsAdmin();
        bool isModerator = SecurityHelper.IsModerator();

        // Moderator chỉ được gửi email reset mật khẩu, không được các thao tác khác
        if (!isAdmin)
        {
            if (isModerator && e.CommandName == "GuiEmailReset")
            {
                // Cho phép Moderator gửi email reset — xử lý bên dưới
            }
            else
            {
                ShowMsg("danger", "<i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền thực hiện thao tác này.");
                return;
            }
        }

        // Reset cả 2 hidden field trước mỗi command
        // để JS không mở lại modal cũ từ ViewState lần trước
        hfResetMaTK.Value = "";
        hfSuaMaTK.Value   = "";

        int currentAdminId = SecurityHelper.GetCurrentMaTaiKhoan();

        switch (e.CommandName)
        {
            case "ToggleTT":
            {
                var parts = e.CommandArgument.ToString().Split(',');
                int id   = int.Parse(parts[0]);
                bool cur = parts[1] == "True";

                // ★ LỚP 1 — Chặn admin tự khóa chính mình
                if (id == currentAdminId)
                {
                    ShowMsg("danger", "<i class='bi bi-shield-exclamation me-1'></i>Không thể thay đổi trạng thái tài khoản của chính bạn.");
                    BindData();
                    return;
                }

                TaiKhoanBLL.CapNhatTrangThai(id, !cur);
                ShowMsg("success", cur ? "Đã vô hiệu hóa tài khoản." : "Đã kích hoạt tài khoản.");
                ClearFilter();
                BindData();
                break;
            }
            case "SuaQuyen":
            {
                // format: MaTaiKhoan|Email|MaQuyen
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);

                // ★ LỚP 1 — Chặn admin tự sửa quyền chính mình
                if (id == currentAdminId)
                {
                    ShowMsg("danger", "<i class='bi bi-shield-exclamation me-1'></i>Không thể sửa quyền tài khoản của chính bạn.");
                    BindData();
                    return;
                }

                hfSuaMaTK.Value  = parts[0];
                litSuaEmail.Text = Server.HtmlEncode(parts[1]);
                ddlSuaQuyen.SelectedValue = parts[2];
                // Preset dropdown Trường nếu quyền hiện tại là TruongHoc
                if (parts[2] == "2")
                {
                    var tk = TaiKhoanBLL.GetById(int.Parse(parts[0]));
                    if (tk != null && tk.MaTruong.HasValue)
                        ddlSuaTruong.SelectedValue = tk.MaTruong.Value.ToString();
                }
                // JS sẽ mở modal dựa vào hfSuaMaTK.Value != ''
                ClearFilter();
                BindData();
                break;
            }
            case "GuiEmailReset":
            {
                // format: MaTaiKhoan|Email
                var parts = e.CommandArgument.ToString().Split('|');
                int id = int.Parse(parts[0]);
                var (ok, err) = TaiKhoanBLL.AdminGuiEmailReset(id);
                if (ok)
                    ShowMsg("success", $"Đã gửi email reset mật khẩu tới <strong>{Server.HtmlEncode(parts[1])}</strong>.");
                else
                    ShowMsg("danger", Server.HtmlEncode(err ?? "Không gửi được email."));
                ClearFilter();
                BindData();
                break;
            }
            case "ResetMK":
            {
                // format: MaTaiKhoan|Email
                var parts = e.CommandArgument.ToString().Split('|');
                hfResetMaTK.Value  = parts[0];
                litResetEmail.Text = Server.HtmlEncode(parts[1]);
                // JS sẽ mở modal dựa vào hfResetMaTK.Value != ''
                ClearFilter();
                BindData();
                break;
            }
        }
    }

    /// <summary>Xử lý chuyển trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page") { CurrentPage = int.Parse(e.CommandArgument.ToString()); BindData(); }
    }

    /// <summary>Tạo tài khoản mới — chỉ Admin.</summary>
    protected void btnTaoTK_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;
        if (!SecurityHelper.IsAdmin())
        {
            litTaoLoi.Text = Alert("danger", "<i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền tạo tài khoản.");
            return;
        }

        string email = txtTaoEmail.Text.Trim().ToLower();
        int maQuyen  = int.Parse(ddlTaoQuyen.SelectedValue);
        string mk    = txtTaoMK.Text;

        // Lấy maTruong nếu quyền = TruongHoc (2)
        int? maTruong = null;
        if (maQuyen == 2)
        {
            if (!int.TryParse(ddlTaoTruong.SelectedValue, out int maTr) || maTr <= 0)
            {
                litTaoLoi.Text = Alert("danger", "Vui lòng chọn trường khi tạo tài khoản Trường học.");
                return;
            }
            maTruong = maTr;
        }

        var (ok, err) = TaiKhoanBLL.AdminTaoTaiKhoan(email, mk, maQuyen, maTruong);
        if (ok)
        {
            txtTaoEmail.Text = "";
            txtTaoMK.Text    = "";
            ShowMsg("success", $"Đã tạo tài khoản <strong>{Server.HtmlEncode(email)}</strong> thành công.");
            BindData();
        }
        else
        {
            litTaoLoi.Text = Alert("danger", Server.HtmlEncode(err));
        }
    }

    /// <summary>Lưu quyền mới — chỉ Admin.</summary>
    protected void btnSuaQuyen_Click(object sender, EventArgs e)
    {
        if (!SecurityHelper.IsAdmin())
        {
            litSuaLoi.Text = Alert("danger", "<i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền sửa quyền tài khoản.");
            return;
        }

        int maTK    = int.Parse(hfSuaMaTK.Value);
        int maQuyen = int.Parse(ddlSuaQuyen.SelectedValue);

        // Lấy maTruong nếu quyền = TruongHoc (2)
        int? maTruong = null;
        if (maQuyen == 2)
        {
            if (!int.TryParse(ddlSuaTruong.SelectedValue, out int maTr) || maTr <= 0)
            {
                litSuaLoi.Text = Alert("danger", "Vui lòng chọn trường khi đặt quyền Trường học.");
                return;
            }
            maTruong = maTr;
        }

        var (ok, err) = TaiKhoanBLL.AdminCapNhatQuyen(maTK, maQuyen, maTruong);
        if (ok)
        {
            hfSuaMaTK.Value = "";
            ShowMsg("success", "Đã cập nhật quyền thành công.");
            BindData();
        }
        else
        {
            litSuaLoi.Text = Alert("danger", Server.HtmlEncode(err));
        }
    }

    /// <summary>Đặt mật khẩu mới trực tiếp — chỉ Admin.</summary>
    protected void btnResetMK_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;
        if (!SecurityHelper.IsAdmin())
        {
            litResetLoi.Text = Alert("danger", "<i class='bi bi-shield-exclamation me-1'></i>Bạn không có quyền đặt lại mật khẩu.");
            return;
        }

        int maTK  = int.Parse(hfResetMaTK.Value);
        string mk = txtResetMK.Text;

        var (ok, err) = TaiKhoanBLL.AdminDatMatKhauTrucTiep(maTK, mk);
        if (ok)
        {
            // AdminDatMatKhauTrucTiep đã set YeuCauDoiMatKhau=1 trong DB
            hfResetMaTK.Value = "";
            txtResetMK.Text   = "";
            ShowMsg("success", "Đã đặt lại mật khẩu. User sẽ phải đổi mật khẩu lần đăng nhập kế tiếp.");
            BindData();
        }
        else
        {
            litResetLoi.Text = Alert("danger", Server.HtmlEncode(err));
        }
    }

    // ─────────────────────────────────────────────
    // Private helpers
    // ─────────────────────────────────────────────

    /// <summary>Load danh sách trường vào dropdown Tạo TK + Sửa quyền.</summary>
    private void BindTruongDropdowns()
    {
        var dsTruong = TruongBLL.GetDanhSachDropdown();   // List<LookupModel> {Id, Ten}
        foreach (var item in dsTruong)
        {
            ddlTaoTruong.Items.Add(new ListItem(item.Ten, item.Id.ToString()));
            ddlSuaTruong.Items.Add(new ListItem(item.Ten, item.Id.ToString()));
        }
    }

    protected void gvTK_PreRender(object sender, EventArgs e)
    {
        if (gvTK.HeaderRow != null)
            gvTK.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    private void BindStats()
    {
        var stats = ThongKeBLL.ThongKeTaiKhoan();
        litTongTK.Text = stats.tong.ToString("N0");
        litPhanQuyen.Text = $"{stats.qAdmin} / {stats.qTruong} / {stats.qUser}";
        litChuaXN.Text = stats.chuaXacNhan.ToString("N0");
        litHoatDong.Text = stats.dangHoatDong.ToString("N0");
    }

    private void BindData()
    {
        int? quyen = int.TryParse(ddlQuyen.SelectedValue, out int q) ? q : (int?)null;
        var result = TaiKhoanBLL.GetDanhSach(CurrentPage, PAGE_SIZE, quyen, txtEmail.Text.Trim());

        litTong.Text = result.TongSo.ToString("N0");
        gvTK.DataSource = result.Data;
        gvTK.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }

    /// <summary>Xóa bộ lọc email và quyền — gọi trước BindData() trong các action commands.</summary>
    private void ClearFilter()
    {
        txtEmail.Text = "";
        ddlQuyen.SelectedIndex = 0;
        CurrentPage = 0;
    }

    private void ShowMsg(string type, string msg)
        => litThongBao.Text = Alert(type, msg);

    private static string Alert(string type, string msg)
        => $"<div class='alert alert-{type} alert-dismissible fade show' role='alert'>" +
           $"{msg}<button type='button' class='btn-close' data-bs-dismiss='alert'></button></div>";
}
