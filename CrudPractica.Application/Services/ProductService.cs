using CrudPractica.Application.Common;
using CrudPractica.Application.Interfaces;
using CrudPractica.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudPractica.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository) { 
            _productRepository = productRepository;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            if (product.Price < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            if (product.Quantity < 0)
                throw new ArgumentException("La cantidad no puede ser negativa.");

            return await _productRepository.CreateAsync(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);

            return true;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(product.Id);

            if (existingProduct == null)
                return false;

            if (product.Price < 0 || product.Quantity < 0)
                throw new ArgumentException("Precio y cantidad deben ser positivos.");

            await _productRepository.UpdateAsync(product);

            return true;
        }

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
