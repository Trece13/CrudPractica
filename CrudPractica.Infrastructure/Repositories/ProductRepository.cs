using CrudPractica.Application.Common;
using CrudPractica.Application.Interfaces;
using CrudPractica.Domain.Entities;
using CrudPractica.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudPractica.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;

        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                            .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            var porductExist = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (porductExist == null)
                return;

            porductExist.Name = product.Name;
            porductExist.Description = product.Description;
            porductExist.Price = product.Price;
            porductExist.Quantity = product.Quantity;
            await _context.SaveChangesAsync();  
        }

        public async Task<PagedResult<Product>> GetPagedAsync(
        string? search,
        int page,
        int pageSize)
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking();

            // BÚSQUEDA
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Description.Contains(search));
            }

            // Contamos DESPUÉS del filtro
            var totalCount = await query.CountAsync();

            // PAGINAMOS
            var items = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
