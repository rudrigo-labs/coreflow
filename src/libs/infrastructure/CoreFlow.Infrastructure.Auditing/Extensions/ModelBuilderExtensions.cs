using Microsoft.EntityFrameworkCore;
using CoreFlow.Infrastructure.Auditing.Models;

namespace CoreFlow.Infrastructure.Auditing.Extensions
{
	/// <summary>
	/// Representa um plug-in para Microsoft.EntityFrameworkCore para dar suporte ao registro automático do histórico de alterações de dados.
	/// </summary>
	public static class ModelBuilderExtensions
    {
		/// <summary>
		/// Ativa o histórico de alterações de auditoria.
		/// </summary>
		/// <param name="modelBuilder">O <see cref="ModelBuilder"/> para ativar a funcionalidade de histórico automático.</param>
		/// <returns>O <see cref="ModelBuilder"/> para ativar a funcionalidade de histórico automático.</returns>
		public static ModelBuilder EnableAuditHistory(this ModelBuilder modelBuilder) {
            modelBuilder.Entity<AuditHistory>().ToTable("AuditHistory").Ignore(t => t.AutoHistoryDetails);
            modelBuilder.Entity<AuditHistory>(b =>
            {
                b.Property(c => c.Id).UseIdentityColumn(); //TODO: Possibly change this to avoid integer overflow, or cleanup every once in a while
                b.Property(c => c.RowId).IsRequired().HasDefaultValue("0").HasMaxLength(128);
                b.Property(c => c.TableName).IsRequired().HasMaxLength(128);
                b.Property(c => c.Changed).HasMaxLength(10000);
                b.Property(c => c.Username).HasMaxLength(128);
                // This MSSQL
                b.Property(c => c.Created).HasDefaultValueSql("getdate()");
            });

            return modelBuilder;
        }
    }
}
