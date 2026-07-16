# Docker 部署說明

## 重要觀念：程式碼與站台資料是分開的

| 種類 | 放哪裡 | 誰管理 | 部署時會不會被動到 |
|---|---|---|---|
| 程式碼（含預設菜單種子） | Git repo | `git pull` 更新 | 每次部署更新 |
| 站台資料：`Menu.xlsx`、上傳圖片 | 伺服器 `./data/`（掛進容器 `/app/data`） | 後台網頁維護 | **絕不會被動到** |

- `./data/` 已加入 `.gitignore`，`git pull` 與 `docker compose up --build` 都碰不到它。
- **首次啟動**容器內程式（`EnsureSeeded`）偵測 `data/` 是空的，會自動：
  1. 依程式內建種子建立 `data/Menu.xlsx`（18 筆預設菜單，每筆含 8 列預設營養標示）
  2. 複製內建 Logo 與預設首頁圖到 `data/Pic/`
  3. 建立 `data/Pic/` 的五個子目錄（home / about / logo / products / qrcode）
  → 所以上線第一天就能直接進 `/Admin` 做主檔維護，不需手動準備 Excel。
- 之後每次重新部署，`data/` 內容原封不動——正式環境的菜單永遠是最新的，不會被本機測試資料覆蓋。

---

## 前置準備（每台新電腦只需做一次）

1. 安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop/)（Windows / macOS 皆適用）
2. Clone 專案：
   ```bash
   git clone https://github.com/shenher/linghsiang.git
   cd linghsiang
   ```
3. 建立 `certs/` 資料夾並放入憑證：
   ```bash
   mkdir certs
   # 將 cert.pfx 複製到 certs/ 資料夾
   ```
4. 在專案根目錄建立 `.env` 檔案，填入所需密碼：
   ```
   CERT_PASSWORD=你的憑證密碼
   ADMIN_PASSWORD=後台登入密碼（不設則使用 appsettings.json 的 AdminSettings:Password）
   ```
   > ⚠️ `.env` 已加入 `.gitignore`，不會上傳到 Git。
   >
   > 若要讓 `ADMIN_PASSWORD` 生效，需在 `docker-compose.yml` 的 `environment` 加上
   > `- ADMIN_PASSWORD=${ADMIN_PASSWORD}`（預設未加，使用 appsettings 密碼）。

---

## 首次啟動

```bash
docker compose up -d --build
```

- `--build`：建立 Docker image（首次必須加）
- `-d`：在背景執行，不會佔用終端機視窗

啟動後：
- 網站運行於 `https://你的網域`（443 port）
- `./data/` 自動生成初始 `Menu.xlsx` 與圖片目錄（見上方「重要觀念」）
- 後台網址 `https://你的網域/Admin`

**啟動後建議手動放一張 LINE QRCode**：把 `line.png` 放到 `./data/Pic/qrcode/`，下單頁即會顯示（沒放則顯示佔位圖）。

---

## 程式碼異動後重新部署（日常更新流程）

每次修改程式碼並推上 Git 後，在伺服器上執行：

```bash
git pull
docker compose up -d --build
```

- Docker 重新編譯並替換容器，舊容器自動停止。
- **`./data/` 完全不受影響**：正式環境的菜單、產品圖、首頁/關於圖全部保留。
- repo 內的 `OfficialWeb/Menu.xlsx` 只是「開發用種子」，正式環境只在首次部署（`data/Menu.xlsx` 不存在）時以程式內建的同一份種子重建，之後永遠以 `data/` 為準。

> **Layer cache 說明：** 若只修改了 C# 原始碼（未動 `.csproj`），Docker 會跳過
> `dotnet restore` 步驟，僅重新執行 `dotnet publish`，加快 build 速度。

> **若曾用更早版本（volume 直接掛 `OfficialWeb/Menu.xlsx`）部署過**：升級前先把舊資料搬進 data——
> `mkdir -p data && cp OfficialWeb/Menu.xlsx data/ && cp -r OfficialWeb/Pic data/`，再 `docker compose up -d --build`。

---

## 站台資料維護與備份

- **日常維護一律走後台** `/Admin`：產品主檔/Detail、首頁＆關於圖片。存檔直接落地到 `./data/`。
- 也可以直接動 `./data/` 裡的檔案（進階）：
  - 用 Excel 直接編輯 `data/Menu.xlsx`（工作表與欄位結構不可改），存檔後**不用重啟**，重新整理頁面即生效；
  - 圖檔直接放：LINE QRCode → `data/Pic/qrcode/line.png`、產品照 → `data/Pic/products/{產品編號}.jpg`。
  - 注意：避免「後台正在存檔」的同時用 Excel 開著檔案（檔案被鎖住會導致儲存失敗）。
- **備份**：定期複製整個 `./data/` 即可（`cp -r data data-backup-$(date +%Y%m%d)`）。

---

## 日常操作指令

### 查看運行狀態
```bash
docker compose ps
```

### 查看即時 Log
```bash
docker compose logs -f web
```

### 停止伺服器
```bash
docker compose down
```

### 重新啟動（不重新 build）
```bash
docker compose restart
```

---

## 換電腦部署（Windows ↔ macOS）

在新電腦上重複「前置準備」，再執行首次啟動指令。
`certs/cert.pfx`、`.env`、**`data/`（站台資料）** 皆不在 Git 中，搬家時需一併手動複製。

```bash
# 新電腦完整流程
git clone https://github.com/shenher/linghsiang.git
cd linghsiang
mkdir certs
# 複製 cert.pfx 到 certs/
# 複製舊機的 .env 與 data/（若是全新站台可略過 data/，會自動生成初始資料）
docker compose up -d --build
```

---

## 憑證更換

1. 將新的 `cert.pfx` 覆蓋至 `certs/` 資料夾
2. 若密碼有變更，同步更新 `.env` 中的 `CERT_PASSWORD`
3. 重新啟動容器以套用新憑證：
   ```bash
   docker compose restart
   ```

---

## 目錄結構說明

```
linghsiang/
├── certs/               ← 放憑證 cert.pfx（不上傳 Git）
├── .env                 ← 放密碼：CERT_PASSWORD 等（不上傳 Git）
├── data/                ← 站台資料：Menu.xlsx + Pic/（不上傳 Git，首次啟動自動生成）
│   ├── Menu.xlsx        ←   菜單主檔（後台維護寫這裡）
│   └── Pic/
│       ├── home/        ←   首頁 Hero 背景（後台上傳）
│       ├── about/       ←   關於頁圖片（後台上傳）
│       ├── logo/        ←   商標（首次啟動從程式內建複製）
│       ├── products/    ←   產品照（後台上傳，檔名=產品編號）
│       └── qrcode/      ←   LINE QRCode（手動放 line.png）
├── Dockerfile           ← Docker image 建置設定
├── docker-compose.yml   ← 容器啟動設定（掛載 certs/ 與 data/）
├── .dockerignore        ← 排除不需進入 image 的檔案
└── OfficialWeb/         ← .NET MVC 專案原始碼（含開發用種子 Menu.xlsx 與 Pic/）
```
