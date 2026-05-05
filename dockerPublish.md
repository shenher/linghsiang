# Docker 部署說明

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
   SMTP_PASSWORD=Gmail 應用程式密碼（16 碼，去除空格）
   ```
   > ⚠️ `.env` 已加入 `.gitignore`，不會上傳到 Git。
   >
   > `SMTP_PASSWORD` 為 Gmail 應用程式密碼，非 Google 帳號密碼。
   > 申請方式：Google 帳戶 → 安全性 → 兩步驟驗證 → 應用程式密碼。
   > 若 `SMTP_PASSWORD` 未設定，聯絡表單仍可正常使用，但不會寄出通知信。

---

## 首次啟動

```bash
docker compose up -d --build
```

- `--build`：建立 Docker image（首次必須加）
- `-d`：在背景執行，不會佔用終端機視窗

啟動後網站即運行於 `https://你的網域`，443 port。

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

## 程式碼異動後重新部署

每次修改程式碼並推上 Git 後，在伺服器上執行：

```bash
git pull
docker compose up -d --build
```

Docker 會重新編譯並替換容器，舊容器會自動停止。

> **Layer cache 說明：** 若只修改了 C# 原始碼（未動 `.csproj`），Docker 會跳過 `dotnet restore` 步驟，僅重新執行 `dotnet publish`，加快 build 速度。

---

## 換電腦部署（Windows ↔ macOS）

在新電腦上只需重複「前置準備」的步驟，再執行首次啟動指令即可。
`certs/cert.pfx` 與 `.env` 不在 Git 中，需手動複製到新電腦。

```bash
# 新電腦完整流程
git clone https://github.com/shenher/linghsiang.git
cd linghsiang
mkdir certs
# 複製 cert.pfx 到 certs/
printf "CERT_PASSWORD=你的憑證密碼\nSMTP_PASSWORD=Gmail應用程式密碼\n" > .env
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
├── .env                 ← 放密碼：CERT_PASSWORD、SMTP_PASSWORD（不上傳 Git）
├── Dockerfile           ← Docker image 建置設定
├── docker-compose.yml   ← 容器啟動設定
├── .dockerignore        ← 排除不需進入 image 的檔案
└── OfficialWeb/         ← .NET MVC 專案原始碼
```
