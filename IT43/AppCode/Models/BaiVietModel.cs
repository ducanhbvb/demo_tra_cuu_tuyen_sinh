using System;

/// <summary>Model — Bài viết (tbl_BaiViet).</summary>
public class BaiVietModel
{
    public int      MaBaiViet  { get; set; }
    public string   TieuDe     { get; set; }
    public string   Slug       { get; set; }
    public string   AnhChinh   { get; set; }
    public string   NoiDung    { get; set; }
    public int      MaTacGia   { get; set; }
    public DateTime NgayDang   { get; set; }
    public int      LuotXem    { get; set; }
    public bool     TrangThai  { get; set; }
    public int?     MaTruong   { get; set; }
    public string   TenTruong  { get; set; }
}
