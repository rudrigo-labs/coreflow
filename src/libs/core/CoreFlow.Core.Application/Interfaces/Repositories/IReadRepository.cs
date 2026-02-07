using System.Linq.Expressions;

namespace CoreFlow.Core.Application.Interfaces.Repositories
{
    public interface IReadRepository<T>
    {
        bool Exists(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        IQueryable<T> GetBy(Expression<Func<T, bool>> predicate);
        T GetFirst(Expression<Func<T, bool>> predicate);
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate);
        T GetSingle(Expression<Func<T, bool>> predicate);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
    }
}
