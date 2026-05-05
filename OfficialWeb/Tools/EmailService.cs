using System.Net;
using System.Net.Mail;

namespace OfficialWeb.Tools
{
    /// <summary>
    /// 通用電子郵件服務介面，供需要透過 SMTP 寄信的功能使用。
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 寄送一封純文字郵件給 <c>EmailSettings:Recipients</c> 中設定的所有收件者。
        /// </summary>
        /// <param name="subject">郵件主旨。</param>
        /// <param name="body">郵件內文（純文字）。</param>
        Task SendAsync(string subject, string body);
    }

    /// <summary>
    /// 使用 SMTP 協定寄送電子郵件的服務實作。
    /// SMTP 相關設定（主機、帳密、收件者等）統一讀取自 appsettings.json 的 EmailSettings 區段。
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task SendAsync(string subject, string body)
        {
            // 讀取 SMTP 設定
            var smtpHost     = _configuration["EmailSettings:SmtpHost"];
            var smtpPortRaw  = _configuration["EmailSettings:SmtpPort"];
            var smtpUser     = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            if (string.IsNullOrWhiteSpace(smtpPassword))
                smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            var fromAddress  = _configuration["EmailSettings:FromAddress"];
            var fromName     = _configuration["EmailSettings:FromDisplayName"] ?? "拎香培室";

            // 缺少必要設定時記錄警告並略過寄信，避免阻斷使用者流程
            if (string.IsNullOrWhiteSpace(smtpHost) ||
                string.IsNullOrWhiteSpace(smtpUser) ||
                string.IsNullOrWhiteSpace(fromAddress))
            {
                _logger.LogWarning("電子郵件設定不完整（SmtpHost / SmtpUser / FromAddress），略過寄信。");
                return;
            }

            var smtpPort = int.TryParse(smtpPortRaw, out var parsedPort) ? parsedPort : 587;

            // 解析收件者清單：以分號分隔，支援多位收件者
            var recipientsRaw = _configuration["EmailSettings:Recipients"] ?? string.Empty;
            var recipients = recipientsRaw
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (recipients.Count == 0)
            {
                _logger.LogWarning("EmailSettings:Recipients 未設定任何收件者，略過寄信。");
                return;
            }

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            using var mail = new MailMessage
            {
                From       = new MailAddress(fromAddress, fromName),
                Subject    = $"[拎香培室] {subject}",
                IsBodyHtml = false,
                Body       = body
            };

            foreach (var recipient in recipients)
            {
                mail.To.Add(recipient);
            }

            await smtpClient.SendMailAsync(mail);

            _logger.LogInformation(
                "郵件已成功寄出，主旨：{Subject}，收件者：{Recipients}",
                subject, string.Join("; ", recipients));
        }
    }
}
