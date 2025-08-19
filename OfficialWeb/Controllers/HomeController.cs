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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
