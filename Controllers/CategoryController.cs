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
        public ActionResult<CategoryDto> GetCategory(Guid id)
        {
            var category = repository.GetCategory(id);

            if(category is null)
            {
                return NotFound();
            }

            return category.AsDto();
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
        public ActionResult UpdateCategory(Guid id, UpdateCategoryDto categoryDto)
        {
            var existingCategory = repository.GetCategory(id);

            if(existingCategory is null){
                return NotFound();
            }

            Category updatedCategory = existingCategory with {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = categoryDto.IsActive,
                Slug = categoryDto.Slug,
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