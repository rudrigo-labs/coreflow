using System.Reflection;
using CoreFlow.Core.Domain.Entities;
using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Domain.Entities
{
    public abstract class AggregateRootBase<TId> : IAggregateRoot<TId>
    {
        public const int NewAggregateVersion = -1;

        private readonly ICollection<IDomainEvent<TId>> _uncommittedEvents = new LinkedList<IDomainEvent<TId>>();

		/// <summary>
		/// O ID da raiz agregada
		/// </summary>
		public TId Id { get; protected set; }

        /// <summary>
		/// A versão atual da raiz agregada
		/// </summary>
		public int Version { get; private set; } = NewAggregateVersion;

		/// <summary>
		/// Indica se este agregado é excluído logicamente
		/// </summary>
		public bool IsDeleted { get; private set; }

        public bool IsActive { get; set; } = true;

        protected AggregateRootBase()
        {
        }

        protected AggregateRootBase(TId aggregateId) : this()
        {
            Id = aggregateId;
        }

        void IAggregateRoot<TId>.ApplyEvent(IDomainEvent<TId> @event, int version) {
            if (!_uncommittedEvents.Any(x => Equals(x.EventId, @event.EventId))) {
                try {
                    InvokeHandler(@event);
                    Version = version;
                }
                catch (TargetInvocationException ex) {
                    throw ex.InnerException;
                }
            }
            Version = version;
        }

        void IAggregateRoot<TId>.ClearUncommittedEvents() {
            _uncommittedEvents.Clear();
        }

        IEnumerable<IDomainEvent<TId>> IAggregateRoot<TId>.GetUncommittedEvents() {
            return _uncommittedEvents.AsEnumerable();
        }

        protected void MarkAsDeleted() {
            IsDeleted = true;
        }

        protected void RaiseEvent<TEvent>(TEvent @event)
            where TEvent : DomainEventBase<TId> {
            int version = Version + 1;
            IDomainEvent<TId> eventWithAggregate = @event.WithAggregate(
                Equals(Id, default(TId)) ? @event.AggregateId : Id,
                version);

            ((IAggregateRoot<TId>)this).ApplyEvent(eventWithAggregate, version);
            _uncommittedEvents.Add(eventWithAggregate);
        }

        private void InvokeHandler(IDomainEvent<TId> @event) {
            var handlerMethod = GetEventHandlerMethodInfo(@event);
            handlerMethod.Invoke(this, new object[] { @event });
        }

        private MethodInfo GetEventHandlerMethodInfo(IDomainEvent<TId> @event) {
            var handlerType = GetType()
                    .GetInterfaces()
                    .Single(i => i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>) && i.GetGenericArguments()[0] == @event.GetType());
            return handlerType.GetTypeInfo().GetDeclaredMethod(nameof(IDomainEventHandler<IDomainEvent>.Apply));
        }
    }
}
