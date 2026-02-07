namespace CoreFlow.Infrastructure.Auditing.Attributes
{
	/// <summary>
	/// Anotação de dados personalizada para definir se uma classe (tabela) ou propriedade (campo) não será auditável. Por padrão, é true.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NotAuditableAttribute : Attribute
    {
		/// <summary>
		/// Define se class(table) ou property(field) é auditável ou não
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="nonAuditable">NÃO é auditável</param>
		public NotAuditableAttribute(bool nonAuditable = true) {
            this.Enabled = nonAuditable;
        }
    }
}
