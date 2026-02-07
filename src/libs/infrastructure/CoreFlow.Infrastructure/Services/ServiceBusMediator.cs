using CoreFlow.Core.Application.Interfaces;
using MassTransit;
using MassTransit.Mediator;

namespace CoreFlow.Infrastructure.Services
{
	/// <summary>
	/// Abstração sobre as especificidades de implementação de uma transmissão de intermediário de mensagem
	/// Implementação concreta de <see cref="IServiceBus"/> que usa MassTransit's <see cref="IMediator"/>
	/// </summary>
    public class ServiceBusMediator : IServiceBus
    {
        private readonly IMediator _mediator;

        public ServiceBusMediator(IMediator mediator) {
            _mediator = mediator;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) where TResponse : class 
        {
            var client = _mediator.CreateRequestClient<IRequest<TResponse>>();
            cancellationToken.ThrowIfCancellationRequested();
            var response = await client.GetResponse<TResponse>(request, cancellationToken, TimeSpan.FromMinutes(10));
            return response.Message;
        }
    }
}
