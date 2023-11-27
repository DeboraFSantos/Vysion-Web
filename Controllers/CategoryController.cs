using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("categories")]
    public class CategoriesController : ControllerBase
    {   
        private readonly ICategoriesRepository repository;

        public CategoriesController(ICategoriesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetCategories([FromQuery] PaginationParams paginationParams)
        {
            var categories = repository.GetCategories();

            categories = categories.OrderByDescending(p => p.CreatedDate);

            var totalItems = categories.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedCategories = categories
            .Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(category => category.AsDto());

            var response = new
            {
                Total = totalItems,
                PageSize = paginationParams.PageSize,
                CurrentPage = paginationParams.CurrentPage,
                TotalPages = totalPages,
                Categories = pagedCategories
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
         public async Task<IActionResult> GetCategory(Guid id)
        {
            var categoryInfo = await repository.GetCategory(id);

            Category category = new Category
            {
                Id = categoryInfo.Id,
                Name = categoryInfo.Name,
                Description = categoryInfo.Description,
                IsActive = categoryInfo.IsActive,
                Slug = categoryInfo.Slug,
                CreatedDate = categoryInfo.CreatedDate,
            };

             return Ok(category);
        }

        // POST /categories
        [HttpPost]
        [Authorize]
        public ActionResult<CategoryDto> CreateCategory(CreateCategoryDto categoryDto)
        {
            Category category = new()
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive,
                Slug = categoryDto.Slug,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateCategory(category);

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category.AsDto());
        }
        
        // PUT /categories/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateCategory(Guid id, UpdateCategoryDto categoryDto)
        {
            var existingClient = await repository.GetCategory(id);

            if (existingClient is null)
            {
                return NotFound();
            }

            Category updatedCategory = existingClient with
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive,
                Slug = categoryDto.Slug
            };

            repository.UpdateCategory(updatedCategory);

            return NoContent();
        }

        // Deleete /categories/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteCategory(Guid id){

            var existingCategory = repository.GetCategory(id);

            if(existingCategory is null){
                return NotFound();
            }

            repository.DeleteCategory(id);

            return NoContent();
        }
    }
}