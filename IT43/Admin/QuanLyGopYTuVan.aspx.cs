using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Quản lý Góp ý / Tư vấn (Admin) — hiển thị danh sách tư vấn/góp ý
/// với tab chuyển đổi, lọc trạng thái, xem chi tiết qua modal,
/// và các action: phản hồi, đóng, mở lại.
/// </summary>
public partial class Admin_QuanLyGopYTuVan : Page
{
    private const int PAGE_SIZE = 20;

    /// <summary>Trang hiện tại (lưu ViewState).</summary>
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }

    /// <summary>Loại dữ liệu hiện tại: "TUVAN" hoặc "GOPY" (lưu HiddenField).</summary>
    private string Loai => hfLoai.Value;

    /// <summary>Flag điều khiển hiển thị modal chi tiết từ server-side.</summary>
    public string ShowDetail = "false";

    /// <summary>Load badge số lượng chờ xử lý và bind dữ liệu nếu lần đầu.</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadCounts();
        if (!IsPostBack)
            BindData();
    }

    /// <summary>Đếm số tư vấn/góp ý chờ xử lý để hiển thị badge trên tab.</summary>
    private void LoadCounts()
    {
        int soTuVan = DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_TuVan WHERE TrangThai=0"));
        int soGopY  = DBHelper.Val<int>(DBHelper.Scalar("SELECT COUNT(1) FROM tbl_GopY  WHERE TrangThai=0"));
        litSoTuVan.Text = soTuVan > 0 ? $"<span class='badge'>{soTuVan}</span>" : "";
        litSoGopY.Text  = soGopY  > 0 ? $"<span class='badge'>{soGopY}</span>"  : "";
    }

    /// <summary>Xử lý chuyển tab (gọi từ JS switchTab qua hidden button).</summary>
    protected void btnDoTab_Click(object sender, EventArgs e)
    {
        ddlTrangThai.SelectedIndex = 0;
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Khi thay đổi dropdown trạng thái → reset trang và tải lại.</summary>
    protected void ddlTrangThai_Changed(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    /// <summary>
    /// Xử lý command trên GridView: PhanHoi, RollBack, Dong, MoLai, XemChiTiet.
    /// </summary>
    protected void gvDanhSach_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (!int.TryParse(e.CommandArgument?.ToString(), out int id)) return;

        switch (e.CommandName)
        {
            case "PhanHoi":
                TuVanDanhGiaDAL.CapNhatTrangThai(Loai, id, 1);
                litThongBao.Text = "<div class='alert alert-success py-2'>Đã đánh dấu phản hồi.</div>";
                break;

            case "RollBack":
                TuVanDanhGiaDAL.CapNhatTrangThai(Loai, id, 0);
                litThongBao.Text = "<div class='alert alert-warning py-2'>Đã hoàn tác về Chờ xử lý.</div>";
                break;

            case "Dong":
                TuVanDanhGiaDAL.CapNhatTrangThai(Loai, id, 2);
                litThongBao.Text = "<div class='alert alert-secondary py-2'>Đã đóng yêu cầu.</div>";
                break;

            case "MoLai":
                TuVanDanhGiaDAL.CapNhatTrangThai(Loai, id, 0);
                litThongBao.Text = "<div class='alert alert-info py-2'>Đã mở lại yêu cầu.</div>";
                break;

            case "XemChiTiet":
                LoadChiTiet(id);
                ShowDetail = "true";
                BindData();
                return;
        }

        LoadCounts();
        BindData();
    }

    /// <summary>Load chi tiết tư vấn/góp ý theo ID để hiển thị trong modal.</summary>
    private void LoadChiTiet(int id)
    {
        string sql = Loai == "TUVAN"
            ? @"SELECT tv.*, tr.TenTruong FROM tbl_TuVan tv
                LEFT JOIN tbl_Truong tr ON tr.MaTruong = tv.MaTruong
                WHERE tv.ID = @id"
            : @"SELECT gy.*, tr.TenTruong FROM tbl_GopY gy
                LEFT JOIN tbl_Truong tr ON tr.MaTruong = gy.MaTruong
                WHERE gy.ID = @id";

        var dt = DBHelper.Query(sql, new[] { new System.Data.SqlClient.SqlParameter("@id", id) });
        if (dt.Rows.Count == 0) return;

        var r = dt.Rows[0];

        if (Loai == "TUVAN")
        {
            litCTHoTen.Text = Server.HtmlEncode(r["HoTen"] == DBNull.Value ? "" : r["HoTen"].ToString());
            litCTEmail.Text = Server.HtmlEncode(r["Email"] == DBNull.Value ? "" : r["Email"].ToString());
        }
        else
        {
            litCTHoTen.Text = "<span class='text-muted fst-italic'>Ẩn danh</span>";
            litCTEmail.Text = "<span class='text-muted'>—</span>";
        }

        litCTTruong.Text = Server.HtmlEncode(
            r["TenTruong"] == DBNull.Value ? "(Không liên kết trường)" : r["TenTruong"].ToString());

        litCTNgay.Text = r["NgayGui"] == DBNull.Value ? ""
            : DBHelper.Val<DateTime>(r["NgayGui"]).ToString("dd/MM/yyyy HH:mm");

        byte tt = DBHelper.Val<byte>(r["TrangThai"]);
        litCTTrangThai.Text = tt == 0
            ? "<span class='badge bg-warning text-dark'>Chờ xử lý</span>"
            : tt == 1 ? "<span class='badge bg-success'>Đã phản hồi</span>"
            : "<span class='badge bg-secondary'>Đã đóng</span>";

        litCTNoiDung.Text = Server.HtmlEncode(
            r["NoiDung"] == DBNull.Value ? "" : r["NoiDung"].ToString());

        if (Loai == "TUVAN" && dt.Columns.Contains("GhiChuAdmin") && r["GhiChuAdmin"] != DBNull.Value)
        {
            litCTGhiChu.Text = Server.HtmlEncode(r["GhiChuAdmin"].ToString());
            pnlGhiChu.Visible = true;
        }
        else
        {
            pnlGhiChu.Visible = false;
        }
    }

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            BindData();
        }
    }

    /// <summary>Gọi TuVanDanhGiaDAL.GetDanhSach với bộ lọc loại/trạng thái, bind GridView + phân trang.</summary>
    private void BindData()
    {
        byte? tt = byte.TryParse(ddlTrangThai.SelectedValue, out byte t) ? t : (byte?)null;
        var result = TuVanDanhGiaDAL.GetDanhSach(Loai, tt, null, CurrentPage, PAGE_SIZE);

        litTong.Text = result.TongSo.ToString("N0");
        gvDanhSach.DataSource = result.Data;
        gvDanhSach.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
