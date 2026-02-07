using CoreFlow.Core.Application.Interfaces.Repositories;
using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Application.Interfaces.Repositories
{
    public interface IEntityRepository<T, TId> : IReadRepository<T>, IWriteRepository<T> where T : class, IEntity<TId>
    {
        T Find(TId id);
        Task<T> FindAsync(TId id);
        Task<bool> ActivateAsync<T>(TId id);
        Task<bool> DeactivateAsync<T>(TId id);
    }
}
