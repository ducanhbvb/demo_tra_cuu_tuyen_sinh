using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Góp ý / Tư vấn cho tài khoản TruongHoc.
/// Xem danh sách + chi tiết — Phản hồi tư vấn (TUVAN) của trường mình.
/// Dữ liệu được lọc theo MaTruong của tài khoản đang đăng nhập.
/// </summary>
public partial class TruongHoc_GopYTuVan : Page
{
    private const int PAGE_SIZE = 20;
    private int CurrentPage { get => ViewState["Page"] is int p ? p : 0; set => ViewState["Page"] = value; }
    private string Loai     => hfLoai.Value;
    private int MaTruong    => SecurityHelper.GetCurrentMaTruong();
    private int MaTK        => SecurityHelper.GetCurrentMaTaiKhoan();

    /// <summary>Flag điều khiển hiển thị modal chi tiết.</summary>
    public string ShowDetail = "false";

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadCounts();
        if (!IsPostBack)
            BindData();
        else
        {
            // EnableViewState="false" → rebind để LinkButton trong GridView tồn tại trước RowCommand
            BindData();
        }
    }

    /// <summary>Đếm số Tư vấn / Góp ý chờ xử lý của trường → hiển thị badge.</summary>
    private void LoadCounts()
    {
        int soTuVan = ThongKeBLL.SoTuVanChoCuaTruong(MaTruong);
        int soGopY  = ThongKeBLL.SoGopYChoCuaTruong(MaTruong);
        litSoTuVan.Text = soTuVan > 0 ? $"<span class='badge'>{soTuVan}</span>" : "";
        litSoGopY.Text  = soGopY  > 0 ? $"<span class='badge'>{soGopY}</span>"  : "";
    }

    /// <summary>Chuyển tab Tư vấn / Góp ý.</summary>
    protected void btnDoTab_Click(object sender, EventArgs e)
    {
        ddlTrangThai.SelectedIndex = 0;
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Thay đổi filter trạng thái.</summary>
    protected void ddlTrangThai_Changed(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Xử lý command GridView — XemChiTiet.</summary>
    protected void gvDanhSach_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "XemChiTiet") return;
        if (!int.TryParse(e.CommandArgument?.ToString(), out int id)) return;

        LoadChiTiet(id);
        ShowDetail = "true";
        BindData();
    }

    /// <summary>Chuyển trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            BindData();
        }
    }

    /// <summary>
    /// Gửi phản hồi tư vấn — chỉ áp dụng cho tab TUVAN.
    /// Kiểm tra IDOR: bản ghi phải thuộc trường mình.
    /// </summary>
    protected void btnGuiPhanHoi_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(hfMaTuVanHienTai.Value, out int maTuVan) || maTuVan <= 0)
            return;

        string noiDung = txtReply.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(noiDung))
        {
            litReplyError.Text = "<div class='alert alert-warning py-1 mt-2 small'>Vui lòng nhập nội dung phản hồi.</div>";
            LoadChiTiet(maTuVan);
            ShowDetail = "true";
            BindData();
            return;
        }

        // Lấy thông tin tư vấn + kiểm tra thuộc trường mình (IDOR)
        var r = TuVanDanhGiaBLL.GetChiTietCuaTruong("TUVAN", maTuVan, MaTruong);
        if (r == null)
        {
            litReplyError.Text = "<div class='alert alert-danger py-1 mt-2 small'>Không có quyền phản hồi yêu cầu này.</div>";
            ShowDetail = "true";
            BindData();
            return;
        }

        string emailNguoiHoi      = r["Email"]    == DBNull.Value ? "" : r["Email"].ToString();
        string tenNguoiHoi        = r["HoTen"]    == DBNull.Value ? "Bạn" : r["HoTen"].ToString();
        string cauHoiGoc          = r["NoiDung"]  == DBNull.Value ? "" : r["NoiDung"].ToString();
        string tenTruong          = r["TenTruong"] == DBNull.Value ? "" : r["TenTruong"].ToString();
        int? maTaiKhoanHocSinh    = r.Table.Columns.Contains("MaTaiKhoan") && r["MaTaiKhoan"] != DBNull.Value
            ? (int?)DBHelper.Val<int>(r["MaTaiKhoan"]) : null;

        // Lấy tên trường để hiển thị trong email
        if (string.IsNullOrEmpty(tenTruong) && MaTruong > 0)
        {
            var truong = TruongBLL.LayChiTiet(MaTruong);
            if (truong != null) tenTruong = truong.TenTruong;
        }

        var (ok, error) = TuVanDanhGiaBLL.GuiPhanHoi(
            maTuVan, MaTK, "TruongHoc", tenTruong,
            noiDung, chkGuiEmail.Checked,
            emailNguoiHoi, tenNguoiHoi, cauHoiGoc, tenTruong,
            maTaiKhoanHocSinh);

        if (!ok)
        {
            litReplyError.Text = $"<div class='alert alert-warning py-1 mt-2 small'>{Server.HtmlEncode(error)}</div>";
        }
        else
        {
            litThongBao.Text = "<div class='alert alert-success py-2'><i class='bi bi-check-circle me-1'></i>Đã gửi phản hồi thành công.</div>";

            LogHelper.Ghi(MaTK, "TRUONGHOC_PHAN_HOI_TUVAN",
                $"Trường (MaTruong={MaTruong}) phản hồi tư vấn (MaTuVan={maTuVan})",
                bangTacDong: "tbl_TuVanPhanHoi");
        }

        LoadCounts();
        LoadChiTiet(maTuVan);
        ShowDetail = "true";
        BindData();
    }

    /// <summary>Đóng yêu cầu tư vấn — kiểm tra IDOR trước khi thực hiện.</summary>
    protected void btnDongYeuCau_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(hfMaTuVanHienTai.Value, out int maTuVan) || maTuVan <= 0)
            return;

        // IDOR: chỉ đóng yêu cầu thuộc trường mình
        var r = TuVanDanhGiaBLL.GetChiTietCuaTruong(Loai, maTuVan, MaTruong);
        if (r == null) return;

        TuVanDanhGiaBLL.CapNhatTrangThai(Loai, maTuVan, 2);
        litThongBao.Text = "<div class='alert alert-secondary py-2'><i class='bi bi-x-circle me-1'></i>Đã đóng yêu cầu.</div>";

        LogHelper.Ghi(MaTK, "TRUONGHOC_DONG_TUVAN",
            $"Trường (MaTruong={MaTruong}) đóng yêu cầu (Loai={Loai}, ID={maTuVan})",
            bangTacDong: Loai == "TUVAN" ? "tbl_TuVan" : "tbl_GopY");

        LoadCounts();
        LoadChiTiet(maTuVan);
        ShowDetail = "true";
        BindData();
    }

    /// <summary>Load chi tiết tư vấn/góp ý vào modal + điều khiển reply box.</summary>
    private void LoadChiTiet(int id)
    {
        var r = TuVanDanhGiaBLL.GetChiTietCuaTruong(Loai, id, MaTruong);
        if (r == null) return;

        hfMaTuVanHienTai.Value = id.ToString();

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

        litCTNgay.Text = r["NgayGui"] == DBNull.Value ? ""
            : DBHelper.Val<DateTime>(r["NgayGui"]).ToString("dd/MM/yyyy HH:mm");

        byte tt = DBHelper.Val<byte>(r["TrangThai"]);
        litCTTrangThai.Text = tt == 0
            ? "<span class='badge badge-soft-warning'><i class='bi bi-hourglass-split me-1'></i>Chờ xử lý</span>"
            : tt == 1 ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Đã phản hồi</span>"
            : "<span class='badge badge-soft-secondary'><i class='bi bi-x-circle me-1'></i>Đã đóng</span>";

        litCTNoiDung.Text = Server.HtmlEncode(
            r["NoiDung"] == DBNull.Value ? "" : r["NoiDung"].ToString());

        pnlGhiChu.Visible = false;

        if (Loai == "TUVAN")
        {
            // Hiện thread + reply box (chỉ khi chưa đóng)
            pnlThread.Visible = true;

            var thread = TuVanPhanHoiDAL.GetByMaTuVan(id);
            rptThread.DataSource = thread;
            rptThread.DataBind();

            bool chuaDong = (tt != 2);
            pnlReply.Visible       = chuaDong;
            btnGuiPhanHoi.Visible  = chuaDong;
            btnDongYeuCau.Visible  = chuaDong;
            txtReply.Text          = "";
            litReplyError.Text     = "";
            chkGuiEmail.Checked    = true;
        }
        else
        {
            // Góp ý: chỉ xem, không reply
            pnlThread.Visible      = false;
            pnlReply.Visible       = false;
            btnGuiPhanHoi.Visible  = false;
            btnDongYeuCau.Visible  = false;
            litReplyError.Text     = "";
        }
    }

    private void BindData()
    {
        byte? tt = byte.TryParse(ddlTrangThai.SelectedValue, out byte t) ? t : (byte?)null;
        var result = TuVanDanhGiaBLL.GetDanhSach(Loai, tt, MaTruong, CurrentPage, PAGE_SIZE);

        litTong.Text          = result.TongSo.ToString("N0");
        gvDanhSach.DataSource = result.Data;
        gvDanhSach.DataBind();

        var pages = new List<object>();
        for (int i = 0; i < result.TongTrang; i++)
            pages.Add(new { PageIndex = i, PageText = (i + 1).ToString(), IsActive = i == CurrentPage });
        rptPaging.DataSource = pages;
        rptPaging.DataBind();
    }
}
