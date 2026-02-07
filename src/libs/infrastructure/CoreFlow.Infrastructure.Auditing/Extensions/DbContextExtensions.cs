using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CoreFlow.Infrastructure.Auditing;
using CoreFlow.Infrastructure.Auditing.Extensions;
using CoreFlow.Infrastructure.Auditing.Models;

namespace CoreFlow.Infrastructure.Auditing.Extensions
{
    public static class DbContextExtensions
    {
		/// <summary>
		/// Garante o histórico de auditoria automática.
		/// </summary>
		/// <param name="context">O contexto.</param>
		/// <param name="username">O usuário realizou a ação.</param>
		public static void EnsureAuditHistory(this DbContext context, string username) {
            var entries = context.ChangeTracker.Entries().Where(e => !AuditUtilities.IsAuditDisabled(e.Entity.GetType()) && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)).ToArray();
            foreach (var entry in entries) {
                context.Add(entry.AutoHistory(username));
            }
        }

        //Mark as deleted entities that had the relationship severed
        //To do
        public static void EnsureDeletionOfSeveredRelationships(this DbContext context) {
            var entries = context.ChangeTracker.Entries().Where(e => !AuditUtilities.IsAuditDisabled(e.Entity.GetType()) && e.State == EntityState.Modified).ToArray();
            foreach (var entry in entries) {
                foreach (var prop in entry.Properties) {
                    if (prop.Metadata.IsForeignKey()) {
                        var fk = prop.CurrentValue;
                        if (fk == null) {
                            entry.State = EntityState.Deleted;
                            break;
                        }
                    }
                }
            }
        }

        private static AuditHistory AutoHistory(this EntityEntry entry, string username) {
            var history = new AuditHistory
            {
                TableName = entry.Metadata.GetTableName(),
                Username = username
            };

			// Obtenha as propriedades mapeadas para o tipo de entidade.
			// (incluir propriedades de sombra, não incluir navegações e referências)
			var properties = entry.Properties.Where(p => !AuditUtilities.IsAuditDisabled(p.EntityEntry.Entity.GetType(), p.Metadata.Name));

            foreach (var prop in properties) {
                string propertyName = prop.Metadata.Name;
                if (prop.Metadata.IsPrimaryKey()) {
                    history.AutoHistoryDetails.NewValues[propertyName] = prop.CurrentValue;
                    continue;
                }

                switch (entry.State) {
                    case EntityState.Added:
                        history.RowId = "0";
                        history.Kind = EntityState.Added;
                        history.AutoHistoryDetails.NewValues.Add(propertyName, prop.CurrentValue);
                        break;

                    case EntityState.Modified:
                        history.RowId = entry.PrimaryKey();
                        history.Kind = EntityState.Modified;
                        history.AutoHistoryDetails.OldValues.Add(propertyName, prop.OriginalValue);
                        history.AutoHistoryDetails.NewValues.Add(propertyName, prop.CurrentValue);
                        break;

                    case EntityState.Deleted:
                        history.RowId = entry.PrimaryKey();
                        history.Kind = EntityState.Deleted;
                        history.AutoHistoryDetails.OldValues.Add(propertyName, prop.OriginalValue);
                        break;
                }
            }

            history.Changed = JsonSerializer.Serialize(history.AutoHistoryDetails);

            return history;
        }

        private static string PrimaryKey(this EntityEntry entry) {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties) {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null) {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }
    }
}
