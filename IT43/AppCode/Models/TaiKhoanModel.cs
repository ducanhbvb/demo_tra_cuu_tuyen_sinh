using System;

/// <summary>Model — Tài khoản người dùng (tbl_TaiKhoan).</summary>
public class TaiKhoanModel
{
    public int      MaTaiKhoan         { get; set; }
    public string   Email              { get; set; }
    public string   MatKhau            { get; set; }   // lưu hash
    public int      MaQuyen            { get; set; }
    public string   TenQuyen           { get; set; }   // join từ tbl_Quyen
    public DateTime NgayTao            { get; set; }
    public bool     TrangThai          { get; set; }

    // Brute-force
    public int       SoLanDangNhapSai  { get; set; }
    public DateTime? KhoaTaiKhoanDen   { get; set; }
    public DateTime? LanDangNhapCuoi   { get; set; }

    // Email verification
    public bool   EmailDaXacNhan       { get; set; }
    public string TokenXacNhanEmail    { get; set; }

    // Forgot password
    public string   TokenDatLaiMatKhau { get; set; }
    public DateTime? TokenHetHan       { get; set; }

    // Remember Me
    public string TokenNhoMatKhau      { get; set; }

    // Force password change — admin tạo tài khoản / reset mật khẩu sẽ set = true
    public bool   YeuCauDoiMatKhau     { get; set; }

    // TruongHoc — trường gắn với tài khoản (chỉ MaQuyen=2 có giá trị)
    public int?   MaTruong  { get; set; }

    // Helpers — kiểm tra quyền
    public bool BiKhoa        => KhoaTaiKhoanDen.HasValue && KhoaTaiKhoanDen > DateTime.Now;
    public bool IsAdmin        => MaQuyen == 1;
    public bool IsTruongHoc    => MaQuyen == 2;
    public bool IsHocSinh      => MaQuyen == 3;
    public bool IsModerator    => MaQuyen == 4;
    public bool IsTuVanVien    => MaQuyen == 5;

    /// <summary>Có thể truy cập khu vực /Admin/ không? (Admin, Moderator, TuVanVien)</summary>
    public bool CanAccessAdmin    => MaQuyen == 1 || MaQuyen == 4 || MaQuyen == 5;

    /// <summary>Có thể CRUD nội dung (bài viết, tin TS)? (Admin, Moderator)</summary>
    public bool CanManageContent  => MaQuyen == 1 || MaQuyen == 4;

    /// <summary>Có thể phản hồi góp ý/tư vấn? (Admin, Moderator, TuVanVien)</summary>
    public bool CanReplyTuVan     => MaQuyen == 1 || MaQuyen == 4 || MaQuyen == 5;

    /// <summary>Có toàn quyền quản trị (tài khoản, logs, trường)? Chỉ Admin.</summary>
    public bool CanFullAdmin      => MaQuyen == 1;
}
