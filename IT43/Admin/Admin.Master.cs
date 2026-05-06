using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Master Page phía Admin — bảo vệ truy cập bằng kiểm tra role Admin/Moderator/TuVanVien,
/// hiển thị email + role người dùng trên sidebar.
/// </summary>
public partial class MasterPages_Admin : MasterPage
{
    /// <summary>
    /// Kiểm tra quyền truy cập /Admin/ (Admin, Moderator, TuVanVien).
    /// Redirect về Login nếu chưa đăng nhập hoặc không có quyền.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Chưa đăng nhập → về Login
        if (!Context.User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/Client/Login.aspx?returnUrl=" +
                Server.UrlEncode(Request.RawUrl));
            return;
        }

        // Đăng nhập nhưng không có quyền vào Admin → về trang chủ
        if (!SecurityHelper.CanAccessAdmin())
        {
            Response.Redirect("~/Client/index.aspx");
            return;
        }

        if (!IsPostBack)
        {
            // Ẩn/hiện menu theo role
            bool isAdmin     = SecurityHelper.IsAdmin();
            bool isModerator = SecurityHelper.IsModerator();
            bool canContent  = SecurityHelper.CanManageContent();   // Admin + Moderator
            bool canView     = SecurityHelper.CanViewContent();     // Admin + Moderator + TuVanVien

            pnlMenuNoidung.Visible  = canView;              // TuVanVien thấy menu Nội dung (chỉ xem)
            pnlMenuTaiKhoan.Visible = isAdmin || isModerator; // Moderator thấy menu TK (chỉ xem)
            pnlMenuHeThong.Visible  = isAdmin;              // Logs + Cài đặt: chỉ Admin

            // Hiển thị badge tư vấn chờ xử lý (màu tím #8b5cf6, dot tròn)
            int soTuVanCho = ThongKeBLL.SoTuVanCho();
            if (soTuVanCho > 0)
                litBadgeTuVan.Text = $"<span class='badge ms-auto' style='background:#8b5cf6;border-radius:50%;min-width:20px;height:20px;padding:0 5px;font-size:10px;font-weight:700;line-height:20px;display:inline-flex;align-items:center;justify-content:center;box-shadow:0 0 0 2.5px rgba(255,255,255,0.9);'>{soTuVanCho}</span>";

            // Hiển thị email + role người dùng
            int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
            if (maTK > 0)
            {
                var tk = TaiKhoanDAL.GetById(maTK);
                if (tk != null)
                    litAdminEmail.Text = Server.HtmlEncode($"{tk.Email} ({tk.TenQuyen})");
            }
        }
    }
}
