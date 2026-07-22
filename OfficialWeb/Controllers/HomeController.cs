using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OfficialWeb.Models;
using OfficialWeb.Models.ViewModels;
using OfficialWeb.Services;

namespace OfficialWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMenuService _menuService;

        public HomeController(ILogger<HomeController> logger, IMenuService menuService)
        {
            _logger = logger;
            _menuService = menuService;
        }

        // 首頁（Hero 背景經由 /Pic/Hero 載入）
        public IActionResult Index()
        {
            return View();
        }

        // 關於（開店理念、地圖、營業時間、聯絡方式；涵蓋舊 Contact 內容）
        public IActionResult About()
        {
            return View();
        }

        // 舊連結保留：/Home/Contact 301 轉址到 About
        public IActionResult Contact()
        {
            return RedirectToActionPermanent(nameof(About));
        }

        // 產品列表：讀 Excel 組全部產品＋動態類別清單；tab 切換由前端 JS 過濾
        public IActionResult Products()
        {
            var products = _menuService.GetAll();
            var vm = new ProductsPageViewModel
            {
                Categories = products
                    .Select(p => p.Category)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList(),
            };

            foreach (var p in products)
            {
                var detail = _menuService.GetById(p.Id);
                var sizes = detail?.Sizes ?? new List<Models.Menu.SizePrice>();
                vm.Products.Add(new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Tag = p.Tag,
                    SizeSummary = BuildSizeSummary(sizes),
                    PriceSummary = BuildPriceSummary(sizes),
                });
            }
            return View(vm);
        }

        // 產品詳細：讀 Excel 單筆＋子表；查無 → 404
        public IActionResult ProductDetail(int id)
        {
            var data = _menuService.GetById(id);
            if (data is null) return NotFound();

            return View(new ProductDetailViewModel
            {
                Main = data.Main,
                Sizes = data.Sizes,
                Ingredients = data.Ingredients,
                Nutrition = data.Nutrition,
                Notes = data.Notes,
            });
        }

        // 下單頁（四步驟流程、LINE QRCode、其他管道）
        public IActionResult Order()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>尺寸摘要：去掉括號附註後以「 / 」串接；同單位時只保留最後一個單位（例：4 / 6 / 8 吋）。</summary>
        private static string BuildSizeSummary(IReadOnlyList<Models.Menu.SizePrice> sizes)
        {
            if (sizes.Count == 0) return string.Empty;
            var names = sizes.Select(s => s.SizeName.Split('（', '(')[0].Trim()).ToList();
            if (names.Count == 1) return names[0];

            // 全部以相同單位結尾（吋/入/條…）時壓縮為「4 / 6 吋」形式
            var last = names[^1];
            var unitIdx = last.LastIndexOf(' ');
            if (unitIdx > 0)
            {
                var unit = last[(unitIdx + 1)..];
                if (names.All(n => n.EndsWith(" " + unit, StringComparison.Ordinal)))
                    return string.Join(" / ", names.Select(n => n[..^(unit.Length + 1)])) + " " + unit;
            }
            return string.Join(" / ", names);
        }

        /// <summary>價格摘要：單一價格「NT$ 680」；多價格取最低「NT$ 480 起」。</summary>
        private static string BuildPriceSummary(IReadOnlyList<Models.Menu.SizePrice> sizes)
        {
            if (sizes.Count == 0) return string.Empty;
            var prices = sizes.Select(s => s.Price).Distinct().ToList();
            var min = prices.Min().ToString("N0");
            return prices.Count == 1 ? $"NT$ {min}" : $"NT$ {min} 起";
        }
    }
}
