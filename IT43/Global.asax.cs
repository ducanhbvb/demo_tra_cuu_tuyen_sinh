using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.Routing;

/// <summary>
/// Global Application — Đăng ký URL Routes, FormsAuth role mapping,
/// và ghi log Application_Error vào tbl_Logs.
/// </summary>
public class Global : HttpApplication
{
    protected void Application_Start(object sender, EventArgs e) 
    { 
        RegisterRoutes(RouteTable.Routes);
    }

    void RegisterRoutes(RouteCollection routes)
    {
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
        routes.MapPageRoute("LogoutRoute",          "logout.aspx",              "~/Client/Logout.aspx");
        routes.MapPageRoute("LogoutRouteBlank",     "logout",                   "~/Client/Logout.aspx");

        // Profile
        routes.MapPageRoute("MyProfileRoute",       "my-profile.aspx",          "~/Client/MyProfile.aspx");
        routes.MapPageRoute("WishListRoute",        "wish-list.aspx",           "~/Client/WishList.aspx");

        // Lỗi
        routes.MapPageRoute("ErrorRoute",           "Error.aspx",               "~/Client/Error.aspx");

        // ══════════════════════════════════════════════
        // ADMIN ROUTES — ẩn /Admin/ thành /admin/
        // ══════════════════════════════════════════════
        routes.MapPageRoute("AdminDefaultRoute",        "admin/index.aspx",                 "~/Admin/Default.aspx");
        routes.MapPageRoute("AdminTruongRoute",         "admin/quan-ly-truong.aspx",         "~/Admin/QuanLyTruong.aspx");
        routes.MapPageRoute("AdminChinhSuaRoute",       "admin/chinh-sua-truong.aspx",       "~/Admin/ChinhSuaTruong.aspx");
        routes.MapPageRoute("AdminTinTuyenSinhRoute",   "admin/quan-ly-tin-tuyen-sinh.aspx", "~/Admin/QuanLyTinTuyenSinh.aspx");
        routes.MapPageRoute("AdminGopYRoute",           "admin/quan-ly-gop-y.aspx",          "~/Admin/QuanLyGopYTuVan.aspx");
        routes.MapPageRoute("AdminBaiVietRoute",        "admin/quan-ly-bai-viet.aspx",       "~/Admin/QuanLyBaiViet.aspx");
        routes.MapPageRoute("AdminTaiKhoanRoute",       "admin/quan-ly-tai-khoan.aspx",      "~/Admin/QuanLyTaiKhoan.aspx");
        routes.MapPageRoute("AdminLogsRoute",           "admin/quan-ly-logs.aspx",           "~/Admin/QuanLyLogs.aspx");
    }

    // Gắn role vào FormsAuthentication ticket (UserData = "Admin" / "TruongHoc" / "HocSinh")
    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        var ctx = HttpContext.Current;
        if (ctx?.User?.Identity?.IsAuthenticated != true) return;

        if (ctx.User.Identity is FormsIdentity fi)
        {
            var roles = (fi.Ticket.UserData ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            ctx.User = new GenericPrincipal(fi, roles);
        }
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
