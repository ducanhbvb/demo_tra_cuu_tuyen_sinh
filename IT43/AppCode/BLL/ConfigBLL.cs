using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// BLL cho cài đặt hệ thống — cache PER-REQUEST (HttpContext.Items).
///
/// Lý do KHÔNG dùng MemoryCache:
///  - Admin và Client chạy trong 2 app domain khác nhau (Admin/ có Web.config riêng).
///  - MemoryCache là process-level → khi Admin xóa cache, Client KHÔNG thấy thay đổi.
///
/// Lý do dùng HttpContext.Items:
///  - Scope theo 1 request duy nhất → luôn đọc DB mới nhất giữa các request.
///  - Trong cùng 1 request, gọi GetInt() 5 lần chỉ hit DB 1 lần (query SELECT * từ tbl_Config).
///  - Không có vấn đề stale data xuyên domain.
///
/// Bảng tbl_Config chỉ có ~5 rows → 1 query/request là rất nhẹ.
/// </summary>
public static class ConfigBLL
{
    private const string REQUEST_KEY = "__ConfigBLL_All__";

    // ── Lấy toàn bộ config trong 1 request (cache HttpContext.Items) ─────
    private static Dictionary<string, string> GetAllForRequest()
    {
        var ctx = HttpContext.Current;
        if (ctx == null)
        {
            // Ngoài ngữ cảnh HTTP (VD: background task) → đọc thẳng DB
            return ConfigDAL.GetAll();
        }

        if (ctx.Items[REQUEST_KEY] is Dictionary<string, string> cached)
            return cached;

        var dict = ConfigDAL.GetAll();
        ctx.Items[REQUEST_KEY] = dict;
        return dict;
    }

    // ── Đọc theo kiểu ─────────────────────────────────────────────────────
    /// <summary>Lấy giá trị string, trả defaultValue nếu key không tồn tại.</summary>
    public static string GetString(string key, string defaultValue = "")
    {
        var dict = GetAllForRequest();
        return dict.TryGetValue(key, out var val) && !string.IsNullOrEmpty(val)
            ? val
            : defaultValue;
    }

    /// <summary>Lấy giá trị int, trả defaultValue nếu key không tồn tại hoặc không parse được.</summary>
    public static int GetInt(string key, int defaultValue = 0)
    {
        var dict = GetAllForRequest();
        if (!dict.TryGetValue(key, out var str) || string.IsNullOrEmpty(str))
            return defaultValue;
        return int.TryParse(str, out int v) ? v : defaultValue;
    }

    /// <summary>Lấy giá trị boolean ("true"/"1" = true, "false"/"0" = false).</summary>
    public static bool GetBool(string key, bool defaultValue = false)
    {
        var dict = GetAllForRequest();
        if (!dict.TryGetValue(key, out var str) || string.IsNullOrEmpty(str))
            return defaultValue;
        return str == "true" || str == "1";
    }

    // ── Ghi config ────────────────────────────────────────────────────────
    /// <summary>Lưu giá trị vào DB rồi xóa cache request hiện tại (nếu có).</summary>
    public static void SetValue(string key, string value)
    {
        ConfigDAL.SetValue(key, value);
        // Xóa cache request hiện tại để lần đọc kế tiếp trong cùng request (VD: LoadConfigSettings sau Save) thấy giá trị mới
        HttpContext.Current?.Items.Remove(REQUEST_KEY);
    }

    // ── Cache management (giữ lại để tương thích — no-op cross-request) ──
    /// <summary>No-op: cache tầng BLL giờ là per-request nên không cần clear thủ công giữa các request.</summary>
    public static void ClearCache() { /* no-op */ }
}
