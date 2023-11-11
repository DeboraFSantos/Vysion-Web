using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult DeleteCleint(Guid id){

            var existingClient = repository.GetClient(id);

            if(existingClient is null){
                return NotFound();
            }

            repository.DeleteClient(id);

            return NoContent();
        }
    }
}