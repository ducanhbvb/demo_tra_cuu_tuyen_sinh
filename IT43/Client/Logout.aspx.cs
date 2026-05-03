using System;
using System.Web.UI;

/// <summary>
/// Trang Đăng xuất — xóa cookie xác thực và redirect về trang chủ.
/// </summary>
public partial class Account_Logout : Page
{
    /// <summary>Gọi SecurityHelper.SignOut để xóa session/cookie, sau đó redirect.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        SecurityHelper.SignOut();
        Response.Redirect("~/Client/index.aspx");
    }
}
