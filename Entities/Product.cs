using System;

namespace Vysion.Entities
{
    public record Product
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Category { get; init; }
        public string SKU { get; init; }
        public int StockQuantity { get; init; }
        public decimal Price { get; init; }
        public bool IsActive { get; init; }
        public string ImageUrl { get; init; }
        public decimal Discount { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
