using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class ClientsRepository : IClientsRepository
    {
        
        private const string databaseName = "vysion";
        private const string collectionName = "clients";
        private readonly IMongoCollection<Client> clientsCollection;

        private readonly FilterDefinitionBuilder<Client> filterBuilder = Builders<Client>.Filter;
        public ClientsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            clientsCollection = database.GetCollection<Client>(collectionName);
        }

        public void CreateClient(Client client)
        {
            clientsCollection.InsertOne(client);
        }

        public void DeleteClient(Guid id)
        {
            var filter = filterBuilder.Eq(client => client.Id, id);
            clientsCollection.DeleteOne(filter);
        }

        public Client GetClient(Guid id)
        {
            var filter = filterBuilder.Eq(client => client.Id, id);
            return clientsCollection.Find(filter).SingleOrDefault();
        }

        public IEnumerable<Client> GetClients()
        {
            return clientsCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdateClient(Client client)
        {
            var filter = filterBuilder.Eq(existingClient => existingClient.Id, client.Id);
            clientsCollection.ReplaceOne(filter, client);
        }
    }
}