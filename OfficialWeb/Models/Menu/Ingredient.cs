namespace OfficialWeb.Models.Menu
{
    /// <summary>成分（Menu.xlsx「Ingredients」工作表一列，以產品編號關聯主檔）。</summary>
    public class Ingredient
    {
        public int ProductId { get; set; }

        /// <summary>成分名稱。</summary>
        public string Name { get; set; } = string.Empty;

        public int Sort { get; set; }
    }
}
