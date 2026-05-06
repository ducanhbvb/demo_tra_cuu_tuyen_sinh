using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

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
        gvDanhSach.PreRender += gvDanhSach_PreRender;
        LoadCounts();
        if (!IsPostBack)
        {
            LoadTruongDropdown();
            BindData();
        }
        else
        {
            // EnableViewState="false" → rebind để LinkButton trong GridView tồn tại trước RowCommand
            BindData();
        }
        // Chỉ hiện filter trường khi ở tab Tư vấn
        pnlFilterTruong.Visible = (Loai == "TUVAN");
    }

    /// <summary>Load dropdown danh sách trường (chỉ dùng 1 lần lúc đầu).</summary>
    private void LoadTruongDropdown()
    {
        var list = TruongBLL.GetDanhSachDropdown();
        ddlTruong.Items.Clear();
        ddlTruong.Items.Add(new ListItem("-- Tất cả trường --", ""));
        foreach (var t in list)
            ddlTruong.Items.Add(new ListItem(t.Ten, t.Id.ToString()));
    }

    /// <summary>Đếm số tư vấn/góp ý chờ xử lý để hiển thị badge trên tab.</summary>
    private void LoadCounts()
    {
        int soTuVan = ThongKeBLL.SoTuVanCho();
        int soGopY  = ThongKeBLL.SoGopYCho();
        litSoTuVan.Text = soTuVan > 0 ? $"<span class='badge'>{soTuVan}</span>" : "";
        litSoGopY.Text  = soGopY  > 0 ? $"<span class='badge'>{soGopY}</span>"  : "";
    }

    /// <summary>Xử lý chuyển tab (gọi từ JS switchTab qua hidden button).</summary>
    protected void btnDoTab_Click(object sender, EventArgs e)
    {
        ddlTrangThai.SelectedIndex = 0;
        ddlTruong.SelectedIndex = 0;
        CurrentPage = 0;
        pnlFilterTruong.Visible = (Loai == "TUVAN");
        BindData();
    }

    /// <summary>Khi thay đổi dropdown trạng thái → reset trang và tải lại.</summary>
    protected void ddlTrangThai_Changed(object sender, EventArgs e)
    {
        CurrentPage = 0;
        BindData();
    }

    /// <summary>Khi thay đổi dropdown trường → reset trang và tải lại.</summary>
    protected void ddlTruong_Changed(object sender, EventArgs e)
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
                TuVanDanhGiaBLL.CapNhatTrangThai(Loai, id, 1);
                litThongBao.Text = "<div class='alert alert-success py-2'>Đã đánh dấu phản hồi.</div>";
                break;

            case "RollBack":
                TuVanDanhGiaBLL.CapNhatTrangThai(Loai, id, 0);
                litThongBao.Text = "<div class='alert alert-warning py-2'>Đã hoàn tác về Chờ xử lý.</div>";
                break;

            case "Dong":
                TuVanDanhGiaBLL.CapNhatTrangThai(Loai, id, 2);
                litThongBao.Text = "<div class='alert alert-secondary py-2'>Đã đóng yêu cầu.</div>";
                break;

            case "MoLai":
                TuVanDanhGiaBLL.CapNhatTrangThai(Loai, id, 0);
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

    /// <summary>Load chi tiết tư vấn/góp ý theo ID vào modal thread.</summary>
    private void LoadChiTiet(int id)
    {
        var r = TuVanDanhGiaBLL.GetChiTiet(Loai, id);
        if (r == null) return;

        hfMaTuVanHienTai.Value = id.ToString();

        // ── Info bar ──────────────────────────────────────────────────────────
        if (Loai == "TUVAN")
        {
            string hoTen = r["HoTen"] == DBNull.Value ? "" : r["HoTen"].ToString();
            litCTHoTen.Text = "<strong>" + Server.HtmlEncode(hoTen) + "</strong>";
            litCTEmail.Text = Server.HtmlEncode(r["Email"] == DBNull.Value ? "" : r["Email"].ToString());
        }
        else
        {
            litCTHoTen.Text = "<span class='text-muted fst-italic'>Ẩn danh</span>";
            litCTEmail.Text = "<span class='text-muted'>—</span>";
        }

        string tenTruong = r["TenTruong"] == DBNull.Value ? "(Chung)" : r["TenTruong"].ToString();
        litCTTruong.Text = Server.HtmlEncode(tenTruong);

        litCTNgay.Text = r["NgayGui"] == DBNull.Value ? ""
            : RelativeTime.FromWithTitle(DBHelper.Val<DateTime>(r["NgayGui"]));

        byte tt = DBHelper.Val<byte>(r["TrangThai"]);
        litCTTrangThai.Text = tt == 0
            ? "<span class='badge badge-soft-warning'><i class='bi bi-hourglass-split me-1'></i>Chờ phản hồi</span>"
            : tt == 1 ? "<span class='badge badge-soft-success'><i class='bi bi-check-circle me-1'></i>Đã phản hồi</span>"
            : "<span class='badge badge-soft-secondary'><i class='bi bi-x-circle me-1'></i>Đã đóng</span>";

        // ── Header gradient động theo trạng thái (M5) ────────────────────────
        // Tím đồng nhất với navbar site — sắc độ theo trạng thái
        string gradientStyle = tt == 0
            ? "background:linear-gradient(135deg,#7c3aed,#6d28d9)"   // tím đậm = chờ
            : tt == 1 ? "background:linear-gradient(135deg,#8b5cf6,#7c3aed)"  // tím sáng = đã phản hồi
            : "background:linear-gradient(135deg,#64748b,#475569)";  // xám = đóng

        string loaiLabel = Loai == "TUVAN" ? "Tư vấn" : "Góp ý";
        string nguoiGui  = Loai == "TUVAN" && r.Table.Columns.Contains("HoTen") && r["HoTen"] != DBNull.Value
            ? r["HoTen"].ToString() : "Ẩn danh";
        litModalHeader.Text = $@"<div style='{gradientStyle};padding:16px 20px;border-radius:12px 12px 0 0;color:#fff;flex:1'>
            <h5 class='modal-title mb-0 fw-bold'>
                <i class='bi bi-chat-left-dots me-2'></i>Chi tiết {loaiLabel} — {Server.HtmlEncode(nguoiGui)}
            </h5></div>";

        // ── Nội dung câu hỏi gốc (hiện với cả TUVAN và GOPY) ─────────────────
        litCTNoiDung.Text = Server.HtmlEncode(
            r["NoiDung"] == DBNull.Value ? "" : r["NoiDung"].ToString());

        // ── Nội dung / Thread ─────────────────────────────────────────────────
        pnlGhiChu.Visible = false;  // ẩn panel cũ

        if (Loai == "TUVAN")
        {
            // Hiện timeline thread
            pnlThread.Visible = true;
            pnlNoiDungGopY.Visible = false;

            var thread = TuVanPhanHoiDAL.GetByMaTuVan(id);
            rptThread.DataSource = thread;
            rptThread.DataBind();

            // Reply box: chỉ hiện khi chưa đóng
            pnlReply.Visible = (tt != 2);
            btnGuiPhanHoi.Visible = (tt != 2);
            btnDongYeuCau.Visible = (tt != 2);
            txtReply.Text = "";
            litReplyError.Text = "";
            chkGuiEmail.Checked = true;
        }
        else
        {
            // Góp ý: chỉ xem nội dung tĩnh, không có thread
            pnlThread.Visible = false;
            pnlNoiDungGopY.Visible = false;  // nội dung đã hiện ở trên
            pnlReply.Visible = false;
            btnGuiPhanHoi.Visible = false;
            btnDongYeuCau.Visible = (tt != 2);

            litCTNoiDung.Text = Server.HtmlEncode(
                r["NoiDung"] == DBNull.Value ? "" : r["NoiDung"].ToString());
        }
    }

    /// <summary>Xử lý nút "Gửi phản hồi" trong modal — gọi BLL.GuiPhanHoi.</summary>
    protected void btnGuiPhanHoi_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(hfMaTuVanHienTai.Value, out int maTuVan) || maTuVan <= 0)
            return;

        string noiDung = txtReply.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(noiDung))
        {
            litReplyError.Text = "<div class='alert alert-warning py-1 mt-2 small'>Vui lòng nhập nội dung phản hồi.</div>";
            ShowDetail = "true";
            LoadChiTiet(maTuVan);
            BindData();
            return;
        }

        // Lấy thông tin tư vấn để gửi email
        var r = TuVanDanhGiaBLL.GetChiTiet("TUVAN", maTuVan);
        if (r == null) return;

        string emailNguoiHoi = r["Email"] == DBNull.Value ? "" : r["Email"].ToString();
        string tenNguoiHoi   = r["HoTen"] == DBNull.Value ? "Bạn" : r["HoTen"].ToString();
        string cauHoiGoc     = r["NoiDung"] == DBNull.Value ? "" : r["NoiDung"].ToString();
        string tenTruong     = r["TenTruong"] == DBNull.Value ? "" : r["TenTruong"].ToString();
        int? maTaiKhoanHocSinh = r.Table.Columns.Contains("MaTaiKhoan") && r["MaTaiKhoan"] != DBNull.Value
            ? (int?)DBHelper.Val<int>(r["MaTaiKhoan"]) : null;

        string hoTenAdmin = SecurityHelper.GetCurrentRole(); // fallback
        int maTKAdmin     = SecurityHelper.GetCurrentMaTaiKhoan();
        string role       = SecurityHelper.GetCurrentRole(); // Admin/Moderator/TuVanVien

        var (ok, error) = TuVanDanhGiaBLL.GuiPhanHoi(
            maTuVan, maTKAdmin, role, hoTenAdmin,
            noiDung, chkGuiEmail.Checked,
            emailNguoiHoi, tenNguoiHoi, cauHoiGoc, tenTruong,
            maTaiKhoanHocSinh);

        if (!ok)
        {
            litReplyError.Text = $"<div class='alert alert-warning py-1 mt-2 small'>{error}</div>";
        }
        else
        {
            litThongBao.Text = "<div class='alert alert-success py-2'><i class='bi bi-check-circle me-1'></i>Đã gửi phản hồi thành công.</div>";
        }

        LoadCounts();
        LoadChiTiet(maTuVan);
        ShowDetail = "true";
        BindData();
    }

    /// <summary>Xử lý nút "Đóng yêu cầu" trong modal footer.</summary>
    protected void btnDongYeuCau_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(hfMaTuVanHienTai.Value, out int maTuVan) || maTuVan <= 0)
            return;

        TuVanDanhGiaBLL.CapNhatTrangThai(Loai, maTuVan, 2);
        litThongBao.Text = "<div class='alert alert-secondary py-2'>Đã đóng yêu cầu.</div>";

        LoadCounts();
        LoadChiTiet(maTuVan);
        ShowDetail = "true";
        BindData();
    }

    // ── Helpers dùng trong Repeater template ────────────────────────────────

    /// <summary>CSS class cho timeline dot theo loại người gửi.</summary>
    protected string GetDotClass(string loaiNguoi) => loaiNguoi switch
    {
        "System"    => "dot-system",
        "Admin"     => "dot-admin",
        "Moderator" => "dot-admin",
        "TuVanVien" => "dot-admin",
        "TruongHoc" => "dot-truong",
        _           => "dot-user",  // HocSinh / default
    };

    /// <summary>Label hiển thị loại người gửi trong timeline meta.</summary>
    protected string GetLoaiNguoiLabel(string loaiNguoi) => loaiNguoi switch
    {
        "System"    => "",
        "Admin"     => "<span class='text-muted small'>(Quản trị viên)</span>",
        "Moderator" => "<span class='text-muted small'>(Moderator)</span>",
        "TuVanVien" => "<span class='text-muted small'>(Tư vấn viên)</span>",
        "TruongHoc" => "<span class='text-muted small'>(Đại diện trường)</span>",
        "HocSinh"   => "<span class='text-muted small'>(Học sinh)</span>",
        _           => "",
    };

    /// <summary>Xử lý chuyển trang qua Repeater phân trang.</summary>
    protected void rptPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Page" && int.TryParse(e.CommandArgument?.ToString(), out int pg))
        {
            CurrentPage = pg;
            BindData();
        }
    }

    protected void gvDanhSach_PreRender(object sender, EventArgs e)
    {
        if (gvDanhSach.HeaderRow != null)
            gvDanhSach.HeaderRow.TableSection = TableRowSection.TableHeader;
    }

    /// <summary>Color-code row theo trạng thái: vàng=chờ, xanh=đã phản hồi, xám=đóng.</summary>
    protected void gvDanhSach_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow) return;
        var row = e.Row.DataItem as DataRowView;
        if (row == null) return;
        byte tt = DBHelper.Val<byte>(row["TrangThai"]);
        switch (tt)
        {
            case 0: e.Row.CssClass += " table-row-pending";   break; // chờ xử lý
            case 1: e.Row.CssClass += " table-row-replied";   break; // đã phản hồi
            case 2: e.Row.CssClass += " table-row-closed";    break; // đã đóng
        }
    }

    /// <summary>Gọi TuVanDanhGiaBLL.GetDanhSach với bộ lọc loại/trạng thái/trường, bind GridView + phân trang.</summary>
    private void BindData()
    {
        byte? tt = byte.TryParse(ddlTrangThai.SelectedValue, out byte t) ? t : (byte?)null;
        int? maTruong = null;
        if (Loai == "TUVAN" && int.TryParse(ddlTruong.SelectedValue, out int mt) && mt > 0)
            maTruong = mt;
        var result = TuVanDanhGiaBLL.GetDanhSach(Loai, tt, maTruong, CurrentPage, PAGE_SIZE);

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
