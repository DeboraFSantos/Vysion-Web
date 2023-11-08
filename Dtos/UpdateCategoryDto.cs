using System;

namespace Vysion.Dtos
{
    public record UpdateCategoryDto
    {
        public string Name { get; init; }
        public string Description { get; init; }        
        public bool IsActive { get; init; }
        public string Slug { get; init; }
    }
}
