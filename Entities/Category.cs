using System;

namespace Vysion.Entities
{
    public record Category
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }        
        public bool IsActive { get; init; }
        public string Slug { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
