
using CoreFlow.Core.Application.Interfaces;

namespace CoreFlow.Infrastructure.Models
{
	/// <summary>
	/// Classe de entidade de dados base a ser herdada de cada entidade no contexto do banco de dados
	/// Herda <see cref="AuditableEntity"/> por conveniência.
	/// </summary>
	public class DataEntityBase<TId> : AuditableEntity, IDataEntity<TId>
    {
        public TId Id { get; set; }
    }
}
