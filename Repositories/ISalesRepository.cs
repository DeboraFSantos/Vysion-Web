using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface ISalesRepository
    {
        Task<Sale> GetSale(Guid id);

        Task<List<Product>> GetProductInfos(List<Guid> productIds);
        IEnumerable<Sale> GetSales();

        void CreateSale(Sale sale);
        void UpdateSale (Sale sale);
        void DeleteSale (Guid id);
    }
}