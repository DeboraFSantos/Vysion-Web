using Vysion.Dtos;
using Vysion.Entities;

namespace Vysion
{
    public static class Extensions
    {
        public static ProductDto AsDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                Discount = product.Discount,
                CreatedDate = product.CreatedDate
            };
        }
    }
}