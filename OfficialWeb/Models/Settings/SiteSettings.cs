namespace OfficialWeb.Models.Settings
{
    /// <summary>全站站台設定（appsettings.json → SiteSettings 區段，Options Pattern 綁定）。</summary>
    public class SiteSettings
    {
        /// <summary>全站字體名稱（Google Fonts 名稱，例：Noto Sans TC）；空值時 fallback 為預設字體。</summary>
        public string? FontFamily { get; set; }
    }
}
