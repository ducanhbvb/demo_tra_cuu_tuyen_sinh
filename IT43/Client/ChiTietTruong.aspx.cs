using System;
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
        imgBia.ImageUrl  = string.IsNullOrEmpty(_truong.AnhBia)       ? "~/Content/img/no-image.png" : _truong.AnhBia;
        imgLogo.ImageUrl = string.IsNullOrEmpty(_truong.AnhDaiDien)    ? "~/Content/img/no-image.png" : _truong.AnhDaiDien;

        litTenTruong.Text = Server.HtmlEncode(_truong.TenTruong);
        litDiaChi.Text    = Server.HtmlEncode(_truong.DiaChi ?? _truong.TinhThanh ?? "");
        litMoTa.Text      = _truong.MoTa ?? "<p class='text-muted'>Chưa có thông tin mô tả.</p>";
        litLoai.Text      = _truong.TenLoaiTruong;
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

        // Ngành đào tạo
        gvNganh.DataSource = TruongDAL.GetNganhCuaTruong(_truong.MaTruong);
        gvNganh.DataBind();

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

        // Bài viết của trường
        var bvResult = BaiVietDAL.GetDanhSach(_truong.MaTruong, 0, 20, chiActive: true);
        if (bvResult.Data.Rows.Count > 0)
        {
            rptBaiVietTruong.DataSource = bvResult.Data;
            rptBaiVietTruong.DataBind();
        }
        else
        {
            pnlNoBaiViet.Visible = true;
        }

        // Đánh giá
        rptDanhGia.DataSource = TuVanDanhGiaDAL.GetDanhGia(_truong.MaTruong, 1);
        rptDanhGia.DataBind();

        // Wishlist / đánh giá buttons
        if (User.Identity.IsAuthenticated)
        {
            pnlWishlist.Visible  = true;
            pnlFormDanhGia.Visible = true;
            int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
            bool daThem = WishListDAL.DaThem(maTK, _truong.MaTruong, null);
            btnWishList.Text     = daThem ? "❤ Bỏ yêu thích" : "♡ Thêm yêu thích";
            btnWishList.CommandArgument = daThem ? "remove" : "add";
        }
    }

    /// <summary>Lấy MaTruong an toàn từ ViewState (dùng cho mọi PostBack event).</summary>
    private int GetMaTruongFromViewState()
    {
        return ViewState["MaTruong"] is int id ? id : 0;
    }

    /// <summary>Xử lý nút Thêm/Bỏ yêu thích — toggle wishlist cho người dùng hiện tại.</summary>
    protected void btnWishList_Click(object sender, EventArgs e)
    {
        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK == 0) { Response.Redirect("~/Client/Login.aspx"); return; }

        int maTruong = GetMaTruongFromViewState();
        if (maTruong == 0) return;

        if (btnWishList.CommandArgument == "add")
            WishListDAL.Them(maTK, maTruong, null);
        else
        {
            var list = WishListDAL.GetByTaiKhoan(maTK);
            foreach (var w in list)
                if (w.MaTruong == maTruong) { WishListDAL.Xoa(w.ID, maTK); break; }
        }
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
            TuVanDanhGiaDAL.GuiDanhGia(maTruong, maTK, diem, txtDanhGia.Text.Trim());
        BindData();
    }

    /// <summary>
    /// Xử lý nút Gửi tư vấn — validate form, lưu câu hỏi tư vấn vào DB
    /// và hiển thị thông báo thành công.
    /// </summary>
    protected void btnGuiTuVan_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTVHoTen.Text) || string.IsNullOrWhiteSpace(txtTVEmail.Text)
            || string.IsNullOrWhiteSpace(txtTVNoiDung.Text))
        {
            litTVThongBao.Text = "<div class='alert alert-warning mt-2 small'>Vui lòng điền đầy đủ họ tên, email và nội dung.</div>";
            return;
        }

        int maTruong = GetMaTruongFromViewState();
        if (maTruong == 0) return;

        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        TuVanDanhGiaDAL.GuiTuVan(new TuVanModel
        {
            MaTaiKhoan  = maTK > 0 ? maTK : (int?)null,
            MaTruong    = maTruong,
            HoTen       = txtTVHoTen.Text.Trim(),
            Email       = txtTVEmail.Text.Trim(),
            SoDienThoai = txtTVSdt.Text.Trim(),
            NoiDung     = txtTVNoiDung.Text.Trim()
        });
        litTVThongBao.Text = "<div class='alert alert-success mt-2 small'>Gửi thành công! Nhà trường sẽ liên hệ bạn sớm.</div>";
        txtTVHoTen.Text = txtTVEmail.Text = txtTVSdt.Text = txtTVNoiDung.Text = "";
    }
}
