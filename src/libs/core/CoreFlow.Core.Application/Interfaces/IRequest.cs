namespace CoreFlow.Core.Application.Interfaces
{
    /// <summary>
    ///Abstração sobre uma requisição/mensagem a ser enviada para um message broker
    /// Deve implementar todas as interfaces para cumprir com qualquer objeto de solicitação do intermediário de mensagem (se houver)
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IRequest<TResponse> where TResponse : class
    {
    }
}
