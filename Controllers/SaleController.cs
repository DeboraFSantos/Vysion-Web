using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IProductsRepository productsRepository;
        private readonly IClientsRepository clientsRepository;
        private readonly ICategoriesRepository categoriesRepository;

        public SalesController(ISalesRepository repository,  IProductsRepository productsRepository, IClientsRepository clientsRepository, ICategoriesRepository categoriesRepository)
        {
            this.repository = repository;
            this.productsRepository = productsRepository;
            this.clientsRepository = clientsRepository;
            this.categoriesRepository = categoriesRepository;
        }

        [HttpGet]
        public IActionResult GetSales([FromQuery] PaginationParams paginationParams, string clientName = "", string sellerName = "", string paymentMethod = "")
        {
            var sales = repository.GetSales();

            if (!string.IsNullOrWhiteSpace(clientName))
            {
                var formattedClientName = clientName.ToLower();
                sales = sales.Where(s => s.ClientInfo != null && s.ClientInfo.Name.ToLower().Contains(formattedClientName));
            }

            if (!string.IsNullOrWhiteSpace(sellerName))
            {
                var formattedSellerName = sellerName.ToLower();
                sales = sales.Where(s => s.SellerInfo != null && s.SellerInfo.Name.ToLower().Contains(formattedSellerName));
            }

            if (!string.IsNullOrWhiteSpace(paymentMethod))
            {
                var formattedPaymentMethod = paymentMethod.ToLower();
                sales = sales.Where(s => s.PaymentMethod.ToLower().Contains(formattedPaymentMethod));
            }

            sales = sales.OrderByDescending(p => p.CreatedDate);

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
        [Authorize]
        public async Task<IActionResult> GetSale(Guid id)
        {
            var saleInfo = await repository.GetSale(id);

            Sale sale = new Sale
            {
                Id = saleInfo.Id,
                ClientId = saleInfo.ClientId,
                SellerId = saleInfo.SellerId,
                TotalSale = saleInfo.TotalSale,
                PaymentMethod = saleInfo.PaymentMethod,
                DeliveryInformation = saleInfo.DeliveryInformation,
                SaleNumber = saleInfo.SaleNumber,
                SaleNotes = saleInfo.SaleNotes,
                CommissionForSeller = saleInfo.CommissionForSeller,
                CommissionForSellerValue = saleInfo.CommissionForSellerValue,
                CreatedDate = DateTimeOffset.UtcNow,
                ClientInfo = saleInfo.ClientInfo,
                SellerInfo = saleInfo.SellerInfo,
                ProductsInfos = saleInfo.ProductsInfos,
                ProductQuantity = saleInfo.ProductQuantity
            };

             return Ok(sale);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSale(CreateSaleDto saleDto)
        {
            var productInfos = await repository.GetProductInfos(saleDto.Products);

            decimal totalSale = 0;
            var productQuantities = saleDto.ProductQuantity;

            foreach (var (productId, quantity) in productQuantities)
            {
                var product = productInfos.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    decimal pricePerThousandUnits = product.Price;
                    decimal pricePerUnit = pricePerThousandUnits / 1000; 

                    decimal priceInCents = pricePerUnit * quantity * 10 * 100;

                    totalSale += priceInCents;
                }
            }

            decimal commissionAmount = 0;

            if (saleDto.CommissionForSeller >= 0 && saleDto.CommissionForSeller <= 100)
            {
                commissionAmount = totalSale * saleDto.CommissionForSeller / 100;
            }

            Sale sale = new Sale
            {
                Id = Guid.NewGuid(),
                ClientId = saleDto.ClientId,
                SellerId = saleDto.SellerId,
                Products = saleDto.Products,
                PaymentMethod = saleDto.PaymentMethod,
                DeliveryInformation = saleDto.DeliveryInformation,
                SaleNotes = saleDto.SaleNotes,
                CommissionForSeller = saleDto.CommissionForSeller,
                CommissionForSellerValue = commissionAmount,
                ProductQuantity = saleDto.ProductQuantity,
                TotalSale = totalSale, 
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateSale(sale);

            return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale.AsDto());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSale(Guid id, UpdateSaleDto saleDto)
        {
            var existingSale = await repository.GetSale(id);

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
            existingSale.ProductQuantity = saleDto.ProductQuantity;

            var productInfos = await repository.GetProductInfos(saleDto.Products);
            decimal totalSale = 0;
            var productQuantities = saleDto.ProductQuantity;

            foreach (var (productId, quantity) in productQuantities)
            {
                var product = productInfos.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    decimal pricePerThousandUnits = product.Price;
                    decimal pricePerUnit = pricePerThousandUnits / 1000;

                    decimal priceInCents = pricePerUnit * quantity * 10 * 100;

                    totalSale += priceInCents;
                }
            }

            existingSale.TotalSale = totalSale;

            decimal commissionAmount = 0;

            if (saleDto.CommissionForSeller >= 0 && saleDto.CommissionForSeller <= 100)
            {
                commissionAmount = totalSale * saleDto.CommissionForSeller / 100;
            }

            existingSale.CommissionForSellerValue = commissionAmount;

            repository.UpdateSale(existingSale);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
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

        [HttpGet("totalSalesStatistics")]
        public IActionResult GetTotalSalesStatistics(DateTime startDate, DateTime endDate)
        {
            var sales = repository.GetSales()
                .Where(s => s.CreatedDate.Date >= startDate.Date && s.CreatedDate.Date <= endDate.Date)
                .GroupBy(s => s.CreatedDate.Date)
                .Select(group => new
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    TotalSales = group.Count()
                })
                .ToDictionary(item => item.Date, item => item.TotalSales);

            return Ok(sales);
        }

        [HttpGet("topSellerRanking")]
        public IActionResult GetTopSellerRanking(DateTime startDate, DateTime endDate)
        {
            var salesWithinRange = repository.GetSales()
                .Where(s => s.CreatedDate.Date >= startDate.Date && s.CreatedDate.Date <= endDate.Date)
                .Where(s => s.SellerInfo != null && s.SellerInfo.Name != null)
                .GroupBy(s => new { s.SellerId, s.SellerInfo.Name }) 
                .Select(group => new
                {
                    SellerId = group.Key.SellerId,
                    SellerName = group.Key.Name,
                    TotalSales = group.Count() 
                })
                .OrderByDescending(s => s.TotalSales)
                .Take(5) 
                .ToList();

            return Ok(salesWithinRange);
        }

        [HttpGet("topSoldProducts")]
        public async Task<IActionResult> GetTopSoldProducts(DateTime startDate, DateTime endDate)
        {
            var salesData = repository.GetSales()
                .Where(s => s.CreatedDate.Date >= startDate.Date && s.CreatedDate.Date <= endDate.Date)
                .Where(s => s.Products != null)
                .SelectMany(s => s.Products ?? Enumerable.Empty<Guid>())
                .GroupBy(productId => productId)
                .Select(async group =>
                {
                    var productInfo = await productsRepository.GetProduct(group.Key);
                    var productName = productInfo?.Name ?? "Unknown";

                    return new
                    {
                        ProductId = group.Key,
                        ProductName = productName,
                        TotalQuantitySold = group.Count()
                    };
                })
                .ToList();

            var result = new List<object>();

            foreach (var task in salesData)
            {
                var productInfo = await task;
                result.Add(productInfo);
            }

            var orderedResult = result.OrderByDescending(p => ((dynamic)p).TotalQuantitySold).ToList();
            
            return Ok(orderedResult);
        }

        [HttpGet("topBuyingClients")]
        public async Task<IActionResult> GetTopBuyingClients(DateTime startDate, DateTime endDate)
        {
            var salesData = repository.GetSales()
                .Where(s => s.CreatedDate.Date >= startDate.Date && s.CreatedDate.Date <= endDate.Date)
                .GroupBy(s => s.ClientId)
                .Select(group => new
                {
                    ClientId = group.Key,
                    TotalPurchases = group.Count()
                })
                .ToList();

            var clientsInfo = new List<dynamic>();

            foreach (var sale in salesData)
            {
                var clientInfo = await clientsRepository.GetClient(sale.ClientId); 
                var clientName = clientInfo?.Name ?? "Unknown";

                clientsInfo.Add(new
                {
                    ClientId = sale.ClientId,
                    ClientName = clientName,
                    TotalPurchases = sale.TotalPurchases
                });
            }

            var orderedClients = clientsInfo.OrderByDescending(c => c.TotalPurchases).ToList();

            return Ok(orderedClients);
        }

        private async Task<Category> GetCategoryInfo(Guid categoryId)
        {
            try
            {
                var category = await categoriesRepository.GetCategory(categoryId);
                return category;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("topSoldCategories")]
        public async Task<IActionResult> GetTopSoldCategories(DateTime startDate, DateTime endDate)
        {
            var salesData = repository.GetSales()
                .Where(s => s.CreatedDate.Date >= startDate.Date && s.CreatedDate.Date <= endDate.Date)
                .SelectMany(s => s.ProductsInfos != null ? s.ProductsInfos : new List<Product>())
                .GroupBy(productInfo => productInfo.CategoryId)
                .Select(async group =>
                {
                    var categoryInfo = await GetCategoryInfo(group.Key);
                    var categoryName = categoryInfo?.Name ?? "Unknown";

                    return new
                    {
                        CategoryId = group.Key,
                        CategoryName = categoryName,
                        TotalQuantitySold = group.Count()
                    };
                })
                .ToList();

            var result = new List<object>();

            foreach (var task in salesData)
            {
                var categoryInfo = await task;
                result.Add(categoryInfo);
            }

            var orderedResult = result.OrderByDescending(c => ((dynamic)c).TotalQuantitySold).ToList();

            return Ok(orderedResult);
        }

        
        [HttpGet("export")]
        [Authorize]
        public IActionResult ExportProductsToExcel()
        {
            var sales = repository.GetSales();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sales");

                worksheet.Cells["A1"].Value = "ID do cliente";
                worksheet.Cells["B1"].Value = "ID do vendedor";
                worksheet.Cells["C1"].Value = "ID dos produtos";
                worksheet.Cells["D1"].Value = "Total da venda";
                worksheet.Cells["E1"].Value = "Tipo de pagamento";
                worksheet.Cells["F1"].Value = "Informações de entrega";
                worksheet.Cells["G1"].Value = "Número da venda";
                worksheet.Cells["H1"].Value = "Notas da venda";
                worksheet.Cells["I1"].Value = "Porcentagem da Comissão do vendedor";
                worksheet.Cells["I1"].Value = "Valor da Comissão do vendedor";
                worksheet.Cells["J1"].Value = "Data de Criação";

                int row = 2;

                foreach (var sale in sales)
                {
                    worksheet.Cells[$"A{row}"].Value = sale.ClientId;
                    worksheet.Cells[$"B{row}"].Value = sale.SellerId;
                    worksheet.Cells[$"C{row}"].Value = sale.Products;
                    worksheet.Cells[$"D{row}"].Value = sale.TotalSale;
                    worksheet.Cells[$"E{row}"].Value = sale.PaymentMethod;
                    worksheet.Cells[$"F{row}"].Value = sale.DeliveryInformation;
                    worksheet.Cells[$"G{row}"].Value = sale.SaleNumber;
                    worksheet.Cells[$"H{row}"].Value = sale.SaleNotes;
                    worksheet.Cells[$"I{row}"].Value = sale.CommissionForSeller;
                    worksheet.Cells[$"J{row}"].Value = sale.CommissionForSellerValue;
                    worksheet.Cells[$"K{row}"].Value = sale.CreatedDate;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());

                Response.Headers.Add("Content-Disposition", "attachment; filename=sales.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}
