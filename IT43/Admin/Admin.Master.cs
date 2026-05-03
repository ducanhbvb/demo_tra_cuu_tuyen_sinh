using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Master Page phía Admin — bảo vệ truy cập bằng kiểm tra role Admin,
/// hiển thị email quản trị viên trên sidebar.
/// </summary>
public partial class MasterPages_Admin : MasterPage
{
    /// <summary>
    /// Kiểm tra role Admin (redirect Login nếu không có quyền),
    /// lấy email admin từ DB để hiển thị trên giao diện.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Context.User.IsInRole("Admin"))
        {
            Response.Redirect("~/login.aspx?returnUrl=" +
                Server.UrlEncode(Request.RawUrl));
        }

        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK > 0)
        {
            var tk = TaiKhoanDAL.GetById(maTK);
            if (tk != null) litAdminEmail.Text = Server.HtmlEncode(tk.Email);
        }
    }
}
