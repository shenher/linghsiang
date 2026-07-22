namespace OfficialWeb.Models.Menu
{
    /// <summary>產品主檔（Menu.xlsx「Products」工作表一列）。</summary>
    public class ProductMain
    {
        /// <summary>產品編號（流水號主鍵，新增時取最大值 +1）。</summary>
        public int Id { get; set; }

        /// <summary>產品名稱。</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>產品類別（自由文字，前台分類 tab 依此欄位動態產生）。</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>產品標籤（卡片左上角徽章，例：招牌／季節／熱賣，可空）。</summary>
        public string? Tag { get; set; }

        /// <summary>可宅配。</summary>
        public bool CanDeliver { get; set; }

        /// <summary>可店取。</summary>
        public bool CanPickup { get; set; }

        /// <summary>產品描述（詳細頁開頭介紹文，可空）。</summary>
        public string? Description { get; set; }

        /// <summary>過敏原提示（詳細頁成分區下方小字，可空）。</summary>
        public string? AllergenNote { get; set; }

        /// <summary>營養標示「每一份量」克數。</summary>
        public decimal? ServingGrams { get; set; }

        /// <summary>營養標示「本包裝含 N 份」（可小數）。</summary>
        public decimal? ServingsPerPack { get; set; }

        /// <summary>前台顯示順序。</summary>
        public int Sort { get; set; }
    }
}
