using CoreFlow.Core.Application.ReadModels.Customer;
using CoreFlow.Core.Contracts.Customers;
using CoreFlow.Shared.Results;
using CoreFlow.Shared.Results.Extensions;


namespace CoreFlow.Api.Extensions
{
    public static class CustomerExtensions
    {
        public static CustomerResponse ToResponse(this CustomerReadModel model)
                => new CustomerResponse
                {
                    Id = model.Id,
                    Name = model.Name,
                    Email = model.Email,
                };

        public static Result<CustomerResponse> MapToResponse(
        this Result<CustomerReadModel> input)
        => input.MapResult(x => x.ToResponse());
    }
}
