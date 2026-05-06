using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Trang Hộp thư — HocSinh xem danh sách tư vấn và chi tiết thread.
/// Tách riêng khỏi MyProfile để có giao diện thoáng hơn.
/// </summary>
public partial class Client_HopThu : Page
{
    private int MaTK => SecurityHelper.GetCurrentMaTaiKhoan();

    // Bảng màu gradient cho avatar trường (theo ký tự đầu)
    private static readonly string[] AVATAR_GRADIENTS = {
        "linear-gradient(135deg,#4f46e5,#7c3aed)",
        "linear-gradient(135deg,#ec4899,#f43f5e)",
        "linear-gradient(135deg,#10b981,#059669)",
        "linear-gradient(135deg,#f59e0b,#d97706)",
        "linear-gradient(135deg,#3b82f6,#2563eb)",
        "linear-gradient(135deg,#8b5cf6,#7c3aed)",
        "linear-gradient(135deg,#06b6d4,#0891b2)",
        "linear-gradient(135deg,#f97316,#ea580c)",
    };

    protected void Page_Load(object sender, EventArgs e)
    {
        // Chỉ cho HocSinh truy cập
        if (!User.Identity.IsAuthenticated)
        {
            Response.Redirect("~/Client/Login.aspx");
            return;
        }
        string role = SecurityHelper.GetCurrentRole();
        if (role != "HocSinh")
        {
            Response.Redirect("~/Client/index.aspx");
            return;
        }

        if (!IsPostBack)
        {
            LoadHopThu();
        }
    }

    // ── Load danh sách ───────────────────────────────────────────────────────

    private void LoadHopThu()
    {
        var dt      = HopThuBLL.GetDanhSach(MaTK);
        int chuaDoc = HopThuBLL.DemChuaDoc(MaTK);

        // Tính tổng phản hồi
        int tongPhanHoi = 0;
        foreach (DataRow r in dt.Rows)
            tongPhanHoi += DBHelper.Val<int>(r["SoLuotPhanHoi"]);

        // Cập nhật summary
        litTongCuoc.Text    = dt.Rows.Count.ToString();
        litChuaDoc.Text     = chuaDoc.ToString();
        litTongPhanHoi.Text = tongPhanHoi.ToString();

        // Badge trên tiêu đề
        if (chuaDoc > 0)
            litBadgeHopThu.Text = $" <span class='badge bg-danger rounded-pill' style='font-size:.65rem;vertical-align:middle;'>{chuaDoc}</span>";

        if (dt.Rows.Count == 0)
        {
            litEmpty.Text = "<div class='ht-empty'><div class='ht-empty-icon'><i class='bi bi-inbox'></i></div><div class='fw-semibold mb-1'>Bạn chưa có cuộc tư vấn nào</div><small>Hãy vào trang chi tiết trường để gửi câu hỏi!</small></div>";
            return;
        }

        rptHopThu.DataSource = dt;
        rptHopThu.DataBind();
    }

    // ── ItemCommand — xem thread ─────────────────────────────────────────────

    protected void rptHopThu_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "XemThread") return;
        if (!int.TryParse(e.CommandArgument?.ToString(), out int maTuVan) || maTuVan <= 0) return;

        // Lấy meta trước để xác nhận tư vấn thuộc đúng học sinh hiện tại.
        var r = TuVanDanhGiaBLL.GetChiTiet("TUVAN", maTuVan);
        if (r == null || !IsCurrentUserThread(r))
            return;

        // Đánh dấu đã đọc + xóa cache
        HopThuBLL.DanhDauDaDoc(maTuVan, MaTK);

        // Load toàn bộ lịch sử thuộc cùng hội thoại của học sinh hiện tại, sắp xếp cũ → mới.
        BindThread(maTuVan);
        if (r != null)
        {
            string tenTruong = r["TenTruong"] == DBNull.Value ? "(Chung)" : r["TenTruong"].ToString();
            string ngayGui   = r["NgayGui"] == DBNull.Value ? ""
                : RelativeTime.FromWithTitle(DBHelper.Val<DateTime>(r["NgayGui"]));
            byte tt = DBHelper.Val<byte>(r["TrangThai"]);
            string badge = tt == 0
                ? "<span class='bs-waiting'>Chờ phản hồi</span>"
                : tt == 1 ? "<span class='bs-replied'>Đã phản hồi</span>"
                : "<span class='bs-closed'>Đã đóng</span>";
            // Column tên thực trong tbl_TuVan là NoiDung (CauHoiGoc chỉ có trong SP HopThu)
            string cauHoiText = "";
            if (r.Table.Columns.Contains("NoiDung") && r["NoiDung"] != DBNull.Value)
                cauHoiText = r["NoiDung"].ToString();
            else if (r.Table.Columns.Contains("CauHoiGoc") && r["CauHoiGoc"] != DBNull.Value)
                cauHoiText = r["CauHoiGoc"].ToString();

            litModalTruong.Text = Server.HtmlEncode(tenTruong);
            litHopThuMeta.Text  =
                $"<div class='d-flex gap-2 align-items-center flex-wrap'>" +
                $"<i class='bi bi-building text-muted'></i><strong>{Server.HtmlEncode(tenTruong)}</strong>" +
                $"<span class='text-muted'>·</span><i class='bi bi-clock text-muted'></i><span class='text-muted'>{ngayGui}</span>" +
                $"<span class='text-muted'>·</span>{badge}" +
                $"</div>";
            litCauHoiGoc.Text = string.IsNullOrWhiteSpace(cauHoiText)
                ? "<span class='text-muted fst-italic'>Không có nội dung câu hỏi gốc.</span>"
                : Server.HtmlEncode(cauHoiText);
        }

        hfMaTuVan.Value = maTuVan.ToString();

        // Ẩn/hiện reply box theo trạng thái
        byte trangThai = r != null ? DBHelper.Val<byte>(r["TrangThai"]) : (byte)0;
        pnlReply.Visible = (trangThai != 2); // ẩn khi đóng
        txtReply.Text = "";
        litReplyMsg.Text = "";

        // Reload danh sách để cập nhật badge
        LoadHopThu();
    }

    /// <summary>HocSinh gửi reply vào thread.</summary>
    protected void btnGuiReply_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(hfMaTuVan.Value, out int maTuVan) || maTuVan <= 0) return;

        var currentThread = TuVanDanhGiaBLL.GetChiTiet("TUVAN", maTuVan);
        if (currentThread == null || !IsCurrentUserThread(currentThread))
            return;

        string noiDung = txtReply.Text.Trim();
        if (string.IsNullOrEmpty(noiDung))
        {
            litReplyMsg.Text = "<div class='alert alert-warning py-1 px-3 mb-2' style='font-size:.82rem;'>Vui lòng nhập nội dung.</div>";
            return;
        }

        // Lấy họ tên — fallback về email nếu chưa cập nhật profile
        var p = ProfileBLL.GetProfile(MaTK);
        string hoTen = !string.IsNullOrWhiteSpace(p?.HoTen)
            ? p.HoTen
            : (User.Identity.Name?.Split('@')[0] ?? "Học sinh");

        var (ok, err) = TuVanDanhGiaBLL.GuiReplyHocSinh(maTuVan, MaTK, hoTen, noiDung);
        if (ok)
        {
            txtReply.Text = "";
            litReplyMsg.Text = "<div class='alert alert-success py-1 px-3 mb-2' style='font-size:.82rem;'><i class='bi bi-check2 me-1'></i>Đã gửi phản hồi!</div>";

            // Reload thread và danh sách
            BindThread(maTuVan);
            LoadHopThu();
        }
        else
        {
            litReplyMsg.Text = $"<div class='alert alert-danger py-1 px-3 mb-2' style='font-size:.82rem;'>{err}</div>";
        }
    }

    private void BindThread(int maTuVan)
    {
        var thread = TuVanPhanHoiDAL.GetByMaTuVan(maTuVan, MaTK);
        if (thread.Rows.Count == 0)
        {
            thread.Rows.Add(thread.NewRow());
            thread.Rows[0]["LoaiNguoi"] = "System";
            thread.Rows[0]["HoTen"] = "Hệ thống";
            thread.Rows[0]["NoiDung"] = "Chưa có phản hồi nào trong lịch sử trao đổi.";
        }

        rptThread.DataSource = thread;
        rptThread.DataBind();
    }

    private bool IsCurrentUserThread(DataRow r)
    {
        return r.Table.Columns.Contains("MaTaiKhoan")
            && r["MaTaiKhoan"] != DBNull.Value
            && DBHelper.Val<int>(r["MaTaiKhoan"]) == MaTK;
    }

    // ── Helpers dùng trong Repeater template ─────────────────────────────────

    /// <summary>Trả về CSS class dot cho timeline theo loại người gửi.</summary>
    protected string GetThreadDotClass(string loaiNguoi) => loaiNguoi switch
    {
        "System"    => "dot-system",
        "Admin"     => "dot-admin",
        "Moderator" => "dot-admin",
        "TuVanVien" => "dot-admin",
        "TruongHoc" => "dot-truong",
        _           => "dot-user",
    };

    /// <summary>Gradient style cho avatar avatar trường theo ký tự đầu.</summary>
    protected string GetAvatarStyle(string tenTruong)
    {
        int idx = 0;
        if (!string.IsNullOrEmpty(tenTruong))
            idx = Math.Abs(tenTruong[0]) % AVATAR_GRADIENTS.Length;
        return $"background:{AVATAR_GRADIENTS[idx]};";
    }

    /// <summary>Ký tự đầu viết hoa để hiển thị trong avatar.</summary>
    protected string GetAvatarChar(string tenTruong)
        => !string.IsNullOrEmpty(tenTruong) ? tenTruong.Substring(0, 1).ToUpper() : "?";

    /// <summary>Status string để JS filter theo trạng thái.</summary>
    protected string GetItemStatus(int soChuaDoc, int soLuotPhanHoi, byte trangThai)
    {
        if (soChuaDoc > 0) return "unread";
        if (trangThai == 2) return "closed";
        if (soLuotPhanHoi > 0) return "replied";
        return "waiting";
    }

    /// <summary>ISO date string để JS filter theo thời gian.</summary>
    protected string GetItemDateIso(object phanHoiCuoi, object ngayGui)
    {
        if (phanHoiCuoi != DBNull.Value && phanHoiCuoi is DateTime dt1)
            return dt1.ToString("o");
        if (ngayGui is DateTime dt2)
            return dt2.ToString("o");
        return "";
    }

    /// <summary>Badge HTML theo trạng thái tư vấn.</summary>
    protected string GetBadgeHtml(int soChuaDoc, int soLuotPhanHoi, byte trangThai)
    {
        if (soChuaDoc > 0)
            return $"<span class='bs-unread'>{soChuaDoc} mới</span>";
        if (trangThai == 2)
            return "<span class='bs-closed'>Đã đóng</span>";
        if (soLuotPhanHoi > 0)
            return $"<span class='bs-replied'>{soLuotPhanHoi} lượt</span>";
        return "<span class='bs-waiting'>Chờ PH</span>";
    }

    /// <summary>Render bubble chat cho 1 dòng thread (không phải System).</summary>
    protected string GetBubbleHtml(string loaiNguoi, string hoTen, string noiDung, DateTime? ngayPhanHoi, string loai)
    {
        // HocSinh bubble = bên phải (me), Admin/TuVanVien/TruongHoc = bên trái (them)
        bool isMe = loaiNguoi == "HocSinh";
        string wrapClass = isMe ? "ht-bubble-wrap me" : "ht-bubble-wrap";
        string bubbleClass = isMe ? "ht-bubble me" : "ht-bubble them";

        // Avatar màu theo loại
        string avGrad = isMe
            ? "linear-gradient(135deg,#7c3aed,#4f46e5)"
            : loaiNguoi == "TruongHoc"
                ? "linear-gradient(135deg,#059669,#10b981)"
                : "linear-gradient(135deg,#f59e0b,#d97706)";
        string avChar = !string.IsNullOrEmpty(hoTen) ? hoTen.Substring(0, 1).ToUpper() : "?";
        string avHtml = $"<div class='ht-bubble-av' style='background:{avGrad};'>{avChar}</div>";

        string timeHtml = ngayPhanHoi.HasValue
            ? RelativeTime.FromWithTitle(ngayPhanHoi.Value)
            : "";
        string metaHtml = $"<div class='ht-bubble-meta'><strong>{Server.HtmlEncode(hoTen)}</strong> {timeHtml}</div>";
        string textHtml = $"<div>{Server.HtmlEncode(noiDung)}</div>";

        string bubbleContent = $"<div class='{bubbleClass}'>{metaHtml}{textHtml}</div>";

        return isMe
            ? $"<div class='{wrapClass}'>{bubbleContent}{avHtml}</div>"
            : $"<div class='{wrapClass}'>{avHtml}{bubbleContent}</div>";
    }
}
