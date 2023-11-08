using System;

namespace Vysion.Dtos
{
    public record CategoryDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }        
        public bool IsActive { get; init; }
        public string Slug { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
