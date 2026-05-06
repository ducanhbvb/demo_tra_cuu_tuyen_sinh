using System.Web;

/// <summary>
/// AJAX endpoint — trả JSON {"chuaDoc": N} cho polling navbar bell badge.
/// Chỉ xử lý HocSinh đã đăng nhập. Cache 1 phút qua HopThuBLL.
/// Route: /Handlers/HopThuCount.ashx
/// </summary>
public class HopThuCount : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        // Chỉ xử lý GET, từ chối POST/PUT/...
        if (!string.Equals(context.Request.HttpMethod, "GET", System.StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 405;
            context.Response.Write("{\"error\":\"Method Not Allowed\"}");
            return;
        }

        // Chỉ trả count cho HocSinh đã đăng nhập
        if (!context.User.Identity.IsAuthenticated
            || SecurityHelper.GetCurrentRole() != "HocSinh")
        {
            context.Response.Write("{\"chuaDoc\":0}");
            return;
        }

        try
        {
            int maTaiKhoan = SecurityHelper.GetCurrentMaTaiKhoan();
            int count      = HopThuBLL.DemChuaDoc(maTaiKhoan);
            context.Response.Write($"{{\"chuaDoc\":{count}}}");
        }
        catch
        {
            context.Response.Write("{\"chuaDoc\":0}");
        }
    }

    public bool IsReusable => false;
}
