using System;
using System.Configuration;
using System.Web;

/// <summary>
/// Helper sinh HTML tag cho Bootstrap CSS/JS.
/// - Khi UseCdnAssets=true (production): dùng CDN jsdelivr + SRI integrity + auto-fallback về /lib/
/// - Khi UseCdnAssets=false (demo offline / dev): dùng /lib/ local trực tiếp
///
/// LUÔN dùng local (không qua CDN):
///   - Font Be Vietnam Pro  (~/lib/fonts/be-vietnam-pro.css)
///   - Bootstrap Icons      (~/lib/bootstrap/bootstrap-icons.css + fonts/bootstrap-icons.woff2)
///   - TinyMCE, Chart.js
///
/// Lý do Bootstrap Icons luôn local: CSS và font .woff2 phải cùng nguồn để
/// @font-face resolve đúng. Dùng CDN CSS nhưng font local sẽ gây mất icon.
///
/// Cách dùng trong Master page:
///   <%= AssetHelper.RenderBootstrapCss() %>
///   <%= AssetHelper.RenderBootstrapIconsCss() %>
///   ...
///   <%= AssetHelper.RenderBootstrapJs() %>
/// </summary>
public static class AssetHelper
{
    // ── Đọc config một lần, cache static ──────────────────────────────────
    private static readonly bool UseCdn =
        ParseBool("UseCdnAssets", defaultVal: false);   // default false = an toàn offline

    private static readonly bool FallbackEnabled =
        ParseBool("CdnFallbackEnabled", defaultVal: true);

    private static bool ParseBool(string key, bool defaultVal)
    {
        var raw = ConfigurationManager.AppSettings[key];
        return bool.TryParse(raw, out var result) ? result : defaultVal;
    }

    // ── Bootstrap 5.3.3 ───────────────────────────────────────────────────
    // SRI hashes từ: https://getbootstrap.com/docs/5.3/getting-started/introduction/
    private const string BS_VERSION     = "5.3.3";
    private const string BS_CSS_CDN     = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css";
    private const string BS_CSS_SRI     = "sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH";
    private const string BS_CSS_LOCAL   = "~/lib/bootstrap/bootstrap.min.css";

    private const string BS_JS_CDN      = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js";
    private const string BS_JS_SRI      = "sha384-YvpcrYf0tY3lHB60NNkmXc5s9fDVZLESaAA55NDzOxhy9GkcIdslK1eN7N6jIeHz";
    private const string BS_JS_LOCAL    = "~/lib/bootstrap/bootstrap.bundle.min.js";

    // ── Bootstrap Icons — LUÔN LOCAL ──────────────────────────────────────
    // Lý do: CSS và font .woff2 phải cùng origin.
    // CDN CSS trỏ font đến CDN, local CSS trỏ font đến /lib/bootstrap/fonts/.
    // Trộn 2 nguồn (CDN CSS + local font hoặc ngược lại) sẽ làm mất icon.
    private const string BSI_CSS_LOCAL  = "~/lib/bootstrap/bootstrap-icons.css";

    // ── Public methods ─────────────────────────────────────────────────────

    /// <summary>Bootstrap CSS tag (CDN hoặc local tuỳ config UseCdnAssets)</summary>
    public static string RenderBootstrapCss()
    {
        return UseCdn
            ? CdnCss(BS_CSS_CDN, BS_CSS_SRI, BS_CSS_LOCAL)
            : LocalCss(BS_CSS_LOCAL);
    }

    /// <summary>
    /// Bootstrap Icons CSS tag — LUÔN dùng local.
    /// Font .woff2 phải cùng nguồn với CSS nên không dùng CDN.
    /// </summary>
    public static string RenderBootstrapIconsCss()
    {
        return LocalCss(BSI_CSS_LOCAL);
    }

    /// <summary>Bootstrap JS Bundle tag + fallback script (CDN hoặc local tuỳ config)</summary>
    public static string RenderBootstrapJs()
    {
        return UseCdn
            ? CdnJs(BS_JS_CDN, BS_JS_SRI, BS_JS_LOCAL, globalCheck: "bootstrap")
            : LocalJs(BS_JS_LOCAL);
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private static string Abs(string virtualPath)
        => VirtualPathUtility.ToAbsolute(virtualPath);

    private static string LocalCss(string virtualPath)
        => $"<link rel=\"stylesheet\" href=\"{Abs(virtualPath)}\">";

    private static string LocalJs(string virtualPath)
        => $"<script src=\"{Abs(virtualPath)}\"></script>";

    private static string CdnCss(string cdnUrl, string sri, string localVirtualPath)
    {
        var fallbackUrl = Abs(localVirtualPath);
        // onerror: set this.href về local khi CDN fail hoặc SRI không khớp
        var fallbackAttr = FallbackEnabled
            ? $" onerror=\"this.onerror=null;this.href='{fallbackUrl}'\""
            : string.Empty;

        return $"<link rel=\"stylesheet\" href=\"{cdnUrl}\" " +
               $"integrity=\"{sri}\" crossorigin=\"anonymous\"{fallbackAttr}>";
    }

    private static string CdnJs(string cdnUrl, string sri, string localVirtualPath, string globalCheck)
    {
        var fallbackUrl = Abs(localVirtualPath);
        var primary = $"<script src=\"{cdnUrl}\" " +
                      $"integrity=\"{sri}\" crossorigin=\"anonymous\"></script>";

        if (!FallbackEnabled)
            return primary;

        // Kiểm tra biến global sau khi script CDN load xong.
        // Nếu undefined → CDN fail → inject script local.
        // Dùng window[...] thay vì typeof để tránh issue với minifier.
        var fallback = $"<script>" +
                       $"if(!window['{globalCheck}'])" +
                       $"document.write('<script src=\"{fallbackUrl}\"><\\/script>');" +
                       $"</script>";

        return primary + Environment.NewLine + fallback;
    }
}
