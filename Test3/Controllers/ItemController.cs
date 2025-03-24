using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Test3.Models;

namespace Test3.Controllers
{
    public class ItemController : Controller
    {
        private readonly HttpClient _client;

        public ItemController(HttpClient client)
        {
            _client = client;
        }

        public IActionResult Index()
        {
            var item = new List<Item>
            {
                new Item { Id = 1, Nome = "Item 1" },
                new Item { Id = 2, Nome = "Item 2" },
                new Item { Id = 3, Nome = "Item 3" },
            };

            return View(item);
        }

        public IActionResult Detail()
        {
            var item = new Item { Id = 1, Nome = "Item 1" };

            return View(item);
        }

        public IActionResult AddItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(Item newItem)
        {
            if (ModelState.IsValid)
            {
                var response = await _client.PostAsJsonAsync("https://localhost:7069/scalar/#tag/item/POST/api/Item", newItem); // change this to the correct URL

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Error while adding new item");
            }

            return View(newItem);
        }

        public IActionResult EditItem()
        {
            return View();
        }

        [HttpPut]
        public async Task<IActionResult> EditItem(Item item)
        {
            string url = $"https://localhost:7069/scalar/#tag/item/PUT/api/Item/{item.Id}"; // change this to the correct URL
            var response = await _client.PutAsJsonAsync(url, item);

            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Error while updating item");
            return RedirectToAction("Index");
        }

        public IActionResult DeleteItem()
        {
            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItem(int id)
        {
            string url = $"https://localhost:7069/scalar/#tag/item/DELETE/api/Item/{id}"; // change this to the correct URL
            var response = await _client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error while deleting item");
            return RedirectToAction("Index");
        }
    }
}
