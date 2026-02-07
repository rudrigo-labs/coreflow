using AutoMapper;
using CoreFlow.Core.Application.Interfaces;
using CoreFlow.Core.Application.Interfaces.Repositories.Domain;
using CoreFlow.Core.Application.ReadModels.Customer;
using CoreFlow.Shared.Results;
using CoreFlow.Shared.Results.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Core.Application.Queries.Customers
{
    public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerReadModel>>;

    public sealed class GetCustomerByIdQueryHandler
        : BaseMessageHandler<GetCustomerByIdQuery, Result<CustomerReadModel>>
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public GetCustomerByIdQueryHandler(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override async Task<Result<CustomerReadModel>> HandleAsync(GetCustomerByIdQuery request, CancellationToken ct)
        {
            var result = new Result<CustomerReadModel>();

            var aggregate = await _repository.GetByIdAsync(request.Id, ct);
            if (aggregate is null)
                return result.NotFound("Customer não encontrado.");

            var data = _mapper.Map<CustomerReadModel>(aggregate);
            return result.Successful().WithData(data);
        }
    }
}
