using System.Collections.Generic;

/// <summary>
/// Business Logic Layer — Danh sách yêu thích.
/// Kiểm tra, thêm, xóa wishlist và toggle tiện lợi.
/// </summary>
public static class WishListBLL
{
    public static List<WishListModel> GetByTaiKhoan(int maTaiKhoan)
        => WishListDAL.GetByTaiKhoan(maTaiKhoan);

    public static bool DaThem(int maTaiKhoan, int maTruong, int? maChuyenNganh = null)
        => WishListDAL.DaThem(maTaiKhoan, maTruong, maChuyenNganh);

    /// <summary>Thêm vào wishlist (bỏ qua nếu đã tồn tại).</summary>
    public static void Them(int maTaiKhoan, int maTruong, int? maChuyenNganh = null, string ghiChu = null)
        => WishListDAL.Them(maTaiKhoan, maTruong, maChuyenNganh, ghiChu);

    public static void Xoa(int id, int maTaiKhoan)
        => WishListDAL.Xoa(id, maTaiKhoan);

    /// <summary>
    /// Toggle: nếu đã có trong wishlist thì xóa, chưa có thì thêm.
    /// Trả về true = vừa thêm, false = vừa xóa.
    /// </summary>
    public static bool Toggle(int maTaiKhoan, int maTruong, int? maChuyenNganh = null)
    {
        if (WishListDAL.DaThem(maTaiKhoan, maTruong, maChuyenNganh))
        {
            // Tìm ID rồi xóa
            var list = WishListDAL.GetByTaiKhoan(maTaiKhoan);
            foreach (var w in list)
                if (w.MaTruong == maTruong && w.MaChuyenNganh == maChuyenNganh)
                { WishListDAL.Xoa(w.ID, maTaiKhoan); break; }
            return false;
        }
        WishListDAL.Them(maTaiKhoan, maTruong, maChuyenNganh);
        return true;
    }
}
