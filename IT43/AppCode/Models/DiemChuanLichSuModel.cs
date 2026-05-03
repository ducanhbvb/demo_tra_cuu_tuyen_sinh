/// <summary>Model — Lịch sử điểm chuẩn qua các năm.</summary>
public class DiemChuanLichSuModel
{
    public int      ID             { get; set; }
    public int      MaTruong       { get; set; }
    public int      MaChuyenNganh  { get; set; }
    public string   TenChuyenNganh { get; set; }
    public int      MaPhuongThuc   { get; set; }
    public string   TenPhuongThuc  { get; set; }
    public short    NamTuyenSinh   { get; set; }
    public decimal? DiemChuan      { get; set; }
    public int?     ChiTieu        { get; set; }
    public string   GhiChu         { get; set; }
}
