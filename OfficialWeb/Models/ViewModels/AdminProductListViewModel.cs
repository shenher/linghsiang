using OfficialWeb.Models.Menu;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>後台產品維護 Main 列表。</summary>
    public class AdminProductListViewModel
    {
        public List<ProductMain> Products { get; set; } = new();
    }
}
