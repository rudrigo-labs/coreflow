using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Infrastructure.Models
{
    /// <summary>
    /// Entidade de persistência (Infra) — Customer.
    /// </summary>
    public sealed class CustomerEntity : DataEntityBase<Guid>
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
