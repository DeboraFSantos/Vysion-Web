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
    [Route("users")]
    public class UsersController : ControllerBase
    {   
        private readonly IUsersRepository repository;

        public UsersController(IUsersRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<UserDto> GetUsers()
        {
            var users = repository.GetUsers().Select(user => user.AsDto());

            return users;
        }

        [HttpGet("{id}")]
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
        public ActionResult DeleteUser(Guid id){

            var existingUser = repository.GetUser(id);

            if(existingUser is null){
                return NotFound();
            }

            repository.DeleteUser(id);

            return NoContent();
        }
    }
}