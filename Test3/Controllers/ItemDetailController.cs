using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Test3.Models;

namespace Test3.Controllers
{
    public class ItemDetailController : Controller
    {
        private readonly HttpClient _client;

        public ItemDetailController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            string url = "https://localhost:7069/api/ItemDetail";
            List<ItemDetail> items = await _client.GetFromJsonAsync<List<ItemDetail>>(url);
            return View(items);
        }

        public async Task<IActionResult> AddItemDetail(ItemDetail item)
        {
            var response = await _client.GetAsync($"https://localhost:7069/api/Item");
            if (!response.IsSuccessStatusCode) return NotFound();

            string responseBody = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<Item>>(responseBody);

            var viewModel = new ItemDetailView
            {
                Items = new SelectList(items, "Id", "Nome")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemDetail(ItemDetailView itemDetail)
        {
            if (itemDetail is null) return BadRequest();

            ItemDetail item = new ItemDetail();
            item.ItemId = itemDetail.ItemId;
            item.Amount = itemDetail.Amount;
            item.IdItem = itemDetail.IdItem;

            foreach (var state in ModelState)
            {
                var key = state.Key;
                var errors = state.Value.Errors;

                foreach (var error in errors)
                {
                    // Ogni errore associato alla proprietà 'key'
                    Console.WriteLine($"Error on {key}: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                var response = await _client.PostAsJsonAsync("https://localhost:7069/api/ItemDetail", item);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error while adding new item detail");
            }
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            //var itemNames = await _client.GetFromJsonAsync<List<ItemName>>($"https://localhost:7069/api/ItemDetail/detail/{id}");

            var response = await _client.GetAsync($"https://localhost:7069/api/ItemDetail/detail/{id}");
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("JSON ricevuto: " + json);
            Console.WriteLine("Tipo di json: " + json.GetType());
            var itemNames = JsonConvert.DeserializeObject<List<ItemName>>(json);

            if (itemNames == null) return NotFound();

            var itemDetailViewName = new ItemDetailViewName
            { 
                ItemNames = itemNames
            };

            return View(itemDetailViewName);
        }
    }
}
