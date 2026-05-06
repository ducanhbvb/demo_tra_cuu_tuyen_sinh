using System;

/// <summary>
/// Helper render avatar tròn gradient dựa theo hash của tên người dùng.
/// Màu nhất quán cho mỗi người (cùng tên → cùng màu), đa dạng giữa các người khác nhau.
/// Dùng trong timeline thread tư vấn và danh sách Hộp thư.
/// </summary>
public static class AvatarHelper
{
    // Bảng màu gradient [from, to] — 8 màu
    private static readonly string[][] Palettes = {
        new[] { "#f59e0b", "#d97706" }, // cam-vàng
        new[] { "#10b981", "#059669" }, // xanh lá
        new[] { "#3b82f6", "#2563eb" }, // xanh dương
        new[] { "#8b5cf6", "#7c3aed" }, // tím
        new[] { "#ef4444", "#dc2626" }, // đỏ
        new[] { "#06b6d4", "#0891b2" }, // cyan
        new[] { "#f97316", "#ea580c" }, // cam
        new[] { "#ec4899", "#db2777" }, // hồng
    };

    // Màu cố định cho các loại đặc biệt
    private static readonly string[] SystemColors  = { "#94a3b8", "#64748b" }; // xám
    private static readonly string[] AdminColors   = { "#10b981", "#059669" }; // xanh lá (giống dot-admin)

    /// <summary>
    /// Render HTML span avatar tròn với kích thước tùy chọn.
    /// Ví dụ: &lt;span class="avatar-circle" style="..."&gt;NV&lt;/span&gt;
    /// </summary>
    /// <param name="hoTen">Họ tên đầy đủ — dùng để hash màu và lấy chữ cái đầu.</param>
    /// <param name="size">Kích thước px (default 36).</param>
    /// <param name="loaiNguoi">Loại người gửi: 'System', 'Admin', 'Moderator', 'TuVanVien', 'HocSinh', 'TruongHoc'.</param>
    public static string GetHtml(string hoTen, int size = 36, string loaiNguoi = null)
    {
        if (string.IsNullOrWhiteSpace(hoTen)) hoTen = "?";

        string[] pal;
        if (loaiNguoi == "System")
            pal = SystemColors;
        else if (loaiNguoi == "Admin" || loaiNguoi == "Moderator" || loaiNguoi == "TuVanVien")
            pal = AdminColors;
        else
        {
            int hash = Math.Abs(hoTen.GetHashCode());
            pal = Palettes[hash % Palettes.Length];
        }

        string initials  = GetInitials(hoTen, loaiNguoi);
        string fontSize  = (int)(size * 0.38) + "px";

        return $"<span class=\"avatar-circle\" style=\"" +
               $"width:{size}px;height:{size}px;line-height:{size}px;" +
               $"font-size:{fontSize};" +
               $"background:linear-gradient(135deg,{pal[0]},{pal[1]});\" " +
               $"title=\"{System.Web.HttpUtility.HtmlAttributeEncode(hoTen)}\">" +
               $"{initials}</span>";
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static string GetInitials(string hoTen, string loaiNguoi)
    {
        if (loaiNguoi == "System") return "<i class='bi bi-gear-fill' style='font-size:12px'></i>";

        var words = hoTen.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return "?";
        if (words.Length == 1) return words[0].Substring(0, 1).ToUpper();

        // Chữ cái đầu từ đầu + từ cuối
        string first = words[0].Length > 0 ? words[0].Substring(0, 1).ToUpper() : "";
        string last  = words[words.Length - 1].Length > 0 ? words[words.Length - 1].Substring(0, 1).ToUpper() : "";
        return first + last;
    }
}
