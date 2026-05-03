using System;

/// <summary>Model — Đánh giá trường (tbl_DanhGiaTruong).</summary>
public class DanhGiaModel
{
    public int      ID           { get; set; }
    public int      MaTruong     { get; set; }
    public int      MaTaiKhoan   { get; set; }
    public string   Email        { get; set; }
    public byte     DiemDanhGia  { get; set; }   // 1-5 sao
    public string   NoiDung      { get; set; }
    public byte     TrangThai    { get; set; }   // 0=Pending,1=Hiển thị,2=Ẩn
    public DateTime NgayDang     { get; set; }
}
