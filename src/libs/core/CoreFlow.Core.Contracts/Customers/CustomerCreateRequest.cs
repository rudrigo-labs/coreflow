using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Core.Contracts.Customers
{
    public sealed class CustomerCreateRequest
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
