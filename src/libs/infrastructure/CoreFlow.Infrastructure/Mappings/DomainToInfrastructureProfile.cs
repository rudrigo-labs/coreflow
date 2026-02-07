using AutoMapper;
using CoreFlow.Core.Domain.Entities.CustomerAggregate;
using CoreFlow.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CoreFlow.Infrastructure.Mappings
{
    public sealed class DomainToInfrastructureProfile : Profile
    {
        public DomainToInfrastructureProfile()
        {
            CreateMap<Customer, CustomerEntity>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email));

            CreateMap<CustomerEntity, Customer>()
                .ConstructUsing(e => Customer.Rehydrate(e.Id, e.Name, e.Email));
        }
    }
}
