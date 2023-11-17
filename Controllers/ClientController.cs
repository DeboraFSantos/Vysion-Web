using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [Route("clients")]
    public class ClientsController : ControllerBase
    {   
        private readonly IClientsRepository repository;

        public ClientsController(IClientsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetClients([FromQuery] PaginationParams paginationParams)
        {
             var clients = repository.GetClients();

            var totalItems = clients.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedClients = clients
            .Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(client => client.AsDto());

            var response = new
            {
                Total = totalItems,
                PageSize = paginationParams.PageSize,
                CurrentPage = paginationParams.CurrentPage,
                TotalPages = totalPages,
                Clients = pagedClients
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<ClientDto> GetClient(Guid id)
        {
            var client = repository.GetClient(id);

            if(client is null)
            {
                return NotFound();
            }

            return client.AsDto();
        }

        // POST /clients
        [HttpPost]
        [Authorize]
        public ActionResult<ClientDto> CreateClient(CreateClientDto clientDto)
        {
            Client client = new()
            {
                Id = Guid.NewGuid(),
                Name = clientDto.Name,
                Email = clientDto.Email,
                Document = clientDto.Document,
                Phone = clientDto.Phone,
                Address = clientDto.Address,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateClient(client);

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client.AsDto());
        }
        
        // PUT /clients/{id}
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult UpdateClient(Guid id, UpdateClientDto clientDto)
        {
            var existingClient = repository.GetClient(id);

            if(existingClient is null){
                return NotFound();
            }

            Client updatedClient = existingClient with {
                Name = clientDto.Name,
                Email = clientDto.Email,
                Document = clientDto.Document,
                Phone = clientDto.Phone,
                Address = clientDto.Address,
            };

            repository.UpdateClient(updatedClient);

            return NoContent();
        }

        // Deleete /clients/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteCleint(Guid id){

            var existingClient = repository.GetClient(id);

            if(existingClient is null){
                return NotFound();
            }

            repository.DeleteClient(id);

            return NoContent();
        }

        [HttpGet("export")]
        [Authorize]
        public IActionResult ExportProductsToExcel()
        {
            var clients = repository.GetClients();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clients");

                worksheet.Cells["A1"].Value = "Nome";
                worksheet.Cells["B1"].Value = "Email";
                worksheet.Cells["C1"].Value = "Documento";
                worksheet.Cells["D1"].Value = "Telefone";
                worksheet.Cells["E1"].Value = "Endereço";
                worksheet.Cells["F1"].Value = "Data de Criação";

                int row = 2;

                foreach (var client in clients)
                {
                    worksheet.Cells[$"A{row}"].Value = client.Name;
                    worksheet.Cells[$"B{row}"].Value = client.Email;
                    worksheet.Cells[$"C{row}"].Value = client.Document;
                    worksheet.Cells[$"D{row}"].Value = client.Phone;
                    worksheet.Cells[$"E{row}"].Value = client.Address;
                    worksheet.Cells[$"F{row}"].Value = client.CreatedDate;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());

                Response.Headers.Add("Content-Disposition", "attachment; filename=clients.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}