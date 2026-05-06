using System.Collections.Generic;
using System.Data;

/// <summary>
/// Business Logic Layer cho tính năng So sánh ngành.
/// Validate danh sách MaTin và gọi DAL tương ứng.
/// </summary>
public static class SoSanhNganhBLL
{
    /// <summary>Số ngành tối đa cho phép so sánh song song.</summary>
    public const int MAX_NGANH = 4;

    /// <summary>
    /// Lấy dữ liệu so sánh cho danh sách MaTin (tối đa MAX_NGANH).
    /// Trả DataTable rỗng nếu danh sách null/empty.
    /// </summary>
    public static DataTable GetDanhSachSoSanh(List<int> danhSachMaTin)
    {
        if (danhSachMaTin == null || danhSachMaTin.Count == 0)
            return new DataTable();

        // Chỉ lấy tối đa MAX_NGANH phần tử đầu (phòng trường hợp session bị thêm thừa)
        var safelist = danhSachMaTin.Count <= MAX_NGANH
            ? danhSachMaTin
            : danhSachMaTin.GetRange(0, MAX_NGANH);

        return SoSanhNganhDAL.GetDanhSachSoSanhNganh(safelist);
    }

    /// <summary>
    /// Lấy danh sách tin tuyển sinh của một trường để hiển thị trong dropdown cascading.
    /// Kết quả không cần cache vì ít trường hợp gọi lặp.
    /// </summary>
    public static DataTable GetNganhCuaTruong(int maTruong, int? nam = null)
    {
        if (maTruong <= 0) return new DataTable();
        return SoSanhNganhDAL.GetNganhCuaTruong(maTruong, nam);
    }
}
