namespace OfficialWeb.Models.Menu
{
    /// <summary>尺寸價格（Menu.xlsx「Sizes」工作表一列，以產品編號關聯主檔）。</summary>
    public class SizePrice
    {
        public int ProductId { get; set; }

        /// <summary>尺寸名稱（例：4 吋、6 入、一條）。</summary>
        public string SizeName { get; set; } = string.Empty;

        /// <summary>價格（新台幣）。</summary>
        public decimal Price { get; set; }

        public int Sort { get; set; }
    }
}
