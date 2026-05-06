using System;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Business Logic Layer — Tư vấn, Góp ý và Đánh giá trường.
/// Validate input trước khi gọi DAL tương ứng.
/// </summary>
public static class TuVanDanhGiaBLL
{
    // ── Tư vấn ────────────────────────────────────────────────────────────

    /// <summary>
    /// Validate và gửi yêu cầu tư vấn.
    /// Trả (true, idMoi) hoặc (false, thông báo lỗi).
    /// </summary>
    public static (bool ok, string errorOrId) GuiTuVan(TuVanModel m)
    {
        if (string.IsNullOrWhiteSpace(m.HoTen))
            return (false, "Vui lòng nhập họ tên.");
        if (string.IsNullOrWhiteSpace(m.Email))
            return (false, "Vui lòng nhập email.");
        if (string.IsNullOrWhiteSpace(m.NoiDung))
            return (false, "Vui lòng nhập nội dung câu hỏi.");
        if (m.MaTruong <= 0)
            return (false, "Thông tin trường không hợp lệ.");

        int id = TuVanDanhGiaDAL.GuiTuVan(m);

        // Auto-insert system event vào timeline (M4 — mockup: "Hệ thống tự động nhận câu hỏi")
        try
        {
            TuVanPhanHoiDAL.Them(id, null, "System", "Hệ thống",
                "Hệ thống đã tự động ghi nhận câu hỏi của bạn. Tư vấn viên sẽ phản hồi sớm nhất có thể.");
        }
        catch (Exception ex)
        {
            LogHelper.Ghi(null, "GuiTuVan.SystemEvent", ex.Message, isSuccess: false);
        }

        return (true, id.ToString());
    }

    // ── Góp ý ─────────────────────────────────────────────────────────────

    public static (bool ok, string error) GuiGopY(int? maTaiKhoan, int? maTruong, byte loaiGopY, string noiDung)
    {
        if (string.IsNullOrWhiteSpace(noiDung))
            return (false, "Nội dung góp ý không được để trống.");

        TuVanDanhGiaDAL.GuiGopY(maTaiKhoan, maTruong, loaiGopY, noiDung);
        return (true, null);
    }

    // ── Admin: chi tiết & danh sách ──────────────────────────────────────

    /// <summary>Lấy chi tiết 1 bản ghi tư vấn/góp ý theo ID để hiển thị trong modal.</summary>
    public static System.Data.DataRow GetChiTiet(string loai, int id)
        => TuVanDanhGiaDAL.GetChiTiet(loai, id);

    /// <summary>
    /// Lấy chi tiết bản ghi tư vấn/góp ý theo ID và kiểm tra thuộc về <paramref name="maTruong"/>.
    /// Trả null nếu không tồn tại hoặc không thuộc trường đó.
    /// </summary>
    public static System.Data.DataRow GetChiTietCuaTruong(string loai, int id, int maTruong)
    {
        var row = TuVanDanhGiaDAL.GetChiTiet(loai, id);
        if (row == null) return null;
        // Kiểm tra quyền: bản ghi phải thuộc trường hiện tại
        if (row.Table.Columns.Contains("MaTruong"))
        {
            var mt = row["MaTruong"];
            if (mt == System.DBNull.Value || (int)mt != maTruong)
                return null;
        }
        return row;
    }

    public static PagedTable GetDanhSach(string loai, byte? trangThai, int? maTruong,
                                         int pageIndex, int pageSize)
        => TuVanDanhGiaDAL.GetDanhSach(loai, trangThai, maTruong, pageIndex, pageSize);

    public static void CapNhatTrangThai(string loai, int id, byte trangThai, string ghiChu = null)
        => TuVanDanhGiaDAL.CapNhatTrangThai(loai, id, trangThai, ghiChu);

    /// <summary>
    /// Admin/TuVanVien gửi phản hồi vào thread tư vấn.
    /// Tự động: insert timeline row, cập nhật TrangThai=1, gửi email (nếu có), invalidate cache HopThu.
    /// </summary>
    /// <param name="maTuVan">ID tư vấn cần phản hồi.</param>
    /// <param name="maTaiKhoanAdmin">MaTaiKhoan của admin đang đăng nhập.</param>
    /// <param name="role">Role của admin: Admin | Moderator | TuVanVien</param>
    /// <param name="hoTenAdmin">Họ tên admin (cache vào timeline).</param>
    /// <param name="noiDungPhanHoi">Nội dung phản hồi (bắt buộc, không để trống).</param>
    /// <param name="guiEmail">Có gửi email thông báo cho học sinh không.</param>
    /// <param name="emailNguoiHoi">Email người gửi tư vấn (để gửi email thông báo).</param>
    /// <param name="tenNguoiHoi">Họ tên người gửi (để cá nhân hoá email).</param>
    /// <param name="cauHoiGoc">Nội dung câu hỏi gốc (trích dẫn trong email).</param>
    /// <param name="tenTruong">Tên trường liên quan.</param>
    /// <param name="maTaiKhoanHocSinh">MaTaiKhoan của HocSinh (nullable — nếu có sẽ invalidate cache HopThu).</param>
    /// <returns>(true, null) nếu thành công; (false, errorMsg) nếu validate fail.</returns>
    public static (bool ok, string error) GuiPhanHoi(
        int    maTuVan,
        int    maTaiKhoanAdmin,
        string role,
        string hoTenAdmin,
        string noiDungPhanHoi,
        bool   guiEmail,
        string emailNguoiHoi,
        string tenNguoiHoi,
        string cauHoiGoc,
        string tenTruong,
        int?   maTaiKhoanHocSinh = null)
    {
        if (string.IsNullOrWhiteSpace(noiDungPhanHoi))
            return (false, "Vui lòng nhập nội dung phản hồi.");
        if (noiDungPhanHoi.Length > 4000)
            return (false, "Nội dung phản hồi không được vượt quá 4000 ký tự.");

        // 1. Insert vào timeline
        int phanHoiId = TuVanPhanHoiDAL.Them(maTuVan, maTaiKhoanAdmin, role, hoTenAdmin, noiDungPhanHoi.Trim());

        // 2. Cập nhật TrangThai → 1 (Đã phản hồi)
        TuVanDanhGiaDAL.CapNhatTrangThai("TUVAN", maTuVan, 1);

        // 3. Gửi email nếu user tích checkbox và có email
        if (guiEmail && !string.IsNullOrWhiteSpace(emailNguoiHoi))
        {
            try
            {
                EmailHelper.GuiPhanHoiTuVan(emailNguoiHoi, tenNguoiHoi ?? "Bạn",
                                             cauHoiGoc ?? "", noiDungPhanHoi.Trim(), tenTruong ?? "");
                TuVanPhanHoiDAL.DanhDauDaGuiEmail(phanHoiId);
            }
            catch (Exception ex)
            {
                // Lỗi email không làm fail cả action — log và tiếp tục
                LogHelper.Ghi(maTaiKhoanAdmin, "GuiPhanHoi.SendEmail", $"maTuVan={maTuVan}: {ex.Message}", isSuccess: false);
            }
        }

        // 4. Invalidate cache HopThu của HocSinh (nếu họ có tài khoản)
        if (maTaiKhoanHocSinh.HasValue && maTaiKhoanHocSinh.Value > 0)
        {
            try { HopThuBLL.InvalidateCache(maTaiKhoanHocSinh.Value); }
            catch { /* ignore cache error */ }
        }

        return (true, null);
    }

    /// <summary>
    /// HocSinh reply lại trong thread (qua hộp thư).
    /// Rate-limit: tối đa 10 lượt/24h.
    /// </summary>
    public static (bool ok, string error) GuiReplyHocSinh(
        int maTuVan, int maTaiKhoan, string hoTen, string noiDung)
    {
        if (string.IsNullOrWhiteSpace(noiDung))
            return (false, "Vui lòng nhập nội dung.");
        if (noiDung.Length > 2000)
            return (false, "Nội dung không được vượt quá 2000 ký tự.");

        // Rate-limit chống spam: tối đa 10 reply trong 24h
        int soReply24h = TuVanPhanHoiDAL.DemReplyTrongNgay(maTaiKhoan);
        if (soReply24h >= 10)
            return (false, "Bạn đã gửi quá nhiều phản hồi hôm nay. Vui lòng thử lại sau.");

        TuVanPhanHoiDAL.Them(maTuVan, maTaiKhoan, "HocSinh", hoTen, noiDung.Trim());
        return (true, null);
    }

    // ── Đánh giá trường ───────────────────────────────────────────────────

    public static DataTable GetDanhGia(int maTruong, byte? trangThai = 1)
        => TuVanDanhGiaDAL.GetDanhGia(maTruong, trangThai);

    /// <summary>
    /// Validate và lưu đánh giá (upsert).
    /// Điểm hợp lệ: 1–5.
    /// </summary>
    public static (bool ok, string error) GuiDanhGia(int maTruong, int maTaiKhoan, byte diem, string noiDung)
    {
        if (diem < 1 || diem > 5)
            return (false, "Điểm đánh giá phải từ 1 đến 5.");
        if (maTruong <= 0)
            return (false, "Thông tin trường không hợp lệ.");

        TuVanDanhGiaDAL.GuiDanhGia(maTruong, maTaiKhoan, diem, noiDung);
        return (true, null);
    }
}
