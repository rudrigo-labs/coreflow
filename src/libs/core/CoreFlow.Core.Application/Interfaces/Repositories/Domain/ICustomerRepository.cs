using CoreFlow.Core.Domain.Entities.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Core.Application.Interfaces.Repositories.Domain
{
    public interface ICustomerRepository : IAggregateRepository<Customer, Guid>
    {
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Customer?> GetByEmailAsync(string email, CancellationToken ct);
    }
}
