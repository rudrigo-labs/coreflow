using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Domain.Entities.CustomerAggregate
{
    public sealed class Customer : AggregateRootBase<Guid>, IDomainEventHandler<CustomerCreatedEvent>
    {
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;

        private Customer() { }

        public Customer(string name, string email)
        {
            // Evento controla o estado inicial.
            RaiseEvent(new CustomerCreatedEvent(Guid.NewGuid(), name.Trim(), email.Trim()));
        }

        public static Customer Rehydrate(Guid id, string name, string email)
        {
            // Reidratação (sem gerar eventos)
            var c = new Customer();
            c.Id = id;
            c.Name = name;
            c.Email = email;
            return c;
        }

        void IDomainEventHandler<CustomerCreatedEvent>.Apply(CustomerCreatedEvent @event)
        {
            Id = @event.AggregateId;
            Name = @event.Name;
            Email = @event.Email;
        }

        public void Update(string name, string email)
        {
            // Mantido simples por enquanto (sem evento de update, conforme fluxo mínimo).
            Name = name.Trim();
            Email = email.Trim();
        }
    }
}
