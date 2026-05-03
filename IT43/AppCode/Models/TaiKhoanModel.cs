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

    // Helpers
    public bool BiKhoa => KhoaTaiKhoanDen.HasValue && KhoaTaiKhoanDen > DateTime.Now;
    public bool IsAdmin    => MaQuyen == 1;
    public bool IsTruong   => MaQuyen == 2;
    public bool IsHocSinh  => MaQuyen == 3;
}
