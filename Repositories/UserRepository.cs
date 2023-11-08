using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        
        private const string databaseName = "vysion";
        private const string collectionName = "users";
        private readonly IMongoCollection<User> usersCollection;

        private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;
        public UsersRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            usersCollection = database.GetCollection<User>(collectionName);
        }

        public void CreateUser(User user)
        {
            usersCollection.InsertOne(user);
        }

        public void DeleteUser(Guid id)
        {
            var filter = filterBuilder.Eq(user => user.Id, id);
            usersCollection.DeleteOne(filter);
        }

        public User GetUser(Guid id)
        {
            var filter = filterBuilder.Eq(user => user.Id, id);
            return usersCollection.Find(filter).SingleOrDefault();
        }

        public IEnumerable<User> GetUsers()
        {
            return usersCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateUser(User user)
        {
            var filter = filterBuilder.Eq(existingUser => existingUser.Id, user.Id);
            usersCollection.ReplaceOne(filter, user);
        }

        internal Task GetUser(string email)
        {
            throw new NotImplementedException();
        }
    }
}