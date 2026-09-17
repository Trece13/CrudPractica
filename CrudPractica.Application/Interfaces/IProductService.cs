using CrudPractica.Application.Common;
using CrudPractica.Application.DTOs;
using CrudPractica.Domain.Entities;

namespace CrudPractica.Application.Interfaces
{
    public interface IProductService
    {
        // Obtener todos los productos
        Task<IEnumerable<Product>> GetAllAsync();

        // Obtener producto por ID incluyendo categorías
        // El Service transforma Product -> ProductResponseDto
        Task<ProductResponseDto?> GetByIdAsync(int id);

        // Crear producto y asignar categorías
        Task<Product> CreateAsync(CreateProductDto dto);

        // Actualizar producto y sus categorías
        Task<bool> UpdateAsync(
            int id,
            UpdateProductDto dto);

        // Eliminar producto
        Task<bool> DeleteAsync(int id);

        // Búsqueda + paginación
        Task<PagedResult<Product>> GetPagedAsync(
            string? search,
            int page,
            int pageSize);
    }
}