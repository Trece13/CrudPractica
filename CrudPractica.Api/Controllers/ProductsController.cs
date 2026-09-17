using CrudPractica.Application.Common;
using CrudPractica.Application.DTOs;
using CrudPractica.Application.Interfaces;
using CrudPractica.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrudPractica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productService.GetAllAsync();

            if (products == null)
                return NotFound();

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity
            };


            var createdProduct = await _productService.CreateAsync(product);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var updated = await _productService.UpdateAsync(product);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<Product>>> GetPaged(
        string? search = null,
        int page = 1,
        int pageSize = 5)
        {
            var result = await _productService.GetPagedAsync(
                search,
                page,
                pageSize);

            return Ok(result);
        }

    }
}
