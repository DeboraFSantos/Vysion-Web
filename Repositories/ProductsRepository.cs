using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        
        private const string databaseName = "vysion";
        private const string collectionName = "products";
        private readonly IMongoCollection<Product> productsCollection;

        private readonly IMongoCollection<Category> categoriesCollection;

        private readonly FilterDefinitionBuilder<Product> filterBuilder = Builders<Product>.Filter;
        public ProductsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            productsCollection = database.GetCollection<Product>(collectionName);
            categoriesCollection = database.GetCollection<Category>("categories");
        }

        public void CreateProduct(Product product)
        {
            productsCollection.InsertOne(product);
        }

        public void DeleteProduct(Guid id)
        {
            var filter = filterBuilder.Eq(product => product.Id, id);
            productsCollection.DeleteOne(filter);
        }

        public async Task<Category> GetCategoryInfo(Guid categoryId)
        {
            return await categoriesCollection.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
        }

        public async Task<Product> GetProduct(Guid id)
        {
            var filter = filterBuilder.Eq(product => product.Id, id);
            var product = await productsCollection.Find(filter).SingleOrDefaultAsync();

            if (product != null)
            {
                product.Category = await GetCategoryInfo(product.CategoryId);
            }

            return product;
        }

        public IEnumerable<Product> GetProducts()
        {
            var products = productsCollection.Find(new BsonDocument()).ToList();
            
            foreach (var product in products)
            {
                product.Category = GetCategoryInfo(product.CategoryId).Result;
            }

            return products;
        }

        public void UpdateProduct(Product product)
        {
            var filter = filterBuilder.Eq(existingProduct => existingProduct.Id, product.Id);
            productsCollection.ReplaceOne(filter, product);
        }
    }
}