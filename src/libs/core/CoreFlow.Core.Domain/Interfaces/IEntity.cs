namespace CoreFlow.Core.Domain.Interfaces
{
	/// <summary>
	/// Abstração genérica para uma entidade de domínio
	/// </summary>
	/// <typeparam name="TId"></typeparam>
	public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
