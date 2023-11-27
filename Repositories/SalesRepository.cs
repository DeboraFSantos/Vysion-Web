using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private const string databaseName = "vysion";
        private const string collectionName = "sales";
        private readonly IMongoCollection<Sale> salesCollection;
        private readonly IMongoCollection<Client> clientsCollection;
        private readonly IMongoCollection<User> usersCollection;
        private readonly IMongoCollection<Product> productsCollection;

        private readonly FilterDefinitionBuilder<Sale> filterBuilder = Builders<Sale>.Filter;

        public SalesRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            salesCollection = database.GetCollection<Sale>(collectionName);
            clientsCollection = database.GetCollection<Client>("clients");
            usersCollection = database.GetCollection<User>("users");
            productsCollection = database.GetCollection<Product>("products");
        }

        public async Task<Client> GetClientInfo(Guid clientId)
        {
            return await clientsCollection.Find(c => c.Id == clientId).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserInfo(Guid userId)
        {
            return await usersCollection.Find(user => user.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProductInfos(List<Guid> productIds)
        {
            if (productIds == null || productIds.Count == 0)
            {
                return new List<Product>();
            }

            var filter = Builders<Product>.Filter.In(p => p.Id, productIds);
            var products = await productsCollection.Find(filter).ToListAsync();

            return products;
        }

        public void CreateSale(Sale sale)
        {
            salesCollection.InsertOne(sale);
        }

        public void DeleteSale(Guid id)
        {
            var filter = filterBuilder.Eq(sale => sale.Id, id);
            salesCollection.DeleteOne(filter);
        }

        public async Task<decimal> CalculateTotalSale(List<Guid> productIds)
        {

            var filter = Builders<Product>.Filter.In(p => p.Id, productIds);
            var products = await productsCollection.Find(filter).ToListAsync();

            decimal totalSale = 0;

            foreach (var product in products)
            {
                totalSale += product.Price;
            }

            return totalSale;
        }


        public async Task<Sale> GetSale(Guid id)
        {
            var filter = filterBuilder.Eq(sale => sale.Id, id);
            var sale = await salesCollection.Find(filter).SingleOrDefaultAsync();

            if (sale != null)
            {
                sale.ClientInfo = await GetClientInfo(sale.ClientId);
                sale.SellerInfo = await GetUserInfo(sale.SellerId);
                sale.TotalSale = sale.TotalSale;
                sale.ProductsInfos = await GetProductInfos(sale.Products);
            }

            return sale;
        }

        public IEnumerable<Sale> GetSales()
        {
            var sales = salesCollection.Find(new BsonDocument()).ToList();
            
            foreach (var sale in sales)
            {
                sale.ClientInfo = GetClientInfo(sale.ClientId).Result;
                sale.SellerInfo = GetUserInfo(sale.SellerId).Result;
                sale.ProductsInfos = GetProductInfos(sale.Products).Result;
                sale.TotalSale = sale.TotalSale;
            }

            return sales;
        }

        public void UpdateSale(Sale sale)
        {
            var filter = filterBuilder.Eq(existingSale => existingSale.Id, sale.Id);
            salesCollection.ReplaceOne(filter, sale);
        }
    }
}
