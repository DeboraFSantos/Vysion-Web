using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Vysion.Entities;

namespace Vysion.Dtos
{
    public record SaleDto
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
        public decimal CommissionForSellerValue { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
        public Client ClientInfo; 
        public User SellerInfo;
        public List<Product> ProductsInfos;
    }
}