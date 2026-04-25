using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OfficialWeb.Models;
using OfficialWeb.Tools;

namespace OfficialWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // 關於我們
        public IActionResult About()
        {
            return View();
        }

        // 產品介紹
        public IActionResult Products()
        {
            return View();
        }

        // 聯絡我們 GET
        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        // 聯絡我們 POST
        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation(
                "收到聯絡表單 — 主旨: {Subject}",
                model.Subject);

            _logger.LogInformation(
                "收到聯絡表單 — 姓名: {Name}, 電話: {Phone}, 電子郵件: {Email}, 留言內容: {Message}",
                model.Name, model.Phone, model.Email, model.Message);

            // 驗證通過後寄送通知信給管理人員；若寄信失敗不中斷使用者流程
            try
            {
                await _emailService.SendContactNotificationAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寄送聯絡表單通知信時發生錯誤。");
            }

            TempData["SuccessMessage"] = "感謝您的留言！我們將盡速與您聯繫。";
            return RedirectToAction(nameof(Contact));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
