using CoreFlow.Core.Application.Interfaces;
namespace CoreFlow.Core.Application.Interfaces
{
    /// <summary>
    /// Abstração sobre um manipulador de mensagens provenientes de um intermediário de mensagens.
    /// Deve implementar quaisquer interfaces para cumprir com qualquer objeto manipulador de corretores de mensagens
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TResponse : class
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken ct);
    }
}
