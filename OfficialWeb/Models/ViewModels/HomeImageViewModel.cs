using System.ComponentModel.DataAnnotations;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>後台首頁圖片維護（上傳取代 Hero 背景）。</summary>
    public class HomeImageViewModel
    {
        [Required(ErrorMessage = "請選擇要上傳的圖片")]
        [Display(Name = "首頁背景圖片")]
        public IFormFile? Image { get; set; }
    }
}
