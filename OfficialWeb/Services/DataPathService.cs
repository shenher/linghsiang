namespace OfficialWeb.Services
{
    /// <summary>
    /// 站台資料檔（Menu.xlsx、Pic/）的實體路徑來源。
    /// 未設定 DataRoot 時使用專案根目錄（本機開發：repo 內的 OfficialWeb/Menu.xlsx 與 Pic/）；
    /// 正式環境（Docker）以環境變數 DataRoot 指到掛載目錄（如 /app/data），
    /// 讓 git 管的程式碼與站台自己長的正式資料徹底分離，重新部署不會互相覆蓋。
    /// </summary>
    public interface IDataPaths
    {
        /// <summary>菜單主檔實體路徑。</summary>
        string MenuXlsxPath { get; }

        /// <summary>圖片根目錄（home / logo / products / qrcode 子目錄的上層）。</summary>
        string PicRoot { get; }

        /// <summary>程式內建種子圖片目錄（隨發佈複製的 Pic/，供首次啟動複製初始素材）。</summary>
        string SeedPicRoot { get; }
    }

    public class DataPathService : IDataPaths
    {
        public string MenuXlsxPath { get; }
        public string PicRoot { get; }
        public string SeedPicRoot { get; }

        public DataPathService(IWebHostEnvironment env, IConfiguration configuration)
        {
            var configured = configuration["DataRoot"];
            var dataRoot = string.IsNullOrWhiteSpace(configured)
                ? env.ContentRootPath
                : Path.GetFullPath(configured);

            MenuXlsxPath = Path.Combine(dataRoot, "Menu.xlsx");
            PicRoot = Path.Combine(dataRoot, "Pic");
            SeedPicRoot = Path.Combine(env.ContentRootPath, "Pic");
        }
    }
}
