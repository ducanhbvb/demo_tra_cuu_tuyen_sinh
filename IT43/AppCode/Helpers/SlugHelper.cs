using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Helper tạo URL slug từ chuỗi tiếng Việt (loại dấu, chuẩn hóa, xử lý đ/Đ).
/// </summary>
public static class SlugHelper
{
    public static string ToSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        // Bước 1: xử lý ký tự đặc biệt tiếng Việt không qua NFD được
        input = input.Replace("đ", "d").Replace("Đ", "d");

        // Bước 2: Unicode NFD — tách dấu thanh/dấu mũ ra khỏi ký tự gốc
        string normalized = input.Normalize(NormalizationForm.FormD);

        // Bước 3: loại bỏ tất cả NonSpacingMark (dấu thanh, dấu mũ, dấu hỏi...)
        var sb = new StringBuilder(normalized.Length);
        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        // Bước 4: chuyển thường, xóa ký tự lạ, chuẩn hóa dấu gạch ngang
        string result = sb.ToString()
                          .Normalize(NormalizationForm.FormC)
                          .ToLowerInvariant();

        result = Regex.Replace(result, @"[^a-z0-9\s-]", "");
        result = Regex.Replace(result, @"\s+", "-");
        result = Regex.Replace(result, @"-+",  "-");
        return result.Trim('-');
    }
}
