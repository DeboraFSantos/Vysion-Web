using System;

namespace Vysion.Dtos
{
    public record CreateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }
}