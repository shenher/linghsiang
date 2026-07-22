using System.ComponentModel.DataAnnotations;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>後台「新增產品」Modal 表單。</summary>
    public class AdminProductCreateViewModel
    {
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

        /// <summary>顯示排序（升冪）；留空 = 自動排在最後（既有最大排序 +1）。</summary>
        [Range(1, 100000, ErrorMessage = "排序須為 1–100000 的整數")]
        [Display(Name = "排序")]
        public int? Sort { get; set; }
    }
}
