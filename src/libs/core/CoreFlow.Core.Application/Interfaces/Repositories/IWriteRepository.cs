using CoreFlow.Core.Application.Interfaces.Repositories;

namespace CoreFlow.Core.Application.Interfaces.Repositories
{
    public interface IWriteRepository<T> : IRepository
    {
        void Add(T entity);
        ValueTask AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
    }

}
