using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Vysion.Dtos;
using Vysion.Entities;
using Vysion.Helpers;
using Vysion.Repositories;

namespace Vysion.Controllers
{
    [ApiController]
    [Route("sales")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesRepository repository;

        public SalesController(ISalesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult GetSales([FromQuery] PaginationParams paginationParams)
        {
            var sales = repository.GetSales();

            var totalItems = sales.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedSales = sales
            .Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(sale => sale.AsDetailDto());

            var response = new
            {
                Total = totalItems,
                PageSize = paginationParams.PageSize,
                CurrentPage = paginationParams.CurrentPage,
                TotalPages = totalPages,
                Sales = pagedSales
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSale(Guid id)
        {
            var saleInfo = await repository.GetSale(id);

            Sale sale = new Sale
            {
                Id = Guid.NewGuid(),
                ClientId = saleInfo.ClientId,
                SellerId = saleInfo.SellerId,
                TotalSale = saleInfo.TotalSale,
                PaymentMethod = saleInfo.PaymentMethod,
                DeliveryInformation = saleInfo.DeliveryInformation,
                SaleNumber = saleInfo.SaleNumber,
                SaleNotes = saleInfo.SaleNotes,
                CommissionForSeller = saleInfo.CommissionForSeller,
                CreatedDate = DateTimeOffset.UtcNow,
                ClientInfo = saleInfo.ClientInfo,
                SellerInfo = saleInfo.SellerInfo,
                ProductsInfos = saleInfo.ProductsInfos
            };

             return Ok(sale);
        }

        [HttpPost]
        public IActionResult CreateSale(CreateSaleDto saleDto)
        {
            Sale sale = new Sale
            {
                Id = Guid.NewGuid(),
                ClientId = saleDto.ClientId,
                SellerId = saleDto.SellerId,
                Products = saleDto.Products,
                PaymentMethod = saleDto.PaymentMethod,
                DeliveryInformation = saleDto.DeliveryInformation,
                SaleNumber = saleDto.SaleNumber,
                SaleNotes = saleDto.SaleNotes,
                CommissionForSeller = saleDto.CommissionForSeller,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateSale(sale);

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(Guid id, UpdateSaleDto saleDto)
        {
            var existingSale = await repository.GetSale(id); // Use await para esperar a conclusão da tarefa

            if (existingSale is null)
            {
                return NotFound();
            }

            existingSale.ClientId = saleDto.ClientId;
            existingSale.SellerId = saleDto.SellerId;
            existingSale.Products = saleDto.Products;
            existingSale.PaymentMethod = saleDto.PaymentMethod;
            existingSale.DeliveryInformation = saleDto.DeliveryInformation;
            existingSale.SaleNumber = saleDto.SaleNumber;
            existingSale.SaleNotes = saleDto.SaleNotes;
            existingSale.CommissionForSeller = saleDto.CommissionForSeller;

            repository.UpdateSale(existingSale);

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteSale(Guid id)
        {
            var existingSale = repository.GetSale(id);

            if (existingSale is null)
            {
                return NotFound();
            }

            repository.DeleteSale(id);

            return NoContent();
        }

        [HttpGet("export")]
        public IActionResult ExportProductsToExcel()
        {
            var sales = repository.GetSales();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sales");

                worksheet.Cells["A1"].Value = "ID do cliente";
                worksheet.Cells["B1"].Value = "ID do vendedor";
                worksheet.Cells["C1"].Value = "ID dos produtos";
                worksheet.Cells["D1"].Value = "Tipo de pagamento";
                worksheet.Cells["E1"].Value = "Informações de entrega";
                worksheet.Cells["F1"].Value = "Número da venda";
                worksheet.Cells["G1"].Value = "Notas da venda";
                worksheet.Cells["H1"].Value = "Comissão do vendedor";
                worksheet.Cells["I1"].Value = "Data de Criação";

                int row = 2;

                foreach (var sale in sales)
                {
                    worksheet.Cells[$"A{row}"].Value = sale.ClientId;
                    worksheet.Cells[$"B{row}"].Value = sale.SellerId;
                    worksheet.Cells[$"C{row}"].Value = sale.Products;
                    worksheet.Cells[$"D{row}"].Value = sale.PaymentMethod;
                    worksheet.Cells[$"E{row}"].Value = sale.DeliveryInformation;
                    worksheet.Cells[$"F{row}"].Value = sale.SaleNumber;
                    worksheet.Cells[$"G{row}"].Value = sale.SaleNotes;
                    worksheet.Cells[$"H{row}"].Value = sale.CommissionForSeller;
                    worksheet.Cells[$"I{row}"].Value = sale.CreatedDate;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());

                Response.Headers.Add("Content-Disposition", "attachment; filename=sales.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}
