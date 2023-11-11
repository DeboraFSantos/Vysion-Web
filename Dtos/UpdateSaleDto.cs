using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Vysion.Dtos
{
    public record UpdateSaleDto
    {
        public Guid ClientId { get; set; }
        public Guid SellerId { get; set; }
        public List<Guid> Products { get; set; }
        public string PaymentMethod { get; set; }
        public string DeliveryInformation { get; set; }
        public string SaleNumber { get; set; }
        public string SaleNotes { get; set; }
        public decimal CommissionForSeller { get; set; }
        
    }
}