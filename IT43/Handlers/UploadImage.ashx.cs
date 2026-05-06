using System;
using System.Web;
using System.Web.SessionState;

/// <summary>
/// API Handler — Upload ảnh nội dung bài viết cho Quill editor.
/// POST /Handlers/UploadImage.ashx  (multipart/form-data, field "image")
/// Trả JSON: { "url": "/Resources/Images/content/xxx.jpg" }
/// Chỉ cho phép user có quyền quản trị nội dung (Admin, Moderator).
/// </summary>
public class Handlers_UploadImage : IHttpHandler, IRequiresSessionState
{
    private const int MAX_CONTENT_IMAGE = 5 * 1024 * 1024; // 5 MB

    public bool IsReusable => false;

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";

        // ── Auth check: chỉ user có quyền quản trị nội dung ──
        int maTK = SecurityHelper.GetCurrentMaTaiKhoan();
        if (maTK <= 0)
        {
            context.Response.StatusCode = 401;
            context.Response.Write("{\"error\":\"Chưa đăng nhập.\"}");
            return;
        }

        if (!SecurityHelper.CanManageContent())
        {
            context.Response.StatusCode = 403;
            context.Response.Write("{\"error\":\"Bạn không có quyền upload ảnh nội dung.\"}");
            return;
        }

        // ── Chỉ chấp nhận POST ──
        if (context.Request.HttpMethod != "POST")
        {
            context.Response.StatusCode = 405;
            context.Response.Write("{\"error\":\"Method not allowed.\"}");
            return;
        }

        // ── Lấy file từ form ──
        HttpPostedFile file = context.Request.Files["image"];
        if (file == null || file.ContentLength == 0)
        {
            context.Response.StatusCode = 400;
            context.Response.Write("{\"error\":\"Không tìm thấy file ảnh.\"}");
            return;
        }

        // ── Upload qua ImageUploadHelper ──
        string prefix = "content_" + DateTime.Now.ToString("yyyyMMdd");
        var (ok, result) = ImageUploadHelper.Upload(file, "content", prefix, MAX_CONTENT_IMAGE);

        if (ok)
        {
            // Escape JSON đơn giản
            string url = result.Replace("\\", "/").Replace("\"", "\\\"");
            context.Response.Write("{\"url\":\"" + url + "\"}");
        }
        else
        {
            context.Response.StatusCode = 400;
            string msg = result.Replace("\"", "\\\"");
            context.Response.Write("{\"error\":\"" + msg + "\"}");
        }
    }
}
