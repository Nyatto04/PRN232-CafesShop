using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Settings;

namespace Services.Implementations
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;

        // Dùng IOptions để inject MailSettings từ appsettings.json
        // Dùng ILogger để ghi log nếu có lỗi
        public MailService(IOptions<MailSettings> mailSettings, ILogger<MailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task<BaseResponseDto> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var email = new MimeMessage();
                email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
                email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = htmlBody; // Nội dung email (HTML)
                email.Body = builder.ToMessageBody();

                // Dùng SmtpClient của MailKit
                using var smtp = new SmtpClient();

                // Kết nối tới server SMTP
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

                // Xác thực bằng Mật khẩu ứng dụng
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);

                // Gửi email
                await smtp.SendAsync(email);

                // Ngắt kết nối
                await smtp.DisconnectAsync(true);

                return new BaseResponseDto { Result = ResultValue.Success, Message = "Email đã được gửi." };
            }
            catch (Exception ex)
            {
                // Ghi log lỗi để bạn có thể xem trong cửa sổ Output (Debug)
                _logger.LogError(ex, $"Lỗi khi gửi email đến {toEmail}: {ex.Message}");
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Gửi email thất bại: {ex.Message}" };
            }
        }
    }
}