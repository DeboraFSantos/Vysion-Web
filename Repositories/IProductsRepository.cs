using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface IProductsRepository
    {
        Task<Product> GetProduct(Guid id);
        IEnumerable<Product> GetProducts();

        void CreateProduct(Product product);
        void UpdateProduct (Product product);
        void DeleteProduct (Guid id);
    }
}