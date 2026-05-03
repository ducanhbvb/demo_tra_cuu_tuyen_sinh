using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Helper upload ảnh local vào /Resources/Images/{subFolder}/
/// Format tên file: {prefix}_{8_hex_random}.{ext}
///   - Avatar user  → email_avatar_a3f8b21c.jpg
///   - Logo trường  → slug-truong_logo_5e2d91ab.jpg
///   - Cover trường → slug-truong_cover_7c4f12de.jpg
///   - Ảnh bài viết → slug-baiviet_thumb_9a1b3c5d.jpg
/// Backward-compatible: method Upload cũ vẫn giữ nguyên.
/// </summary>
public static class ImageUploadHelper
{
    private const string BASE_PATH = "~/Resources/Images/";

    private static readonly string[] ALLOWED_EXT =
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public const int MAX_AVATAR_SIZE = 5  * 1024 * 1024;  // 5 MB
    public const int MAX_COVER_SIZE  = 10 * 1024 * 1024;  // 10 MB

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Upload file ảnh với prefix tuỳ chỉnh (KHUYẾN NGHỊ dùng method này).
    /// Tên file: {prefix}_{8hex}.{ext}
    /// </summary>
    /// <param name="file">HttpPostedFile từ FileUpload.PostedFile</param>
    /// <param name="subFolder">"truong" | "avatar" | "baiviet"</param>
    /// <param name="prefix">Prefix có ý nghĩa, VD: "user@mail.com_avatar", "slug-truong_logo"</param>
    /// <param name="maxSize">Byte tối đa (mặc định MAX_AVATAR_SIZE)</param>
    /// <returns>(true, đường-dẫn-tương-đối) hoặc (false, thông-báo-lỗi)</returns>
    public static (bool ok, string result) Upload(
        HttpPostedFile file,
        string subFolder,
        string prefix,
        int maxSize)
    {
        if (file == null || file.ContentLength == 0)
            return (false, "Vui lòng chọn file ảnh.");

        if (file.ContentLength > maxSize)
            return (false, $"File quá lớn. Tối đa {maxSize / (1024 * 1024)} MB.");

        string ext = Path.GetExtension(file.FileName ?? "").ToLower();
        if (Array.IndexOf(ALLOWED_EXT, ext) < 0)
            return (false, "Chỉ chấp nhận ảnh: .jpg, .jpeg, .png, .gif, .webp");

        // Tên file: {sanitizedPrefix}_{8hex}.ext
        string safeName = SanitizePrefix(prefix);
        string suffix   = GenerateHex(8);
        string newName  = $"{safeName}_{suffix}{ext}";

        // Tạo thư mục nếu chưa có
        string folder = HttpContext.Current.Server.MapPath($"{BASE_PATH}{subFolder}/");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        file.SaveAs(Path.Combine(folder, newName));

        return (true, $"/Resources/Images/{subFolder}/{newName}");
    }

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Upload file ảnh (backward-compatible, dùng tên file gốc làm prefix).
    /// </summary>
    public static (bool ok, string result) Upload(
        HttpPostedFile file,
        string subFolder,
        int maxSize = MAX_AVATAR_SIZE)
    {
        // Fallback: dùng tên file gốc làm prefix
        string baseName = Path.GetFileNameWithoutExtension(file?.FileName ?? "unknown");
        return Upload(file, subFolder, baseName, maxSize);
    }

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Xóa file ảnh cũ (bỏ qua nếu là URL ngoài hoặc file không tồn tại).
    /// </summary>
    public static void DeleteOld(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        if (IsExternalUrl(relativePath)) return;

        try
        {
            string full = HttpContext.Current.Server.MapPath("~" + relativePath);
            if (File.Exists(full)) File.Delete(full);
        }
        catch { /* bỏ qua lỗi xóa file */ }
    }

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Kiểm tra đường dẫn có phải URL ngoài không (backward-compat).
    /// </summary>
    public static bool IsExternalUrl(string path)
        => !string.IsNullOrEmpty(path)
           && (path.StartsWith("http://") || path.StartsWith("https://"));

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Trả về URL hiển thị ảnh: giữ nguyên URL ngoài, trả path local nguyên vẹn.
    /// Nếu rỗng → trả fallback.
    /// </summary>
    public static string Resolve(string path, string fallback = "/Resources/Images/no-image.png")
    {
        if (string.IsNullOrEmpty(path)) return fallback;
        return path;  // cả URL ngoài lẫn path local đều dùng được trong <img src>
    }

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Làm sạch prefix: chỉ giữ chữ, số, dấu -, _, @, .
    /// Chuyển thường, khoảng trắng → gạch ngang, bỏ ký tự đặc biệt.
    /// </summary>
    private static string SanitizePrefix(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix)) return "file";
        string s = prefix.Trim().ToLower();
        s = s.Replace(" ", "-");
        // Chỉ giữ a-z, 0-9, -, _, @, .
        s = Regex.Replace(s, @"[^a-z0-9\-_@\.]", "");
        // Loại bỏ nhiều dấu gạch liên tiếp
        s = Regex.Replace(s, @"-{2,}", "-");
        s = s.Trim('-', '.');
        return string.IsNullOrEmpty(s) ? "file" : s;
    }

    // ─────────────────────────────────────────────────────────────────
    private static string GenerateHex(int length)
    {
        byte[] buf = new byte[(length / 2) + 1];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(buf);
        return BitConverter.ToString(buf).Replace("-", "").ToLower().Substring(0, length);
    }
}
