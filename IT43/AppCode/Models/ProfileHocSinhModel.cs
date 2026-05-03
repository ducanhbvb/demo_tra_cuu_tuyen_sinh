using System;

/// <summary>Model — Hồ sơ học sinh (tbl_ProfileHocSinh).</summary>
public class ProfileHocSinhModel
{
    public int      ID             { get; set; }
    public int      MaTaiKhoan     { get; set; }
    public string   HoTen          { get; set; }
    public DateTime? NgaySinh      { get; set; }
    public string   TinhThanh      { get; set; }
    public decimal? DiemDuKien     { get; set; }   // tổng 3 môn dự kiến
    public string   DiemMonHoc     { get; set; }   // JSON {"Toan":9.5,...}
    public int?     MaChuyenNganh  { get; set; }
    public string   TenChuyenNganh { get; set; }
    public string   MucTieuNghe    { get; set; }
    public byte?    KhuVuc         { get; set; }   // 1,2,3
    public string   AnhDaiDien     { get; set; }
    public DateTime NgayCapNhat    { get; set; }
}
