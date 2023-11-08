using System;

namespace Vysion.Dtos
{
    public record ClientDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }        
        public string Document { get; init; } 
        public string Phone { get; init; }
        public string Address { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
