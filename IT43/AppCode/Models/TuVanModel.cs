using System;

/// <summary>Model — Yêu cầu tư vấn (tbl_TuVan).</summary>
public class TuVanModel
{
    public int      ID           { get; set; }
    public int?     MaTaiKhoan   { get; set; }
    public int      MaTruong     { get; set; }
    public string   TenTruong    { get; set; }
    public string   HoTen        { get; set; }
    public string   Email        { get; set; }
    public string   SoDienThoai  { get; set; }
    public string   NoiDung      { get; set; }
    public byte     TrangThai    { get; set; }   // 0=Chờ,1=Đã phản hồi,2=Đóng
    public DateTime NgayGui      { get; set; }
    public DateTime? NgayPhanHoi { get; set; }
    public string   GhiChuAdmin  { get; set; }

    public string TenTrangThai => TrangThai switch
    {
        0 => "Chờ xử lý", 1 => "Đã phản hồi", 2 => "Đã đóng", _ => ""
    };
}
