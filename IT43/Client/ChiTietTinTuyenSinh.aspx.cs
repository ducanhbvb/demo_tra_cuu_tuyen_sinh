using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

/// <summary>
/// Trang Chi tiết tin tuyển sinh — hiển thị thông tin ngành, phương thức,
/// điểm chuẩn, chỉ tiêu, học phí, hạn nộp hồ sơ và các tin cùng trường.
/// </summary>
public partial class ChiTietTinTuyenSinh : Page
{
    /// <summary>Mã tin đang xem (expose cho JS).</summary>
    public int MaTinHienTai { get; private set; }
    /// <summary>Mã trường sở hữu tin (expose cho JS chart API).</summary>
    public int MaTruongHienTai { get; private set; }
    /// <summary>Mã chuyên ngành của tin (expose cho JS).</summary>
    public int MaChuyenNganhHienTai { get; private set; }
    /// <summary>Slug của trường để tạo link chi tiết.</summary>
    public string TruongSlug { get; private set; }

    /// <summary>
    /// Load chi tiết tin theo id từ QueryString, tăng lượt xem,
    /// bind toàn bộ thông tin và danh sách tin cùng trường.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;

        int id;
        if (!int.TryParse(Request.QueryString["id"], out id) || id <= 0)
        {
            pnlNotFound.Visible = true;
            pnlChiTiet.Visible = false;
            return;
        }

        var tin = TinTuyenSinhDAL.GetByIdPublic(id);
        if (tin == null)
        {
            pnlNotFound.Visible = true;
            pnlChiTiet.Visible = false;
            return;
        }

        // Tăng lượt xem
        TinTuyenSinhDAL.TangLuotXem(id);
        tin.LuotXem++;

        // Lưu cho JS
        MaTinHienTai = tin.MaTin;
        MaTruongHienTai = tin.MaTruong;
        MaChuyenNganhHienTai = tin.MaChuyenNganh;

        // Lấy slug trường
        var truong = TruongDAL.LayChiTiet(tin.MaTruong);
        TruongSlug = truong != null ? truong.Slug : "";

        // Page title
        Page.Title = tin.TieuDeHienThi + " - Tra Cứu Tuyển Sinh";

        // SEO: Open Graph + Twitter + Canonical + Article structured data
        string moTaClean = HtmlSanitizerHelper.ToPlainText(tin.MoTa, 155);
        string metaDesc = !string.IsNullOrWhiteSpace(moTaClean)
            ? moTaClean
            : $"{tin.TieuDeHienThi} - Thông tin tuyển sinh trường {tin.TenTruong}.";
        var master = Master as MasterPages_Site;
        if (master != null)
        {
            string absoluteUrl = Request.Url.GetLeftPart(UriPartial.Path) + Request.Url.Query;
            master.SetOgTitle(tin.TieuDeHienThi + " - Tra Cứu Tuyển Sinh");
            master.SetOgDescription(metaDesc);
            master.SetOgUrl(absoluteUrl);
            master.SetOgType("article");
            string imageUrl = !string.IsNullOrEmpty(truong?.AnhDaiDien)
                ? Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl(truong.AnhDaiDien)
                : Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/Resources/Images/no-image.png");
            master.SetOgImage(imageUrl);
            master.SetTwTitle(tin.TieuDeHienThi);
            master.SetTwDescription(metaDesc);
            master.SetTwImage(imageUrl);
            master.SetCanonicalUrl(absoluteUrl);

            // Additional article properties
            var seoContent = Master.FindControl("SeoHeadContent") as System.Web.UI.WebControls.ContentPlaceHolder;
            if (seoContent != null)
            {
                var pubMeta = new HtmlGenericControl("meta");
                pubMeta.Attributes["property"] = "article:published_time";
                pubMeta.Attributes["content"] = tin.NgayDang.ToString("yyyy-MM-ddTHH:mm:ssK");
                seoContent.Controls.Add(pubMeta);
            }

            // JSON-LD: Article (admission notice)
            string jsonLd = $@"
{{
  ""@context"": ""https://schema.org"",
  ""@type"": ""Article"",
  ""headline"": ""{HttpUtility.JavaScriptStringEncode(tin.TieuDeHienThi)}"",
  ""image"": [""{imageUrl}""],
  ""datePublished"": ""{tin.NgayDang.ToString("yyyy-MM-ddTHH:mm:ssK")}"",
  ""author"": {{
    ""@type"": ""Organization"",
    ""name"": ""Tra Cứu Tuyển Sinh""
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

        // Breadcrumb
        litBreadTruong.Text = Server.HtmlEncode(tin.TenTruong);
        litBreadTieuDe.Text = Server.HtmlEncode(tin.TieuDeHienThi);

        // Header
        litTieuDe.Text = Server.HtmlEncode(tin.TieuDeHienThi);
        litTenTruong.Text = Server.HtmlEncode(tin.TenTruong);
        if (truong != null && !string.IsNullOrEmpty(truong.AnhDaiDien))
            imgLogo.ImageUrl = truong.AnhDaiDien;
        else
            imgLogo.ImageUrl = "/Resources/Images/no-image.png";

        // Hạn nộp
        if (tin.HanNop.HasValue)
        {
            pnlHanNop.Visible = true;
            var conLai = (tin.HanNop.Value - DateTime.Now).Days;
            if (conLai > 0)
                litHanNop.Text = "Còn " + conLai + " ngày (hạn " + tin.HanNop.Value.ToString("dd/MM/yyyy") + ")";
            else if (conLai == 0)
                litHanNop.Text = "Hạn nộp hôm nay!";
            else
                litHanNop.Text = "Đã hết hạn (" + tin.HanNop.Value.ToString("dd/MM/yyyy") + ")";
        }

        // Thông tin chung
        litNganh.Text = Server.HtmlEncode(tin.TenChuyenNganh);
        litPhuongThuc.Text = Server.HtmlEncode(tin.TenPhuongThuc);
        litToHop.Text = Server.HtmlEncode(tin.ToHopMonHoc ?? "—");
        litNam.Text = tin.NamTuyenSinh.ToString();
        litChiTieu.Text = tin.ChiTieu.HasValue ? tin.ChiTieu.Value.ToString("N0") : "—";
        litDiemChuan.Text = tin.DiemChuanNamTruoc.HasValue ? tin.DiemChuanNamTruoc.Value.ToString("F2") : "—";

        if (tin.DiemChuanNamNay.HasValue)
        {
            pnlDiemNamNay.Visible = true;
            litDiemNamNay.Text = tin.DiemChuanNamNay.Value.ToString("F2");
        }

        // Sprint 1: HocPhi là string tự do, hiển thị trực tiếp (encode XSS)
        litHocPhi.Text = string.IsNullOrWhiteSpace(tin.HocPhi) ? "—" : Server.HtmlEncode(tin.HocPhi);
        litLoaiHinh.Text = Server.HtmlEncode(tin.LoaiHinhDaoTao ?? "—");
        litCoSo.Text = Server.HtmlEncode(tin.CoSoDaoTao ?? "—");

        // Mô tả: render rich-text đã sanitize bằng whitelist để giữ định dạng nhưng tránh XSS lưu trữ.
        if (!string.IsNullOrWhiteSpace(tin.MoTa))
        {
            pnlMoTa.Visible = true;
            litMoTa.Text = HtmlSanitizerHelper.SanitizeRichText(tin.MoTa);
        }

        // Meta
        litNgayDang.Text = tin.NgayDang.ToString("dd/MM/yyyy");
        litLuotXem.Text = tin.LuotXem.ToString("N0");

        // Tin cùng trường
        var dsTin = TinTuyenSinhDAL.GetListTheoTruong(tin.MaTruong);
        rptTinCungTruong.DataSource = dsTin;
        rptTinCungTruong.DataBind();

        // DataBind cho breadcrumb expressions
        pnlChiTiet.DataBind();
    }

    // ── Safe conversion helpers ─────────────────────────────────────────────
    protected int SafeGetInt(object value)
    {
        return value != DBNull.Value && value != null ? Convert.ToInt32(value) : 0;
    }

    protected string FormatDecimal(object value, string format = "F2")
    {
        return value != DBNull.Value && value != null
            ? ((decimal)value).ToString(format)
            : "";
    }

    protected string FormatInt(object value, string format = "N0")
    {
        return value != DBNull.Value && value != null
            ? ((int)value).ToString(format)
            : "";
    }
}
