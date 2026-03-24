using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OfficialWeb.Models;

namespace OfficialWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            return View();
        }

        // 聯絡我們 POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string Name, string Phone, string Email, string Subject, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
            {
                ModelState.AddModelError(string.Empty, "請填寫必填欄位（姓名、電子郵件、留言內容）。");
                return View();
            }

            _logger.LogInformation("收到聯絡表單 — 主旨: {Subject}", Subject);
            _logger.LogInformation("收到聯絡表單 — 姓名: {Name}, 電話: {Phone}, 電子郵件: {Email}, 留言內容: {Message}", Name, Phone, Email, Message);
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
