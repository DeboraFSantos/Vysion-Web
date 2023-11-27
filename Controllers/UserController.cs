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
    [Route("users")]
    public class UsersController : ControllerBase
    {   
        private readonly IUsersRepository repository;

        public UsersController(IUsersRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUsers([FromQuery] PaginationParams paginationParams)
        {
             var users = repository.GetUsers();

            users = users.OrderByDescending(p => p.CreatedDate);

            var totalItems = users.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedUsers = users
            .Skip((paginationParams.CurrentPage - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(User => User.AsDto());

            var response = new
            {
                Total = totalItems,
                PageSize = paginationParams.PageSize,
                CurrentPage = paginationParams.CurrentPage,
                TotalPages = totalPages,
                Users = pagedUsers
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<UserDto> GetUser(Guid id)
        {
            var user = repository.GetUser(id);

            if(user is null)
            {
                return NotFound();
            }

            return user.AsDto();
        }

        // POST /users
        [HttpPost]
        [Authorize]
        public ActionResult<UserDto> CreateUser(CreateUserDto userDto)
        {
            User user = new()
            {
                Id = Guid.NewGuid(),
                Name = userDto.Name,
                Email = userDto.Email,
                Role = userDto.Role,
                Cpf = userDto.Cpf,
                Phone = userDto.Phone,
                CreatedDate = DateTimeOffset.UtcNow
            };

            repository.CreateUser(user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.AsDto());
        }
        
        // PUT /users/{id}
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult UpdateUser(Guid id, UpdateUserDto userDto)
        {
            var existingUser = repository.GetUser(id);

            if(existingUser is null){
                return NotFound();
            }

            User updatedUser = existingUser with {
                Name = userDto.Name,
                Role = userDto.Role,
                Email = userDto.Email,
                Cpf = userDto.Cpf,
                Phone = userDto.Phone
            };

            repository.UpdateUser(updatedUser);

            return NoContent();
        }

        // Deleete /users/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteUser(Guid id){

            var existingUser = repository.GetUser(id);

            if(existingUser is null){
                return NotFound();
            }

            repository.DeleteUser(id);

            return NoContent();
        }

        [HttpGet("email")]
        public ActionResult<UserDto> GetUserByEmail([FromQuery] string email)
        {
            var user = repository.GetUsers().FirstOrDefault(u => u.Email == email);

            if (user is null)
            {
                return NotFound();
            }

            return user.AsDto();
        }
    }
}