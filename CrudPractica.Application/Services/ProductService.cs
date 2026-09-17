using CrudPractica.Application.Common;
using CrudPractica.Application.DTOs;
using CrudPractica.Application.Interfaces;
using CrudPractica.Domain.Entities;

namespace CrudPractica.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }


        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }


        // =========================================================
        // GET BY ID
        // Devuelve DTO para evitar exponer directamente
        // la entidad y para incluir las categorías.
        // =========================================================
        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return null;

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,

                Categories = product.Categories
                    .Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList()
            };
        }


        // =========================================================
        // CREATE
        // Recibe DTO con CategoryIds.
        // =========================================================
        public async Task<Product> CreateAsync(CreateProductDto dto)
        {
            if (dto.Price < 0)
                throw new ArgumentException(
                    "El precio no puede ser negativo.");

            if (dto.Quantity < 0)
                throw new ArgumentException(
                    "La cantidad no puede ser negativa.");

            // Evitamos IDs repetidos:
            // [1, 1, 2, 3] -> [1, 2, 3]
            var categoryIds = dto.CategoryIds
                .Distinct()
                .ToList();

            // Buscamos las entidades Category existentes.
            var categories =
                await _categoryRepository.GetByIdsAsync(categoryIds);

            // Construimos la entidad de dominio.
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                Categories = categories
            };

            // EF insertará Product y las relaciones
            // correspondientes en ProductCategories.
            return await _productRepository.CreateAsync(product);
        }


        // =========================================================
        // UPDATE
        // Actualiza Product + relación muchos a muchos.
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateProductDto dto)
        {
            // IMPORTANTE:
            // Este método del Repository debe traer el producto
            // CON Categories y CON tracking.
            var product =
                await _productRepository
                    .GetByIdWithCategoriesAsync(id);

            if (product == null)
                return false;


            // Validaciones de negocio
            if (dto.Price < 0)
                throw new ArgumentException(
                    "El precio no puede ser negativo.");

            if (dto.Quantity < 0)
                throw new ArgumentException(
                    "La cantidad no puede ser negativa.");


            var categoryIds = dto.CategoryIds
                .Distinct()
                .ToList();


            // Recuperamos las categorías que fueron seleccionadas.
            var categories =
                await _categoryRepository
                    .GetByIdsAsync(categoryIds);


            // Actualizamos propiedades simples.
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;


            // Eliminamos las relaciones anteriores.
            // NO elimina las categorías de la tabla Categories.
            product.Categories.Clear();


            // Agregamos las nuevas relaciones.
            foreach (var category in categories)
            {
                product.Categories.Add(category);
            }


            // Como Product está siendo trackeado,
            // NO necesitamos _context.Products.Update(product).
            await _productRepository.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var product =
                await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);

            return true;
        }


        // =========================================================
        // PAGINACIÓN + BÚSQUEDA
        // =========================================================
        public async Task<PagedResult<Product>> GetPagedAsync(
            string? search,
            int page,
            int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 5;

            if (pageSize > 50)
                pageSize = 50;

            return await _productRepository.GetPagedAsync(
                search,
                page,
                pageSize);
        }
    }
}