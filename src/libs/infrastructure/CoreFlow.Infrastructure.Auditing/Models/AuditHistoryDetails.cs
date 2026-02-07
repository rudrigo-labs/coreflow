namespace CoreFlow.Infrastructure.Auditing.Models
{
	/// <summary>
	/// Detalhes dos valores alterados
	/// </summary>
	public class AutoHistoryDetails
    {
		/// <summary>
		/// Os valores após a ação.
		/// Key contém o nome da coluna e Value o valor da coluna.
		/// </summary>
		public Dictionary<string, object> NewValues { get; set; } = new Dictionary<string, object>();

		/// <summary>
		/// Os valores antes da ação.
		/// Key contém o nome da coluna e Value o valor da coluna.
		/// </summary>
		public Dictionary<string, object> OldValues { get; set; } = new Dictionary<string, object>();
    }
}
