# 拎香培室 LING HSIANG BAKERY — 官網 + 菜單主檔維護後台

> 使用 ASP.NET Core 8 MVC 建置的全素接單烘焙品牌官網，支援響應式設計，適用手機與桌機瀏覽；另附密碼登入的 `/Admin` 後台維護菜單主檔與首頁圖片。

---

## 目錄

- [專案簡介](#專案簡介)
- [技術棧](#技術棧)
- [專案結構](#專案結構)
- [環境需求](#環境需求)
- [快速開始](#快速開始)
- [HTTPS 憑證設定](#https-憑證設定)
- [CSS 架構說明](#css-架構說明)
- [資安措施](#資安措施)
- [頁面說明](#頁面說明)
- [注意事項](#注意事項)

---

## 專案簡介

**拎香培室** 官網提供以下功能：

- 前台五頁：首頁（全屏 Hero）、關於（理念/地圖/聯絡）、產品（分類 tab）、產品詳細（尺寸價格/成分/營養標示/備註）、下單導引（LINE QRCode）
- `/Admin` 主檔維護後台：Cookie 密碼登入，維護首頁＆關於圖片、產品主檔（新增/刪除/編輯明細）

本網站**不連接資料庫**：產品資料由 `Menu.xlsx`（ClosedXML 讀寫）驅動，圖片實體檔集中在 `Pic/` 並一律經由後端 `PicController` 提供。正式環境（Docker）以 `DataRoot` 環境變數將站台資料（`Menu.xlsx`、`Pic/`）與 git 管理的程式碼分離，詳見 [`dockerPublish.md`](dockerPublish.md)。

---

## 技術棧

| 項目 | 版本 / 說明 |
|---|---|
| 框架 | ASP.NET Core 8 MVC |
| 資料存取 | ClosedXML 0.105（讀寫 `Menu.xlsx`，無資料庫） |
| 前端 CSS | Bootstrap 5.3.2（本機 `wwwroot/lib/`，非 CDN） |
| 字型 | Google Fonts，透過 `SiteSettings:FontFamily` 設定驅動（預設 Noto Sans TC） |
| HTTPS | Kestrel + PFX 憑證 |
| 安全性 | Cookie 驗證（後台登入）、CSRF Token、HTTP 安全標頭 |

---

## 專案結構

```
OfficialWeb/
├── Controllers/
│   ├── HomeController.cs        # 前台：Index/About/Products/ProductDetail/Order（Contact 301 轉址 About）
│   ├── PicController.cs         # 圖片一律走後端 GET：Hero/About/Logo/Product/LineQr
│   └── AdminController.cs       # 後台：Login/Logout、HomeImage、Products（新增/刪除）、ProductDetail（編輯）
├── Services/
│   ├── DataPathService.cs       # IDataPaths：解析 Menu.xlsx 與 Pic/ 實體路徑（DataRoot 或專案根目錄）
│   └── MenuExcelService.cs      # IMenuService：EnsureSeeded/GetAll/GetCategories/GetById/Create/Delete/SaveDetail
├── Tools/
│   └── EmailService.cs          # IEmailService 介面與 SMTP 寄信實作（通用工具，尚未接上任何頁面動作）
├── Models/
│   ├── ErrorViewModel.cs
│   ├── Menu/                    # ProductMain、SizePrice、Ingredient、NutritionRow、ProductNote、ProductDetailData
│   ├── Settings/                # SiteSettings、AdminSettings
│   └── ViewModels/              # 前台/後台強型別 ViewModel（含 DataAnnotations）
├── Properties/
│   └── launchSettings.json      # 開發環境啟動設定（HTTP/HTTPS 埠號）
├── Views/
│   ├── Home/                    # Index / About / Products / ProductDetail / Order
│   ├── Admin/                   # Login / HomeImage / Products / ProductDetail
│   ├── Shared/
│   │   ├── _Layout.cshtml       # 前台共用版型（雙型態導覽列 + 頁尾）
│   │   ├── _AdminLayout.cshtml  # 後台共用版型（含 AJAX 防偽 token 來源 #af-token）
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css             # 主題樣式（分節管理，見 CSS 架構）
│   ├── js/                      # home.js / products.js / admin-products.js / admin-product-detail.js
│   └── lib/                     # 本機靜態函式庫（Bootstrap 5.3.2、jQuery、jquery-validation）
├── design-src/                  # 設計稿解包產物（純文字 JSX + CSS），供實作對照
├── Menu.xlsx                    # 本機開發用菜單主檔（5 個工作表：Products/Sizes/Ingredients/Nutrition/Notes）
├── Pic/                         # 本機開發用圖片實體檔（home/、logo/、依產品編號的產品圖等）
├── appsettings.json             # 應用程式設定（含憑證密碼佔位符、SiteSettings、AdminSettings、EmailSettings）
├── appsettings.Development.json # 開發環境覆蓋設定
├── global.json                  # .NET SDK 版本鎖定（8.0.300）
├── Program.cs                   # 應用程式進入點、Kestrel 設定、安全標頭、DI 註冊
├── OfficialWeb.csproj
└── OfficialWeb.sln
```

> 正式環境（Docker）以 `DataRoot=/app/data` 將 `Menu.xlsx`、`Pic/` 指到掛載的 `./data/`，與 repo 內僅供本機開發/種子用的 `OfficialWeb/Menu.xlsx`、`OfficialWeb/Pic/` 分離，避免 `git pull` 覆蓋正式資料。

---

## 環境需求

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)（版本鎖定於 `global.json`：8.0.300）
- 有效的 PFX 格式 HTTPS 憑證（正式環境）

---

## 快速開始

### 1. 複製專案

```bash
git clone https://github.com/shenher/linghsiang.git
cd linghsiang/OfficialWeb
```

### 2. 設定憑證密碼

**方式一：環境變數（建議正式環境）**

```bash
export CERT_PASSWORD="你的憑證密碼"
```

**方式二：User Secrets（建議開發環境）**

```bash
dotnet user-secrets init
dotnet user-secrets set "CertificatePassword" "你的憑證密碼"
```

**方式三：appsettings.json（不建議，僅限本機測試）**

在 `appsettings.json` 中填入 `CertificatePassword`，但請確保此檔案不上傳至版本控制。

### 3. 放置憑證

將 PFX 憑證放至：

```
bin/Debug/net8.0/certs/cert.pfx
```

> `.gitignore` 已忽略 `*.pfx` 與 `*/certs` 目錄，憑證不會被提交至版本控制。

### 4. 執行專案

```bash
dotnet run
```

瀏覽器開啟（開發環境）：

- HTTP：`http://localhost:5227`
- HTTPS：`https://localhost:7097`

> 注意：Port 443 僅在正式環境（非 Development）由 Kestrel 直接監聽，開發時使用上述 launchSettings.json 預設埠號。

---

## HTTPS 憑證設定

憑證讀取邏輯位於 `Program.cs`，**讀取方式不需異動**。密碼優先順序如下：

1. `appsettings.json` 中的 `CertificatePassword`
2. 環境變數 `CERT_PASSWORD`
3. 以上皆未設定時，程式將丟出例外並拒絕啟動

> ⚠️ **請勿**將憑證密碼直接寫入程式碼或上傳至版本控制。

---

## CSS 架構說明

主題樣式集中於 `wwwroot/css/site.css`，以區塊注解清楚分類，便於日後維護：

| 節次 | 說明 |
|---|---|
| 1. 主題變數與全域基礎 | `:root` 主題色系變數、內容容器寬度 |
| 2. 導覽列 | 雙型態導覽列（首頁 light 透明白字 / 內頁 solid sticky） |
| 3. 頁尾 | 共用頁尾 |
| 4. 共用按鈕・佔位圖・區塊小標 | 無實體圖檔時顯示的佔位圖、區塊小標樣式 |
| 5. 首頁 | Hero、進場動畫 |
| 6. 關於頁 | 理念/地圖/聯絡卡片 |
| 7. 產品頁 | 分類 tab、產品卡片格線 |
| 8. 產品詳細頁 | 尺寸價格、成分、營養標示表 |
| 9. 下單頁 | 四步驟流程卡、LINE QRCode 主卡 |
| 10. 後台 | `/Admin` 版面樣式 |
| RWD：共用（<760px） | 響應式斷點調整 |

新增樣式時，請依頁面或功能歸入對應節次，保持一致性。

---

## 資安措施

| 措施 | 說明 |
|---|---|
| HTTPS 強制導向 | `app.UseHttpsRedirection()` |
| HSTS | 非開發環境自動啟用 |
| CSRF 防護 | 全域套用 `AutoValidateAntiforgeryTokenAttribute`，無需在個別 Action 額外標注 `[ValidateAntiForgeryToken]`；AJAX 統一從 `_AdminLayout` 的 `#af-token` 取 token 放進 `RequestVerificationToken` header |
| 後台登入保護 | Cookie 驗證（`/Admin/Login`，8 小時 sliding），密碼由環境變數 `ADMIN_PASSWORD` 或 `appsettings.json` → `AdminSettings:Password` 設定 |
| 圖片存取白名單 | `PicController` 路徑全由白名單（Logo 名稱）或數字 id 組成，不接受任意檔名；上傳副檔名白名單 jpg/png/webp、5MB 上限 |
| 本機靜態函式庫 | Bootstrap 5.3.2 / jQuery / jquery-validation 已放置於 `wwwroot/lib/`，非 CDN 載入 |
| HTTP 安全標頭 | `X-Content-Type-Options`、`X-Frame-Options`、`X-XSS-Protection`、`Referrer-Policy`、`Permissions-Policy` |
| 憑證密碼保護 | 透過環境變數或 User Secrets 注入，不寫死於程式碼 |
| 憑證檔案保護 | `.gitignore` 已排除 `*.pfx` 與 `*/certs` |

---

## 頁面說明

### 前台

| 路由 | 頁面 | 說明 |
|---|---|---|
| `/` | 首頁 | 全屏 Hero、進場動畫、LINE CTA |
| `/Home/About` | 關於 | 開店理念＋統計、Google 地圖與營業時間、聯絡方式卡片（`/Home/Contact` 301 轉址於此，保留舊連結） |
| `/Home/Products` | 產品 | 動態分類 tab（全部＋Excel 類別）、產品卡片格線，前端 JS 過濾 |
| `/Home/ProductDetail/{id}` | 產品詳細 | 尺寸價格選項、成分標籤、台灣格式營養標示表、備註、LINE CTA；查無資料回 404 |
| `/Home/Order` | 下單 | 四步驟流程卡、LINE QRCode 主卡、IG/FB 其他管道 |

### 後台

| 路由 | 頁面 | 說明 |
|---|---|---|
| `/Admin/Login` | 登入 | Cookie 密碼驗證 |
| `/Admin/HomeImage` | 首頁＆關於圖片維護 | 上傳首頁 Hero、關於頁圖片 |
| `/Admin/Products` | 產品主檔列表 | Modal AJAX 新增／刪除產品 |
| `/Admin/ProductDetail/{id}` | 產品明細編輯 | 主檔基本欄位＋動態子表列（尺寸/成分/營養/備註）＋圖片上傳整批儲存 |

---

## 注意事項

- 產品資料由 `Menu.xlsx` 驅動（無資料庫），圖片實體檔集中在 `Pic/`；首次啟動時若 `Menu.xlsx`／`Pic/` 缺件，`EnsureSeeded` 會自動以設計稿 18 筆菜單種子重建。
- 圖片一律透過 `PicController` 後端 GET 提供，不要在前台直接引用 `Pic/` 路徑；缺圖時前端會顯示佔位圖。
- 後台登入密碼預設為 `appsettings.json` → `AdminSettings:Password`，正式環境請改用環境變數 `ADMIN_PASSWORD` 覆蓋，勿沿用預設密碼。
- 正式環境（Docker）以 `DataRoot=/app/data` 將站台資料與程式碼分離，`git pull`、`docker compose up --build` 不會覆蓋正式的 `Menu.xlsx`／`Pic/`；部署細節見 [`dockerPublish.md`](dockerPublish.md)。
- 門市地址、電話、營業時間等聯絡資訊請修改 `Views/Home/About.cshtml` 中的對應內容。
- 若未來需要透過 SMTP 寄信，`EmailService` 已實作為通用工具但尚未接上任何頁面動作，只需在 `appsettings.json` 的 `EmailSettings` 區段設定 SMTP 主機、帳號、收件者等；`SmtpPassword` 可改用環境變數 `SMTP_PASSWORD` 注入。

