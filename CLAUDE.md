# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LING HSIANG BAKERY（拎香焙室）— 全素接單烘焙品牌官網 + 菜單主檔維護後台。無資料庫：產品資料由 `Menu.xlsx` 驅動（ClosedXML 讀寫），圖片實體檔集中在 `Pic/` 並一律經由後端 `PicController` GET 提供。資料檔實體位置由 `IDataPaths`（`Services/DataPathService.cs`）解析：未設定 `DataRoot` 時用專案根目錄（本機開發 = repo 內 `OfficialWeb/Menu.xlsx`、`OfficialWeb/Pic/`）；正式環境（Docker）以環境變數 `DataRoot=/app/data` 指到掛載的 `./data/`，讓 git 管的程式碼與站台自己長的正式資料徹底分離。前台五頁（首頁/關於/產品/產品詳細/下單）依 `design-src/` 設計稿實作；`/Admin` 為簡單密碼登入的主檔維護後台。`EmailService` exists as a generic SMTP utility but is not currently wired to any page action.

## Commands

Solution/project under `OfficialWeb/` — build/run with the standard `dotnet` CLI; `docker-compose up --build` for the container. Dev URLs: `http://localhost:5227` (HTTP) / `https://localhost:7097` (HTTPS). There are no automated tests in this project.

## 設計稿讀取規範（context 保護，全程遵守）

1. **嚴禁以 Read 工具直接開啟 `design-raw/*.html`**（每檔約 16MB）。任何需要從原始檔取資料的操作，一律用 bash 腳本（grep / sed / node / base64）處理，腳本輸出到檔案，終端只印必要的確認訊息。
2. **讀任何檔案前先 `wc -c` 確認大小**。超過 200KB 的檔案不得整檔 Read，改用 grep 定位行號 + Read 帶 offset/limit 分段讀取。
3. **實作採「一頁一循環」**：實作第 N 頁時只讀 `design-src/0N-*.jsx`，完成該頁 View + JS 並通過驗證後，才讀下一頁的 JSX。不得一次讀入五頁 JSX。
4. **優先引用計畫已摘錄的資訊**（色系變數、字體規則、頁面對應表），只有計畫未涵蓋的細節才回頭讀 `design-src/`。
5. **跨頁比對或大量讀檔的探索工作交給 subagent**，在獨立 context 完成、只回傳精簡摘要；主對話 context 保留給實作與決策。
6. `design-raw/` 已加入 `.gitignore`，不 commit；解包產物 `design-src/` 與落地圖檔照常 commit。日後微調畫面直接讀 `design-src/`，不需重新解包。

## 開發慣例與陷阱

- `PicController` 是圖片存取唯一入口，前台/後台一律透過它取圖，不要直接引用 `Pic/` 路徑；路徑僅接受白名單（Logo 名稱）或數字 id，避免路徑穿越。
- `MenuExcelService`（`IMenuService`）內部以 `lock` 序列化所有寫入（無資料庫交易可用）；新增寫入邏輯務必經過它，不要繞過直接操作 `Menu.xlsx`。
- 新增產品務必連動寫入預設子表（例如 8 列預設營養標示），不要只寫 Products 主表，否則前台詳細頁的營養標示表判斷邏輯會出錯；細節見 `MenuExcelService.Create`。
- `/Home/Contact` 是相容用 301 轉址（導到 `About`），舊頁面已刪除，勿誤以為是獨立頁面。

### 主題色系（site.css `:root`，一律以此為準，勿另訂新色碼）

`--bg:#faf6ee`、`--surface:#fffaf0`、`--fg:#3a2e22`、`--muted:#7a6a55`、`--line:#e8dfcd`、`--accent:#c97a5a`、`--accent-soft:#f3d9c6`、`--ph-bg:#ece2cf`；LINE 綠 `#06C755`；RWD 斷點 760px

### 全站字體（設定驅動，不要寫死字體名稱）

`appsettings.json` → `SiteSettings:FontFamily`。`_Layout`/`_AdminLayout` 動態組 Google Fonts `<link>` 並輸出 `<style>:root{--site-font:'字體名'}</style>`；`site.css` 全站引用 `var(--site-font)`。改設定重啟即全站換字體，空值 fallback `Noto Sans TC`。

### 後台登入密碼

環境變數 `ADMIN_PASSWORD` > `appsettings.json` → `AdminSettings:Password`。Cookie 驗證（`/Admin/Login`，8 小時 sliding）。

### Docker：站台資料與程式碼分離

`DataRoot=/app/data` 時，`EnsureSeeded` 僅在該目錄缺件時自動種子建立，不會覆蓋既有正式資料；`git pull`／`docker compose up --build` 對正式資料無影響。部署細節見 `dockerPublish.md`。

### HTTPS 憑證密碼

非 Development 環境載入 PFX 憑證，密碼優先序：`appsettings.json` → `CertificatePassword` > 環境變數 `CERT_PASSWORD` > 皆未設定則丟例外拒絕啟動。

### 防偽驗證（Program.cs 全域設定）

已全域套用 `AutoValidateAntiforgeryToken`，**不要**在個別 action 加 `[ValidateAntiForgeryToken]`。AJAX 一律從 `_AdminLayout` 的 `#af-token` 取 token 放進 `RequestVerificationToken` header。

## Coding Conventions

MVC 程式碼撰寫規範定義於 `.github/instructions/dotnetCodingGuidelines.instructions.md`（Tag Helpers、強型別 ViewModel、DataAnnotations 驗證、單一職責等），一律遵守，不在此重複列出。
