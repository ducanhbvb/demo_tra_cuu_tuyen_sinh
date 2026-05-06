using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

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
            phGuest.Visible = false;
            phUser.Visible  = true;

            // Hiện menu "Quản trị" cho mọi role trừ HocSinh
            string role = SecurityHelper.GetCurrentRole();
            bool hasPanel = !string.IsNullOrEmpty(role) && role != "HocSinh";
            phAdminMenu.Visible = hasPanel;

            // Bell icon HopThu — luôn hiện với HocSinh, badge đỏ khi có tin chưa đọc
            if (role == "HocSinh")
            {
                phHopThu.Visible = true;
                int maTK    = SecurityHelper.GetCurrentMaTaiKhoan();
                int chuaDoc = HopThuBLL.DemChuaDoc(maTK);
                if (chuaDoc > 0)
                {
                    litBadgeHopThu.Text = $"<span class='position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger' style='font-size:.6rem;min-width:16px'>{chuaDoc}</span>";
                }
            }

            if (hasPanel)
            {
                // Set link dynamic theo role — tái dụng GetRedirectUrlByRole sẵn có
                int maQuyen = SecurityHelper.GetCurrentMaQuyen();
                lnkQuanTri.HRef = SecurityHelper.GetRedirectUrlByRole(maQuyen);
            }

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

    /// <summary>
    /// Cho phép content page override meta description (SEO).
    /// Gọi trước hoặc trong Page_Load của content page.
    /// </summary>
    public void SetMetaDescription(string description)
    {
        if (metaDescription != null && !string.IsNullOrWhiteSpace(description))
            metaDescription.Attributes["content"] = description;
    }

    /// <summary>Set Open Graph title.</summary>
    public void SetOgTitle(string title)
    {
        if (ogTitle != null) ogTitle.Attributes["content"] = title;
    }

    /// <summary>Set Open Graph description.</summary>
    public void SetOgDescription(string description)
    {
        if (ogDescription != null) ogDescription.Attributes["content"] = description;
    }

    /// <summary>Set Open Graph image URL.</summary>
    public void SetOgImage(string imageUrl)
    {
        if (ogImage != null) ogImage.Attributes["content"] = imageUrl;
    }

    /// <summary>Set Open Graph URL (canonical for social).</summary>
    public void SetOgUrl(string url)
    {
        if (ogUrl != null) ogUrl.Attributes["content"] = url;
    }

    /// <summary>Set Open Graph type (website, article, etc.).</summary>
    public void SetOgType(string type)
    {
        if (ogType != null) ogType.Attributes["content"] = type;
    }

    /// <summary>Set Twitter Card title.</summary>
    public void SetTwTitle(string title)
    {
        if (twTitle != null) twTitle.Attributes["content"] = title;
    }

    /// <summary>Set Twitter Card description.</summary>
    public void SetTwDescription(string description)
    {
        if (twDescription != null) twDescription.Attributes["content"] = description;
    }

    /// <summary>Set Twitter Card image URL.</summary>
    public void SetTwImage(string imageUrl)
    {
        if (twImage != null) twImage.Attributes["content"] = imageUrl;
    }

    /// <summary>Set canonical URL.</summary>
    public void SetCanonicalUrl(string url)
    {
        if (canonicalUrl != null) canonicalUrl.Attributes["href"] = url;
    }

    /// <summary>Inject JSON-LD structured data into SeoHeadContent placeholder.</summary>
    public void SetStructuredData(string jsonLd)
    {
        if (SeoHeadContent != null)
        {
            var script = new HtmlGenericControl("script");
            script.Attributes["type"] = "application/ld+json";
            script.InnerHtml = jsonLd;
            SeoHeadContent.Controls.Add(script);
        }
    }
}
