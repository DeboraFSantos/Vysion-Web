using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Vysion.Dtos;
using Vysion.Entities;
using Vysion.Repositories;

namespace Vysion.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {   
        private readonly IProductsRepository repository;

        public ProductsController(IProductsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<ProductDto> GetProducts()
        {
            var products = repository.GetProducts().Select(product => product.AsDto());

            return products;
        }

        [HttpGet("{id}")]
        public ActionResult<ProductDto> GetProduct(Guid id)
        {
            var product = repository.GetProduct(id);

            if(product is null)
            {
                return NotFound();
            }

            return product.AsDto();
        }

        // POST /products
        [HttpPost]
        public ActionResult<ProductDto> CreateProduct(CreateProductDto productDto)
        {
            Product product = new()
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                Category = productDto.Category,
                SKU = productDto.SKU,
                Price = productDto.Price,
                IsActive = productDto.IsActive,
                ImageUrl = productDto.ImageUrl,
                Discount = productDto.Discount,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateProduct(product);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product.AsDto());
        }
        
        // PUT /products/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateProduct(Guid id, UpdateProductDto productDto)
        {
            var existingProduct = repository.GetProduct(id);

            if(existingProduct is null){
                return NotFound();
            }

            Product updatedProduct = existingProduct with {
                Name = productDto.Name,
                Description = productDto.Description,
                Category = productDto.Category,
                SKU = productDto.SKU,
                Price = productDto.Price,
                IsActive = productDto.IsActive,
                ImageUrl = productDto.ImageUrl,
                Discount = productDto.Discount
            };

            repository.UpdateProduct(updatedProduct);

            return NoContent();
        }

        // Deleete /products/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id){

            var existingProduct = repository.GetProduct(id);

            if(existingProduct is null){
                return NotFound();
            }

            repository.DeleteProduct(id);

            return NoContent();
        }
    }
}