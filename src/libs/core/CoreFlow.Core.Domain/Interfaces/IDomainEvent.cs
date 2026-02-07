using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Domain.Interfaces
{
	/// <summary>
	/// Interface para eventos de domínio
	/// </summary>
	public interface IDomainEvent
    {
		/// <summary>
		/// O identificador de evento
		/// </summary>
		Guid EventId { get; }
    }

	/// <summary>
	/// Interface genérica para eventos de domínio com id agregado
	/// </summary>
	public interface IDomainEvent<TAggregateId> : IDomainEvent
    {
		/// <summary>
		/// O identificador do agregado que gerou o evento
		/// </summary>
		TAggregateId AggregateId { get; }

		/// <summary>
		/// A versão do agregado quando o evento foi gerado
		/// </summary>
		int AggregateVersion { get; }
    }
}
