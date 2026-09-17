using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudPractica.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name {  get; set; }
        public string Description { get; set; }    
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ICollection<Category> Categories { get; set; }
        = new List<Category>();

    }
}
