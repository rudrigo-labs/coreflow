using CoreFlow.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreFlow.Infrastructure.EntityTypeConfigurations
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
    {
        public void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(254)
                .IsRequired();

            builder.HasIndex(x => x.Email).IsUnique();

            // AuditableEntity já cobre DateCreated/DateModified/CreatedBy/ModifiedBy (seus hooks já cuidam)
            // IsActive já vem do AuditableEntity também (via DataEntityBase).
        }
    }
}