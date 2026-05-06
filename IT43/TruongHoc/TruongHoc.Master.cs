using System;
using System.Web.UI;

/// <summary>
/// Master Page khu vực Trường học — chỉ cho tài khoản MaQuyen=2 (TruongHoc).
/// Hiển thị tên trường + email trong sidebar.
/// </summary>
public partial class MasterPages_TruongHoc : MasterPage
{
    /// <summary>
    /// Kiểm tra quyền TruongHoc (redirect nếu không hợp lệ),
    /// load thông tin trường hiển thị trên sidebar.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Chưa đăng nhập
        if (!Context.User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/Client/Login.aspx?returnUrl=" +
                Server.UrlEncode(Request.RawUrl));
            return;
        }

        // Không phải TruongHoc → về trang chủ
        if (!Context.User.IsInRole("TruongHoc"))
        {
            Response.Redirect("~/Client/index.aspx");
            return;
        }

        if (!IsPostBack)
        {
            int maTK     = SecurityHelper.GetCurrentMaTaiKhoan();
            int maTruong = SecurityHelper.GetCurrentMaTruong();

            if (maTK > 0)
            {
                var tk = TaiKhoanDAL.GetById(maTK);
                if (tk != null)
                    litEmail.Text = Server.HtmlEncode(tk.Email);
            }

            if (maTruong > 0)
            {
                var truong = TruongBLL.LayChiTiet(maTruong);
                if (truong != null)
                    litTenTruong.Text = Server.HtmlEncode(truong.TenTruong);
                else
                    litTenTruong.Text = "Chưa gán trường";
            }
            else
            {
                litTenTruong.Text = "Chưa gán trường";
            }
        }
    }
}
