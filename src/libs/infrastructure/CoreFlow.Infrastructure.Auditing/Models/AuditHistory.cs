using Microsoft.EntityFrameworkCore;
using CoreFlow.Infrastructure.Auditing.Attributes;
using CoreFlow.Infrastructure.Auditing.Models;

namespace CoreFlow.Infrastructure.Auditing.Models
{
	/// <summary>
	/// DB Entidade de auditoria
	/// </summary>
	[NotAuditable]
    public class AuditHistory
    {
		/// <summary>
		/// Obtém ou define a chave primária.
		/// </summary>
		/// <value>O id.</value>
		public int Id { get; set; }

		/// <summary>
		/// Obtém ou define o ID da linha de origem.
		/// </summary>
		/// <value>O ID da linha de origem.</value>
		public string RowId { get; set; }

		/// <summary>
		/// Obtém ou define o nome da tabela.
		/// </summary>
		/// <value>O nome da tabela.</value>
		public string TableName { get; set; }

		/// <summary>
		/// Obtém ou define o json sobre a alteração.
		/// </summary>
		/// <value>O json sobre a mudança.</value>
		public string Changed { get; set; }

		/// <summary>
		/// Obtém ou define o tipo de alteração.
		/// </summary>
		/// <value>o tipo de mudança.</value>
		public EntityState Kind { get; set; }

		/// <summary>
		/// Obtém ou define o tempo de criação.
		/// </summary>
		/// <value>O tempo de criação.</value>
		public DateTime Created { get; set; } = DateTime.Now;

		/// <summary>
		/// Nome de usuário do usuário que realizou a ação.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Os valores estruturados contidos na propriedade <see cref="Changed"/>.
		/// </summary>
		public AutoHistoryDetails AutoHistoryDetails { get; set; } = new AutoHistoryDetails();
    }
}
