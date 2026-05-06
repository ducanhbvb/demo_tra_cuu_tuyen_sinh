using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

/// <summary>
/// Trang Chi tiết bài viết — hiển thị nội dung bài viết theo slug,
/// ảnh bìa, meta (ngày đăng, lượt xem), bài viết liên quan và sidebar mới nhất.
/// </summary>
public partial class ChiTietBaiViet_Page : Page
{
    protected string BannerUrl = "/Resources/Images/campus-background-opt.jpg";
    protected string TheLoaiBadge = "";

    /// <summary>
    /// Load bài viết theo slug từ QueryString, hiển thị nội dung,
    /// bài viết liên quan (cùng trường) và sidebar bài mới nhất.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string slug = Request.QueryString["slug"];
        if (string.IsNullOrWhiteSpace(slug))
        {
            Response.Redirect("~/Client/BaiViet.aspx");
            return;
        }

        var bv = BaiVietBLL.GetChiTiet(slug);
        if (bv == null)
        {
            pnlBaiViet.Visible = false;
            litNotFound.Text   = "<div class='alert alert-warning'>Không tìm thấy bài viết này.</div>";
            return;
        }

        Page.Title = bv.TieuDe;

        // SEO: override meta description theo bài viết
        string moTaClean = HtmlSanitizerHelper.ToPlainText(bv.NoiDung, 155);
        string metaDesc = !string.IsNullOrWhiteSpace(moTaClean)
            ? moTaClean
            : $"{bv.TieuDe} - Tin tức tuyển sinh đại học, cao đẳng Việt Nam.";
        (Master as MasterPages_Site)?.SetMetaDescription(metaDesc);

        // SEO: Open Graph + Twitter + Canonical + Article meta
        var master = Master as MasterPages_Site;
        if (master != null)
        {
            string absoluteUrl = Request.Url.GetLeftPart(UriPartial.Path) + Request.Url.Query;
            master.SetOgTitle(bv.TieuDe + " - Tra Cứu Tuyển Sinh");
            master.SetOgDescription(metaDesc);
            master.SetOgUrl(absoluteUrl);
            master.SetOgType("article");
            string imageUrl = !string.IsNullOrEmpty(bv.AnhChinh)
                ? Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl(bv.AnhChinh)
                : Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/Resources/Images/no-image.png");
            master.SetOgImage(imageUrl);
            master.SetTwTitle(bv.TieuDe);
            master.SetTwDescription(metaDesc);
            master.SetTwImage(imageUrl);
            master.SetCanonicalUrl(absoluteUrl);

            // Additional Open Graph article properties via placeholder
            var seoContent = Master.FindControl("SeoHeadContent") as System.Web.UI.WebControls.ContentPlaceHolder;
            if (seoContent != null)
            {
                // article:published_time
                var pubMeta = new HtmlGenericControl("meta");
                pubMeta.Attributes["property"] = "article:published_time";
                pubMeta.Attributes["content"] = bv.NgayDang.ToString("yyyy-MM-ddTHH:mm:ssK");
                seoContent.Controls.Add(pubMeta);

                // article:section (the loai)
                if (!string.IsNullOrEmpty(bv.TheLoai))
                {
                    var sectionMeta = new HtmlGenericControl("meta");
                    sectionMeta.Attributes["property"] = "article:section";
                    sectionMeta.Attributes["content"] = bv.TheLoai;
                    seoContent.Controls.Add(sectionMeta);
                }

                // article:author
                var authorMeta = new HtmlGenericControl("meta");
                authorMeta.Attributes["property"] = "article:author";
                authorMeta.Attributes["content"] = GetNguoiDangHienThi(bv);
                seoContent.Controls.Add(authorMeta);
            }

            // JSON-LD: Article
            string jsonLd = $@"
{{
  ""@context"": ""https://schema.org"",
  ""@type"": ""Article"",
  ""headline"": ""{HttpUtility.JavaScriptStringEncode(bv.TieuDe)}"",
  ""image"": [""{imageUrl}""],
  ""datePublished"": ""{bv.NgayDang.ToString("yyyy-MM-ddTHH:mm:ssK")}"",
  ""author"": {{
    ""@type"": ""Person"",
    ""name"": ""{HttpUtility.JavaScriptStringEncode(GetNguoiDangHienThi(bv))}""
  }},
  ""publisher"": {{
    ""@type"": ""Organization"",
    ""name"": ""Tra Cứu Tuyển Sinh"",
    ""logo"": {{
      ""@type"": ""ImageObject"",
      ""url"": ""{Request.Url.GetLeftPart(UriPartial.Authority)}/Resources/Images/logo-default.png""
    }}
  }},
  ""description"": ""{HttpUtility.JavaScriptStringEncode(metaDesc)}""
}}";
            master.SetStructuredData(jsonLd);
        }

        // Ảnh bìa
        if (!string.IsNullOrEmpty(bv.AnhChinh))
        {
            BannerUrl = bv.AnhChinh;
        }

        // Cập nhật Tag Thể loại
        TheLoaiBadge = bv.TheLoai ?? "";

        // Meta
        litNguoiDang.Text = Server.HtmlEncode(GetNguoiDangHienThi(bv));
        litNgayDang.Text  = bv.NgayDang.ToString("dd/MM/yyyy");
        litLuotXem.Text   = bv.LuotXem.ToString("N0");

        if (!string.IsNullOrEmpty(bv.TenTruong))
        {
            litTenTruong.Text    = Server.HtmlEncode(bv.TenTruong);
            pnlTruongBadge.Visible = true;
        }
        else
        {
            pnlTruongBadge.Visible = false;
        }

        litTieuDe.Text  = Server.HtmlEncode(bv.TieuDe);
        litNoiDung.Text = string.IsNullOrWhiteSpace(bv.NoiDung)
            ? "<p class='text-muted'>Bài viết chưa có nội dung.</p>"
            : HtmlSanitizerHelper.SanitizeRichText(bv.NoiDung);

        // Bài viết liên quan (cùng trường, tối đa 3)
        var lienQuan = BaiVietBLL.GetDanhSach(bv.MaTruong, 0, 3);
        if (lienQuan.TongSo > 0)
        {
            // Currently hidden in redesign, can re-enable if required.
            // rptLienQuan.DataSource = lienQuan.Data;
            // rptLienQuan.DataBind();
        }
        else
        {
            pnlLienQuan.Visible = false;
        }

        // Sidebar: bài viết mới nhất
        var moiNhat = BaiVietBLL.GetDanhSach(null, 0, 5);
        rptMoiNhat.DataSource = moiNhat.Data;
        rptMoiNhat.DataBind();
    }

    private string GetNguoiDangHienThi(BaiVietModel bv)
    {
        if (!string.IsNullOrWhiteSpace(bv?.TenNguoiDangHienThi))
            return bv.TenNguoiDangHienThi.Trim();
        if (!string.IsNullOrWhiteSpace(bv?.TenNguoiDang))
            return bv.TenNguoiDang.Trim();
        return "Ban Biên tập";
    }

    // ── Safe conversion helpers ─────────────────────────────────────────────
    protected string FormatDate(object value)
    {
        return value != DBNull.Value && value != null
            ? ((DateTime)value).ToString("dd/MM/yyyy")
            : "";
    }

    protected string FormatInt(object value, string format = "N0")
    {
        return value != DBNull.Value && value != null
            ? Convert.ToInt64(value).ToString(format)
            : "";
    }
}
