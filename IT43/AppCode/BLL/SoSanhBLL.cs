using System.Collections.Generic;
using System.Data;

/// <summary>
/// Business Logic Layer — So sánh trường (tối đa 3 trường).
/// Parse, validate danh sách MaTruong và gọi SoSanhDAL.
/// </summary>
public static class SoSanhBLL
{
    public const int MAX_SO_SANH = 3;

    /// <summary>
    /// Lấy DataTable so sánh cho danh sách maTruong.
    /// Trả DataTable rỗng nếu danh sách rỗng hoặc vượt quá MAX_SO_SANH.
    /// </summary>
    public static DataTable GetDanhSachSoSanh(List<int> danhSachMaTruong)
    {
        if (danhSachMaTruong == null || danhSachMaTruong.Count == 0)
            return new DataTable();
        if (danhSachMaTruong.Count > MAX_SO_SANH)
            danhSachMaTruong = danhSachMaTruong.GetRange(0, MAX_SO_SANH);

        return SoSanhDAL.GetDanhSachSoSanh(danhSachMaTruong);
    }

    /// <summary>
    /// Parse chuỗi "id1,id2,id3" từ QueryString / Cookie thành List&lt;int&gt;.
    /// Tự loại bỏ giá trị không hợp lệ và trùng lặp.
    /// </summary>
    public static List<int> ParseIds(string raw)
    {
        var result = new List<int>();
        if (string.IsNullOrWhiteSpace(raw)) return result;

        foreach (var part in raw.Split(','))
            if (int.TryParse(part.Trim(), out int id) && id > 0 && !result.Contains(id))
                result.Add(id);

        return result;
    }
}
