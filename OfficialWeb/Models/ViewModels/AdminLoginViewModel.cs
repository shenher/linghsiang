using System.ComponentModel.DataAnnotations;

namespace OfficialWeb.Models.ViewModels
{
    /// <summary>後台登入表單。</summary>
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "管理密碼")]
        public string Password { get; set; } = string.Empty;

        /// <summary>登入成功後的導回網址（僅接受站內相對路徑）。</summary>
        public string? ReturnUrl { get; set; }
    }
}
