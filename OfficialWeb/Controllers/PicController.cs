using Microsoft.AspNetCore.Mvc;

namespace OfficialWeb.Controllers
{
    /// <summary>
    /// 圖片供應：所有站內圖片一律經由本控制器 GET 提供，實體檔案集中在專案根目錄 Pic/。
    /// 路徑一律由白名單名稱或數字 id 組成，不接受任意檔名（防路徑穿越）。
    /// </summary>
    public class PicController : Controller
    {
        /// <summary>Logo 名稱白名單 → 實體檔名。</summary>
        private static readonly Dictionary<string, string> LogoWhitelist = new(StringComparer.OrdinalIgnoreCase)
        {
            ["mark"] = "logo-mark.png",
            ["h-gold"] = "logo-h-gold.png",
            ["h-white"] = "logo-h-white.png",
            ["v-gold"] = "logo-v-gold.png",
        };

        private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp",
        };

        private readonly string _picRoot;

        public PicController(Services.IDataPaths dataPaths)
        {
            _picRoot = dataPaths.PicRoot;
        }

        /// <summary>首頁 Hero 背景（Pic/home/hero.*）。短快取：後台換圖即時生效。</summary>
        [HttpGet]
        public IActionResult Hero()
        {
            var file = FindByStem(Path.Combine(_picRoot, "home"), "hero");
            return file is null ? NotFound() : ServeFile(file, cacheSeconds: 0);
        }

        /// <summary>關於頁圖片（Pic/about/about.*）；無檔 404，前端顯示佔位圖。後台換圖即時生效。</summary>
        [HttpGet]
        public IActionResult About()
        {
            var file = FindByStem(Path.Combine(_picRoot, "about"), "about");
            return file is null ? NotFound() : ServeFile(file, cacheSeconds: 0);
        }

        /// <summary>Logo（白名單：mark / h-gold / h-white / v-gold）。長快取：Logo 不常更換。</summary>
        [HttpGet]
        public IActionResult Logo(string name)
        {
            if (string.IsNullOrEmpty(name) || !LogoWhitelist.TryGetValue(name, out var fileName))
                return NotFound();
            var path = Path.Combine(_picRoot, "logo", fileName);
            return !System.IO.File.Exists(path) ? NotFound() : ServeFile(path, cacheSeconds: 86400);
        }

        /// <summary>產品照（Pic/products/{id}.jpg）；無檔回 404，前端以佔位圖呈現。</summary>
        [HttpGet]
        public IActionResult Product(int id)
        {
            if (id <= 0) return NotFound();
            var file = FindByStem(Path.Combine(_picRoot, "products"), id.ToString());
            return file is null ? NotFound() : ServeFile(file, cacheSeconds: 0);
        }

        /// <summary>LINE 加好友 QRCode（Pic/qrcode/line.png）；無檔回 404，前端以佔位圖呈現。</summary>
        [HttpGet]
        public IActionResult LineQr()
        {
            var file = FindByStem(Path.Combine(_picRoot, "qrcode"), "line");
            return file is null ? NotFound() : ServeFile(file, cacheSeconds: 3600);
        }

        /// <summary>在指定目錄找「主檔名.白名單副檔名」的實體檔（副檔名依上傳格式而異）。</summary>
        private static string? FindByStem(string dir, string stem)
        {
            if (!Directory.Exists(dir)) return null;
            foreach (var ext in new[] { ".jpg", ".jpeg", ".png", ".webp" })
            {
                var path = Path.Combine(dir, stem + ext);
                if (System.IO.File.Exists(path)) return path;
            }
            return null;
        }

        private IActionResult ServeFile(string path, int cacheSeconds)
        {
            Response.Headers.CacheControl = cacheSeconds <= 0
                ? "no-cache"
                : $"public, max-age={cacheSeconds}";
            var contentType = ContentTypes.GetValueOrDefault(Path.GetExtension(path), "application/octet-stream");
            return PhysicalFile(path, contentType);
        }
    }
}
