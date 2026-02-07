using CoreFlow.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoreFlow.Infrastructure.DbContexts
{
	/// <summary>
	/// Contém um conjunto de operações automáticas contra o banco de dados
	/// </summary>
	internal static class DbContextUpdateOperations
    {
		/// <summary>
		/// Atualiza automaticamente as propriedades BaseEntity sem nenhuma ação do desenvolvedor
		/// </summary>
		/// <param name="changes">A coleção de entidades BaseEntity para salvar no banco de dados</param>
		/// <param name="username">O usuário logado</param>
		public static void UpdateDates(IEnumerable<EntityEntry<AuditableEntity>> changes, string username) {
            DateTime now = DateTime.UtcNow;
            foreach (var change in changes) {
                switch (change.State) {
                    case EntityState.Added:
                        change.Entity.DateCreated = now;
                        if (!string.IsNullOrEmpty(username)) {
                            change.Entity.CreatedBy = username;
                        }
                        break;

                    case EntityState.Modified:
                        if (!string.IsNullOrEmpty(username)) {
                            change.Entity.ModifiedBy = username;
                            change.Entity.DateModified = now;
                            //mark datecreated as unchanged
                            change.Property("DateCreated").IsModified = false;
                        }
						break;
                }
            }
        }
    }
}
