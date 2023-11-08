using System;

namespace Vysion.Entities
{
    public record User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}