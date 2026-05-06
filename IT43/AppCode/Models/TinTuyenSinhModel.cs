using System;

/// <summary>Model — Tin tuyển sinh (tbl_TinTuyenSinh).</summary>
public class TinTuyenSinhModel
{
    public int      MaTin              { get; set; }
    public int      MaTruong           { get; set; }
    public string   TenTruong          { get; set; }
    public string   TinhThanh          { get; set; }
    public string   AnhDaiDien         { get; set; }
    public string   TruongSlug         { get; set; }
    public int      MaChuyenNganh      { get; set; }
    public string   TenChuyenNganh     { get; set; }
    public int      MaPhuongThuc       { get; set; }
    public string   TenPhuongThuc      { get; set; }
    public short    NamTuyenSinh       { get; set; }
    public int?     ChiTieu            { get; set; }
    public string   HocPhi             { get; set; }  // Sprint 1: đổi từ decimal? → string để hỗ trợ text tự do "10-20 triệu"
    public string   ToHopMonHoc        { get; set; }
    public decimal? DiemChuanNamTruoc  { get; set; }
    public decimal? DiemChuanNamNay    { get; set; }
    public decimal? ChenhLechDiem      { get; set; }
    public string   LoaiHinhDaoTao     { get; set; }
    public string   CoSoDaoTao         { get; set; }
    public string   MoTa               { get; set; }
    public string   TieuDe             { get; set; }
    public DateTime? HanNop            { get; set; }
    public DateTime NgayDang           { get; set; }
    public int      LuotXem            { get; set; }
    public bool     TrangThai          { get; set; }

    // Helper: tự sinh tiêu đề nếu để trống
    public string TieuDeHienThi =>
        !string.IsNullOrWhiteSpace(TieuDe) ? TieuDe
        : $"Tuyển sinh {NamTuyenSinh} - {TenChuyenNganh} ({TenPhuongThuc})";
}
