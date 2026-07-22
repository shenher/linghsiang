using System.ComponentModel.DataAnnotations;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>後台產品 Detail 維護（主檔基本欄位 + 動態子表列 + 圖片上傳）。</summary>
    public class AdminProductDetailEditViewModel
    {
        public int Id { get; set; }

        // ── 主檔基本欄位（儲存時寫回 Products 工作表，與 Main 列表同源） ──
        [Required(ErrorMessage = "請輸入產品名稱")]
        [StringLength(100)]
        [Display(Name = "產品名稱")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入產品類別")]
        [StringLength(50)]
        [Display(Name = "產品類別")]
        public string Category { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "產品標籤")]
        public string? Tag { get; set; }

        [Display(Name = "可宅配")]
        public bool CanDeliver { get; set; }

        [Display(Name = "可店取")]
        public bool CanPickup { get; set; }

        /// <summary>顯示排序（後台列表與前台菜單皆升冪）。</summary>
        [Required(ErrorMessage = "請輸入排序")]
        [Range(0, 100000, ErrorMessage = "排序須為 0–100000 的整數")]
        [Display(Name = "排序")]
        public int? Sort { get; set; }

        // ── 詳細頁文案 ──
        [Display(Name = "產品描述")]
        public string? Description { get; set; }

        [Display(Name = "過敏原提示")]
        public string? AllergenNote { get; set; }

        // ── 營養標示份量 ──
        [Range(0, 100000, ErrorMessage = "每一份量須為 0 以上的數字")]
        [Display(Name = "每一份量（公克）")]
        public decimal? ServingGrams { get; set; }

        [Range(0, 10000, ErrorMessage = "本包裝含份數須為 0 以上的數字")]
        [Display(Name = "本包裝含（份）")]
        public decimal? ServingsPerPack { get; set; }

        // ── 動態列（前端 JS 增刪，索引繫結） ──
        public List<SizeRowInput> Sizes { get; set; } = new();
        public List<IngredientRowInput> Ingredients { get; set; } = new();
        public List<NutritionRowInput> Nutrition { get; set; } = new();
        public List<NoteRowInput> Notes { get; set; } = new();

        // ── 產品圖片上傳（存 Pic/products/{id}.jpg，可空 = 不更換） ──
        [Display(Name = "產品圖片")]
        public IFormFile? Image { get; set; }

        /// <summary>目前是否已有產品圖（供畫面預覽判斷）。</summary>
        public bool HasImage { get; set; }
    }

    public class SizeRowInput
    {
        [Required(ErrorMessage = "請輸入尺寸名稱")]
        [StringLength(50)]
        public string SizeName { get; set; } = string.Empty;

        [Range(0, 1000000, ErrorMessage = "價格須為 0 以上的數字")]
        public decimal Price { get; set; }
    }

    public class IngredientRowInput
    {
        [Required(ErrorMessage = "請輸入成分名稱")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class NutritionRowInput
    {
        [Required(ErrorMessage = "請輸入營養項目")]
        [StringLength(50)]
        public string Item { get; set; } = string.Empty;

        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public decimal? PerServing { get; set; }

        [Range(0, 1000000)]
        public decimal? Per100g { get; set; }
    }

    public class NoteRowInput
    {
        [Required(ErrorMessage = "請輸入備註內容")]
        [StringLength(200)]
        public string Content { get; set; } = string.Empty;
    }
}
