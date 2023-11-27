using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        
        private const string databaseName = "vysion";
        private const string collectionName = "categories";
        private readonly IMongoCollection<Category> categoriesCollection;

        private readonly FilterDefinitionBuilder<Category> filterBuilder = Builders<Category>.Filter;
        public CategoriesRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            categoriesCollection = database.GetCollection<Category>(collectionName);
        }

        public void CreateCategory(Category category)
        {
            categoriesCollection.InsertOne(category);
        }

        public void DeleteCategory(Guid id)
        {
            var filter = filterBuilder.Eq(category => category.Id, id);
            categoriesCollection.DeleteOne(filter);
        }

        public async Task<Category> GetCategory(Guid id)
        {
            var filter = filterBuilder.Eq(product => product.Id, id);
            var category = await categoriesCollection.Find(filter).SingleOrDefaultAsync();

            return category;
        }

        public IEnumerable<Category> GetCategories()
        {
            return categoriesCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateCategory(Category category)
        {
            var filter = filterBuilder.Eq(existingCategory => existingCategory.Id, category.Id);
            categoriesCollection.ReplaceOne(filter, category);
        }

        internal Task GetCategory(string email)
        {
            throw new NotImplementedException();
        }
    }
}