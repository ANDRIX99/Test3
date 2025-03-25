using Newtonsoft.Json;

namespace Test3.Models
{
    public class ItemDetailAdd
    {
        [JsonProperty("Amount")]
        public float Amount { get; set; }

        [JsonProperty("idItem")]
        public int IdItem { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }
    }
}
