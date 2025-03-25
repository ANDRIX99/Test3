using System.Data;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Test3.Models;


namespace Test3.Controllers
{
    public class ItemController : Controller
    {
        // HttpClient is used to send HTTP requests and receive HTTP responses from a web service
        private readonly HttpClient _client;

        public ItemController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            string url = "https://localhost:7069/api/Item";

            List<Item> items = await _client.GetFromJsonAsync<List<Item>>(url);

            return View(items);
        }

        //public async Task<IActionResult> Detail(int id)
        //{
        //    if (id <= 0) return BadRequest();

        //    var response = await _client.GetAsync($"https://localhost:7069/api/Item/{id}");

        //    if (!response.IsSuccessStatusCode) return NotFound();

        //    var json = await response.Content.ReadAsStringAsync();
        //    var item = JsonConvert.DeserializeObject<Item>(json);

        //    return View(item);
        //}

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();

            var response = await _client.GetAsync($"https://localhost:7069/api/Item");
            if (!response.IsSuccessStatusCode) return NotFound();

            string responseBody = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<Item>>(responseBody);

            // Populating items for the select form
            var viewModel = new ItemList
            {
                Items = new SelectList(items, "Id", "Nome")
            };

            return View(viewModel);
        }

        // Used to add new item like flour, pasta, etc
        public IActionResult AddItem()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(Item newItem)
        {
            if (ModelState.IsValid)
            {
                var response = await _client.PostAsJsonAsync("https://localhost:7069/api/Item", newItem);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Error while adding new item");
            }

            return View(newItem);
        }

        // user can edit item name like flour to wheat flour
        public async Task<IActionResult> EditItem(int id)
        {
            if (id <= 0) return BadRequest();

            var response = await _client.GetAsync($"https://localhost:7069/api/Item/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<Item>(json);

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(int id, Item item)
        {
            if (id != item.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"https://localhost:7069/api/Item/{id}", content);  // put content on json body and then send to api

                if (response.IsSuccessStatusCode) return RedirectToAction("Index");
                else ModelState.AddModelError(string.Empty, "Error while updating item");
            }

            return View(item);
        }

        // user can delete item
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0) return BadRequest();

            var response = await _client.DeleteAsync($"https://localhost:7069/api/Item/{id}");
            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            return RedirectToAction("Index");
        }
    }
}
