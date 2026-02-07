using CoreFlow.Core.Application.Interfaces.Repositories;
using CoreFlow.Core.Application.Interfaces.Shared;
using CoreFlow.Infrastructure.Auditing.Extensions;
using CoreFlow.Infrastructure.Auditing.Models;
using CoreFlow.Infrastructure.EntityTypeConfigurations;
using CoreFlow.Infrastructure.Models;
using MassTransit.Configuration;
using MassTransit.Monitoring.Performance.StatsD;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Infrastructure.DbContexts
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public DbSet<AuditHistory> AuditHistory { get; set; }

        public UserResolverService _userResolverService { get; set; }

        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, UserResolverService userResolverService)
            : base(options)
        {
            _userResolverService = userResolverService;
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Definições de modelo de entidade
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.EnableAuditHistory();

            builder.ApplyConfiguration(new CustomerConfiguration());

            builder.ApplyConfiguration(new OutboxMessageConfiguration());

            foreach (var relationship in builder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //optionsBuilder.EnableSensitiveDataLogging();
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{envName}.json", true)
                .AddEnvironmentVariables()
                .Build();

            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection")); //.UseLazyLoadingProxies();
        }

        public DbSet<TEntity> DbSet<TEntity>() where TEntity : DataEntityBase<Guid>
        {
            return Set<TEntity>();
        }

        /// <summary>
        /// Substitui o <see cref="SaveChanges(bool)"/>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var username = _userResolverService.GetUserId();

            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), username);
            this.EnsureAuditHistory(username);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <summary>
        /// Substitui o <see cref="DbContext.SaveChanges()"/>.
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var username = _userResolverService.GetUserId();

            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), username);
            this.EnsureAuditHistory(username);
            return base.SaveChanges(true);
        }

        /// <summary>
        /// Substitui o <see cref="DbContext.SaveChangesAsync(bool, CancellationToken)"/>
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var username = _userResolverService.GetUserId();

            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), username);
            this.EnsureAuditHistory(username);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Substitui o <see cref="SaveChangesAsync(CancellationToken)"/>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var username = _userResolverService.GetUserId();

            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), username);
            this.EnsureAuditHistory(username);
            this.EnsureDeletionOfSeveredRelationships();
            var result = base.SaveChangesAsync(true, cancellationToken);
            return result;
        }


        /// <summary>
        /// Metodo para salvar alterações no banco de dados e não rastrear as entidades posteriormente
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<int> SaveChangesAndUntrackAsync(CancellationToken cancellationToken = default)
        {
            var username = _userResolverService.GetUserId();

            DbContextUpdateOperations.UpdateDates(ChangeTracker.Entries<AuditableEntity>(), username);
            this.EnsureAuditHistory(username);
            this.EnsureDeletionOfSeveredRelationships();
            Task<int>? result;

            try
            {
                result = base.SaveChangesAsync(true, cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency exception", ex);
            }
            finally
            {
                ChangeTracker.Clear();
            }

            return result;
        }
    }
}
