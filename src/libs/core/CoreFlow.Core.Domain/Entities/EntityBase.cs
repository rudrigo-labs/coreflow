using CoreFlow.Core.Domain.Interfaces;

namespace CoreFlow.Core.Domain.Entities
{
    public abstract class EntityBase : IEntity<Guid>
    {
        public Guid Id { get; }
        public bool IsActive { get; set; }
    }
}
