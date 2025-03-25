using System.Text;
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

        public async Task<IActionResult> Index()
        {
            string url = "https://localhost:7069/api/Item";

            List<Item> items = await _client.GetFromJsonAsync<List<Item>>(url);

            return View(items);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();

            var response = await _client.GetAsync($"https://localhost:7069/api/Item/{id}");

            if (!response.IsSuccessStatusCode) return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var item = JsonConvert.DeserializeObject<Item>(json);

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
                var response = await _client.PostAsJsonAsync("https://localhost:7069/api/Item", newItem);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Error while adding new item");
            }

            return View(newItem);
        }

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
                var response = await _client.PutAsync($"https://localhost:7069/api/Item/{id}", content);  // put json variable on the http body

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error while updating item");
                }
            }

            return View(item);
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0) return BadRequest();

            var response = await _client.DeleteAsync($"https://localhost:7069/api/Item/{id}");
            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> DeleteItem(int id)
        //{
        //    if (id <= 0) return BadRequest();

        //    var response = await _client.GetAsync($"https://localhost:7069/api/Item/{id}");
        //    if (!response.IsSuccessStatusCode) return NotFound();

        //    var json = await response.Content.ReadAsStringAsync();
        //    var item = JsonConvert.DeserializeObject<Item>(json);

        //    return View(item);
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ConfirmDelete(int id)
        //{
        //    if (id >= 0) return BadRequest();

        //    var response = await _client.DeleteAsync($"https://localhost:7069/api/Item/{id}");
        //    if (response.IsSuccessStatusCode) return RedirectToAction("Index");

        //    return RedirectToAction("Index");

        //}
    }
}
