namespace OfficialWeb.Models.Menu
{
    /// <summary>備註（Menu.xlsx「Notes」工作表一列，以產品編號關聯主檔）。</summary>
    public class ProductNote
    {
        public int ProductId { get; set; }

        /// <summary>備註內容（例：全素・無蛋無奶）。</summary>
        public string Content { get; set; } = string.Empty;

        public int Sort { get; set; }
    }
}
