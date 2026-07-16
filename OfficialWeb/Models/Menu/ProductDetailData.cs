namespace OfficialWeb.Models.Menu
{
    /// <summary>單一產品完整資料（主檔 + 四張子表），供詳細頁與後台 Detail 維護使用。</summary>
    public class ProductDetailData
    {
        public ProductMain Main { get; set; } = new();
        public List<SizePrice> Sizes { get; set; } = new();
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<NutritionRow> Nutrition { get; set; } = new();
        public List<ProductNote> Notes { get; set; } = new();
    }
}
