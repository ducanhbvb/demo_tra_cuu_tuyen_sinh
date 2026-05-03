using System;

/// <summary>Model — Trường yêu thích (tbl_WishList).</summary>
public class WishListModel
{
    public int      ID             { get; set; }
    public int      MaTaiKhoan     { get; set; }
    public int      MaTruong       { get; set; }
    public string   TenTruong      { get; set; }
    public string   TruongSlug     { get; set; }
    public string   AnhDaiDien     { get; set; }
    public string   TinhThanh      { get; set; }
    public int?     MaChuyenNganh  { get; set; }
    public string   TenChuyenNganh { get; set; }
    public string   GhiChu         { get; set; }
    public DateTime NgayThem       { get; set; }
}
