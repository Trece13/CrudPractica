using CrudPractica.Application.Common;
using CrudPractica.Application.DTOs;
using CrudPractica.Application.Interfaces;
using CrudPractica.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CrudPractica.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // =========================================================
        // GET: api/Products
        // =========================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(products);
        }


        // =========================================================
        // GET: api/Products/5
        // Devuelve ProductResponseDto con las categorías
        // =========================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }


        // =========================================================
        // GET: api/Products/paged?search=mouse&page=1&pageSize=5
        // =========================================================
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            string? search,
            int page = 1,
            int pageSize = 5)
        {
            var result = await _productService.GetPagedAsync(
                search,
                page,
                pageSize);

            return Ok(result);
        }


        // =========================================================
        // POST: api/Products
        // Recibe CreateProductDto
        //
        // Ejemplo:
        // {
        //   "name": "Teclado",
        //   "description": "Teclado mecánico",
        //   "price": 250000,
        //   "quantity": 10,
        //   "categoryIds": [1, 2]
        // }
        // =========================================================
        [HttpPost]
        public async Task<ActionResult<Product>> Create(
            CreateProductDto dto)
        {
            var product = await _productService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product);
        }


        // =========================================================
        // PUT: api/Products/5
        //
        // Ejemplo:
        // {
        //   "id": 5,
        //   "name": "Teclado Pro",
        //   "description": "Teclado mecánico RGB",
        //   "price": 300000,
        //   "quantity": 8,
        //   "categoryIds": [1, 3]
        // }
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateProductDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El id de la URL no coincide con el producto.");

            var updated = await _productService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }


        // =========================================================
        // DELETE: api/Products/5
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}