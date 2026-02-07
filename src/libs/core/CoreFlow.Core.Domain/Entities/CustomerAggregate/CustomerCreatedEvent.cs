using CoreFlow.Core.Domain.Entities;
using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Domain.Entities.CustomerAggregate
{
    /// <summary>
    /// Evento de domínio: Customer criado.
    /// </summary>
    public sealed class CustomerCreatedEvent : DomainEventBase<Guid>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }

        private CustomerCreatedEvent() { }

        public CustomerCreatedEvent(Guid customerId, string name, string email)
            : base(customerId)
        {
            Name = name;
            Email = email;
        }

        private CustomerCreatedEvent(Guid customerId, int aggregateVersion, string name, string email)
            : base(customerId, aggregateVersion)
        {
            Name = name;
            Email = email;
        }

        public override IDomainEvent<Guid> WithAggregate(Guid aggregateId, int aggregateVersion)
            => new CustomerCreatedEvent(aggregateId, aggregateVersion, Name, Email);
    }
}
