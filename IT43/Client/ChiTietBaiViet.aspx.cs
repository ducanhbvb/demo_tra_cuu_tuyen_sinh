using System;
using System.Web.UI;

/// <summary>
/// Trang Chi tiết bài viết — hiển thị nội dung bài viết theo slug,
/// ảnh bìa, meta (ngày đăng, lượt xem), bài viết liên quan và sidebar mới nhất.
/// </summary>
public partial class ChiTietBaiViet_Page : Page
{
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

        var bv = BaiVietDAL.GetChiTiet(slug);
        if (bv == null)
        {
            pnlBaiViet.Visible = false;
            litNotFound.Text   = "<div class='alert alert-warning'>Không tìm thấy bài viết này.</div>";
            return;
        }

        Page.Title = bv.TieuDe;

        // Ảnh bìa
        if (!string.IsNullOrEmpty(bv.AnhChinh))
        {
            imgAnhBia.ImageUrl = bv.AnhChinh;
            imgAnhBia.AlternateText = bv.TieuDe;
        }
        else
        {
            pnlAnhBia.Visible = false;
        }

        // Meta
        litNgayDang.Text = bv.NgayDang.ToString("dd/MM/yyyy");
        litLuotXem.Text  = bv.LuotXem.ToString("N0");

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
        litNoiDung.Text = bv.NoiDung ?? "<p class='text-muted'>Bài viết chưa có nội dung.</p>";

        // Bài viết liên quan (cùng trường, tối đa 3)
        var lienQuan = BaiVietDAL.GetDanhSach(bv.MaTruong, 0, 3);
        if (lienQuan.TongSo > 0)
        {
            rptLienQuan.DataSource = lienQuan.Data;
            rptLienQuan.DataBind();
        }
        else
        {
            pnlLienQuan.Visible = false;
        }

        // Sidebar: bài viết mới nhất
        var moiNhat = BaiVietDAL.GetDanhSach(null, 0, 5);
        rptMoiNhat.DataSource = moiNhat.Data;
        rptMoiNhat.DataBind();
    }
}
