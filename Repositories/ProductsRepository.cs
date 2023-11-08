using System;
using System.Collections.Generic;
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

        private readonly FilterDefinitionBuilder<Product> filterBuilder = Builders<Product>.Filter;
        public ProductsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            productsCollection = database.GetCollection<Product>(collectionName);
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

        public Product GetProduct(Guid id)
        {
            var filter = filterBuilder.Eq(product => product.Id, id);
            return productsCollection.Find(filter).SingleOrDefault();
        }

        public IEnumerable<Product> GetProducts()
        {
            return productsCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateProduct(Product product)
        {
            var filter = filterBuilder.Eq(existingProduct => existingProduct.Id, product.Id);
            productsCollection.ReplaceOne(filter, product);
        }
    }
}