using System;
using System.Collections.Generic;
using Vysion.Entities;

namespace Vysion.Dtos
{
    public record SaleDetailDto
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid SellerId { get; set; }
        public decimal TotalSale { get; set; }
        public string PaymentMethod { get; set; }
        public string DeliveryInformation { get; set; }
        public string SaleNumber { get; set; }
        public string SaleNotes { get; set; }
        public decimal CommissionForSeller { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public Client ClientInfo { get; set; }
        public User SellerInfo { get; set; }
        public List<Product> ProductsInfos { get; set; }
    }
}