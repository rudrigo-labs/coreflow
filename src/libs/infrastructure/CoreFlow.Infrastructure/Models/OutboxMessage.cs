using System;

namespace CoreFlow.Infrastructure.Models
{
    /// <summary>
    /// Registro de Outbox para eventos de domínio (eventual publish).
    /// </summary>
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public DateTime OccurredAtUtc { get; set; }
        public string AggregateType { get; set; } = default!;
        public string EventType { get; set; } = default!;
        public string PayloadJson { get; set; } = default!;
        public bool Published { get; set; }
        public DateTime? PublishedAtUtc { get; set; }
    }
}
