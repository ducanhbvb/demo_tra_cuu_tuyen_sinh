using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

/// <summary>
/// Helper gửi email qua SMTP (xác nhận email, đặt lại mật khẩu).
/// Cấu hình SMTP đọc từ Web.config (SmtpHost, SmtpPort, SmtpUser, SmtpPass, SmtpFrom).
/// </summary>
public static class EmailHelper
{
    private static string Host => ConfigurationManager.AppSettings["SmtpHost"];
    private static int    Port => int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
    private static string User => ConfigurationManager.AppSettings["SmtpUser"];
    private static string Pass => ConfigurationManager.AppSettings["SmtpPass"];
    private static string From => ConfigurationManager.AppSettings["SmtpFrom"];
    private static string SiteUrl => ConfigurationManager.AppSettings["SiteUrl"];

    public static void Send(string to, string subject, string body)
    {
        using var client = new SmtpClient(Host, Port)
        {
            EnableSsl   = true,
            Credentials = new NetworkCredential(User, Pass)
        };
        var msg = new MailMessage(From, to, subject, body) { IsBodyHtml = true };
        client.Send(msg);
    }

    public static void GuiXacNhanEmail(string to, string token)
    {
        var link = $"{SiteUrl}/xac-nhan-email.aspx?token={Uri.EscapeDataString(token)}";
        var body = $@"
<h2>Xác nhận email đăng ký</h2>
<p>Vui lòng nhấn vào đường dẫn bên dưới để kích hoạt tài khoản:</p>
<p><a href='{link}'>{link}</a></p>
<p>Đường dẫn có hiệu lực trong 24 giờ.</p>";
        Send(to, "[Tra Cứu Tuyển Sinh] Xác nhận email", body);
    }

    public static void GuiDatLaiMatKhau(string to, string token)
    {
        var link = $"{SiteUrl}/dat-lai-mat-khau.aspx?token={Uri.EscapeDataString(token)}";
        var body = $@"
<h2>Đặt lại mật khẩu</h2>
<p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản <b>{to}</b>.</p>
<p><a href='{link}'>Nhấn vào đây để đặt lại mật khẩu</a></p>
<p>Đường dẫn có hiệu lực trong 2 giờ. Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>";
        Send(to, "[Tra Cứu Tuyển Sinh] Đặt lại mật khẩu", body);
    }

    /// <summary>
    /// Gửi email thông báo đến học sinh khi admin/tư vấn viên phản hồi tư vấn của họ.
    /// Template HTML có nút CTA dẫn về Hộp thư.
    /// </summary>
    /// <param name="toEmail">Email người hỏi.</param>
    /// <param name="toName">Họ tên người hỏi để cá nhân hoá.</param>
    /// <param name="cauHoiGoc">Nội dung câu hỏi gốc (text thuần, không HTML).</param>
    /// <param name="noiDungPhanHoi">Nội dung phản hồi từ admin (text thuần).</param>
    /// <param name="tenTruong">Tên trường liên quan đến tư vấn.</param>
    public static void GuiPhanHoiTuVan(string toEmail, string toName,
                                        string cauHoiGoc, string noiDungPhanHoi,
                                        string tenTruong)
    {
        if (string.IsNullOrWhiteSpace(toEmail)) return;

        string subject = $"[Tra cứu tuyển sinh] Phản hồi tư vấn về {tenTruong}";
        string body    = $@"
<div style='font-family:""Be Vietnam Pro"",Arial,sans-serif;max-width:600px;margin:0 auto;color:#1e293b'>
  <div style='background:linear-gradient(135deg,#f59e0b,#d97706);padding:24px 32px;border-radius:12px 12px 0 0'>
    <h2 style='margin:0;color:#fff;font-size:20px'>
      <span style='margin-right:8px'>📬</span>Bạn có phản hồi mới từ tư vấn viên
    </h2>
  </div>
  <div style='background:#fff;padding:24px 32px;border:1px solid #e2e8f0;border-top:none'>
    <p>Xin chào <strong>{System.Net.WebUtility.HtmlEncode(toName)}</strong>,</p>
    <p>Câu hỏi của bạn về <strong>{System.Net.WebUtility.HtmlEncode(tenTruong)}</strong>:</p>
    <blockquote style='border-left:3px solid #e5e7eb;margin:0;padding:10px 16px;
                        color:#64748b;background:#f8fafc;border-radius:0 6px 6px 0;font-style:italic'>
      {System.Net.WebUtility.HtmlEncode(cauHoiGoc)}
    </blockquote>
    <p style='margin-top:20px'>Phản hồi từ tư vấn viên:</p>
    <div style='background:#f0fdf4;border:1px solid #bbf7d0;border-radius:8px;padding:16px;
                white-space:pre-wrap;line-height:1.6'>
      {System.Net.WebUtility.HtmlEncode(noiDungPhanHoi)}
    </div>
    <div style='margin-top:28px;text-align:center'>
      <a href='{SiteUrl}/my-profile.aspx#hop-thu'
         style='display:inline-block;background:#10b981;color:#fff;
                padding:12px 28px;border-radius:8px;text-decoration:none;
                font-weight:700;font-size:15px'>
        📬 Xem hộp thư của bạn
      </a>
    </div>
  </div>
  <div style='background:#f8fafc;padding:16px 32px;border:1px solid #e2e8f0;
              border-top:none;border-radius:0 0 12px 12px;
              color:#94a3b8;font-size:12px;text-align:center'>
    Email này được gửi tự động. Vui lòng không reply trực tiếp email này.<br/>
    Hệ thống Tra cứu tuyển sinh — <a href='{SiteUrl}' style='color:#94a3b8'>{SiteUrl}</a>
  </div>
</div>";
        Send(toEmail, subject, body);
    }
}
