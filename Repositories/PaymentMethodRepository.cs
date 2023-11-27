using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public class PaymentMethodsRepository : IPaymentMethodsRepository
    {
        
        private const string databaseName = "vysion";
        private const string collectionName = "paymentMethods";
        private readonly IMongoCollection<PaymentMethod> paymentMethodsCollection;

        private readonly FilterDefinitionBuilder<PaymentMethod> filterBuilder = Builders<PaymentMethod>.Filter;
        public PaymentMethodsRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            paymentMethodsCollection = database.GetCollection<PaymentMethod>(collectionName);
        }

        public void CreatePaymentMethod(PaymentMethod paymentMethod)
        {
            paymentMethodsCollection.InsertOne(paymentMethod);
        }

        public void DeletePaymentMethod(Guid id)
        {
            var filter = filterBuilder.Eq(paymentMethod => paymentMethod.Id, id);
            paymentMethodsCollection.DeleteOne(filter);
        }

        public async Task<PaymentMethod> GetPaymentMethod(Guid id)
        {
            var filter = filterBuilder.Eq(product => product.Id, id);
            var paymentMethod = await paymentMethodsCollection.Find(filter).SingleOrDefaultAsync();

            return paymentMethod;
        }
        public IEnumerable<PaymentMethod> GetPaymentMethods()
        {
            return paymentMethodsCollection.Find(new BsonDocument()).ToList();
        }

        public void UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            var filter = filterBuilder.Eq(existingPaymentMethod => existingPaymentMethod.Id, paymentMethod.Id);
            paymentMethodsCollection.ReplaceOne(filter, paymentMethod);
        }
    }
}