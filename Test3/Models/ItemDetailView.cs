using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Test3.Models
{
    public class ItemDetailView
    {
        [JsonProperty("Amount")]
        public float Amount { get; set; }
        public SelectList? Items { get; set; }

        [JsonProperty("idItem")]
        public int? IdItem { get; set; }

        [JsonProperty("itemId")]
        public int? ItemId { get; set; }
    }
}
