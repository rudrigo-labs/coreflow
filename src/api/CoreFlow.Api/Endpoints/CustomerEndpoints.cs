using CoreFlow.Api.Extensions;
using CoreFlow.Core.Application.Commands.Customers;
using CoreFlow.Core.Application.Interfaces;
using CoreFlow.Core.Application.Queries.Customers;
using CoreFlow.Core.Contracts.Customers;

namespace CoreFlow.Api.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void MapCustomerEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/customers")
                .WithTags("Customers");

            group.MapPost("/", async (
                CustomerCreateRequest request,
                IServiceBus bus,
                CancellationToken ct) =>
            {
                var command = new CreateCustomerCommand(request.Name, request.Email);
                var result = await bus.Send(command, ct);

                return result.ToHttpResult();
            })
            .WithName("CreateCustomer")
            .WithSummary("Criar customer")
            .WithDescription("Cria um novo customer.")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", async (
                Guid id,
                IServiceBus bus,
                CancellationToken ct) =>
            {
                var query = new GetCustomerByIdQuery(id);
                var result = await bus.Send(query, ct);

                return result.MapToResponse().ToHttpResult();
            })
            .WithName("GetCustomerById")
            .WithSummary("Obter customer por id")
            .WithDescription("Retorna um customer a partir do seu identificador.")
            .Produces<CustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
