using System;

namespace Vysion.Dtos
{
    public record PaymentMethodDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
