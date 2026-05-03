using System;
using System.Collections.Generic;
using System.Web.UI;

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

        var tin = TinTuyenSinhDAL.GetById(id);
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

        litHocPhi.Text = tin.HocPhi.HasValue ? tin.HocPhi.Value.ToString("N0") + " VNĐ/năm" : "—";
        litLoaiHinh.Text = Server.HtmlEncode(tin.LoaiHinhDaoTao ?? "—");
        litCoSo.Text = Server.HtmlEncode(tin.CoSoDaoTao ?? "—");

        // Mô tả
        if (!string.IsNullOrWhiteSpace(tin.MoTa))
        {
            pnlMoTa.Visible = true;
            litMoTa.Text = tin.MoTa;
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
}
