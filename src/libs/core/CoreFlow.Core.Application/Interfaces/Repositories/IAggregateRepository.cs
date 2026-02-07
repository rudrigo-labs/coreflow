using CoreFlow.Core.Application.Interfaces.Repositories;
using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Application.Interfaces.Repositories
{
    public interface IAggregateRepository<T, TId> : IEntityRepository<T, TId> where T : class, IAggregateRoot<TId>
    {
    }
}
