using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Vysion.Entities
{
    public record Sale
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid SellerId { get; set; }
        public List<Guid> Products { get; set; }
        public decimal TotalSale { get; set; }
        public string PaymentMethod { get; set; }
        public string DeliveryInformation { get; set; }
        public string SaleNumber { get; set; }
        public string SaleNotes { get; set; }
        public decimal CommissionForSeller { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
        public Client ClientInfo { get; internal set; }
        public User SellerInfo { get; internal set; }
        public List<Product> ProductsInfos { get; internal set; }
    }
}
