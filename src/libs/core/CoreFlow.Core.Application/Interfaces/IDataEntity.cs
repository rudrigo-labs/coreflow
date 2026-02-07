
using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Application.Interfaces
{
    /// <summary>
    /// Interface de marcador genérico para um modelo de dados.
    /// Usado para identificar especificamente modelos de dados (relacionados à persistência)
    /// </summary>
    /// <typeparam name="TId">O tipo do Id</typeparam>
    public interface IDataEntity<TId> : IEntity<TId>
    {
    }
}
