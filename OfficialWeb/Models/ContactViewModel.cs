using System.ComponentModel.DataAnnotations;

namespace OfficialWeb.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "請填寫姓名。")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "請填寫電子郵件。")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確。")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "請填寫留言內容。")]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
