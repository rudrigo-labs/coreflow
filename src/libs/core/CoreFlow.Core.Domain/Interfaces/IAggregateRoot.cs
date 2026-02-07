using CoreFlow.Core.Domain.Interfaces;
namespace CoreFlow.Core.Domain.Interfaces
{
	/// <summary>
	/// Interface para raiz agregada
	/// </summary>
	public interface IAggregateRoot<TId> : IEntity<TId>
    {
        int Version { get; }

        void ApplyEvent(IDomainEvent<TId> @event, int version);

        IEnumerable<IDomainEvent<TId>> GetUncommittedEvents();

        void ClearUncommittedEvents();

        bool IsDeleted { get; }
    }
}
