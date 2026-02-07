using System.Text.Json;
using CoreFlow.Core.Application.Interfaces.Events;
using CoreFlow.Core.Domain.Interfaces;
using CoreFlow.Infrastructure.DbContexts;
using CoreFlow.Infrastructure.Models;

namespace CoreFlow.Infrastructure.Services
{
    public sealed class OutboxWriter : IOutboxWriter
    {
        private readonly ApplicationDbContext _db;

        public OutboxWriter(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task WriteAsync<TId>(IEnumerable<IDomainEvent<TId>> events, CancellationToken ct)
        {
            if (events is null) return;

            foreach (var e in events)
            {
                var msg = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredAtUtc = DateTime.UtcNow,
                    AggregateType = typeof(TId).Name, // mínimo; ajuste se quiser AggregateName real
                    EventType = e.GetType().FullName ?? e.GetType().Name,
                    PayloadJson = JsonSerializer.Serialize(e, e.GetType(), new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = false
                    }),
                    Published = false
                };

                await _db.Set<OutboxMessage>().AddAsync(msg, ct);
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
