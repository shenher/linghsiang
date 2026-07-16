using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OfficialWeb.Models.Menu;
using OfficialWeb.Models.Settings;
using OfficialWeb.Models.ViewModels;
using OfficialWeb.Services;

namespace OfficialWeb.Controllers
{
    /// <summary>主檔維護後台：簡單密碼登入（Cookie 驗證）＋首頁圖片＋產品 Main / Detail 維護。</summary>
    [Authorize]
    public class AdminController : Controller
    {
        /// <summary>圖片上傳白名單與大小上限（5MB）。</summary>
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IMenuService _menuService;
        private readonly AdminSettings _adminSettings;
        private readonly string _picRoot;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IMenuService menuService,
            IOptions<AdminSettings> adminSettings,
            IWebHostEnvironment env,
            ILogger<AdminController> logger)
        {
            _menuService = menuService;
            _adminSettings = adminSettings.Value;
            _picRoot = Path.Combine(env.ContentRootPath, "Pic");
            _logger = logger;
        }

        // ────────────────────────────── 登入 / 登出 ──────────────────────────────

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction(nameof(Products));
            return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // 密碼優先序：環境變數 ADMIN_PASSWORD > appsettings AdminSettings:Password
            var expected = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
            if (string.IsNullOrEmpty(expected)) expected = _adminSettings.Password;

            if (string.IsNullOrEmpty(expected) || vm.Password != expected)
            {
                ModelState.AddModelError(nameof(vm.Password), "密碼錯誤");
                return View(vm);
            }

            var identity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, "admin") },
                CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // ────────────────────────────── 首頁圖片維護 ──────────────────────────────

        [HttpGet]
        public IActionResult HomeImage()
        {
            return View(new HomeImageViewModel());
        }

        [HttpPost]
        public IActionResult UploadHomeImage(HomeImageViewModel vm)
        {
            var error = ValidateImage(vm.Image);
            if (error is not null) ModelState.AddModelError(nameof(vm.Image), error);
            if (!ModelState.IsValid) return View(nameof(HomeImage), vm);

            var dir = Path.Combine(_picRoot, "home");
            Directory.CreateDirectory(dir);

            // 移除既有 hero.*（副檔名可能不同），再存新檔
            foreach (var old in Directory.GetFiles(dir, "hero.*"))
                System.IO.File.Delete(old);

            var ext = Path.GetExtension(vm.Image!.FileName).ToLowerInvariant();
            var path = Path.Combine(dir, "hero" + ext);
            using (var stream = System.IO.File.Create(path))
            {
                vm.Image.CopyTo(stream);
            }
            _logger.LogInformation("首頁背景已更新：{Path}", path);

            TempData["Message"] = "首頁圖片已更新。";
            return RedirectToAction(nameof(HomeImage));
        }

        // ────────────────────────────── 產品維護 Main ──────────────────────────────

        [HttpGet]
        public IActionResult Products()
        {
            return View(new AdminProductListViewModel { Products = _menuService.GetAll() });
        }

        /// <summary>新增產品（Modal AJAX）。</summary>
        [HttpPost]
        public IActionResult CreateProduct(AdminProductCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, message = FirstError() });

            var id = _menuService.Create(new ProductMain
            {
                Name = vm.Name.Trim(),
                Category = vm.Category.Trim(),
                Tag = string.IsNullOrWhiteSpace(vm.Tag) ? null : vm.Tag.Trim(),
                CanDeliver = vm.CanDeliver,
                CanPickup = vm.CanPickup,
            });
            return Json(new { ok = true, id });
        }

        /// <summary>刪除產品：主檔＋四張子表＋產品圖檔（Modal AJAX）。</summary>
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var deleted = _menuService.Delete(id);
            if (!deleted) return NotFound(new { ok = false, message = "查無此產品" });

            var dir = Path.Combine(_picRoot, "products");
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.GetFiles(dir, id + ".*"))
                    System.IO.File.Delete(file);
            }
            return Json(new { ok = true });
        }

        // ────────────────────────────── 產品維護 Detail ──────────────────────────────

        [HttpGet]
        public IActionResult ProductDetail(int id)
        {
            var data = _menuService.GetById(id);
            if (data is null) return NotFound();

            var vm = new AdminProductDetailEditViewModel
            {
                Id = data.Main.Id,
                Name = data.Main.Name,
                Category = data.Main.Category,
                Tag = data.Main.Tag,
                CanDeliver = data.Main.CanDeliver,
                CanPickup = data.Main.CanPickup,
                Description = data.Main.Description,
                AllergenNote = data.Main.AllergenNote,
                ServingGrams = data.Main.ServingGrams,
                ServingsPerPack = data.Main.ServingsPerPack,
                Sizes = data.Sizes.Select(x => new SizeRowInput { SizeName = x.SizeName, Price = x.Price }).ToList(),
                Ingredients = data.Ingredients.Select(x => new IngredientRowInput { Name = x.Name }).ToList(),
                Nutrition = data.Nutrition.Select(x => new NutritionRowInput
                {
                    Item = x.Item,
                    Unit = x.Unit,
                    PerServing = x.PerServing,
                    Per100g = x.Per100g,
                }).ToList(),
                Notes = data.Notes.Select(x => new NoteRowInput { Content = x.Content }).ToList(),
                HasImage = ProductImageExists(id),
            };
            return View(vm);
        }

        /// <summary>整頁儲存：主檔基本欄位＋營養份量欄位＋四張子表＋（選擇性）產品圖片。</summary>
        [HttpPost]
        public IActionResult SaveProductDetail(AdminProductDetailEditViewModel vm)
        {
            if (vm.Image is not null)
            {
                var imageError = ValidateImage(vm.Image);
                if (imageError is not null) ModelState.AddModelError(nameof(vm.Image), imageError);
            }
            if (!ModelState.IsValid)
            {
                vm.HasImage = ProductImageExists(vm.Id);
                return View(nameof(ProductDetail), vm);
            }

            var data = new ProductDetailData
            {
                Main = new ProductMain
                {
                    Id = vm.Id,
                    Name = vm.Name.Trim(),
                    Category = vm.Category.Trim(),
                    Tag = string.IsNullOrWhiteSpace(vm.Tag) ? null : vm.Tag.Trim(),
                    CanDeliver = vm.CanDeliver,
                    CanPickup = vm.CanPickup,
                    Description = string.IsNullOrWhiteSpace(vm.Description) ? null : vm.Description.Trim(),
                    AllergenNote = string.IsNullOrWhiteSpace(vm.AllergenNote) ? null : vm.AllergenNote.Trim(),
                    ServingGrams = vm.ServingGrams,
                    ServingsPerPack = vm.ServingsPerPack,
                },
                Sizes = vm.Sizes.Select((x, i) => new SizePrice
                {
                    ProductId = vm.Id, SizeName = x.SizeName.Trim(), Price = x.Price, Sort = i + 1,
                }).ToList(),
                Ingredients = vm.Ingredients.Select((x, i) => new Ingredient
                {
                    ProductId = vm.Id, Name = x.Name.Trim(), Sort = i + 1,
                }).ToList(),
                Nutrition = vm.Nutrition.Select((x, i) => new NutritionRow
                {
                    ProductId = vm.Id, Item = x.Item.Trim(), Unit = x.Unit.Trim(),
                    PerServing = x.PerServing, Per100g = x.Per100g, Sort = i + 1,
                }).ToList(),
                Notes = vm.Notes.Select((x, i) => new ProductNote
                {
                    ProductId = vm.Id, Content = x.Content.Trim(), Sort = i + 1,
                }).ToList(),
            };

            if (!_menuService.SaveDetail(data)) return NotFound();

            if (vm.Image is not null) SaveProductImage(vm.Id, vm.Image);

            TempData["Message"] = "已儲存。";
            return RedirectToAction(nameof(ProductDetail), new { id = vm.Id });
        }

        // ────────────────────────────── 私用工具 ──────────────────────────────

        /// <summary>圖片驗證：副檔名白名單 jpg/png/webp、大小上限 5MB。回傳錯誤訊息，null 表示通過。</summary>
        private static string? ValidateImage(IFormFile? file)
        {
            if (file is null || file.Length == 0) return "請選擇要上傳的圖片";
            if (file.Length > MaxImageBytes) return "圖片大小不可超過 5MB";
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext)) return "僅接受 jpg / png / webp 圖片";
            return null;
        }

        private bool ProductImageExists(int id)
        {
            var dir = Path.Combine(_picRoot, "products");
            return Directory.Exists(dir) && Directory.GetFiles(dir, id + ".*").Length > 0;
        }

        private void SaveProductImage(int id, IFormFile file)
        {
            var dir = Path.Combine(_picRoot, "products");
            Directory.CreateDirectory(dir);
            foreach (var old in Directory.GetFiles(dir, id + ".*"))
                System.IO.File.Delete(old);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var path = Path.Combine(dir, id + ext);
            using var stream = System.IO.File.Create(path);
            file.CopyTo(stream);
        }

        private string FirstError()
            => ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault() ?? "輸入資料有誤";
    }
}
