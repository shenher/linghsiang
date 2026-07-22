using System.ComponentModel.DataAnnotations;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>後台「首頁＆關於圖片」維護：兩個上傳表單共用此 ViewModel，各自繫結一個欄位。</summary>
    public class HomeImageViewModel
    {
        [Display(Name = "首頁背景圖片")]
        public IFormFile? HeroImage { get; set; }

        [Display(Name = "關於頁圖片")]
        public IFormFile? AboutImage { get; set; }
    }
}
