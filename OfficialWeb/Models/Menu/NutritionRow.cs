namespace OfficialWeb.Models.Menu
{
    /// <summary>營養標示列（Menu.xlsx「Nutrition」工作表一列，以產品編號關聯主檔）。</summary>
    public class NutritionRow
    {
        public int ProductId { get; set; }

        /// <summary>營養項目（例：熱量、蛋白質；特殊成分可自行手打）。</summary>
        public string Item { get; set; } = string.Empty;

        /// <summary>單位（例：大卡、公克、毫克）。</summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>每份含量。</summary>
        public decimal? PerServing { get; set; }

        /// <summary>每 100 克含量。</summary>
        public decimal? Per100g { get; set; }

        public int Sort { get; set; }
    }
}
