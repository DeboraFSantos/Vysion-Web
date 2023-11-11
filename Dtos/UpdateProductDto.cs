using System;
using System.ComponentModel.DataAnnotations;
using Vysion.Entities;

namespace Vysion.Dtos
{
    public record UpdateProductDto {
        [Required]
        public string Name { get; init; }
        public string Description { get; init; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public string SKU { get; init; }
        public int StockQuantity { get; init; }
        [Required]
        public decimal Price { get; init; }
        public bool IsActive { get; init; }
        public string ImageUrl { get; init; }
        public decimal Discount { get; init; }
        public Category Category;
    }
}