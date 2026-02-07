using MassTransit;
using CoreFlow.Core.Application.Interfaces;

namespace CoreFlow.Core.Application
{
    /// <summary>
    /// Abstração sobre um manipulador de mensagens provenientes de um intermediário de mensagens.
    /// Deve implementar quaisquer interfaces para cumprir com qualquer objeto manipulador de corretores de mensagens
    /// Implementos <see cref="IConsumer"/> para implementação do MassTransit
    /// Caso o MassTransit seja substituído por algum outro corretor, então <see cref="IConsumer{TMessage}"/> deve ser alterado para qualquer interface que precise ser implementada pelo intermediário ou criar outro manipulador de mensagens base
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class BaseMessageHandler<TRequest, TResponse>
    : IHandler<TRequest, TResponse>, IConsumer<TRequest>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var messageResult = await HandleAsync(context.Message, context.CancellationToken);
            await context.RespondAsync(messageResult);
        }

        // Conveniência: mantém compatibilidade para chamadas sem token.
        public virtual Task<TResponse> HandleAsync(TRequest request)
            => HandleAsync(request, CancellationToken.None);

        // ✅ ÚNICO método obrigatório para handlers
        public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken ct);
    }

}
