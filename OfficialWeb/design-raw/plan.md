# 拎香焙室官網 UI 改版 + 菜單主檔維護系統 — 詳細實作計畫 v3.1

> v3.1 變更摘要：新增「步驟 P 前置防呆」— `design-src/` 與 `Pic/`（含子目錄）不存在時自動建立；
> `Menu.xlsx` 不存在時，須先讀完本計畫，再依「四、Excel 菜單設計」自行設計欄位並填入預設資料建立初始檔。
>
> v3 變更摘要：新增「步驟 0 設計稿解包」與「設計稿讀取規範（context 保護）」；
> 步驟 7 改為逐頁循環；明確標註哪些工作交給 subagent；`design-raw/` 加入 .gitignore。
> 其餘架構與 v2 相同。

## 一、背景與目標（Context）

現有專案是 ASP.NET Core 8 MVC 的靜態品牌官網（首頁 / 產品介紹 / 聯絡我們），無資料庫。
本次改版要達成：

1. **以五份設計稿為準全面翻新前端**（舊畫面直接蓋掉）：設計稿是 React 原型（自解壓 bundler 格式，共約 79MB，已放在專案 `design-raw/`），需先依「步驟 0」解包為小型純文字檔，再拆解為 MVC 的 Razor View + wwwroot 的 css/js，日後好維護。
2. **產品資料改由 Excel 菜單驅動**：專案根目錄放一份 `Menu.xlsx`，前台產品列表 / 詳細頁都讀它。
3. **新增主檔維護後台（/Admin）**：首頁圖片維護、產品維護 Main / Detail（含營養標示），維護結果寫回 Excel。
4. **所有圖片改由後端 GET Action 提供**，實體檔案集中放在專案根目錄新建的 `Pic/` 資料夾。
5. **全站文字字體由 `appsettings.json` 設定驅動**：改設定重啟即可換全站字體，不需改 CSS。
6. **docker-compose.yml 掛 volume** 持久化 `Menu.xlsx` 與 `Pic/`，容器重建資料不遺失。

已和使用者確認的三項決策：

| 決策 | 結論 |
|---|---|
| 後台保護 | **簡單密碼登入**（appsettings 設密碼 + Cookie 驗證） |
| 產品分類 tab | **以 Excel「產品類別」欄為準動態產生**；畫面寫死一個「全部」tab，進頁預設選「全部」 |
| 產品照片 | **Detail 維護頁加圖片上傳**，存 `Pic/products/{產品編號}.jpg` |

---

## 二、設計稿解析結果（五份 HTML，需先解包）

五份檔案是自解壓 bundler 格式，內部為 React + Babel 原型，內含 JSX 原始碼、主題 CSS 與 base64 圖檔（Logo 等）。原始檔案巨大（合計約 79MB，多數為 base64 資產），**任何工具都不得直接讀取原始 HTML**，一律先依「步驟 0」以腳本解包成 `design-src/` 的小型純文字檔後再使用。

### 頁面對應

| 設計稿 | 內容 | 對應 MVC 頁面 | 處理方式 |
|---|---|---|---|
| 1 首頁 | 全屏蛋糕背景 Hero、透明導覽列、品牌標題、LINE 下單 CTA、進場動畫 | `Home/Index` | **覆蓋** 舊首頁 |
| 2 關於 | 開店理念 + 統計數字、Google 地圖、營業時間、聯絡方式卡片(LINE/IG/FB/電話) | `Home/About` | **新增**（涵蓋舊 Contact 內容；`Home/Contact` 做 301 轉址到 About） |
| 3 產品 | 分類 tab 切換 + 產品卡片格線（hover 顯示「查看更多」）、下單導引卡 | `Home/Products` | **覆蓋** 舊產品頁，資料改讀 Excel |
| 4 下單 | 四步驟下單流程卡、LINE QRCode 大卡、IG/FB 其他管道 | `Home/Order` | **新增** |
| 5 產品詳細 | 左圖右文：尺寸價格選項、成分標籤、台灣格式營養標示表、備註、LINE CTA | `Home/ProductDetail/{id}` | **新增**，資料讀 Excel（原型用 sessionStorage 傳參，改為正規路由參數） |

### 色系（全站 CSS 變數，一律以此為準）

```css
:root {
  --bg: #faf6ee;           /* 米白背景 */
  --surface: #fffaf0;      /* 卡片底 */
  --fg: #3a2e22;           /* 深棕文字 */
  --muted: #7a6a55;        /* 次要文字 */
  --line: #e8dfcd;         /* 邊線 */
  --accent: #c97a5a;       /* 主題陶土橘 */
  --accent-soft: #f3d9c6;  /* 主題淡橘 */
  --ph-bg: #ece2cf;        /* 佔位圖底 */
}
/* LINE 綠 #06C755；RWD 斷點 760px（設計稿的 isMobile 判斷） */
```

### 字體（改為 appsettings.json 設定驅動，Google Fonts CDN 載入）

全站文字字體不寫死在 CSS，改由設定檔控制：

- `appsettings.json` 新增：

  ```json
  "SiteSettings": {
    "FontFamily": "Noto Sans TC"
  }
  ```

- 新增 `Models/Settings/SiteSettings.cs`，`Program.cs` 以 `builder.Services.Configure<SiteSettings>(...)` 綁定（Options Pattern）。
- `_Layout.cshtml` 注入 `IOptions<SiteSettings>` 讀取字體名後做兩件事：
  1. 動態組 Google Fonts `<link>`（字體名做 URL encode，如 `Noto+Sans+TC`）；若為系統字體，CDN 查無該字體只會回 404、不影響顯示，直接落到 fallback。
  2. 在 `<head>` 輸出 `<style>:root{--site-font:'字體名';}</style>`。
- `site.css` 全站文字（body、標題、按鈕、後台）一律 `font-family: var(--site-font), "Noto Sans TC", "Microsoft JhengHei", sans-serif;`，改設定重啟即全站換字體。
- `FontFamily` 空值或未設定時 fallback 為設計稿預設 `Noto Sans TC`。
- 設計稿原本的四字體組合（標題 `Noto Serif TC`／內文 `Noto Sans TC`／英文斜體 `Cormorant Garamond`／等寬標籤 `DM Mono`）**不再分角色套用**，統一被 `--site-font` 取代，確保「一個設定管全站」。

### 其他素材

- **Logo 4 個 PNG**（方標 mark / 橫式金 / 橫式白 / 直式金）以 base64 藏在設計稿共用 JS 內 → 於步驟 0 解包時一併解碼落地到 `Pic/logo/`。
- **首頁 Hero 背景**：設計稿引用 Unsplash 蛋糕照 → 下載存為 `Pic/home/hero.jpg` 當預設圖（下載失敗則以現有 `wwwroot/images/cake1.png` 代替）；之後由後台「首頁圖片維護」上傳取代。
- **LINE QRCode**：設計稿留空顯示佔位圖 → 照做（放 `Pic/qrcode/line.png` 有檔就顯示、沒檔顯示佔位圖）。

---

## 三、設計稿讀取規範（context 保護，全程遵守）

> 本節規則除寫在本計畫外，**須同步抄一份進專案根目錄 `CLAUDE.md`**（新增「設計稿讀取規範」小節）。
> 原因：本計畫是對話中讀入的內容，長 session 觸發 context 壓縮後可能被摘要掉；
> 根目錄 CLAUDE.md 每個 session 常駐，規則放那裡才能撐完整個實作過程。

1. **嚴禁以 Read 工具直接開啟 `design-raw/*.html`**（每檔約 16MB）。任何需要從原始檔取資料的操作，一律用 bash 腳本（grep / sed / node / base64）處理，腳本輸出到檔案，終端只印必要的確認訊息。
2. **讀任何檔案前先 `wc -c` 確認大小**。超過 200KB 的檔案不得整檔 Read，改用 grep 定位行號 + Read 帶 offset/limit 分段讀取。
3. **實作採「一頁一循環」**：實作第 N 頁時只讀 `design-src/0N-*.jsx`，完成該頁 View + JS 並通過驗證後，才讀下一頁的 JSX。**不得一次讀入五頁 JSX**。
4. **優先引用本計畫已摘錄的資訊**（色系變數、字體規則、頁面對應表），只有計畫未涵蓋的細節才回頭讀 `design-src/`。
5. **跨頁比對或大量讀檔的探索工作交給 subagent**（詳見「六、實作步驟」各步驟標註），在獨立 context 完成、只回傳精簡摘要；主對話 context 保留給實作與決策。
6. `design-raw/` 已加入 `.gitignore`，不 commit；解包產物 `design-src/` 與落地圖檔照常 commit。

### Subagent 使用原則（哪些交給 subagent、哪些不要）

- **交給 subagent**（讀得多、產出少的「研究型」工作）：
  - 步驟 0 之後的解包產物健檢（逐檔回報大小與結構摘要）
  - 步驟 6 前的共用元件分析（跨五頁比對導覽列/頁尾/共用樣式，回傳一份共用元件規格）
  - 步驟 7 各頁實作前，若該頁 JSX 解包後仍超過 200KB，先派 subagent 讀該頁並回傳「頁面規格摘要」
- **留在主對話**（需要連貫上下文的「實作型」工作）：
  - 所有程式碼撰寫、View/CSS/JS 實作、Excel Service、後台功能
  - 驗證與除錯（需要看到前面實作的完整脈絡）
- 派 subagent 時注意：**subagent 不繼承主對話歷史**，任務 prompt 須自帶必要背景（例如指明檔案路徑、要回傳的欄位格式），並要求回傳內容控制在精簡摘要，不得整段貼回 JSX 原文。

---

## 四、Excel 菜單設計（`OfficialWeb/Menu.xlsx`，共 5 個工作表）

> 產品類別為**自由文字**，前台 tab 依此欄位的不重複值動態產生（＋寫死的「全部」）。
> 多值資料（尺寸/成分/營養/備註）採「子表 + 產品編號關聯」設計，維護與讀取都單純。

### 工作表 1：產品主檔（Products）
| 欄位 | 型別 | 說明 |
|---|---|---|
| 產品編號 | 整數 | 流水號主鍵，新增時取最大值+1 |
| 產品名稱 | 文字 | 必填 |
| 產品類別 | 文字 | 例：蛋糕、餅乾、塔類…（前台 tab 來源） |
| 產品標籤 | 文字 | 例：招牌、季節、熱賣（卡片左上角徽章，可空） |
| 可宅配 | TRUE/FALSE | |
| 可店取 | TRUE/FALSE | |
| 產品描述 | 文字 | 詳細頁開頭介紹文（可空） |
| 過敏原提示 | 文字 | 詳細頁成分區下方小字（可空） |
| 每一份量克數 | 數字 | 營養標示「每一份量」 |
| 本包裝含份數 | 數字(可小數) | 營養標示「本包裝含 N 份」 |
| 排序 | 整數 | 前台顯示順序 |

### 工作表 2：尺寸價格（Sizes）
| 產品編號 | 尺寸名稱(文字) | 價格(數字) | 排序 |

### 工作表 3：成分（Ingredients）
| 產品編號 | 成分名稱 | 排序 |

### 工作表 4：營養標示（Nutrition）
| 產品編號 | 營養項目 | 單位 | 每份含量(數字) | 每100克含量(數字) | 排序 |

- 預設八列：熱量(大卡)、蛋白質(公克)、脂肪(公克)、飽和脂肪(公克)、反式脂肪(公克)、碳水化合物(公克)、糖(公克)、鈉(毫克)；後台可自行**新增 / 修改 / 刪除**列（特殊成分手打）。

### 工作表 5：備註（Notes）
| 產品編號 | 備註內容 | 排序 |

### 初始資料（Seed）
以設計稿內建菜單建立初始 Excel：蛋糕 8 款、餅乾 8 款、塔類 1 款、吐司 1 款（共 18 筆，含尺寸價格與標籤）；「抹茶紅豆生乳蛋糕」帶完整示範 Detail（成分 6 項、營養標示 8 列、備註 3 條，數值取自設計稿）。App 啟動時若 `Menu.xlsx` 不存在自動以 Seed 重建（防呆），同時把產好的初始檔 commit 進 repo。

> **Menu.xlsx 不存在時的建檔規則（實作階段防呆）**：進入實作前若專案根目錄查無 `Menu.xlsx`，
> 須**先完整讀取本計畫**，再依本節 5 個工作表結構自行思考並設計欄位與**預設資料**建立初始檔：
> 各工作表欄位以上表定義為準（可視實作需要補充合理欄位，但不得刪減既有欄位）；
> 預設資料先以符合手作烘焙品牌調性的合理假資料填入（蛋糕/餅乾/塔類/吐司共 18 筆、含尺寸價格與標籤，
> 並讓其中一筆帶完整示範 Detail：成分、營養標示 8 列、備註）。
> 待步驟 7 從設計稿抽出真實內建菜單後，再回填覆蓋這批預設資料。

> Seed 菜單內容位於設計稿「產品頁」JSX 的內建資料，於步驟 7 實作 Products 頁時（該頁 JSX 已在 context 內）一併抽出，避免為了 Seed 再讀一次設計稿。

---

## 五、後端架構

### 套件
- **ClosedXML**（MIT 授權，讀寫 .xlsx；不用 EPPlus 因其商用需授權）→ 實作前用 context7 / Microsoft Learn 確認 .NET 8 相容版本與 API 寫法。

### 新增 / 修改檔案總覽

```
OfficialWeb/
├── design-raw/                      # 五份設計稿原始 HTML（79MB，.gitignore，禁止直接 Read）
├── design-src/                      # 步驟 0 解包產物（純文字 JSX/CSS，commit 進 repo）
│   ├── 01-home.jsx / 02-about.jsx / 03-products.jsx
│   ├── 04-order.jsx / 05-product-detail.jsx
│   └── theme.css
├── Menu.xlsx                        # 菜單主檔（新增，csproj 設定隨發佈複製）
├── Pic/                             # 圖片實體目錄（新增，隨發佈複製）
│   ├── home/hero.jpg                # 首頁背景（後台可換）
│   ├── logo/logo-mark.png / logo-h-gold.png / logo-h-white.png
│   ├── products/{產品編號}.jpg      # 產品照（後台上傳）
│   └── qrcode/line.png              # LINE QR（手動放檔，可空）
├── Controllers/
│   ├── HomeController.cs            # 改寫：Index/About/Products/Order/ProductDetail/Error(+Contact轉址)
│   ├── PicController.cs             # 新增：圖片 GET Actions
│   └── AdminController.cs           # 新增：登入 + 首頁圖 + 產品 Main/Detail 維護
├── docker-compose.yml               # 修改：加 volumes 掛載 Menu.xlsx 與 Pic/（見「九、注意事項」）
├── appsettings.json                 # 修改：加 SiteSettings:FontFamily、AdminSettings:Password
├── Models/
│   ├── Settings/SiteSettings.cs     # 新增：字體設定 Options 類別
│   ├── Menu/                        # 資料模型：ProductMain, SizePrice, Ingredient, NutritionRow, ProductNote, ProductDetailData
│   └── ViewModels/                  # 強型別 ViewModel（含 DataAnnotations 驗證）：
│                                    #   ProductsPageViewModel, ProductDetailViewModel,
│                                    #   AdminLoginViewModel, AdminProductListViewModel,
│                                    #   AdminProductCreateViewModel, AdminProductDetailEditViewModel, HomeImageViewModel
├── Services/
│   └── MenuExcelService.cs          # IMenuService：ClosedXML 讀寫 Menu.xlsx（lock 保護併發寫入）
│                                    #   GetAll / GetById / Create / Delete(連子表) / SaveDetail / EnsureSeeded
├── Views/
│   ├── Shared/_Layout.cshtml        # 全面改寫：新導覽列(首頁透明白字/內頁sticky實色) + 新頁尾 + Google Fonts
│   ├── Shared/_AdminLayout.cshtml   # 後台簡易版面
│   ├── Home/ Index / About / Products / Order / ProductDetail .cshtml
│   └── Admin/ Login / HomeImage / Products / ProductDetail .cshtml
└── wwwroot/
    ├── css/site.css                 # 全面改寫：主題變數＋分區註解（基礎/導覽/頁尾/各頁/後台），RWD @media 760px
    └── js/ site.js(共用)、home.js(進場動畫)、products.js(分類tab前端過濾)、
        admin-products.js(新增/刪除 modal + AJAX)、admin-product-detail.js(動態列增刪 + 儲存)
```

### Controller / Action 規劃

**HomeController**（全 GET）
- `Index()`：首頁（Hero 背景經由 `/Pic/Hero` 載入）
- `About()`：關於；`Contact()` → `RedirectToActionPermanent("About")`（保留舊連結）
- `Products()`：讀 Excel 組 `ProductsPageViewModel`（全部產品＋動態類別清單），tab 切換由前端 JS 過濾（不重新載頁，符合設計稿行為）
- `ProductDetail(int id)`：讀 Excel 單筆＋子表；查無 → 404
- `Order()`：下單頁

**PicController**（圖片一律走後端 GET，進畫面時才載圖）
- `Hero()`：回傳 `Pic/home/hero.*`
- `Logo(string name)`：白名單（mark / h-gold / h-white）
- `Product(int id)`：`Pic/products/{id}.jpg`，無檔回 404（前端以佔位圖呈現）
- `LineQr()`：`Pic/qrcode/line.png`
- 共通：`PhysicalFile` + 正確 Content-Type + Cache-Control（hero 用短快取或 no-cache，換圖立即生效）；路徑全部由白名單/數字 id 組成，**不接受任意檔名**（防路徑穿越）

**AdminController**（`[Authorize]`，登入除外）
- `Login()` GET/POST：密碼比對 `appsettings: AdminSettings:Password`（支援環境變數覆蓋），成功發 Cookie；`Logout()`
- `HomeImage()` GET + `UploadHomeImage(IFormFile)` POST：預覽現行首頁圖＋上傳取代（副檔名白名單 jpg/png/webp、大小上限 5MB）
- `Products()` GET：主檔列表（產品名/類別/標籤/可宅配/可店取）
- `CreateProduct(vm)` POST：新增 modal 送出 → Excel 主檔 insert
- `DeleteProduct(id)` POST：確認警告後 → 刪主檔＋四張子表該產品所有列＋產品圖檔
- `ProductDetail(int id)` GET：Detail 維護畫面
- `SaveProductDetail(vm)` POST：整頁儲存 → 覆寫該產品的**主檔基本欄位（產品名/類別/標籤/可宅配/可店取）**＋子表資料＋主檔營養份量欄位（＋圖片上傳）

**Program.cs 修改**
- `AddAuthentication().AddCookie(...)`（登入路徑 `/Admin/Login`）；`app.UseAuthentication()` 加在 `UseAuthorization()` 前
- `builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection("SiteSettings"))`（字體設定）
- DI 註冊 `IMenuService`（Singleton，內部 lock）
- 啟動時 `EnsureSeeded()`：`Menu.xlsx` 或 `Pic/` 缺件時自動建立
- 既有安全 Header、全域 AutoValidateAntiforgeryToken **維持不動**（表單用 Tag Helper 自動帶 token；AJAX 以 `RequestVerificationToken` header 帶入）

---

## 六、主檔維護畫面規格

### 1. 首頁圖片維護（/Admin/HomeImage)
- 顯示目前首頁背景預覽（`/Pic/Hero`）＋檔案上傳欄＋「上傳取代」按鈕；成功後顯示新圖。

### 2. 產品維護 Main（/Admin/Products）
- 進頁直接列出全部產品，欄位：**產品名、產品類別、產品標籤、可宅配、可店取**（＋「編輯 Detail」連結）
- **新增**：Bootstrap Modal — 產品名(textbox)、產品類別(textbox)、產品標籤(textbox)、可宅配(checkbox)、可店取(checkbox) → insert 進 Excel 主檔
- **刪除**：確認 Modal 警告「**將連同 Detail 資料一併刪除**」→ 確認後刪 Excel 主檔＋全部子表資料

### 3. 產品維護 Detail（/Admin/ProductDetail/{id}）
頁面最上方為**主檔基本欄位區**（儲存時寫回 Products 工作表，與 Main 列表資料同源）：
- 產品名（textbox，必填）
- 產品類別（textbox，必填；改了類別前台 tab 會跟著動態變動）
- 產品標籤（textbox，可空）
- 可宅配（checkbox）
- 可店取（checkbox）

其下為維護區塊（皆可動態新增/刪除列，數字欄位用 `type="number"`）：
- **尺寸與價格**：尺寸名稱(文字) + 價格(數字)，可多列
- **成分**：成分名稱(文字)，可多列
- **營養標示**：
  - 每一份量＿公克（數字）
  - 本包裝含＿份（數字，可小數）
  - 明細表：營養項目 / 單位 / 每份含量 / 每100克含量，預設 8 列（熱量kcal、蛋白質g、脂肪g、飽和脂肪g、反式脂肪g、碳水化合物g、糖g、鈉mg），列可**新增、修改、刪除**
- **備註**：備註內容(文字)，可多列
- **產品圖片**：上傳欄（存 `Pic/products/{id}.jpg`，前台卡片與詳細頁自動顯示）
- 另含：產品描述、過敏原提示（textarea，供詳細頁文案）
- 最下方**儲存**按鈕 → 依畫面資料整批更新 Excel

---

## 七、實作步驟（依序執行）

P. **前置防呆（進入步驟 0 前必先執行）**：
   - **目錄檢查**：檢查 `design-src/` 與 `Pic/`（含 `Pic/home/`、`Pic/logo/`、`Pic/products/`、`Pic/qrcode/` 四個子目錄）是否存在，不存在則以 `mkdir -p` 建立，並印出實際建立了哪些目錄。
   - **Menu.xlsx 檢查**：檢查專案根目錄是否有 `Menu.xlsx`；**不存在**則：
     1. 先**完整讀取本計畫**（特別是「四、Excel 菜單設計」）；
     2. 依 5 個工作表結構自行思考欄位設計與**預設資料**（規則見「四、初始資料（Seed）」的建檔規則）；
     3. 以 ClosedXML（或步驟 4 的 `EnsureSeeded()` 邏輯）產出初始 `Menu.xlsx` 並 commit 進 repo。
   - 本步驟為冪等操作：目錄與檔案已存在時直接略過、不覆蓋既有內容。

0. **設計稿解包（腳本處理，禁止直接讀原檔）**：
   - 確認 `design-raw/` 內五份 HTML 存在，`ls -la` 記錄大小；**全程不以 Read 工具開啟這些檔案**。
   - 撰寫解包腳本（bash + node 或 python）：以 grep 定位 bundler payload → 解出各頁 JSX 原始碼與主題 CSS → base64 圖檔（Logo 4 個 PNG）直接解碼落地到 `Pic/logo/`，**不留在文字檔內**。
   - 解包產物寫入 `design-src/`：`01-home.jsx`、`02-about.jsx`、`03-products.jsx`、`04-order.jsx`、`05-product-detail.jsx`、`theme.css`。
   - 腳本對每個產物只輸出「檔名 + `wc -c` 大小 + 前 30 行預覽」到終端確認成功，不 cat 全文。
   - 【subagent】解包完成後，派一個 subagent 做產物健檢：逐檔回報大小、是否仍殘留 base64 大段內容、JSX 結構是否完整（有無截斷），回傳一份不超過 30 行的健檢摘要。若有任一檔案超過 200KB，於摘要中標註，後續該頁依「三、設計稿讀取規範」第 2、5 條處理。
   - 把 `design-raw/` 加入 `.gitignore`；`design-src/` commit 進 repo。此後 `design-raw/` 在本任務中不再被讀取。
1. **素材落地**：確認 `Pic/logo/` 已有步驟 0 解碼的 Logo；下載 Hero 圖 → `Pic/home/hero.jpg`（失敗用現有 cake1.png）；`Pic/` 目錄結構（home / products / qrcode）已由步驟 P 建立，此處僅確認完整
2. **csproj**：加 ClosedXML 套件；`Pic/**`、`Menu.xlsx` 設 `CopyToOutputDirectory=PreserveNewest`（Docker 發佈可用）
3. **Models**：Menu 資料模型 + 各 ViewModel（DataAnnotations 驗證：必填、數值範圍）
4. **MenuExcelService**：ClosedXML 讀寫 + lock + `EnsureSeeded()`（產出含 18 筆初始產品的 `Menu.xlsx` 並 commit；Seed 資料先以佔位值建立，步驟 7 實作 Products 頁抽出設計稿內建菜單後回填）。若步驟 P 已建立初始 `Menu.xlsx`，本步驟改為將該欄位設計與預設資料**收斂進 `EnsureSeeded()` 程式碼**（確保執行期防呆與初始檔一致），不重複建檔
5. **PicController**：四個圖片 GET Action + 快取/白名單
6. **_Layout 改寫 + site.css 全面重寫**：
   - 【subagent】實作前先派一個 subagent 做「共用元件分析」：讀取 `design-src/` 五頁 JSX，比對並回傳一份共用元件規格摘要（導覽列兩種型態的結構與 class、頁尾結構、共用按鈕/卡片樣式、`theme.css` 中全站級樣式清單），摘要控制在 60 行以內，不得貼回 JSX 原文。
   - 依摘要 + 本計畫「二、設計稿解析結果」實作：主題變數、雙型態導覽列（首頁透明 / 內頁 sticky）、新頁尾、**字體設定驅動**（`SiteSettings.cs` + appsettings + `_Layout` 動態 Google Fonts link 與 `--site-font` 變數）、RWD（Bootstrap grid + 760px media query）
7. **前台五頁 View + JS（逐頁循環，每頁獨立完成再進下一頁）**：
   每頁固定流程：
   a. `wc -c` 確認該頁 `design-src/0N-*.jsx` 大小 → 未超過 200KB 直接整檔讀入；超過則依「三、設計稿讀取規範」以 grep + 分段讀取，或派 subagent 回傳頁面規格摘要
   b. 拆解為 Razor View + 對應 css 分區 + 頁面 JS
   c. `dotnet build` 通過 → 該頁截圖初步比對
   d. 確認無誤後才讀下一頁 JSX（前一頁 JSX 不再重讀）
   順序與備註：
   - `Index`（進場動畫 home.js）
   - `About`
   - `Products`（products.js tab 過濾、「全部」預設；**同時從本頁 JSX 抽出內建菜單資料回填步驟 4 的 Seed**）
   - `ProductDetail`（營養標示表；sessionStorage 傳參改為路由參數）
   - `Order`
   - HomeController 對應改寫、Contact 轉址、刪除 Contact.cshtml
8. **後台登入**：Cookie 驗證、Login 頁、appsettings 加 `AdminSettings`
9. **後台三畫面 + JS**：HomeImage 上傳、Products Main（modal 新增/刪除）、ProductDetail 編輯（**主檔基本欄位** + 動態列 + 圖片上傳 + 儲存）
10. **docker-compose.yml**：加 volumes 掛載 `Menu.xlsx` 與 `Pic/`（設定見「九、注意事項」），並更新註解
11. **語法查核**：用 context7（或 Microsoft Learn MCP）確認 ClosedXML 與 .NET 8（Cookie 驗證、IFormFile 上傳、PhysicalFile）寫法無誤
12. **驗證**（見下節）後整理註解與 CLAUDE.md（更新專案架構描述＋**新增「設計稿讀取規範」小節**，內容同「三、設計稿讀取規範」第 1–6 條），commit → push `claude/linghsiang-ui-product-system-qfxu8g`

> 補充：後台三畫面（步驟 9）為自行設計的簡易版面，沿用前台主題變數即可，**不需要回頭讀任何 design-src 檔案**。

---

## 八、驗證方式

1. `dotnet build` 無警告錯誤 → `dotnet run --urls=http://localhost:5227`
2. **前台**：五頁逐一開啟，用環境內建 Chromium(Playwright) 以 **375px（手機）與 1280px（桌機）** 各截圖檢查 RWD 與色系是否忠於設計稿
3. **圖片鏈路**：確認頁面圖片皆來自 `/Pic/...`（開發者工具檢查）；刪 hero 檔測 404 fallback
4. **後台流程實測**：
   - 未登入進 /Admin/Products → 導向登入頁；輸入密碼 → 進入
   - 新增產品 → 開 Excel 確認 insert；前台產品頁出現、類別 tab 動態長出
   - 編輯 Detail（改**產品名/類別/標籤/可宅配/可店取**、改尺寸價格/加自訂營養列/上傳圖片）→ 儲存 → Excel 主檔與子表更新、Main 列表與前台（含類別 tab）同步
   - 刪除產品 → 警告 modal → 確認 → 主檔＋子表＋圖檔皆清除
   - 上傳首頁圖 → 首頁背景立即更換
5. 舊網址 `/Home/Contact` 轉址到 `/Home/About`
6. **字體設定**：改 `SiteSettings:FontFamily`（例：換成 `Noto Serif TC`）重啟 → 前台與後台全站字體同步變更；設為空值 → fallback 預設字體、頁面不噴錯
7. **Docker 持久化**（若環境可跑 Docker）：`docker compose up -d` → 後台新增產品、上傳圖片 → `docker compose down` 再 `up` → 資料與圖片仍在；宿主機直接看得到更新後的 `Menu.xlsx` 與 `Pic/products/*.jpg`
8. **repo 檢查**：`git status` 確認 `design-raw/` 未被追蹤（.gitignore 生效）、`design-src/` 與 `Menu.xlsx`、`Pic/` 素材已納入 commit

---

## 九、注意事項

- **Docker 持久化**：`Menu.xlsx`、`Pic/` 在容器內會隨重建消失，本次**直接修改 `docker-compose.yml`** 加上 bind mount（容器內路徑依 Dockerfile `WORKDIR` 為準，預設 `/app`）：

  ```yaml
  services:
    officialweb:            # 沿用既有 service 名稱，其餘設定不動
      volumes:
        - ./Menu.xlsx:/app/Menu.xlsx   # 菜單主檔：後台維護寫入直接落地到宿主機
        - ./Pic:/app/Pic               # 圖片（logo / hero / 產品照 / QRCode）
  ```

  注意事項：
  - **掛載單一檔案時，宿主機上該檔必須先存在**，否則 Docker 會自動建立一個同名「資料夾」導致 app 讀檔失敗——初始 `Menu.xlsx` 已 commit 進 repo，`git clone` 後即存在，正常流程不會踩到；若手動刪檔請先跑一次 `EnsureSeeded`（本機 `dotnet run`）或從 repo 還原再 `docker compose up`。
  - `Pic/` 同理，repo 內保留完整目錄結構（logo 等素材已 commit），確保首次啟動掛載即有內容。
  - 掛 volume 後，容器內 `EnsureSeeded()` 寫入的檔案會同步保留在宿主機，重建容器（`docker compose up -d --build`）資料不遺失。
- **併發寫入**：Excel 非資料庫，MenuExcelService 用 lock 序列化寫入（單站台流量小，足夠）
- **防偽驗證**：沿用全域 `AutoValidateAntiforgeryToken`，不在個別 Action 加屬性；AJAX 統一從 hidden input 取 token 放 header
- **編碼規範**：全程遵守專案 `.github/instructions` 規範 — Tag Helpers、強型別 ViewModel（不用 ViewBag）、DataAnnotations 驗證、單一職責 Action
- **設計稿檔案管理**：`design-raw/`（79MB 原始檔）進 `.gitignore` 不 commit；`design-src/`（解包純文字）commit 進 repo，日後其他 session 微調畫面時直接讀 `design-src/`，不需重新解包
- `EmailService` 不動
