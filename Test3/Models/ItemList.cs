using Microsoft.AspNetCore.Mvc.Rendering;

namespace Test3.Models
{
    public class ItemList
    {
        public Item item { get; set; }
        public ItemDetail itemDetail { get; set; }
        public SelectList? Items { get; set; }
    }
}
