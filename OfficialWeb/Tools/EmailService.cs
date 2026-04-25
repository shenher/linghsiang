using System.Net;
using System.Net.Mail;
using OfficialWeb.Models;

namespace OfficialWeb.Tools
{
    /// <summary>
    /// 電子郵件服務介面，定義寄信行為供依賴注入使用。
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// 傳送聯絡表單通知信給所有設定的收件者。
        /// </summary>
        /// <param name="model">已通過驗證的聯絡表單資料。</param>
        Task SendContactNotificationAsync(ContactViewModel model);
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
        public async Task SendContactNotificationAsync(ContactViewModel model)
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
                Subject    = $"[拎香培室] 新聯絡表單 — {model.Subject ?? "（無主旨）"}",
                IsBodyHtml = false,
                Body       = BuildEmailBody(model)
            };

            foreach (var recipient in recipients)
            {
                mail.To.Add(recipient);
            }

            await smtpClient.SendMailAsync(mail);

            _logger.LogInformation(
                "聯絡表單通知信已成功寄出，收件者：{Recipients}",
                string.Join("; ", recipients));
        }

        /// <summary>
        /// 組合純文字郵件內文，列出表單各欄位資料。
        /// </summary>
        private static string BuildEmailBody(ContactViewModel model)
        {
            return $"""
                您好，

                您的網站收到一筆新的聯絡表單，詳細資料如下：

                姓名：{model.Name}
                電話：{model.Phone ?? "（未填）"}
                電子郵件：{model.Email}
                主旨：{model.Subject ?? "（未填）"}

                留言內容：
                {model.Message}

                ---
                此封郵件由系統自動發送，請勿直接回覆。
                """;
        }
    }
}
