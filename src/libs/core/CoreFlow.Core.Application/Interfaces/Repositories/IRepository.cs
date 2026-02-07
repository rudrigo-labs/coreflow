using CoreFlow.Core.Application.Interfaces.Repositories;
namespace CoreFlow.Core.Application.Interfaces.Repositories
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
