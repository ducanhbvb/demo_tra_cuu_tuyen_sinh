using System;
using System.Runtime.Caching;

/// <summary>
/// Helper cache dùng MemoryCache — tránh query DB lặp lại cho dữ liệu ít thay đổi.
/// Thread-safe, tự expire theo thời gian hoặc xóa thủ công khi data thay đổi.
/// Dùng cho: danh mục ngành, danh sách trường dropdown, thống kê trang chủ.
/// </summary>
public static class CacheHelper
{
    private static readonly MemoryCache _cache = MemoryCache.Default;

    // ── Prefix keys cho từng nhóm ────────────────────────────────────────
    public const string PREFIX_DANHMUC  = "DanhMuc_";
    public const string PREFIX_TRUONG   = "Truong_";
    public const string PREFIX_THONGKE  = "ThongKe_";

    // ── Keys cụ thể ──────────────────────────────────────────────────────
    public const string KEY_CHUYEN_NGANH_ALL    = "DanhMuc_ChuyenNganh_All";
    public const string KEY_DANH_MUC_NGANH      = "DanhMuc_DanhMucNganh";
    public const string KEY_PHUONG_THUC         = "DanhMuc_PhuongThuc";
    public const string KEY_NAM_TUYEN_SINH      = "DanhMuc_NamTuyenSinh";
    public const string KEY_TRUONG_DROPDOWN     = "Truong_Dropdown";
    public const string KEY_SO_TRUONG           = "ThongKe_SoTruong";
    public const string KEY_SO_NGANH            = "ThongKe_SoNganh";
    public const string KEY_SO_TIN_ACTIVE       = "ThongKe_SoTinActive";
    public const string KEY_NAM_MOI_NHAT        = "ThongKe_NamMoiNhat";
    public const string KEY_TRUONG_NOI_BAT      = "ThongKe_TruongNoiBat";

    // ── Thời gian cache mặc định ─────────────────────────────────────────
    private const int MINUTES_DANHMUC  = 30;  // Danh mục: 30 phút
    private const int MINUTES_TRUONG   = 15;  // Danh sách trường: 15 phút
    private const int MINUTES_THONGKE  = 10;  // Thống kê: 10 phút

    /// <summary>
    /// Lấy giá trị từ cache theo key. Trả về default(T) nếu không có.
    /// </summary>
    public static T Get<T>(string key)
    {
        var item = _cache.Get(key);
        return item is T t ? t : default(T);
    }

    /// <summary>
    /// Lấy từ cache nếu có; nếu không có thì gọi factory, lưu vào cache rồi trả về.
    /// Pattern: cache-aside (lazy loading).
    /// </summary>
    public static T GetOrSet<T>(string key, Func<T> factory, int minutes = 10)
    {
        var item = _cache.Get(key);
        if (item is T cached) return cached;

        var value = factory();
        if (value != null)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(minutes)
            };
            _cache.Set(key, value, policy);
        }
        return value;
    }

    /// <summary>
    /// Lưu giá trị vào cache với thời gian hết hạn tuyệt đối.
    /// </summary>
    public static void Set<T>(string key, T value, int minutes = 10)
    {
        if (value == null) return;
        var policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(minutes)
        };
        _cache.Set(key, value, policy);
    }

    /// <summary>
    /// Xóa 1 key khỏi cache (dùng khi CUD 1 record cụ thể).
    /// </summary>
    public static void Remove(string key)
    {
        _cache.Remove(key);
    }

    /// <summary>
    /// Xóa tất cả các key bắt đầu bằng prefix (dùng khi CUD ảnh hưởng nhóm).
    /// Ví dụ: RemoveByPrefix("DanhMuc_") sẽ xóa mọi cache liên quan đến DanhMuc.
    /// </summary>
    public static void RemoveByPrefix(string prefix)
    {
        // MemoryCache không hỗ trợ wildcard delete trực tiếp
        // → enumerate toàn bộ keys, xóa những key có prefix phù hợp
        var keysToRemove = new System.Collections.Generic.List<string>();
        foreach (var item in _cache)
        {
            if (item.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                keysToRemove.Add(item.Key);
        }
        foreach (var key in keysToRemove)
            _cache.Remove(key);
    }

    /// <summary>
    /// Xóa toàn bộ cache (dùng cho debug hoặc khi cần reset hoàn toàn).
    /// </summary>
    public static void Clear()
    {
        var keysToRemove = new System.Collections.Generic.List<string>();
        foreach (var item in _cache)
            keysToRemove.Add(item.Key);
        foreach (var key in keysToRemove)
            _cache.Remove(key);
    }

    /// <summary>Trả về số item đang có trong cache (dùng cho monitoring).</summary>
    public static long Count => _cache.GetCount();
}
