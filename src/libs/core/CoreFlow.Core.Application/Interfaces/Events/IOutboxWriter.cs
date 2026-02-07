using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Application.Interfaces.Events
{
    /// <summary>
    /// Porta (Application) para persistir eventos de domínio (Outbox).
    /// A implementação concreta fica na Infrastructure.
    /// </summary>
    public interface IOutboxWriter
    {
        Task WriteAsync<TId>(IEnumerable<IDomainEvent<TId>> events, CancellationToken ct);
    }
}
