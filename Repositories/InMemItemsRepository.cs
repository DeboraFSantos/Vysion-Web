
using System;
using System.Collections.Generic;
using System.Linq;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class InMemItemsRepository : IItemsRepository
    {
        private readonly List<Product> products = new()
        {
            new Product { Id = Guid.NewGuid(), Name = "Serviço 1", Description = "Divisórias", Price = 12, SKU = "123", CreatedDate = System.DateTimeOffset.UtcNow },
            new Product { Id = Guid.NewGuid(), Name = "Serviço 2", Description = "Forros", Price = 10, SKU = "1234", CreatedDate = DateTimeOffset.UtcNow },
            new Product { Id = Guid.NewGuid(), Name = "Serviço 3", Description = "Drywall", Price = 9, SKU = "1235", CreatedDate = DateTimeOffset.UtcNow }
        };

        public IEnumerable<Product> GetProducts()
        {
            return products;
        }

        public Product GetProduct(Guid id)
        {
            return products.Where(product => product.Id == id).SingleOrDefault();
        }

        public void CreateProduct(Product product)
        {
            products.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            var index = products.FindIndex(existingProduct => existingProduct.Id == product.Id);
            products[index] = product;
        }

        public void DeleteProduct(Guid id)
        {
            var index = products.FindIndex(existingProduct => existingProduct.Id == id);
            products.RemoveAt(index);
        }
    }
}