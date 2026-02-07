
using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Application.Interfaces.Repositories
{
	/// <summary>
	/// Interface genérica para repositório de fornecimento de eventos para raízes agregadas. Ver <see cref="IAggregateRoot{TId}"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TId"></typeparam>
	public interface IESRepository<T, TId> where T : IAggregateRoot<TId>
    {
		/// <summary>
		/// Recupera uma entidade do armazenamento de eventos por id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<T> GetByIdAsync(TId id);

		/// <summary>
		/// Salva o agregado no armazenamento de eventos
		/// </summary>
		/// <param name="aggregate"></param>
		/// <returns></returns>
		Task SaveAsync(T aggregate);
    }
}
