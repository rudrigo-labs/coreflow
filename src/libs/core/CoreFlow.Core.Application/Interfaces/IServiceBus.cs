using CoreFlow.Core.Application.Interfaces;

namespace CoreFlow.Core.Application.Interfaces
{
    public interface IServiceBus
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) where TResponse : class;
    }
}
