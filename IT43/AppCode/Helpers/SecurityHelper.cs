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
    public static void SignIn(int maTaiKhoan, string email, string role, bool rememberMe)
    {
        var ticket = new FormsAuthenticationTicket(
            version:    1,
            name:       maTaiKhoan.ToString(),
            issueDate:  DateTime.Now,
            expiration: DateTime.Now.AddHours(rememberMe ? 720 : 1),
            isPersistent: rememberMe,
            userData:   role
        );
        var encrypted = FormsAuthentication.Encrypt(ticket);
        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
        {
            HttpOnly = true,
            Expires  = rememberMe ? DateTime.Now.AddDays(30) : DateTime.MinValue
        };
        HttpContext.Current.Response.Cookies.Add(cookie);

        // Lưu email vào Session để MasterPage đọc mà không cần query DB mỗi request
        HttpContext.Current.Session["UserEmail"] = email;
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

    public static string GetCurrentRole()
    {
        var ticket = (HttpContext.Current?.User?.Identity as FormsIdentity)?.Ticket;
        return ticket?.UserData ?? "";
    }
}
