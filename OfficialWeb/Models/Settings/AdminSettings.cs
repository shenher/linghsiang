namespace OfficialWeb.Models.Settings
{
    /// <summary>後台設定（appsettings.json → AdminSettings 區段；密碼可由環境變數 ADMIN_PASSWORD 覆蓋）。</summary>
    public class AdminSettings
    {
        /// <summary>後台登入密碼。</summary>
        public string? Password { get; set; }
    }
}
