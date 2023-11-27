using System;

namespace Vysion.Entities
{
    public record PaymentMethod
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public DateTimeOffset CreatedDate { get; init; }
    }
}
