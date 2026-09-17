using CrudPractica.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudPractica.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<List<Category>> GetByIdsAsync(
            IEnumerable<int> ids);
        Task<Category> CreateAsync(Category category);
    }
}
