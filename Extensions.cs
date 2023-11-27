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
                CategoryId = product.CategoryId,
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

        public static PaymentMethodDto AsDto(this PaymentMethod paymentMethodDto)
        {
            return new PaymentMethodDto
            {
                Id = paymentMethodDto.Id,
                Name = paymentMethodDto.Name,
                CreatedDate = paymentMethodDto.CreatedDate
            };
        }

        public static SaleDto AsDto(this Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                ClientId = sale.ClientId,
                SellerId = sale.SellerId,
                Products = sale.Products,
                TotalSale = sale.TotalSale,
                PaymentMethod = sale.PaymentMethod,
                DeliveryInformation = sale.DeliveryInformation,
                SaleNotes = sale.SaleNotes,
                CommissionForSeller = sale.CommissionForSeller,
                CommissionForSellerValue = sale.CommissionForSellerValue,
                CreatedDate = sale.CreatedDate
            };
        }

        public static CreateSaleDto AsCreateDto(this Sale sale)
        {
            return new CreateSaleDto
            {
                ClientId = sale.ClientId,
                SellerId = sale.SellerId,
                Products = sale.Products,
                PaymentMethod = sale.PaymentMethod,
                DeliveryInformation = sale.DeliveryInformation,
                SaleNotes = sale.SaleNotes,
                CommissionForSeller = sale.CommissionForSeller
            };
        }

        public static SaleDetailDto AsDetailDto(this Sale sale)
        {
            return new SaleDetailDto
            {
                Id = sale.Id,
                ClientId = sale.ClientId,
                SellerId = sale.SellerId,
                TotalSale = sale.TotalSale,
                PaymentMethod = sale.PaymentMethod,
                DeliveryInformation = sale.DeliveryInformation,
                SaleNumber = sale.SaleNumber,
                SaleNotes = sale.SaleNotes,
                CommissionForSeller = sale.CommissionForSeller,
                CreatedDate = sale.CreatedDate,
                ClientInfo = sale.ClientInfo,
                SellerInfo = sale.SellerInfo,
                ProductsInfos = sale.ProductsInfos
            };
        }

        public static ProductDetailDto AsProductDetailDto(this Product product)
        {
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                ImageUrl = product.ImageUrl,
                Discount = product.Discount,
                CreatedDate = product.CreatedDate,
                Category = product.Category
            };
        }
    }
}