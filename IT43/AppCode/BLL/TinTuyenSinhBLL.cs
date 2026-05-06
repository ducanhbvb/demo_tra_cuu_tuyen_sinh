using System;

/// <summary>
/// Business Logic Layer — Tin tuyển sinh.
/// Validate dữ liệu và gọi DAL cho CRUD tin tuyển sinh (client + admin).
/// </summary>
public static class TinTuyenSinhBLL
{
    /// <summary>Tìm kiếm tin tuyển sinh phía Client (chỉ tin đang hiển thị).</summary>
    public static PagedTable TimKiem(int? maTruong, int? maChuyenNganh, int? maPhuongThuc,
        short? nam, decimal? diemTu, decimal? diemDen, string tinhThanh,
        int pageIndex = 0, int pageSize = 20)
        => TinTuyenSinhDAL.TimKiem(maTruong, maChuyenNganh, maPhuongThuc,
            nam, diemTu, diemDen, tinhThanh, pageIndex, pageSize);

    /// <summary>Tìm kiếm tin tuyển sinh phía Admin (bao gồm cả tin đang ẩn).</summary>
    public static PagedTable TimKiemAdmin(int? maTruong, int? maChuyenNganh,
        short? nam, int pageIndex = 0, int pageSize = 20)
        => TinTuyenSinhDAL.TimKiemAdmin(maTruong, maChuyenNganh, nam, pageIndex, pageSize);

    /// <summary>
    /// Validate toàn bộ dữ liệu tin tuyển sinh — dùng chung cho Thêm và Cập nhật.
    /// Trả về null nếu hợp lệ, trả về string lỗi nếu không hợp lệ.
    /// </summary>
    private static string Validate(TinTuyenSinhModel m, bool isThem)
    {
        // Bắt buộc: Trường, Ngành, Phương thức
        if (m.MaTruong <= 0 || m.MaChuyenNganh <= 0 || m.MaPhuongThuc <= 0)
            return "Vui lòng chọn đầy đủ Trường, Ngành và Phương thức xét tuyển.";

        // Năm tuyển sinh hợp lệ
        if (m.NamTuyenSinh < 2020 || m.NamTuyenSinh > DateTime.Now.Year + 1)
            return $"Năm tuyển sinh phải trong khoảng 2020 – {DateTime.Now.Year + 1}.";

        // Chỉ tiêu: nếu có giá trị thì phải hợp lệ
        if (m.ChiTieu.HasValue && (m.ChiTieu.Value < 1 || m.ChiTieu.Value > 100000))
            return "Chỉ tiêu phải từ 1 đến 100.000.";

        // Điểm chuẩn năm trước: 0 – 40
        if (m.DiemChuanNamTruoc.HasValue && (m.DiemChuanNamTruoc < 0 || m.DiemChuanNamTruoc > 40))
            return "Điểm chuẩn năm trước phải trong khoảng 0 – 40.";

        // Điểm chuẩn năm nay: 0 – 40
        if (m.DiemChuanNamNay.HasValue && (m.DiemChuanNamNay < 0 || m.DiemChuanNamNay > 40))
            return "Điểm chuẩn năm nay phải trong khoảng 0 – 40.";

        // Học phí: Sprint 1 — NVARCHAR(100), chỉ kiểm tra độ dài
        if (!string.IsNullOrWhiteSpace(m.HocPhi) && m.HocPhi.Length > 100)
            return "Học phí tối đa 100 ký tự.";

        // Tổ hợp môn: định dạng A00,A01,D01
        if (!string.IsNullOrWhiteSpace(m.ToHopMonHoc) &&
            !System.Text.RegularExpressions.Regex.IsMatch(m.ToHopMonHoc,
                @"^[A-Z][0-9]{2}(,[A-Z][0-9]{2})*$"))
            return "Tổ hợp môn sai định dạng. Ví dụ: A00,A01,D01";

        // Hạn nộp: không được trong quá khứ, trừ khi Admin bật cấu hình cho phép nhập ngày quá khứ.
        bool allowPastDates = ConfigBLL.GetBool("AllowPastDates", false);
        if (!allowPastDates && m.HanNop.HasValue && m.HanNop.Value.Date < DateTime.Today)
            return "Hạn nộp hồ sơ không được trong quá khứ.";

        // Tiêu đề: nếu có thì từ 5–200 ký tự
        if (!string.IsNullOrWhiteSpace(m.TieuDe) && (m.TieuDe.Length < 5 || m.TieuDe.Length > 200))
            return "Tiêu đề phải từ 5 đến 200 ký tự.";

        // Loại hình đào tạo: tối đa 100 ký tự
        if (!string.IsNullOrWhiteSpace(m.LoaiHinhDaoTao) && m.LoaiHinhDaoTao.Length > 100)
            return "Loại hình đào tạo tối đa 100 ký tự.";

        // Cơ sở đào tạo: tối đa 200 ký tự
        if (!string.IsNullOrWhiteSpace(m.CoSoDaoTao) && m.CoSoDaoTao.Length > 200)
            return "Cơ sở đào tạo tối đa 200 ký tự.";

        return null; // hợp lệ
    }

    /// <summary>Validate và thêm tin tuyển sinh mới.</summary>
    public static (bool ok, string error) Them(TinTuyenSinhModel m)
    {
        var err = Validate(m, isThem: true);
        if (err != null) return (false, err);

        m.MoTa = HtmlSanitizerHelper.SanitizeRichText(m.MoTa);

        TinTuyenSinhDAL.Them(m);
        InvalidateCache();
        return (true, null);
    }

    /// <summary>Validate và cập nhật tin tuyển sinh.</summary>
    public static (bool ok, string error) CapNhat(TinTuyenSinhModel m)
    {
        var err = Validate(m, isThem: false);
        if (err != null) return (false, err);

        m.MoTa = HtmlSanitizerHelper.SanitizeRichText(m.MoTa);

        TinTuyenSinhDAL.CapNhat(m);
        InvalidateCache();
        return (true, null);
    }

    /// <summary>Xóa tin tuyển sinh theo mã tin.</summary>
    public static void Xoa(int maTin)
    {
        TinTuyenSinhDAL.Xoa(maTin);
        InvalidateCache();
    }

    /// <summary>Toggle trạng thái hiển thị (ẩn/hiện) tin tuyển sinh.</summary>
    public static void ToggleTrangThai(int maTin)
    {
        TinTuyenSinhDAL.ToggleTrangThai(maTin);
        InvalidateCache();
    }

    /// <summary>
    /// Xóa cache thống kê trang chủ khi tin tuyển sinh thay đổi
    /// (SoTinActive, NamTuyenSinhMoiNhat có thể bị ảnh hưởng).
    /// </summary>
    public static void InvalidateCache()
    {
        CacheHelper.Remove(CacheHelper.KEY_SO_TIN_ACTIVE);
        CacheHelper.Remove(CacheHelper.KEY_NAM_MOI_NHAT);
        // Xóa cache năm tuyển sinh trong dropdown nếu có năm mới
        CacheHelper.Remove(CacheHelper.KEY_NAM_TUYEN_SINH);
    }
}
