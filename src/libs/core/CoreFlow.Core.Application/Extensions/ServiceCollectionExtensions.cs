using CoreFlow.Core.Application.Commands.Customers;
using CoreFlow.Core.Application.Mappings;
using CoreFlow.Core.Application.Queries.Customers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Core.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile<ApplicationReadModelsProfile>();
            });


            services.AddMediator(x =>
            {
                x.AddConsumer<CreateCustomerCommandHandler>();
                x.AddConsumer<GetCustomerByIdQueryHandler>();
            });
        }
    }
}
