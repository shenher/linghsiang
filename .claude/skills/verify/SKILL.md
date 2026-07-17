---
name: verify
description: 本專案（ASP.NET Core 8 MVC + Menu.xlsx）改動的端到端驗證配方：隔離 DataRoot 啟動站台、Playwright 驅動前後台畫面。
---

# OfficialWeb 驗證配方

## 建置與啟動

- 環境若無 dotnet SDK：`apt-get install -y dotnet-sdk-8.0`（Ubuntu noble 內建 8.0.1xx；`OfficialWeb/global.json` 要求 8.0.300，但從 **repo 根目錄** 執行 dotnet 不會套用該 global.json，可直接建置）。`builds.dotnet.microsoft.com` 被網路政策擋掉，dotnet-install.sh 不可用。
- 啟動時務必用 scratch 目錄當 `DataRoot`，避免弄髒 repo 內種子 `OfficialWeb/Menu.xlsx`：

```bash
dotnet build OfficialWeb/OfficialWeb.csproj
DataRoot=<scratch>/data ASPNETCORE_ENVIRONMENT=Development \
  dotnet run --project OfficialWeb/OfficialWeb.csproj --no-build --urls="http://localhost:5227" &
```

- 首次啟動 `EnsureSeeded` 會在空的 DataRoot 自動建 18 筆種子菜單（排序 1–18）。要重測乾淨狀態就刪掉 data 目錄重啟。

## 驅動畫面（Playwright）

- `npm install playwright`；chromium 執行檔在 `/opt/pw-browsers/chromium-1194/chrome-linux/chrome`（用 `executablePath` 指定，勿 `playwright install`）。
- 後台登入：`/Admin/Login`，密碼在 `appsettings.json` → `AdminSettings:Password`（目前 `linghsiang2024`），欄位 `input[name="Password"]`。
- 後台 AJAX（新增/刪除產品）防偽 token：`#af-token input[name="__RequestVerificationToken"]`，放 `RequestVerificationToken` header。
- 常用選擇器：後台列表 `.admin-table tbody tr`；新增 Modal `#createModal`／`#createSubmit`；Detail 表單欄位用 asp-for 產生的 id（如 `#Sort`、`#CanDeliver`）；前台卡片 `.product-card`（tab 過濾只切 `style.display`）。

## 值得驅動的流程

1. 後台登入 → 產品列表順序／欄位。
2. 新增產品 Modal（AJAX，成功後 `location.reload()`，等新列出現即可）。
3. 編輯 Detail → 儲存 → 回列表與前台 `/Home/Products` 對照。
4. 前台分類 tab 前端過濾（不重載頁）。

## 陷阱

- 儲存 Detail 成功後是 redirect 回同頁；驗證失敗則同頁重繪並顯示中文錯誤訊息。
- 桌機/手機版面差異大（RWD 斷點 760px），版面問題兩種 viewport 都要截圖。
