using System;
using System.Web;
using System.Web.UI;

/// <summary>
/// Trang Chi tiết trường — hiển thị thông tin tổng quan, ngành đào tạo,
/// điểm chuẩn, tin tuyển sinh, bài viết, đánh giá và form tư vấn.
/// </summary>
public partial class ChiTietTruong : Page
{
    private TruongModel _truong;

    /// <summary>Expose MaTruong cho JavaScript để gọi Chart API biểu đồ điểm chuẩn.</summary>
    public string MaTruongHienTai => _truong?.MaTruong.ToString() ?? "0";

    /// <summary>
    /// Load thông tin trường theo slug hoặc id từ QueryString.
    /// Nếu không tìm thấy → hiển thị thông báo lỗi.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        string slug = Request.QueryString["slug"];
        int.TryParse(Request.QueryString["id"], out int id);

        _truong = TruongBLL.LayChiTiet(id > 0 ? id : (int?)null, slug);

        if (_truong == null)
        {
            pnlTruong.Visible = false;
            litNotFound.Text  = "<div class='alert alert-warning'>Không tìm thấy thông tin trường này.</div>";
            return;
        }

        Page.Title = _truong.TenTruong;

        // SEO: override meta description theo trường
        string moTaClean = HtmlSanitizerHelper.ToPlainText(_truong.MoTa, 155);
        string metaDesc = !string.IsNullOrWhiteSpace(moTaClean)
            ? moTaClean
            : $"Thông tin tuyển sinh, điểm chuẩn, ngành đào tạo của {_truong.TenTruong}. Tra cứu tuyển sinh đại học, cao đẳng Việt Nam.";
        (Master as MasterPages_Site)?.SetMetaDescription(metaDesc);

        // SEO: Open Graph + Twitter + Canonical
        var master = Master as MasterPages_Site;
        if (master != null)
        {
            string absoluteUrl = Request.Url.GetLeftPart(UriPartial.Path) + Request.Url.Query;
            master.SetOgTitle(_truong.TenTruong + " - Tra Cứu Tuyển Sinh");
            master.SetOgDescription(metaDesc);
            master.SetOgUrl(absoluteUrl);
            master.SetOgType("website");
            string imageUrl = !string.IsNullOrEmpty(_truong.AnhDaiDien)
                ? Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl(_truong.AnhDaiDien)
                : Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/Resources/Images/no-image.png");
            master.SetOgImage(imageUrl);

            master.SetTwTitle(_truong.TenTruong);
            master.SetTwDescription(metaDesc);
            master.SetTwImage(imageUrl);

            master.SetCanonicalUrl(absoluteUrl);

            // JSON-LD Structured Data: EducationalOrganization
            string jsonLd = $@"
{{
  ""@context"": ""https://schema.org"",
  ""@type"": ""EducationalOrganization"",
  ""name"": ""{HttpUtility.JavaScriptStringEncode(_truong.TenTruong)}"",
  ""description"": ""{HttpUtility.JavaScriptStringEncode(moTaClean)}"",
  ""address"": {{
    ""@type"": ""PostalAddress"",
    ""addressLocality"": ""{HttpUtility.JavaScriptStringEncode(_truong.TinhThanh)}"",
    ""streetAddress"": ""{HttpUtility.JavaScriptStringEncode(_truong.DiaChi)}""
  }},
  ""telephone"": ""{HttpUtility.JavaScriptStringEncode(_truong.SoDienThoai)}"",
  ""url"": ""{absoluteUrl}"",
  ""logo"": ""{Request.Url.GetLeftPart(UriPartial.Authority)}{ResolveUrl(!string.IsNullOrEmpty(_truong.AnhDaiDien) ? _truong.AnhDaiDien : "~/Resources/Images/no-image.png")}"",
  ""sameAs"": []
}}";
            master.SetStructuredData(jsonLd);
        }

        if (!IsPostBack) BindData();
    }

    /// <summary>
    /// Bind toàn bộ dữ liệu chi tiết trường: ảnh, thông tin cơ bản, ngành đào tạo,
    /// điểm chuẩn, tin tuyển sinh, bài viết, đánh giá và trạng thái wishlist.
    /// </summary>
    private void BindData()
    {
        // Lưu MaTruong vào ViewState để các event handler PostBack truy cập an toàn
        ViewState["MaTruong"] = _truong.MaTruong;

        // Ảnh bìa / logo
        imgBia.ImageUrl  = string.IsNullOrEmpty(_truong.AnhBia)    ? "~/Content/img/no-image.png" : _truong.AnhBia;
        imgLogo.ImageUrl = string.IsNullOrEmpty(_truong.AnhDaiDien) ? "~/Content/img/no-image.png" : _truong.AnhDaiDien;

        litTenTruong.Text = Server.HtmlEncode(_truong.TenTruong);
        litDiaChi.Text    = Server.HtmlEncode(_truong.DiaChi ?? _truong.TinhThanh ?? "");
        litMoTa.Text      = string.IsNullOrWhiteSpace(_truong.MoTa)
            ? "<p class='text-muted'>Chưa có thông tin mô tả.</p>"
            : HtmlSanitizerHelper.SanitizeRichText(_truong.MoTa);
        litLoai.Text      = _truong.TenLoaiTruong;
        litCapBacDaoTao.Text = string.IsNullOrEmpty(_truong.TenCapBacDaoTao)
            ? "—"
            : $"<span class='school-level-badge {GetCapBacCss(_truong.CapBacDaoTao)}'>{Server.HtmlEncode(_truong.TenCapBacDaoTao)}</span>";
        litVung.Text      = _truong.TenVung;
        litSdt.Text       = Server.HtmlEncode(_truong.SoDienThoai ?? "—");
        litKiemDinh.Text  = _truong.KiemDinhChatLuong
            ? "<span class='badge bg-success'>Đã kiểm định</span>"
            : "<span class='badge bg-secondary'>Chưa kiểm định</span>";

        if (!string.IsNullOrEmpty(_truong.Website))
        {
            hlkWeb.Text        = _truong.Website;
            hlkWeb.NavigateUrl = _truong.Website.StartsWith("http") ? _truong.Website : "http://" + _truong.Website;
        }

        string rating = _truong.DiemDanhGiaTB.HasValue
            ? $"<span class='text-warning fw-bold'>{_truong.DiemDanhGiaTB:F1}/5</span> ({_truong.SoLuongDanhGia} đánh giá)"
            : "Chưa có";
        litDanhGiaSidebar.Text = rating;
        litDiemTB.Text = _truong.DiemDanhGiaTB.HasValue
            ? $"<p class='mb-3'>Điểm đánh giá trung bình: <strong class='text-warning'>{_truong.DiemDanhGiaTB:F1}/5</strong> ({_truong.SoLuongDanhGia} đánh giá)</p>"
            : "<p class='text-muted mb-3'>Chưa có đánh giá nào.</p>";

        // Điểm chuẩn
        gvDiemChuan.DataSource = TinTuyenSinhDAL.GetTheoTruong(_truong.MaTruong);
        gvDiemChuan.DataBind();

        // Tin tuyển sinh (card list dạng Repeater)
        var dsTin = TinTuyenSinhDAL.GetListTheoTruong(_truong.MaTruong);
        if (dsTin.Count > 0)
        {
            rptTinTuyenSinh.DataSource = dsTin;
            rptTinTuyenSinh.DataBind();
        }
        else
        {
            pnlNoTinTuyenSinh.Visible = true;
        }

        // Bài viết của trường — dùng BaiVietBLL thay vì gọi DAL trực tiếp
        var bvResult = BaiVietBLL.GetDanhSach(_truong.MaTruong, 0, 20, chiActive: true);
        if (bvResult?.Data != null && bvResult.Data.Rows.Count > 0)
        {
            rptBaiVietTruong.DataSource = bvResult.Data;
            rptBaiVietTruong.DataBind();
        }
        else
        {
            pnlNoBaiViet.Visible = true;
        }

        // Đánh giá — dùng TuVanDanhGiaBLL
        rptDanhGia.DataSource = TuVanDanhGiaBLL.GetDanhGia(_truong.MaTruong, 1);
        rptDanhGia.DataBind();

        // Wishlist / đánh giá buttons
        if (User.Identity.IsAuthenticated)
        {
            pnlWishlist.Visible    = true;
            pnlFormDanhGia.Visible = true;
            int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
            bool daThem = WishListBLL.DaThem(maTK, _truong.MaTruong);
            btnWishList.Text            = daThem ? "❤ Bỏ yêu thích" : "♡ Thêm yêu thích";
            btnWishList.CommandArgument = daThem ? "remove" : "add";
        }
    }

    private string GetCapBacCss(byte? value)
    {
        return value == 1 ? "school-level-university" : value == 2 ? "school-level-college" : value == 3 ? "school-level-vocational" : "school-level-university";
    }

    /// <summary>Lấy MaTruong an toàn từ ViewState (dùng cho mọi PostBack event).</summary>
    private int GetMaTruongFromViewState()
        => ViewState["MaTruong"] is int id ? id : 0;

    /// <summary>Xử lý nút Thêm/Bỏ yêu thích — toggle wishlist cho người dùng hiện tại.</summary>
    protected void btnWishList_Click(object sender, EventArgs e)
    {
        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK == 0) { Response.Redirect("~/Client/Login.aspx"); return; }

        int maTruong = GetMaTruongFromViewState();
        if (maTruong == 0) return;

        // WishListBLL.Toggle xử lý toàn bộ logic add/remove
        WishListBLL.Toggle(maTK, maTruong);
        BindData();
    }

    /// <summary>Xử lý nút Gửi đánh giá — lưu điểm và nội dung đánh giá vào DB.</summary>
    protected void btnGuiDanhGia_Click(object sender, EventArgs e)
    {
        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK == 0) return;

        int maTruong = GetMaTruongFromViewState();
        if (maTruong == 0) return;

        if (byte.TryParse(ddlDiem.SelectedValue, out byte diem))
        {
            var (ok, err) = TuVanDanhGiaBLL.GuiDanhGia(maTruong, maTK, diem, txtDanhGia.Text.Trim());
            if (!ok)
            {
                // hiển thị lỗi nếu cần — tạm dùng litDiemTB để thông báo
                litDiemTB.Text = $"<div class='alert alert-warning small'>{err}</div>";
                return;
            }
        }
        BindData();
    }

    /// <summary>
    /// Xử lý nút Gửi tư vấn — validate form qua BLL, lưu vào DB
    /// và hiển thị thông báo thành công.
    /// </summary>
    protected void btnGuiTuVan_Click(object sender, EventArgs e)
    {
        int maTruong = GetMaTruongFromViewState();
        if (maTruong == 0) return;

        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();

        var model = new TuVanModel
        {
            MaTaiKhoan  = maTK > 0 ? maTK : (int?)null,
            MaTruong    = maTruong,
            HoTen       = txtTVHoTen.Text.Trim(),
            Email       = txtTVEmail.Text.Trim(),
            SoDienThoai = txtTVSdt.Text.Trim(),
            NoiDung     = txtTVNoiDung.Text.Trim()
        };

        var (ok, msg) = TuVanDanhGiaBLL.GuiTuVan(model);
        if (!ok)
        {
            litTVThongBao.Text = $"<div class='alert alert-warning mt-2 small'>{msg}</div>";
            return;
        }

        litTVThongBao.Text = "<div class='alert alert-success mt-2 small'>Gửi thành công! Nhà trường sẽ liên hệ bạn sớm.</div>";
        txtTVHoTen.Text = txtTVEmail.Text = txtTVSdt.Text = txtTVNoiDung.Text = "";
    }

    // ── Data binding helpers (null-safe) ─────────────────────────────────────────
    protected string FormatDate(object value)
    {
        return value != DBNull.Value && value != null
            ? ((DateTime)value).ToString("dd/MM/yyyy")
            : "";
    }

    protected string FormatBool(object value, string trueText = "✓", string falseText = "")
    {
        if (value == DBNull.Value || value == null) return "";
        return (bool)value ? trueText : falseText;
    }

    protected string FormatStars(object rating)
    {
        if (rating == DBNull.Value || rating == null) return "";
        byte r = (byte)rating;
        return new string('★', r) + new string('☆', 5 - r);
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

    protected string GetHanNopBadge(object hanNopValue)
    {
        if (hanNopValue == DBNull.Value || hanNopValue == null) return "";
        DateTime hanNop = (DateTime)hanNopValue;
        if (hanNop > DateTime.Now)
        {
            int days = (hanNop - DateTime.Now).Days;
            return $"<span class='badge bg-success mb-2 p-2 px-3 fs-6 rounded-pill shadow-sm'><i class='bi bi-hourglass-split me-1'></i>Còn {days} ngày</span>";
        }
        else
        {
            return "<span class='badge bg-secondary mb-2 p-2 px-3 fs-6 rounded-pill'>Đã hết hạn</span>";
        }
    }
}
