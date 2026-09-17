using CrudPractica.Application.Common;
using CrudPractica.Domain.Entities;

namespace CrudPractica.Application.Interfaces
{
    public interface IProductRepository
    {
        // Obtener todos
        Task<IEnumerable<Product>> GetAllAsync();

        // Obtener por ID para lectura
        Task<Product?> GetByIdAsync(int id);

        // Obtener por ID + Categories con tracking para UPDATE N:N
        Task<Product?> GetByIdWithCategoriesAsync(int id);

        // Crear
        Task<Product> CreateAsync(Product product);

        // Actualización tradicional
        Task UpdateAsync(Product product);

        // Eliminar
        Task DeleteAsync(Product product);

        // Guardar cambios de entidades que ya están siendo trackeadas
        Task SaveChangesAsync();

        // Búsqueda + paginación
        Task<PagedResult<Product>> GetPagedAsync(
            string? search,
            int page,
            int pageSize);
    }
}