using CrudPractica.Application.Interfaces;
using CrudPractica.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrudPractica.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _service.GetAllAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Category category)
        {
            var created = await _service.CreateAsync(category);

            return CreatedAtAction(
                nameof(Get),
                new { id = created.Id },
                created);
        }
    }
}
