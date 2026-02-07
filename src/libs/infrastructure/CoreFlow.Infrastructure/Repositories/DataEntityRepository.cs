using System.Linq.Expressions;
using Ardalis.GuardClauses;
using CoreFlow.Core.Application.Interfaces;
using CoreFlow.Core.Application.Interfaces.Repositories;
using CoreFlow.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoreFlow.Infrastructure.Repositories
{
    /// <inheritdoc cref="IEntityRepository{T,TId}"/>
    /// <summary>
    /// Repositório para entidades de dados. Ver <see cref="IDataEntity{TId}"/>. Implements <see cref="IEntityRepository{T, TId}"/>
    /// Usado apenas para entidades de negócios específicas do aplicativo, caso você não precise deles em seu modelo de domínio de negócios (por exemplo, Configurações de aplicativos)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    internal class DataEntityRepository<T, TId> : IEntityRepository<T, TId> where T : class, IDataEntity<TId>
    {
        #region Private Fields

        private readonly DbSet<T> _entities;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Database context
        /// </summary>
        private readonly ApplicationDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        #endregion Private Fields

        public DataEntityRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration;
            _entities = context.Set<T>();
        }

        public void Add(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));
            _entities.Add(entity);
        }

        public async ValueTask AddAsync(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));
            await _entities.AddAsync(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _entities.AddRange(entities);
        }

        public Task AddRangeAsync(IEnumerable<T> entities)
        {
            return _entities.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));
            _entities.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            Guard.Against.Null(entities, nameof(entities));
            _entities.RemoveRange(entities);
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _entities.Any(predicate);
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return _entities.AnyAsync(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return GetEntities();
        }

        public IQueryable<T> GetBy(Expression<Func<T, bool>> predicate)
        {
            return GetEntities().Where(predicate);
        }

        public T GetFirst(Expression<Func<T, bool>> predicate)
        {
            return GetEntities().FirstOrDefault(predicate);
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetEntities().FirstOrDefaultAsync(predicate);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return GetEntities().SingleOrDefault(predicate);
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetEntities().SingleOrDefaultAsync(predicate);
        }

        public void Update(T entity)
        {
            Guard.Against.Null(entity, nameof(entity));
            _entities.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            Guard.Against.Null(entities, nameof(entities));
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public T Find(TId id)
        {
            return _entities.Find(id);
        }

        public async Task<T> FindAsync(TId id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<bool> ActivateAsync<T1>(TId id)
        {
            var isActiveName = _configuration.GetSection("IsActiveName").Value;
            var entity = await _entities.FindAsync(id);
            entity.GetType().GetProperty(isActiveName)!.SetValue(entity, true);
            Update(entity);
            return await UnitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateAsync<T1>(TId id)
        {
            var isActiveName = _configuration.GetSection("IsActiveName").Value;
            var entity = await _entities.FindAsync(id);
            entity.GetType().GetProperty(isActiveName)!.SetValue(entity, false);
            Update(entity);
            return await UnitOfWork.SaveChangesAsync() > 0;
        }

        private IQueryable<T> GetEntities(bool asNoTracking = true)
        {
            if (asNoTracking)
                return _entities.AsNoTracking();
            return _entities;
        }
    }
}
