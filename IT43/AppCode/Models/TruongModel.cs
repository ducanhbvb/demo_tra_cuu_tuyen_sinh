using System;

/// <summary>Model — Trường đại học/cao đẳng (tbl_Truong).</summary>
public class TruongModel
{
    public int      MaTruong           { get; set; }
    public string   TenTruong          { get; set; }
    public string   DiaChi             { get; set; }
    public string   TinhThanh          { get; set; }
    public byte?    MaVung             { get; set; }   // 1=Bắc, 2=Trung, 3=Nam
    public byte?    LoaiTruong         { get; set; }   // 1=Công lập, 2=Tư thục, 3=Quốc tế
    public string   SoDienThoai        { get; set; }
    public string   Website            { get; set; }
    public string   AnhDaiDien         { get; set; }
    public string   AnhBia             { get; set; }
    public string   MoTa               { get; set; }
    public string   QuyMo              { get; set; }
    public bool     KiemDinhChatLuong  { get; set; }
    public bool     TrangThai          { get; set; } = true;
    public string   Slug               { get; set; }
    public int      MaTaiKhoan         { get; set; }
    public DateTime ThoiGianCapNhat    { get; set; }

    // Tính từ SP / subquery
    public double? DiemDanhGiaTB       { get; set; }
    public int     SoLuongDanhGia      { get; set; }

    // Helper
    public string TenLoaiTruong => LoaiTruong switch
    {
        1 => "Công lập", 2 => "Tư thục", 3 => "Quốc tế", _ => "Khác"
    };
    public string TenVung => MaVung switch
    {
        1 => "Miền Bắc", 2 => "Miền Trung", 3 => "Miền Nam", _ => ""
    };
}
