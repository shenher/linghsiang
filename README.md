# 拎香培室 LING HSIANG BAKERY — 形象官網

> 使用 ASP.NET Core 8 MVC 建置的烘焙坊形象官網，支援響應式設計，適用手機與桌機瀏覽。

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

**拎香培室** 形象官網提供以下功能：

- 首頁品牌形象展示
- 關於我們（品牌故事）
- 產品介紹（響應式卡片列表）
- 聯絡我們（含 CSRF 防護表單）

本網站定位為靜態形象官網，**不連接資料庫**，聯絡表單僅記錄伺服器 Log，如需寄送郵件可自行擴充 SMTP 服務。

---

## 技術棧

| 項目 | 版本 / 說明 |
|---|---|
| 框架 | ASP.NET Core 8 MVC |
| 前端 CSS | Bootstrap 5.3.2（CDN + SRI 驗證） |
| 字型 | Google Fonts — Noto Sans TC |
| HTTPS | Kestrel + PFX 憑證 |
| 安全性 | CSRF Token、HTTP 安全標頭 |

---

## 專案結構

```
OfficialWeb/
├── Controllers/
│   └── HomeController.cs        # 首頁、關於、產品、聯絡 控制器
├── Models/
│   ├── ContactViewModel.cs      # 聯絡表單驗證 ViewModel（Data Annotations）
│   └── ErrorViewModel.cs
├── Properties/
│   └── launchSettings.json      # 開發環境啟動設定（HTTP/HTTPS 埠號）
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml         # 首頁
│   │   ├── About.cshtml         # 關於我們
│   │   ├── Products.cshtml      # 產品介紹
│   │   └── Contact.cshtml       # 聯絡我們
│   ├── Shared/
│   │   ├── _Layout.cshtml       # 共用版型（導覽列 + 頁尾）
│   │   ├── _Layout.cshtml.css   # 版型 Scoped 樣式（最小化）
│   │   ├── _ValidationScriptsPartial.cshtml  # 用戶端驗證腳本 Partial
│   │   └── Error.cshtml         # 錯誤頁面
│   ├── _ViewImports.cshtml      # 全域 using、Tag Helper 宣告
│   └── _ViewStart.cshtml        # 預設 Layout 設定
├── wwwroot/
│   ├── css/
│   │   └── site.css             # 主題樣式（分節管理，見 CSS 架構）
│   ├── js/
│   │   └── site.js              # 自訂 JavaScript
│   ├── lib/                     # 本機靜態函式庫（Bootstrap 5.3.2、jQuery、jquery-validation）
│   ├── images/                  # 產品圖片（cake1.png、cake2.png）
│   └── img/                     # Logo、Hero 圖片（需自行放置）
├── appsettings.json             # 應用程式設定（含憑證密碼佔位符）
├── appsettings.Development.json # 開發環境覆蓋設定
├── global.json                  # .NET SDK 版本鎖定（8.0.300）
├── Program.cs                   # 應用程式進入點、Kestrel 設定、安全標頭
├── OfficialWeb.csproj
└── OfficialWeb.sln
```

---

## 環境需求

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
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
| 1. 全域 / 基礎樣式 | `body`、`a` 等基本元素 |
| 2. 字型與標籤 | `.bakery-font`、`.bakery-badge` |
| 3. 導覽列 | `.bg-bakery`、`.navbar-*` |
| 4. 主視覺區塊 | `.bg-hero` |
| 5. 卡片 | `.card`（含 hover 效果） |
| 6. 頁尾 | `.text-bakery-footer` |
| 7. 產品頁 | `.product-img` |
| 8. 聯絡表單 | `.contact-form`、`.btn-bakery` |
| 9. 響應式調整 | `@media` 查詢 |

新增樣式時，請依頁面或功能歸入對應節次，保持一致性。

---

## 資安措施

| 措施 | 說明 |
|---|---|
| HTTPS 強制導向 | `app.UseHttpsRedirection()` |
| HSTS | 非開發環境自動啟用 |
| CSRF 防護 | 全域套用 `AutoValidateAntiforgeryTokenAttribute`，無需在個別 Action 額外標注 `[ValidateAntiForgeryToken]` |
| CDN SRI 驗證 | Bootstrap CSS/JS 含 `integrity` 與 `crossorigin` 屬性 |
| HTTP 安全標頭 | `X-Content-Type-Options`、`X-Frame-Options`、`X-XSS-Protection`、`Referrer-Policy`、`Permissions-Policy` |
| 憑證密碼保護 | 透過環境變數或 User Secrets 注入，不寫死於程式碼 |
| 憑證檔案保護 | `.gitignore` 已排除 `*.pfx` 與 `*/certs` |

---

## 頁面說明

| 路由 | 頁面 | 說明 |
|---|---|---|
| `/` | 首頁 | 品牌形象、特色介紹卡片 |
| `/Home/About` | 關於我們 | 品牌故事、門市照片 |
| `/Home/Products` | 產品介紹 | 響應式產品卡片列表 |
| `/Home/Contact` | 聯絡我們 | 聯絡表單（含 CSRF 防護）、門市資訊 |

---

## 注意事項

- `~/img/logo.svg`、`~/img/bakery-hero.png`、`~/img/store-photo.jpg` 需自行放置於 `wwwroot/img/` 目錄。
- `~/images/cake1.png`、`~/images/cake2.png` 已存在於 `wwwroot/images/`，可依需求替換。
- 門市地址、電話、營業時間等資訊請修改 `Contact.cshtml` 中的對應內容。
- 聯絡表單目前僅記錄 Server Log，如需 Email 通知功能，請在 `HomeController.Contact (POST)` 中整合 SMTP 服務。

