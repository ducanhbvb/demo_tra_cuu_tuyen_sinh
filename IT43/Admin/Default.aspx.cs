using System;
using System.Web.UI;

/// <summary>
/// Trang Dashboard Admin — hiển thị thống kê tổng quan (số trường, tin, tư vấn, tài khoản)
/// và danh sách trường/tư vấn mới nhất.
/// </summary>
public partial class Admin_Default : Page
{
    /// <summary>Khởi tạo trang: load dữ liệu thống kê.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) LoadStats();
    }

    /// <summary>
    /// Load thống kê tổng quan từ ThongKeDAL và bind top 5 trường/tư vấn mới nhất.
    /// </summary>
    private void LoadStats()
    {
        litSoTruong.Text = ThongKeDAL.SoTruong().ToString("N0");
        litSoTin.Text    = ThongKeDAL.SoTin().ToString("N0");
        litSoTuVan.Text  = ThongKeDAL.SoTuVanCho().ToString("N0");
        litSoTK.Text     = ThongKeDAL.SoTaiKhoan().ToString("N0");

        gvTruong.DataSource = DBHelper.Query("SELECT TOP 5 MaTruong,TenTruong,TinhThanh FROM tbl_Truong ORDER BY ThoiGianCapNhat DESC");
        gvTruong.DataBind();

        var tv = TuVanDanhGiaDAL.GetDanhSach("TUVAN", 0, null, 0, 5);
        gvTuVan.DataSource = tv.Data;
        gvTuVan.DataBind();
    }
}
