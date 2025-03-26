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

        // Used to add new ItemDetail of an item
        // for example: pasta is not an ingredient and then we add only the amount of pasta in the recipe
        // for example: flour is an ingredient and the we add the amount of flour we need to make pasta
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
                    // Every error associated with the 'key' property
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

        public async Task<IActionResult> Detail(int id) // Get detail of itemDetail for example pasta detail are flower, water and relative amount
        {
            //var itemNames = await _client.GetFromJsonAsync<List<ItemName>>($"https://localhost:7069/api/ItemDetail/detail/{id}");

            // Get detail of that item
            var response = await _client.GetAsync($"https://localhost:7069/api/ItemDetail/detail/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();   // if item doesn't exists

            var item = await response.Content.ReadFromJsonAsync<ItemDetail>();
            var detailsResponse = await _client.GetAsync($"https://localhost:7069/api/Item");
            var itemDetails = new List<ItemDetailDto>();

            if (detailsResponse.IsSuccessStatusCode) itemDetails = await detailsResponse.Content.ReadFromJsonAsync<List<ItemDetailDto>>();
            var viewModel = new ItemDetailEditView
            {
                ItemId = item.Id,
                ItemName = item.Nome,
                ItemDetails = itemDetails
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges(ItemDetailEditView model)
        {
            if (!ModelState.IsValid) return View("Edit", model);

            foreach (var detail in model.ItemDetails)
            {
                var response = await _client.PutAsJsonAsync($"https://localhost:7069/ItemDetail/{detail.Id}", detail);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Error during updating");
                    return View("Edit", model);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
