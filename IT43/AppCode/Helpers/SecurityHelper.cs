using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

/// <summary>
/// Helper bảo mật — Hash mật khẩu (PBKDF2 + backward-compat SHA256),
/// verify, tạo token, FormsAuthentication (sign in/out, lấy role).
/// </summary>
public static class SecurityHelper
{
    // ── PBKDF2 (Rfc2898DeriveBytes) — thay SHA256 ─────────────────────────
    // Format lưu DB: "pbkdf2$<iterations>$<salt_base64>$<hash_base64>"
    // Backward-compatible: nếu hash cũ (Base64 ngắn, không có prefix) → vẫn verify được
    private const int PBKDF2_ITERATIONS = 100_000;
    private const int SALT_SIZE         = 16; // bytes
    private const int HASH_SIZE         = 32; // bytes (SHA256)

    // Salt tĩnh chỉ dùng cho backward-compat verify hash SHA256 cũ
    private const string LEGACY_SALT = "TCTS@2026#Salt!";

    /// <summary>Tạo hash PBKDF2 mới cho mật khẩu.</summary>
    public static string HashPassword(string plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return "";

        byte[] salt = new byte[SALT_SIZE];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        using var kdf = new Rfc2898DeriveBytes(plainText, salt, PBKDF2_ITERATIONS, HashAlgorithmName.SHA256);
        byte[] hash = kdf.GetBytes(HASH_SIZE);

        return $"pbkdf2${PBKDF2_ITERATIONS}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verify mật khẩu — hỗ trợ cả hash PBKDF2 mới và SHA256 cũ (backward-compat).
    /// Nếu hash cũ và đúng → tự động re-hash lên PBKDF2 (upgrade trong DB qua TaiKhoanDAL).
    /// </summary>
    public static bool VerifyPassword(string plain, string storedHash)
    {
        if (string.IsNullOrEmpty(plain) || string.IsNullOrEmpty(storedHash))
            return false;

        // Hash PBKDF2 mới
        if (storedHash.StartsWith("pbkdf2$"))
            return VerifyPbkdf2(plain, storedHash);

        // Hash SHA256 cũ — backward-compat
        return VerifyLegacySha256(plain, storedHash);
    }

    /// <summary>Kiểm tra hash có phải format cũ (SHA256) không — dùng để tự động upgrade.</summary>
    public static bool IsLegacyHash(string storedHash)
        => !string.IsNullOrEmpty(storedHash) && !storedHash.StartsWith("pbkdf2$");

    private static bool VerifyPbkdf2(string plain, string storedHash)
    {
        try
        {
            var parts = storedHash.Split('$');
            // Format: pbkdf2$iterations$salt$hash
            if (parts.Length != 4) return false;

            int iterations = int.Parse(parts[1]);
            byte[] salt    = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);

            using var kdf = new Rfc2898DeriveBytes(plain, salt, iterations, HashAlgorithmName.SHA256);
            byte[] actual = kdf.GetBytes(expected.Length);

            return CryptographicEquals(actual, expected);
        }
        catch { return false; }
    }

    private static bool VerifyLegacySha256(string plain, string storedHash)
    {
        try
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(plain + LEGACY_SALT);
            string computed = Convert.ToBase64String(sha.ComputeHash(bytes));
            return computed == storedHash;
        }
        catch { return false; }
    }

    /// <summary>So sánh constant-time để tránh timing attack.</summary>
    private static bool CryptographicEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) return false;
        int diff = 0;
        for (int i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];
        return diff == 0;
    }

    // ── Tạo token ngẫu nhiên (dùng cho email verify / reset password) ──────
    public static string GenerateToken(int length = 64)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
                      .Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    // ── FormsAuthentication ticket (gắn role vào UserData) ─────────────────
    /// <summary>
    /// Đăng nhập — tạo FormsAuth ticket với userData = "Role|MaTruong"
    /// VD: "Admin|0", "TruongHoc|5", "Moderator|0"
    /// </summary>
    public static void SignIn(int maTaiKhoan, string email, string role, bool rememberMe, int maTruong = 0)
    {
        // userData format: "Role|MaTruong" — MaTruong=0 nếu không phải TruongHoc
        string userData = $"{role}|{maTruong}";

        var ticket = new FormsAuthenticationTicket(
            version:    1,
            name:       maTaiKhoan.ToString(),
            issueDate:  DateTime.Now,
            expiration: DateTime.Now.AddHours(rememberMe ? 720 : 8),
            isPersistent: rememberMe,
            userData:   userData
        );
        var encrypted = FormsAuthentication.Encrypt(ticket);
        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
        {
            HttpOnly = true,
            Expires  = rememberMe ? DateTime.Now.AddDays(30) : DateTime.MinValue
        };
        HttpContext.Current.Response.Cookies.Add(cookie);

        // Lưu email + MaTruong vào Session để MasterPage đọc mà không cần query DB mỗi request
        HttpContext.Current.Session["UserEmail"]  = email;
        HttpContext.Current.Session["MaTruong"]   = maTruong;
    }

    public static void SignOut() => FormsAuthentication.SignOut();

    // ── Lấy MaTaiKhoan từ ticket hiện tại ────────────────────────────────
    public static int GetCurrentMaTaiKhoan()
    {
        var identity = HttpContext.Current?.User?.Identity;
        if (identity?.IsAuthenticated == true && int.TryParse(identity.Name, out int id))
            return id;
        return 0;
    }

    /// <summary>Lấy role name từ ticket (phần trước dấu '|').</summary>
    public static string GetCurrentRole()
    {
        var ticket = (HttpContext.Current?.User?.Identity as FormsIdentity)?.Ticket;
        if (ticket == null) return "";
        var parts = ticket.UserData?.Split('|');
        return parts?.Length > 0 ? parts[0] : "";
    }

    /// <summary>Lấy MaTruong từ ticket (phần sau dấu '|'). 0 nếu không phải TruongHoc.</summary>
    public static int GetCurrentMaTruong()
    {
        var ticket = (HttpContext.Current?.User?.Identity as FormsIdentity)?.Ticket;
        if (ticket == null) return 0;
        var parts = ticket.UserData?.Split('|');
        if (parts?.Length > 1 && int.TryParse(parts[1], out int maTruong))
            return maTruong;

        // Fallback: đọc từ Session
        if (HttpContext.Current?.Session["MaTruong"] is int sessionMaTruong)
            return sessionMaTruong;
        return 0;
    }

    // ── Kiểm tra quyền hiện tại ──────────────────────────────────────────
    /// <summary>Lấy MaQuyen từ role name hiện tại.</summary>
    public static int GetCurrentMaQuyen()
    {
        return GetCurrentRole() switch
        {
            "Admin"      => 1,
            "TruongHoc"  => 2,
            "HocSinh"    => 3,
            "Moderator"  => 4,
            "TuVanVien"  => 5,
            _            => 0
        };
    }

    public static bool IsAdmin()       => GetCurrentRole() == "Admin";
    public static bool IsTruongHoc()   => GetCurrentRole() == "TruongHoc";
    public static bool IsHocSinh()     => GetCurrentRole() == "HocSinh";
    public static bool IsModerator()   => GetCurrentRole() == "Moderator";
    public static bool IsTuVanVien()   => GetCurrentRole() == "TuVanVien";

    /// <summary>Có thể truy cập /Admin/ không? (Admin, Moderator, TuVanVien)</summary>
    public static bool CanAccessAdmin()
    {
        var role = GetCurrentRole();
        return role == "Admin" || role == "Moderator" || role == "TuVanVien";
    }

    /// <summary>
    /// Kiểm tra URL redirect nội bộ an toàn.
    /// Chỉ cho phép đường dẫn local dạng "/...", chặn URL protocol-relative "//..." và "/\\...".
    /// </summary>
    public static bool IsSafeLocalUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        if (!url.StartsWith("/")) return false;
        if (url.StartsWith("//") || url.StartsWith("/\\")) return false;
        if (url.Contains("\r") || url.Contains("\n")) return false;
        return true;
    }

    /// <summary>Lấy ReturnUrl an toàn hoặc null nếu không hợp lệ.</summary>
    public static string GetSafeReturnUrl(string returnUrl)
        => IsSafeLocalUrl(returnUrl) ? returnUrl : null;

    /// <summary>Có thể CRUD nội dung (bài viết, tin TS)? (Admin, Moderator)</summary>
    public static bool CanManageContent()
    {
        var role = GetCurrentRole();
        return role == "Admin" || role == "Moderator";
    }

    /// <summary>Có thể xem nội dung (bài viết, tin TS, thông tin trường)? (Admin, Moderator, TuVanVien)</summary>
    public static bool CanViewContent()
    {
        var role = GetCurrentRole();
        return role == "Admin" || role == "Moderator" || role == "TuVanVien";
    }

    /// <summary>Có thể phản hồi góp ý/tư vấn? (Admin, Moderator, TuVanVien)</summary>
    public static bool CanReplyTuVan()
    {
        var role = GetCurrentRole();
        return role == "Admin" || role == "Moderator" || role == "TuVanVien";
    }

    /// <summary>Có toàn quyền quản trị? Chỉ Admin.</summary>
    public static bool CanFullAdmin() => GetCurrentRole() == "Admin";

    /// <summary>
    /// Ánh xạ MaQuyen số → tên role string (dùng cho FormsAuth và redirect).
    /// </summary>
    public static string MaQuyenToRole(int maQuyen) => maQuyen switch
    {
        1 => "Admin",
        2 => "TruongHoc",
        3 => "HocSinh",
        4 => "Moderator",
        5 => "TuVanVien",
        _ => "HocSinh"
    };

    /// <summary>
    /// Ánh xạ role string → MaQuyen số — nghịch đảo MaQuyenToRole.
    /// Dùng khi cần redirect ngay sau SignIn (cookie chưa khả dụng trong cùng request).
    /// </summary>
    public static int RoleToMaQuyen(string role) => role switch
    {
        "Admin"     => 1,
        "TruongHoc" => 2,
        "HocSinh"   => 3,
        "Moderator" => 4,
        "TuVanVien" => 5,
        _           => 3
    };

    /// <summary>
    /// Trả về URL redirect sau khi đăng nhập theo role.
    /// </summary>
    public static string GetRedirectUrlByRole(int maQuyen) => maQuyen switch
    {
        1 => "/Admin/Default.aspx",
        2 => "/TruongHoc/Default.aspx",
        3 => "/Client/index.aspx",
        4 => "/Admin/Default.aspx",
        5 => "/Admin/Default.aspx",
        _ => "/Client/index.aspx"
    };
}
