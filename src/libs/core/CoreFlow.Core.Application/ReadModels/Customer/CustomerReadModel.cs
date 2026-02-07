using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Core.Application.ReadModels.Customer
{
    public sealed class CustomerReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
