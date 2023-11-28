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
    [Route("paymentMethods")]
    public class PaymentMethodsController : ControllerBase
    {   
        private readonly IPaymentMethodsRepository repository;

        public PaymentMethodsController(IPaymentMethodsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetPaymentMethods([FromQuery] PaginationParams paginationParams)
        {
            var paymentMethods = repository.GetPaymentMethods();

            paymentMethods = paymentMethods.OrderByDescending(p => p.CreatedDate);

            var totalItems = paymentMethods.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedPaymentMethods = paymentMethods
            .Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(paymentMethod => paymentMethod.AsDto());

            var response = new
            {
                Total = totalItems,
                PageSize = paginationParams.PageSize,
                CurrentPage = paginationParams.CurrentPage,
                TotalPages = totalPages,
                PaymentMethods = pagedPaymentMethods
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
         public async Task<IActionResult> GetPaymentMethod(Guid id)
        {
            var paymentMethodInfo = await repository.GetPaymentMethod(id);

            PaymentMethod paymentMethod = new PaymentMethod
            {
                Id = paymentMethodInfo.Id,
                Name = paymentMethodInfo.Name,
                CreatedDate = paymentMethodInfo.CreatedDate,
            };

             return Ok(paymentMethod);
        }

        // POST /paymentMethod
        [HttpPost]
        [Authorize]
        public ActionResult<PaymentMethodDto> CreatePaymentMethod(CreatePaymentMethodDto paymentMethodDto)
        {
            PaymentMethod paymentMethod = new()
            {
                Id = Guid.NewGuid(),
                Name = paymentMethodDto.Name,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreatePaymentMethod(paymentMethod);

            return CreatedAtAction(nameof(GetPaymentMethod), new { id = paymentMethod.Id }, paymentMethod.AsDto());
        }
        
        // PUT /paymentMethods/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdatePaymentMethod(Guid id, UpdatePaymentMethodDto paymentMethodDto)
        {
            var existingPaymentMethod = await repository.GetPaymentMethod(id);

            if (existingPaymentMethod is null)
            {
                return NotFound();
            }

            PaymentMethod updatedPaymentMethod = existingPaymentMethod with
            {
                Name = paymentMethodDto.Name,
            };

            repository.UpdatePaymentMethod(updatedPaymentMethod);

            return NoContent();
        }

        // Deleete /paymentMethods/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeletePaymentMethod(Guid id){

            var existingPaymentMethod = repository.GetPaymentMethod(id);

            if(existingPaymentMethod is null){
                return NotFound();
            }

            repository.DeletePaymentMethod(id);

            return NoContent();
        }
    }
}