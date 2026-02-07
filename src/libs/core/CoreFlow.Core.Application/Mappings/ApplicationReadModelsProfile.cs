using AutoMapper;
using CoreFlow.Core.Application.ReadModels.Customer;
using CoreFlow.Core.Domain.Entities.CustomerAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Core.Application.Mappings
{
    public sealed class ApplicationReadModelsProfile : Profile
    {
        public ApplicationReadModelsProfile()
        {
            CreateMap<Customer, CustomerReadModel>();
        }
    }

}
