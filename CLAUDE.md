# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LING HSIANG BAKERY（拎香焙室）— 全素接單烘焙品牌官網 + 菜單主檔維護後台。無資料庫：產品資料由 `Menu.xlsx` 驅動（ClosedXML 讀寫），圖片實體檔集中在 `Pic/` 並一律經由後端 `PicController` GET 提供。資料檔實體位置由 `IDataPaths`（`Services/DataPathService.cs`）解析：未設定 `DataRoot` 時用專案根目錄（本機開發 = repo 內 `OfficialWeb/Menu.xlsx`、`OfficialWeb/Pic/`）；正式環境（Docker）以環境變數 `DataRoot=/app/data` 指到掛載的 `./data/`，讓 git 管的程式碼與站台自己長的正式資料徹底分離。前台五頁（首頁/關於/產品/產品詳細/下單）依 `design-src/` 設計稿實作；`/Admin` 為簡單密碼登入的主檔維護後台。`EmailService` exists as a generic SMTP utility but is not currently wired to any page action.

## Commands

All commands run from the repo root. The solution and project files are under `OfficialWeb/`.

```bash
# Restore dependencies
dotnet restore OfficialWeb/OfficialWeb.csproj

# Build
dotnet build OfficialWeb/OfficialWeb.csproj

# Run (HTTP, development)
dotnet run --project OfficialWeb/OfficialWeb.csproj --urls="http://localhost:5227"

# Run (HTTPS, development)
dotnet run --project OfficialWeb/OfficialWeb.csproj --urls="https://localhost:7097"

# Docker (production)
docker-compose up --build
```

There are no automated tests in this project.

## 設計稿讀取規範（context 保護，全程遵守）

1. **嚴禁以 Read 工具直接開啟 `design-raw/*.html`**（每檔約 16MB）。任何需要從原始檔取資料的操作，一律用 bash 腳本（grep / sed / node / base64）處理，腳本輸出到檔案，終端只印必要的確認訊息。
2. **讀任何檔案前先 `wc -c` 確認大小**。超過 200KB 的檔案不得整檔 Read，改用 grep 定位行號 + Read 帶 offset/limit 分段讀取。
3. **實作採「一頁一循環」**：實作第 N 頁時只讀 `design-src/0N-*.jsx`，完成該頁 View + JS 並通過驗證後，才讀下一頁的 JSX。不得一次讀入五頁 JSX。
4. **優先引用計畫已摘錄的資訊**（色系變數、字體規則、頁面對應表），只有計畫未涵蓋的細節才回頭讀 `design-src/`。
5. **跨頁比對或大量讀檔的探索工作交給 subagent**，在獨立 context 完成、只回傳精簡摘要；主對話 context 保留給實作與決策。
6. `design-raw/` 已加入 `.gitignore`，不 commit；解包產物 `design-src/` 與落地圖檔照常 commit。日後微調畫面直接讀 `design-src/`，不需重新解包。

## Architecture

- **Framework**: ASP.NET Core 8 MVC, .NET SDK 8.0.300 (`OfficialWeb/global.json`)；NuGet：ClosedXML 0.105（MIT，讀寫 .xlsx）
- **Controllers**:
  - `HomeController` — 前台全 GET：`Index`（全屏 Hero）、`About`（理念/地圖/聯絡）、`Products`（讀 Excel 組 `ProductsPageViewModel`，分類 tab 由前端 JS 過濾）、`ProductDetail(int id)`（查無 404）、`Order`；`Contact` 301 轉址到 `About`（保留舊連結）
  - `PicController` — 圖片一律走後端 GET：`Hero`（`Pic/home/hero.*`，no-cache 換圖即生效）、`About`（`Pic/about/about.*`，無檔 404 前端佔位圖）、`Logo(name)`（白名單 mark/h-gold/h-white/v-gold）、`Product(id)`（無檔 404，前端顯示佔位圖）、`LineQr`；路徑全由白名單/數字 id 組成，不接受任意檔名
  - `AdminController` — `[Authorize]`（Cookie 驗證，登入路徑 `/Admin/Login`）：`Login`/`Logout`、`HomeImage`（首頁＆關於圖片維護）+`UploadHomeImage`/`UploadAboutImage`（副檔名白名單 jpg/png/webp、5MB 上限）、`Products`（主檔列表）、`CreateProduct`/`DeleteProduct`（Modal AJAX，JSON 回應；刪除連同四張子表＋產品圖檔）、`ProductDetail(id)`/`SaveProductDetail`（主檔基本欄位＋動態子表列＋圖片上傳整批儲存）
- **Services**:
  - `Services/DataPathService.cs` — `IDataPaths`（DI Singleton）：解析 `Menu.xlsx` 與 `Pic/` 實體路徑（設定 `DataRoot` 有值時用該目錄，否則用 ContentRootPath）
  - `Services/MenuExcelService.cs` — `IMenuService`（DI Singleton，內部 lock 序列化寫入）：`EnsureSeeded`（啟動時 `Menu.xlsx`/`Pic/` 缺件自動重建，Seed 為設計稿 18 筆菜單；DataRoot 與程式目錄分離時另將內建種子圖片補進資料目錄，只補缺不覆蓋）、`GetAll`、`GetCategories`、`GetById`、`Create`（同時寫入 8 列預設營養標示，數值 0：熱量/蛋白質/脂肪/飽和脂肪/反式脂肪/碳水化合物/糖/鈉）、`Delete`、`SaveDetail`。後台 Detail GET 對無營養列的舊資料 fallback 帶入同一組預設列；前台詳細頁在份量未設且營養數值全 0 時隱藏整張營養標示表（視為尚未填寫）
  - `Tools/EmailService.cs` — `IEmailService` generic SMTP utility；registered in DI but not injected anywhere
- **Menu.xlsx（5 個工作表，中文欄名）**: `Products`（產品編號/名稱/類別/標籤/可宅配/可店取/描述/過敏原/每一份量克數/本包裝含份數/排序）、`Sizes`、`Ingredients`、`Nutrition`、`Notes`（子表皆以產品編號關聯）。產品類別為自由文字，前台 tab 依不重複值動態產生＋寫死「全部」
- **Models**: `Models/Menu/`（ProductMain、SizePrice、Ingredient、NutritionRow、ProductNote、ProductDetailData）、`Models/ViewModels/`（強型別 + DataAnnotations）、`Models/Settings/`（SiteSettings、AdminSettings）
- **Views**: `Views/Home/`（前台五頁）、`Views/Admin/`（Login/HomeImage/Products/ProductDetail）、`Views/Shared/_Layout.cshtml`（雙型態導覽列：首頁 light 透明白字（`ViewData["NavVariant"]="light"`）/ 內頁 solid sticky；新頁尾）、`_AdminLayout.cshtml`（後台版面，含 AJAX 防偽 token 來源 `#af-token`）
- **Static assets**: `wwwroot/css/site.css`（主題變數＋分區註解：基礎/導覽/頁尾/共用元件/各頁/後台；RWD 斷點 760px）；`wwwroot/js/` — `home.js`（進場動畫）、`products.js`（分類 tab 前端過濾）、`admin-products.js`（新增/刪除 Modal + AJAX，token 放 `RequestVerificationToken` header）、`admin-product-detail.js`（動態列增刪＋索引重排）；`wwwroot/lib/` local Bootstrap 5.3.2 / jQuery / jquery-validation（layout 引用本地 Bootstrap）

### 主題色系（site.css `:root`，一律以此為準）

`--bg:#faf6ee`、`--surface:#fffaf0`、`--fg:#3a2e22`、`--muted:#7a6a55`、`--line:#e8dfcd`、`--accent:#c97a5a`、`--accent-soft:#f3d9c6`、`--ph-bg:#ece2cf`；LINE 綠 `#06C755`；RWD 斷點 760px

### 全站字體（設定驅動）

`appsettings.json` → `SiteSettings:FontFamily`（Options Pattern 綁定 `SiteSettings`）。`_Layout`/`_AdminLayout` 動態組 Google Fonts `<link>` 並輸出 `<style>:root{--site-font:'字體名'}</style>`；`site.css` 全站 `font-family: var(--site-font), "Noto Sans TC", "Microsoft JhengHei", sans-serif`。改設定重啟即全站換字體；空值 fallback `Noto Sans TC`。

### 後台登入

密碼優先序：環境變數 `ADMIN_PASSWORD` > `appsettings.json` → `AdminSettings:Password`。Cookie 驗證（`/Admin/Login`，8 小時 sliding）。

### Docker 持久化（程式碼 / 站台資料分離）

`docker-compose.yml` 掛 `./data:/app/data`（gitignore）並設 `DataRoot=/app/data`（另有 `./certs:/app/certs`）。首次啟動 `EnsureSeeded` 在空的 data 目錄自動建立初始 `Menu.xlsx` 並複製內建 Logo/Hero 圖；之後 `git pull`、`docker compose up --build` 皆不會動到正式資料。repo 內 `OfficialWeb/Menu.xlsx`、`OfficialWeb/Pic/` 僅為本機開發與種子用。部署細節見 `dockerPublish.md`。

### HTTPS & Certificate

In non-Development environments, the app loads a PFX certificate. Password is resolved in this priority:

1. `appsettings.json` → `CertificatePassword`
2. Environment variable `CERT_PASSWORD`
3. Exception thrown

In Docker, mount certs into `/app/certs/` and set `CERT_PASSWORD` in `docker-compose.yml` or a `.env` file.

### Security middleware (Program.cs)

`X-Content-Type-Options`, `X-Frame-Options`, `X-XSS-Protection`, `Referrer-Policy`, `Permissions-Policy` headers are added manually. Global `AutoValidateAntiforgeryToken` is configured — do **not** add `[ValidateAntiForgeryToken]` on individual actions. AJAX 統一從 `_AdminLayout` 的 `#af-token` 隱藏表單取 token 放進 `RequestVerificationToken` header。

## Coding Conventions

These apply to all MVC code in this project (sourced from `.github/instructions/dotnetCodingGuidelines.instructions.md`):

- **Tag Helpers over Html Helpers** — always use `asp-for`, `asp-action`, `asp-controller`, `asp-validation-for`, etc.
- **Strong-typed ViewModels required** — no `ViewBag` or `ViewData` for passing data to views
- **Data Annotations for validation** — place validation attributes on ViewModel properties; pair with `asp-validation-for` spans in views
- **Single responsibility per action** — keep action methods focused; extract logic to private methods or services if needed
- **Anti-forgery**: global setup is already in place; do not add per-action attributes
- Controller actions that accept form data must use the corresponding ViewModel as the parameter type
