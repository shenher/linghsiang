using OfficialWeb.Models.Menu;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>前台產品詳細頁。</summary>
    public class ProductDetailViewModel
    {
        public ProductMain Main { get; set; } = new();
        public List<SizePrice> Sizes { get; set; } = new();
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<NutritionRow> Nutrition { get; set; } = new();
        public List<ProductNote> Notes { get; set; } = new();
    }
}
