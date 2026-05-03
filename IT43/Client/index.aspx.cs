using System;
using System.Web.UI;

/// <summary>
/// Trang chủ — hiển thị thống kê tổng quan, trường nổi bật,
/// bài viết mới nhất và tin tuyển sinh mới nhất.
/// </summary>
public partial class index_page : Page
{
    /// <summary>Khởi tạo trang chủ: load dữ liệu thống kê và danh sách nổi bật.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadData();
    }

    /// <summary>
    /// Load toàn bộ dữ liệu trang chủ: thống kê (số trường, ngành, tin),
    /// trường nổi bật (có kiểm định), bài viết mới và tin tuyển sinh mới.
    /// </summary>
    private void LoadData()
    {
        // Thống kê — dùng ThongKeDAL, tuân thủ kiến trúc 3-layer
        litSoTruong.Text   = ThongKeDAL.SoTruong().ToString("N0");
        litSoNganh.Text    = ThongKeDAL.SoNganh().ToString("N0");
        litSoTin.Text      = ThongKeDAL.SoTinActive().ToString("N0");
        litNamMoiNhat.Text = ThongKeDAL.NamTuyenSinhMoiNhat().ToString();

        // Trường nổi bật (8 trường có kiểm định, đang hiển thị)
        var dt = DBHelper.Query(@"SELECT TOP 8 MaTruong, TenTruong, TinhThanh, AnhDaiDien, Slug, LoaiTruong
            FROM tbl_Truong WHERE KiemDinhChatLuong=1 AND TrangThai=1 ORDER BY ThoiGianCapNhat DESC");
        rptTruong.DataSource = dt;
        rptTruong.DataBind();

        // Bài viết mới nhất (8 bài active)
        var baiViet = BaiVietDAL.GetDanhSach(null, 0, 8, chiActive: true);
        rptBaiViet.DataSource = baiViet.Data;
        rptBaiViet.DataBind();

        // Tin tuyển sinh mới nhất
        var paged = TinTuyenSinhDAL.TimKiem(null, null, null, null, null, null, null, 0, 10);
        gvTinMoiNhat.DataSource = paged.Data;
        gvTinMoiNhat.DataBind();
    }

    /// <summary>Xử lý ô tìm kiếm nhanh trên trang chủ — redirect sang trang tìm kiếm trường.</summary>
    protected void btnTimKiem_Click(object sender, EventArgs e)
    {
        string kw = Server.UrlEncode(txtTimKiem.Text.Trim());
        Response.Redirect($"TimKiemTruong.aspx?q={kw}");
    }
}
