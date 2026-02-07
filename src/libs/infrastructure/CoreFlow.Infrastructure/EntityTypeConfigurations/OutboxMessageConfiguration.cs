using CoreFlow.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreFlow.Infrastructure.EntityTypeConfigurations
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OccurredAtUtc).IsRequired();
            builder.Property(x => x.AggregateType).HasMaxLength(200).IsRequired();
            builder.Property(x => x.EventType).HasMaxLength(400).IsRequired();
            builder.Property(x => x.PayloadJson).IsRequired();

            builder.Property(x => x.Published).IsRequired();
            builder.Property(x => x.PublishedAtUtc).IsRequired(false);

            builder.HasIndex(x => new { x.Published, x.OccurredAtUtc });
        }
    }
}
