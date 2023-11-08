using System;
using System.Collections.Generic;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface IItemsRepository
    {
        Product GetProduct(Guid id);
        IEnumerable<Product> GetProducts();

        void CreateProduct(Product product);
        void UpdateProduct (Product product);
        void DeleteProduct (Guid id);
    }
}