using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.Routing;
using System.Web.Optimization;

/// <summary>
/// Global Application — Đăng ký URL Routes, FormsAuth role mapping,
/// và ghi log Application_Error vào tbl_Logs.
/// </summary>
public class Global : HttpApplication
{
    protected void Application_Start(object sender, EventArgs e)
    {
        RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    void RegisterRoutes(RouteCollection routes)
    {
        routes.RouteExistingFiles = true;

        // ══════════════════════════════════════════════
        // CLIENT ROUTES — ẩn /Client/ khỏi URL
        // ══════════════════════════════════════════════

        // Trang chủ
        routes.MapPageRoute("HomeRoute",            "index.aspx",               "~/Client/index.aspx");

        // Tìm kiếm & tra cứu
        routes.MapPageRoute("TimKiemTruongRoute",   "TimKiemTruong.aspx",       "~/Client/TimKiemTruong.aspx");
        routes.MapPageRoute("TraCuuDiemChuanRoute", "TraCuuDiemChuan.aspx",     "~/Client/TraCuuDiemChuan.aspx");
        routes.MapPageRoute("ChiTietTruongRoute",   "ChiTietTruong.aspx",       "~/Client/ChiTietTruong.aspx");
        routes.MapPageRoute("SoSanhTruongRoute",    "SoSanhTruong.aspx",        "~/Client/SoSanhTruong.aspx");
        routes.MapPageRoute("TimKiemTheoNganhRoute","TimKiemTheoNganh.aspx",    "~/Client/TimKiemTheoNganh.aspx");
        routes.MapPageRoute("GoiYTruongRoute",       "GoiYTruong.aspx",          "~/Client/GoiYTruong.aspx");

        // Chi tiết tin tuyển sinh
        routes.MapPageRoute("ChiTietTinTuyenSinhRoute", "ChiTietTinTuyenSinh.aspx", "~/Client/ChiTietTinTuyenSinh.aspx");

        // Bài viết
        routes.MapPageRoute("BaiVietRoute",         "BaiViet.aspx",             "~/Client/BaiViet.aspx");
        routes.MapPageRoute("ChiTietBaiVietRoute",  "ChiTietBaiViet.aspx",      "~/Client/ChiTietBaiViet.aspx");

        // Tài khoản
        routes.MapPageRoute("LoginRoute",           "login.aspx",               "~/Client/Login.aspx");
        routes.MapPageRoute("LoginRouteBlank",      "login",                    "~/Client/Login.aspx");
        routes.MapPageRoute("RegisterRoute",        "dang-ky.aspx",             "~/Client/DangKy.aspx");
        routes.MapPageRoute("RegisterRouteBlank",   "dang-ky",                  "~/Client/DangKy.aspx");
        routes.MapPageRoute("ForgotPassRoute",      "quen-mat-khau.aspx",       "~/Client/QuenMatKhau.aspx");
        routes.MapPageRoute("ResetPassRoute",       "dat-lai-mat-khau.aspx",    "~/Client/DatLaiMatKhau.aspx");
        routes.MapPageRoute("XacNhanEmailRoute",    "xac-nhan-email.aspx",      "~/Client/XacNhanEmail.aspx");
        routes.MapPageRoute("DoiMatKhauBatBuocRoute", "doi-mat-khau-bat-buoc.aspx", "~/Client/DoiMatKhauBatBuoc.aspx");
        routes.MapPageRoute("LogoutRoute",          "logout.aspx",              "~/Client/Logout.aspx");
        routes.MapPageRoute("LogoutRouteBlank",     "logout",                   "~/Client/Logout.aspx");

        // Profile
        routes.MapPageRoute("MyProfileRoute",       "my-profile.aspx",          "~/Client/MyProfile.aspx");
        routes.MapPageRoute("HopThuRoute",          "hop-thu.aspx",             "~/Client/HopThu.aspx");
        routes.MapPageRoute("WishListRoute",        "wish-list.aspx",           "~/Client/WishList.aspx");

        // Lỗi
        routes.MapPageRoute("ErrorRoute",           "Error.aspx",               "~/Client/Error.aspx");

        // ══════════════════════════════════════════════
        // ADMIN ROUTES — ẩn /Admin/ thành /admin/
        // ══════════════════════════════════════════════
        // ══════════════════════════════════════════════
        // TRUONGHOC ROUTES — ẩn /TruongHoc/ thành /truonghoc/
        // ══════════════════════════════════════════════
        routes.MapPageRoute("TruongHocDefaultRoute",        "truonghoc/index.aspx",                 "~/TruongHoc/Default.aspx");
        routes.MapPageRoute("TruongHocDefaultBlankRoute",   "truonghoc",                            "~/TruongHoc/Default.aspx");
        routes.MapPageRoute("TruongHocBaiVietRoute",        "truonghoc/quan-ly-bai-viet.aspx",      "~/TruongHoc/QuanLyBaiViet.aspx");
        routes.MapPageRoute("TruongHocTinTuyenSinhRoute",   "truonghoc/quan-ly-tin-tuyen-sinh.aspx","~/TruongHoc/QuanLyTinTuyenSinh.aspx");
        routes.MapPageRoute("TruongHocGopYTuVanRoute",      "truonghoc/gop-y-tu-van.aspx",          "~/TruongHoc/GopYTuVan.aspx");

        routes.MapPageRoute("AdminDefaultRoute",        "admin/index.aspx",                 "~/Admin/Default.aspx");
        routes.MapPageRoute("AdminTruongRoute",         "admin/quan-ly-truong.aspx",         "~/Admin/QuanLyTruong.aspx");
        routes.MapPageRoute("AdminChinhSuaRoute",       "admin/chinh-sua-truong.aspx",       "~/Admin/ChinhSuaTruong.aspx");
        routes.MapPageRoute("AdminTinTuyenSinhRoute",   "admin/quan-ly-tin-tuyen-sinh.aspx", "~/Admin/QuanLyTinTuyenSinh.aspx");
        routes.MapPageRoute("AdminGopYRoute",           "admin/quan-ly-gop-y.aspx",          "~/Admin/QuanLyGopYTuVan.aspx");
        routes.MapPageRoute("AdminBaiVietRoute",        "admin/quan-ly-bai-viet.aspx",       "~/Admin/QuanLyBaiViet.aspx");
        routes.MapPageRoute("AdminTaiKhoanRoute",       "admin/quan-ly-tai-khoan.aspx",      "~/Admin/QuanLyTaiKhoan.aspx");
        routes.MapPageRoute("AdminLogsRoute",           "admin/quan-ly-logs.aspx",           "~/Admin/QuanLyLogs.aspx");
        routes.MapPageRoute("AdminCaiDatRoute",         "admin/cai-dat.aspx",                "~/Admin/CaiDat.aspx");
    }

    // Gắn role vào FormsAuthentication ticket
    // UserData format: "RoleName|MaTruong" (VD: "Admin|0", "TruongHoc|5")
    // Chỉ lấy phần role (trước dấu '|') để gán vào GenericPrincipal
    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        var ctx = HttpContext.Current;
        if (ctx?.User?.Identity?.IsAuthenticated != true) return;

        if (ctx.User.Identity is FormsIdentity fi)
        {
            // Lấy role name từ UserData (phần trước '|')
            string userData = fi.Ticket.UserData ?? "";
            string roleName = userData.Contains("|")
                ? userData.Split('|')[0]
                : userData;

            string[] roles = string.IsNullOrEmpty(roleName)
                ? new string[0]
                : new[] { roleName };

            ctx.User = new GenericPrincipal(fi, roles);
        }
    }

    /// <summary>
    /// Hỗ trợ VaryByCustom="auth" cho OutputCache:
    /// - Người dùng chưa đăng nhập → key "guest"
    /// - Đã đăng nhập → key "user_[MaTaiKhoan]"
    /// Đảm bảo trang chủ / chi tiết trường cache riêng biệt theo trạng thái login.
    /// </summary>
    public override string GetVaryByCustomString(HttpContext context, string custom)
    {
        if (custom == "auth")
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                // Cache riêng cho từng user (vì wishlist/nút đánh giá khác nhau)
                try
                {
                    int id = SecurityHelper.GetCurrentMaTaiKhoan();
                    return "user_" + id;
                }
                catch { return "user"; }
            }
            return "guest";
        }
        return base.GetVaryByCustomString(context, custom);
    }

    protected void Application_Error(object sender, EventArgs e)
    {
        // Fix 3.4: Ghi log exception vào tbl_Logs thay vì bỏ trống
        var ex = Server.GetLastError();
        if (ex != null)
        {
            // Lấy inner exception nếu có
            var inner = ex.InnerException ?? ex;
            string moTa = $"{inner.GetType().Name}: {inner.Message}\n{inner.StackTrace}";
            string maLoi = inner.GetType().Name;

            // Lấy MaTaiKhoan nếu đã đăng nhập
            int? maTK = null;
            try
            {
                int id = SecurityHelper.GetCurrentMaTaiKhoan();
                if (id > 0) maTK = id;
            }
            catch { /* bỏ qua nếu không đọc được identity */ }

            LogHelper.Ghi(maTK, "APPLICATION_ERROR", moTa, isSuccess: false, maLoi: maLoi);
        }
    }
}
