using AutoMapper;
using CoreFlow.Core.Application.Interfaces.Repositories;
using CoreFlow.Core.Application.Interfaces.Repositories.Domain;
using CoreFlow.Core.Domain.Entities.CustomerAggregate;
using CoreFlow.Infrastructure.DbContexts;
using CoreFlow.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Infrastructure.Repositories
{
    public sealed class CustomerRepository
        : EFRepository<Customer, CustomerEntity, Guid>, ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomerRepository(
            ApplicationDbContext context,
            IMapper mapper,
            IEntityRepository<CustomerEntity, Guid> persistenceRepo,
            IConfiguration configuration)
            : base(mapper, persistenceRepo, configuration)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var entity = await _context
                .Set<CustomerEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id), ct);

            return entity is null ? null : _mapper.Map<Customer>(entity);
        }

        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct)
        {
            var entity = await _context
                .Set<CustomerEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email, ct);

            return entity is null ? null : _mapper.Map<Customer>(entity);
        }
    }
}
