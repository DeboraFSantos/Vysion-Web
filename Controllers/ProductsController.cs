using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Vysion.Dtos;
using Vysion.Entities;
using Vysion.Helpers;
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
        public IActionResult GetProducts([FromQuery] PaginationParams paginationParams)
        {
            var products = repository.GetProducts();

            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedProducts = products
            .Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(product => product.AsProductDetailDto());

            var response = new
            {
                Total = totalItems,
                PageSize = paginationParams.PageSize,
                CurrentPage = paginationParams.CurrentPage,
                TotalPages = totalPages,
                Products = pagedProducts
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
         public async Task<IActionResult> GetProduct(Guid id)
        {
            var productInfo = await repository.GetProduct(id);

            Product product = new Product
            {
                Id = productInfo.Id,
                Name = productInfo.Name,
                Description = productInfo.Description,
                Category = productInfo.Category,
                SKU = productInfo.SKU,
                Price = productInfo.Price,
                IsActive = productInfo.IsActive,
                ImageUrl = productInfo.ImageUrl,
                Discount = productInfo.Discount,
                CreatedDate = productInfo.CreatedDate,
            };

             return Ok(product);
        }

        // POST /products
        [HttpPost]
        [Authorize]
        public ActionResult<ProductDto> CreateProduct(CreateProductDto productDto)
        {
            Product product = new()
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId,
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
        [Authorize]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto productDto)
        {
            var existingProduct = await repository.GetProduct(id);

            if (existingProduct is null)
            {
                return NotFound();
            }

            Product updatedProduct = existingProduct with
            {
                Name = productDto.Name,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId,
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

        [HttpGet("export")]
        [Authorize]
        public IActionResult ExportProductsToExcel()
        {
            var products = repository.GetProducts();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Products");

                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Nome";
                worksheet.Cells["C1"].Value = "Descrição";
                worksheet.Cells["D1"].Value = "Categoria";
                worksheet.Cells["E1"].Value = "SKU";
                worksheet.Cells["F1"].Value = "Preço";
                worksheet.Cells["G1"].Value = "Ativo";
                worksheet.Cells["H1"].Value = "URL da Imagem";
                worksheet.Cells["I1"].Value = "Desconto";
                worksheet.Cells["J1"].Value = "Data de Criação";

                int row = 2;

                foreach (var product in products)
                {
                    worksheet.Cells[$"A{row}"].Value = product.Id;
                    worksheet.Cells[$"B{row}"].Value = product.Name;
                    worksheet.Cells[$"C{row}"].Value = product.Description;
                    worksheet.Cells[$"D{row}"].Value = product.Category;
                    worksheet.Cells[$"E{row}"].Value = product.SKU;
                    worksheet.Cells[$"F{row}"].Value = product.Price;
                    worksheet.Cells[$"G{row}"].Value = product.IsActive;
                    worksheet.Cells[$"H{row}"].Value = product.ImageUrl;
                    worksheet.Cells[$"I{row}"].Value = product.Discount;
                    worksheet.Cells[$"J{row}"].Value = product.CreatedDate;

                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());

                Response.Headers.Add("Content-Disposition", "attachment; filename=products.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}