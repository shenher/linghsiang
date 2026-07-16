using ClosedXML.Excel;
using OfficialWeb.Models.Menu;

namespace OfficialWeb.Services
{
    /// <summary>菜單主檔服務：讀寫專案根目錄 Menu.xlsx（5 個工作表）。</summary>
    public interface IMenuService
    {
        /// <summary>啟動防呆：Menu.xlsx 不存在時以內建 Seed 重建；Pic/ 目錄缺件時自動建立。</summary>
        void EnsureSeeded();

        /// <summary>全部產品主檔（依排序）。</summary>
        List<ProductMain> GetAll();

        /// <summary>不重複產品類別（依主檔排序後的出現順序），前台分類 tab 來源。</summary>
        List<string> GetCategories();

        /// <summary>單一產品完整資料（主檔 + 四張子表）；查無回傳 null。</summary>
        ProductDetailData? GetById(int id);

        /// <summary>新增產品主檔（產品編號取最大值 +1），回傳新編號。</summary>
        int Create(ProductMain product);

        /// <summary>刪除產品：主檔 + 四張子表該產品所有列。回傳是否有刪到主檔。</summary>
        bool Delete(int id);

        /// <summary>整批儲存單一產品：覆寫主檔基本欄位與營養份量欄位 + 重建四張子表該產品資料。</summary>
        bool SaveDetail(ProductDetailData data);
    }

    /// <summary>
    /// IMenuService 的 ClosedXML 實作。
    /// Excel 非資料庫：以單一 lock 序列化所有讀寫（單站台流量小，足夠）。
    /// </summary>
    public class MenuExcelService : IMenuService
    {
        private const string ProductsSheet = "Products";
        private const string SizesSheet = "Sizes";
        private const string IngredientsSheet = "Ingredients";
        private const string NutritionSheet = "Nutrition";
        private const string NotesSheet = "Notes";

        private readonly string _xlsxPath;
        private readonly string _picRoot;
        private readonly object _lock = new();

        public MenuExcelService(IWebHostEnvironment env)
        {
            _xlsxPath = Path.Combine(env.ContentRootPath, "Menu.xlsx");
            _picRoot = Path.Combine(env.ContentRootPath, "Pic");
        }

        // ────────────────────────────── 讀取 ──────────────────────────────

        public List<ProductMain> GetAll()
        {
            lock (_lock)
            {
                using var wb = new XLWorkbook(_xlsxPath);
                return ReadProducts(wb.Worksheet(ProductsSheet))
                    .OrderBy(p => p.Sort).ThenBy(p => p.Id)
                    .ToList();
            }
        }

        public List<string> GetCategories()
        {
            return GetAll()
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .ToList();
        }

        public ProductDetailData? GetById(int id)
        {
            lock (_lock)
            {
                using var wb = new XLWorkbook(_xlsxPath);
                var main = ReadProducts(wb.Worksheet(ProductsSheet)).FirstOrDefault(p => p.Id == id);
                if (main is null) return null;

                var data = new ProductDetailData { Main = main };

                foreach (var row in DataRows(wb.Worksheet(SizesSheet)).Where(r => CellInt(r, 1) == id))
                    data.Sizes.Add(new SizePrice
                    {
                        ProductId = id,
                        SizeName = row.Cell(2).GetString().Trim(),
                        Price = CellDecimal(row, 3) ?? 0,
                        Sort = CellInt(row, 4) ?? 0,
                    });

                foreach (var row in DataRows(wb.Worksheet(IngredientsSheet)).Where(r => CellInt(r, 1) == id))
                    data.Ingredients.Add(new Ingredient
                    {
                        ProductId = id,
                        Name = row.Cell(2).GetString().Trim(),
                        Sort = CellInt(row, 3) ?? 0,
                    });

                foreach (var row in DataRows(wb.Worksheet(NutritionSheet)).Where(r => CellInt(r, 1) == id))
                    data.Nutrition.Add(new NutritionRow
                    {
                        ProductId = id,
                        Item = row.Cell(2).GetString().Trim(),
                        Unit = row.Cell(3).GetString().Trim(),
                        PerServing = CellDecimal(row, 4),
                        Per100g = CellDecimal(row, 5),
                        Sort = CellInt(row, 6) ?? 0,
                    });

                foreach (var row in DataRows(wb.Worksheet(NotesSheet)).Where(r => CellInt(r, 1) == id))
                    data.Notes.Add(new ProductNote
                    {
                        ProductId = id,
                        Content = row.Cell(2).GetString().Trim(),
                        Sort = CellInt(row, 3) ?? 0,
                    });

                data.Sizes = data.Sizes.OrderBy(x => x.Sort).ToList();
                data.Ingredients = data.Ingredients.OrderBy(x => x.Sort).ToList();
                data.Nutrition = data.Nutrition.OrderBy(x => x.Sort).ToList();
                data.Notes = data.Notes.OrderBy(x => x.Sort).ToList();
                return data;
            }
        }

        // ────────────────────────────── 寫入 ──────────────────────────────

        public int Create(ProductMain product)
        {
            lock (_lock)
            {
                using var wb = new XLWorkbook(_xlsxPath);
                var ws = wb.Worksheet(ProductsSheet);
                var existing = ReadProducts(ws);
                var newId = existing.Count == 0 ? 1 : existing.Max(p => p.Id) + 1;
                var newSort = existing.Count == 0 ? 1 : existing.Max(p => p.Sort) + 1;

                var row = ws.Row(LastDataRow(ws) + 1);
                WriteProductRow(row, new ProductMain
                {
                    Id = newId,
                    Name = product.Name,
                    Category = product.Category,
                    Tag = product.Tag,
                    CanDeliver = product.CanDeliver,
                    CanPickup = product.CanPickup,
                    Description = product.Description,
                    AllergenNote = product.AllergenNote,
                    ServingGrams = product.ServingGrams,
                    ServingsPerPack = product.ServingsPerPack,
                    Sort = newSort,
                });
                wb.Save();
                return newId;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                using var wb = new XLWorkbook(_xlsxPath);
                var deleted = DeleteRowsById(wb.Worksheet(ProductsSheet), id) > 0;
                DeleteRowsById(wb.Worksheet(SizesSheet), id);
                DeleteRowsById(wb.Worksheet(IngredientsSheet), id);
                DeleteRowsById(wb.Worksheet(NutritionSheet), id);
                DeleteRowsById(wb.Worksheet(NotesSheet), id);
                wb.Save();
                return deleted;
            }
        }

        public bool SaveDetail(ProductDetailData data)
        {
            lock (_lock)
            {
                using var wb = new XLWorkbook(_xlsxPath);
                var ws = wb.Worksheet(ProductsSheet);
                var mainRow = DataRows(ws).FirstOrDefault(r => CellInt(r, 1) == data.Main.Id);
                if (mainRow is null) return false;

                // 主檔：保留既有排序，覆寫其餘欄位
                var keepSort = CellInt(mainRow, 11) ?? 0;
                data.Main.Sort = keepSort;
                WriteProductRow(mainRow, data.Main);

                // 子表：先刪該產品所有列，再依畫面資料重建
                RewriteChildRows(wb.Worksheet(SizesSheet), data.Main.Id, data.Sizes,
                    (row, x, sort) =>
                    {
                        row.Cell(2).Value = x.SizeName;
                        row.Cell(3).Value = x.Price;
                        row.Cell(4).Value = sort;
                    });
                RewriteChildRows(wb.Worksheet(IngredientsSheet), data.Main.Id, data.Ingredients,
                    (row, x, sort) =>
                    {
                        row.Cell(2).Value = x.Name;
                        row.Cell(3).Value = sort;
                    });
                RewriteChildRows(wb.Worksheet(NutritionSheet), data.Main.Id, data.Nutrition,
                    (row, x, sort) =>
                    {
                        row.Cell(2).Value = x.Item;
                        row.Cell(3).Value = x.Unit;
                        row.Cell(4).Value = x.PerServing is null ? Blank.Value : x.PerServing.Value;
                        row.Cell(5).Value = x.Per100g is null ? Blank.Value : x.Per100g.Value;
                        row.Cell(6).Value = sort;
                    });
                RewriteChildRows(wb.Worksheet(NotesSheet), data.Main.Id, data.Notes,
                    (row, x, sort) =>
                    {
                        row.Cell(2).Value = x.Content;
                        row.Cell(3).Value = sort;
                    });

                wb.Save();
                return true;
            }
        }

        // ────────────────────────────── 共用讀寫 ──────────────────────────────

        private static IEnumerable<IXLRow> DataRows(IXLWorksheet ws) => ws.RowsUsed().Skip(1);

        private static int LastDataRow(IXLWorksheet ws) => ws.LastRowUsed()?.RowNumber() ?? 1;

        private static int? CellInt(IXLRow row, int col)
            => row.Cell(col).TryGetValue<int>(out var v) ? v : null;

        private static decimal? CellDecimal(IXLRow row, int col)
            => row.Cell(col).TryGetValue<decimal>(out var v) ? v : null;

        private static bool CellBool(IXLRow row, int col)
        {
            var cell = row.Cell(col);
            if (cell.TryGetValue<bool>(out var b)) return b;
            return cell.GetString().Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase);
        }

        private static List<ProductMain> ReadProducts(IXLWorksheet ws)
        {
            var list = new List<ProductMain>();
            foreach (var row in DataRows(ws))
            {
                var id = CellInt(row, 1);
                if (id is null) continue;
                list.Add(new ProductMain
                {
                    Id = id.Value,
                    Name = row.Cell(2).GetString().Trim(),
                    Category = row.Cell(3).GetString().Trim(),
                    Tag = NullIfEmpty(row.Cell(4).GetString()),
                    CanDeliver = CellBool(row, 5),
                    CanPickup = CellBool(row, 6),
                    Description = NullIfEmpty(row.Cell(7).GetString()),
                    AllergenNote = NullIfEmpty(row.Cell(8).GetString()),
                    ServingGrams = CellDecimal(row, 9),
                    ServingsPerPack = CellDecimal(row, 10),
                    Sort = CellInt(row, 11) ?? 0,
                });
            }
            return list;
        }

        private static string? NullIfEmpty(string s)
            => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static void WriteProductRow(IXLRow row, ProductMain p)
        {
            row.Cell(1).Value = p.Id;
            row.Cell(2).Value = p.Name;
            row.Cell(3).Value = p.Category;
            row.Cell(4).Value = p.Tag is null ? Blank.Value : p.Tag;
            row.Cell(5).Value = p.CanDeliver;
            row.Cell(6).Value = p.CanPickup;
            row.Cell(7).Value = p.Description is null ? Blank.Value : p.Description;
            row.Cell(8).Value = p.AllergenNote is null ? Blank.Value : p.AllergenNote;
            row.Cell(9).Value = p.ServingGrams is null ? Blank.Value : p.ServingGrams.Value;
            row.Cell(10).Value = p.ServingsPerPack is null ? Blank.Value : p.ServingsPerPack.Value;
            row.Cell(11).Value = p.Sort;
        }

        private static int DeleteRowsById(IXLWorksheet ws, int id)
        {
            var targets = DataRows(ws).Where(r => CellInt(r, 1) == id).ToList();
            // 由下往上刪，避免列號位移
            foreach (var row in targets.OrderByDescending(r => r.RowNumber()))
                row.Delete();
            return targets.Count;
        }

        private static void RewriteChildRows<T>(
            IXLWorksheet ws, int productId, IReadOnlyList<T> items, Action<IXLRow, T, int> writeCells)
        {
            DeleteRowsById(ws, productId);
            var next = LastDataRow(ws) + 1;
            for (var i = 0; i < items.Count; i++)
            {
                var row = ws.Row(next + i);
                row.Cell(1).Value = productId;
                writeCells(row, items[i], i + 1);
            }
        }

        // ────────────────────────────── Seed ──────────────────────────────

        public void EnsureSeeded()
        {
            lock (_lock)
            {
                // Pic/ 目錄缺件時自動建立（含四個子目錄）
                foreach (var sub in new[] { "home", "logo", "products", "qrcode" })
                    Directory.CreateDirectory(Path.Combine(_picRoot, sub));

                if (File.Exists(_xlsxPath)) return;

                using var wb = new XLWorkbook();

                var products = wb.Worksheets.Add(ProductsSheet);
                SetHeader(products, "產品編號", "產品名稱", "產品類別", "產品標籤", "可宅配", "可店取",
                    "產品描述", "過敏原提示", "每一份量克數", "本包裝含份數", "排序");

                var sizes = wb.Worksheets.Add(SizesSheet);
                SetHeader(sizes, "產品編號", "尺寸名稱", "價格", "排序");

                var ingredients = wb.Worksheets.Add(IngredientsSheet);
                SetHeader(ingredients, "產品編號", "成分名稱", "排序");

                var nutrition = wb.Worksheets.Add(NutritionSheet);
                SetHeader(nutrition, "產品編號", "營養項目", "單位", "每份含量", "每100克含量", "排序");

                var notes = wb.Worksheets.Add(NotesSheet);
                SetHeader(notes, "產品編號", "備註內容", "排序");

                var seed = BuildSeedData();
                var r = 2;
                foreach (var p in seed)
                {
                    WriteProductRow(products.Row(r++), p.Main);
                }
                AppendSeedRows(sizes, seed.SelectMany(d => d.Sizes),
                    (row, x) => { row.Cell(1).Value = x.ProductId; row.Cell(2).Value = x.SizeName; row.Cell(3).Value = x.Price; row.Cell(4).Value = x.Sort; });
                AppendSeedRows(ingredients, seed.SelectMany(d => d.Ingredients),
                    (row, x) => { row.Cell(1).Value = x.ProductId; row.Cell(2).Value = x.Name; row.Cell(3).Value = x.Sort; });
                AppendSeedRows(nutrition, seed.SelectMany(d => d.Nutrition),
                    (row, x) =>
                    {
                        row.Cell(1).Value = x.ProductId; row.Cell(2).Value = x.Item; row.Cell(3).Value = x.Unit;
                        row.Cell(4).Value = x.PerServing is null ? Blank.Value : x.PerServing.Value;
                        row.Cell(5).Value = x.Per100g is null ? Blank.Value : x.Per100g.Value;
                        row.Cell(6).Value = x.Sort;
                    });
                AppendSeedRows(notes, seed.SelectMany(d => d.Notes),
                    (row, x) => { row.Cell(1).Value = x.ProductId; row.Cell(2).Value = x.Content; row.Cell(3).Value = x.Sort; });

                foreach (var ws in wb.Worksheets) ws.Columns().AdjustToContents();
                wb.SaveAs(_xlsxPath);
            }
        }

        private static void SetHeader(IXLWorksheet ws, params string[] titles)
        {
            for (var i = 0; i < titles.Length; i++)
                ws.Cell(1, i + 1).Value = titles[i];
            ws.Row(1).Style.Font.SetBold();
        }

        private static void AppendSeedRows<T>(IXLWorksheet ws, IEnumerable<T> items, Action<IXLRow, T> writeCells)
        {
            var r = 2;
            foreach (var x in items) writeCells(ws.Row(r++), x);
        }

        /// <summary>
        /// 內建 Seed 菜單（取自設計稿產品頁：蛋糕 8、餅乾 8、塔類 1、吐司 1，共 18 筆；
        /// 「抹茶紅豆生乳蛋糕」帶完整示範 Detail — 成分 6 項、營養標示 8 列、備註 3 條）。
        /// </summary>
        private static List<ProductDetailData> BuildSeedData()
        {
            var list = new List<ProductDetailData>();
            var sort = 0;

            ProductDetailData Add(string name, string category, string? tag, bool deliver, bool pickup,
                params (string size, decimal price)[] sizePrices)
            {
                sort++;
                var d = new ProductDetailData
                {
                    Main = new ProductMain
                    {
                        Id = sort, Name = name, Category = category, Tag = tag,
                        CanDeliver = deliver, CanPickup = pickup, Sort = sort,
                    },
                };
                for (var i = 0; i < sizePrices.Length; i++)
                    d.Sizes.Add(new SizePrice
                    {
                        ProductId = sort, SizeName = sizePrices[i].size,
                        Price = sizePrices[i].price, Sort = i + 1,
                    });
                list.Add(d);
                return d;
            }

            // ── 蛋糕（8 款） ──
            var matcha = Add("抹茶紅豆生乳蛋糕", "蛋糕", "招牌", false, true,
                ("4 吋（2–3 人）", 480), ("6 吋（4–6 人）", 680), ("8 吋（8–10 人）", 980));
            Add("巧克力堅果磅蛋糕", "蛋糕", null, true, true, ("6 吋", 580));
            Add("香草檸檬戚風", "蛋糕", null, false, true, ("6 吋", 520));
            Add("莓果優格生乳酪", "蛋糕", "季節", false, true, ("4 吋", 520), ("6 吋", 720));
            Add("伯爵奶茶戚風", "蛋糕", null, false, true, ("6 吋", 560));
            Add("桂圓核桃磅蛋糕", "蛋糕", null, true, true, ("6 吋", 580));
            Add("焦糖蘋果蛋糕", "蛋糕", "季節", true, true, ("6 吋", 620));
            Add("黑糖薑味蛋糕", "蛋糕", null, true, true, ("6 吋", 540));

            // ── 餅乾（8 款） ──
            Add("伯爵奶茶餅乾", "餅乾", "熱賣", true, true, ("6 入", 180));
            Add("檸檬糖霜餅乾", "餅乾", null, true, true, ("6 入", 160));
            Add("抹茶夏威夷豆餅乾", "餅乾", "熱賣", true, true, ("6 入", 190));
            Add("黑芝麻杏仁餅乾", "餅乾", null, true, true, ("6 入", 170));
            Add("蔓越莓燕麥餅乾", "餅乾", null, true, true, ("6 入", 160));
            Add("可可榛果餅乾", "餅乾", null, true, true, ("6 入", 185));
            Add("海鹽巧克力餅乾", "餅乾", "季節", true, true, ("6 入", 185));
            Add("椰香雪球餅乾", "餅乾", null, true, true, ("8 入", 150));

            // ── 塔類 / 吐司 ──
            Add("焦糖海鹽巧克力塔", "塔類", null, false, true, ("單顆", 110));
            Add("黑芝麻米吐司", "吐司", null, true, true, ("一條", 240));

            // ── 示範 Detail：抹茶紅豆生乳蛋糕（數值取自設計稿詳細頁） ──
            var id = matcha.Main.Id;
            matcha.Main.Description = "宇治抹茶配自家熬煮紅豆，夾入輕盈的植物性生乳餡。全素、無蛋無奶，茶香紮實而不甜膩。";
            matcha.Main.AllergenNote = "本產品不含蛋、奶、堅果可另外註記；製作環境含有堅果與小麥。";
            matcha.Main.ServingGrams = 100;
            matcha.Main.ServingsPerPack = 4.5m;
            matcha.Ingredients.AddRange(new[]
            {
                "台灣米麩", "日本宇治抹茶", "有機豆漿生乳餡", "北海道紅豆（自家熬煮）", "海藻糖", "冷壓椰子油",
            }.Select((n, i) => new Ingredient { ProductId = id, Name = n, Sort = i + 1 }));
            matcha.Nutrition.AddRange(new (string item, string unit, decimal per, decimal per100)[]
            {
                ("熱量", "大卡", 285, 285),
                ("蛋白質", "公克", 4.2m, 4.2m),
                ("脂肪", "公克", 12.5m, 12.5m),
                ("飽和脂肪", "公克", 6.1m, 6.1m),
                ("反式脂肪", "公克", 0, 0),
                ("碳水化合物", "公克", 34.0m, 34.0m),
                ("糖", "公克", 18.0m, 18.0m),
                ("鈉", "毫克", 95, 95),
            }.Select((n, i) => new NutritionRow
            {
                ProductId = id, Item = n.item, Unit = n.unit,
                PerServing = n.per, Per100g = n.per100, Sort = i + 1,
            }));
            matcha.Notes.AddRange(new[]
            {
                "全素・無蛋無奶", "冷藏保存，建議 2 日內享用", "需提前 5 天於 LINE 預約",
            }.Select((n, i) => new ProductNote { ProductId = id, Content = n, Sort = i + 1 }));

            return list;
        }
    }
}
