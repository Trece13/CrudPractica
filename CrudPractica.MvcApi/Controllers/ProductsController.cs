using CrudPractica.Application.Common;
using CrudPractica.Domain.Entities;
using CrudPractica.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CrudPractica.MvcApi.Controllers
{
    public class ProductsController : Controller
    {
        //private readonly AppDbContext _context;

        //public ProductsController(AppDbContext context)
        //{
        //    _context = context;
        //}
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ProductsApi");
        }

        // GET: Products
        //public async Task<IActionResult> Index()
        //{
        //    var products = await _httpClient
        //    .GetFromJsonAsync<List<Product>>("api/Products");

        //    return View(products);
        //}

        public async Task<IActionResult> Index(
        string? search,
        int page = 1,
        int pageSize = 5)
        {
            var url =
                $"api/Products/paged?search={Uri.EscapeDataString(search ?? "")}" +
                $"&page={page}" +
                $"&pageSize={pageSize}";

            var result =
                await _httpClient.GetFromJsonAsync<PagedResult<Product>>(url);

            ViewBag.Search = search;

            return View(result);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync($"api/Products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            response.EnsureSuccessStatusCode();

            var product = await response.Content.ReadFromJsonAsync<Product>();

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Quantity")] Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            var response = await _httpClient.PostAsJsonAsync(
                "api/Products",
                product);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(
                "",
                "No fue posible crear el producto.");

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync(
                $"api/Products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            response.EnsureSuccessStatusCode();

            var product =
                await response.Content.ReadFromJsonAsync<Product>();

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Quantity")] Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(product);

            var response = await _httpClient.PutAsJsonAsync(
                $"api/Products/{id}",
                product);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(
                "",
                "No fue posible actualizar el producto.");

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync(
                $"api/Products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            response.EnsureSuccessStatusCode();

            var product =
                await response.Content.ReadFromJsonAsync<Product>();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync(
        $"api/Products/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            return BadRequest();
        }
    }
}
