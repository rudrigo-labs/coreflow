using System;

namespace CoreFlow.Core.Contracts.Customers
{
    public sealed class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
