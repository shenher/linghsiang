namespace OfficialWeb.Models.ViewModels
{
    /// <summary>前台產品頁：分類 tab 清單 + 產品卡片集合。</summary>
    public class ProductsPageViewModel
    {
        /// <summary>不重複產品類別（依 Excel 出現順序），前台另外寫死一個「全部」tab。</summary>
        public List<string> Categories { get; set; } = new();

        public List<ProductCardViewModel> Products { get; set; } = new();
    }

    /// <summary>產品卡片（產品頁格線一格）。</summary>
    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        /// <summary>徽章（招牌／季節／熱賣，可空）。</summary>
        public string? Tag { get; set; }

        /// <summary>尺寸摘要（例：「4 吋 / 6 吋」、「6 入」）。</summary>
        public string SizeSummary { get; set; } = string.Empty;

        /// <summary>價格摘要（例：「NT$ 680 起」、「NT$ 110」）。</summary>
        public string PriceSummary { get; set; } = string.Empty;
    }
}
