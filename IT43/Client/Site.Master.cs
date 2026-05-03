using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Master Page phía Client — quản lý navbar, hiển thị menu Guest/User/Admin
/// dựa trên trạng thái xác thực và role.
/// </summary>
public partial class MasterPages_Site : MasterPage
{
    /// <summary>
    /// Xác định hiển thị menu Guest hay User, ẩn/hiện menu Admin,
    /// và lấy email người dùng từ Session (fallback query DB nếu cần).
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Context.User.Identity.IsAuthenticated)
        {
            phGuest.Visible     = false;
            phUser.Visible      = true;
            phAdminMenu.Visible = Context.User.IsInRole("Admin");

            // Đọc email từ Session — không cần query DB mỗi request
            string email = Session["UserEmail"]?.ToString();

            // Fallback: nếu Session hết hạn thì query DB 1 lần và cập nhật lại Session
            if (string.IsNullOrEmpty(email))
            {
                int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
                if (maTK > 0)
                {
                    var tk = TaiKhoanDAL.GetById(maTK);
                    email = tk?.Email ?? "";
                    Session["UserEmail"] = email;
                }
            }

            if (!string.IsNullOrEmpty(email))
                litEmail.Text = Server.HtmlEncode(email.Length > 20 ? email.Substring(0, 18) + "..." : email);
        }
    }
}
