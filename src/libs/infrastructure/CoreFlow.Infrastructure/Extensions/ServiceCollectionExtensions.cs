using CoreFlow.Core.Application.Interfaces;
using CoreFlow.Core.Application.Interfaces.Repositories;
using CoreFlow.Core.Application.Interfaces.Repositories.Domain;
using CoreFlow.Core.Application.Interfaces.Events;
using CoreFlow.Infrastructure.DbContexts;
using CoreFlow.Infrastructure.Mappings;
using CoreFlow.Infrastructure.Repositories;
using CoreFlow.Infrastructure.Services;
using MassTransit;
using MassTransit.Saga;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabasePersistence(configuration);
            services.AddRepositories();
            services.AddAutoMapper(config =>
            {
                config.AddProfile<DomainToInfrastructureProfile>();

            });

            services.AddScoped<IServiceBus, ServiceBusMediator>();
            services.AddScoped<IOutboxWriter, OutboxWriter>();
        }

        private static void AddDatabasePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(60000)), ServiceLifetime.Scoped
                );
            }
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IEntityRepository<,>), typeof(DataEntityRepository<,>));

            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

    }
}
