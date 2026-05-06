using System.Web.Optimization;

/// <summary>
/// Cấu hình Bundling &amp; Minification cho ASP.NET Web Optimization Framework.
/// Gộp nhiều file JS/CSS thành 1 bundle → giảm số HTTP request và tự minify ở Release mode.
///
/// Đăng ký tại: Global.asax.cs → Application_Start()
/// Sử dụng trong Master files:
///   Scripts.Render("~/bundles/site")
///   Scripts.Render("~/bundles/admin")
///   Styles.Render("~/bundles/admin-css")
///
/// Ghi chú về môi trường:
///   - Debug mode   : BundleTable.EnableOptimizations = false → render từng file riêng (dễ debug)
///   - Release mode : BundleTable.EnableOptimizations = true  → gộp + minify + hash version
/// </summary>
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        // ── Bundle JS dùng chung cho tất cả trang (Client + Admin + TruongHoc) ──────────
        // site.js     : helper UI chung (scroll, toast, confirm dialogs...)
        // perf.js     : đo TTFB, DOMReady, log performance metrics
        bundles.Add(new ScriptBundle("~/bundles/site").Include(
            "~/Scripts/site.js",
            "~/Scripts/perf.js"));

        // ── Bundle JS riêng cho trang Admin và TruongHoc ─────────────────────────────────
        // admin-modal-preview.js : preview bài viết / tin tuyển sinh trong modal
        // validation_data_input.js : AdminValidator — 3-layer validation cho form modal
        bundles.Add(new ScriptBundle("~/bundles/admin").Include(
            "~/Scripts/admin-modal-preview.js",
            "~/Scripts/validation_data_input.js"));

        // ── Bundle JS riêng cho trang HopThu (Client/HopThu.aspx) ───────────────────────
        // hopthu.js : logic đọc tin nhắn, real-time count badge
        bundles.Add(new ScriptBundle("~/bundles/hopthu").Include(
            "~/Scripts/hopthu.js"));

        // ── Bundle CSS riêng cho Admin ────────────────────────────────────────────────────
        // Admin.css           : layout dashboard, sidebar, stat-card, admin-card
        // AdminModalPreview.css: style cho modal preview bài viết / tin tuyển sinh
        bundles.Add(new StyleBundle("~/bundles/admin-css").Include(
            "~/Content/Admin.css",
            "~/Content/AdminModalPreview.css"));

        // ── Bật minify ở Release, tắt ở Debug ────────────────────────────────────────────
        // Khi debug: render từng file riêng với đường dẫn gốc → dễ đặt breakpoint JS
        // Khi release: gộp + minify + thêm hash vào URL → browser cache tự invalidate
        BundleTable.EnableOptimizations = false; // Sẽ tự override = true khi build Release
    }
}
