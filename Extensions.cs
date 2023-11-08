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

        public static UserDto AsDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Cpf = user.Cpf,
                Phone = user.Phone,
                CreatedDate = user.CreatedDate
            };
        }

        public static CategoryDto AsDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                Slug = category.Slug,
                CreatedDate = category.CreatedDate
            };
        }

        public static ClientDto AsDto(this Client client)
        {
            return new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Document = client.Document,
                Phone = client.Phone,
                Address = client.Address,
                CreatedDate = client.CreatedDate
            };
        }
    }
}