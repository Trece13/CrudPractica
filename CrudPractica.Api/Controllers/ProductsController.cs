using CrudPractica.Application.Common;
using CrudPractica.Application.DTOs;
using CrudPractica.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CrudPractica.MvcApi.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ProductsApi");
        }


        // =========================================================
        // INDEX
        // Búsqueda + paginación
        // =========================================================
        public async Task<IActionResult> Index(
            string? search,
            int page = 1,
            int pageSize = 5)
        {
            var url =
                $"api/Products/paged?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            var result =
                await _httpClient
                    .GetFromJsonAsync<PagedResult<Product>>(url);

            ViewBag.Search = search;

            return View(result);
        }


        // =========================================================
        // DETAILS
        // =========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product =
                await _httpClient
                    .GetFromJsonAsync<ProductResponseDto>(
                        $"api/Products/{id}");

            if (product == null)
                return NotFound();

            return View(product);
        }


        // =========================================================
        // CREATE GET
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();

            return View(new CreateProductDto());
        }


        // =========================================================
        // CREATE POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();

                return View(dto);
            }

            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/Products",
                    dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "No fue posible crear el producto.");

            await LoadCategoriesAsync();

            return View(dto);
        }


        // =========================================================
        // EDIT GET
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var product =
                await _httpClient
                    .GetFromJsonAsync<ProductResponseDto>(
                        $"api/Products/{id}");

            if (product == null)
                return NotFound();

            await LoadCategoriesAsync();

            var model = new UpdateProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,

                CategoryIds = product.Categories
                    .Select(c => c.Id)
                    .ToList()
            };

            return View(model);
        }


        // =========================================================
        // EDIT POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateProductDto dto)
        {
            if (id != dto.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();

                return View(dto);
            }

            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/Products/{id}",
                    dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                "",
                "No fue posible actualizar el producto.");

            await LoadCategoriesAsync();

            return View(dto);
        }


        // =========================================================
        // DELETE GET
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product =
                await _httpClient
                    .GetFromJsonAsync<ProductResponseDto>(
                        $"api/Products/{id}");

            if (product == null)
                return NotFound();

            return View(product);
        }


        // =========================================================
        // DELETE POST
        // =========================================================
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/Products/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // MÉTODO PRIVADO PARA CARGAR CATEGORÍAS
        // =========================================================
        private async Task LoadCategoriesAsync()
        {
            var categories =
                await _httpClient
                    .GetFromJsonAsync<List<CategoryDto>>(
                        "api/Categories");

            ViewBag.Categories =
                categories ?? new List<CategoryDto>();
        }
    }
}