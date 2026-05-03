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
        var link = $"{SiteUrl}/Account/XacNhanEmail.aspx?token={Uri.EscapeDataString(token)}";
        var body = $@"
<h2>Xác nhận email đăng ký</h2>
<p>Vui lòng nhấn vào đường dẫn bên dưới để kích hoạt tài khoản:</p>
<p><a href='{link}'>{link}</a></p>
<p>Đường dẫn có hiệu lực trong 24 giờ.</p>";
        Send(to, "[Tra Cứu Tuyển Sinh] Xác nhận email", body);
    }

    public static void GuiDatLaiMatKhau(string to, string token)
    {
        var link = $"{SiteUrl}/Account/DatLaiMatKhau.aspx?token={Uri.EscapeDataString(token)}";
        var body = $@"
<h2>Đặt lại mật khẩu</h2>
<p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản <b>{to}</b>.</p>
<p><a href='{link}'>Nhấn vào đây để đặt lại mật khẩu</a></p>
<p>Đường dẫn có hiệu lực trong 2 giờ. Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>";
        Send(to, "[Tra Cứu Tuyển Sinh] Đặt lại mật khẩu", body);
    }
}
