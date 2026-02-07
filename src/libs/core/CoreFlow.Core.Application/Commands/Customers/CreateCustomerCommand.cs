using CoreFlow.Core.Application.Interfaces;
using CoreFlow.Core.Application.Interfaces.Events;
using CoreFlow.Core.Application.Interfaces.Repositories.Domain;
using CoreFlow.Core.Domain.Entities.CustomerAggregate;
using CoreFlow.Core.Domain.Interfaces;
using CoreFlow.Shared.Results;
using CoreFlow.Shared.Results.Extensions;

namespace CoreFlow.Core.Application.Commands.Customers
{
    public sealed record CreateCustomerCommand(
        string Name,
        string Email
    ) : IRequest<Result<Guid>>;

    public sealed class CreateCustomerCommandHandler
        : BaseMessageHandler<CreateCustomerCommand, Result<Guid>>
    {
        private readonly ICustomerRepository _repository;
        private readonly IOutboxWriter _outbox;

        public CreateCustomerCommandHandler(ICustomerRepository repository, IOutboxWriter outbox)
        {
            _repository = repository;
            _outbox = outbox;
        }

        public override async Task<Result<Guid>> HandleAsync(CreateCustomerCommand request, CancellationToken ct)
        {
            var result = new Result<Guid>();

            // Regra simples: email único
            if (await _repository.ExistsAsync(x => x.Email == request.Email))
                return result.Failed().WithMessage("Já existe Customer com este email.");

            var aggregate = new Customer(request.Name, request.Email);

            await _repository.AddAsync(aggregate);
            await _repository.UnitOfWork.SaveChangesAsync(ct);

            // Persistir eventos do domínio (Outbox) e limpar a fila local
            var events = ((IAggregateRoot<Guid>)aggregate).GetUncommittedEvents().ToList();
            await _outbox.WriteAsync(events, ct);
            ((IAggregateRoot<Guid>)aggregate).ClearUncommittedEvents();

            return result.Created().WithData(aggregate.Id);
        }
    }
}
